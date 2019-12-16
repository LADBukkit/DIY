using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DIY.Project
{
    /// <summary>
    /// The heart of the Project
    /// This holds all the data for the drawing project
    /// </summary>
    public class DIYProject
    {
        /// <summary>
        /// The list of the layers
        /// </summary>
        public List<Layer> Layers { get; set; } = new List<Layer>();

        /// <summary>
        /// The Index of the currently selected Layer
        /// </summary>
        public int SelectedLayer { get; set; }

        /// <summary>
        /// The Width of the Project
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The Height of the Project
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The Save-Path of the Project
        /// </summary>
        public string PATH = null;

        /// <summary>
        /// The Pixels to Update
        /// </summary>
        public ConcurrentHashSet<int> PixelCache { get; set; }

        /// <summary>
        /// The Actions to undo
        /// </summary>
        public FixedStack<DIYAction> UndoCache = new FixedStack<DIYAction>(20);

        /// <summary>
        /// The Actions to redo
        /// </summary>
        public FixedStack<DIYAction> RedoCache = new FixedStack<DIYAction>(20);

        /// <summary>
        /// Empty Project used for opening
        /// </summary>
        public DIYProject(){}

        /// <summary>
        /// A Project with 1 transparent Layer
        /// </summary>
        /// <param name="width">The Width of the Project</param>
        /// <param name="height">The Height of the Project</param>
        public DIYProject(int width, int height)
        {
            Width = width;
            Height = height;

            Layer lay = new ImageLayer(Width, Height);
            Layers.Add(lay);
            SelectedLayer = 0;

            // Update all Pixels
            PixelCache = new ConcurrentHashSet<int>();
            for(int i = 0; i < width * height; i++)
            {
                PixelCache.Add(i);
            }
        }

        /// <summary>
        /// Disposes the PixelCache
        /// </summary>
        ~DIYProject()
        {
            PixelCache.Dispose();
        }

        /// <summary>
        /// Calculates 1 Pixel and calls the action
        /// </summary>
        /// <param name="x">The X Coordinate</param>
        /// <param name="y">The Y Coordinate</param>
        /// <param name="action">The Action (Normally the drawing)</param>
        /// <param name="underlying">If there should be the underlying pattern</param>
        public void DrawPixel(int x, int y, Action<int, int, DIYColor> action, bool underlying = true)
        {
            DIYColor under = ((x % 2) == (y % 2)) ? new DIYColor(255, 64, 64, 64) : new DIYColor(255, 16, 16, 16);

            DIYColor pxl = new DIYColor(0, 0, 0, 0);
            if(Layers.Count > 0)
            {
                Layer lay = Layers[0];
                if(x >= lay.OffsetX && x < lay.GetBitmap().Width + lay.OffsetX && y >= lay.OffsetY && y < lay.GetBitmap().Height + lay.OffsetY)
                {
                    DIYColor co = lay.GetBitmap().GetPixel(x - lay.OffsetX, y - lay.OffsetY);
                    if(co.A != 0)
                    {
                        pxl = BlendMode.NORMAL.BlendColors(pxl, co, lay.Opacity);
                    }
                }
            }
            for(int i = 1; i < Layers.Count; i++)
            {
                Layer lay = Layers[i];
                if (x >= lay.OffsetX && x < lay.GetBitmap().Width + lay.OffsetX && y >= lay.OffsetY && y < lay.GetBitmap().Height + lay.OffsetY)
                {
                    DIYColor co = lay.GetBitmap().GetPixel(x - lay.OffsetX, y - lay.OffsetY);
                    if (co.A != 0)
                    {
                        pxl = lay.Mode.BlendColors(pxl, co, lay.Opacity);
                    }
                }
            }
            if(underlying)
            {
                action(x, y, BlendMode.NORMAL.BlendColors(under, pxl, 1));
            }
            else
            {
                action(x, y, pxl);
            }
        }

        /// <summary>
        /// Calculates all pixels that have been changed
        /// </summary>
        /// <param name="action">The Action for drawing</param>
        public void CalcBitmap(Action<int, int, DIYColor> action) {
            HashSet<int> hsI = new HashSet<int>();
            PixelCache.ForEach(pos => {
                if (pos < 0 || hsI.Contains(pos)) return;
                int x = pos % Width;
                int y = pos / Width;
                DrawPixel(x, y, action);
                hsI.Add(pos);
            });
            foreach(int pos in hsI)
            {
                PixelCache.Remove(pos);
            }
        }

        /// <summary>
        /// Pushes 1 action onto the undo stack, clears he redo stack and changes the edit menu items
        /// </summary>
        /// <param name="mw">The MainWindow</param>
        /// <param name="action">The Action to push</param>
        public void PushUndo(MainWindow mw, DIYAction action)
        {
            UndoCache.Push(action);
            RedoCache.Clear();

            mw.Dispatcher.Invoke(() =>
            {
                mw.UndoMenu.Header = "_Undo" + (UndoCache.Count > 0 ? "   -   " + UndoCache.Peek().Name : "");
                mw.RedoMenu.Header = "_Redo" + (RedoCache.Count > 0 ? "   -   " + RedoCache.Peek().Name : "");
            });
        }

        /// <summary>
        /// Undos one Action and changes the edit menu items
        /// </summary>
        /// <param name="mw">The MainWindow</param>
        public void Undo(MainWindow mw)
        {
            if (UndoCache.Count < 1) return;

            DIYAction a = UndoCache.Pop();
            if (a == null) return;
            a.Undo(this);
            RedoCache.Push(a);

            mw.Dispatcher.Invoke(() =>
            {
                mw.UndoMenu.Header = "_Undo" + (UndoCache.Count > 0 ? "   -   " + UndoCache.Peek().Name : "");
                mw.RedoMenu.Header = "_Redo" + (RedoCache.Count > 0 ? "   -   " + RedoCache.Peek().Name : "");
            });
        }

        /// <summary>
        /// Redos one Action and changes the edit menu items
        /// </summary>
        /// <param name="mw">The MainWindow</param>
        public void Redo(MainWindow mw)
        {
            if (RedoCache.Count < 1) return;

            DIYAction a = RedoCache.Pop();
            a.Redo(this);
            UndoCache.Push(a);

            mw.Dispatcher.Invoke(() =>
            {
                mw.UndoMenu.Header = "_Undo" + (UndoCache.Count > 0 ? "   -   " + UndoCache.Peek().Name : "");
                mw.RedoMenu.Header = "_Redo" + (RedoCache.Count > 0 ? "   -   " + RedoCache.Peek().Name : "");
            });
        }


        /// <summary>
        /// Magic Int is 0x0D201912 but is swapped due to endiness
        /// </summary>
        private static readonly uint MAGIC_INT = 0x1219200D;

        /// <summary>
        /// Saves this Project as .diy file
        /// </summary>
        /// <param name="path">The path of the file</param>
        public void Save(string path)
        {
            using(BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                // Write the Magic int for identifying
                writer.Write(MAGIC_INT);

                // Write the Project Properties
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(SelectedLayer);

                // Write the Layers
                writer.Write(Layers.Count);
                for(int i = 0; i < Layers.Count; i++)
                {
                    Layer lay = Layers[i];
                    if(lay is ImageLayer)
                    {
                        // Write the bitmap if this is an ImageLayer
                        writer.Write((byte) 1);
                        ImageLayer ilay = (ImageLayer)lay;
                        writer.Write(ilay.Img.Width);
                        writer.Write(ilay.Img.Height);
                        for(int j = 0; j < ilay.Img.Bits.Length; j++)
                        {
                            writer.Write(ilay.Img.Bits[j]);
                        }
                    }
                    else
                    {
                        writer.Write(0);
                    }

                    // Write the Layer Properties
                    writer.Write(lay.Name);
                    writer.Write(lay.Mode.Name);
                    writer.Write(lay.OffsetX);
                    writer.Write(lay.OffsetY);
                    writer.Write(lay.Opacity);
                }
            }
        }

        /// <summary>
        /// Opens a .diy file
        /// 
        /// It just reverses the save function.
        /// </summary>
        /// <param name="path">the path of the file</param>
        public void Open(string path)
        {
            using(BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                if (reader.ReadUInt32() != MAGIC_INT) throw new ArgumentException("Wrong Magic Byte!");

                Width = reader.ReadInt32();
                Height = reader.ReadInt32();
                SelectedLayer = reader.ReadInt32();

                int layCount = reader.ReadInt32();
                for(int i = 0; i < layCount; i++)
                {
                    byte type = reader.ReadByte();
                    Layer lay;
                    if(type == 1)
                    {
                        int w = reader.ReadInt32();
                        int h = reader.ReadInt32();
                        lay = new ImageLayer(w, h);
                        ImageLayer ilay = (ImageLayer) lay;
                        for(int j = 0; j < w * h; j++)
                        {
                            ilay.Img.Bits[j] = reader.ReadInt32();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Wrong Layer Type");
                    }

                    lay.Name = reader.ReadString();
                    lay.Mode = BlendMode.GetByName(reader.ReadString());
                    lay.OffsetX = reader.ReadInt32();
                    lay.OffsetY = reader.ReadInt32();
                    lay.Opacity = reader.ReadDouble();

                    Layers.Add(lay);
                }
            }

            PixelCache = new ConcurrentHashSet<int>();
            for (int j = 0; j < Width * Height; j++)
            {
                PixelCache.Add(j);
            }
        }
    }
}

using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DIY.Project
{
    public class DIYProject
    {
        public List<Layer> Layers { get; set; } = new List<Layer>();
        public int SelectedLayer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public DirectBitmap Render { get; private set; }
        public ConcurrentDictionary<int, bool> PixelCache { get; set; }

        public FixedStack<DIYAction> UndoCache = new FixedStack<DIYAction>(50);
        public FixedStack<DIYAction> RedoCache = new FixedStack<DIYAction>(50);

        public DIYProject(int width, int height)
        {
            Width = width;
            Height = height;

            Layer lay = new ImageLayer(Width, Height);
            Layers.Add(lay);
            SelectedLayer = 0;

            Render = new DirectBitmap(width, height);
            PixelCache = new ConcurrentDictionary<int, bool>();
            for(int i = 0; i < width * height; i++)
            {
                PixelCache[i] = false;
            }
        }

        private void DrawPixel(int x, int y)
        {
            DIYColor pxl = ((x % 2) == (y % 2)) ? new DIYColor(255, 64, 64, 64) : new DIYColor(255, 16, 16, 16);

            if(Layers.Count > 0)
            {
                Layer lay = Layers[0];
                if(x >= lay.OffsetX && y >= lay.OffsetY)
                {
                    pxl = BlendMode.NORMAL.BlendColors(pxl, lay.GetBitmap().GetPixel(x - lay.OffsetX, y - lay.OffsetY), lay.Opacity);
                }
            }
            for(int i = 1; i < Layers.Count; i++)
            {
                Layer lay = Layers[i];
                if (x >= lay.OffsetX && y >= lay.OffsetY)
                {
                    pxl = lay.Mode.BlendColors(pxl, lay.GetBitmap().GetPixel(x - lay.OffsetX, y - lay.OffsetY), lay.Opacity);
                }
            }
            Render.SetPixel(x, y, pxl);
        }

        public void CalcBitmap() {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int pos = x + (y * Width);
                    if (!PixelCache[pos])
                    {
                        DrawPixel(x, y);
                        PixelCache[pos] = true;
                    }
                }
            }
        }

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
    }
}

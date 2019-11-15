using DIY.Util;
using System;
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
        public bool[] PixelCache { get; set; }

        public DIYProject(int width, int height)
        {
            Width = width;
            Height = height;

            Layer lay = new ImageLayer(Width, Height);
            Layers.Add(lay);
            SelectedLayer = 0;

            Render = new DirectBitmap(width, height);
            PixelCache = new bool[width * height];
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

        /*private void DrawToBitmap(DirectBitmap bottom, DirectBitmap top, int offX, int offY, BlendMode mode, double opacity)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            for (int x = 0; x < Width; x++)
            {
                if (x < offX) continue;
                for (int y = 0; y < Height; y++)
                {
                    if (y < offY) continue;

                    DIYColor c1 = bottom.GetPixel(x, y);
                    DIYColor c2 = top.GetPixel(x - offX, y - offY);
                    DIYColor c3 = BlendMode.NORMAL.BlendColors(c1, c2, opacity);
                    bottom.SetPixel(x, y, c3);
                }
            }
            //sw.Stop();
            //MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }*/

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
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DIY.Util
{
    /// <summary>
    /// Code from:
    /// https://stackoverflow.com/a/34801225
    /// </summary>
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public int[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    SetPixel(x, y, new DIYColor());
                }
            }
        }

        public void SetPixel(int x, int y, DIYColor c)
        {
            // better handling of different color types
            int index = x + (y * Width);
            int col = c.Argb;

            Bits[index] = col;
        }

        public DIYColor GetPixel(int x, int y)
        {
            // better handling of different color types
            int index = x + (y * Width);
            int col = Bits[index];
            DIYColor c = new DIYColor(col);
            return c;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}

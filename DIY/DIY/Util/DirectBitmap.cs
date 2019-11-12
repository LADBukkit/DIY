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
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            // better handling of different color types
            int index = x + (y * Width);
            int col = Color.FromArgb(r, g, b).ToArgb();

            Bits[index] = col;
        }

        public System.Windows.Media.Color GetPixel(int x, int y)
        {
            // better handling of different color types
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);
            System.Windows.Media.Color r = System.Windows.Media.Color.FromArgb(result.A, result.R, result.G, result.B);
            return r;
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

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
        }

        public DirectBitmap Clone()
        {
            DirectBitmap db = new DirectBitmap(Width, Height);
            db.Disposed = Disposed;
            if(!Disposed)
            {
                for (int i = 0; i < Bits.Length; i++)
                {
                    db.Bits[i] = Bits[i];
                }
            }
            return db;
        }

        private int ToIndex(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return -1;

            return x + (y * Width);
        }

        public void SetPixel(int x, int y, DIYColor c, bool overlay = true)
        {
            if(x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }
            int index = ToIndex(x, y);
            if (overlay)
            {
                DIYColor old = GetPixel(x, y);
                if(old.A != 0)
                {
                    double a1 = old.A / 255D;
                    double a0 = c.A / 255D;
                    double r1 = old.R / 255D;
                    double r0 = c.R / 255D;
                    double g1 = old.G / 255D;
                    double g0 = c.G / 255D;
                    double b1 = old.B / 255D;
                    double b0 = c.B / 255D;

                    double a01 = a0 + a1 * (1D - a0);
                    if (a01 == 0D)
                    {
                        c = new DIYColor(0);
                    }
                    else
                    {
                        double r01 = (r0 * a0 + r1 * a1 * (1D - a0));
                        double g01 = (g0 * a0 + g1 * a1 * (1D - a0));
                        double b01 = (b0 * a0 + b1 * a1 * (1D - a0));
                        c = new DIYColor((int)(a01 * 255), (int)(r01 * 255), (int)(g01 * 255), (int)(b01 * 255));
                    }
                }
            }

            int col = c.Argb;
            Bits[index] = col;
        }

        public DIYColor GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }
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

        public List<int> DrawLine(int x0, int y0, int x1, int y1, DIYColor c)
        {
            List<int> points = new List<int>();

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;
            int e2 = 0;
            while(true)
            {
                points.Add(ToIndex(x0, y0));
                SetPixel(x0, y0, c);
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if(e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if(e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            return points;
        }

        public List<int> DrawCircle(int x0, int y0, int radius, DIYColor c)
        {
            List<int> points = new List<int>();

            int f = 1 - radius;
            int ddF_x = 0;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            points.Add(ToIndex(x0, y0 + radius));
            SetPixel(x0, y0 + radius, c);

            points.Add(ToIndex(x0, y0 - radius));
            SetPixel(x0, y0 - radius, c);

            points.Add(ToIndex(x0 + radius, y0));
            SetPixel(x0 + radius, y0, c);

            points.Add(ToIndex(x0 - radius, y0));
            SetPixel(x0 - radius, y0, c);

            while(x < y)
            {
                if(f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x + 1;

                SetPixel(x0 + x, y0 + y, c);
                SetPixel(x0 - x, y0 + y, c);
                SetPixel(x0 + x, y0 - y, c);
                SetPixel(x0 - x, y0 - y, c);
                SetPixel(x0 + y, y0 + x, c);
                SetPixel(x0 - y, y0 + x, c);
                SetPixel(x0 + y, y0 - x, c);
                SetPixel(x0 - y, y0 - x, c);

                points.Add(ToIndex(x0 + x, y0 + y));
                points.Add(ToIndex(x0 - x, y0 + y));
                points.Add(ToIndex(x0 + x, y0 - y));
                points.Add(ToIndex(x0 - x, y0 - y));
                points.Add(ToIndex(x0 + y, y0 + x));
                points.Add(ToIndex(x0 - y, y0 + x));
                points.Add(ToIndex(x0 + y, y0 - x));
                points.Add(ToIndex(x0 - y, y0 - x));
            }
            return points;
        }

        private List<int> hLine(int x0, int y0, int w)
        {
            List<int> points = new List<int>();
            for(int i = 0; i < w; i++)
            {
                points.Add(ToIndex(x0 + i, y0));
            }
            return points;
        }

        public List<int> PointsFilledCircle(int x0, int y0, int radius)
        {
            List<int> points = new List<int>();

            int f = 1 - radius;
            int ddF_x = 0;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            points.Add(ToIndex(x0, y0 + radius));

            points.Add(ToIndex(x0, y0 - radius));

            points.Add(ToIndex(x0 + radius, y0));

            points.Add(ToIndex(x0 - radius, y0));

            points.AddRange(hLine(x0 - radius, y0 , radius * 2));

            while (x < y)
            {
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x + 1;

                points.AddRange(hLine(x0 - x, y0 + y, x * 2));
                points.AddRange(hLine(x0 - x, y0 - y, x * 2));

                points.AddRange(hLine(x0 - y, y0 + x, y * 2));
                points.AddRange(hLine(x0 - y, y0 - x, y * 2));


                points.Add(ToIndex(x0 + x, y0 + y));
                points.Add(ToIndex(x0 - x, y0 + y));
                points.Add(ToIndex(x0 + x, y0 - y));
                points.Add(ToIndex(x0 - x, y0 - y));
                points.Add(ToIndex(x0 + y, y0 + x));
                points.Add(ToIndex(x0 - y, y0 + x));
                points.Add(ToIndex(x0 + y, y0 - x));
                points.Add(ToIndex(x0 - y, y0 - x));
            }

            HashSet<int> set = new HashSet<int>();
            
            foreach(int i in points)
            {
                set.Add(i);              
            }
            return new List<int>(set);
        }

        public List<int> DrawFilledCircle(int x0, int y0, int radius, DIYColor c)
        {
            List<int> points = PointsFilledCircle(x0, y0, radius);
            foreach (int i in points)
            {
                int nx = i % Width;
                int ny = i / Width;
                SetPixel(nx, ny, c);
            }
            return points;
        }

        public List<int> RemoveFilledCircle(int x0, int y0, int radius, double percent)
        {
            List<int> points = PointsFilledCircle(x0, y0, radius);
            foreach (int i in points)
            {
                int nx = i % Width;
                int ny = i / Width;

                DIYColor c = GetPixel(nx, ny);
                if (c == null) continue;
                c.A = (int) (c.A * (1 - percent));
                SetPixel(nx, ny, c, false);
            }
            return points;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Point = System.Windows.Point;

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

        public List<Point> DrawLine(int x0, int y0, int x1, int y1, DIYColor c)
        {
            List<Point> points = new List<Point>();

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;
            int e2 = 0;
            while(true)
            {
                points.Add(new Point(x0, y0));
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

        public List<Point> DrawCircle(int x0, int y0, int radius, DIYColor c)
        {
            List<Point> points = new List<Point>();

            int f = 1 - radius;
            int ddF_x = 0;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            points.Add(new Point(x0, y0 + radius));
            SetPixel(x0, y0 + radius, c);

            points.Add(new Point(x0, y0 - radius));
            SetPixel(x0, y0 - radius, c);

            points.Add(new Point(x0 + radius, y0));
            SetPixel(x0 + radius, y0, c);

            points.Add(new Point(x0 - radius, y0));
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

                points.Add(new Point(x0 + x, y0 + y));
                points.Add(new Point(x0 - x, y0 + y));
                points.Add(new Point(x0 + x, y0 - y));
                points.Add(new Point(x0 - x, y0 - y));
                points.Add(new Point(x0 + y, y0 + x));
                points.Add(new Point(x0 - y, y0 + x));
                points.Add(new Point(x0 + y, y0 - x));
                points.Add(new Point(x0 - y, y0 - x));
            }
            return points;
        }

        private List<Point> hLine(int x0, int y0, int w)
        {
            List<Point> points = new List<Point>();
            for(int i = 0; i < w; i++)
            {
                points.Add(new Point(x0 + i, y0));
            }
            return points;
        }

        public List<Point> PointsFilledCircle(int x0, int y0, int radius)
        {
            List<Point> points = new List<Point>();

            int f = 1 - radius;
            int ddF_x = 0;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            points.Add(new Point(x0, y0 + radius));

            points.Add(new Point(x0, y0 - radius));

            points.Add(new Point(x0 + radius, y0));

            points.Add(new Point(x0 - radius, y0));

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


                points.Add(new Point(x0 + x, y0 + y));
                points.Add(new Point(x0 - x, y0 + y));
                points.Add(new Point(x0 + x, y0 - y));
                points.Add(new Point(x0 - x, y0 - y));
                points.Add(new Point(x0 + y, y0 + x));
                points.Add(new Point(x0 - y, y0 + x));
                points.Add(new Point(x0 + y, y0 - x));
                points.Add(new Point(x0 - y, y0 - x));
            }

            HashSet<Point> set = new HashSet<Point>();
            
            foreach(Point i in points)
            {
                set.Add(i);              
            }
            return new List<Point>(set);
        }

        public List<Point> DrawFilledCircle(int x0, int y0, int radius, DIYColor c)
        {
            List<Point> points = PointsFilledCircle(x0, y0, radius);
            foreach (Point i in points)
            {
                SetPixel((int) i.X, (int) i.Y, c);
            }
            return points;
        }

        public List<Point> RemoveFilledCircle(int x0, int y0, int radius, double percent)
        {
            List<Point> points = PointsFilledCircle(x0, y0, radius);
            foreach (Point i in points)
            {
                DIYColor c = GetPixel((int) i.X, (int)i.Y);
                if (c == null) continue;
                c.A = (int) (c.A * (1 - percent));
                SetPixel((int) i.X, (int)i.Y, c, false);
            }
            return points;
        }

        private static double FF_MAX_DIS = 1D / ColorUtil.DistanceSquared(new DIYColor(0, 0, 0, 0), new DIYColor(255, 255, 255, 255)) * 100;

        public List<Point> FloodFill(int x0, int y0, int threshold, DIYColor cOld, DIYColor cNew)
        {
            // Performance Problems on Bigger pictures
            HashSet<Point> points = new HashSet<Point>();
            Queue<Point> queue = new Queue<Point>();

            queue.Enqueue(new Point(x0, y0));
            while(queue.Count > 0)
            {
                Point p = queue.Dequeue();
                if (points.Contains(new Point(p.X, p.Y))) continue;
                if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Height) continue;

                DIYColor c = GetPixel((int) p.X, (int) p.Y);

                double dis = ColorUtil.DistanceSquared(c, cOld) * FF_MAX_DIS;
                if(dis <= threshold)
                {
                    points.Add(new Point(p.X, p.Y));
                    SetPixel((int) p.X, (int) p.Y, cNew, false);

                    queue.Enqueue(new Point(p.X, p.Y + 1));
                    queue.Enqueue(new Point(p.X, p.Y - 1));
                    queue.Enqueue(new Point(p.X + 1, p.Y));
                    queue.Enqueue(new Point(p.X - 1, p.Y));
                }
            }

            return new List<Point>(points);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Point = System.Windows.Point;
using System.Diagnostics;
using System.Windows;

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
                    c = Project.BlendMode.NORMAL.BlendColors(old, c, 1);
                }
            }

            int col = c.Argb;
            Bits[index] = col;
        }

        public DIYColor GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return DIYColor.NULL_TYPE;
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

        private static readonly Dictionary<int, List<Point>> FILLED_CIRCLE_CACHE = new Dictionary<int, List<Point>>();

        public List<Point> DrawFilledCircle(int x0, int y0, int radius, DIYColor c)
        {
            List<Point> ps;
            if(FILLED_CIRCLE_CACHE.ContainsKey(radius))
            {
                ps = FILLED_CIRCLE_CACHE[radius];
            }
            else
            {
                ps = PointsFilledCircle(0, 0, radius);
                FILLED_CIRCLE_CACHE[radius] = ps;
            }

            List<Point> points = new List<Point>();
            foreach(Point p in ps)
            {
                points.Add(new Point(p.X + x0, p.Y + y0));
            }

            foreach (Point i in points)
            {
                SetPixel((int) i.X, (int) i.Y, c);
            }

            return points;
        }

        public List<Point> PointsFilledSquare(int x0, int y0, int radius)
        {
            List<Point> points = new List<Point>();
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    points.Add(new Point(x0 + x, y0 + y));
                }
            }
            return points;
        }

        public List<Point> DrawFilledSquare(int x0, int y0, int radius, DIYColor c)
        {
            List<Point> points = PointsFilledSquare(x0, y0, radius);
            foreach (Point i in points)
            {
                SetPixel((int)i.X, (int)i.Y, c);
            }
            return points;
        }

        public List<Point> RemoveFilledSquare(int x0, int y0, int radius, double percent)
        {
            List<Point> points = PointsFilledSquare(x0, y0, radius);
            foreach (Point i in points)
            {
                DIYColor c = GetPixel((int)i.X, (int)i.Y);
                c.A = (byte)(c.A * (1 - percent));
                SetPixel((int)i.X, (int)i.Y, c, false);
            }
            return points;
        }

        public List<Point> RemoveFilledCircle(int x0, int y0, int radius, double percent)
        {
            List<Point> ps;
            if (FILLED_CIRCLE_CACHE.ContainsKey(radius))
            {
                ps = FILLED_CIRCLE_CACHE[radius];
            }
            else
            {
                ps = PointsFilledCircle(0, 0, radius);
                FILLED_CIRCLE_CACHE[radius] = ps;
            }

            List<Point> points = new List<Point>();
            foreach (Point p in ps)
            {
                points.Add(new Point(p.X + x0, p.Y + y0));
            }

            foreach (Point i in points)
            {
                DIYColor c = GetPixel((int) i.X, (int)i.Y);
                c.A = (byte) (c.A * (1 - percent));
                SetPixel((int) i.X, (int)i.Y, c, false);
            }
            return points;
        }

        private static double FF_MAX_DIS = 1D / ColorUtil.DistanceSquared(new DIYColor(0, 0, 0, 0), new DIYColor(255, 255, 255, 255)) * 100;

        public List<Point> FloodFill(int x0, int y0, int threshold, DIYColor cOld, DIYColor cNew)
        {
            bool[] ps = new bool[Width * Height];
            bool IsEqual(Point p, DIYColor c2)
            {
                double dis = ColorUtil.DistanceSquared(GetPixel((int) p.X,(int) p.Y), c2) * FF_MAX_DIS;
                return dis <= threshold && !(ps[((int)p.X) + ((int)p.Y) * Width]);
            }

            List<Point> points = new List<Point>();
            int x1;
            bool spanAbove, spanBelow;
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(x0, y0));
            while(stack.TryPop(out Point p))
            {
                x0 = (int) p.X;
                y0 = (int) p.Y;

                x1 = x0;
                while (x1 >= 0 && IsEqual(new Point(x1, y0), cOld)) x1--;
                x1++;
                spanAbove = spanBelow = false;
                while(x1 < Width && IsEqual(new Point(x1, y0), cOld))
                {
                    points.Add(new Point(x1, y0));
                    ps[x1 + y0 * Width] = true;
                    SetPixel(x1, y0, cNew, false);

                    if (!spanAbove && y0 > 0 && IsEqual(new Point(x1, y0 - 1), cOld))
                    {
                        stack.Push(new Point(x1, y0 - 1));
                        spanAbove = true;
                    }
                    else if (spanAbove && y0 > 0 && !IsEqual(new Point(x1, y0 - 1), cOld))
                    {
                        spanAbove = false;
                    }

                    if (!spanBelow && y0 < Height - 1 && IsEqual(new Point(x1, y0 + 1), cOld))
                    {
                        stack.Push(new Point(x1, y0 + 1));
                        spanBelow = true;
                    }
                    else if (spanBelow && y0 < Height - 1 && !IsEqual(new Point(x1, y0 + 1), cOld))
                    {
                        spanBelow = false;
                    }
                    x1++;
                }
            }

            return points;
        }
    }
}

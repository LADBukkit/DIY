using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DIY.Util
{
    /// <summary>
    /// A Custom Color Class as the C# native classes are highly in efficient
    /// </summary>
    public class DIYColor
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct color
        {
            [FieldOffset(3)]
            public byte A;
            [FieldOffset(2)]
            public byte R;
            [FieldOffset(1)]
            public byte G;
            [FieldOffset(0)]
            public byte B;

            [FieldOffset(0)]
            public int Argb;
        }

        private color Color = new color();

        public int A { get => Color.A; set => Color.A = (byte) value; }
        public int R { get => Color.R; set => Color.R = (byte) value; }
        public int G { get => Color.G; set => Color.G = (byte) value; }
        public int B { get => Color.B; set => Color.B = (byte) value; }
        public int Argb { get => Color.Argb; set => Color.Argb = value; }

        public DIYColor(int a = 0, int r = 0, int g = 0, int b = 0)
        {
            A = a;
            R = r;
            G = g;
            B = b;
            //Clamp();
        }

        public DIYColor(int argb)
        {
            Argb = argb;
        }

        public  void Clamp()
        {
            A = A > 255 ? 255 : A < 0 ? 0 : A;
            R = R > 255 ? 255 : R < 0 ? 0 : R;
            G = G > 255 ? 255 : G < 0 ? 0 : G;
            B = B > 255 ? 255 : B < 0 ? 0 : B;
        }

        public static DIYColor operator +(DIYColor c1, DIYColor c2)
        {
            return new DIYColor(c1.A + c2.A, c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
        }

        public static DIYColor operator +(DIYColor c1, int i)
        {
            return c1 + new DIYColor(i, i, i, i);
        }

        public static DIYColor operator +(DIYColor c1, double i)
        {
            return c1 + (int) i;
        }

        public static DIYColor operator -(DIYColor c1, DIYColor c2)
        {
            return new DIYColor(c1.A - c2.A, c1.R - c2.R, c1.G - c2.G, c1.B - c2.B);
        }

        public static DIYColor operator -(DIYColor c1, int i)
        {
            return c1 - new DIYColor(i, i, i, i);
        }
        public static DIYColor operator -(DIYColor c1, double i)
        {
            return c1 - (int)i;
        }

        public static DIYColor operator *(DIYColor c1, DIYColor c2)
        {
            return new DIYColor(c1.A * c2.A, c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
        }

        public static DIYColor operator *(DIYColor c1, int i)
        {
            return c1 * new DIYColor(i, i, i, i);
        }

        public static DIYColor operator *(DIYColor c1, double i)
        {
            return new DIYColor((int) (c1.A * i), (int) (c1.R * i), (int) (c1.G * i), (int) (c1.B * i));
        }

        public static DIYColor operator /(DIYColor c1, DIYColor c2)
        {
            return new DIYColor(c1.A / c2.A, c1.R / c2.R, c1.G / c2.G, c1.B / c2.B);
        }

        public static DIYColor operator /(DIYColor c1, int i)
        {
            return c1 / new DIYColor(i, i, i, i);
        }

        public static DIYColor operator /(DIYColor c1, double i)
        {
            return new DIYColor((int) (c1.A / i), (int) (c1.R / i), (int) (c1.G * i), (int) (c1.B / i));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return true;
            if(obj is DIYColor)
            {
                return Argb == ((DIYColor)obj).Argb;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Argb;
        }
    }
}

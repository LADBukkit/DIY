using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DIY.Util
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DIYColor
    {
        public static DIYColor NULL_TYPE = new DIYColor { NULL = true };


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

        [FieldOffset(4)]
        private bool NULL;

        public DIYColor(byte a = 0, byte r = 0, byte g = 0, byte b = 0)
        {
            Argb = 0;

            A = a;
            R = r;
            G = g;
            B = b;

            NULL = false;
        }

        public static bool operator ==(DIYColor c1, DIYColor c2)
        {
            return c1.Argb == c2.Argb && c1.NULL == c2.NULL;
        }

        public static bool operator !=(DIYColor c1, DIYColor c2)
        {
            return !(c1 == c2);
        }

        public DIYColor(int argb) : this()
        {
            Argb = argb;
        }
    }
}

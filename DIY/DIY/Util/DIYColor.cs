using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DIY.Util
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DIYColor
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

        public DIYColor(byte a = 0, byte r = 0, byte g = 0, byte b = 0)
        {
            Argb = 0;

            A = a;
            R = r;
            G = g;
            B = b;
        }

        public DIYColor(int argb) : this()
        {
            Argb = argb;
        }
    }
}

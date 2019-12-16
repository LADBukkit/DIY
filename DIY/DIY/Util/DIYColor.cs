using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DIY.Util
{
    /// <summary>
    /// A Struct for the colors as the standard C# classes and structs are highly inperformant
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DIYColor
    {
        /// <summary>
        /// The Null Constant
        /// </summary>
        public static readonly DIYColor NULL_TYPE = new DIYColor { NULL = true };

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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is DIYColor)) return false;

            DIYColor c2 = (DIYColor)obj;
            return Argb == c2.Argb && NULL == c2.NULL;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(DIYColor c1, DIYColor c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(DIYColor c1, DIYColor c2)
        {
            return !c1.Equals(c2);
        }

        public DIYColor(int argb) : this()
        {
            Argb = argb;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using DIY.Util;

namespace DIY.Filter
{
    /// <summary>
    /// Inverts the Image
    /// </summary>
    public class Invert : Filter
    {
        public override string Name => "Invert";

        public Invert()
        {
            // No Properties needed
            Properties = new FilterProperty[0];
        }

        public override DirectBitmap CalculateFilter(DirectBitmap input)
        {
            DirectBitmap db = input.Clone();

            for(int i = 0; i < db.Bits.Length; i++)
            {
                // Invert the rgb channels
                DIYColor dc = new DIYColor(db.Bits[i]);
                dc.R = (byte)(255 - dc.R);
                dc.G = (byte)(255 - dc.G);
                dc.B = (byte)(255 - dc.B);
                db.Bits[i] = dc.Argb;
            }
            return db;
        }
    }
}

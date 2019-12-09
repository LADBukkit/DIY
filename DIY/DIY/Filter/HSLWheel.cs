using System;
using System.Collections.Generic;
using System.Text;
using DIY.Util;

namespace DIY.Filter
{
    public class HSLWheel : Filter
    {
        public override string Name => "HSL Wheel";

        public HSLWheel()
        {
            Properties = new FilterProperty[] {
                new FilterPropertyNumeric<int>("Hue", -180, 180, 0, 1),
                new FilterPropertyNumeric<int>("Saturation", -100, 100, 0, 1),
                new FilterPropertyNumeric<int>("Lightness", -100, 100, 0, 1)
            };
        }

        public override DirectBitmap CalculateFilter(DirectBitmap input)
        {
            DirectBitmap db = input.Clone();
            for(int i = 0; i < db.Bits.Length; i++)
            {
                DIYColor dc = new DIYColor(db.Bits[i]);
                ColorUtil.ToHSL(dc.R, dc.G, dc.B, out double h, out double s, out double l);

                h += ((FilterPropertyNumeric<int>)Properties[0]).Value;
                s += ((FilterPropertyNumeric<int>)Properties[1]).Value / 100D;
                l += ((FilterPropertyNumeric<int>)Properties[2]).Value / 100D;
                ColorUtil.ToRGB(h, s, l, out byte r, out byte g, out byte b);

                dc.R = r;
                dc.G = g;
                dc.B = b;
                db.Bits[i] = dc.Argb;
            }

            return db;
        }
    }
}

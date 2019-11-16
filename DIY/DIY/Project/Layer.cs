using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DIY.Project
{
    public abstract class Layer
    {
        public string Name { get; set; }
        public double Opacity { get; set; } = 1.0;
        public BlendMode Mode { get; set; } = BlendMode.NORMAL;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public Layer(string name = "Unnamed")
        {
            Name = name;
        }

        public abstract DirectBitmap GetBitmap();
    }
}

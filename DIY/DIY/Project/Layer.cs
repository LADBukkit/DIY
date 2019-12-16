using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DIY.Project
{
    /// <summary>
    /// A Blueprint for the Layers of the Project
    /// </summary>
    public abstract class Layer
    {
        /// <summary>
        /// The Name of the Layer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Opacity of the Layer (1.0 = 100%)
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// The BlendMode of the Layer
        /// </summary>
        public BlendMode Mode { get; set; } = BlendMode.NORMAL;

        /// <summary>
        /// The Offset for the X Coordinate
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// The Offset for the Y Coordinate
        /// </summary>
        public int OffsetY { get; set; }

        public Layer(string name = "Unnamed")
        {
            Name = name;
        }

        /// <summary>
        /// Calculates the Rendered Bitmap of this Layer
        /// </summary>
        /// <returns>The Rendered Bitmap of this Layer</returns>
        public abstract DirectBitmap GetBitmap();
    }
}

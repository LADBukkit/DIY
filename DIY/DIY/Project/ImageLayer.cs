using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DIY.Project
{
    public class ImageLayer : Layer
    {
        public DirectBitmap Img { get; set; }
        
        public ImageLayer(int width, int height)
        {
            Img = new DirectBitmap(width, height);
        }

        public override DirectBitmap GetBitmap()
        {
            return Img;
        }
    }
}

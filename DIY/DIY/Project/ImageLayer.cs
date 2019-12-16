using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DIY.Project
{
    /// <summary>
    /// A Layer for normal drawing
    /// </summary>
    public class ImageLayer : Layer
    {
        /// <summary>
        /// The Underlying Image
        /// </summary>
        public DirectBitmap Img { get; set; }
        
        public ImageLayer(int width, int height)
        {
            Img = new DirectBitmap(width, height);
        }

        ~ImageLayer()
        {
            if(Img != null)
            {
                Img.Dispose();
            }
        }

        public override DirectBitmap GetBitmap()
        {
            return Img;
        }
    }
}

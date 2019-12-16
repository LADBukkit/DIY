using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// An Action for different partial Image-Resets
    /// </summary>
    public class ImageAction : DIYAction
    {
        /// <summary>
        /// The Old Image
        /// </summary>
        public DirectBitmap Old { get; set; }

        /// <summary>
        /// The New Image
        /// </summary>
        public DirectBitmap New { get; set; }

        /// <summary>
        /// A Set of changed Pixels
        /// </summary>
        public HashSet<int> ChangedPixels { get; private set; } = new HashSet<int>();

        /// <summary>
        /// The Layer the changes are meant for
        /// </summary>
        public ImageLayer Layer { get; set; }


        public ImageAction(string name) : base(name) {}

        public override void Redo(DIYProject pr)
        {
            Layer.Img = New;
            foreach(int i in ChangedPixels)
            {
                pr.PixelCache.Add(i);
            }
        }

        public override void Undo(DIYProject pr)
        {
            Layer.Img = Old;
            foreach (int i in ChangedPixels)
            {
                pr.PixelCache.Add(i);
            }
        }
    }
}

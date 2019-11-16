using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class ImageAction : DIYAction
    {
        public DirectBitmap Old { get; set; }
        public DirectBitmap New { get; set; }
        public HashSet<int> ChangedPixels { get; private set; } = new HashSet<int>();
        public int Layer { get; set; }

        public ImageAction(string name) : base(name) {}


        public override void Redo(DIYProject pr)
        {
            ImageLayer lay = (ImageLayer) pr.Layers[Layer];
            lay.Img = New;
            foreach(int i in ChangedPixels)
            {
                pr.PixelCache[i] = false;
            }
        }

        public override void Undo(DIYProject pr)
        {
            ImageLayer lay = (ImageLayer)pr.Layers[Layer];
            lay.Img = Old;
            foreach (int i in ChangedPixels)
            {
                pr.PixelCache[i] = false;
            }
        }
    }
}

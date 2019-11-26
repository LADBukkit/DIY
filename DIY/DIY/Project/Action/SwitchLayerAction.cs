using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class SwitchLayerAction : DIYAction
    {
        public int POld { get; set; }
        public int PNew { get; set; }

        public SwitchLayerAction() : base("Move Layer") { }

        public override void Redo(DIYProject pr)
        {
            Layer help = pr.Layers[POld];
            pr.Layers[POld] = pr.Layers[PNew];
            pr.Layers[PNew] = help;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }

        public override void Undo(DIYProject pr)
        {
            Redo(pr);
        }
    }
}

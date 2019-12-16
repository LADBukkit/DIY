using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// Action for switching the positions of layers
    /// </summary>
    public class SwitchLayerAction : DIYAction
    {
        /// <summary>
        /// The Old Position
        /// </summary>
        public int POld { get; set; }

        /// <summary>
        /// The New Position
        /// </summary>
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

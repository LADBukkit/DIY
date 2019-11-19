using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class LayerBlendModeAction : DIYAction
    {
        public Layer Layer { get; set; }
        public BlendMode Old { get; set; }
        public BlendMode New { get; set; }

        public LayerBlendModeAction() : base("Change Layer BlendMode") { }

        public override void Redo(DIYProject pr)
        {
            Layer.Mode = New;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache[i] = false;
            }
        }

        public override void Undo(DIYProject pr)
        {
            Layer.Mode = Old;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache[i] = false;
            }
        }
    }
}

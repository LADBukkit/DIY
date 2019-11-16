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
        }

        public override void Undo(DIYProject pr)
        {
            Layer.Mode = Old;
        }
    }
}

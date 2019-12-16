using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// The Action for changing the BlendMode of a Layer
    /// </summary>
    public class LayerBlendModeAction : DIYAction
    {
        /// <summary>
        /// The Layer the changes are from
        /// </summary>
        public Layer Layer { get; set; }

        /// <summary>
        /// The Old BlendMode
        /// </summary>
        public BlendMode Old { get; set; }

        /// <summary>
        /// The New BlendMode
        /// </summary>
        public BlendMode New { get; set; }

        public LayerBlendModeAction() : base("Change Layer BlendMode") { }

        public override void Redo(DIYProject pr)
        {
            Layer.Mode = New;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }

        public override void Undo(DIYProject pr)
        {
            Layer.Mode = Old;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }
    }
}

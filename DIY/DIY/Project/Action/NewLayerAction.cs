using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// The Action for creating a new Layer
    /// </summary>
    public class NewLayerAction : DIYAction
    {
        /// <summary>
        /// The Layer to add
        /// </summary>
        public Layer Layer { get; set; }

        public NewLayerAction() : base("New Layer"){}

        public override void Redo(DIYProject pr)
        {
            pr.Layers.Add(Layer);
        }

        public override void Undo(DIYProject pr)
        {
            pr.Layers.Remove(Layer);
        }
    }
}

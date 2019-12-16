using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// The Action for changing the selected Layer
    /// </summary>
    public class LayerSelectionAction : DIYAction
    {
        /// <summary>
        /// The Old Selection
        /// </summary>
        public int Old { get; set; }

        /// <summary>
        /// The New Selection
        /// </summary>
        public int New { get; set; }

        public LayerSelectionAction() : base("Layer Selection") { }

        public override void Redo(DIYProject pr)
        {
            pr.SelectedLayer = New;
        }

        public override void Undo(DIYProject pr)
        {
            pr.SelectedLayer = Old;
        }
    }
}

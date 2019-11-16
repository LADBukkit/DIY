using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class LayerSelectionAction : DIYAction
    {
        public int Old { get; set; }
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

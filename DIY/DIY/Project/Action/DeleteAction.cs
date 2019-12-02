using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    public class DeleteAction : DIYAction
    {
        public int Position { get; set; }
        public Layer Layer { get; set; }

        public DeleteAction() : base("Delete Layer") { }

        public override void Redo(DIYProject pr)
        {
            pr.Layers.RemoveAt(Position);

            if (Position > 0)
            {
                pr.SelectedLayer = Position - 1;
            }
        }

        public override void Undo(DIYProject pr)
        {
            pr.Layers.Insert(Position, Layer);
            pr.SelectedLayer = Position;
        }
    }
}

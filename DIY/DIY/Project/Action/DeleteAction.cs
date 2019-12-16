using System;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project.Action
{
    /// <summary>
    /// Action for deleting a Layer
    /// </summary>
    public class DeleteAction : DIYAction
    {
        /// <summary>
        /// The Position of the deleted Layer
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The Deleted Layer
        /// </summary>
        public Layer Layer { get; set; }

        public DeleteAction() : base("Delete Layer") { }

        public override void Redo(DIYProject pr)
        {
            pr.Layers.RemoveAt(Position);

            if (Position > 0)
            {
                pr.SelectedLayer = Position - 1;
            }

            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }

        public override void Undo(DIYProject pr)
        {
            pr.Layers.Insert(Position, Layer);
            pr.SelectedLayer = Position;

            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }
    }
}

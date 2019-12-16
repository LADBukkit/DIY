using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DIY.Project.Action
{
    /// <summary>
    /// The Action for moving the layer
    /// </summary>
    public class MoveAction : DIYAction
    {
        /// <summary>
        /// The Layer the changes are from
        /// </summary>
        public Layer Layer { get; set; }

        /// <summary>
        /// The Old Offset
        /// </summary>
        public Point OldOff { get; set; }

        /// <summary>
        /// The New Offset
        /// </summary>
        public Point NewOff { get; set; }

        public MoveAction() : base("Move")
        {
        }

        public override void Redo(DIYProject pr)
        {
            Layer.OffsetX = (int)NewOff.X;
            Layer.OffsetY = (int)NewOff.Y;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }

        public override void Undo(DIYProject pr)
        {
            Layer.OffsetX = (int)OldOff.X;
            Layer.OffsetY = (int)OldOff.Y;
            for (int i = 0; i < pr.Width * pr.Height; i++)
            {
                pr.PixelCache.Add(i);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DIY.Project.Action
{
    public class MoveAction : DIYAction
    {
        public Layer Layer { get; set; }
        public Point OldOff { get; set; }
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

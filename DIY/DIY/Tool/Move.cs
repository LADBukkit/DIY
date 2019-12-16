using DIY.Project;
using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY.Tool
{
    /// <summary>
    /// The Move Tool
    /// Used for moving a layer
    /// </summary>
    class Move : Tool
    {
        /// <summary>
        /// The Layer to Move
        /// </summary>
        public Layer Layer { get; set; }

        /// <summary>
        /// The Old Position of the Layer
        /// </summary>
        public Point OldPosition;

        /// <summary>
        /// The Old Offset X
        /// </summary>
        public int OldOffX { get; set; }

        /// <summary>
        /// The Old Offset Y
        /// </summary>
        public int OldOffY { get; set; }

        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;

            Layer = project.Layers[project.SelectedLayer];
            OldPosition = p;

            OldOffX = Layer.OffsetX;
            OldOffY = Layer.OffsetY;
        }

        public override void MouseMove(MainWindow mw, Point p)
        {
            int oOffX = Layer.OffsetX;
            int oOffY = Layer.OffsetY;
            Layer.OffsetX = (int) (OldOffX + (p.X - OldPosition.X));
            Layer.OffsetY = (int) (OldOffY + (p.Y - OldPosition.Y));

            if(oOffX != Layer.OffsetX || oOffY != Layer.OffsetY)
            {
                // Worst performance for bigger images
                for (int i = 0; i < mw.Project.Width * mw.Project.Height; i++)
                {
                    mw.Project.PixelCache.Add(i);
                }
            }
        }

        public override void MouseUp(MainWindow mw, Point p)
        {
            MoveAction ma = new MoveAction();
            ma.Layer = Layer;
            ma.OldOff = new Point(OldOffX, OldOffY);
            ma.NewOff = new Point(Layer.OffsetX, Layer.OffsetY);
            mw.Project.PushUndo(mw, ma);
        }

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();
        }
    }
}

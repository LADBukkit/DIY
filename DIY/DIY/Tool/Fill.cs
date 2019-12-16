using DIY.Project;
using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using System.Linq;

namespace DIY.Tool
{
    /// <summary>
    /// The Pipette Tool
    /// Used for grabbing a color
    /// </summary>
    class Fill : Tool
    {
        public int Threshold { get; set; }

        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;

            Layer lay = project.Layers[project.SelectedLayer];

            if(lay is ImageLayer) {
                ImageLayer ilay = (ImageLayer)lay;
                ImageAction action = new ImageAction("Fill");
                action.Layer = (ImageLayer)mw.Project.Layers[mw.Project.SelectedLayer];
                action.Old = ilay.Img.Clone();

                Color co = mw.ColorPicker.GetColor();
                DIYColor c = new DIYColor(255, co.R, co.G, co.B);
                List<Point> ppos = ilay.Img.FloodFill((int) p.X, (int) p.Y, Threshold, ilay.Img.GetPixel((int) p.X, (int) p.Y), c);
                List<int> pos = new List<int>(ppos.Select(i => (int)((i.X + ilay.OffsetX) + ((i.Y + lay.OffsetY) * mw.Project.Width))));

                foreach (int i in pos)
                {
                    if (i < 0) continue;
                    action.ChangedPixels.Add(i);
                }
                mw.ActionQueue.Enqueue(() => project.PixelCache.AddAll(pos));

                action.New = ilay.Img.Clone();
                mw.Project.PushUndo(mw, action);
            }
        }

        public override void MouseMove(MainWindow mw, Point p)
        {
            // nothing
        }

        public override void MouseUp(MainWindow mw, Point p)
        {
            // nothing
        }

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();

            // Threshold Regulator
            ValueRegulator tReg = new ValueRegulator();
            tReg.Minimum = 0;
            tReg.Maximum = 100;
            tReg.Label = "Threshold";
            Binding tBind = new Binding("Threshold");
            tBind.Source = this;
            tBind.Mode = BindingMode.TwoWay;
            tReg.SetBinding(ValueRegulator.ValueProperty, tBind);
            parent.Children.Add(tReg);
        }
    }
}

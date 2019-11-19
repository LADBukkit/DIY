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
    /// The Eraser Tool
    /// Used for erasing color
    /// </summary>
    class Eraser : Tool
    {
        /// <summary>
        /// The Size of the eraser
        /// </summary>
        public int Size { get; set; } = 1;

        /// <summary>
        /// The opacity of the eraser
        /// </summary>
        public int Opacity { get; set; } = 100;

        private ImageAction action { get; set; }

        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;

            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                action = new ImageAction("Erase");
                action.Layer = project.SelectedLayer;
                action.Old = ilay.Img.Clone();

                List<int> pos = ilay.Img.RemoveFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D), Opacity / 100D);

                foreach (int i in pos)
                {
                    if (i < 0) continue;
                    action.ChangedPixels.Add(i);
                }

                mw.ActionQueue.Enqueue(() =>
                {
                    foreach (int i in pos)
                    {
                        if (i < 0) continue;
                        project.PixelCache[i] = false;
                    }
                });
            }
        }

        public override void MouseMove(MainWindow mw, Point p)
        {
            if (action == null) return;
            DIYProject project = mw.Project;

            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                List<int> pos = ilay.Img.RemoveFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D), Opacity / 100D);

                foreach (int i in pos)
                {
                    if (i < 0) continue;
                    if (action == null) continue;
                    action.ChangedPixels.Add(i);
                }

                mw.ActionQueue.Enqueue(() =>
                {
                    foreach (int i in pos)
                    {
                        if (i < 0) continue;
                        project.PixelCache[i] = false;
                    }
                });
            }
        }

        public override void MouseUp(MainWindow mw, Point p)
        {
            if (action == null) return;
            DIYProject project = mw.Project;

            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;

                action.New = ilay.Img.Clone();
                mw.Project.PushUndo(mw, action);
                action = null;
            }
        }

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();

            // Size Regulator
            ValueRegulator sReg = new ValueRegulator();
            sReg.Minimum = 1;
            sReg.Maximum = 256;
            sReg.Label = "Size";
            Binding sBind = new Binding("Size");
            sBind.Source = this;
            sBind.Mode = BindingMode.TwoWay;
            sReg.SetBinding(ValueRegulator.ValueProperty, sBind);
            parent.Children.Add(sReg);

            // Opacity Regulator
            ValueRegulator oReg = new ValueRegulator();
            oReg.Minimum = 1;
            oReg.Maximum = 100;
            oReg.Label = "Opacity";
            Binding oBind = new Binding("Opacity");
            oBind.Source = this;
            oBind.Mode = BindingMode.TwoWay;
            oReg.SetBinding(ValueRegulator.ValueProperty, oBind);
            parent.Children.Add(oReg);
        }
    }
}

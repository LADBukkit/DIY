using DIY.Project;
using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// The Brush Tool
    /// Used for normal drawing
    /// </summary>
    class Brush : Tool
    {
        /// <summary>
        /// The Size of the brush
        /// </summary>
        public int Size { get; set; } = 1;

        /// <summary>
        /// The opacity of the brush
        /// </summary>
        public int Opacity { get; set; } = 100;

        private ImageAction action { get; set; }

        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;
            Color co = mw.ColorPicker.GetColor();
            DIYColor c = new DIYColor((int)(Opacity / 100D * 255D), co.R, co.G, co.B);

            Layer lay = project.Layers[project.SelectedLayer];
            if(lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                action = new ImageAction("Draw");
                action.Layer = project.SelectedLayer;
                action.Old = ilay.Img.Clone();
                List<Point> ppos = ilay.Img.DrawFilledCircle((int)p.X, (int)p.Y, (int) Math.Round(Size / 2D), c);
                List<int> pos = new List<int>(ppos.Select(i => (int)((i.X + ilay.OffsetX) + ((i.Y + lay.OffsetY) * ilay.Img.Width))));

                foreach (int i in pos)
                {
                    if (i < 0) continue;
                    action.ChangedPixels.Add(i);
                }
                mw.ActionQueue.Enqueue(() => project.PixelCache.AddAll(pos));
            }
        }

        public override void MouseMove(MainWindow mw, Point p)
        {
            if (action == null) return;
            DIYProject project = mw.Project;
            Color co = mw.ColorPicker.GetColor();
            DIYColor c = new DIYColor((int)(Opacity / 100D * 255D), co.R, co.G, co.B);

            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                List<Point> ppos = ilay.Img.DrawFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D), c);
                List<int> pos = new List<int>(ppos.Select(i => (int)((i.X + ilay.OffsetX) + ((i.Y + lay.OffsetY) * ilay.Img.Width))));

                foreach (int i in pos)
                {
                    if (i < 0) continue;
                    if (action == null) continue;
                    action.ChangedPixels.Add(i);
                }

                mw.ActionQueue.Enqueue(() => project.PixelCache.AddAll(pos));
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
            sReg.Maximum = 100;
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

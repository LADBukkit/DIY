using DIY.Project;
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

        public override void MouseDown(DIYProject project, Point p, DIYColor c)
        {
            c = new DIYColor(c.Argb);
            c.A = (int) (Opacity / 100D * 255D);
            Layer lay = project.Layers[project.SelectedLayer];
            if(lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                List<int> pos = ilay.Img.DrawFilledCircle((int)p.X, (int)p.Y, (int) Math.Round(Size / 2D), c);
                foreach(int i in pos)
                {
                    if (i < 0 || i >= project.PixelCache.Length) continue;
                    project.PixelCache[i] = false;
                }
            }
        }

        public override void MouseMove(DIYProject project, Point p, DIYColor c)
        {
            c = new DIYColor(c.Argb);
            c.A = (int)(Opacity / 100D * 255D);
            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                List<int> pos = ilay.Img.DrawFilledCircle((int)p.X, (int)p.Y, (int) Math.Round(Size / 2D), c);
                foreach (int i in pos)
                {
                    if (i < 0 || i >= project.PixelCache.Length) continue;
                    project.PixelCache[i] = false;
                }
            }
        }

        public override void MouseUp(DIYProject project, Point p, DIYColor c)
        {
            //throw new NotImplementedException();
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

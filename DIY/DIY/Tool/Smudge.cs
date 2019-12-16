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
    /// The Smudge Tool
    /// Used for smudging
    /// </summary>
    class Smudge : Tool
    {
        /// <summary>
        /// The Size of the Smudge
        /// </summary>
        public int Size { get; set; } = 1;

        /// <summary>
        /// The Hardness of the Smudge
        /// </summary>
        public int Hardness { get; set; } = 5;

        /// <summary>
        /// The Form of this Brush
        /// 0 = Circle
        /// 1 = Square
        /// </summary>
        public int Form { get; set; } = 0;

        private ImageAction action { get; set; }

        private double[] kernel;

        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;
            Color co = mw.ColorPicker.GetColor();

            Layer lay = project.Layers[project.SelectedLayer];
            if(lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                action = new ImageAction("Draw");
                action.Layer = (ImageLayer)mw.Project.Layers[mw.Project.SelectedLayer];
                action.Old = ilay.Img.Clone();
                List<Point> ppos = new List<Point>();
                if (Form == 0)
                {
                    ppos = ilay.Img.PointsFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D));
                }
                else if(Form == 1)
                {
                    ppos = ilay.Img.PointsFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D));
                }
                SmudgePoints(ilay, ppos);
                List<int> pos = new List<int>(ppos.Select(i => (int)((i.X + ilay.OffsetX) + ((i.Y + lay.OffsetY) * mw.Project.Width))));

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

            Layer lay = project.Layers[project.SelectedLayer];
            if (lay is ImageLayer)
            {
                ImageLayer ilay = (ImageLayer)lay;
                List<Point> ppos = new List<Point>();
                if (Form == 0)
                {
                    ppos = ilay.Img.PointsFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D));
                }
                else if (Form == 1)
                {
                    ppos = ilay.Img.PointsFilledCircle((int)p.X, (int)p.Y, (int)Math.Round(Size / 2D));
                }
                SmudgePoints(ilay, ppos);
                List<int> pos = new List<int>(ppos.Select(i => (int)((i.X + ilay.OffsetX) + ((i.Y + lay.OffsetY) * mw.Project.Width))));

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

            ComboBox cb = new ComboBox();
            cb.Items.Add("Circle");
            cb.Items.Add("Square");
            Binding cBind = new Binding("Form");
            cBind.Source = this;
            cBind.Mode = BindingMode.TwoWay;
            cb.SetBinding(ComboBox.SelectedIndexProperty, cBind);
            parent.Children.Add(cb);

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
            oReg.Minimum = 0;
            oReg.Maximum = 150;
            oReg.Label = "Hardness";
            Binding oBind = new Binding("Hardness");
            oBind.Source = this;
            oBind.Mode = BindingMode.TwoWay;
            oReg.vSlider.ValueChanged += (sender, e) => RecalculateKernel();
            oReg.SetBinding(ValueRegulator.ValueProperty, oBind);
            parent.Children.Add(oReg);
        }

        public void RecalculateKernel()
        {
            double std = Hardness / 10D;
            kernel = Filter.GaussianBlur.CalculateKernel(std);
        }

        public void SmudgePoints(ImageLayer ilay, List<Point> points)
        {
            Dictionary<Point, DIYColor> NewColor = new Dictionary<Point, DIYColor>();

            foreach (Point p in points)
            {
                int x = (int)p.X;
                int y = (int)p.Y;
                DIYColor dc = ilay.Img.GetPixel(x, y);
                double a = dc.A * kernel[0];
                double r = dc.R * kernel[0];
                double g = dc.G * kernel[0];
                double b = dc.B * kernel[0];
                for (int i = 1; i < kernel.Length; i++)
                {
                    DIYColor dc1 = ilay.Img.GetPixel(x + i, y);
                    if (dc1 == DIYColor.NULL_TYPE)
                    {
                        dc1 = dc;
                    }
                    a += dc1.A * kernel[i];
                    r += dc1.R * kernel[i];
                    g += dc1.G * kernel[i];
                    b += dc1.B * kernel[i];

                    dc1 = ilay.Img.GetPixel(x - i, y);
                    if (dc1 == DIYColor.NULL_TYPE)
                    {
                        dc1 = dc;
                    }
                    a += dc1.A * kernel[i];
                    r += dc1.R * kernel[i];
                    g += dc1.G * kernel[i];
                    b += dc1.B * kernel[i];
                }
                a = Math.Max(0, Math.Min(255, a));
                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));
                NewColor.Add(p, new DIYColor((byte)a, (byte)r, (byte)g, (byte)b));
            }

            foreach (Point p in points)
            {
                ilay.Img.SetPixel((int)p.X, (int)p.Y, NewColor[p], false);
            }
            NewColor.Clear();
            foreach (Point p in points)
            {
                int x = (int)p.X;
                int y = (int)p.Y;
                DIYColor dc = ilay.Img.GetPixel(x, y);
                double a = dc.A * kernel[0];
                double r = dc.R * kernel[0];
                double g = dc.G * kernel[0];
                double b = dc.B * kernel[0];
                for (int i = 1; i < kernel.Length; i++)
                {
                    DIYColor dc1 = ilay.Img.GetPixel(x, y + i);
                    if (dc1 == DIYColor.NULL_TYPE)
                    {
                        dc1 = dc;
                    }
                    a += dc1.A * kernel[i];
                    r += dc1.R * kernel[i];
                    g += dc1.G * kernel[i];
                    b += dc1.B * kernel[i];

                    dc1 = ilay.Img.GetPixel(x, y - i);
                    if (dc1 == DIYColor.NULL_TYPE)
                    {
                        dc1 = dc;
                    }
                    a += dc1.A * kernel[i];
                    r += dc1.R * kernel[i];
                    g += dc1.G * kernel[i];
                    b += dc1.B * kernel[i];
                }
                a = Math.Max(0, Math.Min(255, a));
                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));
                NewColor.Add(p, new DIYColor((byte)a, (byte)r, (byte)g, (byte)b));
            }

            foreach (Point p in points)
            {
                ilay.Img.SetPixel((int)p.X, (int)p.Y, NewColor[p], false);
            }
        }
    }
}

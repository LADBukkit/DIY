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
    /// The Pipette Tool
    /// Used for grabbing a color
    /// </summary>
    class Pipette : Tool
    {
        public override void MouseDown(MainWindow mw, Point p)
        {
            DIYProject project = mw.Project;

            Layer lay = project.Layers[project.SelectedLayer];

            DIYColor c = lay.GetBitmap().GetPixel((int) p.X, (int) p.Y);
            ColorUtil.ToHSL((byte) c.R, (byte) c.G, (byte) c.B, out double h, out double s, out double l);
            mw.ColorPicker.hue = h;
            mw.ColorPicker.saturation = s;
            mw.ColorPicker.lightness = l;
            mw.ColorPicker.Dispatcher.Invoke(() => mw.ColorPicker.UpdatePointer());
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
        }
    }
}

using DIY.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DIY
{
    /// <summary>
    /// Codebehind for the color wheel
    /// </summary>
    public partial class ColorWheel : UserControl
    {
        /// <summary>
        /// The hue of the current color
        /// </summary>
        public double hue = 0D;

        /// <summary>
        /// The saturation of the current color
        /// </summary>
        public double saturation = 1.0D;

        /// <summary>
        /// The lightness of the current color
        /// </summary>
        public double lightness = 0.5D;

        /// <summary>
        /// Whether the color should be updated on textchanged or not
        /// </summary>
        private bool noUpdate = false;

        public ColorWheel()
        {
            InitializeComponent();

            UpdatePointer();
        }

        /// <summary>
        /// Updates the pointer and the bg-color of the colorView TextBox as well as the text of said textbox
        /// </summary>
        public void UpdatePointer()
        {
            double w = Width;
            if(Double.IsNaN(w))
            {
                w = 200;
            }

            double rad = Math.PI * hue / 180.0;
            double radius = (w - 10) * saturation;
            double x = radius * Math.Cos(rad);
            double y = radius * Math.Sin(rad);
            Pointer.Margin = new Thickness(y, 0, 0, x);

            lightSlider.Value = lightness * 100;

            ColorUtil.ToRGB(hue, saturation, lightness, out byte r, out byte g, out byte b);

            colorView.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            if(lightness > 0.5)
            {
                colorView.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            } else
            {
                colorView.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

            noUpdate = true;
            colorView.Text = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
            noUpdate = false;
        }

        /// <summary>
        /// Handles the change in the slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lightness = lightSlider.Value / 100.0;
            UpdatePointer();
        }

        /// <summary>
        /// Calculates hue and saturation of a given point inside of the wheel
        /// </summary>
        /// <param name="p">The Point</param>
        private void colorWheel_Handle(Point p)
        {
            double w = Width;
            if (Double.IsNaN(w))
            {
                w = 200;
            }

            double x = p.X - (w / 2);
            double y = p.Y - (w / 2);

            double radius = Math.Sqrt(x * x + y * y);
            double alpha = Math.Atan2(y, x);
            double grad = alpha * 180.0 / Math.PI;
            grad += 90;
            if (grad < 0)
            {
                grad = 360 + grad;
            }
            hue = grad;

            radius /= (w - 10) / 2;
            if (radius > 1) radius = 1;
            saturation = radius;

            UpdatePointer();
        }

        /// <summary>
        /// Handles clicking down on the wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorWheel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Grid)sender);

            colorWheel_Handle(p);
        }

        /// <summary>
        /// Handles moving while holding down on the wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorWheel_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition((Grid) sender);
                colorWheel_Handle(p);
            }
        }

        /// <summary>
        /// Handles the text change of the colorview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorView_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noUpdate) return;
            try
            {
                Color c = (Color) ColorConverter.ConvertFromString(colorView.Text);
                ColorUtil.ToHSL(c.R, c.G, c.B, out double h, out double s, out double l);
                hue = h;
                saturation = s;
                lightness = l;
                UpdatePointer();
            }
            catch (FormatException) { }
        }

        /// <summary>
        /// Gets the currently selected color
        /// </summary>
        /// <returns>The selected Color</returns>
        public Color GetColor()
        {
            ColorUtil.ToRGB(hue, saturation, lightness, out byte r, out byte g, out byte b);
            return Color.FromRgb(r, g, b);
        }
    }
}

using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für ColorWheel.xaml
    /// </summary>
    public partial class ColorWheel : UserControl
    {
        public double hue = 0D;
        public double saturation = 0D;
        public double lightness = 1D;

        public ColorWheel()
        {
            InitializeComponent();

            UpdatePointer();
        }

        private void ToRGB(double h, double s, double l, out byte red, out byte green, out byte blue)
        {
            if (h < 0) h = 360 + h;
            if (h > 360) h = h - 360;
            if (s < 0) s = 0;
            if (s > 1) s = 1;
            if (l < 0) l = 0;
            if (l > 1) l = 1;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;
            double r = 0, g = 0, b = 0;
            if(h >= 0 && h < 60)
            {
                r = c;
                g = x;
            }
            else if(h >= 60 && h < 120)
            {
                r = x;
                g = c;
            }
            else if(h >= 120 && h < 180)
            {
                g = c;
                b = x;
            }
            else if(h >= 180 && h < 240)
            {
                g = x;
                b = c;
            }
            else if(h >= 240 && h < 300)
            {
                r = x;
                b = c;
            }
            else if(h >= 300 && h < 360)
            {
                r = c;
                b = x;
            }

            red = Convert.ToByte((r + m) * 255);
            green = Convert.ToByte((g + m) * 255);
            blue = Convert.ToByte((b + m) * 255);
        }

        private void ToHSL(byte red, byte green, byte blue, out double h, out double s, out double l)
        {
            double r = red / 255.0;
            double g = green / 255.0;
            double b = blue / 255.0;

            if (r < 0) r = 0;
            if (r > 1) r = 1;
            if (g < 0) g = 0;
            if (g > 1) g = 1;
            if (b < 0) b = 0;
            if (b > 1) b = 1;

            double cMax = Math.Max(r, Math.Max(g, b));
            double cMin = Math.Min(r, Math.Min(g, b));
            double delta = cMax - cMin;

            if (delta == 0)
            {
                h = 0;
            }
            else if (cMax == r)
            {
                h = 60 * ((g - b) / delta % 6);
            }
            else if (cMax == g)
            {
                h = 60 * ((b - r) / delta + 2);
            }
            else
            {
                h = 60 * ((r - g) / delta + 4);
            }

            l = (cMax + cMin) / 2;

            s = 0;
            if (delta != 0)
            {
                s = delta / (1 - Math.Abs(2 * l - 1));
            }
        }

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

            byte r, g, b;
            ToRGB(hue, saturation, lightness, out r, out g, out b);

            colorView.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            if(lightness > 0.5)
            {
                colorView.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            } else
            {
                colorView.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            colorView.Text = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        private void lightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lightness = lightSlider.Value / 100.0;
            UpdatePointer();
        }

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

            radius /= w / 2;
            saturation = radius;

            UpdatePointer();
        }

        private void colorWheel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Grid)sender);

            colorWheel_Handle(p);
        }

        private void colorWheel_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition((Grid) sender);
                colorWheel_Handle(p);
            }
        }

        private void colorView_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (colorView.Text.Length != 7) return;
            try
            {
                Color c = (Color) ColorConverter.ConvertFromString(colorView.Text);
                double h, s, l;
                ToHSL(c.R, c.G, c.B, out h, out s, out l);
                hue = h;
                saturation = s;
                lightness = l;
                UpdatePointer();
            }
            catch (FormatException) { }
        }
    }
}

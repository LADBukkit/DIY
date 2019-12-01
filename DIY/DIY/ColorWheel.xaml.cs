using DIY.Util;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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

            // calculate which is the best foreground color
            Color white = Color.FromRgb(240, 240, 240);
            Color black = Color.FromRgb(16, 16, 16);
            Color bg = Color.FromRgb(r, g, b);
            colorView.Background = new SolidColorBrush(bg);

            if (ColorUtil.CalculateContrast(white, bg) > ColorUtil.CalculateContrast(black, bg))
            {
                colorView.Foreground = new SolidColorBrush(white);
            } else
            {
                colorView.Foreground = new SolidColorBrush(black);
            }

            noUpdate = true;
            colorView.Text = "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
            noUpdate = false;

            // Updates the lightness in the colorwheel-drawing
            // Comment if you don't want that
            colorDrawing.Lightness = lightness;
            colorDrawing.InvalidateVisual();
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
            if (colorView.Text.StartsWith("#") && colorView.Text.Length != 7) return;
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

    class WheelDrawing : Canvas
    {
        public double Lightness { get; set; } = 0.5;

        protected override void OnRender(DrawingContext dc)
        {
            DirectBitmap db = new DirectBitmap((int) ActualWidth, (int) ActualHeight);

            int half = (int) ActualWidth / 2;
            for(int px = 0; px < ActualWidth; px++)
            {
                for(int py = 0; py < ActualHeight; py++)
                {
                    double x = px - half;
                    double y = py - half;

                    double radius = Math.Sqrt(x * x + y * y);
                    radius /= half - 1;
                    if (radius > 1) continue;

                    double alpha = Math.Atan2(y, x);
                    double grad = alpha * 180.0 / Math.PI;
                    grad += 90;
                    if (grad < 0)
                    {
                        grad = 360 + grad;
                    }
                    double hue = grad;
                    double saturation = radius;

                    ColorUtil.ToRGB(hue, saturation, Lightness, out byte red, out byte green, out byte blue);
                    db.SetPixel(px, py, new DIYColor(255, red, green, blue));
                }
            }

            ImageSource iso = ColorUtil.ImageSourceFromBitmap(db.Bitmap);
            dc.DrawImage(iso, new Rect(0, 0, ActualWidth, ActualHeight));
            db.Dispose();
        }
    }
}

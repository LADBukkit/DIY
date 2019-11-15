using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace DIY.Util
{
    /// <summary>
    /// Some Utils for colors and images.
    /// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// Converts hsl to rgb.
        /// </summary>
        /// <param name="h">The Hue</param>
        /// <param name="s">The Saturation</param>
        /// <param name="l">The Lightness</param>
        /// <param name="red">Red color value</param>
        /// <param name="green">Green color value</param>
        /// <param name="blue">Blue color value</param>
        public static void ToRGB(double h, double s, double l, out byte red, out byte green, out byte blue)
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
            if (h >= 0 && h < 60)
            {
                r = c;
                g = x;
            }
            else if (h >= 60 && h < 120)
            {
                r = x;
                g = c;
            }
            else if (h >= 120 && h < 180)
            {
                g = c;
                b = x;
            }
            else if (h >= 180 && h < 240)
            {
                g = x;
                b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x;
                b = c;
            }
            else if (h >= 300 && h < 360)
            {
                r = c;
                b = x;
            }

            red = Convert.ToByte((r + m) * 255);
            green = Convert.ToByte((g + m) * 255);
            blue = Convert.ToByte((b + m) * 255);
        }

        /// <summary>
        /// Converts rgb to hsl
        /// </summary>
        /// <param name="red">The red color value</param>
        /// <param name="green">The green color value</param>
        /// <param name="blue">The blue color value</param>
        /// <param name="h">The hue</param>
        /// <param name="s">The saturation</param>
        /// <param name="l">The lightness</param>
        public static void ToHSL(byte red, byte green, byte blue, out double h, out double s, out double l)
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

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Converts a bitmap to an ImageSource
        /// </summary>
        /// <param name="bmp">The bitmap</param>
        /// <returns></returns>
        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        /// <summary>
        /// Calculates the luminance of the color in relation to a standard monitor
        /// </summary>
        /// <param name="color">The Color</param>
        /// <returns></returns>
        public static double CalculateRelativeLuminance(System.Windows.Media.Color color)
        {
            double rg = color.R <= 10 ? color.R / 3294D : Math.Pow((color.R / 269D + 0.0513), 2.4);
            double gg = color.G <= 10 ? color.G / 3294D : Math.Pow((color.G / 269D + 0.0513), 2.4);
            double bg = color.B <= 10 ? color.B / 3294D : Math.Pow((color.B / 269D + 0.0513), 2.4);

            return 0.2126 * rg + 0.7152 * gg + 0.0722 * bg;
        }

        /// <summary>
        /// Calculates the contrast between two colors
        /// </summary>
        /// <param name="c1">The first color</param>
        /// <param name="c2">The second color</param>
        /// <returns></returns>
        public static double calculateContrast(System.Windows.Media.Color c1, System.Windows.Media.Color c2)
        {
            double l1 = CalculateRelativeLuminance(c1);
            double l2 = CalculateRelativeLuminance(c2);
            return (Math.Max(l1, l2) + 0.05D) / (Math.Min(l1, l2) + 0.05D);
        }

        // TODO auf A updaten
        #region Extensions
        public static Color add(this Color c1, Color c2)
        {
            int a = c1.A + c2.A;
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = c1.R + c2.R;
            int g = c1.R + c2.R;
            int b = c1.R + c2.R;
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte) r, (byte) g, (byte) b);
        }
        public static Color substract(this Color c1, Color c2)
        {
            int a = c1.A - c2.A;
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = c1.R - c2.R;
            int g = c1.R - c2.R;
            int b = c1.R - c2.R;
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color multiply(this Color c1, Color c2)
        {
            int a = c1.A * c2.A;
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = c1.R * c2.R;
            int g = c1.R * c2.R;
            int b = c1.R * c2.R;
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color divide(this Color c1, Color c2)
        {
            int a = c1.A / c2.A;
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = c1.R / c2.R;
            int g = c1.R / c2.R;
            int b = c1.R / c2.R;
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color add(this Color c1, double d)
        {
            int a = Convert.ToInt32(c1.A + d);
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = Convert.ToInt32(c1.R + d);
            int g = Convert.ToInt32(c1.R + d);
            int b = Convert.ToInt32(c1.R + d);
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color substract(this Color c1, double d)
        {
            int a = Convert.ToInt32(c1.A - d);
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = Convert.ToInt32(c1.R - d);
            int g = Convert.ToInt32(c1.R - d);
            int b = Convert.ToInt32(c1.R - d);
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color multiply(this Color c1, double d)
        {
            int a = Convert.ToInt32(c1.A * d);
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = Convert.ToInt32(c1.R * d);
            int g = Convert.ToInt32(c1.R * d);
            int b = Convert.ToInt32(c1.R * d);
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }

        public static Color divide(this Color c1, double d)
        {
            int a = Convert.ToInt32(c1.A / d);
            a = a > 255 ? 255 : a < 0 ? 0 : a;

            int r = Convert.ToInt32(c1.R / d);
            int g = Convert.ToInt32(c1.R / d);
            int b = Convert.ToInt32(c1.R / d);
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
            return Color.FromArgb((byte) a, (byte)r, (byte)g, (byte)b);
        }
        #endregion
    }
}

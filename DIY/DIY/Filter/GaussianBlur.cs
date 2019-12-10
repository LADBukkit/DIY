using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using DIY.Util;

namespace DIY.Filter
{
    public class GaussianBlur : Filter
    {
        public override string Name => "Gaussian Blur";

        public GaussianBlur()
        {
            Properties = new FilterProperty[] {
                new FilterPropertyNumeric<double>("Offset X", 0, 15, 0.5, 0.1),
                new FilterPropertyNumeric<double>("Offset Y", 0, 15, 0.5, 0.1)
            };
        }

        public override DirectBitmap CalculateFilter(DirectBitmap input)
        {
            DirectBitmap db = input.Clone();
            double[] kernelH = CalculateKernel(((FilterPropertyNumeric<double>)Properties[0]).Value);

            for (int x = 0; x < db.Width; x++)
            {
                for (int y = 0; y < db.Height; y++)
                {
                    DIYColor dc = input.GetPixel(x, y);
                    double a = dc.A * kernelH[0];
                    double r = dc.R * kernelH[0];
                    double g = dc.G * kernelH[0];
                    double b = dc.B * kernelH[0];
                    for(int i = 1; i < kernelH.Length; i++)
                    {
                        DIYColor dc1 = input.GetPixel(x + i, y);
                        if (dc1 == DIYColor.NULL_TYPE)
                        {
                            dc1 = dc;
                        }
                        a += dc1.A * kernelH[i];
                        r += dc1.R * kernelH[i];
                        g += dc1.G * kernelH[i];
                        b += dc1.B * kernelH[i];

                        dc1 = input.GetPixel(x - i, y);
                        if (dc1 == DIYColor.NULL_TYPE)
                        {
                            dc1 = dc;
                        }
                        a += dc1.A * kernelH[i];
                        r += dc1.R * kernelH[i];
                        g += dc1.G * kernelH[i];
                        b += dc1.B * kernelH[i];
                    }
                    a = Math.Max(0, Math.Min(255, a));
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    db.SetPixel(x, y, new DIYColor((byte)a, (byte)r, (byte)g, (byte)b), false);
                }
            }

            DirectBitmap db1 = db.Clone();
            double[] kernelV = CalculateKernel(((FilterPropertyNumeric<double>)Properties[1]).Value);

            for (int x = 0; x < db.Width; x++)
            {
                for (int y = 0; y < db.Height; y++)
                {
                    DIYColor dc = db.GetPixel(x, y);
                    double a = dc.A * kernelV[0];
                    double r = dc.R * kernelV[0];
                    double g = dc.G * kernelV[0];
                    double b = dc.B * kernelV[0];
                    for (int i = 1; i < kernelV.Length; i++)
                    {
                        DIYColor dc1 = db.GetPixel(x, y + i);
                        if(dc1 == DIYColor.NULL_TYPE)
                        {
                            dc1 = dc;
                        }
                        a += dc1.A * kernelV[i];
                        r += dc1.R * kernelV[i];
                        g += dc1.G * kernelV[i];
                        b += dc1.B * kernelV[i];

                        dc1 = db.GetPixel(x, y - i);
                        if (dc1 == DIYColor.NULL_TYPE)
                        {
                            dc1 = dc;
                        }
                        a += dc1.A * kernelV[i];
                        r += dc1.R * kernelV[i];
                        g += dc1.G * kernelV[i];
                        b += dc1.B * kernelV[i];
                    }
                    a = Math.Max(0, Math.Min(255, a));
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    db1.SetPixel(x, y, new DIYColor((byte)a, (byte)r, (byte)g, (byte)b), false);
                }
            }

            db.Dispose();

            return db1;
        }

        public static double[] CalculateKernel(double std)
        {
            int length = (int) Math.Ceiling(3 * std);
            if(length == 0)
            {
                return new double[] { 1 };
            }
            double[] kernel = new double[length];
            double sum = 0;
            for (int i = 0; i < length; i++)
            {
                kernel[i] = (1.0 / Math.Sqrt(2 * Math.PI * std * std)) * Math.Exp(- ((i * i) / (2.0 * std * std)));
                if(i == 0)
                {
                    sum += kernel[i];
                }
                else
                {
                    sum += kernel[i] * 2;
                }
            }
            sum = 1 / sum;
            for(int i = 0; i < kernel.Length; i++)
            {
                kernel[i] *= sum;
            }
            return kernel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using DIY.Util;

namespace DIY.Project
{
    /// <summary>
    /// Blendmode 'Enum'
    /// </summary>
    public class BlendMode
    {
        /// <summary>
        /// applies: (a, b) -> b
        /// </summary>
        public static readonly BlendMode NORMAL = new BlendMode("Normal", BlendModeFunctions.BM_Normal);

        /// <summary>
        /// applies: (a, b) -> a * b
        /// </summary>
        public static readonly BlendMode MULTIPLY = new BlendMode("Multiply", BlendModeFunctions.BM_Multiply);

        /// <summary>
        /// applies: (a, b) -> 1 - (1 - a) * (1 - b)
        /// </summary>
        public static readonly BlendMode SCREEN = new BlendMode("Screen", BlendModeFunctions.BM_Screen);

        /// <summary>
        /// applies: (a, b) ->
        ///                     2 * a * b                   | a < 0.5
        ///                     1 - 2 (1 - a) * (1 - b)     | else
        /// </summary>
        public static readonly BlendMode OVERLAY = new BlendMode("Overlay", BlendModeFunctions.BM_Overlay);

        /// <summary>
        /// applies: (a, b) -> Overlay(b, a)
        /// </summary>
        public static readonly BlendMode HARD_LIGHT = new BlendMode("Hard Light", BlendModeFunctions.BM_HardLight);

        /// <summary>
        /// applies: (a, b) -> (1 - 2 * b) * (a * a) + 2 * b * a
        /// </summary>
        public static readonly BlendMode SOFT_LIGHT = new BlendMode("Soft Light", BlendModeFunctions.BM_SoftLight);

        /// <summary>
        /// applies: (a, b) -> a / b
        /// </summary>
        public static readonly BlendMode DIVIDE = new BlendMode("Divide", BlendModeFunctions.BM_Divide);

        /// <summary>
        /// applies: (a, b) -> a + b
        /// </summary>
        public static readonly BlendMode ADD = new BlendMode("Add", BlendModeFunctions.BM_Add);

        /// <summary>
        /// applies: (a, b) -> a - b
        /// </summary>
        public static readonly BlendMode SUBTRACT = new BlendMode("Subtract", BlendModeFunctions.BM_Subtract);

        /// <summary>
        /// applies: (a, b) -> abs(a - b)
        /// </summary>
        public static readonly BlendMode DIFFERENCE = new BlendMode("Difference", BlendModeFunctions.BM_Difference);

        /// <summary>
        /// applies: (a, b) -> min(a, b)
        /// </summary>
        public static readonly BlendMode DARKEN_ONLY = new BlendMode("Darken Only", BlendModeFunctions.BM_DarkenOnly);

        /// <summary>
        /// applies: (a, b) -> max(a, b)
        /// </summary>
        public static readonly BlendMode LIGHTEN_ONLY = new BlendMode("Lighten Only", BlendModeFunctions.BM_LightenOnly);

        /// <summary>
        /// Contains alle Values of the enum
        /// </summary>
        public static IEnumerable<BlendMode> Values
        {
            get
            {
                yield return NORMAL;
                yield return null;
                yield return MULTIPLY;
                yield return SCREEN;
                yield return OVERLAY;
                yield return HARD_LIGHT;
                yield return SOFT_LIGHT;
                yield return null;
                yield return DIVIDE;
                yield return ADD;
                yield return SUBTRACT;
                yield return DIFFERENCE;
                yield return DARKEN_ONLY;
                yield return LIGHTEN_ONLY;
            }
        }

        /// <summary>
        /// Delegate for blending colors
        /// </summary>
        public DelBlendColors BlendColors { get; private set; }

        /// <summary>
        /// The Name of the BlendMode
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructs a BlendMode
        /// </summary>
        /// <param name="name">The Name of the BlendMode</param>
        /// <param name="mode">The delegate for blending colors</param>
        private BlendMode(string name, DelBlendColors mode)
        {
            Name = name;
            BlendColors = mode;
        }

        /// <summary>
        /// Overloads the default ToString() function to acustom to a BlendMode
        /// </summary>
        /// <returns>The Name of the BlendMode</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Delegate prototype for blending colors
        /// </summary>
        /// <param name="c1">Color one</param>
        /// <param name="c2">Color two</param>
        /// <param name="opacity">The Opacity of c2</param>
        /// <returns>Color One and Two blended together</returns>
        public delegate DIYColor DelBlendColors(DIYColor c1, DIYColor c2, double opacity);
    }

    /// <summary>
    /// Static Functions for the BlendColors delegate
    /// </summary>
    static class BlendModeFunctions
    {
        public static DIYColor BM_Normal(DIYColor c1, DIYColor c2, double opacity)
        {
            opacity = (c2.A / 255D) * opacity;

            return (c2 * opacity) + (c1 * (1D - opacity));
        }

        public static DIYColor BM_Multiply(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b) => a * b / 255;

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Screen(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int)((1 - (1 - ad) * (1 - bd)) * 255);
            }

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Overlay(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                if(ad < 0.5)
                {
                    return (int)(2 * ad * bd * 255);
                }
                return (int)((1 - 2 * (1 - ad) * (1 - bd)) * 255);
            }

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_HardLight(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                if (ad < 0.5)
                {
                    return (int)(2 * ad * bd * 255);
                }
                return (int)((1 - 2 * (1 - ad) * (1 - bd)) * 255);
            }

            c2 = new DIYColor(f(c2.A, c1.A), f(c2.R, c1.R), f(c2.G, c1.G), f(c2.B, c1.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_SoftLight(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int) (((1 - 2 * bd) * (ad * ad) + 2 * bd * ad) * 255);
            }

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Divide(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int)((ad / bd) * 255);
            }

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Add(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int)((ad + bd) * 255);
            }

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Subtract(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int)((ad - bd) * 255);
            }

            c2 = new DIYColor(f(c1.A, 255 - c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_Difference(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b)
            {
                double ad = a / 255D;
                double bd = b / 255D;
                return (int)(Math.Abs(ad - bd) * 255);
            }

            c2 = new DIYColor(f(c1.A, 255 - c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_DarkenOnly(DIYColor c1, DIYColor c2, double opacity) 
        {
            static int f(int a, int b) => Math.Min(a, b);

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }

        public static DIYColor BM_LightenOnly(DIYColor c1, DIYColor c2, double opacity)
        {
            static int f(int a, int b) => Math.Max(a, b);

            c2 = new DIYColor(f(c1.A, c2.A), f(c1.R, c2.R), f(c1.G, c2.G), f(c1.B, c2.B));

            return BM_Normal(c1, c2, opacity);
        }
    }
}

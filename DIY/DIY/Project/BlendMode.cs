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
        /// The Normal BlendMode it applies: (c1, c2) -> c2
        /// </summary>
        public static readonly BlendMode NORMAL = new BlendMode("Normal", BlendModeFunctions.BM_Normal);
        public static readonly BlendMode MULTIPLY = new BlendMode("Multiply", BlendModeFunctions.BM_Multiply);

        /// <summary>
        /// Contains alle Values of the enum
        /// </summary>
        public static IEnumerable<BlendMode> Values
        {
            get
            {
                yield return NORMAL;
                yield return MULTIPLY;
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
            c2 = new DIYColor(c2.A * c1.A / 255, c2.R * c1.R / 255, c2.G * c1.G / 255, c2.B * c1.B / 255);

            return (c2 * opacity) + (c1 * (1D - opacity));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DIY.Project
{
    /// <summary>
    /// Blendmode 'Enum'
    /// </summary>
    class BlendMode
    {
        /// <summary>
        /// The Normal BlendMode it applies: (c1, c2) -> c2
        /// </summary>
        public static readonly BlendMode NORMAL = new BlendMode("Normal", BlendModeFunctions.BM_Normal);

        /// <summary>
        /// Contains alle Values of the enum
        /// </summary>
        public static IEnumerable<BlendMode> Values
        {
            get
            {
                yield return NORMAL;
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
        /// <returns>Color One and Two blended together</returns>
        public delegate Color DelBlendColors(Color c1, Color c2);
    }

    /// <summary>
    /// Static Functions for the BlendColors delegate
    /// </summary>
    static class BlendModeFunctions
    {
        public static Color BM_Normal(Color c1, Color c2)
        {
            return c2;
        }
    }
}

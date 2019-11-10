using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DIY.Tool
{
    /// <summary>
    /// Template for the tools
    /// </summary>
    abstract class Tool
    {
        /// <summary>
        /// Prepares the Controls for the properties
        /// </summary>
        /// <param name="parent">The StackPanel to put the Controls in</param>
        public abstract void PrepareProperties(StackPanel parent);
    }
}

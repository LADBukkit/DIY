using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using DIY.Project;
using DIY.Util;

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

        public abstract void MouseDown(MainWindow mw, Point p);
        public abstract void MouseMove(MainWindow mw, Point p);
        public abstract void MouseUp(MainWindow mw, Point p);

    }
}

using DIY.Project;
using DIY.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY.Tool
{
    /// <summary>
    /// The Pipette Tool
    /// Used for grabbing a color
    /// </summary>
    class Pipette : Tool
    {
        public override void MouseDown(DIYProject project, Point p, DIYColor c)
        {
            throw new NotImplementedException();
        }

        public override void MouseMove(DIYProject project, Point p, DIYColor c)
        {
            throw new NotImplementedException();
        }

        public override void MouseUp(DIYProject project, Point p, DIYColor c)
        {
            throw new NotImplementedException();
        }

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();
        }
    }
}

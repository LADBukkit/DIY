using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY.Tool
{
    class Pipette : Tool
    {
        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();
        }
    }
}

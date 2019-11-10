using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY.Tool
{
    /// <summary>
    /// The Eraser Tool
    /// Used for erasing color
    /// </summary>
    class Eraser : Tool
    {
        /// <summary>
        /// The Size of the eraser
        /// </summary>
        public int Size { get; set; } = 1;

        /// <summary>
        /// The opacity of the eraser
        /// </summary>
        public int Opacity { get; set; } = 100;

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();

            // Size Regulator
            ValueRegulator sReg = new ValueRegulator();
            sReg.Minimum = 1;
            sReg.Maximum = 256;
            sReg.Label = "Size";
            Binding sBind = new Binding("Size");
            sBind.Source = this;
            sBind.Mode = BindingMode.TwoWay;
            sReg.SetBinding(ValueRegulator.ValueProperty, sBind);
            parent.Children.Add(sReg);

            // Opacity Regulator
            ValueRegulator oReg = new ValueRegulator();
            oReg.Minimum = 1;
            oReg.Maximum = 100;
            oReg.Label = "Opacity";
            Binding oBind = new Binding("Opacity");
            oBind.Source = this;
            oBind.Mode = BindingMode.TwoWay;
            oReg.SetBinding(ValueRegulator.ValueProperty, oBind);
            parent.Children.Add(oReg);
        }
    }
}

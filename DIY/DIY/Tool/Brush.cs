using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY.Tool
{
    class Brush : Tool
    {
        public int Size { get; set; } = 1;

        public override void PrepareProperties(StackPanel parent)
        {
            parent.Children.Clear();

            // Size Label
            TextBlock sLabel = new TextBlock();
            sLabel.Text = "Size";

            // Grid
            Grid sGrid = new Grid();
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new System.Windows.GridLength(70, System.Windows.GridUnitType.Star);
            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new System.Windows.GridLength(30, System.Windows.GridUnitType.Star);
            sGrid.ColumnDefinitions.Add(cd1);
            sGrid.ColumnDefinitions.Add(cd2);

            // Slider
            Slider sSlider = new Slider();
            sSlider.Minimum = 1;
            sSlider.Maximum = 256;
            Grid.SetColumn(sSlider, 0);

            // Up Down
            IntegerUpDown sUD = new IntegerUpDown();
            sUD.Minimum = (int) sSlider.Minimum;
            sUD.Maximum = (int) sSlider.Maximum;
            Grid.SetColumn(sUD, 1);

            // Binding
            Binding sBinding = new Binding("Size");
            sBinding.Source = this;
            sSlider.SetBinding(Slider.ValueProperty, sBinding);
            sUD.SetBinding(IntegerUpDown.ValueProperty, sBinding);

            // Add
            parent.Children.Add(sLabel);
            sGrid.Children.Add(sSlider);
            sGrid.Children.Add(sUD);
            parent.Children.Add(sGrid);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace DIY
{
    /// <summary>
    /// Interaktionslogik für ValueRegulator.xaml
    /// </summary>
    public partial class ValueRegulator : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ValueRegulator), new PropertyMetadata(""));
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(ValueRegulator), new PropertyMetadata(0));
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(ValueRegulator), new PropertyMetadata(0));
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(ValueRegulator), new PropertyMetadata(0));
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ValueRegulator()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }
    }
}

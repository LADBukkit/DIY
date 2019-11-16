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

namespace DIY
{
    /// <summary>
    /// Interaktionslogik für LayerCtrl.xaml
    /// </summary>
    public partial class LayerCtrl : UserControl
    {
        public LayerCtrl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty LayerNameProperty = DependencyProperty.Register("LayerName", typeof(string), typeof(LayerCtrl), new PropertyMetadata(""));
        public string LayerName
        {
            get { return (string)GetValue(LayerNameProperty); }
            set { SetValue(LayerNameProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(LayerCtrl), new PropertyMetadata(null));
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(LayerCtrl), new PropertyMetadata(false));
        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }
    }
}

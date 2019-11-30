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
using System.Windows.Shapes;

namespace DIY
{
    /// <summary>
    /// Interaktionslogik für RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {

        public static readonly DependencyProperty OldnameProperty = DependencyProperty.Register("Oldname", typeof(string), typeof(RenameWindow), new PropertyMetadata(""));
        public string Oldname
        {
            get { return (string)GetValue(OldnameProperty); }
            set { SetValue(OldnameProperty, value); }
        }

        public bool Okay = false;

        public RenameWindow(string Oldname)
        {
            this.Oldname = Oldname;
            InitializeComponent();

            DataContext = this;
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            Okay = false;
            Close();
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            Okay = true;
            Close();
        }
    }
}

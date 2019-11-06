using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PreferencesWindow pWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            if(pWindow == null)
            {
                pWindow = new PreferencesWindow();
            }
            if(pWindow.IsVisible)
            {
                pWindow.Focus();
            } else
            {
                pWindow.Close();
                pWindow = new PreferencesWindow();
                pWindow.Show();
            }
        }
    }
}

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

            brush.IsChecked = true;
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

        private void Tool_Checked(object sender, RoutedEventArgs e)
        {
            if (toolProperties == null) return;
            if(sender == brush)
            {
                Tool.Brush b = new Tool.Brush();
                b.PrepareProperties(toolProperties);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            contentZoomBox.FitToBounds();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

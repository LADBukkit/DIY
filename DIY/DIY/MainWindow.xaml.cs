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
        /// <summary>
        /// The Preferences window. Saved so you can't open it multiple times.
        /// </summary>
        private PreferencesWindow pWindow;

        /// <summary>
        /// Dictionary for all the tools.
        /// </summary>
        private Dictionary<string, DIY.Tool.Tool> tools = new Dictionary<string, DIY.Tool.Tool>();

        public MainWindow()
        {
            InitializeComponent();

            // Add the tools to the dictionary
            tools.Add("brush", new DIY.Tool.Brush());
            tools.Add("eraser", new DIY.Tool.Eraser());
            tools.Add("pipette", new DIY.Tool.Pipette());

            // Select the default brush
            brush.IsChecked = true;

            // BlendModes binding
            LayerBlendMode.ItemsSource = Project.BlendMode.Values;
            LayerBlendMode.DisplayMemberPath = "Name";
            LayerBlendMode.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles opening the Preferences
        /// 
        /// If already opened than refocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handles selecting the tools and displaying the preference controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tool_Checked(object sender, RoutedEventArgs e)
        {
            if (toolProperties == null) return;
            RadioButton rb = (RadioButton)sender;
            if (!tools.ContainsKey(rb.Name)) return;

            tools[rb.Name].PrepareProperties(toolProperties);
        }

        /// <summary>
        /// Fits the zoombox content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            contentZoomBox.FitToBounds();
        }

        /// <summary>
        /// Shutdowns the application on exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

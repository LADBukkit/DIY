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
    /// Code-Behind for the NewWindow
    /// </summary>
    public partial class NewWindow : Window
    {
        /// <summary>
        /// Whether this Window was successful or not
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// All the different Layouts
        /// </summary>
        public static IEnumerable<LayoutC> Layouts
        {
            get
            {
                yield return new LayoutC("720p (1280x720)", new Size(1280, 720));
                yield return new LayoutC("1080p (1920x1080)", new Size(1920, 1080));
            }
        }

        public NewWindow()
        {
            InitializeComponent();

            Layout.ItemsSource = Layouts;
            Layout.DisplayMemberPath = "Name";
            Layout.SelectedIndex = 0;
        }

        /// <summary>
        /// Called when clicking on abort
        /// Closes the window and says no success
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        /// <summary>
        /// Called when clicking on Create New
        /// Closes the window and says success
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            Close();
        }

        /// <summary>
        /// When a new Layout gets selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Layout_Selected(object sender, RoutedEventArgs e)
        {
            LayoutC lay = (LayoutC)Layout.SelectedItem;
            UDWidth.Value = (int) lay.Size.Width;
            UDHeight.Value = (int) lay.Size.Height;
        }
    }

    /// <summary>
    /// A Class for containing different layouts
    /// </summary>
    public class LayoutC
    {
        public Size Size { get; private set; }
        public string Name { get; private set; }

        public LayoutC(string name, Size size)
        {
            Name = name;
            Size = size;
        }
    }
}

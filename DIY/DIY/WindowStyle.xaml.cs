using System.Windows;

namespace DIY
{
    /// <summary>
    /// Code behind for the Custom WindowStyle
    /// </summary>
    public partial class WindowStyle : ResourceDictionary
    {
        public WindowStyle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles pressing the exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        /// <summary>
        ///  Handles pressing the maximize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize_Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            if(window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            } else
            {
                window.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Handles pressing the minimize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Codebehind for the preferences Window
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        /// <summary>
        /// The settings to save to.
        /// </summary>
        private Settings settings;

        public PreferencesWindow(Settings set)
        {
            InitializeComponent();

            settings = set;
        }

        /// <summary>
        /// Handles the updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            foreach(PrefColorCtrl pcc in Theme.Children.OfType<PrefColorCtrl>())
            {
                try
                {
                    Color c = (Color)ColorConverter.ConvertFromString(pcc.Text);
                    Application.Current.Resources[pcc.Resource] = new SolidColorBrush(c);
                    settings["c_" + pcc.Resource] = pcc.Text;
                }
                catch (FormatException) { }
            }

            settings.Save();
        }
    }
}

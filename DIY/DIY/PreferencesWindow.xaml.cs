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
                }
                catch (FormatException) { }
            }
        }

        /// <summary>
        /// Handles saving of the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (PrefColorCtrl pcc in Theme.Children.OfType<PrefColorCtrl>())
            {
                settings["c_" + pcc.Resource] = pcc.Text;
            }
            settings.Save();
            this.Close();
        }

        /// <summary>
        /// Handles the reset on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (string key in Application.Current.Resources.MergedDictionaries[0].Keys)
            {
                if (key.StartsWith("c_"))
                {
                    if (settings[key] == null)
                    {
                        settings[key] = Application.Current.Resources[key].ToString();
                    }
                    Application.Current.Resources[key.Substring(2)] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings[key].ToString()));
                }
            }
        }
    }
}
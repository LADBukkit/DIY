﻿using System;
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
    /// Interaktionslogik für PrefColorCtrl.xaml
    /// </summary>
    public partial class PrefColorCtrl : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(PrefColorCtrl), new PropertyMetadata(""));
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PrefColorCtrl), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ResourceProperty = DependencyProperty.Register("Resource", typeof(string), typeof(PrefColorCtrl), new PropertyMetadata(""));
        public string Resource
        {
            get { return (string)GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register("Default", typeof(Color), typeof(PrefColorCtrl), new PropertyMetadata(null));
        public Color Default
        {
            get { return (Color)GetValue(DefaultProperty); }
            set { SetValue(DefaultProperty, value); }
        }

        public PrefColorCtrl()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            // For instant update: Application.Current.Resources[Resource] = Default;
            Text = "#" + Default.R.ToString("X2") + Default.G.ToString("X2") + Default.B.ToString("X2");
        }
    }
}

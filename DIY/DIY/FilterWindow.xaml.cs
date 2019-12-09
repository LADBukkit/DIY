using DIY.Project;
using DIY.Util;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace DIY
{
    /// <summary>
    /// Interaktionslogik für FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        public Filter.Filter Filter { get; set; }
        public ImageLayer Layer { get; set; }

        public FilterWindow(Filter.Filter filter, ImageLayer layer)
        {
            InitializeComponent();

            Filter = filter;
            Title = Filter.Name;
            Layer = layer;

            foreach(Filter.FilterProperty fp in Filter.Properties)
            {
                stackControls.Children.Add(new TextBlock() { Text = fp.Name });
                if(fp.Type == typeof(int))
                {
                    Filter.FilterPropertyNumeric<int> fpi = (Filter.FilterPropertyNumeric<int>)fp;
                    Slider sli = new Slider();
                    sli.Maximum = fpi.Max;
                    sli.Minimum = fpi.Min;
                    sli.Value = fpi.Default;
                    sli.HorizontalAlignment = HorizontalAlignment.Stretch;
                    sli.MouseDoubleClick += (sender, e) => sli.Value = fpi.Default;
                    sli.ValueChanged += (sender, e) => sli.Value = fpi.Value = (int)sli.Value;
                    sli.SmallChange = fpi.Interval;

                    TextBlock tb = new TextBlock();
                    Binding bin = new Binding("Value");
                    bin.Source = sli;
                    tb.SetBinding(TextBlock.TextProperty, bin);

                    stackControls.Children.Add(sli);
                    stackControls.Children.Add(tb);
                }
                else if(fp.Type == typeof(double))
                {
                    Filter.FilterPropertyNumeric<double> fpi = (Filter.FilterPropertyNumeric<double>)fp;
                    Slider sli = new Slider();
                    sli.Maximum = fpi.Max;
                    sli.Minimum = fpi.Min;
                    sli.Value = fpi.Default;
                    sli.HorizontalAlignment = HorizontalAlignment.Stretch;
                    sli.MouseDoubleClick += (sender, e) => sli.Value = fpi.Default;
                    sli.ValueChanged += (sender, e) => sli.Value = fpi.Value = Math.Round(sli.Value, 1);
                    sli.SmallChange = fpi.Interval;

                    TextBlock tb = new TextBlock();
                    Binding bin = new Binding("Value");
                    bin.Source = sli;
                    tb.SetBinding(TextBlock.TextProperty, bin);

                    stackControls.Children.Add(sli);
                    stackControls.Children.Add(tb);
                }
            }

            DataContext = this;
        }

        public void PreviewImage()
        {
            using(DirectBitmap db = Filter.CalculateFilter(Layer.GetBitmap()))
            {
                preview.Source = ColorUtil.ImageSourceFromBitmap(db.Bitmap);
                preview.Width = Layer.GetBitmap().Width;
                preview.Height = Layer.GetBitmap().Height;
                previewZoomBox.FitToBounds();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PreviewImage();
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            PreviewImage();
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

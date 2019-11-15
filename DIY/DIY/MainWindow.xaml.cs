using DIY.Project;
using DIY.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly Dictionary<string, DIY.Tool.Tool> tools = new Dictionary<string, DIY.Tool.Tool>();

        /// <summary>
        /// The settings handler.
        /// </summary>
        private readonly Settings settings = new Settings();

        /// <summary>
        /// The underlying Project
        /// </summary>
        private DIYProject Project { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Add the tools to the dictionary
            tools.Add("brush", new DIY.Tool.Brush());
            tools.Add("eraser", new DIY.Tool.Eraser());
            tools.Add("pipette", new DIY.Tool.Pipette());

            // Select the default brush
            brush.IsChecked = true;

            // BlendModes binding
            LayerBlendMode.ItemsSource = BlendMode.Values;
            LayerBlendMode.DisplayMemberPath = "Name";
            LayerBlendMode.SelectedIndex = 0;

            // Settings
            foreach (string key in Application.Current.Resources.MergedDictionaries[0].Keys)
            {
                if (key.StartsWith("c_"))
                {
                    if(settings[key] == null)
                    {
                        settings[key] = Application.Current.Resources[key].ToString();
                    }
                    Application.Current.Resources[key.Substring(2)] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings[key].ToString()));
                }
            }
            settings.Save();
        }

        /// <summary>
        /// Updates the canvas
        /// </summary>
        public void UpdateCanvas()
        {
            if (Project == null) return;

            drawingPanel.Width = Project.Width;
            drawingPanel.Height = Project.Height;
            Project.CalcBitmap();
            drawingPanel.Img = Project.Render;
            drawingPanel.InvalidateVisual();
            drawingPanel.UpdateLayout();
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
                pWindow = new PreferencesWindow(settings);
            }
            if(pWindow.IsVisible)
            {
                pWindow.Focus();
            } else
            {
                pWindow.Close();
                pWindow = new PreferencesWindow(settings);
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

        /// <summary>
        /// When the new button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewWindow nw = new NewWindow();
            nw.ShowDialog();

            if(nw.Success)
            {
                Project = new DIYProject((int) nw.UDWidth.Value, (int) nw.UDHeight.Value);
                UpdateCanvas();
                contentZoomBox.FitToBounds();
            }
        }

        /// <summary>
        /// Clicking on the drawing panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(drawingPanel);
                if(p.X > 0 && p.X < Project.Width && p.Y > 0 && p.Y < Project.Height)
                {
                    // test drawing code
                    int x = (int)p.X;
                    int y = (int)p.Y;
                    //MessageBox.Show(p.X + " " + p.Y);
                    Color c = ColorPicker.GetColor();
                    DIYColor dc = new DIYColor(255, c.R, c.G, c.B);
                    ((ImageLayer)Project.Layers[0]).Img.SetPixel(x, y, dc);
                    int pos = x + (y * Project.Width);
                    Project.PixelCache[pos] = false;
                    UpdateCanvas();
                }
            }
        }
    }

    public class ImgCanvas : Canvas
    {
        public DirectBitmap Img { get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            if(Img != null)
            {
                dc.DrawImage(ColorUtil.ImageSourceFromBitmap(Img.Bitmap), new Rect(0, 0, Img.Width, Img.Height));
            }
        }
    }
}

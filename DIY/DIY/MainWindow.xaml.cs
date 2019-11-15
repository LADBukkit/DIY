using DIY.Project;
using DIY.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
        /// The Current brush selected
        /// </summary>
        private Tool.Tool CurrentBrush;

        /// <summary>
        /// The Action queue
        /// </summary>
        private ConcurrentQueue<Action> ActionQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// Timer for the queue
        /// </summary>
        private Timer Timer;

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

            Timer = new Timer();
            Timer.Interval = 1;
            Timer.Elapsed += (source, e) => HandleQueue();
            Timer.Enabled = true;

            DispatcherTimer t2 = new DispatcherTimer();
            t2.Tick += (sender, e) => {
                if (Project == null) return;
                Project.CalcBitmap();
                drawingPanel.InvalidateVisual();
            };
            t2.Interval = new TimeSpan(0, 0, 0, 0, 25);
            t2.Start();
        }

        /// <summary>
        /// Handles the next element in the queue.
        /// </summary>
        private void HandleQueue()
        {
            while(ActionQueue.Count > 0)
            {
                if(ActionQueue.TryDequeue(out Action a) && a != null)
                {
                    a();
                }
            }
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
            if (!tools.ContainsKey(rb.Name))
            {
                CurrentBrush = null;
                return;
            }

            CurrentBrush = tools[rb.Name];
            CurrentBrush.PrepareProperties(toolProperties);
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
                drawingPanel.Width = Project.Width;
                drawingPanel.Height = Project.Height;
                drawingPanel.Img = Project.Render;
                contentZoomBox.FitToBounds();
            }
        }

        private void contentZoomBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if(p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    Color c = ColorPicker.GetColor();
                    DIYColor dc = new DIYColor(255, c.R, c.G, c.B);
                    ActionQueue.Enqueue(() => {
                        CurrentBrush.MouseMove(Project, p, dc);
                    }); 
                }
            }
        }

        private void contentZoomBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if (p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    Color c = ColorPicker.GetColor();
                    DIYColor dc = new DIYColor(255, c.R, c.G, c.B);
                    ActionQueue.Enqueue(() => {
                        CurrentBrush.MouseDown(Project, p, dc);
                    });
                }
            }
        }

        private void contentZoomBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if (p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    Color c = ColorPicker.GetColor();
                    DIYColor dc = new DIYColor(255, c.R, c.G, c.B);
                    ActionQueue.Enqueue(() => {
                        CurrentBrush.MouseUp(Project, p, dc);
                    });
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

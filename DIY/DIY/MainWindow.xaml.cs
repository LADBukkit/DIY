using DIY.Project;
using DIY.Project.Action;
using DIY.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        public ConcurrentQueue<Action> ActionQueue = new ConcurrentQueue<Action>();

        public Thread Updater;
        public bool Running = true;

        /// <summary>
        /// The underlying Project
        /// </summary>
        public DIYProject Project { get; set; }

        private bool BlockMouse = false;

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
                    if (settings[key] == null)
                    {
                        settings[key] = Application.Current.Resources[key].ToString();
                    }
                    Application.Current.Resources[key.Substring(2)] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings[key].ToString()));
                }
            }
            settings.Save();

            Updater = new Thread(() => HandleQueue());
            Updater.Start();

            DispatcherTimer t2 = new DispatcherTimer();
            t2.Tick += (sender, e) => UpdateProject();
            t2.Interval = new TimeSpan(0, 0, 0, 0, 50);
            t2.Start();
        }

        /// <summary>
        /// Handles the next element in the queue.
        /// </summary>
        private void HandleQueue()
        {
            while (Running)
            {
                if (ActionQueue.TryDequeue(out Action a) && a != null)
                {
                    a();
                }
            }
        }

        private void UpdateProject()
        {
            if (Project == null) return;

            Stopwatch sw = Stopwatch.StartNew();
            Project.CalcBitmap();
            drawingPanel.InvalidateVisual();

            LayerList.Children.Clear();
            for(int i = Project.Layers.Count - 1; i >= 0 ; i--)
            {
                Layer l = Project.Layers[i];
                LayerCtrl layerCtrl = new LayerCtrl();
                layerCtrl.LayerName = l.Name;
                layerCtrl.Image = ColorUtil.ImageSourceFromBitmap(l.GetBitmap().Bitmap);
                layerCtrl.Height = 75;
                layerCtrl.Selected = i == Project.SelectedLayer;
                int layerNumber = i;
                layerCtrl.MouseDown += (sender, e) =>
                {
                    LayerSelectionAction ac = new LayerSelectionAction();
                    ac.Old = Project.SelectedLayer;
                    ac.New = layerNumber;
                    Project.PushUndo(this, ac);

                    Project.SelectedLayer = layerNumber;
                };
                
                LayerList.Children.Add(layerCtrl);
            }

            Layer sel = Project.Layers[Project.SelectedLayer];
            LayerBlendMode.SelectedItem = sel.Mode;
            LayerOpacity.Value = (int) (sel.Opacity * 100);
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
            if (pWindow == null)
            {
                pWindow = new PreferencesWindow(settings);
            }
            if (pWindow.IsVisible)
            {
                pWindow.Focus();
            }
            else
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
            Running = false;
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

            if (nw.Success)
            {
                Project = new DIYProject((int)nw.UDWidth.Value, (int)nw.UDHeight.Value);
                drawingPanel.Width = Project.Width;
                drawingPanel.Height = Project.Height;
                drawingPanel.Img = Project.Render;
                contentZoomBox.FitToBounds();
            }
        }

        private void contentZoomBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (BlockMouse) return;

            if (e.LeftButton == MouseButtonState.Pressed && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if (p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    ActionQueue.Enqueue(() => CurrentBrush.MouseMove(this, p));
                }
            }
        }

        private void contentZoomBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BlockMouse) return;

            if (e.ChangedButton == MouseButton.Left && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if (p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    ActionQueue.Enqueue(() => CurrentBrush.MouseDown(this, p));
                }
            }
        }

        private void contentZoomBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (BlockMouse) return;

            if (e.ChangedButton == MouseButton.Left && CurrentBrush != null && Project != null)
            {
                Point p = e.GetPosition(drawingPanel);
                if (p.X >= 0 && p.X < Project.Width && p.Y >= 0 && p.Y < Project.Height)
                {
                    ActionQueue.Enqueue(() => CurrentBrush.MouseUp(this, p));
                }
            }
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(Project != null)
            {
                if(CurrentBrush != null)
                {
                    ActionQueue.Enqueue(() => { BlockMouse = true; CurrentBrush.MouseUp(this, new Point(-1, -1)); });
                }
                ActionQueue.Enqueue(() => {
                    Project.Undo(this);
                    BlockMouse = false;
                });
                while (!ActionQueue.IsEmpty) { }
            }
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project != null)
            {
                if (CurrentBrush != null)
                {
                    ActionQueue.Enqueue(() => { BlockMouse = true; CurrentBrush.MouseUp(this, new Point(-1, -1)); });
                }
                ActionQueue.Enqueue(() => {
                    Project.Redo(this);
                    BlockMouse = false;
                });
                while (!ActionQueue.IsEmpty) { }
            }
        }

        private void NewLayer_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;

            Layer lay = new ImageLayer(Project.Width, Project.Height);
            Project.Layers.Add(lay);

            NewLayerAction ac = new NewLayerAction();
            ac.Layer = lay;
            Project.PushUndo(this, ac);
        }

        private void LayerOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Project == null) return;

            Project.Layers[Project.SelectedLayer].Opacity = e.NewValue / 100D;
            for(int i = 0; i < Project.Width * Project.Height; i++)
            {
                Project.PixelCache[i] = false;
            }

            // add to undo
        }

        private void LayerBlendMode_Selected(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;
            LayerBlendModeAction ac = new LayerBlendModeAction();
            ac.Layer = Project.Layers[Project.SelectedLayer];
            ac.Old = ac.Layer.Mode;
            ac.New = (BlendMode)LayerBlendMode.SelectedItem;
            Project.PushUndo(this, ac);

            ac.Layer.Mode = ac.New;
            for (int i = 0; i < Project.Width * Project.Height; i++)
            {
                Project.PixelCache[i] = false;
            }

            // add to undo
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

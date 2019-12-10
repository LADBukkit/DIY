using DIY.Project;
using DIY.Project.Action;
using DIY.Util;
using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;

namespace DIY
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedUICommand C_IMPORT = new RoutedUICommand("Import", "Import", typeof(MainWindow));
        public static RoutedUICommand C_EXPORT = new RoutedUICommand("Export", "Export", typeof(MainWindow));

        /// <summary>
        /// The Preferences window. Saved so you can't open it multiple times.
        /// </summary>
        private PreferencesWindow pWindow;

        /// <summary>
        /// Dictionary for all the tools.
        /// </summary>
        private readonly Dictionary<string, DIY.Tool.Tool> tools = new Dictionary<string, DIY.Tool.Tool>();

        private readonly Dictionary<string, Type> filterList = new Dictionary<string, Type>();

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
        private bool IsMouseDown = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Add the tools to the dictionary
            tools.Add("brush", new DIY.Tool.Brush());
            tools.Add("eraser", new DIY.Tool.Eraser());
            tools.Add("pipette", new DIY.Tool.Pipette());
            tools.Add("fill", new DIY.Tool.Fill());
            tools.Add("move", new DIY.Tool.Move());
            tools.Add("smudge", new DIY.Tool.Smudge());

            filterList.Add("filter_hslwheel", typeof(Filter.HSLWheel));
            filterList.Add("filter_gaussian", typeof(Filter.GaussianBlur));

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
            Updater.IsBackground = true;
            Updater.Priority = ThreadPriority.Highest;
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

            //Project.CalcBitmap();
            //drawingPanel.InvalidateVisual();

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

            this.Title = "DrawItYourself";
            if(Project.PATH != null)
            {
                this.Title += " - " + Project.PATH;
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
            Updater.Join(1000);
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
                /*drawingPanel.Width = Project.Width;
                drawingPanel.Height = Project.Height;
                drawingPanel.Img = Project.Render;*/
                opglDraw.Height = Project.Height;
                opglDraw.Width = Project.Width;
                contentZoomBox.FitToBounds();
            }
        }

        private Point OLDPOINT;

        private void contentZoomBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (BlockMouse) return;

            if (e.LeftButton == MouseButtonState.Pressed && CurrentBrush != null && Project != null)
            {
                Layer lay = Project.Layers[Project.SelectedLayer];
                Point p = e.GetPosition(opglDraw);
                if((int) p.X == (int) OLDPOINT.X && (int)p.Y == (int)OLDPOINT.Y)
                {
                    return;
                }
                OLDPOINT = p;
                if(!(CurrentBrush is DIY.Tool.Move))
                {
                    p.X -= lay.OffsetX;
                    p.Y -= lay.OffsetY;
                }
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
                IsMouseDown = true;
                Layer lay = Project.Layers[Project.SelectedLayer];
                Point p = e.GetPosition(opglDraw);
                OLDPOINT = p;
                if (!(CurrentBrush is DIY.Tool.Move))
                {
                    p.X -= lay.OffsetX;
                    p.Y -= lay.OffsetY;
                }
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
                IsMouseDown = false;
                Layer lay = Project.Layers[Project.SelectedLayer];
                Point p = e.GetPosition(opglDraw);
                if (!(CurrentBrush is DIY.Tool.Move))
                {
                    p.X -= lay.OffsetX;
                    p.Y -= lay.OffsetY;
                }
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
                    ActionQueue.Enqueue(() => {
                        BlockMouse = true;
                        if(IsMouseDown)
                        {
                            CurrentBrush.MouseUp(this, new Point(-1, -1));
                        }
                    });
                }
                ActionQueue.Enqueue(() => {
                    Project.Undo(this);
                    BlockMouse = false;
                });
                //while (!ActionQueue.IsEmpty) { }
            }
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project != null)
            {
                if (CurrentBrush != null)
                {
                    ActionQueue.Enqueue(() => {
                        BlockMouse = true;
                        if (IsMouseDown)
                        {
                            CurrentBrush.MouseUp(this, new Point(-1, -1));
                        }
                    });
                }
                ActionQueue.Enqueue(() => {
                    Project.Redo(this);
                    BlockMouse = false;
                });
                //while (!ActionQueue.IsEmpty) { }
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
                Project.PixelCache.Add(i);
            }
        }

        private void LayerBlendMode_Selected(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;
            if ((BlendMode)LayerBlendMode.SelectedItem == Project.Layers[Project.SelectedLayer].Mode) return;

            LayerBlendModeAction ac = new LayerBlendModeAction();
            ac.Layer = Project.Layers[Project.SelectedLayer];
            ac.Old = ac.Layer.Mode;
            ac.New = (BlendMode)LayerBlendMode.SelectedItem;
            Project.PushUndo(this, ac);

            ac.Layer.Mode = ac.New;
            for (int i = 0; i < Project.Width * Project.Height; i++)
            {
                Project.PixelCache.Add(i);
            }
        }

        private void opglDraw_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs e)
        {
            if (Project == null) return;
            ActionQueue.Enqueue(() => {
                bool drawn = false;
                opglDraw.Dispatcher.Invoke(() =>
                {
                    OpenGL gl = opglDraw.OpenGL;

                    gl.Color(0f, 0f, 0f);
                    gl.PointSize(1f);
                    gl.Begin(BeginMode.Points);

                    Project.CalcBitmap((x, y, c) => {
                        gl.Color((byte)c.R, (byte)c.G, (byte)c.B);
                        gl.Vertex(x + 0.5, y + 0.5, -1);
                    });

                    gl.End();
                    drawn = true;
                });
                while (!drawn) { }
            });
        }

        private void opglDraw_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = opglDraw.OpenGL;
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            gl.Ortho(0, opglDraw.Width, opglDraw.Height, 0, 1, 2);
            gl.MatrixMode(MatrixMode.Modelview);
        }

        private void opglDraw_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = opglDraw.OpenGL;
            gl.ClearColor(0, 0, 0, 0);
        }

        private void LayerDown_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null || Project.SelectedLayer <= 0) return;
            int pold = Project.SelectedLayer;
            int pnew = Project.SelectedLayer - 1;

            SwitchLayerAction ac = new SwitchLayerAction();
            ac.PNew = pnew;
            ac.POld = pold;
            ac.Redo(Project);
            Project.PushUndo(this, ac);
        }

        private void LayerUp_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null || Project.SelectedLayer >= Project.Layers.Count - 1) return;
            int pold = Project.SelectedLayer;
            int pnew = Project.SelectedLayer + 1;

            SwitchLayerAction ac = new SwitchLayerAction();
            ac.PNew = pnew;
            ac.POld = pold;
            ac.Redo(Project);
            Project.PushUndo(this, ac);
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;
            Layer lay = Project.Layers[Project.SelectedLayer];
            RenameWindow rn = new RenameWindow(lay.Name);
            rn.ShowDialog();

            if(rn.Okay)
            {

                RenameAction ra = new RenameAction();
                ra.Oldname = lay.Name;
                ra.Newname = rn.Oldname;
                ra.Layer = lay;
                Project.PushUndo(this, ra);

                lay.Name = rn.Oldname;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;
            if (Project.Layers.Count <= 1) return;

            Layer lay = Project.Layers[Project.SelectedLayer];

            DeleteAction da = new DeleteAction();
            da.Layer = lay;
            da.Position = Project.SelectedLayer;
            Project.PushUndo(this, da);

            Project.Layers.RemoveAt(Project.SelectedLayer);
            
            if(Project.SelectedLayer > 0)
            {
                Project.SelectedLayer--;
            }

            for (int i = 0; i < Project.Width * Project.Height; i++)
            {
                Project.PixelCache.Add(i);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project == null) return;

            if (Project.PATH == null)
            {
                Save_As_Executed(sender, e);
            }
            else
            {
                Project.Save(Project.PATH);

                Xceed.Wpf.Toolkit.MessageBox.Show("The Projects has been saved to" + Environment.NewLine + "> " + Project.PATH, "Save");
            }
        }

        private void Save_As_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project == null) return;

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "DIY Project File(*.diy)|*.diy";
            sfd.Title = "Save File...";
            
            sfd.FileOk += (sender, e) => {
                Project.Save(sfd.FileName);
                Project.PATH = sfd.FileName;
            };

            if(sfd.ShowDialog() == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("The Projects has been saved to" + Environment.NewLine + "> " + Project.PATH, "Save");
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "DIY Project File(*.diy)|*.diy";
            ofd.Title = "Open File...";

            ofd.FileOk += (sender, e) => {
                Project = new DIYProject();

                Project.Open(ofd.FileName);
                Project.PATH = ofd.FileName;

                opglDraw.Height = Project.Height;
                opglDraw.Width = Project.Width;
                contentZoomBox.FitToBounds();
            };

            ofd.ShowDialog();
        }

        private void Import_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Supported Format (*.bmp;*.gif;*.jpeg;*.jpg;*.png)|*.bmp;*.gif;*.jpeg;*.jpg;*.png";
            ofd.Title = "Import Image...";
            
            ofd.FileOk += (sender, e) => {
                Bitmap bmp = new Bitmap(ofd.FileName);
                ImageLayer il = null;
                if(Project == null)
                {
                    Project = new DIYProject(bmp.Width, bmp.Height);
                    il = (ImageLayer)Project.Layers[0];
                }
                else
                {
                    il = new ImageLayer(bmp.Width, bmp.Height);
                    Project.Layers.Add(il);
                    for (int i = 0; i < Project.Width * Project.Height; i++)
                    {
                        Project.PixelCache.Add(i);
                    }
                }

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        System.Drawing.Color c = bmp.GetPixel(x, y);
                        il.Img.SetPixel(x, y, new DIYColor(c.A, c.R, c.G, c.B), false);
                    }
                }
                bmp.Dispose();

                opglDraw.Height = Project.Height;
                opglDraw.Width = Project.Width;
                contentZoomBox.FitToBounds();
            };

            ofd.ShowDialog();
        }

        private void Export_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Project == null) return;

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "BMP|*.bmp|GIF|*.gif|JPEG|*.jpg|PNG|*.png";
            sfd.Title = "Export Image...";

            sfd.FileOk += (sender, e) =>
            {
                DirectBitmap bmp = new DirectBitmap(Project.Width, Project.Height);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Project.DrawPixel(x, y, (x0, y0, c) => bmp.SetPixel(x, y, c, false), false);
                    }
                }
                bmp.Bitmap.Save(sfd.FileName);
                bmp.Dispose();
            };
            if(sfd.ShowDialog() == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("The Projects has been exported to" + Environment.NewLine + "> " + sfd.FileName, "Save");
            }
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            if (Project == null) return;
            if(Project.Layers[Project.SelectedLayer] is ImageLayer)
            {
                ImageLayer il = (ImageLayer)Project.Layers[Project.SelectedLayer];
                Filter.Filter filter = (Filter.Filter) Activator.CreateInstance(filterList[((MenuItem)sender).Name]);
                FilterWindow fw = new FilterWindow(filter, il);
                if(fw.ShowDialog() == true)
                {
                    ImageAction ia = new ImageAction("Filter: " + filter.Name);
                    ia.Layer = Project.SelectedLayer;
                    ia.Old = il.Img;
                    il.Img = fw.Filter.CalculateFilter(il.Img);
                    ia.New = il.Img;

                    for (int i = 0; i < Project.Width * Project.Height; i++)
                    {
                        ia.ChangedPixels.Add(i);
                        Project.PixelCache.Add(i);
                    }
                    Project.PushUndo(this, ia);
                }
            }
        }
    }
}

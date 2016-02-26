using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Timers;
using System;
using System.ComponentModel;

namespace DynamicGeometry
{
    /// <summary>
    /// Trạng thái thanh Status
    /// </summary>
    public enum StatusBarState
    {
        Normal, Error, Inprogress, Warning
    }
    public class StatusBar : Grid
    {
        public StatusBarState State { get; set; }
        private NotifycationLabel statusText { get; set; }
        private NotifycationLabel hintText { get; set; }
        private NotifycationLabel coorText { get; set; }
        private NotifycationLabel scaleText { get; set; }

        private StatusButton orthoButton { get; set; }
        private StatusButton showGridButton { get; set; }
        private StatusButton snapButton { get; set; }
        private ComboBox unitComboBox { get; set; }
        
        public ProgressBar ProgressBar { get; set; }
        public Border border = new Border() 
        { 
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        private DynamicGeometry.DrawingHost _drawingHost;
        public DynamicGeometry.DrawingHost DrawingHost 
        {
            get { return _drawingHost; }
            set 
            {
                _drawingHost = value;
                NotifyChanged("DrawingHost");
            }
        }
        public StatusBar() { }
        public StatusBar(DynamicGeometry.DrawingHost drawingHost)
        {
            DrawingHost = drawingHost;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;

            border.MouseLeftButtonDown += border_MouseLeftButtonDown;
            Grid g = new Grid();
            //g.ShowGridLines = true;
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(0.3, GridUnitType.Star);

            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(0.2, GridUnitType.Star);

            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(0.3, GridUnitType.Star);

            ColumnDefinition c4 = new ColumnDefinition();
            c4.Width = new GridLength(0.2, GridUnitType.Star);

            ColumnDefinition c5 = new ColumnDefinition();
            c5.Width = new GridLength(50, GridUnitType.Pixel);

            ColumnDefinition c6 = new ColumnDefinition();
            c6.Width = new GridLength(50, GridUnitType.Pixel);

            ColumnDefinition c7 = new ColumnDefinition();
            c7.Width = new GridLength(50, GridUnitType.Pixel);

            ColumnDefinition c8 = new ColumnDefinition();
            c8.Width = new GridLength(50, GridUnitType.Pixel);
            ColumnDefinition c9 = new ColumnDefinition();
            c8.Width = new GridLength(50, GridUnitType.Pixel);

            g.ColumnDefinitions.Add(c1); // Cột chứa Status của phần mềm
            g.ColumnDefinitions.Add(c2); // Cột chứa thông tin khi Mouse Over trên Drawing
            g.ColumnDefinitions.Add(c3);
            g.ColumnDefinitions.Add(c4);
            g.ColumnDefinitions.Add(c5);
            g.ColumnDefinitions.Add(c6);
            g.ColumnDefinitions.Add(c7);
            g.ColumnDefinitions.Add(c8);

            statusText = new NotifycationLabel();
            Grid.SetColumn(statusText, 0);
            g.Children.Add(statusText);

            ProgressBar = new ProgressBar();
            Grid.SetColumn(ProgressBar, 1);
            ProgressBar.Maximum = 100;
            ProgressBar.Minimum = 0;
            ProgressBar.Width = 100;
            ProgressBar.Margin = new Thickness(5,3,5,3);
            ProgressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            g.Children.Add(ProgressBar);

            hintText = new NotifycationLabel();
            hintText.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(hintText, 2);
            g.Children.Add(hintText);

            coorText = new NotifycationLabel();
            coorText.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Grid.SetColumn(coorText, 3);
            g.Children.Add(coorText);

            scaleText = new NotifycationLabel() { Text = ""};
            Grid.SetColumn(scaleText, 4);
            g.Children.Add(scaleText);


            orthoButton = new StatusButton() { Content = "ORTHO"};
            orthoButton.ToolTip = (bool)orthoButton.IsChecked ? "Disable Ortho mode" : "Enable Ortho mode";
            Grid.SetColumn(orthoButton, 5);
            g.Children.Add(orthoButton);

            showGridButton = new StatusButton() { Content = "GRID"};
            showGridButton.ToolTip = (bool)showGridButton.IsChecked ? "Hide Grid" : "Show Grid";
            showGridButton.Checked += showGridButton_Checked;
            showGridButton.Unchecked += showGridButton_Unchecked;
            Grid.SetColumn(showGridButton, 6);
            g.Children.Add(showGridButton);

            snapButton = new StatusButton() { Content = "SNAP"};
            snapButton.ToolTip = (bool)snapButton.IsChecked ? "Disable Snap mode" : "Enable Snap mode";
            snapButton.Checked += snapButton_Checked;
            snapButton.Unchecked += snapButton_Unchecked;
            Grid.SetColumn(snapButton, 7);
            g.Children.Add(snapButton);

            unitComboBox = new ComboBox();
            unitComboBox.SelectionChanged += unitComboBox_SelectionChanged;
            ComboBoxItem mmUnit = new ComboBoxItem() { Content = "mm", ToolTip = "Set mm unit for drawing"};
            unitComboBox.Items.Add(mmUnit);
            ComboBoxItem cmUnit = new ComboBoxItem() { Content = "cm", ToolTip = "Set cm unit for drawing" };
            unitComboBox.Items.Add(cmUnit);
            ComboBoxItem mUnit = new ComboBoxItem() { Content = "m", ToolTip = "Set m unit for drawing" };
            unitComboBox.Items.Add(mUnit);
            unitComboBox.SelectedIndex = 1;
            Grid.SetColumn(unitComboBox, 8);
            g.Children.Add(unitComboBox);
            
            border.Child = g;
            this.Children.Add(border);

            TestProgressBar();
        }

        private void unitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.HintText = this.RenderSize.ToString();
        }

        private void snapButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Instance.EnableSnapToPoint = false;
        }

        private void snapButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Instance.EnableSnapToPoint = true;
        }

        private void showGridButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (DrawingHost.CurrentDrawing != null)
                DrawingHost.CurrentDrawing.CoordinateGrid.Visible = false;
            else
                this.HintText = "Error!";
        }
        public void RaiseDrawingChanged(DynamicGeometry.DrawingHost drawingHost)
        {
            this.DrawingHost = drawingHost;
        }
        private void showGridButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DrawingHost.CurrentDrawing != null)
                DrawingHost.CurrentDrawing.CoordinateGrid.Visible = true;
            else
                this.HintText = "Error!";
        }



        public void UpdateStatusBar()
        {
            showGridButton.IsChecked = DrawingHost.CurrentDrawing.CoordinateGrid.Visible;
            showGridButton.ToolTip = (bool)showGridButton.IsChecked ? "Hide Grid" : "Show Grid";
        }

        #region tests
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ProgressBarValue += 5;
        }

        public void TestProgressBar()
        {
            ProgressBar.Value = 0;
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            this.StatusText = "Loading...";
        }
        #endregion

        #region Public Methods
        //public void Raise
        #endregion
        /// <summary>
        /// Khi Mouse Over các đối tượng trên Drawing, hiển thị thông tin đối tượng
        /// </summary>
        public string HintText
        {
            get
            {
                return hintText.Text;
            }
            set
            {
                hintText.Text = value;
                NotifyChanged("HintText");
            }
        }

        public string ScaleText
        {
            get
            {
                return scaleText.Text;
            }
            set
            {
                scaleText.Text = value;
                NotifyChanged("ScaleText");
            }
        }

        public string StatusText
        {
            get
            {
                return statusText.Text;
            }
            set
            {
                statusText.Text = value;
            }
        }
        public string CoordinatesText
        {
            get { return coorText.Text; }
            set { coorText.Text = value; }
        }

        public double ProgressBarValue
        {
            get
            {
                return ProgressBar.Value;
            }
            set
            {
                ProgressBar.Value = value;
                if (value == 100)
                {
                    this.StatusText = "Actions completed";
                }
                UpdateProgressBar();
            }
        }

        public void UpdateProgressBar()
        {
            ProgressBar.Visibility = ProgressBarValue > 0 && ProgressBarValue < 100 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

#if !SILVERLIGHT
        new 
#endif
        public bool IsVisible
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
            set
            {
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                MessageBox.Show("Context Menu");
            }
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; //CC
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace DynamicGeometry
{
    public class DrawingHost : Grid
    {
        public event EventHandler ReadyForInteraction;
        public event EventHandler<UnhandledExceptionNotificationEventArgs> UnhandledException = delegate { };

        public Drawing CurrentDrawing
        {
            get
            {
                return this.DrawingControl.Drawing;
            }
        }

        public Ribbon Ribbon { get; set; }
        public DrawingControl DrawingControl { get; set; }
        public PropertyGrid PropertyGrid { get; set; }
        public StatusBar StatusBar { get; set; }
        public FigureExplorer FigureExplorer { get; set; }

        public PropertyGridHost propertyGridScrollViewer;

        public Command CommandToggleGrid { get; set; }
        public Command CommandToggleOrtho { get; set; }
        public Command CommandToggleSnapToGrid { get; set; }
        public Command CommandToggleSnapToPoint { get; set; }
        public Command CommandToggleLabelNewPoints { get; set; }
        public Command CommandTogglePolar { get; set; }
        public Command CommandToggleSnapToCenter { get; set; }
        public Command CommandShowFigureExplorer { get; set; }

        
        public DrawingHost()
        {
            Behavior.NewBehaviorCreated += Behavior_NewBehaviorCreated;
            Behavior.BehaviorDeleted += Behavior_BehaviorDeleted;
            SetupLayout();
        }

        protected virtual void SetupLayout()
        {
            this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            this.RowDefinitions.Add(new RowDefinition());
            this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            CreateRibbon();
            CreateCanvas();
            CreatePropertyGrid();
            
            CreateFigureExplorer();

            this.Children.Add(Ribbon);
            this.Children.Add(DrawingControl);
            this.Children.Add(propertyGridScrollViewer);
            
            
            this.Children.Add(FigureExplorer);

            
            FigureExplorer.Visible = Settings.Instance.ShowFigureExplorer;

            Grid.SetColumnSpan(Ribbon, 2);
            Grid.SetColumn(FigureExplorer, 1);
            Grid.SetRow(FigureExplorer, 1);
            Grid.SetRow(DrawingControl, 1);
            Grid.SetRow(propertyGridScrollViewer, 1);
            //CreateStatusBar();
            //Grid.SetRow(StatusBar, 2);
            //this.Children.Add(StatusBar);

            CommandToggleGrid = new Command(ToggleGrid, CartesianGrid.GetIcon(), "Grid", BehaviorCategories.Coordinates);
            CommandToggleOrtho = new Command(ToggleOrtho, new CheckBox(), "Ortho", BehaviorCategories.Selection);
            CommandToggleSnapToGrid = new Command(ToggleSnapToGrid, new CheckBox(), "Snap to grid", BehaviorCategories.Selection);
            CommandToggleSnapToPoint = new Command(ToggleSnapToPoint, new CheckBox(), "Snap to point", BehaviorCategories.Selection);
            CommandToggleLabelNewPoints = new Command(ToggleLabelNewPoints, new CheckBox(), "Label New Points", BehaviorCategories.Points);
            CommandTogglePolar = new Command(TogglePolar, new CheckBox(), "Polar", BehaviorCategories.Selection);
            CommandToggleSnapToCenter = new Command(ToggleSnapToCenter, new CheckBox(), "Snap to Center", BehaviorCategories.Selection);
            CommandShowFigureExplorer = new Command(ToggleFigureExplorer, new CheckBox() {IsChecked = FigureExplorer.Visible }, "Figure List", BehaviorCategories.Drawing);
        }
        
        protected void CreateFigureExplorer()
        {
            FigureExplorer = new FigureExplorer()
            {
                MinWidth = 200,
                MaxWidth = 400
            };
            FigureExplorer.SelectionChanged += FigureExplorer_SelectionChanged;
        }

        bool guard = false; // to prevent reentrancy
        void FigureExplorer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (guard)
            {
                return;
            }
            
            guard = true;
            
            foreach (var deselected in e.RemovedItems)
            {
                IFigure deselectedFigure = deselected as IFigure;
                if (deselectedFigure != null)
                {
                    deselectedFigure.Selected = false;
                }
            }

            foreach (var selected in e.AddedItems)
            {
                IFigure selectedFigure = selected as IFigure;
                if (selectedFigure != null)
                {
                    selectedFigure.Selected = true;
                }
            }

            CurrentDrawing.RaiseSelectionChanged(CurrentDrawing.GetSelectedFigures());
            
            guard = false;
        }

        void drawing_SelectionChanged(object sender, Drawing.SelectionChangedEventArgs e)
        {
            SyncFigureExplorerSelection();
            SyncFigureItems();
        }

        private void SyncFigureExplorerSelection()
        {
            if (guard)
            {
                return;
            }
            guard = true;

            // Temporary Solution.  This causes figure's name change to show in FigureExplorer. - D.H.
            // Same temporary solution is used in ToggleFigureExplorer.
            if (FigureExplorer.Visible)
            {
                FigureExplorer.ItemsSource = null;

                List<IFigure> figureIncludes = new List<IFigure>();
                foreach (IFigure f in CurrentDrawing.Figures)
                {
                    if (!(f is IPoint))
                    {
                        figureIncludes.Add(f);
                    }

                }
                FigureExplorer.ItemsSource = figureIncludes;
            }
            // End Temporary Solution
            FigureExplorer.SelectedItem = null;

            foreach (var selectedFigure in CurrentDrawing.GetSelectedFigures())
            {
                FigureExplorer.SelectedItems.Add(selectedFigure);
            }
            guard = false;
        }

        protected virtual void CreateStatusBar()
        {
            StatusBar = new StatusBar(this);

            //StatusBar.Drawing = this.CurrentDrawing;
            StatusBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            StatusBar.VerticalAlignment = VerticalAlignment.Bottom;
            Canvas.SetZIndex(StatusBar, (int)ZOrder.StatusBar);
        }

        // CC
        public void SyncFigureItems()
        {
            FigureItems.ItemsSource = null;

            List<IFigure> figureIncludes = new List<IFigure>();
            if (CurrentDrawing != null)
            {
                if (CurrentDrawing.Figures != null)
                {
                    foreach (IFigure f in CurrentDrawing.Figures)
                    {
                        if (!(f is IPoint))
                        {
                            figureIncludes.Add(f);
                        }
                    }
                }
            }

            FigureItems.ItemsSource = figureIncludes;
            FigureItems.DataContext = figureIncludes;
        }

        public ComboBox FigureItems { get; set; }
        PropertyGrid pg;
        protected void CreatePropertyGrid()
        {
            propertyGridScrollViewer = new PropertyGridHost();

            //
            Grid st = new Grid();
            st.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            st.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            st.RowDefinitions.Add(new RowDefinition());

            st.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            st.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            PropertyGrid = new PropertyGrid();
            MEFHost.Instance.SatisfyImportsOnce(PropertyGrid);

            TitleLabel title = new TitleLabel() 
            { 
                Text = "Properties",
                //FontSize = 12, 
                //FontWeight = FontWeights.Bold,
                //Background = Brushes.White,
                Foreground = Brushes.White,
                Margin = new Thickness(5, 2, 5, 2),
            };

            FigureItems = new ComboBox();
            FigureItems.ItemsSource = null;
            List<IFigure> figureIncludes = new List<IFigure>();
            if (CurrentDrawing != null)
            {
                if (CurrentDrawing.Figures != null)
                {
                    foreach (IFigure f in CurrentDrawing.Figures)
                    {
                        if (!(f is IPoint))
                        {
                            figureIncludes.Add(f);
                        }
                    }
                }
            }

            FigureItems.ItemsSource = figureIncludes;
            FigureItems.SelectionChanged += FigureItems_SelectionChanged;

            pg = new PropertyGrid() 
            {
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            };
            //pg.MinWidth = 200;
            //pg.Background = Brushes.Red;
            
           
            st.Children.Add(title);
            //st.Children.Add(FigureItems);

            st.Children.Add(pg);
            //
            Grid.SetRow(title, 0);
            Grid.SetRow(FigureItems, 1);
            Grid.SetRow(pg, 2);

            propertyGridScrollViewer.Children.Add(st);

            PropertyGrid.VisibilityChanged += PropertyGrid_VisibilityChanged;

            Canvas.SetZIndex(propertyGridScrollViewer, (int)ZOrder.StatusBar);

            PropertyGrid.ValueDiscoveryStrategy = new ExcludeByDefaultValueDiscoveryStrategy();

            propertyGridScrollViewer.Visibility = Visibility.Hidden;
        }

        private void FigureItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = FigureItems.SelectedItem;

                IFigure selectedFigure = selected as IFigure;
                if (selectedFigure != null)
                {
                    selectedFigure.Selected = true;
                }

                pg.Show(selectedFigure, CurrentDrawing.ActionManager);

                //FigureItems.SelectedValue = selectedFigure;

            CurrentDrawing.RaiseSelectionChanged(CurrentDrawing.GetSelectedFigures());
        }

        private void PropertyGrid_VisibilityChanged(object sender, EventArgs e)
        {
            propertyGridScrollViewer.Visibility = System.Windows.Visibility.Hidden;  //PropertyGrid.Visibility; //CC
        }

        public void ToggleLabelNewPoints()
        {
            Settings.Instance.AutoLabelPoints = !Settings.Instance.AutoLabelPoints;
        }

        public void ToggleGrid()
        {
            CurrentDrawing.CoordinateGrid.Visible = !CurrentDrawing.CoordinateGrid.Visible;
        }

        public void ToggleFigureExplorer()
        {
            if (FigureExplorer.Visible)
            {
                FigureExplorer.Visible = false;
            }
            else
            {
                // Temporary Solution.  This causes figure's name change to should show in FigureExplorer. - D.H.
                // Same temporary solution is used in SyncFigureExplorerSelection().
                FigureExplorer.ItemsSource = null;

                List<IFigure> figureIncludes = new List<IFigure>();
                foreach (IFigure f in CurrentDrawing.Figures)
                {
                    if (!(f is IPoint))
                    {
                        figureIncludes.Add(f);
                    }

                }
                FigureExplorer.ItemsSource = figureIncludes;

                //FigureExplorer.ItemsSource = CurrentDrawing.Figures;

                FigureExplorer.Visible = true;
            }
        }

        public void ToggleOrtho()
        {
            Settings.Instance.EnableOrtho = !Settings.Instance.EnableOrtho;
            Settings.Instance.EnablePolar = false;
        }

        public void TogglePolar()
        {
            Settings.Instance.EnablePolar = !Settings.Instance.EnablePolar;
            Settings.Instance.EnableOrtho = false;
        }

        public void ToggleSnapToGrid()
        {
            Settings.Instance.EnableSnapToGrid = !Settings.Instance.EnableSnapToGrid;
        }

        public void ToggleSnapToPoint()
        {
            Settings.Instance.EnableSnapToPoint = !Settings.Instance.EnableSnapToPoint;
        }

        public void ToggleSnapToCenter()
        {
            Settings.Instance.EnableSnapToCenter = !Settings.Instance.EnableSnapToCenter;
        }

        protected void CreateCanvas()
        {
            DrawingControl = new DrawingControl();
            
            DrawingControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            DrawingControl.VerticalAlignment = VerticalAlignment.Stretch;
            DrawingControl.ReadyForInteraction += RaiseReadyForInteraction;
            DrawingControl.DrawingAttach += DrawingControl_DrawingAttach;
            DrawingControl.DrawingDetach += DrawingControl_DrawingDetach;
        }

        protected void RaiseReadyForInteraction(object sender, EventArgs e)
        {
            if (ReadyForInteraction != null)
            {
                ReadyForInteraction(sender, e);
            }
        }

        public virtual void RaiseCommandExecuted(Command command)
        {
            // Do nothing when a command is executed but allow this to be overridden.
        }

        protected virtual void DrawingControl_DrawingAttach(Drawing drawing)
        {
            drawing.Status += mCurrentDrawing_Status;
            drawing.SelectionChanged += mCurrentDrawing_SelectionChanged;
            drawing.BehaviorChanged += mCurrentDrawing_BehaviorChanged;
            drawing.DisplayProperties += mCurrentDrawing_DisplayProperties;
            drawing.UnhandledException += UnhandledException;
            drawing.SelectionChanged += drawing_SelectionChanged;

            List<IFigure> figureIncludes = new List<IFigure>();
            foreach (IFigure f in CurrentDrawing.Figures)
            {
                if (!(f is IPoint))
                {
                    figureIncludes.Add(f);
                }

            }
            FigureExplorer.ItemsSource = figureIncludes;

            SyncFigureItems(); //CC

            //FigureExplorer.ItemsSource = drawing.Figures;
        }

        protected virtual void DrawingControl_DrawingDetach(Drawing drawing)
        {
            drawing.Status -= mCurrentDrawing_Status;
            drawing.SelectionChanged -= mCurrentDrawing_SelectionChanged;
            drawing.BehaviorChanged -= mCurrentDrawing_BehaviorChanged;
            drawing.DisplayProperties -= mCurrentDrawing_DisplayProperties;
            drawing.UnhandledException -= UnhandledException;
            drawing.SelectionChanged -= drawing_SelectionChanged;
            FigureExplorer.ItemsSource = null;
            FigureItems.ItemsSource = null; //CC
            ShowProperties(null);
        }

        public BehaviorToolButton AddToolButton(Behavior behavior)
        {
            return Ribbon.AddToolButton(behavior);
        }

        public CommandToolButton AddToolbarButton(Command command)
        {
            return Ribbon.AddToolButton(command);
        }

        public void RemoveToolButton(Behavior behavior)
        {
            Ribbon.RemoveToolButton(behavior);
        }

        protected virtual void Behavior_NewBehaviorCreated(Behavior behavior)
        {
            AddToolButton(behavior);
        }

        protected virtual void Behavior_BehaviorDeleted(Behavior behavior)
        {
            RemoveToolButton(behavior);
        }

        public void CreateRibbon()
        {
            Ribbon = new Ribbon(this);

        }

        public void AddBehaviors(Assembly assembly)
        {
            var behaviors = Behavior.LoadBehaviors(assembly);
            foreach (var behavior in behaviors)
            {
                AddToolButton(behavior);
            }
        }

        public void Clear()
        {
            this.DrawingControl.Clear();
        }

        protected virtual void mCurrentDrawing_DisplayProperties(object sender, Drawing.DisplayPropertiesEventArgs e)
        {
            ShowProperties(e.Object);
        }

        protected virtual void mCurrentDrawing_BehaviorChanged(Behavior newBehavior)
        {
            Ribbon.SelectBehavior(newBehavior);
            var help = newBehavior.HintText;
            if (!help.IsEmpty())
            {
                ShowHint(help);
            }
            ShowProperties(newBehavior.PropertyBag);
        }

        protected virtual void mCurrentDrawing_SelectionChanged(object sender, Drawing.SelectionChangedEventArgs e)
        {
            ShowSelectionProperties();
        }

        private void mCurrentDrawing_Status(string status)
        {
                ShowHint(status);
        }

        public virtual void ShowHint(string text)
        {
            if (!Settings.Instance.HideHints)
            {
                //StatusBar.HintText = text;
                //StatusBar.Visibility = Visibility.Visible;
            }
        }

        protected virtual void ShowSelectionProperties()
        {
            //var selection = CurrentDrawing.GetSelectedFigures().ToArray();
            //if (selection.Length == 1)
            //{
            //    ShowProperties(selection[0]);
            //}
            //else if (selection.Length > 1)
            //{
            //    ShowProperties(selection);
            //}
            //else
            //{
            //    ShowProperties(CurrentDrawing);
            //}
        }

        public virtual void ShowProperties(object selection)
        {
            try
            {
                PropertyGrid.Show(selection, CurrentDrawing.ActionManager);

                pg.Show(selection, CurrentDrawing.ActionManager);// = selection; // CC
            }
            catch (Exception ex)
            {
                CurrentDrawing.RaiseError(this, ex);
            }
        }

        public void ShowProperties(IEnumerable<object> selection)
        {
            try
            {
                PropertyGrid.Show(selection, CurrentDrawing.ActionManager);

                //pg.SelectedObjects = selection.ToArray(); // CC
            }
            catch (Exception ex)
            {
                CurrentDrawing.RaiseError(this, ex);
            }
        }
    }
}

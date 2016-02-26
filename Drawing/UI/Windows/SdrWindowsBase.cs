using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicGeometry;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Data;

namespace Shopdrawing.Windows
{
    [TemplatePart(Name = "PART_TITLEBAR", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_MINIMIZE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MAXIMIZE_RESTORE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LEFT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_RIGHT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_TOP_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_BOTTOM_BORDER", Type = typeof(UIElement))]

    public class SdrWindowsBase : Window
    {
        #region private members
        // Private members
        private static DependencyPropertyKey MaximizedPropertyKey = DependencyProperty.RegisterReadOnly("Maximized",
            typeof(bool),
            typeof(SdrWindowsBase),
            new PropertyMetadata(false));
        #endregion
        #region Properties
        public Drawing Drawing { get; set; }

        public string TestString { get; set; }

        #endregion

        #region Constructors

        public SdrWindowsBase()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            //ResizeMode = ResizeMode.NoResize;
            //AllowsTransparency = true;
            SnapsToDevicePixels = true;
            ShowInTaskbar = false;

        }
        public SdrWindowsBase(Window owner)
        {
            this.Owner = owner;
        }
        public SdrWindowsBase(Drawing drawing)
            : this()
        {
            Drawing = drawing;
        }
        #endregion

        #region Tests
        public void Test1()
        {
            MessageBox.Show(this.Drawing.Figures.Count.ToString() + TestString);
        }
        #endregion


        #region Overrides

        #endregion


        /// <summary>
        /// Maximized property
        /// </summary>
        public static DependencyProperty MaximizedProperty = MaximizedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CustomWindow"/> is maximized
        /// </summary>
        /// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
        public bool Maximized
        {
            get
            {
                return (bool)GetValue(MaximizedProperty);
            }
            private set
            {
                if (value)
                {
                    UpdateRestoreBounds();

                    // Maximize hides taskbar, hence workaround
                    Top = Left = 0;
                    Height = SystemParameters.MaximizedPrimaryScreenHeight - SystemParameters.BorderWidth * 2;
                    Width = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.BorderWidth * 2;

                }
                else
                {
                    ApplyRestoreBounds();
                }

                Visibility sizerVisibility = value ? Visibility.Hidden : Visibility.Visible;
                UpdateBorderVisibility(RightBorder, sizerVisibility);
                UpdateBorderVisibility(TopBorder, sizerVisibility);
                UpdateBorderVisibility(BottomBorder, sizerVisibility);
                UpdateBorderVisibility(LeftBorder, sizerVisibility);

                SetValue(MaximizedPropertyKey, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
        }
        /// <summary>
        /// Creates the command bindings
        /// </summary>
        private void CreateCommandBindings()
        {
            // Command binding for close button
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Close,
                (a, b) => { Close(); }));

            // Command binding for minimize button
            CommandBindings.Add(new CommandBinding(
                MinimizeCommand,
                (a, b) => { WindowState = WindowState.Minimized; }));

            // Command binding for maximize / restore button
            CommandBindings.Add(new CommandBinding(MaximizeRestoreCommand,
                (a, b) =>
                {
                    Maximized = !Maximized;
                }));
        }

        /// <summary>
        /// Attaches to visual tree to the template
        /// </summary>
        private void AttachToVisualTree()
        {
            AttachCloseButton();
            AttachMinimizeButton();
            AttachMaximizeRestoreButton();
            AttachTitleBar();
            AttachBorders();
        }

        /// <summary>
        /// Attaches the close button
        /// </summary>
        private void AttachCloseButton()
        {
            if (CloseButton != null)
            {
                CloseButton.Command = null;
            }

            Button closeButton = GetChildControl<Button>("PART_CLOSE");
            if (closeButton != null)
            {
                closeButton.Command = ApplicationCommands.Close;
                CloseButton = closeButton;
            }
        }

        /// <summary>
        /// Attaches the minimize button
        /// </summary>
        private void AttachMinimizeButton()
        {
            if (MinimizeButton != null)
            {
                MinimizeButton.Command = null;
            }

            Button minimizeButton = GetChildControl<Button>("PART_MINIMIZE");
            if (minimizeButton != null)
            {
                minimizeButton.Command = MinimizeCommand;
                MinimizeButton = minimizeButton;
            }
        }

        /// <summary>
        /// Attaches the maximize restore button
        /// </summary>
        private void AttachMaximizeRestoreButton()
        {
            if (MaximizeRestoreButton != null)
            {
                MaximizeRestoreButton.Command = null;
            }

            Button maximizeRestoreButton = GetChildControl<Button>("PART_MAXIMIZE_RESTORE");
            if (maximizeRestoreButton != null)
            {
                maximizeRestoreButton.Command = MaximizeRestoreCommand;
                MaximizeRestoreButton = maximizeRestoreButton;
            }
        }

        /// <summary>
        /// Attaches the title bar to visual tree
        /// </summary>
        private void AttachTitleBar()
        {
            if (TitleBar != null)
            {
                TitleBar.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnTitlebarClick));
            }

            UIElement titleBar = GetChildControl<UIElement>("PART_TITLEBAR");
            if (titleBar != null)
            {
                TitleBar = titleBar;
                titleBar.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnTitlebarClick));
            }
        }

        /// <summary>
        /// Called when titlebar is clicked or double clicked
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseLeftButtonEventArgs"/> instance containing the event data</param>
        private void OnTitlebarClick(object source, MouseButtonEventArgs args)
        {
            switch (args.ClickCount)
            {
                case 1:
                    if (!Maximized)
                    {
                        DragMove();
                    }
                    break;
                case 2:
                    Maximized = !Maximized;
                    break;
            }
        }

        /// <summary>
        /// Attaches the borders to the visual tree
        /// </summary>
        private void AttachBorders()
        {
            AttachLeftBorder();
            AttachRightBorder();
            AttachTopBorder();
            AttachBottomBorder();
        }

        /// <summary>
        /// Attaches the left border to the visual tree
        /// </summary>
        private void AttachLeftBorder()
        {
            if (LeftBorder != null)
            {
                LeftBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                LeftBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                LeftBorder.MouseMove -= OnLeftBorderMouseMove;
            }

            UIElement leftBorder = GetChildControl<UIElement>("PART_LEFT_BORDER");
            if (leftBorder != null)
            {
                LeftBorder = leftBorder;
                leftBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                leftBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                leftBorder.MouseMove += OnLeftBorderMouseMove;
            }
        }

        /// <summary>
        /// Called when mouse left button is down on a border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data</param>
        private void OnBorderMouseLeftButtonDown(object source, MouseButtonEventArgs args)
        {
            IsResizing = true;
        }

        /// <summary>
        /// Called when mouse left button is up on a border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data</param>
        private void OnBorderMouseLeftButtonUp(object source, MouseButtonEventArgs args)
        {
            IsResizing = false;
            if (source is UIElement)
            {
                (source as UIElement).ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Called when mouse moves over left border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        private void OnLeftBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!LeftBorder.IsMouseCaptured) && IsResizing)
            {
                LeftBorder.CaptureMouse();
            }

            if (IsResizing)
            {
                double position = args.GetPosition(this).X;

                if (System.Math.Abs(position) < 10)
                {
                    return;
                }

                if ((position > 0) && ((Width - position) > MinWidth) && (Width > position))
                {
                    Left += position;
                    Width -= position;
                }
                else if ((position < 0) && (Left > 0))
                {
                    position = (Left + position > 0) ? position : -1 * Left;
                    Width = ActualWidth - position;
                    Left += position;
                }
            }
        }

        /// <summary>
        /// Attaches the right border to the visual tree
        /// </summary>
        private void AttachRightBorder()
        {
            if (RightBorder != null)
            {
                RightBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                RightBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                RightBorder.MouseMove -= OnRightBorderMouseMove;
            }

            UIElement rightBorder = GetChildControl<UIElement>("PART_RIGHT_BORDER");
            if (rightBorder != null)
            {
                RightBorder = rightBorder;
                rightBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                rightBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                rightBorder.MouseMove += OnRightBorderMouseMove;
            }
        }

        /// <summary>
        /// Called when mouse moves over right border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        private void OnRightBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!RightBorder.IsMouseCaptured) && IsResizing)
            {
                RightBorder.CaptureMouse();
            }

            if (IsResizing)
            {
                double position = args.GetPosition(this).X;

                if (System.Math.Abs(position) < 10)
                {
                    return;
                }

                if (position > 0)
                {
                    Width = position;
                }
                else if ((position < 0) && (ActualWidth > MinWidth))
                {
                    position = (ActualWidth + position < MinWidth) ? MinWidth - ActualWidth : position;
                    Width = ActualWidth + position;
                }
            }
        }

        /// <summary>
        /// Attaches the top border to the visual tree
        /// </summary>
        private void AttachTopBorder()
        {
            if (TopBorder != null)
            {
                TopBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                TopBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                TopBorder.MouseMove -= OnRightBorderMouseMove;
            }

            UIElement topBorder = GetChildControl<UIElement>("PART_TOP_BORDER");
            if (topBorder != null)
            {
                TopBorder = topBorder;
                topBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                topBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                topBorder.MouseMove += OnTopBorderMouseMove;
            }
        }

        /// <summary>
        /// Called when mouse moves over top border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        private void OnTopBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!TopBorder.IsMouseCaptured) && IsResizing)
            {
                TopBorder.CaptureMouse();
            }

            if (IsResizing)
            {
                double position = args.GetPosition(this).Y;

                if (System.Math.Abs(position) < 10)
                {
                    return;
                }

                if (position < 0)
                {
                    position = Top + position < 0 ? -Top : position;
                    Top += position;
                    Height = ActualHeight - position;
                }
                else if ((position > 0) && (ActualHeight - position > MinHeight))
                {
                    position = (ActualHeight - position < MinHeight) ? MinHeight - ActualHeight : position;
                    Height = ActualHeight - position;
                    Top += position;
                }
            }
        }

        /// <summary>
        /// Attaches the bottom border to the visual tree
        /// </summary>
        private void AttachBottomBorder()
        {
            if (BottomBorder != null)
            {
                BottomBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                BottomBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                BottomBorder.MouseMove -= OnBottomBorderMouseMove;
            }

            UIElement bottomBorder = GetChildControl<UIElement>("PART_BOTTOM_BORDER");
            if (bottomBorder != null)
            {
                BottomBorder = bottomBorder;
                bottomBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
                bottomBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
                bottomBorder.MouseMove += OnBottomBorderMouseMove;
            }
        }

        /// <summary>
        /// Called when mouse moves over bottom border
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        private void OnBottomBorderMouseMove(object source, MouseEventArgs args)
        {
            if ((!BottomBorder.IsMouseCaptured) && IsResizing)
            {
                BottomBorder.CaptureMouse();
            }

            if (IsResizing)
            {
                double position = args.GetPosition(this).Y - ActualHeight;

                if (System.Math.Abs(position) < 10)
                {
                    return;
                }

                if (position > 0)
                {
                    Height = ActualHeight + position;
                }
                else if ((position < 0) && (ActualHeight + position > MinHeight))
                {
                    position = (ActualHeight + position < MinHeight) ? MinHeight - ActualHeight : position;
                    Height = ActualHeight + position;
                }
            }
        }

        /// <summary>
        /// Gets the child control from the template
        /// </summary>
        /// <typeparam name="T">Type of control requested</typeparam>
        /// <param name="controlName">Name of the control</param>
        /// <returns>Control instance if there is one with the specified name; null otherwise</returns>
        private T GetChildControl<T>(string controlName) where T : DependencyObject
        {
            T control = GetTemplateChild(controlName) as T;
            return control;
        }

        /// <summary>
        /// Updates the border visibility.
        /// </summary>
        /// <param name="border">Border</param>
        /// <param name="visibility">Visibility</param>
        private void UpdateBorderVisibility(UIElement border, Visibility visibility)
        {
            if (border != null)
            {
                border.Visibility = visibility;
            }
        }

        /// <summary>
        /// Updates the restore bounds
        /// </summary>
        private void UpdateRestoreBounds()
        {
            RestoreBounds = new Rect(new Point(Left, Top), new Point(Left + ActualWidth, Top + ActualHeight));
        }

        /// <summary>
        /// Applies the restore bounds to the window
        /// </summary>
        private void ApplyRestoreBounds()
        {
            Left = RestoreBounds.Left;
            Top = RestoreBounds.Top;
            Width = RestoreBounds.Width;
            Height = RestoreBounds.Height;
        }

        /// <summary>
        /// Gets the size and location of a window before being either minimized or maximized.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Windows.Rect"/> that specifies the size and location of a window before being either minimized or maximized.</returns>
        private new Rect RestoreBounds { get; set; }

        /// <summary>
        /// Close button
        /// </summary>
        private Button CloseButton { get; set; }

        /// <summary>
        /// Minimize button
        /// </summary>
        private Button MinimizeButton { get; set; }

        /// <summary>
        /// Maximize / restore button
        /// </summary>
        /// <value>The maximize restore button.</value>
        private Button MaximizeRestoreButton { get; set; }

        /// <summary>
        /// Title bar
        /// </summary>
        private UIElement TitleBar { get; set; }

        /// <summary>
        /// Left border
        /// </summary>
        private UIElement LeftBorder { get; set; }

        /// <summary>
        /// Right border
        /// </summary>
        private UIElement RightBorder { get; set; }

        /// <summary>
        /// Top border
        /// </summary>
        private UIElement TopBorder { get; set; }

        /// <summary>
        /// Bottom border
        /// </summary>
        private UIElement BottomBorder { get; set; }

        /// <summary>
        /// Indicates whether window is currently resizing
        /// </summary>
        /// <value>
        /// 	<c>true</c> If window is currently resizing; otherwise, <c>false</c>.
        /// </value>
        private bool IsResizing { get; set; }

        /// <summary>
        /// Minimize Command
        /// </summary>
        private readonly RoutedUICommand MinimizeCommand =
            new RoutedUICommand("Minimize", "Minimize", typeof(SdrWindowsBase));

        /// <summary>
        /// Maximize / Restore command
        /// </summary>
        private readonly RoutedUICommand MaximizeRestoreCommand =
            new RoutedUICommand("MaximizeRestore", "MaximizeRestore", typeof(SdrWindowsBase));
    }
}

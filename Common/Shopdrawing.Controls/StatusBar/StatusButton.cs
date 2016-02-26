//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Windows;
//using System.Windows.Controls.Primitives;
//using System.Windows.Media;
//using System.Windows.Shapes;
//using Shopdrawing.Core.Utilities;
//using Shopdrawing.Wpf;

//namespace Shopdrawing.Controls
//{
//    public class StatusButton : ToggleButton
//    {
//        /// <summary>
//        /// Button Icon
//        /// </summary>
//        public Brush Icon
//        {
//            get 
//            { 
//                return (Brush)GetValue(IconProperty); 
//            }
//            set { SetValue(IconProperty, value); }
//        }

//        /// <summary>
//        /// Button Icon
//        /// </summary>
//        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Brush), typeof(StatusButton), new PropertyMetadata(Brushes.Transparent));

//        /// <summary>Access key to be displayed for the button</summary>
//        public string AccessKey
//        {
//            get { return (string)GetValue(AccessKeyProperty); }
//            set { SetValue(AccessKeyProperty, value); }
//        }

//        /// <summary>Access key to be displayed for the button</summary>
//        public static readonly DependencyProperty AccessKeyProperty = DependencyProperty.Register("AccessKey", typeof(string), typeof(StatusButton), new PropertyMetadata(string.Empty, OnAccessKeyChanged));

//        /// <summary>Fires when the access key changes</summary>
//        /// <param name="d">The dependency object.</param>
//        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
//        private static void OnAccessKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
//        {
//            var button = d as StatusButton;
//            if (button == null) return;
//            button.AccessKeySet = !string.IsNullOrEmpty(args.NewValue.ToString());
//        }

//        /// <summary>Indicates whether a access key has been set</summary>
//        /// <value><c>true</c> if [access key set]; otherwise, <c>false</c>.</value>
//        public bool AccessKeySet
//        {
//            get { return (bool)GetValue(AccessKeySetProperty); }
//            set { SetValue(AccessKeySetProperty, value); }
//        }

//        /// <summary>Indicates whether a page access key has been set</summary>
//        public static readonly DependencyProperty AccessKeySetProperty = DependencyProperty.Register("AccessKeySet", typeof(bool), typeof(StatusButton), new PropertyMetadata(false));

//        ///// <summary>Indicates whether the button is to be rendered as "checked" (often used in Toggle-style actions)</summary>
//        //public bool IsChecked
//        //{
//        //    get { return (bool)GetValue(IsCheckedProperty); }
//        //    set { SetValue(IsCheckedProperty, value); }
//        //}

//        ///// <summary>Indicates whether the button is to be rendered as "checked" (often used in Toggle-style actions)</summary>
//        //public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(StatusButton), new PropertyMetadata(false));

//        /// <summary>Key of a visual XAML resource associated with this action (such as an icon)</summary>
//        public string VisualResourceKey
//        {
//            get { return _visualResourceKey; }
//            set
//            {
//                _visualResourceKey = value;
//                _latestBrush = null;
//                NotifyChanged("VisualResourceKey");
//                NotifyChanged("Visual");
//                NotifyChanged("Brush");
//                NotifyChanged("PopulatedBrush");
//                NotifyChanged("PopulatedVisual");
//            }
//        }

//        private string _visualResourceKey;
//        /// <summary>Key of a visual XAML resource associated with this action (such as an icon)</summary>
//        public string BrushResourceKey
//        {
//            get { return _brushResourceKey; }
//            set
//            {
//                _brushResourceKey = value;
//                _latestBrush = null;
//                NotifyChanged("BrushResourceKey");
//                NotifyChanged("Visual");
//                NotifyChanged("Brush");
//                NotifyChanged("PopulatedBrush");
//                NotifyChanged("PopulatedVisual");
//                NotifyChanged("Image1");
//            }
//        }

//        private string _brushResourceKey;
//        /// <summary>
//        /// Actual visual associated with an action (such as an icon). This visual is set (identified) by the VisualResourceKey property
//        /// </summary>
//        public Visual Visual
//        {
//            get
//            {
//                try
//                {
//                    if (!string.IsNullOrEmpty(VisualResourceKey))
//                        return (Visual)Application.Current.FindResource(VisualResourceKey);
//                    if (!string.IsNullOrEmpty(BrushResourceKey))
//                        return new Rectangle
//                        {
//                            Fill = Application.Current.FindResource(BrushResourceKey) as Brush,
//                            MinHeight = 16,
//                            MinWidth = 16
//                        };
//                    return null;
//                }
//                catch
//                {
//                    return null;
//                }
//            }
//        }

//        /// <summary>
//        /// Like Visual, but when no visual resource is found, it attempts to load a standard icon
//        /// </summary>
//        public Visual PopulatedVisual
//        {
//            get
//            {
//                try
//                {
//                    var visual = Visual;
//                    if (visual == null)
//                    {
//                        var rectangle = new Rectangle
//                        {
//                            MinHeight = 16,
//                            MinWidth = 16,
//                            Fill = Application.Current.FindResource("STRUCTURES.DESIGN-Icon-More") as Brush
//                        };

//                        visual = rectangle;
//                    }
//                    return visual;
//                }
//                catch
//                {
//                    return null;
//                }
//            }
//        }

//        private Brush _latestBrush;
//        private Brush _latestLogoBrush;

//        /// <summary>Tries to find a named XAML resource of type brush and returns it.</summary>
//        /// <param name="resourceName">Name of the resource.</param>
//        /// <returns>Brush or null</returns>
//        /// <remarks>The returned brush is a clone, so it can be manipulated at will without impacting other users of the same brush.</remarks>
//        public Brush GetBrushFromResource(string resourceName)
//        {
//            var resource = Application.Current.FindResource(resourceName);
//            if (resource == null) return null;

//            var brush = resource as Brush;
//            if (brush == null) return null;

//            return brush.Clone();
//        }

//        /// <summary>
//        /// Returns a brush of a brush resource is defined.
//        /// </summary>
//        public Brush Brush
//        {
//            get
//            {
//                if (_latestBrush != null) return _latestBrush;

//                try
//                {
//                    if (!string.IsNullOrEmpty(BrushResourceKey))
//                    {
//                        var brushResources = new Dictionary<object, Brush>();
//                        FrameworkElement resourceSearchContext = this;

//                        ResourceHelper.GetBrushResources(resourceSearchContext, brushResources);

//                        var icon = resourceSearchContext != null ? resourceSearchContext.FindResource(BrushResourceKey) as Brush : Application.Current.FindResource(BrushResourceKey) as Brush;

//                        if (brushResources.Count > 0) // We may have some resources we need to replace
//                            If.Real<DrawingBrush>(icon, drawing => ResourceHelper.ReplaceDynamicDrawingBrushResources(drawing, brushResources));

//                        _latestBrush = icon;
//                        NotifyChanged();
//                    }
//                    return _latestBrush;
//                }
//                catch
//                {
//                    return null;
//                }
//            }
//        }

//        /// <summary>Indicates whether this action has an assigned brush</summary>
//        public bool HasBrush
//        {
//            get { return !string.IsNullOrEmpty(BrushResourceKey); }
//        }

//        /// <summary>
//        /// Returns a brush if defined, otherwise loads a default brush
//        /// </summary>
//        public Brush PopulatedBrush
//        {
//            get
//            {
//                try
//                {
//                    if (string.IsNullOrEmpty(BrushResourceKey))
//                    {
//                        _latestBrush = null;
//                        _brushResourceKey = "STRUCTURES.DESIGN-Icon-MissingIcon"; // Must use internal field here, otherwise all kinds of stuff gets triggered!!!
//                    }
//                    return Brush;
//                }
//                catch
//                {
//                    return Brushes.Transparent;
//                }
//            }
//        }


//        /// <summary>Occurs when a property value changes.</summary>
//        public event PropertyChangedEventHandler PropertyChanged;

//        /// <summary>
//        /// Notifies the changed.
//        /// </summary>
//        /// <param name="propertyName">Name of the property.</param>
//        protected virtual void NotifyChanged(string propertyName = "")
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}

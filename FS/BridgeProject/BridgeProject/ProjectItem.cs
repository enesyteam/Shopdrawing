namespace BridgeProject.Model
{
    #region

    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Controls.DragNDrop;
    using System.Linq;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Windows.Shapes;
    using System.Windows;
    using Shopdrawing.Wpf;
    using System.ComponentModel;
    using Shopdrawing.Core.Utilities;
    #endregion

    public enum ItemType
    { 
        Properties,
        References,
        Folder,
        Drawing,
        CalculationSheet,
        Note,
        UnKnown
    }
    /// <summary>
    /// Model for testing
    /// </summary>
    public class ProjectItem : INotifyPropertyChanged
    {
        public ObservableCollection<ProjectItem> NodeList { get; set; }

        ItemType _itemType = ItemType.UnKnown;
        public ItemType ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
        #region Constructors and Destructors

        public ProjectItem()
        {
            Children = new ObservableCollection<ProjectItem>();
        }

        public void OnInsert(int index, object obj)
        {
            DragContent content = obj as DragContent;
            if (content != null)
            {
                foreach (var item in content.Items.Reverse())
                {
                    ProjectItem oldNode = (ProjectItem)item;
                    ProjectItem newNode = new ProjectItem();
                    newNode.Name = string.Format("Copy of {0}", oldNode.Name.Replace(" (Drag Allowed)", string.Empty));
                    //Children.Remove(oldNode);
                    Children.Insert(index, newNode);
                }
            }
            else
            {
                Children.Insert(index, new ProjectItem() { Name = "New node" });
            }
        }

        public bool CanInsertFormat(int index, string format)
        {
            return true;
        }

        public bool CanInsert(int index, object obj)
        {
            return AllowInsert;
        }

        public bool CanDropFormat(string arg)
        {
            return true;
        }

        bool _allowDrop = true;
        public bool AllowDrop
        {
            get { return _allowDrop; }
            set { _allowDrop = value;
            }
        }
        bool _allowDrag = true;
        public bool AllowDrag
        {
            get { return _allowDrag; }
            set
            {
                _allowDrag = value;
            }
        }
        bool _allowInsert = true;
        public bool AllowInsert
        {
            get { return _allowInsert; }
            set
            {
                _allowInsert = value;
            }
        }

        public bool CanDrop(object obj)
        {
            return AllowDrop;
        }

        public void OnDrop(object obj)
        {
            DragContent content = obj as DragContent;
            if (content != null)
            {
                foreach (var item in content.Items.Reverse())
                {
                    ProjectItem oldNode = (ProjectItem)item;
                    ProjectItem newNode = new ProjectItem()
                    { 
                        AllowDrag = oldNode.AllowDrag, 
                        AllowDrop = oldNode.AllowDrop, 
                        AllowInsert = oldNode.AllowInsert,
                    ItemType = oldNode.ItemType};
                    newNode.Name = string.Format("Copy of {0}", oldNode.Name.Replace(" (Drag Allowed)", string.Empty));
                    Children.Add(newNode);
                }
            }
            else
            {
                Children.Add(new ProjectItem() { Name = "New node" });
            }
        }

        public bool CanDrag()
        {
            return AllowDrag;
        }

        public object OnDrag()
        {
            return this;
        }

        #endregion

        #region Public Properties

        public ObservableCollection<ProjectItem> Children { get; set; }

        string _name;
        public string Name 
        {
            get { return _name; }
            set { _name = value;
            NotifyChanged("Name");
            }
        }
        public string IconBrushKey { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Name;
        }
        public Brush Color
        {
            get { return _color; }
            set { _color = value; }
        }
        private Brush _color = Brushes.Gray;
        public List<Brush> Colors { get; set; }
        #endregion


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

        #region Temp
        /// <summary>Key of a visual XAML resource associated with this action (such as an icon)</summary>
        public string VisualResourceKey
        {
            get { return _visualResourceKey; }
            set
            {
                _visualResourceKey = value;
                _latestBrush = null;
                NotifyChanged("VisualResourceKey");
                NotifyChanged("Visual");
                NotifyChanged("Brush");
                NotifyChanged("PopulatedBrush");
                NotifyChanged("PopulatedVisual");
            }
        }
        private string _visualResourceKey;

        /// <summary>Key of a visual XAML resource associated with this action (such as an icon)</summary>
        public string BrushResourceKey
        {
            get 
            {
                switch (ItemType)
                { 
                    case Model.ItemType.UnKnown:
                        return "Icon-Open";
                    case Model.ItemType.Folder:
                        return "Icon-Folder";
                    case Model.ItemType.Properties:
                        return "Icon-Settings";
                    case Model.ItemType.Note:
                        return "Icon-Text";
                    default: return "Icon-Settings";
                }
            }
            //set
            //{
            //    _brushResourceKey = value;
            //    _latestBrush = null;
            //    NotifyChanged("BrushResourceKey");
            //    NotifyChanged("Visual");
            //    NotifyChanged("Brush");
            //    NotifyChanged("PopulatedBrush");
            //    NotifyChanged("PopulatedVisual");
            //    NotifyChanged("Image1");
            //}
        }
        private string _brushResourceKey = "Open";

        /// <summary>Key of a visual XAML resource for Logo1 associated with this action (such as an icon)</summary>
        public string LogoBrushResourceKey
        {
            get { return _logoBrushResourceKey; }
            set
            {
                _logoBrushResourceKey = value;
                _latestLogoBrush = null;
                NotifyChanged("LogoBrushResourceKey");
                NotifyChanged("Logo1");
            }
        }
        private string _logoBrushResourceKey;
        /// <summary>
        /// Actual visual associated with an action (such as an icon). This visual is set (identified) by the VisualResourceKey property
        /// </summary>
        public Visual Visual
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(VisualResourceKey))
                        return (Visual)Application.Current.FindResource(VisualResourceKey);
                    if (!string.IsNullOrEmpty(BrushResourceKey))
                        return new Rectangle
                        {
                            Fill = Application.Current.FindResource(BrushResourceKey) as Brush,
                            MinHeight = 16,
                            MinWidth = 16
                        };
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Like Visual, but when no visual resource is found, it attempts to load a standard icon
        /// </summary>
        public Visual PopulatedVisual
        {
            get
            {
                try
                {
                    var visual = Visual;
                    if (visual == null)
                    {
                        var rectangle = new Rectangle
                        {
                            MinHeight = 16,
                            MinWidth = 16,
                            Fill = Application.Current.FindResource("STRUCTURES.DESIGN-Icon-More") as Brush
                        };

                        visual = rectangle;
                    }
                    return visual;
                }
                catch
                {
                    return null;
                }
            }
        }

        private Brush _latestBrush;
        private Brush _latestLogoBrush;

        /// <summary>Tries to find a named XAML resource of type brush and returns it.</summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Brush or null</returns>
        /// <remarks>The returned brush is a clone, so it can be manipulated at will without impacting other users of the same brush.</remarks>
        public Brush GetBrushFromResource(string resourceName)
        {
            var resource = Application.Current.FindResource(resourceName);
            if (resource == null) return null;

            var brush = resource as Brush;
            if (brush == null) return null;

            return brush.Clone();
        }

        ///// <summary>
        ///// Returns a brush of a brush resource is defined.
        ///// </summary>
        //public Brush Brush
        //{
        //    get
        //    {
        //        if (_latestBrush != null) return _latestBrush;

        //        try
        //        {
        //            if (!string.IsNullOrEmpty(BrushResourceKey))
        //            {
        //                var brushResources = new Dictionary<object, Brush>();
        //                FrameworkElement resourceSearchContext = null;

 
        //                    resourceSearchContext = this;
        //                    ResourceHelper.GetBrushResources(resourceSearchContext, brushResources);
        //                    var standardGrid = resourceSearchContext;
        //                        var firstChild = this as FrameworkElement;
        //                        if (firstChild != null)
        //                        {
        //                            resourceSearchContext = firstChild;
        //                            ResourceHelper.GetBrushResources(resourceSearchContext, brushResources);
        //                        }

        //                if (resourceSearchContext == null && ResourceContextObject != null)
        //                {
        //                    resourceSearchContext = (FrameworkElement)ResourceContextObject;
        //                    ResourceHelper.GetBrushResources(resourceSearchContext, brushResources);
        //                }

        //                var icon = resourceSearchContext != null ? resourceSearchContext.FindResource(BrushResourceKey) as Brush : Application.Current.FindResource(BrushResourceKey) as Brush;

        //                if (brushResources.Count > 0) // We may have some resources we need to replace
        //                    If.Real<DrawingBrush>(icon, drawing => ResourceHelper.ReplaceDynamicDrawingBrushResources(drawing, brushResources));

        //                _latestBrush = icon;
        //                //NotifyChanged();
        //            }
        //            return _latestBrush;
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}

        ///// <summary>Indicates whether this action has an assigned brush</summary>
        //public bool HasBrush
        //{
        //    get { return !string.IsNullOrEmpty(BrushResourceKey); }
        //}

        ///// <summary>
        ///// Returns a brush if defined, otherwise loads a default brush
        ///// </summary>
        //public Brush PopulatedBrush
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (string.IsNullOrEmpty(BrushResourceKey))
        //            {
        //                _latestBrush = null;
        //                _brushResourceKey = "STRUCTURES.DESIGN-Icon-MissingIcon"; // Must use internal field here, otherwise all kinds of stuff gets triggered!!!
        //            }
        //            return Brush;
        //        }
        //        catch
        //        {
        //            return Brushes.Transparent;
        //        }
        //    }
        //}


        ///// <summary>This property is mostly userd internally only. It can be used to set objects that provide resource dictionaries which can then be considered for brush resources</summary>
        ///// <value>The resource context object.</value>
        //public object ResourceContextObject
        //{
        //    get { return _resourceContextObject; }
        //    set
        //    {
        //        _resourceContextObject = value as FrameworkElement;
        //        NotifyChanged("Brush");
        //        NotifyChanged("PopulatedBrush");
        //    }
        //}
        //private FrameworkElement _resourceContextObject;
        #endregion
    }
}
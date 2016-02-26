namespace System.Windows.Controls
{
	#region

	using System.Windows.Automation.Peers;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Linq;
	using System.ComponentModel;
    using System.Collections.Specialized;
	#endregion

	public class TreeViewExItem : HeaderedItemsControl
	{
		#region Brushes properties

		#region Brushes focused
		public static DependencyProperty BackgroundFocusedProperty = DependencyProperty.Register(
					"BackgroundFocused",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(SystemColors.HighlightBrush, null));

		public static DependencyProperty BorderBrushFocusedProperty = DependencyProperty.Register(
					"BorderBrushFocused",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Transparent, null));

		public Brush BackgroundFocused
		{
			get
			{
				return (Brush)GetValue(BackgroundFocusedProperty);
			}

			set
			{
				SetValue(BackgroundFocusedProperty, value);
			}
		}

		public Brush BorderBrushFocused
		{
			get
			{
				return (Brush)GetValue(BorderBrushFocusedProperty);
			}

			set
			{
				SetValue(BorderBrushFocusedProperty, value);
			}
		}
		#endregion

		#region Brushes selected and focused
		public static DependencyProperty BackgroundFocusedSelectedProperty =
					DependencyProperty.Register(
					"BackgroundFocusedSelected",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.DarkGray, null));

		public static DependencyProperty BorderBrushFocusedSelectedProperty =
					DependencyProperty.Register(
					"BorderBrushFocusedSelected",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Black, null));

		public Brush BackgroundFocusedSelected
		{
			get
			{
				return (Brush)GetValue(BackgroundFocusedSelectedProperty);
			}

			set
			{
				SetValue(BackgroundFocusedSelectedProperty, value);
			}
		}

		public Brush BorderBrushFocusedSelected
		{
			get
			{
				return (Brush)GetValue(BorderBrushFocusedSelectedProperty);
			}

			set
			{
				SetValue(BorderBrushFocusedSelectedProperty, value);
			}
		}
		#endregion

		#region Brushes hovered

		public static DependencyProperty BackgroundHoveredProperty = DependencyProperty.Register(
					"BackgroundHovered",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.LightGray, null));

		public static DependencyProperty BorderBrushHoveredProperty = DependencyProperty.Register(
					"BorderBrushHovered",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Transparent, null));

		public Brush BackgroundHovered
		{
			get
			{
				return (Brush)GetValue(BackgroundHoveredProperty);
			}

			set
			{
				SetValue(BackgroundHoveredProperty, value);
			}
		}

		public Brush BorderBrushHovered
		{
			get
			{
				return (Brush)GetValue(BorderBrushHoveredProperty);
			}

			set
			{
				SetValue(BorderBrushHoveredProperty, value);
			}
		}
		#endregion

		#region Brushes selected and hovered
		public static DependencyProperty BackgroundSelectedHoveredProperty = DependencyProperty.Register(
					"BackgroundSelectedHovered",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.DarkGray, null));

		public static DependencyProperty BorderBrushSelectedHoveredProperty =
					DependencyProperty.Register(
					"BorderBrushSelectedHovered",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Black, null));

		public Brush BorderBrushSelectedHovered
		{
			get
			{
				return (Brush)GetValue(BorderBrushSelectedHoveredProperty);
			}
			set
			{
				SetValue(BorderBrushSelectedHoveredProperty, value);
			}
		}

		public Brush BackgroundSelectedHovered
		{
			get
			{
				return (Brush)GetValue(BackgroundSelectedHoveredProperty);
			}
			set
			{
				SetValue(BackgroundSelectedHoveredProperty, value);
			}
		}
		#endregion

		#region Brushes selected
		public static DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
					"BackgroundSelected",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.LightGray, null));

		public static DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
					"BorderBrushSelected",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Transparent, null));

		public Brush BackgroundSelected
		{
			get
			{
				return (Brush)GetValue(BackgroundSelectedProperty);
			}

			set
			{
				SetValue(BackgroundSelectedProperty, value);
			}
		}

		public Brush BorderBrushSelected
		{
			get
			{
				return (Brush)GetValue(BorderBrushSelectedProperty);
			}

			set
			{
				SetValue(BorderBrushSelectedProperty, value);
			}
		}
		#endregion

		#region Brushes disabled
		public static DependencyProperty BackgroundInactiveProperty = DependencyProperty.Register(
					"BackgroundInactive",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.LightGray, null));

		public static DependencyProperty BorderBrushInactiveProperty =
					DependencyProperty.Register(
					"BorderBrushInactive",
					typeof(Brush),
					typeof(TreeViewExItem),
					new FrameworkPropertyMetadata(Brushes.Black, null));

		public Brush BackgroundInactive
		{
			get
			{
				return (Brush)GetValue(BackgroundInactiveProperty);
			}

			set
			{
				SetValue(BackgroundInactiveProperty, value);
			}
		}

		public Brush BorderBrushInactive
		{
			get
			{
				return (Brush)GetValue(BorderBrushInactiveProperty);
			}

			set
			{
				SetValue(BorderBrushInactiveProperty, value);
			}
		}
		#endregion
		#endregion

		#region Constants and Fields
		public static DependencyProperty IsExpandedProperty = DependencyProperty.Register(
					"IsExpanded", typeof(bool), typeof(TreeViewExItem), new FrameworkPropertyMetadata(false, null));

		public static DependencyProperty IsEditableProperty = DependencyProperty.Register(
					"IsEditable", typeof(bool), typeof(TreeViewExItem), new FrameworkPropertyMetadata(true, null));

		public static DependencyProperty IsSelectedProperty = DependencyProperty.Register(
					"IsSelected", typeof(bool), typeof(TreeViewExItem), new FrameworkPropertyMetadata(false, null));

		public static DependencyProperty IsEditingProperty = DependencyProperty.Register(
					"IsEditing", typeof(bool), typeof(TreeViewExItem), new FrameworkPropertyMetadata(false, null));

		public static new DependencyProperty IsVisibleProperty = DependencyProperty.Register(
			"IsVisible",
			typeof(bool),
			typeof(TreeViewExItem),
			new FrameworkPropertyMetadata(true, null));

		public static DependencyProperty ContentTemplateEditProperty = DependencyProperty.Register(
					"ContentTemplateEdit", typeof(DataTemplate), typeof(TreeViewExItem), new FrameworkPropertyMetadata(null, null));

		#endregion

		#region Constructors and Destructors

		static TreeViewExItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
						typeof(TreeViewExItem), new FrameworkPropertyMetadata(typeof(TreeViewExItem)));
		}

		public TreeViewExItem()
		{
			// Debug.WriteLine("TreeViewExItem ctor");
		}
		#endregion

		#region Public Properties

		public DataTemplate ContentTemplateEdit
		{
			get
			{
				return (DataTemplate)GetValue(ContentTemplateEditProperty);
			}

			set
			{
				SetValue(ContentTemplateEditProperty, value);
			}
		}

		public bool IsExpanded
		{
			get
			{
				return (bool)GetValue(IsExpandedProperty);
			}

			set
			{
				SetValue(IsExpandedProperty, value);
			}
		}

		public bool IsEditable
		{
			get
			{
				return (bool)GetValue(IsEditableProperty);
			}

			set
			{
				SetValue(IsEditableProperty, value);
			}
		}

		public bool IsEditing
		{
			get
			{
				return (bool)GetValue(IsEditingProperty);
			}

			set
			{
				SetValue(IsEditingProperty, value);
			}
		}

		public bool IsSelected
		{
			get
			{
				return (bool)GetValue(IsSelectedProperty);
			}

			set
			{
				//Debug.WriteLine("IsSelected of " + DataContext + " is " + value + " from " + ParentItemsControl.GetHashCode());
				SetValue(IsSelectedProperty, value);
			}
		}
		
		public new bool IsVisible
		{
			get
			{
				return (bool)GetValue(IsVisibleProperty);
			}
			set
			{
				SetValue(IsVisibleProperty, value);
			}
		}
		#endregion

		#region Properties

		internal TreeViewEx ParentTreeView
		{
			get
			{
				for (ItemsControl itemsControl = ParentItemsControl;
						itemsControl != null;
						itemsControl = ItemsControlFromItemContainer(itemsControl))
				{
					TreeViewEx treeView = itemsControl as TreeViewEx;
					if (treeView != null)
					{
						return treeView;
					}
				}

				return null;
			}
		}

		private static bool IsControlKeyDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
			}
		}

		private static bool IsShiftKeyDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			}
		}

		private bool CanExpand
		{
			get
			{
				return HasItems;
			}
		}

		private bool CanExpandOnInput
		{
			get
			{
				return CanExpand && IsEnabled;
			}
		}

		private ItemsControl ParentItemsControl
		{
			get
			{
				return ItemsControlFromItemContainer(this);
			}
		}

		#endregion

		#region Public Methods

		public override string ToString()
		{
			if (DataContext != null)
			{
				return string.Format("{0} ({1})", DataContext, base.ToString());
			}

			return base.ToString();
		}

		#endregion

		#region Protected Methods

		protected override void OnKeyUp(KeyEventArgs e)
		{ 
			base.OnKeyUp(e);
			if (!e.Handled)
			{
				Key key = e.Key;
				switch (key)
				{
					case Key.Left:
					case Key.Right:
					case Key.Up:
					case Key.Down:
					case Key.Add:
					case Key.Subtract:
					case Key.Space:
						IEnumerable<TreeViewExItem> items = TreeViewEx.RecursiveTreeViewItemEnumerable(ParentTreeView, false);
						TreeViewExItem focusedItem = items.Where<TreeViewExItem>(x => x.IsFocused).FirstOrDefault();

						if (focusedItem != null)
							focusedItem.BringIntoView(new Rect(1, 1, 1, 1));
						break;
				}
			}
		}
        
        TreeViewExItem GetTreeViewItemUnderMouse(TreeViewEx treeView, Point positionRelativeToTree)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeView, positionRelativeToTree);
            if (hitTestResult == null || hitTestResult.VisualHit == null) return null;

            TreeViewExItem item = null;
            DependencyObject currentObject = hitTestResult.VisualHit;

            while (item == null && currentObject != null)
            {
                item = currentObject as TreeViewExItem;
                currentObject = VisualTreeHelper.GetParent(currentObject);
            }

            return item;
        }

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (ParentTreeView == null) return;

			// System.Diagnostics.Debug.WriteLine("P(" + ParentTreeView.Name + "): " + e.Property + " " + e.NewValue);

			if (e.Property.Name == "IsEditing")
			{
				if ((bool)e.NewValue == false)
				{
					StopEditing();
				}
			}
			
			if (e.Property.Name == "IsSelected")
			{
				if (ParentTreeView.SelectedItems.Contains(DataContext) != IsSelected)
					ParentTreeView.Selection.SelectFromProperty(this, IsSelected);
            }

            if (e.Property.Name == "ItemsSource")
            {
                INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged -= OnItemsSourceChanged;
                }

                collection = e.NewValue as INotifyCollectionChanged;
                if (collection != null)
                {
                    collection.CollectionChanged += OnItemsSourceChanged;
                }
            }

			base.OnPropertyChanged(e);
		}

        private void OnItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.OldItems)
            {
                if (ParentTreeView.SelectedItems.Contains(item))
                {
                    ParentTreeView.SelectedItems.Remove(item);
                }
            }
        }

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeViewExItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeViewExItem;
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new TreeViewExItemAutomationPeer(this);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			if (ParentTreeView.SelectedItems.Contains(DataContext))
			{
				IsSelected = true;
			}
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);

            // if the user double clicks the toggle button, the event goes to the parent item...
            if (GetTreeViewItemUnderMouse(ParentTreeView, e.GetPosition(ParentTreeView)) != this) return;

			if (IsKeyboardFocused) IsExpanded = !IsExpanded;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				Key key = e.Key;
				switch (key)
				{
					case Key.Left:
						if (IsExpanded)
						{
							IsExpanded = false;
						}

						e.Handled = true;
						break;
					case Key.Right:
						if (CanExpand)
						{
							IsExpanded = true;
						}

						e.Handled = true;
						break;
					case Key.Up:
						ParentTreeView.Selection.SelectPreviousFromKey();
						e.Handled = true;
						break;
					case Key.Down:
						ParentTreeView.Selection.SelectNextFromKey();
						e.Handled = true;
						break;
					case Key.Add:
						if (CanExpandOnInput && !IsExpanded)
						{
							IsExpanded = true;
						}

						e.Handled = true;
						break;
					case Key.Subtract:
						if (CanExpandOnInput && IsExpanded)
						{
							IsExpanded = false;
						}

						e.Handled = true;
						break;
					case Key.F2:
						if (ContentTemplateEdit != null && IsFocused && IsEditable)
						{
							IsEditing = true;
						}

						e.Handled = true;
						break;
					case Key.Escape:
						StopEditing();
						e.Handled = true;
						break;
					case Key.Return:
						FocusHelper.Focus(this);
						IsEditing = false;
						e.Handled = true;
						break;
					case Key.Space:
						ParentTreeView.Selection.SelectCurrentBySpace();
						e.Handled = true;
						break;
					case Key.Home:
						ParentTreeView.Selection.SelectFirst();
						e.Handled = true;
						break;
					case Key.End:
						ParentTreeView.Selection.SelectLast();
						e.Handled = true;
						break;
				}
			}
		}

		private void StopEditing()
		{
			FocusHelper.Focus(this);
			IsEditing = false;
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			IsEditing = false;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			ParentTreeView.Selection.Select(this);

			e.Handled = true;
		}
		#endregion

		#region Internal Methods
		internal void InvokeMouseDown()
		{
			var e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right);
			e.RoutedEvent = Mouse.MouseDownEvent;
			this.OnMouseDown(e);
		}
		#endregion
	}
}
namespace System.Windows.Controls
{
	#region

	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Windows.Automation.Peers;
	using System.Windows.Media;
	using System.ComponentModel;

	#endregion

	public class TreeViewEx : ItemsControl
	{
		public event EventHandler<SelectionChangedCancelEventArgs> OnSelecting;
		#region Constants and Fields

		public static readonly DependencyProperty LastSelectedItemProperty;

		public static DependencyProperty BackgroundSelectionRectangleProperty =
		DependencyProperty.Register(
		"BackgroundSelectionRectangle",
		typeof(Brush),
		typeof(TreeViewExItem),
		new FrameworkPropertyMetadata(Brushes.LightBlue, null));

		public static DependencyProperty BorderBrushSelectionRectangleProperty =
		DependencyProperty.Register(
		"BorderBrushSelectionRectangle",
		typeof(Brush),
		typeof(TreeViewExItem),
		new FrameworkPropertyMetadata(Brushes.Blue, null));

		public static DependencyPropertyKey LastSelectedItemPropertyKey =
		DependencyProperty.RegisterReadOnly(
		"LastSelectedItem",
		typeof(object),
		typeof(TreeViewEx),
		new FrameworkPropertyMetadata(null));

		public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
		"SelectedItems",
		typeof(IList),
		typeof(TreeViewEx),
		new FrameworkPropertyMetadata(
		null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));

		#endregion

		#region Constructors and Destructors

		static TreeViewEx()
		{
			LastSelectedItemProperty = LastSelectedItemPropertyKey.DependencyProperty;
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewEx), new FrameworkPropertyMetadata(typeof(TreeViewEx)));
		}

		public TreeViewEx()
		{
			SelectedItems = new ObservableCollection<object>();
			Selection = new SelectionMultiple(this);
		}

		#endregion

		#region Public Properties

		public Brush BackgroundSelectionRectangle
		{
			get
			{
				return (Brush)GetValue(BackgroundSelectionRectangleProperty);
			}

			set
			{
				SetValue(BackgroundSelectionRectangleProperty, value);
			}
		}

		public Brush BorderBrushSelectionRectangle
		{
			get
			{
				return (Brush)GetValue(BorderBrushSelectionRectangleProperty);
			}

			set
			{
				SetValue(BorderBrushSelectionRectangleProperty, value);
			}
		}

		/// <summary>
		///	Gets the last selected item.
		/// </summary>
		public object LastSelectedItem
		{
			get
			{
				return GetValue(LastSelectedItemProperty);
			}

			private set
			{
				SetValue(LastSelectedItemPropertyKey, value);
			}
		}

		/// <summary>
		///	Gets or sets a list of selected items and can be bound to another list. If the source list 
		///	implements <see cref="INotifyPropertyChanged"/> the changes are automatically taken over.
		/// </summary>
		public IList SelectedItems
		{
			get
			{
				return (IList)GetValue(SelectedItemsProperty);
			}

			set
			{
				SetValue(SelectedItemsProperty, value);
			}
		}

		internal ISelectionStrategy Selection { get; private set; }

		#endregion

		#region Public Methods and Operators

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Selection.ApplyTemplate();

			Unloaded += OnUnLoaded;
		}

		#endregion

        #region Methods
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
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
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
            }
        }

		internal bool CheckSelectionAllowed(object item, bool isItemAdded)
		{
			if (isItemAdded)
			{
				return CheckSelectionAllowed(new List<object> { item }, new List<object>());
			}
			else
			{
				return CheckSelectionAllowed(new List<object>(), new List<object> { item });
			}
		}

		internal bool CheckSelectionAllowed(IEnumerable<object> itemsToSelect, IEnumerable<object> itemsToUnselect)
		{
			if (OnSelecting != null)
			{
				var e = new SelectionChangedCancelEventArgs(itemsToSelect, itemsToUnselect);
				foreach (var method in OnSelecting.GetInvocationList())
				{
					method.Method.Invoke(method.Target, new object[] { this, e });
					// stop iteration if one subscriber wants to cancel
					if (e.Cancel) return false;
				}

				return true;
			}

			return true;
		}

		internal void ClearSelectionByRectangle()
		{
			SelectedItems.Clear();
		}

		internal TreeViewExItem GetNextItem(TreeViewExItem item, List<TreeViewExItem> items)
		{
			int indexOfCurrent = items.IndexOf(item);

			for (int i = indexOfCurrent + 1; i < items.Count; i++)
			{
				if (items[i].IsVisible)
				{
					return items[i];
				}
			}

			return null;
		}

		internal TreeViewExItem GetPreviousItem(TreeViewExItem item, List<TreeViewExItem> items)
		{
			int indexOfCurrent = items.IndexOf(item);
			for (int i = indexOfCurrent - 1; i >= 0; i--)
			{
				if (items[i].IsVisible)
				{
					return items[i];
				}
			}

			return null;
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
			return new TreeViewExAutomationPeer(this);
		}

		private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TreeViewEx treeView = (TreeViewEx)d;
			if (e.OldValue != null)
			{
				INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
				if (collection != null) collection.CollectionChanged -= treeView.OnSelectedItemsChanged;
			}

			if (e.NewValue != null)
			{
				INotifyCollectionChanged collection = e.NewValue as INotifyCollectionChanged;
				if (collection != null)
				{
					collection.CollectionChanged += treeView.OnSelectedItemsChanged;
				}
			}
		}

		internal static IEnumerable<TreeViewExItem> RecursiveTreeViewItemEnumerable(ItemsControl parent, bool ignoreInvisible)
		{
			foreach (var item in parent.Items)
			{
				TreeViewExItem tve = (TreeViewExItem)parent.ItemContainerGenerator.ContainerFromItem(item);
				if (tve == null)
				{
					// container was not generated, therefor it is probably not visible, so we can ignore it.
					continue;
				}

				if (ignoreInvisible)
				{
					if (tve.IsVisible && tve.Visibility == Visibility.Visible)
					{
						yield return tve;

						if (tve.IsExpanded)
						{
							foreach (var childItem in RecursiveTreeViewItemEnumerable(tve, ignoreInvisible))
							{
								yield return childItem;
							}
						}
					}
				}
				else
				{
					yield return tve;

					foreach (var childItem in RecursiveTreeViewItemEnumerable(tve, ignoreInvisible))
					{
						yield return childItem;
					}
				}
			}
		}

		internal IEnumerable<TreeViewExItem> GetNodesToSelectBetween(TreeViewExItem firstNode, TreeViewExItem lastNode)
		{
			var allNodes = RecursiveTreeViewItemEnumerable(this, false).ToList();
			var firstIndex = allNodes.IndexOf(firstNode);
			var lastIndex = allNodes.IndexOf(lastNode);

			if (firstIndex >= allNodes.Count)
			{
				throw new InvalidOperationException(
							"First node index " + firstIndex + "greater or equal than count " + allNodes.Count + ".");
			}

			if (lastIndex >= allNodes.Count)
			{
				throw new InvalidOperationException(
							"Last node index " + lastIndex + " greater or equal than count " + allNodes.Count + ".");
			}

			var nodesToSelect = new List<TreeViewExItem>();

			if (lastIndex == firstIndex)
			{
				return new List<TreeViewExItem> { firstNode };
			}

			if (lastIndex > firstIndex)
			{
				for (int i = firstIndex; i <= lastIndex; i++)
				{
					if (allNodes[i].IsVisible)
					{
						nodesToSelect.Add(allNodes[i]);
					}
				}
			}
			else
			{
				for (int i = firstIndex; i >= lastIndex; i--)
				{
					if (allNodes[i].IsVisible)
					{
						nodesToSelect.Add(allNodes[i]);
					}
				}
			}

			return nodesToSelect;
		}

		internal IEnumerable<TreeViewExItem> GetTreeViewItemsFor(IEnumerable objects)
		{
			if (objects == null)
			{
				yield break;
			}

			foreach (var newItem in objects)
			{
				foreach (var treeViewExItem in RecursiveTreeViewItemEnumerable(this, false))
				{
					if (newItem == treeViewExItem.DataContext)
					{
						yield return treeViewExItem;
						break;
					}
				}
			}
		}

		// this eventhandler reacts on the firing control to, in order to update the own status
		private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					object last = null;
					foreach (var item in GetTreeViewItemsFor(e.NewItems))
					{
						if (!item.IsSelected)
						{
							item.IsSelected = true;
						}

						last = item.DataContext;
					}

					LastSelectedItem = last;
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var item in GetTreeViewItemsFor(e.OldItems))
					{
						item.IsSelected = false;
						if (item.DataContext == LastSelectedItem)
						{
							if (SelectedItems.Count > 0)
							{
								LastSelectedItem = SelectedItems[SelectedItems.Count - 1];
							}
							else
							{
								LastSelectedItem = null;
							}
						}
					}

					break;
				case NotifyCollectionChangedAction.Reset:
					foreach (var item in RecursiveTreeViewItemEnumerable(this, false))
					{
						if (item.IsSelected)
						{
							item.IsSelected = false;
						}
					}

					LastSelectedItem = null;
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private void OnUnLoaded(object sender, RoutedEventArgs e)
		{
			Unloaded -= OnUnLoaded;
			if (Selection != null)
				Selection.Dispose();
		}

		#endregion
	}
}
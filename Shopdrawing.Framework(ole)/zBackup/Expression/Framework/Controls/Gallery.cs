// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.Gallery
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class Gallery : Grid, INotifyPropertyChanged, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof (IEnumerable), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(Gallery.OnItemsChanged)));
    public static readonly DependencyProperty FavoriteItemsProperty = DependencyProperty.Register("FavoriteItems", typeof (IEnumerable), typeof (Gallery));
    public static readonly DependencyProperty MostRecentlyUsedItemsProperty = DependencyProperty.Register("MostRecentlyUsedItems", typeof (IEnumerable), typeof (Gallery));
    public static readonly DependencyProperty MostRecentlyUsedItemContainerStyleProperty = DependencyProperty.Register("MostRecentlyUsedItemContainerStyle", typeof (Style), typeof (Gallery));
    public static readonly DependencyProperty MaxMostRecentlyUsedProperty = DependencyProperty.Register("MaxMostRecentlyUsed", typeof (int), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) 2));
    public static readonly DependencyProperty MaxFavoritesProperty = DependencyProperty.Register("MaxFavorites", typeof (int), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
    public static readonly DependencyProperty MainItemDataTemplateProperty = DependencyProperty.Register("MainItemDataTemplate", typeof (DataTemplate), typeof (Gallery));
    public static readonly DependencyProperty MainItemContainerStyleProperty = DependencyProperty.Register("MainItemContainerStyle", typeof (Style), typeof (Gallery));
    public static readonly DependencyProperty GroupExpanderStyleProperty = DependencyProperty.Register("GroupExpanderStyle", typeof (Style), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    public static readonly DependencyProperty FavoriteDataTemplateProperty = DependencyProperty.Register("FavoriteDataTemplate", typeof (DataTemplate), typeof (Gallery));
    public static readonly DependencyProperty MostRecentlyUsedDataTemplateProperty = DependencyProperty.Register("MostRecentlyUsedDataTemplate", typeof (DataTemplate), typeof (Gallery));
    public static readonly DependencyProperty MainItemsHeaderProperty = DependencyProperty.Register("MainItemsHeader", typeof (string), typeof (Gallery), new PropertyMetadata((object) StringTable.GalleryMainItemDefaultHeader));
    public static readonly DependencyProperty MainItemsPanelTemplateProperty = DependencyProperty.Register("MainItemsPanelTemplate", typeof (ItemsPanelTemplate), typeof (Gallery));
    public static readonly DependencyProperty FavoritesItemsPanelTemplateProperty = DependencyProperty.Register("FavoritesItemsPanelTemplate", typeof (ItemsPanelTemplate), typeof (Gallery));
    public static readonly DependencyProperty FavoritesItemContainerStyleProperty = DependencyProperty.Register("FavoritesItemContainerStyle", typeof (Style), typeof (Gallery));
    public static readonly DependencyProperty MostRecentlyUsedItemsPanelTemplateProperty = DependencyProperty.Register("MostRecentlyUsedItemsPanelTemplate", typeof (ItemsPanelTemplate), typeof (Gallery));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof (object), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof (bool), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty MostRecentlyUsedMinHeightProperty = DependencyProperty.Register("MostRecentlyUsedMinHeight", typeof (int), typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
    public static readonly RoutedEvent GroupToggleExpandEvent = EventManager.RegisterRoutedEvent("GroupToggleExpand", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Gallery));
    public static readonly RoutedEvent DoubleClickOnItemEvent = EventManager.RegisterRoutedEvent("DoubleClickOnItem", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Gallery));
    public static readonly RoutedEvent GroupNameClickedEvent = EventManager.RegisterRoutedEvent("GroupNameClicked", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Gallery));
    public static readonly RoutedEvent SelectedItemKeyDownEvent = EventManager.RegisterRoutedEvent("SelectedItemKeyDown", RoutingStrategy.Bubble, typeof (KeyEventHandler), typeof (Gallery));
    public static readonly string UnfetchedFolderItemName = "*** unfetched placeholder ***";
    private Point mouseDownPoint;
    private bool waitingForDrag;
    private object draggedItem;
    private object dragOverItem;
    internal Gallery UserControlSelf;
    internal SelectingListBox CategoryListBox;
    private bool _contentLoaded;

    public IEnumerable Items
    {
      get
      {
        return (IEnumerable) this.GetValue(Gallery.ItemsProperty);
      }
      set
      {
        this.SetValue(Gallery.ItemsProperty, (object) value);
      }
    }

    public IEnumerable FavoriteItems
    {
      get
      {
        return (IEnumerable) this.GetValue(Gallery.FavoriteItemsProperty);
      }
      set
      {
        this.SetValue(Gallery.FavoriteItemsProperty, (object) value);
      }
    }

    public IEnumerable MostRecentlyUsedItems
    {
      get
      {
        return (IEnumerable) this.GetValue(Gallery.MostRecentlyUsedItemsProperty);
      }
      set
      {
        this.SetValue(Gallery.MostRecentlyUsedItemsProperty, (object) value);
      }
    }

    public int MaxMostRecentlyUsed
    {
      get
      {
        return (int) this.GetValue(Gallery.MaxMostRecentlyUsedProperty);
      }
      set
      {
        this.SetValue(Gallery.MaxMostRecentlyUsedProperty, (object) value);
      }
    }

    public int MaxFavorites
    {
      get
      {
        return (int) this.GetValue(Gallery.MaxFavoritesProperty);
      }
      set
      {
        this.SetValue(Gallery.MaxFavoritesProperty, (object) value);
      }
    }

    public DataTemplate MainItemDataTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(Gallery.MainItemDataTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.MainItemDataTemplateProperty, (object) value);
      }
    }

    public Style MainItemContainerStyle
    {
      get
      {
        return (Style) this.GetValue(Gallery.MainItemContainerStyleProperty);
      }
      set
      {
        this.SetValue(Gallery.MainItemContainerStyleProperty, (object) value);
      }
    }

    public Style GroupExpanderStyle
    {
      get
      {
        return (Style) this.GetValue(Gallery.GroupExpanderStyleProperty);
      }
      set
      {
        this.SetValue(Gallery.GroupExpanderStyleProperty, (object) value);
      }
    }

    public Style FavoritesItemContainerStyle
    {
      get
      {
        return (Style) this.GetValue(Gallery.FavoritesItemContainerStyleProperty);
      }
      set
      {
        this.SetValue(Gallery.FavoritesItemContainerStyleProperty, (object) value);
      }
    }

    public Style MostRecentlyUsedItemContainerStyle
    {
      get
      {
        return (Style) this.GetValue(Gallery.MostRecentlyUsedItemContainerStyleProperty);
      }
      set
      {
        this.SetValue(Gallery.MostRecentlyUsedItemContainerStyleProperty, (object) value);
      }
    }

    public DataTemplate FavoriteDataTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(Gallery.FavoriteDataTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.FavoriteDataTemplateProperty, (object) value);
      }
    }

    public DataTemplate MostRecentlyUsedDataTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(Gallery.MostRecentlyUsedDataTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.MostRecentlyUsedDataTemplateProperty, (object) value);
      }
    }

    public ItemsPanelTemplate MainItemsPanelTemplate
    {
      get
      {
        return (ItemsPanelTemplate) this.GetValue(Gallery.MainItemsPanelTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.MainItemsPanelTemplateProperty, (object) value);
      }
    }

    public string MainItemsHeader
    {
      get
      {
        return (string) this.GetValue(Gallery.MainItemsHeaderProperty);
      }
      set
      {
        this.SetValue(Gallery.MainItemsHeaderProperty, (object) value);
      }
    }

    public ItemsPanelTemplate FavoritesItemsPanelTemplate
    {
      get
      {
        return (ItemsPanelTemplate) this.GetValue(Gallery.FavoritesItemsPanelTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.FavoritesItemsPanelTemplateProperty, (object) value);
      }
    }

    public ItemsPanelTemplate MostRecentlyUsedItemsPanelTemplate
    {
      get
      {
        return (ItemsPanelTemplate) this.GetValue(Gallery.MostRecentlyUsedItemsPanelTemplateProperty);
      }
      set
      {
        this.SetValue(Gallery.MostRecentlyUsedItemsPanelTemplateProperty, (object) value);
      }
    }

    public bool IsGrouped
    {
        get
        {
            return (bool)base.GetValue(Gallery.IsGroupedProperty);
        }
        set
        {
            base.SetValue(Gallery.IsGroupedProperty, value);
        }
    }

    public int MostRecentlyUsedMinHeight
    {
      get
      {
        return (int) this.GetValue(Gallery.MostRecentlyUsedMinHeightProperty);
      }
      set
      {
        this.SetValue(Gallery.MostRecentlyUsedMinHeightProperty, (object) value);
      }
    }

    public object DragOverItem
    {
      get
      {
        return this.dragOverItem;
      }
    }

    public ICommand SelectItemCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OnSelectItem));
      }
    }

    public object SelectedItem
    {
      get
      {
        return this.GetValue(Gallery.SelectedItemProperty);
      }
      set
      {
        this.SetValue(Gallery.SelectedItemProperty, value);
      }
    }

    public event RoutedEventHandler GroupToggleExpand
    {
      add
      {
        this.AddHandler(Gallery.GroupToggleExpandEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(Gallery.GroupToggleExpandEvent, (Delegate) value);
      }
    }

    public event RoutedEventHandler DoubleClickOnItem
    {
      add
      {
        this.AddHandler(Gallery.DoubleClickOnItemEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(Gallery.DoubleClickOnItemEvent, (Delegate) value);
      }
    }

    public event RoutedEventHandler GroupNameClicked
    {
      add
      {
        this.AddHandler(Gallery.GroupNameClickedEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(Gallery.GroupNameClickedEvent, (Delegate) value);
      }
    }

    public event KeyEventHandler SelectedItemKeyDown
    {
      add
      {
        this.AddHandler(Gallery.SelectedItemKeyDownEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(Gallery.SelectedItemKeyDownEvent, (Delegate) value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    static Gallery()
    {
      ItemsPanelTemplate itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof (StackPanel)));
      Gallery.MainItemsPanelTemplateProperty.OverrideMetadata(typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) itemsPanelTemplate));
      Gallery.FavoritesItemsPanelTemplateProperty.OverrideMetadata(typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) itemsPanelTemplate));
      Gallery.MostRecentlyUsedItemsPanelTemplateProperty.OverrideMetadata(typeof (Gallery), (PropertyMetadata) new FrameworkPropertyMetadata((object) itemsPanelTemplate));
    }

    public Gallery()
    {
      this.InitializeComponent();
    }

    public void PromoteItemToMru(object item)
    {
      if (item == null)
        return;
      IList list1 = Gallery.GetList(this.FavoriteItems);
      if (list1 == null || list1.Contains(item))
        return;
      IList list2 = Gallery.GetList(this.MostRecentlyUsedItems);
      if (list2 == null || list2.Contains(item))
        return;
      if (list2.Count > 0 && list2.Count >= this.MaxMostRecentlyUsed)
        list2.RemoveAt(list2.Count - 1);
      Type c = list2.GetType().GetGenericArguments()[0];
      if (!item.GetType().IsAssignableFrom(c))
        return;
      list2.Insert(0, item);
    }

    public void PromoteCurrentItemToMru()
    {
      ICollectionView collectionView = this.Items as ICollectionView;
      if (this.MostRecentlyUsedItems == null || collectionView == null || collectionView.CurrentItem == null)
        return;
      this.PromoteItemToMru(collectionView.CurrentItem);
    }

    public void MoveFocusToCurrentItem()
    {
      ItemContainerGenerator containerGenerator = this.CategoryListBox.ItemContainerGenerator;
      if (this.CategoryListBox.SelectedIndex < 0)
        return;
      UIElement uiElement = containerGenerator.ContainerFromIndex(this.CategoryListBox.SelectedIndex) as UIElement;
      if (uiElement == null || !uiElement.IsVisible)
        return;
      uiElement.Focus();
    }

    public void MoveCurrentItemIntoView()
    {
      ICollectionView collectionView = this.Items as ICollectionView;
      if (collectionView.CurrentPosition < 0)
        return;
      this.CategoryListBox.ScrollIntoView(this.CategoryListBox.Items[collectionView.CurrentPosition]);
    }

    public void DeselectAll()
    {
      this.CategoryListBox.SelectedIndex = -1;
    }

    private void GalleryItemDoubleClicked(object sender, MouseButtonEventArgs e)
    {
      this.RaiseEvent(new RoutedEventArgs(Gallery.DoubleClickOnItemEvent, sender));
    }

    private Expander FindParentExpander(DependencyObject element)
    {
      while (element != null && !(element is Gallery) && !(element is Expander))
        element = VisualTreeHelper.GetParent(element);
      return element as Expander;
    }

    private void CategoryListBox_KeyDown(object sender, KeyEventArgs e)
    {
      SelectingListBox selectingListBox = sender as SelectingListBox;
      if (e.Handled || selectingListBox == null || !selectingListBox.IsKeyboardFocusWithin)
        return;
      FocusNavigationDirection focusNavigationDirection;
      switch (e.Key)
      {
        case Key.Left:
          focusNavigationDirection = FocusNavigationDirection.Left;
          break;
        case Key.Up:
          focusNavigationDirection = FocusNavigationDirection.Up;
          break;
        case Key.Right:
          focusNavigationDirection = FocusNavigationDirection.Right;
          break;
        case Key.Down:
          focusNavigationDirection = FocusNavigationDirection.Down;
          break;
        default:
          return;
      }
      TraversalRequest request = new TraversalRequest(focusNavigationDirection);
      if (Keyboard.FocusedElement is SelectingListBoxItem)
      {
        SelectingListBoxItem selectingListBoxItem = Keyboard.FocusedElement as SelectingListBoxItem;
        KeyEventArgs keyEventArgs = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key);
        keyEventArgs.RoutedEvent = Gallery.SelectedItemKeyDownEvent;
        keyEventArgs.Source = (object) selectingListBoxItem;
        this.RaiseEvent((RoutedEventArgs) keyEventArgs);
        if (!keyEventArgs.Handled)
          selectingListBoxItem.MoveFocus(request);
        e.Handled = true;
      }
      else
      {
        Expander parentExpander = this.FindParentExpander(Keyboard.FocusedElement as DependencyObject);
        if (parentExpander == null || !parentExpander.IsKeyboardFocusWithin)
          return;
        switch (focusNavigationDirection)
        {
          case FocusNavigationDirection.Left:
            if (parentExpander.IsExpanded)
            {
              parentExpander.IsExpanded = false;
              break;
            }
            break;
          case FocusNavigationDirection.Right:
            if (!parentExpander.IsExpanded)
            {
              parentExpander.IsExpanded = true;
              break;
            }
            break;
          default:
            UIElement uiElement = Keyboard.FocusedElement as UIElement;
            if (uiElement != null)
            {
              uiElement.MoveFocus(request);
              break;
            }
            break;
        }
        e.Handled = true;
      }
    }

    private static void OnItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      Gallery gallery = o as Gallery;
      if (gallery == null)
        return;
      ICollectionView defaultView = CollectionViewSource.GetDefaultView(e.OldValue);
      if (defaultView != null)
        defaultView.CurrentChanged -= new EventHandler(gallery.OnCurrentItemChanged);
      ICollectionView collectionView = e.NewValue as ICollectionView;
      if (collectionView == null)
        return;
      collectionView.CurrentChanged += new EventHandler(gallery.OnCurrentItemChanged);
    }

    private void OnGroupToggleExpand(string groupName, bool expanded)
    {
      this.RaiseEvent((RoutedEventArgs) new Gallery.GroupToggleExpandEventArgs(groupName, expanded, this));
    }

    private void OnGroupNameClicked(string groupName)
    {
      this.RaiseEvent((RoutedEventArgs) new GroupNameClickedEventArgs(groupName, this));
    }

    private void GroupExpander_Expanded(object sender, EventArgs e)
    {
      this.OnGroupToggleExpand(((CollectionViewGroup) ((FrameworkElement) sender).DataContext).Name.ToString(), true);
    }

    private void GroupExpander_Collapsed(object sender, EventArgs e)
    {
      this.OnGroupToggleExpand(((CollectionViewGroup) ((FrameworkElement) sender).DataContext).Name.ToString(), false);
    }

    private void GroupExpander_NameClicked(object sender, MouseButtonEventArgs e)
    {
      this.OnGroupNameClicked(((CollectionViewGroup) ((FrameworkElement) sender).DataContext).Name.ToString());
    }

    private void OnCurrentItemChanged(object sender, EventArgs e)
    {
    }

    private void OnSelectItem(object parameter)
    {
      if (parameter == null)
        return;
      ICollectionView collectionView = this.Items as ICollectionView;
      if (collectionView == null)
        return;
      collectionView.MoveCurrentTo(parameter);
      this.MoveCurrentItemIntoView();
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      object obj = (object) null;
      FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
      if (frameworkElement != null && frameworkElement.TemplatedParent != null)
        obj = ((FrameworkElement) frameworkElement.TemplatedParent).DataContext;
      IList list = Gallery.GetList(this.Items);
      if (obj is IDragDropDataProvider || list.Contains(obj))
      {
        this.draggedItem = obj;
        if (this.draggedItem is IDragDropDataProvider)
          this.draggedItem = ((IDragDropDataProvider) this.draggedItem).DragDropData;
        if (this.draggedItem != null)
        {
          this.mouseDownPoint = e.GetPosition((IInputElement) this);
          this.waitingForDrag = true;
        }
      }
      base.OnPreviewMouseLeftButtonDown(e);
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      Gallery.GetList(this.FavoriteItems);
      if (this.waitingForDrag && this.draggedItem != null && (e.GetPosition((IInputElement) this) - this.mouseDownPoint).Length > 6.0)
      {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
          int num = (int) DragDrop.DoDragDrop((DependencyObject) this, this.draggedItem, DragDropEffects.All);
          this.dragOverItem = (object) null;
          this.OnPropertyChanged("DragOverItem");
          this.draggedItem = (object) null;
          this.waitingForDrag = false;
          e.Handled = true;
        }
        else
        {
          this.draggedItem = (object) null;
          this.waitingForDrag = false;
        }
      }
      base.OnPreviewMouseMove(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      this.waitingForDrag = false;
      if (this.draggedItem != null)
        this.draggedItem = (object) null;
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnPreviewDragEnter(DragEventArgs e)
    {
      base.OnPreviewDragEnter(e);
      IList list = Gallery.GetList(this.FavoriteItems);
      if (list != null && !list.Contains(this.draggedItem))
        return;
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }

    protected override void OnPreviewDragLeave(DragEventArgs e)
    {
      base.OnPreviewDragLeave(e);
      this.dragOverItem = (object) null;
      this.OnPropertyChanged("DragOverItem");
    }

    protected override void OnPreviewDragOver(DragEventArgs e)
    {
      base.OnPreviewDragOver(e);
      IList list = Gallery.GetList(this.FavoriteItems);
      if (list == null || list.Contains(this.draggedItem))
      {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
      }
      else
      {
        FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
        if (frameworkElement == null)
          return;
        this.dragOverItem = frameworkElement.DataContext;
        this.OnPropertyChanged("DragOverItem");
      }
    }

    protected override void OnPreviewDrop(DragEventArgs e)
    {
      this.dragOverItem = (object) null;
      this.OnPropertyChanged("DragOverItem");
      IList list1 = Gallery.GetList(this.FavoriteItems);
      FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
      if (!list1.Contains(this.draggedItem) && this.draggedItem != null)
      {
        if (frameworkElement != null && frameworkElement.TemplatedParent != null)
        {
          object dataContext = ((FrameworkElement) frameworkElement.TemplatedParent).DataContext;
          if (dataContext != null && list1.Contains(dataContext))
          {
            int index = list1.IndexOf(dataContext);
            if (list1.Count >= this.MaxFavorites)
              list1.Remove(dataContext);
            list1.Insert(index, this.draggedItem);
            IList list2 = Gallery.GetList(this.MostRecentlyUsedItems);
            if (list2 != null && list2.Contains(this.draggedItem))
              list2.Remove(this.draggedItem);
          }
        }
      }
      base.OnPreviewDrop(e);
    }

    private static IList GetList(IEnumerable collection)
    {
      ICollectionView collectionView = collection as ICollectionView;
      if (collectionView != null)
        return collectionView.SourceCollection as IList;
      return collection as IList;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/resources/controls/gallery.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControlSelf = (Gallery) target;
          break;
        case 7:
          this.CategoryListBox = (SelectingListBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          ((Control) target).MouseDoubleClick += new MouseButtonEventHandler(this.GalleryItemDoubleClicked);
          break;
        case 3:
          ((Control) target).MouseDoubleClick += new MouseButtonEventHandler(this.GalleryItemDoubleClicked);
          break;
        case 4:
          ((Style) target).Setters.Add((SetterBase) new EventSetter()
          {
            Event = Control.MouseDoubleClickEvent,
            Handler = (Delegate) new MouseButtonEventHandler(this.GalleryItemDoubleClicked)
          });
          break;
        case 5:
          ((Expander) target).Expanded += new RoutedEventHandler(this.GroupExpander_Expanded);
          ((Expander) target).Collapsed += new RoutedEventHandler(this.GroupExpander_Collapsed);
          break;
        case 6:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.GroupExpander_NameClicked);
          break;
      }
    }

    public class GroupToggleExpandEventArgs : RoutedEventArgs
    {
      private string groupName;
      private bool expanded;

      public string GroupName
      {
        get
        {
          return this.groupName;
        }
        set
        {
          this.groupName = value;
        }
      }

      public bool Expanded
      {
        get
        {
          return this.expanded;
        }
        set
        {
          this.expanded = value;
        }
      }

      public GroupToggleExpandEventArgs(string groupName, bool expanded, Gallery gallery)
        : base(Gallery.GroupToggleExpandEvent, (object) gallery)
      {
        this.groupName = groupName;
        this.expanded = expanded;
      }
    }
  }
}

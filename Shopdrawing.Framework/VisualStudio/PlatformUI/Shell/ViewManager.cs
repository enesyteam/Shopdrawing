// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell.Preferences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class ViewManager
  {
    private ContentControl mainWindowContent;
    private HwndSource mainWindowHwndSource;
    private WindowProfile windowProfile;
    private bool isWindowProfileChanging;
    private bool isPendingActiveView;
    private ResourceDictionary theme;
    private View activeView;
    private View pendingActiveView;
    private static ViewManager instance;
    private ViewManagerPreferences preferences;

    private FloatingWindowManager FloatingWindowManager { get; set; }

    private AutoHideWindowManager AutoHideWindowManager { get; set; }

    private HwndSource MainWindowHwndSource
    {
      get
      {
        return this.mainWindowHwndSource;
      }
      set
      {
        if (this.mainWindowHwndSource == value)
          return;
        this.mainWindowHwndSource = value;
        this.FloatingWindowManager.OwnerWindow = this.MainWindowHandle;
        DockManager.Instance.UnregisterSite((Visual) this.mainWindowContent);
        DockManager.Instance.RegisterSite((Visual) this.mainWindowContent, this.MainWindowHandle);
        ApplicationActivationMonitor.Instance.HwndSource = this.mainWindowHwndSource;
      }
    }

    private IntPtr MainWindowHandle
    {
      get
      {
        if (this.MainWindowHwndSource != null)
          return this.MainWindowHwndSource.Handle;
        return IntPtr.Zero;
      }
    }

    public bool IsInitialized
    {
      get
      {
        return this.mainWindowContent != null;
      }
    }

    public ViewManagerPreferences Preferences
    {
      get
      {
        if (this.preferences == null)
          this.preferences = new ViewManagerPreferences();
        return this.preferences;
      }
    }

    public WindowProfile WindowProfile
    {
      get
      {
        return this.windowProfile;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        if (this.isWindowProfileChanging)
          throw new InvalidOperationException("ViewManager does not support reentrant calls to set_WindowProfile.");
        this.isWindowProfileChanging = true;
        try
        {
          if (value == this.windowProfile)
            return;
          using (this.DeferActiveViewChanges())
          {
            Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent<WindowProfileChangingEventArgs>(this.WindowProfileChanging, (object) this, new WindowProfileChangingEventArgs(this.windowProfile, value));
            if (this.windowProfile != null)
            {
              this.FloatingWindowManager.RemoveAllFloats(this.windowProfile);
              this.windowProfile.Children.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnViewSitesChanged);
            }
            MainSite mainSite1 = (MainSite) null;
            List<FloatSite> list = new List<FloatSite>();
            foreach (ViewGroup viewGroup in (IEnumerable<ViewElement>) value.Children)
            {
              MainSite mainSite2 = viewGroup as MainSite;
              if (mainSite2 != null)
                mainSite1 = mainSite2;
              else
                list.Add(viewGroup as FloatSite);
            }
            this.mainWindowContent.Content = (object) mainSite1;
            value.Children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnViewSitesChanged);
            foreach (FloatSite floatSite in list)
              this.FloatingWindowManager.AddFloat(floatSite);
            this.windowProfile = value;
            Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.WindowProfileChanged, (object) this);
            this.ActiveView = (View) null;
          }
        }
        finally
        {
          this.isWindowProfileChanging = false;
        }
      }
    }

    public ResourceDictionary Theme
    {
      get
      {
        return this.theme;
      }
      set
      {
        if (this.theme != null)
        {
          Application.Current.Resources.MergedDictionaries.Remove(this.theme);
          if (this.mainWindowContent != null)
            this.mainWindowContent.Resources.MergedDictionaries.Remove(this.theme);
        }
        this.theme = value;
        if (this.theme == null)
          return;
        Application.Current.Resources.MergedDictionaries.Add(this.theme);
        if (this.mainWindowContent == null)
          return;
        this.mainWindowContent.Resources.MergedDictionaries.Add(this.theme);
      }
    }

    public static ViewManager Instance
    {
      get
      {
        return ViewManager.instance ?? (ViewManager.instance = new ViewManager());
      }
      internal set
      {
        ViewManager.instance = value;
      }
    }

    public View ActiveView
    {
      get
      {
        return this.activeView;
      }
      set
      {
        if (this.isPendingActiveView)
        {
          this.pendingActiveView = value;
          if (value == null)
            return;
          value.IsActive = false;
        }
        else if (this.activeView != value)
        {
          if (value != null && value.WindowProfile != this.WindowProfile)
            throw new ArgumentException("The ViewManager.ActiveView must be a View contained within the ViewManager.WindowProfile.");
          if (this.activeView != null)
            this.activeView.IsActive = false;
          this.activeView = value;
          if (this.activeView != null)
          {
            this.activeView.IsActive = true;
            this.FloatingWindowManager.ActivateFloatingControl((ViewElement) this.activeView);
          }
          Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.ActiveViewChanged, (object) this);
        }
        else
        {
          if (this.activeView == null)
            return;
          this.activeView.IsActive = true;
        }
      }
    }

    public event EventHandler<WindowProfileChangingEventArgs> WindowProfileChanging;

    public event EventHandler WindowProfileChanged;

    public event EventHandler ActiveViewChanged;

    protected ViewManager()
    {
      EventManager.RegisterClassHandler(typeof (DragUndockHeader), DragUndockHeader.DragStartedEvent, (Delegate) new EventHandler<DragAbsoluteEventArgs>(this.OnViewHeaderDragStarted));
      EventManager.RegisterClassHandler(typeof (DragUndockHeader), DragUndockHeader.DragDeltaEvent, (Delegate) new DragDeltaEventHandler(this.OnViewHeaderDragDelta));
      EventManager.RegisterClassHandler(typeof (DragUndockHeader), DragUndockHeader.DragAbsoluteEvent, (Delegate) new EventHandler<DragAbsoluteEventArgs>(this.OnViewHeaderDragAbsolute));
      EventManager.RegisterClassHandler(typeof (DragUndockHeader), DragUndockHeader.DragCompletedAbsoluteEvent, (Delegate) new EventHandler<DragAbsoluteCompletedEventArgs>(this.OnViewHeaderDragCompleted));
      EventManager.RegisterClassHandler(typeof (DragUndockHeader), DragUndockHeader.DragHeaderClickedEvent, (Delegate) new RoutedEventHandler(this.OnViewHeaderClicked));
      EventManager.RegisterClassHandler(typeof (DockTarget), DockManager.FloatingElementDockedEvent, (Delegate) new EventHandler<FloatingElementDockedEventArgs>(this.OnFloatingElementDocked));
      EventManager.RegisterClassHandler(typeof (DocumentTabPanel), DocumentTabPanel.SelectedItemHiddenEvent, (Delegate) new EventHandler<SelectedItemHiddenEventArgs>(this.OnSelectedItemHidden));
      EventManager.RegisterClassHandler(typeof (AutoHideTabItem), MouseHover.MouseHoverEvent, (Delegate) new RoutedEventHandler(this.OnMouseHoverOverAutoHideTabItem));
      CommandManager.RegisterClassCommandBinding(typeof (AutoHideTabItem), new CommandBinding((ICommand) ViewCommands.ShowAutoHiddenView, new ExecutedRoutedEventHandler(this.OnShowAutoHiddenView)));
      CommandManager.RegisterClassCommandBinding(typeof (AutoHideTabItem), new CommandBinding((ICommand) ViewCommands.ShowAndActivateAutoHiddenView, new ExecutedRoutedEventHandler(this.OnShowAndActivateAutoHiddenView)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.HideViewCommand, new ExecutedRoutedEventHandler(this.OnHideView)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.HideViewInvertPreferenceCommand, new ExecutedRoutedEventHandler(this.OnHideViewInvertPreference)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.AutoHideViewCommand, new ExecutedRoutedEventHandler(this.OnAutoHideView)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.AutoHideViewInvertPreferenceCommand, new ExecutedRoutedEventHandler(this.OnAutoHideViewInvertPreference)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.NewHorizontalTabGroupCommand, new ExecutedRoutedEventHandler(this.OnNewHorizontalTabGroup)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.NewVerticalTabGroupCommand, new ExecutedRoutedEventHandler(this.OnNewVerticalTabGroup)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.MoveToNextTabGroupCommand, new ExecutedRoutedEventHandler(this.OnMoveToNextTabGroup)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.MoveToPreviousTabGroupCommand, new ExecutedRoutedEventHandler(this.OnMoveToPreviousTabGroup)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.ActivateDocumentViewCommand, new ExecutedRoutedEventHandler(this.OnActivateDocumentView)));
      CommandManager.RegisterClassCommandBinding(typeof (UIElement), new CommandBinding((ICommand) ViewCommands.ToggleDocked, new ExecutedRoutedEventHandler(this.OnToggleDocked)));
      EventManager.RegisterClassHandler(typeof (ViewPresenter), UIElement.PreviewGotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(this.OnViewPreviewGotKeyboardFocus));
      EventManager.RegisterClassHandler(typeof (ViewPresenter), UIElement.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnViewMouseDown));
      EventManager.RegisterClassHandler(typeof (DocumentGroupControl), UIElement.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnTabControlMouseDown));
      EventManager.RegisterClassHandler(typeof (DocumentGroupControl), Selector.SelectionChangedEvent, (Delegate) new SelectionChangedEventHandler(this.OnTabControlSelectionChanged));
      EventManager.RegisterClassHandler(typeof (TabGroupControl), UIElement.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnTabControlMouseDown));
      EventManager.RegisterClassHandler(typeof (TabGroupControl), Selector.SelectionChangedEvent, (Delegate) new SelectionChangedEventHandler(this.OnTabControlSelectionChanged));
      EventManager.RegisterClassHandler(typeof (FloatingWindow), FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnFloatingWindowSizeChanged));
      EventManager.RegisterClassHandler(typeof (FloatingWindow), FloatingWindow.LocationChangedEvent, (Delegate) new RoutedEventHandler(this.OnFloatingWindowLocationChanged));
    }

    protected virtual FloatingWindowManager CreateFloatingWindowManager()
    {
      return new FloatingWindowManager();
    }

    public void Initialize(ContentControl mainWindowContent)
    {
      if (mainWindowContent == null)
        throw new ArgumentNullException("mainWindowContent");
      this.mainWindowContent = mainWindowContent;
      this.FloatingWindowManager = this.CreateFloatingWindowManager();
      this.AutoHideWindowManager = new AutoHideWindowManager();
      this.MergeResources();
      this.InitializePresentationSource((UIElement) mainWindowContent);
    }

    private void InitializePresentationSource(UIElement content)
    {
      PresentationSource.AddSourceChangedHandler((IInputElement) content, new SourceChangedEventHandler(this.OnPresentationSourceChanged));
      this.UpdateMainWindowHandle(content);
    }

    private void UpdateMainWindowHandle(UIElement content)
    {
      this.MainWindowHwndSource = ScreenLocation.FindTopLevelHwndSource(content);
    }

    private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs args)
    {
      this.UpdateMainWindowHandle((UIElement) this.mainWindowContent);
    }

    private bool CanShowAutoHiddenView(ExecutedRoutedEventArgs args)
    {
      if (args.Parameter is View)
        return args.OriginalSource is AutoHideTabItem;
      return false;
    }

    private void OnShowAutoHiddenView(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanShowAutoHiddenView(args))
        return;
      AutoHideTabItem autoHideTabItem = args.OriginalSource as AutoHideTabItem;
      View view = (View) args.Parameter;
      ViewManager.instance.AutoHideWindowManager.ShowAutoHideWindow(autoHideTabItem, view);
    }

    private bool CanShowAndActivateAutoHiddenView(ExecutedRoutedEventArgs args)
    {
      if (args.Parameter is View)
        return args.OriginalSource is AutoHideTabItem;
      return false;
    }

    private void OnShowAndActivateAutoHiddenView(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanShowAndActivateAutoHiddenView(args))
        return;
      AutoHideTabItem autoHideTabItem = args.OriginalSource as AutoHideTabItem;
      View view = (View) args.Parameter;
      ViewManager.instance.ActiveView = view;
      ViewManager.instance.AutoHideWindowManager.ShowAutoHideWindow(autoHideTabItem, view);
    }

    private bool CanHideView(ExecutedRoutedEventArgs args)
    {
      return args.Parameter is ViewElement;
    }

    private bool ShouldHideTabGroup(ViewElement element, bool hideOnlyActiveView)
    {
      FloatSite floatSite = ViewElement.FindRootElement(element) as FloatSite;
      bool flag = floatSite != null && !floatSite.HasMultipleOnScreenViews;
      return element.Parent is TabGroup && (flag && this.Preferences.AlwaysHideAllViewInFloatingTabGroup || !hideOnlyActiveView);
    }

    private void OnHideViewCore(ViewElement closingViewElement, bool hideOnlyActiveView)
    {
      if (this.ShouldHideTabGroup(closingViewElement, hideOnlyActiveView))
        closingViewElement = (ViewElement) closingViewElement.Parent;
      foreach (View view in closingViewElement.FindAll((Predicate<ViewElement>) (e => e is View)))
        view.Hide();
    }

    private void OnHideView(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanHideView(args))
        return;
      this.OnHideViewCore((ViewElement) args.Parameter, this.Preferences.HideOnlyActiveView);
    }

    private void OnHideViewInvertPreference(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanHideView(args))
        return;
      this.OnHideViewCore((ViewElement) args.Parameter, !this.Preferences.HideOnlyActiveView);
    }

    private bool CanAutoHideView(ExecutedRoutedEventArgs args)
    {
      ViewElement element = args.Parameter as ViewElement;
      if (element == null)
        return false;
      if (!(element.Parent is AutoHideGroup))
        return DockOperations.CanAutoHide(element);
      return true;
    }

    protected virtual void OnAutoHideViewCore(ViewElement autoHidingElement, bool autoHideOnlyActiveView)
    {
      if (autoHidingElement.Parent is AutoHideGroup)
      {
        DockOperations.DockViewElementOrGroup(autoHidingElement, autoHideOnlyActiveView);
      }
      else
      {
        if (autoHideOnlyActiveView)
        {
          TabGroup tabGroup = autoHidingElement as TabGroup;
          if (tabGroup != null)
            autoHidingElement = tabGroup.SelectedElement;
        }
        DockOperations.AutoHide(autoHidingElement);
      }
    }

    private void OnAutoHideView(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanAutoHideView(args))
        return;
      this.OnAutoHideViewCore((ViewElement) args.Parameter, this.Preferences.AutoHideOnlyActiveView);
    }

    private void OnAutoHideViewInvertPreference(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanAutoHideView(args))
        return;
      this.OnAutoHideViewCore((ViewElement) args.Parameter, !this.Preferences.AutoHideOnlyActiveView);
    }

    private bool CanToggleDocked(ExecutedRoutedEventArgs args)
    {
      return args.Parameter is ViewElement;
    }

    private void OnToggleDocked(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanToggleDocked(args))
        return;
      ViewElement element = (ViewElement) args.Parameter;
      if (FloatSite.IsFloating(element))
        DockOperations.SnapToBookmark(element);
      else
        DockOperations.Float(element, element.WindowProfile);
    }

    private bool IsViewContainedInDocumentGroupContainer(object parameter)
    {
      View view = parameter as View;
      if (view != null)
      {
        DocumentGroup documentGroup = view.Parent as DocumentGroup;
        if (documentGroup != null)
          return documentGroup.Parent is DocumentGroupContainer;
      }
      return false;
    }

    private bool CanExecuteNewHorizontalTabGroup(ExecutedRoutedEventArgs args)
    {
      return this.IsViewContainedInDocumentGroupContainer(args.Parameter);
    }

    private void OnNewHorizontalTabGroup(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanExecuteNewHorizontalTabGroup(args))
        return;
      this.CreateDocumentGroup((ViewElement) args.Parameter, Orientation.Vertical);
    }

    private bool CanExecuteNewVerticalTabGroup(ExecutedRoutedEventArgs args)
    {
      return this.IsViewContainedInDocumentGroupContainer(args.Parameter);
    }

    private void OnNewVerticalTabGroup(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanExecuteNewVerticalTabGroup(args))
        return;
      this.CreateDocumentGroup((ViewElement) args.Parameter, Orientation.Horizontal);
    }

    private bool CanMoveToNextTabGroup(ExecutedRoutedEventArgs args)
    {
      return this.IsViewContainedInDocumentGroupContainer(args.Parameter);
    }

    private void OnMoveToNextTabGroup(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanMoveToNextTabGroup(args))
        return;
      this.MoveToTabGroup((ViewElement) args.Parameter, 1);
    }

    private bool CanMoveToPreviousTabGroup(ExecutedRoutedEventArgs args)
    {
      return this.IsViewContainedInDocumentGroupContainer(args.Parameter);
    }

    private void OnMoveToPreviousTabGroup(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanMoveToPreviousTabGroup(args))
        return;
      this.MoveToTabGroup((ViewElement) args.Parameter, -1);
    }

    private void MoveToTabGroup(ViewElement view, int tabGroupOffset)
    {
      DocumentGroup documentGroup1 = view.Parent as DocumentGroup;
      if (documentGroup1 == null)
        throw new InvalidOperationException("View that is being moved must be child of a DocumentGroup.");
      DocumentGroupContainer documentGroupContainer = documentGroup1.Parent as DocumentGroupContainer;
      if (documentGroupContainer == null)
        throw new InvalidOperationException("DocumentGroup must be child of a DocumentGroupContainer");
      List<ViewElement> list = new List<ViewElement>(documentGroupContainer.FindAll((Predicate<ViewElement>) (v => v is DocumentGroup)));
      int num = list.IndexOf((ViewElement) documentGroup1);
      if (num + tabGroupOffset < 0 || num + tabGroupOffset >= list.Count)
        return;
      DocumentGroup documentGroup2 = list[num + tabGroupOffset] as DocumentGroup;
      view.Detach();
      DockOperations.Dock((ViewElement) documentGroup2, view, DockDirection.Fill);
      documentGroup2.SelectedElement = view;
    }

    private bool CanActivateDocumentView(ExecutedRoutedEventArgs args)
    {
      return args.Parameter is View;
    }

    private void OnActivateDocumentView(object sender, ExecutedRoutedEventArgs args)
    {
      if (!this.CanActivateDocumentView(args))
        return;
      ((ViewElement) args.Parameter).IsSelected = true;
    }

    private void CreateDocumentGroup(ViewElement view, Orientation orientation)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      DocumentGroup documentGroup1 = view.Parent as DocumentGroup;
      if (documentGroup1 == null)
        throw new InvalidOperationException("View that is being moved must be child of a DocumentGroup.");
      DocumentGroupContainer documentGroupContainer = documentGroup1.Parent as DocumentGroupContainer;
      if (documentGroupContainer == null)
        throw new InvalidOperationException("DocumentGroup must be child of a DocumentGroupContainer");
      DocumentGroup documentGroup2 = DocumentGroup.Create();
      documentGroup2.FloatingHeight = 200.0;
      documentGroup2.FloatingWidth = 300.0;
      documentGroup2.IsVisible = true;
      documentGroup1.Children.Remove(view);
      documentGroup2.Children.Add(view);
      if (documentGroupContainer.VisibleChildren.Count <= 1)
        documentGroupContainer.Orientation = orientation;
      int num = documentGroupContainer.Children.IndexOf((ViewElement) documentGroup1);
      documentGroupContainer.Children.Insert(num + 1, (ViewElement) documentGroup2);
      documentGroup2.SelectedElement = view;
    }

    public IDisposable DeferActiveViewChanges()
    {
      return (IDisposable) new ViewManager.ActiveViewDeferrer();
    }

    private void OnMouseHoverOverAutoHideTabItem(object sender, RoutedEventArgs args)
    {
      AutoHideTabItem autoHideTabItem = args.OriginalSource as AutoHideTabItem;
      if (autoHideTabItem == null)
        return;
      View view = autoHideTabItem.DataContext as View;
      if (view == null)
        return;
      view.IsSelected = true;
    }

    internal static bool ShouldActivateFromFocusChange(RoutedEventArgs args)
    {
      FocusableHwndHost focusableHwndHost = args.OriginalSource as FocusableHwndHost;
      if (focusableHwndHost == null)
        return true;
      IntPtr lastFocusedHwnd = focusableHwndHost.LastFocusedHwnd;
      if (!(lastFocusedHwnd == IntPtr.Zero))
        return lastFocusedHwnd == focusableHwndHost.Handle;
      return true;
    }

    protected virtual void ActivateViewOnGotFocus(ViewPresenter presenter)
    {
      this.ActivateViewFromPresenter(presenter);
    }

    internal virtual void OnViewPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
    {
      if (!ViewManager.ShouldActivateFromFocusChange((RoutedEventArgs) args))
        return;
      this.ActivateViewFromPresenter(sender as ViewPresenter);
      args.Handled = true;
    }

    private void OnViewMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.ActivateViewFromPresenter(sender as ViewPresenter);
    }

    protected virtual void ActivateViewFromPresenter(ViewPresenter presenter)
    {
      if (presenter == null)
        return;
      View view = presenter.DataContext as View;
      if (view == null)
        return;
      ViewManager.Instance.ActiveView = view;
    }

    private void OnTabControlMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.ActivateViewFromTabControl((TabControl) sender);
    }

    private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      TabControl tabControl = (TabControl) sender;
      if (!this.DoesTabControlContainActiveView(tabControl))
        return;
      this.ActivateViewFromTabControl(tabControl);
    }

    private bool DoesTabControlContainActiveView(TabControl tabControl)
    {
      ViewGroup viewGroup = tabControl.DataContext as ViewGroup;
      if (viewGroup != null)
        return viewGroup.Children.Contains((ViewElement) ViewManager.Instance.ActiveView);
      return false;
    }

    private void ActivateViewFromTabControl(TabControl tabControl)
    {
      ViewGroup viewGroup = tabControl.DataContext as ViewGroup;
      if (viewGroup == null)
        return;
      View view = viewGroup.SelectedElement as View;
      if (view == null)
        return;
      ViewManager.Instance.ActiveView = view;
    }

    private bool IsAutoDockAllowed(ReorderTabPanel panel)
    {
      if (panel is DocumentTabPanel)
        return this.Preferences.AllowDocumentTabAutoDocking;
      return this.Preferences.AllowTabGroupTabAutoDocking;
    }

    private void OnViewHeaderDragStarted(object sender, DragAbsoluteEventArgs args)
    {
      DragUndockHeader dragUndockHeader = (DragUndockHeader) args.OriginalSource;
      if (dragUndockHeader.ViewElement != null && DockManager.Instance.DraggedViewElements.Count == 0)
        DockManager.Instance.SetDraggedViewElements(dragUndockHeader.ViewElement);
      if (!dragUndockHeader.IsWindowTitleBar && dragUndockHeader.ViewElement != null)
      {
        dragUndockHeader.CancelDrag();
        if (DockManager.Instance.DraggedTabInfo != null && this.IsAutoDockAllowed(DockManager.Instance.DraggedTabInfo.TabStrip))
        {
          if (-1 != DockManager.Instance.DraggedTabInfo.DraggedTabPosition)
            DockManager.Instance.DraggedTabInfo.RemoveTabRect(DockManager.Instance.DraggedTabInfo.DraggedTabPosition);
          DockManager.Instance.DraggedTabInfo.Initialize(dragUndockHeader.ViewElement);
          DockManager.Instance.DraggedTabInfo.DraggedTabPosition = -1;
        }
        else
          DockManager.Instance.DraggedTabInfo = (DraggedTabInfo) null;
        Rect currentUndockingRect = Rect.Empty;
        if (dragUndockHeader.ViewFrameworkElement != null && ExtensionMethods.IsConnectedToPresentationSource((DependencyObject) dragUndockHeader.ViewFrameworkElement))
          currentUndockingRect = new Rect(dragUndockHeader.ViewFrameworkElement.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(dragUndockHeader.ViewFrameworkElement.RenderSize));
        DockOperations.Undock(dragUndockHeader.ViewElement, dragUndockHeader.ViewElement.WindowProfile, args.ScreenPoint, currentUndockingRect);
      }
      DockManager.Instance.IsDragging = true;
    }

    private void OnViewHeaderDragDelta(object sender, DragDeltaEventArgs args)
    {
      DragUndockHeader dragUndockHeader = (DragUndockHeader) args.OriginalSource;
      if (!dragUndockHeader.IsWindowTitleBar)
        return;
      FloatingWindow ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<FloatingWindow>((Visual) dragUndockHeader);
      DpiHelper.SetDeviceLeft((Window) ancestor, DpiHelper.GetDeviceLeft((Window) ancestor) + args.HorizontalChange);
      DpiHelper.SetDeviceTop((Window) ancestor, DpiHelper.GetDeviceTop((Window) ancestor) + args.VerticalChange);
    }

    private void OnViewHeaderDragAbsolute(object sender, DragAbsoluteEventArgs args)
    {
      DragUndockHeader header = (DragUndockHeader) args.OriginalSource;
      if (header.IsWindowTitleBar)
      {
        this.HandleDragAbsoluteFloatingWindow(header, args);
      }
      else
      {
        if (DockManager.Instance.DraggedTabInfo == null)
          return;
        this.HandleDragAbsoluteMoveTabInPlace(header, args);
      }
    }

    private void OnViewHeaderDragCompleted(object sender, DragAbsoluteCompletedEventArgs args)
    {
      if (((DragUndockHeader) args.OriginalSource).IsWindowTitleBar && args.IsCompleted)
        DockManager.Instance.PerformDrop((DragAbsoluteEventArgs) args);
      else
        DockManager.Instance.ClearAdorners();
      if (!args.IsCompleted)
        return;
      DockOperations.ClearViewBookmarks(DockManager.Instance.DraggedViewElements);
      DockManager.Instance.DraggedTabInfo = (DraggedTabInfo) null;
      DockManager.Instance.IsDragging = false;
      DockManager.Instance.DraggedViewElements.Clear();
    }

    private void OnViewHeaderClicked(object sender, RoutedEventArgs args)
    {
      DragUndockHeader dragUndockHeader = args.OriginalSource as DragUndockHeader;
      ReorderTabPanel ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<ReorderTabPanel>((Visual) dragUndockHeader);
      if (ancestor != null)
      {
        DockManager.Instance.DraggedTabInfo = new DraggedTabInfo();
        DockManager.Instance.DraggedTabInfo.TabStrip = ancestor;
        DockManager.Instance.DraggedTabInfo.DraggedViewElement = dragUndockHeader.ViewElement;
        DockManager.Instance.DraggedTabInfo.MeasureTabStrip();
      }
      else
        DockManager.Instance.DraggedTabInfo = (DraggedTabInfo) null;
    }

    private void HandleDragAbsoluteFloatingWindow(DragUndockHeader header, DragAbsoluteEventArgs args)
    {
      DockManager.Instance.UpdateTargets(args);
      DraggedTabInfo autodockTarget = DockManager.Instance.GetAutodockTarget(args);
      bool flag = true;
      ViewGroup viewGroup = header.ViewElement as ViewGroup;
      if (viewGroup != null)
        flag = viewGroup.VisibleChildren.Count == 1;
      if (autodockTarget == null || !flag || DockManager.Instance.IsFloatingOverDockAdorner)
        return;
      ViewElement viewElement = header.ViewElement;
      this.HandleDockIntoTabStrip(autodockTarget, header, args);
      DockManager.Instance.DraggedTabInfo = autodockTarget;
      DockManager.Instance.DraggedTabInfo.DraggedViewElement = viewElement;
    }

    private void HandleDragAbsoluteMoveTabInPlace(DragUndockHeader header, DragAbsoluteEventArgs args)
    {
      DraggedTabInfo draggedTabInfo = DockManager.Instance.DraggedTabInfo;
      bool flag = draggedTabInfo.GetTabIndexAt(args.ScreenPoint) == draggedTabInfo.DraggedTabPosition;
      if (flag)
        draggedTabInfo.ClearVirtualTabRect();
      if (header.ViewElement == null && draggedTabInfo.DraggedViewElement == null)
        return;
      ViewElement tab = header.ViewElement ?? draggedTabInfo.DraggedViewElement;
      if (!draggedTabInfo.TabStripRect.Contains(args.ScreenPoint) || flag || draggedTabInfo.VirtualTabRect.Contains(args.ScreenPoint))
        return;
      int tabIndexAt = draggedTabInfo.GetTabIndexAt(args.ScreenPoint);
      if (-1 == tabIndexAt)
        return;
      DockOperations.MoveTab(tab, tabIndexAt);
      draggedTabInfo.SetVirtualTabRect(tabIndexAt);
      draggedTabInfo.MoveTabRect(draggedTabInfo.DraggedTabPosition, tabIndexAt);
      draggedTabInfo.DraggedTabPosition = tabIndexAt;
    }

    private void HandleDockIntoTabStrip(DraggedTabInfo tabInfo, DragUndockHeader header, DragAbsoluteEventArgs args)
    {
      int dockPosition = tabInfo.GetClosestTabIndexAt(args.ScreenPoint);
      if (-1 == dockPosition)
        return;
      ViewElement viewElement = tabInfo.TargetElement;
      if (viewElement == null && tabInfo.GroupContainer != null)
      {
        viewElement = (ViewElement) DockOperations.CreateDocumentGroupAt(tabInfo.GroupContainer, tabInfo.GroupPosition);
        viewElement.DockedHeight = tabInfo.GroupDockedHeight;
        viewElement.DockedWidth = tabInfo.GroupDockedWidth;
        viewElement.FloatingHeight = tabInfo.GroupFloatingHeight;
        viewElement.FloatingWidth = tabInfo.GroupFloatingWidth;
      }
      if (!DockOperations.AreDockRestrictionsFulfilled(header.ViewElement, viewElement))
        return;
      bool flag = false;
      ViewGroup viewGroup = tabInfo.NestedGroup as ViewGroup;
      if (viewGroup != null)
        flag = viewGroup.Children.Contains(header.ViewElement);
      if (!flag && tabInfo.TabRects.Count > 0 && args.ScreenPoint.X > tabInfo.TabRects[tabInfo.TabRects.Count - 1].Right)
        dockPosition = tabInfo.TabRects.Count;
      if (tabInfo.TabRects.Count == 0)
        dockPosition = 0;
      if (DockManager.Instance.DraggedTabInfo != null && -1 != DockManager.Instance.DraggedTabInfo.DraggedTabPosition)
        DockManager.Instance.DraggedTabInfo.RemoveTabRect(DockManager.Instance.DraggedTabInfo.DraggedTabPosition);
      DockOperations.DockAt(viewElement, header.ViewElement, dockPosition);
      tabInfo.TabStrip.IsNotificationNeeded = true;
      tabInfo.DraggedTabPosition = dockPosition;
      tabInfo.ClearVirtualTabRect();
      DockManager.Instance.ClearAdorners();
    }

    private void OnFloatingElementDocked(object sender, FloatingElementDockedEventArgs args)
    {
      if (args.Content == null)
        return;
      DockTarget dockTarget = (DockTarget) sender;
      if (args.CreateDocumentGroup)
      {
        DocumentGroup documentGroup = dockTarget.DataContext as DocumentGroup;
        DocumentGroupContainer container = dockTarget.DataContext as DocumentGroupContainer;
        if (documentGroup == null)
          documentGroup = container.Children[0] as DocumentGroup;
        if (container == null)
          container = documentGroup.Parent as DocumentGroupContainer;
        Orientation orientation = container.Orientation;
        int position = container.Children.IndexOf((ViewElement) documentGroup);
        switch (args.DockDirection)
        {
          case DockDirection.FirstValue:
            orientation = Orientation.Vertical;
            break;
          case DockDirection.Bottom:
            orientation = Orientation.Vertical;
            ++position;
            break;
          case DockDirection.Left:
            orientation = Orientation.Horizontal;
            break;
          case DockDirection.Right:
            orientation = Orientation.Horizontal;
            ++position;
            break;
        }
        container.Orientation = orientation;
        ViewElement viewElement = container.Children[0];
        DockOperations.Dock((ViewElement) DockOperations.CreateDocumentGroupAt(container, position), args.Content, DockDirection.Fill);
      }
      else if (dockTarget.DockTargetType == DockTargetType.Inside || dockTarget.DockTargetType == DockTargetType.CenterOnly || (dockTarget.DockTargetType == DockTargetType.SidesOnly || dockTarget.DockTargetType == DockTargetType.FillPreview))
      {
        ViewElement targetView = dockTarget.DataContext as ViewElement;
        DocumentGroup documentGroup = targetView as DocumentGroup;
        if (documentGroup != null && args.DockDirection != DockDirection.Fill)
          targetView = (ViewElement) documentGroup.Parent;
        DockOperations.Dock(targetView, args.Content, args.DockDirection);
      }
      else
        DockOperations.DockOutside(dockTarget.DataContext as ViewGroup, args.Content, args.DockDirection);
    }

    private void OnSelectedItemHidden(object sender, SelectedItemHiddenEventArgs args)
    {
      GroupControl groupControl = args.OriginalSource as GroupControl;
      if (groupControl == null)
        return;
      NestedGroup nestedGroup = groupControl.DataContext as NestedGroup;
      int position = 0;
      if (ViewManager.Instance.Preferences.DocumentDockPreference == DockPreference.DockAtEnd)
        position = args.LastVisiblePosition;
      DockOperations.MoveTab(nestedGroup.SelectedElement, position);
    }

    private void OnFloatingWindowSizeChanged(object sender, SizeChangedEventArgs args)
    {
      FloatingWindow floatingWindow = (FloatingWindow) sender;
      FloatSite site = floatingWindow.DataContext as FloatSite;
      if (site == null || floatingWindow.WindowState != WindowState.Normal)
        return;
      foreach (ViewElement viewElement in site.FindAll((Predicate<ViewElement>) (element =>
      {
        if (element.IsVisible)
          return element != site;
        return false;
      })))
      {
        viewElement.FloatingWidth = args.NewSize.Width;
        viewElement.FloatingHeight = args.NewSize.Height;
      }
    }

    private void OnFloatingWindowLocationChanged(object sender, RoutedEventArgs args)
    {
      FloatingWindow floatingWindow = (FloatingWindow) sender;
      FloatSite floatSite = floatingWindow.DataContext as FloatSite;
      if (floatSite == null || floatingWindow.WindowState != WindowState.Normal)
        return;
      foreach (ViewElement viewElement in floatSite.FindAll((Predicate<ViewElement>) (element =>
      {
        if (element.IsVisible)
          return !(element is FloatSite);
        return false;
      })))
      {
        viewElement.FloatingLeft = floatingWindow.Left;
        viewElement.FloatingTop = floatingWindow.Top;
      }
    }

    protected virtual void MergeResources()
    {
      (Application.Current ?? new Application()).Resources.MergedDictionaries.Add(ViewManager.LoadResourceValue<ResourceDictionary>("Workspaces/DataTemplates.xaml"));
    }

    internal static T LoadResourceValue<T>(string xamlName)
    {
      return (T) Application.LoadComponent(new Uri(Assembly.GetExecutingAssembly().GetName().Name + ";component/" + xamlName, UriKind.Relative));
    }

    private void OnViewSitesChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (args.OldItems != null)
      {
        foreach (object obj in (IEnumerable) args.OldItems)
        {
          FloatSite floatSite = obj as FloatSite;
          if (floatSite != null)
            this.FloatingWindowManager.RemoveFloat(floatSite);
        }
      }
      if (args.NewItems == null)
        return;
      foreach (object obj in (IEnumerable) args.NewItems)
      {
        FloatSite floatSite = obj as FloatSite;
        if (floatSite != null)
          this.FloatingWindowManager.AddFloat(floatSite);
      }
    }

    internal class ActiveViewDeferrer : IDisposable
    {
      private static int refCount;

      public ActiveViewDeferrer()
      {
        if (ViewManager.ActiveViewDeferrer.refCount == 0)
        {
          ViewManager.instance.pendingActiveView = ViewManager.instance.ActiveView;
          ViewManager.instance.isPendingActiveView = true;
        }
        ++ViewManager.ActiveViewDeferrer.refCount;
      }

      public void Dispose()
      {
        --ViewManager.ActiveViewDeferrer.refCount;
        if (ViewManager.ActiveViewDeferrer.refCount != 0)
          return;
        ViewManager.instance.isPendingActiveView = false;
        ViewManager.instance.ActiveView = ViewManager.instance.pendingActiveView;
      }
    }
  }
}

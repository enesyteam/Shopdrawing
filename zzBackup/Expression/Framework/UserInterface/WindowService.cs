// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.WindowService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Globalization;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell.Preferences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class WindowService : IWindowService, IOrderedViewProvider, INotifyPropertyChanged
  {
    private bool isFirstDisplay = true;
    private ApplicationWindow applicationWindow;
    private ICommandService commandService;
    private ICommandBarService commandBarService;
    private IViewService viewService;
    private ViewBridge viewBridge;
    private PaletteRegistry paletteRegistry;
    private IWorkspaceService workspaceService;
    private FrameworkElement mainBody;
    private string activeTheme;
    private IThemeCollection themes;
    private ExpressionViewManager dockingViewManager;

    public string Title
    {
      get
      {
        return this.applicationWindow.Title;
      }
      set
      {
        this.applicationWindow.Title = value;
      }
    }

    public bool IsEnabled
    {
      get
      {
        return this.applicationWindow.IsEnabled;
      }
      set
      {
        this.applicationWindow.IsEnabled = value;
      }
    }

    public bool IsVisible
    {
      get
      {
        return this.applicationWindow.Visibility == Visibility.Visible;
      }
      set
      {
        if (value)
        {
          if (this.isFirstDisplay)
          {
            this.applicationWindow.SetInitialWindowPlacement();
            this.isFirstDisplay = false;
          }
          this.applicationWindow.Show();
        }
        else
          this.applicationWindow.Hide();
      }
    }

    public Window MainWindow
    {
      get
      {
        return (Window) this.applicationWindow;
      }
    }

    public double WorkspaceZoom
    {
      get
      {
        return 1.0;
      }
      set
      {
      }
    }

    public PaletteRegistry PaletteRegistry
    {
      get
      {
        return this.paletteRegistry;
      }
    }

    public string ActiveTheme
    {
      get
      {
        return this.activeTheme;
      }
      set
      {
        if (!(this.activeTheme != value))
          return;
        this.activeTheme = value;
        this.UpdateActiveTheme();
        this.OnPropertyChanged("ActiveTheme");
      }
    }

    public IThemeCollection Themes
    {
      get
      {
        if (this.themes == null)
          this.themes = (IThemeCollection) new WindowService.ThemeCollection(this);
        return this.themes;
      }
    }

    public IViewCollection OrderedViews
    {
      get
      {
        if (this.viewBridge == null)
          return (IViewCollection) null;
        return this.viewBridge.OrderedViews;
      }
    }

    public event CancelEventHandler Closing;

    public event EventHandler ClosingCanceled;

    public event EventHandler Closed;

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler ThemeChanged;

    public event EventHandler StateChanged;

    public event EventHandler Initialized;

    public WindowService(IConfigurationObject configuration, ICommandBarService commandBarService, ICommandService commandService, IViewService viewService, IWorkspaceService workspaceService, IMessageDisplayService messageDisplayService, FrameworkElement mainWindowRootElement)
      : this(configuration, commandBarService, commandService, viewService, workspaceService, messageDisplayService, (IFeedbackService) null, mainWindowRootElement, (ResourceDictionary) null, new List<Theme>().AsReadOnly(), false)
    {
    }

    public WindowService(IConfigurationObject configuration, ICommandBarService commandBarService, ICommandService commandService, IDocumentService documentService, IViewService viewService, IWorkspaceService workspaceService, IMessageDisplayService messageDisplayService, FrameworkElement mainWindowRootElement)
      : this(configuration, commandBarService, commandService, viewService, workspaceService, messageDisplayService, (IFeedbackService) null, mainWindowRootElement, (ResourceDictionary) null, new List<Theme>().AsReadOnly(), false)
    {
    }

    public WindowService(IConfigurationObject configuration, ICommandBarService commandBarService, ICommandService commandService, IViewService viewService, IWorkspaceService workspaceService, IMessageDisplayService messageDisplayService, IFeedbackService feedbackService, FrameworkElement mainWindowRootElement, ResourceDictionary icons, ReadOnlyCollection<Theme> themes)
      : this(configuration, commandBarService, commandService, viewService, workspaceService, messageDisplayService, feedbackService, mainWindowRootElement, icons, themes, false)
    {
    }

    public WindowService(IConfigurationObject configuration, ICommandBarService commandBarService, ICommandService commandService, IDocumentService documentService, IViewService viewService, IWorkspaceService workspaceService, IMessageDisplayService messageDisplayService, IFeedbackService feedbackService, FrameworkElement mainWindowRootElement, ResourceDictionary icons, ReadOnlyCollection<Theme> themes)
      : this(configuration, commandBarService, commandService, viewService, workspaceService, messageDisplayService, feedbackService, mainWindowRootElement, icons, themes, false)
    {
    }

    public WindowService(IConfigurationObject configuration, ICommandBarService commandBarService, ICommandService commandService, IViewService viewService, IWorkspaceService workspaceService, IMessageDisplayService messageDisplayService, IFeedbackService feedbackService, FrameworkElement mainWindowRootElement, ResourceDictionary icons, ReadOnlyCollection<Theme> themes, bool suppressViewUI)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.WindowServiceConstructor);
      this.applicationWindow = new ApplicationWindow(configuration, feedbackService);
      this.applicationWindow.Closed += new EventHandler(this.ApplicationWindow_Closed);
      this.applicationWindow.Closing += new CancelEventHandler(this.ApplicationWindow_Closing);
      this.applicationWindow.ClosingCanceled += new EventHandler(this.ApplicationWindow_ClosingCanceled);
      this.applicationWindow.KeyDown += new KeyEventHandler(this.ApplicationWindow_KeyDown);
      this.applicationWindow.PreviewKeyDown += new KeyEventHandler(this.ApplicationWindow_PreviewKeyDown);
      this.applicationWindow.ThemeChanged += new EventHandler(this.ApplicationWindow_ThemeChanged);
      this.applicationWindow.StateChanged += new EventHandler(this.ApplicationWindow_StateChanged);
      this.applicationWindow.SourceInitialized += new EventHandler(this.ApplicationWindow_SourceInitialized);
      EventManager.RegisterClassHandler(typeof (GroupControl), UIElement.GotKeyboardFocusEvent, (Delegate) new KeyboardFocusChangedEventHandler(this.OnTabGroupGotKeyboardFocus));
      EventManager.RegisterClassHandler(typeof (DocumentGroupControl), UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.OnDocumentGroupKeyDownOrUp));
      EventManager.RegisterClassHandler(typeof (DocumentGroupControl), UIElement.KeyUpEvent, (Delegate) new KeyEventHandler(this.OnDocumentGroupKeyDownOrUp));
      this.commandService = commandService;
      this.viewService = viewService;
      this.workspaceService = workspaceService;
      if (icons != null)
        this.AddResourceDictionary(icons);
      if (themes.Count > 0)
      {
        foreach (ITheme theme in themes)
          this.Themes.Add(theme);
        if (configuration != null)
        {
          this.ActiveTheme = (string) configuration.GetProperty("ActiveTheme", (object) this.Themes[0].Name);
          int num = -1;
          for (int index = 0; index < this.Themes.Count; ++index)
          {
            if (this.Themes[index].Name == this.ActiveTheme)
            {
              num = index;
              break;
            }
          }
          if (feedbackService != null)
            feedbackService.SetData(25, num + 1);
        }
      }
      PerformanceUtility.MarkInterimStep(PerformanceEvent.WindowServiceConstructor, "Create PaletteRegistry");
      this.paletteRegistry = new PaletteRegistry(workspaceService);
      ViewElementFactory.Current = (ViewElementFactory) new ExpressionViewElementFactory();
      if (!suppressViewUI)
      {
        this.dockingViewManager = new ExpressionViewManager();
        ViewManager.Instance = (ViewManager) this.dockingViewManager;
        DockManager.Instance = (DockManager) new ExpressionDockManager();
        if (viewService != null)
          this.viewBridge = new ViewBridge(this.workspaceService, this.viewService, messageDisplayService);
      }
      PerformanceUtility.MarkInterimStep(PerformanceEvent.WindowServiceConstructor, "Create CommandBarService");
      this.commandBarService = commandBarService;
      TextBoxHelper.RegisterType(typeof (TextBox));
      PerformanceUtility.MarkInterimStep(PerformanceEvent.WindowServiceConstructor, "Load Window Content");
      this.mainBody = mainWindowRootElement;
      if (this.mainBody != null)
      {
        this.mainBody.DataContext = (object) this;
        FrameworkElement frameworkElement = this.commandBarService as FrameworkElement;
        if (frameworkElement != null)
        {
          frameworkElement.Name = "CommandBarService";
          frameworkElement.VerticalAlignment = VerticalAlignment.Top;
          frameworkElement.SetValue(Grid.ColumnSpanProperty, (object) 2);
          ((Panel) this.mainBody).Children.Insert(0, (UIElement) frameworkElement);
        }
      }
      PerformanceUtility.MarkInterimStep(PerformanceEvent.WindowServiceConstructor, "Hookup Tree to Window Content");
      this.applicationWindow.Content = (object) mainWindowRootElement;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.WindowServiceConstructor);
    }

    private void OnTabGroupGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      TabItem tabItem = e.NewFocus as TabItem;
      if (tabItem == null || !(tabItem.DataContext is Microsoft.VisualStudio.PlatformUI.Shell.View))
        return;
      this.ReturnFocus();
    }

    private void OnDocumentGroupKeyDownOrUp(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Home && e.Key != Key.End && (e.Key != Key.Prior && e.Key != Key.Next))
        return;
      e.Handled = true;
    }

    public void SetCommandService(ICommandBarService commandBarService, ICommandService commandService)
    {
      this.commandService = commandService;
      this.commandBarService = commandBarService;
      this.commandService.AddTarget((ICommandTarget) this.workspaceService);
    }

    private void ApplicationWindow_SourceInitialized(object sender, EventArgs e)
    {
      this.OnInitialized(e);
      ContentControl mainWindowContent = ((FrameworkElement) this.applicationWindow.Content).FindName("DockingControl") as ContentControl;
      ViewManager instance = ViewManager.Instance;
      instance.Initialize(mainWindowContent);
      Application.Current.Resources.MergedDictionaries.Add(ViewManager.LoadResourceValue<ResourceDictionary>("Workspaces/Themes/ExpressionDocking/ExpressionDockingCommon.xaml"));
      instance.Theme = ViewManager.LoadResourceValue<ResourceDictionary>("Workspaces/Themes/ExpressionDocking/generic.xaml");
      instance.Preferences.DocumentDockPreference = DockPreference.DockAtBeginning;
      instance.Preferences.TabDockPreference = DockPreference.DockAtEnd;
      instance.Preferences.AlwaysHideAllViewInFloatingTabGroup = false;
    }

    private void ApplicationWindow_Closed(object sender, EventArgs e)
    {
      this.OnClosed(e);
      if (Application.Current == null)
        return;
      Application.Current.Shutdown();
    }

    private void ApplicationWindow_Closing(object sender, CancelEventArgs e)
    {
      this.OnClosing(e);
    }

    private void OnClosed(EventArgs e)
    {
      if (this.Closed == null)
        return;
      this.Closed((object) this, e);
    }

    private void OnClosing(CancelEventArgs e)
    {
      if (this.Closing != null)
        this.Closing((object) this, e);
      if (e.Cancel || this.workspaceService == null)
        return;
      this.workspaceService.SaveConfiguration(false);
    }

    private void ApplicationWindow_ClosingCanceled(object sender, EventArgs e)
    {
      this.OnClosingCanceled(e);
    }

    private void OnClosingCanceled(EventArgs e)
    {
      if (this.ClosingCanceled == null)
        return;
      this.ClosingCanceled((object) this, e);
    }

    private void ApplicationWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (this.commandBarService == null || this.commandBarService.HasOpenSubmenu || e.Handled)
        return;
      this.HandleSwitchToDialog(e);
    }

    private void ApplicationWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (this.commandBarService == null || this.commandBarService.HasOpenSubmenu)
        return;
      if (!e.Handled && Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F4)
      {
        this.commandService.Execute("Application_FileClose", CommandInvocationSource.AcceleratorKey);
        e.Handled = true;
      }
      if (!e.Handled)
        this.HandleSwitchToDialog(e);
      if (e.Handled)
        return;
      Key shortcutKey = e.Key;
      ModifierKeys modifiers = Keyboard.Modifiers;
      if (shortcutKey == Key.Back)
        shortcutKey = Key.Delete;
      if (shortcutKey == Key.Add)
        shortcutKey = Key.OemPlus;
      if (shortcutKey == Key.Subtract)
        shortcutKey = Key.OemMinus;
      if (shortcutKey == Key.System && (modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        shortcutKey = e.SystemKey;
      if (!this.commandService.HandleShortcut(shortcutKey, modifiers))
        return;
      e.Handled = true;
      CultureManager.ClearDeadKeyBuffer();
    }

    private void HandleSwitchToDialog(KeyEventArgs e)
    {
      if (Keyboard.Modifiers != ModifierKeys.Control || e.Key != Key.Tab || this.viewService == null)
        return;
      this.ShowSwitchToDialog();
      e.Handled = true;
    }

    public void ShowSwitchToDialog()
    {
      if (!SwitchToDialog.IsInUse)
      {
        FocusManager.SetFocusedElement((DependencyObject) this.applicationWindow, (IInputElement) this.applicationWindow);
        new SwitchToDialog((IWindowService) this, this.viewService).ShowDialog();
      }
      else
        SwitchToDialog.FocusCurrentInstance();
    }

    public PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption)
    {
      return this.paletteRegistry.Add(identifier, content, caption, (KeyBinding) null);
    }

    public PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption, KeyBinding keyBinding)
    {
      return this.paletteRegistry.Add(identifier, content, caption, keyBinding, new ExpressionViewProperties(true));
    }

    public PaletteRegistryEntry RegisterPalette(string identifier, FrameworkElement content, string caption, KeyBinding keyBinding, ExpressionViewProperties viewProperties)
    {
      return this.paletteRegistry.Add(identifier, content, caption, keyBinding, viewProperties);
    }

    public void UnregisterPalette(string paletteName)
    {
      this.paletteRegistry.Remove(paletteName);
    }

    private void ApplicationWindow_ThemeChanged(object sender, EventArgs e)
    {
      if (this.themes == null)
        return;
      foreach (ITheme theme in (IEnumerable) this.themes)
      {
        if (theme.IsSystemTheme)
          theme.Reset();
      }
      this.UpdateActiveTheme();
    }

    private void ApplicationWindow_StateChanged(object sender, EventArgs e)
    {
      this.OnStateChanged(EventArgs.Empty);
    }

    public void UpdateActiveTheme()
    {
      bool flag = false;
      this.EnsureMergedDictionaries();
      foreach (ITheme theme in (IEnumerable) this.themes)
      {
        if (this.activeTheme == theme.Name)
        {
          Application.Current.Resources.MergedDictionaries[0] = theme.ResourceDictionary;
          flag = true;
        }
      }
      if (!flag && !string.IsNullOrEmpty(this.activeTheme) && this.themes.Count != 0)
      {
        Application.Current.Resources.MergedDictionaries[0] = this.themes[0].ResourceDictionary;
        flag = true;
      }
      if (!flag)
        Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary();
      this.OnThemeChanged(EventArgs.Empty);
      string familyName = (string) Application.Current.TryFindResource((object) "DefaultUIFont");
      if (familyName == null)
        return;
      FontFamily fontFamily = new FontFamily(familyName);
      if (!fontFamily.FamilyNames.Values.Contains(familyName) && Directory.Exists(ExpressionApplication.CommonFontsLocation))
        fontFamily = new FontFamily(Path.Combine(ExpressionApplication.CommonFontsLocation, "#" + familyName));
      Application.Current.Resources[(object) SystemFonts.IconFontFamilyKey] = (object) fontFamily;
      Application.Current.Resources[(object) SystemFonts.MessageFontFamilyKey] = (object) fontFamily;
      Application.Current.Resources[(object) SystemFonts.CaptionFontFamilyKey] = (object) fontFamily;
    }

    internal void EnsureMergedDictionaries()
    {
      if (Application.Current.Resources == null)
        Application.Current.Resources = new ResourceDictionary();
      if (Application.Current.Resources.MergedDictionaries.Count != 0)
        return;
      Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary());
    }

    public void AddResourceDictionary(ResourceDictionary dictionary)
    {
      this.EnsureMergedDictionaries();
      Application.Current.Resources.MergedDictionaries.Add(dictionary);
    }

    public void RemoveResourceDictionary(ResourceDictionary dictionary)
    {
      this.EnsureMergedDictionaries();
      int index = Application.Current.Resources.MergedDictionaries.IndexOf(dictionary);
      if (index < 0)
        return;
      Application.Current.Resources.MergedDictionaries[index] = new ResourceDictionary();
    }

    public IDisposable SuppressViewActivationOnGotFocus()
    {
      if (this.dockingViewManager != null)
        return this.dockingViewManager.SuppressViewActivationOnGotFocus();
      return (IDisposable) null;
    }

    public void ReturnFocus()
    {
      FocusScopeManager.Instance.ReturnFocus();
    }

    public void Focus()
    {
      this.applicationWindow.Focus();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnThemeChanged(EventArgs e)
    {
      if (this.ThemeChanged == null)
        return;
      this.ThemeChanged((object) this, e);
    }

    private void OnStateChanged(EventArgs e)
    {
      if (this.StateChanged == null)
        return;
      this.StateChanged((object) this, e);
    }

    private void OnInitialized(EventArgs e)
    {
      if (this.Initialized == null)
        return;
      this.Initialized((object) this, e);
    }

    private class ThemeCollection : IThemeCollection, ICollection, IEnumerable
    {
      private ArrayList list = new ArrayList();
      private WindowService windowService;

      public ITheme this[int index]
      {
        get
        {
          return (ITheme) this.list[index];
        }
      }

      public int Count
      {
        get
        {
          return this.list.Count;
        }
      }

      public bool IsSynchronized
      {
        get
        {
          return this.list.IsSynchronized;
        }
      }

      public object SyncRoot
      {
        get
        {
          return this.list.SyncRoot;
        }
      }

      public ThemeCollection(WindowService windowService)
      {
        this.windowService = windowService;
      }

      public void Add(ITheme value)
      {
        this.list.Add((object) value);
        this.windowService.UpdateActiveTheme();
      }

      public void Remove(ITheme value)
      {
        this.list.Remove((object) value);
        this.windowService.UpdateActiveTheme();
      }

      public IEnumerator GetEnumerator()
      {
        return this.list.GetEnumerator();
      }

      public void CopyTo(Array array, int index)
      {
        this.list.CopyTo(array, index);
      }
    }
  }
}

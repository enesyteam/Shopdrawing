// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ScreenPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public abstract class ScreenPickerEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private IProjectContext activeProjectContext;
    private IPrototypingScreen currentScreenInfo;
    private DesignerContext designerContext;
    private SceneNodeProperty editingProperty;
    private readonly List<IPrototypingScreen> screens;
    private SceneViewModel viewModel;
    internal ScreenPickerEditor ScreenPickerEditorControl;
    private bool _contentLoaded;

    private IProjectContext ActiveProjectContext
    {
      get
      {
        return this.activeProjectContext;
      }
      set
      {
        if (this.activeProjectContext != value)
          this.screens.Clear();
        if (this.activeProjectContext != null)
          this.activeProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        this.activeProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.ProjectContext_TypesChanged);
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    protected virtual IPrototypingScreen DefaultValue
    {
      get
      {
        return (IPrototypingScreen) null;
      }
    }

    public string CurrentScreen
    {
      get
      {
        if (this.EditingProperty == null || this.EditingProperty.get_PropertyValue() == null)
          return (string) null;
        IPrototypingScreen prototypingScreen = !string.IsNullOrEmpty((string) this.EditingProperty.get_PropertyValue().get_Value()) ? this.FindScreenByName(this.EditingProperty.get_PropertyValue().get_Value() as string) : this.DefaultValue;
        if (prototypingScreen != null)
          return prototypingScreen.DisplayName;
        return string.Empty;
      }
      set
      {
        if (this.EditingProperty == null || this.EditingProperty.get_PropertyValue() == null)
          return;
        IPrototypingScreen screenByName = this.FindScreenByName(value);
        if (screenByName == null || screenByName == this.DefaultValue)
          this.EditingProperty.ClearValue();
        else
          this.EditingProperty.get_PropertyValue().set_Value((object) screenByName.ClassName);
      }
    }

    private IPrototypingScreen CurrentScreenInfo
    {
      get
      {
        return this.currentScreenInfo;
      }
      set
      {
        if (this.currentScreenInfo != null)
          this.currentScreenInfo.PropertyChanged -= new PropertyChangedEventHandler(this.CurrentScreenInfo_PropertyChanged);
        this.currentScreenInfo = value;
        if (this.currentScreenInfo == null)
          return;
        this.currentScreenInfo.PropertyChanged += new PropertyChangedEventHandler(this.CurrentScreenInfo_PropertyChanged);
      }
    }

    private SceneNodeProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
      set
      {
        if (this.editingProperty != null)
          this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.ScreenNameProperty_PropertyReferenceChanged);
        this.CurrentScreenInfo = (IPrototypingScreen) null;
        this.editingProperty = (SceneNodeProperty) null;
        this.viewModel = (SceneViewModel) null;
        SceneNodeProperty sceneNodeProperty = value;
        if (sceneNodeProperty == null || sceneNodeProperty.ObjectSet == null || (sceneNodeProperty.ObjectSet.DesignerContext == null || sceneNodeProperty.SceneNodeObjectSet == null) || sceneNodeProperty.SceneNodeObjectSet.ViewModel == null)
          return;
        this.editingProperty = sceneNodeProperty;
        this.viewModel = this.EditingProperty.SceneNodeObjectSet.ViewModel;
        this.EditingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.ScreenNameProperty_PropertyReferenceChanged);
        this.CurrentScreenInfo = this.FindScreenByName(this.editingProperty.get_PropertyValue().get_Value() as string);
      }
    }

    protected IPrototypingService PrototypingService
    {
      get
      {
        if (this.designerContext == null)
          return (IPrototypingService) null;
        return this.designerContext.PrototypingService;
      }
    }

    public ICollectionView ScreensView { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public ScreenPickerEditor()
    {
      this.screens = new List<IPrototypingScreen>();
      this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.ScreenPickerEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.ScreenPickerEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.ScreenPickerEditor_Unloaded);
    }

    private void UpdateFromDataContext()
    {
      this.EditingProperty = (SceneNodeProperty) null;
      PropertyValue propertyValue = this.DataContext as PropertyValue;
      if (propertyValue == null)
        return;
      this.EditingProperty = propertyValue.get_ParentProperty() as SceneNodeProperty;
      if (this.EditingProperty == null || this.EditingProperty.SceneNodeObjectSet == null)
      {
        this.EditingProperty = (SceneNodeProperty) null;
      }
      else
      {
        if (this.designerContext == null)
          this.designerContext = this.EditingProperty.SceneNodeObjectSet.DesignerContext;
        this.ActiveProjectContext = this.EditingProperty.SceneNodeObjectSet.ProjectContext;
        this.EditingProperty.Recache();
        this.Rebuild();
      }
    }

    private void Rebuild()
    {
      if (this.EditingProperty == null || this.designerContext == null || this.designerContext.PrototypingService == null)
        return;
      IPrototypingScreen activeScreen = this.designerContext.PrototypingService.ActiveScreen;
      if (activeScreen == null)
        return;
      IEnumerable<IPrototypingScreen> screensToDisplay = this.FindScreensToDisplay(activeScreen);
      this.screens.Clear();
      this.screens.AddRange(screensToDisplay);
      this.CurrentScreenInfo = this.FindScreenByName(this.editingProperty.get_PropertyValue().get_Value() as string);
      this.ScreensView = CollectionViewSource.GetDefaultView(this.screens);
      this.NotifyPropertyChanged("CurrentScreen");
      this.NotifyPropertyChanged("ScreensView");
    }

    private IPrototypingScreen FindScreenByName(string name)
    {
      return Enumerable.FirstOrDefault<IPrototypingScreen>((IEnumerable<IPrototypingScreen>) this.screens, (Func<IPrototypingScreen, bool>) (screenInfo => screenInfo.ClassName == name)) ?? Enumerable.FirstOrDefault<IPrototypingScreen>((IEnumerable<IPrototypingScreen>) this.screens, (Func<IPrototypingScreen, bool>) (screenInfo => screenInfo.DisplayName == name));
    }

    protected abstract IEnumerable<IPrototypingScreen> FindScreensToDisplay(IPrototypingScreen activeScreen);

    private void CurrentScreenInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "DisplayName"))
        return;
      this.NotifyPropertyChanged("CurrentScreen");
    }

    private void ProjectContext_TypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.screens.Clear();
      this.Rebuild();
    }

    private void ScreenNameProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void ScreenPickerEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.Rebuild();
    }

    private void ScreenPickerEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void ScreenPickerEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.EditingProperty = (SceneNodeProperty) null;
      this.ActiveProjectContext = (IProjectContext) null;
    }

    private void NotifyPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/screenpickereditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ScreenPickerEditorControl = (ScreenPickerEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}

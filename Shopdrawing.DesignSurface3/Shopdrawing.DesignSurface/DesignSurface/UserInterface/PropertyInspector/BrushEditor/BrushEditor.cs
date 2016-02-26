// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.BrushEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class BrushEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty ShouldShowNullBrushTabProperty = DependencyProperty.Register("ShouldShowNullBrushTab", typeof (bool), typeof (BrushEditor), new PropertyMetadata((object) true));
    private static readonly RoutedCommand onResourceSelectCommand = new RoutedCommand("OnResourceSelectCommand", typeof (BrushEditor));
    private static readonly RoutedCommand onResourceEditCommand = new RoutedCommand("OnEditResourceCommand", typeof (BrushEditor));
    private SceneNodeProperty editingProperty;
    private BrushSubtypeEditor brushSubtypeEditor;
    private bool editingResource;
    private bool oldNichedState;
    private ObservableCollectionAggregator advancedPropertiesAggregated;
    private ObservableCollection<PropertyEntry> advancedBrushProperties;
    private bool resourcePopupOpen;
    private WorkaroundPopup resourceEditorPopup;
    internal BrushEditor BrushEditorControl;
    internal FocusReturningWorkaroundRadioButton BrushResourcePickerButton;
    internal OnDemandControl ResourceList;
    internal StandardCategoryLayout AdvancedBrushProperties;
    private bool _contentLoaded;

    public static RoutedCommand OnResourceSelectCommand
    {
      get
      {
        return BrushEditor.onResourceSelectCommand;
      }
    }

    public static RoutedCommand OnEditResourceCommand
    {
      get
      {
        return BrushEditor.onResourceEditCommand;
      }
    }

    public BrushSubtypeEditor BrushSubtypeEditor
    {
      get
      {
        return this.brushSubtypeEditor;
      }
      private set
      {
        if (this.brushSubtypeEditor == value)
          return;
        if (this.brushSubtypeEditor != null)
        {
          this.advancedPropertiesAggregated.RemoveCollection((IList) this.brushSubtypeEditor.AdvancedProperties);
          this.brushSubtypeEditor.Disassociate();
        }
        this.brushSubtypeEditor = value;
        if (this.brushSubtypeEditor != null)
          this.advancedPropertiesAggregated.AddCollection((IList) this.brushSubtypeEditor.AdvancedProperties);
        this.OnPropertyChanged("BrushSubtypeEditor");
      }
    }

    public SceneNodeProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
    }

    public bool ShouldShowNullBrushTab
    {
      get
      {
        return (bool) this.GetValue(BrushEditor.ShouldShowNullBrushTabProperty);
      }
      set
      {
        this.SetValue(BrushEditor.ShouldShowNullBrushTabProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public ObservableCollectionAggregator AdvancedProperties
    {
      get
      {
        return this.advancedPropertiesAggregated;
      }
    }

    public bool IsNullBrush
    {
      get
      {
        return this.IsEditingCategory(BrushCategory.NullBrush);
      }
      set
      {
        if (!value)
          return;
        this.ChangeBrushType(BrushCategory.NullBrush);
      }
    }

    public bool IsSolidColorBrush
    {
      get
      {
        return this.IsEditingCategory(BrushCategory.SolidColor);
      }
      set
      {
        if (!value)
          return;
        this.ChangeBrushType(BrushCategory.SolidColor);
      }
    }

    public bool IsGradientBrush
    {
      get
      {
        if (!this.IsEditingCategory(BrushCategory.LinearGradient))
          return this.IsEditingCategory(BrushCategory.RadialGradient);
        return true;
      }
      set
      {
        if (!value)
          return;
        this.ChangeBrushType(BrushCategory.Gradient);
      }
    }

    public bool IsTileBrush
    {
      get
      {
        if (!this.IsEditingCategory(BrushCategory.Drawing) && !this.IsEditingCategory(BrushCategory.Visual) && !this.IsEditingCategory(BrushCategory.Image))
          return this.IsEditingCategory(BrushCategory.Video);
        return true;
      }
      set
      {
        if (!value)
          return;
        this.ChangeBrushType(BrushCategory.Tile);
      }
    }

    public bool IsEditingResource
    {
      get
      {
        return this.editingResource;
      }
      set
      {
        if (!value)
          return;
        this.BrushSubtypeEditor = (BrushSubtypeEditor) null;
        this.editingResource = true;
        this.editingProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
        this.OnEditingTypeChange();
      }
    }

    public bool HasAdvancedProperties
    {
      get
      {
        IEnumerable enumerable = (IEnumerable) this.advancedPropertiesAggregated;
        if (enumerable != null)
          return enumerable.GetEnumerator().MoveNext();
        return false;
      }
    }

    public bool IsResourcePopupOpen
    {
      get
      {
        return this.resourcePopupOpen;
      }
      private set
      {
        this.resourcePopupOpen = value;
        this.OnPropertyChanged("IsResourcePopupOpen");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public BrushEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnBrushEditorDataContextChanged);
      this.Loaded += new RoutedEventHandler(this.OnBrushEditorLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnBrushEditorUnloaded);
      this.advancedPropertiesAggregated = new ObservableCollectionAggregator();
      this.advancedBrushProperties = new ObservableCollection<PropertyEntry>();
      this.advancedPropertiesAggregated.AddCollection((IList) this.advancedBrushProperties);
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) BrushEditor.OnResourceSelectCommand, new ExecutedRoutedEventHandler(BrushEditor.OnResourceSelect)));
      this.CommandBindings.Add(new CommandBinding((ICommand) BrushEditor.OnEditResourceCommand, new ExecutedRoutedEventHandler(this.OnEditResource)));
    }

    private bool IsEditingCategory(BrushCategory brushCategory)
    {
      if (this.brushSubtypeEditor != null && this.brushSubtypeEditor.Category == brushCategory)
        return !this.editingResource;
      return false;
    }

    private void OnBrushEditorUnloaded(object sender, RoutedEventArgs e)
    {
      this.ResetEditingProperty();
      this.Rebuild();
    }

    private void OnBrushEditorLoaded(object sender, RoutedEventArgs e)
    {
      this.UpdateBrushEditorFromDataContext();
    }

    private void OnBrushEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateBrushEditorFromDataContext();
    }

    private void UpdateBrushEditorFromDataContext()
    {
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
      if (propertyValue != null)
        sceneNodeProperty = (SceneNodeProperty) propertyValue.ParentProperty;
      if (sceneNodeProperty == this.editingProperty)
        return;
      BrushCategory.ResetLastUsedBrushes();
      this.ResetEditingProperty();
      this.editingProperty = sceneNodeProperty;
      if (this.editingProperty != null)
        this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnBrushChanged);
      this.OnPropertyChanged("EditingProperty");
      this.Rebuild();
    }

    private void ResetEditingProperty()
    {
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnBrushChanged);
        this.editingProperty = (SceneNodeProperty) null;
        this.editingResource = false;
      }
      this.BrushSubtypeEditor = (BrushSubtypeEditor) null;
    }

    private void OnBrushChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (e.PropertyReference.CompareTo((object) this.editingProperty.Reference) != 0 && this.oldNichedState == this.editingProperty.IsMixedValue || e.DirtyViewState == SceneViewModel.ViewStateBits.CurrentValues && !this.editingProperty.IsMixedValue)
        return;
      this.Rebuild();
    }

    private void Rebuild()
    {
      if (this.editingProperty == null)
      {
        this.BrushSubtypeEditor = (BrushSubtypeEditor) null;
      }
      else
      {
        if (!this.editingProperty.Associated)
          return;
        this.oldNichedState = this.editingProperty.IsMixedValue;
        this.editingResource = false;
        bool flag = this.brushSubtypeEditor is BrushEditor.NullBrushEditor;
        if (this.editingProperty.IsResource)
        {
          this.editingResource = true;
          this.editingProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
          this.BrushSubtypeEditor = (BrushSubtypeEditor) null;
          this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (o =>
          {
            if (this.editingProperty != null && this.ResourceList.IsLoaded && this.Parent != null)
            {
              this.ResourceList.ApplyTemplate();
              ItemsControl itemsControl = (ItemsControl) this.ResourceList.Template.FindName("ResourcesControl", (FrameworkElement) this.ResourceList);
              itemsControl.ApplyTemplate();
              ItemsPresenter itemsPresenter = (ItemsPresenter) itemsControl.Template.FindName("ResourcesItemsPresenter", (FrameworkElement) itemsControl);
              itemsPresenter.ApplyTemplate();
              WorkaroundVirtualizingStackPanel virtualizingStackPanel = (WorkaroundVirtualizingStackPanel) itemsControl.ItemsPanel.FindName("VirtualizingStackPanel", (FrameworkElement) itemsPresenter);
              int index = -1;
              if (this.editingProperty.SelectedLocalResourceModel != null)
              {
                this.editingProperty.SelectedLocalResourceModel.Parent.IsExpanded = true;
                index = itemsControl.Items.IndexOf((object) this.editingProperty.SelectedLocalResourceModel);
              }
              else if (this.editingProperty.SelectedSystemResourceModel != null)
              {
                this.editingProperty.SelectedSystemResourceModel.Parent.IsExpanded = true;
                index = itemsControl.Items.IndexOf((object) this.editingProperty.SelectedSystemResourceModel);
              }
              if (index >= 0)
                virtualizingStackPanel.BringIndexIntoViewWorkaround(index);
            }
            return (object) null;
          }), (object) null);
        }
        else if (this.editingProperty.IsMixedValue)
        {
          this.BrushSubtypeEditor = (BrushSubtypeEditor) null;
        }
        else
        {
          ITypeId typeId = (ITypeId) this.editingProperty.ComputedValueTypeId;
          if (typeId != null)
          {
            if (typeId.Equals((object) PlatformTypes.SolidColorBrush) && !(this.BrushSubtypeEditor is SolidColorBrushEditor))
              this.BrushSubtypeEditor = (BrushSubtypeEditor) new SolidColorBrushEditor(this, this.editingProperty);
            else if (PlatformTypes.GradientBrush.IsAssignableFrom(typeId))
            {
              if (this.BrushSubtypeEditor == null || !typeId.Equals((object) this.BrushSubtypeEditor.Category.Type))
                this.BrushSubtypeEditor = (BrushSubtypeEditor) new GradientBrushEditor(this, typeId, this.editingProperty);
            }
            else if (PlatformTypes.TileBrush.IsAssignableFrom(typeId) && (this.BrushSubtypeEditor == null || !typeId.Equals((object) this.BrushSubtypeEditor.Category.Type)))
              this.BrushSubtypeEditor = (BrushSubtypeEditor) new TileBrushEditor(this, typeId, this.editingProperty);
            if (this.BrushSubtypeEditor != null)
              BrushCategory.SetLastUsed(this.BrushSubtypeEditor.Category, this.editingProperty.GetValue());
          }
          else
            this.BrushSubtypeEditor = (BrushSubtypeEditor) new BrushEditor.NullBrushEditor();
        }
        if (!(this.brushSubtypeEditor is BrushEditor.NullBrushEditor) && flag)
          this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (o =>
          {
            this.BringIntoView();
            return (object) null;
          }), (object) null);
        this.OnEditingTypeChange();
      }
    }

    private void OnEditingTypeChange()
    {
      this.OnPropertyChanged("IsEditingResource");
      this.OnPropertyChanged("IsNullBrush");
      this.OnPropertyChanged("IsSolidColorBrush");
      this.OnPropertyChanged("IsGradientBrush");
      this.OnPropertyChanged("IsTileBrush");
      this.OnPropertyChanged("HasAdvancedProperties");
    }

    private void ChangeBrushType(BrushCategory brushCategory)
    {
      if (this.editingProperty == null || this.IsEditingCategory(brushCategory))
        return;
      ITypeId type = (ITypeId) this.editingProperty.ComputedValueTypeId;
      bool flag = false;
      if (type != null && this.editingProperty.IsResource && (type.Equals((object) brushCategory.Type) || PlatformTypes.GradientBrush.IsAssignableFrom(type) && brushCategory.DefaultBrush is GradientBrush || PlatformTypes.TileBrush.IsAssignableFrom(type) && brushCategory.DefaultBrush is TileBrush))
        flag = true;
      if (this.brushSubtypeEditor != null)
        BrushCategory.SetLastUsed(this.brushSubtypeEditor.Category, this.editingProperty.GetValue());
      if (flag)
        this.editingProperty.DoSetLocalValue();
      else
        this.editingProperty.SetValue(BrushCategory.GetLastUsed(this.editingProperty.SceneNodeObjectSet.DesignerContext, this.editingProperty.SceneNodeObjectSet.DocumentContext, brushCategory));
      this.editingProperty.Recache();
      this.Rebuild();
    }

    private static void OnResourceSelect(object sender, ExecutedRoutedEventArgs e)
    {
      BrushEditor brushEditor = sender as BrushEditor;
      if (brushEditor == null)
        return;
      VirtualizingResourceItem<LocalResourceModel> virtualizingResourceItem1 = e.Parameter as VirtualizingResourceItem<LocalResourceModel>;
      VirtualizingResourceItem<SystemResourceModel> virtualizingResourceItem2 = e.Parameter as VirtualizingResourceItem<SystemResourceModel>;
      if (virtualizingResourceItem1 != null)
      {
        brushEditor.EditingProperty.DoSetToLocalResource(virtualizingResourceItem1.Model.PropertyModel);
        e.Handled = true;
      }
      else
      {
        if (virtualizingResourceItem2 == null)
          return;
        brushEditor.EditingProperty.DoSetToSystemResource(virtualizingResourceItem2.Model.PropertyModel);
        e.Handled = true;
      }
    }

    private void OnEditResource(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.editingProperty == null || this.editingProperty.IsValueSystemResource)
        return;
      if (this.resourcePopupOpen)
      {
        this.resourceEditorPopup.IsOpen = false;
      }
      else
      {
        BrushEditor brushEditor = sender as BrushEditor;
        if (brushEditor != null)
        {
          this.resourceEditorPopup = new WorkaroundPopup();
          this.resourceEditorPopup.Child = (UIElement) new ResourceEditorControl(brushEditor.editingProperty.SceneNodeObjectSet.DesignerContext, brushEditor.editingProperty);
          UIElement uiElement = e.Parameter as UIElement;
          if (uiElement != null)
          {
            Point point = uiElement.TransformToAncestor((Visual) brushEditor).Transform(new Point(0.0, 0.0));
            this.resourceEditorPopup.PlacementTarget = (UIElement) brushEditor;
            this.resourceEditorPopup.Placement = PlacementMode.RelativePoint;
            this.resourceEditorPopup.HorizontalOffset = point.X;
            this.resourceEditorPopup.VerticalOffset = point.Y + uiElement.RenderSize.Height;
            this.resourceEditorPopup.Closed += new EventHandler(this.EditResourcePopupClosed);
            this.resourceEditorPopup.IsOpen = true;
            this.IsResourcePopupOpen = true;
          }
        }
      }
      e.Handled = true;
    }

    private void EditResourcePopupClosed(object sender, EventArgs e)
    {
      this.IsResourcePopupOpen = false;
      if (this.editingProperty != null && this.editingProperty.SceneNodeObjectSet != null)
        this.editingProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
      IDisposable disposable = this.resourceEditorPopup.Child as IDisposable;
      if (disposable == null)
        return;
      disposable.Dispose();
    }

    private void OnPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/brusheditor/brusheditor.xaml", UriKind.Relative));
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
          this.BrushEditorControl = (BrushEditor) target;
          break;
        case 2:
          this.BrushResourcePickerButton = (FocusReturningWorkaroundRadioButton) target;
          break;
        case 3:
          this.ResourceList = (OnDemandControl) target;
          break;
        case 4:
          this.AdvancedBrushProperties = (StandardCategoryLayout) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class NullBrushEditor : BrushSubtypeEditor
    {
      public override BrushCategory Category
      {
        get
        {
          return BrushCategory.NullBrush;
        }
      }

      public override DataTemplate EditorTemplate
      {
        get
        {
          return new DataTemplate();
        }
      }

      public NullBrushEditor()
        : base((BrushEditor) null, (SceneNodeProperty) null)
      {
      }
    }
  }
}

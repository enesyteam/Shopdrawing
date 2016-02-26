// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TabbedColorEditorControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors.ColorEditor;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TabbedColorEditorControl : UserControl, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty ShowResourcePickerProperty = DependencyProperty.Register("ShowResourcePicker", typeof (bool), typeof (TabbedColorEditorControl), new PropertyMetadata((object) true));
    public static readonly DependencyProperty EyedropperTemplateProperty = ColorEditor.EyedropperTemplateProperty.AddOwner(typeof (TabbedColorEditorControl));
    private static readonly RoutedCommand onResourceSelectedCommand = new RoutedCommand("OnResourceSelected", typeof (TabbedColorEditorControl));
    private SceneNodeProperty editingProperty;
    private bool resourcePopupOpen;
    private WorkaroundPopup resourceEditorPopup;
    internal TabbedColorEditorControl ColorTabControl;
    internal TabControl TabbedView;
    internal TabItem ColorEditorTabItem;
    internal PropertyMarker ColorMarker;
    internal ColorEditor ColorEditor;
    internal TabItem ColorResourceTabItem;
    internal OnDemandControl ResourceList;
    private bool _contentLoaded;

    public Color EditingColor
    {
      get
      {
        if (this.editingProperty != null)
        {
          object obj = this.editingProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.editingProperty.SceneNodeObjectSet.DocumentContext, this.editingProperty.GetValue());
          if (obj is Color)
            return (Color) obj;
        }
        return Colors.Black;
      }
      set
      {
        if (this.editingProperty == null)
          return;
        this.editingProperty.SetValue(this.editingProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue(value));
      }
    }

    public bool ShowResourcePicker
    {
      get
      {
        return (bool) this.GetValue(TabbedColorEditorControl.ShowResourcePickerProperty);
      }
      set
      {
        this.SetValue(TabbedColorEditorControl.ShowResourcePickerProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public DataTemplate EyedropperTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(TabbedColorEditorControl.EyedropperTemplateProperty);
      }
      set
      {
        this.SetValue(TabbedColorEditorControl.EyedropperTemplateProperty, value);
      }
    }

    public ICommand ConvertToColorResourceCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OnConvertToColorResource));
      }
    }

    public ICommand OnResourceSelectedCommand
    {
      get
      {
        return (ICommand) new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OnResourceSelected));
      }
    }

    public static RoutedCommand OnEditResourceCommand
    {
      get
      {
        return TabbedColorEditorControl.onResourceSelectedCommand;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TabbedColorEditorControl()
    {
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) TabbedColorEditorControl.OnEditResourceCommand, new ExecutedRoutedEventHandler(this.OnEditResource)));
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.TabbedColorEditorControl_DataContextChanged);
    }

    public void OnResourceSelected(object parameter)
    {
      VirtualizingResourceItem<SystemResourceModel> virtualizingResourceItem1 = parameter as VirtualizingResourceItem<SystemResourceModel>;
      VirtualizingResourceItem<LocalResourceModel> virtualizingResourceItem2 = parameter as VirtualizingResourceItem<LocalResourceModel>;
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      if (sceneNodeProperty == null)
        return;
      if (virtualizingResourceItem1 != null)
      {
        sceneNodeProperty.DoSetToSystemResource(virtualizingResourceItem1.Model.PropertyModel);
      }
      else
      {
        if (virtualizingResourceItem2 == null)
          return;
        sceneNodeProperty.DoSetToLocalResource(virtualizingResourceItem2.Model.PropertyModel);
      }
    }

    private void TabbedColorEditorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      PropertyValue propertyValue = (PropertyValue) this.DataContext;
      this.UpdateCurrentTab();
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
        this.editingProperty = (SceneNodeProperty) null;
      }
      if (propertyValue == null)
        return;
      this.editingProperty = (SceneNodeProperty) propertyValue.get_ParentProperty();
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnPropertyReferenceChanged);
      this.Rebuild();
    }

    private void Rebuild()
    {
      SceneNodeProperty colorProperty = this.editingProperty;
      if (colorProperty != null && !colorProperty.IsMixedValue)
      {
        this.UpdateCurrentTab();
        if (colorProperty.IsResource)
        {
          colorProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
          this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (o =>
          {
            if (this.ResourceList.IsLoaded)
            {
              this.ResourceList.ApplyTemplate();
              ItemsControl itemsControl = (ItemsControl) this.ResourceList.Template.FindName("ResourcesControl", (FrameworkElement) this.ResourceList);
              itemsControl.ApplyTemplate();
              ItemsPresenter itemsPresenter = (ItemsPresenter) itemsControl.Template.FindName("ResourcesItemsPresenter", (FrameworkElement) itemsControl);
              itemsPresenter.ApplyTemplate();
              WorkaroundVirtualizingStackPanel virtualizingStackPanel = (WorkaroundVirtualizingStackPanel) itemsControl.ItemsPanel.FindName("VirtualizingStackPanel", (FrameworkElement) itemsPresenter);
              int index = -1;
              if (colorProperty.SelectedLocalResourceModel != null)
              {
                colorProperty.SelectedLocalResourceModel.Parent.IsExpanded = true;
                index = itemsControl.Items.IndexOf((object) colorProperty.SelectedLocalResourceModel);
              }
              else if (colorProperty.SelectedSystemResourceModel != null)
              {
                colorProperty.SelectedSystemResourceModel.Parent.IsExpanded = true;
                index = itemsControl.Items.IndexOf((object) colorProperty.SelectedSystemResourceModel);
              }
              if (index >= 0)
                virtualizingStackPanel.BringIndexIntoViewWorkaround(index);
            }
            return null;
          }), null);
        }
      }
      this.OnPropertyChanged("EditingColor");
    }

    private void OnPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (this.editingProperty == null || e.PropertyReference.CompareTo(this.editingProperty.Reference) != 0)
        return;
      this.Rebuild();
    }

    public void UpdateCurrentTab()
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      if (sceneNodeProperty == null)
        return;
      if (sceneNodeProperty.IsResource)
        this.TabbedView.SelectedIndex = 1;
      else
        this.TabbedView.SelectedIndex = 0;
    }

    internal void OnConvertToColorResource(object arg)
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      if (sceneNodeProperty == null || !sceneNodeProperty.IsEnabledMakeNewResource)
        return;
      sceneNodeProperty.DoConvertToResource();
    }

    private void OnColorTabChanged(object sender, SelectionChangedEventArgs e)
    {
      TabControl tabControl = sender as TabControl;
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      if (tabControl == null || sceneNodeProperty == null || e.RemovedItems.Count <= 0)
        return;
      int num = this.ColorEditorTabItem.Equals(e.RemovedItems[0]) ? 0 : 1;
      if (tabControl.SelectedIndex == num)
        return;
      if (num == 1)
      {
        if (sceneNodeProperty == null || !sceneNodeProperty.IsResource)
          return;
        sceneNodeProperty.DoSetLocalValue();
        this.OnPropertyChanged("EditingColor");
      }
      else
        sceneNodeProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
    }

    private void OnEditResource(object sender, ExecutedRoutedEventArgs e)
    {
      TabbedColorEditorControl colorEditorControl = sender as TabbedColorEditorControl;
      if (colorEditorControl != null)
      {
        SceneNodeProperty editingProperty = colorEditorControl.editingProperty;
        if (!editingProperty.IsValueSystemResource)
        {
          if (this.resourcePopupOpen)
          {
            this.resourceEditorPopup.IsOpen = false;
          }
          else
          {
            this.resourceEditorPopup = new WorkaroundPopup();
            this.resourceEditorPopup.Child = (UIElement) new ResourceEditorControl(editingProperty.SceneNodeObjectSet.DesignerContext, editingProperty);
            UIElement uiElement = e.Parameter as UIElement;
            if (uiElement != null)
            {
              Point point = uiElement.TransformToAncestor((Visual) colorEditorControl).Transform(new Point(0.0, 0.0));
              this.resourceEditorPopup.PlacementTarget = (UIElement) colorEditorControl;
              this.resourceEditorPopup.Placement = PlacementMode.RelativePoint;
              this.resourceEditorPopup.HorizontalOffset = point.X;
              this.resourceEditorPopup.VerticalOffset = point.Y + uiElement.RenderSize.Height;
              this.resourceEditorPopup.Closed += new EventHandler(this.EditResourcePopupClosed);
              this.resourceEditorPopup.IsOpen = true;
              this.resourcePopupOpen = true;
            }
          }
        }
      }
      e.Handled = true;
    }

    private void EditResourcePopupClosed(object sender, EventArgs e)
    {
      this.resourcePopupOpen = false;
      SceneNodeProperty sceneNodeProperty = this.editingProperty;
      if (sceneNodeProperty != null)
        sceneNodeProperty.SceneNodeObjectSet.InvalidateLocalResourcesCache();
      IDisposable disposable = this.resourceEditorPopup.Child as IDisposable;
      if (disposable == null)
        return;
      disposable.Dispose();
    }

    private void OnPropertyChanged(string propertyName)
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
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/brusheditor/tabbedcoloreditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ColorTabControl = (TabbedColorEditorControl) target;
          break;
        case 2:
          this.TabbedView = (TabControl) target;
          this.TabbedView.SelectionChanged += new SelectionChangedEventHandler(this.OnColorTabChanged);
          break;
        case 3:
          this.ColorEditorTabItem = (TabItem) target;
          break;
        case 4:
          this.ColorMarker = (PropertyMarker) target;
          break;
        case 5:
          this.ColorEditor = (ColorEditor) target;
          break;
        case 6:
          this.ColorResourceTabItem = (TabItem) target;
          break;
        case 7:
          this.ResourceList = (OnDemandControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

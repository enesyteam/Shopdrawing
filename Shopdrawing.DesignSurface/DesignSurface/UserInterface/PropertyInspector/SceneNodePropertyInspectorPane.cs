// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodePropertyInspectorPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class SceneNodePropertyInspectorPane : Grid, IComponentConnector
  {
    public static readonly DependencyProperty InPropertyInspectorProperty = DependencyProperty.RegisterAttached("InPropertyInspector", typeof (bool), typeof (SceneNodePropertyInspectorPane), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof (int), typeof (SceneNodePropertyInspectorPane), (PropertyMetadata) new FrameworkPropertyMetadata((object) 90, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty PropertyContextMenuProperty = DependencyProperty.Register("PropertyContextMenu", typeof (ContextMenu), typeof (SceneNodePropertyInspectorPane), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    private PropertyInspectorModel propertyInspectorModel;
    private bool isUIAdded;
    internal SceneNodePropertyInspectorPane UserControlSelf;
    internal Border InfoBar;
    internal StringEditor SelectionNameDisplay;
    internal Icon IconImage;
    internal TextBlock SelectionTypeDisplay;
    internal ClearableTextBox SearchBox;
    internal Decorator PopupHost;
    private bool _contentLoaded;

    public int NameColumnWidth
    {
      get
      {
        return (int) this.GetValue(SceneNodePropertyInspectorPane.NameColumnWidthProperty);
      }
      set
      {
        this.SetValue(SceneNodePropertyInspectorPane.NameColumnWidthProperty, (object) value);
      }
    }

    public ContextMenu PropertyContextMenu
    {
      get
      {
        return (ContextMenu) this.GetValue(SceneNodePropertyInspectorPane.PropertyContextMenuProperty);
      }
      set
      {
        this.SetValue(SceneNodePropertyInspectorPane.PropertyContextMenuProperty, (object) value);
      }
    }

    public double ContentMinWidth
    {
      get
      {
        return 276.0 - ExpressionDockingConstants.ScrollBarSize;
      }
    }

    public PropertyInspectorModel Model
    {
      get
      {
        return this.propertyInspectorModel;
      }
    }

    private DesignerContext DesignerContext
    {
      get
      {
        return this.Model.DesignerContext;
      }
    }

    internal SceneNodePropertyInspectorPane(PropertyInspectorModel propertyInspectorModel)
    {
      this.propertyInspectorModel = propertyInspectorModel;
      this.DataContext = (object) propertyInspectorModel;
      PropertyInspectorHelper.SetOwningPropertyInspectorModel((DependencyObject) this, (IPropertyInspector) propertyInspectorModel);
      PropertyInspectorHelper.SetOwningPropertyInspectorElement((DependencyObject) this, (UIElement) this);
      propertyInspectorModel.TransactionHelper.AddCommandBindings((UIElement) this, SupportedPropertyCommands.All);
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.FinishEditing, new ExecutedRoutedEventHandler(this.OnPropertyValueFinishEditingCommand)));
      this.InitializeComponent();
      this.SetValue(DesignerProperties.IsInDesignModeProperty, (object) false);
      this.Loaded += new RoutedEventHandler(this.This_Loaded);
      this.Unloaded += new RoutedEventHandler(this.This_Unloaded);
    }

    public static bool GetInPropertyInspector(DependencyObject source)
    {
      return (bool) source.GetValue(SceneNodePropertyInspectorPane.InPropertyInspectorProperty);
    }

    public static void SetInPropertyInspector(DependencyObject target, bool value)
    {
      target.SetValue(SceneNodePropertyInspectorPane.InPropertyInspectorProperty, (object) (bool) (value ? true : false));
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.UpdateIsSearchEnabled(e.NewView as SceneView);
    }

    private void UpdateIsSearchEnabled(SceneView view)
    {
      this.SearchBox.IsEnabled = view != null && view.IsDesignSurfaceVisible;
    }

    private void This_Loaded(object sender, RoutedEventArgs e)
    {
      if (!this.isUIAdded)
      {
        this.Model.AddUserInterface();
        this.propertyInspectorModel.DesignerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
        this.isUIAdded = true;
      }
      this.UpdateIsSearchEnabled(this.propertyInspectorModel.DesignerContext.ActiveView);
    }

    private void This_Unloaded(object sender, RoutedEventArgs e)
    {
      if (!this.isUIAdded)
        return;
      this.Model.RemoveUserInterface();
      if (this.propertyInspectorModel.DesignerContext.ViewService != null)
        this.propertyInspectorModel.DesignerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.isUIAdded = false;
    }

    private static CategoryContainer GetCategoryContainer(RoutedEventArgs eventArgs)
    {
      FrameworkElement frameworkElement = eventArgs.OriginalSource as FrameworkElement;
      if (frameworkElement != null)
        return CategoryContainer.GetOwningCategoryContainer((DependencyObject) frameworkElement);
      return (CategoryContainer) null;
    }

    private void OnPropertyValueFinishEditingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      if (this.DesignerContext.ActiveView == null)
        return;
      this.DesignerContext.ActiveView.ReturnFocus();
    }

    private void OnCategoryContainerCommandsUpdateCategoryExpansionState(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      this.Model.UpdateCategoryExpansion(SceneNodePropertyInspectorPane.GetCategoryContainer((RoutedEventArgs) eventArgs));
      eventArgs.Handled = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/scenenodepropertyinspectorpane.xaml", UriKind.Relative));
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
          this.UserControlSelf = (SceneNodePropertyInspectorPane) target;
          break;
        case 2:
          this.InfoBar = (Border) target;
          break;
        case 3:
          this.SelectionNameDisplay = (StringEditor) target;
          break;
        case 4:
          this.IconImage = (Icon) target;
          break;
        case 5:
          this.SelectionTypeDisplay = (TextBlock) target;
          break;
        case 6:
          this.SearchBox = (ClearableTextBox) target;
          break;
        case 7:
          this.PopupHost = (Decorator) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyMarker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class PropertyMarker : Icon
  {
    public static readonly DependencyProperty DotBrushProperty = DependencyProperty.Register("DotBrush", typeof (Brush), typeof (PropertyMarker), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Purple, new PropertyChangedCallback(PropertyMarker.OnDotBrushChanged)));
    public static readonly DependencyProperty AssociatedPropertyProperty = DependencyProperty.Register("AssociatedProperty", typeof (SceneNodeProperty), typeof (PropertyMarker), (PropertyMetadata) new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyMarker.OnAssociatedPropertyChanged)));
    private static bool isMenuVisible;
    private DelegateCommand showContextMenuCommand;
    private ContextMenu currentContextMenu;
    private PickWhipEngine elementPropertyPicker;

    public Brush DotBrush
    {
      get
      {
        return (Brush) this.GetValue(PropertyMarker.DotBrushProperty);
      }
      set
      {
        this.SetValue(PropertyMarker.DotBrushProperty, value);
      }
    }

    public SceneNodeProperty AssociatedProperty
    {
      get
      {
        return (SceneNodeProperty) this.GetValue(PropertyMarker.AssociatedPropertyProperty);
      }
      set
      {
        this.SetValue(PropertyMarker.AssociatedPropertyProperty, value);
      }
    }

    private bool IsPickingElementProperty
    {
      get
      {
        if (this.elementPropertyPicker != null)
          return this.elementPropertyPicker.IsActive;
        return false;
      }
    }

    public DelegateCommand ShowContextMenuCommand
    {
      get
      {
        return this.showContextMenuCommand;
      }
    }

    public PropertyMarker()
    {
      this.showContextMenuCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ShowContextMenu));
      this.SetBinding(PropertyMarker.AssociatedPropertyProperty, (BindingBase) new Binding()
      {
        RelativeSource = RelativeSource.Self,
        Path = new PropertyPath("(0).(1)", (object[]) new DependencyProperty[2]
        {
          (DependencyProperty) PropertyContainer.OwningPropertyContainerProperty,
          (DependencyProperty) PropertyContainer.PropertyEntryProperty
        })
      });
      this.Width = 11.0;
      this.Height = 17.0;
      this.Focusable = false;
      this.ToolTip = (object) StringTable.PropertyMarkerTooltip;
      this.SetValue(Icon.SourceBrushProperty, (object) (Brush) this.TryFindResource((object) "MarkerBackgroundBrush"));
      this.SetValue(Icon.BlueChromaProperty, (object) (Brush) this.TryFindResource((object) "RecessedBrush"));
      this.SetValue(Icon.RedChromaProperty, (object) Brushes.Transparent);
    }

    private static void OnDotBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
      PropertyMarker propertyMarker = (PropertyMarker) d;
      propertyMarker.SetValue(Icon.GreenChromaProperty, (object) propertyMarker.DotBrush);
    }

    private static void OnAssociatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
      PropertyMarker propertyMarker = (PropertyMarker) d;
      SceneNodeProperty sceneNodeProperty = args.OldValue as SceneNodeProperty;
      if (sceneNodeProperty != null)
        sceneNodeProperty.remove_PropertyChanged(new PropertyChangedEventHandler(propertyMarker.AssociatedProperty_PropertyChanged));
      if (propertyMarker.AssociatedProperty != null)
      {
        propertyMarker.SetValue(AutomationElement.IdProperty, (object) (((PropertyEntry) propertyMarker.AssociatedProperty).get_PropertyName() + "_Marker"));
        propertyMarker.UpdateBrushFromValueSource();
        propertyMarker.AssociatedProperty.add_PropertyChanged(new PropertyChangedEventHandler(propertyMarker.AssociatedProperty_PropertyChanged));
      }
      if (!propertyMarker.IsPickingElementProperty)
        return;
      propertyMarker.elementPropertyPicker.CancelEditing();
      propertyMarker.elementPropertyPicker = (PickWhipEngine) null;
    }

    public void ShowContextMenu()
    {
      if (PropertyMarker.isMenuVisible || this.currentContextMenu != null || this.AssociatedProperty == null)
        return;
      Keyboard.Focus((IInputElement) null);
      this.AssociatedProperty.UpdateResourceAndTemplateBindingSelection();
      this.currentContextMenu = (ContextMenu) this.FindResource((object) "MarkerContextMenu");
      this.currentContextMenu.Opened += new RoutedEventHandler(this.ContextMenuOpened);
      this.currentContextMenu.Closed += new RoutedEventHandler(this.ContextMenuClosed);
      this.currentContextMenu.Unloaded += new RoutedEventHandler(this.ContextMenuUnloaded);
      this.currentContextMenu.PlacementTarget = (UIElement) this;
      this.currentContextMenu.Placement = PlacementMode.Left;
      this.currentContextMenu.HorizontalOffset = 3.0;
      this.currentContextMenu.VerticalOffset = 3.0;
      this.currentContextMenu.IsOpen = true;
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.ClearValueCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsClearValueCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsClearValueCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.LocalValueCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsLocalValueCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsLocalValueCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.LocalResourceCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsLocalResourceCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.SystemResourceCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsSystemResourceCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.DataBindingCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsDataBindingCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsDataBindingCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.ElementPropertyBindingCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsElementPropertyBindingCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsElementPropertyBindingCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.TemplateBindingCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsTemplateBindingCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.CustomExpressionCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsCustomExpressionCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsCustomExpressionCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.EditResourceCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsEditResourceCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsEditResourceCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.ConvertToResourceCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsConvertToResourceCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsConvertToResourceCommand)));
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyMarkerCommands.RecordCurrentValueCommand, new ExecutedRoutedEventHandler(this.OnPropertyMarkerCommandsRecordCurrentValueCommand), new CanExecuteRoutedEventHandler(this.IsEnabledPropertyMarkerCommandsRecordCurrentValueCommand)));
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left && !this.IsPickingElementProperty)
      {
        e.Handled = true;
        this.ShowContextMenu();
      }
      base.OnMouseDown(e);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      this.OnMouseOverChanged();
      base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      this.OnMouseOverChanged();
      base.OnMouseLeave(e);
    }

    private void OnMouseOverChanged()
    {
      if (this.IsMouseOver)
        this.SetValue(Icon.RedChromaProperty, (object) (Brush) this.TryFindResource((object) "FadedTextBrush"));
      else
        this.SetValue(Icon.RedChromaProperty, (object) Brushes.Transparent);
    }

    private void ContextMenuUnloaded(object sender, RoutedEventArgs e)
    {
      if (!PropertyMarker.isMenuVisible)
        return;
      this.ContextMenuClosed();
    }

    private void ContextMenuOpened(object sender, RoutedEventArgs args)
    {
      PropertyMarker.isMenuVisible = true;
      ContextMenu contextMenu = sender as ContextMenu;
      if (contextMenu == null)
        return;
      contextMenu.DataContext = this;
    }

    private void ContextMenuClosed(object sender, RoutedEventArgs e)
    {
      this.ContextMenuClosed();
    }

    private void ContextMenuClosed()
    {
      PropertyMarker.isMenuVisible = false;
      this.currentContextMenu.Opened -= new RoutedEventHandler(this.ContextMenuOpened);
      this.currentContextMenu.Closed -= new RoutedEventHandler(this.ContextMenuClosed);
      this.currentContextMenu.Unloaded -= new RoutedEventHandler(this.ContextMenuUnloaded);
      this.currentContextMenu = (ContextMenu) null;
      this.CommandBindings.Clear();
    }

    private void AssociatedProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "ValueSource"))
        return;
      this.UpdateBrushFromValueSource();
    }

    private void UpdateBrushFromValueSource()
    {
      DependencyPropertyValueSource valueSource = this.AssociatedProperty.ValueSource;
      if (valueSource.get_IsLocalValue())
        this.DotBrush = (Brush) this.TryFindResource((object) "WhiteBrush");
      else if (valueSource.get_IsInheritedValue() || valueSource.get_IsDefaultValue())
        this.DotBrush = (Brush) this.TryFindResource((object) "PaletteBrush");
      else if (valueSource.get_IsBinding() || valueSource.get_IsTemplateBinding() || valueSource.get_IsCustomMarkupExtension())
      {
        this.DotBrush = (Brush) this.TryFindResource((object) "DatabindingBrush");
      }
      else
      {
        if (!valueSource.get_IsResource() && !valueSource.get_IsStatic())
          return;
        this.DotBrush = (Brush) this.TryFindResource((object) "ResourceLinkBrush");
      }
    }

    private void OnPropertyMarkerCommandsClearValueCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoClearValue();
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsClearValueCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      if (!PropertyMarker.isMenuVisible)
        return;
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledClearValue;
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsLocalValueCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
          associatedProperty.DoSetLocalValue();
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsLocalValueCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledSetLocal;
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsLocalResourceCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoSetToLocalResource((LocalResourceModel) eventArgs.Parameter);
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsSystemResourceCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoSetToSystemResource((SystemResourceModel) eventArgs.Parameter);
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsDataBindingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoSetDataBinding();
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsDataBindingCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledDatabind;
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsElementPropertyBindingCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledDatabind;
      if (eventArgs.CanExecute && associatedProperty.SceneNodeObjectSet.RepresentativeSceneNode is StyleNode)
        eventArgs.CanExecute = false;
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsElementPropertyBindingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        SceneView activeView = associatedProperty.SceneNodeObjectSet.DesignerContext.ActiveView;
        if (activeView != null)
          activeView.ReturnFocus();
        if (this.elementPropertyPicker == null)
          this.elementPropertyPicker = new PickWhipEngine((FrameworkElement) this, associatedProperty, (IElementSelectionStrategy) new CreatePropertyBindingPickWhipStrategy(associatedProperty.Reference.LastStep), ToolCursors.PickWhipCursor);
        this.elementPropertyPicker.BeginEditing();
      }
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsTemplateBindingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoSetToTemplateBinding((TemplateBindablePropertyModel) eventArgs.Parameter);
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsCustomExpressionCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          associatedProperty.DoEditCustomExpression((FrameworkElement) this);
          PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsCustomExpressionCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      eventArgs.CanExecute = this.AssociatedProperty != null && this.AssociatedProperty.IsEnabledEditCustomExpression;
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsEditResourceCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
          associatedProperty.DoEditResource();
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsEditResourceCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledEditResource;
      eventArgs.Handled = true;
    }

    private void OnPropertyMarkerCommandsConvertToResourceCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
      {
        using (WorkaroundPopup.LockOpen((DependencyObject) this))
        {
          if (associatedProperty.DoConvertToResource())
            PropertyMarker.ResetPropertyContainerMode((RoutedEventArgs) eventArgs);
        }
      }
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsConvertToResourceCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledMakeNewResource;
      eventArgs.Handled = true;
    }

    private static void ResetPropertyContainerMode(RoutedEventArgs eventArgs)
    {
      PropertyContainer propertyContainer = PropertyMarker.GetOwningPropertyContainer(eventArgs);
      if (propertyContainer == null)
        return;
      PropertyMarker propertyMarker = eventArgs.Source as PropertyMarker;
      if (propertyMarker != null && propertyMarker.AssociatedProperty != propertyContainer.get_PropertyEntry())
        return;
      propertyContainer.set_ActiveEditMode((PropertyContainerEditMode) 0);
    }

    private static PropertyContainer GetOwningPropertyContainer(RoutedEventArgs eventArgs)
    {
      DependencyObject dependencyObject = eventArgs.OriginalSource as DependencyObject;
      if (dependencyObject != null)
        return PropertyContainer.GetOwningPropertyContainer(dependencyObject);
      return (PropertyContainer) null;
    }

    private void OnPropertyMarkerCommandsRecordCurrentValueCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty != null)
        associatedProperty.DoRecordCurrentValue();
      eventArgs.Handled = true;
    }

    private void IsEnabledPropertyMarkerCommandsRecordCurrentValueCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
    {
      SceneNodeProperty associatedProperty = this.AssociatedProperty;
      if (associatedProperty == null)
        return;
      eventArgs.CanExecute = associatedProperty.IsEnabledRecordCurrentValue;
      eventArgs.Handled = true;
    }
  }
}

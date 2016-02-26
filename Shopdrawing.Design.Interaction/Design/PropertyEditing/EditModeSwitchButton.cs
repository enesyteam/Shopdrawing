// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.EditModeSwitchButton
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class EditModeSwitchButton : Button
  {
    public static readonly DependencyProperty TargetEditModeProperty = DependencyProperty.Register("TargetEditMode", typeof (PropertyContainerEditMode), typeof (EditModeSwitchButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) PropertyContainerEditMode.Inline, (PropertyChangedCallback) null, new CoerceValueCallback(EditModeSwitchButton.OnCoerceEditModeProperty)));
    public static readonly DependencyProperty SyncModeToOwningContainerProperty = DependencyProperty.Register("SyncModeToOwningContainer", typeof (bool), typeof (EditModeSwitchButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(EditModeSwitchButton.OnSyncModeToOwningContainerChanged)));
    private PropertyContainer _owningContainer;
    private bool _attachedToContainerEvents;

    public PropertyContainerEditMode TargetEditMode
    {
      get
      {
        return (PropertyContainerEditMode) this.GetValue(EditModeSwitchButton.TargetEditModeProperty);
      }
      set
      {
        this.SetValue(EditModeSwitchButton.TargetEditModeProperty, (object) value);
      }
    }

    public bool SyncModeToOwningContainer
    {
      get
      {
        return (bool) this.GetValue(EditModeSwitchButton.SyncModeToOwningContainerProperty);
      }
      set
      {
        this.SetValue(EditModeSwitchButton.SyncModeToOwningContainerProperty, (object) (bool) (value ? true : false));
      }
    }

    public EditModeSwitchButton()
    {
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    }

    private static object OnCoerceEditModeProperty(DependencyObject obj, object value)
    {
      EditModeSwitchButton modeSwitchButton = (EditModeSwitchButton) obj;
      if (!modeSwitchButton.SyncModeToOwningContainer || modeSwitchButton._owningContainer == null)
        return value;
      PropertyContainer propertyContainer = modeSwitchButton._owningContainer;
      PropertyContainerEditMode containerEditMode;
      switch (propertyContainer.ActiveEditMode)
      {
        case PropertyContainerEditMode.Inline:
          containerEditMode = !propertyContainer.SupportsEditMode(PropertyContainerEditMode.Dialog) ? (!propertyContainer.SupportsEditMode(PropertyContainerEditMode.ExtendedPopup) ? PropertyContainerEditMode.Inline : PropertyContainerEditMode.ExtendedPopup) : PropertyContainerEditMode.Dialog;
          break;
        case PropertyContainerEditMode.ExtendedPopup:
          containerEditMode = PropertyContainerEditMode.ExtendedPinned;
          break;
        case PropertyContainerEditMode.ExtendedPinned:
          containerEditMode = PropertyContainerEditMode.Inline;
          break;
        case PropertyContainerEditMode.Dialog:
          containerEditMode = modeSwitchButton.TargetEditMode;
          break;
        default:
          containerEditMode = (PropertyContainerEditMode) value;
          break;
      }
      return (object) containerEditMode;
    }

    private static void OnSyncModeToOwningContainerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      obj.CoerceValue(EditModeSwitchButton.TargetEditModeProperty);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property == PropertyContainer.OwningPropertyContainerProperty)
      {
        PropertyContainer container1 = (PropertyContainer) e.OldValue;
        PropertyContainer container2 = (PropertyContainer) e.NewValue;
        this._owningContainer = container2;
        if (container1 != null)
          this.DisassociateContainerEventHandlers(container1);
        if (container2 != null)
          this.AssociateContainerEventHandlers(container2);
        this.CoerceValue(EditModeSwitchButton.TargetEditModeProperty);
      }
      base.OnPropertyChanged(e);
    }

    private void OnPropertyContainerDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.Property != PropertyContainer.ActiveEditModeProperty && e.Property != PropertyContainer.PropertyEntryProperty && (e.Property != PropertyContainer.DefaultStandardValuesPropertyValueEditorProperty && e.Property != PropertyContainer.DefaultPropertyValueEditorProperty))
        return;
      this.CoerceValue(EditModeSwitchButton.TargetEditModeProperty);
    }

    private void AssociateContainerEventHandlers(PropertyContainer container)
    {
      if (this._attachedToContainerEvents)
        return;
      container.DependencyPropertyChanged += new DependencyPropertyChangedEventHandler(this.OnPropertyContainerDependencyPropertyChanged);
      this._attachedToContainerEvents = true;
    }

    private void DisassociateContainerEventHandlers(PropertyContainer container)
    {
      if (!this._attachedToContainerEvents)
        return;
      container.DependencyPropertyChanged -= new DependencyPropertyChangedEventHandler(this.OnPropertyContainerDependencyPropertyChanged);
      this._attachedToContainerEvents = false;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if (this._owningContainer == null)
        return;
      this.DisassociateContainerEventHandlers(this._owningContainer);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (this._owningContainer == null)
        return;
      this.AssociateContainerEventHandlers(this._owningContainer);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
        this.InvokePropertyValueEditorCommand();
      base.OnMouseDown(e);
    }

    private void InvokePropertyValueEditorCommand()
    {
      switch (this.TargetEditMode)
      {
        case PropertyContainerEditMode.Inline:
          PropertyValueEditorCommands.ShowInlineEditor.Execute((object) null, (IInputElement) this);
          break;
        case PropertyContainerEditMode.ExtendedPopup:
          PropertyValueEditorCommands.ShowExtendedPopupEditor.Execute((object) null, (IInputElement) this);
          break;
        case PropertyContainerEditMode.ExtendedPinned:
          PropertyValueEditorCommands.ShowExtendedPinnedEditor.Execute((object) null, (IInputElement) this);
          break;
        case PropertyContainerEditMode.Dialog:
          PropertyValueEditorCommands.ShowDialogEditor.Execute((object) null, (IInputElement) this);
          break;
      }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new EditModeSwitchButton.EditModeSwitchButtonAutomationPeer(this);
    }

    private class EditModeSwitchButtonAutomationPeer : ButtonAutomationPeer, IInvokeProvider, IToggleProvider
    {
      public ToggleState ToggleState
      {
        get
        {
          PropertyContainer propertyContainer = ((EditModeSwitchButton) this.Owner)._owningContainer;
          if (propertyContainer == null)
            return ToggleState.Indeterminate;
          switch (propertyContainer.ActiveEditMode)
          {
            case PropertyContainerEditMode.Inline:
              return ToggleState.Off;
            case PropertyContainerEditMode.ExtendedPinned:
              return ToggleState.On;
            default:
              return ToggleState.Indeterminate;
          }
        }
      }

      public EditModeSwitchButtonAutomationPeer(EditModeSwitchButton owner)
        : base((Button) owner)
      {
      }

      public override object GetPattern(PatternInterface patternInterface)
      {
        string name = Enum.GetName(typeof (PatternInterface), (object) patternInterface);
        if (patternInterface == PatternInterface.Invoke || name == "Toggle")
          return (object) this;
        return base.GetPattern(patternInterface);
      }

      public void Toggle()
      {
        this.Invoke();
      }

      public void Invoke()
      {
        EditModeSwitchButton button = (EditModeSwitchButton) this.Owner;
        //button.Dispatcher.BeginInvoke((Delegate) (() => button.InvokePropertyValueEditorCommand()));
      }

      private delegate void VoidInvoker();
    }
  }
}

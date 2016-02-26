// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyContainer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class PropertyContainer : Control, INotifyPropertyChanged
  {
    public static readonly DependencyProperty PropertyEntryProperty = DependencyProperty.Register("PropertyEntry", typeof (PropertyEntry), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(PropertyContainer.PropertyEntryPropertyChanged)));
    public static readonly DependencyProperty ActiveEditModeProperty = DependencyProperty.Register("ActiveEditMode", typeof (PropertyContainerEditMode), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) PropertyContainerEditMode.Inline, new PropertyChangedCallback(PropertyContainer.OnActiveEditModePropertyChanged)));
    public static readonly DependencyProperty DialogCommandSourceProperty = DependencyProperty.Register("DialogCommandSource", typeof (IInputElement), typeof (PropertyContainer), new PropertyMetadata((object) null));
    public static readonly DependencyProperty OwningPropertyContainerProperty = DependencyProperty.RegisterAttached("OwningPropertyContainer", typeof (PropertyContainer), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty InlineRowTemplateProperty = DependencyProperty.Register("InlineRowTemplate", typeof (ControlTemplate), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(PropertyContainer.RowTemplateChanged)));
    public static readonly DependencyProperty ExtendedPopupRowTemplateProperty = DependencyProperty.Register("ExtendedPopupRowTemplate", typeof (ControlTemplate), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(PropertyContainer.RowTemplateChanged)));
    public static readonly DependencyProperty ExtendedPinnedRowTemplateProperty = DependencyProperty.Register("ExtendedPinnedRowTemplate", typeof (ControlTemplate), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(PropertyContainer.RowTemplateChanged)));
    public static readonly DependencyProperty DefaultStandardValuesPropertyValueEditorProperty = DependencyProperty.Register("DefaultStandardValuesPropertyValueEditor", typeof (PropertyValueEditor), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(PropertyContainer.DefaultPropertyValueEditorChanged)));
    public static readonly DependencyProperty DefaultPropertyValueEditorProperty = DependencyProperty.Register("DefaultPropertyValueEditor", typeof (PropertyValueEditor), typeof (PropertyContainer), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(PropertyContainer.DefaultPropertyValueEditorChanged)));
    private static RoutedCommand _openDialogWindow;
    private bool _attachedToPropertyEntryEvents;

    public PropertyEntry PropertyEntry
    {
      get
      {
        return (PropertyEntry) this.GetValue(PropertyContainer.PropertyEntryProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.PropertyEntryProperty, (object) value);
      }
    }

    public PropertyContainerEditMode ActiveEditMode
    {
      get
      {
        return (PropertyContainerEditMode) this.GetValue(PropertyContainer.ActiveEditModeProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.ActiveEditModeProperty, (object) value);
      }
    }

    public IInputElement DialogCommandSource
    {
      get
      {
        return (IInputElement) this.GetValue(PropertyContainer.DialogCommandSourceProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.DialogCommandSourceProperty, (object) value);
      }
    }

    public ControlTemplate InlineRowTemplate
    {
      get
      {
        return (ControlTemplate) this.GetValue(PropertyContainer.InlineRowTemplateProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.InlineRowTemplateProperty, (object) value);
      }
    }

    public ControlTemplate ExtendedPopupRowTemplate
    {
      get
      {
        return (ControlTemplate) this.GetValue(PropertyContainer.ExtendedPopupRowTemplateProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.ExtendedPopupRowTemplateProperty, (object) value);
      }
    }

    public ControlTemplate ExtendedPinnedRowTemplate
    {
      get
      {
        return (ControlTemplate) this.GetValue(PropertyContainer.ExtendedPinnedRowTemplateProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.ExtendedPinnedRowTemplateProperty, (object) value);
      }
    }

    public PropertyValueEditor DefaultStandardValuesPropertyValueEditor
    {
      get
      {
        return (PropertyValueEditor) this.GetValue(PropertyContainer.DefaultStandardValuesPropertyValueEditorProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.DefaultStandardValuesPropertyValueEditorProperty, (object) value);
      }
    }

    public PropertyValueEditor DefaultPropertyValueEditor
    {
      get
      {
        return (PropertyValueEditor) this.GetValue(PropertyContainer.DefaultPropertyValueEditorProperty);
      }
      set
      {
        this.SetValue(PropertyContainer.DefaultPropertyValueEditorProperty, (object) value);
      }
    }

    public DataTemplate InlineEditorTemplate
    {
      get
      {
        return this.FindPropertyValueEditorTemplate(PropertyContainerEditMode.Inline);
      }
    }

    public DataTemplate ExtendedEditorTemplate
    {
      get
      {
        return this.FindPropertyValueEditorTemplate(PropertyContainerEditMode.ExtendedPinned);
      }
    }

    public DataTemplate DialogEditorTemplate
    {
      get
      {
        return this.FindPropertyValueEditorTemplate(PropertyContainerEditMode.Dialog);
      }
    }

    public bool MatchesFilter
    {
      get
      {
        PropertyEntry propertyEntry = this.PropertyEntry;
        if (propertyEntry != null)
          return propertyEntry.MatchesFilter;
        return false;
      }
    }

    public static RoutedCommand OpenDialogWindow
    {
      get
      {
        if (PropertyContainer._openDialogWindow == null)
          PropertyContainer._openDialogWindow = new RoutedCommand("OpenDialogWindow", typeof (PropertyContainer));
        return PropertyContainer._openDialogWindow;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal event DependencyPropertyChangedEventHandler DependencyPropertyChanged;

    public event EventHandler PropertyEntryChanged;

    public event EventHandler ActiveEditModeChanged;

    public PropertyContainer()
    {
      PropertyContainer.SetOwningPropertyContainer((DependencyObject) this, this);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    }

    private static void PropertyEntryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      PropertyContainer container = (PropertyContainer) obj;
      container.NotifyTemplatesChanged();
      container.OnPropertyChanged("MatchesFilter");
      container.ActiveEditMode = PropertyContainerEditMode.Inline;
      PropertyContainer.UpdateControlTemplate(container);
      if (e.OldValue != null)
        container.DisassociatePropertyEventHandlers((PropertyEntry) e.OldValue);
      if (e.NewValue != null)
        container.AssociatePropertyEventHandlers((PropertyEntry) e.NewValue);
      if (container.PropertyEntryChanged == null)
        return;
      container.PropertyEntryChanged((object) container, EventArgs.Empty);
    }

    private static void OnActiveEditModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      PropertyContainer container = (PropertyContainer) obj;
      PropertyContainer.UpdateControlTemplate(container);
      if (container.ActiveEditModeChanged != null)
        container.ActiveEditModeChanged((object) container, EventArgs.Empty);
      if (!object.Equals(e.NewValue, (object) PropertyContainerEditMode.Dialog))
        return;
      IInputElement inputElement = container.DialogCommandSource ?? (IInputElement) container;
      if (PropertyContainer.OpenDialogWindow.CanExecute((object) container.PropertyEntry, inputElement))
      {
        PropertyContainer.OpenDialogWindow.Execute((object) container.PropertyEntry, inputElement);
      }
      else
      {
        DialogPropertyValueEditor propertyValueEditor = container.FindDialogPropertyValueEditor();
        if (propertyValueEditor != null)
          propertyValueEditor.ShowDialog(container.PropertyEntry.PropertyValue, inputElement);
      }
      container.ActiveEditMode = (PropertyContainerEditMode) e.OldValue;
    }

    public static void SetOwningPropertyContainer(DependencyObject dependencyObject, PropertyContainer value)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      dependencyObject.SetValue(PropertyContainer.OwningPropertyContainerProperty, (object) value);
    }

    public static PropertyContainer GetOwningPropertyContainer(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");
      return (PropertyContainer) dependencyObject.GetValue(PropertyContainer.OwningPropertyContainerProperty);
    }

    private static void RowTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      PropertyContainer container = (PropertyContainer) obj;
      if (((((((false ? 1 : 0) | (e.Property != PropertyContainer.InlineRowTemplateProperty ? 0 : (container.ActiveEditMode == PropertyContainerEditMode.Inline ? 1 : 0))) != 0 ? 1 : 0) | (e.Property != PropertyContainer.ExtendedPopupRowTemplateProperty ? 0 : (container.ActiveEditMode == PropertyContainerEditMode.ExtendedPopup ? 1 : 0))) != 0 ? 1 : 0) | (e.Property != PropertyContainer.ExtendedPinnedRowTemplateProperty ? 0 : (container.ActiveEditMode == PropertyContainerEditMode.ExtendedPinned ? 1 : 0))) == 0)
        return;
      PropertyContainer.UpdateControlTemplate(container);
    }

    private static void DefaultPropertyValueEditorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      ((PropertyContainer) obj).NotifyTemplatesChanged();
    }

    internal bool SupportsEditMode(PropertyContainerEditMode mode)
    {
      if (mode == PropertyContainerEditMode.Dialog)
        return this.FindDialogPropertyValueEditor() != null;
      return this.FindPropertyValueEditorTemplate(mode) != null;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      PropertyEntry propertyEntry = this.PropertyEntry;
      if (propertyEntry == null)
        return;
      this.DisassociatePropertyEventHandlers(propertyEntry);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      PropertyEntry propertyEntry = this.PropertyEntry;
      if (propertyEntry == null)
        return;
      this.AssociatePropertyEventHandlers(propertyEntry);
    }

    private void AssociatePropertyEventHandlers(PropertyEntry property)
    {
      if (this._attachedToPropertyEntryEvents)
        return;
      property.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyPropertyChanged);
      this._attachedToPropertyEntryEvents = true;
    }

    private void DisassociatePropertyEventHandlers(PropertyEntry property)
    {
      if (!this._attachedToPropertyEntryEvents)
        return;
      property.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyPropertyChanged);
      this._attachedToPropertyEntryEvents = false;
    }

    private void OnPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ("MatchesFilter".Equals(e.PropertyName))
      {
        this.OnPropertyChanged("MatchesFilter");
      }
      else
      {
        if (!"PropertyValueEditor".Equals(e.PropertyName))
          return;
        this.NotifyTemplatesChanged();
      }
    }

    private DataTemplate FindPropertyValueEditorTemplate(PropertyContainerEditMode editMode)
    {
      PropertyEntry propertyEntry = this.PropertyEntry;
      DataTemplate dataTemplate = (DataTemplate) null;
      if (propertyEntry != null && dataTemplate == null)
      {
        PropertyValueEditor propertyValueEditor = propertyEntry.PropertyValueEditor;
        if (propertyValueEditor != null)
          dataTemplate = propertyValueEditor.GetPropertyValueEditor(editMode);
      }
      if (dataTemplate != null)
        return dataTemplate;
      if (propertyEntry != null && propertyEntry.HasStandardValuesInternal)
      {
        PropertyValueEditor propertyValueEditor = this.DefaultStandardValuesPropertyValueEditor;
        if (propertyValueEditor != null)
          dataTemplate = propertyValueEditor.GetPropertyValueEditor(editMode);
      }
      if (dataTemplate != null)
        return dataTemplate;
      PropertyValueEditor propertyValueEditor1 = this.DefaultPropertyValueEditor;
      if (propertyValueEditor1 != null)
        dataTemplate = propertyValueEditor1.GetPropertyValueEditor(editMode);
      return dataTemplate;
    }

    private DialogPropertyValueEditor FindDialogPropertyValueEditor()
    {
      PropertyEntry propertyEntry = this.PropertyEntry;
      DialogPropertyValueEditor propertyValueEditor = (DialogPropertyValueEditor) null;
      if (propertyEntry != null)
        propertyValueEditor = propertyEntry.PropertyValueEditor as DialogPropertyValueEditor;
      if (propertyValueEditor != null)
        return propertyValueEditor;
      if (propertyEntry != null && propertyEntry.HasStandardValuesInternal)
        propertyValueEditor = this.DefaultStandardValuesPropertyValueEditor as DialogPropertyValueEditor;
      return propertyValueEditor ?? this.DefaultPropertyValueEditor as DialogPropertyValueEditor;
    }

    private static void UpdateControlTemplate(PropertyContainer container)
    {
      ControlTemplate controlTemplate;
      switch (container.ActiveEditMode)
      {
        case PropertyContainerEditMode.Inline:
          controlTemplate = container.InlineRowTemplate;
          break;
        case PropertyContainerEditMode.ExtendedPopup:
          controlTemplate = container.ExtendedPopupRowTemplate;
          break;
        case PropertyContainerEditMode.ExtendedPinned:
          controlTemplate = container.ExtendedPinnedRowTemplate;
          break;
        case PropertyContainerEditMode.Dialog:
          return;
        default:
          controlTemplate = container.Template;
          break;
      }
      if (controlTemplate == container.Template)
        return;
      container.Template = controlTemplate;
    }

    private void NotifyTemplatesChanged()
    {
      this.OnPropertyChanged("InlineEditorTemplate");
      this.OnPropertyChanged("ExtendedEditorTemplate");
      this.OnPropertyChanged("DialogEditorTemplate");
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (this.DependencyPropertyChanged != null)
        this.DependencyPropertyChanged((object) this, e);
      base.OnPropertyChanged(e);
    }
  }
}

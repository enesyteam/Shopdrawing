// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ChoiceEditor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class ChoiceEditor : Control, INotifyPropertyChanged, IIconProvider
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (object), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ChoiceEditor.ValueChanged), (CoerceValueCallback) null, false, UpdateSourceTrigger.Explicit));
    public static readonly DependencyProperty ValueIndexProperty = DependencyProperty.Register("ValueIndex", typeof (int), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ChoiceEditor.ValueIndexChanged)));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof (IEnumerable), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(ChoiceEditor.ItemsSourceChanged)));
    public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register("Converter", typeof (TypeConverter), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.None));
    public static readonly DependencyProperty ViewTypeProperty = DependencyProperty.Register("ViewType", typeof (ChoiceEditorViewType), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) ChoiceEditorViewType.Combo, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof (bool), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(ChoiceEditor.IsEditableChanged)));
    public static readonly DependencyProperty VerifyEditableInputProperty = DependencyProperty.Register("VerifyEditableInput", typeof (bool), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(ChoiceEditor.VerifyEditableInputChanged)));
    public static readonly DependencyProperty IconResourcePrefixProperty = DependencyProperty.Register("IconResourcePrefix", typeof (string), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IconResourceSuffixProperty = DependencyProperty.Register("IconResourceSuffix", typeof (string), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) "Icon"));
    public static readonly DependencyProperty IsNinchedProperty = DependencyProperty.Register("IsNinched", typeof (bool), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(ChoiceEditor.IsNinchedChanged)));
    public static readonly DependencyProperty ShowFullControlProperty = DependencyProperty.Register("ShowFullControl", typeof (bool), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender, (PropertyChangedCallback) null, new CoerceValueCallback(ChoiceEditor.CoerceShowFullControl)));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof (DataTemplate), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ChoiceEditor.ItemTemplateChanged)));
    public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof (DataTemplateSelector), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ChoiceEditor.ItemTemplateSelectorChanged)));
    public static readonly DependencyProperty UseItemTemplateForSelectionProperty = DependencyProperty.Register("UseItemTemplateForSelection", typeof (bool?), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(ChoiceEditor.CoerceUseItemTemplateForSelection)));
    public static readonly DependencyProperty GroupStyleProperty = DependencyProperty.Register("GroupStyle", typeof (GroupStyle), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ChoiceEditor.GroupStyleChanged)));
    public static readonly DependencyProperty BorderCornerRadiusProperty = DependencyProperty.Register("BorderCornerRadius", typeof (double), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty DropButtonInsetProperty = DependencyProperty.Register("DropButtonInset", typeof (Thickness), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty TextAreaInsetProperty = DependencyProperty.Register("TextAreaInset", typeof (Thickness), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty DropButtonBrushProperty = DependencyProperty.Register("DropButtonBrush", typeof (Brush), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty InnerCornerRadiusProperty = DependencyProperty.Register("InnerCornerRadius", typeof (double), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ButtonIconProperty = DependencyProperty.Register("ButtonIcon", typeof (ImageSource), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof (double), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof (double), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BeginCommandProperty = DependencyProperty.Register("BeginCommand", typeof (ICommand), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty UpdateCommandProperty = DependencyProperty.Register("UpdateCommand", typeof (ICommand), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof (ICommand), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty CommitCommandProperty = DependencyProperty.Register("CommitCommand", typeof (ICommand), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof (ICommand), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ComboBoxLoadingCursorProperty = DependencyProperty.Register("ComboBoxLoadingCursor", typeof (Cursor), typeof (ChoiceEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ForceBindingProperty = DependencyProperty.Register("ForceBinding", typeof (bool), typeof (ChoiceEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private string internalStringValue = string.Empty;
    private bool isTextEditing;
    private bool isSelectingValue;
    private int internalChangeLockCount;
    private int internalStringValueChangeLockCount;
    private bool ignoreValueChanges;
    private ChoiceEditor.LostFocusAction lostFocusAction;
    private object internalValue;
    private CollectionView collectionView;
    private bool templateApplyPending;

    public object Value
    {
      get
      {
        return this.GetValue(ChoiceEditor.ValueProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ValueProperty, value);
      }
    }

    public int ValueIndex
    {
      get
      {
        return (int) this.GetValue(ChoiceEditor.ValueIndexProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ValueIndexProperty, (object) value);
      }
    }

    public IEnumerable ItemsSource
    {
      get
      {
        return (IEnumerable) this.GetValue(ChoiceEditor.ItemsSourceProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ItemsSourceProperty, (object) value);
      }
    }

    public TypeConverter Converter
    {
      get
      {
        return (TypeConverter) this.GetValue(ChoiceEditor.ConverterProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ConverterProperty, (object) value);
      }
    }

    public ChoiceEditorViewType ViewType
    {
      get
      {
        return (ChoiceEditorViewType) this.GetValue(ChoiceEditor.ViewTypeProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ViewTypeProperty, (object) value);
      }
    }

    public bool IsEditable
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.IsEditableProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.IsEditableProperty, value);
        }
    }

    public bool VerifyEditableInput
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.VerifyEditableInputProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.VerifyEditableInputProperty, value);
        }
    }

    public string IconResourcePrefix
    {
      get
      {
        return (string) this.GetValue(ChoiceEditor.IconResourcePrefixProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.IconResourcePrefixProperty, (object) value);
      }
    }

    public string IconResourceSuffix
    {
      get
      {
        return (string) this.GetValue(ChoiceEditor.IconResourceSuffixProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.IconResourceSuffixProperty, (object) value);
      }
    }

    public bool IsNinched
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.IsNinchedProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.IsNinchedProperty, value);
        }
    }

    public bool ShowFullControl
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.ShowFullControlProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.ShowFullControlProperty, value);
        }
    }

    public DataTemplate ItemTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(ChoiceEditor.ItemTemplateProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ItemTemplateProperty, (object) value);
      }
    }

    public DataTemplateSelector ItemTemplateSelector
    {
      get
      {
        return (DataTemplateSelector) this.GetValue(ChoiceEditor.ItemTemplateSelectorProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ItemTemplateSelectorProperty, (object) value);
      }
    }

    public GroupStyle GroupStyle
    {
      get
      {
        return (GroupStyle) this.GetValue(ChoiceEditor.GroupStyleProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.GroupStyleProperty, (object) value);
      }
    }

    public GroupStyleSelector GroupStyleSelectorInternal { get; private set; }

    public bool UseItemTemplateForSelection
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.UseItemTemplateForSelectionProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.UseItemTemplateForSelectionProperty, value);
        }
    }

    public double BorderCornerRadius
    {
      get
      {
        return (double) this.GetValue(ChoiceEditor.BorderCornerRadiusProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.BorderCornerRadiusProperty, (object) value);
      }
    }

    public Thickness DropButtonInset
    {
      get
      {
        return (Thickness) this.GetValue(ChoiceEditor.DropButtonInsetProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.DropButtonInsetProperty, (object) value);
      }
    }

    public Thickness TextAreaInset
    {
      get
      {
        return (Thickness) this.GetValue(ChoiceEditor.TextAreaInsetProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.TextAreaInsetProperty, (object) value);
      }
    }

    public Brush DropButtonBrush
    {
      get
      {
        return (Brush) this.GetValue(ChoiceEditor.DropButtonBrushProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.DropButtonBrushProperty, (object) value);
      }
    }

    public double InnerCornerRadius
    {
      get
      {
        return (double) this.GetValue(ChoiceEditor.InnerCornerRadiusProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.InnerCornerRadiusProperty, (object) value);
      }
    }

    public ImageSource ButtonIcon
    {
      get
      {
        return (ImageSource) this.GetValue(ChoiceEditor.ButtonIconProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ButtonIconProperty, (object) value);
      }
    }

    public double IconWidth
    {
      get
      {
        return (double) this.GetValue(ChoiceEditor.IconWidthProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.IconWidthProperty, (object) value);
      }
    }

    public double IconHeight
    {
      get
      {
        return (double) this.GetValue(ChoiceEditor.IconHeightProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.IconHeightProperty, (object) value);
      }
    }

    public ICommand BeginCommand
    {
      get
      {
        return (ICommand) this.GetValue(ChoiceEditor.BeginCommandProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.BeginCommandProperty, (object) value);
      }
    }

    public ICommand UpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(ChoiceEditor.UpdateCommandProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.UpdateCommandProperty, (object) value);
      }
    }

    public ICommand CancelCommand
    {
      get
      {
        return (ICommand) this.GetValue(ChoiceEditor.CancelCommandProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.CancelCommandProperty, (object) value);
      }
    }

    public ICommand CommitCommand
    {
      get
      {
        return (ICommand) this.GetValue(ChoiceEditor.CommitCommandProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.CommitCommandProperty, (object) value);
      }
    }

    public ICommand FinishEditingCommand
    {
      get
      {
        return (ICommand) this.GetValue(ChoiceEditor.FinishEditingCommandProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.FinishEditingCommandProperty, (object) value);
      }
    }

    public Cursor ComboBoxLoadingCursor
    {
      get
      {
        return (Cursor) this.GetValue(ChoiceEditor.ComboBoxLoadingCursorProperty);
      }
      set
      {
        this.SetValue(ChoiceEditor.ComboBoxLoadingCursorProperty, (object) value);
      }
    }

    public ICommand NextValueCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectNextValue));
      }
    }

    public ICommand PreviousValueCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectPreviousValue));
      }
    }

    public bool ForceBinding
    {
        get
        {
            return (bool)base.GetValue(ChoiceEditor.ForceBindingProperty);
        }
        set
        {
            base.SetValue(ChoiceEditor.ForceBindingProperty, value);
        }
    }

    public object InternalValue
    {
      get
      {
        return this.internalValue;
      }
      set
      {
        if (this.internalValue == value)
          return;
        this.internalValue = value;
        if (this.ShouldCommitInternalValueChanges)
        {
          if (!this.isTextEditing)
            this.CommitChange();
          else
            this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
        }
        if (this.isTextEditing)
          this.internalStringValue = (value ?? (object) string.Empty).ToString();
        else
          this.SendPropertyChanged("InternalValue");
      }
    }

    public string InternalStringValue
    {
      get
      {
        return this.internalStringValue;
      }
      set
      {
        if (string.Equals(value, this.internalStringValue))
          return;
        if (this.ShouldCommitInternalStringValueChanges && this.isTextEditing)
        {
          this.InternalValue = (object) null;
          this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
        }
        this.internalStringValue = value;
        this.SendPropertyChanged("InternalStringValue");
      }
    }

    public bool InternalIsSelectingValue
    {
      get
      {
        return this.isSelectingValue;
      }
      set
      {
        if (this.isSelectingValue == value)
          return;
        this.isSelectingValue = value;
        if (this.isTextEditing && !this.isSelectingValue)
          this.OnFinishEditing();
        if (this.isSelectingValue && this.collectionView != null)
        {
          this.BeginIgnoreExternalValueChangeBlock();
          try
          {
            this.ValueIndex = this.IndexOf(this.Value);
          }
          finally
          {
            this.EndIgnoreExternalValueChangeBlock();
          }
        }
        Cursor boxLoadingCursor = this.ComboBoxLoadingCursor;
        if (!value || this.ViewType != ChoiceEditorViewType.Combo || boxLoadingCursor == null)
          return;
        bool flag = false;
        Mouse.OverrideCursor = boxLoadingCursor;
        ComboBox comboBox = this.Template.FindName("PART_Combo", (FrameworkElement) this) as ComboBox;
        if (comboBox != null)
        {
          Popup popup = comboBox.Template.FindName("PART_Popup", (FrameworkElement) comboBox) as Popup;
          if (popup != null)
          {
            flag = true;
            popup.Opened += new EventHandler(this.OnPopupLoaded);
          }
        }
        if (flag)
          return;
        Mouse.OverrideCursor = (Cursor) null;
      }
    }

    private bool ShouldCommitInternalValueChanges
    {
      get
      {
        return this.internalChangeLockCount == 0;
      }
    }

    private bool ShouldCommitInternalStringValueChanges
    {
      get
      {
        return this.internalStringValueChangeLockCount == 0;
      }
    }

    private bool ShouldIgnoreExternalValueChanges
    {
      get
      {
        return this.ignoreValueChanges;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ChoiceEditor()
    {
      this.GroupStyleSelectorInternal = new GroupStyleSelector(this.GroupStyleSelectorImplementation);
      this.CoerceValue(ChoiceEditor.UseItemTemplateForSelectionProperty);
    }

    public GroupStyle GroupStyleSelectorImplementation(CollectionViewGroup group, int level)
    {
      return this.GroupStyle;
    }

    public void SelectNextValue()
    {
      if (this.collectionView == null)
        return;
      int num = this.collectionView.IndexOf(this.InternalValue);
      if (num == -1)
        num = this.ValueIndex;
      this.ValueIndex = num < 0 || num >= this.collectionView.Count - 1 ? 0 : num + 1;
    }

    public void SelectPreviousValue()
    {
      if (this.collectionView == null)
        return;
      int num = this.collectionView.IndexOf(this.InternalValue);
      if (num == -1)
        num = this.ValueIndex;
      this.ValueIndex = num <= 0 || num > this.collectionView.Count - 1 ? this.collectionView.Count - 1 : num - 1;
    }

    public ImageSource GetIconAsImageSource(object key, object parameter)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        string iconResourcePrefix = this.IconResourcePrefix;
        string iconResourceSuffix = this.IconResourceSuffix;
        string str = parameter as string;
        if (iconResourcePrefix != null)
          stringBuilder.Append(iconResourcePrefix);
        stringBuilder.Append(key.ToString());
        if (iconResourceSuffix != null)
          stringBuilder.Append(iconResourceSuffix);
        if (str != null)
          stringBuilder.Append(str);
        return this.FindResource((object) stringBuilder.ToString()) as ImageSource;
      }
      catch (ResourceReferenceKeyNotFoundException ex)
      {
        return (ImageSource) null;
      }
    }

    private void OnPopupLoaded(object sender, EventArgs e)
    {
      Popup popup = sender as Popup;
      Mouse.OverrideCursor = (Cursor) null;
      if (popup == null)
        return;
      popup.Opened -= new EventHandler(this.OnPopupLoaded);
    }

    private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (object.Equals(e.OldValue, e.NewValue))
        return;
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null || choiceEditor.ShouldIgnoreExternalValueChanges)
        return;
      choiceEditor.UpdateInternalValuesFromValue();
    }

    private static void ValueIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null || choiceEditor.ShouldIgnoreExternalValueChanges)
        return;
      choiceEditor.UpdateInternalValuesFromValueIndex();
      choiceEditor.UpdateValueFromInternalValues();
    }

    private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null)
        return;
      choiceEditor.ItemsSourceChanged();
    }

    private static void IsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null || choiceEditor.ShouldIgnoreExternalValueChanges)
        return;
      choiceEditor.UpdateInternalValuesFromValue();
    }

    private static void VerifyEditableInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      bool flag = (bool) e.NewValue;
      if (choiceEditor == null || !flag || (choiceEditor.collectionView == null || choiceEditor.IndexOf(choiceEditor.Value) != -1))
        return;
      choiceEditor.ValueIndex = 0;
      choiceEditor.UpdateInternalValuesFromValue();
    }

    private static void IsNinchedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null || choiceEditor.ShouldIgnoreExternalValueChanges)
        return;
      choiceEditor.UpdateInternalValuesFromValue();
    }

    private static object CoerceShowFullControl(DependencyObject target, object value)
    {
        ChoiceEditor choiceEditor = target as ChoiceEditor;
        if (choiceEditor == null || !(value is bool) || (bool)value)
        {
            return value;
        }
        return choiceEditor.isTextEditing;
    }

    private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null)
        return;
      choiceEditor.CoerceValue(ChoiceEditor.UseItemTemplateForSelectionProperty);
    }

    private static void ItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null)
        return;
      choiceEditor.CoerceValue(ChoiceEditor.UseItemTemplateForSelectionProperty);
    }

    private static object CoerceUseItemTemplateForSelection(DependencyObject target, object value)
    {
      ChoiceEditor choiceEditor = target as ChoiceEditor;
      if (choiceEditor != null && value == null)
        return (object) (bool) (choiceEditor.ItemTemplate != null ? true : (choiceEditor.ItemTemplateSelector != null ? true : false));
      return value;
    }

    private static void GroupStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ChoiceEditor choiceEditor = d as ChoiceEditor;
      if (choiceEditor == null)
        return;
      choiceEditor.SendPropertyChanged("GroupStyleSelectorInternal");
    }

    private void SendPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnPreviewGotKeyboardFocus(e);
      FrameworkElement frameworkElement = e.NewFocus as FrameworkElement;
      if (frameworkElement == null || !frameworkElement.Name.Equals("PART_EditableTextBox", StringComparison.Ordinal))
        return;
      this.isTextEditing = true;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      bool handlesCommitKeys = ValueEditorUtils.GetHandlesCommitKeys((DependencyObject) this);
      if (e.Key == Key.Return || e.Key == Key.Return)
      {
        KeyEventArgs keyEventArgs = e;
        int num = keyEventArgs.Handled | handlesCommitKeys ? 1 : 0;
        keyEventArgs.Handled = num != 0;
        ChoiceEditor.LostFocusAction lostFocusAction = this.lostFocusAction;
        this.lostFocusAction = ChoiceEditor.LostFocusAction.None;
        if (lostFocusAction == ChoiceEditor.LostFocusAction.Commit)
          this.CommitChange();
        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
          this.OnFinishEditing();
      }
      else if (e.Key == Key.Escape)
      {
        KeyEventArgs keyEventArgs = e;
        int num = keyEventArgs.Handled | handlesCommitKeys ? 1 : 0;
        keyEventArgs.Handled = num != 0;
        ChoiceEditor.LostFocusAction lostFocusAction = this.lostFocusAction;
        this.lostFocusAction = ChoiceEditor.LostFocusAction.None;
        if (lostFocusAction != ChoiceEditor.LostFocusAction.None)
          this.CancelChange();
        this.OnFinishEditing();
      }
      if (this.InternalIsSelectingValue && this.collectionView != null && !this.collectionView.IsEmpty)
      {
        if (e.Key == Key.Up || !this.IsEditable && e.Key == Key.Left)
        {
          this.SelectPreviousValue();
          this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
          e.Handled = true;
        }
        else if (e.Key == Key.Down || !this.IsEditable && e.Key == Key.Right)
        {
          this.SelectNextValue();
          this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
          e.Handled = true;
        }
        else if (!this.IsEditable && e.Key == Key.Home)
        {
          this.ValueIndex = 0;
          this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
          e.Handled = true;
        }
        else if (!this.IsEditable && e.Key == Key.End)
        {
          this.ValueIndex = this.collectionView.Count - 1;
          this.lostFocusAction = ChoiceEditor.LostFocusAction.Commit;
          e.Handled = true;
        }
      }
      base.OnPreviewKeyDown(e);
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnPreviewLostKeyboardFocus(e);
      FrameworkElement frameworkElement = e.OldFocus as FrameworkElement;
      if (frameworkElement == null || !frameworkElement.Name.Equals("PART_EditableTextBox", StringComparison.Ordinal))
        return;
      this.HandleLostFocus();
    }

    private void HandleLostFocus()
    {
      if (!this.isTextEditing)
        return;
      ChoiceEditor.LostFocusAction lostFocusAction = this.lostFocusAction;
      this.lostFocusAction = ChoiceEditor.LostFocusAction.None;
      this.isTextEditing = false;
      if (lostFocusAction == ChoiceEditor.LostFocusAction.Commit)
        this.CommitChange();
      else if (lostFocusAction == ChoiceEditor.LostFocusAction.Cancel)
        this.CancelChange();
      this.CoerceValue(ChoiceEditor.ShowFullControlProperty);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      if (this.ViewType != ChoiceEditorViewType.Combo || this.ShowFullControl)
        return;
      double borderCornerRadius = this.BorderCornerRadius;
      Thickness dropButtonInset = this.DropButtonInset;
      Thickness textAreaInset = this.TextAreaInset;
      Brush dropButtonBrush = this.DropButtonBrush;
      double innerCornerRadius = this.InnerCornerRadius;
      ImageSource buttonIcon = this.ButtonIcon;
      double iconWidth = this.IconWidth;
      double iconHeight = this.IconHeight;
      Rect outerBounds = new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight);
      if (!RenderUtils.DrawInscribedRoundedRect(drawingContext, this.BorderBrush, (Pen) null, outerBounds, borderCornerRadius))
        return;
      Rect rect = RenderUtils.CalculateInnerRect(outerBounds, 0.0);
      double x1 = (rect.Right > textAreaInset.Right ? rect.Right - textAreaInset.Right : 0.0) + dropButtonInset.Left;
      double y1 = rect.Top + dropButtonInset.Top;
      double x2 = rect.Right - dropButtonInset.Right;
      double y2 = rect.Bottom - dropButtonInset.Bottom;
      RenderUtils.DrawInscribedRoundedRect(drawingContext, dropButtonBrush, (Pen) null, new Rect(new Point(x1, y1), new Point(x2, y2)), innerCornerRadius);
      if (buttonIcon != null)
      {
        double num1 = x1 + (x2 - x1) / 2.0;
        double num2 = y1 + (y2 - y1) / 2.0;
        drawingContext.DrawImage(buttonIcon, new Rect(new Point(num1 - iconWidth / 2.0, num2 - iconHeight / 2.0), new Point(num1 + iconWidth / 2.0, num2 + iconHeight / 2.0)));
      }
      double x3 = rect.Left + textAreaInset.Left;
      double y3 = rect.Top + textAreaInset.Top;
      double x4 = rect.Right > textAreaInset.Right ? rect.Right - textAreaInset.Right : x3;
      double y4 = rect.Bottom - textAreaInset.Bottom;
      RenderUtils.DrawInscribedRoundedRect(drawingContext, this.Background, (Pen) null, new Rect(new Point(x3, y3), new Point(x4, y4)), innerCornerRadius);
    }

    protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
    {
      this.ForceBinding = false;
      if (!this.templateApplyPending)
      {
        this.BeginNoCommitInternalValueChangeBlock();
        this.templateApplyPending = true;
      }
      base.OnTemplateChanged(oldTemplate, newTemplate);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.ForceBinding = true;
      this.templateApplyPending = false;
      this.EndNoCommitInternalValueChangeBlock();
      this.HandleLostFocus();
    }

    private void CommitChange()
    {
      ValueEditorUtils.ExecuteCommand(this.BeginCommand, (IInputElement) this, (object) null);
      if (this.UpdateValueFromInternalValues())
      {
        ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueProperty, UpdateBindingType.Source);
        ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueIndexProperty, UpdateBindingType.Source);
        ValueEditorUtils.ExecuteCommand(this.CommitCommand, (IInputElement) this, (object) null);
        ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueProperty, UpdateBindingType.Target);
        ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueIndexProperty, UpdateBindingType.Target);
      }
      else
        this.CancelStartedChange();
      this.lostFocusAction = ChoiceEditor.LostFocusAction.None;
    }

    private void CancelChange()
    {
      ValueEditorUtils.ExecuteCommand(this.BeginCommand, (IInputElement) this, (object) null);
      this.CancelStartedChange();
    }

    private void CancelStartedChange()
    {
      ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueProperty, UpdateBindingType.Target);
      ValueEditorUtils.UpdateBinding((FrameworkElement) this, ChoiceEditor.ValueIndexProperty, UpdateBindingType.Target);
      this.UpdateInternalValuesFromValue();
      ValueEditorUtils.ExecuteCommand(this.CancelCommand, (IInputElement) this, (object) null);
    }

    private void OnFinishEditing()
    {
      ICommand finishEditingCommand = this.FinishEditingCommand;
      if (finishEditingCommand != null)
        ValueEditorUtils.ExecuteCommand(finishEditingCommand, (IInputElement) this, (object) null);
      else
        Keyboard.Focus((IInputElement) null);
    }

    private void ItemsSourceChanged()
    {
      this.collectionView = this.ItemsSource == null ? (CollectionView) null : new CollectionView(this.ItemsSource);
      this.UpdateInternalValuesFromValue();
      this.UpdateValueFromInternalValues();
    }

    private int IndexOf(object item)
    {
      if (this.collectionView != null)
        return this.collectionView.IndexOf(item);
      return -1;
    }

    private object GetItemAt(int index)
    {
      if (this.collectionView != null)
        return this.collectionView.GetItemAt(index);
      return (object) null;
    }

    protected void UpdateInternalValuesFromValue()
    {
      this.BeginNoCommitInternalValueChangeBlock();
      this.BeginNoCommitInternalStringValueChangeBlock();
      try
      {
        this.InternalValue = this.IsNinched ? (object) null : this.Value;
        if (!this.IsEditable)
          return;
        string str = string.Empty;
        if (this.InternalValue != null && !this.IsNinched)
        {
          TypeConverter converter = this.Converter;
          str = converter == null || !converter.CanConvertFrom(this.InternalValue.GetType()) ? this.InternalValue.ToString() : converter.ConvertToString(this.InternalValue);
        }
        this.InternalStringValue = str;
      }
      finally
      {
        this.EndNoCommitInternalStringValueChangeBlock();
        this.EndNoCommitInternalValueChangeBlock();
      }
    }

    public void UpdateInternalValuesFromValueIndex()
    {
      this.BeginNoCommitInternalValueChangeBlock();
      this.BeginNoCommitInternalStringValueChangeBlock();
      try
      {
        this.InternalValue = this.GetItemAt(this.ValueIndex);
        this.SendPropertyChanged("InternalValue");
      }
      finally
      {
        this.EndNoCommitInternalStringValueChangeBlock();
        this.EndNoCommitInternalValueChangeBlock();
      }
    }

    protected bool UpdateValueFromInternalValues()
    {
      this.BeginIgnoreExternalValueChangeBlock();
      try
      {
        if (!this.IsEditable)
          this.Value = this.InternalValue;
        else if (this.InternalValue != null)
        {
          if (this.VerifyEditableInput)
          {
            if (this.IndexOf(this.InternalValue) != -1)
              this.Value = this.InternalValue;
          }
          else
            this.Value = this.InternalValue;
        }
        else
        {
          string internalStringValue = this.InternalStringValue;
          if (internalStringValue != null)
          {
            TypeConverter converter = this.Converter;
            object obj;
            if (converter != null)
            {
              try
              {
                obj = converter.ConvertFromString(internalStringValue);
              }
              catch (Exception ex)
              {
                return false;
              }
            }
            else
              obj = (object) internalStringValue;
            if (this.VerifyEditableInput)
            {
              if (this.IndexOf(obj) != -1)
                this.Value = obj;
            }
            else
              this.Value = obj;
            if (obj != this.InternalValue)
              this.UpdateInternalValuesFromValue();
          }
        }
        this.ValueIndex = this.IndexOf(this.Value);
      }
      finally
      {
        this.EndIgnoreExternalValueChangeBlock();
      }
      return true;
    }

    protected void BeginNoCommitInternalValueChangeBlock()
    {
      ++this.internalChangeLockCount;
    }

    protected void EndNoCommitInternalValueChangeBlock()
    {
      --this.internalChangeLockCount;
    }

    protected void BeginNoCommitInternalStringValueChangeBlock()
    {
      ++this.internalStringValueChangeLockCount;
    }

    protected void EndNoCommitInternalStringValueChangeBlock()
    {
      --this.internalStringValueChangeLockCount;
    }

    protected void BeginIgnoreExternalValueChangeBlock()
    {
      this.ignoreValueChanges = true;
    }

    protected void EndIgnoreExternalValueChangeBlock()
    {
      this.ignoreValueChanges = false;
    }

    private enum LostFocusAction
    {
      None,
      Commit,
      Cancel,
    }
  }
}

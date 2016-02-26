// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextChoiceEditorWrapper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TextChoiceEditorWrapper : UserControl, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty WrappedValueProperty = DependencyProperty.Register("WrappedValue", typeof (object), typeof (TextChoiceEditorWrapper), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TextChoiceEditorWrapper.WrappedValueChanged), (CoerceValueCallback) null, false, UpdateSourceTrigger.PropertyChanged));
    public static readonly DependencyProperty UnitTypeProperty = DependencyProperty.Register("UnitType", typeof (UnitType), typeof (TextChoiceEditorWrapper), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextChoiceEditorWrapper.UnitTypeChanged)));
    private bool isUpdatingValue;
    private Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextSizeConverter textTypeConverter;
    private DependencyPropertyDescriptor choiceEditorValueDescriptor;
    internal TextChoiceEditorWrapper TextChoiceEditorWrapper_1;
    internal ChoiceEditor TextChoiceEditor;
    private bool _contentLoaded;

    public TypeConverter TextSizeConverter
    {
      get
      {
        return (TypeConverter) this.textTypeConverter;
      }
    }

    public object WrappedValue
    {
      get
      {
        return this.GetValue(TextChoiceEditorWrapper.WrappedValueProperty);
      }
      set
      {
        this.SetValue(TextChoiceEditorWrapper.WrappedValueProperty, value);
      }
    }

    public UnitType UnitType
    {
      get
      {
        return (UnitType) this.GetValue(TextChoiceEditorWrapper.UnitTypeProperty);
      }
      set
      {
        this.SetValue(TextChoiceEditorWrapper.UnitTypeProperty, value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TextChoiceEditorWrapper()
    {
      this.textTypeConverter = new Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextSizeConverter(this.UnitType);
      this.choiceEditorValueDescriptor = DependencyPropertyDescriptor.FromProperty(ChoiceEditor.ValueProperty, typeof (ChoiceEditor));
      this.Loaded += new RoutedEventHandler(this.TextChoiceEditorWrapper_Loaded);
      this.Unloaded += new RoutedEventHandler(this.TextChoiceEditorWrapper_Unloaded);
      this.InitializeComponent();
    }

    private void TextChoiceEditorWrapper_Loaded(object sender, RoutedEventArgs e)
    {
      this.choiceEditorValueDescriptor.AddValueChanged(this.TextChoiceEditor, new EventHandler(this.ChoiceEditorValueChanged));
    }

    private void TextChoiceEditorWrapper_Unloaded(object sender, RoutedEventArgs e)
    {
      this.choiceEditorValueDescriptor.RemoveValueChanged(this.TextChoiceEditor, new EventHandler(this.ChoiceEditorValueChanged));
    }

    private void ChoiceEditorValueChanged(object sender, EventArgs e)
    {
      if (this.isUpdatingValue)
        return;
      this.isUpdatingValue = true;
      try
      {
        object obj = this.TextChoiceEditor.Value;
        if (obj == null)
        {
          this.WrappedValue = MixedProperty.Mixed;
        }
        else
        {
          UnitTypedSize unitTypedSize1 = obj as UnitTypedSize;
          if (unitTypedSize1 == null)
            return;
          UnitTypedSize unitTypedSize2 = unitTypedSize1.ConvertTo(UnitType.Pixels);
          double size = unitTypedSize2.Size;
          unitTypedSize2.Size = this.EnforceHardMaximumAndMinimum(unitTypedSize2.Size);
          if (size != unitTypedSize2.Size)
            this.TextChoiceEditor.Value = (object) unitTypedSize2.ConvertTo(this.UnitType);
          unitTypedSize2.Size = RoundingHelper.RoundScale(unitTypedSize2.Size);
          this.WrappedValue = (object) unitTypedSize2.Size;
        }
      }
      finally
      {
        this.isUpdatingValue = false;
      }
    }

    private double EnforceHardMaximumAndMinimum(double size)
    {
      SceneNodePropertyValue nodePropertyValue = this.DataContext as SceneNodePropertyValue;
      if (nodePropertyValue != null)
      {
        SceneNodeProperty sceneNodeProperty = nodePropertyValue.get_ParentProperty() as SceneNodeProperty;
        if (sceneNodeProperty != null && sceneNodeProperty.ValueEditorParameters != null)
        {
          NumberRangesAttribute numberRangesAttribute = sceneNodeProperty.ValueEditorParameters["NumberRanges"] as NumberRangesAttribute;
          if (numberRangesAttribute != null)
          {
            if (numberRangesAttribute.get_HardMaximum().HasValue && numberRangesAttribute.get_HardMaximum().HasValue)
              size = Math.Min(numberRangesAttribute.get_HardMaximum().Value, size);
            if (numberRangesAttribute.get_HardMinimum().HasValue && numberRangesAttribute.get_HardMinimum().HasValue)
              size = Math.Max(numberRangesAttribute.get_HardMinimum().Value, size);
          }
        }
      }
      return size;
    }

    private static void UnitTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (object.Equals(e.OldValue, e.NewValue))
        return;
      TextChoiceEditorWrapper choiceEditorWrapper = d as TextChoiceEditorWrapper;
      if (choiceEditorWrapper == null || !(e.NewValue is UnitType))
        return;
      UnitType unitType = (UnitType) e.NewValue;
      choiceEditorWrapper.textTypeConverter = new Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextSizeConverter(unitType);
      choiceEditorWrapper.OnPropertyChanged("TextSizeConverter");
      if (choiceEditorWrapper.isUpdatingValue)
        return;
      choiceEditorWrapper.isUpdatingValue = true;
      try
      {
        UnitTypedSize unitTypedSize = choiceEditorWrapper.TextChoiceEditor.Value as UnitTypedSize;
        if (unitTypedSize == null)
          return;
        choiceEditorWrapper.TextChoiceEditor.Value = (object) unitTypedSize.ConvertTo(unitType);
      }
      finally
      {
        choiceEditorWrapper.isUpdatingValue = false;
      }
    }

    private static void WrappedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (object.Equals(e.OldValue, e.NewValue))
        return;
      TextChoiceEditorWrapper choiceEditorWrapper = d as TextChoiceEditorWrapper;
      if (choiceEditorWrapper == null || choiceEditorWrapper.isUpdatingValue)
        return;
      choiceEditorWrapper.isUpdatingValue = true;
      try
      {
        object newValue = e.NewValue;
        if (newValue != null && newValue != MixedProperty.Mixed && newValue is double)
          choiceEditorWrapper.TextChoiceEditor.Value = (object) UnitTypedSize.CreateFromPixels((double) newValue, choiceEditorWrapper.UnitType);
        else
          choiceEditorWrapper.TextChoiceEditor.Value = null;
      }
      finally
      {
        choiceEditorWrapper.isUpdatingValue = false;
      }
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
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/text/textchoiceeditorwrapper.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.TextChoiceEditorWrapper_1 = (TextChoiceEditorWrapper) target;
          break;
        case 2:
          this.TextChoiceEditor = (ChoiceEditor) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

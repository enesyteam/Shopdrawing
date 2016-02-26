// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.NumberEditor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class NumberEditor : Control, INotifyEdit
  {
    private SuperRoundedRectRenderer sliderRenderer = new SuperRoundedRectRenderer();
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(NumberEditor.This_ValueChanged), (CoerceValueCallback) null, false, UpdateSourceTrigger.PropertyChanged));
    public static readonly DependencyProperty AutoValueProperty = DependencyProperty.Register("AutoValue", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(NumberEditor.This_ValueChanged), (CoerceValueCallback) null, false, UpdateSourceTrigger.PropertyChanged));
    public static readonly DependencyProperty FormattedValueProperty = DependencyProperty.Register("FormattedValue", typeof (string), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(NumberEditor.This_CoerceFormattedValue)));
    public static readonly DependencyProperty StringValueProperty = DependencyProperty.Register("StringValue", typeof (string), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty HardMinimumProperty = DependencyProperty.Register("HardMinimum", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(NumberEditor.This_HardMinimumChanged), new CoerceValueCallback(NumberEditor.This_CoerceHardMinimum)));
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.MinValue, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(NumberEditor.This_MinimumChanged)));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.MaxValue, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(NumberEditor.This_MaximumChanged)));
    public static readonly DependencyProperty HardMaximumProperty = DependencyProperty.Register("HardMaximum", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(NumberEditor.This_HardMaximumChanged), new CoerceValueCallback(NumberEditor.This_CoerceHardMaximum)));
    public static readonly DependencyProperty DefaultChangeProperty = DependencyProperty.Register("DefaultChange", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, new PropertyChangedCallback(NumberEditor.This_DefaultChangeChanged)));
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(NumberEditor.This_SmallChangeChanged), new CoerceValueCallback(NumberEditor.This_CoerceSmallChange)));
    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, (PropertyChangedCallback) null, new CoerceValueCallback(NumberEditor.This_CoerceLargeChange)));
    public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof (string), typeof (NumberEditor), new PropertyMetadata((object) "0.#", new PropertyChangedCallback(NumberEditor.This_FormatChanged)));
    public static readonly DependencyProperty IsTextEditingProperty = DependencyProperty.Register("IsTextEditing", typeof (bool), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty DragNotifierProperty = DependencyProperty.Register("DragNotifier", typeof (IDragEditNotifier), typeof (NumberEditor), new PropertyMetadata((object) null, new PropertyChangedCallback(NumberEditor.This_OnDragNotifierChanged)));
    public static readonly DependencyProperty IsNinchedProperty = DependencyProperty.Register("IsNinched", typeof (bool), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(NumberEditor.This_IsNinchedChanged)));
    public static readonly DependencyProperty IsSliderLogarithmicProperty = DependencyProperty.Register("IsSliderLogarithmic", typeof (bool), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register("Converter", typeof (TypeConverter), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(NumberEditor.This_ConverterChanged)));
    public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, new PropertyChangedCallback(NumberEditor.This_ScaleChanged)));
    public static readonly DependencyProperty MaxPrecisionProperty = DependencyProperty.Register("MaxPrecision", typeof (int), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) int.MaxValue, FrameworkPropertyMetadataOptions.None, (PropertyChangedCallback) null, new CoerceValueCallback(NumberEditor.This_CoerceMaxPrecision)));
    public static readonly DependencyProperty AlwaysEnforceMaxPrecisionProperty = DependencyProperty.Register("AlwaysEnforceMaxPrecision", typeof (bool), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty CanBeAutoProperty = DependencyProperty.Register("CanBeAuto", typeof (bool), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty NamedValuePickerTemplateProperty = DependencyProperty.Register("NamedValuePickerTemplate", typeof (DataTemplate), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DragBehaviorProperty = DependencyProperty.Register("DragBehavior", typeof (INumberEditorDragBehavior), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new FadingDragBehavior()));
    public static readonly DependencyProperty StringValueFilterProperty = DependencyProperty.Register("StringValueFilter", typeof (IStringValueFilter), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new PercentageStringValueFilter()));
    public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof (Brush), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty HighlightMarginProperty = DependencyProperty.Register("HighlightMargin", typeof (Thickness), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty HighlightCornerRadiusProperty = DependencyProperty.Register("HighlightCornerRadius", typeof (System.Windows.CornerRadius), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new System.Windows.CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty HighlightHeightProperty = DependencyProperty.Register("HighlightHeight", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ShowSliderProperty = DependencyProperty.Register("ShowSlider", typeof (bool?), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender, (PropertyChangedCallback) null, new CoerceValueCallback(NumberEditor.This_CoerceShowSlider)));
    public static readonly DependencyProperty NumberAreaMarginProperty = DependencyProperty.Register("NumberAreaMargin", typeof (Thickness), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SliderBrushProperty = DependencyProperty.Register("SliderBrush", typeof (Brush), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SliderCornerRadiusProperty = DependencyProperty.Register("SliderCornerRadius", typeof (System.Windows.CornerRadius), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new System.Windows.CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof (double), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SliderBorderProperty = DependencyProperty.Register("SliderBorder", typeof (Thickness), typeof (NumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof (ICommand), typeof (NumberEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty UpdateEditCommandProperty = DependencyProperty.Register("UpdateEditCommand", typeof (ICommand), typeof (NumberEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty CancelEditCommandProperty = DependencyProperty.Register("CancelEditCommand", typeof (ICommand), typeof (NumberEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty CommitEditCommandProperty = DependencyProperty.Register("CommitEditCommand", typeof (ICommand), typeof (NumberEditor), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof (ICommand), typeof (NumberEditor), new PropertyMetadata((PropertyChangedCallback) null));
    private static double dragTolerance = 2.0;
    private static int maxPrecisionInDouble = 15;
    private static bool mouseWarpEnabled = true;
    private static TypeConverter doubleConverter = (TypeConverter) NoNanDoubleConverter.Instance;
    private bool ignoreNextMove;
    private Point dragStartPoint;
    private Point lastDragPoint;
    private double valueChangeBase;
    private double valueChangeOffset;
    private double initialValue;
    private bool isDragging;
    private bool isMouseDown;
    private bool isStylusDown;
    private bool hasCommittedTextEdit;
    private bool hasCancelledTextEdit;
    private bool shouldCancel;
    private bool hasChangedText;
    private DelegateCommand beginTextEditCommand;
    private DelegateCommand cancelTextEditCommand;
    private DelegateCommand commitTextEditCommand;
    private DelegateCommand updateTextEditCommand;
    private DelegateCommand textLostFocusCommand;
    private ArgumentDelegateCommand textChangedCommand;
    private ArgumentDelegateCommand setAutoCommand;
    private SuperRoundedRectRenderer highlightRenderer;

    public static bool MouseWarpEnabled
    {
      get
      {
        return NumberEditor.mouseWarpEnabled;
      }
      set
      {
        NumberEditor.mouseWarpEnabled = value;
      }
    }

    public virtual double Value
    {
      get
      {
        return (double) this.GetValue(NumberEditor.ValueProperty);
      }
      set
      {
        this.SetValue(NumberEditor.ValueProperty, (object) value);
      }
    }

    public virtual double AutoValue
    {
      get
      {
        return (double) this.GetValue(NumberEditor.AutoValueProperty);
      }
      set
      {
        this.SetValue(NumberEditor.AutoValueProperty, (object) value);
      }
    }

    public virtual string FormattedValue
    {
      get
      {
        return (string) this.GetValue(NumberEditor.FormattedValueProperty);
      }
      protected set
      {
        this.SetValue(NumberEditor.FormattedValueProperty, (object) value);
      }
    }

    public virtual string StringValue
    {
      get
      {
        return (string) this.GetValue(NumberEditor.StringValueProperty);
      }
      set
      {
        this.SetValue(NumberEditor.StringValueProperty, (object) value);
      }
    }

    public double HardMinimum
    {
      get
      {
        return (double) this.GetValue(NumberEditor.HardMinimumProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HardMinimumProperty, (object) value);
      }
    }

    public double Minimum
    {
      get
      {
        return (double) this.GetValue(NumberEditor.MinimumProperty);
      }
      set
      {
        this.SetValue(NumberEditor.MinimumProperty, (object) value);
      }
    }

    public double Maximum
    {
      get
      {
        return (double) this.GetValue(NumberEditor.MaximumProperty);
      }
      set
      {
        this.SetValue(NumberEditor.MaximumProperty, (object) value);
      }
    }

    public double HardMaximum
    {
      get
      {
        return (double) this.GetValue(NumberEditor.HardMaximumProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HardMaximumProperty, (object) value);
      }
    }

    public double DefaultChange
    {
      get
      {
        return (double) this.GetValue(NumberEditor.DefaultChangeProperty);
      }
      set
      {
        this.SetValue(NumberEditor.DefaultChangeProperty, (object) value);
      }
    }

    public double SmallChange
    {
      get
      {
        return (double) this.GetValue(NumberEditor.SmallChangeProperty);
      }
      set
      {
        this.SetValue(NumberEditor.SmallChangeProperty, (object) value);
      }
    }

    public double LargeChange
    {
      get
      {
        return (double) this.GetValue(NumberEditor.LargeChangeProperty);
      }
      set
      {
        this.SetValue(NumberEditor.LargeChangeProperty, (object) value);
      }
    }

    public string Format
    {
      get
      {
        return (string) this.GetValue(NumberEditor.FormatProperty);
      }
      set
      {
        this.SetValue(NumberEditor.FormatProperty, (object) value);
      }
    }

    public bool IsTextEditing
    {
        get
        {
            return (bool)base.GetValue(NumberEditor.IsTextEditingProperty);
        }
        set
        {
            base.SetValue(NumberEditor.IsTextEditingProperty, value);
        }
    }

    public IDragEditNotifier DragNotifier
    {
      get
      {
        return (IDragEditNotifier) this.GetValue(NumberEditor.DragNotifierProperty);
      }
      set
      {
        this.SetValue(NumberEditor.DragNotifierProperty, (object) value);
      }
    }

    public bool IsNinched
    {
        get
        {
            return (bool)base.GetValue(NumberEditor.IsNinchedProperty);
        }
        set
        {
            base.SetValue(NumberEditor.IsNinchedProperty, value);
        }
    }

    public bool? ShowSlider
    {
      get
      {
        return (bool?) this.GetValue(NumberEditor.ShowSliderProperty);
      }
      set
      {
        this.SetValue(NumberEditor.ShowSliderProperty, (object) value);
      }
    }

    public bool IsSliderLogarithmic
    {
        get
        {
            return (bool)base.GetValue(NumberEditor.IsSliderLogarithmicProperty);
        }
        set
        {
            base.SetValue(NumberEditor.IsSliderLogarithmicProperty, value);
        }
    }

    public TypeConverter Converter
    {
      get
      {
        return (TypeConverter) this.GetValue(NumberEditor.ConverterProperty);
      }
      set
      {
        this.SetValue(NumberEditor.ConverterProperty, (object) value);
      }
    }

    public double Scale
    {
      get
      {
        return (double) this.GetValue(NumberEditor.ScaleProperty);
      }
      set
      {
        this.SetValue(NumberEditor.ScaleProperty, (object) value);
      }
    }

    public int MaxPrecision
    {
      get
      {
        return (int) this.GetValue(NumberEditor.MaxPrecisionProperty);
      }
      set
      {
        this.SetValue(NumberEditor.MaxPrecisionProperty, (object) value);
      }
    }

    public bool AlwaysEnforceMaxPrecision
    {
        get
        {
            return (bool)base.GetValue(NumberEditor.AlwaysEnforceMaxPrecisionProperty);
        }
        set
        {
            base.SetValue(NumberEditor.AlwaysEnforceMaxPrecisionProperty, value);
        }
    }

    public bool CanBeAuto
    {
        get
        {
            return (bool)base.GetValue(NumberEditor.CanBeAutoProperty);
        }
        set
        {
            base.SetValue(NumberEditor.CanBeAutoProperty, value);
        }
    }

    public INumberEditorDragBehavior DragBehavior
    {
      get
      {
        return (INumberEditorDragBehavior) this.GetValue(NumberEditor.DragBehaviorProperty);
      }
      set
      {
        this.SetValue(NumberEditor.DragBehaviorProperty, (object) value);
      }
    }

    public IStringValueFilter StringValueFilter
    {
      get
      {
        return (IStringValueFilter) this.GetValue(NumberEditor.StringValueFilterProperty);
      }
      set
      {
        this.SetValue(NumberEditor.StringValueFilterProperty, (object) value);
      }
    }

    public DataTemplate NamedValuePickerTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(NumberEditor.NamedValuePickerTemplateProperty);
      }
      set
      {
        this.SetValue(NumberEditor.NamedValuePickerTemplateProperty, (object) value);
      }
    }

    public Thickness HighlightMargin
    {
      get
      {
        return (Thickness) this.GetValue(NumberEditor.HighlightMarginProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HighlightMarginProperty, (object) value);
      }
    }

    public Brush HighlightBrush
    {
      get
      {
        return (Brush) this.GetValue(NumberEditor.HighlightBrushProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HighlightBrushProperty, (object) value);
      }
    }

    public System.Windows.CornerRadius HighlightCornerRadius
    {
      get
      {
        return (System.Windows.CornerRadius) this.GetValue(NumberEditor.HighlightCornerRadiusProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HighlightCornerRadiusProperty, (object) value);
      }
    }

    public double HighlightHeight
    {
      get
      {
        return (double) this.GetValue(NumberEditor.HighlightHeightProperty);
      }
      set
      {
        this.SetValue(NumberEditor.HighlightHeightProperty, (object) value);
      }
    }

    public Thickness NumberAreaMargin
    {
      get
      {
        return (Thickness) this.GetValue(NumberEditor.NumberAreaMarginProperty);
      }
      set
      {
        this.SetValue(NumberEditor.NumberAreaMarginProperty, (object) value);
      }
    }

    public double BorderWidth
    {
      get
      {
        return (double) this.GetValue(NumberEditor.BorderWidthProperty);
      }
      set
      {
        this.SetValue(NumberEditor.BorderWidthProperty, (object) value);
      }
    }

    public Brush SliderBrush
    {
      get
      {
        return (Brush) this.GetValue(NumberEditor.SliderBrushProperty);
      }
      set
      {
        this.SetValue(NumberEditor.SliderBrushProperty, (object) value);
      }
    }

    public System.Windows.CornerRadius SliderCornerRadius
    {
      get
      {
        return (System.Windows.CornerRadius) this.GetValue(NumberEditor.SliderCornerRadiusProperty);
      }
      set
      {
        this.SetValue(NumberEditor.SliderCornerRadiusProperty, (object) value);
      }
    }

    public double CornerRadius
    {
      get
      {
        return (double) this.GetValue(NumberEditor.CornerRadiusProperty);
      }
      set
      {
        this.SetValue(NumberEditor.CornerRadiusProperty, (object) value);
      }
    }

    public Thickness SliderBorder
    {
      get
      {
        return (Thickness) this.GetValue(NumberEditor.SliderBorderProperty);
      }
      set
      {
        this.SetValue(NumberEditor.SliderBorderProperty, (object) value);
      }
    }

    public ICommand BeginEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberEditor.BeginEditCommandProperty);
      }
      set
      {
        this.SetValue(NumberEditor.BeginEditCommandProperty, (object) value);
      }
    }

    public ICommand UpdateEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberEditor.UpdateEditCommandProperty);
      }
      set
      {
        this.SetValue(NumberEditor.UpdateEditCommandProperty, (object) value);
      }
    }

    public ICommand CancelEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberEditor.CancelEditCommandProperty);
      }
      set
      {
        this.SetValue(NumberEditor.CancelEditCommandProperty, (object) value);
      }
    }

    public ICommand CommitEditCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberEditor.CommitEditCommandProperty);
      }
      set
      {
        this.SetValue(NumberEditor.CommitEditCommandProperty, (object) value);
      }
    }

    public ICommand FinishEditingCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberEditor.FinishEditingCommandProperty);
      }
      set
      {
        this.SetValue(NumberEditor.FinishEditingCommandProperty, (object) value);
      }
    }

    public ICommand EndEditCommand
    {
      get
      {
        return this.FinishEditingCommand;
      }
      set
      {
        this.FinishEditingCommand = value;
      }
    }

    public ICommand BeginTextEditCommand
    {
      get
      {
        return (ICommand) this.beginTextEditCommand;
      }
    }

    public ICommand CancelTextEditCommand
    {
      get
      {
        return (ICommand) this.cancelTextEditCommand;
      }
    }

    public ICommand CommitTextEditCommand
    {
      get
      {
        return (ICommand) this.commitTextEditCommand;
      }
    }

    public ICommand UpdateTextEditCommand
    {
      get
      {
        return (ICommand) this.updateTextEditCommand;
      }
    }

    public ICommand TextLostFocusCommand
    {
      get
      {
        return (ICommand) this.textLostFocusCommand;
      }
    }

    public ICommand TextChangedCommand
    {
      get
      {
        return (ICommand) this.textChangedCommand;
      }
    }

    public ICommand SetAutoCommand
    {
      get
      {
        return (ICommand) this.setAutoCommand;
      }
    }

    protected virtual ITypeDescriptorContext TypeDescriptorContext
    {
      get
      {
        return (ITypeDescriptorContext) new ConverterContext((object) this);
      }
    }

    private bool HasRange
    {
      get
      {
        if (this.Minimum > double.MinValue && this.Maximum < double.MaxValue)
          return this.Minimum < this.Maximum;
        return false;
      }
    }

    private bool HasSoftMin
    {
      get
      {
        return this.Minimum > double.MinValue;
      }
    }

    private bool HasSoftMax
    {
      get
      {
        return this.Maximum < double.MaxValue;
      }
    }

    private bool ShouldWarp
    {
      get
      {
        if (NumberEditor.MouseWarpEnabled && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.None && !SystemParameters.IsRemoteSession)
          return !this.isStylusDown;
        return false;
      }
    }

    private bool IsDragging
    {
      get
      {
        return this.isDragging;
      }
      set
      {
        if (this.isDragging == value)
          return;
        this.isDragging = value;
        if (this.DragNotifier == null)
          return;
        if (value)
          this.DragNotifier.BeginDragEdit();
        else
          this.DragNotifier.EndDragEdit();
      }
    }

    private double ValueOrAutoValue
    {
      get
      {
        if (!double.IsNaN(this.Value))
          return this.Value;
        return this.AutoValue;
      }
    }

    static NumberEditor()
    {
      EventManager.RegisterClassHandler(typeof (NumberEditor), Mouse.QueryCursorEvent, (Delegate) new QueryCursorEventHandler(NumberEditor.OnQueryCursor), true);
    }

    public NumberEditor()
    {
      this.beginTextEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnBeginTextEdit));
      this.cancelTextEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCancelTextEdit));
      this.commitTextEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCommitTextEdit));
      this.updateTextEditCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnUpdateTextEdit));
      this.textLostFocusCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnTextLostFocus));
      this.textChangedCommand = new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OnTextChanged));
      this.setAutoCommand = new ArgumentDelegateCommand(new ArgumentDelegateCommand.ArgumentEventHandler(this.OnSetAuto));
      this.CoerceValue(NumberEditor.SmallChangeProperty);
      this.CoerceValue(NumberEditor.LargeChangeProperty);
      this.CoerceValue(NumberEditor.MaxPrecisionProperty);
      this.CoerceValue(NumberEditor.HardMinimumProperty);
      this.CoerceValue(NumberEditor.HardMaximumProperty);
    }

    private static void This_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumberEditor) d).OnValueChanged((double) e.OldValue, (double) e.NewValue);
    }

    protected virtual void OnValueChanged(double oldValue, double newValue)
    {
      this.UpdateValueAsStringFromValue();
      this.CoerceValue(NumberEditor.FormattedValueProperty);
    }

    private static object This_CoerceFormattedValue(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null)
        return (object) numberEditor.FormatValue();
      return value;
    }

    private static object This_CoerceHardMinimum(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null && double.IsNaN((double) value))
        return (object) numberEditor.Minimum;
      return value;
    }

    private static void This_HardMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.ShowSliderProperty);
    }

    private static void This_MinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.HardMinimumProperty);
      numberEditor.CoerceValue(NumberEditor.ShowSliderProperty);
    }

    private static void This_MaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.HardMaximumProperty);
      numberEditor.CoerceValue(NumberEditor.ShowSliderProperty);
    }

    private static void This_HardMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.ShowSliderProperty);
    }

    private static object This_CoerceHardMaximum(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null && double.IsNaN((double) value))
        return (object) numberEditor.Maximum;
      return value;
    }

    private static void This_DefaultChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.SmallChangeProperty);
      numberEditor.CoerceValue(NumberEditor.LargeChangeProperty);
    }

    private static void This_SmallChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.MaxPrecisionProperty);
    }

    private static object This_CoerceSmallChange(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null && double.IsNaN((double) value))
        return (object) (numberEditor.DefaultChange * 0.1);
      return value;
    }

    private static object This_CoerceLargeChange(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null && double.IsNaN((double) value))
        return (object) (numberEditor.DefaultChange * 10.0);
      return value;
    }

    private static void This_FormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.FormattedValueProperty);
    }

    private static void This_IsNinchedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.FormattedValueProperty);
    }

    private static void This_ConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.FormattedValueProperty);
      numberEditor.UpdateValueAsStringFromValue();
    }

    private static void This_ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = d as NumberEditor;
      if (numberEditor == null)
        return;
      numberEditor.CoerceValue(NumberEditor.MaxPrecisionProperty);
      numberEditor.CoerceValue(NumberEditor.FormattedValueProperty);
      numberEditor.UpdateValueAsStringFromValue();
    }

    private static object This_CoerceMaxPrecision(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null)
      {
        int num1 = (int) value;
        if (num1 == int.MaxValue)
        {
          int num2 = 0;
          double d1 = numberEditor.SmallChange * numberEditor.Scale;
          double d2;
          for (double num3 = d1 - Math.Floor(d1); !Tolerances.AreClose(num3, 0.0) && num2 < NumberEditor.maxPrecisionInDouble; num3 = d2 - Math.Floor(d2))
          {
            ++num2;
            d2 = num3 * 10.0;
          }
          return (object) num2;
        }
        if (num1 > NumberEditor.maxPrecisionInDouble)
          return (object) NumberEditor.maxPrecisionInDouble;
      }
      return value;
    }

    private static object This_CoerceShowSlider(DependencyObject target, object value)
    {
      NumberEditor numberEditor = target as NumberEditor;
      if (numberEditor != null && !(value as bool?).HasValue)
        return (object) (bool) (numberEditor.HasRange ? true : false);
      return value;
    }

    private static void This_OnDragNotifierChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      NumberEditor numberEditor = sender as NumberEditor;
    }

    private void OnBeginTextEdit()
    {
    }

    private void OnCancelTextEdit()
    {
      ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
      this.hasCancelledTextEdit = true;
      this.shouldCancel = false;
    }

    private void OnCommitTextEdit()
    {
      if (this.hasCommittedTextEdit || this.hasCancelledTextEdit)
        return;
      double? nullable = this.ParseDoubleFromValueAsString();
      bool flag = nullable.HasValue && (object.Equals((object) nullable.Value, (object) this.initialValue) || double.IsNaN(this.initialValue) && object.Equals((object) nullable.Value, (object) this.AutoValue));
      if (nullable.HasValue && (!flag || this.hasChangedText))
      {
        this.Value = nullable.Value;
        ValueEditorUtils.ExecuteCommand(this.CommitEditCommand, (IInputElement) this, (object) null);
        this.hasCommittedTextEdit = true;
      }
      else
      {
        this.hasCancelledTextEdit = true;
        ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
      }
      ValueEditorUtils.UpdateBinding((FrameworkElement) this, NumberEditor.ValueProperty, UpdateBindingType.Target);
      this.UpdateValueAsStringFromValue();
    }

    private void OnUpdateTextEdit()
    {
      this.OnCommitTextEdit();
      this.BeginTextEdit();
    }

    private void OnTextLostFocus()
    {
      this.IsTextEditing = false;
      this.RemoveMouseWheelHandler();
      if (this.shouldCancel)
        this.OnCancelTextEdit();
      else
        this.OnCommitTextEdit();
      this.OnFinishEditing();
    }

    private void OnTextChanged(object parameter)
    {
      string str = parameter as string;
      if (!this.IsTextEditing || str == null)
        return;
      this.hasChangedText = true;
      this.StringValue = str;
      double? nullable = this.ParseDoubleFromValueAsString();
      if (!nullable.HasValue)
        return;
      if (double.IsNaN(this.valueChangeBase))
      {
        this.valueChangeBase = nullable.Value;
        this.valueChangeOffset = 0.0;
      }
      else
        this.valueChangeOffset = nullable.Value - this.valueChangeBase;
    }

    private void OnSetAuto(object parameter)
    {
      if (!this.IsTextEditing)
      {
        this.PrepareForTextEdit();
        ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
      }
      this.StringValue = parameter != null ? parameter.ToString() : "Auto";
      this.OnCommitTextEdit();
      if (!this.IsTextEditing)
        return;
      this.OnFinishEditing();
    }

    private void PrepareForTextEdit()
    {
      this.shouldCancel = false;
      this.valueChangeOffset = 0.0;
      this.initialValue = this.Value;
      this.valueChangeBase = this.ValueOrAutoValue;
      this.hasCancelledTextEdit = false;
      this.hasCommittedTextEdit = false;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.InvalidateCustomRenderers();
      return base.ArrangeOverride(finalSize);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      double borderWidth = this.BorderWidth;
      Brush borderBrush = this.BorderBrush;
      Pen stroke = (Pen) null;
      if (borderWidth > 0.0 && borderBrush != null)
        stroke = new Pen(borderBrush, borderWidth);
      Thickness numberAreaMargin = this.NumberAreaMargin;
      Rect outerBounds = new Rect(new Point(numberAreaMargin.Left, numberAreaMargin.Top), new Point(this.ActualWidth - numberAreaMargin.Right, this.ActualHeight - numberAreaMargin.Bottom));
      if (RenderUtils.DrawInscribedRoundedRect(drawingContext, this.Background, stroke, outerBounds, this.CornerRadius))
      {
        bool? showSlider = this.ShowSlider;
        if ((!showSlider.GetValueOrDefault() ? false : (showSlider.HasValue ? true : false)) != false)
        {
          Rect rect1 = RenderUtils.CalculateInnerRect(outerBounds, borderWidth);
          System.Windows.CornerRadius sliderCornerRadius = this.SliderCornerRadius;
          Thickness highlightMargin = this.HighlightMargin;
          Thickness sliderBorder = this.SliderBorder;
          double num = rect1.Width - (sliderBorder.Left + sliderBorder.Right);
          double height = rect1.Height - (sliderBorder.Top + sliderBorder.Bottom);
          double valueOrAutoValue = this.ValueOrAutoValue;
          double d = valueOrAutoValue > this.Maximum ? this.Maximum : (valueOrAutoValue < this.Minimum ? this.Minimum : valueOrAutoValue);
          double width = !this.IsSliderLogarithmic ? num * ((d - this.Minimum) / (this.Maximum - this.Minimum)) : num * ((Math.Log(d) - Math.Log(this.Minimum)) / (Math.Log(this.Maximum) - Math.Log(this.Minimum)));
          if (width > 0.0 && height > 0.0)
          {
            Rect rect2 = new Rect(rect1.Left + sliderBorder.Left, rect1.Top + sliderBorder.Top, width, height);
            if (Tolerances.IsUniform(sliderCornerRadius))
            {
              drawingContext.DrawRoundedRectangle(this.SliderBrush, (Pen) null, rect2, sliderCornerRadius.TopLeft, sliderCornerRadius.TopLeft);
            }
            else
            {
              if (this.sliderRenderer == null)
                this.sliderRenderer = new SuperRoundedRectRenderer();
              this.sliderRenderer.Render(drawingContext, rect2, this.SliderBrush, sliderCornerRadius);
            }
          }
          if (this.HighlightBrush != null)
          {
            Point point1 = new Point(rect1.Left + highlightMargin.Left, rect1.Top + highlightMargin.Top);
            Point point2 = new Point(rect1.Right - highlightMargin.Right, point1.Y + this.HighlightHeight);
            Rect renderRect = new Rect(point1, point2);
            if (this.highlightRenderer == null)
              this.highlightRenderer = new SuperRoundedRectRenderer();
            this.highlightRenderer.Render(drawingContext, renderRect, this.HighlightBrush, this.HighlightCornerRadius);
          }
        }
      }
      base.OnRender(drawingContext);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      double changeAmount = 0.0;
      switch (e.Key)
      {
        case Key.Escape:
          if (this.IsTextEditing)
          {
            this.shouldCancel = true;
            break;
          }
          break;
        case Key.Up:
          changeAmount = 1.0;
          break;
        case Key.Down:
          changeAmount = -1.0;
          break;
        default:
          changeAmount = 0.0;
          break;
      }
      if (changeAmount == 0.0)
        return;
      e.Handled = this.ApplyStringValueChange(changeAmount);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      if (e.LeftButton != MouseButtonState.Pressed)
        return;
      this.isMouseDown = true;
      this.dragStartPoint = e.GetPosition((IInputElement) this);
      this.Focus();
      this.CaptureMouse();
      e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.ignoreNextMove)
      {
        this.ignoreNextMove = false;
      }
      else
      {
        Point position = e.GetPosition((IInputElement) this);
        Vector vector = position - this.dragStartPoint;
        if (!this.isMouseDown)
          return;
        if (!this.IsDragging)
        {
          if (vector.Length > NumberEditor.dragTolerance)
          {
            this.IsDragging = true;
            e.Handled = true;
            ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
            this.dragStartPoint = position;
            this.lastDragPoint = this.dragStartPoint;
            this.initialValue = this.Value;
            this.valueChangeBase = this.ValueOrAutoValue;
            this.valueChangeOffset = 0.0;
            INumberEditorDragBehavior dragBehavior = this.DragBehavior;
            if (dragBehavior != null)
              dragBehavior.BeginDrag(this);
          }
        }
        else
        {
          Vector offset = position - this.lastDragPoint;
          INumberEditorDragBehavior dragBehavior = this.DragBehavior;
          double num = dragBehavior == null ? Math.Round(offset.Length) : dragBehavior.GetDragOffsetAmount(this, offset);
          if (num != 0.0)
          {
            bool flag = this.AdjustValue(offset.X > offset.Y ? num : -num);
            if (this.ShouldWarp)
            {
              MouseCursor.SetMousePos(this.dragStartPoint, (Visual) this);
              this.lastDragPoint = this.dragStartPoint;
            }
            else
              this.lastDragPoint = position;
            if (flag)
              ValueEditorUtils.ExecuteCommand(this.UpdateEditCommand, (IInputElement) this, (object) null);
          }
        }
        e.Handled = true;
      }
    }

    protected override void OnStylusButtonDown(StylusButtonEventArgs e)
    {
      this.isStylusDown = true;
      base.OnStylusButtonDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      if (this.IsDragging || this.isMouseDown)
        e.Handled = true;
      this.ReleaseMouseCapture();
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      if (e.Source == this)
      {
        this.isStylusDown = false;
        if (this.IsDragging)
        {
          INumberEditorDragBehavior dragBehavior = this.DragBehavior;
          if (dragBehavior != null)
            dragBehavior.EndDrag(this);
          this.ignoreNextMove = true;
          e.GetPosition((IInputElement) this);
          MouseCursor.SetMousePos(this.GetDesiredCursorPositionBasedOnValue(), (Visual) this);
          Mouse.Synchronize();
          this.OnFinishEditing();
          if (!object.Equals((object) this.ValueOrAutoValue, (object) this.initialValue))
            ValueEditorUtils.ExecuteCommand(this.CommitEditCommand, (IInputElement) this, (object) null);
          else
            ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
          ValueEditorUtils.UpdateBinding((FrameworkElement) this, NumberEditor.ValueProperty, UpdateBindingType.Target);
        }
        else if (this.isMouseDown)
          this.BeginTextEdit();
        this.IsDragging = false;
        this.isMouseDown = false;
      }
      base.OnLostMouseCapture(e);
    }

    private void HandleMouseWheel(MouseWheelEventArgs e)
    {
      this.ApplyStringValueChange((double) (e.Delta / 120));
      e.Handled = this.IsTextEditing;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      if (!e.Handled && e.OriginalSource == this && (!this.isMouseDown && !this.IsTextEditing))
        this.BeginTextEdit();
      base.OnGotFocus(e);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.CoerceValue(NumberEditor.FormattedValueProperty);
    }

    private static void OnQueryCursor(object sender, QueryCursorEventArgs e)
    {
      if (e.Handled)
        return;
      NumberEditor numberEditor = sender as NumberEditor;
      if (numberEditor == null)
        return;
      if (numberEditor.Maximum == double.MaxValue)
      {
        double valueOrAutoValue = numberEditor.ValueOrAutoValue;
        e.Cursor = double.IsNaN(valueOrAutoValue) || double.IsInfinity(valueOrAutoValue) ? Cursors.Arrow : numberEditor.Cursor;
        e.Handled = true;
      }
      if (!numberEditor.IsDragging || !numberEditor.ShouldWarp)
        return;
      e.Cursor = Cursors.None;
      e.Handled = true;
    }

    private void InvalidateCustomRenderers()
    {
      if (this.sliderRenderer != null)
        this.sliderRenderer.InvalidateGeometry();
      if (this.highlightRenderer == null)
        return;
      this.highlightRenderer.InvalidateGeometry();
    }

    private void AddMouseWheelHandler()
    {
      InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(this.Current_PreNotifyInput);
    }

    private void RemoveMouseWheelHandler()
    {
      InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.Current_PreNotifyInput);
    }

    private void Current_PreNotifyInput(object sender, NotifyInputEventArgs e)
    {
      MouseWheelEventArgs e1 = e.StagingItem.Input as MouseWheelEventArgs;
      if (e1 == null || !this.IsKeyboardFocusWithin)
        return;
      this.HandleMouseWheel(e1);
    }

    private void BeginTextEdit()
    {
      this.UpdateValueAsStringFromValue();
      this.PrepareForTextEdit();
      this.IsTextEditing = true;
      this.hasChangedText = false;
      this.AddMouseWheelHandler();
      ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
      this.OnBeginTextEdit();
    }

    private string FormatValue()
    {
      double d = this.ValueOrAutoValue * this.Scale;
      if (this.IsNinched)
        return StringTable.NumberEditorNinchedValue;
      if (double.IsInfinity(d) || double.IsNaN(d))
        return this.StringValue;
      string format = this.Format;
      string str = d.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);
      if (double.IsNaN(this.Value))
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.NumberEditorAutoFormat, new object[1]
        {
          (object) str
        });
      return str;
    }

    private double? ParseDoubleFromValueAsString()
    {
      double? nullable = new double?();
      TypeConverter typeConverter = this.Converter ?? NumberEditor.doubleConverter;
      string str = this.StringValue;
      IStringValueFilter stringValueFilter = this.StringValueFilter;
      if (stringValueFilter != null)
        str = stringValueFilter.FilterStringValue((object) this, str);
      try
      {
        double d = Convert.ToDouble(typeConverter.ConvertFromString(this.TypeDescriptorContext, CultureInfo.CurrentCulture, str), (IFormatProvider) CultureInfo.CurrentCulture);
        if (double.IsNaN(d))
        {
          if (!this.CanBeAuto)
            goto label_7;
        }
        nullable = new double?(d);
      }
      catch (FormatException ex)
      {
        return nullable;
      }
      catch (Exception ex)
      {
        return nullable;
      }
label_7:
      if (!nullable.HasValue)
        return nullable;
      return new double?(this.OptionallyEnforceMaxPrecision(this.EnforceHardLimits(nullable.Value / this.Scale)));
    }

    private void UpdateValueAsStringFromValue()
    {
      TypeConverter converter = this.Converter;
      string str = converter == null || !converter.CanConvertFrom(typeof (double)) ? (string) NumberEditor.doubleConverter.ConvertTo(this.TypeDescriptorContext, CultureInfo.CurrentCulture, (object) this.OptionallyEnforceMaxPrecision(this.ValueOrAutoValue * this.Scale), typeof (string)) : (string) converter.ConvertTo(this.TypeDescriptorContext, CultureInfo.CurrentCulture, converter.ConvertFrom(this.TypeDescriptorContext, CultureInfo.CurrentCulture, (object) this.OptionallyEnforceMaxPrecision(this.ValueOrAutoValue * this.Scale)), typeof (string));
      if (!(str != this.StringValue))
        return;
      this.StringValue = str;
    }

    private bool ApplyStringValueChange(double changeAmount)
    {
      if (!this.IsTextEditing || changeAmount == 0.0 || !this.AdjustStringValue(changeAmount))
        return false;
      ValueEditorUtils.ExecuteCommand(this.UpdateEditCommand, (IInputElement) this, (object) null);
      return true;
    }

    private bool AdjustValue(double baseAmount)
    {
      double valueOrAutoValue = this.ValueOrAutoValue;
      if (double.IsNaN(valueOrAutoValue) || double.IsInfinity(valueOrAutoValue))
        return false;
      this.Value = this.CalculateValueChange(baseAmount);
      return !object.Equals((object) this.ValueOrAutoValue, (object) valueOrAutoValue);
    }

    private bool AdjustStringValue(double baseAmount)
    {
      string stringValue = this.StringValue;
      double? nullable = this.ParseDoubleFromValueAsString();
      if (!nullable.HasValue || double.IsNaN(nullable.Value) || double.IsInfinity(nullable.Value))
        return false;
      this.Value = this.CalculateValueChange(baseAmount);
      this.UpdateValueAsStringFromValue();
      return !string.Equals(this.StringValue, stringValue);
    }

    private double CalculateValueChange(double baseAmount)
    {
      if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
        baseAmount *= this.SmallChange;
      else if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        baseAmount *= this.LargeChange;
      else
        baseAmount *= this.DefaultChange;
      this.valueChangeOffset += baseAmount;
      double d = this.valueChangeBase + this.valueChangeOffset;
      double minimum = this.Minimum;
      double maximum = this.Maximum;
      if (this.HasSoftMin && (d < minimum || double.IsNegativeInfinity(d)))
      {
        d = minimum;
        this.valueChangeOffset = minimum - this.valueChangeBase;
      }
      else if (this.HasSoftMax && (d > maximum || double.IsNaN(d) || double.IsPositiveInfinity(d)))
      {
        d = this.Maximum;
        this.valueChangeOffset = maximum - this.valueChangeBase;
      }
      return this.EnforceMaxPrecision(this.EnforceHardLimits(this.SnapToSmallChange(d)));
    }

    private Point GetDesiredCursorPositionBasedOnValue()
    {
      Point point = this.dragStartPoint;
      if (this.HasRange && !double.IsNaN(this.ValueOrAutoValue) && !double.IsInfinity(this.Value))
        point.X = this.ActualWidth * (this.ValueOrAutoValue - this.Minimum) / (this.Maximum - this.Minimum);
      point.Y = this.ActualHeight / 2.0;
      return point;
    }

    private double SnapToSmallChange(double value)
    {
      double smallChange = this.SmallChange;
      return Math.Round(value / smallChange) * smallChange;
    }

    private double OptionallyEnforceMaxPrecision(double value)
    {
      if (this.AlwaysEnforceMaxPrecision)
        return this.EnforceMaxPrecision(value);
      return ValueEditorUtils.RoundToDoublePrecision(value, 15);
    }

    private double EnforceMaxPrecision(double value)
    {
      return Math.Round(value, this.MaxPrecision);
    }

    private double EnforceHardLimits(double value)
    {
      return Math.Max(this.HardMinimum, Math.Min(this.HardMaximum, value));
    }

    private void OnFinishEditing()
    {
      if (this.FinishEditingCommand != null)
        ValueEditorUtils.ExecuteCommand(this.FinishEditingCommand, (IInputElement) this, (object) null);
      else
        Keyboard.Focus((IInputElement) null);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.NumberComboBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Controls
{
  public sealed class NumberComboBox : ComboBox
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (double), typeof (NumberComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(NumberComboBox.OnValueChanged), new CoerceValueCallback(NumberComboBox.OnCoerceValue)));
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof (double), typeof (NumberComboBox), new PropertyMetadata((object) double.NegativeInfinity));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof (double), typeof (NumberComboBox), new PropertyMetadata((object) double.PositiveInfinity));
    public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.Register("NumberFormat", typeof (string), typeof (NumberComboBox), new PropertyMetadata((object) "G0"));
    public static readonly DependencyProperty IsSliderLogarithmicProperty = DependencyProperty.Register("IsSliderLogarithmic", typeof (bool), typeof (NumberComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof (ICommand), typeof (NumberComboBox), new PropertyMetadata((PropertyChangedCallback) null));

    public double Value
    {
      get
      {
        return (double) this.GetValue(NumberComboBox.ValueProperty);
      }
      set
      {
        this.SetValue(NumberComboBox.ValueProperty, (object) value);
      }
    }

    public double Minimum
    {
      get
      {
        return (double) this.GetValue(NumberComboBox.MinimumProperty);
      }
      set
      {
        this.SetValue(NumberComboBox.MinimumProperty, (object) value);
      }
    }

    public double Maximum
    {
      get
      {
        return (double) this.GetValue(NumberComboBox.MaximumProperty);
      }
      set
      {
        this.SetValue(NumberComboBox.MaximumProperty, (object) value);
      }
    }

    public string NumberFormat
    {
      get
      {
        return (string) this.GetValue(NumberComboBox.NumberFormatProperty);
      }
      set
      {
        this.SetValue(NumberComboBox.NumberFormatProperty, (object) value);
      }
    }

    public bool IsSliderLogarithmic
    {
        get
        {
            return (bool)base.GetValue(NumberComboBox.IsSliderLogarithmicProperty);
        }
        set
        {
            base.SetValue(NumberComboBox.IsSliderLogarithmicProperty, value);
        }
    }

    public ICommand FinishEditingCommand
    {
      get
      {
        return (ICommand) this.GetValue(NumberComboBox.FinishEditingCommandProperty);
      }
      set
      {
        this.SetValue(NumberComboBox.FinishEditingCommandProperty, (object) value);
      }
    }

    public event DependencyPropertyChangedEventHandler ValueChanged;

    public NumberComboBox()
    {
      this.SetValue(InputMethod.IsInputMethodEnabledProperty, (object) false);
    }

    protected override void OnDropDownClosed(EventArgs e)
    {
      base.OnDropDownClosed(e);
      if (this.FinishEditingCommand == null)
        return;
      this.FinishEditingCommand.Execute((object) null);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count <= 0 || !(e.AddedItems[0] is double))
        return;
      this.Value = (double) e.AddedItems[0];
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      NumberComboBox numberComboBox = (NumberComboBox) d;
      if (numberComboBox.ValueChanged == null)
        return;
      numberComboBox.ValueChanged((object) numberComboBox, e);
    }

    private static object OnCoerceValue(DependencyObject d, object value)
    {
      if (!(value is double))
        return value;
      NumberComboBox numberComboBox = (NumberComboBox) d;
      return (object) Math.Max(numberComboBox.Minimum, Math.Min((double) value, numberComboBox.Maximum));
    }
  }
}

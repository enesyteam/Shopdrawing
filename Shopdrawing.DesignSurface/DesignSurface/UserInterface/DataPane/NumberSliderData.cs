// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.NumberSliderData
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Globalization;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class NumberSliderData : IConfigurationOptionData
  {
    private IPopupControlCallback popupCallback;
    private ConfigurationPlaceholder control;
    private double value;
    private double pendingValue;

    public string Label
    {
      get
      {
        return this.control.Label;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.control.AutomationId;
      }
    }

    public double Minimum
    {
      get
      {
        return (double) this.control.SliderRange.Minimum;
      }
    }

    public double Maximum
    {
      get
      {
        return (double) this.control.SliderRange.Maximum;
      }
    }

    public double SmallIncrement
    {
      get
      {
        return this.control.SliderRange.SmallIncrement;
      }
    }

    public double LargeIncrement
    {
      get
      {
        return this.control.SliderRange.LargeIncrement;
      }
    }

    public bool IsDraggingSlider { get; private set; }

    public ICommand SlideBeginCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.IsDraggingSlider = true));
      }
    }

    public ICommand SlideEndCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          this.IsDraggingSlider = false;
          this.Value = this.pendingValue;
        }));
      }
    }

    public double Value
    {
      get
      {
        return this.pendingValue;
      }
      set
      {
        this.pendingValue = value;
        if (this.IsDraggingSlider || this.value == this.pendingValue)
          return;
        this.value = this.pendingValue;
        this.popupCallback.SetValue(this.control, (object) this.pendingValue);
      }
    }

    public NumberSliderData(IPopupControlCallback popupCallback, ConfigurationPlaceholder control)
    {
      this.popupCallback = popupCallback;
      this.control = control;
      this.value = double.Parse(this.popupCallback.GetValue(control).ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      this.pendingValue = this.value;
    }
  }
}

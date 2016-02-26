// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ExtendedScrollBar
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class ExtendedScrollBar : ScrollBar
  {
    private double hardMinimum = double.NegativeInfinity;
    private double hardMaximum = double.PositiveInfinity;
    private bool haveInitialized;
    private double contentMinimum;
    private double contentMaximum;
    private bool isScrolling;

    public bool IsScrolling
    {
      get
      {
        return this.isScrolling;
      }
    }

    public new double Value
    {
      get
      {
        return base.Value;
      }
      set
      {
        value = Math.Max(this.HardMinimum, Math.Min(this.HardMaximum, value));
        if (this.Value == value)
          return;
        this.UpdateMinimum(value, false);
        this.UpdateMaximum(value, false);
        base.Value = value;
        this.UpdateMinimum(value, true);
        this.UpdateMaximum(value, true);
      }
    }

    public double HardMinimum
    {
      get
      {
        return this.hardMinimum;
      }
      set
      {
        if (this.hardMinimum == value)
          return;
        this.hardMinimum = value;
        if (this.ContentMinimum >= this.hardMinimum)
          return;
        this.ContentMinimum = this.hardMinimum;
      }
    }

    public double HardMaximum
    {
      get
      {
        return this.hardMaximum;
      }
      set
      {
        if (this.hardMaximum == value)
          return;
        this.hardMaximum = value;
        if (this.ContentMaximum <= this.hardMaximum)
          return;
        this.ContentMaximum = this.hardMaximum;
      }
    }

    public double ContentMinimum
    {
      get
      {
        return this.contentMinimum;
      }
      set
      {
        if (this.contentMinimum == value || double.IsNaN(value))
          return;
        this.contentMinimum = value;
        this.UpdateMinimum(this.Value);
      }
    }

    public double ContentMaximum
    {
      get
      {
        return this.contentMaximum;
      }
      set
      {
        if (this.contentMaximum == value || double.IsNaN(value))
          return;
        this.contentMaximum = value;
        this.UpdateMaximum(this.Value);
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.Loaded += new RoutedEventHandler(this.This_Loaded);
      this.ApplyTemplate();
    }

    public new bool ApplyTemplate()
    {
      bool flag = base.ApplyTemplate();
      if (!this.haveInitialized)
      {
        this.Initialize();
        this.haveInitialized = true;
      }
      return flag;
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
      if (!this.isScrolling)
      {
        this.UpdateMinimum(newValue);
        this.UpdateMaximum(newValue);
      }
      base.OnValueChanged(oldValue, newValue);
    }

    private void Initialize()
    {
      this.AddHandler(Mouse.GotMouseCaptureEvent, (Delegate) new MouseEventHandler(this.This_GotMouseCapture), true);
      this.AddHandler(Mouse.LostMouseCaptureEvent, (Delegate) new MouseEventHandler(this.This_LostMouseCapture), true);
    }

    private void ModifyRepeatButtons(Visual visual)
    {
      RepeatButton repeatButton = visual as RepeatButton;
      Track track = visual as Track;
      if (repeatButton != null)
      {
        if (repeatButton.Command == ScrollBar.LineLeftCommand || repeatButton.Command == ScrollBar.LineUpCommand)
        {
          repeatButton.Command = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ScrollUpRepeatButton_Click));
        }
        else
        {
          if (repeatButton.Command != ScrollBar.LineRightCommand && repeatButton.Command != ScrollBar.LineDownCommand)
            return;
          repeatButton.Command = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ScrollDownRepeatButton_Click));
        }
      }
      else if (track != null)
      {
        track.DecreaseRepeatButton.Command = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.PageUpRepeatButton_Click));
        track.IncreaseRepeatButton.Command = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.PageDownRepeatButton_Click));
      }
      else
      {
        for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) visual); ++childIndex)
        {
          Visual visual1 = VisualTreeHelper.GetChild((DependencyObject) visual, childIndex) as Visual;
          if (visual1 != null)
            this.ModifyRepeatButtons(visual1);
        }
      }
    }

    private void ScrollUpRepeatButton_Click()
    {
      this.Value -= this.SmallChange;
    }

    private void ScrollDownRepeatButton_Click()
    {
      this.Value += this.SmallChange;
    }

    private void PageUpRepeatButton_Click()
    {
      this.Value = Math.Max(this.Minimum, this.Value - this.ViewportSize);
    }

    private void PageDownRepeatButton_Click()
    {
      this.Value = Math.Min(this.Maximum, this.Value + this.ViewportSize);
    }

    private void This_GotMouseCapture(object sender, MouseEventArgs e)
    {
      this.isScrolling = true;
    }

    private void This_LostMouseCapture(object sender, MouseEventArgs e)
    {
      if (!this.isScrolling)
        return;
      this.isScrolling = false;
      this.UpdateMinimum(this.Value);
      this.UpdateMaximum(this.Value);
    }

    private void UpdateMinimum(double value)
    {
      this.UpdateMinimum(value, true);
    }

    private void UpdateMinimum(double value, bool forceSnap)
    {
      double num = Math.Min(value, this.ContentMinimum);
      if (this.Minimum == num || !forceSnap && this.Minimum <= num)
        return;
      this.Minimum = num;
    }

    private void UpdateMaximum(double value)
    {
      this.UpdateMaximum(value, true);
    }

    private void UpdateMaximum(double value, bool forceSnap)
    {
      double num = Math.Max(value, this.ContentMaximum);
      if (this.Maximum == num || !forceSnap && this.Maximum >= num)
        return;
      this.Maximum = num;
    }

    private void This_Loaded(object sender, EventArgs e)
    {
      this.ModifyRepeatButtons((Visual) this);
    }
  }
}

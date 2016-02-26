// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimePanel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimePanel : Panel
  {
    public static readonly DependencyProperty TimeProperty = DependencyProperty.RegisterAttached("Time", typeof (double), typeof (TimePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached("Duration", typeof (double), typeof (TimePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    public static readonly DependencyProperty UnitsPerSecondProperty = DependencyProperty.Register("UnitsPerSecond", typeof (double), typeof (TimePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
    public static readonly DependencyProperty IndentProperty = DependencyProperty.Register("Indent", typeof (double), typeof (TimePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

    public double UnitsPerSecond
    {
      get
      {
        return (double) this.GetValue(TimePanel.UnitsPerSecondProperty);
      }
      set
      {
        this.SetValue(TimePanel.UnitsPerSecondProperty, (object) value);
      }
    }

    public double Indent
    {
      get
      {
        return (double) this.GetValue(TimePanel.IndentProperty);
      }
      set
      {
        this.SetValue(TimePanel.IndentProperty, (object) value);
      }
    }

    public static double GetTime(DependencyObject target)
    {
      return (double) target.GetValue(TimePanel.TimeProperty);
    }

    public static void SetTime(DependencyObject target, double time)
    {
      target.SetValue(TimePanel.TimeProperty, (object) time);
    }

    public static double GetDuration(DependencyObject target)
    {
      return (double) target.GetValue(TimePanel.DurationProperty);
    }

    public static void SetDuration(DependencyObject target, double time)
    {
      target.SetValue(TimePanel.DurationProperty, (object) time);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      double unitsPerSecond = this.UnitsPerSecond;
      Size availableSize1 = new Size(double.PositiveInfinity, double.PositiveInfinity);
      foreach (UIElement uiElement in this.InternalChildren)
      {
        if (uiElement != null)
        {
          double duration = TimePanel.GetDuration((DependencyObject) uiElement);
          if (!double.IsNaN(duration))
          {
            availableSize1.Width = duration * unitsPerSecond;
            uiElement.Measure(availableSize1);
            availableSize1.Width = double.PositiveInfinity;
          }
          else
            uiElement.Measure(availableSize1);
        }
      }
      return new Size();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double unitsPerSecond = this.UnitsPerSecond;
      double indent = this.Indent;
      foreach (UIElement uiElement in this.InternalChildren)
      {
        double d = TimePanel.GetTime((DependencyObject) uiElement);
        if (double.IsNaN(d))
          d = 0.0;
        double x = unitsPerSecond * d + indent;
        Size desiredSize = uiElement.DesiredSize;
        double duration = TimePanel.GetDuration((DependencyObject) uiElement);
        if (!double.IsNaN(duration))
          desiredSize.Width = !double.IsInfinity(duration) ? duration * unitsPerSecond : finalSize.Width;
        uiElement.Arrange(new Rect(new Point(x, 0.0), desiredSize));
      }
      return finalSize;
    }
  }
}

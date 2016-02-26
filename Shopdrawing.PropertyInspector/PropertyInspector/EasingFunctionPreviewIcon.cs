// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingFunctionPreviewIcon
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class EasingFunctionPreviewIcon : Icon
  {
    private static double MaxExtraRange = 2.0;
    public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof (IEasingFunctionDefinition), typeof (EasingFunctionPreviewIcon), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty EasingProperty = DependencyProperty.Register("Easing", typeof (EasingMode), typeof (EasingFunctionPreviewIcon), (PropertyMetadata) new FrameworkPropertyMetadata((object) EasingMode.None, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof (Brush), typeof (EasingFunctionPreviewIcon), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public IEasingFunctionDefinition EasingFunction
    {
      get
      {
        return (IEasingFunctionDefinition) this.GetValue(EasingFunctionPreviewIcon.EasingFunctionProperty);
      }
      set
      {
        this.SetValue(EasingFunctionPreviewIcon.EasingFunctionProperty, value);
      }
    }

    public EasingMode Easing
    {
      get
      {
        return (EasingMode) this.GetValue(EasingFunctionPreviewIcon.EasingProperty);
      }
      set
      {
        this.SetValue(EasingFunctionPreviewIcon.EasingProperty, value);
      }
    }

    public Brush Brush
    {
      get
      {
        return (Brush) this.GetValue(EasingFunctionPreviewIcon.BrushProperty);
      }
      set
      {
        this.SetValue(EasingFunctionPreviewIcon.BrushProperty, value);
      }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      IEasingFunctionDefinition easingFunction = this.EasingFunction;
      EasingMode easing = this.Easing;
      Size size = new Size(this.ActualWidth, this.ActualHeight);
      Pen pen = new Pen(this.Brush, 1.0);
      Point point = new Point(5.0, 5.0);
      if (easingFunction != null)
      {
        EasingMode easingMode = EasingMode.None;
        if (easing != EasingMode.None)
        {
          easingMode = easingFunction.EasingMode;
          easingFunction.EasingMode = easing;
        }
        int capacity = (int) Math.Floor(size.Width);
        List<double> list = new List<double>(capacity);
        double val2_1 = 0.0;
        double val2_2 = 1.0;
        for (int index = 0; index < capacity; ++index)
        {
          try
          {
            double val1 = (size.Height - 1.0) * (1.0 - easingFunction.Ease(1.0 * (double) index / ((double) capacity - 1.0)));
            list.Add(val1);
            val2_1 = Math.Min(val1, val2_1);
            val2_2 = Math.Max(val1, val2_2);
          }
          catch (Exception ex)
          {
            return;
          }
        }
        if (val2_2 > this.ActualHeight - 1.0 + EasingFunctionPreviewIcon.MaxExtraRange || val2_1 < 0.0 - EasingFunctionPreviewIcon.MaxExtraRange)
        {
          double num1 = val2_2 - val2_1;
          double num2 = (size.Height + EasingFunctionPreviewIcon.MaxExtraRange) / num1;
          double num3 = val2_1 + point.Y / 2.0;
          for (int index = 0; index < capacity; ++index)
            list[index] = (list[index] - num3) * num2;
        }
        Point point0 = new Point(point.X, point.Y + list[0]);
        for (int index = 1; index < capacity; ++index)
        {
          Point point1 = new Point(point.X + (double) index, point.Y + list[index]);
          drawingContext.DrawLine(pen, point0, point1);
          point0 = point1;
        }
        easingFunction.EasingMode = easingMode;
      }
      else
        drawingContext.DrawLine(pen, new Point(point.X, point.Y + size.Height - 1.0), new Point(point.X + size.Width - 1.0, point.Y));
    }
  }
}

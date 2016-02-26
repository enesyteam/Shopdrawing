// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ControlStylingUtilities
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public static class ControlStylingUtilities
  {
    public static readonly DependencyProperty CustomAccentBrushProperty = DependencyProperty.RegisterAttached("CustomAccentBrush", typeof (Brush), typeof (ControlStylingUtilities));
    public static readonly DependencyProperty CustomCornerRadiusProperty = DependencyProperty.RegisterAttached("CustomCornerRadius", typeof (CornerRadius), typeof (ControlStylingUtilities));

    public static Brush GetCustomAccentBrush(DependencyObject target)
    {
      return (Brush) target.GetValue(ControlStylingUtilities.CustomAccentBrushProperty);
    }

    public static void SetCustomAccentBrush(DependencyObject target, Brush value)
    {
      target.SetValue(ControlStylingUtilities.CustomAccentBrushProperty, (object) value);
    }

    public static CornerRadius GetCustomCornerRadius(DependencyObject target)
    {
      return (CornerRadius) target.GetValue(ControlStylingUtilities.CustomCornerRadiusProperty);
    }

    public static void SetCustomCornerRadius(DependencyObject target, CornerRadius value)
    {
      target.SetValue(ControlStylingUtilities.CustomCornerRadiusProperty, (object) value);
    }
  }
}

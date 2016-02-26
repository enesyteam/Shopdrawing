// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.DependencyObject.DependencyObjectExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Extensions.DependencyObject
{
  public static class DependencyObjectExtensions
  {
      public static Point GetCanvasPos(this System.Windows.DependencyObject obj)
    {
      return new Point((double) obj.GetValue(Canvas.LeftProperty), (double) obj.GetValue(Canvas.TopProperty));
    }

      public static void SetCanvasPos(this System.Windows.DependencyObject obj, Point point)
    {
      obj.SetValue(Canvas.LeftProperty, (object) point.X);
      obj.SetValue(Canvas.TopProperty, (object) point.Y);
    }

      public static IEnumerable<System.Windows.DependencyObject> VisualChildren(this System.Windows.DependencyObject obj)
    {
      int numChildren = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < numChildren; ++i)
        yield return VisualTreeHelper.GetChild(obj, i);
    }
  }
}

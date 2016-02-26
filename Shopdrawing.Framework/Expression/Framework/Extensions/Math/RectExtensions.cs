// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.Math.RectExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.Framework.Extensions.Math
{
  public static class RectExtensions
  {
    public static Point GetCenter(this Rect rect)
    {
      return rect.Location + (Vector) rect.Size / 2.0;
    }

    public static double GetCenterX(this Rect rect)
    {
      return rect.Left + rect.Width / 2.0;
    }

    public static double GetCenterY(this Rect rect)
    {
      return rect.Top + rect.Height / 2.0;
    }

    public static double GetProportionalScale(this Rect rect, Size maxSize)
    {
        return System.Linq.Enumerable.Min((IEnumerable<double>)new double[3]
      {
        1.0,
        maxSize.Width / rect.Width,
        maxSize.Height / rect.Height
      });
    }

    public static Rect Union(this IEnumerable<Rect> rects)
    {
        return System.Linq.Enumerable.Aggregate<Rect>(rects, new Func<Rect, Rect, Rect>(Rect.Union));
    }
  }
}

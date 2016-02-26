// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.RenderUtils
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  internal sealed class RenderUtils
  {
    private RenderUtils()
    {
    }

    public static bool DrawInscribedRoundedRect(DrawingContext drawingContext, Brush fill, Pen stroke, Rect outerBounds, double cornerRadius)
    {
      Point point1 = new Point(outerBounds.Left, outerBounds.Top);
      Point point2 = new Point(outerBounds.Right, outerBounds.Bottom);
      bool flag = false;
      if (stroke != null && !Tolerances.NearZero(stroke.Thickness))
      {
        double num = stroke.Thickness / 2.0;
        point1.X += num;
        point1.Y += num;
        point2.X -= num;
        point2.Y -= num;
      }
      Rect rectangle = new Rect(point1, point2);
      if (!Tolerances.NearZero(rectangle.Width) && !Tolerances.NearZero(rectangle.Height))
      {
        drawingContext.DrawRoundedRectangle(fill, stroke, rectangle, cornerRadius, cornerRadius);
        flag = true;
      }
      return flag;
    }

    public static Rect CalculateInnerRect(Rect outerBounds, double strokeThickness)
    {
      if (!Tolerances.NearZero(strokeThickness))
        return new Rect(new Point(outerBounds.Left + strokeThickness, outerBounds.Top + strokeThickness), new Point(outerBounds.Right - strokeThickness, outerBounds.Bottom - strokeThickness));
      return outerBounds;
    }
  }
}

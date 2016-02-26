// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PrototypingCompositionBadgeAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class PrototypingCompositionBadgeAdorner : Adorner
  {
    private static readonly DrawingImage Badge = FileTable.GetDrawingImage("Resources\\Adorners\\PrototypingCompositionBadgeAdorner.xaml");
    private static readonly Rect BadgeRect;

    static PrototypingCompositionBadgeAdorner()
    {
      PrototypingCompositionBadgeAdorner.Badge.Freeze();
      Rect bounds = PrototypingCompositionBadgeAdorner.Badge.Drawing.Bounds;
      PrototypingCompositionBadgeAdorner.BadgeRect = new Rect()
      {
        X = -bounds.Width / 2.0 - 1.0,
        Y = -(bounds.Height + 4.0),
        Width = bounds.Width,
        Height = bounds.Height
      };
    }

    public PrototypingCompositionBadgeAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      Rect elementBounds = this.ElementBounds;
      if (elementBounds.IsEmpty)
        return;
      Point point = Point.Multiply(elementBounds.TopLeft, matrix);
      Rect rectangle = Rect.Offset(PrototypingCompositionBadgeAdorner.BadgeRect, (Vector) point);
      context.DrawImage((ImageSource) PrototypingCompositionBadgeAdorner.Badge, rectangle);
    }
  }
}

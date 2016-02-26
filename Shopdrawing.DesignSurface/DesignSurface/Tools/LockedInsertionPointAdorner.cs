// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.LockedInsertionPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class LockedInsertionPointAdorner : BoundingBoxAdorner
  {
    private static readonly SolidColorBrush BoundingBoxBrush = new SolidColorBrush(Color.FromScRgb(0.5f, 1f, 1f, 0.0f));
    private static readonly Pen BoundingBoxPen;

    public override Pen BorderPen
    {
      get
      {
        return LockedInsertionPointAdorner.BoundingBoxPen;
      }
    }

    static LockedInsertionPointAdorner()
    {
      LockedInsertionPointAdorner.BoundingBoxBrush.Freeze();
      LockedInsertionPointAdorner.BoundingBoxPen = new Pen((Brush) LockedInsertionPointAdorner.BoundingBoxBrush, 8.0);
      LockedInsertionPointAdorner.BoundingBoxPen.Freeze();
    }

    public LockedInsertionPointAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }
  }
}

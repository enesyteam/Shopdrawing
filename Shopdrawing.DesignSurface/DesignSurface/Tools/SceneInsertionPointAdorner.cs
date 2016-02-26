// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SceneInsertionPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class SceneInsertionPointAdorner : BoundingBoxAdorner
  {
    private static readonly SolidColorBrush BoundingBoxBrush = new SolidColorBrush(Color.FromArgb((byte) sbyte.MinValue, Brushes.CornflowerBlue.Color.R, Brushes.CornflowerBlue.Color.G, Brushes.CornflowerBlue.Color.B));
    public const double InsertionPointAdornerWidth = 8.0;
    private static readonly Pen BoundingBoxPen;

    public override Pen BorderPen
    {
      get
      {
        return SceneInsertionPointAdorner.BoundingBoxPen;
      }
    }

    static SceneInsertionPointAdorner()
    {
      SceneInsertionPointAdorner.BoundingBoxBrush.Freeze();
      SceneInsertionPointAdorner.BoundingBoxPen = new Pen((Brush) SceneInsertionPointAdorner.BoundingBoxBrush, 8.0);
      SceneInsertionPointAdorner.BoundingBoxPen.Freeze();
    }

    public SceneInsertionPointAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }
  }
}

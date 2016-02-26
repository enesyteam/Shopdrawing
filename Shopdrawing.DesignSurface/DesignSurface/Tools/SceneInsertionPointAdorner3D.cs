// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SceneInsertionPointAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class SceneInsertionPointAdorner3D : Adorner
  {
    private static Pen DefaultPen = new Pen((Brush) new SolidColorBrush(Color.FromScRgb(0.5f, 1f, 1f, 0.0f)), 8.0);

    static SceneInsertionPointAdorner3D()
    {
      SceneInsertionPointAdorner3D.DefaultPen.StartLineCap = PenLineCap.Round;
      SceneInsertionPointAdorner3D.DefaultPen.EndLineCap = PenLineCap.Round;
      SceneInsertionPointAdorner3D.DefaultPen.LineJoin = PenLineJoin.Round;
      SceneInsertionPointAdorner3D.DefaultPen.Freeze();
    }

    public SceneInsertionPointAdorner3D(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      Base3DElement element = this.Element as Base3DElement;
      if (element == null)
        return;
      HighlightAdorner3D.DrawCube(drawingContext, matrix, element, SceneInsertionPointAdorner3D.DefaultPen);
    }
  }
}

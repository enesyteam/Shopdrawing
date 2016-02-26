// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public abstract class LayoutAdorner : Adorner
  {
    protected const double HeaderOffset = 8.0;
    protected const double HeaderThickness = 9.0;
    protected const double LockOffset = 27.0;
    protected const double SelectionOffset = 37.0;
    protected static readonly System.Windows.Media.Geometry TriangleGeometry;
    private bool isX;

    public bool IsX
    {
      get
      {
        return this.isX;
      }
    }

    static LayoutAdorner()
    {
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      streamGeometryContext.BeginFigure(new Point(0.0, 4.5), true, true);
      streamGeometryContext.LineTo(new Point(3.0, -3.0), true, false);
      streamGeometryContext.LineTo(new Point(-3.0, -3.0), true, false);
      streamGeometryContext.Close();
      LayoutAdorner.TriangleGeometry = (System.Windows.Media.Geometry)streamGeometry;
      LayoutAdorner.TriangleGeometry.Freeze();
    }

    protected LayoutAdorner(AdornerSet adornerSet, bool isX)
      : base(adornerSet)
    {
      this.isX = isX;
    }

    protected Matrix TruncateMatrix(Matrix matrix)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      return new CanonicalTransform()
      {
        Skew = canonicalTransform.Skew,
        RotationAngle = canonicalTransform.RotationAngle
      }.TransformGroup.Value;
    }
  }
}

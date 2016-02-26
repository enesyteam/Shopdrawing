// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushAnchorPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal abstract class BrushAnchorPointAdorner : BrushAdorner
  {
    private EdgeFlags edgeFlags;

    public EdgeFlags EdgeFlags
    {
      get
      {
        return this.edgeFlags;
      }
    }

    public Vector EdgeDirection
    {
      get
      {
        Matrix matrix = this.BrushTransform.Value * this.AdornerSet.Matrix;
        Vector vector1 = new Vector(1.0, 0.0) * matrix;
        Vector vector2 = new Vector(0.0, 1.0) * matrix;
        vector1.Normalize();
        vector2.Normalize();
        Vector vector3 = new Vector();
        if (this.TestFlags(EdgeFlags.Left))
          vector3 -= vector2;
        else if (this.TestFlags(EdgeFlags.Right))
          vector3 += vector2;
        if (this.TestFlags(EdgeFlags.Top))
          vector3 += vector1;
        else if (this.TestFlags(EdgeFlags.Bottom))
          vector3 -= vector1;
        return vector3;
      }
    }

    public Vector NormalDirection
    {
      get
      {
        Matrix matrix = this.BrushTransform.Value * this.AdornerSet.Matrix;
        Vector vector1 = new Vector(1.0, 0.0) * matrix;
        Vector vector2 = new Vector(0.0, 1.0) * matrix;
        vector1.Normalize();
        vector2.Normalize();
        Vector vector3 = new Vector();
        if (this.TestFlags(EdgeFlags.Left))
          vector3 -= vector1;
        else if (this.TestFlags(EdgeFlags.Right))
          vector3 += vector1;
        if (this.TestFlags(EdgeFlags.Top))
          vector3 -= vector2;
        else if (this.TestFlags(EdgeFlags.Bottom))
          vector3 += vector2;
        return vector3;
      }
    }

    protected BrushAnchorPointAdorner(BrushTransformAdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet)
    {
      this.edgeFlags = edgeFlags;
    }

    public bool TestFlags(EdgeFlags flagsToTest)
    {
      return (this.edgeFlags & flagsToTest) != EdgeFlags.None;
    }

    public Point GetAnchorPoint(Matrix matrix)
    {
      Rect brushBounds = this.BrushBounds;
      Point point = new Point((brushBounds.Left + brushBounds.Right) / 2.0, (brushBounds.Top + brushBounds.Bottom) / 2.0);
      if (this.TestFlags(EdgeFlags.Left))
        point.X = brushBounds.Left;
      else if (this.TestFlags(EdgeFlags.Right))
        point.X = brushBounds.Right;
      if (this.TestFlags(EdgeFlags.Top))
        point.Y = brushBounds.Top;
      else if (this.TestFlags(EdgeFlags.Bottom))
        point.Y = brushBounds.Bottom;
      return this.TransformPoint(point, true) * matrix;
    }

    protected Point GetOffsetAnchorPoint(Matrix matrix, double offset)
    {
      Matrix matrix1 = this.BrushTransform.Value;
      Vector vector1 = new Vector(1.0, 0.0) * matrix1 * matrix;
      Vector vector2 = new Vector(0.0, 1.0) * matrix1 * matrix;
      Vector vector3 = new Vector(vector2.Y, -vector2.X);
      Vector vector4 = new Vector(vector1.Y, -vector1.X);
      if (vector3.X * vector1.X + vector3.Y * vector1.Y < 0.0)
        vector3 *= -1.0;
      if (vector4.X * vector2.X + vector4.Y * vector2.Y < 0.0)
        vector4 *= -1.0;
      double length1 = vector3.Length;
      if (length1 > 0.0)
        vector3 /= length1;
      Vector vector5 = vector3 * offset;
      double length2 = vector4.Length;
      if (length2 > 0.0)
        vector4 /= length2;
      vector4 *= offset;
      Rect brushBounds = this.BrushBounds;
      Point point = new Point((brushBounds.Left + brushBounds.Right) / 2.0, (brushBounds.Top + brushBounds.Bottom) / 2.0);
      Vector vector6 = new Vector();
      if (this.TestFlags(EdgeFlags.Left))
      {
        point.X = brushBounds.Left;
        vector6 -= vector5;
      }
      else if (this.TestFlags(EdgeFlags.Right))
      {
        point.X = brushBounds.Right;
        vector6 += vector5;
      }
      if (this.TestFlags(EdgeFlags.Top))
      {
        point.Y = brushBounds.Top;
        vector6 -= vector4;
      }
      else if (this.TestFlags(EdgeFlags.Bottom))
      {
        point.Y = brushBounds.Bottom;
        vector6 += vector4;
      }
      return this.TransformPoint(point, true) * matrix + vector6;
    }
  }
}

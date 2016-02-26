// Decompiled with JetBrains decompiler
// Type: MS.Internal.Transforms.CanonicalDecomposition
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal.Transforms
{
  internal class CanonicalDecomposition : ICloneable
  {
    private static readonly double tolerance = 2.22044604925031E-16;
    private Point center;
    private Vector scale;
    private Vector skew;
    private double rotationAngle;
    private Vector translation;
    private Matrix value;
    private bool needsUpdate;

    public Point Center
    {
      get
      {
        return this.center;
      }
      set
      {
        if (!(this.center != value))
          return;
        this.center = value;
        this.needsUpdate = true;
      }
    }

    public Vector Scale
    {
      get
      {
        return this.scale;
      }
      set
      {
        if (!(this.scale != value))
          return;
        this.scale = value;
        this.needsUpdate = true;
      }
    }

    public double ScaleX
    {
      get
      {
        return this.scale.X;
      }
      set
      {
        if (this.scale.X == value)
          return;
        this.scale.X = value;
        this.needsUpdate = true;
      }
    }

    public double ScaleY
    {
      get
      {
        return this.scale.Y;
      }
      set
      {
        if (this.scale.Y == value)
          return;
        this.scale.Y = value;
        this.needsUpdate = true;
      }
    }

    public Vector Skew
    {
      get
      {
        return this.skew;
      }
      set
      {
        if (!(this.skew != value))
          return;
        this.skew = value;
        this.needsUpdate = true;
      }
    }

    public double SkewX
    {
      get
      {
        return this.skew.X;
      }
      set
      {
        if (this.skew.X == value)
          return;
        this.skew.X = value;
        this.needsUpdate = true;
      }
    }

    public double SkewY
    {
      get
      {
        return this.skew.Y;
      }
      set
      {
        if (this.skew.Y == value)
          return;
        this.skew.Y = value;
        this.needsUpdate = true;
      }
    }

    public double RotationAngle
    {
      get
      {
        return this.rotationAngle;
      }
      set
      {
        if (this.rotationAngle == value)
          return;
        this.rotationAngle = value;
        this.needsUpdate = true;
      }
    }

    public Vector Translation
    {
      get
      {
        return this.translation;
      }
      set
      {
        if (!(this.translation != value))
          return;
        this.translation = value;
        this.needsUpdate = true;
      }
    }

    public double TranslationX
    {
      get
      {
        return this.translation.X;
      }
      set
      {
        if (this.translation.X == value)
          return;
        this.translation.X = value;
        this.needsUpdate = true;
      }
    }

    public double TranslationY
    {
      get
      {
        return this.translation.Y;
      }
      set
      {
        if (this.translation.Y == value)
          return;
        this.translation.Y = value;
        this.needsUpdate = true;
      }
    }

    public Matrix Value
    {
      get
      {
        if (this.needsUpdate)
          this.UpdateValue();
        return this.value;
      }
      set
      {
        this.ConvertGenericTransform(value);
        this.value = value;
        this.needsUpdate = false;
      }
    }

    public CanonicalDecomposition()
    {
      this.center = new Point(0.0, 0.0);
      this.scale = new Vector(1.0, 1.0);
      this.skew = new Vector(0.0, 0.0);
      this.rotationAngle = 0.0;
      this.translation = new Vector(0.0, 0.0);
      this.value = Matrix.Identity;
      this.needsUpdate = false;
    }

    public CanonicalDecomposition(CanonicalDecomposition sourceDecomposition)
    {
      this.Center = sourceDecomposition.center;
      this.Scale = sourceDecomposition.scale;
      this.Skew = sourceDecomposition.skew;
      this.RotationAngle = sourceDecomposition.rotationAngle;
      this.Translation = sourceDecomposition.translation;
      this.UpdateValue();
    }

    public CanonicalDecomposition(Matrix sourceMatrix)
    {
      this.ConvertGenericTransform(sourceMatrix);
      this.value = sourceMatrix;
    }

    public static bool operator ==(CanonicalDecomposition cd1, CanonicalDecomposition cd2)
    {
      bool flag1 = cd1 == null;
      bool flag2 = cd2 == null;
      if (flag1 && flag2)
        return true;
      if (!flag1 && !flag2 && (cd1.center == cd2.center && cd1.scale == cd2.scale) && (cd1.skew == cd2.skew && cd1.rotationAngle == cd2.rotationAngle))
        return cd1.translation == cd2.translation;
      return false;
    }

    public static bool operator !=(CanonicalDecomposition cd1, CanonicalDecomposition cd2)
    {
      return !(cd1 == cd2);
    }

    public override string ToString()
    {
      return "center(" + (object) this.Center.X + ", " + (string) (object) this.Center.Y + ") scale(" + (string) (object) this.Scale.X + ", " + (string) (object) this.Scale.Y + ") skew(" + (string) (object) this.Skew.X + ", " + (string) (object) this.Skew.Y + ") rotate(" + (string) (object) this.RotationAngle + ") translate(" + (string) (object) this.Translation.X + ", " + (string) (object) this.Translation.Y + ")";
    }

    public override int GetHashCode()
    {
      return this.Center.GetHashCode() ^ this.Scale.GetHashCode() ^ this.Skew.GetHashCode() ^ this.RotationAngle.GetHashCode() ^ this.Translation.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this == obj as CanonicalDecomposition;
    }

    public void ApplyScale(Vector scale, Point origin, Point fixedPoint)
    {
      this.Translation += new Vector((1.0 - scale.X) * (fixedPoint.X - origin.X), (1.0 - scale.Y) * (fixedPoint.Y - origin.Y)) * this.Value;
      double num1 = Tolerances.NearZero(this.ScaleX) ? 0.001 : this.ScaleX;
      double num2 = Tolerances.NearZero(this.ScaleY) ? 0.001 : this.ScaleY;
      this.Scale = new Vector(num1 * scale.X, num2 * scale.Y);
    }

    public void ApplySkewScale(Vector basisX, Vector basisY, Point origin, Point fixedPoint, Vector newBasisX, Vector newBasisY)
    {
      Matrix matrix1 = this.Value;
      Matrix matrix2 = new Matrix(basisX.X, basisX.Y, basisY.X, basisY.Y, 0.0, 0.0);
      matrix2.Invert();
      Matrix matrix3 = new Matrix(newBasisX.X, newBasisX.Y, newBasisY.X, newBasisY.Y, 0.0, 0.0);
      Matrix matrix4 = matrix2 * matrix3;
      if (matrix4.M22 == 0.0 && matrix4.M21 != 0.0 || matrix4.M11 == 0.0 && matrix4.M12 != 0.0)
        throw new InvalidOperationException();
      double x = 0.0;
      double y = 0.0;
      if (matrix4.M22 != 0.0)
        x = matrix4.M21 / matrix4.M22;
      if (matrix4.M11 != 0.0)
        y = matrix4.M12 / matrix4.M11;
      this.ApplySkewScaleInternal(new Vector(x, y), new Vector(matrix4.M11, matrix4.M22));
      Vector vector = fixedPoint - origin;
      this.Translation += (vector - vector * matrix4) * matrix1;
    }

    public void ApplySkewScale(Vector appliedSkew, Vector appliedScale, Point origin, Point fixedPoint)
    {
      appliedSkew = new Vector(Math.Tan(appliedSkew.X * Math.PI / 180.0), Math.Tan(appliedSkew.Y * Math.PI / 180.0));
      this.ApplySkewScaleInternal(appliedSkew, appliedScale);
      Matrix matrix1 = this.Value;
      Matrix matrix2 = new Matrix(appliedScale.X, appliedSkew.X * appliedScale.Y, appliedSkew.Y * appliedScale.X, appliedScale.Y, 0.0, 0.0);
      Vector vector = fixedPoint - origin;
      this.Translation += (vector - vector * matrix2) * matrix1;
    }

    private void ApplySkewScaleInternal(Vector appliedSkew, Vector appliedScale)
    {
      Vector vector = new Vector(this.Skew.X, this.Skew.Y);
      vector.X = Math.Tan(vector.X * Math.PI / 180.0);
      vector.Y = Math.Tan(vector.Y * Math.PI / 180.0);
      Vector scale = this.Scale;
      if (scale.X == 0.0)
        appliedSkew.Y = 0.0;
      else
        appliedSkew.Y *= scale.Y / scale.X;
      if (scale.Y == 0.0)
        appliedSkew.X = 0.0;
      else
        appliedSkew.X *= scale.X / scale.Y;
      this.SkewX = Math.Atan2(appliedSkew.X + vector.X, 1.0 + appliedSkew.X * vector.Y) * 180.0 / Math.PI;
      this.SkewY = Math.Atan2(appliedSkew.Y + vector.Y, 1.0 + appliedSkew.Y * vector.X) * 180.0 / Math.PI;
      this.ScaleX *= appliedScale.X * (1.0 + appliedSkew.Y * vector.X);
      this.ScaleY *= appliedScale.Y * (1.0 + appliedSkew.X * vector.Y);
    }

    public void ApplyRotation(double angle, Point fixedPoint)
    {
      Point point1 = fixedPoint * this.Value;
      this.RotationAngle += angle;
      Point point2 = fixedPoint * this.Value;
      this.Translation += point1 - point2;
    }

    private void UpdateValue()
    {
      this.value = Matrix.Identity;
      this.value.ScaleAt(this.Scale.X, this.Scale.Y, this.Center.X, this.Center.Y);
      this.value.Translate(-this.Center.X, -this.Center.Y);
      this.value.Skew(this.Skew.X, this.Skew.Y);
      this.value.Translate(this.Center.X, this.Center.Y);
      this.value.RotateAt(this.RotationAngle, this.Center.X, this.Center.Y);
      this.value.Translate(this.Translation.X, this.Translation.Y);
      this.needsUpdate = false;
    }

    private void ConvertGenericTransform(Matrix transform)
    {
      this.Center = new Point(0.0, 0.0);
      this.Skew = new Vector(0.0, 0.0);
      this.RotationAngle = 0.0;
      if (transform.M21 == 0.0 && transform.M12 == 0.0)
      {
        this.Translation = new Vector(transform.OffsetX, transform.OffsetY);
        this.Scale = new Vector(transform.M11, transform.M22);
      }
      else
      {
        Point point1 = new Point(0.0, 0.0);
        Vector vector1 = new Vector(1.0, 0.0);
        Vector vector2 = new Vector(0.0, 1.0);
        vector1 *= transform;
        Vector vector3 = vector2 * transform;
        Point point2 = point1 * transform;
        this.Translation = new Vector(point2.X, point2.Y);
        this.Scale = new Vector(vector1.Length, vector3.Length);
        bool flag1 = vector1.LengthSquared > CanonicalDecomposition.tolerance;
        bool flag2 = vector3.LengthSquared > CanonicalDecomposition.tolerance;
        double num1 = 0.0;
        double num2 = 0.0;
        if (flag1)
          num1 = this.GetAngle(new Vector(1.0, 0.0), vector1);
        if (flag2)
          num2 = this.GetAngle(new Vector(0.0, 1.0), vector3);
        if (flag1 && flag2 && Math.Abs(Vector.CrossProduct(vector1, vector3)) <= CanonicalDecomposition.tolerance)
        {
          Vector v2 = Math.Abs(num1) <= Math.Abs(num2) ? vector1 : vector3;
          this.Skew = new Vector(45.0, 45.0);
          this.ScaleX *= 0.707106781186548;
          this.ScaleY *= 0.707106781186548;
          this.RotationAngle = this.GetAngle(new Vector(1.0, 1.0), v2);
          if (vector1 * v2 < 0.0)
            this.ScaleX = -this.ScaleX;
          if (vector3 * v2 >= 0.0)
            return;
          this.ScaleY = -this.ScaleY;
        }
        else
        {
          if (!flag1 && !flag2)
            return;
          if (!flag2 || flag1 && Math.Abs(num1) <= Math.Abs(num2))
          {
            Vector vector4 = vector1 / this.Scale.X;
            this.ScaleY = (vector3 - vector4 * (vector4.X * vector3.X + vector4.Y * vector3.Y)).Length;
            this.RotationAngle = num1;
            if (!flag2)
              return;
            double angle = this.GetAngle(vector1, vector3);
            if (angle < 0.0)
            {
              this.ScaleY = -this.ScaleY;
              this.Skew = new Vector(-angle - 90.0, 0.0);
            }
            else
              this.Skew = new Vector(90.0 - angle, 0.0);
          }
          else
          {
            Vector vector4 = vector3 / this.Scale.Y;
            this.ScaleX = (vector1 - vector4 * (vector4.X * vector1.X + vector4.Y * vector1.Y)).Length;
            this.RotationAngle = num2;
            if (!flag1)
              return;
            double angle = this.GetAngle(vector3, vector1);
            if (angle > 0.0)
            {
              this.ScaleX = -this.ScaleX;
              this.Skew = new Vector(0.0, angle - 90.0);
            }
            else
              this.Skew = new Vector(0.0, angle + 90.0);
          }
        }
      }
    }

    private double GetAngle(Vector v1, Vector v2)
    {
      return Math.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y) * 180.0 / Math.PI;
    }

    public object Clone()
    {
      return (object) new CanonicalDecomposition(this);
    }
  }
}

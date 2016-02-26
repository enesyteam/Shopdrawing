// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.AxisConstraint
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public sealed class AxisConstraint
  {
    private Vector[] axisVectors;

    public AxisConstraint()
      : this(8)
    {
    }

    public AxisConstraint(int numberOfAxisVectors)
    {
      this.axisVectors = new Vector[numberOfAxisVectors];
      for (int index = 0; index < numberOfAxisVectors; ++index)
        this.axisVectors[index] = new Vector(Math.Cos(2.0 * Math.PI * (double) index / (double) numberOfAxisVectors), Math.Sin(2.0 * Math.PI * (double) index / (double) numberOfAxisVectors));
    }

    public Vector GetConstrainedVector(Vector vector)
    {
      double num1 = 0.0;
      Vector vector1 = vector;
      if (this.axisVectors != null)
      {
        foreach (Vector vector2 in this.axisVectors)
        {
          double num2 = Vector.Multiply(vector, vector2);
          if (num2 > num1)
          {
            num1 = num2;
            vector1 = vector2 * num1;
          }
        }
      }
      return vector1;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Matrix3DOperations
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface
{
  public static class Matrix3DOperations
  {
    public static bool IsDiagonal(Matrix matrix, double tolerance)
    {
      return Math.Abs(matrix.M12) + Math.Abs(matrix.M21) < tolerance;
    }

    public static bool IsDiagonal(Matrix3D matrix, double tolerance)
    {
      return Math.Abs(matrix.M12) + Math.Abs(matrix.M13) + Math.Abs(matrix.M21) + Math.Abs(matrix.M23) + Math.Abs(matrix.M31) + Math.Abs(matrix.M32) < tolerance;
    }

    public static Matrix Transpose(Matrix source)
    {
      return new Matrix()
      {
        M11 = source.M11,
        M12 = source.M21,
        M21 = source.M12,
        M22 = source.M22,
        OffsetX = source.OffsetX,
        OffsetY = source.OffsetY
      };
    }

    public static Matrix3D Transpose(Matrix3D source)
    {
      return new Matrix3D()
      {
        M11 = source.M11,
        M12 = source.M21,
        M13 = source.M31,
        M14 = source.OffsetX,
        M21 = source.M12,
        M22 = source.M22,
        M23 = source.M32,
        M24 = source.OffsetY,
        M31 = source.M13,
        M32 = source.M23,
        M33 = source.M33,
        M34 = source.OffsetZ,
        OffsetX = source.M14,
        OffsetY = source.M24,
        OffsetZ = source.M34,
        M44 = source.M44
      };
    }

    public static Matrix CreateMatrix(double[,] source)
    {
      return new Matrix()
      {
        M11 = source[0, 0],
        M12 = source[0, 1],
        M21 = source[1, 0],
        M22 = source[1, 1],
        OffsetX = 0.0,
        OffsetY = 0.0
      };
    }

    public static Matrix3D CreateMatrix3D(double[,] source)
    {
      return new Matrix3D()
      {
        M11 = source[0, 0],
        M12 = source[0, 1],
        M13 = source[0, 2],
        M21 = source[1, 0],
        M22 = source[1, 1],
        M23 = source[1, 2],
        M31 = source[2, 0],
        M32 = source[2, 1],
        M33 = source[2, 2],
        OffsetX = 0.0,
        OffsetY = 0.0,
        OffsetZ = 0.0,
        M44 = 1.0
      };
    }

    public static Vector CreateVector(double[] source)
    {
      return new Vector(source[0], source[1]);
    }

    public static Vector3D CreateVector3D(double[] source)
    {
      return new Vector3D(source[0], source[1], source[2]);
    }

    public static double[,] CreateMatrix2x2(Matrix source)
    {
      double[,] numArray = new double[2, 2]
      {
        {
          source.M11,
          source.M12
        },
        {
          source.M21,
          0.0
        }
      };
      numArray[0, 1] = source.M22;
      return numArray;
    }

    public static double[,] CreateMatrix3x3(Matrix3D source)
    {
      return new double[3, 3]
      {
        {
          source.M11,
          source.M12,
          source.M13
        },
        {
          source.M21,
          source.M22,
          source.M23
        },
        {
          source.M31,
          source.M32,
          source.M33
        }
      };
    }

    public static bool DecomposeIntoRotationAndScale(Matrix source, out Matrix rotation, out Vector scale)
    {
      source.OffsetX = 0.0;
      source.OffsetY = 0.0;
      Matrix U;
      Vector S;
      Matrix VTranspose;
      bool flag = Matrix3DOperations.SVD(source, out U, out S, out VTranspose);
      Matrix matrix1 = Matrix3DOperations.Transpose(VTranspose);
      Matrix matrix2 = new Matrix()
      {
        M11 = S.X,
        M22 = S.Y
      };
      rotation = U * matrix1;
      Matrix matrix3 = rotation;
      matrix3.Invert();
      Matrix matrix4 = source * matrix3;
      Matrix matrix5 = matrix3 * source;
      double tolerance = 1E-13;
      scale = !Matrix3DOperations.IsDiagonal(matrix4, tolerance) ? new Vector(matrix5.M11, matrix5.M22) : new Vector(matrix4.M11, matrix4.M22);
      return flag;
    }

    public static bool DecomposeIntoRotationAndScale(Matrix3D source, out Matrix3D rotation, out Vector3D scale)
    {
      source.OffsetX = 0.0;
      source.OffsetY = 0.0;
      source.OffsetZ = 0.0;
      Matrix3D U;
      Vector3D S;
      Matrix3D VTranspose;
      bool flag = Matrix3DOperations.SVD(source, out U, out S, out VTranspose);
      Matrix3D matrix3D1 = Matrix3DOperations.Transpose(VTranspose);
      Matrix3D matrix3D2 = new Matrix3D()
      {
        M11 = S.X,
        M22 = S.Y,
        M33 = S.Z,
        M44 = 1.0
      };
      rotation = U * matrix3D1;
      Matrix3D matrix3D3 = rotation;
      matrix3D3.Invert();
      Matrix3D matrix = source * matrix3D3;
      Matrix3D matrix3D4 = matrix3D3 * source;
      double tolerance = 1E-13;
      scale = !Matrix3DOperations.IsDiagonal(matrix, tolerance) ? new Vector3D(matrix3D4.M11, matrix3D4.M22, matrix3D4.M33) : new Vector3D(matrix.M11, matrix.M22, matrix.M33);
      return flag;
    }

    public static bool SVD(Matrix3D original, out Matrix3D U, out Vector3D S, out Matrix3D VTranspose)
    {
      double[,] U1;
      double[] S1;
      double[,] V;
      bool flag = Matrix3DOperations.Svd(Matrix3DOperations.CreateMatrix3x3(original), out U1, out S1, out V);
      U = Matrix3DOperations.CreateMatrix3D(U1);
      VTranspose = Matrix3DOperations.CreateMatrix3D(V);
      S = Matrix3DOperations.CreateVector3D(S1);
      return flag;
    }

    public static bool SVD(Matrix original, out Matrix U, out Vector S, out Matrix VTranspose)
    {
      double[,] U1;
      double[] S1;
      double[,] V;
      bool flag = Matrix3DOperations.Svd(Matrix3DOperations.CreateMatrix2x2(original), out U1, out S1, out V);
      U = Matrix3DOperations.CreateMatrix(U1);
      VTranspose = Matrix3DOperations.CreateMatrix(V);
      S = Matrix3DOperations.CreateVector(S1);
      return flag;
    }

    internal static double Hypotenuse(double a, double b)
    {
      double num1 = Math.Abs(a);
      double num2 = Math.Abs(b);
      if (num1 > num2)
      {
        double num3 = num2 / num1;
        return num1 * Math.Sqrt(1.0 + num3 * num3);
      }
      if (num2 == 0.0)
        return 0.0;
      double num4 = num1 / num2;
      return num2 * Math.Sqrt(1.0 + num4 * num4);
    }

    internal static bool Svd(double[,] M, out double[,] U, out double[] S, out double[,] V)
    {
      double val1 = 0.0;
      double num1 = 0.0;
      double num2 = 0.0;
      int index1 = 0;
      int index2 = 0;
      int length = M.Length == 4 ? 2 : 3;
      double[] numArray = new double[length];
      U = new double[length, length];
      V = new double[length, length];
      S = new double[length];
      for (int index3 = 0; index3 < length; ++index3)
      {
        for (int index4 = 0; index4 < length; ++index4)
          U[index3, index4] = M[index3, index4];
        S[index3] = 0.0;
      }
      for (int index3 = 0; index3 < length; ++index3)
      {
        index1 = index3 + 1;
        numArray[index3] = num2 * num1;
        double num3;
        double num4 = num3 = 0.0;
        double d1 = num3;
        double num5 = num3;
        for (int index4 = index3; index4 < length; ++index4)
          num4 += Math.Abs(U[index4, index3]);
        if (num4 != 0.0)
        {
          for (int index4 = index3; index4 < length; ++index4)
          {
            U[index4, index3] /= num4;
            d1 += U[index4, index3] * U[index4, index3];
          }
          double num6 = U[index3, index3];
          num5 = num6 >= 0.0 ? -Math.Sqrt(d1) : Math.Sqrt(d1);
          double num7 = num6 * num5 - d1;
          U[index3, index3] = num6 - num5;
          for (int index4 = index1; index4 < length; ++index4)
          {
            double num8 = 0.0;
            for (int index5 = index3; index5 < length; ++index5)
              num8 += U[index5, index3] * U[index5, index4];
            double num9 = num8 / num7;
            for (int index5 = index3; index5 < length; ++index5)
              U[index5, index4] += num9 * U[index5, index3];
          }
          for (int index4 = index3; index4 < length; ++index4)
            U[index4, index3] *= num4;
        }
        S[index3] = num4 * num5;
        double num10;
        num2 = num10 = 0.0;
        double d2 = num10;
        num1 = num10;
        for (int index4 = index1; index4 < length; ++index4)
          num2 += Math.Abs(U[index3, index4]);
        if (num2 != 0.0)
        {
          for (int index4 = index1; index4 < length; ++index4)
          {
            U[index3, index4] /= num2;
            d2 += U[index3, index4] * U[index3, index4];
          }
          double num6 = U[index3, index1];
          num1 = num6 >= 0.0 ? -Math.Sqrt(d2) : Math.Sqrt(d2);
          double num7 = num6 * num1 - d2;
          U[index3, index1] = num6 - num1;
          for (int index4 = index1; index4 < length; ++index4)
            numArray[index4] = U[index3, index4] / num7;
          for (int index4 = index1; index4 < length; ++index4)
          {
            double num8 = 0.0;
            for (int index5 = index1; index5 < length; ++index5)
              num8 += U[index4, index5] * U[index3, index5];
            for (int index5 = index1; index5 < length; ++index5)
              U[index4, index5] += num8 * numArray[index5];
          }
          for (int index4 = index1; index4 < length; ++index4)
            U[index3, index4] *= num2;
        }
        val1 = Math.Max(val1, Math.Abs(S[index3]) + Math.Abs(numArray[index3]));
      }
      int index6 = length;
      while (--index6 >= 0)
      {
        if (num1 != 0.0)
        {
          for (int index3 = index1; index3 < length; ++index3)
            V[index3, index6] = U[index6, index3] / U[index6, index1] / num1;
          for (int index3 = index1; index3 < length; ++index3)
          {
            double num3 = 0.0;
            for (int index4 = index1; index4 < length; ++index4)
              num3 += U[index6, index4] * V[index4, index3];
            for (int index4 = index1; index4 < length; ++index4)
              V[index4, index3] += num3 * V[index4, index6];
          }
        }
        for (int index3 = index1; index3 < length; ++index3)
          V[index6, index3] = V[index3, index6] = 0.0;
        V[index6, index6] = 1.0;
        num1 = numArray[index6];
        index1 = index6;
      }
      int index7 = length;
      while (--index7 >= 0)
      {
        int num3 = index7 + 1;
        double num4 = S[index7];
        for (int index3 = num3; index3 < length; ++index3)
          U[index7, index3] = 0.0;
        if (num4 != 0.0)
        {
          double num5 = 1.0 / num4;
          for (int index3 = num3; index3 < length; ++index3)
          {
            double num6 = 0.0;
            for (int index4 = num3; index4 < length; ++index4)
              num6 += U[index4, index7] * U[index4, index3];
            double num7 = num6 / U[index7, index7] * num5;
            for (int index4 = index7; index4 < length; ++index4)
              U[index4, index3] += num7 * U[index4, index7];
          }
          for (int index3 = index7; index3 < length; ++index3)
            U[index3, index7] *= num5;
        }
        else
        {
          for (int index3 = index7; index3 < length; ++index3)
            U[index3, index7] = 0.0;
        }
        ++U[index7, index7];
      }
      int index8 = length;
      while (--index8 >= 0)
      {
        for (int index3 = 1; index3 <= 30; ++index3)
        {
          bool flag = true;
          int index4;
          for (index4 = index8; index4 >= 0; --index4)
          {
            index2 = index4 - 1;
            if (Math.Abs(numArray[index4]) + val1 == val1)
            {
              flag = false;
              break;
            }
            if (index2 < 0)
              return false;
            if (Math.Abs(S[index2]) + val1 == val1)
              break;
          }
          if (flag)
          {
            double num3 = 0.0;
            double num4 = 1.0;
            for (int index5 = index4; index5 <= index8; ++index5)
            {
              double a = num4 * numArray[index5];
              numArray[index5] *= num3;
              if (Math.Abs(a) + val1 != val1)
              {
                double b = S[index5];
                double num5 = Matrix3DOperations.Hypotenuse(a, b);
                S[index5] = num5;
                double num6 = 1.0 / num5;
                num3 = b * num6;
                num4 = -a * num6;
                for (int index9 = 0; index9 < length; ++index9)
                {
                  double num7 = U[index9, index2];
                  double num8 = U[index9, index5];
                  U[index9, index2] = num7 * num3 + num8 * num4;
                  U[index9, index5] = num8 * num3 - num7 * num4;
                }
              }
              else
                break;
            }
          }
          double num9 = S[index8];
          if (index4 == index8)
          {
            if (num9 < 0.0)
            {
              S[index8] = -num9;
              for (int index5 = 0; index5 < length; ++index5)
                V[index5, index8] = -V[index5, index8];
              break;
            }
            break;
          }
          if (index3 == 30)
            return false;
          double num10 = S[index4];
          index2 = index8 - 1;
          double num11 = S[index2];
          double num12 = numArray[index2];
          double num13 = numArray[index8];
          double a1 = ((num11 - num9) * (num11 + num9) + (num12 - num13) * (num12 + num13)) / (2.0 * num13 * num11);
          double num14 = Matrix3DOperations.Hypotenuse(a1, 1.0);
          double a2 = ((num10 - num9) * (num10 + num9) + num13 * (num11 / (a1 + (a1 >= 0.0 ? num14 : -num14)) - num13)) / num10;
          double num15;
          double num16 = num15 = 1.0;
          for (int index5 = index4; index5 <= index2; ++index5)
          {
            int index9 = index5 + 1;
            double num3 = numArray[index9];
            double num4 = S[index9];
            double b1 = num15 * num3;
            double num5 = num16 * num3;
            double num6 = Matrix3DOperations.Hypotenuse(a2, b1);
            numArray[index5] = num6;
            num16 = a2 / num6;
            num15 = b1 / num6;
            double a3 = num10 * num16 + num5 * num15;
            double num7 = num5 * num16 - num10 * num15;
            double b2 = num4 * num15;
            double num8 = num4 * num16;
            for (int index10 = 0; index10 < length; ++index10)
            {
              double num17 = V[index10, index5];
              double num18 = V[index10, index9];
              V[index10, index5] = num17 * num16 + num18 * num15;
              V[index10, index9] = num18 * num16 - num17 * num15;
            }
            double num19 = Matrix3DOperations.Hypotenuse(a3, b2);
            S[index5] = num19;
            if (num19 != 0.0)
            {
              double num17 = 1.0 / num19;
              num16 = a3 * num17;
              num15 = b2 * num17;
            }
            a2 = num16 * num7 + num15 * num8;
            num10 = num16 * num8 - num15 * num7;
            for (int index10 = 0; index10 < length; ++index10)
            {
              double num17 = U[index10, index5];
              double num18 = U[index10, index9];
              U[index10, index5] = num17 * num16 + num18 * num15;
              U[index10, index9] = num18 * num16 - num17 * num15;
            }
          }
          numArray[index4] = 0.0;
          numArray[index8] = a2;
          S[index8] = num10;
        }
      }
      return true;
    }

    public static double FrobeniusNorm(Matrix3D transform, out Matrix3D rotation)
    {
      double num1 = Math.Sqrt((transform.M11 * transform.M11 + transform.M21 * transform.M21 + transform.M31 * transform.M31 + transform.M12 * transform.M12 + transform.M22 * transform.M22 + transform.M32 * transform.M32 + transform.M13 * transform.M13 + transform.M23 * transform.M23 + transform.M33 * transform.M33) / 3.0);
      double num2 = num1 == 0.0 ? 0.0 : 1.0 / num1;
      rotation = new Matrix3D();
      rotation.M11 = num2 * transform.M11;
      rotation.M12 = num2 * transform.M12;
      rotation.M13 = num2 * transform.M13;
      rotation.M21 = num2 * transform.M21;
      rotation.M22 = num2 * transform.M22;
      rotation.M23 = num2 * transform.M23;
      rotation.M31 = num2 * transform.M31;
      rotation.M32 = num2 * transform.M32;
      rotation.M33 = num2 * transform.M33;
      rotation.M44 = 1.0;
      return num1;
    }
  }
}

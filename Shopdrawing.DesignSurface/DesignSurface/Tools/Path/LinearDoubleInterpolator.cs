// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.LinearDoubleInterpolator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public sealed class LinearDoubleInterpolator : DoubleInterpolator, ILinearInterpolator, IInterpolator
  {
    public double BeginValue { get; set; }

    public double EndValue { get; set; }

    object ILinearInterpolator.BeginValue
    {
      get
      {
        return (object) this.BeginValue;
      }
      set
      {
        this.BeginValue = (double) value;
      }
    }

    object ILinearInterpolator.EndValue
    {
      get
      {
        return (object) this.EndValue;
      }
      set
      {
        this.EndValue = (double) value;
      }
    }

    public LinearDoubleInterpolator(double beginValue, double endValue)
    {
      this.BeginValue = beginValue;
      this.EndValue = endValue;
    }

    public override double Interpolate(double parameter)
    {
      return this.BeginValue * (1.0 - parameter) + this.EndValue * parameter;
    }
  }
}

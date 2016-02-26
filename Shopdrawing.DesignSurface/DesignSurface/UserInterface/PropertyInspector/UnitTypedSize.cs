// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.UnitTypedSize
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class UnitTypedSize
  {
    private UnitType units;
    private double size;

    public UnitType Units
    {
      get
      {
        return this.units;
      }
      set
      {
        this.units = value;
      }
    }

    public double Size
    {
      get
      {
        return this.size;
      }
      set
      {
        this.size = value;
      }
    }

    private UnitTypedSize(double size, UnitType units)
    {
      this.size = size;
      this.units = units;
    }

    public static UnitTypedSize CreateFromPixels(double pixelValue, UnitType type)
    {
      UnitTypedSize unitTypedSize = new UnitTypedSize(pixelValue, UnitType.Pixels);
      if (type != UnitType.Pixels)
        unitTypedSize = unitTypedSize.ConvertTo(type);
      return unitTypedSize;
    }

    public static UnitTypedSize CreateFromUnits(double unitValue, UnitType type)
    {
      return new UnitTypedSize(unitValue, type);
    }

    public override bool Equals(object obj)
    {
      UnitTypedSize unitTypedSize = obj as UnitTypedSize;
      if (unitTypedSize != null && unitTypedSize.size == this.size)
        return unitTypedSize.units == this.units;
      return false;
    }

    public override int GetHashCode()
    {
      return this.size.GetHashCode() ^ this.units.GetHashCode();
    }

    public override string ToString()
    {
      if (double.IsNaN(this.size))
        return StringTable.Auto;
      if (this.units == UnitType.Points)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} pt", new object[1]
        {
          (object) Math.Round(this.size, 3)
        });
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} px", new object[1]
      {
        (object) Math.Round(this.size, 3)
      });
    }

    public UnitTypedSize ConvertTo(UnitType destinationType)
    {
      double size = this.Size;
      if (this.Units != destinationType)
      {
        switch (destinationType)
        {
          case UnitType.Points:
            size = UnitTypedSize.ConvertPixelToPoint(this.Size);
            break;
          case UnitType.Pixels:
            size = UnitTypedSize.ConvertPointToPixel(this.Size);
            break;
        }
      }
      return new UnitTypedSize(size, destinationType);
    }

    private static double ConvertPixelToPoint(double pixelValue)
    {
      return 0.75 * pixelValue;
    }

    private static double ConvertPointToPixel(double pointValue)
    {
      return 4.0 / 3.0 * pointValue;
    }
  }
}

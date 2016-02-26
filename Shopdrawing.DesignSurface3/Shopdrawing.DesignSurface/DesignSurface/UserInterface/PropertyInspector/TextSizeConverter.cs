// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextSizeConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class TextSizeConverter : TypeConverter
  {
    private UnitType unitType;

    public TextSizeConverter(UnitType unitType)
    {
      this.unitType = unitType;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = value as string;
      if (string.IsNullOrEmpty(str))
        return (object) null;
      FontSizeConverter fontSizeConverter = new FontSizeConverter();
      double result;
      if (double.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) culture, out result))
        return (object) UnitTypedSize.CreateFromUnits(result, this.unitType);
      return (object) UnitTypedSize.CreateFromPixels((double) fontSizeConverter.ConvertFromString(context, culture, str), this.unitType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      UnitTypedSize unitTypedSize = value as UnitTypedSize;
      if (unitTypedSize == null)
        return base.ConvertTo(context, culture, value, destinationType);
      return (object) unitTypedSize.ConvertTo(this.unitType).ToString();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.RepeatConverterBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class RepeatConverterBase : TypeConverter
  {
    protected abstract string InfinityName { get; }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof (string) || sourceType == typeof (double))
        return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (double))
        return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = value as string;
      if (str != null)
      {
        if (str.Equals(this.InfinityName, StringComparison.OrdinalIgnoreCase))
          return (object) double.PositiveInfinity;
        return NoNanDoubleConverter.Instance.ConvertFrom(context, culture, value);
      }
      double? nullable = value as double?;
      if (nullable.HasValue)
        return (object) nullable.Value;
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      double? nullable = value as double?;
      if (!nullable.HasValue)
        return base.ConvertTo(context, culture, value, destinationType);
      if (double.IsPositiveInfinity(nullable.Value))
        return (object) this.InfinityName;
      return TypeDescriptor.GetConverter(typeof (double)).ConvertTo(context, culture, (object) nullable.Value, destinationType);
    }
  }
}

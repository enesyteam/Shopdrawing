// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterLengthConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterLengthConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      switch (Type.GetTypeCode(sourceType))
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.String:
          return true;
        default:
          return false;
      }
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType != typeof (InstanceDescriptor))
        return destinationType == typeof (string);
      return true;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null || !this.CanConvertFrom(value.GetType()))
        throw this.GetConvertFromException(value);
      string s = value as string;
      if (s != null)
        return (object) SplitterLengthConverter.FromString(s, culture);
      double d = Convert.ToDouble(value, (IFormatProvider) culture);
      if (double.IsNaN(d))
        d = 1.0;
      return (object) new SplitterLength(d, SplitterUnitType.Stretch);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == (Type) null)
        throw new ArgumentNullException("destinationType");
      if (value != null && value is SplitterLength)
      {
        SplitterLength length = (SplitterLength) value;
        if (destinationType == typeof (string))
          return (object) SplitterLengthConverter.ToString(length, culture);
        if (destinationType == typeof (InstanceDescriptor))
          return (object) new InstanceDescriptor((MemberInfo) typeof (SplitterLength).GetConstructor(new Type[2]
          {
            typeof (double),
            typeof (SplitterUnitType)
          }), (ICollection) new object[2]
          {
            (object) length.Value,
            (object) length.SplitterUnitType
          });
      }
      throw this.GetConvertToException(value, destinationType);
    }

    internal static SplitterLength FromString(string s, CultureInfo cultureInfo)
    {
      string str = s.Trim();
      double num = 1.0;
      SplitterUnitType unitType = SplitterUnitType.Stretch;
      if (str == "*")
        unitType = SplitterUnitType.Fill;
      else if (str.EndsWith("fixed", StringComparison.OrdinalIgnoreCase))
      {
        unitType = SplitterUnitType.Fixed;
        num = Convert.ToDouble(str.Substring(0, str.Length - 5), (IFormatProvider) cultureInfo);
      }
      else
        num = Convert.ToDouble(str, (IFormatProvider) cultureInfo);
      return new SplitterLength(num, unitType);
    }

    internal static string ToString(SplitterLength length, CultureInfo cultureInfo)
    {
      if (length.SplitterUnitType == SplitterUnitType.Fill)
        return "*";
      if (length.SplitterUnitType == SplitterUnitType.Fixed)
        return Convert.ToString(length.Value, (IFormatProvider) cultureInfo) + "fixed";
      return Convert.ToString(length.Value, (IFormatProvider) cultureInfo);
    }
  }
}

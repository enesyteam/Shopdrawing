// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DataStorePropertyEntryConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DataStorePropertyEntryConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (!(sourceType == typeof (string)))
        return base.CanConvertFrom(context, sourceType);
      return true;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = value as string;
      if (str == null)
        return base.ConvertFrom(context, culture, value);
      return (object) new DataStorePropertyEntry((SampleDataSet) null, (string) null, false)
      {
        Name = str
      };
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return false;
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}

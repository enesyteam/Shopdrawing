// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.NoNanDoubleConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class NoNanDoubleConverter : DoubleConverter
  {
    private static NoNanDoubleConverter instance;

    public static NoNanDoubleConverter Instance
    {
      get
      {
        if (NoNanDoubleConverter.instance == null)
          NoNanDoubleConverter.instance = new NoNanDoubleConverter();
        return NoNanDoubleConverter.instance;
      }
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      object obj = base.ConvertFrom(context, culture, value);
      if (obj is double && double.IsNaN((double) obj))
        throw new FormatException();
      return obj;
    }
  }
}

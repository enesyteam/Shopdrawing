// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ArrayInsertionConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Data
{
  [ContentProperty("Insertions")]
  public class ArrayInsertionConverter : IValueConverter
  {
    private List<ArrayInsertion> insertions;

    public List<ArrayInsertion> Insertions
    {
      get
      {
        return this.insertions;
      }
    }

    public ArrayInsertionConverter()
    {
      this.insertions = new List<ArrayInsertion>();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int num = 0;
      ArrayList arrayList = new ArrayList((ICollection) value);
      foreach (ArrayInsertion arrayInsertion in this.insertions)
      {
        if (arrayInsertion.Position >= 0 && arrayInsertion.Position + num <= arrayList.Count)
        {
          arrayList.Insert(arrayInsertion.Position + num, arrayInsertion.Value);
          ++num;
        }
      }
      return (object) arrayList;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.MultiValueHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  internal static class MultiValueHelper
  {
    public static void CheckValue<T>(object[] values, int index)
    {
      if (!(values[index] is T) && (values[index] != null || typeof (T).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ValueAtOffsetNotOfType, new object[2]
        {
          (object) index,
          (object) typeof (T).FullName
        }));
    }

    public static void CheckType<T>(Type[] types, int index)
    {
      if (!types[index].IsAssignableFrom(typeof (T)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_TargetAtOffsetNotExtendingType, new object[2]
        {
          (object) index,
          (object) typeof (T).FullName
        }));
    }
  }
}

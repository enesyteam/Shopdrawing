// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.PercentageStringValueFilter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class PercentageStringValueFilter : IStringValueFilter
  {
    public string FilterStringValue(object instance, string stringValue)
    {
      if (stringValue == null)
        return (string) null;
      string str = (string) null;
      NumberEditor numberEditor = instance as NumberEditor;
      if (numberEditor != null)
        str = numberEditor.Format;
      if (str != null && str.IndexOf('%') != -1 && stringValue.EndsWith("%", StringComparison.OrdinalIgnoreCase))
        stringValue = stringValue.Substring(0, stringValue.Length - 1);
      return stringValue;
    }
  }
}

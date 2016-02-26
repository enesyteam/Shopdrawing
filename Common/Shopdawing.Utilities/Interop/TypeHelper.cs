// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Interop.TypeHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Utility.Interop
{
  public static class TypeHelper
  {
    public static T ConvertType<T>(object source)
    {
      if (source == null)
        return default (T);
      Type type = typeof (T);
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
        type = type.GetGenericArguments()[0];
      if (type.IsAssignableFrom(source.GetType()))
        return (T) source;
      if (type.IsPrimitive)
      {
        try
        {
          return (T) Convert.ChangeType(source, type, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
          if (!(ex is InvalidCastException))
          {
            if (!(ex is ArgumentNullException))
            {
              if (!(ex is FormatException))
              {
                if (!(ex is OverflowException))
                  throw;
              }
            }
          }
        }
      }
      else
      {
        try
        {
          TypeConverter converter = TypeDescriptor.GetConverter(type);
          if (converter != null)
          {
            if (converter.CanConvertFrom(source.GetType()))
              return (T) converter.ConvertFrom(source);
          }
        }
        catch (NotSupportedException ex)
        {
        }
      }
      if (type == typeof (string))
        return (T) source.ToString();
      return default (T);
    }
  }
}

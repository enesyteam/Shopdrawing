// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Interop.TypeHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.Interop
{
  public static class TypeHelper
  {
      public static T ConvertType<T>(object source)
      {
          T t;
          if (source == null)
          {
              return default(T);
          }
          Type genericArguments = typeof(T);
          if (genericArguments.IsGenericType && genericArguments.GetGenericTypeDefinition() == typeof(Nullable<>))
          {
              genericArguments = genericArguments.GetGenericArguments()[0];
          }
          if (genericArguments.IsAssignableFrom(source.GetType()))
          {
              return (T)source;
          }
          if (!genericArguments.IsPrimitive)
          {
              try
              {
                  TypeConverter converter = TypeDescriptor.GetConverter(genericArguments);
                  if (converter != null && converter.CanConvertFrom(source.GetType()))
                  {
                      t = (T)converter.ConvertFrom(source);
                      return t;
                  }
              }
              catch (NotSupportedException notSupportedException)
              {
              }
          }
          else
          {
              try
              {
                  t = (T)Convert.ChangeType(source, genericArguments, CultureInfo.InvariantCulture);
                  return t;
              }
              catch (Exception exception1)
              {
                  Exception exception = exception1;
                  if (!(exception is InvalidCastException) && !(exception is ArgumentNullException) && !(exception is FormatException) && !(exception is OverflowException))
                  {
                      throw;
                  }
              }
          }
          if (genericArguments == typeof(string))
          {
              return (T)source; //(T)source.ToString();
          }
          return default(T);
      }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.TypeNameFormatter
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Expression.Code
{
  public static class TypeNameFormatter
  {
    private static Dictionary<Type, string> VBPrettyNameLookup = new Dictionary<Type, string>()
    {
      {
        typeof (object),
        "Object"
      },
      {
        typeof (void),
        ""
      },
      {
        typeof (string),
        "String"
      },
      {
        typeof (bool),
        "Boolean"
      },
      {
        typeof (int),
        "Integer"
      },
      {
        typeof (uint),
        "UInteger"
      },
      {
        typeof (float),
        "Float"
      },
      {
        typeof (double),
        "Double"
      },
      {
        typeof (long),
        "Long"
      },
      {
        typeof (ulong),
        "ULong"
      },
      {
        typeof (byte),
        "Byte"
      },
      {
        typeof (sbyte),
        "SByte"
      },
      {
        typeof (short),
        "Short"
      },
      {
        typeof (ushort),
        "UShort"
      },
      {
        typeof (char),
        "Char"
      }
    };
    private static Dictionary<Type, string> CSharpPrettyNameLookup = new Dictionary<Type, string>()
    {
      {
        typeof (object),
        "object"
      },
      {
        typeof (void),
        "void"
      },
      {
        typeof (string),
        "string"
      },
      {
        typeof (bool),
        "bool"
      },
      {
        typeof (int),
        "int"
      },
      {
        typeof (uint),
        "uint"
      },
      {
        typeof (float),
        "float"
      },
      {
        typeof (double),
        "double"
      },
      {
        typeof (long),
        "long"
      },
      {
        typeof (ulong),
        "ulong"
      },
      {
        typeof (byte),
        "byte"
      },
      {
        typeof (sbyte),
        "sbyte"
      },
      {
        typeof (short),
        "short"
      },
      {
        typeof (ushort),
        "ushort"
      },
      {
        typeof (char),
        "char"
      }
    };
    private static Func<Type, bool, string> VBGenericArgumentFormatter = (Func<Type, bool, string>) ((type, useFullName) => "of " + TypeNameFormatter.FormatTypeForVisualBasic(type, useFullName));
    private static Func<Type, bool, string> CSharpGenericArgumentFormatter = (Func<Type, bool, string>) ((type, useFullName) => TypeNameFormatter.FormatTypeForCSharp(type, useFullName));

    public static string FormatTypeForDefaultLanguage(Type type, bool useFullName)
    {
      return TypeNameFormatter.FormatTypeForCSharp(type, useFullName);
    }

    public static string GetShortName(string typeName)
    {
      if (string.IsNullOrEmpty(typeName))
        return string.Empty;
      int num = typeName.LastIndexOf('.');
      if (num != -1 && num != typeName.Length - 1)
        return typeName.Substring(num + 1);
      return typeName;
    }

    public static string FormatTypeForVisualBasic(Type type, bool useFullName)
    {
      if (type == (Type) null)
        return string.Empty;
      if (TypeNameFormatter.VBPrettyNameLookup.ContainsKey(type))
        return TypeNameFormatter.VBPrettyNameLookup[type];
      if (type.IsGenericType)
        return TypeNameFormatter.CreateGenericTypeString(type, useFullName, "(", TypeNameFormatter.VBGenericArgumentFormatter, ")");
      return TypeNameFormatter.CreateTypeString(type, useFullName);
    }

    public static string FormatTypeForCSharp(Type type, bool useFullName)
    {
      if (type == (Type) null)
        return string.Empty;
      if (TypeNameFormatter.CSharpPrettyNameLookup.ContainsKey(type))
        return TypeNameFormatter.CSharpPrettyNameLookup[type];
      if (type.IsGenericType)
        return TypeNameFormatter.CreateGenericTypeString(type, useFullName, "<", TypeNameFormatter.CSharpGenericArgumentFormatter, ">");
      return TypeNameFormatter.CreateTypeString(type, useFullName);
    }

    private static string CreateGenericTypeString(Type type, bool useFullName, string genericArgumentListStartCharacter, Func<Type, bool, string> genericTypeFormatter, string genericArgumentListEndCharacter)
    {
      StringBuilder stringBuilder = new StringBuilder(TypeNameFormatter.FormatGenericTypeName(type, useFullName) + genericArgumentListStartCharacter);
      Type[] genericArguments = type.GetGenericArguments();
      for (int index = 0; index < genericArguments.Length; ++index)
      {
        stringBuilder.Append(genericTypeFormatter(genericArguments[index], useFullName));
        if (index != genericArguments.Length - 1)
          stringBuilder.Append(", ");
      }
      stringBuilder.Append(genericArgumentListEndCharacter);
      return stringBuilder.ToString();
    }

    internal static string FormatGenericTypeName(Type type, bool useFullName)
    {
      string typeString = TypeNameFormatter.CreateTypeString(type, useFullName);
      int length = typeString.IndexOf('`');
      if (length == -1)
        return typeString;
      return typeString.Substring(0, length);
    }

    private static string CreateTypeString(Type type, bool useFullName)
    {
      return (useFullName ? type.ToString() : type.Name).Replace('+', '.');
    }
  }
}

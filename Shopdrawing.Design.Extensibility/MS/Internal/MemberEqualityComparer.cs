// Decompiled with JetBrains decompiler
// Type: MS.Internal.MemberEqualityComparer
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MS.Internal
{
  internal class MemberEqualityComparer : IEqualityComparer, IEqualityComparer<MemberInfo>
  {
    internal static bool Equals(MemberInfo mx, MemberInfo my)
    {
      if (object.ReferenceEquals((object) mx, (object) my))
        return true;
      if (mx == null || my == null || (mx.MetadataToken != my.MetadataToken || !object.Equals((object) mx.Module, (object) my.Module)))
        return false;
      Type type1;
      Type type2;
      if ((type1 = mx as Type) != null && (type2 = my as Type) != null)
      {
        if (type1.IsGenericType)
        {
          if (type1.IsGenericTypeDefinition != type2.IsGenericTypeDefinition)
            return false;
          if (!type1.IsGenericTypeDefinition)
            return MemberEqualityComparer.TypesEqual(type1.GetGenericArguments(), type2.GetGenericArguments());
        }
      }
      else
      {
        MethodInfo methodInfo1;
        MethodInfo methodInfo2;
        if ((methodInfo1 = mx as MethodInfo) != null && (methodInfo2 = my as MethodInfo) != null && methodInfo1.IsGenericMethod)
        {
          if (methodInfo1.IsGenericMethodDefinition != methodInfo2.IsGenericMethodDefinition)
            return false;
          if (!methodInfo1.IsGenericMethodDefinition)
            return MemberEqualityComparer.TypesEqual(methodInfo1.GetGenericArguments(), methodInfo2.GetGenericArguments());
        }
      }
      return true;
    }

    private static bool TypesEqual(Type[] tx, Type[] ty)
    {
      for (int index = 0; index < tx.Length; ++index)
      {
        if (!MemberEqualityComparer.Equals((MemberInfo) tx[index], (MemberInfo) ty[index]))
          return false;
      }
      return true;
    }

    bool IEqualityComparer.Equals(object x, object y)
    {
      return MemberEqualityComparer.Equals(x as MemberInfo, y as MemberInfo);
    }

    int IEqualityComparer.GetHashCode(object obj)
    {
      MemberInfo memberInfo = obj as MemberInfo;
      if (memberInfo != null)
        return memberInfo.MetadataToken;
      return 0;
    }

    bool IEqualityComparer<MemberInfo>.Equals(MemberInfo x, MemberInfo y)
    {
      return MemberEqualityComparer.Equals(x, y);
    }

    int IEqualityComparer<MemberInfo>.GetHashCode(MemberInfo obj)
    {
      if (obj != null)
        return obj.MetadataToken;
      return 0;
    }
  }
}

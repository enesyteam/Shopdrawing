// Decompiled with JetBrains decompiler
// Type: MS.Internal.Metadata.AttributeDataCache
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using Microsoft.Windows.Design.Metadata;
using MS.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MS.Internal.Metadata
{
  internal static class AttributeDataCache
  {
    private static Hashtable _baseMemberMap = new Hashtable((IEqualityComparer) new MemberEqualityComparer());
    private static Hashtable _attributeDataCache = new Hashtable();
    private static object _noMemberInfo = new object();
    private static object _syncObject = new object();
    private static readonly BindingFlags _getInfoBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private static Dictionary<MemberTypes, AttributeDataCache.GetBaseMemberCallback> _baseMemberFinders = new Dictionary<MemberTypes, AttributeDataCache.GetBaseMemberCallback>();
    private static Dictionary<AttributeDataCache.AttributeKey, object[]> _attributeCache;

    static AttributeDataCache()
    {
      AttributeDataCache._baseMemberFinders[MemberTypes.Constructor] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseConstructorInfo);
      AttributeDataCache._baseMemberFinders[MemberTypes.Method] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseMethodInfo);
      AttributeDataCache._baseMemberFinders[MemberTypes.Property] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBasePropertyInfo);
      AttributeDataCache._baseMemberFinders[MemberTypes.Event] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseEventInfo);
    }

    internal static MemberInfo GetBaseMemberInfo(MemberInfo member)
    {
      object obj = AttributeDataCache._baseMemberMap[(object) member];
      if (obj == AttributeDataCache._noMemberInfo)
        return (MemberInfo) null;
      if (obj == null)
      {
        obj = (object) AttributeDataCache.CalculateBaseMemberInfo(member);
        lock (AttributeDataCache._syncObject)
          AttributeDataCache._baseMemberMap[(object) member] = obj ?? AttributeDataCache._noMemberInfo;
      }
      return (MemberInfo) obj;
    }

    internal static IEnumerable<object> GetAttributeTableAttributes(Assembly assembly, Type type, string memberName, AttributeTable[] tables)
    {
      if (tables != null && tables.Length != 0)
      {
        Identifier memberIdentifier = Identifier.For(memberName);
        for (int idx = tables.Length - 1; idx >= 0; --idx)
        {
          AttributeTable table = tables[idx];
          IEnumerable attrEnum;
          if (assembly != null)
            attrEnum = table.GetCustomAttributes(assembly);
          else if (table.ContainsAttributes(type))
            attrEnum = memberName != null ? table.GetCustomAttributes(type, memberIdentifier) : table.GetCustomAttributes(type);
          else
            continue;
          foreach (object obj in attrEnum)
            yield return obj;
        }
      }
    }

    internal static IEnumerable<object> GetClrAttributes(Assembly assembly, MemberInfo member, Type attributeType)
    {
      AttributeDataCache.AttributeKey key = new AttributeDataCache.AttributeKey(assembly, member, attributeType);
      object[] objArray;
      bool flag;
      lock (AttributeDataCache._syncObject)
      {
        if (AttributeDataCache._attributeCache == null)
          AttributeDataCache._attributeCache = new Dictionary<AttributeDataCache.AttributeKey, object[]>();
        flag = AttributeDataCache._attributeCache.TryGetValue(key, out objArray);
      }
      if (!flag)
      {
        try
        {
          objArray = assembly == null ? (attributeType == null ? member.GetCustomAttributes(false) : member.GetCustomAttributes(attributeType, false)) : (attributeType == null ? assembly.GetCustomAttributes(false) : assembly.GetCustomAttributes(attributeType, false));
        }
        catch
        {
          objArray = (object[]) null;
        }
        lock (AttributeDataCache._syncObject)
          AttributeDataCache._attributeCache[key] = objArray;
      }
      return (IEnumerable<object>) objArray;
    }

    internal static AttributeData GetAttributeData(Type attributeType)
    {
      AttributeData attributeData = AttributeDataCache._attributeDataCache[(object) attributeType] as AttributeData;
      if (attributeData == null)
      {
        attributeData = new AttributeData(attributeType);
        lock (AttributeDataCache._syncObject)
          AttributeDataCache._attributeDataCache[(object) attributeType] = (object) attributeData;
      }
      return attributeData;
    }

    private static MemberInfo CalculateBaseMemberInfo(MemberInfo member)
    {
      Type type = member as Type;
      if (type != null)
        return (MemberInfo) type.BaseType;
      Type baseType = member.DeclaringType.BaseType;
      MemberInfo memberInfo;
      for (memberInfo = (MemberInfo) null; baseType != null && memberInfo == null; baseType = baseType.BaseType)
        memberInfo = AttributeDataCache._baseMemberFinders[member.MemberType](member, baseType);
      return memberInfo;
    }

    private static MemberInfo GetBaseConstructorInfo(MemberInfo info, Type targetType)
    {
      return (MemberInfo) null;
    }

    private static MemberInfo GetBaseMethodInfo(MemberInfo info, Type targetType)
    {
      MethodInfo methodInfo = info as MethodInfo;
      if (methodInfo.IsStatic)
        return (MemberInfo) null;
      return (MemberInfo) targetType.GetMethod(methodInfo.Name, AttributeDataCache._getInfoBindingFlags, (Binder) null, AttributeDataCache.ToTypeArray(methodInfo.GetParameters()), (ParameterModifier[]) null);
    }

    private static MemberInfo GetBasePropertyInfo(MemberInfo info, Type targetType)
    {
      PropertyInfo propertyInfo = info as PropertyInfo;
      return (MemberInfo) targetType.GetProperty(propertyInfo.Name, AttributeDataCache._getInfoBindingFlags, (Binder) null, propertyInfo.PropertyType, AttributeDataCache.ToTypeArray(propertyInfo.GetIndexParameters()), (ParameterModifier[]) null);
    }

    private static MemberInfo GetBaseEventInfo(MemberInfo info, Type targetType)
    {
      EventInfo eventInfo = info as EventInfo;
      return (MemberInfo) targetType.GetEvent(eventInfo.Name, AttributeDataCache._getInfoBindingFlags);
    }

    private static Type[] ToTypeArray(ParameterInfo[] parameterInfo)
    {
      if (parameterInfo == null)
        return (Type[]) null;
      Type[] typeArray = new Type[parameterInfo.Length];
      for (int index = 0; index < parameterInfo.Length; ++index)
        typeArray[index] = parameterInfo[index].ParameterType;
      return typeArray;
    }

    private delegate MemberInfo GetBaseMemberCallback(MemberInfo member, Type targetType);

    private struct AttributeKey : IEquatable<AttributeDataCache.AttributeKey>
    {
      private Assembly _assembly;
      private MemberInfo _member;
      private Type _attributeType;
      private int _hashCode;

      internal AttributeKey(Assembly assembly, MemberInfo member, Type attributeType)
      {
        this._assembly = assembly;
        this._member = member;
        this._attributeType = attributeType;
        this._hashCode = assembly == null ? member.GetHashCode() : assembly.GetHashCode();
        if (this._attributeType == null)
          return;
        this._hashCode += this._attributeType.GetHashCode();
      }

      public override int GetHashCode()
      {
        return this._hashCode;
      }

      public override bool Equals(object obj)
      {
        if (obj is AttributeDataCache.AttributeKey)
          return this.Equals((AttributeDataCache.AttributeKey) obj);
        return false;
      }

      public bool Equals(AttributeDataCache.AttributeKey other)
      {
        if ((!object.ReferenceEquals((object) this._assembly, (object) other._assembly) || this._assembly == null) && !MemberEqualityComparer.Equals(this._member, other._member))
          return false;
        return MemberEqualityComparer.Equals((MemberInfo) this._attributeType, (MemberInfo) other._attributeType);
      }
    }
  }
}

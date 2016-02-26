// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.AttributeTableContainer
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Windows.Design.Metadata
{
  public class AttributeTableContainer
  {
    private object _syncLock = new object();
    private List<AttributeTable> _tables;
    private AttributeTable[] _tableArray;
    private Dictionary<Type, object> _seenAttributes;
    private List<object> _compiledAttributes;

    public IEnumerable<AttributeTable> AttributeTables
    {
      get
      {
        return (IEnumerable<AttributeTable>) this.Tables;
      }
    }

    private AttributeTable[] AttributeTableArray
    {
      get
      {
        AttributeTable[] attributeTableArray = this._tableArray;
        if (attributeTableArray == null)
        {
          lock (this._syncLock)
          {
            attributeTableArray = this.Tables.ToArray();
            this._tableArray = attributeTableArray;
          }
        }
        return attributeTableArray;
      }
    }

    private List<AttributeTable> Tables
    {
      get
      {
        if (this._tables == null)
        {
          lock (this._syncLock)
          {
            if (this._tables == null)
              this._tables = new List<AttributeTable>();
          }
        }
        return this._tables;
      }
    }

    public void AddAttributeTable(AttributeTable table)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      lock (this._syncLock)
      {
        this.Tables.Add(table);
        this._tableArray = (AttributeTable[]) null;
      }
    }

    private void FillAttributes(Type type, MemberInfo member, Type attributeType, Func<object, object> reflectionMapper, List<object> compiledAttributes, Dictionary<Type, object> seenAttributes)
    {
      Type currentType = type;
      MemberInfo memberInfo = member;
      bool firstIteration = true;
      bool flag = member is Type;
      bool includeClrAttributes = flag || memberInfo.DeclaringType == currentType;
      Type runtimeAttributeType = attributeType;
      if (reflectionMapper != null)
        runtimeAttributeType = (Type) reflectionMapper((object) attributeType);
      if (runtimeAttributeType == null && attributeType != null)
        return;
      while (currentType != null && memberInfo != null)
      {
        this.FillAttributesHelper(attributeType, reflectionMapper, compiledAttributes, seenAttributes, currentType, memberInfo, firstIteration, includeClrAttributes, runtimeAttributeType);
        firstIteration = false;
        if (flag && currentType.IsGenericType)
        {
          Type genericTypeDefinition = currentType.GetGenericTypeDefinition();
          this.FillAttributesHelper(attributeType, reflectionMapper, compiledAttributes, seenAttributes, genericTypeDefinition, (MemberInfo) genericTypeDefinition, firstIteration, includeClrAttributes, runtimeAttributeType);
        }
        if (flag || memberInfo.DeclaringType == currentType)
          memberInfo = AttributeDataCache.GetBaseMemberInfo(memberInfo);
        currentType = currentType.BaseType;
        includeClrAttributes = flag || memberInfo == null || memberInfo.DeclaringType == currentType;
      }
    }

    private void FillAttributesHelper(Type attributeType, Func<object, object> reflectionMapper, List<object> compiledAttributes, Dictionary<Type, object> seenAttributes, Type currentType, MemberInfo currentMember, bool firstIteration, bool includeClrAttributes, Type runtimeAttributeType)
    {
      Type runtimeType = reflectionMapper == null ? currentType : (Type) reflectionMapper((object) currentType);
      if (runtimeType == null && currentType != null)
        return;
      foreach (object obj in this.MergeAttributesIterator(currentType, currentMember, attributeType, runtimeType, runtimeAttributeType, includeClrAttributes))
      {
        AttributeData attributeData = AttributeDataCache.GetAttributeData(obj.GetType());
        if ((!seenAttributes.ContainsKey(attributeData.AttributeType) || attributeData.AllowsMultiple) && (firstIteration || attributeData.IsInheritable))
        {
          compiledAttributes.Add(obj);
          seenAttributes[attributeData.AttributeType] = obj;
        }
      }
    }

    public IEnumerable<object> GetAttributes(Assembly assembly, Type attributeType)
    {
      return this.GetAttributes(assembly, attributeType, (Func<object, object>) null);
    }

    public IEnumerable<object> GetAttributes(Assembly assembly, Type attributeType, Func<object, object> reflectionMapper)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");
      List<object> list;
      Dictionary<Type, object> dictionary;
      lock (this._syncLock)
      {
        list = this._compiledAttributes;
        dictionary = this._seenAttributes;
        this._compiledAttributes = (List<object>) null;
        this._seenAttributes = (Dictionary<Type, object>) null;
      }
      if (list == null)
        list = new List<object>();
      else
        list.Clear();
      if (dictionary == null)
        dictionary = new Dictionary<Type, object>();
      else
        dictionary.Clear();
      foreach (object obj in this.MergeAttributesIterator(assembly, attributeType, true, reflectionMapper))
      {
        AttributeData attributeData = AttributeDataCache.GetAttributeData(obj.GetType());
        if (!dictionary.ContainsKey(attributeData.AttributeType) || attributeData.AllowsMultiple)
        {
          list.Add(obj);
          dictionary[attributeData.AttributeType] = obj;
        }
      }
      object[] objArray = list.ToArray();
      lock (this._syncLock)
      {
        this._compiledAttributes = list;
        this._seenAttributes = dictionary;
      }
      return (IEnumerable<object>) objArray;
    }

    public IEnumerable<object> GetAttributes(MemberInfo member, Type attributeType)
    {
      return this.GetAttributes(member, attributeType, (Func<object, object>) null);
    }

    public IEnumerable<object> GetAttributes(MemberInfo member, Type attributeType, Func<object, object> reflectionMapper)
    {
      if (member == null)
        throw new ArgumentNullException("member");
      PropertyInfo propertyInfo;
      if ((propertyInfo = member as PropertyInfo) != null)
        return this.GetAttributes(propertyInfo.ReflectedType, (MemberInfo) propertyInfo, attributeType, reflectionMapper, propertyInfo.PropertyType);
      Type type;
      if ((type = member as Type) != null)
        return this.GetAttributes(type, (MemberInfo) type, attributeType, reflectionMapper, type.GetInterfaces());
      EventInfo eventInfo;
      if ((eventInfo = member as EventInfo) != null)
        return this.GetAttributes(eventInfo.ReflectedType, (MemberInfo) eventInfo, attributeType, reflectionMapper, eventInfo.EventHandlerType);
      FieldInfo fieldInfo;
      if ((fieldInfo = member as FieldInfo) == null)
        return this.GetAttributes(member.ReflectedType, member, attributeType, reflectionMapper);
      return this.GetAttributes(fieldInfo.ReflectedType, (MemberInfo) fieldInfo, attributeType, reflectionMapper, fieldInfo.FieldType);
    }

    private IEnumerable<object> GetAttributes(Type type, MemberInfo member, Type attributeType, Func<object, object> reflectionMapper, params Type[] mergeTypes)
    {
      List<object> compiledAttributes;
      Dictionary<Type, object> seenAttributes;
      lock (this._syncLock)
      {
        compiledAttributes = this._compiledAttributes;
        seenAttributes = this._seenAttributes;
        this._compiledAttributes = (List<object>) null;
        this._seenAttributes = (Dictionary<Type, object>) null;
      }
      if (compiledAttributes == null)
        compiledAttributes = new List<object>();
      else
        compiledAttributes.Clear();
      if (seenAttributes == null)
        seenAttributes = new Dictionary<Type, object>();
      else
        seenAttributes.Clear();
      this.FillAttributes(type, member, attributeType, reflectionMapper, compiledAttributes, seenAttributes);
      if (mergeTypes != null)
      {
        foreach (Type type1 in mergeTypes)
          this.FillAttributes(type1, (MemberInfo) type1, attributeType, reflectionMapper, compiledAttributes, seenAttributes);
      }
      object[] objArray = compiledAttributes.ToArray();
      lock (this._syncLock)
      {
        this._compiledAttributes = compiledAttributes;
        this._seenAttributes = seenAttributes;
      }
      return (IEnumerable<object>) objArray;
    }

    public IEnumerable<object> GetLocalAttributes(MemberInfo member, Type attributeType)
    {
      return this.GetLocalAttributes(member, attributeType, (Func<object, object>) null);
    }

    public IEnumerable<object> GetLocalAttributes(MemberInfo member, Type attributeType, Func<object, object> reflectionMapper)
    {
      if (member == null)
        throw new ArgumentNullException("member");
      return this.GetAttributes(member as Type ?? member.ReflectedType, member, attributeType, reflectionMapper);
    }

    private IEnumerable<object> MergeAttributesIterator(Type type, MemberInfo member, Type attributeType, Type runtimeType, Type runtimeAttributeType, bool includeClrAttributes)
    {
      string memberName = member is Type ? (string) null : member.Name;
      AttributeTable[] tables = this.AttributeTableArray;
      foreach (object o in AttributeDataCache.GetAttributeTableAttributes((Assembly) null, runtimeType, memberName, tables))
      {
        if (runtimeAttributeType == null || runtimeAttributeType.IsInstanceOfType(o))
          yield return o;
      }
      if (includeClrAttributes)
      {
        IEnumerable<object> enumerator = AttributeDataCache.GetClrAttributes((Assembly) null, member, attributeType);
        if (enumerator != null)
        {
          foreach (object obj in enumerator)
            yield return obj;
        }
      }
    }

    private IEnumerable<object> MergeAttributesIterator(Assembly assembly, Type attributeType, bool includeClrAttributes, Func<object, object> reflectionMapper)
    {
      AttributeTable[] tables = this.AttributeTableArray;
      Assembly runtimeAssembly = assembly;
      Type runtimeAttributeType = attributeType;
      if (reflectionMapper != null)
      {
        runtimeAssembly = (Assembly) reflectionMapper((object) assembly);
        if (runtimeAttributeType != null)
          runtimeAttributeType = (Type) reflectionMapper((object) attributeType);
      }
      if ((runtimeAssembly != null || assembly == null) && (runtimeAttributeType != null || attributeType == null))
      {
        foreach (object o in AttributeDataCache.GetAttributeTableAttributes(runtimeAssembly, (Type) null, (string) null, tables))
        {
          if (runtimeAttributeType == null || runtimeAttributeType.IsInstanceOfType(o))
            yield return o;
        }
        if (includeClrAttributes)
        {
          IEnumerable<object> enumerator = AttributeDataCache.GetClrAttributes(assembly, (MemberInfo) null, attributeType);
          if (enumerator != null)
          {
            foreach (object obj in enumerator)
              yield return obj;
          }
        }
      }
    }
  }
}

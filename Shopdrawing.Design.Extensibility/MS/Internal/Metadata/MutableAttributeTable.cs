// Decompiled with JetBrains decompiler
// Type: MS.Internal.Metadata.MutableAttributeTable
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using Microsoft.Windows.Design.Metadata;
using MS.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MS.Internal.Metadata
{
  internal class MutableAttributeTable
  {
    private static object[] _empty = new object[0];
    private object _syncLock = new object();
    private Dictionary<Assembly, MutableAttributeTable.AttributeList> _assemblyAttributes;
    private Dictionary<Type, MutableAttributeTable.TypeMetadata> _metadata;

    internal IEnumerable<Type> AttributedTypes
    {
      get
      {
        return (IEnumerable<Type>) this._metadata.Keys;
      }
    }

    internal MutableAttributeTable()
    {
      this._assemblyAttributes = new Dictionary<Assembly, MutableAttributeTable.AttributeList>();
      this._metadata = new Dictionary<Type, MutableAttributeTable.TypeMetadata>();
    }

    private static void AddAttributeMetadata(MutableAttributeTable.TypeMetadata newMd, MutableAttributeTable.TypeMetadata existingMd)
    {
      if (newMd.TypeAttributes == null)
        return;
      if (existingMd.TypeAttributes != null)
        existingMd.TypeAttributes.AddRange((IEnumerable<object>) newMd.TypeAttributes);
      else
        existingMd.TypeAttributes = newMd.TypeAttributes;
    }

    private static void AddAttributes(MutableAttributeTable.AttributeList list, IEnumerable<object> attributes)
    {
      list.AddRange(attributes);
    }

    internal void AddCallback(Type type, AttributeCallback callback)
    {
      this.GetTypeList(type).Add((object) callback);
    }

    internal void AddCustomAttributes(Assembly assembly, IEnumerable<object> attributes)
    {
      MutableAttributeTable.AddAttributes(this.GetAssemblyList(assembly), attributes);
    }

    internal void AddCustomAttributes(Type type, IEnumerable<object> attributes)
    {
      MutableAttributeTable.AddAttributes(this.GetTypeList(type), attributes);
    }

    internal void AddCustomAttributes(Type ownerType, string memberName, IEnumerable<object> attributes)
    {
      MutableAttributeTable.AddAttributes(this.GetMemberList(ownerType, memberName), attributes);
    }

    private static void AddMemberMetadata(MutableAttributeTable.TypeMetadata newMd, MutableAttributeTable.TypeMetadata existingMd)
    {
      if (newMd.MemberAttributes == null)
        return;
      if (existingMd.MemberAttributes != null)
      {
        foreach (KeyValuePair<string, MutableAttributeTable.AttributeList> keyValuePair in newMd.MemberAttributes)
        {
          MutableAttributeTable.AttributeList attributeList;
          if (existingMd.MemberAttributes.TryGetValue(keyValuePair.Key, out attributeList))
            attributeList.AddRange((IEnumerable<object>) keyValuePair.Value);
          else
            existingMd.MemberAttributes.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      else
        existingMd.MemberAttributes = newMd.MemberAttributes;
    }

    internal void AddTable(MutableAttributeTable table)
    {
      foreach (KeyValuePair<Type, MutableAttributeTable.TypeMetadata> keyValuePair in table._metadata)
        this.AddTypeMetadata(keyValuePair.Key, keyValuePair.Value);
      foreach (KeyValuePair<Assembly, MutableAttributeTable.AttributeList> keyValuePair in table._assemblyAttributes)
        this.AddAssemblyMetadata(keyValuePair.Key, keyValuePair.Value);
    }

    private void AddTypeMetadata(Type type, MutableAttributeTable.TypeMetadata md)
    {
      MutableAttributeTable.TypeMetadata existingMd;
      if (this._metadata.TryGetValue(type, out existingMd))
      {
        MutableAttributeTable.AddAttributeMetadata(md, existingMd);
        MutableAttributeTable.AddMemberMetadata(md, existingMd);
      }
      else
        this._metadata.Add(type, md);
    }

    private void AddAssemblyMetadata(Assembly assembly, MutableAttributeTable.AttributeList attributes)
    {
      MutableAttributeTable.AttributeList attributeList;
      if (this._assemblyAttributes.TryGetValue(assembly, out attributeList))
      {
        if (!attributes.IsExpanded)
          attributeList.IsExpanded = false;
        attributeList.AddRange((IEnumerable<object>) attributes);
      }
      else
        this._assemblyAttributes.Add(assembly, attributes);
    }

    internal bool ContainsAttributes(Type type)
    {
      return this._metadata.ContainsKey(type);
    }

    private void ExpandAttributes(Type type, MutableAttributeTable.AttributeList attributes)
    {
      if (attributes.IsExpanded)
        return;
      for (int index = 0; index < attributes.Count; ++index)
      {
        for (AttributeCallback attributeCallback = attributes[index] as AttributeCallback; attributeCallback != null; attributeCallback = index >= attributes.Count ? (AttributeCallback) null : attributes[index] as AttributeCallback)
        {
          attributes.RemoveAt(index);
          AttributeCallbackBuilder builder = new AttributeCallbackBuilder(this, type);
          attributeCallback(builder);
        }
      }
    }

    internal IEnumerable GetCustomAttributes(Assembly assembly)
    {
      MutableAttributeTable.AttributeList attributeList;
      if (this._assemblyAttributes.TryGetValue(assembly, out attributeList))
        return (IEnumerable) attributeList.AsReadOnly();
      return (IEnumerable) MutableAttributeTable._empty;
    }

    internal IEnumerable GetCustomAttributes(Type type)
    {
      MutableAttributeTable.AttributeList expandedAttributes = this.GetExpandedAttributes(type, (object) null, (MutableAttributeTable.GetAttributesCallback) ((typeToGet, callbackParam) =>
      {
        MutableAttributeTable.TypeMetadata typeMetadata;
        if (this._metadata.TryGetValue(typeToGet, out typeMetadata))
          return typeMetadata.TypeAttributes;
        return (MutableAttributeTable.AttributeList) null;
      }));
      if (expandedAttributes != null)
        return (IEnumerable) expandedAttributes.AsReadOnly();
      return (IEnumerable) MutableAttributeTable._empty;
    }

    internal IEnumerable GetCustomAttributes(Type ownerType, string memberName)
    {
      MutableAttributeTable.AttributeList expandedAttributes = this.GetExpandedAttributes(ownerType, (object) memberName, (MutableAttributeTable.GetAttributesCallback) ((typeToGet, callbackParam) =>
      {
        string key = (string) callbackParam;
        MutableAttributeTable.TypeMetadata typeMetadata;
        if (this._metadata.TryGetValue(typeToGet, out typeMetadata))
        {
          if (typeMetadata.MemberAttributes == null && typeMetadata.TypeAttributes != null && !typeMetadata.TypeAttributes.IsExpanded)
          {
            lock (this._syncLock)
            {
              this.ExpandAttributes(ownerType, typeMetadata.TypeAttributes);
              typeMetadata.TypeAttributes.IsExpanded = true;
            }
          }
          MutableAttributeTable.AttributeList attributeList;
          if (typeMetadata.MemberAttributes != null && typeMetadata.MemberAttributes.TryGetValue(key, out attributeList))
            return attributeList;
        }
        return (MutableAttributeTable.AttributeList) null;
      }));
      if (expandedAttributes != null)
        return (IEnumerable) expandedAttributes.AsReadOnly();
      return (IEnumerable) MutableAttributeTable._empty;
    }

    private MutableAttributeTable.AttributeList GetMemberList(Type ownerType, string memberName)
    {
      MutableAttributeTable.TypeMetadata typeMetadata = this.GetTypeMetadata(ownerType);
      if (typeMetadata.MemberAttributes == null)
        typeMetadata.MemberAttributes = new Dictionary<string, MutableAttributeTable.AttributeList>();
      MutableAttributeTable.AttributeList attributeList;
      if (!typeMetadata.MemberAttributes.TryGetValue(memberName, out attributeList))
      {
        attributeList = new MutableAttributeTable.AttributeList();
        typeMetadata.MemberAttributes.Add(memberName, attributeList);
      }
      return attributeList;
    }

    private MutableAttributeTable.AttributeList GetAssemblyList(Assembly assembly)
    {
      MutableAttributeTable.AttributeList attributeList;
      if (!this._assemblyAttributes.TryGetValue(assembly, out attributeList))
      {
        attributeList = new MutableAttributeTable.AttributeList();
        this._assemblyAttributes.Add(assembly, attributeList);
      }
      return attributeList;
    }

    private MutableAttributeTable.AttributeList GetExpandedAttributes(Type type, object callbackParam, MutableAttributeTable.GetAttributesCallback callback)
    {
      MutableAttributeTable.AttributeList attributes = callback(type, callbackParam);
      if (attributes != null && !attributes.IsExpanded)
      {
        lock (attributes)
        {
          if (!attributes.IsExpanded)
          {
            lock (this._syncLock)
            {
              this.ExpandAttributes(type, attributes);
              attributes.IsExpanded = true;
            }
          }
        }
      }
      return attributes;
    }

    private MutableAttributeTable.AttributeList GetTypeList(Type type)
    {
      MutableAttributeTable.TypeMetadata typeMetadata = this.GetTypeMetadata(type);
      if (typeMetadata.TypeAttributes == null)
        typeMetadata.TypeAttributes = new MutableAttributeTable.AttributeList();
      return typeMetadata.TypeAttributes;
    }

    private MutableAttributeTable.TypeMetadata GetTypeMetadata(Type type)
    {
      MutableAttributeTable.TypeMetadata typeMetadata;
      if (!this._metadata.TryGetValue(type, out typeMetadata))
      {
        typeMetadata = new MutableAttributeTable.TypeMetadata();
        this._metadata.Add(type, typeMetadata);
      }
      return typeMetadata;
    }

    public void ValidateTable()
    {
      List<string> list = (List<string>) null;
      foreach (KeyValuePair<Type, MutableAttributeTable.TypeMetadata> keyValuePair1 in this._metadata)
      {
        this.GetCustomAttributes(keyValuePair1.Key);
        if (keyValuePair1.Value.MemberAttributes != null)
        {
          foreach (KeyValuePair<string, MutableAttributeTable.AttributeList> keyValuePair2 in keyValuePair1.Value.MemberAttributes)
          {
            this.GetCustomAttributes(keyValuePair1.Key, keyValuePair2.Key);
            MemberInfo[] member = keyValuePair1.Key.GetMember(keyValuePair2.Key, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
            string str = (string) null;
            if (member == null || member.Length == 0)
              str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValidationNoMatchingMember, new object[2]
              {
                (object) keyValuePair2.Key,
                (object) keyValuePair1.Key.FullName
              });
            if (str != null)
            {
              if (list == null)
                list = new List<string>();
              list.Add(str);
            }
          }
        }
      }
      if (list != null)
        throw new AttributeTableValidationException(Resources.Error_TableValidationFailed, (IEnumerable<string>) list);
    }

    private class TypeMetadata
    {
      internal MutableAttributeTable.AttributeList TypeAttributes;
      internal Dictionary<string, MutableAttributeTable.AttributeList> MemberAttributes;
    }

    private class AttributeList : List<object>
    {
      private bool _isExpanded;

      internal bool IsExpanded
      {
        get
        {
          return this._isExpanded;
        }
        set
        {
          this._isExpanded = value;
        }
      }
    }

    private delegate MutableAttributeTable.AttributeList GetAttributesCallback(Type type, object callbackParam);
  }
}

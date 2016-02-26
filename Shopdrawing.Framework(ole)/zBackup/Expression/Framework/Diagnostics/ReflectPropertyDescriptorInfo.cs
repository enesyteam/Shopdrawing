// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.ReflectPropertyDescriptorInfo
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public sealed class ReflectPropertyDescriptorInfo
  {
    private readonly string typeName;
    private readonly string propertyName;
    private readonly int count;

    public string TypeName
    {
      get
      {
        return this.typeName;
      }
    }

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }
    }

    public int Count
    {
      get
      {
        return this.count;
      }
    }

    private ReflectPropertyDescriptorInfo(string typeName, string propertyName, int count)
    {
      this.typeName = typeName;
      this.propertyName = propertyName;
      this.count = count;
    }

    public static IEnumerable<ReflectPropertyDescriptorInfo> Find()
    {
      ReflectionPermission perm = new ReflectionPermission(PermissionState.Unrestricted);
      if (perm.IsUnrestricted())
      {
        Type reflectType = typeof (PropertyDescriptor).Module.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");
        FieldInfo propertyCacheFieldInfo = reflectType.GetField("_propertyCache", BindingFlags.Static | BindingFlags.NonPublic);
        Hashtable propertyCache = (Hashtable) propertyCacheFieldInfo.GetValue((object) null);
        if (propertyCache != null)
        {
          DictionaryEntry[] entries = new DictionaryEntry[propertyCache.Count];
          propertyCache.CopyTo((Array) entries, 0);
          FieldInfo valueChangedHandlersFieldInfo = typeof (PropertyDescriptor).GetField("valueChangedHandlers", BindingFlags.Instance | BindingFlags.NonPublic);
          foreach (DictionaryEntry dictionaryEntry in entries)
          {
            PropertyDescriptor[] propertyDescriptors = (PropertyDescriptor[]) dictionaryEntry.Value;
            if (propertyDescriptors != null)
            {
              foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
              {
                Hashtable valueChangedHandlers = (Hashtable) valueChangedHandlersFieldInfo.GetValue((object) propertyDescriptor);
                if (valueChangedHandlers != null && valueChangedHandlers.Count != 0)
                  yield return new ReflectPropertyDescriptorInfo(dictionaryEntry.Key.ToString(), propertyDescriptor.Name, valueChangedHandlers.Count);
              }
            }
          }
        }
      }
    }
  }
}

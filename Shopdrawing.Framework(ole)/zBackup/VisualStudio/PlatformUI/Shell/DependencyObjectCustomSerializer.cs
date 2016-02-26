// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DependencyObjectCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public abstract class DependencyObjectCustomSerializer : ICustomXmlSerializer
  {
    public IDependencyObjectCustomSerializerAccess Owner { get; private set; }

    protected abstract IEnumerable<DependencyProperty> SerializableProperties { get; }

    public virtual object Content
    {
      get
      {
        return (object) null;
      }
    }

    protected DependencyObjectCustomSerializer(IDependencyObjectCustomSerializerAccess owner)
    {
      this.Owner = owner;
    }

    protected bool ShouldSerializeProperty(DependencyProperty property, out object value)
    {
      value = (object) null;
      if (!this.Owner.ShouldSerializeProperty(property))
        return false;
      object obj = this.Owner.GetValue(property);
      if (DependencyObjectCustomSerializer.IsDefaultValue(property, obj))
        return false;
      value = obj;
      return true;
    }

    private void SerializeNonDefaultProperties(XmlWriter writer)
    {
      foreach (DependencyProperty property in this.SerializableProperties)
      {
        object obj = (object) null;
        if (this.ShouldSerializeProperty(property, out obj))
        {
          string str = Convert.ToString(obj, (IFormatProvider) CultureInfo.InvariantCulture);
          writer.WriteAttributeString(property.Name, str);
        }
      }
    }

    private static bool IsDefaultValue(DependencyProperty dp, object value)
    {
      object defaultValue = dp.DefaultMetadata.DefaultValue;
      if (object.ReferenceEquals(value, defaultValue))
        return true;
      if (value != null)
        return value.Equals(defaultValue);
      return false;
    }

    public virtual void WriteXmlAttributes(XmlWriter writer)
    {
      this.SerializeNonDefaultProperties(writer);
    }

    public virtual IEnumerable<KeyValuePair<string, object>> GetNonContentPropertyValues()
    {
      yield break;
    }
  }
}

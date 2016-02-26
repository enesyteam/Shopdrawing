// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.ConfigurationObjectCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Expression.Framework.Configuration
{
  internal class ConfigurationObjectCollection : IConfigurationObjectCollection, ICollection, IEnumerable
  {
    private List<IConfigurationObject> configurations = new List<IConfigurationObject>();

    public IConfigurationObject this[int index]
    {
      get
      {
        return this.configurations[index];
      }
      set
      {
        this.configurations[index] = value;
      }
    }

    public int Count
    {
      get
      {
        return this.configurations.Count;
      }
    }

    public object SyncRoot
    {
      get
      {
        return (object) null;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public void Clear()
    {
      this.configurations.Clear();
    }

    public void Add(IConfigurationObject value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.configurations.Add(value);
    }

    public void Insert(int index, IConfigurationObject value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.configurations.Insert(index, value);
    }

    public void Remove(IConfigurationObject value)
    {
      this.configurations.Remove(value);
    }

    public void RemoveAt(int index)
    {
      if (index >= this.configurations.Count)
        throw new IndexOutOfRangeException();
      this.configurations.RemoveAt(index);
    }

    public void CopyTo(Array array, int index)
    {
      IConfigurationObject[] configurationObjectArray = this.configurations.ToArray();
      Array.Copy((Array) configurationObjectArray, 0, array, index, configurationObjectArray.Length);
    }

    public IEnumerator GetEnumerator()
    {
      return (IEnumerator) this.configurations.GetEnumerator();
    }

    internal void Load(XmlReader reader)
    {
      while (reader.IsStartElement("ConfigurationObject"))
      {
        bool isEmptyElement = reader.IsEmptyElement;
        reader.ReadStartElement("ConfigurationObject");
        ConfigurationObject configurationObject = new ConfigurationObject();
        this.Add((IConfigurationObject) configurationObject);
        if (!isEmptyElement)
        {
          configurationObject.Load(reader);
          reader.ReadEndElement();
        }
      }
    }

    internal void Save(XmlWriter writer)
    {
      foreach (ConfigurationObject configurationObject in this.configurations)
      {
        writer.WriteStartElement("ConfigurationObject");
        configurationObject.Save(writer);
        writer.WriteEndElement();
      }
    }
  }
}

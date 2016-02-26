// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.ConfigurationObject
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.Expression.Framework.Configuration
{
  internal class ConfigurationObject : IConfigurationObject, INotifyPropertyChanged
  {
    private SortedList properties = new SortedList();

    public event PropertyChangedEventHandler PropertyChanged;

    public IConfigurationObject CreateConfigurationObject()
    {
      return (IConfigurationObject) new ConfigurationObject();
    }

    public IConfigurationObjectCollection CreateConfigurationObjectCollection()
    {
      return (IConfigurationObjectCollection) new ConfigurationObjectCollection();
    }

    public void Clear()
    {
      string[] strArray = new string[this.properties.Count];
      this.properties.Keys.CopyTo((Array) strArray, 0);
      foreach (string name in strArray)
        this.ClearProperty(name);
    }

    public bool HasProperty(string name)
    {
      return this.properties.Contains((object) name);
    }

    public void ClearProperty(string name)
    {
      if (!this.properties.Contains((object) name))
        return;
      this.properties.Remove((object) name);
      this.OnPropertyChanged(new PropertyChangedEventArgs(name));
    }

    public void SetProperty(string name, object value)
    {
      this.SetProperty(name, value, (object) null);
    }

    public void SetProperty(string name, object value, object defaultValue)
    {
      if (value == null || object.Equals(value, defaultValue))
      {
        this.ClearProperty(name);
      }
      else
      {
        if (this.properties.Contains((object) name) && object.Equals(this.properties[(object) name], value))
          return;
        this.properties[(object) name] = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs(name));
      }
    }

    public object GetProperty(string name)
    {
      return this.GetProperty(name, (object) null);
    }

    public object GetProperty(string name, object defaultValue)
    {
      if (this.properties.Contains((object) name))
        return this.properties[(object) name];
      return defaultValue;
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }

    internal void Load(XmlReader reader)
    {
      while (reader.IsStartElement())
      {
        bool isEmptyElement = reader.IsEmptyElement;
        string attribute1 = reader.GetAttribute("Name");
        if (reader.IsStartElement("PrimitiveObject"))
        {
          string attribute2 = reader.GetAttribute("Type");
          reader.ReadStartElement("PrimitiveObject");
          if (!isEmptyElement)
          {
            string text = reader.ReadString();
            reader.ReadEndElement();
            Type type = !string.IsNullOrEmpty(attribute2) ? Type.GetType(attribute2) : typeof (string);
            if (type != (Type) null)
            {
              TypeConverter converter = TypeDescriptor.GetConverter(type);
              if (converter != null)
              {
                try
                {
                  object obj = converter.ConvertFromString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, text);
                  this.SetProperty(attribute1, obj);
                }
                catch (NotSupportedException ex)
                {
                }
                catch (FormatException ex)
                {
                }
              }
            }
          }
        }
        else if (reader.IsStartElement("ConfigurationObject"))
        {
          reader.ReadStartElement("ConfigurationObject");
          ConfigurationObject configurationObject = (ConfigurationObject) this.CreateConfigurationObject();
          this.SetProperty(attribute1, (object) configurationObject);
          if (!isEmptyElement)
          {
            configurationObject.Load(reader);
            reader.ReadEndElement();
          }
        }
        else
        {
          if (!reader.IsStartElement("ConfigurationObjectCollection"))
            throw new NotSupportedException();
          reader.ReadStartElement("ConfigurationObjectCollection");
          ConfigurationObjectCollection objectCollection = (ConfigurationObjectCollection) this.CreateConfigurationObjectCollection();
          this.SetProperty(attribute1, (object) objectCollection);
          if (!isEmptyElement)
          {
            objectCollection.Load(reader);
            reader.ReadEndElement();
          }
        }
      }
    }

    internal void Save(XmlWriter writer)
    {
      string[] strArray = new string[this.properties.Count];
      this.properties.Keys.CopyTo((Array) strArray, 0);
      foreach (string str in strArray)
      {
        object obj = this.properties[(object) str];
        if (obj is IConfigurationObject)
        {
          writer.WriteStartElement("ConfigurationObject");
          writer.WriteAttributeString("Name", str);
          (obj as ConfigurationObject).Save(writer);
          writer.WriteEndElement();
        }
        else if (obj is IConfigurationObjectCollection)
        {
          writer.WriteStartElement("ConfigurationObjectCollection");
          writer.WriteAttributeString("Name", str);
          (obj as ConfigurationObjectCollection).Save(writer);
          writer.WriteEndElement();
        }
        else
        {
          writer.WriteStartElement("PrimitiveObject");
          writer.WriteAttributeString("Name", str);
          Type type = obj.GetType();
          if (type != typeof (string))
            writer.WriteAttributeString("Type", type.AssemblyQualifiedName);
          string text = TypeDescriptor.GetConverter(type).ConvertToString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, obj);
          writer.WriteString(text);
          writer.WriteEndElement();
        }
      }
    }
  }
}

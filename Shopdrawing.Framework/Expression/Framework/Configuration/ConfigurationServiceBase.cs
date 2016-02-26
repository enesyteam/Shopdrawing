// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.ConfigurationServiceBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Xml;

namespace Microsoft.Expression.Framework.Configuration
{
  public abstract class ConfigurationServiceBase : IConfigurationService
  {
    private SortedList configurations = new SortedList();
    private Version version = new Version(0, 1, 0, 0);

    protected SortedList Configurations
    {
      get
      {
        return this.configurations;
      }
    }

    public IConfigurationObject this[string name]
    {
      get
      {
        if (this.configurations.Contains((object) name))
          return (IConfigurationObject) this.configurations[(object) name];
        ConfigurationObject configurationObject = new ConfigurationObject();
        this.configurations[(object) name] = (object) configurationObject;
        return (IConfigurationObject) configurationObject;
      }
      set
      {
        if (value == null)
        {
          if (!this.configurations.Contains((object) name))
            return;
          this.configurations.Remove((object) name);
        }
        else
          this.configurations[(object) name] = (object) value;
      }
    }

    public virtual string ConfigurationDirectoryPath
    {
      get
      {
        return (string) null;
      }
    }

    protected void LoadInternal(XmlReader reader)
    {
      if (reader == null)
        return;
      using (reader)
      {
        try
        {
          if (!reader.IsStartElement("ConfigurationService") || !(new Version(reader.GetAttribute("Version")) == this.version))
            return;
          reader.ReadStartElement("ConfigurationService");
          while (reader.IsStartElement("ConfigurationObject"))
          {
            string attribute = reader.GetAttribute("Name");
            if (reader.IsEmptyElement)
            {
              reader.ReadStartElement("ConfigurationObject");
            }
            else
            {
              reader.ReadStartElement("ConfigurationObject");
              ((ConfigurationObject) this[attribute]).Load(reader);
              reader.ReadEndElement();
            }
          }
          reader.ReadEndElement();
        }
        catch (XmlException ex)
        {
        }
      }
    }

    protected void SaveInternal(XmlWriter writer)
    {
      if (writer == null)
        return;
      using (writer)
      {
        writer.WriteStartElement("ConfigurationService");
        writer.WriteAttributeString("Version", this.version.ToString());
        string[] strArray = new string[this.configurations.Count];
        this.configurations.Keys.CopyTo((Array) strArray, 0);
        foreach (string str in strArray)
        {
          writer.WriteStartElement("ConfigurationObject");
          writer.WriteAttributeString("Name", str);
          ((ConfigurationObject) this.configurations[(object) str]).Save(writer);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }

    public abstract void Load();

    public abstract void Save();
  }
}

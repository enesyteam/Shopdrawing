// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.ConfigurationService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.IO;
using System.Security;
using System.Xml;

namespace Microsoft.Expression.Framework.Configuration
{
  public sealed class ConfigurationService : ConfigurationServiceBase
  {
    private string configurationPath;

    public override string ConfigurationDirectoryPath
    {
      get
      {
        return this.configurationPath;
      }
    }

    private string ConfigurationFileName
    {
      get
      {
        return Path.Combine(this.ConfigurationDirectoryPath, "user.config");
      }
    }

    public ConfigurationService(string configurationPath)
    {
      this.configurationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), configurationPath);
      this.Load();
    }

    public override void Save()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ConfigurationServiceSave);
      try
      {
        Directory.CreateDirectory(this.ConfigurationDirectoryPath);
        using (FileStream fileStream = File.Create(this.ConfigurationFileName))
          this.SaveInternal(XmlWriter.Create((Stream) fileStream, new XmlWriterSettings()
          {
            Indent = true
          }));
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (IOException ex)
      {
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ConfigurationServiceSave);
    }

    public override void Load()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ConfigurationServiceLoad);
      this.Configurations.Clear();
      try
      {
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(this.ConfigurationFileName))
        {
          using (FileStream fileStream = File.OpenRead(this.ConfigurationFileName))
          {
            XmlReader reader = (XmlReader) null;
            try
            {
              reader = XmlReader.Create((Stream) fileStream);
            }
            catch (SecurityException ex)
            {
            }
            this.LoadInternal(reader);
          }
        }
      }
      catch (IOException ex)
      {
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ConfigurationServiceLoad);
    }
  }
}

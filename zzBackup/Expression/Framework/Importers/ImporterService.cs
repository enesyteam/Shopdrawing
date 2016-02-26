// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Importers.ImporterService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Framework.Importers
{
  public class ImporterService : IImporterService, IImporterDataStore
  {
    private IDictionary<string, ImporterService.ImporterEntry> registeredImporters = (IDictionary<string, ImporterService.ImporterEntry>) new Dictionary<string, ImporterService.ImporterEntry>();
    private string dataStorePath;
    private IConfigurationObject dataStoreConfiguration;

    public object ImporterManager { get; set; }

    public IEnumerable<string> RegisteredImporters
    {
      get
      {
        return (IEnumerable<string>) this.registeredImporters.Keys;
      }
    }

    public IImporter this[string identifier]
    {
      get
      {
        return this.registeredImporters[identifier].Importer;
      }
    }

    public ImporterService(IConfigurationObject dataStoreConfiguration, string dataStorePath)
    {
      this.dataStoreConfiguration = dataStoreConfiguration;
      this.dataStorePath = dataStorePath;
    }

    public void RegisterImporter(string identifier, ImporterCreatorCallback callback)
    {
      this.registeredImporters.Add(identifier, new ImporterService.ImporterEntry(callback));
    }

    public void RegisterImporter(string identifier, IImporter importer)
    {
      this.registeredImporters.Add(identifier, new ImporterService.ImporterEntry(importer));
    }

    public void UnregisterImporter(string identifier)
    {
      this.registeredImporters.Remove(identifier);
    }

    public void SetProperty(string identifier, string propertyName, object propertyValue)
    {
      this.registeredImporters[identifier].SetProperty(propertyName, propertyValue);
    }

    public object GetProperty(string identifier, string propertyName)
    {
      return this.registeredImporters[identifier].GetProperty(propertyName);
    }

    public string GetDataFileEntry(string filePath)
    {
      this.EnsureDataStorePath();
      string path2 = (string) this.dataStoreConfiguration.GetProperty(filePath);
      if (path2 != null)
        return Path.Combine(this.dataStorePath, path2);
      return (string) null;
    }

    public string CreateDataFileEntry(string filePath)
    {
      this.EnsureDataStorePath();
      if (this.GetDataFileEntry(filePath) != null)
        return (string) null;
      string withoutExtension = Path.GetFileNameWithoutExtension(filePath);
      string extension = Path.GetExtension(filePath);
      int num = 0;
      CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
      string format1 = "{0}{1}.msxdata";
      object[] objArray1 = new object[2]
      {
        (object) withoutExtension,
        (object) (extension ?? "")
      };
      string path2;
      CultureInfo invariantCulture2;
      string format2;
      object[] objArray2;
      for (path2 = string.Format((IFormatProvider) invariantCulture1, format1, objArray1); Microsoft.Expression.Framework.Documents.PathHelper.FileExists(Path.Combine(this.dataStorePath, path2)); path2 = string.Format((IFormatProvider) invariantCulture2, format2, objArray2))
      {
        invariantCulture2 = CultureInfo.InvariantCulture;
        format2 = "{0}{1}{2}.msxdata";
        objArray2 = new object[3]
        {
          (object) withoutExtension,
          (object) num++,
          (object) (extension ?? "")
        };
      }
      this.dataStoreConfiguration.SetProperty(filePath, (object) path2);
      return Path.Combine(this.dataStorePath, path2);
    }

    public bool ClearDataFileEntry(string filePath)
    {
      this.EnsureDataStorePath();
      string dataFileEntry = this.GetDataFileEntry(filePath);
      if (dataFileEntry == null)
        return false;
      this.dataStoreConfiguration.ClearProperty(filePath);
      File.Delete(dataFileEntry);
      return true;
    }

    private void EnsureDataStorePath()
    {
      if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(this.dataStorePath))
        return;
      Directory.CreateDirectory(this.dataStorePath);
    }

    private class ImporterEntry
    {
      private IDictionary<string, object> propertyTable = (IDictionary<string, object>) new Dictionary<string, object>();
      private ImporterCreatorCallback callback;
      private IImporter importer;

      public IImporter Importer
      {
        get
        {
          if (this.importer == null && this.callback != null)
          {
            this.importer = this.callback();
            if (this.importer == null)
              return (IImporter) null;
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) this.propertyTable)
              this.importer.SetProperty(keyValuePair.Key, keyValuePair.Value);
            this.propertyTable.Clear();
          }
          return this.importer;
        }
      }

      public ImporterEntry(ImporterCreatorCallback callback)
      {
        this.callback = callback;
      }

      public ImporterEntry(IImporter importer)
      {
        this.importer = importer;
      }

      public void SetProperty(string propertyName, object propertyValue)
      {
        if (this.importer == null)
          this.propertyTable[propertyName] = propertyValue;
        else
          this.importer.SetProperty(propertyName, propertyValue);
      }

      public object GetProperty(string propertyName)
      {
        if (this.importer == null && this.propertyTable.ContainsKey(propertyName))
          return this.propertyTable[propertyName];
        return this.importer.GetProperty(propertyName);
      }
    }
  }
}

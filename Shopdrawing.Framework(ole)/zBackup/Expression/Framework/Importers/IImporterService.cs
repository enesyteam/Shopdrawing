// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Importers.IImporterService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Importers
{
  public interface IImporterService
  {
    IEnumerable<string> RegisteredImporters { get; }

    IImporter this[string identifier] { get; }

    object ImporterManager { get; set; }

    void RegisterImporter(string identifier, ImporterCreatorCallback callback);

    void RegisterImporter(string identifier, IImporter importer);

    void UnregisterImporter(string identifier);

    void SetProperty(string identifier, string propertyName, object propertyValue);

    object GetProperty(string identifier, string propertyName);
  }
}

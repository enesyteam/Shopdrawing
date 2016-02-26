// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Importers.Importer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.Framework.Importers
{
  public abstract class Importer : IImporter
  {
    private IDictionary<string, object> propertyTable = (IDictionary<string, object>) new Dictionary<string, object>();

    public abstract bool IsSupported(Type contextType, DocumentReference inputFile);

    public abstract bool Import(object importerContext, DocumentReference inputFile, Stream importData);

    public virtual void SetProperty(string propertyName, object propertyValue)
    {
      this.propertyTable[propertyName] = propertyValue;
    }

    public virtual object GetProperty(string propertyName)
    {
      try
      {
        return this.propertyTable[propertyName];
      }
      catch (KeyNotFoundException ex)
      {
      }
      return (object) null;
    }
  }
}

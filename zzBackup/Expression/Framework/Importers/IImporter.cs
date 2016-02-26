// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Importers.IImporter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;

namespace Microsoft.Expression.Framework.Importers
{
  public interface IImporter
  {
    bool IsSupported(Type contextType, DocumentReference inputFile);

    bool Import(object importerContext, DocumentReference inputFile, Stream importData);

    void SetProperty(string propertyName, object propertyValue);

    object GetProperty(string propertyName);
  }
}

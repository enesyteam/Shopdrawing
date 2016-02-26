// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.DataSetContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class DataSetContext
  {
    public static readonly DataSetContext SampleData = new DataSetContext(DataSetType.SampleDataSet, "SampleData", "Expression.Blend.SampleData.", "_.SampleData.v");
    public static readonly DataSetContext DataStore = new DataSetContext(DataSetType.DataStoreSet, "DataStore", "Expression.Blend.DataStore.", "_.DataStore.v");

    public DataSetType DataSetType { get; private set; }

    public string DataRootFolder { get; private set; }

    public string ClrNamespacePrefixBase { get; private set; }

    public string DesignTimeClrNamespacePrefix { get; private set; }

    public DataSetContext(DataSetType dataSetType, string dataRootFolder, string clrNamespacePrefixBase, string designTimeClrNamespacePrefix)
    {
      this.DataSetType = dataSetType;
      this.DataRootFolder = dataRootFolder;
      this.ClrNamespacePrefixBase = clrNamespacePrefixBase;
      this.DesignTimeClrNamespacePrefix = designTimeClrNamespacePrefix;
    }

    public static bool DesignTimeClrNamespacePrefixMatchesDataSetContext(string fullName)
    {
      if (!fullName.StartsWith(DataSetContext.SampleData.DesignTimeClrNamespacePrefix, StringComparison.Ordinal))
        return fullName.StartsWith(DataSetContext.DataStore.DesignTimeClrNamespacePrefix, StringComparison.Ordinal);
      return true;
    }

    public static string GetClrNamespacePrefix(DataSetType dataSetType, string rootNamespace)
    {
      if (string.IsNullOrEmpty(rootNamespace))
        return DataSetContext.ClrNamespacePrefixBaseFromDataSetType(dataSetType);
      return rootNamespace + "." + DataSetContext.ClrNamespacePrefixBaseFromDataSetType(dataSetType);
    }

    public static string ClrNamespacePrefixBaseFromDataSetType(DataSetType dataSetType)
    {
      if (dataSetType == DataSetType.SampleDataSet)
        return DataSetContext.SampleData.ClrNamespacePrefixBase;
      if (dataSetType == DataSetType.DataStoreSet)
        return DataSetContext.DataStore.ClrNamespacePrefixBase;
      return (string) null;
    }

    public string GetClrNamespacePrefix(string rootNamespace)
    {
      return DataSetContext.GetClrNamespacePrefix(this.DataSetType, rootNamespace);
    }
  }
}

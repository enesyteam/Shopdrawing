// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.SampleDataAwareNamespaceTypeResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using System;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public class SampleDataAwareNamespaceTypeResolver : IXmlNamespaceTypeResolver
  {
    private static string sampleDataNamespacePrefix = "SampleData";
    private static string dataStoreNamespacePrefix = "DataStore";
    private string sampleDataNamespace;
    private string dataStoreNamespace;
    private IXmlNamespaceTypeResolver actualNamespaceResolver;

    public SampleDataAwareNamespaceTypeResolver(IXmlNamespaceTypeResolver actualNamespaceResolver, string rootNamespace)
    {
      this.actualNamespaceResolver = actualNamespaceResolver;
      this.sampleDataNamespace = DataSetContext.GetClrNamespacePrefix(DataSetType.SampleDataSet, rootNamespace);
      this.dataStoreNamespace = DataSetContext.GetClrNamespacePrefix(DataSetType.DataStoreSet, rootNamespace);
    }

    public bool Contains(IXmlNamespace xmlNamespace)
    {
      return this.actualNamespaceResolver.Contains(xmlNamespace);
    }

    public string GetDefaultPrefix(IXmlNamespace xmlNamespace)
    {
      return this.actualNamespaceResolver.GetDefaultPrefix(xmlNamespace);
    }

    public IXmlNamespace GetNamespace(IAssembly assembly, Type type)
    {
      return this.actualNamespaceResolver.GetNamespace(assembly, type);
    }

    public IXmlNamespace GetNamespace(IAssembly assembly, string clrNamespace)
    {
      return this.actualNamespaceResolver.GetNamespace(assembly, clrNamespace);
    }

    public IType GetType(IXmlNamespace xmlNamespace, string typeName)
    {
      return this.actualNamespaceResolver.GetType(xmlNamespace, typeName);
    }

    public string GetClrNamespacePrefixWorkaround(IAssembly assemblyReference, string clrNamespace)
    {
      if (clrNamespace.StartsWith(this.sampleDataNamespace, StringComparison.Ordinal))
        return SampleDataAwareNamespaceTypeResolver.sampleDataNamespacePrefix;
      if (clrNamespace.StartsWith(this.dataStoreNamespace, StringComparison.Ordinal))
        return SampleDataAwareNamespaceTypeResolver.dataStoreNamespacePrefix;
      return this.actualNamespaceResolver.GetClrNamespacePrefixWorkaround(assemblyReference, clrNamespace);
    }
  }
}

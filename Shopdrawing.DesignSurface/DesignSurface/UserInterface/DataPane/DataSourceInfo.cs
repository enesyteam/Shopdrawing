// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSourceInfo
  {
    public DocumentNode SourceNode { get; private set; }

    public string Path { get; private set; }

    public DataSourceCategory Category { get; private set; }

    public bool IsValidWithSource
    {
      get
      {
        return this.Category != DataSourceCategory.Invalid && this.SourceNode != null;
      }
    }

    public string SourceName
    {
      get
      {
        return DataSourceInfo.GetDataSourceName(this.SourceNode);
      }
    }

    public IType DataSourceType
    {
      get
      {
        IType type = (IType) null;
        if (this.SourceNode != null && this.Category != DataSourceCategory.Invalid)
          type = DataContextHelper.GetDataType(this.SourceNode);
        return type;
      }
    }

    public IType SourceType
    {
      get
      {
        return DataContextHelper.GetDataType(this.SourceNode);
      }
    }

    public DataSourceInfo(DataSchemaNodePath bindingPath)
    {
      this.SourceNode = bindingPath.Schema.DataSource.DocumentNode;
      if (this.SourceNode == null)
      {
        this.Category = DataSourceCategory.Invalid;
      }
      else
      {
        this.Category = bindingPath.Schema is XmlSchema ? DataSourceCategory.Xml : DataSourceCategory.Clr;
        this.Path = bindingPath.Path;
      }
    }

    public DataSourceInfo(DocumentNode sourceNode, string path, DataSourceCategory category)
    {
      this.SourceNode = sourceNode;
      this.Path = path;
      this.Category = category;
    }

    public DataSourceMatchCriteria CompareSources(DataSourceInfo other)
    {
      if (this == other)
        return DataSourceMatchCriteria.Exact;
      if (this.Category == DataSourceCategory.Invalid || this.Category != other.Category || !DesignDataHelper.CompareDataSources(this.SourceNode, other.SourceNode))
        return DataSourceMatchCriteria.Ignore;
      DataSourceMatchCriteria sourceMatchCriteria = DataSourceMatchCriteria.Ignore;
      if (this.Category == DataSourceCategory.Clr)
        sourceMatchCriteria = DataSourceInfo.CompareClrPaths(this.Path, other.Path);
      else if (this.Category == DataSourceCategory.Xml)
        sourceMatchCriteria = DataSourceInfo.CompareXmlPaths(XmlSchema.NormalizeXPath(this.Path), XmlSchema.NormalizeXPath(other.Path));
      return sourceMatchCriteria;
    }

    private static DataSourceMatchCriteria CompareXmlPaths(string thisPath, string otherPath)
    {
      if (string.IsNullOrEmpty(thisPath))
        return string.IsNullOrEmpty(otherPath) ? DataSourceMatchCriteria.Exact : DataSourceMatchCriteria.Compatible;
      if (string.IsNullOrEmpty(otherPath) || thisPath.Length > otherPath.Length)
        return DataSourceMatchCriteria.Ignore;
      if (thisPath.Length == otherPath.Length)
        return !(thisPath == otherPath) ? DataSourceMatchCriteria.Ignore : DataSourceMatchCriteria.Exact;
      return otherPath.StartsWith(thisPath, StringComparison.Ordinal) && (int) otherPath[thisPath.Length] == 47 ? DataSourceMatchCriteria.Compatible : DataSourceMatchCriteria.Ignore;
    }

    private static DataSourceMatchCriteria CompareClrPaths(string thisPath, string otherPath)
    {
      if (string.IsNullOrEmpty(thisPath))
        return string.IsNullOrEmpty(otherPath) ? DataSourceMatchCriteria.Exact : DataSourceMatchCriteria.Compatible;
      if (string.IsNullOrEmpty(otherPath))
        return DataSourceMatchCriteria.Ignore;
      IList<ClrPathPart> list1 = ClrPropertyPathHelper.SplitPath(thisPath);
      IList<ClrPathPart> list2 = ClrPropertyPathHelper.SplitPath(otherPath);
      if (list1 == null || list2 == null || list1.Count > list2.Count)
        return DataSourceMatchCriteria.Ignore;
      for (int index = 0; index < list1.Count; ++index)
      {
        ClrPathPart clrPathPart1 = list1[index];
        ClrPathPart clrPathPart2 = list2[index];
        if (clrPathPart1.Category == ClrPathPartCategory.PropertyName != (clrPathPart2.Category == ClrPathPartCategory.PropertyName) || clrPathPart1.Category == ClrPathPartCategory.PropertyName && clrPathPart1.Path != clrPathPart2.Path)
          return DataSourceMatchCriteria.Ignore;
      }
      return list1.Count < list2.Count ? DataSourceMatchCriteria.Compatible : DataSourceMatchCriteria.Exact;
    }

    public static string GetDataSourceName(DocumentNode dataSourceNode)
    {
      string str = (string) null;
      if (dataSourceNode != null)
      {
        DocumentCompositeNode parent = dataSourceNode.Parent;
        if (parent != null && PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) parent.Type))
        {
          DocumentNode resourceEntryKey = DocumentNodeHelper.GetResourceEntryKey(parent);
          if (resourceEntryKey != null)
            str = DocumentPrimitiveNode.GetValueAsString(resourceEntryKey);
        }
        if (string.IsNullOrEmpty(str))
          str = dataSourceNode.Type.Name + (object) " " + (string) (object) dataSourceNode.GetHashCode();
      }
      return str ?? string.Empty;
    }

    public override string ToString()
    {
      string str1 = string.Empty;
      string str2;
      if (this.SourceNode != null)
        str2 = str1 + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0} {1}]", new object[2]
        {
          (object) this.Category,
          (object) this.SourceName
        });
      else
        str2 = str1 + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", new object[1]
        {
          (object) this.Category
        });
      if (!string.IsNullOrEmpty(this.Path))
        str2 = str2 + ": " + this.Path;
      return str2;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.RawDataSourceInfoBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class RawDataSourceInfoBase
  {
    private bool isValid = true;

    public abstract DocumentNode SourceNode { get; }

    public abstract string ClrPath { get; }

    public abstract string NormalizedClrPath { get; }

    public abstract RawDataSourceInfo NormalizedDataSource { get; }

    public string XmlPath { get; set; }

    public IType SourceType
    {
      get
      {
        return DataContextHelper.GetDataType(this.SourceNode);
      }
    }

    public string SourceName
    {
      get
      {
        return DataSourceInfo.GetDataSourceName(this.SourceNode);
      }
    }

    public bool HasSource
    {
      get
      {
        return this.SourceNode != null;
      }
    }

    public bool HasClrPath
    {
      get
      {
        return !string.IsNullOrEmpty(this.ClrPath);
      }
    }

    public bool HasXmlPath
    {
      get
      {
        return !string.IsNullOrEmpty(this.XmlPath);
      }
    }

    public DataSourceInfo DataSourceInfo
    {
      get
      {
        string path = (string) null;
        DataSourceCategory category;
        if (!this.IsValid)
          category = DataSourceCategory.Invalid;
        else if (this.IsValidClr)
        {
          category = DataSourceCategory.Clr;
          path = this.NormalizedClrPath;
        }
        else
        {
          category = DataSourceCategory.Xml;
          path = XmlSchema.NormalizeXPath(this.XmlPath);
        }
        return new DataSourceInfo(this.SourceNode, path, category);
      }
    }

    public bool IsValid
    {
      get
      {
        return this.isValid && (string.IsNullOrEmpty(this.ClrPath) || string.IsNullOrEmpty(this.XmlPath));
      }
    }

    public bool IsValidClr
    {
      get
      {
        if (!this.IsValid || !string.IsNullOrEmpty(this.XmlPath))
          return false;
        if (this.SourceNode != null)
          return !PlatformTypes.XmlDataProvider.IsAssignableFrom((ITypeId) this.SourceNode.Type);
        return true;
      }
    }

    public bool IsValidXml
    {
      get
      {
        if (!this.IsValid || !string.IsNullOrEmpty(this.ClrPath))
          return false;
        if (this.SourceNode != null)
          return PlatformTypes.XmlDataProvider.IsAssignableFrom((ITypeId) this.SourceNode.Type);
        return true;
      }
    }

    public abstract void AppendClrPath(string path);

    public abstract void AppendIndexStep();

    public abstract RawDataSourceInfoBase CombineWith(RawDataSourceInfoBase localSource);

    public abstract RawDataSourceInfoBase Clone();

    public void SetInvalid()
    {
      this.isValid = false;
    }
  }
}

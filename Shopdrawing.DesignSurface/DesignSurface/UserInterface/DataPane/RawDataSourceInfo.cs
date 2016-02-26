// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.RawDataSourceInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class RawDataSourceInfo : RawDataSourceInfoBase
  {
    private static RawDataSourceInfo invalid;
    private DocumentNode sourceNode;
    private int pendingIndexSteps;
    private string clrPath;

    public static RawDataSourceInfo NewEmpty
    {
      get
      {
        return new RawDataSourceInfo((DocumentNode) null, (string) null);
      }
    }

    public static RawDataSourceInfo Invalid
    {
      get
      {
        if (RawDataSourceInfo.invalid == null)
        {
          RawDataSourceInfo.invalid = new RawDataSourceInfo((DocumentNode) null, (string) null);
          RawDataSourceInfo.invalid.SetInvalid();
        }
        return RawDataSourceInfo.invalid;
      }
    }

    public override DocumentNode SourceNode
    {
      get
      {
        return this.sourceNode;
      }
    }

    public override string ClrPath
    {
      get
      {
        return this.clrPath;
      }
    }

    public override string NormalizedClrPath
    {
      get
      {
        return this.clrPath;
      }
    }

    public override RawDataSourceInfo NormalizedDataSource
    {
      get
      {
        return this;
      }
    }

    public bool IsEmpty
    {
      get
      {
        if (!this.HasClrPath && !this.HasXmlPath)
          return !this.HasSource;
        return false;
      }
    }

    public RawDataSourceInfo(DocumentNode sourceNode, string clrPath)
    {
      this.sourceNode = sourceNode;
      this.clrPath = clrPath;
    }

    public override RawDataSourceInfoBase Clone()
    {
      RawDataSourceInfo rawDataSourceInfo = new RawDataSourceInfo(this.sourceNode, this.clrPath);
      rawDataSourceInfo.XmlPath = this.XmlPath;
      return (RawDataSourceInfoBase) rawDataSourceInfo;
    }

    public override void AppendIndexStep()
    {
      if (this.HasClrPath)
      {
        this.AppendClrPath(DataSchemaNode.IndexNodePath);
      }
      else
      {
        if (this.HasXmlPath)
          return;
        if (this.SourceNode != null)
        {
          if (PlatformTypes.XmlDataProvider.IsAssignableFrom((ITypeId) this.SourceNode.Type))
            return;
          this.AppendClrPath(DataSchemaNode.IndexNodePath);
        }
        else
          ++this.pendingIndexSteps;
      }
    }

    public override void AppendClrPath(string path)
    {
      this.clrPath = ClrPropertyPathHelper.CombinePaths(this.clrPath, path);
    }

    public override RawDataSourceInfoBase CombineWith(RawDataSourceInfoBase localSource)
    {
      if (localSource == null)
        return (RawDataSourceInfoBase) this;
      if (!localSource.IsValid || localSource.SourceNode != null)
        return localSource;
      if (!this.IsValid)
        return (RawDataSourceInfoBase) this;
      RawDataSourceInfo rawDataSourceInfo1 = localSource as RawDataSourceInfo;
      RawDataSourceInfo rawDataSourceInfo2 = new RawDataSourceInfo(this.SourceNode, ClrPropertyPathHelper.CombinePaths(this.ClrPath, rawDataSourceInfo1.ClrPath));
      rawDataSourceInfo2.XmlPath = XmlSchema.CombineXPaths(this.XmlPath, rawDataSourceInfo1.XmlPath);
      for (int index = 0; index < rawDataSourceInfo1.pendingIndexSteps; ++index)
        rawDataSourceInfo2.AppendIndexStep();
      return (RawDataSourceInfoBase) rawDataSourceInfo2;
    }

    public override string ToString()
    {
      if (!this.IsValid)
        return "Invalid";
      if (this.IsValidClr)
      {
        if (this.HasSource && this.HasClrPath)
          return this.SourceName + ":" + this.ClrPath;
        if (this.HasSource)
          return "[CLR Source]: " + this.SourceName;
        if (this.HasClrPath)
          return "[Path]: " + this.ClrPath;
        return "Empty";
      }
      if (this.HasSource && this.HasXmlPath)
        return this.SourceName + ":" + this.XmlPath;
      if (this.HasSource)
        return "[XML Source]: " + this.SourceName;
      if (this.HasXmlPath)
        return "[XPath]: " + this.XmlPath;
      return "Empty";
    }
  }
}

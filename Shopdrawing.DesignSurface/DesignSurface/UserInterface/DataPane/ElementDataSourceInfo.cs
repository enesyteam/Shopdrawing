// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ElementDataSourceInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ElementDataSourceInfo : RawDataSourceInfoBase
  {
    private static ElementDataSourceInfo invalid;
    private string clrPath;

    public static ElementDataSourceInfo Invalid
    {
      get
      {
        if (ElementDataSourceInfo.invalid == null)
        {
          ElementDataSourceInfo.invalid = new ElementDataSourceInfo();
          ElementDataSourceInfo.invalid.SetInvalid();
        }
        return ElementDataSourceInfo.invalid;
      }
    }

    public override string ClrPath
    {
      get
      {
        return this.clrPath;
      }
    }

    public DocumentCompositeNode TargetElement { get; private set; }

    public IProperty TargetProperty { get; set; }

    public string TargetPropertyName { get; private set; }

    public string PathSuffix { get; private set; }

    public RawDataSourceInfoBase TargetDataSource { get; set; }

    public bool IsCollectionItem { get; set; }

    public override DocumentNode SourceNode
    {
      get
      {
        if (this.TargetDataSource != null)
          return this.TargetDataSource.SourceNode;
        if (this.TargetProperty == null)
          return (DocumentNode) this.TargetElement;
        return (DocumentNode) null;
      }
    }

    public ElementDataSourceInfo RootElementBinding
    {
      get
      {
        ElementDataSourceInfo elementDataSourceInfo1 = this;
        for (ElementDataSourceInfo elementDataSourceInfo2 = this; elementDataSourceInfo2 != null; elementDataSourceInfo2 = elementDataSourceInfo2.TargetDataSource as ElementDataSourceInfo)
          elementDataSourceInfo1 = elementDataSourceInfo2;
        return elementDataSourceInfo1;
      }
    }

    public DocumentCompositeNode RootElement
    {
      get
      {
        return this.RootElementBinding.TargetElement;
      }
    }

    public IProperty RootTargetProperty
    {
      get
      {
        return this.RootElementBinding.TargetProperty;
      }
    }

    public RawDataSourceInfo RootTargetDataSource
    {
      get
      {
        return (RawDataSourceInfo) this.RootElementBinding.TargetDataSource;
      }
    }

    public string SubstitutePropertyPath
    {
      get
      {
        string inheritedPath = this.TargetDataSource != null ? this.TargetDataSource.NormalizedClrPath : (string) null;
        if (this.IsCollectionItem)
          inheritedPath = ClrPropertyPathHelper.CombinePaths(inheritedPath, DataSchemaNode.IndexNodePath);
        return inheritedPath;
      }
    }

    public override string NormalizedClrPath
    {
      get
      {
        return this.TargetProperty == null ? ClrPropertyPathHelper.CombinePaths(this.TargetPropertyName, this.PathSuffix) : ClrPropertyPathHelper.CombinePaths(this.SubstitutePropertyPath, this.PathSuffix);
      }
    }

    public override RawDataSourceInfo NormalizedDataSource
    {
      get
      {
        ElementDataSourceInfo rootElementBinding = this.RootElementBinding;
        RawDataSourceInfo rawDataSourceInfo = rootElementBinding.TargetProperty != null ? new RawDataSourceInfo(rootElementBinding.SourceNode, this.NormalizedClrPath) : new RawDataSourceInfo((DocumentNode) rootElementBinding.TargetElement, this.NormalizedClrPath);
        rawDataSourceInfo.XmlPath = this.XmlPath;
        if (!this.IsValid)
          rawDataSourceInfo.SetInvalid();
        return rawDataSourceInfo;
      }
    }

    private ElementDataSourceInfo()
    {
    }

    public ElementDataSourceInfo(DocumentCompositeNode namedElement, string path)
    {
      this.TargetElement = namedElement;
      this.InitializePath(path);
    }

    public override RawDataSourceInfoBase Clone()
    {
      ElementDataSourceInfo elementDataSourceInfo = new ElementDataSourceInfo(this.TargetElement, (string) null);
      elementDataSourceInfo.clrPath = this.clrPath;
      elementDataSourceInfo.TargetProperty = this.TargetProperty;
      elementDataSourceInfo.TargetPropertyName = this.TargetPropertyName;
      elementDataSourceInfo.PathSuffix = this.PathSuffix;
      elementDataSourceInfo.IsCollectionItem = this.IsCollectionItem;
      elementDataSourceInfo.XmlPath = this.XmlPath;
      if (this.TargetDataSource != null)
        elementDataSourceInfo.TargetDataSource = this.TargetDataSource.Clone();
      return (RawDataSourceInfoBase) elementDataSourceInfo;
    }

    private void InitializePath(string path)
    {
      this.clrPath = path;
      string[] strArray = ClrPropertyPathHelper.SplitAtFirstProperty(this.clrPath);
      if (strArray == null)
      {
        this.SetInvalid();
      }
      else
      {
        this.TargetPropertyName = strArray[0];
        this.PathSuffix = strArray[1];
        this.TargetProperty = this.TargetElement.Type.GetMember(MemberType.LocalProperty, this.TargetPropertyName, MemberAccessTypes.Public) as IProperty;
        if (this.TargetProperty != null || string.IsNullOrEmpty(this.TargetPropertyName))
          return;
        this.SetInvalid();
      }
    }

    public override void AppendIndexStep()
    {
      this.AppendClrPath(DataSchemaNode.IndexNodePath);
    }

    public override void AppendClrPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return;
      if (this.TargetProperty == null)
      {
        this.InitializePath(path);
      }
      else
      {
        this.PathSuffix = ClrPropertyPathHelper.CombinePaths(this.PathSuffix, path);
        this.clrPath = ClrPropertyPathHelper.CombinePaths(this.clrPath, path);
      }
    }

    public override RawDataSourceInfoBase CombineWith(RawDataSourceInfoBase localSource)
    {
      if (localSource == null)
        return (RawDataSourceInfoBase) this;
      if (!this.IsValid || !localSource.IsValid || (localSource.SourceNode != null || localSource is ElementDataSourceInfo))
        return localSource;
      RawDataSourceInfo rawDataSourceInfo = (RawDataSourceInfo) localSource;
      if (rawDataSourceInfo.IsEmpty)
        return (RawDataSourceInfoBase) this;
      ElementDataSourceInfo elementDataSourceInfo = (ElementDataSourceInfo) this.Clone();
      elementDataSourceInfo.AppendClrPath(rawDataSourceInfo.ClrPath);
      elementDataSourceInfo.XmlPath = localSource.XmlPath;
      return (RawDataSourceInfoBase) elementDataSourceInfo;
    }

    public override string ToString()
    {
      string str1 = this.TargetElement.ToString();
      if (!string.IsNullOrEmpty(this.TargetPropertyName))
        str1 = str1 + "." + this.ClrPath;
      if (this.TargetDataSource != null)
      {
        string str2 = str1 + " -> ";
        if (this.TargetDataSource.SourceNode != null)
          str2 = str2 + this.SourceName + ":";
        str1 = str2 + this.NormalizedClrPath;
      }
      return str1;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System.IO;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class DataSourceNode : NotifyingObject
  {
    private DataSourceHost host;
    private DocumentNode documentNode;

    public abstract ISchema Schema { get; }

    public DataSourceHost Host
    {
      get
      {
        return this.host;
      }
      set
      {
        this.host = value;
        this.OnPropertyChanged("Host");
      }
    }

    public virtual string ErrorMessage
    {
      get
      {
        return (string) null;
      }
    }

    public bool IsSampleDataSource
    {
      get
      {
        return this.SampleData != null;
      }
    }

    public bool IsDataStoreSource
    {
      get
      {
        if (this.SampleData != null)
          return this.SampleData.Context.DataSetType == DataSetType.DataStoreSet;
        return false;
      }
    }

    public bool IsDesignData { get; private set; }

    public SampleDataSet SampleData
    {
      get
      {
        if (this.IsDesignData)
          return (SampleDataSet) null;
        SampleNonBasicType sampleNonBasicType = DataContextHelper.GetDataType(this.documentNode) as SampleNonBasicType;
        if (sampleNonBasicType != null)
          return sampleNonBasicType.DeclaringDataSet;
        return (SampleDataSet) null;
      }
    }

    public DocumentNode DocumentNode
    {
      get
      {
        return this.documentNode;
      }
    }

    public string UniqueId
    {
      get
      {
        return this.host == null ? this.Name : this.host.Name + "/" + this.Name;
      }
    }

    public DocumentNode ResourceKey
    {
      get
      {
        if (this.documentNode.Parent != null)
          return ResourceNodeHelper.GetResourceEntryKey(this.documentNode.Parent);
        return (DocumentNode) null;
      }
    }

    public string Name
    {
      get
      {
        DocumentNode resourceKey = this.ResourceKey;
        if (resourceKey != null)
          return DocumentPrimitiveNode.GetValueAsString(resourceKey);
        IProjectItem designDataFile = DesignDataHelper.GetDesignDataFile(this.DocumentNode);
        if (designDataFile != null)
          return Path.GetFileNameWithoutExtension(designDataFile.DocumentReference.Path);
        IType dataType = DataContextHelper.GetDataType(this.DocumentNode);
        if (dataType != null)
          return dataType.Name;
        return (string) null;
      }
    }

    public DataSourceNode(DocumentNode documentNode)
    {
      this.documentNode = documentNode;
      this.IsDesignData = DesignDataHelper.GetDesignDataFile(documentNode) != null;
    }

    public abstract SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty);

    public void OnNameChanged()
    {
      this.OnPropertyChanged("Name");
      this.OnPropertyChanged("UniqueId");
    }

    internal bool UpdateDocumentNode(DocumentNode node)
    {
      if (this.documentNode == node)
        return true;
      if (node == null || this.documentNode == null || (this.documentNode == null || !this.documentNode.Type.Equals((object) node.Type)))
        return false;
      this.documentNode = node;
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSchemaNodePath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Diagnostics;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSchemaNodePath
  {
    private ISchema relativeSchema;

    public ISchema Schema { get; private set; }

    public DataSchemaNode Node { get; private set; }

    public string Path
    {
      get
      {
        if (this.Schema != null)
          return this.Schema.GetPath(this);
        return (string) null;
      }
    }

    public ISchema RelativeSchema
    {
      get
      {
        if (this.relativeSchema == null)
          this.relativeSchema = this.Schema.MakeRelativeToNode(this.Node);
        return this.relativeSchema;
      }
    }

    public DataSchemaNodePath AbsolutePath
    {
      get
      {
        if (this.Schema.Root == this.Schema.AbsoluteRoot)
          return this;
        return new DataSchemaNodePath(this.Schema.MakeRelativeToNode((DataSchemaNode) null), this.Node);
      }
    }

    public IType Type
    {
      get
      {
        IType type = (IType) null;
        if (this.Schema.DataSource != null && this.Schema.DataSource.DocumentNode != null)
          type = this.Node.ResolveType(this.Schema.DataSource.DocumentNode.TypeResolver);
        return type;
      }
    }

    public int CollectionDepth
    {
      get
      {
        return this.Schema.GetCollectionDepth(this.Node);
      }
    }

    public SchemaNodeTypes NodeType
    {
      get
      {
        return this.Node.NodeType;
      }
    }

    public bool IsCollection
    {
      get
      {
        return this.Node.IsCollection;
      }
    }

    public bool IsCollectionItem
    {
      get
      {
        return this.Node.IsCollectionItem;
      }
    }

    public bool IsProperty
    {
      get
      {
        return this.Node.IsProperty;
      }
    }

    public bool IsMethod
    {
      get
      {
        return this.Node.IsMethod;
      }
    }

    public bool IsHierarchicalCollection
    {
      get
      {
        bool flag = false;
        if (this.Schema.DataSource != null && this.Schema.DataSource.DocumentNode != null)
          flag = this.Node.IsHierarchicalCollection(this.Schema.DataSource.DocumentNode.TypeResolver);
        return flag;
      }
    }

    public DataSchemaNode EffectiveCollectionNode
    {
      get
      {
        return this.Schema.GetEffectiveCollectionNode(this.Node);
      }
    }

    public DataSchemaNode EffectiveCollectionItemNode
    {
      get
      {
        DataSchemaNode dataSchemaNode = this.IsCollectionItem ? this.Node : this.EffectiveCollectionNode;
        if (dataSchemaNode == null)
          return (DataSchemaNode) null;
        for (DataSchemaNode collectionItem = dataSchemaNode.CollectionItem; collectionItem != null; collectionItem = collectionItem.CollectionItem)
          dataSchemaNode = collectionItem;
        return dataSchemaNode;
      }
    }

    public DataSchemaNodePath(ISchema schema, DataSchemaNode endNode)
    {
      this.Schema = schema;
      this.Node = endNode;
    }

    public DataSchemaNodePath GetExtendedPath(DataSchemaNode node)
    {
      return new DataSchemaNodePath(this.Schema, node);
    }

    [Conditional("DEBUG")]
    private void Verify()
    {
      if (this.Schema.Root == null)
        return;
      bool flag = false;
      for (DataSchemaNode dataSchemaNode = this.Node; dataSchemaNode != null && !flag; dataSchemaNode = dataSchemaNode.Parent)
        flag = dataSchemaNode == this.Schema.Root;
    }

    public DataSchemaNodePath GetRelativeNodePath(DataSchemaNodePath childNodePath)
    {
      return this.GetRelativeNodePath(childNodePath.Node);
    }

    public DataSchemaNodePath GetRelativeNodePath(DataSchemaNode childNode)
    {
      return new DataSchemaNodePath(this.RelativeSchema, childNode);
    }

    [Conditional("DEBUG")]
    private void VerifyNodesRelated(DataSchemaNode parentNode, DataSchemaNode childNode)
    {
      DataSchemaNode dataSchemaNode = childNode;
      while (dataSchemaNode != null && dataSchemaNode != parentNode)
        dataSchemaNode = dataSchemaNode.Parent;
    }

    public bool IsSubpathOf(DataSchemaNodePath superPath)
    {
      if (this.Schema.Root != superPath.Schema.Root || this.Schema.Root == null || superPath.Schema.Root == null)
        return false;
      DataSchemaNode parent = superPath.Schema.Root.Parent;
      for (DataSchemaNode dataSchemaNode = superPath.Node; dataSchemaNode != parent; dataSchemaNode = dataSchemaNode.Parent)
      {
        if (dataSchemaNode == this.Node)
          return true;
      }
      return false;
    }

    public override string ToString()
    {
      string str = this.Path;
      if (string.IsNullOrEmpty(str))
        str = "<Empty>";
      return str;
    }
  }
}

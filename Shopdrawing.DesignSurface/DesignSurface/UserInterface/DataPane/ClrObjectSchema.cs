// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrObjectSchema
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class ClrObjectSchema : ISchema, INotifyPropertyChanged, IDataSchemaNodeDelayLoader
  {
    private DataSourceNode dataSource;
    private DataSchemaNode root;

    public DataSchemaNode Root
    {
      get
      {
        return this.root;
      }
    }

    public DataSchemaNode AbsoluteRoot
    {
      get
      {
        DataSchemaNode dataSchemaNode = this.root;
        for (DataSchemaNode parent = this.root.Parent; parent != null; parent = parent.Parent)
          dataSchemaNode = parent;
        return dataSchemaNode;
      }
    }

    public string PathDescription
    {
      get
      {
        return StringTable.UseCustomPropertyPathDescription;
      }
    }

    public DataSourceNode DataSource
    {
      get
      {
        return this.dataSource;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ClrObjectSchema(Type type, DocumentNode dataSourceNode)
    {
      SchemaNodeTypes nodeType = ClrObjectSchema.IsCollection(type) ? SchemaNodeTypes.Collection : SchemaNodeTypes.Property;
      this.root = new DataSchemaNode(type.Name, type.Name, nodeType, (string) null, type, (IDataSchemaNodeDelayLoader) this);
      this.dataSource = (DataSourceNode) new ClrObjectDataSourceNode(this, dataSourceNode);
      this.OnPropertyChanged("Root");
    }

    private ClrObjectSchema(DataSchemaNode root, DocumentNode dataSourceNode)
    {
      this.root = root;
      this.dataSource = (DataSourceNode) new ClrObjectDataSourceNode(this, dataSourceNode);
    }

    public ISchema MakeRelativeToNode(DataSchemaNode node)
    {
      ClrObjectSchema clrObjectSchema;
      if (node == this.Root)
        clrObjectSchema = this;
      else if (node != null)
      {
        clrObjectSchema = new ClrObjectSchema(node, this.DataSource.DocumentNode);
      }
      else
      {
        DataSchemaNode absoluteRoot = this.AbsoluteRoot;
        clrObjectSchema = this.Root != absoluteRoot ? new ClrObjectSchema(absoluteRoot, this.DataSource.DocumentNode) : this;
      }
      return (ISchema) clrObjectSchema;
    }

    public string GetPath(DataSchemaNodePath nodePath)
    {
      if (nodePath == null || nodePath.Node == this.root)
        return string.Empty;
      DataSchemaNode node = nodePath.Node;
      bool isCollectionItem = node.IsCollectionItem;
      string str = node.PathName;
      for (DataSchemaNode parent = node.Parent; parent != this.Root && parent != null; parent = parent.Parent)
      {
        str = isCollectionItem ? parent.PathName + str : parent.PathName + "." + str;
        isCollectionItem = parent.IsCollectionItem;
      }
      return str;
    }

    public int GetCollectionDepth(DataSchemaNode endNode)
    {
      int num = endNode.IsProperty || endNode.IsMethod ? 0 : 1;
      if (endNode == this.Root)
        return num;
      for (DataSchemaNode parent = endNode.Parent; parent != this.Root; parent = parent.Parent)
      {
        if (parent == null)
          return -1;
        if (parent.IsCollectionItem)
          ++num;
      }
      return num;
    }

    public DataSchemaNode GetEffectiveCollectionNode(DataSchemaNode endNode)
    {
      if (endNode.IsCollection)
        return endNode;
      if (endNode == this.Root)
        return (DataSchemaNode) null;
      for (DataSchemaNode parent = endNode.Parent; parent != this.Root; parent = parent.Parent)
      {
        if (parent == null)
          return (DataSchemaNode) null;
        if (parent.IsCollectionItem)
          return parent.Parent;
      }
      return (DataSchemaNode) null;
    }

    public DataSchemaNodePath CreateNodePath()
    {
      return new DataSchemaNodePath((ISchema) this, this.root);
    }

    public DataSchemaNodePath GetNodePathFromPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return this.CreateNodePath();
      DataSchemaNode endNode = this.root;
      IList<ClrPathPart> list = ClrPropertyPathHelper.SplitPath(path);
      if (list == null)
        return (DataSchemaNodePath) null;
      for (int index = 0; index < list.Count && endNode != null; ++index)
      {
        ClrPathPart clrPathPart = list[index];
        string pathName = clrPathPart.Category == ClrPathPartCategory.PropertyName ? clrPathPart.Path : DataSchemaNode.IndexNodePath;
        endNode = endNode.FindChildByPathName(pathName);
      }
      if (endNode != null)
        return new DataSchemaNodePath((ISchema) this, endNode);
      return (DataSchemaNodePath) null;
    }

    void IDataSchemaNodeDelayLoader.ProcessChildren(DataSchemaNode node)
    {
      if ((node.NodeType | SchemaNodeTypes.Method) == SchemaNodeTypes.Method)
        return;
      if (ClrObjectSchema.IsCollection(node.Type))
      {
        Type type = CollectionAdapterDescription.GetGenericCollectionType(node.Type);
        if (type == (Type) null)
          type = typeof (object);
        SchemaNodeTypes nodeType = SchemaNodeTypes.CollectionItem;
        if (ClrObjectSchema.IsCollection(type))
          nodeType |= SchemaNodeTypes.CollectionItem;
        DataSchemaNode dataSchemaNode = new DataSchemaNode(node.PathName, DataSchemaNode.IndexNodePath, nodeType, string.Empty, type, (IDataSchemaNodeDelayLoader) this);
        node.CollectionItem = dataSchemaNode;
      }
      else
        this.AddMethodBasedChildren(node);
      this.AddPropertyBasedChildren(node);
      node.Children.Sort((IComparer<DataSchemaNode>) new DataSchemaNode.PathNameComparer());
    }

    private void AddPropertyBasedChildren(DataSchemaNode node)
    {
      PropertyDescriptorCollection properties;
      try
      {
        properties = TypeDescriptor.GetProperties(node.Type);
      }
      catch
      {
        return;
      }
      foreach (PropertyDescriptor propertyDescriptor in properties)
      {
        Type propertyType;
        try
        {
          propertyType = propertyDescriptor.PropertyType;
        }
        catch
        {
          continue;
        }
        if (!propertyDescriptor.DesignTimeOnly)
        {
          SchemaNodeTypes nodeType = ClrObjectSchema.IsCollection(propertyType) ? SchemaNodeTypes.Collection : SchemaNodeTypes.Property;
          node.AddChild(new DataSchemaNode(propertyDescriptor.DisplayName, propertyDescriptor.Name, nodeType, propertyType.Name, propertyType, (IDataSchemaNodeDelayLoader) this)
          {
            IsReadOnly = propertyDescriptor.IsReadOnly
          });
        }
      }
      PropertyInfo[] propertyInfoArray = (PropertyInfo[]) null;
      try
      {
        propertyInfoArray = node.Type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      }
      catch
      {
      }
      if (propertyInfoArray == null || propertyInfoArray.Length <= 0)
        return;
      for (int index = 0; index < propertyInfoArray.Length; ++index)
      {
        PropertyInfo propertyInfo = propertyInfoArray[index];
        Type propertyType;
        try
        {
          propertyType = propertyInfo.PropertyType;
        }
        catch
        {
          continue;
        }
        if (propertyInfo.CanRead)
        {
          SchemaNodeTypes nodeType = ClrObjectSchema.IsCollection(propertyType) ? SchemaNodeTypes.Collection : SchemaNodeTypes.Property;
          node.AddChild(new DataSchemaNode(propertyInfo.Name, propertyInfo.Name, nodeType, propertyType.Name, propertyType, (IDataSchemaNodeDelayLoader) this)
          {
            IsReadOnly = !propertyInfo.CanWrite
          });
        }
      }
    }

    private void AddMethodBasedChildren(DataSchemaNode node)
    {
      List<MethodInfo> supportedMethods = DataBindingDragDropAddTriggerHandler.GetSupportedMethods(this.DataSource.DocumentNode.TypeResolver, node.Type);
      if (supportedMethods == null || supportedMethods.Count == 0)
        return;
      for (int index = 0; index < supportedMethods.Count; ++index)
      {
        MethodInfo methodInfo = supportedMethods[index];
        node.AddChild(new DataSchemaNode(methodInfo.Name, methodInfo.Name, SchemaNodeTypes.Method, (string) null, methodInfo.ReturnType, (IDataSchemaNodeDelayLoader) this)
        {
          IsReadOnly = true
        });
      }
    }

    public static bool IsCollection(Type type)
    {
      bool flag = typeof (ICollection).IsAssignableFrom(type) || typeof (IListSource).IsAssignableFrom(type);
      if (CollectionAdapterDescription.GetGenericCollectionType(type) != (Type) null)
        flag = true;
      return flag;
    }

    public static string CombinePaths(string inheritedPath, string localPath)
    {
      return ClrPropertyPathHelper.CombinePaths(inheritedPath, localPath);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
      if (this.DataSource == null)
        return "Empty";
      return this.DataSource.Name;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SchemaItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SchemaItem : NotifyingObject, ITreeViewItemProvider<DataModelItemBase>
  {
    private ISchema schema;
    private VirtualizingTreeItemFlattener<DataModelItemBase> flattener;
    private DataSchemaItem root;
    private DataSchemaItemFilter lastFilter;
    private DataPanelModel model;
    private SelectionContext<DataModelItemBase> itemSelectionContext;

    DataModelItemBase ITreeViewItemProvider<DataModelItemBase>.RootItem
    {
      get
      {
        return (DataModelItemBase) this.Root;
      }
    }

    public DataSchemaItem Root
    {
      get
      {
        return this.root;
      }
    }

    public ReadOnlyObservableCollection<DataModelItemBase> ItemList
    {
      get
      {
        return this.flattener.ItemList;
      }
    }

    public DataSchemaItem SelectedItem
    {
      get
      {
        return Enumerable.FirstOrDefault<DataSchemaItem>(Enumerable.OfType<DataSchemaItem>((IEnumerable) this.itemSelectionContext));
      }
      set
      {
        this.itemSelectionContext.SetSelection((DataModelItemBase) value);
      }
    }

    public string SelectionPath
    {
      get
      {
        if (this.SelectedItem != null)
          return this.SelectedItem.DataSchemaNodePath.Path;
        return string.Empty;
      }
    }

    public ISchema Schema
    {
      get
      {
        return this.schema;
      }
    }

    public SchemaItem(ISchema schema)
      : this(schema, (DataPanelModel) null, (SelectionContext<DataModelItemBase>) null)
    {
    }

    public SchemaItem(ISchema schema, DataPanelModel model, SelectionContext<DataModelItemBase> selectionContext)
    {
      this.model = model;
      this.itemSelectionContext = selectionContext ?? (SelectionContext<DataModelItemBase>) new SingleSelectionContext<DataModelItemBase>();
      this.itemSelectionContext.PropertyChanged += new PropertyChangedEventHandler(this.itemSelectionContext_PropertyChanged);
      this.schema = schema;
      this.flattener = new VirtualizingTreeItemFlattener<DataModelItemBase>((ITreeViewItemProvider<DataModelItemBase>) this);
      if (this.schema.Root != null)
      {
        DataSchemaNodePath nodePath = new DataSchemaNodePath(this.schema, this.schema.Root);
        this.itemSelectionContext.Clear();
        this.ClearSchemaItemTree(this.root);
        this.root = new DataSchemaItem(nodePath, this, this.model, this.itemSelectionContext);
        this.root.EnsureChildrenExpanded();
      }
      this.OnPropertyChanged("Root");
      this.flattener.RebuildList();
    }

    public DataSchemaItem FindDataSchemaItemForNode(DataSchemaNodePath descendant)
    {
      DataSchemaItem currentItem = this.root;
      Stack<DataSchemaNode> stack = new Stack<DataSchemaNode>();
      for (DataSchemaNode dataSchemaNode = descendant.Node; dataSchemaNode != this.root.DataSchemaNode; dataSchemaNode = dataSchemaNode.Parent)
        stack.Push(dataSchemaNode);
      while (stack.Count > 0)
      {
        DataSchemaNode nextNode = stack.Pop();
        DataSchemaNode adjustedNextNode;
        currentItem = this.GetItemForNode(currentItem, nextNode, out adjustedNextNode);
        if (adjustedNextNode != nextNode)
        {
          do
            ;
          while (stack.Count > 0 && stack.Pop() != adjustedNextNode);
        }
      }
      return currentItem;
    }

    public void SetDataSchemaItemFilter(DataSchemaItemFilter filter)
    {
      this.lastFilter = filter;
      if (this.root == null)
        return;
      this.SetDataSchemaItemFilter(this.root);
    }

    public void ClearDataSchemaItemFilter()
    {
      if (this.root != null)
        this.ClearDataSchemaItemFilter(this.root);
      this.lastFilter = (DataSchemaItemFilter) null;
    }

    public void RefreshDataSchemaItemFilter()
    {
      if (this.root == null)
        return;
      this.RefreshDataSchemaItemFilter(this.root);
    }

    public DataSchemaItem GetItemFromPath(string path)
    {
      DataSchemaNodePath nodePathFromPath = this.schema.GetNodePathFromPath(path);
      if (nodePathFromPath != null && nodePathFromPath.Node != null)
        return this.FindDataSchemaItemForNode(nodePathFromPath);
      return (DataSchemaItem) null;
    }

    public void ProcessChildren(DataSchemaItem dataSchemaItem)
    {
      DataSchemaNodePath dataSchemaNodePath = dataSchemaItem.DataSchemaNodePath;
      DataSchemaNode dataSchemaNode = dataSchemaNodePath.Node;
      bool flag = true;
      while (dataSchemaNode.IsCollection && dataSchemaNode.CollectionItem != null && dataSchemaNode.CollectionItem.Type != typeof (object))
        dataSchemaNode = dataSchemaNode.CollectionItem;
      if (dataSchemaNode.Type == typeof (string) && !(dataSchemaItem.DataSchemaNodePath.Schema is XmlSchema))
        flag = false;
      else if (dataSchemaItem.SampleType is SampleBasicType)
        flag = false;
      else if (dataSchemaNode != dataSchemaNodePath.Node)
      {
        SampleCollectionType sampleCollectionType = dataSchemaNodePath.Node.SampleType as SampleCollectionType;
        if (sampleCollectionType != null && sampleCollectionType.ItemSampleType is SampleBasicType)
          flag = false;
      }
      if (flag)
      {
        foreach (DataSchemaNode node in dataSchemaNode.Children)
        {
          DataSchemaItem dataSchemaItem1 = new DataSchemaItem(dataSchemaNodePath.GetExtendedPath(node), this, this.model, this.itemSelectionContext);
          dataSchemaItem.AddChild((DataModelItemBase) dataSchemaItem1);
        }
      }
      if (this.lastFilter == null)
        return;
      dataSchemaItem.SetFilter(this.lastFilter);
    }

    private void SetDataSchemaItemFilter(DataSchemaItem item)
    {
      if (!item.HasLoadedChildren)
        return;
      item.SetFilter(this.lastFilter);
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) item.Children))
        this.SetDataSchemaItemFilter(dataSchemaItem);
    }

    private void ClearDataSchemaItemFilter(DataSchemaItem item)
    {
      if (!item.HasLoadedChildren)
        return;
      item.SetFilter((DataSchemaItemFilter) null);
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) item.Children))
        this.ClearDataSchemaItemFilter(dataSchemaItem);
    }

    public void RefreshDataSchemaItemFilter(DataSchemaItem item)
    {
      if (!item.HasLoadedChildren)
        return;
      item.RefreshFilter();
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) item.Children))
        this.RefreshDataSchemaItemFilter(dataSchemaItem);
    }

    private void ClearSchemaItemTree(DataSchemaItem item)
    {
      if (item == null)
        return;
      item.Dispose();
      if (!item.HasLoadedChildren)
        return;
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) item.Children))
        this.ClearSchemaItemTree(dataSchemaItem);
    }

    private void itemSelectionContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "PrimarySelection"))
        return;
      this.OnPropertyChanged("SelectionPath");
      this.OnPropertyChanged("SelectedItem");
    }

    private DataSchemaItem GetItemForNode(DataSchemaItem currentItem, DataSchemaNode nextNode, out DataSchemaNode adjustedNextNode)
    {
      currentItem.EnsureChildrenExpanded();
      adjustedNextNode = nextNode;
      foreach (DataSchemaItem dataSchemaItem in Enumerable.OfType<DataSchemaItem>((IEnumerable) currentItem.Children))
      {
        if (dataSchemaItem.DataSchemaNode == nextNode)
        {
          while (adjustedNextNode.IsCollection && adjustedNextNode.CollectionItem != null && adjustedNextNode.CollectionItem.Type != typeof (object))
            adjustedNextNode = adjustedNextNode.CollectionItem;
          return dataSchemaItem;
        }
      }
      if (currentItem.DataSchemaNode.CollectionItem == nextNode)
        return currentItem;
      return (DataSchemaItem) null;
    }
  }
}

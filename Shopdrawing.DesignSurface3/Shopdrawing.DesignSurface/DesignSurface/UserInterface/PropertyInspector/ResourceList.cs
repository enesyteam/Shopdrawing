// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ResourceList
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal sealed class ResourceList : ITreeViewItemProvider<ResourceVirtualizingTreeItem>
  {
    private VirtualizingTreeItemFlattener<ResourceVirtualizingTreeItem> flattener;
    private ResourceVirtualizingTreeItem root;
    private Dictionary<string, ResourceTypeItem> categories;

    public ReadOnlyObservableCollection<ResourceVirtualizingTreeItem> ItemList
    {
      get
      {
        return this.flattener.ItemList;
      }
    }

    public ResourceVirtualizingTreeItem RootItem
    {
      get
      {
        return this.root;
      }
    }

    public ResourceList()
    {
      this.root = (ResourceVirtualizingTreeItem) new ResourceContainerItem();
      this.root.IsExpanded = true;
      this.flattener = new VirtualizingTreeItemFlattener<ResourceVirtualizingTreeItem>((ITreeViewItemProvider<ResourceVirtualizingTreeItem>) this);
      this.categories = new Dictionary<string, ResourceTypeItem>();
    }

    public void EnsureCategoryExists(string categoryKey, string categoryName)
    {
      ResourceTypeItem resourceTypeItem1;
      if (this.categories.TryGetValue(categoryKey, out resourceTypeItem1))
        return;
      ResourceTypeItem resourceTypeItem2 = new ResourceTypeItem(categoryName, categoryKey);
      this.categories.Add(categoryKey, resourceTypeItem2);
      this.root.AddChild((ResourceVirtualizingTreeItem) resourceTypeItem2);
    }

    public void UpdateCategory<T>(string categoryKey, IEnumerable<SelectablePropertyModel<T>> resources) where T : BaseResourceModel
    {
      ResourceTypeItem resourceTypeItem;
      if (!this.categories.TryGetValue(categoryKey, out resourceTypeItem))
        return;
      resourceTypeItem.Children.Clear();
      foreach (SelectablePropertyModel<T> resourceModel in resources)
        resourceTypeItem.AddChild((ResourceVirtualizingTreeItem) new VirtualizingResourceItem<T>(resourceModel));
    }

    public VirtualizingResourceItem<T> FindItemInCategory<T>(string categoryKey, SelectablePropertyModel<T> resource) where T : BaseResourceModel
    {
      ResourceTypeItem resourceTypeItem;
      if (this.categories.TryGetValue(categoryKey, out resourceTypeItem))
      {
        foreach (VirtualizingResourceItem<T> virtualizingResourceItem in (Collection<ResourceVirtualizingTreeItem>) resourceTypeItem.Children)
        {
          if (virtualizingResourceItem.Model.Equals((object) resource))
            return virtualizingResourceItem;
        }
      }
      return (VirtualizingResourceItem<T>) null;
    }
  }
}

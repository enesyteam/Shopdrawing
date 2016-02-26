// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.VirtualizingResourceItem`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal sealed class VirtualizingResourceItem<T> : ResourceVirtualizingTreeItem, ISelectable where T : BaseResourceModel
  {
    private SelectablePropertyModel<T> resourceModel;

    public SelectablePropertyModel<T> Model
    {
      get
      {
        return this.resourceModel;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.resourceModel.PropertyModel.ResourceName;
      }
      set
      {
      }
    }

    public VirtualizingResourceItem(SelectablePropertyModel<T> resourceModel)
    {
      this.resourceModel = resourceModel;
    }

    public override int CompareTo(ResourceVirtualizingTreeItem treeItem)
    {
      VirtualizingResourceItem<T> virtualizingResourceItem = treeItem as VirtualizingResourceItem<T>;
      if (virtualizingResourceItem != null)
        return this.resourceModel.CompareTo((object) virtualizingResourceItem.resourceModel);
      return -1;
    }

    [SpecialName]
    bool ISelectable.get_IsSelected()
    {
      return this.IsSelected;
    }

    [SpecialName]
    void ISelectable.set_IsSelected([In] bool obj0)
    {
      this.IsSelected = obj0;
    }
  }
}

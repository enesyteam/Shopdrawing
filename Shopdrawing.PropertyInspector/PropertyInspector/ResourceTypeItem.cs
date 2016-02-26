// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ResourceTypeItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class ResourceTypeItem : ResourceVirtualizingTreeItem
  {
    private string displayName;
    private string id;

    public override string DisplayName
    {
      get
      {
        return this.displayName;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        return this.id;
      }
    }

    public string Id
    {
      get
      {
        return this.id;
      }
    }

    public ResourceTypeItem(string displayName, string id)
    {
      this.displayName = displayName;
      this.id = id;
      this.IsExpanded = true;
    }

    public override int CompareTo(ResourceVirtualizingTreeItem treeItem)
    {
      return this.FullName.CompareTo(treeItem.FullName);
    }
  }
}

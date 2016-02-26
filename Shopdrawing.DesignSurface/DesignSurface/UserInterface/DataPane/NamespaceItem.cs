// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.NamespaceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class NamespaceItem : FilteredTreeItem
  {
    public const int MaxTypesForAutoExpand = 6;
    private string assemblyName;
    private string namespaceName;
    private AssemblyItem owningAssembly;
    private int visibleChildrenCount;

    public int VisibleChildrenCount
    {
      get
      {
        return this.visibleChildrenCount;
      }
    }

    public bool IsForcedVisible
    {
      get
      {
        foreach (FilteredTreeItem filteredTreeItem in (Collection<FilteredTreeItem>) this.Children)
        {
          TypeItem typeItem = filteredTreeItem as TypeItem;
          if (typeItem != null && typeItem.ForceVisible)
            return true;
        }
        return false;
      }
    }

    public override bool IsVisible
    {
      get
      {
        if (this.Children.Count == 0)
          return false;
        if (this.IsForcedVisible)
          return true;
        if (this.Filter == null)
          return this.owningAssembly.MatchesFilter;
        foreach (FilteredTreeItem filteredTreeItem in (Collection<FilteredTreeItem>) this.Children)
        {
          TypeItem typeItem = filteredTreeItem as TypeItem;
          if (typeItem != null && typeItem.MatchesFilter)
            return true;
        }
        if (!this.owningAssembly.MatchesFilter)
          return this.MatchesFilter;
        return true;
      }
    }

    public override string DisplayName
    {
      get
      {
        return this.namespaceName;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        return this.assemblyName + ":" + this.namespaceName;
      }
    }

    public NamespaceItem(string assemblyName, string namespaceName, AssemblyItem owningAssembly)
    {
      this.assemblyName = assemblyName;
      this.namespaceName = namespaceName;
      this.owningAssembly = owningAssembly;
    }

    public void IncrementVisibleChildren()
    {
      ++this.visibleChildrenCount;
    }

    public void DecrementVisibleChildren()
    {
      --this.visibleChildrenCount;
    }

    public override int CompareTo(FilteredTreeItem treeItem)
    {
      return this.DisplayName.CompareTo(treeItem.DisplayName);
    }
  }
}

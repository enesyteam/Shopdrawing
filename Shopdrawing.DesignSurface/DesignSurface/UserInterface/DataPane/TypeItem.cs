// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.TypeItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.Data;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class TypeItem : FilteredTreeItem, ISelectable
  {
    private SelectionContext<TypeItem> selectionContext;
    private Type type;
    private NamespaceItem owningNamespace;
    private string fullName;
    private bool isVisible;
    private Predicate<FilteredTreeItem> forceVisibleFilter;

    public override string DisplayName
    {
      get
      {
        return this.type.Name;
      }
      set
      {
      }
    }

    public override string FullName
    {
      get
      {
        return this.fullName;
      }
    }

    public Type Type
    {
      get
      {
        return this.type;
      }
    }

    public override bool IsVisible
    {
      get
      {
        return this.isVisible;
      }
    }

    public virtual Predicate<FilteredTreeItem> ForceVisibleFilter
    {
      get
      {
        return this.forceVisibleFilter;
      }
      set
      {
        this.forceVisibleFilter = value;
        this.OnPropertyChanged("ForceVisibleFilter");
      }
    }

    public virtual bool ForceVisible
    {
      get
      {
        if (this.ForceVisibleFilter != null)
          return this.ForceVisibleFilter((FilteredTreeItem) this);
        return false;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        return true;
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnSelect));
      }
    }

    bool ISelectable.IsSelected
    {
      get
      {
        return this.IsSelected;
      }
      set
      {
        this.IsSelected = value;
      }
    }

    public TypeItem(Type type, SelectionContext<TypeItem> selectionContext, NamespaceItem owningNamespace)
    {
      this.type = type;
      this.selectionContext = selectionContext;
      this.owningNamespace = owningNamespace;
      this.fullName = AssemblyHelper.GetAssemblyName(this.type.Assembly).Name + ":" + this.type.Namespace + "." + this.type.Name;
    }

    public override int CompareTo(FilteredTreeItem treeItem)
    {
      return this.DisplayName.CompareTo(treeItem.DisplayName);
    }

    private void UpdateVisibility()
    {
      bool flag = this.owningNamespace.MatchesFilter || this.MatchesFilter || this.ForceVisible;
      if (flag == this.isVisible)
        return;
      if (this.isVisible)
        this.owningNamespace.DecrementVisibleChildren();
      else
        this.owningNamespace.IncrementVisibleChildren();
      this.isVisible = flag;
    }

    public override void OnFilterChanged()
    {
      this.UpdateVisibility();
    }

    private void OnSelect()
    {
      this.Select();
    }

    public override void Select()
    {
      base.Select();
      this.selectionContext.SetSelection(this);
    }
  }
}

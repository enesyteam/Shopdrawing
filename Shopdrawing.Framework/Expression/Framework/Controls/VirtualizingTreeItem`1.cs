// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.VirtualizingTreeItem`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Controls
{
  public abstract class VirtualizingTreeItem<T> : INotifyPropertyChanged where T : VirtualizingTreeItem<T>
  {
    private T parent = default (T);
    private bool isExpanded;
    private bool isSelected;
    private VirtualizingTreeItemCollection<T> children;
    private int selectedChildCount;
    private bool isFinishingExpansion;
    private bool isMouseOver;

    public T Parent
    {
      get
      {
        return this.parent;
      }
    }

    public virtual bool IsVisible
    {
      get
      {
        return true;
      }
    }

    public virtual bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        if (this.isExpanded == value)
          return;
        if (value)
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ExpandTimelineItem);
        else
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.CollapseTimelineItem);
        this.isExpanded = value;
        if (this.IsExpandedChanged != null)
          this.IsExpandedChanged((object) this, new EventArgs());
        this.OnPropertyChanged("IsExpanded");
        if (!this.isExpanded)
          return;
        this.isFinishingExpansion = true;
        this.OnPropertyChanged("IsFinishingExpansion");
        this.isFinishingExpansion = false;
        this.OnPropertyChanged("IsFinishingExpansion");
      }
    }

    public bool IsFinishingExpansion
    {
      get
      {
        return this.isFinishingExpansion;
      }
    }

    public abstract string DisplayName { get; set; }

    public virtual string DisplayNameNoTextContent
    {
      get
      {
        return this.DisplayName;
      }
    }

    public virtual string FullName
    {
      get
      {
        return this.DisplayName;
      }
    }

    public virtual bool IsSelectable
    {
      get
      {
        return false;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        if (this.isSelected == value)
          return;
        this.SetSelection(value);
        this.OnPropertyChanged("IsSelected");
        if ((object) this.Parent == null || this.HasSelectedChild)
          return;
        if (this.isSelected)
          ++this.Parent.SelectedChildCount;
        else
          --this.Parent.SelectedChildCount;
      }
    }

    public bool HasSelectedChild
    {
      get
      {
        return this.selectedChildCount > 0;
      }
    }

    public VirtualizingTreeItemCollection<T> Children
    {
      get
      {
        return this.children;
      }
    }

    public int Depth { get; set; }

    public bool IsMouseOver
    {
      get
      {
        return this.isMouseOver;
      }
      set
      {
        if (this.isMouseOver == value)
          return;
        this.isMouseOver = value;
        this.OnPropertyChanged("IsMouseOver");
      }
    }

    public virtual IComparer<T> TreeItemComparer
    {
      get
      {
        return (IComparer<T>) new VirtualizingTreeItem<T>.DefaultTreeItemComparer();
      }
    }

    protected int SelectedChildCount
    {
      get
      {
        return this.selectedChildCount;
      }
      set
      {
        int num = this.selectedChildCount;
        this.selectedChildCount = value;
        if (value > 0 && num == 0)
        {
          this.OnPropertyChanged("HasSelectedChild");
          if ((object) this.Parent == null || this.IsSelected)
            return;
          ++this.Parent.SelectedChildCount;
        }
        else
        {
          if (value != 0 || num == 0)
            return;
          this.OnPropertyChanged("HasSelectedChild");
          if ((object) this.Parent == null || this.IsSelected)
            return;
          --this.Parent.SelectedChildCount;
        }
      }
    }

    public event EventHandler IsExpandedChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected VirtualizingTreeItem()
    {
      this.children = new VirtualizingTreeItemCollection<T>((T) this);
    }

    protected virtual void SetSelection(bool value)
    {
      this.isSelected = value;
    }

    public abstract int CompareTo(T treeItem);

    public void AddChild(T child)
    {
      child.SetParent((T) this);
      int index = this.children.BinarySearch(child, this.TreeItemComparer);
      if (index < 0)
        index = ~index;
      this.children.Insert(index, child);
      child.PropertyChanged += new PropertyChangedEventHandler(this.Child_PropertyChanged);
      child.Initialize();
      this.ChildAdded(child);
      if (!child.IsSelected && child.SelectedChildCount <= 0)
        return;
      ++this.SelectedChildCount;
    }

    protected void InternalInsertChild(T child, int insertionIndex)
    {
      child.SetParent((T) this);
      this.children.Insert(insertionIndex, child);
    }

    public void RemoveChild(T child)
    {
      this.ChildRemoving(child);
      this.children.Remove(child);
      child.OnRemoved();
      child.SetParent(default (T));
      child.PropertyChanged -= new PropertyChangedEventHandler(this.Child_PropertyChanged);
      this.ChildRemoved(child);
      if (!child.IsSelected && child.SelectedChildCount <= 0)
        return;
      --this.SelectedChildCount;
    }

    private void RemoveChildren()
    {
      foreach (T child in new List<T>((IEnumerable<T>) this.children))
        this.RemoveSubtree(child);
    }

    public void RemoveSubtree(T child)
    {
      child.OnRemoving();
      child.RemoveChildren();
      this.RemoveChild(child);
    }

    public void RemoveFromTree()
    {
      if ((object) this.Parent != null)
      {
        this.Parent.RemoveSubtree((T) this);
      }
      else
      {
        this.OnRemoving();
        this.RemoveChildren();
        this.OnRemoved();
      }
    }

    public virtual void Initialize()
    {
    }

    public virtual void Select()
    {
    }

    public virtual void ToggleSelect()
    {
    }

    public virtual void ExtendSelect()
    {
    }

    public virtual void EnsureSelect()
    {
      if (this.IsSelected)
        return;
      this.Select();
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void ChildAdded(T child)
    {
    }

    protected virtual void ChildRemoving(T child)
    {
    }

    protected virtual void ChildRemoved(T child)
    {
    }

    protected virtual void ChildPropertyChanged(string propertyName)
    {
    }

    protected virtual void OnRemoving()
    {
    }

    protected virtual void OnRemoved()
    {
    }

    private void SetParent(T parent)
    {
      this.parent = parent;
    }

    private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.ChildPropertyChanged(e.PropertyName);
    }

    private class DefaultTreeItemComparer : Comparer<T>
    {
      public override int Compare(T x, T y)
      {
        return x.CompareTo(y);
      }
    }
  }
}

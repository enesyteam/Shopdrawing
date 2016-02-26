// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.VirtualizingTreeItemFlattener`1
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Controls
{
  public class VirtualizingTreeItemFlattener<T> where T : VirtualizingTreeItem<T>
  {
    private ITreeViewItemProvider<T> provider;
    private ObservableCollection<T> itemList;
    private ReadOnlyObservableCollection<T> readonlyList;

    public ReadOnlyObservableCollection<T> ItemList
    {
      get
      {
        return this.readonlyList;
      }
    }

    public VirtualizingTreeItemFlattener(ITreeViewItemProvider<T> provider)
      : this(provider, true)
    {
    }

    public VirtualizingTreeItemFlattener(ITreeViewItemProvider<T> provider, bool includeRoot)
    {
      this.itemList = new ObservableCollection<T>();
      this.readonlyList = new ReadOnlyObservableCollection<T>(this.itemList);
      this.provider = provider;
      this.RebuildList(includeRoot);
    }

    public void RebuildList()
    {
      this.RebuildList(true);
    }

    public void RebuildList(bool includeRoot)
    {
      T rootItem = this.provider.RootItem;
      foreach (T treeItem in (Collection<T>) this.itemList)
        this.RemoveHandlers(treeItem);
      if ((object) rootItem != null)
        this.RemoveHandlers(rootItem);
      this.itemList.Clear();
      if ((object) rootItem == null)
        return;
      this.AddHandlers(rootItem);
      rootItem.Depth = includeRoot ? 0 : -1;
      if (includeRoot)
        this.itemList.Add(rootItem);
      if (!rootItem.IsExpanded && includeRoot)
        return;
      int index = includeRoot ? 1 : 0;
      this.InsertChildrenInList(rootItem, ref index);
    }

    private void InsertChildrenInList(T parentItem, ref int index)
    {
      foreach (T obj in (Collection<T>) parentItem.Children)
      {
        if (obj.IsVisible)
        {
          this.AddHandlers(obj);
          obj.Depth = parentItem.Depth + 1;
          this.itemList.Insert(index, obj);
          ++index;
          if (obj.IsExpanded)
            this.InsertChildrenInList(obj, ref index);
        }
      }
    }

    private void RemoveChildrenFromList(T parentItem)
    {
      int num = this.itemList.IndexOf(parentItem) + 1;
      int index1 = num;
      while (index1 < this.itemList.Count && this.itemList[index1].Depth > parentItem.Depth)
        ++index1;
      for (int index2 = index1 - 1; index2 >= num; --index2)
      {
        this.RemoveHandlers(this.itemList[index2]);
        this.itemList.RemoveAt(index2);
      }
    }

    private void VirtualizingTreeItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsExpanded"))
        return;
      T parentItem = (T) sender;
      if (parentItem.IsExpanded)
      {
        int index = this.itemList.IndexOf(parentItem) + 1;
        this.InsertChildrenInList(parentItem, ref index);
      }
      else
        this.RemoveChildrenFromList(parentItem);
    }

    private void VirtualizingTreeItem_ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      T parentItem = ((VirtualizingTreeItemCollection<T>) sender).ParentItem;
      if (!parentItem.IsExpanded)
        return;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          int num1 = 0;
          for (int index = 0; index < e.NewStartingIndex; ++index)
          {
            if (!parentItem.Children[index].IsVisible)
              ++num1;
          }
          int index1 = this.itemList.IndexOf(parentItem) + 1;
          int num2 = 0;
          for (; index1 < this.itemList.Count; ++index1)
          {
            if (this.itemList[index1].Depth <= parentItem.Depth + 1)
              ++num2;
            if (num2 > e.NewStartingIndex - num1)
              break;
          }
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              T obj = (T) enumerator1.Current;
              obj.Depth = parentItem.Depth + 1;
              this.AddHandlers(obj);
              this.itemList.Insert(index1, obj);
              ++index1;
              if (obj.IsExpanded)
                this.InsertChildrenInList(obj, ref index1);
            }
            break;
          }
          finally
          {
            IDisposable disposable = enumerator1 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              T obj = (T) enumerator2.Current;
              this.RemoveChildrenFromList(obj);
              this.RemoveHandlers(obj);
              this.itemList.Remove(obj);
            }
            break;
          }
          finally
          {
            IDisposable disposable = enumerator2 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Reset:
          int index2 = this.itemList.IndexOf(parentItem) + 1;
          this.RemoveChildrenFromList(parentItem);
          this.InsertChildrenInList(parentItem, ref index2);
          break;
      }
    }

    private void AddHandlers(T treeItem)
    {
      treeItem.PropertyChanged += new PropertyChangedEventHandler(this.VirtualizingTreeItem_PropertyChanged);
      treeItem.Children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.VirtualizingTreeItem_ChildrenCollectionChanged);
    }

    private void RemoveHandlers(T treeItem)
    {
      treeItem.PropertyChanged -= new PropertyChangedEventHandler(this.VirtualizingTreeItem_PropertyChanged);
      treeItem.Children.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.VirtualizingTreeItem_ChildrenCollectionChanged);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SingleSelectionContext`1
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SingleSelectionContext<T> : SelectionContext<T> where T : class, ISelectable
  {
    private T selectedNode;

    public override T PrimarySelection
    {
      get
      {
        return this.selectedNode;
      }
      set
      {
        this.SelectInternal(value);
      }
    }

    public override int Count
    {
      get
      {
        return (object) this.selectedNode == null ? 0 : 1;
      }
    }

    public override void SetSelection(T item)
    {
      this.SelectInternal(item);
    }

    public override void SetSelection(IEnumerable<T> items)
    {
      this.SelectInternal(Enumerable.Last<T>(items));
    }

    public override void Add(T item)
    {
      this.SelectInternal(item);
    }

    public override void Clear()
    {
      this.SelectInternal(default (T));
    }

    public override bool Contains(T item)
    {
      if ((object) this.selectedNode != null)
        return (object) item == (object) this.selectedNode;
      return false;
    }

    public override bool Remove(T item)
    {
      if (!this.Contains(item))
        return false;
      this.SelectInternal(default (T));
      return true;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) new SingleSelectionContext<T>.SingleItemEnumerator<T>(this.selectedNode);
    }

    private void SelectInternal(T item)
    {
      if ((object) this.selectedNode != null)
        this.selectedNode.IsSelected = false;
      this.selectedNode = item;
      if ((object) this.selectedNode != null)
        this.selectedNode.IsSelected = true;
      this.OnPropertyChanged("PrimarySelection");
    }

    private class SingleItemEnumerator<EnumerationType> : IEnumerator<EnumerationType>, IDisposable, IEnumerator where EnumerationType : class
    {
      private EnumerationType item;
      private int index;
      private int count;

      public EnumerationType Current
      {
        get
        {
          if (this.index != 0)
            return default (EnumerationType);
          return this.item;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public SingleItemEnumerator(EnumerationType item)
      {
        this.index = -1;
        this.item = item;
        this.count = (object) item == null ? 0 : 1;
      }

      public void Dispose()
      {
        this.item = default (EnumerationType);
      }

      public bool MoveNext()
      {
        ++this.index;
        return this.index < this.count;
      }

      public void Reset()
      {
        this.index = -1;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.FormattedNodeCollection
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class FormattedNodeCollection : ICollection<FormattedNode>, IEnumerable<FormattedNode>, IEnumerable
  {
    private ContainerNode container;
    private List<FormattedNode> collection;

    public FormattedNode this[int index]
    {
      get
      {
        return this.collection[index];
      }
    }

    public int Count
    {
      get
      {
        return this.collection.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public FormattedNodeCollection(ContainerNode container)
    {
      this.container = container;
      this.collection = new List<FormattedNode>();
    }

    public void Insert(int index, FormattedNode item)
    {
      this.collection.Insert(index, item);
      item.Parent = this.container;
    }

    public void Add(FormattedNode item)
    {
      this.collection.Add(item);
      item.Parent = this.container;
    }

    public void Clear()
    {
      foreach (FormattedNode formattedNode in this.collection)
        formattedNode.Parent = (ContainerNode) null;
      this.collection.Clear();
    }

    public bool Contains(FormattedNode item)
    {
      return this.collection.Contains(item);
    }

    public void CopyTo(FormattedNode[] array, int arrayIndex)
    {
      this.collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(FormattedNode item)
    {
      item.Parent = (ContainerNode) null;
      return this.collection.Remove(item);
    }

    public IEnumerator<FormattedNode> GetEnumerator()
    {
      return (IEnumerator<FormattedNode>) this.collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.collection.GetEnumerator();
    }

    public void EnsureOrdering()
    {
      for (int index1 = 1; index1 < this.collection.Count; ++index1)
      {
        FormattedNode formattedNode = this.collection[index1];
        int index2;
        for (index2 = index1 - 1; index2 >= 0 && this.collection[index2].Ordering > formattedNode.Ordering; --index2)
          this.collection[index2 + 1] = this.collection[index2];
        this.collection[index2 + 1] = formattedNode;
      }
    }
  }
}

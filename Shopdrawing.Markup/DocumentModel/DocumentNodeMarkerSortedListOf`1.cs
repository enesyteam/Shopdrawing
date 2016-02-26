// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeMarkerSortedListOf`1
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public class DocumentNodeMarkerSortedListOf<T> : DocumentNodeMarkerSortedListBase
  {
    private List<KeyValuePair<DocumentNodeMarker, T>> list;

    public override int Count
    {
      get
      {
        return this.list.Count;
      }
    }

    public IEnumerable<T> Values
    {
      get
      {
        for (int i = 0; i < this.list.Count; ++i)
          yield return this.list[i].Value;
      }
    }

    public DocumentNodeMarkerSortedListOf()
    {
      this.list = new List<KeyValuePair<DocumentNodeMarker, T>>();
    }

    public DocumentNodeMarkerSortedListOf(int capacity)
    {
      this.list = new List<KeyValuePair<DocumentNodeMarker, T>>(capacity);
    }

    public DocumentNodeMarkerSortedListOf(DocumentNodeMarkerSortedListOf<T> other)
      : this(other.Count)
    {
      this.list.AddRange((IEnumerable<KeyValuePair<DocumentNodeMarker, T>>) other.list);
    }

    public override DocumentNodeMarker MarkerAt(int index)
    {
      return this.list[index].Key;
    }

    public override void Clear()
    {
      this.list.Clear();
    }

    public override void RemoveRange(int index, int count)
    {
      this.list.RemoveRange(index, count);
    }

    public override void Copy(int destinationIndex, DocumentNodeMarkerSortedListBase sourceList, int sourceIndex)
    {
      DocumentNodeMarkerSortedListOf<T> markerSortedListOf = (DocumentNodeMarkerSortedListOf<T>) sourceList;
      this.list[destinationIndex] = markerSortedListOf.list[sourceIndex];
    }

    public void Merge(DocumentNodeMarkerSortedListOf<T> otherList, bool coalesce)
    {
      int count1 = this.list.Count;
      int count2 = otherList.list.Count;
      int num = count1 + count2;
      if (this.list.Count == num)
        return;
      this.list.AddRange((IEnumerable<KeyValuePair<DocumentNodeMarker, T>>) otherList.list);
      int index1 = count1 - 1;
      int index2 = count2 - 1;
      int index3;
      for (index3 = num - 1; index3 >= 0 && index1 >= 0 && index2 >= 0; --index3)
        this.list[index3] = this.list[index1].Key.CompareTo((object) otherList.list[index2].Key) < 0 ? otherList.list[index2--] : this.list[index1--];
      for (; index2 >= 0; --index2)
      {
        this.list[index3] = otherList.list[index2];
        --index3;
      }
      if (!coalesce)
        return;
      this.CoalesceAncestry();
    }

    public T Find(DocumentNodeMarker marker)
    {
      int position = this.FindPosition(marker);
      if (position >= 0)
        return this.ValueAt(position);
      return default (T);
    }

    public int Add(DocumentNodeMarker marker, T value)
    {
      int index = ~this.FindPosition(marker);
      if (index < 0)
        return -1;
      this.list.Insert(index, new KeyValuePair<DocumentNodeMarker, T>(marker, value));
      return index;
    }

    public int AddCoalesced(DocumentNodeMarker marker, T value)
    {
      int countContained;
      int coalescePoint;
      if ((coalescePoint = this.FindCoalescePoint(marker, out countContained)) >= 0)
      {
        this.list.RemoveRange(coalescePoint, countContained);
        this.list.Insert(coalescePoint, new KeyValuePair<DocumentNodeMarker, T>(marker, value));
      }
      return coalescePoint;
    }

    public void CoalesceAncestry()
    {
      int index1 = 0;
      for (int index2 = index1 + 1; index2 < this.list.Count; ++index2)
      {
        if (!this.list[index1].Key.Contains(this.list[index2].Key))
        {
          ++index1;
          this.list[index1] = this.list[index2];
        }
      }
      if (index1 >= this.list.Count - 1)
        return;
      this.list.RemoveRange(index1 + 1, this.Count - index1 - 1);
    }

    public IEnumerable<T> FindDescendantValues(DocumentNodeMarker ancestor)
    {
      int startIndex;
      int endIndex;
      if (this.FindDescendantRange(ancestor, out startIndex, out endIndex))
      {
        for (int i = startIndex; i <= endIndex; ++i)
          yield return this.list[i].Value;
      }
    }

    public T RemoveAt(int index)
    {
      if (index < 0 || index >= this.list.Count)
        return default (T);
      T obj = this.list[index].Value;
      this.list.RemoveAt(index);
      return obj;
    }

    public T ValueAt(int index)
    {
      if (index >= 0 && index < this.list.Count)
        return this.list[index].Value;
      return default (T);
    }

    public void SetValueAt(int index, T value)
    {
      if (index < 0 || index >= this.list.Count)
        return;
      this.list[index] = new KeyValuePair<DocumentNodeMarker, T>(this.list[index].Key, value);
    }

    public void InsertAt(int index, DocumentNodeMarker marker, T item)
    {
      this.list.Insert(index, new KeyValuePair<DocumentNodeMarker, T>(marker, item));
    }

    public override string ToString()
    {
      return "Count = " + (object) this.list.Count;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeMarkerSortedList
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public class DocumentNodeMarkerSortedList : DocumentNodeMarkerSortedListBase
  {
    private List<DocumentNodeMarker> list;

    public override int Count
    {
      get
      {
        return this.list.Count;
      }
    }

    public DocumentNodeMarkerSortedList()
    {
      this.list = new List<DocumentNodeMarker>();
    }

    public DocumentNodeMarkerSortedList(int capacity)
    {
      this.list = new List<DocumentNodeMarker>(capacity);
    }

    public DocumentNodeMarkerSortedList(DocumentNodeMarkerSortedList other)
      : this(other.Count)
    {
      this.list.AddRange((IEnumerable<DocumentNodeMarker>) other.list);
    }

    public void CopyFrom(IEnumerable<DocumentNode> enumerable, bool presorted, bool forward)
    {
      foreach (DocumentNode documentNode in enumerable)
      {
        if (presorted)
          this.list.Insert(this.list.Count, documentNode.Marker);
        else
          this.Add(documentNode.Marker);
      }
      if (forward)
        return;
      this.list.Reverse();
    }

    public override DocumentNodeMarker MarkerAt(int index)
    {
      return this.list[index];
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
      DocumentNodeMarkerSortedList markerSortedList = (DocumentNodeMarkerSortedList) sourceList;
      this.list[destinationIndex] = markerSortedList.list[sourceIndex];
    }

    public void Add(DocumentNodeMarker marker)
    {
      this.list.Insert(~this.FindPosition(marker), marker);
    }

    public int AddCoalesced(DocumentNodeMarker marker)
    {
      int countContained;
      int coalescePoint;
      if ((coalescePoint = this.FindCoalescePoint(marker, out countContained)) >= 0)
      {
        this.list.RemoveRange(coalescePoint, countContained);
        this.list.Insert(coalescePoint, marker);
      }
      return coalescePoint;
    }
  }
}

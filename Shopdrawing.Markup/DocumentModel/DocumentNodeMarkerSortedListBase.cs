// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeMarkerSortedListBase
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public abstract class DocumentNodeMarkerSortedListBase
  {
    public abstract int Count { get; }

    public IEnumerable<DocumentNodeMarker> Markers
    {
      get
      {
        for (int i = 0; i < this.Count; ++i)
          yield return this.MarkerAt(i);
      }
    }

    public abstract DocumentNodeMarker MarkerAt(int index);

    public abstract void Clear();

    public abstract void RemoveRange(int index, int count);

    public abstract void Copy(int destinationIndex, DocumentNodeMarkerSortedListBase sourceList, int sourceIndex);

    public bool FindDescendantRange(DocumentNodeMarker ancestor, out int startIndex, out int endIndex)
    {
      return this.FindDescendantRangeWorker(ancestor, out startIndex, out endIndex, false);
    }

    public bool FindSelfAndDescendantRange(DocumentNodeMarker ancestor, out int startIndex, out int endIndex)
    {
      return this.FindDescendantRangeWorker(ancestor, out startIndex, out endIndex, true);
    }

    private bool FindDescendantRangeWorker(DocumentNodeMarker ancestor, out int startIndex, out int endIndex, bool includeAncestor)
    {
      int position = this.FindPosition(ancestor, 0, -1);
      int num1 = position >= 0 ? position + (includeAncestor ? 0 : 1) : ~position;
      int count = this.Count;
      int index;
      for (index = num1; index < count; ++index)
      {
        DocumentNodeMarker documentNodeMarker = this.MarkerAt(index);
        if (!documentNodeMarker.IsContainedBy(ancestor) && documentNodeMarker != ancestor)
          break;
      }
      int num2 = index - num1;
      if (num2 > 0)
      {
        startIndex = num1;
        endIndex = startIndex + num2 - 1;
        return true;
      }
      startIndex = -1;
      endIndex = -1;
      return false;
    }

    public IEnumerable<int> FindDescendants(DocumentNodeMarker ancestor)
    {
      int startIndex;
      int endIndex;
      if (this.FindDescendantRange(ancestor, out startIndex, out endIndex))
      {
        for (int i = startIndex; i <= endIndex; ++i)
          yield return i;
      }
    }

    public IEnumerable<int> FindSelfAndDescendants(DocumentNodeMarker ancestor)
    {
      int startIndex;
      int endIndex;
      if (this.FindSelfAndDescendantRange(ancestor, out startIndex, out endIndex))
      {
        for (int i = startIndex; i <= endIndex; ++i)
          yield return i;
      }
    }

    public int FindPosition(DocumentNodeMarker marker)
    {
      return this.FindPosition(marker, 0, -1);
    }

    public int FindNormalizedPosition(DocumentNodeMarker marker)
    {
      int position = this.FindPosition(marker);
      if (position >= 0)
        return position;
      return ~position;
    }

    public int FindPosition(DocumentNodeMarker marker, int startIndex, int endIndex)
    {
      int firstPosition = this.FindFirstPosition(marker, startIndex, endIndex);
      if (firstPosition < 0)
        return firstPosition;
      int index = firstPosition;
      DocumentNodeMarker documentNodeMarker = this.MarkerAt(index);
      while (documentNodeMarker != marker)
      {
        if (documentNodeMarker.IsContainedBy(marker))
          return ~index;
        ++index;
        if (index < this.Count)
        {
          documentNodeMarker = this.MarkerAt(index);
          if (documentNodeMarker.CompareTo((object) marker) == 0)
            continue;
        }
        return ~firstPosition;
      }
      return index;
    }

    public int FindFirstPosition(DocumentNodeMarker marker)
    {
      return this.FindFirstPosition(marker, 0, -1);
    }

    public void CullDeletedMarkers()
    {
      int num = 0;
      for (int index = 0; index < this.Count; ++index)
      {
        if (!this.MarkerAt(index).IsDeleted)
        {
          if (num < index)
            this.Copy(num, this, index);
          ++num;
        }
      }
      this.RemoveRange(num, this.Count - num);
    }

    private int FindAncestorWorker(DocumentNodeMarker marker, bool stopAtFirst)
    {
      int num = 0;
      if (marker.Parent != null)
      {
        num = this.FindAncestorWorker(marker.Parent, stopAtFirst);
        if (num >= 0 && stopAtFirst)
          return num;
      }
      int startIndex = num < 0 ? ~num : num;
      return this.FindPosition(marker, startIndex, -1);
    }

    public int FindFarthestAncestor(DocumentNodeMarker marker)
    {
      if (marker.Parent == null)
        return -1;
      return this.FindAncestorWorker(marker.Parent, true);
    }

    public int FindNearestAncestor(DocumentNodeMarker marker)
    {
      if (marker.Parent == null)
        return -1;
      return this.FindAncestorWorker(marker.Parent, false);
    }

    public IEnumerable<int> FindSelfAndDescendantsCoalesced(DocumentNodeMarker ancestor)
    {
      int index = this.FindPosition(ancestor);
      if (index < 0)
      {
        index = ~index;
        int lastReturnedIndex = -1;
        for (; index < this.Count && this.MarkerAt(index).IsContainedBy(ancestor); ++index)
        {
          if (lastReturnedIndex == -1 || !this.MarkerAt(lastReturnedIndex).Contains(this.MarkerAt(index)))
          {
            lastReturnedIndex = index;
            yield return index;
          }
        }
      }
      else
        yield return index;
    }

    public int FindFirstPosition(DocumentNodeMarker marker, int startIndex, int endIndex)
    {
      int num1 = startIndex;
      int num2 = endIndex < 0 || endIndex >= this.Count ? this.Count - 1 : endIndex;
      while (num1 <= num2)
      {
        int index = (num1 + num2) / 2;
        DocumentNodeMarker documentNodeMarker = this.MarkerAt(index);
        int num3 = marker.CompareTo((object) documentNodeMarker);
        if (num3 == 0)
        {
          if (marker.IsDeleted)
          {
            while (index > startIndex && marker.CompareTo((object) this.MarkerAt(index - 1)) == 0)
              --index;
          }
          return index;
        }
        if (num3 < 0)
          num2 = index - 1;
        else
          num1 = index + 1;
      }
      return ~num1;
    }

    protected int FindCoalescePoint(DocumentNodeMarker marker, out int countContained)
    {
      if (this.Count == 0)
      {
        countContained = 0;
        return 0;
      }
      int farthestAncestor = this.FindFarthestAncestor(marker);
      if (farthestAncestor >= 0)
      {
        countContained = 0;
        return ~farthestAncestor;
      }
      int num = ~this.FindPosition(marker, ~farthestAncestor, -1);
      if (num < 0)
      {
        countContained = 0;
        return num;
      }
      countContained = 0;
      while (num + countContained < this.Count && this.MarkerAt(num + countContained).IsContainedBy(marker))
        ++countContained;
      return num;
    }

    public void Remove(DocumentNodeMarker marker)
    {
      int position = this.FindPosition(marker);
      if (position < 0)
        return;
      this.RemoveRange(position, 1);
    }

    public void RemoveSelfAndDescendants(DocumentNodeMarker ancestor)
    {
      int count = 0;
      int normalizedPosition;
      for (normalizedPosition = this.FindNormalizedPosition(ancestor); normalizedPosition + count < this.Count; ++count)
      {
        DocumentNodeMarker documentNodeMarker = this.MarkerAt(normalizedPosition + count);
        if (!documentNodeMarker.IsContainedBy(ancestor) && documentNodeMarker != ancestor)
          break;
      }
      if (count <= 0)
        return;
      this.RemoveRange(normalizedPosition, count);
    }

    public void RemoveSelfAndDescendants(int index)
    {
      int count = 0;
      DocumentNodeMarker marker = this.MarkerAt(index);
      for (; index + count < this.Count; ++count)
      {
        DocumentNodeMarker documentNodeMarker = this.MarkerAt(index + count);
        if (!documentNodeMarker.IsContainedBy(marker) && documentNodeMarker != marker)
          break;
      }
      this.RemoveRange(index, count);
    }

    public IEnumerable<DocumentNodeMarkerSortedListBase.IntersectionResult> Intersect(DocumentNodeMarkerSortedListBase rhs, DocumentNodeMarkerSortedListBase.Flags flags)
    {
      int thisCur = 0;
      int rhsCur = 0;
      for (; thisCur < this.Count; ++thisCur)
      {
        while (rhsCur < rhs.Count && thisCur < this.Count && rhs.MarkerAt(rhsCur).CompareTo((object) this.MarkerAt(thisCur)) < 0)
        {
          bool wasContained = false;
          if ((flags & DocumentNodeMarkerSortedListBase.Flags.ContainedBy) != DocumentNodeMarkerSortedListBase.Flags.None)
          {
            int rhsLookahead;
            for (rhsLookahead = rhsCur; rhsLookahead < rhs.Count && rhs.MarkerAt(rhsLookahead).Contains(this.MarkerAt(thisCur)); ++rhsLookahead)
              yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, rhsLookahead, DocumentNodeMarkerSortedListBase.Flags.ContainedBy);
            if (rhsLookahead != rhsCur)
              wasContained = true;
          }
          if (wasContained)
            ++thisCur;
          else
            ++rhsCur;
        }
        if (rhsCur >= rhs.Count || thisCur >= this.Count)
          break;
        if ((flags & DocumentNodeMarkerSortedListBase.Flags.Equals) != DocumentNodeMarkerSortedListBase.Flags.None && rhs.MarkerAt(rhsCur).CompareTo((object) this.MarkerAt(thisCur)) == 0)
          yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, rhsCur, DocumentNodeMarkerSortedListBase.Flags.Equals);
        else if ((flags & DocumentNodeMarkerSortedListBase.Flags.Contains) != DocumentNodeMarkerSortedListBase.Flags.None)
        {
          for (int rhsLookahead = rhsCur; rhsLookahead < rhs.Count && this.MarkerAt(thisCur).Contains(rhs.MarkerAt(rhsLookahead)); ++rhsLookahead)
            yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, rhsLookahead, DocumentNodeMarkerSortedListBase.Flags.Contains);
        }
      }
    }

    public IEnumerable<DocumentNodeMarkerSortedListBase.IntersectionResult> IntersectIdentity(DocumentNodeMarkerSortedListBase rhs)
    {
      int thisCur = 0;
      int rhsCur = 0;
      for (; thisCur < this.Count; ++thisCur)
      {
        if (rhs.MarkerAt(rhsCur).CompareTo((object) this.MarkerAt(thisCur)) <= 0)
        {
          rhsCur = rhs.FindNormalizedPosition(this.MarkerAt(thisCur));
          if (rhsCur >= rhs.Count)
            break;
          if (rhs.MarkerAt(rhsCur) == this.MarkerAt(thisCur))
            yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, rhsCur, DocumentNodeMarkerSortedListBase.Flags.Equals);
        }
      }
    }

    public IEnumerable<DocumentNodeMarkerSortedListBase.IntersectionResult> UnionIdentity(DocumentNodeMarkerSortedListBase rhs)
    {
      int thisCur = 0;
      int rhsCur = 0;
      for (; thisCur < this.Count; ++thisCur)
      {
        int currentResult = 1;
        for (; rhsCur < rhs.Count && (currentResult = rhs.MarkerAt(rhsCur).CompareTo((object) this.MarkerAt(thisCur))) < 0; ++rhsCur)
          yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(-1, rhsCur, DocumentNodeMarkerSortedListBase.Flags.Equals);
        if (rhsCur < rhs.Count)
        {
          if (currentResult == 0)
          {
            yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, rhsCur, DocumentNodeMarkerSortedListBase.Flags.Equals);
            ++rhsCur;
          }
          else
            yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, -1, DocumentNodeMarkerSortedListBase.Flags.Equals);
        }
        else
          yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(thisCur, -1, DocumentNodeMarkerSortedListBase.Flags.Equals);
      }
      for (; rhsCur < rhs.Count; ++rhsCur)
        yield return new DocumentNodeMarkerSortedListBase.IntersectionResult(-1, rhsCur, DocumentNodeMarkerSortedListBase.Flags.Equals);
    }

    public bool IsCoalesced()
    {
      for (int index = 1; index < this.Count; ++index)
      {
        if (this.MarkerAt(index - 1).Contains(this.MarkerAt(index)))
          return false;
      }
      return true;
    }

    [System.Flags]
    public enum Flags
    {
      None = 0,
      ContainedBy = 1,
      Contains = 2,
      Equals = 4,
      All = Equals | Contains | ContainedBy,
    }

    public struct IntersectionResult
    {
      private int leftHandSideIndex;
      private int rightHandSideIndex;
      private DocumentNodeMarkerSortedListBase.Flags flags;

      public int LeftHandSideIndex
      {
        get
        {
          return this.leftHandSideIndex;
        }
        set
        {
          this.leftHandSideIndex = value;
        }
      }

      public int RightHandSideIndex
      {
        get
        {
          return this.rightHandSideIndex;
        }
        set
        {
          this.rightHandSideIndex = value;
        }
      }

      public DocumentNodeMarkerSortedListBase.Flags Flags
      {
        get
        {
          return this.flags;
        }
        set
        {
          this.flags = value;
        }
      }

      public IntersectionResult(int leftHandSideIndex, int rightHandSideIndex, DocumentNodeMarkerSortedListBase.Flags flags)
      {
        this.leftHandSideIndex = leftHandSideIndex;
        this.rightHandSideIndex = rightHandSideIndex;
        this.flags = flags;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeMarker
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeMarker : IComparable
  {
    private static readonly int noChildIndex = int.MinValue;
    private static int compareArraySize = 500;
    private static DocumentNodeMarker[] lhsCompareArray = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
    private static DocumentNodeMarker[] rhsCompareArray = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
    private int childIndex = DocumentNodeMarker.noChildIndex;
    private DocumentNodeMarker.Flags flags;
    private DocumentNodeMarker parentMarker;
    private DocumentNodeMarker previousSiblingMarker;
    private DocumentNode node;
    private IProperty property;

    public IDocumentContext DocumentContext
    {
      get
      {
        if (this.node != null)
          return this.node.Context;
        if (this.Parent != null)
          return this.Parent.DocumentContext;
        return (IDocumentContext) null;
      }
    }

    public DocumentNode Node
    {
      get
      {
        return this.node;
      }
    }

    public bool IsDeleted
    {
      get
      {
        return this.Node == null;
      }
    }

    public DocumentNodeMarker Parent
    {
      get
      {
        return this.parentMarker;
      }
    }

    public bool IsChild
    {
      get
      {
        return this.childIndex != DocumentNodeMarker.noChildIndex;
      }
    }

    public bool IsProperty
    {
      get
      {
        return this.property != null;
      }
    }

    public bool IsRootNode
    {
      get
      {
        return this.property == null && this.childIndex == DocumentNodeMarker.noChildIndex;
      }
    }

    public int ChildIndex
    {
      get
      {
        this.ValidateChildIndex();
        return this.childIndex;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.property;
      }
    }

    public DocumentNodeMarker NearestUndeletedAncestor
    {
      get
      {
        DocumentNodeMarker documentNodeMarker = this.parentMarker;
        while (documentNodeMarker != null && documentNodeMarker.Node == null)
          documentNodeMarker = documentNodeMarker.Parent;
        return documentNodeMarker;
      }
    }

    internal DocumentNodeMarker(DocumentNode node)
    {
      this.node = node;
      if (node.Parent == null)
        return;
      this.parentMarker = node.Parent.Marker;
      if (node.IsChild)
        return;
      this.property = node.SitePropertyKey;
    }

    private void ValidateChildIndex()
    {
      if (!this.IsChild)
        return;
      if (this.node != null)
        ((DocumentCompositeNode) this.parentMarker.Node).ValidateChildIndices(this.childIndex);
      else if (this.parentMarker.IsDeleted)
      {
        if (this.previousSiblingMarker == null)
          return;
        this.previousSiblingMarker.ValidateChildIndex();
        this.childIndex = (this.previousSiblingMarker.flags & DocumentNodeMarker.Flags.DeletedBeforeParent) == DocumentNodeMarker.Flags.None ? this.previousSiblingMarker.childIndex + 1 : this.previousSiblingMarker.childIndex;
        this.previousSiblingMarker = (DocumentNodeMarker) null;
      }
      else
      {
        ((DocumentCompositeNode) this.parentMarker.Node).ValidateChildIndices(-1);
        for (; this.previousSiblingMarker != null && this.previousSiblingMarker.IsDeleted; this.previousSiblingMarker = this.previousSiblingMarker.previousSiblingMarker)
          this.childIndex = this.previousSiblingMarker.childIndex;
        if (this.previousSiblingMarker == null)
          return;
        this.childIndex = this.previousSiblingMarker.childIndex + 1;
      }
    }

    internal void FixupChildIndex(int newIndex, DocumentNodeMarker previousSiblingMarker)
    {
      this.childIndex = newIndex;
      this.previousSiblingMarker = previousSiblingMarker;
    }

    public bool Contains(DocumentNodeMarker marker)
    {
      return marker.IsContainedBy(this);
    }

    public bool IsContainedBy(DocumentNodeMarker marker)
    {
      for (DocumentNodeMarker documentNodeMarker = this.parentMarker; documentNodeMarker != null; documentNodeMarker = documentNodeMarker.parentMarker)
      {
        if (documentNodeMarker == marker)
          return true;
      }
      return false;
    }

    public DocumentNodeMarker GetCommonAncestor(DocumentNodeMarker other)
    {
      DocumentNodeMarker lastCommonAncestor;
      DocumentNodeMarker.CompareToWorker(this, other, out lastCommonAncestor);
      return lastCommonAncestor;
    }

    public DocumentNodeMarker GetCommonUndeletedAncestor(DocumentNodeMarker other)
    {
      DocumentNodeMarker documentNodeMarker = this.GetCommonAncestor(other);
      while (documentNodeMarker != null && documentNodeMarker.Node != null)
        documentNodeMarker = documentNodeMarker.Parent;
      return documentNodeMarker;
    }

    public Stack<DocumentNodeMarker> GetParentChain()
    {
      Stack<DocumentNodeMarker> stack = new Stack<DocumentNodeMarker>();
      for (DocumentNodeMarker documentNodeMarker = this; documentNodeMarker != null; documentNodeMarker = documentNodeMarker.Parent)
        stack.Push(documentNodeMarker);
      return stack;
    }

    internal void SetDeleted()
    {
      this.node = (DocumentNode) null;
      if (this.parentMarker != null && this.parentMarker.node != null)
        this.flags |= DocumentNodeMarker.Flags.DeletedBeforeParent;
      else
        this.previousSiblingMarker = (DocumentNodeMarker) null;
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      DocumentNodeMarker lastCommonAncestor;
      return DocumentNodeMarker.CompareToWorker(this, (DocumentNodeMarker) obj, out lastCommonAncestor);
    }

    private static void ClearCompareToWorkerCache(int lhLength, int rhLength)
    {
      for (int index = 0; index < lhLength; ++index)
        DocumentNodeMarker.lhsCompareArray[index] = (DocumentNodeMarker) null;
      for (int index = 0; index < rhLength; ++index)
        DocumentNodeMarker.rhsCompareArray[index] = (DocumentNodeMarker) null;
    }

    private static int CompareToWorker(DocumentNodeMarker lhs, DocumentNodeMarker rhs, out DocumentNodeMarker lastCommonAncestor)
    {
      lastCommonAncestor = (DocumentNodeMarker) null;
      if (lhs == rhs)
      {
        lastCommonAncestor = lhs;
        return 0;
      }
      DocumentNodeMarker documentNodeMarker1 = lhs;
      int num1 = 0;
      for (; documentNodeMarker1 != null; documentNodeMarker1 = documentNodeMarker1.parentMarker)
      {
        if (documentNodeMarker1 == rhs)
        {
          lastCommonAncestor = rhs;
          for (int index = 0; index < num1 + 1; ++index)
            DocumentNodeMarker.lhsCompareArray[index] = (DocumentNodeMarker) null;
          return !lastCommonAncestor.IsDeleted ? 1 : 0;
        }
        if (num1 == DocumentNodeMarker.compareArraySize)
        {
          DocumentNodeMarker.compareArraySize = DocumentNodeMarker.compareArraySize * 3 / 2;
          DocumentNodeMarker[] documentNodeMarkerArray1 = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
          DocumentNodeMarker[] documentNodeMarkerArray2 = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
          for (int index = 0; index < num1; ++index)
            documentNodeMarkerArray1[index] = DocumentNodeMarker.lhsCompareArray[index];
          DocumentNodeMarker.lhsCompareArray = documentNodeMarkerArray1;
          DocumentNodeMarker.rhsCompareArray = documentNodeMarkerArray2;
        }
        DocumentNodeMarker.lhsCompareArray[num1++] = documentNodeMarker1;
      }
      DocumentNodeMarker documentNodeMarker2 = rhs;
      int num2 = 0;
      for (; documentNodeMarker2 != null; documentNodeMarker2 = documentNodeMarker2.parentMarker)
      {
        if (documentNodeMarker2 == lhs)
        {
          lastCommonAncestor = lhs;
          DocumentNodeMarker.ClearCompareToWorkerCache(num1 + 1, num2 + 1);
          return !lastCommonAncestor.IsDeleted ? -1 : 0;
        }
        if (num2 == DocumentNodeMarker.compareArraySize)
        {
          DocumentNodeMarker.compareArraySize = DocumentNodeMarker.compareArraySize * 3 / 2;
          DocumentNodeMarker[] documentNodeMarkerArray1 = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
          DocumentNodeMarker[] documentNodeMarkerArray2 = new DocumentNodeMarker[DocumentNodeMarker.compareArraySize];
          for (int index = 0; index < num1; ++index)
            documentNodeMarkerArray1[index] = DocumentNodeMarker.lhsCompareArray[index];
          for (int index = 0; index < num2; ++index)
            documentNodeMarkerArray2[index] = DocumentNodeMarker.rhsCompareArray[index];
          DocumentNodeMarker.lhsCompareArray = documentNodeMarkerArray1;
          DocumentNodeMarker.rhsCompareArray = documentNodeMarkerArray2;
        }
        DocumentNodeMarker.rhsCompareArray[num2++] = documentNodeMarker2;
      }
      while (num1 > 0 && num2 > 0)
      {
        DocumentNodeMarker lhs1 = DocumentNodeMarker.lhsCompareArray[--num1];
        DocumentNodeMarker.lhsCompareArray[num1 + 1] = (DocumentNodeMarker) null;
        DocumentNodeMarker rhs1 = DocumentNodeMarker.rhsCompareArray[--num2];
        DocumentNodeMarker.rhsCompareArray[num2 + 1] = (DocumentNodeMarker) null;
        int num3 = DocumentNodeMarker.CompareToSingleNode(lhs1, rhs1);
        if (num3 != 0)
        {
          DocumentNodeMarker.ClearCompareToWorkerCache(num1 + 1, num2 + 1);
          return num3;
        }
        if (lhs1 == rhs1)
          lastCommonAncestor = lhs1;
        else if (lhs1.IsDeleted && rhs1.IsDeleted)
        {
          DocumentNodeMarker.ClearCompareToWorkerCache(num1 + 1, num2 + 1);
          return 0;
        }
      }
      DocumentNodeMarker.ClearCompareToWorkerCache(num1 + 1, num2 + 1);
      if (num1 > 0)
        return 1;
      return num2 > 0 ? -1 : 0;
    }

    private static int CompareToSingleNode(DocumentNodeMarker lhs, DocumentNodeMarker rhs)
    {
      if (lhs == rhs || lhs.parentMarker != null && lhs.parentMarker.IsDeleted && (rhs.parentMarker != null && rhs.parentMarker.IsDeleted))
        return 0;
      lhs.ValidateChildIndex();
      rhs.ValidateChildIndex();
      if (lhs.property != null != (rhs.property != null))
        return lhs.property != null ? -1 : 1;
      int num1 = lhs.property != null ? lhs.property.SortValue : lhs.childIndex;
      int num2 = rhs.property != null ? rhs.property.SortValue : rhs.childIndex;
      if (num1 == num2)
      {
        if (lhs.IsDeleted && rhs.IsDeleted)
          return 0;
        return lhs.IsDeleted ? -1 : 1;
      }
      return num1 < num2 ? -1 : 1;
    }

    public override bool Equals(object obj)
    {
      return this.CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      string str = "Node=" + (this.node != null ? this.node.ToString() : "<deleted>") + ", ";
      return !this.IsProperty ? str + (object) "index=" + (string) (object) this.childIndex : str + (object) "property=" + (string) (object) this.property.SortValue;
    }

    internal void InitializeChildIndex()
    {
      if (this.node.Parent == null || !this.node.IsChild)
        return;
      this.childIndex = this.node.SiteChildIndex;
      this.previousSiblingMarker = this.node.SiteChildIndex > 0 ? this.node.Parent.Children[this.node.SiteChildIndex - 1].Marker : (DocumentNodeMarker) null;
    }

    [System.Flags]
    private enum Flags
    {
      None = 0,
      DeletedBeforeParent = 1,
    }
  }
}

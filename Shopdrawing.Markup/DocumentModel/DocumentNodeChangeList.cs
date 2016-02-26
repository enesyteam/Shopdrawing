// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeChangeList
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeChangeList : DocumentNodeMarkerSortedListOf<DocumentNodeChange>
  {
    public IEnumerable<DocumentNodeChange> DistinctChanges
    {
      get
      {
        for (int i = 0; i < this.Count; ++i)
        {
          if (this.MarkerAt(i).IsDeleted || this.ValueAt(i).Action != DocumentNodeChangeAction.Replace)
            yield return this.ValueAt(i);
        }
      }
    }

    public IEnumerable<DocumentNodeChange> CollapsedChangeList
    {
      get
      {
        for (int i = 0; i < this.Count; ++i)
        {
          if (this.FindFarthestAncestor(this.MarkerAt(i)) < 0)
          {
            DocumentNodeChange changeToReturn = this.ValueAt(i);
            for (int index = i + 1; index < this.Count && changeToReturn.ParentNode == this.ValueAt(index).ParentNode; ++index)
            {
              if (changeToReturn.IsPropertyChange && changeToReturn.PropertyKey == this.ValueAt(index).PropertyKey)
                changeToReturn = new DocumentNodeChange(changeToReturn.ParentNode, changeToReturn.PropertyKey, changeToReturn.OldChildNode, changeToReturn.ParentNode.Properties[(IPropertyId) changeToReturn.PropertyKey]);
              else if (changeToReturn.IsRootNodeChange)
                changeToReturn = new DocumentNodeChange(changeToReturn.OldChildNode, this.ValueAt(index).NewChildNode);
              else
                break;
              ++i;
            }
            yield return changeToReturn;
          }
        }
      }
    }

    public bool Contains(DocumentNode node)
    {
      if (this.FindPosition(node.Marker) < 0)
        return this.FindFarthestAncestor(node.Marker) >= 0;
      return true;
    }
  }
}

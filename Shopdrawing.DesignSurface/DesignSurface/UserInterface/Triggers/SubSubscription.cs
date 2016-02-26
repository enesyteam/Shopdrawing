// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.SubSubscription
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class SubSubscription
  {
    private DocumentNodeChangeList accumulatedChanges = new DocumentNodeChangeList();

    public void AddChange(DocumentNodeMarker marker, DocumentNodeChange change, IList<SubSubscription> delayedUpdates)
    {
      if (this.accumulatedChanges.Count == 0)
        delayedUpdates.Add(this);
      this.accumulatedChanges.Add(marker, change);
    }

    public void ProcessChanges(uint documentChangeStamp)
    {
      if (this.accumulatedChanges.Count <= 0)
        return;
      this.Update(this.accumulatedChanges, documentChangeStamp);
      this.accumulatedChanges.Clear();
    }

    protected virtual void Update(DocumentNodeChangeList changes, uint documentChangeStamp)
    {
    }
  }
}

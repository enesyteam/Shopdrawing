// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataChangeProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataChangeProcessor : AsyncDataXamlProcessor
  {
    private int pass;
    private bool allChangesCollected;
    private SampleDataDocumentChangeProcessor documentProcessor;

    public SampleDataSet SampleData
    {
      get
      {
        return this.documentProcessor.SampleData;
      }
    }

    public override int CompletedCount
    {
      get
      {
        if (this.IsCollectingChanges)
          return base.CompletedCount + base.Count * this.pass;
        return base.CompletedCount;
      }
    }

    public override int Count
    {
      get
      {
        if (this.IsCollectingChanges)
          return base.Count * (this.pass + 1);
        return base.Count;
      }
    }

    public override bool ShouldProcessCurrentDocument
    {
      get
      {
        if (this.documentProcessor.RenameChangesCount == 0 && this.CurrentDocument.Document == null || PathHelper.ArePathsEquivalent(this.CurrentDocument.Path, this.SampleData.XamlFilePath))
          return false;
        return base.ShouldProcessCurrentDocument;
      }
    }

    public SampleDataChangeProcessor(IAsyncMechanism asyncMechanism, SampleDataSet dataSet, IList<SampleDataChange> normalizedChanges, ChangeProcessingModes processingMode)
      : base(asyncMechanism, (IProjectContext) dataSet.ProjectContext, processingMode)
    {
      this.documentProcessor = new SampleDataDocumentChangeProcessor(this, dataSet, normalizedChanges);
    }

    public static bool ShouldProcessChanges(IList<SampleDataChange> normalizedChanges)
    {
      return EnumerableExtensions.CountIsMoreThan<SampleDataChange>(Enumerable.Where<SampleDataChange>((IEnumerable<SampleDataChange>) normalizedChanges, (Func<SampleDataChange, bool>) (change => change is SamplePropertyRenamed)), 0);
    }

    protected override bool MoveNext()
    {
      if (this.IsKilled)
        return false;
      bool flag = base.MoveNext();
      if (!flag && this.IsCollectingChanges && !this.allChangesCollected)
      {
        this.allChangesCollected = this.documentProcessor.AllChangesCollected;
        ++this.pass;
        this.Reset();
        flag = base.MoveNext();
        if (this.pass > 16)
          this.allChangesCollected = true;
      }
      return flag;
    }

    protected override void Work()
    {
      if (this.IsKilled || !this.ShouldProcessCurrentDocument)
        return;
      SceneDocument sceneDocument = this.GetSceneDocument(this.CurrentDocument, true);
      if (sceneDocument == null)
        return;
      if (this.IsCollectingChanges)
      {
        if (this.pass == 0)
          this.documentProcessor.ProcessDocument(sceneDocument, DataBindingProcessingOptions.FirstPass);
        else
          this.documentProcessor.ProcessDocument(sceneDocument, DataBindingProcessingOptions.SecondPass);
      }
      if (!this.IsApplyingChanges || !this.allChangesCollected && this.IsCollectingChanges)
        return;
      this.documentProcessor.ApplyChanges(sceneDocument);
      this.documentProcessor.InvalidateNodes(sceneDocument);
    }
  }
}

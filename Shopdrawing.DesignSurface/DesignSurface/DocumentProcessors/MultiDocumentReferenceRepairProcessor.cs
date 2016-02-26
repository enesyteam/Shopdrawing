// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.MultiDocumentReferenceRepairProcessor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal abstract class MultiDocumentReferenceRepairProcessor : DocumentProcessor
  {
    private MultiDocumentReferenceChangeModel multiDocumentReferenceChangeModel;
    private List<SceneDocument> documentsToClose;

    public bool Cancelled { get; private set; }

    protected override DocumentSearchScope SearchScope
    {
      get
      {
        return this.multiDocumentReferenceChangeModel.ChangeScope;
      }
    }

    protected MultiDocumentReferenceChangeModel MultiDocumentReferenceChangeModel
    {
      get
      {
        return this.multiDocumentReferenceChangeModel;
      }
    }

    protected abstract string UndoDescription { get; }

    public MultiDocumentReferenceRepairProcessor(DesignerContext designerContext, MultiDocumentReferenceChangeModel multiDocumentReferenceChangeModel)
      : base(designerContext, (IAsyncMechanism) new SynchronousAsyncMechanism())
    {
      this.multiDocumentReferenceChangeModel = multiDocumentReferenceChangeModel;
      this.documentsToClose = new List<SceneDocument>();
      this.Cancelled = false;
      this.Complete += new EventHandler(this.OnComplete);
    }

    protected override void ProcessDocument(SceneDocument document)
    {
      DocumentNode rootNode = document.DocumentRoot.RootNode;
      foreach (ReferenceRepairer referenceRepairer in this.multiDocumentReferenceChangeModel.ReferenceRepairers)
      {
        foreach (DocumentNode node in rootNode.SelectDescendantNodes(referenceRepairer.AppliesTo))
          referenceRepairer.Repair(node);
      }
    }

    protected abstract bool ShouldUpdateWithExternalChanges(List<ChangedNodeInfo> coalescedNodes);

    private void OnComplete(object sender, EventArgs e)
    {
      if (this.multiDocumentReferenceChangeModel.NodesForExternalUpdate.Count > 0)
      {
        List<ChangedNodeInfo> coalescedNodes = new List<ChangedNodeInfo>();
        coalescedNodes.AddRange((IEnumerable<ChangedNodeInfo>) this.multiDocumentReferenceChangeModel.NodesForLocalUpdate);
        coalescedNodes.AddRange((IEnumerable<ChangedNodeInfo>) this.multiDocumentReferenceChangeModel.NodesForExternalUpdate);
        if (this.ShouldUpdateWithExternalChanges(coalescedNodes))
        {
          this.UpdateLocalNodes();
          this.UpdateExternalNodes();
        }
        else
          this.Cancelled = true;
      }
      else if (this.multiDocumentReferenceChangeModel.NodesForLocalUpdate.Count > 0)
        this.UpdateLocalNodes();
      foreach (SceneDocument document in this.documentsToClose)
        base.CloseDocument(document);
    }

    protected override void CloseDocument(SceneDocument document)
    {
      this.documentsToClose.Add(document);
    }

    protected abstract void ApplyChange(ChangedNodeInfo changedNodeInfo);

    private void UpdateLocalNodes()
    {
      foreach (ChangedNodeInfo changedNodeInfo in (IEnumerable<ChangedNodeInfo>) this.multiDocumentReferenceChangeModel.NodesForLocalUpdate)
        this.ApplyChange(changedNodeInfo);
    }

    private void UpdateExternalNodes()
    {
      ISceneViewHost sceneViewHost = (ISceneViewHost) this.multiDocumentReferenceChangeModel.ProjectContext.GetService(typeof (ISceneViewHost));
      foreach (IGrouping<IDocumentRoot, ChangedNodeInfo> grouping in Enumerable.Select<IGrouping<IDocumentRoot, ChangedNodeInfo>, IGrouping<IDocumentRoot, ChangedNodeInfo>>(Enumerable.GroupBy<ChangedNodeInfo, IDocumentRoot>((IEnumerable<ChangedNodeInfo>) this.multiDocumentReferenceChangeModel.NodesForExternalUpdate, (Func<ChangedNodeInfo, IDocumentRoot>) (changeInfo => changeInfo.Node.DocumentRoot)), (Func<IGrouping<IDocumentRoot, ChangedNodeInfo>, IGrouping<IDocumentRoot, ChangedNodeInfo>>) (documentGroup => documentGroup)))
      {
        SceneView sceneView = sceneViewHost.OpenView(grouping.Key, false);
        SceneViewModel sceneViewModel = sceneView != null ? sceneView.ViewModel : (SceneViewModel) null;
        using (SceneEditTransaction editTransaction = sceneViewModel.CreateEditTransaction(this.UndoDescription))
        {
          if (sceneViewModel != null)
          {
            foreach (ChangedNodeInfo changedNodeInfo in (IEnumerable<ChangedNodeInfo>) grouping)
              this.ApplyChange(changedNodeInfo);
          }
          if (this.documentsToClose.Contains(sceneViewModel.Document))
            this.documentsToClose.Remove(sceneViewModel.Document);
          editTransaction.Commit();
        }
      }
    }
  }
}

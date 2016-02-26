// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ExternalOpenSceneResourceReferenceAnalyzer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ExternalOpenSceneResourceReferenceAnalyzer : AsyncProcess
  {
    private int currentViewIndex = -1;
    private ReferencesFoundModel model;

    public override int Count
    {
      get
      {
        return this.model.ResourceEntryNode.ViewModel.DesignerContext.ViewService.Views.Count;
      }
    }

    public override int CompletedCount
    {
      get
      {
        return Math.Max(this.currentViewIndex, 0);
      }
    }

    public override string StatusText
    {
      get
      {
        return StringTable.ReferencesFixupGenericStatusText;
      }
    }

    public ExternalOpenSceneResourceReferenceAnalyzer(ReferencesFoundModel model)
      : base((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background))
    {
      this.model = model;
    }

    public override void Reset()
    {
      this.currentViewIndex = -1;
    }

    protected override void Work()
    {
      IViewCollection views = this.model.ResourceEntryNode.ViewModel.DesignerContext.ViewService.Views;
      if (this.currentViewIndex < 0 || this.currentViewIndex >= views.Count)
        return;
      SceneView sceneView = views[this.currentViewIndex] as SceneView;
      if (sceneView == null)
        return;
      SceneViewModel viewModel = sceneView.ViewModel;
      if (viewModel == this.model.ResourceEntryNode.ViewModel || !viewModel.Document.IsEditable)
        return;
      List<SceneNode> list = new List<SceneNode>();
      viewModel.FindInternalResourceReferences((DocumentCompositeNode) this.model.ResourceEntryNode.DocumentNode, (ICollection<SceneNode>) list);
      foreach (SceneNode node in list)
      {
        this.model.ReferenceNames.Add(ReferencesFoundModel.SceneNodeElementName(node));
        this.model.AddReferencesFile(sceneView.ViewModel.Document);
      }
    }

    protected override bool MoveNext()
    {
      return ++this.currentViewIndex < this.Count;
    }
  }
}

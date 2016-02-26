// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ResourceAssetAggregator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class ResourceAssetAggregator : AssetAggregator
  {
    private DesignerContext designerContext;
    private uint resourceChangeStamp;
    private SceneDocument currentDocument;
    private SelectionManager selectionManager;
    private IDocumentService documentService;

    public ResourceAssetAggregator(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.selectionManager = this.designerContext.SelectionManager;
      this.selectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.documentService = this.designerContext.DocumentService;
      this.documentService.ActiveDocumentChanged += new DocumentChangedEventHandler(this.DocumentService_ActiveDocumentChanged);
      this.AddProvider((AssetProvider) new StyleAssetProvider());
      this.NeedsUpdate = true;
    }

    protected override void InternalDispose(bool disposing)
    {
      base.InternalDispose(disposing);
      if (!disposing)
        return;
      this.selectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.documentService.ActiveDocumentChanged -= new DocumentChangedEventHandler(this.DocumentService_ActiveDocumentChanged);
      this.selectionManager = (SelectionManager) null;
      this.documentService = (IDocumentService) null;
      this.designerContext = (DesignerContext) null;
    }

    protected override bool UpdateAssets()
    {
      foreach (ResourceAssetProvider resourceAssetProvider in (IEnumerable<AssetProvider>) this.AssetProviders)
      {
        if (resourceAssetProvider.Assets.Count > 0)
        {
          resourceAssetProvider.Assets.Clear();
          this.NeedsUpdate = true;
        }
      }
      this.currentDocument = this.designerContext.ActiveDocument;
      if (this.designerContext.ActiveDocument != null && this.designerContext.ActiveDocument.XamlDocument != null)
      {
        IEnumerable<IDocumentRoot> source = (IEnumerable<IDocumentRoot>) null;
        if (this.designerContext.ActiveDocument.ProjectContext != null)
          source = Enumerable.Select<IProjectDocument, IDocumentRoot>(Enumerable.Where<IProjectDocument>((IEnumerable<IProjectDocument>) this.designerContext.ActiveDocument.ProjectContext.Documents, (Func<IProjectDocument, bool>) (document => document.DocumentType == ProjectDocumentType.ResourceDictionary)), (Func<IProjectDocument, IDocumentRoot>) (document => document.DocumentRoot));
        foreach (DocumentCompositeNode resourceNode in this.designerContext.ResourceManager.GetResourcesInRootScope(PlatformTypes.Object, ResourceResolutionFlags.IncludeApplicationResources | ResourceResolutionFlags.UniqueKeysOnly))
        {
          if (source == null || !Enumerable.Contains<IDocumentRoot>(source, resourceNode.DocumentRoot))
          {
            ResourceModel resourceModel = new ResourceModel(resourceNode);
            foreach (ResourceAssetProvider resourceAssetProvider in (IEnumerable<AssetProvider>) this.AssetProviders)
            {
              if (resourceAssetProvider.IsResourceValid(resourceModel))
              {
                ResourceAsset asset = resourceAssetProvider.CreateAsset(resourceModel);
                resourceAssetProvider.Assets.Add((Asset) asset);
                this.NeedsUpdate = true;
              }
            }
          }
        }
      }
      this.resourceChangeStamp = this.designerContext.ResourceManager.ResourceChangeStamp;
      return base.UpdateAssets();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.Document != null && args.Document.HasOpenTransaction)
        return;
      this.designerContext.ResourceManager.OnSceneUpdate(args);
      if ((int) this.resourceChangeStamp == (int) this.designerContext.ResourceManager.ResourceChangeStamp && !args.SceneSwitched)
        return;
      this.NeedsUpdate = true;
    }

    private void DocumentService_ActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
      if (e.NewDocument != null && e.NewDocument == this.currentDocument)
        return;
      EnumerableExtensions.ForEach<AssetProvider>((IEnumerable<AssetProvider>) this.AssetProviders, (Action<AssetProvider>) (provider => provider.Assets.Clear()));
      this.Assets.Clear();
      this.NeedsUpdate = true;
      this.currentDocument = (SceneDocument) null;
    }
  }
}

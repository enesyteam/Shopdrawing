// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ViewRootResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  internal class ViewRootResolver : IViewRootResolver
  {
    private Dictionary<IDocumentRoot, SceneView> hiddenDocumentsViews = new Dictionary<IDocumentRoot, SceneView>();
    private DesignerContext designerContext;
    private int isInitializingUnopenedView;

    private IViewService ViewService
    {
      get
      {
        return this.designerContext.ViewService;
      }
    }

    public bool IsInitializingUnopenedView
    {
      get
      {
        return this.isInitializingUnopenedView > 0;
      }
    }

    public ICollection<SceneView> ResolvedViews
    {
      get
      {
        List<SceneView> list = new List<SceneView>();
        foreach (IView view in (IEnumerable<IView>) this.ViewService.Views)
        {
          SceneView sceneView = view as SceneView;
          if (sceneView != null && sceneView.DocumentRoot != null)
            list.Add(sceneView);
        }
        foreach (SceneView sceneView in this.hiddenDocumentsViews.Values)
        {
          if (sceneView != null && sceneView.DocumentRoot != null)
            list.Add(sceneView);
        }
        return (ICollection<SceneView>) list;
      }
    }

    public event EventHandler<ViewEventArgs> ViewCreated;

    public ViewRootResolver(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.DocumentService.DocumentClosed += new DocumentEventHandler(this.DocumentManager_DocumentClosed);
    }

    public void Dispose()
    {
      this.designerContext.DocumentService.DocumentClosed -= new DocumentEventHandler(this.DocumentManager_DocumentClosed);
      foreach (View view in this.hiddenDocumentsViews.Values)
        view.Dispose();
      this.hiddenDocumentsViews = (Dictionary<IDocumentRoot, SceneView>) null;
    }

    public IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot)
    {
      if (documentRoot == null)
        return (IInstanceBuilderContext) null;
      foreach (IView view in (IEnumerable<IView>) this.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && !sceneView.IsClosing && sceneView.DocumentRoot == documentRoot)
          return sceneView.RootInstanceBuilderContext;
      }
      return this.GetUnopenedDocumentContext(documentRoot);
    }

    private IInstanceBuilderContext GetUnopenedDocumentContext(IDocumentRoot documentRoot)
    {
      SceneView sceneView;
      if (!this.hiddenDocumentsViews.TryGetValue(documentRoot, out sceneView))
      {
        ++this.isInitializingUnopenedView;
        try
        {
          SceneDocument sceneDocument = ViewRootResolver.GetSceneDocument(this.designerContext, documentRoot);
          if (sceneDocument != null)
          {
            sceneView = (SceneView) sceneDocument.CreateDefaultView();
            this.hiddenDocumentsViews[documentRoot] = sceneView;
            sceneView.Initialize();
          }
        }
        finally
        {
          --this.isInitializingUnopenedView;
        }
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() => this.NotifyViewCreated(sceneView)));
      }
      if (sceneView == null)
        return (IInstanceBuilderContext) null;
      return sceneView.InstanceBuilderContext;
    }

    private void NotifyViewCreated(SceneView sceneView)
    {
      if (this.ViewCreated == null)
        return;
      this.ViewCreated((object) this, new ViewEventArgs((IView) sceneView));
    }

    private void DocumentManager_DocumentClosed(object sender, DocumentEventArgs e)
    {
      SceneDocument sceneDocument = e.Document as SceneDocument;
      SceneView sceneView;
      if (sceneDocument == null || !this.hiddenDocumentsViews.TryGetValue(sceneDocument.DocumentRoot, out sceneView))
        return;
      this.hiddenDocumentsViews.Remove(sceneDocument.DocumentRoot);
      sceneView.Dispose();
    }

    public bool RecycleView(SceneView sceneView)
    {
      if (sceneView.IsDirty || sceneView.ViewModel.Document.HasOpenTransaction || (sceneView.ViewModel.DefaultView != sceneView || !sceneView.ViewUpdateManager.IsApplicationResourceContributor(sceneView.Document)))
        return false;
      this.hiddenDocumentsViews[sceneView.DocumentRoot] = sceneView;
      sceneView.Reset();
      return true;
    }

    internal bool IsViewHidden(SceneView view)
    {
      if (view == null || view.DocumentRoot == null)
        return false;
      return this.hiddenDocumentsViews.ContainsKey(view.DocumentRoot);
    }

    public SceneView GetRecycledView(SceneDocument document)
    {
      SceneView sceneView = (SceneView) null;
      if (this.hiddenDocumentsViews.TryGetValue(document.DocumentRoot, out sceneView))
        this.hiddenDocumentsViews.Remove(document.DocumentRoot);
      return sceneView;
    }

    internal static SceneDocument GetSceneDocument(DesignerContext designerContext, IDocumentRoot documentRoot)
    {
      foreach (IDocument document in (IEnumerable<IDocument>) designerContext.DocumentService.Documents)
      {
        SceneDocument sceneDocument = document as SceneDocument;
        if (sceneDocument != null && sceneDocument.DocumentRoot == documentRoot)
          return sceneDocument;
      }
      return (SceneDocument) null;
    }
  }
}

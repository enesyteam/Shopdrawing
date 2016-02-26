// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ViewUpdateManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  public class ViewUpdateManager
  {
    private Dictionary<SceneView, ViewUpdateManager.RelatedDocumentTable> relatedDocumentsForView = new Dictionary<SceneView, ViewUpdateManager.RelatedDocumentTable>();
    private Dictionary<SceneDocument, List<SceneView>> primaryViewsForDocument = new Dictionary<SceneDocument, List<SceneView>>();
    private Dictionary<SceneDocument, List<SceneView>> relatedViewsForDocument = new Dictionary<SceneDocument, List<SceneView>>();
    private List<SceneDocument> watchedDocuments = new List<SceneDocument>();
    private HashSet<IProjectContext> watchedProjectContexts = new HashSet<IProjectContext>();
    private Dictionary<IProjectContext, List<SceneDocument>> watchedDocumentsForProjectContext = new Dictionary<IProjectContext, List<SceneDocument>>();
    private Dictionary<IProjectDocument, List<SceneView>> pendingViewsForClosedDocuments = new Dictionary<IProjectDocument, List<SceneView>>();
    private List<SceneView> postponedInstanceUpdates = new List<SceneView>();
    private List<SceneView> postponedReferenceUpdates = new List<SceneView>();
    private DesignerContext designerContext;
    private bool postponeUpdates;
    private int postponeUpdateCount;
    private Action pendingUpdateCallback;

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    internal bool UpdatesPostponed
    {
      get
      {
        if (!this.postponeUpdates)
          return this.postponeUpdateCount > 0;
        return true;
      }
    }

    internal ViewUpdateManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ProjectManager.ProjectOpened += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
      this.designerContext.ProjectManager.ProjectClosing += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
    }

    public IDisposable PostponeUpdates()
    {
      return (IDisposable) new ViewUpdateManager.PostponeUpdateToken(this);
    }

    public void EnsureViewUpdatesForRelatedDocument(SceneView sceneView, SceneDocument sceneDocument)
    {
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable;
      if (!this.relatedDocumentsForView.TryGetValue(sceneView, out relatedDocumentTable))
      {
        relatedDocumentTable = new ViewUpdateManager.RelatedDocumentTable();
        this.relatedDocumentsForView[sceneView] = relatedDocumentTable;
      }
      if (sceneDocument == sceneView.Document || relatedDocumentTable.Contains(sceneDocument))
        return;
      IProjectDocument document = sceneDocument.ProjectContext.GetDocument(sceneDocument.DocumentRoot);
      if (ViewUpdateManager.GetDocumentRelation(sceneView, document) != ViewUpdateManager.DocumentRelation.Related || ViewUpdateManager.IsDisposingOrDisposed(sceneDocument.ProjectContext))
        return;
      relatedDocumentTable.Add(sceneDocument);
      this.RegisterRelatedDocumentInternal(sceneView, sceneDocument);
    }

    public void Register(SceneView sceneView)
    {
      this.AddHandlers(sceneView.Document);
      this.EnsureViewUpdatesForDesignTimeResources(sceneView);
      SceneDocument document = sceneView.Document;
      List<SceneView> list = (List<SceneView>) null;
      if (!this.primaryViewsForDocument.TryGetValue(document, out list))
      {
        list = new List<SceneView>();
        this.primaryViewsForDocument[document] = list;
      }
      list.Add(sceneView);
    }

    public void Unregister(SceneView sceneView)
    {
      this.UnregisterRelatedDocuments(sceneView);
      SceneDocument document = sceneView.Document;
      List<SceneView> list = (List<SceneView>) null;
      if (this.primaryViewsForDocument.TryGetValue(document, out list))
      {
        list.Remove(sceneView);
        if (list.Count == 0)
        {
          this.primaryViewsForDocument.Remove(document);
          if (!this.relatedViewsForDocument.ContainsKey(document))
            this.RemoveHandlers(document);
        }
      }
      this.postponedInstanceUpdates.Remove(sceneView);
      this.postponedReferenceUpdates.Remove(sceneView);
    }

    public ICollection<SceneDocument> GetRelatedDocuments(SceneView sceneView)
    {
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable;
      if (this.relatedDocumentsForView.TryGetValue(sceneView, out relatedDocumentTable))
        return relatedDocumentTable.Documents;
      return (ICollection<SceneDocument>) new List<SceneDocument>();
    }

    public void InvalidateViews(ICollection<SceneView> views, bool updateActiveView)
    {
      List<SceneView> list = new List<SceneView>((IEnumerable<SceneView>) views);
      foreach (SceneView sceneView in list)
      {
        if (!sceneView.IsClosing)
          sceneView.ShutdownVisualTree();
      }
      if (!updateActiveView)
        return;
      SceneView sceneView1 = this.designerContext.ViewService.ActiveView as SceneView;
      if (sceneView1 == null || sceneView1.IsClosing)
        return;
      if (list.Contains(sceneView1))
        sceneView1.EnsureActiveViewUpdated();
      else
        sceneView1.RefreshApplicationResourceDictionary();
    }

    public void UpdateInvalidRelatedDocuments(SceneView view)
    {
      if (view.IsClosing)
        return;
      SceneDocument applicationSceneDocument = view.Document.ApplicationSceneDocument;
      if (applicationSceneDocument == view.Document && !view.IsEditingOutOfPlace)
        return;
      List<SceneView> orderedRelatedViews = this.GetOrderedRelatedViews(view);
      orderedRelatedViews.RemoveAll((Predicate<SceneView>) (v =>
      {
        if (v != view && !v.IsEditingOutOfPlace && !v.IsClosing)
          return !v.ViewNodeManager.IsRootInvalid;
        return true;
      }));
      if (applicationSceneDocument != null)
      {
        if (!this.primaryViewsForDocument.ContainsKey(applicationSceneDocument))
          this.designerContext.ViewRootResolver.GetViewContext((IDocumentRoot) applicationSceneDocument.XamlDocument);
        List<SceneView> list;
        if (this.primaryViewsForDocument.TryGetValue(applicationSceneDocument, out list))
        {
          foreach (SceneView sceneView in list)
          {
            if (!sceneView.IsClosing && !sceneView.IsEditingOutOfPlace && sceneView.ViewNodeManager.IsRootInvalid)
              sceneView.Update(true);
          }
        }
      }
      for (int index = 0; index < orderedRelatedViews.Count; ++index)
      {
        SceneView sceneView = orderedRelatedViews[index];
        if (!sceneView.IsClosing && sceneView.ViewNodeManager.IsRootInvalid)
          sceneView.Update(false);
      }
      for (int index = 0; index < orderedRelatedViews.Count; ++index)
      {
        SceneView sceneView = orderedRelatedViews[index];
        if (!sceneView.IsClosing)
          sceneView.Update(true);
      }
    }

    public bool ResolveRelatedMissingResources(SceneView view, DesignTimeResourceResolverContext resourceResolverContext)
    {
      if (view.IsClosing)
        return false;
      List<SceneView> orderedRelatedViews = this.GetOrderedRelatedViews(view);
      foreach (SceneDocument key in new List<SceneDocument>(view.Document.DesignTimeResourceDocuments))
      {
        List<SceneView> list;
        if (this.primaryViewsForDocument.TryGetValue(key, out list))
        {
          foreach (SceneView view1 in list)
          {
            if (!view1.IsClosing && !view1.IsEditingOutOfPlace)
              ViewUpdateManager.AddViewInOrder(view1, (IList<SceneView>) orderedRelatedViews);
          }
        }
      }
      bool flag = false;
      foreach (SceneView sceneView in orderedRelatedViews)
      {
        if (sceneView.ResolveMissingResourcesOnce(resourceResolverContext))
          flag = true;
        if (resourceResolverContext.IsCanceled)
          break;
      }
      return flag;
    }

    public void ExchangeRelatedDocumentHandlers(SceneView firstView, SceneView secondView)
    {
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable1 = (ViewUpdateManager.RelatedDocumentTable) null;
      this.relatedDocumentsForView.TryGetValue(firstView, out relatedDocumentTable1);
      this.relatedDocumentsForView.Remove(firstView);
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable2 = (ViewUpdateManager.RelatedDocumentTable) null;
      this.relatedDocumentsForView.TryGetValue(secondView, out relatedDocumentTable2);
      this.relatedDocumentsForView.Remove(secondView);
      if (relatedDocumentTable1 != null)
      {
        foreach (SceneDocument relatedDocument in (IEnumerable<SceneDocument>) relatedDocumentTable1.Documents)
          this.UnregisterRelatedDocumentInternal(firstView, relatedDocument);
        this.relatedDocumentsForView[secondView] = relatedDocumentTable1;
      }
      if (relatedDocumentTable2 != null)
      {
        foreach (SceneDocument relatedDocument in (IEnumerable<SceneDocument>) relatedDocumentTable2.Documents)
          this.UnregisterRelatedDocumentInternal(secondView, relatedDocument);
        this.relatedDocumentsForView[firstView] = relatedDocumentTable2;
        foreach (SceneDocument sceneDocument in (IEnumerable<SceneDocument>) relatedDocumentTable2.Documents)
          this.RegisterRelatedDocumentInternal(firstView, sceneDocument);
      }
      if (relatedDocumentTable1 == null)
        return;
      foreach (SceneDocument sceneDocument in (IEnumerable<SceneDocument>) relatedDocumentTable1.Documents)
        this.RegisterRelatedDocumentInternal(secondView, sceneDocument);
    }

    private List<SceneView> GetOrderedRelatedViews(SceneView view)
    {
      List<SceneView> list1 = new List<SceneView>();
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable;
      if (this.relatedDocumentsForView.TryGetValue(view, out relatedDocumentTable))
      {
        foreach (SceneDocument key in Enumerable.ToArray<SceneDocument>((IEnumerable<SceneDocument>) relatedDocumentTable.Documents))
        {
          List<SceneView> list2;
          if (this.primaryViewsForDocument.TryGetValue(key, out list2))
          {
            foreach (SceneView view1 in list2.ToArray())
              ViewUpdateManager.AddViewInOrder(view1, (IList<SceneView>) list1);
          }
        }
      }
      return list1;
    }

    private void UnregisterRelatedDocuments(SceneView sceneView)
    {
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable = (ViewUpdateManager.RelatedDocumentTable) null;
      if (!this.relatedDocumentsForView.TryGetValue(sceneView, out relatedDocumentTable))
        return;
      foreach (SceneDocument sceneDocument in new List<SceneDocument>((IEnumerable<SceneDocument>) relatedDocumentTable.Documents))
      {
        this.UnregisterRelatedDocumentInternal(sceneView, sceneDocument);
        relatedDocumentTable.Remove(sceneDocument);
      }
      this.relatedDocumentsForView.Remove(sceneView);
    }

    private void UnregisterRelatedDocumentInternal(SceneView sceneView, SceneDocument relatedDocument)
    {
      List<SceneView> list = (List<SceneView>) null;
      if (!this.relatedViewsForDocument.TryGetValue(relatedDocument, out list))
        return;
      list.Remove(sceneView);
      if (list.Count != 0)
        return;
      this.relatedViewsForDocument.Remove(relatedDocument);
      if (this.primaryViewsForDocument.ContainsKey(relatedDocument))
        return;
      this.RemoveHandlers(relatedDocument);
    }

    private void RegisterRelatedDocumentInternal(SceneView sceneView, SceneDocument sceneDocument)
    {
      List<SceneView> list = (List<SceneView>) null;
      if (!this.relatedViewsForDocument.TryGetValue(sceneDocument, out list))
      {
        list = new List<SceneView>();
        this.relatedViewsForDocument[sceneDocument] = list;
      }
      list.Add(sceneView);
      this.AddHandlers(sceneDocument);
    }

    private void InvalidateViewsInternal(ICollection<SceneView> affectedViews)
    {
      if (affectedViews == null || affectedViews.Count <= 0)
        return;
      if (this.UpdatesPostponed)
      {
        foreach (SceneView view in (IEnumerable<SceneView>) affectedViews)
          ViewUpdateManager.AddViewInOrder(view, (IList<SceneView>) this.postponedInstanceUpdates);
        this.RebuildPostponedViewsAsync();
      }
      else
        this.InvalidateViews(affectedViews, true);
    }

    private void UpdateReferences(ICollection<SceneView> affectedViews)
    {
      if (affectedViews == null || affectedViews.Count <= 0)
        return;
      foreach (SceneView view in (IEnumerable<SceneView>) affectedViews)
      {
        if (!this.postponedInstanceUpdates.Contains(view))
        {
          if (this.UpdatesPostponed)
          {
            ViewUpdateManager.AddViewInOrder(view, (IList<SceneView>) this.postponedReferenceUpdates);
            this.RebuildPostponedViewsAsync();
          }
          else if (!view.IsClosing)
            view.UpdateReferences();
        }
      }
    }

    private void RebuildPostponedViewsAsync()
    {
      if (this.pendingUpdateCallback != null)
        return;
      this.pendingUpdateCallback = (Action) (() => this.RebuildPostponedViews());
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, this.pendingUpdateCallback);
    }

    internal void RefreshActiveViewApplicationResources()
    {
      SceneView activeView = this.designerContext.ActiveView;
      if (activeView == null || activeView.IsClosing)
        return;
      activeView.RefreshApplicationResourceDictionary();
      activeView.UpdateLayout();
    }

    internal void RebuildPostponedViews()
    {
      this.postponeUpdates = false;
      this.pendingUpdateCallback = (Action) null;
      this.InvalidateViews((ICollection<SceneView>) this.postponedInstanceUpdates, true);
      this.UpdateReferences((ICollection<SceneView>) this.postponedReferenceUpdates);
      this.postponedInstanceUpdates.Clear();
      this.postponedReferenceUpdates.Clear();
    }

    public bool IsApplicationResourceContributor(SceneDocument sceneDocument)
    {
      if (sceneDocument == null || sceneDocument.ProjectDocumentType != ProjectDocumentType.Application && sceneDocument.ProjectDocumentType != ProjectDocumentType.ResourceDictionary)
        return false;
      List<SceneDocument> designTimeResourceDocuments = new List<SceneDocument>(sceneDocument.DesignTimeResourceDocuments);
      if (designTimeResourceDocuments.Count == 0)
        return false;
      HashSet<SceneDocument> visitedDocuments = new HashSet<SceneDocument>();
      return this.IsApplicationResourceContributor(sceneDocument, visitedDocuments, designTimeResourceDocuments);
    }

    private bool IsApplicationResourceContributor(SceneDocument sceneDocument, HashSet<SceneDocument> visitedDocuments, List<SceneDocument> designTimeResourceDocuments)
    {
      if (sceneDocument.ProjectDocumentType != ProjectDocumentType.Application && sceneDocument.ProjectDocumentType != ProjectDocumentType.ResourceDictionary)
        return false;
      if (designTimeResourceDocuments.Contains(sceneDocument))
        return true;
      if (visitedDocuments.Contains(sceneDocument))
        return false;
      visitedDocuments.Add(sceneDocument);
      List<SceneView> list = (List<SceneView>) null;
      if (this.relatedViewsForDocument.TryGetValue(sceneDocument, out list))
      {
        foreach (SceneView sceneView in list)
        {
          if (!sceneView.IsClosing && !sceneView.IsEditingOutOfPlace && this.IsApplicationResourceContributor(sceneView.Document, visitedDocuments, designTimeResourceDocuments))
            return true;
        }
      }
      return false;
    }

    public static void AddViewInOrder(SceneView view, IList<SceneView> target)
    {
      if (view.IsClosing)
        return;
      int index1 = -1;
      for (int index2 = 0; index2 < target.Count; ++index2)
      {
        SceneView sceneView = target[index2];
        if (view == sceneView)
          return;
        if (index1 == -1 && !sceneView.IsClosing && sceneView.RelatedDocuments.Contains(view.Document))
          index1 = index2;
      }
      if (index1 == -1)
        target.Add(view);
      else
        target.Insert(index1, view);
    }

    private void AddHandlers(SceneDocument document)
    {
      if (this.watchedDocuments.Contains(document))
        return;
      document.PostEditTransactionCompleted += new EventHandler(this.OnEditTransactionCompleted);
      document.PostEditTransactionUpdated += new EventHandler(this.OnEditTransactionUpdated);
      document.PostEditTransactionUndoRedo += new EventHandler(this.OnEditTransactionCompleted);
      document.TypesChanged += new EventHandler(this.OnDocumentModelChanged);
      document.RootNodeChanged += new EventHandler(this.OnDocumentModelChanged);
      IProjectContext projectContext = document.ProjectContext;
      List<SceneDocument> list = (List<SceneDocument>) null;
      if (!this.watchedDocumentsForProjectContext.TryGetValue(projectContext, out list))
      {
        list = new List<SceneDocument>();
        this.watchedDocumentsForProjectContext[projectContext] = list;
      }
      if (!list.Contains(document))
        list.Add(document);
      this.watchedDocuments.Add(document);
    }

    private void RemoveHandlers(SceneDocument document)
    {
      if (!this.watchedDocuments.Contains(document))
        return;
      document.PostEditTransactionCompleted -= new EventHandler(this.OnEditTransactionCompleted);
      document.PostEditTransactionUpdated -= new EventHandler(this.OnEditTransactionUpdated);
      document.PostEditTransactionUndoRedo -= new EventHandler(this.OnEditTransactionCompleted);
      document.TypesChanged -= new EventHandler(this.OnDocumentModelChanged);
      document.RootNodeChanged -= new EventHandler(this.OnDocumentModelChanged);
      IProjectContext projectContext = document.ProjectContext;
      List<SceneDocument> list = (List<SceneDocument>) null;
      if (this.watchedDocumentsForProjectContext.TryGetValue(projectContext, out list))
      {
        if (list.Contains(document))
          list.Remove(document);
        if (list.Count == 0)
          this.watchedDocumentsForProjectContext.Remove(projectContext);
      }
      this.watchedDocuments.Remove(document);
    }

    private void OnProjectContextDocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Remove:
        case NotifyCollectionChangedAction.Reset:
          List<SceneView> list1 = new List<SceneView>();
          IProjectContext index = (IProjectContext) null;
          foreach (IProjectContext projectContext in this.watchedDocumentsForProjectContext.Keys)
          {
            if (projectContext.Documents == sender)
            {
              index = projectContext;
              break;
            }
          }
          if (index == null)
            break;
          foreach (SceneDocument key in this.watchedDocumentsForProjectContext[index])
          {
            List<SceneView> list2 = (List<SceneView>) null;
            if (this.primaryViewsForDocument.TryGetValue(key, out list2))
              list1.AddRange((IEnumerable<SceneView>) list2);
            List<SceneView> list3 = (List<SceneView>) null;
            if (this.relatedViewsForDocument.TryGetValue(key, out list3))
              list1.AddRange((IEnumerable<SceneView>) list3);
          }
          this.InvalidateViewsInternal((ICollection<SceneView>) list1);
          break;
      }
    }

    private void OnProjectContextDocumentClosedEarly(object sender, ProjectDocumentEventArgs e)
    {
      IProjectDocument document = e.Document;
      SceneDocument sceneDocument = e.Document.Document as SceneDocument;
      if (sceneDocument == null)
        return;
      List<SceneView> list1 = (List<SceneView>) null;
      if (this.primaryViewsForDocument.TryGetValue(sceneDocument, out list1))
      {
        foreach (SceneView sceneView in new List<SceneView>((IEnumerable<SceneView>) list1))
        {
          sceneView.SuspendUpdatesForViewShutdown();
          this.Unregister(sceneView);
        }
      }
      List<SceneView> list2 = (List<SceneView>) null;
      if (!this.relatedViewsForDocument.TryGetValue(sceneDocument, out list2))
        return;
      List<SceneView> list3 = new List<SceneView>((IEnumerable<SceneView>) list2);
      foreach (SceneView sceneView in list3)
      {
        ViewUpdateManager.RelatedDocumentTable relatedDocumentTable = this.relatedDocumentsForView[sceneView];
        this.UnregisterRelatedDocumentInternal(sceneView, sceneDocument);
        relatedDocumentTable.Remove(sceneDocument);
      }
      List<SceneView> list4;
      if (!this.pendingViewsForClosedDocuments.TryGetValue(e.Document, out list4))
        this.pendingViewsForClosedDocuments.Add(e.Document, list3);
      else
        list4.AddRange(Enumerable.Except<SceneView>((IEnumerable<SceneView>) list3, (IEnumerable<SceneView>) list4));
    }

    private void OnProjectContextDocumentClosedLate(object sender, ProjectDocumentEventArgs e)
    {
      List<SceneView> list1 = (List<SceneView>) null;
      if (this.pendingViewsForClosedDocuments.TryGetValue(e.Document, out list1))
      {
        this.pendingViewsForClosedDocuments.Remove(e.Document);
        List<SceneView> list2 = (List<SceneView>) null;
        foreach (SceneView sceneView in list1)
        {
          if (ViewUpdateManager.GetDocumentRelation(sceneView, e.Document) == ViewUpdateManager.DocumentRelation.Related)
          {
            if (list2 == null)
              list2 = new List<SceneView>();
            list2.Add(sceneView);
          }
        }
        if (list2 != null)
          this.InvalidateViewsInternal((ICollection<SceneView>) list2);
      }
      SceneView activeView = this.DesignerContext.ActiveView;
      if (activeView == null)
        return;
      activeView.RebuildUserControls(e.Document);
    }

    private void OnProjectContextDocumentOpened(object sender, ProjectDocumentEventArgs e)
    {
      if (e.Document == null || e.Document.DocumentType != ProjectDocumentType.Application && e.Document.DocumentType != ProjectDocumentType.ResourceDictionary)
        return;
      SceneDocument sceneDocument = e.Document.Document as SceneDocument;
      if (!Enumerable.Contains<SceneDocument>(sceneDocument.DesignTimeResourceDocuments, sceneDocument))
        return;
      this.RefreshViewUpdatesForDesignTimeResources(false);
    }

    private void OnEditTransactionCompleted(object sender, EventArgs e)
    {
      this.UpdateRelatedViews(sender as SceneDocument, false);
    }

    internal void UpdateRelatedViews(SceneDocument sceneDocument, bool majorChange)
    {
      List<SceneView> list;
      if (sceneDocument == null || !this.relatedViewsForDocument.TryGetValue(sceneDocument, out list))
        return;
      HashSet<SceneView> views = new HashSet<SceneView>();
      foreach (SceneView view in list)
      {
        if (!view.IsClosing)
        {
          this.CollectRelatedViewsRecursively(view, views);
          RelatedDocumentInfo documentInfo = this.relatedDocumentsForView[view][sceneDocument];
          view.UpdateFromRelatedDocument(sceneDocument, documentInfo, true, majorChange);
        }
      }
      this.UpdateReferences((ICollection<SceneView>) views);
    }

    internal void InvalidateRelatedDocumentTemplates(SceneView view)
    {
      HashSet<SceneView> views = new HashSet<SceneView>();
      List<SceneView> list;
      if (this.relatedViewsForDocument.TryGetValue(view.Document, out list))
      {
        foreach (SceneView view1 in list)
          this.CollectRelatedViewsRecursively(view1, views);
      }
      foreach (SceneView sceneView in views)
        sceneView.InvalidateTemplates();
    }

    private void CollectRelatedViewsRecursively(SceneView view, HashSet<SceneView> views)
    {
      if (view.IsClosing || views.Contains(view))
        return;
      views.Add(view);
      List<SceneView> list;
      if (!this.relatedViewsForDocument.TryGetValue(view.Document, out list))
        return;
      foreach (SceneView view1 in list)
        this.CollectRelatedViewsRecursively(view1, views);
    }

    private void OnEditTransactionUpdated(object sender, EventArgs e)
    {
      SceneDocument document = sender as SceneDocument;
      SceneView activeView = this.DesignerContext.ActiveView;
      ViewUpdateManager.RelatedDocumentTable relatedDocumentTable = (ViewUpdateManager.RelatedDocumentTable) null;
      if (activeView == null || activeView.IsClosing || (!this.relatedDocumentsForView.TryGetValue(activeView, out relatedDocumentTable) || !relatedDocumentTable.Contains(document)))
        return;
      RelatedDocumentInfo documentInfo = relatedDocumentTable[document];
      activeView.UpdateFromRelatedDocument(document, documentInfo, true, false);
      activeView.UpdateReferences();
    }

    private void OnDocumentModelChanged(object sender, EventArgs e)
    {
      SceneDocument sceneDocument = sender as SceneDocument;
      if (sceneDocument == null)
        return;
      List<SceneView> list = (List<SceneView>) null;
      if (this.primaryViewsForDocument.TryGetValue(sceneDocument, out list))
        this.InvalidateViewsInternal((ICollection<SceneView>) list);
      this.UpdateRelatedViews(sceneDocument, true);
      this.UpdateReferences((ICollection<SceneView>) list);
    }

    private static ViewUpdateManager.DocumentRelation GetDocumentRelation(SceneView sceneView, IProjectDocument projectDocument)
    {
      if (projectDocument == null)
        return ViewUpdateManager.DocumentRelation.Unrelated;
      if (projectDocument.DocumentType == ProjectDocumentType.Application || projectDocument.DocumentType == ProjectDocumentType.ResourceDictionary || projectDocument.DocumentRoot != null && projectDocument.DocumentRoot.RootNode != null && PlatformTypes.UserControl.IsAssignableFrom((ITypeId) projectDocument.DocumentRoot.RootNode.Type))
        return ViewUpdateManager.DocumentRelation.Related;
      string withoutExtension = Path.GetFileNameWithoutExtension(projectDocument.Path);
      SampleDataSet sampleDataSet = sceneView.SampleData.GetSampleDataSet(withoutExtension, true);
      if (sampleDataSet == null)
        return DocumentContextHelper.GetDesignDataMode(ProjectHelper.GetProject(sceneView.DesignerContext.ProjectManager, sceneView.Document.DocumentContext), projectDocument.Path) != DesignDataMode.None ? ViewUpdateManager.DocumentRelation.Related : ViewUpdateManager.DocumentRelation.Unrelated;
      if (!sampleDataSet.XamlFilePath.Equals(projectDocument.Path, StringComparison.OrdinalIgnoreCase))
        return ViewUpdateManager.DocumentRelation.Unrelated;
      return !sampleDataSet.IsOnline ? ViewUpdateManager.DocumentRelation.OfflineSampleData : ViewUpdateManager.DocumentRelation.Related;
    }

    private static bool IsDisposingOrDisposed(IProjectContext projectContext)
    {
      if (projectContext != null)
        return ProjectBase.IsDisposingOrDisposed((IProject) projectContext.GetService(typeof (IProject)));
      return false;
    }

    private void AssemblyCollection_CollectionChanging(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (this.primaryViewsForDocument.Count <= 0)
        return;
      this.postponeUpdates = true;
    }

    private void ProjectManager_ProjectOpened(object sender, ProjectEventArgs e)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(e.Project);
      if (projectContext == null)
        return;
      this.AddProjectContextHandlers(projectContext);
    }

    private void ProjectManager_ProjectClosing(object sender, ProjectEventArgs e)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(e.Project);
      List<SceneDocument> list1 = (List<SceneDocument>) null;
      if (projectContext == null)
        return;
      this.MarkViewsForClosing(projectContext);
      this.RemoveProjectContextHandlers(projectContext);
      if (!this.watchedDocumentsForProjectContext.TryGetValue(projectContext, out list1))
        return;
      foreach (SceneDocument sceneDocument in new List<SceneDocument>((IEnumerable<SceneDocument>) list1))
      {
        List<SceneView> list2;
        if (this.primaryViewsForDocument.TryGetValue(sceneDocument, out list2))
        {
          foreach (SceneView sceneView in list2)
            this.UnregisterRelatedDocuments(sceneView);
          this.primaryViewsForDocument.Remove(sceneDocument);
        }
        List<SceneView> list3;
        if (this.relatedViewsForDocument.TryGetValue(sceneDocument, out list3))
        {
          foreach (SceneView sceneView in new List<SceneView>((IEnumerable<SceneView>) list3))
          {
            if (sceneView.ProjectContext == projectContext)
              sceneView.SuspendUpdatesForViewShutdown();
            this.UnregisterRelatedDocumentInternal(sceneView, sceneDocument);
            this.relatedDocumentsForView[sceneView].Remove(sceneDocument);
          }
        }
        this.RemoveHandlers(sceneDocument);
      }
    }

    private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
    {
      this.CheckForProjectReferenceChange(e.ProjectItem);
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      this.CheckForProjectReferenceChange(e.ProjectItem);
    }

    private void CheckForProjectReferenceChange(IProjectItem projectItem)
    {
      if (!(projectItem.DocumentType is ProjectReferenceDocumentType))
        return;
      this.RefreshViewUpdatesForDesignTimeResources(true);
    }

    internal void RefreshViewUpdatesForDesignTimeResources(bool updateViews)
    {
      List<SceneView> list1 = new List<SceneView>();
      foreach (List<SceneView> list2 in this.primaryViewsForDocument.Values)
      {
        foreach (SceneView view in list2)
          ViewUpdateManager.AddViewInOrder(view, (IList<SceneView>) list1);
      }
      foreach (SceneView sceneView in list1)
        this.EnsureViewUpdatesForDesignTimeResources(sceneView);
      if (!updateViews)
        return;
      foreach (SceneView view in list1)
      {
        if (!view.IsClosing && view.InstanceBuilderContext.WarningDictionary.Count > 0)
        {
          List<DocumentNode> list2 = new List<DocumentNode>(view.InstanceBuilderContext.WarningDictionary.Keys);
          bool flag = false;
          foreach (DocumentNode documentNode in list2)
          {
            if (documentNode.Type.IsResource)
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            if (this.UpdatesPostponed)
              ViewUpdateManager.AddViewInOrder(view, (IList<SceneView>) this.postponedInstanceUpdates);
            else
              view.InvalidateAndUpdate();
          }
        }
      }
    }

    private void EnsureViewUpdatesForDesignTimeResources(SceneView sceneView)
    {
      if (sceneView.IsClosing)
        return;
      foreach (SceneDocument sceneDocument in sceneView.Document.DesignTimeResourceDocuments)
        this.EnsureViewUpdatesForRelatedDocument(sceneView, sceneDocument);
    }

    private void MarkViewsForClosing(IProjectContext projectContext)
    {
      foreach (SceneView sceneView in (IEnumerable<SceneView>) this.designerContext.ViewRootResolver.ResolvedViews)
      {
        if (sceneView.ProjectContext == projectContext)
          sceneView.SuspendUpdatesForViewShutdown();
      }
    }

    private void AddProjectContextHandlers(IProjectContext projectContext)
    {
      if (this.watchedProjectContexts.Contains(projectContext))
        return;
      this.watchedProjectContexts.Add(projectContext);
      projectContext.DocumentClosing += new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentClosedEarly);
      projectContext.DocumentClosed += new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentClosedLate);
      projectContext.DocumentOpened += new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentOpened);
      INotifyCollectionChanged collectionChanged = projectContext.Documents as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnProjectContextDocumentsCollectionChanged);
      INotifyCollectionChanges collectionChanges = projectContext.AssemblyReferences as INotifyCollectionChanges;
      if (collectionChanges != null)
        collectionChanges.CollectionChanging += new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanging);
      IProject project = (IProject) projectContext.GetService(typeof (IProject));
      if (project == null)
        return;
      project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
      project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
    }

    private void RemoveProjectContextHandlers(IProjectContext projectContext)
    {
      if (!this.watchedProjectContexts.Contains(projectContext))
        return;
      this.watchedProjectContexts.Remove(projectContext);
      projectContext.DocumentClosing -= new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentClosedEarly);
      projectContext.DocumentClosed -= new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentClosedLate);
      projectContext.DocumentOpened -= new EventHandler<ProjectDocumentEventArgs>(this.OnProjectContextDocumentOpened);
      INotifyCollectionChanged collectionChanged = projectContext.Documents as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnProjectContextDocumentsCollectionChanged);
      INotifyCollectionChanges collectionChanges = projectContext.AssemblyReferences as INotifyCollectionChanges;
      if (collectionChanges != null)
        collectionChanges.CollectionChanging -= new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanging);
      IProject project = (IProject) projectContext.GetService(typeof (IProject));
      if (project == null)
        return;
      project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
      project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
    }

    private class PostponeUpdateToken : IDisposable
    {
      private ViewUpdateManager manager;

      public PostponeUpdateToken(ViewUpdateManager manager)
      {
        this.manager = manager;
        ++this.manager.postponeUpdateCount;
      }

      public void Dispose()
      {
        --this.manager.postponeUpdateCount;
        GC.SuppressFinalize((object) this);
      }
    }

    private enum DocumentRelation
    {
      Unrelated,
      Related,
      OfflineSampleData,
    }

    private class RelatedDocumentTable
    {
      private Dictionary<SceneDocument, RelatedDocumentInfo> documents = new Dictionary<SceneDocument, RelatedDocumentInfo>();

      public ICollection<SceneDocument> Documents
      {
        get
        {
          return (ICollection<SceneDocument>) this.documents.Keys;
        }
      }

      public RelatedDocumentInfo this[SceneDocument index]
      {
        get
        {
          return this.documents[index];
        }
      }

      public bool Contains(SceneDocument document)
      {
        return this.documents.ContainsKey(document);
      }

      public void Add(SceneDocument document)
      {
        RelatedDocumentInfo relatedDocumentInfo = new RelatedDocumentInfo(document);
        this.documents.Add(document, relatedDocumentInfo);
      }

      public void Remove(SceneDocument document)
      {
        RelatedDocumentInfo relatedDocumentInfo = this[document];
        this.documents.Remove(document);
        relatedDocumentInfo.Unregister();
      }
    }
  }
}

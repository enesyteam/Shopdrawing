// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Subscription.XamlProjectSubscription
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Subscription
{
  internal class XamlProjectSubscription
  {
    private IProjectContext projectContext;
    private DesignerContext designerContext;
    private SearchPath sceneSearch;
    private XamlProjectSubscription.DocumentNodeFilter documentSearch;
    private Dictionary<XamlDocument, IXamlSubscription> watchers;
    private Queue<XamlDocument> chainUpdate;
    private DocumentNodeMarkerSortedListOf<DocumentNodePath> basisNodes;
    private DocumentNodeMarkerSortedListOf<DocumentNodePath> newBasisNodes;

    public bool IsEmpty
    {
      get
      {
        return this.ImmediateBasisNodes.Count == 0;
      }
    }

    private DocumentNodeMarkerSortedListOf<DocumentNodePath> ImmediateBasisNodes
    {
      get
      {
        if (this.newBasisNodes == null)
          return this.basisNodes;
        return this.newBasisNodes;
      }
    }

    public IProjectContext ProjectContext
    {
      get
      {
        return this.projectContext;
      }
    }

    public event XamlProjectSubscription.DocumentNodeInsertedHandler PathNodeInserted;

    public event XamlProjectSubscription.DocumentNodeRemovedHandler PathNodeRemoved;

    public event EventHandler CatastrophicUpdate;

    public XamlProjectSubscription(DesignerContext designerContext, IProjectContext projectContext, SearchPath sceneSearch, XamlProjectSubscription.DocumentNodeFilter documentSearch)
    {
      this.designerContext = designerContext;
      this.projectContext = projectContext;
      this.sceneSearch = sceneSearch;
      this.documentSearch = documentSearch;
      this.watchers = new Dictionary<XamlDocument, IXamlSubscription>();
      this.chainUpdate = new Queue<XamlDocument>();
      this.basisNodes = new DocumentNodeMarkerSortedListOf<DocumentNodePath>();
      this.designerContext.ViewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewOpenedOrClosed);
      this.designerContext.ViewService.ViewOpened += new ViewEventHandler(this.ViewService_ViewOpenedOrClosed);
    }

    public void Update()
    {
      if (this.newBasisNodes != null)
      {
        foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.basisNodes.UnionIdentity((DocumentNodeMarkerSortedListBase) this.newBasisNodes))
        {
          if (intersectionResult.LeftHandSideIndex == -1)
          {
            DocumentNodePath nodePath = this.newBasisNodes.ValueAt(intersectionResult.RightHandSideIndex);
            this.Register(nodePath.RootNode.DocumentRoot as XamlDocument, nodePath);
          }
          if (intersectionResult.RightHandSideIndex == -1)
          {
            DocumentNodePath documentNodePath = this.basisNodes.ValueAt(intersectionResult.LeftHandSideIndex);
            this.Unregister(documentNodePath.RootNode.DocumentRoot as XamlDocument, documentNodePath.Node);
          }
        }
      }
      this.basisNodes = this.newBasisNodes;
      this.newBasisNodes = (DocumentNodeMarkerSortedListOf<DocumentNodePath>) null;
      this.UpdateInternal();
    }

    public void Dispose()
    {
      this.Clear();
    }

    public void SetBasisNodes(DocumentNodeMarkerSortedListOf<DocumentNodePath> newBasisNodes)
    {
      this.newBasisNodes = newBasisNodes;
    }

    public void Clear()
    {
      foreach (IDisposable disposable in this.watchers.Values)
        disposable.Dispose();
      this.watchers.Clear();
      this.chainUpdate.Clear();
      this.basisNodes.Clear();
      this.newBasisNodes = (DocumentNodeMarkerSortedListOf<DocumentNodePath>) null;
    }

    private void UpdateInternal()
    {
      while (this.chainUpdate.Count > 0)
      {
        IXamlSubscription xamlSubscription;
        if (this.watchers.TryGetValue(this.chainUpdate.Dequeue(), out xamlSubscription))
          xamlSubscription.Update();
      }
    }

    private void OnCatastrophicUpdate()
    {
      DocumentNodeMarkerSortedListOf<DocumentNodePath> immediateBasisNodes = this.ImmediateBasisNodes;
      if (this.CatastrophicUpdate != null)
        this.CatastrophicUpdate((object) this, EventArgs.Empty);
      this.Clear();
      this.newBasisNodes = immediateBasisNodes;
    }

    private bool Register(XamlDocument document, DocumentNodePath nodePath)
    {
      if (document != null && this.GetWatcher(document).Register(nodePath) && !this.chainUpdate.Contains(document))
        this.chainUpdate.Enqueue(document);
      return true;
    }

    private bool Unregister(XamlDocument document, DocumentNode node)
    {
      if (document != null)
      {
        IXamlSubscription watcher = this.GetWatcher(document);
        bool flag = watcher.Unregister(node);
        if (watcher.IsEmpty)
        {
          watcher.Update();
          watcher.Dispose();
          this.watchers.Remove(document);
        }
        else if (flag && !this.chainUpdate.Contains(document))
          this.chainUpdate.Enqueue(document);
      }
      return true;
    }

    private IXamlSubscription GetWatcher(XamlDocument document)
    {
      IXamlSubscription xamlSubscription;
      if (!this.watchers.TryGetValue(document, out xamlSubscription))
      {
        SceneView view = this.FindView(document.DocumentContext);
        if (view != null)
        {
          xamlSubscription = (IXamlSubscription) new XamlProjectSubscription.XamlSceneSubscription(view.ViewModel, this, this.sceneSearch);
          this.watchers.Add(document, xamlSubscription);
        }
        else
        {
          xamlSubscription = (IXamlSubscription) new XamlProjectSubscription.XamlDocumentSubscription(document, this, this.documentSearch);
          this.watchers.Add(document, xamlSubscription);
        }
      }
      return xamlSubscription;
    }

    private void ViewService_ViewOpenedOrClosed(object sender, ViewEventArgs e)
    {
      this.OnCatastrophicUpdate();
    }

    private SceneView FindView(IDocumentContext documentContext)
    {
      foreach (IView view in (IEnumerable<IView>) this.designerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && sceneView.ViewModel.Document.DocumentContext == documentContext)
          return sceneView;
      }
      return (SceneView) null;
    }

    internal void OnDocumentNodeAdded(IXamlSubscription sender, DocumentNode newNode, DocumentNodePath watchedRoot)
    {
      if (this.PathNodeInserted == null)
        return;
      this.PathNodeInserted(sender, newNode, watchedRoot);
    }

    internal void OnDocumentNodeRemoved(IXamlSubscription sender, DocumentNode oldNode, DocumentNodePath watchedRoot)
    {
      if (this.PathNodeRemoved == null)
        return;
      this.PathNodeRemoved(sender, oldNode, watchedRoot);
    }

    private abstract class BaseXamlSubscription<T>
    {
      private Dictionary<T, KeyValuePair<DocumentNodePath, int>> oldBasisNodes;
      private Dictionary<T, KeyValuePair<DocumentNodePath, int>> newBasisNodes;
      private XamlProjectSubscription parentXamlSubscription;

      public bool IsEmpty
      {
        get
        {
          return this.ImmediateBasisNodes.Count == 0;
        }
      }

      public XamlProjectSubscription ProjectSubscription
      {
        get
        {
          return this.parentXamlSubscription;
        }
      }

      protected abstract IDocumentRoot DocumentRoot { get; }

      protected Dictionary<T, KeyValuePair<DocumentNodePath, int>> OldBasisNodes
      {
        get
        {
          return this.oldBasisNodes;
        }
      }

      protected Dictionary<T, KeyValuePair<DocumentNodePath, int>> NewBasisNodes
      {
        get
        {
          return this.newBasisNodes;
        }
      }

      protected Dictionary<T, KeyValuePair<DocumentNodePath, int>> ImmediateBasisNodes
      {
        get
        {
          return this.newBasisNodes != null ? this.newBasisNodes : this.oldBasisNodes;
        }
      }

      public BaseXamlSubscription(XamlProjectSubscription parentXamlSubscription)
      {
        this.parentXamlSubscription = parentXamlSubscription;
        this.oldBasisNodes = new Dictionary<T, KeyValuePair<DocumentNodePath, int>>();
      }

      public void Update()
      {
        this.UpdateInternal();
        if (this.newBasisNodes == null)
          return;
        this.oldBasisNodes = this.newBasisNodes;
        this.newBasisNodes = (Dictionary<T, KeyValuePair<DocumentNodePath, int>>) null;
      }

      protected abstract T ProvideBasis(DocumentNodePath nodePath);

      protected abstract T ProvideBasis(DocumentNode node);

      protected virtual void UpdateInternal()
      {
      }

      public bool Register(DocumentNodePath nodePath)
      {
        T key = this.ProvideBasis(nodePath);
        if ((object) key == null)
          return false;
        KeyValuePair<DocumentNodePath, int> keyValuePair;
        if (this.ImmediateBasisNodes.TryGetValue(key, out keyValuePair))
        {
          this.ImmediateBasisNodes[key] = new KeyValuePair<DocumentNodePath, int>(keyValuePair.Key, keyValuePair.Value + 1);
          return false;
        }
        if (this.newBasisNodes == null)
          this.newBasisNodes = new Dictionary<T, KeyValuePair<DocumentNodePath, int>>((IDictionary<T, KeyValuePair<DocumentNodePath, int>>) this.oldBasisNodes);
        this.newBasisNodes.Add(key, new KeyValuePair<DocumentNodePath, int>(nodePath, 1));
        return true;
      }

      public bool Unregister(DocumentNode source)
      {
        T key = this.ProvideBasis(source);
        KeyValuePair<DocumentNodePath, int> keyValuePair;
        if ((object) key != null && this.ImmediateBasisNodes.TryGetValue(key, out keyValuePair))
        {
          if (keyValuePair.Value == 1)
          {
            if (this.newBasisNodes == null)
              this.newBasisNodes = new Dictionary<T, KeyValuePair<DocumentNodePath, int>>((IDictionary<T, KeyValuePair<DocumentNodePath, int>>) this.oldBasisNodes);
            this.newBasisNodes.Remove(key);
            return true;
          }
          this.ImmediateBasisNodes[key] = new KeyValuePair<DocumentNodePath, int>(keyValuePair.Key, keyValuePair.Value - 1);
        }
        return false;
      }

      protected void OnExpressionRemoved(DocumentNodePath basisPath, DocumentCompositeNode compositeNode)
      {
        DocumentNode node = new ExpressionEvaluator((IDocumentRootResolver) this.ProjectSubscription.ProjectContext).EvaluateExpression(basisPath, (DocumentNode) compositeNode);
        if (node == null || node.DocumentRoot == this.DocumentRoot)
          return;
        this.ProjectSubscription.Unregister(node.DocumentRoot as XamlDocument, node);
      }

      protected void OnExpressionAdded(DocumentNodePath basisPath, DocumentCompositeNode compositeNode)
      {
        if (compositeNode.Parent == null || !compositeNode.IsProperty)
          return;
        DocumentNode newContainer = new ExpressionEvaluator((IDocumentRootResolver) this.ProjectSubscription.ProjectContext).EvaluateExpression(basisPath, (DocumentNode) compositeNode);
        if (newContainer == null || newContainer.DocumentRoot == this.DocumentRoot)
          return;
        basisPath.GetPathInContainer((DocumentNode) compositeNode.Parent);
        DocumentNodePath pathInSubContainer = basisPath.GetPathInSubContainer(compositeNode.SitePropertyKey, newContainer);
        this.ProjectSubscription.Register(newContainer.DocumentRoot as XamlDocument, pathInSubContainer);
      }
    }

    private class XamlDocumentSubscription : XamlProjectSubscription.BaseXamlSubscription<DocumentNode>, IXamlSubscription, IDisposable
    {
      private XamlProjectSubscription.DocumentNodeFilter searchPath;
      private XamlDocument xamlDocument;

      protected override IDocumentRoot DocumentRoot
      {
        get
        {
          return (IDocumentRoot) this.xamlDocument;
        }
      }

      public XamlDocumentSubscription(XamlDocument document, XamlProjectSubscription parentXamlSubscription, XamlProjectSubscription.DocumentNodeFilter searchPath)
        : base(parentXamlSubscription)
      {
        this.searchPath = searchPath;
        this.xamlDocument = document;
      }

      public void Dispose()
      {
      }

      protected override DocumentNode ProvideBasis(DocumentNodePath nodePath)
      {
        return nodePath.Node;
      }

      protected override DocumentNode ProvideBasis(DocumentNode node)
      {
        return node;
      }

      protected override void UpdateInternal()
      {
        foreach (KeyValuePair<DocumentNodePath, int> keyValuePair in this.OldBasisNodes.Values)
        {
          DocumentNode documentNode = this.ProvideBasis(keyValuePair.Key);
          if (!this.NewBasisNodes.ContainsKey(documentNode))
            this.WalkDescendants(keyValuePair.Key, keyValuePair.Key, documentNode, new XamlProjectSubscription.XamlDocumentSubscription.NodeChangeCallback(this.OnNodeRemoved));
        }
        foreach (KeyValuePair<DocumentNodePath, int> keyValuePair in this.NewBasisNodes.Values)
        {
          DocumentNode documentNode = this.ProvideBasis(keyValuePair.Key);
          if (!this.OldBasisNodes.ContainsKey(documentNode))
            this.WalkDescendants(keyValuePair.Key, keyValuePair.Key, documentNode, new XamlProjectSubscription.XamlDocumentSubscription.NodeChangeCallback(this.OnNodeAdded));
        }
      }

      private void OnNodeRemoved(DocumentNodePath basis, DocumentNode oldNode)
      {
        DocumentCompositeNode compositeNode = oldNode as DocumentCompositeNode;
        if (compositeNode != null && compositeNode.Type.IsExpression)
          this.OnExpressionRemoved(basis, compositeNode);
        this.ProjectSubscription.OnDocumentNodeRemoved((IXamlSubscription) this, oldNode, basis);
      }

      private void OnNodeAdded(DocumentNodePath basis, DocumentNode newNode)
      {
        DocumentCompositeNode compositeNode = newNode as DocumentCompositeNode;
        if (compositeNode != null && compositeNode.Type.IsExpression)
          this.OnExpressionAdded(basis, compositeNode);
        this.ProjectSubscription.OnDocumentNodeAdded((IXamlSubscription) this, newNode, basis);
      }

      private void WalkDescendants(DocumentNodePath basisPath, DocumentNodePath rootPath, DocumentNode currentNode, XamlProjectSubscription.XamlDocumentSubscription.NodeChangeCallback callback)
      {
        this.Visit(basisPath, rootPath, currentNode, callback);
        foreach (DocumentNode current in currentNode.DescendantNodes)
          this.Visit(basisPath, rootPath, current, callback);
      }

      private void Visit(DocumentNodePath basisPath, DocumentNodePath rootPath, DocumentNode current, XamlProjectSubscription.XamlDocumentSubscription.NodeChangeCallback callback)
      {
        foreach (DocumentNode newNode in this.searchPath(current))
          callback(basisPath, newNode);
        if (!current.Type.IsExpression || current.Parent == null || !current.IsProperty)
          return;
        DocumentNode documentNode = new ExpressionEvaluator((IDocumentRootResolver) this.ProjectSubscription.ProjectContext).EvaluateExpression(basisPath, current);
        if (documentNode == null || documentNode == current)
          return;
        if (documentNode.DocumentRoot != this.DocumentRoot)
        {
          callback(basisPath, current);
        }
        else
        {
          DocumentNodePath pathInSubContainer = rootPath.GetPathInContainer(current).GetPathInSubContainer(current.SitePropertyKey, documentNode);
          this.WalkDescendants(basisPath, pathInSubContainer, documentNode, callback);
        }
      }

      private delegate void NodeChangeCallback(DocumentNodePath basis, DocumentNode newNode);
    }

    public delegate void DocumentNodeInsertedHandler(IXamlSubscription sender, DocumentNode newNode, DocumentNodePath watchedRoot);

    public delegate void DocumentNodeRemovedHandler(IXamlSubscription sender, DocumentNode oldNode, DocumentNodePath watchedRoot);

    public delegate IEnumerable<DocumentNode> DocumentNodeFilter(DocumentNode current);

    private class XamlSceneSubscription : XamlProjectSubscription.BaseXamlSubscription<SceneNode>, IXamlSubscription, IDisposable
    {
      private SceneViewModel viewModel;
      private SceneNodeSubscription<SceneNode, SceneNode> subscription;

      protected override IDocumentRoot DocumentRoot
      {
        get
        {
          return this.viewModel.DocumentRoot;
        }
      }

      public XamlSceneSubscription(SceneViewModel viewModel, XamlProjectSubscription parentXamlSubscription, SearchPath searchPath)
        : base(parentXamlSubscription)
      {
        this.viewModel = viewModel;
        this.subscription = new SceneNodeSubscription<SceneNode, SceneNode>();
        this.subscription.Path = searchPath;
        this.subscription.PathNodeInserted += new SceneNodeSubscription<SceneNode, SceneNode>.PathNodeInsertedListener(this.Subscription_PathNodeInserted);
        this.subscription.PathNodeRemoved += new SceneNodeSubscription<SceneNode, SceneNode>.PathNodeRemovedListener(this.Subscription_PathNodeRemoved);
        this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      }

      public void Dispose()
      {
        if (this.subscription != null)
        {
          this.subscription.PathNodeInserted -= new SceneNodeSubscription<SceneNode, SceneNode>.PathNodeInsertedListener(this.Subscription_PathNodeInserted);
          this.subscription.PathNodeRemoved -= new SceneNodeSubscription<SceneNode, SceneNode>.PathNodeRemovedListener(this.Subscription_PathNodeRemoved);
          this.subscription.CurrentViewModel = (SceneViewModel) null;
          this.subscription = (SceneNodeSubscription<SceneNode, SceneNode>) null;
        }
        this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
      }

      protected override SceneNode ProvideBasis(DocumentNodePath nodePath)
      {
        return this.viewModel.GetSceneNode(nodePath.Node);
      }

      protected override SceneNode ProvideBasis(DocumentNode node)
      {
        return this.viewModel.GetSceneNode(node);
      }

      private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
      {
        if (args.IsRadicalChange || args.RootNodeChanged)
        {
          this.ProjectSubscription.OnCatastrophicUpdate();
        }
        else
        {
          if (args.Document == null)
            return;
          this.Update();
          List<SceneNode> list1 = (List<SceneNode>) null;
          List<SceneNode> list2 = (List<SceneNode>) null;
          foreach (SceneNodeSubscription<SceneNode, SceneNode>.BasisNodeInfo basisNodeInfo in this.subscription.BasisNodes)
          {
            SceneNode sceneNode = basisNodeInfo.Node;
            DocumentNodeMarker documentNodeMarker = this.subscription.BasisNodeList.MarkerAt(basisNodeInfo.Index);
            if (documentNodeMarker.IsDeleted && documentNodeMarker.Parent != null && (documentNodeMarker.Parent.Node != null && documentNodeMarker.Property != null))
            {
              DocumentCompositeNode documentCompositeNode = documentNodeMarker.Parent.Node as DocumentCompositeNode;
              if (documentCompositeNode != null)
              {
                DocumentNode node = documentCompositeNode.Properties[(IPropertyId) documentNodeMarker.Property];
                if (node != null)
                {
                  if (list1 == null)
                  {
                    list1 = new List<SceneNode>();
                    list2 = new List<SceneNode>();
                  }
                  list1.Add(this.viewModel.GetSceneNode(node));
                  list2.Add(sceneNode);
                }
              }
            }
          }
          this.subscription.Update(this.viewModel, args.DocumentChanges, args.DocumentChangeStamp);
          if (list1 != null)
          {
            for (int index = 0; index < list1.Count; ++index)
            {
              this.ImmediateBasisNodes[list1[index]] = this.ImmediateBasisNodes[list2[index]];
              this.ImmediateBasisNodes.Remove(list2[index]);
              this.subscription.InsertBasisNode(list1[index]);
            }
          }
          this.ProjectSubscription.UpdateInternal();
        }
      }

      protected override void UpdateInternal()
      {
        if (this.NewBasisNodes == null)
          return;
        this.subscription.SetBasisNodes(this.viewModel, (IEnumerable<SceneNode>) this.NewBasisNodes.Keys);
      }

      private void Subscription_PathNodeRemoved(object sender, SceneNode basisNode, SceneNode basisContent, SceneNode oldPathNode, SceneNode oldContent)
      {
        this.ProjectSubscription.OnDocumentNodeRemoved((IXamlSubscription) this, oldPathNode.DocumentNode, this.OldBasisNodes[basisNode].Key);
        if (!oldPathNode.Type.IsExpression)
          return;
        DocumentCompositeNode compositeNode = oldPathNode.DocumentNode as DocumentCompositeNode;
        if (compositeNode == null)
          return;
        this.OnExpressionRemoved(this.OldBasisNodes[basisNode].Key, compositeNode);
      }

      private void Subscription_PathNodeInserted(object sender, SceneNode basisNode, SceneNode basisContent, SceneNode newPathNode, SceneNode newContent)
      {
        this.ProjectSubscription.OnDocumentNodeAdded((IXamlSubscription) this, newPathNode.DocumentNode, this.ImmediateBasisNodes[basisNode].Key);
        if (!newPathNode.Type.IsExpression)
          return;
        DocumentCompositeNode compositeNode = newPathNode.DocumentNode as DocumentCompositeNode;
        if (compositeNode == null)
          return;
        this.OnExpressionAdded(this.ImmediateBasisNodes[basisNode].Key, compositeNode);
      }
    }
  }
}

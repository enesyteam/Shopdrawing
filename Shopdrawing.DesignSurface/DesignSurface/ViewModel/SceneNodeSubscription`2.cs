// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSubscription`2
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class SceneNodeSubscription<basisT, pathT>
  {
    private SceneNodeSubscription<basisT, pathT>.BasisNodeInsertedHandler basisNodeInsertedHandler;
    private SceneNodeSubscription<basisT, pathT>.PathNodeInsertedHandler pathNodeInsertedHandler;
    private DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.PathNodeHolder> pathNodeList;
    private DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> basisNodeList;
    private SearchPath path;
    private SceneViewModel currentViewModel;
    private XamlDocument currentViewModelDocument;
    private bool documentRootIsStale;
    private uint basisNodesChangeStamp;
    private uint pathNodesChangeStamp;

    public SearchPath Path
    {
      get
      {
        return this.path;
      }
      set
      {
        this.path = value;
      }
    }

    public IEnumerable<SceneNodeSubscription<basisT, pathT>.BasisNodeInfo> BasisNodes
    {
      get
      {
        for (int i = 0; i < this.basisNodeList.Count; ++i)
          yield return this.BasisNodeAt(i);
      }
    }

    public IEnumerable<SceneNodeSubscription<basisT, pathT>.PathNodeInfo> PathNodes
    {
      get
      {
        for (int i = 0; i < this.pathNodeList.Count; ++i)
          yield return this.PathNodeAt(i);
      }
    }

    public DocumentNodeMarkerSortedListBase BasisNodeList
    {
      get
      {
        return (DocumentNodeMarkerSortedListBase) this.basisNodeList;
      }
    }

    public DocumentNodeMarkerSortedListBase PathNodeList
    {
      get
      {
        return (DocumentNodeMarkerSortedListBase) this.pathNodeList;
      }
    }

    public int BasisNodeCount
    {
      get
      {
        return this.basisNodeList.Count;
      }
    }

    public int PathNodeCount
    {
      get
      {
        return this.pathNodeList.Count;
      }
    }

    public uint BasisNodesChangeStamp
    {
      get
      {
        return this.basisNodesChangeStamp;
      }
    }

    public uint PathNodesChangeStamp
    {
      get
      {
        return this.pathNodesChangeStamp;
      }
    }

    public SceneViewModel CurrentViewModel
    {
      get
      {
        return this.currentViewModel;
      }
      set
      {
        if (this.currentViewModel == value)
          return;
        if (this.currentViewModel != null)
        {
          this.currentViewModelDocument.RootNodeChangedOutsideUndo -= new EventHandler(this.DocumentRoot_RootNodeChanged);
          this.currentViewModelDocument.RootNodeChanged -= new EventHandler(this.DocumentRoot_RootNodeChanged);
          this.currentViewModelDocument = (XamlDocument) null;
        }
        this.currentViewModel = value;
        if (this.currentViewModel != null)
        {
          this.currentViewModelDocument = (XamlDocument) this.currentViewModel.XamlDocument;
          this.currentViewModelDocument.RootNodeChangedOutsideUndo += new EventHandler(this.DocumentRoot_RootNodeChanged);
          this.currentViewModelDocument.RootNodeChanged += new EventHandler(this.DocumentRoot_RootNodeChanged);
        }
        this.ClearBasisNodes();
      }
    }

    public event SceneNodeSubscription<basisT, pathT>.BasisNodeInsertedListener BasisNodeInserted;

    public event SceneNodeSubscription<basisT, pathT>.BasisNodeRemovedListener BasisNodeRemoved;

    public event SceneNodeSubscription<basisT, pathT>.PathNodeInsertedListener PathNodeInserted;

    public event SceneNodeSubscription<basisT, pathT>.PathNodeRemovedListener PathNodeRemoved;

    public event SceneNodeSubscription<basisT, pathT>.PathNodeContentChangedListener PathNodeContentChanged;

    public SceneNodeSubscription()
    {
      this.pathNodeList = new DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.PathNodeHolder>();
      this.basisNodeList = new DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder>();
    }

    public void SetBasisNodeInsertedHandler(SceneNodeSubscription<basisT, pathT>.BasisNodeInsertedHandler value)
    {
      this.basisNodeInsertedHandler = value;
    }

    public void SetPathNodeInsertedHandler(SceneNodeSubscription<basisT, pathT>.PathNodeInsertedHandler value)
    {
      this.pathNodeInsertedHandler = value;
    }

    public void SetSceneRootNodeAsTheBasisNode(SceneViewModel viewModel)
    {
      if (viewModel.RootNode == null)
      {
        this.ClearBasisNodes();
      }
      else
      {
        DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> newBasisNodeList = new DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder>();
        newBasisNodeList.Add(viewModel.RootNode.DocumentNode.Marker, (SceneNodeSubscription<basisT, pathT>.BasisNodeHolder) null);
        this.SetBasisNodesWorker(viewModel, newBasisNodeList);
      }
    }

    public void SetBasisNodes(SceneViewModel viewModel, IEnumerable<SceneNode> basisNodes)
    {
      DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> newBasisNodeList = new DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder>();
      foreach (SceneNode sceneNode in basisNodes)
      {
        if (sceneNode != null)
          newBasisNodeList.Add(sceneNode.DocumentNode.Marker, (SceneNodeSubscription<basisT, pathT>.BasisNodeHolder) null);
      }
      this.SetBasisNodesWorker(viewModel, newBasisNodeList);
    }

    private void SetBasisNodesWorker(SceneViewModel viewModel, DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> newBasisNodeList)
    {
      this.ValidateBasisNodeStateWithViewModel(viewModel);
      DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> markerSortedListOf = this.basisNodeList;
      this.basisNodeList = newBasisNodeList;
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in markerSortedListOf.UnionIdentity((DocumentNodeMarkerSortedListBase) this.basisNodeList))
      {
        if (intersectionResult.RightHandSideIndex == -1)
        {
          this.OnBasisNodeRemoved(markerSortedListOf.ValueAt(intersectionResult.LeftHandSideIndex));
          this.CleanOutPathNodesForBasisNode(markerSortedListOf.ValueAt(intersectionResult.LeftHandSideIndex), -1);
        }
        else if (intersectionResult.LeftHandSideIndex == -1)
        {
          SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder = new SceneNodeSubscription<basisT, pathT>.BasisNodeHolder(this.currentViewModel.GetSceneNode(this.basisNodeList.MarkerAt(intersectionResult.RightHandSideIndex).Node), default (basisT));
          this.basisNodeList.SetValueAt(intersectionResult.RightHandSideIndex, holder);
          this.FireBasisNodeInsertedHandler(holder);
          this.OnBasisNodeInserted(holder);
          this.RecalculateBasisNode(intersectionResult.RightHandSideIndex);
        }
        else
          this.basisNodeList.SetValueAt(intersectionResult.RightHandSideIndex, markerSortedListOf.ValueAt(intersectionResult.LeftHandSideIndex));
      }
    }

    private void ClearBasisNodes()
    {
      while (this.basisNodeList.Count > 0)
        this.RemoveBasisNode(0);
    }

    public void InsertBasisNode(SceneNode newBasisNode)
    {
      this.ValidateBasisNodeStateWithViewModel(newBasisNode.ViewModel);
      if (newBasisNode == null || this.basisNodeList.FindPosition(newBasisNode.DocumentNode.Marker) >= 0)
        return;
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder = new SceneNodeSubscription<basisT, pathT>.BasisNodeHolder(newBasisNode, default (basisT));
      int index = this.basisNodeList.Add(newBasisNode.DocumentNode.Marker, holder);
      this.FireBasisNodeInsertedHandler(holder);
      this.OnBasisNodeInserted(holder);
      this.RecalculateBasisNode(index);
    }

    public void RemoveBasisNode(SceneNode oldBasisNode)
    {
      if (oldBasisNode.DocumentNode.Marker == null)
        return;
      int position = this.basisNodeList.FindPosition(oldBasisNode.DocumentNode.Marker);
      if (position < 0)
        return;
      this.RemoveBasisNode(position);
    }

    public void Recalculate(SceneViewModel viewModel)
    {
      this.ValidateBasisNodeStateWithViewModel(viewModel);
      for (int index = this.basisNodeList.Count - 1; index >= 0; --index)
      {
        if (this.basisNodeList.MarkerAt(index).IsDeleted)
          this.RemoveBasisNode(index);
        else
          this.RecalculateBasisNode(index);
      }
    }

    public void Update(SceneViewModel viewModel, DocumentNodeChangeList damage, uint damageChangeStamp)
    {
      this.ValidateBasisNodeStateWithViewModel(viewModel);
      int count = this.basisNodeList.Count;
      this.ChainUpdate(viewModel, (DocumentNodeMarkerSortedListBase) null, (IEnumerable<SceneNode>) null, damage, damageChangeStamp);
    }

    public void ChainUpdate(SceneViewModel viewModel, DocumentNodeMarkerSortedListBase newBasisNodes, IEnumerable<SceneNode> bonusBasisNodes, DocumentNodeChangeList damage, uint damageChangeStamp)
    {
      this.ValidateBasisNodeStateWithViewModel(viewModel);
      if (this.currentViewModel == null)
        return;
      SearchScope searchScope = (SearchScope) 6;
      SearchScope scope = this.path.Scope;
      bool flag1 = (scope & searchScope) != scope;
      bool flag2 = false;
      if (this.path.NumberOfSteps == 1)
      {
        SearchStep searchStep = this.path.Step(0);
        if (searchStep.Axis == SearchAxis.DocumentDescendant && (searchStep.Predicate.AnalysisScope & (SearchScope) 6) == searchStep.Predicate.AnalysisScope)
          flag2 = true;
      }
      if (newBasisNodes != null || bonusBasisNodes != null)
      {
        DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder> newBasisNodeList = new DocumentNodeMarkerSortedListOf<SceneNodeSubscription<basisT, pathT>.BasisNodeHolder>(newBasisNodes.Count);
        if (newBasisNodes != null)
        {
          for (int index = 0; index < newBasisNodes.Count; ++index)
            newBasisNodeList.Add(newBasisNodes.MarkerAt(index), (SceneNodeSubscription<basisT, pathT>.BasisNodeHolder) null);
        }
        if (bonusBasisNodes != null)
        {
          foreach (SceneNode sceneNode in bonusBasisNodes)
            newBasisNodeList.Add(sceneNode.DocumentNode.Marker, (SceneNodeSubscription<basisT, pathT>.BasisNodeHolder) null);
        }
        this.SetBasisNodesWorker(viewModel, newBasisNodeList);
      }
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.pathNodeList.Intersect((DocumentNodeMarkerSortedListBase) damage, DocumentNodeMarkerSortedListBase.Flags.Contains))
        this.OnPathNodeContentChanged(this.pathNodeList.ValueAt(intersectionResult.LeftHandSideIndex), damage.MarkerAt(intersectionResult.RightHandSideIndex), damage.ValueAt(intersectionResult.RightHandSideIndex));
      if (flag1)
      {
        this.Recalculate(viewModel);
      }
      else
      {
        for (int index = this.basisNodeList.Count - 1; index >= 0; --index)
        {
          if (this.basisNodeList.MarkerAt(index).IsDeleted)
            this.RemoveBasisNode(index);
          else if (flag2)
            this.IncrementalRecalculateBasisNode(index, damage, damageChangeStamp);
          else
            this.RecalculateBasisNode(index);
        }
      }
    }

    public IEnumerable<SceneNodeSubscription<basisT, pathT>.PathNodeInfo> PathNodesFor(SceneNode basisNode)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder = this.basisNodeList.Find(basisNode.DocumentNode.Marker);
      if (holder != null)
      {
        foreach (DocumentNodeMarker marker in holder.PathNodeList.Markers)
        {
          int index = this.pathNodeList.FindPosition(marker);
          yield return this.PathNodeAt(index);
        }
      }
    }

    public IEnumerable<SceneNodeSubscription<basisT, pathT>.PathNodeInfo> PathNodeDescendants(SceneNode ancestorNode)
    {
      int start;
      int end;
      if (this.pathNodeList.FindDescendantRange(ancestorNode.DocumentNode.Marker, out start, out end))
      {
        for (int index = start; index <= end; ++index)
          yield return this.PathNodeAt(index);
      }
    }

    public SceneNodeSubscription<basisT, pathT>.BasisNodeInfo BasisNodeAt(int index)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder basisNodeHolder = this.basisNodeList.ValueAt(index);
      return new SceneNodeSubscription<basisT, pathT>.BasisNodeInfo()
      {
        Index = index,
        Info = basisNodeHolder.Content,
        Node = basisNodeHolder.Node
      };
    }

    public SceneNodeSubscription<basisT, pathT>.PathNodeInfo PathNodeAt(int index)
    {
      SceneNodeSubscription<basisT, pathT>.PathNodeHolder pathNodeHolder = this.pathNodeList.ValueAt(index);
      return new SceneNodeSubscription<basisT, pathT>.PathNodeInfo()
      {
        Index = index,
        Info = pathNodeHolder.Content,
        Node = pathNodeHolder.Node
      };
    }

    public SceneNodeSubscription<basisT, pathT>.BasisNodeInfo FindBasisNode(SceneNode basisNode)
    {
      int index = basisNode.DocumentNode.Marker != null ? this.basisNodeList.FindPosition(basisNode.DocumentNode.Marker) : -1;
      if (index < 0)
        return new SceneNodeSubscription<basisT, pathT>.BasisNodeInfo();
      return this.BasisNodeAt(index);
    }

    public SceneNodeSubscription<basisT, pathT>.PathNodeInfo FindPathNode(SceneNode pathNode)
    {
      int index = pathNode.DocumentNode.Marker != null ? this.pathNodeList.FindPosition(pathNode.DocumentNode.Marker) : -1;
      if (index < 0)
        return new SceneNodeSubscription<basisT, pathT>.PathNodeInfo();
      return this.PathNodeAt(index);
    }

    private void ValidateBasisNodeStateWithViewModel(SceneViewModel viewModel)
    {
      if (this.CurrentViewModel != viewModel)
      {
        this.CurrentViewModel = viewModel;
      }
      else
      {
        if (!this.documentRootIsStale)
          return;
        this.ClearBasisNodes();
        this.documentRootIsStale = false;
      }
    }

    private void DocumentRoot_RootNodeChanged(object sender, EventArgs args)
    {
      this.documentRootIsStale = true;
    }

    private void IncrementalRecalculateBasisNode(int index, DocumentNodeChangeList damage, uint documentChangeStamp)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder basisNodeHolder = this.basisNodeList.ValueAt(index);
      DocumentNodeMarker ancestor = this.basisNodeList.MarkerAt(index);
      if ((int) basisNodeHolder.lastRecalcChangeStamp == (int) documentChangeStamp)
        return;
      SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo = this.BasisNodeAt(index);
      if (damage.Count == 0)
        this.RecalculateBasisNode(index);
      foreach (int index1 in damage.FindDescendants(ancestor))
      {
        DocumentNodeMarker documentNodeMarker = damage.MarkerAt(index1);
        if (documentNodeMarker.IsDeleted)
        {
          this.RemovePathNodes(basisNodeInfo, documentNodeMarker);
          basisNodeHolder.PathNodeList.RemoveSelfAndDescendants(documentNodeMarker);
        }
        else
        {
          foreach (SceneNode sceneNode in this.IncrementalTraverseWithSearchPath(basisNodeHolder.Node, documentNodeMarker))
          {
            this.AddPathNode(basisNodeInfo, sceneNode.DocumentNode.Marker);
            basisNodeHolder.PathNodeList.Add(sceneNode.DocumentNode.Marker);
          }
        }
      }
      basisNodeHolder.lastRecalcChangeStamp = documentChangeStamp;
    }

    private IEnumerable<SceneNode> IncrementalTraverseWithSearchPath(SceneNode basisNode, DocumentNodeMarker rootMarker)
    {
      if (this.path.NumberOfSteps == 1)
      {
        SearchStep step = this.path.Step(0);
        if (step.Axis == SearchAxis.DocumentDescendant && (step.Predicate == null || (step.Predicate.AnalysisScope & (SearchScope) 6) == step.Predicate.AnalysisScope))
        {
          SceneNode rootSceneNode = this.currentViewModel.GetSceneNode(rootMarker.Node);
          if (step.ContinuePredicate != null)
          {
            for (DocumentNode node = rootMarker.Node; node != null && node != basisNode.DocumentNode; node = (DocumentNode) node.Parent)
            {
              if (!step.ContinuePredicate.Test(this.currentViewModel.GetSceneNode(node)))
                yield break;
            }
          }
          if (step.Predicate == null || step.Predicate.Test(rootSceneNode))
            yield return rootSceneNode;
          foreach (SceneNode sceneNode in this.path.Query(rootSceneNode))
            yield return sceneNode;
        }
      }
    }

    private void RecalculateBasisNode(int index)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder basisNodeHolder = this.basisNodeList.ValueAt(index);
      if ((int) this.currentViewModel.XamlDocument.ChangeStamp == (int) basisNodeHolder.lastRecalcChangeStamp)
        return;
      SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo = this.BasisNodeAt(index);
      DocumentNodeMarkerSortedList markerSortedList = basisNodeHolder.PathNodeList;
      basisNodeHolder.PathNodeList = SceneNode.GetMarkerList<SceneNode>(this.path.Query(basisNodeHolder.Node), true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in markerSortedList.UnionIdentity((DocumentNodeMarkerSortedListBase) basisNodeHolder.PathNodeList))
      {
        if (intersectionResult.RightHandSideIndex == -1)
          this.RemovePathNode(basisNodeInfo, this.pathNodeList.FindPosition(markerSortedList.MarkerAt(intersectionResult.LeftHandSideIndex)));
        else if (intersectionResult.LeftHandSideIndex == -1)
        {
          DocumentNodeMarker newMarker = basisNodeHolder.PathNodeList.MarkerAt(intersectionResult.RightHandSideIndex);
          this.AddPathNode(basisNodeInfo, newMarker);
        }
      }
      basisNodeHolder.lastRecalcChangeStamp = this.currentViewModel.XamlDocument.ChangeStamp;
    }

    private void AddPathNode(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, DocumentNodeMarker newMarker)
    {
      SceneNodeSubscription<basisT, pathT>.PathNodeHolder pathNodeHolder = this.pathNodeList.Find(newMarker);
      if (pathNodeHolder != null)
      {
        ++pathNodeHolder.ReferenceCount;
      }
      else
      {
        SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder = new SceneNodeSubscription<basisT, pathT>.PathNodeHolder(default (pathT), this.currentViewModel.GetSceneNode(newMarker.Node), 1);
        this.pathNodeList.Add(newMarker, holder);
        this.FirePathNodeInsertedHandler(basisNodeInfo, holder);
        this.OnPathNodeInserted(basisNodeInfo, holder);
      }
    }

    private void RemoveBasisNode(int index)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeHolder basisNodeHolder = this.basisNodeList.ValueAt(index);
      this.OnBasisNodeRemoved(basisNodeHolder);
      this.CleanOutPathNodesForBasisNode(basisNodeHolder, index);
      this.basisNodeList.RemoveAt(index);
    }

    private void RemovePathNodes(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, DocumentNodeMarker ancestor)
    {
      foreach (int index in this.pathNodeList.FindSelfAndDescendants(ancestor))
      {
        SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder = this.pathNodeList.ValueAt(index);
        --holder.ReferenceCount;
        if (holder.ReferenceCount == 0)
          this.OnPathNodeRemoved(basisNodeInfo, holder);
      }
      this.pathNodeList.RemoveSelfAndDescendants(ancestor);
    }

    private void RemovePathNode(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, int pathNodeListIndex)
    {
      SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder = this.pathNodeList.ValueAt(pathNodeListIndex);
      --holder.ReferenceCount;
      if (holder.ReferenceCount != 0)
        return;
      this.pathNodeList.RemoveAt(pathNodeListIndex);
      this.OnPathNodeRemoved(basisNodeInfo, holder);
    }

    private void CleanOutPathNodesForBasisNode(SceneNodeSubscription<basisT, pathT>.BasisNodeHolder basisNodeHolder, int basisNodeIndex)
    {
      SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo = new SceneNodeSubscription<basisT, pathT>.BasisNodeInfo()
      {
        Index = basisNodeIndex,
        Info = basisNodeHolder.Content,
        Node = basisNodeHolder.Node
      };
      foreach (DocumentNodeMarker ancestor in basisNodeHolder.PathNodeList.Markers)
        this.RemovePathNodes(basisNodeInfo, ancestor);
    }

    private void FireBasisNodeInsertedHandler(SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder)
    {
      if (this.basisNodeInsertedHandler != null)
        holder.Content = this.basisNodeInsertedHandler((object) this, holder.Node);
      else
        holder.Content = default (basisT);
    }

    private void OnBasisNodeInserted(SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder)
    {
      ++this.basisNodesChangeStamp;
      if (this.BasisNodeInserted == null)
        return;
      this.BasisNodeInserted((object) this, holder.Node, holder.Content);
    }

    private void OnBasisNodeRemoved(SceneNodeSubscription<basisT, pathT>.BasisNodeHolder holder)
    {
      ++this.basisNodesChangeStamp;
      if (this.BasisNodeRemoved == null)
        return;
      this.BasisNodeRemoved((object) this, holder.Node, holder.Content);
    }

    private void FirePathNodeInsertedHandler(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder)
    {
      if (this.pathNodeInsertedHandler != null)
        holder.Content = this.pathNodeInsertedHandler((object) this, basisNodeInfo.Node, basisNodeInfo.Info, holder.Node);
      else
        holder.Content = default (pathT);
    }

    private void OnPathNodeInserted(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder)
    {
      ++this.pathNodesChangeStamp;
      if (this.PathNodeInserted == null)
        return;
      this.PathNodeInserted((object) this, basisNodeInfo.Node, basisNodeInfo.Info, holder.Node, holder.Content);
    }

    private void OnPathNodeRemoved(SceneNodeSubscription<basisT, pathT>.BasisNodeInfo basisNodeInfo, SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder)
    {
      ++this.pathNodesChangeStamp;
      if (this.PathNodeRemoved == null)
        return;
      this.PathNodeRemoved((object) this, basisNodeInfo.Node, basisNodeInfo.Info, holder.Node, holder.Content);
    }

    private void OnPathNodeContentChanged(SceneNodeSubscription<basisT, pathT>.PathNodeHolder holder, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      ++this.pathNodesChangeStamp;
      if (this.PathNodeContentChanged == null)
        return;
      this.PathNodeContentChanged((object) this, holder.Node, holder.Content, damageMarker, damage);
    }

    public delegate basisT BasisNodeInsertedHandler(object sender, SceneNode newBasisNode);

    public delegate void BasisNodeInsertedListener(object sender, SceneNode newBasisNode, basisT newContent);

    public delegate void BasisNodeRemovedListener(object sender, SceneNode oldBasisNode, basisT oldContent);

    public delegate pathT PathNodeInsertedHandler(object sender, SceneNode basisNode, basisT basisContent, SceneNode newPathNode);

    public delegate void PathNodeInsertedListener(object sender, SceneNode basisNode, basisT basisContent, SceneNode newPathNode, pathT newContent);

    public delegate void PathNodeRemovedListener(object sender, SceneNode basisNode, basisT basisContent, SceneNode oldPathNode, pathT oldContent);

    public delegate void PathNodeContentChangedListener(object sender, SceneNode pathNode, pathT content, DocumentNodeMarker damageMarker, DocumentNodeChange damage);

    private class PathNodeHolder
    {
      public pathT Content;
      public SceneNode Node;
      public int ReferenceCount;

      public PathNodeHolder(pathT content, SceneNode node, int referenceCount)
      {
        this.Content = content;
        this.Node = node;
        this.ReferenceCount = referenceCount;
      }
    }

    private class BasisNodeHolder
    {
      public DocumentNodeMarkerSortedList PathNodeList;
      public SceneNode Node;
      public basisT Content;
      public uint lastRecalcChangeStamp;

      public BasisNodeHolder(SceneNode node, basisT content)
      {
        this.Node = node;
        this.Content = content;
        this.PathNodeList = new DocumentNodeMarkerSortedList();
        this.lastRecalcChangeStamp = uint.MaxValue;
      }
    }

    public struct BasisNodeInfo
    {
      public SceneNode Node;
      public basisT Info;
      public int Index;
    }

    public struct PathNodeInfo
    {
      public SceneNode Node;
      public pathT Info;
      public int Index;
    }
  }
}

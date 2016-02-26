// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.SearchAxis
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class SearchAxis
  {
    private static SearchAxis documentAncestor = new SearchAxis(SearchAxis.AxisType.DocumentAncestor);
    private static SearchAxis documentParent = new SearchAxis(SearchAxis.AxisType.DocumentParent);
    private static SearchAxis documentChild = new SearchAxis(SearchAxis.AxisType.DocumentChild);
    private static SearchAxis documentDescendant = new SearchAxis(SearchAxis.AxisType.DocumentDescendant);
    private static SearchAxis documentSelfAndDescendant = new SearchAxis(SearchAxis.AxisType.DocumentSelfAndDescendant);
    private static SearchAxis self = new SearchAxis(SearchAxis.AxisType.Self);
    private static SearchAxis root = new SearchAxis(SearchAxis.AxisType.Root);
    private static SearchAxis storyboardContainer = new SearchAxis(SearchAxis.AxisType.StoryboardContainer);
    private static SearchAxis storyboardAncestor = new SearchAxis(SearchAxis.AxisType.StoryboardAncestor);
    private static SearchAxis sceneAncestor = new SearchAxis(SearchAxis.AxisType.SceneAncestor);
    private static SearchAxis sceneParent = new SearchAxis(SearchAxis.AxisType.SceneParent);
    private static SearchAxis sceneChild = new SearchAxis(SearchAxis.AxisType.SceneChild);
    private static SearchAxis sceneDescendant = new SearchAxis(SearchAxis.AxisType.SceneDescendant);
    private static SearchAxis animations = new SearchAxis(SearchAxis.AxisType.Animations);
    private static SearchAxis activeStoryboard = new SearchAxis(SearchAxis.AxisType.ActiveStoryboard);
    private static SearchAxis keyframes = new SearchAxis(SearchAxis.AxisType.KeyFrames);
    private SearchAxis.AxisType axisType;
    private int sortValue;
    private IPropertyId propertyKey;

    public SearchAxis.AxisType Type
    {
      get
      {
        return this.axisType;
      }
    }

    public IPropertyId Property
    {
      get
      {
        return this.propertyKey;
      }
    }

    public int ChildIndex
    {
      get
      {
        return this.sortValue;
      }
    }

    public static SearchAxis Animations
    {
      get
      {
        return SearchAxis.animations;
      }
    }

    public static SearchAxis ActiveStoryboard
    {
      get
      {
        return SearchAxis.activeStoryboard;
      }
    }

    public static SearchAxis KeyFrames
    {
      get
      {
        return SearchAxis.keyframes;
      }
    }

    public static SearchAxis DocumentAncestor
    {
      get
      {
        return SearchAxis.documentAncestor;
      }
    }

    public static SearchAxis DocumentParent
    {
      get
      {
        return SearchAxis.documentParent;
      }
    }

    public static SearchAxis DocumentChild
    {
      get
      {
        return SearchAxis.documentChild;
      }
    }

    public static SearchAxis DocumentDescendant
    {
      get
      {
        return SearchAxis.documentDescendant;
      }
    }

    public static SearchAxis DocumentSelfAndDescendant
    {
      get
      {
        return SearchAxis.documentSelfAndDescendant;
      }
    }

    public static SearchAxis Self
    {
      get
      {
        return SearchAxis.self;
      }
    }

    public static SearchAxis Root
    {
      get
      {
        return SearchAxis.root;
      }
    }

    public static SearchAxis StoryboardContainer
    {
      get
      {
        return SearchAxis.storyboardContainer;
      }
    }

    public static SearchAxis StoryboardAncestor
    {
      get
      {
        return SearchAxis.storyboardAncestor;
      }
    }

    public static SearchAxis SceneAncestor
    {
      get
      {
        return SearchAxis.sceneAncestor;
      }
    }

    public static SearchAxis SceneParent
    {
      get
      {
        return SearchAxis.sceneParent;
      }
    }

    public static SearchAxis SceneChild
    {
      get
      {
        return SearchAxis.sceneChild;
      }
    }

    public static SearchAxis SceneDescendant
    {
      get
      {
        return SearchAxis.sceneDescendant;
      }
    }

    public virtual SearchScope Scope
    {
      get
      {
        return SearchAxis.GetSearchScope(this);
      }
    }

    protected SearchAxis(SearchAxis.AxisType axisType)
    {
      this.axisType = axisType;
    }

    public SearchAxis(int childIndex)
    {
      this.axisType = SearchAxis.AxisType.CollectionChild;
      this.sortValue = childIndex;
    }

    public SearchAxis(IPropertyId propertyKey)
    {
      this.axisType = SearchAxis.AxisType.Property;
      this.propertyKey = propertyKey;
    }

    public virtual IEnumerable<SceneNode> Enumerate(SceneNode pivot, ISearchPredicate continueTester)
    {
      return SearchAxis.Enumerate(pivot, this, continueTester);
    }

    protected static SearchScope GetSearchScope(SearchAxis axis)
    {
      switch (axis.axisType)
      {
        case SearchAxis.AxisType.DocumentAncestor:
        case SearchAxis.AxisType.DocumentParent:
        case SearchAxis.AxisType.Root:
          return SearchScope.NodeTreeAncestor;
        case SearchAxis.AxisType.DocumentChild:
        case SearchAxis.AxisType.DocumentDescendant:
        case SearchAxis.AxisType.Animations:
        case SearchAxis.AxisType.KeyFrames:
        case SearchAxis.AxisType.Property:
        case SearchAxis.AxisType.CollectionChild:
          return SearchScope.NodeTreeDescendant;
        case SearchAxis.AxisType.DocumentSelfAndDescendant:
          return (SearchScope) 6;
        case SearchAxis.AxisType.Self:
          return SearchScope.NodeTreeSelf;
        case SearchAxis.AxisType.StoryboardContainer:
        case SearchAxis.AxisType.StoryboardAncestor:
        case SearchAxis.AxisType.ActiveStoryboard:
        case SearchAxis.AxisType.SceneAncestor:
        case SearchAxis.AxisType.SceneParent:
        case SearchAxis.AxisType.SceneChild:
        case SearchAxis.AxisType.SceneDescendant:
          return SearchScope.Unknown;
        default:
          return SearchScope.Unknown;
      }
    }

    protected static IEnumerable<SceneNode> Enumerate(SceneNode pivot, SearchAxis axis, ISearchPredicate continueTester)
    {
      SceneViewModel viewModel = pivot.ViewModel;
      switch (axis.axisType)
      {
        case SearchAxis.AxisType.DocumentAncestor:
          foreach (DocumentNode node in pivot.DocumentNode.AncestorNodes)
          {
            SceneNode sceneNode = viewModel.GetSceneNode(node);
            yield return sceneNode;
            if (continueTester != null && !continueTester.Test(sceneNode))
              break;
          }
          break;
        case SearchAxis.AxisType.DocumentParent:
          DocumentNode parentNode = (DocumentNode) pivot.DocumentNode.Parent;
          if (parentNode == null)
            break;
          yield return viewModel.GetSceneNode(parentNode);
          break;
        case SearchAxis.AxisType.DocumentChild:
          foreach (DocumentNode node in pivot.DocumentNode.ChildNodes)
          {
            SceneNode sceneNode = viewModel.GetSceneNode(node);
            yield return sceneNode;
          }
          break;
        case SearchAxis.AxisType.DocumentDescendant:
          foreach (DocumentNode node in SearchAxis.TraverseDocumentDescendants(pivot.DocumentNode, continueTester, viewModel))
            yield return viewModel.GetSceneNode(node);
          break;
        case SearchAxis.AxisType.DocumentSelfAndDescendant:
          yield return pivot;
          foreach (DocumentNode node in SearchAxis.TraverseDocumentDescendants(pivot.DocumentNode, continueTester, viewModel))
            yield return viewModel.GetSceneNode(node);
          break;
        case SearchAxis.AxisType.Self:
          yield return pivot;
          break;
        case SearchAxis.AxisType.Root:
          yield return viewModel.GetSceneNode(pivot.DocumentNode.DocumentRoot.RootNode);
          break;
        case SearchAxis.AxisType.StoryboardContainer:
          SceneNode thisStoryboardContainer = (SceneNode) pivot.StoryboardContainer;
          if (thisStoryboardContainer == null)
            break;
          yield return thisStoryboardContainer;
          break;
        case SearchAxis.AxisType.StoryboardAncestor:
          for (IStoryboardContainer cur = pivot.StoryboardContainer; cur != null; cur = cur.TargetElement.StoryboardContainer)
          {
            yield return (SceneNode) cur;
            if (continueTester != null && !continueTester.Test((SceneNode) cur))
              break;
          }
          break;
        case SearchAxis.AxisType.ActiveStoryboard:
          if (pivot.ViewModel.ActiveStoryboardTimeline == null)
            break;
          yield return (SceneNode) pivot.ViewModel.ActiveStoryboardTimeline;
          break;
        case SearchAxis.AxisType.Animations:
          StoryboardTimelineSceneNode storyboard = pivot as StoryboardTimelineSceneNode;
          if (storyboard == null)
            break;
          foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) storyboard.Children)
            yield return (SceneNode) timelineSceneNode;
          break;
        case SearchAxis.AxisType.KeyFrames:
          KeyFrameAnimationSceneNode keyFrameAnimation = pivot as KeyFrameAnimationSceneNode;
          if (keyFrameAnimation == null)
            break;
          foreach (KeyFrameSceneNode keyFrameSceneNode in keyFrameAnimation.KeyFrames)
            yield return (SceneNode) keyFrameSceneNode;
          break;
        case SearchAxis.AxisType.SceneAncestor:
          for (SceneNode parent = pivot.Parent; parent != null; parent = parent.Parent)
          {
            yield return parent;
            if (continueTester != null && !continueTester.Test(parent))
              break;
          }
          break;
        case SearchAxis.AxisType.SceneParent:
          SceneNode node1 = pivot.Parent;
          if (node1 == null)
            break;
          yield return node1;
          break;
        case SearchAxis.AxisType.SceneChild:
          DocumentCompositeNode docNode = pivot.DocumentNode as DocumentCompositeNode;
          if (docNode == null)
            break;
          if (docNode.SupportsChildren)
          {
            ISceneNodeCollection<SceneNode> children = pivot.GetChildren();
            for (int i = 0; i < children.Count; ++i)
              yield return children[i];
            break;
          }
          foreach (IPropertyId propertyKey in (IEnumerable<IProperty>) docNode.Properties.Keys)
          {
            SceneNode subNode = pivot.GetLocalValueAsSceneNode(propertyKey);
            yield return subNode;
          }
          break;
        case SearchAxis.AxisType.SceneDescendant:
          IEnumerator<SceneNode> descendantEnumerator = (IEnumerator<SceneNode>) new SearchAxis.SceneDescendantTraversalEnumerator(pivot, continueTester);
          while (descendantEnumerator.MoveNext())
            yield return descendantEnumerator.Current;
          break;
        case SearchAxis.AxisType.Property:
          IProperty resolvedProperty = pivot.ProjectContext.ResolveProperty(axis.Property);
          if (resolvedProperty == null)
            break;
          SceneNode node2 = pivot.GetLocalValueAsSceneNode((IPropertyId) resolvedProperty);
          if (node2 == null)
            break;
          yield return node2;
          break;
        case SearchAxis.AxisType.CollectionChild:
          SceneNode node3 = pivot.GetChildren()[axis.ChildIndex];
          if (node3 == null)
            break;
          yield return node3;
          break;
      }
    }

    private static IEnumerable<DocumentNode> TraverseDocumentDescendants(DocumentNode node, ISearchPredicate continueTester, SceneViewModel viewModel)
    {
      if (node.ChildNodesCount != 0)
      {
        IEnumerator<DocumentNode> descendantEnumerator = node.DescendantNodes.GetEnumerator();
        while (descendantEnumerator.MoveNext())
        {
          DocumentNode current = descendantEnumerator.Current;
          yield return current;
          if (continueTester != null && !continueTester.Test(viewModel.GetSceneNode(current)))
          {
            IDescendantEnumerator descendantEnumerator1 = descendantEnumerator as IDescendantEnumerator;
            if (descendantEnumerator1 != null)
              descendantEnumerator1.SkipPastDescendants(current);
          }
        }
      }
    }

    public enum AxisType
    {
      DocumentAncestor,
      DocumentParent,
      DocumentChild,
      DocumentDescendant,
      DocumentSelfAndDescendant,
      Self,
      Root,
      StoryboardContainer,
      StoryboardAncestor,
      ActiveStoryboard,
      Animations,
      KeyFrames,
      SceneAncestor,
      SceneParent,
      SceneChild,
      SceneDescendant,
      Property,
      CollectionChild,
      Custom,
    }

    private class SceneDescendantTraversalEnumerator : IEnumerator<SceneNode>, IDisposable, IEnumerator
    {
      private static EmptyList<object> EmptyList = new EmptyList<object>();
      private Stack<SceneNode> sceneNodes;
      private Stack<IEnumerator> children;

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public SceneNode Current
      {
        get
        {
          return this.sceneNodes.Peek();
        }
      }

      public SceneDescendantTraversalEnumerator(SceneNode node, ISearchPredicate continueTester)
      {
        this.sceneNodes = new Stack<SceneNode>();
        this.children = new Stack<IEnumerator>();
        this.sceneNodes.Push(node);
        this.children.Push(this.GetChildEnumerator(node));
      }

      private IEnumerator GetChildEnumerator(SceneNode current)
      {
        DocumentCompositeNode documentCompositeNode = current.DocumentNode as DocumentCompositeNode;
        if (documentCompositeNode == null)
          return (IEnumerator) SearchAxis.SceneDescendantTraversalEnumerator.EmptyList;
        if (documentCompositeNode.SupportsChildren)
          return (IEnumerator) current.GetChildren().GetEnumerator();
        return (IEnumerator) documentCompositeNode.Properties.Keys.GetEnumerator();
      }

      void IDisposable.Dispose()
      {
      }

      private SceneNode ProcessChild(SceneNode node, object child)
      {
        SceneNode sceneNode1 = child as SceneNode;
        if (sceneNode1 != null)
          return sceneNode1;
        IPropertyId propertyKey = (IPropertyId) child;
        SceneNode sceneNode2 = node.GetLocalValueAsSceneNode(propertyKey);
        if (sceneNode2 == null)
        {
          DocumentCompositeNode documentCompositeNode = node.DocumentNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            DocumentNode node1 = documentCompositeNode.Properties[propertyKey];
            if (node1 != null)
              sceneNode2 = node.ViewModel.GetSceneNode(node1);
          }
        }
        return sceneNode2;
      }

      public bool MoveNext()
      {
        IEnumerator enumerator = this.children.Peek();
        SceneNode current = (SceneNode) null;
        while (current == null && enumerator.MoveNext())
          current = this.ProcessChild(this.sceneNodes.Peek(), enumerator.Current);
        if (current != null)
        {
          this.sceneNodes.Push(current);
          this.children.Push(this.GetChildEnumerator(current));
          return true;
        }
        this.sceneNodes.Pop();
        this.children.Pop();
        if (this.sceneNodes.Count == 0)
          return false;
        return this.MoveNext();
      }

      public void Reset()
      {
        throw new NotImplementedException();
      }
    }
  }
}

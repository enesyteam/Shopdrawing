// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public sealed class AnnotationEditor : IDisposable
  {
    private AnnotationEditor.NullTool nullTool;
    private SceneViewModel sceneViewModel;
    private SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo> annotationSub;
    private SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo> referenceSub;
    private SearchPath annotationsSearchPath;

    public IEnumerable<AnnotationSceneNode> Annotations
    {
      get
      {
        return Enumerable.Select<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo, AnnotationSceneNode>(this.annotationSub.PathNodes, (Func<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo, AnnotationSceneNode>) (pathNode => pathNode.Info.Annotation));
      }
    }

    private IEnumerable<AnnotationVisual> AnnotationVisuals
    {
      get
      {
        return Enumerable.Select<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo, AnnotationVisual>(Enumerable.Where<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo>(this.annotationSub.PathNodes, (Func<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo, bool>) (pathNode => pathNode.Info.Visual != null)), (Func<SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInfo, AnnotationVisual>) (pathNode => pathNode.Info.Visual));
      }
    }

    public AnnotationService AnnotationService
    {
      get
      {
        return this.sceneViewModel.DesignerContext.AnnotationService;
      }
    }

    private AnnotationLayer AnnotationLayer
    {
      get
      {
        if (this.sceneViewModel.DefaultView == null)
          return (AnnotationLayer) null;
        return this.sceneViewModel.DefaultView.Artboard.AnnotationLayer;
      }
    }

    private AdornerLayer AdornerLayer
    {
      get
      {
        if (this.sceneViewModel.DefaultView == null)
          return (AdornerLayer) null;
        return this.sceneViewModel.DefaultView.AdornerLayer;
      }
    }

    private ToolBehaviorContext ToolContext
    {
      get
      {
        return new ToolBehaviorContext(this.nullTool.ToolContext, (Tool) this.nullTool, this.sceneViewModel.DefaultView);
      }
    }

    public AnnotationEditor(SceneViewModel sceneViewModel)
    {
      this.sceneViewModel = sceneViewModel;
      this.nullTool = new AnnotationEditor.NullTool(this.sceneViewModel.DesignerContext.ToolContext);
      if (this.AnnotationService != null)
      {
        this.AnnotationService.ShowAnnotationsChanged += new EventHandler(this.AnnotationService_ShowAnnotationsChanged);
        this.AnnotationService.AnnotationsEnabledChanged += new EventHandler(this.AnnotationService_AnnotationsEnabledChanged);
      }
      this.sceneViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SceneViewModel_LateSceneUpdatePhase);
      this.sceneViewModel.Document.EditTransactionCompleting += new EventHandler(this.SceneDocument_EditTransactionCompleting);
      this.annotationsSearchPath = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (sceneNode =>
        {
          if (sceneNode is AnnotationSceneNode)
            return sceneNode.Parent is SceneElement;
          return false;
        }), SearchScope.NodeTreeDescendant))
      });
      this.annotationSub = new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>();
      this.annotationSub.Path = this.annotationsSearchPath;
      this.annotationSub.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInsertedHandler(this.Annotation_InsertedHandler));
      this.annotationSub.PathNodeInserted += new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInsertedListener(this.Annotation_Inserted);
      this.annotationSub.PathNodeContentChanged += new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeContentChangedListener(this.Annotation_Changed);
      this.annotationSub.PathNodeRemoved += new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeRemovedListener(this.Annotation_Removed);
      this.annotationSub.CurrentViewModel = (SceneViewModel) null;
      IProperty annotationReferencesProperty = this.sceneViewModel.ProjectContext.ResolveProperty(AnnotationSceneNode.ReferencesProperty);
      this.referenceSub = new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>();
      this.referenceSub.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (sceneNode =>
        {
          if (sceneNode.DocumentNode.IsProperty)
            return sceneNode.DocumentNode.SitePropertyKey == annotationReferencesProperty;
          return false;
        }), SearchScope.NodeTreeDescendant))
      });
      this.referenceSub.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInsertedHandler(this.References_InsertedHandler));
      this.referenceSub.PathNodeInserted += new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInsertedListener(this.References_Inserted);
      this.referenceSub.PathNodeContentChanged += new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeContentChangedListener(this.References_Changed);
      this.referenceSub.PathNodeRemoved += new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeRemovedListener(this.References_Removed);
      this.referenceSub.CurrentViewModel = (SceneViewModel) null;
    }

    public void Dispose()
    {
      if (this.AnnotationService != null)
      {
        this.AnnotationService.ShowAnnotationsChanged -= new EventHandler(this.AnnotationService_ShowAnnotationsChanged);
        this.AnnotationService.AnnotationsEnabledChanged -= new EventHandler(this.AnnotationService_AnnotationsEnabledChanged);
      }
      this.sceneViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SceneViewModel_LateSceneUpdatePhase);
      this.sceneViewModel.Document.EditTransactionCompleting -= new EventHandler(this.SceneDocument_EditTransactionCompleting);
      this.annotationSub.SetPathNodeInsertedHandler((SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInsertedHandler) null);
      this.annotationSub.PathNodeInserted -= new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeInsertedListener(this.Annotation_Inserted);
      this.annotationSub.PathNodeContentChanged -= new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeContentChangedListener(this.Annotation_Changed);
      this.annotationSub.PathNodeRemoved -= new SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>.PathNodeRemovedListener(this.Annotation_Removed);
      this.annotationSub.CurrentViewModel = (SceneViewModel) null;
      this.annotationSub = (SceneNodeSubscription<object, AnnotationEditor.AnnotationInfo>) null;
      this.referenceSub.SetPathNodeInsertedHandler((SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInsertedHandler) null);
      this.referenceSub.PathNodeInserted -= new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInsertedListener(this.References_Inserted);
      this.referenceSub.PathNodeContentChanged -= new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeContentChangedListener(this.References_Changed);
      this.referenceSub.PathNodeRemoved -= new SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeRemovedListener(this.References_Removed);
      this.referenceSub.CurrentViewModel = (SceneViewModel) null;
      this.referenceSub = (SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>) null;
      GC.SuppressFinalize((object) this);
    }

    internal AnnotationSceneNode CreateAnnotation(IEnumerable<SceneElement> targets)
    {
      Artboard artboard = this.sceneViewModel.DefaultView.Artboard;
      SceneElement sceneElement = Enumerable.First<SceneElement>(targets);
      int num = Enumerable.Count<SceneElement>(targets);
      bool treatTopLeftAsCenter = num > 1 || sceneElement.Equals((object) sceneElement.ViewModel.RootNode);
      AnnotationSceneNode annotation;
      using (SceneEditTransaction editTransaction = sceneElement.ViewModel.CreateEditTransaction(StringTable.AddAnnotationUndoUnit))
      {
        annotation = AnnotationManagerSceneNode.CreateAnnotation((SceneNode) sceneElement);
        foreach (SceneElement element in Enumerable.Skip<SceneElement>(targets, 1))
          AnnotationUtils.AddAnnotationReference(element, annotation);
        Point point1;
        if (treatTopLeftAsCenter)
        {
          Rect rect = Rect.Empty;
          if (num > 1 || !sceneElement.Equals((object) annotation.ViewModel.RootNode))
            rect = artboard.GetElementListBounds(targets);
          if (rect.IsEmpty)
            rect = artboard.ArtboardBounds;
          point1 = new Point(rect.Left + rect.Width / 2.0, rect.Top + rect.Height / 2.0);
        }
        else
          point1 = artboard.GetElementBounds(sceneElement).TopRight;
        Point point2 = RoundingHelper.RoundPosition(point1);
        annotation.Left = point2.X;
        annotation.Top = point2.Y;
        string authorName = this.AnnotationService.DesignerContext.AnnotationsOptionsModel.AuthorName;
        if (!string.IsNullOrEmpty(authorName))
          annotation.Author = authorName;
        string authorInitials = this.AnnotationService.DesignerContext.AnnotationsOptionsModel.AuthorInitials;
        if (!string.IsNullOrEmpty(authorInitials))
          annotation.AuthorInitials = authorInitials;
        AnnotationUtils.SetSerialNumber(annotation);
        editTransaction.Commit();
      }
      this.CreateAnnotationVisual(annotation).Initialize(treatTopLeftAsCenter);
      return annotation;
    }

    public IEnumerable<AnnotationSceneNode> GetAnnotationsWithoutCache()
    {
      return Enumerable.Cast<AnnotationSceneNode>((IEnumerable) this.annotationsSearchPath.Query(this.sceneViewModel.RootNode));
    }

    public IEnumerable<AnnotationSceneNode> GetAttachedAnnotations(SceneElement element)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      if (element == element.ViewModel.RootNode)
        return Enumerable.Empty<AnnotationSceneNode>();
      return Enumerable.Union<AnnotationSceneNode>(Enumerable.Cast<AnnotationSceneNode>((IEnumerable) element.GetCollectionForProperty(AnnotationManagerSceneNode.AnnotationsProperty)), this.GetReferencedAnnotations(element));
    }

    internal IEnumerable<SceneElement> GetAttachedElements(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      List<SceneElement> list = new List<SceneElement>();
      if (annotation.IsInDocument)
      {
        if (annotation.Parent != annotation.ViewModel.RootNode)
        {
          SceneElement sceneElement = annotation.Parent as SceneElement;
          if (sceneElement != null)
            list.Add(sceneElement);
        }
        IEnumerable<SceneElement> collection = Enumerable.Select(Enumerable.Where(Enumerable.Select(this.referenceSub.PathNodes, pathNode => new
        {
          pathNode = pathNode,
          referenceInfo = pathNode.Info
        }), param0 =>
        {
          if (param0.referenceInfo.ReferencedAnnotations.Contains(annotation))
            return param0.referenceInfo.ReferenceOwner != annotation.ViewModel.RootNode;
          return false;
        }), param0 => param0.referenceInfo.ReferenceOwner);
        list.AddRange(collection);
      }
      return (IEnumerable<SceneElement>) list;
    }

    internal AnnotationVisual GetAnnotationVisual(AnnotationSceneNode annotation)
    {
      AnnotationEditor.AnnotationInfo annotationInfo = this.annotationSub.FindPathNode((SceneNode) annotation).Info;
      if (annotationInfo != null)
        return annotationInfo.Visual;
      return Enumerable.FirstOrDefault<AnnotationVisual>(Enumerable.OfType<AnnotationVisual>((IEnumerable) this.sceneViewModel.DefaultView.Artboard.AnnotationLayer.Children), (Func<AnnotationVisual, bool>) (visual => visual.Annotation == annotation));
    }

    private void SceneViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.sceneViewModel.DesignerContext == null || this.sceneViewModel.DesignerContext.ActiveView == null)
        return;
      if (args.IsRadicalChange || this.referenceSub.CurrentViewModel == null || this.annotationSub.CurrentViewModel == null)
      {
        this.annotationSub.SetSceneRootNodeAsTheBasisNode(this.sceneViewModel);
        this.referenceSub.SetSceneRootNodeAsTheBasisNode(this.sceneViewModel);
      }
      else
      {
        this.annotationSub.Update(this.sceneViewModel, args.DocumentChanges, args.DocumentChangeStamp);
        this.referenceSub.Update(this.sceneViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      }
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.AnnotationSelection) || this.AnnotationLayer == null)
        return;
      EnumerableExtensions.ForEach<AnnotationVisual>(this.AnnotationVisuals, (Action<AnnotationVisual>) (visual => visual.ViewModel.RefreshProperty("Selected")));
    }

    private void SceneDocument_EditTransactionCompleting(object sender, EventArgs e)
    {
      SceneElement rootElement = this.sceneViewModel.RootNode as SceneElement;
      if (rootElement == null)
        return;
      Enumerable.ToList<AnnotationSceneNode>(Enumerable.Distinct<AnnotationSceneNode>(Enumerable.SelectMany<SceneElement, AnnotationSceneNode>(Enumerable.Select(Enumerable.Where(Enumerable.Select(Enumerable.Where<DocumentNodeChange>(this.sceneViewModel.Damage.CollapsedChangeList, (Func<DocumentNodeChange, bool>) (docChange => docChange.Action == DocumentNodeChangeAction.Remove)), docChange => new
      {
        docChange = docChange,
        element = this.sceneViewModel.GetSceneNode(docChange.OldChildNode) as SceneElement
      }), param0 => param0.element != null), param0 => param0.element), (Func<SceneElement, IEnumerable<AnnotationSceneNode>>) (elementDeleted => Enumerable.Cast<AnnotationSceneNode>((IEnumerable) this.annotationsSearchPath.Query((SceneNode) elementDeleted)))))).ForEach((Action<AnnotationSceneNode>) (annotation => AnnotationManagerSceneNode.CloneAnnotation(annotation, (SceneNode) rootElement)));
    }

    private AnnotationEditor.AnnotationInfo Annotation_InsertedHandler(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      AnnotationSceneNode annotationSceneNode = (AnnotationSceneNode) newPathNode;
      return new AnnotationEditor.AnnotationInfo()
      {
        Annotation = annotationSceneNode,
        AnnotationOwner = (SceneElement) annotationSceneNode.Parent
      };
    }

    private void Annotation_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode, AnnotationEditor.AnnotationInfo info)
    {
      info.Visual = this.CreateAnnotationVisual(info.Annotation);
      info.Visual.Visibility = Visibility.Visible;
      this.AnnotationService.OnAnnotationAdded(info.Annotation);
      this.RecomputeCachedAnnotationReferences(info.Annotation.Id);
      this.InvalidateAdorners(info.Annotation, true);
    }

    private void Annotation_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, AnnotationEditor.AnnotationInfo info)
    {
      this.AnnotationService.OnAnnotationDeleted(info.Annotation);
      this.RecomputeCachedAnnotationReferences(info.Annotation.Id);
      this.InvalidateAdorners(info.Annotation, true);
      this.InvalidateAdorners(info.AnnotationOwner);
      if (info.Visual == null || info.Visual.Annotation.IsInDocument)
        return;
      this.AnnotationLayer.Children.Remove((UIElement) info.Visual);
    }

    private void Annotation_Changed(object sender, SceneNode pathNode, AnnotationEditor.AnnotationInfo info, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.AnnotationService.OnAnnotationChanged(info.Annotation);
      if (damage.IsPropertyChange)
      {
        if (damage.PropertyKey.Equals((object) AnnotationSceneNode.IdProperty))
          this.RecomputeCachedAnnotationReferences(((DocumentPrimitiveNode) damage.OldChildNode).GetValue<string>(), ((DocumentPrimitiveNode) damage.NewChildNode).GetValue<string>());
        if (info.Visual != null)
          info.Visual.ViewModel.RefreshProperty(damage.PropertyKey.Name);
      }
      this.InvalidateAdorners(info.Annotation, false);
    }

    private AnnotationEditor.ReferenceInfo References_InsertedHandler(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      SceneElement sceneElement = (SceneElement) newPathNode.Parent;
      IEnumerable<string> enumerable = AnnotationUtils.ParseAnnotationReferences(((DocumentPrimitiveNode) newPathNode.DocumentNode).GetValue<string>());
      return new AnnotationEditor.ReferenceInfo()
      {
        ReferenceOwner = sceneElement,
        ReferencedAnnotationIds = Enumerable.ToList<string>(enumerable),
        ReferencedAnnotations = this.ComputeReferencedAnnotations(enumerable)
      };
    }

    private void References_Inserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode, AnnotationEditor.ReferenceInfo info)
    {
      this.InvalidateAdorners(info.ReferenceOwner);
    }

    private void References_Removed(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, AnnotationEditor.ReferenceInfo info)
    {
      this.InvalidateAdorners(info.ReferenceOwner);
    }

    private void References_Changed(object sender, SceneNode pathNode, AnnotationEditor.ReferenceInfo info, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      IEnumerable<string> enumerable = AnnotationUtils.ParseAnnotationReferences(((DocumentPrimitiveNode) pathNode.DocumentNode).GetValue<string>());
      info.ReferencedAnnotationIds = Enumerable.ToList<string>(enumerable);
      info.ReferencedAnnotations = this.ComputeReferencedAnnotations(enumerable);
      this.InvalidateAdorners(info.ReferenceOwner);
    }

    private IEnumerable<AnnotationSceneNode> GetReferencedAnnotations(SceneElement element)
    {
      ExceptionChecks.CheckNullArgument<SceneElement>(element, "element");
      return (IEnumerable<AnnotationSceneNode>) Enumerable.FirstOrDefault<List<AnnotationSceneNode>>(Enumerable.Select<SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInfo, List<AnnotationSceneNode>>(Enumerable.Where<SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInfo>(this.referenceSub.PathNodes, (Func<SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInfo, bool>) (pathNode => pathNode.Info.ReferenceOwner == element)), (Func<SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInfo, List<AnnotationSceneNode>>) (pathNode => pathNode.Info.ReferencedAnnotations))) ?? Enumerable.Empty<AnnotationSceneNode>();
    }

    private void RecomputeCachedAnnotationReferences(params string[] ids)
    {
      foreach (AnnotationEditor.ReferenceInfo referenceInfo in Enumerable.Distinct<AnnotationEditor.ReferenceInfo>(Enumerable.Join(Enumerable.SelectMany(this.referenceSub.PathNodes, (Func<SceneNodeSubscription<object, AnnotationEditor.ReferenceInfo>.PathNodeInfo, IEnumerable<string>>) (pathNode => (IEnumerable<string>) pathNode.Info.ReferencedAnnotationIds), (pathNode, referencedId) => new
      {
        pathNode = pathNode,
        referencedId = referencedId
      }), (IEnumerable<string>) ids, param0 => param0.referencedId, (Func<string, string>) (id => id), (param0, id) => param0.pathNode.Info)))
        referenceInfo.ReferencedAnnotations = this.ComputeReferencedAnnotations((IEnumerable<string>) referenceInfo.ReferencedAnnotationIds);
    }

    private List<AnnotationSceneNode> ComputeReferencedAnnotations(IEnumerable<string> references)
    {
      ExceptionChecks.CheckNullArgument<IEnumerable<string>>(references, "referencesString");
      return Enumerable.ToList<AnnotationSceneNode>(Enumerable.Distinct<AnnotationSceneNode>(Enumerable.Select(Enumerable.Where(Enumerable.Join(this.Annotations, references, (Func<AnnotationSceneNode, string>) (annotation => annotation.Id), (Func<string, string>) (annoId => annoId), (annotation, annoId) => new
      {
        annotation = annotation,
        annoId = annoId
      }), param0 => param0.annotation.IsAttached), param0 => param0.annotation)));
    }

    private AnnotationVisual CreateAnnotationVisual(AnnotationSceneNode annotation)
    {
      ExceptionChecks.CheckNullArgument<AnnotationSceneNode>(annotation, "annotation");
      if (this.AnnotationLayer == null)
        return (AnnotationVisual) null;
      AnnotationVisual annotationVisual1 = Enumerable.FirstOrDefault<AnnotationVisual>(Enumerable.Cast<AnnotationVisual>((IEnumerable) this.AnnotationLayer.Children), (Func<AnnotationVisual, bool>) (v => v.Annotation == annotation));
      if (annotationVisual1 != null)
        return annotationVisual1;
      AnnotationVisual annotationVisual2 = new AnnotationVisual(new AnnotationViewModel(annotation)
      {
        ShowVisibleAtRuntime = annotation.ProjectContext.IsCapabilitySet(PlatformCapability.SupportPrototyping)
      });
      annotationVisual2.Visibility = Visibility.Hidden;
      this.AnnotationLayer.Children.Add((UIElement) annotationVisual2);
      this.AnnotationLayer.UpdateLayout();
      return annotationVisual2;
    }

    internal void InvalidateAdorners(AnnotationSceneNode annotation, bool fullRebuild)
    {
      IEnumerable<AnnotationAdornerSet> items = Enumerable.Select(Enumerable.Where(Enumerable.Select(annotation.AttachedElements, element => new
      {
        element = element,
        adornerSet = this.GetOrCreateAdornerSet(element)
      }), param0 => param0.adornerSet != null), param0 => param0.adornerSet);
      if (fullRebuild)
        EnumerableExtensions.ForEach<AnnotationAdornerSet>(items, (Action<AnnotationAdornerSet>) (adornerSet => adornerSet.InvalidateStructure()));
      else
        EnumerableExtensions.ForEach<AnnotationAdornerSet>(items, (Action<AnnotationAdornerSet>) (adornerSet => adornerSet.InvalidateRender()));
    }

    private void InvalidateAdorners(SceneElement element)
    {
      AnnotationAdornerSet createAdornerSet = this.GetOrCreateAdornerSet(element);
      if (createAdornerSet == null)
        return;
      createAdornerSet.InvalidateStructure();
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() => element.ViewModel.DefaultView.AdornerLayer.Update2D()));
    }

    private AnnotationAdornerSet GetOrCreateAdornerSet(SceneElement owner)
    {
      if (!owner.IsAttached)
        return (AnnotationAdornerSet) null;
      AnnotationAdornerSet annotationAdornerSet = Enumerable.SingleOrDefault<AnnotationAdornerSet>(Enumerable.OfType<AnnotationAdornerSet>((IEnumerable) this.AdornerLayer.Get2DAdornerSets(owner)));
      if (annotationAdornerSet == null)
      {
        annotationAdornerSet = new AnnotationAdornerSet(this.ToolContext, owner);
        this.AdornerLayer.Add((IAdornerSet) annotationAdornerSet);
      }
      return annotationAdornerSet;
    }

    private void AnnotationService_AnnotationsEnabledChanged(object sender, EventArgs e)
    {
      this.UpdateAnnotationsVisibility();
    }

    private void AnnotationService_ShowAnnotationsChanged(object sender, EventArgs e)
    {
      this.UpdateAnnotationsVisibility();
    }

    public void UpdateAnnotationsVisibility()
    {
      if (this.AnnotationService == null)
        return;
      Visibility visibility = this.AnnotationService.AnnotationsEnabled && this.AnnotationService.ShowAnnotations ? Visibility.Visible : Visibility.Collapsed;
      if (this.AnnotationLayer != null && this.AnnotationLayer.Visibility != visibility)
        this.AnnotationLayer.Visibility = visibility;
      if (this.sceneViewModel.DefaultView == null || this.sceneViewModel.DefaultView.AdornerLayer == null)
        return;
      this.sceneViewModel.DefaultView.AdornerLayer.InvalidateAdornerVisuals();
    }

    private class AnnotationInfo
    {
      public AnnotationSceneNode Annotation { get; set; }

      public SceneElement AnnotationOwner { get; set; }

      public AnnotationVisual Visual { get; set; }
    }

    private class ReferenceInfo
    {
      public SceneElement ReferenceOwner { get; set; }

      public List<string> ReferencedAnnotationIds { get; set; }

      public List<AnnotationSceneNode> ReferencedAnnotations { get; set; }
    }

    private class NullTool : Tool
    {
      public override string Identifier
      {
        get
        {
          return string.Empty;
        }
      }

      public override string Caption
      {
        get
        {
          return string.Empty;
        }
      }

      public override Key Key
      {
        get
        {
          return Key.None;
        }
      }

      public override ToolCategory Category
      {
        get
        {
          return ToolCategory.None;
        }
      }

      public NullTool(Microsoft.Expression.DesignSurface.ToolContext toolContext)
        : base(toolContext)
      {
      }

      protected override ToolBehavior CreateToolBehavior()
      {
        return (ToolBehavior) null;
      }
    }
  }
}

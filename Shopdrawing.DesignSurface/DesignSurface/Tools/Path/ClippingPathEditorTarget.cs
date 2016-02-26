// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ClippingPathEditorTarget
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public sealed class ClippingPathEditorTarget : AnimatablePathEditorTarget
  {
    private Base2DElement editingElement;
    private SceneNodeSubscription<PathElement, PathGeometry> basisSubscription;

    public override PathEditMode PathEditMode
    {
      get
      {
        return PathEditMode.ClippingPath;
      }
    }

    public override Base2DElement EditingElement
    {
      get
      {
        return this.editingElement;
      }
    }

    protected override bool IsAnimated
    {
      get
      {
        foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.EditingElement.StoryboardContainer))
        {
          foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
          {
            if (timelineSceneNode2.TargetElement == this.EditingElement && timelineSceneNode2.TargetProperty != null && timelineSceneNode2.TargetProperty.FirstStep.Equals((object) Base2DElement.ClipProperty))
              return true;
          }
        }
        return false;
      }
    }

    public ClippingPathEditorTarget(Base2DElement editingElement)
      : base(editingElement.ViewModel, Base2DElement.ClipProperty, (SceneNode) editingElement)
    {
      this.editingElement = editingElement;
      IPropertyId propertyKey = (IPropertyId) editingElement.ProjectContext.ResolveProperty(Base2DElement.ClipProperty);
      this.basisSubscription = new SceneNodeSubscription<PathElement, PathGeometry>();
      this.basisSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(new SearchAxis(propertyKey))
      });
      this.basisSubscription.InsertBasisNode((SceneNode) editingElement);
      this.basisSubscription.PathNodeInserted += new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeInsertedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.basisSubscription.PathNodeRemoved += new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeRemovedListener(this.OnBasisSubscriptionPathNodeRemoved);
      this.UpdateCachedPath();
    }

    public override Matrix GetTransformToAncestor(IViewObject ancestorViewObject)
    {
      return VectorUtilities.GetMatrixFromTransform(this.EditingElement.ViewModel.DefaultView.ComputeTransformToVisual(this.EditingElement.ViewObject, ancestorViewObject));
    }

    public override void RemovePath()
    {
      this.EndEditing(false);
      this.EditingElement.ClearValue(Base2DElement.ClipProperty);
    }

    public override void PostDeleteAction()
    {
      if (PathGeometryUtilities.IsEmpty(this.PathGeometry))
        this.RemovePath();
      else
        this.EndEditing(false);
    }

    public override void RefreshSubscription()
    {
      if (this.basisSubscription.BasisNodeCount != 0)
        return;
      this.basisSubscription.InsertBasisNode((SceneNode) this.editingElement);
    }

    public override void UpdateFromDamage(SceneUpdatePhaseEventArgs args)
    {
      base.UpdateFromDamage(args);
      this.basisSubscription.Update(this.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    public override void UpdateCachedPath()
    {
      this.PathGeometry = new PathGeometry();
      if (!this.editingElement.IsAttached || this.editingElement.IsSet(Base2DElement.ClipProperty) != PropertyState.Set)
        return;
      System.Windows.Media.Geometry geometry = this.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null ? (System.Windows.Media.Geometry)this.editingElement.GetComputedValueAsWpf(Base2DElement.ClipProperty) : this.editingElement.GetLocalOrDefaultValueAsWpf(Base2DElement.ClipProperty) as System.Windows.Media.Geometry;
      if (geometry == null)
        return;
      this.PathGeometry.AddGeometry(geometry.CloneCurrentValue());
      if (!this.IsCurrentlyEditing)
        return;
      this.EnsureOnlySingleSegments();
    }

    private void OnBasisSubscriptionPathNodeRemoved(object sender, SceneNode basisNode, PathElement basisContent, SceneNode newPathNode, PathGeometry newContent)
    {
      this.UpdateCachedPath();
      this.ViewModel.DefaultView.AdornerLayer.InvalidateAdornerVisuals((SceneElement) this.EditingElement);
      this.ViewModel.DefaultView.AdornerLayer.InvalidateVisual();
    }

    protected override void EndEditingInternal(bool pathJustCreated)
    {
      PathGeometry pathGeometry1 = PathGeometryUtilities.Copy(this.PathGeometry, false);
      bool isAnimated = this.IsAnimated;
      if (!isAnimated && this.ViewModel.AnimationEditor.IsKeyFraming)
      {
          System.Windows.Media.Geometry geometry1 = (System.Windows.Media.Geometry)this.editingElement.GetComputedValueAsWpf(Base2DElement.ClipProperty);
        PathGeometry geometry2 = new PathGeometry();
        if (geometry1 != null)
          geometry2.AddGeometry(geometry1);
        PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(geometry2);
        SceneNode sceneNode = this.editingElement.ViewModel.CreateSceneNode(this.editingElement.ViewModel.DefaultView.ConvertFromWpfValue((object) geometry2));
        sceneNode.SetLocalValueAsWpf(DesignTimeProperties.IsAnimatedProperty, (object) true);
        this.editingElement.SetValueAsSceneNode(Base2DElement.ClipProperty, sceneNode);
        this.editingElement.ViewModel.Document.OnUpdatedEditTransaction();
      }
      if (this.EditingElement.IsAttached && (this.ViewModel.AnimationEditor.IsKeyFraming || isAnimated))
      {
        new PathDiff(this.EditingElement, Base2DElement.ClipProperty, this.PathDiffChangeList).SetPathUsingMinimalDiff(pathGeometry1);
      }
      else
      {
        PathGeometry pathGeometry2 = PathGeometryUtilities.RemoveMapping(pathGeometry1, true);
        this.EditingElement.SetValueAsWpf(Base2DElement.ClipProperty, (object) pathGeometry2);
      }
    }

    protected override void Dispose(bool fromDispose)
    {
      base.Dispose(fromDispose);
      if (this.basisSubscription == null)
        return;
      this.basisSubscription.PathNodeInserted -= new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeInsertedListener(((PathEditorTarget) this).OnBasisSubscriptionPathNodeInserted);
      this.basisSubscription.PathNodeRemoved -= new SceneNodeSubscription<PathElement, PathGeometry>.PathNodeRemovedListener(this.OnBasisSubscriptionPathNodeRemoved);
      this.basisSubscription.CurrentViewModel = (SceneViewModel) null;
      this.basisSubscription = (SceneNodeSubscription<PathElement, PathGeometry>) null;
    }
  }
}

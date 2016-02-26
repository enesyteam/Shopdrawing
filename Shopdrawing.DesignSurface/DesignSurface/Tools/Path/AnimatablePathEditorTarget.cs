// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.AnimatablePathEditorTarget
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public abstract class AnimatablePathEditorTarget : PathEditorTarget
  {
    private SceneNodeSubscription<PathElement, PointKeyFrame> animationSubscription;
    private bool isDeferringUpdate;
    private uint changeStamp;
    private double storedSeekTime;
    private StoryboardTimelineSceneNode activeTimeline;

    protected abstract bool IsAnimated { get; }

    protected AnimatablePathEditorTarget(SceneViewModel viewModel, IPropertyId animatedProperty, SceneNode animatedElement)
      : base(viewModel)
    {
      IPropertyId animationRoot = (IPropertyId) viewModel.ProjectContext.ResolveProperty(animatedProperty);
      this.animationSubscription = new SceneNodeSubscription<PathElement, PointKeyFrame>();
      this.animationSubscription.Path = new SearchPath(new SearchStep[3]
      {
        new SearchStep(SearchAxis.ActiveStoryboard),
        new SearchStep(SearchAxis.Animations, (ISearchPredicate) new AnimationTargetPredicate(animatedElement, animationRoot)),
        new SearchStep(SearchAxis.KeyFrames)
      });
      this.animationSubscription.InsertBasisNode(animatedElement);
      this.animationSubscription.PathNodeInserted += new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeInsertedListener(this.OnAnimationBasisSubscriptionPathNodeChanged);
      this.animationSubscription.PathNodeRemoved += new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeRemovedListener(this.OnAnimationBasisSubscriptionPathNodeChanged);
      this.animationSubscription.PathNodeContentChanged += new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeContentChangedListener(this.animationSubscription_PathNodeContentChanged);
    }

    public override void UpdateFromDamage(SceneUpdatePhaseEventArgs args)
    {
      this.animationSubscription.Update(this.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
    }

    private void animationSubscription_PathNodeContentChanged(object sender, SceneNode pathNode, PointKeyFrame content, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.FireKeyframeUpdate();
    }

    private void OnAnimationBasisSubscriptionPathNodeChanged(object sender, SceneNode basisNode, PathElement basisContent, SceneNode newPathNode, PointKeyFrame newContent)
    {
      this.FireKeyframeUpdate();
    }

    protected override void OnBasisSubscriptionPathNodeInserted(object sender, SceneNode basisNode, PathElement basisContent, SceneNode newPathNode, PathGeometry newContent)
    {
      base.OnBasisSubscriptionPathNodeInserted(sender, basisNode, basisContent, newPathNode, newContent);
      if (this.ViewModel.ActiveStoryboardTimeline == null)
        return;
      this.FireKeyframeUpdate();
    }

    private void FireKeyframeUpdate()
    {
      if (this.isDeferringUpdate)
        return;
      this.isDeferringUpdate = true;
      this.ViewModel.DefaultView.AdornerLayer.InvalidateAdornerVisuals((SceneElement) this.EditingElement);
      this.ViewModel.DefaultView.AdornerLayer.InvalidateVisual();
    }

    internal override void UpdatePathIfNeeded()
    {
      if (this.IsCurrentlyEditing || !this.EditingElement.IsAttached || (int) this.changeStamp == (int) this.ViewModel.ChangeStamp && this.storedSeekTime == this.ViewModel.StoredSeekTime && (this.activeTimeline == this.ViewModel.ActiveStoryboardTimeline && !this.isDeferringUpdate))
        return;
      this.changeStamp = this.ViewModel.ChangeStamp;
      this.storedSeekTime = this.ViewModel.StoredSeekTime;
      this.activeTimeline = this.ViewModel.ActiveStoryboardTimeline;
      if (this.IsAnimated || this.isDeferringUpdate)
        this.UpdateCachedPath();
      this.isDeferringUpdate = false;
    }

    protected override bool ShouldCollapseSegments()
    {
      if (this.ViewModel.AnimationEditor.IsKeyFraming || this.IsAnimated)
        return false;
      return base.ShouldCollapseSegments();
    }

    protected override void Dispose(bool fromDispose)
    {
      base.Dispose(fromDispose);
      if (this.animationSubscription == null)
        return;
      this.animationSubscription.PathNodeInserted -= new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeInsertedListener(this.OnAnimationBasisSubscriptionPathNodeChanged);
      this.animationSubscription.PathNodeRemoved -= new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeRemovedListener(this.OnAnimationBasisSubscriptionPathNodeChanged);
      this.animationSubscription.PathNodeContentChanged -= new SceneNodeSubscription<PathElement, PointKeyFrame>.PathNodeContentChangedListener(this.animationSubscription_PathNodeContentChanged);
      this.animationSubscription.CurrentViewModel = (SceneViewModel) null;
      this.animationSubscription = (SceneNodeSubscription<PathElement, PointKeyFrame>) null;
    }
  }
}

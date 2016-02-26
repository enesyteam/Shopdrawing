// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch.AnimationTargetPredicate
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch
{
  public class AnimationTargetPredicate : ISearchPredicate
  {
    private IPropertyId animationRoot;
    private PropertyReference animationPrefix;
    private SceneNode target;

    public SearchScope AnalysisScope
    {
      get
      {
        return SearchScope.NodeTreeSelf;
      }
    }

    public AnimationTargetPredicate(SceneNode target)
      : this(target, (IPropertyId) null, (PropertyReference) null)
    {
    }

    public AnimationTargetPredicate(IPropertyId animationRoot)
      : this((SceneNode) null, animationRoot, (PropertyReference) null)
    {
    }

    public AnimationTargetPredicate(PropertyReference animationPrefix)
      : this((SceneNode) null, (IPropertyId) null, animationPrefix)
    {
    }

    public AnimationTargetPredicate(SceneNode target, IPropertyId animationRoot)
      : this(target, animationRoot, (PropertyReference) null)
    {
    }

    public AnimationTargetPredicate(SceneNode target, PropertyReference animationPrefix)
      : this(target, (IPropertyId) null, animationPrefix)
    {
    }

    private AnimationTargetPredicate(SceneNode target, IPropertyId animationRoot, PropertyReference animationPrefix)
    {
      this.target = target;
      this.animationRoot = animationRoot;
      this.animationPrefix = animationPrefix;
    }

    public bool Test(SceneNode subject)
    {
      TimelineSceneNode timelineSceneNode = subject as TimelineSceneNode;
      return timelineSceneNode != null && (this.target == null || this.target == timelineSceneNode.TargetElement) && (this.animationRoot == null || timelineSceneNode.TargetProperty != null && timelineSceneNode.TargetProperty[0] == this.animationRoot) && (this.animationPrefix == null || timelineSceneNode.TargetProperty != null && this.animationPrefix.IsPrefixOf(timelineSceneNode.TargetProperty));
    }
  }
}

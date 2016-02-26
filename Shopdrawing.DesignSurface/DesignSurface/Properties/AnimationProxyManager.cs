// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.AnimationProxyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public class AnimationProxyManager
  {
    private SceneViewModel viewModel;
    private StoryboardTimelineSceneNode activeStoryboard;
    private uint lastUpdateChangeStamp;
    private bool detachingFromStoryboard;
    private Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData> targetPathToProxyData;

    public void Attach(SceneViewModel model)
    {
      this.viewModel = model;
      this.targetPathToProxyData = new Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData>();
    }

    public void Detach(SceneViewModel model)
    {
      if (this.viewModel != model)
        return;
      if (this.viewModel != null)
        this.viewModel = (SceneViewModel) null;
      if (this.targetPathToProxyData == null)
        return;
      this.targetPathToProxyData.Clear();
      this.targetPathToProxyData = (Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData>) null;
    }

    public void Update(SceneUpdateTypeFlags updateType, DocumentNodeChangeList damage, uint changeStamp)
    {
      if (this.activeStoryboard == null || damage.Count == 0 || ((int) this.lastUpdateChangeStamp == (int) changeStamp || !this.activeStoryboard.IsInDocument))
        return;
      this.lastUpdateChangeStamp = changeStamp;
      bool flag = false;
      foreach (DocumentNodeChange documentNodeChange in damage.DistinctChanges)
      {
        if (this.activeStoryboard.DocumentNode.Marker.Equals((object) documentNodeChange.ParentNode.Marker) || this.activeStoryboard.DocumentNode.Marker.Contains(documentNodeChange.ParentNode.Marker))
        {
          if (documentNodeChange.IsPropertyChange && documentNodeChange.ParentNode.IsInDocument)
          {
            DocumentCompositeNode documentCompositeNode = documentNodeChange.ParentNode;
            while (documentCompositeNode != null && !PlatformTypes.Timeline.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
              documentCompositeNode = documentCompositeNode.Parent;
            FromToAnimationSceneNode animationSceneNode = this.viewModel.GetSceneNode((DocumentNode) documentCompositeNode) as FromToAnimationSceneNode;
            AnimationProxyManager.AnimationProxyData animationProxyData;
            if (animationSceneNode != null && animationSceneNode.IsOptimized && this.targetPathToProxyData.TryGetValue(new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) animationSceneNode), out animationProxyData))
              animationProxyData.OptimizedAnimationPropertyChanged = true;
          }
          else if (documentNodeChange.Action == DocumentNodeChangeAction.Add && PlatformTypes.Timeline.IsAssignableFrom((ITypeId) documentNodeChange.NewChildNode.Type))
          {
            TimelineSceneNode timeline = this.viewModel.GetSceneNode(documentNodeChange.NewChildNode) as TimelineSceneNode;
            AnimationProxyManager.AnimationProxyData animationProxyData;
            if (timeline != null && AnimationProxyManager.IsOptimizedAnimation(timeline) && this.targetPathToProxyData.TryGetValue(new AnimationProxyManager.TargetNamePropertyPair(timeline), out animationProxyData))
              animationProxyData.OptimizedAnimationAdded = true;
          }
          flag = true;
        }
      }
      if (!flag)
        return;
      if ((updateType & SceneUpdateTypeFlags.UndoRedo) == SceneUpdateTypeFlags.None && (updateType & SceneUpdateTypeFlags.Canceled) == SceneUpdateTypeFlags.None)
        this.UpdateProxyTable(AnimationProxyManager.AllowedChange.Any);
      else
        this.UpdateProxyTable(AnimationProxyManager.AllowedChange.None);
    }

    public void AttachToStoryboard(StoryboardTimelineSceneNode storyboard)
    {
      if (this.activeStoryboard == storyboard)
        return;
      if (this.activeStoryboard != null && this.activeStoryboard.IsInDocument && this.targetPathToProxyData.Count != 0)
      {
        this.detachingFromStoryboard = true;
        try
        {
          foreach (AnimationProxyManager.AnimationProxyData animationProxyData in this.targetPathToProxyData.Values)
          {
            if (animationProxyData.ProxyAnimation != null && animationProxyData.ProxyAnimation.IsInDocument)
              this.viewModel.AnimationEditor.RemoveAnimation(this.activeStoryboard, (TimelineSceneNode) animationProxyData.ProxyAnimation);
          }
          this.targetPathToProxyData.Clear();
        }
        finally
        {
          this.detachingFromStoryboard = false;
        }
      }
      this.activeStoryboard = storyboard;
      if (this.activeStoryboard == null)
        return;
      this.UpdateProxyTable(AnimationProxyManager.AllowedChange.OnlyNonSerializing);
    }

    public FromToAnimationSceneNode GetAnimationForProxy(KeyFrameAnimationSceneNode proxy)
    {
      AnimationProxyManager.AnimationProxyData animationProxyData;
      if (proxy != null && proxy.ControllingStoryboard == this.activeStoryboard && this.targetPathToProxyData.TryGetValue(new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) proxy), out animationProxyData))
        return animationProxyData.OptimizedAnimation;
      return (FromToAnimationSceneNode) null;
    }

    public static bool IsAnimationProxy(TimelineSceneNode timeline)
    {
      KeyFrameAnimationSceneNode animationSceneNode = timeline as KeyFrameAnimationSceneNode;
      if (animationSceneNode != null)
        return animationSceneNode.IsAnimationProxy;
      return false;
    }

    public static bool IsOptimizedAnimation(TimelineSceneNode timeline)
    {
      FromToAnimationSceneNode animationSceneNode = timeline as FromToAnimationSceneNode;
      if (animationSceneNode != null)
        return animationSceneNode.IsOptimized;
      return false;
    }

    public IDisposable ExpandProxies(StoryboardTimelineSceneNode storyboard)
    {
      return (IDisposable) new AnimationProxyManager.ExpandProxiesToken(this, storyboard);
    }

    public IDisposable ExpandAllProxiesInActiveContainer()
    {
      return (IDisposable) new AnimationProxyManager.ExpandAllProxiesToken(this);
    }

    private void UpdateProxyTable(AnimationProxyManager.AllowedChange allowedChange)
    {
      this.UpdateProxyTable(this.targetPathToProxyData, this.activeStoryboard, allowedChange);
    }

    private void UpdateProxyTable(Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData> proxyTable, StoryboardTimelineSceneNode storyboard, AnimationProxyManager.AllowedChange allowedChange)
    {
      if (allowedChange != AnimationProxyManager.AllowedChange.Any)
        proxyTable.Clear();
      List<KeyFrameAnimationSceneNode> list1 = new List<KeyFrameAnimationSceneNode>();
      foreach (TimelineSceneNode timeline in (IEnumerable<TimelineSceneNode>) storyboard.Children)
      {
        FromToAnimationSceneNode animationSceneNode = timeline as FromToAnimationSceneNode;
        KeyFrameAnimationSceneNode nodeToTest = timeline as KeyFrameAnimationSceneNode;
        if (animationSceneNode != null && animationSceneNode.IsOptimized || nodeToTest != null && nodeToTest.IsAnimationProxy)
        {
          AnimationProxyManager.TargetNamePropertyPair key = new AnimationProxyManager.TargetNamePropertyPair(timeline);
          if (key.TargetName != null)
          {
            AnimationProxyManager.AnimationProxyData animationProxyData;
            if (!proxyTable.TryGetValue(key, out animationProxyData))
            {
              animationProxyData = new AnimationProxyManager.AnimationProxyData((FromToAnimationSceneNode) null, (KeyFrameAnimationSceneNode) null);
              proxyTable[key] = animationProxyData;
            }
            if (animationSceneNode != null)
              animationProxyData.OptimizedAnimation = animationSceneNode;
            else
              animationProxyData.ProxyAnimation = nodeToTest;
          }
        }
        else if (nodeToTest != null && this.CanOptimizeAnimation(nodeToTest))
          list1.Add(nodeToTest);
      }
      if (allowedChange != AnimationProxyManager.AllowedChange.OnlyNonSerializing && allowedChange != AnimationProxyManager.AllowedChange.Any)
        return;
      if (allowedChange == AnimationProxyManager.AllowedChange.Any)
      {
        foreach (KeyFrameAnimationSceneNode proxyAnimation in list1)
        {
          FromToAnimationSceneNode optimizedAnimation = this.ConvertToOptimizedAnimation(proxyAnimation);
          AnimationProxyManager.TargetNamePropertyPair key = new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) proxyAnimation);
          AnimationProxyManager.AnimationProxyData animationProxyData;
          if (proxyTable.TryGetValue(key, out animationProxyData))
          {
            if (animationProxyData.OptimizedAnimation != null && animationProxyData.OptimizedAnimation.IsInDocument)
              animationProxyData.OptimizedAnimation.ControllingStoryboard.Children.Remove((TimelineSceneNode) animationProxyData.OptimizedAnimation);
            if (animationProxyData.ProxyAnimation != null && animationProxyData.ProxyAnimation.IsInDocument)
              animationProxyData.ProxyAnimation.ControllingStoryboard.Children.Remove((TimelineSceneNode) animationProxyData.ProxyAnimation);
            proxyTable.Remove(key);
          }
          proxyTable.Add(key, new AnimationProxyManager.AnimationProxyData(optimizedAnimation, proxyAnimation));
        }
      }
      List<AnimationProxyManager.TargetNamePropertyPair> list2 = new List<AnimationProxyManager.TargetNamePropertyPair>();
      foreach (KeyValuePair<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData> keyValuePair in proxyTable)
      {
        AnimationProxyManager.AnimationProxyData animationProxyData = keyValuePair.Value;
        int count = list2.Count;
        if (animationProxyData.ProxyAnimation != null && !keyValuePair.Key.Equals((object) new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) animationProxyData.ProxyAnimation)))
          animationProxyData.ProxyAnimation = (KeyFrameAnimationSceneNode) null;
        if (animationProxyData.OptimizedAnimation != null && !keyValuePair.Key.Equals((object) new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) animationProxyData.OptimizedAnimation)))
          animationProxyData.OptimizedAnimation = (FromToAnimationSceneNode) null;
        if (animationProxyData.OptimizedAnimation == null && animationProxyData.ProxyAnimation == null)
          list2.Add(keyValuePair.Key);
        else if (animationProxyData.OptimizedAnimation == null || !animationProxyData.OptimizedAnimation.IsInDocument)
        {
          if (allowedChange == AnimationProxyManager.AllowedChange.Any)
          {
            if (animationProxyData.ProxyAnimation != null && animationProxyData.ProxyAnimation.IsInDocument)
            {
              this.viewModel.AnimationEditor.RemoveAnimation(storyboard, (TimelineSceneNode) animationProxyData.ProxyAnimation);
              if (animationProxyData.OptimizedAnimation != null && animationProxyData.OptimizedAnimation.IsInDocument)
                animationProxyData.OptimizedAnimation.IsOptimized = false;
            }
            list2.Add(keyValuePair.Key);
          }
        }
        else if (animationProxyData.ProxyWasUserDeleted && !animationProxyData.OptimizedAnimationAdded)
        {
          if (allowedChange == AnimationProxyManager.AllowedChange.Any)
          {
            this.viewModel.AnimationEditor.RemoveAnimation(storyboard, (TimelineSceneNode) animationProxyData.OptimizedAnimation);
            list2.Add(keyValuePair.Key);
          }
        }
        else if (animationProxyData.ProxyAnimation == null || !animationProxyData.ProxyAnimation.IsInDocument)
        {
          if (allowedChange == AnimationProxyManager.AllowedChange.OnlyNonSerializing || allowedChange == AnimationProxyManager.AllowedChange.Any)
          {
            animationProxyData.OptimizedAnimation.Invalidate();
            if (animationProxyData.OptimizedAnimation.TargetElement != null)
            {
              KeyFrameAnimationSceneNode animationSceneNode = KeyFrameAnimationSceneNode.Factory.InstantiateWithTarget(this.viewModel, animationProxyData.OptimizedAnimation.TargetElement, animationProxyData.OptimizedAnimation.TargetProperty, animationProxyData.OptimizedAnimation.StoryboardContainer, KeyFrameAnimationSceneNode.GetKeyFrameAnimationForType(animationProxyData.OptimizedAnimation.AnimatedType, animationProxyData.OptimizedAnimation.ProjectContext));
              animationSceneNode.AddKeyFrame(animationProxyData.OptimizedAnimation.Duration, animationProxyData.OptimizedAnimation.To);
              KeyFrameSceneNode keyFrameAtIndex = animationSceneNode.GetKeyFrameAtIndex(0);
              SceneNode.CopyPropertyValue((SceneNode) animationProxyData.OptimizedAnimation, animationProxyData.OptimizedAnimation.EasingFunctionProperty, (SceneNode) keyFrameAtIndex, keyFrameAtIndex.EasingFunctionProperty);
              keyFrameAtIndex.ShouldSerialize = false;
              animationSceneNode.ShouldSerialize = false;
              animationSceneNode.IsAnimationProxy = true;
              storyboard.Children.Insert(storyboard.Children.IndexOf((TimelineSceneNode) animationProxyData.OptimizedAnimation) + 1, (TimelineSceneNode) animationSceneNode);
              animationProxyData.ProxyAnimation = animationSceneNode;
            }
          }
        }
        else if (!this.CanOptimizeAnimation(animationProxyData.ProxyAnimation))
        {
          if (allowedChange == AnimationProxyManager.AllowedChange.Any)
          {
            this.RemoveOptimizedAndPromoteProxy(animationProxyData);
            list2.Add(keyValuePair.Key);
          }
        }
        else if ((!animationProxyData.OptimizedAnimation.IsOptimized || !AnimationProxyManager.CanFromToAnimationBeOptimized(animationProxyData.OptimizedAnimation)) && allowedChange == AnimationProxyManager.AllowedChange.Any)
        {
          this.viewModel.AnimationEditor.RemoveAnimation(storyboard, (TimelineSceneNode) animationProxyData.ProxyAnimation);
          animationProxyData.OptimizedAnimation.IsOptimized = false;
          list2.Add(keyValuePair.Key);
        }
        if (count == list2.Count && allowedChange == AnimationProxyManager.AllowedChange.Any)
        {
          if (!animationProxyData.OptimizedAnimationPropertyChanged && !animationProxyData.OptimizedAnimationAdded)
            this.MovePropertiesFromProxyToOptimized(animationProxyData);
          else
            this.MovePropertiesFromOptimizedToProxy(animationProxyData);
        }
        animationProxyData.ResetUpdateState();
      }
      foreach (AnimationProxyManager.TargetNamePropertyPair key in list2)
        proxyTable.Remove(key);
    }

    private void RemoveOptimizedAndPromoteProxy(AnimationProxyManager.AnimationProxyData currentData)
    {
      currentData.ProxyAnimation.IsAnimationProxy = false;
      currentData.ProxyAnimation.ShouldSerialize = true;
      foreach (SceneNode sceneNode in currentData.ProxyAnimation.KeyFrames)
        sceneNode.ShouldSerialize = true;
      if (currentData.OptimizedAnimation == null || !currentData.OptimizedAnimation.IsInDocument)
        return;
      this.viewModel.AnimationEditor.RemoveAnimation(this.activeStoryboard, (TimelineSceneNode) currentData.OptimizedAnimation);
    }

    public void UpdateOnDeletion(KeyFrameAnimationSceneNode deletedAnimation)
    {
      AnimationProxyManager.AnimationProxyData animationProxyData;
      if (this.detachingFromStoryboard || !deletedAnimation.IsAnimationProxy || this.activeStoryboard != deletedAnimation.ControllingStoryboard || !this.targetPathToProxyData.TryGetValue(new AnimationProxyManager.TargetNamePropertyPair((TimelineSceneNode) deletedAnimation), out animationProxyData))
        return;
      animationProxyData.ProxyWasUserDeleted = true;
    }

    private static bool CanFromToAnimationBeOptimized(FromToAnimationSceneNode fromToAnimation)
    {
      foreach (IMemberId memberId in (IEnumerable<IProperty>) ((DocumentCompositeNode) fromToAnimation.DocumentNode).Properties.Keys)
      {
        switch (memberId.Name)
        {
          case "To":
          case "Duration":
          case "TargetName":
          case "TargetProperty":
          case "BeginTime":
          case "EasingFunction":
          case "IsOptimized":
          case "ShouldSerialize":
            continue;
          default:
            return false;
        }
      }
      return true;
    }

    private bool CanOptimizeAnimation(KeyFrameAnimationSceneNode nodeToTest)
    {
      TimelineSceneNode.PropertyNodePair elementAndProperty = nodeToTest.TargetElementAndProperty;
      if (elementAndProperty.PropertyReference == null || FromToAnimationSceneNode.GetFromToAnimationForType((ITypeId) elementAndProperty.PropertyReference.ValueTypeId, nodeToTest.ProjectContext) == null || (nodeToTest.KeyFrameCount != 1 || DesignTimeProperties.ExplicitAnimationProperty.Equals((object) elementAndProperty.PropertyReference[0])))
        return false;
      KeyFrameSceneNode keyFrameAtIndex = nodeToTest.GetKeyFrameAtIndex(0);
      Duration? nullable = nodeToTest.GetLocalOrDefaultValueAsWpf(TimelineSceneNode.DurationProperty) as Duration?;
      bool flag = !nullable.HasValue || nullable.Value == Duration.Automatic || nullable.Value.HasTimeSpan && nullable.Value.TimeSpan.TotalSeconds == keyFrameAtIndex.Time;
      if (keyFrameAtIndex.InterpolationType != KeyFrameInterpolationType.Easing || !flag)
        return false;
      foreach (IMemberId memberId in (IEnumerable<IProperty>) ((DocumentCompositeNode) nodeToTest.DocumentNode).Properties.Keys)
      {
        switch (memberId.Name)
        {
          case "Duration":
          case "TargetName":
          case "TargetProperty":
          case "BeginTime":
          case "IsAnimationProxy":
          case "KeyFrames":
          case "ShouldSerialize":
            continue;
          default:
            return false;
        }
      }
      return true;
    }

    private void MovePropertiesFromProxyToOptimized(AnimationProxyManager.AnimationProxyData data)
    {
      if (data.OptimizedAnimation == null || data.ProxyAnimation == null)
        return;
      KeyFrameSceneNode keyFrameAtIndex = data.ProxyAnimation.GetKeyFrameAtIndex(0);
      if (keyFrameAtIndex == null)
        return;
      SceneNode.CopyPropertyValue((SceneNode) keyFrameAtIndex, keyFrameAtIndex.ValueProperty, (SceneNode) data.OptimizedAnimation, data.OptimizedAnimation.ToProperty);
      SceneNode.CopyPropertyValue((SceneNode) keyFrameAtIndex, keyFrameAtIndex.EasingFunctionProperty, (SceneNode) data.OptimizedAnimation, data.OptimizedAnimation.EasingFunctionProperty);
      data.OptimizedAnimation.Duration = keyFrameAtIndex.Time;
    }

    private void MovePropertiesFromOptimizedToProxy(AnimationProxyManager.AnimationProxyData data)
    {
      if (data.OptimizedAnimation == null || data.ProxyAnimation == null)
        return;
      KeyFrameSceneNode keyFrameAtIndex = data.ProxyAnimation.GetKeyFrameAtIndex(0);
      if (keyFrameAtIndex == null)
        return;
      SceneNode.CopyPropertyValue((SceneNode) data.OptimizedAnimation, data.OptimizedAnimation.ToProperty, (SceneNode) keyFrameAtIndex, keyFrameAtIndex.ValueProperty);
      SceneNode.CopyPropertyValue((SceneNode) data.OptimizedAnimation, data.OptimizedAnimation.EasingFunctionProperty, (SceneNode) keyFrameAtIndex, keyFrameAtIndex.EasingFunctionProperty);
      keyFrameAtIndex.Time = data.OptimizedAnimation.Duration;
    }

    private FromToAnimationSceneNode ConvertToOptimizedAnimation(KeyFrameAnimationSceneNode proxyAnimation)
    {
      StoryboardTimelineSceneNode controllingStoryboard = proxyAnimation.ControllingStoryboard;
      FromToAnimationSceneNode animationSceneNode = FromToAnimationSceneNode.Factory.InstantiateWithTarget(this.viewModel, proxyAnimation.TargetElement, proxyAnimation.TargetProperty, proxyAnimation.StoryboardContainer, FromToAnimationSceneNode.GetFromToAnimationForType(proxyAnimation.AnimatedType, proxyAnimation.ProjectContext));
      KeyFrameSceneNode keyFrameAtIndex = proxyAnimation.GetKeyFrameAtIndex(0);
      animationSceneNode.Duration = keyFrameAtIndex.Time;
      SceneNode.CopyPropertyValue((SceneNode) keyFrameAtIndex, keyFrameAtIndex.ValueProperty, (SceneNode) animationSceneNode, animationSceneNode.ToProperty);
      SceneNode.CopyPropertyValue((SceneNode) keyFrameAtIndex, keyFrameAtIndex.EasingFunctionProperty, (SceneNode) animationSceneNode, animationSceneNode.EasingFunctionProperty);
      animationSceneNode.IsOptimized = true;
      proxyAnimation.IsAnimationProxy = true;
      proxyAnimation.ShouldSerialize = false;
      keyFrameAtIndex.ShouldSerialize = false;
      int index = proxyAnimation.ControllingStoryboard.Children.IndexOf((TimelineSceneNode) proxyAnimation);
      proxyAnimation.ControllingStoryboard.Children.Insert(index, (TimelineSceneNode) animationSceneNode);
      return animationSceneNode;
    }

    private sealed class ExpandProxiesToken : IDisposable
    {
      private AnimationProxyManager manager;
      private StoryboardTimelineSceneNode storyboard;
      private Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData> proxyTable;

      public ExpandProxiesToken(AnimationProxyManager manager, StoryboardTimelineSceneNode storyboard)
      {
        this.manager = manager;
        this.storyboard = storyboard;
        this.proxyTable = new Dictionary<AnimationProxyManager.TargetNamePropertyPair, AnimationProxyManager.AnimationProxyData>();
        this.manager.UpdateProxyTable(this.proxyTable, this.storyboard, AnimationProxyManager.AllowedChange.OnlyNonSerializing);
      }

      public void Dispose()
      {
        this.manager.UpdateProxyTable(this.proxyTable, this.storyboard, AnimationProxyManager.AllowedChange.Any);
      }
    }

    private sealed class ExpandAllProxiesToken : IDisposable
    {
      private List<AnimationProxyManager.ExpandProxiesToken> proxyTokens = new List<AnimationProxyManager.ExpandProxiesToken>();
      private AnimationProxyManager manager;

      public ExpandAllProxiesToken(AnimationProxyManager manager)
      {
        this.manager = manager;
        SceneViewModel sceneViewModel = this.manager.viewModel;
        StoryboardTimelineSceneNode storyboardTimeline = sceneViewModel.ActiveStoryboardTimeline;
        foreach (StoryboardTimelineSceneNode storyboard in sceneViewModel.AnimationEditor.EnumerateStoryboardsForContainer(sceneViewModel.ActiveStoryboardContainer))
        {
          if (storyboard != storyboardTimeline)
            this.proxyTokens.Add(new AnimationProxyManager.ExpandProxiesToken(this.manager, storyboard));
        }
      }

      public void Dispose()
      {
        foreach (AnimationProxyManager.ExpandProxiesToken expandProxiesToken in this.proxyTokens)
          expandProxiesToken.Dispose();
      }
    }

    private enum AllowedChange
    {
      None,
      OnlyNonSerializing,
      Any,
    }

    private class AnimationProxyData
    {
      public FromToAnimationSceneNode OptimizedAnimation;
      public KeyFrameAnimationSceneNode ProxyAnimation;
      public bool ProxyWasUserDeleted;
      public bool OptimizedAnimationPropertyChanged;
      public bool OptimizedAnimationAdded;

      public AnimationProxyData(FromToAnimationSceneNode optimizedAnimation, KeyFrameAnimationSceneNode proxyAnimation)
      {
        this.OptimizedAnimation = optimizedAnimation;
        this.ProxyAnimation = proxyAnimation;
      }

      public void ResetUpdateState()
      {
        this.OptimizedAnimationPropertyChanged = false;
        this.OptimizedAnimationAdded = false;
        this.ProxyWasUserDeleted = false;
      }
    }

    private struct TargetNamePropertyPair
    {
      private string targetName;
      private PropertyReference targetProperty;

      public string TargetName
      {
        get
        {
          return this.targetName;
        }
      }

      public TargetNamePropertyPair(TimelineSceneNode timeline)
      {
        this.targetName = timeline.TargetName;
        this.targetProperty = timeline.TargetProperty;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.CopySelectedStatePropertiesCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class CopySelectedStatePropertiesCommand : CopyStatePropertiesCommand
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          if (this.ViewModel.KeyFrameSelectionSet.Count > 0)
            return true;
          if (this.ViewModel.ElementSelectionSet.Count > 0)
          {
            foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.SourceState.Storyboard.Children)
            {
              SceneElement sceneElement = timelineSceneNode.TargetElement as SceneElement;
              if (sceneElement != null && this.ViewModel.ElementSelectionSet.IsSelected(sceneElement))
                return true;
            }
          }
        }
        return false;
      }
    }

    protected override string UndoString
    {
      get
      {
        return StringTable.CopySelectedStatePropertiesCommandUndoUnit;
      }
    }

    public override string CommandName
    {
      get
      {
        return StringTable.CopySelectedStatePropertiesCommandName;
      }
    }

    public CopySelectedStatePropertiesCommand(SceneViewModel viewModel, VisualStateSceneNode sourceState)
      : base(viewModel, sourceState)
    {
    }

    protected override List<TimelineSceneNode> GetAnimationsToCopy()
    {
      List<TimelineSceneNode> list = new List<TimelineSceneNode>();
      if (this.ViewModel.ElementSelectionSet.Count == 0 && this.ViewModel.KeyFrameSelectionSet.Count > 0)
      {
        Dictionary<TimelineSceneNode.PropertyNodePair, KeyFrameAnimationSceneNode> dictionary = new Dictionary<TimelineSceneNode.PropertyNodePair, KeyFrameAnimationSceneNode>();
        foreach (KeyFrameSceneNode keyFrameSceneNode1 in this.ViewModel.KeyFrameSelectionSet.Selection)
        {
          if (!AnimationProxyManager.IsAnimationProxy((TimelineSceneNode) keyFrameSceneNode1.KeyFrameAnimation))
          {
            KeyFrameSceneNode keyFrameSceneNode2 = this.ViewModel.GetSceneNode(keyFrameSceneNode1.DocumentNode.Clone(keyFrameSceneNode1.DocumentNode.DocumentRoot.DocumentContext)) as KeyFrameSceneNode;
            if (keyFrameSceneNode2 != null)
            {
              TimelineSceneNode.PropertyNodePair key1 = new TimelineSceneNode.PropertyNodePair(keyFrameSceneNode1.TargetElement, keyFrameSceneNode1.TargetProperty);
              KeyFrameAnimationSceneNode animationSceneNode;
              if (!dictionary.ContainsKey(key1))
              {
                animationSceneNode = (KeyFrameAnimationSceneNode) KeyFrameAnimationSceneNode.Factory.Instantiate(this.ViewModel, (ITypeId) keyFrameSceneNode1.KeyFrameAnimation.Type);
                DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) animationSceneNode.DocumentNode;
                foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) ((DocumentCompositeNode) keyFrameSceneNode1.KeyFrameAnimation.DocumentNode).Properties)
                {
                  IProperty key2 = keyValuePair.Key;
                  if (!keyFrameSceneNode1.KeyFrameAnimation.KeyFramesProperty.Equals((object) key2) && !DesignTimeProperties.ShouldSerializeProperty.Equals((object) key2) && !DesignTimeProperties.IsAnimationProxyProperty.Equals((object) key2))
                    documentCompositeNode.Properties[(IPropertyId) key2] = keyValuePair.Value.Clone(documentCompositeNode.Context);
                }
                dictionary.Add(key1, animationSceneNode);
                list.Add((TimelineSceneNode) animationSceneNode);
              }
              else
                animationSceneNode = dictionary[key1];
              animationSceneNode.AddKeyFrame(keyFrameSceneNode2);
            }
          }
          else
          {
            FromToAnimationSceneNode animationSceneNode1 = this.ViewModel.AnimationProxyManager != null ? this.ViewModel.AnimationProxyManager.GetAnimationForProxy(keyFrameSceneNode1.KeyFrameAnimation) : (FromToAnimationSceneNode) null;
            if (animationSceneNode1 != null)
            {
              FromToAnimationSceneNode animationSceneNode2 = this.ViewModel.GetSceneNode(animationSceneNode1.DocumentNode.Clone(animationSceneNode1.DocumentContext)) as FromToAnimationSceneNode;
              list.Add((TimelineSceneNode) animationSceneNode2);
              dictionary.Add(animationSceneNode1.TargetElementAndProperty, (KeyFrameAnimationSceneNode) null);
            }
          }
        }
      }
      else if (this.ViewModel.ElementSelectionSet.Count > 0 && this.ViewModel.KeyFrameSelectionSet.Count == 0)
      {
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.SourceState.Storyboard.Children)
        {
          if (timelineSceneNode.ShouldSerialize)
          {
            SceneElement sceneElement = timelineSceneNode.TargetElement as SceneElement;
            if (sceneElement != null && this.ViewModel.ElementSelectionSet.IsSelected(sceneElement))
              list.Add(timelineSceneNode);
          }
        }
      }
      return list;
    }
  }
}

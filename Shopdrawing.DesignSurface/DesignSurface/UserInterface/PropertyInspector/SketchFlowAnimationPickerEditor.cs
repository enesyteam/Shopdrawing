// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SketchFlowAnimationPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class SketchFlowAnimationPickerEditor : SketchFlowPickerEditor<VisualStateGroupSceneNode>
  {
    protected override IPropertyId TargetScreenProperty
    {
      get
      {
        return PlaySketchFlowAnimationActionNode.TargetScreenProperty;
      }
    }

    public override string AutomationID
    {
      get
      {
        return "SketchFlowAnimationPickerChoiceEditor";
      }
    }

    protected override SceneNodeSubscription<VisualStateGroupSceneNode, VisualStateGroupSceneNode> CreateSubscription()
    {
      return new SceneNodeSubscription<VisualStateGroupSceneNode, VisualStateGroupSceneNode>()
      {
        Path = new SearchPath(new SearchStep[1]
        {
          new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (VisualStateGroupSceneNode)), (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(SketchFlowAnimationPickerEditor.IsSketchFlowAnimationNode), SearchScope.NodeTreeSelf))
        })
      };
    }

    protected override IEnumerable<SketchFlowPickerDisplayItem> FindItemsToDisplay(SceneNode container)
    {
      List<SketchFlowPickerDisplayItem> list = new List<SketchFlowPickerDisplayItem>();
      if (container.ViewObject is IViewControl)
      {
        object stateManagerHost = ((IViewControl) container.ViewObject).VisualStateManagerHost;
        if (stateManagerHost != null)
        {
          IType type = this.TypeResolver.GetType(stateManagerHost.GetType());
          if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type))
          {
            foreach (object group in (IEnumerable) this.GetVisualStateGroups(stateManagerHost))
            {
              string visualStateGroupName = this.GetVisualStateGroupName(group);
              if (!string.IsNullOrEmpty(visualStateGroupName) && visualStateGroupName.StartsWith(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, StringComparison.Ordinal))
                list.Add(new SketchFlowPickerDisplayItem(visualStateGroupName, visualStateGroupName.Replace(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, string.Empty), visualStateGroupName));
            }
          }
        }
      }
      return (IEnumerable<SketchFlowPickerDisplayItem>) list;
    }

    private static bool IsSketchFlowAnimationNode(SceneNode node)
    {
      VisualStateGroupSceneNode stateGroupSceneNode = node as VisualStateGroupSceneNode;
      if (stateGroupSceneNode != null)
        return stateGroupSceneNode.IsSketchFlowAnimation;
      return false;
    }
  }
}

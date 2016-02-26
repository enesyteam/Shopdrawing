// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SketchFlowStatePickerEditor
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
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class SketchFlowStatePickerEditor : SketchFlowPickerEditor<VisualStateSceneNode>
  {
    protected override IPropertyId TargetScreenProperty
    {
      get
      {
        return ActivateStateActionNode.TargetScreenProperty;
      }
    }

    public override string AutomationID
    {
      get
      {
        return "StatePickerChoiceEditor";
      }
    }

    protected override SceneNodeSubscription<VisualStateSceneNode, VisualStateSceneNode> CreateSubscription()
    {
      return new SceneNodeSubscription<VisualStateSceneNode, VisualStateSceneNode>()
      {
        Path = new SearchPath(new SearchStep[1]
        {
          new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (VisualStateSceneNode)))
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
              if (!string.IsNullOrEmpty(visualStateGroupName) && !visualStateGroupName.StartsWith(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, StringComparison.Ordinal))
              {
                foreach (object state in (IEnumerable) this.GetVisualStateGroupStates(group))
                {
                  string visualStateName = this.GetVisualStateName(state);
                  if (!string.IsNullOrEmpty(visualStateName) && !visualStateName.StartsWith("_BlendEditTimeState-", StringComparison.Ordinal))
                    list.Add(new SketchFlowPickerDisplayItem(visualStateGroupName, visualStateName, visualStateName));
                }
              }
            }
          }
        }
      }
      return (IEnumerable<SketchFlowPickerDisplayItem>) list;
    }

    protected override void AddGroupDescriptions()
    {
      this.ItemsView.GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription()
      {
        PropertyName = "GroupName"
      });
    }
  }
}

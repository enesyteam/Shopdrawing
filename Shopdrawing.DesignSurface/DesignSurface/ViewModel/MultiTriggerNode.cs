// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MultiTriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class MultiTriggerNode : BaseTriggerNode
  {
    public static readonly IPropertyId ConditionsProperty = (IPropertyId) PlatformTypes.MultiTrigger.GetMember(MemberType.LocalProperty, "Conditions", MemberAccessTypes.Public);
    public static readonly IPropertyId SettersProperty = (IPropertyId) PlatformTypes.MultiTrigger.GetMember(MemberType.LocalProperty, "Setters", MemberAccessTypes.Public);
    public static readonly MultiTriggerNode.ConcreteMultiTriggerNodeFactory Factory = new MultiTriggerNode.ConcreteMultiTriggerNodeFactory();

    public IList<ConditionNode> Conditions
    {
      get
      {
        return (IList<ConditionNode>) new SceneNode.SceneNodeCollection<ConditionNode>((SceneNode) this, MultiTriggerNode.ConditionsProperty);
      }
    }

    public override ISceneNodeCollection<SceneNode> Setters
    {
      get
      {
        return this.GetCollectionForProperty(MultiTriggerNode.SettersProperty);
      }
    }

    public override string PresentationName
    {
      get
      {
        string str = string.Empty;
        foreach (ITriggerConditionNode triggerConditionNode in (IEnumerable<ConditionNode>) this.Conditions)
        {
          if (str == string.Empty)
            str = triggerConditionNode.PresentationName;
          else
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MultiPropertyTriggerPresentationName, new object[2]
            {
              (object) str,
              (object) triggerConditionNode.PresentationName
            });
        }
        if (str == string.Empty)
          str = StringTable.MultiPropertyTriggerEmptyPresentationName;
        return str;
      }
    }

    public class ConcreteMultiTriggerNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MultiTriggerNode();
      }

      public MultiTriggerNode Instantiate(SceneViewModel viewModel)
      {
        return (MultiTriggerNode) this.Instantiate(viewModel, PlatformTypes.MultiTrigger);
      }
    }
  }
}

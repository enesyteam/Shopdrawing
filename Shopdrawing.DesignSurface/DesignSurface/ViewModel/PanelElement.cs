// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PanelElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PanelElement : BaseFrameworkElement
  {
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.Panel.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.Panel.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly IPropertyId IsItemsHostProperty = (IPropertyId) PlatformTypes.Panel.GetMember(MemberType.LocalProperty, "IsItemsHost", MemberAccessTypes.Public);
    public static readonly IPropertyId ZIndexProperty = (IPropertyId) PlatformTypes.Panel.GetMember(MemberType.AttachedProperty, "ZIndex", MemberAccessTypes.Public);
    public static readonly PanelElement.ConcretePanelElementFactory Factory = new PanelElement.ConcretePanelElementFactory();

    public ISceneNodeCollection<SceneNode> Children
    {
      get
      {
        return this.GetCollectionForProperty(PanelElement.ChildrenProperty);
      }
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (modification == SceneNode.Modification.SetValue || modification == SceneNode.Modification.InsertValue)
      {
        ReferenceStep firstStep = propertyReference.FirstStep;
        if (valueToSet is bool && (bool) valueToSet && PanelElement.IsItemsHostProperty.Equals((object) firstStep))
        {
          ISceneNodeCollection<SceneNode> collectionForProperty = this.GetCollectionForProperty(PanelElement.ChildrenProperty);
          if (collectionForProperty != null)
          {
            foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
            {
              SceneElement element = sceneNode as SceneElement;
              if (element != null)
                this.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
            }
            collectionForProperty.Clear();
          }
        }
        else if (PanelElement.ChildrenProperty.Equals((object) firstStep))
        {
          IProperty property = this.ProjectContext.ResolveProperty(PanelElement.IsItemsHostProperty);
          if (property != null)
          {
            object localValue = this.GetLocalValue((IPropertyId) property);
            if (localValue != null && (bool) localValue)
              this.ClearLocalValue((IPropertyId) property);
          }
        }
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcretePanelElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PanelElement();
      }
    }
  }
}

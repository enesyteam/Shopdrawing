// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PropertySceneInsertionPoint
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class PropertySceneInsertionPoint : ISceneInsertionPoint
  {
    private static IPropertyId ContentProperty = (IPropertyId) PlatformTypes.UserControl.GetMember(MemberType.LocalProperty, "Content", MemberAccessTypes.Public);
    private SceneViewModel sceneViewModel;
    private DocumentNodePath nodePath;
    private IProperty targetProperty;

    public SceneNode SceneNode
    {
      get
      {
        return this.sceneViewModel.GetSceneNode(this.nodePath.Node);
      }
    }

    public SceneElement SceneElement
    {
      get
      {
        return this.SceneNode as SceneElement;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.targetProperty;
      }
    }

    public PropertySceneInsertionPoint(SceneViewModel sceneViewModel, DocumentNodePath nodePath, IProperty targetProperty)
    {
      this.sceneViewModel = sceneViewModel;
      this.nodePath = nodePath;
      this.targetProperty = targetProperty;
    }

    public PropertySceneInsertionPoint(SceneElement sceneElement, IProperty targetProperty)
      : this(sceneElement.ViewModel, sceneElement.DocumentNodePath, targetProperty)
    {
    }

    public void Insert(SceneNode nodeToInsert)
    {
      ISceneNodeCollection<SceneNode> collectionForProperty = this.SceneNode.GetCollectionForProperty((IPropertyId) this.Property);
      if (collectionForProperty.FixedCapacity.HasValue && collectionForProperty.Count >= collectionForProperty.FixedCapacity.Value)
        PropertySceneInsertionPoint.Cleanup(collectionForProperty[collectionForProperty.Count - 1]);
      collectionForProperty.Add(nodeToInsert);
      ControlTemplateElement controlTemplateElement = this.SceneNode as ControlTemplateElement;
      if (controlTemplateElement == null)
        return;
      ITypeId templateTargetTypeId = controlTemplateElement.ControlTemplateTargetTypeId;
      VisualStateManagerSceneNode.AddDefaultStates(nodeToInsert, (SceneNode) controlTemplateElement, templateTargetTypeId);
    }

    public static void Cleanup(SceneNode sceneNode)
    {
      SceneElement element = sceneNode as SceneElement;
      if (element != null)
      {
        sceneNode.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
        sceneNode.ViewModel.RemoveElement((SceneNode) element);
      }
      else
      {
        if (sceneNode == null || !sceneNode.ShouldClearAnimation)
          return;
        SceneElement sceneElement = sceneNode.Parent as SceneElement;
        if (sceneElement == null)
          return;
        IPropertyId propertyId = (IPropertyId) sceneElement.GetPropertyForChild(sceneNode);
        PropertyReference property = new PropertyReference((ReferenceStep) sceneElement.ProjectContext.ResolveProperty(propertyId));
        sceneNode.ViewModel.AnimationEditor.DeleteAllAnimationsInPropertySubtree((SceneNode) sceneElement, property);
      }
    }

    public bool CanInsert(ITypeId typeToInsert)
    {
      IType typeToInsert1 = this.SceneNode.ProjectContext.ResolveType(typeToInsert);
      if (typeToInsert1 != null)
        return PropertySceneInsertionPoint.IsTypeCompatible(this.SceneNode, typeToInsert1, this.Property);
      return false;
    }

    public static bool IsTypeCompatible(SceneNode sceneNode, IType typeToInsert, IProperty property)
    {
      if (typeToInsert.RuntimeType == (Type) null || !sceneNode.ProjectContext.PlatformMetadata.IsSupported((ITypeResolver) sceneNode.ProjectContext, (ITypeId) typeToInsert) || PlatformTypes.UserControl.IsAssignableFrom((ITypeId) sceneNode.Type) && PropertySceneInsertionPoint.ContentProperty.Equals((object) property) && sceneNode.ViewModel.RootNode != sceneNode || (PlatformTypes.Window.IsAssignableFrom((ITypeId) typeToInsert) || PlatformTypes.Page.IsAssignableFrom((ITypeId) typeToInsert) || !property.TargetType.IsAssignableFrom(sceneNode.TargetType) || PlatformTypes.IsEffectType((ITypeId) typeToInsert) && property.PropertyType.Equals((object) PlatformTypes.Object)))
        return false;
      if (property.PropertyType.IsAssignableFrom((ITypeId) typeToInsert))
        return true;
      if (!sceneNode.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAutoTabItemWrapping) && ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) sceneNode.TrueTargetTypeId) && (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) typeToInsert) && !ProjectNeutralTypes.TabItem.IsAssignableFrom((ITypeId) typeToInsert)))
        return false;
      if (property.Equals((object) BehaviorHelper.BehaviorsProperty) && ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) typeToInsert) || property.Equals((object) BehaviorHelper.BehaviorTriggersProperty) && (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) typeToInsert) || ProjectNeutralTypes.BehaviorTriggerBase.IsAssignableFrom((ITypeId) typeToInsert)))
        return BehaviorHelper.IsSceneNodeValidHost(sceneNode, typeToInsert);
      Type genericCollectionType = CollectionAdapterDescription.GetGenericCollectionType(PlatformTypeHelper.GetPropertyType(property));
      return genericCollectionType != (Type) null && sceneNode.ProjectContext.GetType(genericCollectionType).IsAssignableFrom((ITypeId) typeToInsert) || PlatformTypes.UIElementCollection.Equals((object) property.PropertyType) && PlatformTypes.UIElement.IsAssignableFrom((ITypeId) typeToInsert) || (PlatformTypes.FlowDocument.IsAssignableFrom((ITypeId) property.PropertyType) || PlatformTypes.InlineCollection.IsAssignableFrom((ITypeId) property.PropertyType) || PlatformTypes.BlockCollection.IsAssignableFrom((ITypeId) property.PropertyType)) && (PlatformTypes.UIElement.IsAssignableFrom((ITypeId) typeToInsert) && (bool) sceneNode.ViewModel.ProjectContext.GetCapabilityValue(PlatformCapability.SupportsInlineUIContainer)) || (PlatformTypes.ItemCollection.Equals((object) property.PropertyType) || ItemsControlElement.ItemsProperty.Equals((object) property));
    }

    public override bool Equals(object obj)
    {
      PropertySceneInsertionPoint sceneInsertionPoint = obj as PropertySceneInsertionPoint;
      if (sceneInsertionPoint != null && this.sceneViewModel == sceneInsertionPoint.sceneViewModel && object.Equals((object) this.nodePath, (object) sceneInsertionPoint.nodePath))
        return object.Equals((object) this.targetProperty, (object) sceneInsertionPoint.targetProperty);
      return false;
    }

    public override int GetHashCode()
    {
      return this.sceneViewModel.GetHashCode() ^ this.nodePath.GetHashCode() ^ this.targetProperty.GetHashCode();
    }

    public override string ToString()
    {
      string str = this.nodePath.ContainerNode.Type.Name + ":" + this.nodePath.ToString() + "." + this.targetProperty.Name;
      DocumentNodePath containerOwnerPath = this.nodePath.GetContainerOwnerPath();
      if (containerOwnerPath != null)
        str = containerOwnerPath.ToString() + "," + str;
      return str;
    }
  }
}

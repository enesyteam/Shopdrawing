// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.StyleNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class StyleNode : Base2DElement, IStoryboardContainer, ITriggerContainer, IResourceContainer
  {
    public static readonly IPropertyId TargetTypeProperty = (IPropertyId) PlatformTypes.Style.GetMember(MemberType.LocalProperty, "TargetType", MemberAccessTypes.Public);
    public static readonly IPropertyId BasedOnProperty = (IPropertyId) PlatformTypes.Style.GetMember(MemberType.LocalProperty, "BasedOn", MemberAccessTypes.Public);
    public static readonly IPropertyId ResourcesProperty = (IPropertyId) PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Resources", MemberAccessTypes.Public);
    public static readonly IPropertyId TriggersProperty = (IPropertyId) PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
    public static readonly IPropertyId SettersProperty = (IPropertyId) PlatformTypes.Style.GetMember(MemberType.LocalProperty, "Setters", MemberAccessTypes.Public);
    public static readonly StyleNode.ConcreteStyleNodeFactory Factory = new StyleNode.ConcreteStyleNodeFactory();

    public override string DisplayName
    {
      get
      {
        return this.GetPropertyName(this.DocumentNodePath);
      }
    }

    public string Key
    {
      get
      {
        string str = (string) null;
        DictionaryEntryNode dictionaryEntryNode = this.FindNameBaseNode as DictionaryEntryNode;
        if (dictionaryEntryNode != null && dictionaryEntryNode.Key != null)
          str = dictionaryEntryNode.Key.ToString();
        return str;
      }
    }

    public override string ContainerDisplayName
    {
      get
      {
        string key = this.Key;
        string str = "Style";
        string name = this.StyleTargetType.Name;
        if (key != null)
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1} {2})", (object) key, (object) name, (object) str);
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", new object[2]
        {
          (object) name,
          (object) str
        });
      }
    }

    public override SceneNode FindNameBaseNode
    {
      get
      {
        if (this.DocumentNode != null)
        {
          DocumentNode node = (DocumentNode) this.DocumentNode.Parent;
          if (node != null)
            return (SceneNode) (this.ViewModel.GetSceneNode(node) as DictionaryEntryNode);
        }
        return base.FindNameBaseNode;
      }
    }

    public IType StyleTargetTypeId
    {
      get
      {
        return DocumentNodeUtilities.GetStyleOrTemplateTargetType(this.DocumentNode);
      }
      set
      {
        DocumentNode valueNode = (DocumentNode) null;
        if (value != null)
          valueNode = (DocumentNode) new DocumentPrimitiveNode(this.DocumentContext, PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) value));
        this.SetLocalValue(((ITargetTypeMetadata) this.Metadata).TargetTypeProperty, valueNode);
      }
    }

    public Type StyleTargetType
    {
      get
      {
        return this.StyleTargetTypeId.NearestResolvedType.RuntimeType;
      }
    }

    public bool SupportsTemplate
    {
      get
      {
        return PlatformTypes.Control.IsAssignableFrom((ITypeId) this.StyleTargetTypeId);
      }
    }

    public ControlTemplateElement ControlTemplate
    {
      get
      {
        return this.GetLocalValueAsSceneNode(ControlElement.TemplateProperty) as ControlTemplateElement;
      }
      set
      {
        this.SetSetterValue((DependencyPropertyReferenceStep) this.Platform.Metadata.ResolveProperty(ControlElement.TemplateProperty), value.DocumentNode);
      }
    }

    public bool IsDefaultStyle
    {
      get
      {
        return StyleNode.IsDefaultValue(this.DocumentNode);
      }
    }

    public ISceneNodeCollection<SceneNode> Setters
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeCollection<SceneNode>((SceneNode) this, StyleNode.SettersProperty);
      }
    }

    public ResourceDictionaryNode Resources
    {
      get
      {
        return ResourceManager.ProvideResourcesForElement((SceneNode) this);
      }
      set
      {
        this.SetValueAsSceneNode(this.Type.Metadata.ResourcesProperty, (SceneNode) value);
      }
    }

    public bool AreResourcesSupported
    {
      get
      {
        return this.Platform.Metadata.ResolveProperty(StyleNode.ResourcesProperty) != null;
      }
    }

    public bool IsBasedOnSupported
    {
      get
      {
        return this.Platform.Metadata.ResolveProperty(StyleNode.BasedOnProperty) != null;
      }
    }

    public override ISceneInsertionPoint DefaultInsertionPoint
    {
      get
      {
        return (ISceneInsertionPoint) null;
      }
    }

    public override SceneNode Parent
    {
      get
      {
        SceneNode sceneNode = this.PathParent;
        if (sceneNode != null)
        {
          SetterSceneNode setterSceneNode = sceneNode as SetterSceneNode;
          if (setterSceneNode != null)
            sceneNode = setterSceneNode.Parent;
        }
        return sceneNode;
      }
    }

    public override IViewObject ViewTargetElement
    {
      get
      {
        if (this == this.ViewModel.ViewRoot)
          return this.ViewModel.DefaultView.ViewRoot;
        foreach (IViewObject viewObject in (IEnumerable<IViewObject>) this.ViewModel.DefaultView.GetInstantiatedElements(this.DocumentNodePath))
        {
          if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) viewObject.GetIType((ITypeResolver) this.ProjectContext)))
            return viewObject;
        }
        return base.ViewTargetElement;
      }
    }

    BaseFrameworkElement IStoryboardContainer.ScopeElement
    {
      get
      {
        return this.TargetElement;
      }
    }

    public BaseFrameworkElement TargetElement
    {
      get
      {
        SetterSceneNode setterSceneNode = this.Parent as SetterSceneNode;
        if (setterSceneNode == null)
          return this.Parent as BaseFrameworkElement;
        SceneNode sceneNode = setterSceneNode.StoryboardContainer.ResolveTargetName(setterSceneNode.Target);
        if (sceneNode == setterSceneNode.StoryboardContainer)
          return ((IStoryboardContainer) sceneNode).TargetElement;
        return sceneNode as BaseFrameworkElement;
      }
    }

    public bool CanEditTriggers
    {
      get
      {
        return this.ViewModel.ProjectContext.ResolveProperty(StyleNode.TriggersProperty) != null;
      }
    }

    public IList<TriggerBaseNode> VisualTriggers
    {
      get
      {
        IList<TriggerBaseNode> list = (IList<TriggerBaseNode>) null;
        IProperty property = this.Platform.Metadata.ResolveProperty(StyleNode.TriggersProperty);
        if (property != null)
          list = (IList<TriggerBaseNode>) new SceneNode.SceneNodeCollection<TriggerBaseNode>((SceneNode) this, (IPropertyId) property);
        return list;
      }
    }

    bool ITriggerContainer.CanEdit
    {
      get
      {
        return !this.IsLockedOrAncestorLocked;
      }
    }

    Type ITriggerContainer.TargetElementType
    {
      get
      {
        return this.StyleTargetType;
      }
    }

    public override string GetDisplayNameFromPath(DocumentNodePath documentNodePathOverride, bool includeTextContent)
    {
      return this.GetPropertyName(documentNodePathOverride);
    }

    private string GetPropertyName(DocumentNodePath documentNodePath)
    {
      SetterSceneNode setterSceneNode = this.Parent as SetterSceneNode;
      if (setterSceneNode != null && setterSceneNode.Property != null)
        return setterSceneNode.Property.Name;
      return this.GetPropertyNameHelper(documentNodePath) ?? this.TargetType.Name;
    }

    public static bool IsDefaultValue(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
        return documentCompositeNode.GetValue<bool>(DesignTimeProperties.IsDefaultStyleProperty);
      return StyleNode.IsReferenceValue(node);
    }

    public static bool IsReferenceValue(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      return documentPrimitiveNode != null && documentPrimitiveNode.Value is DocumentNodeReferenceValue;
    }

    protected override object GetRawComputedValueInternal(PropertyReference propertyReference)
    {
      if (this.IsStyleProperty(propertyReference))
        return base.GetRawComputedValueInternal(propertyReference);
      IViewObject viewTargetElement = this.ViewTargetElement;
      if (viewTargetElement == null || !PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) viewTargetElement.GetIType((ITypeResolver) this.ProjectContext)))
      {
        ReferenceStep referenceStep = propertyReference[propertyReference.Count - 1];
        return referenceStep.GetDefaultValue(PlatformTypeHelper.GetDeclaringType((IMember) referenceStep));
      }
      PropertyReference propertyReference1 = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) this.Type);
      if (propertyReference1 != propertyReference && !DesignTimeProperties.UseShadowPropertyForInstanceBuilding(this.DocumentContext.TypeResolver, (IPropertyId) propertyReference[0]))
        return SceneNode.GetComputedValueWithShadowCoercion(propertyReference, propertyReference, viewTargetElement.PlatformSpecificObject);
      return this.GetCurrentValueFromPropertyReference(propertyReference1, viewTargetElement.PlatformSpecificObject);
    }

    public override IProperty GetPropertyForChild(SceneNode child)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      if (!(this.DocumentNode is DocumentCompositeNode))
        throw new InvalidOperationException(ExceptionStringTable.SceneNodeMustBeCompositeForLogicalChild);
      IProperty property = (IProperty) null;
      DocumentNodePath documentNodePath = child.DocumentNodePath;
      if (documentNodePath.ContainerNode == documentNodePath.Node)
      {
        SetterSceneNode setterSceneNode = this.ViewModel.GetSceneNode(documentNodePath.ContainerOwner) as SetterSceneNode;
        if (setterSceneNode != null)
          property = (IProperty) setterSceneNode.Property;
      }
      else
        property = base.GetPropertyForChild(child);
      return property;
    }

    protected override object GetDefaultValueInternal(IPropertyId propertyKey)
    {
      if (propertyKey.MemberType == MemberType.DesignTimeProperty)
        return base.GetDefaultValueInternal(propertyKey);
      return this.PropertyReferenceFromPropertyKey(propertyKey).FirstStep.GetDefaultValue(this.StyleTargetType);
    }

    public override ReadOnlyCollection<IProperty> GetProperties()
    {
      List<IProperty> list = new List<IProperty>();
      foreach (IProperty property in this.MetadataFactory.GetMetadata(this.StyleTargetType).Properties)
      {
        if (!property.Equals((object) BaseFrameworkElement.StyleProperty))
          list.Add(property);
      }
      if (this.IsBasedOnSupported)
      {
        IProperty property = this.Platform.Metadata.ResolveProperty(StyleNode.BasedOnProperty);
        list.Add(property);
      }
      return new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
    }

    protected override void EnsureViewTransform(PropertyReference transformProperty, object value)
    {
      if (this.ViewTargetElement == null)
        return;
      this.ViewTargetElement.SetValue((ITypeResolver) this.ProjectContext, transformProperty, value);
    }

    public static StyleNode CreateEmptyStyle(SceneViewModel sceneViewModel, IPropertyId propertyKey, ITypeId elementType)
    {
      IProperty property = sceneViewModel.ProjectContext.ResolveProperty(propertyKey);
      StyleNode styleNode = (StyleNode) sceneViewModel.CreateSceneNode(PlatformTypes.Style);
      IProjectContext projectContext = sceneViewModel.ProjectContext;
      Type type1 = projectContext.MetadataFactory.GetMetadata(projectContext.ResolveType(elementType).RuntimeType).GetStylePropertyTargetType((IPropertyId) property);
      if (type1 == (Type) null && !projectContext.IsCapabilitySet(PlatformCapability.SupportsStyleWithoutTargetType))
        type1 = projectContext.PlatformMetadata.ResolveType(PlatformTypes.FrameworkElement).RuntimeType;
      if (type1 != (Type) null)
      {
        IType type2 = projectContext.GetType(type1);
        styleNode.StyleTargetTypeId = type2;
      }
      return styleNode;
    }

    public BaseFrameworkElement GetRelativeTargetElement(DocumentNodePath relativeEditingContainerPath)
    {
      if (relativeEditingContainerPath.ContainerNode != this.DocumentNode)
        return (BaseFrameworkElement) null;
      DocumentNodePath containerOwnerPath = relativeEditingContainerPath.GetContainerOwnerPath();
      if (containerOwnerPath == null)
        return (BaseFrameworkElement) null;
      SetterSceneNode setterSceneNode = this.ViewModel.GetSceneNode(containerOwnerPath.Node) as SetterSceneNode;
      if (setterSceneNode != null)
        return ((ITriggerContainer) this.ViewModel.GetSceneNode(containerOwnerPath.ContainerNode)).ResolveTargetName(setterSceneNode.Target) as BaseFrameworkElement;
      return this.ViewModel.GetSceneNode(containerOwnerPath.Node) as BaseFrameworkElement;
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name)
    {
      if (string.IsNullOrEmpty(name) || this.TargetElement != null && this.TargetElement.Name == name)
        return (SceneNode) this;
      return (SceneNode) null;
    }

    SceneNode ITriggerContainer.ResolveTargetName(string name, SceneNode relativeSource)
    {
      return ((ITriggerContainer) this).ResolveTargetName(name);
    }

    string IStoryboardContainer.GetTargetName(SceneNode node)
    {
      if (this.TargetElement == node || node == this)
        return (string) null;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.TimelineContainerDoesNotContainElement, new object[0]));
    }

    void IStoryboardContainer.AddResourceStoryboard(string name, StoryboardTimelineSceneNode storyboard)
    {
      DictionaryEntryNode dictionaryEntryNode = DictionaryEntryNode.Factory.Instantiate((object) name, (SceneNode) storyboard);
      if ((IPropertyId) storyboard.Platform.Metadata.ResolveProperty(StyleNode.ResourcesProperty) == null)
        return;
      Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureResourceDictionaryNode((SceneNode) this).Add(dictionaryEntryNode);
    }

    bool IStoryboardContainer.RemoveResourceStoryboard(StoryboardTimelineSceneNode storyboard)
    {
      return this.Resources.RemoveEntryWithValue((SceneNode) storyboard);
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      base.ModifyValue(propertyReference, valueToSet, modification, index);
      if (this.ViewModel.ActiveEditContext != null && propertyReference != null)
      {
        if (this.ViewModel.ActiveEditContext.OutOfPlaceOverriddenProperties == null)
          this.ViewModel.ActiveEditContext.OutOfPlaceOverriddenProperties = (ICollection<IProperty>) new HashSet<IProperty>();
        if (!this.ViewModel.ActiveEditContext.OutOfPlaceOverriddenProperties.Contains((IProperty) propertyReference.FirstStep))
          this.ViewModel.ActiveEditContext.OutOfPlaceOverriddenProperties.Add((IProperty) propertyReference.FirstStep);
      }
      if (this.ViewModel.AnimationEditor.IsCurrentlyAnimated((SceneNode) this, propertyReference, propertyReference.Count))
        return;
      IViewObject viewTargetElement = this.ViewTargetElement;
      if (viewTargetElement == null)
        return;
      DocumentNode correspondingDocumentNode = this.ViewModel.DefaultView.GetCorrespondingDocumentNode(viewTargetElement, false);
      if (correspondingDocumentNode == null || correspondingDocumentNode == this.DocumentNode || (!correspondingDocumentNode.IsInDocument || correspondingDocumentNode.DocumentRoot != this.ViewModel.DocumentRoot))
        return;
      SceneNode sceneNode = this.ViewModel.GetSceneNode(correspondingDocumentNode);
      ReferenceStep referenceStep = propertyReference[0];
      if (sceneNode == null || referenceStep.MemberType == MemberType.DesignTimeProperty || (!referenceStep.TargetType.IsAssignableFrom(sceneNode.TargetType) || sceneNode.IsSet((IPropertyId) referenceStep) != PropertyState.Set))
        return;
      sceneNode.ClearLocalValue((IPropertyId) referenceStep);
    }

    protected override object PartialGetValue(PropertyReference propertyReference, int initialStepIndex, int finalStepIndex)
    {
      if (this.IsStyleProperty(propertyReference))
        return base.PartialGetValue(propertyReference, initialStepIndex, finalStepIndex);
      return propertyReference.PartialGetValue(this.ViewTargetElement.PlatformSpecificObject, initialStepIndex, finalStepIndex);
    }

    private bool IsStyleProperty(PropertyReference propertyReference)
    {
      return PlatformTypes.Style.IsAssignableFrom(propertyReference[0].DeclaringTypeId);
    }

    public override DocumentPropertyNodeReferenceBase CreateLocalDocumentPropertyNodeReference(DocumentCompositeNode parent, IPropertyId propertyKey)
    {
      if (parent == this.DocumentNode && !this.ProjectContext.ResolveType(PlatformTypes.Style).RuntimeType.IsAssignableFrom(propertyKey.TargetType))
      {
        DependencyPropertyReferenceStep referenceStep = propertyKey as DependencyPropertyReferenceStep;
        if (referenceStep != null)
          return (DocumentPropertyNodeReferenceBase) new StyleNode.StyleDocumentPropertyNodeReference(this, referenceStep);
      }
      return base.CreateLocalDocumentPropertyNodeReference(parent, propertyKey);
    }

    private DocumentNode GetSetterValue(DependencyPropertyReferenceStep referenceStep)
    {
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.Setters)
      {
        SetterSceneNode setterSceneNode = sceneNode as SetterSceneNode;
        if (setterSceneNode != null && setterSceneNode.Property == referenceStep)
          return ((DocumentCompositeNode) setterSceneNode.DocumentNode).Properties[SetterSceneNode.ValueProperty];
      }
      return (DocumentNode) null;
    }

    private void SetSetterValue(DependencyPropertyReferenceStep referenceStep, DocumentNode setterValue)
    {
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) this.Setters)
      {
        SetterSceneNode setterSceneNode = sceneNode as SetterSceneNode;
        if (setterSceneNode != null && setterSceneNode.Property == referenceStep)
        {
          if (setterValue != null)
          {
            ((DocumentCompositeNode) setterSceneNode.DocumentNode).Properties[SetterSceneNode.ValueProperty] = setterValue;
            return;
          }
          this.Setters.Remove((SceneNode) setterSceneNode);
          return;
        }
      }
      if (setterValue == null)
        return;
      SetterSceneNode setterSceneNode1 = (SetterSceneNode) this.ViewModel.CreateSceneNode(PlatformTypes.Setter);
      setterSceneNode1.Property = referenceStep;
      ((DocumentCompositeNode) setterSceneNode1.DocumentNode).Properties[SetterSceneNode.ValueProperty] = setterValue;
      this.Setters.Add((SceneNode) setterSceneNode1);
    }

    public class ConcreteStyleNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new StyleNode();
      }
    }

    private class StyleDocumentPropertyNodeReference : DocumentPropertyNodeReferenceBase
    {
      private StyleNode styleNode;
      private DependencyPropertyReferenceStep referenceStep;

      public override DocumentNode Node
      {
        get
        {
          return this.styleNode.GetSetterValue(this.referenceStep);
        }
        set
        {
          this.styleNode.SetSetterValue(this.referenceStep, value);
        }
      }

      public override DocumentCompositeNode Parent
      {
        get
        {
          return (DocumentCompositeNode) this.styleNode.DocumentNode;
        }
      }

      public override IPropertyId PropertyKey
      {
        get
        {
          return (IPropertyId) this.referenceStep;
        }
      }

      public StyleDocumentPropertyNodeReference(StyleNode styleNode, DependencyPropertyReferenceStep referenceStep)
      {
        this.styleNode = styleNode;
        this.referenceStep = referenceStep;
      }
    }
  }
}

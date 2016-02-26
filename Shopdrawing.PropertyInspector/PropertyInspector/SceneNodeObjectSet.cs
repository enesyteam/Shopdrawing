// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class SceneNodeObjectSet : ObjectSetBase
  {
    private static Dictionary<ReferenceStep, ReferenceStep> FilteredProperties = new Dictionary<ReferenceStep, ReferenceStep>();
    private IPropertyInspector transactionContext;
    private static ITypeId FilteredPropertiesType;

    public abstract bool ShouldWalkParentsForGetValue { get; }

    public abstract bool ShouldAllowAnimation { get; }

    public abstract bool IsViewRepresentationValid { get; }

    public abstract SceneDocument Document { get; }

    public abstract IDocumentContext DocumentContext { get; }

    public abstract SceneViewModel ViewModel { get; }

    public abstract bool IsValidForUpdate { get; }

    public abstract bool PropertyUpdatesLocked { get; set; }

    public abstract SceneNode[] Objects { get; }

    public override int Count
    {
      get
      {
        return this.Objects.Length;
      }
    }

    public virtual bool IsMultiSelection
    {
      get
      {
        return this.Count > 1;
      }
    }

    public virtual bool IsEmpty
    {
      get
      {
        return this.Count == 0;
      }
    }

    public virtual bool IsSelectionInvalid
    {
      get
      {
        TextRangeElement textRangeElement = this.RepresentativeSceneNode as TextRangeElement;
        if (this.RepresentativeSceneNode == null || this.RepresentativeSceneNode.IsAttached)
          return false;
        if (textRangeElement != null)
          return !textRangeElement.IsTextRangeAttached;
        return true;
      }
    }

    public abstract ObservableCollection<LocalResourceModel> LocalResources { get; }

    public abstract ObservableCollection<SystemResourceModel> SystemResources { get; }

    public abstract ObservableCollection<TemplateBindablePropertyModel> TemplateBindableProperties { get; }

    public abstract bool IsTextRange { get; }

    public abstract bool CanSetBindingExpression { get; }

    public abstract bool CanSetDynamicExpression { get; }

    public abstract bool IsTargetedByAnimation { get; }

    public abstract DocumentNode RepresentativeNode { get; }

    public abstract SceneNode RepresentativeSceneNode { get; }

    public abstract IType ObjectTypeId { get; }

    public IPropertyInspector TransactionContext
    {
      get
      {
        return this.transactionContext;
      }
    }

    protected SceneNodeObjectSet(IPropertyInspector transactionContext)
    {
      if (transactionContext == null)
        throw new ArgumentNullException("transactionContext");
      this.transactionContext = transactionContext;
    }

    public abstract void SetValueToLocalResource(SceneNodeProperty propertyKey, LocalResourceModel localResource);

    public abstract void SetValueToSystemResource(SceneNodeProperty propertyKey, SystemResourceModel systemResource);

    public abstract void SetValueToTemplateBinding(SceneNodeProperty propertyKey, ReferenceStep referenceStep);

    public void InvalidateLocalResourcesCache()
    {
      this.InvalidateLocalResourcesCache(true);
    }

    public virtual void InvalidateLocalResourcesCache(bool firePropertyChanged)
    {
    }

    public override sealed PropertyReferenceProperty CreateProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      return (PropertyReferenceProperty) this.CreateSceneNodeProperty(propertyReference, attributes);
    }

    public PropertyReferenceProperty CreateKeyframeProperty(PropertyReference propertyReference, AttributeCollection attributes, Type proxyType)
    {
      KeyframeSceneNodeProperty sceneNodeProperty = new KeyframeSceneNodeProperty(this, propertyReference, attributes, proxyType);
      sceneNodeProperty.Recache();
      return (PropertyReferenceProperty) sceneNodeProperty;
    }

    public PropertyReferenceProperty CreateProperty(PropertyReference propertyReference, AttributeCollection attributes, Type proxyType)
    {
      SceneNodeProperty sceneNodeProperty = new SceneNodeProperty(this, propertyReference, attributes, (PropertyValue) null, proxyType);
      sceneNodeProperty.Recache();
      return (PropertyReferenceProperty) sceneNodeProperty;
    }

    public virtual SceneNodeProperty CreateSceneNodeProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      SceneNodeProperty sceneNodeProperty = new SceneNodeProperty(this, propertyReference, attributes);
      sceneNodeProperty.Recache();
      return sceneNodeProperty;
    }

    internal virtual SceneEditTransaction PrepareTreeForModifyValue(PropertyReference propertyReference, object valueToSet, Modification modification, out bool treeModified)
    {
      treeModified = false;
      return (SceneEditTransaction) null;
    }

    public abstract SceneNode CreateAndSetBindingOrData(SceneNodeProperty property);

    public abstract DocumentNode GetLocalValueAsDocumentNode(SceneNodeProperty property, GetLocalValueFlags flags, out bool isMixed);

    public override object GetValue(IPropertyId propertyKey)
    {
      return this.GetValue(new PropertyReference((ReferenceStep) this.ViewModel.ProjectContext.ResolveProperty(propertyKey)), PropertyReference.GetValueFlags.Computed);
    }

    internal static PropertyReference FilterProperty(SceneNode node, PropertyReference propertyReference, bool strictTypeCheck)
    {
      PropertyReference propertyReference1 = SceneNodeObjectSet.FilterProperty(node, propertyReference);
      if (strictTypeCheck && propertyReference1 != null && (propertyReference1.LastStep.MemberType & MemberType.LocalProperty) != MemberType.None)
      {
        IType type = node.ProjectContext.GetType(propertyReference.LastStep.TargetType);
        if (type == null || propertyReference1.LastStep.DeclaringType.PlatformMetadata != type.PlatformMetadata || !propertyReference1.LastStep.DeclaringType.IsAssignableFrom((ITypeId) type))
          propertyReference1 = (PropertyReference) null;
      }
      return propertyReference1;
    }

    internal static PropertyReference FilterProperty(SceneNode node, PropertyReference propertyReference)
    {
      ReferenceStep referenceStep = propertyReference[0];
      ReferenceStep filteredStep = SceneNodeObjectSet.FilterProperty(node, referenceStep);
      return SceneNodeObjectSet.UpdatePropertyReference(propertyReference, referenceStep, filteredStep);
    }

    internal static PropertyReference FilterProperty(ITypeResolver typeResolver, IType type, PropertyReference propertyReference)
    {
      ReferenceStep referenceStep = propertyReference[0];
      ReferenceStep filteredStep = SceneNodeObjectSet.FilterProperty(typeResolver, type, referenceStep);
      return SceneNodeObjectSet.UpdatePropertyReference(propertyReference, referenceStep, filteredStep);
    }

    private static PropertyReference UpdatePropertyReference(PropertyReference propertyReference, ReferenceStep referenceStep, ReferenceStep filteredStep)
    {
      if (filteredStep == null)
        return (PropertyReference) null;
      if (filteredStep != referenceStep && PlatformTypeHelper.GetPropertyType((IProperty) filteredStep).IsAssignableFrom(PlatformTypeHelper.GetPropertyType((IProperty) referenceStep)))
      {
        ReferenceStep[] steps = new ReferenceStep[propertyReference.Count];
        steps[0] = filteredStep;
        for (int index = 1; index < propertyReference.Count; ++index)
          steps[index] = propertyReference[index];
        propertyReference = PropertyReference.CreateNewPropertyReferenceFromStepsWithoutCopy(steps);
      }
      return propertyReference;
    }

    internal static ReferenceStep FilterProperty(SceneNode node, ReferenceStep referenceStep)
    {
      if (node is RichTextBoxRangeElement)
        return referenceStep;
      RichTextBoxElement richTextBoxElement = node as RichTextBoxElement;
      if (richTextBoxElement != null)
      {
        foreach (IPropertyId propertyId in !node.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) ? RichTextBoxParagraphsRangeElement.SilverlightTextParagraphProperties : RichTextBoxParagraphsRangeElement.WpfTextParagraphProperties)
        {
          if (referenceStep.Equals((object) richTextBoxElement.ProjectContext.ResolveProperty(propertyId)))
            return referenceStep;
        }
      }
      StyleNode styleNode = node as StyleNode;
      IType typeId = styleNode == null || styleNode.Platform.Metadata.IsNullType((ITypeId) styleNode.StyleTargetTypeId) ? node.Type : styleNode.StyleTargetTypeId;
      Type runtimeType = typeId.NearestResolvedType.RuntimeType;
      if (referenceStep.TargetType.IsAssignableFrom(runtimeType) && (PlatformTypeHelper.GetDeclaringType((IMember) referenceStep) == runtimeType || referenceStep.MemberType == MemberType.DesignTimeProperty))
        return referenceStep;
      ReferenceStep referenceStep1;
      if (typeId == SceneNodeObjectSet.FilteredPropertiesType)
      {
        if (SceneNodeObjectSet.FilteredProperties.TryGetValue(referenceStep, out referenceStep1))
          return referenceStep1;
      }
      else
      {
        SceneNodeObjectSet.FilteredPropertiesType = (ITypeId) typeId;
        SceneNodeObjectSet.FilteredProperties.Clear();
      }
      referenceStep1 = SceneNodeObjectSet.FilterPropertyInternal((ITypeResolver) node.ProjectContext, typeId, referenceStep);
      SceneNodeObjectSet.FilteredProperties.Add(referenceStep, referenceStep1);
      return referenceStep1;
    }

    internal static ReferenceStep FilterProperty(ITypeResolver typeResolver, IType type, ReferenceStep referenceStep)
    {
      return SceneNodeObjectSet.FilterPropertyInternal(typeResolver, type, referenceStep);
    }

    private static ReferenceStep FilterPropertyInternal(ITypeResolver typeResolver, IType typeId, ReferenceStep referenceStep)
    {
      ReferenceStep referenceStep1 = (ReferenceStep) null;
      MemberType memberTypes = MemberType.LocalProperty | MemberType.AttachedProperty | referenceStep.MemberType;
      DependencyPropertyReferenceStep referenceStep2 = referenceStep as DependencyPropertyReferenceStep;
      if (referenceStep2 != null)
        referenceStep1 = (ReferenceStep) DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, typeId.NearestResolvedType.RuntimeType, referenceStep2, memberTypes);
      else if ((referenceStep.MemberType & MemberType.AttachedProperty) != MemberType.None && typeResolver.GetType(referenceStep.TargetType).IsAssignableFrom((ITypeId) typeId))
        referenceStep1 = referenceStep;
      if (referenceStep1 == null)
        referenceStep1 = (ReferenceStep) typeId.GetMember(memberTypes, referenceStep.Name, TypeHelper.GetAllowableMemberAccess(typeResolver, typeId));
      return referenceStep1;
    }

    public virtual SceneEditTransaction CreateEditTransaction(string description)
    {
      return this.ViewModel.CreateEditTransaction(description);
    }

    public virtual SceneEditTransaction CreateEditTransaction(string description, bool hidden)
    {
      return this.ViewModel.CreateEditTransaction(description, hidden);
    }
  }
}

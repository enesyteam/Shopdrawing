// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingPropertyHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal static class BindingPropertyHelper
  {
    public static BindingPropertyMatchInfo GetDefaultBindingPropertyInfo(SceneNode sceneNode, IType dataType)
    {
      BindingPropertyMatchInfo bestProperty = new BindingPropertyMatchInfo((IProperty) null);
      IProperty property = sceneNode.ProjectContext.ResolveProperty(BaseFrameworkElement.DataContextProperty);
      for (IType type = sceneNode.Type; type != null; type = type.BaseType)
      {
        ReferenceStep propertyInternal = BindingPropertyHelper.GetDefaultBindingPropertyInternal(type.NearestResolvedType, sceneNode.ProjectContext);
        if (propertyInternal != null && BindingPropertyHelper.IsPropertyBindable(sceneNode, new PropertyReference(propertyInternal), BindingPropertyHelper.BindingType.Target))
        {
          BindingPropertyMatchInfo propertyMatchInfo = new BindingPropertyMatchInfo((IProperty) propertyInternal);
          propertyMatchInfo.Compatibility = propertyInternal != property ? BindingPropertyHelper.GetPropertyCompatibility(propertyMatchInfo, dataType, (ITypeResolver) sceneNode.ProjectContext) : BindingPropertyCompatibility.DataContext;
          bestProperty = BindingPropertyHelper.GetBetterProperty(bestProperty, propertyMatchInfo);
        }
      }
      return bestProperty;
    }

    private static ReferenceStep GetDefaultBindingPropertyInternal(IType type, IProjectContext projectContext)
    {
      if (type.RuntimeType == (Type) null)
        return (ReferenceStep) null;
      string result;
      if (!PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(type.RuntimeType, PlatformTypes.DefaultBindingPropertyAttribute, "Name", out result) || string.IsNullOrEmpty(result))
        return (ReferenceStep) null;
      MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess((ITypeResolver) projectContext, type);
      return type.GetMember(MemberType.Property, result, allowableMemberAccess) as ReferenceStep;
    }

    public static IList<ReferenceStep> GetBindableSourceProperties(SceneNode sceneNode)
    {
      return BindingPropertyHelper.GetBindablePropertiesInternal(sceneNode, int.MaxValue, BindingPropertyHelper.BindingType.Source);
    }

    public static IList<ReferenceStep> GetBindableTargetProperties(SceneNode sceneNode)
    {
      return BindingPropertyHelper.GetBindablePropertiesInternal(sceneNode, int.MaxValue, BindingPropertyHelper.BindingType.Target);
    }

    public static bool HasBindableProperties(SceneNode sceneNode)
    {
      return BindingPropertyHelper.GetBindablePropertiesInternal(sceneNode, 1, BindingPropertyHelper.BindingType.Target).Count > 0;
    }

    private static IList<ReferenceStep> GetBindablePropertiesInternal(SceneNode sceneNode, int maxCount, BindingPropertyHelper.BindingType bindingType)
    {
      List<ReferenceStep> list = new List<ReferenceStep>();
      if (!BindingPropertyHelper.IsBindableSceneNode(sceneNode))
        return (IList<ReferenceStep>) list;
      bool isDeclaringTypeBindable = BindingPropertyHelper.IsBindableType(sceneNode.Type.NearestResolvedType, bindingType);
      foreach (TargetedReferenceStep targetedReferenceStep in (IEnumerable<TargetedReferenceStep>) PropertyMerger.GetMergedProperties((IEnumerable<SceneNode>) new SceneNode[1]
      {
        sceneNode
      }))
      {
        if ((!BindingPropertyHelper.IsTargetMode(bindingType) || BindingPropertyHelper.IsPropertyBindableAsTarget(sceneNode, targetedReferenceStep.ReferenceStep, isDeclaringTypeBindable)) && (!BindingPropertyHelper.IsSourceMode(bindingType) || BindingPropertyHelper.IsPropertyBindableAsSource(sceneNode, targetedReferenceStep.ReferenceStep)))
        {
          list.Add(targetedReferenceStep.ReferenceStep);
          if (list.Count >= maxCount)
            break;
        }
      }
      return (IList<ReferenceStep>) list;
    }

    public static bool IsPropertyBindable(SceneNode sceneNode, ReferenceStep referenceStep)
    {
      return BindingPropertyHelper.IsPropertyBindable(sceneNode, new PropertyReference(referenceStep));
    }

    public static bool IsPropertyBindable(SceneNode sceneNode, PropertyReference propertyReference)
    {
      return BindingPropertyHelper.IsPropertyBindable(sceneNode, propertyReference, BindingPropertyHelper.BindingType.Target);
    }

    public static bool IsPropertyValidBindingSource(SceneNode sceneNode, ReferenceStep referenceStep)
    {
      return BindingPropertyHelper.IsPropertyBindable(sceneNode, new PropertyReference(referenceStep), BindingPropertyHelper.BindingType.Source);
    }

    private static bool IsPropertyBindable(SceneNode sceneNode, PropertyReference propertyReference, BindingPropertyHelper.BindingType type)
    {
      if (!BindingPropertyHelper.IsBindableSceneNode(sceneNode))
        return false;
      ReferenceStep lastStep = propertyReference.LastStep;
      if (BindingPropertyHelper.IsTargetMode(type))
      {
        bool isDeclaringTypeBindable = propertyReference.Count <= 1 ? BindingPropertyHelper.IsBindableType(sceneNode.Type.NearestResolvedType, BindingPropertyHelper.BindingType.Target) : BindingPropertyHelper.IsBindableType(lastStep.DeclaringType, BindingPropertyHelper.BindingType.Target);
        if (!BindingPropertyHelper.IsPropertyBindableAsTarget(sceneNode, lastStep, isDeclaringTypeBindable))
          return false;
      }
      return !BindingPropertyHelper.IsSourceMode(type) || BindingPropertyHelper.IsPropertyBindableAsSource(sceneNode, lastStep);
    }

    private static bool IsBindableSceneNode(SceneNode sceneNode)
    {
      if (!JoltHelper.DatabindingSupported(sceneNode.ProjectContext))
        return false;
      Type type = sceneNode.GetType();
      return !typeof (KeyFrameSceneNode).IsAssignableFrom(type) && !typeof (TimelineSceneNode).IsAssignableFrom(type) && (!(sceneNode is StyleNode) || sceneNode.Platform.Metadata.IsCapabilitySet(PlatformCapability.SupportBindingsInStyleSetters));
    }

    internal static bool IsBindableType(IType type)
    {
      return BindingPropertyHelper.IsBindableType(type, BindingPropertyHelper.BindingType.Target);
    }

    private static bool IsBindableType(IType type, BindingPropertyHelper.BindingType bindingType)
    {
      return !(type.RuntimeType == (Type) null) && (!BindingPropertyHelper.IsTargetMode(bindingType) || (!type.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsDependencyObjectDatabinding) ? PlatformTypes.FrameworkElement : PlatformTypes.DependencyObject).IsAssignableFrom((ITypeId) type));
    }

    private static bool IsSourceMode(BindingPropertyHelper.BindingType bindingType)
    {
      return (bindingType & BindingPropertyHelper.BindingType.Source) != (BindingPropertyHelper.BindingType) 0;
    }

    private static bool IsTargetMode(BindingPropertyHelper.BindingType bindingType)
    {
      return (bindingType & BindingPropertyHelper.BindingType.Target) != (BindingPropertyHelper.BindingType) 0;
    }

    private static bool IsPropertyBindableAsSource(SceneNode sceneNode, ReferenceStep referenceStep)
    {
      if ((referenceStep.ReadAccess & (MemberAccessType) 14) == MemberAccessType.None)
        return false;
      SceneNode[] selection = new SceneNode[1]
      {
        sceneNode
      };
      TargetedReferenceStep targetedReferenceStep = new TargetedReferenceStep(referenceStep, sceneNode.Type);
      return PropertyInspectorModel.IsPropertyBrowsable(selection, targetedReferenceStep, true, true) && PropertyInspectorModel.IsAttachedPropertyBrowsable(selection, sceneNode.Type, targetedReferenceStep, (ITypeResolver) sceneNode.ProjectContext);
    }

    private static bool IsPropertyBindableAsTarget(SceneNode sceneNode, ReferenceStep referenceStep, bool isDeclaringTypeBindable)
    {
      if (referenceStep.Equals((object) referenceStep.DeclaringType.Metadata.NameProperty) || (referenceStep.WriteAccess & (MemberAccessType) 14) == MemberAccessType.None)
        return false;
      SceneNode[] selection = new SceneNode[1]
      {
        sceneNode
      };
      TargetedReferenceStep targetedReferenceStep = new TargetedReferenceStep(referenceStep, sceneNode.Type);
      if ((!PropertyInspectorModel.IsPropertyBrowsable(selection, targetedReferenceStep) || !PropertyInspectorModel.IsAttachedPropertyBrowsable(selection, sceneNode.Type, targetedReferenceStep, (ITypeResolver) sceneNode.ProjectContext)) && !referenceStep.Equals((object) ContentControlElement.ContentProperty))
        return false;
      if (referenceStep.PropertyType.IsBinding)
        return true;
      object[] customAttributes = referenceStep.GetCustomAttributes(typeof (BindableAttribute), false);
      if (customAttributes != null && customAttributes.Length > 0)
        return ((BindableAttribute) customAttributes[0]).Bindable;
      return isDeclaringTypeBindable && referenceStep is DependencyPropertyReferenceStep;
    }

    public static BindingPropertyCompatibility GetPropertyCompatibility(IProperty property, IType dataType, ITypeResolver typeResolver)
    {
      return BindingPropertyHelper.GetPropertyCompatibility(new BindingPropertyMatchInfo(property), dataType, typeResolver);
    }

    public static BindingModeInfo GetDefaultBindingMode(DocumentNode targetNode, IPropertyId targetProperty, DataSchemaNodePath schemaPath)
    {
      BindingMode mode = (BindingMode) targetNode.TypeResolver.GetCapabilityValue(PlatformCapability.DefaultBindingMode);
      BindingDirection bindingDirection = (BindingDirection) targetNode.TypeResolver.GetCapabilityValue(PlatformCapability.DefaultBindingDirection);
      DependencyPropertyReferenceStep propertyReferenceStep = targetNode.TypeResolver.ResolveProperty(targetProperty) as DependencyPropertyReferenceStep;
      bool flag = false;
      if (schemaPath != null)
      {
        if (schemaPath.Node.IsReadOnly)
          flag = true;
        else if (schemaPath.Node == schemaPath.Schema.Root)
          flag = !DataContextHelper.IsDataContextProperty(targetNode, (IPropertyId) propertyReferenceStep);
      }
      if (flag)
      {
        if (mode == BindingMode.Default && bindingDirection == BindingDirection.TwoWay && (propertyReferenceStep != null && propertyReferenceStep.BindsTwoWayByDefault(targetNode.Type.RuntimeType)))
          return new BindingModeInfo(BindingMode.OneWay, false);
        if (schemaPath.IsCollection)
          return new BindingModeInfo(mode, true);
        return new BindingModeInfo(BindingMode.OneWay, false);
      }
      if (bindingDirection == BindingDirection.TwoWay)
        return new BindingModeInfo(mode, true);
      IProperty property = targetNode.TypeResolver.ResolveProperty(targetProperty);
      if (property != null)
      {
        BindableAttribute bindableAttribute = property.Attributes[typeof (BindableAttribute)] as BindableAttribute;
        if (bindableAttribute != null && bindableAttribute.Direction == BindingDirection.TwoWay)
          return new BindingModeInfo(BindingMode.TwoWay, false);
      }
      return new BindingModeInfo(mode, true);
    }

    private static BindingPropertyCompatibility GetPropertyCompatibility(BindingPropertyMatchInfo propertyInfo, IType dataType, ITypeResolver typeResolver)
    {
      IType sourceType = DesignDataHelper.GetSourceType(dataType, typeResolver);
      if (PlatformTypes.String.IsAssignableFrom((ITypeId) sourceType) && !PlatformTypes.String.IsAssignableFrom((ITypeId) propertyInfo.PropertyType) && PlatformTypes.IEnumerable.IsAssignableFrom((ITypeId) propertyInfo.PropertyType))
        return BindingPropertyCompatibility.None;
      if (propertyInfo.NullableNormalizedPropertyType.IsAssignableFrom((ITypeId) sourceType) || propertyInfo.PropertyType.IsAssignableFrom((ITypeId) sourceType) || propertyInfo.PropertyType.IsBinding)
        return BindingPropertyCompatibility.Assignable;
      TypeConverter typeConverter = propertyInfo.Property.TypeConverter ?? propertyInfo.Property.PropertyType.TypeConverter;
      if (typeConverter != null && typeConverter.CanConvertFrom(sourceType.RuntimeType) && (!PlatformTypes.String.IsAssignableFrom((ITypeId) sourceType) || !PlatformTypes.IConvertible.IsAssignableFrom((ITypeId) propertyInfo.NullableNormalizedPropertyType)))
        return BindingPropertyCompatibility.Convertible;
      return PlatformTypes.String.IsAssignableFrom((ITypeId) propertyInfo.PropertyType) ? BindingPropertyCompatibility.StringSpecial : BindingPropertyCompatibility.None;
    }

    private static BindingPropertyMatchInfo GetBetterProperty(BindingPropertyMatchInfo bestProperty, BindingPropertyMatchInfo newProperty)
    {
      if (newProperty.Compatibility == BindingPropertyCompatibility.None || bestProperty.Property != null && (bestProperty.Compatibility == BindingPropertyCompatibility.DataContext && PlatformTypes.Object.Equals((object) newProperty.PropertyType) || newProperty.Compatibility - bestProperty.Compatibility >= 0 && (newProperty.NullableNormalizedPropertyType == bestProperty.NullableNormalizedPropertyType || newProperty.NullableNormalizedPropertyType.IsAssignableFrom((ITypeId) bestProperty.NullableNormalizedPropertyType) || !bestProperty.NullableNormalizedPropertyType.IsAssignableFrom((ITypeId) newProperty.NullableNormalizedPropertyType))))
        return bestProperty;
      return newProperty;
    }

    public static string GetElementNameFromBoundProperty(SceneNodeProperty property)
    {
      if (property == null || property.SceneNodeObjectSet == null || property.SceneNodeObjectSet.ViewModel == null)
        return (string) null;
      bool isMixed;
      BindingSceneNode bindingSceneNode = property.SceneNodeObjectSet.ViewModel.GetSceneNode(property.GetLocalValueAsDocumentNode(true, out isMixed)) as BindingSceneNode;
      if (bindingSceneNode == null)
        return (string) null;
      if (!string.IsNullOrEmpty(bindingSceneNode.ElementName))
      {
        if (!string.IsNullOrEmpty(bindingSceneNode.Path))
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
          {
            (object) bindingSceneNode.ElementName,
            (object) bindingSceneNode.Path
          });
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
        {
          (object) bindingSceneNode.ElementName
        });
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
      {
        (object) bindingSceneNode.Path
      });
    }

    [Flags]
    private enum BindingType
    {
      Target = 1,
      Source = 2,
    }
  }
}

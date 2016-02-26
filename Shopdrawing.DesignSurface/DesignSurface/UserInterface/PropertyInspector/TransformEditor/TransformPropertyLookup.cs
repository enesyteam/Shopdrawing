// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.TransformPropertyLookup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public class TransformPropertyLookup
  {
    private static readonly Dictionary<string, string> TranformGroupPathToCompositePathMap = new Dictionary<string, string>()
    {
      {
        "RotateTransform/Angle",
        "CompositeTransform.Rotation"
      },
      {
        "RotationAngle",
        "CompositeTransform.Rotation"
      },
      {
        "ScaleTransform/ScaleX",
        "CompositeTransform.ScaleX"
      },
      {
        "ScaleTransform/ScaleY",
        "CompositeTransform.ScaleY"
      },
      {
        "SkewTransform/AngleX",
        "CompositeTransform.SkewX"
      },
      {
        "SkewTransform/AngleY",
        "CompositeTransform.SkewY"
      },
      {
        "TranslateTransform/X",
        "CompositeTransform.TranslateX"
      },
      {
        "TranslationX",
        "CompositeTransform.TranslateX"
      },
      {
        "TranslateTransform/Y",
        "CompositeTransform.TranslateY"
      },
      {
        "TranslationY",
        "CompositeTransform.TranslateY"
      }
    };
    private SceneNodeProperty transformProperty;
    private LocalValueObjectSet localTransformObjectSet;
    private SceneNodeObjectSet sceneNodeObjectSet;
    private Dictionary<string, PropertyReferenceProperty> localTransformPropertyCache;
    private Dictionary<string, PropertyReferenceProperty> sceneNodePropertyCache;
    private ObjectSetBase activeObjectSet;
    private Dictionary<string, PropertyReferenceProperty> activePropertyCache;
    private DesignerContext designerContext;
    private TransformType transformType;
    private bool relative;
    private bool platformSupportsCompositeTransform;

    public TransformType TransformType
    {
      get
      {
        return this.transformType;
      }
    }

    public bool Relative
    {
      get
      {
        return this.relative;
      }
    }

    public bool IsCompositeSupported
    {
      get
      {
        return this.platformSupportsCompositeTransform;
      }
    }

    public SceneNodeProperty TransformProperty
    {
      get
      {
        return this.transformProperty;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    internal IEnumerable<PropertyReferenceProperty> ActiveProperties
    {
      get
      {
        return (IEnumerable<PropertyReferenceProperty>) this.activePropertyCache.Values;
      }
    }

    public ObjectSetBase ActiveObjectSet
    {
      get
      {
        return this.activeObjectSet;
      }
    }

    public TransformPropertyLookup(SceneNodeProperty transformProperty, TransformType transformType)
    {
      this.transformProperty = transformProperty;
      this.transformType = transformType;
      this.sceneNodeObjectSet = transformProperty.SceneNodeObjectSet;
      this.platformSupportsCompositeTransform = this.sceneNodeObjectSet.ProjectContext != null && this.sceneNodeObjectSet.ProjectContext.PlatformMetadata != null && this.sceneNodeObjectSet.ProjectContext.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform);
      this.designerContext = this.sceneNodeObjectSet.DesignerContext;
      this.localTransformObjectSet = new LocalValueObjectSet(this.CreateDefaultRelativeTransform(), this.sceneNodeObjectSet.DesignerContext, this.sceneNodeObjectSet.ProjectContext);
      this.localTransformPropertyCache = new Dictionary<string, PropertyReferenceProperty>();
      this.sceneNodePropertyCache = new Dictionary<string, PropertyReferenceProperty>();
    }

    public object CreateDefaultRelativeTransform()
    {
      switch (this.transformType)
      {
        case TransformType.Transform2D:
          if (this.platformSupportsCompositeTransform)
            return Activator.CreateInstance(this.sceneNodeObjectSet.ProjectContext.ResolveType(PlatformTypes.CompositeTransform).RuntimeType);
          return (object) new CanonicalTransform();
        case TransformType.Transform3D:
          return (object) new CanonicalTransform3D();
        case TransformType.PlaneProjection:
          return Activator.CreateInstance(this.sceneNodeObjectSet.ProjectContext.ResolveType(PlatformTypes.PlaneProjection).RuntimeType);
        default:
          return (object) null;
      }
    }

    public void RecacheProperties()
    {
      foreach (KeyValuePair<string, PropertyReferenceProperty> keyValuePair in this.activePropertyCache)
      {
        keyValuePair.Value.Recache();
        TransformSceneNodeProperty sceneNodeProperty = keyValuePair.Value as TransformSceneNodeProperty;
        if (sceneNodeProperty != null)
          sceneNodeProperty.UpdateValue();
      }
    }

    public void Unload()
    {
      foreach (PropertyBase propertyBase in this.localTransformPropertyCache.Values)
        propertyBase.OnRemoveFromCategory();
      this.localTransformPropertyCache.Clear();
      foreach (PropertyBase propertyBase in this.sceneNodePropertyCache.Values)
        propertyBase.OnRemoveFromCategory();
      this.sceneNodePropertyCache.Clear();
    }

    public void UpdateEditMode(bool relative)
    {
      this.relative = relative;
      if (relative)
      {
        this.activeObjectSet = (ObjectSetBase) this.localTransformObjectSet;
        this.localTransformObjectSet.LocalValue = this.CreateDefaultRelativeTransform();
        this.activePropertyCache = this.localTransformPropertyCache;
      }
      else
      {
        this.activeObjectSet = (ObjectSetBase) this.sceneNodeObjectSet;
        this.activePropertyCache = this.sceneNodePropertyCache;
      }
    }

    public PropertyReferenceProperty CreateProperty(string propertyPath)
    {
      return this.CreateProperty(propertyPath, (AttributeCollection) null);
    }

    public PropertyReferenceProperty CreateProperty(string propertyPath, AttributeCollection attributes)
    {
      if (this.activeObjectSet == null || this.activePropertyCache == null)
        return (PropertyReferenceProperty) null;
      if (this.platformSupportsCompositeTransform && this.TransformType.Equals((object) TransformType.Transform2D))
      {
        string str = string.Empty;
        propertyPath = !TransformPropertyLookup.TranformGroupPathToCompositePathMap.TryGetValue(propertyPath, out str) ? "CompositeTransform." + propertyPath : str;
      }
      else if (this.Relative)
      {
        switch (this.TransformType)
        {
          case TransformType.Transform2D:
            propertyPath = "CanonicalTransform." + propertyPath;
            break;
          case TransformType.Transform3D:
            propertyPath = "CanonicalTransform3D." + propertyPath;
            break;
        }
      }
      else
      {
        switch (this.TransformType)
        {
          case TransformType.Transform2D:
            if (propertyPath.Equals("CenterX") || propertyPath.Equals("CenterY"))
              return (PropertyReferenceProperty) this.BuildCenterTransformPropertyReference(propertyPath, attributes);
            propertyPath = this.BuildExpandedPropertyPath2D(propertyPath);
            break;
          case TransformType.Transform3D:
            propertyPath = this.BuildExpandedPropertyPath3D(propertyPath);
            break;
        }
      }
      PropertyReferenceProperty referenceProperty = (PropertyReferenceProperty) null;
      if (this.activePropertyCache.TryGetValue(propertyPath, out referenceProperty))
        return referenceProperty;
      PropertyReference propertyReference = (PropertyReference) null;
      try
      {
        propertyReference = new PropertyReference((ITypeResolver) this.sceneNodeObjectSet.ProjectContext, propertyPath);
      }
      catch (Exception ex)
      {
      }
      if (propertyReference != null)
      {
        if (!this.relative)
        {
          referenceProperty = (PropertyReferenceProperty) new TransformSceneNodeProperty((SceneNodeObjectSet) this.activeObjectSet, this.transformProperty.Reference.Append(propertyReference), attributes);
          referenceProperty.Recache();
        }
        else
          referenceProperty = this.activeObjectSet.CreateProperty(propertyReference, attributes);
      }
      if (referenceProperty != null)
        this.activePropertyCache[propertyPath] = referenceProperty;
      return referenceProperty;
    }

    private SceneNodeProperty BuildTransformSceneNodeProperty(string propertyPath, AttributeCollection attributes)
    {
      return (SceneNodeProperty) new TransformSceneNodeProperty((SceneNodeObjectSet) this.activeObjectSet, this.transformProperty.Reference.Append(new PropertyReference((ITypeResolver) this.sceneNodeObjectSet.ProjectContext, propertyPath)), attributes);
    }

    private SceneNodeProperty BuildCenterTransformPropertyReference(string centerProperty, AttributeCollection attributes)
    {
      CoalescingSceneNodeProperty sceneNodeProperty = new CoalescingSceneNodeProperty((SceneNodeObjectSet) this.activeObjectSet, attributes, (IList<SceneNodeProperty>) new List<SceneNodeProperty>()
      {
        this.BuildTransformSceneNodeProperty(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TransformGroup.Children[{0}]/ScaleTransform.{1}", new object[2]
        {
          (object) CanonicalTransformOrder.ScaleIndex,
          (object) centerProperty
        }), attributes),
        this.BuildTransformSceneNodeProperty(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TransformGroup.Children[{0}]/SkewTransform.{1}", new object[2]
        {
          (object) CanonicalTransformOrder.SkewIndex,
          (object) centerProperty
        }), attributes),
        this.BuildTransformSceneNodeProperty(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TransformGroup.Children[{0}]/RotateTransform.{1}", new object[2]
        {
          (object) CanonicalTransformOrder.RotateIndex,
          (object) centerProperty
        }), attributes)
      });
      sceneNodeProperty.Recache();
      return (SceneNodeProperty) sceneNodeProperty;
    }

    private string BuildExpandedPropertyPath2D(string propertyPath)
    {
      Match match = Regex.Match(propertyPath, "(?<transformType>\\w*)Transform/(?<property>\\w*)", RegexOptions.Compiled);
      if (match.Success)
      {
        string str1 = match.Groups["transformType"].Value;
        string str2 = match.Groups["property"].Value;
        int num = -1;
        switch (str1)
        {
          case "Scale":
            num = CanonicalTransformOrder.ScaleIndex;
            break;
          case "Skew":
            num = CanonicalTransformOrder.SkewIndex;
            break;
          case "Rotate":
            num = CanonicalTransformOrder.RotateIndex;
            break;
          case "Translate":
            num = CanonicalTransformOrder.TranslateIndex;
            break;
        }
        propertyPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TransformGroup.Children[{0}]/{1}Transform.{2}", (object) num, (object) str1, (object) str2);
      }
      return propertyPath;
    }

    public static PropertyReference ConvertTransformPropertyToComposite(PropertyReference propertyReference)
    {
      if (propertyReference == null)
        return (PropertyReference) null;
      int num = -1;
      for (int index = 0; index < propertyReference.Count; ++index)
      {
        if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) propertyReference[index].PropertyType))
        {
          num = index;
          break;
        }
      }
      if (num == -1)
        return (PropertyReference) null;
      ReferenceStep compositeProperty = TransformPropertyLookup.GetCorrespondingCompositeProperty(propertyReference);
      if (compositeProperty == null)
        return (PropertyReference) null;
      ReferenceStep[] steps = new ReferenceStep[num + 2];
      for (int index = 0; index <= num; ++index)
        steps[index] = propertyReference.ReferenceSteps[index];
      steps[num + 1] = compositeProperty;
      return PropertyReference.CreateNewPropertyReferenceFromStepsWithoutCopy(steps);
    }

    private static ReferenceStep GetCorrespondingCompositeProperty(PropertyReference propertyReference)
    {
      if (propertyReference == null)
        return (ReferenceStep) null;
      IPropertyId propertyId = (IPropertyId) propertyReference.LastStep;
      IMetadataResolver metadataResolver = (IMetadataResolver) propertyReference.PlatformMetadata;
      if (propertyId.Equals((object) RotateTransformNode.AngleProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.RotationProperty);
      if (propertyId.Equals((object) ScaleTransformNode.ScaleXProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.ScaleXProperty);
      if (propertyId.Equals((object) ScaleTransformNode.ScaleYProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.ScaleYProperty);
      if (propertyId.Equals((object) SkewTransformNode.AngleXProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.SkewXProperty);
      if (propertyId.Equals((object) SkewTransformNode.AngleYProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.SkewYProperty);
      if (propertyId.Equals((object) TranslateTransformNode.XProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.TranslateXProperty);
      if (propertyId.Equals((object) TranslateTransformNode.YProperty))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.TranslateYProperty);
      if (RotateTransformNode.CenterXProperty.Equals((object) propertyId) || SkewTransformNode.CenterXProperty.Equals((object) propertyId) || ScaleTransformNode.CenterXProperty.Equals((object) propertyId))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.CenterXProperty);
      if (RotateTransformNode.CenterYProperty.Equals((object) propertyId) || SkewTransformNode.CenterYProperty.Equals((object) propertyId) || ScaleTransformNode.CenterYProperty.Equals((object) propertyId))
        return (ReferenceStep) metadataResolver.ResolveProperty(CompositeTransformNode.CenterYProperty);
      return (ReferenceStep) null;
    }

    private string BuildExpandedPropertyPath3D(string propertyPath)
    {
      Match match = Regex.Match(propertyPath, "(?<transformType>\\w*)Transform/(?<property>\\w*)", RegexOptions.Compiled);
      if (match.Success)
      {
        string str1 = match.Groups["transformType"].Value;
        string str2 = match.Groups["property"].Value;
        int num = -1;
        switch (str1)
        {
          case "Scale":
            num = 1;
            break;
          case "Translate":
            num = 4;
            break;
        }
        propertyPath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transform3DGroup.Children[{0}]/{1}Transform3D.{2}", (object) num, (object) str1, (object) str2);
      }
      else if (propertyPath.Equals("RotationAngles"))
        propertyPath = "CanonicalTransform3D." + propertyPath;
      else if (propertyPath.Contains("Center"))
        propertyPath = "CanonicalTransform3D." + propertyPath;
      return propertyPath;
    }

    public PropertyReferenceProperty CreateNormalProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      PropertyReferenceProperty property = this.activeObjectSet.CreateProperty(propertyReference, attributes);
      this.sceneNodePropertyCache[propertyReference.Path] = property;
      return property;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.KeyFrameSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class KeyFrameSceneNode : SceneNode, IComparable
  {
    public static readonly KeyFrameSceneNode.ConcreteKeyFrameSceneNodeFactory Factory = new KeyFrameSceneNode.ConcreteKeyFrameSceneNodeFactory();

    public double Time
    {
      get
      {
        DocumentPrimitiveNode documentPrimitiveNode = this.DocumentCompositeNode.Properties[this.TimeProperty] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
        {
          KeyTime keyTime = documentPrimitiveNode.GetValue<KeyTime>();
          if (keyTime.Type == KeyTimeType.TimeSpan)
            return keyTime.TimeSpan.TotalSeconds;
        }
        return 0.0;
      }
      set
      {
        this.SetValueAsWpf(this.TimeProperty, (object) KeyTime.FromTimeSpan(TimeSpan.FromSeconds(value)));
      }
    }

    public object Value
    {
      get
      {
        DocumentNode documentNode = this.DocumentCompositeNode.Properties[this.ValueProperty];
        if (documentNode == null)
          return (object) null;
        return this.ViewModel.CreateInstance(new DocumentNodePath(documentNode, documentNode));
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        Type type;
        if (value != null && (type = value.GetType()).IsEnum && this.ProjectContext.Platform.Metadata.IsSupported((ITypeResolver) this.ProjectContext, PlatformTypes.StaticExtension))
        {
          IMember memberId = (IMember) this.ProjectContext.GetType(type).GetMember(MemberType.LocalProperty | MemberType.Field, Enum.GetName(type, value), MemberAccessTypes.All);
          if (memberId != null)
            documentNode = (DocumentNode) DocumentNodeUtilities.NewStaticNode(this.DocumentContext, memberId);
        }
        else
          documentNode = this.CreateNode(value);
        this.DocumentCompositeNode.Properties[this.ValueProperty] = documentNode;
        VisualStateSceneNode controllingState = this.ControllingState;
        if (controllingState == null || !this.KeyFrameAnimation.ChangeCanAffectTransition((SceneNode) this, (PropertyReference) null) && !this.KeyFrameAnimation.NoAutoTransitionsProvided)
          return;
        controllingState.UpdateTransitionsForStateValueChange(this.KeyFrameAnimation.TargetElementAndProperty, value);
      }
    }

    public DocumentNode ValueNode
    {
      get
      {
        DocumentNodePath valueAsDocumentNode = this.GetLocalValueAsDocumentNode(this.ValueProperty);
        if (valueAsDocumentNode == null)
          return (DocumentNode) null;
        return valueAsDocumentNode.Node;
      }
      set
      {
        this.SetLocalValue(this.ValueProperty, value);
      }
    }

    public Point EaseInControlPoint
    {
      get
      {
        if (this.InterpolationType != KeyFrameInterpolationType.Spline)
          return new Point(1.0, 1.0);
        if (this.KeySpline == null)
          return new Point(1.0, 1.0);
        return this.KeySpline.ControlPoint2;
      }
      set
      {
        if (this.InterpolationType != KeyFrameInterpolationType.Spline)
          throw new InvalidOperationException("Cannot set ease in control point on a non-spline keyframe. Convert first");
        KeySpline keySpline = this.KeySpline != null ? this.KeySpline : new KeySpline();
        keySpline.ControlPoint2 = value;
        this.KeySpline = keySpline;
      }
    }

    public bool CanSetEaseInControlPoint
    {
      get
      {
        return this.InterpolationType == KeyFrameInterpolationType.Spline;
      }
    }

    public Point EaseOutControlPoint
    {
      get
      {
        KeyFrameSceneNode nextKeyFrame = this.KeyFrameAnimation.GetNextKeyFrame(this);
        if (nextKeyFrame == null || nextKeyFrame.InterpolationType != KeyFrameInterpolationType.Spline)
          return new Point();
        if (nextKeyFrame.KeySpline == null)
          return new Point();
        return nextKeyFrame.KeySpline.ControlPoint1;
      }
      set
      {
        KeyFrameSceneNode nextKeyFrame = this.KeyFrameAnimation.GetNextKeyFrame(this);
        if (nextKeyFrame == null || nextKeyFrame.InterpolationType != KeyFrameInterpolationType.Spline)
          throw new InvalidOperationException("Cannot set ease out control point when next keyframe is not a spline point. Convert first");
        KeySpline keySpline = nextKeyFrame.KeySpline != null ? nextKeyFrame.KeySpline : new KeySpline();
        keySpline.ControlPoint1 = value;
        nextKeyFrame.KeySpline = keySpline;
      }
    }

    public bool CanSetEaseOutControlPoint
    {
      get
      {
        if (this.KeyFrameAnimation == null)
          return false;
        KeyFrameSceneNode nextKeyFrame = this.KeyFrameAnimation.GetNextKeyFrame(this);
        if (nextKeyFrame != null)
          return nextKeyFrame.InterpolationType == KeyFrameInterpolationType.Spline;
        return false;
      }
    }

    public KeySpline KeySpline
    {
      get
      {
        if (this.InterpolationType != KeyFrameInterpolationType.Spline)
          throw new InvalidOperationException(ExceptionStringTable.KeySplinePropertyIsOnlyValidForSplineKeyFrames);
        return this.GetLocalValueAsWpf(this.KeySplineProperty) as KeySpline;
      }
      set
      {
        if (this.InterpolationType != KeyFrameInterpolationType.Spline)
          throw new InvalidOperationException(ExceptionStringTable.KeySplinePropertyIsOnlyValidForSplineKeyFrames);
        if (value == null || value.ControlPoint1.X == value.ControlPoint1.Y && value.ControlPoint2.X == value.ControlPoint2.Y)
          this.ClearLocalValue(this.KeySplineProperty);
        else
          this.SetLocalValueAsWpf(this.KeySplineProperty, (object) value);
      }
    }

    public IEasingFunctionDefinition EasingFunction
    {
      get
      {
        if (this.InterpolationType == KeyFrameInterpolationType.Easing)
          return this.Platform.ViewObjectFactory.Instantiate(this.GetLocalValue(this.EasingFunctionProperty)) as IEasingFunctionDefinition;
        return (IEasingFunctionDefinition) null;
      }
    }

    public KeyFrameInterpolationType InterpolationType
    {
      get
      {
        string name = this.TargetType.Name;
        if (name.StartsWith("Discrete"))
          return KeyFrameInterpolationType.Discrete;
        if (name.StartsWith("Spline"))
          return KeyFrameInterpolationType.Spline;
        if (name.StartsWith("Linear"))
          return KeyFrameInterpolationType.Linear;
        if (name.StartsWith("Easing"))
          return KeyFrameInterpolationType.Easing;
        throw new NotImplementedException(ExceptionStringTable.UnknownKeyFrameInterpolationTypeEncountered);
      }
    }

    public KeyFrameAnimationSceneNode KeyFrameAnimation
    {
      get
      {
        return this.Parent as KeyFrameAnimationSceneNode;
      }
    }

    public VisualStateSceneNode ControllingState
    {
      get
      {
        if (this.KeyFrameAnimation == null)
          return (VisualStateSceneNode) null;
        return this.KeyFrameAnimation.ControllingState;
      }
    }

    public StoryboardTimelineSceneNode ControllingStoryboard
    {
      get
      {
        KeyFrameAnimationSceneNode keyFrameAnimation = this.KeyFrameAnimation;
        if (keyFrameAnimation != null)
          return keyFrameAnimation.ControllingStoryboard;
        return (StoryboardTimelineSceneNode) null;
      }
    }

    public PropertyReference TargetProperty
    {
      get
      {
        if (this.KeyFrameAnimation != null)
          return this.KeyFrameAnimation.TargetProperty;
        return (PropertyReference) null;
      }
    }

    public SceneNode TargetElement
    {
      get
      {
        if (this.KeyFrameAnimation != null)
          return this.KeyFrameAnimation.TargetElement;
        return (SceneNode) null;
      }
    }

    public IPropertyId KeySplineProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "KeySpline", MemberAccessTypes.Public);
      }
    }

    public IPropertyId EasingFunctionProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "EasingFunction", MemberAccessTypes.Public);
      }
    }

    private DocumentCompositeNode DocumentCompositeNode
    {
      get
      {
        return (DocumentCompositeNode) this.DocumentNode;
      }
    }

    private IPropertyId TimeProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "KeyTime", MemberAccessTypes.Public);
      }
    }

    public IPropertyId ValueProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
      }
    }

    public static KeyFrameSceneNode EnsureCanSetEaseInControlPoint(KeyFrameSceneNode node)
    {
      if (node.CanSetEaseInControlPoint)
        return node;
      if (node.KeyFrameAnimation == null)
        return (KeyFrameSceneNode) null;
      return node.KeyFrameAnimation.ReplaceKeyFrame(node, KeyFrameInterpolationType.Spline, (KeySpline) null);
    }

    public static bool SetEaseOutControlPoint(KeyFrameSceneNode node, Point newEaseOut)
    {
      bool flag = false;
      if (node.EaseOutControlPoint == newEaseOut)
        return false;
      if (!node.CanSetEaseOutControlPoint)
      {
        KeyFrameSceneNode keyFrame = node.KeyFrameAnimation != null ? node.KeyFrameAnimation.GetNextKeyFrame(node) : (KeyFrameSceneNode) null;
        if (keyFrame != null)
          flag = node.KeyFrameAnimation.ReplaceKeyFrame(keyFrame, KeyFrameInterpolationType.Spline, (KeySpline) null) != null;
      }
      if (node.CanSetEaseOutControlPoint)
        node.EaseOutControlPoint = newEaseOut;
      return flag;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      this.EnsureKeySplineIfNeeded(propertyReference);
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private void EnsureKeySplineIfNeeded(PropertyReference propertyReference)
    {
      if (propertyReference.Count <= 1 || !PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) propertyReference.FirstStep.PropertyType) || this.IsSet((IPropertyId) propertyReference.FirstStep) == PropertyState.Set)
        return;
      this.SetValueAsWpf((IPropertyId) propertyReference.FirstStep, (object) new KeySpline());
    }

    int IComparable.CompareTo(object toCompare)
    {
      KeyFrameSceneNode keyFrameSceneNode = toCompare as KeyFrameSceneNode;
      if (keyFrameSceneNode == null)
        return 1;
      if (this.Time == keyFrameSceneNode.Time)
        return this.KeyFrameAnimation.CompareTo((object) keyFrameSceneNode.KeyFrameAnimation);
      return this.Time.CompareTo(keyFrameSceneNode.Time);
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      if (this.ProjectContext.IsCapabilitySet(PlatformCapability.WorkaroundSL23187) && PlatformTypes.DiscreteObjectKeyFrame.IsAssignableFrom((ITypeId) this.Type))
        return JoltHelper.GetComputedValueFromLocalSceneNode((SceneNode) this, propertyReference);
      return base.GetComputedValueInternal(propertyReference);
    }

    public class ConcreteKeyFrameSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new KeyFrameSceneNode();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TimelineSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TimelineSceneNode : SceneNode
  {
    private static readonly double maxSeconds = Math.Floor(TimeSpan.MaxValue.TotalSeconds / 1000.0);
    public static readonly IPropertyId RepeatBehaviorProperty = (IPropertyId) PlatformTypes.Timeline.GetMember(MemberType.LocalProperty, "RepeatBehavior", MemberAccessTypes.Public);
    public static readonly IPropertyId AutoReverseProperty = (IPropertyId) PlatformTypes.Timeline.GetMember(MemberType.LocalProperty, "AutoReverse", MemberAccessTypes.Public);
    public static readonly IPropertyId BeginTimeProperty = (IPropertyId) PlatformTypes.Timeline.GetMember(MemberType.LocalProperty, "BeginTime", MemberAccessTypes.Public);
    public static readonly IPropertyId DurationProperty = (IPropertyId) PlatformTypes.Timeline.GetMember(MemberType.LocalProperty, "Duration", MemberAccessTypes.Public);
    public static readonly TimelineSceneNode.ConcreteTimelineSceneNodeFactory Factory = new TimelineSceneNode.ConcreteTimelineSceneNodeFactory();
    private SceneNode cachedTargetAvalonElement;
    private PropertyReference cachedTargetAvalonProperty;
    private uint cachedChangeStamp;
    private IStoryboardContainer referenceStoryboardContainer;

    public PropertyReference TargetProperty
    {
      get
      {
        return this.GetTargetAvalonProperty(this.TargetAvalonElement);
      }
      set
      {
        this.TargetAvalonProperty = value;
      }
    }

    public SceneNode TargetElement
    {
      get
      {
        return this.TargetAvalonElement;
      }
      set
      {
        this.TargetAvalonElement = value;
      }
    }

    public TimelineSceneNode.PropertyNodePair TargetElementAndProperty
    {
      get
      {
        SceneNode targetAvalonElement = this.TargetAvalonElement;
        return new TimelineSceneNode.PropertyNodePair(targetAvalonElement, this.GetTargetAvalonProperty(targetAvalonElement));
      }
    }

    public static double MaxSeconds
    {
      get
      {
        return TimelineSceneNode.maxSeconds;
      }
    }

    public System.Windows.Media.Animation.Timeline InstantiatedTimeline
    {
      get
      {
          return (System.Windows.Media.Animation.Timeline)this.CreateInstance();
      }
    }

    public double Begin
    {
      get
      {
        TimeSpan? nullable = this.GetLocalOrDefaultValue(TimelineSceneNode.BeginTimeProperty) as TimeSpan?;
        if (nullable.HasValue && nullable.HasValue)
          return nullable.Value.TotalSeconds;
        return 0.0;
      }
      set
      {
        if (value != 0.0)
        {
          TimeSpan? nullable = new TimeSpan?(TimeSpan.FromSeconds(value));
          this.SetValue(TimelineSceneNode.BeginTimeProperty, (object) nullable);
        }
        else
          this.ClearValue(TimelineSceneNode.BeginTimeProperty);
      }
    }

    public double ActiveDuration
    {
      get
      {
        return this.ClipEnd - this.ClipBegin;
      }
    }

    public virtual double RepeatCount
    {
      get
      {
        object localValueAsWpf = this.GetLocalValueAsWpf(TimelineSceneNode.RepeatBehaviorProperty);
        if (localValueAsWpf == null)
          return 1.0;
        RepeatBehavior repeatBehavior = (RepeatBehavior) localValueAsWpf;
        if (repeatBehavior == RepeatBehavior.Forever)
          return double.PositiveInfinity;
        double num = 1.0;
        if (repeatBehavior.HasCount && repeatBehavior.Count != 0.0)
          num = repeatBehavior.Count;
        return num;
      }
      set
      {
        RepeatBehavior repeatBehavior = !double.IsInfinity(value) ? new RepeatBehavior(Math.Max(0.0, value)) : RepeatBehavior.Forever;
        this.SetValueAsWpf(TimelineSceneNode.RepeatBehaviorProperty, (object) repeatBehavior);
      }
    }

    public double ClipBegin
    {
      get
      {
        return this.Begin;
      }
      set
      {
        this.Begin = value;
        double num = this.ClipEnd - this.ClipBegin;
        if (Tolerances.AreClose(num, this.CalculatedDuration))
          return;
        this.Duration = num;
      }
    }

    public double ClipEnd
    {
      get
      {
        return this.CalculatedDuration + this.ClipBegin;
      }
      set
      {
        double num = value - this.ClipBegin;
        if (Tolerances.AreClose(num, this.CalculatedDuration))
          return;
        this.Duration = num;
      }
    }

    public double CalculatedDuration
    {
      get
      {
        if (this.IsSet(TimelineSceneNode.DurationProperty) == PropertyState.Set)
          return this.Duration;
        return this.NaturalDuration;
      }
    }

    public virtual double NaturalDuration
    {
      get
      {
        return 0.0;
      }
    }

    public virtual StoryboardTimelineSceneNode ControllingStoryboard
    {
      get
      {
        TimelineSceneNode timelineSceneNode = this.Parent as TimelineSceneNode;
        return timelineSceneNode == null ? this as StoryboardTimelineSceneNode : timelineSceneNode.ControllingStoryboard;
      }
    }

    public VisualStateSceneNode ControllingState
    {
      get
      {
        StoryboardTimelineSceneNode controllingStoryboard = this.ControllingStoryboard;
        if (controllingStoryboard != null)
          return controllingStoryboard.Parent as VisualStateSceneNode;
        return (VisualStateSceneNode) null;
      }
    }

    public VisualStateTransitionSceneNode ControllingTransition
    {
      get
      {
        StoryboardTimelineSceneNode controllingStoryboard = this.ControllingStoryboard;
        if (controllingStoryboard != null)
          return controllingStoryboard.Parent as VisualStateTransitionSceneNode;
        return (VisualStateTransitionSceneNode) null;
      }
    }

    protected DocumentCompositeNode DocumentCompositeNode
    {
      get
      {
        return (DocumentCompositeNode) this.DocumentNode;
      }
    }

    public double Duration
    {
      get
      {
        System.Windows.Duration? nullable = this.GetLocalOrDefaultValueAsWpf(TimelineSceneNode.DurationProperty) as System.Windows.Duration?;
        if (nullable.HasValue && nullable.Value.HasTimeSpan)
          return Math.Max(0.0, nullable.Value.TimeSpan.TotalSeconds);
        return 0.0;
      }
      set
      {
        System.Windows.Duration duration = new System.Windows.Duration(TimeSpan.FromSeconds(value));
        this.SetValueAsWpf(TimelineSceneNode.DurationProperty, (object) duration);
      }
    }

    internal string TargetName
    {
      get
      {
        DocumentCompositeNode documentCompositeNode = this.DocumentNode as DocumentCompositeNode;
        DocumentNode node = documentCompositeNode != null ? documentCompositeNode.Properties[StoryboardTimelineSceneNode.TargetNameProperty] : (DocumentNode) null;
        string str = node != null ? DocumentPrimitiveNode.GetValueAsString(node) : (string) null;
        if (str == null && node != null)
        {
          DocumentNodePath valueAsDocumentNode = this.GetLocalValueAsDocumentNode(StoryboardTimelineSceneNode.TargetNameProperty, true);
          if (valueAsDocumentNode != null)
          {
            DocumentPrimitiveNode documentPrimitiveNode = valueAsDocumentNode.Node as DocumentPrimitiveNode;
            if (documentPrimitiveNode != null && PlatformTypes.String.IsAssignableFrom((ITypeId) documentPrimitiveNode.Type))
              str = DocumentPrimitiveNode.GetValueAsString((DocumentNode) documentPrimitiveNode);
          }
        }
        return str;
      }
      set
      {
        this.SetValue(StoryboardTimelineSceneNode.TargetNameProperty, (object) value);
      }
    }

    private PropertyReference TargetAvalonProperty
    {
      set
      {
        this.Invalidate();
        PropertyReference propertyReference = value;
        object valueToSet = (object) null;
        if (propertyReference != null)
        {
          object[] dependencyProperties;
          valueToSet = this.Platform.Metadata.MakePropertyPath(propertyReference.BuildPropertyPath(out dependencyProperties, this.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf)), dependencyProperties);
        }
        if (valueToSet != null)
          this.SetValue(StoryboardTimelineSceneNode.TargetPropertyProperty, valueToSet);
        else
          this.ClearValue(StoryboardTimelineSceneNode.TargetPropertyProperty);
      }
    }

    private SceneNode TargetAvalonElement
    {
      get
      {
        if (this.cachedTargetAvalonElement == null || (int) this.cachedChangeStamp != (int) this.ViewModel.ChangeStamp)
          this.RecacheTargetElement();
        return this.cachedTargetAvalonElement;
      }
      set
      {
        if (value != null)
          value.EnsureNamed();
        if (this.StoryboardContainer != null)
          this.TargetName = this.StoryboardContainer.GetTargetName(value);
        else if (this.referenceStoryboardContainer != null)
          this.TargetName = this.referenceStoryboardContainer.GetTargetName(value);
        else if (value.StoryboardContainer != null)
          this.TargetName = value.StoryboardContainer.GetTargetName(value);
        else
          this.TargetName = value.Name;
      }
    }

    private void Initialize(SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer)
    {
      this.referenceStoryboardContainer = referenceStoryboardContainer;
      this.TargetAvalonElement = targetElement;
      this.TargetAvalonProperty = targetProperty;
    }

    public IViewStoryboard CreateViewStoryboard()
    {
      return this.ViewModel.CreateViewStoryboard(this);
    }

    public void SetTimeRegion(double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitMoveScheduledProperties))
      {
        double scaleFactor = (finalRegionEnd - finalRegionBegin) / (originalRegionEnd - originalRegionBegin);
        TimeRegionChangeDetails details = TimeRegionChangeDetails.None;
        if (Tolerances.AreClose(originalRegionEnd - originalRegionBegin, 0.0))
          details |= TimeRegionChangeDetails.FromZero;
        if (Tolerances.AreClose(finalRegionEnd - finalRegionBegin, 0.0))
          details |= TimeRegionChangeDetails.ToZero;
        if (!Tolerances.AreClose(scaleFactor, 1.0))
          details |= TimeRegionChangeDetails.Scale;
        if (!Tolerances.AreClose(originalRegionBegin, finalRegionBegin))
          details |= TimeRegionChangeDetails.Translate;
        this.RecursiveSetTimeRegion(details, scaleFactor, originalRegionBegin, originalRegionEnd, finalRegionBegin, finalRegionEnd);
        editTransaction.Commit();
      }
    }

    private void RecursiveSetTimeRegion(TimeRegionChangeDetails details, double scaleFactor, double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
      StoryboardTimelineSceneNode timelineSceneNode = this as StoryboardTimelineSceneNode;
      if (timelineSceneNode != null)
      {
        for (int index = 0; index < timelineSceneNode.Children.Count; ++index)
          timelineSceneNode.Children[index].RecursiveSetTimeRegion(details, scaleFactor, originalRegionBegin, originalRegionEnd, finalRegionBegin, finalRegionEnd);
      }
      this.SetTimeRegionCore(details, scaleFactor, originalRegionBegin, originalRegionEnd, finalRegionBegin, finalRegionEnd);
    }

    protected virtual void SetTimeRegionCore(TimeRegionChangeDetails details, double scaleFactor, double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
      if ((details & TimeRegionChangeDetails.FromZero) != TimeRegionChangeDetails.None || (details & TimeRegionChangeDetails.ToZero) != TimeRegionChangeDetails.None)
      {
        this.Begin = finalRegionBegin;
        this.Duration = finalRegionEnd - finalRegionBegin;
      }
      else if ((details & TimeRegionChangeDetails.Scale) != TimeRegionChangeDetails.None)
      {
        System.Windows.Duration? nullable = this.GetLocalOrDefaultValueAsWpf(TimelineSceneNode.DurationProperty) as System.Windows.Duration?;
        this.Duration = (!nullable.HasValue || !nullable.Value.HasTimeSpan ? this.NaturalDuration : nullable.Value.TimeSpan.TotalSeconds) * scaleFactor;
        this.Begin = (this.Begin - originalRegionBegin) * scaleFactor + finalRegionBegin;
      }
      else
      {
        if ((details & TimeRegionChangeDetails.Translate) == TimeRegionChangeDetails.None)
          return;
        this.Begin += finalRegionBegin - originalRegionBegin;
      }
    }

    public static PropertyReference ResolvePropertyPath(ITypeResolver typeResolver, IViewObject propertyPath, SceneNode pathRoot)
    {
      if (propertyPath == null)
        return (PropertyReference) null;
      string path = (string) propertyPath.GetCurrentValue(typeResolver.ResolveProperty(typeResolver.PlatformMetadata.KnownProperties.PropertyPathPath));
      if (path == null)
        return (PropertyReference) null;
      IProperty propertyKey = typeResolver.ResolveProperty(typeResolver.PlatformMetadata.KnownProperties.PropertyPathPathParameters);
      Collection<object> parameters = propertyKey == null ? new Collection<object>() : (Collection<object>) propertyPath.GetCurrentValue(propertyKey);
      return TimelineSceneNode.ResolvePropertyPathParts(typeResolver, path, parameters, pathRoot);
    }

    private static PropertyReference ResolvePropertyPathParts(ITypeResolver typeResolver, string path, Collection<object> parameters, SceneNode pathRoot)
    {
      if (path == "(0)")
      {
        if (parameters == null || parameters.Count == 0)
          return (PropertyReference) null;
        DependencyProperty dependencyProperty = parameters[0] as DependencyProperty;
        if (dependencyProperty == null)
          return (PropertyReference) null;
        Type targetType = pathRoot != null ? pathRoot.TargetType : dependencyProperty.OwnerType;
        DependencyPropertyReferenceStep referenceStep = DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, dependencyProperty);
        if (referenceStep == null)
          return (PropertyReference) null;
        return new PropertyReference((ReferenceStep) referenceStep);
      }
      List<ReferenceStep> list = new List<ReferenceStep>();
      object obj = (object) null;
      if (typeResolver.IsCapabilitySet(PlatformCapability.SupportTypelessPropertyPath) && pathRoot != null && pathRoot.IsViewObjectValid)
        obj = pathRoot.ViewObject.PlatformSpecificObject;
      IType typeId = obj == null ? (pathRoot == null ? (parameters == null || parameters.Count <= 0 || !(parameters[0] is DependencyProperty) ? typeResolver.ResolveType(PlatformTypes.Object) : typeResolver.GetType(((DependencyProperty) parameters[0]).OwnerType)) : pathRoot.Type) : typeResolver.GetType(obj.GetType());
      for (int index1 = 0; index1 < path.Length; ++index1)
      {
        ReferenceStep referenceStep = (ReferenceStep) null;
        char ch = path[index1];
        switch (ch)
        {
          case '(':
            int num1 = path.IndexOf(')', index1 + 1);
            if (num1 < 0)
              return (PropertyReference) null;
            string s = path.Substring(index1 + 1, num1 - index1 - 1);
            int result1;
            if (int.TryParse(s, out result1))
            {
              if (parameters == null || result1 < 0 || result1 >= parameters.Count)
                return (PropertyReference) null;
              DependencyProperty dependencyProperty = parameters[result1] as DependencyProperty;
              if (dependencyProperty == null)
                return (PropertyReference) null;
              referenceStep = (ReferenceStep) DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, typeId.NearestResolvedType.RuntimeType, dependencyProperty);
              if (referenceStep == null)
              {
                PlatformTypes platformTypes = (PlatformTypes) typeResolver.PlatformMetadata;
                Type runtimeType = typeId.NearestResolvedType.RuntimeType;
                Type ownerType = dependencyProperty.OwnerType;
                referenceStep = (ReferenceStep) (platformTypes.GetProperty(typeResolver, ownerType, MemberType.LocalProperty, dependencyProperty.Name) as DependencyPropertyReferenceStep);
                if (referenceStep == null || !runtimeType.IsAssignableFrom(ownerType))
                  return (PropertyReference) null;
              }
            }
            else
            {
              int length = s.IndexOf('.');
              string typeName;
              if (length < 0)
              {
                if (!typeResolver.IsCapabilitySet(PlatformCapability.SupportTypelessPropertyPath))
                  return (PropertyReference) null;
                typeName = typeId.Name;
                length = -1;
              }
              else
                typeName = s.Substring(0, length);
              string str = s.Substring(length + 1);
              if (string.IsNullOrEmpty(typeName))
                return (PropertyReference) null;
              IType type = typeResolver.GetType((IXmlNamespace) typeResolver.GetCapabilityValue(PlatformCapability.DefaultXmlns), typeName);
              if (type == null)
              {
                IProperty designTimeProperty = typeResolver.PlatformMetadata.GetDesignTimeProperty(str, (IType) null);
                if (designTimeProperty != null && designTimeProperty.DeclaringType.Name == typeName)
                  referenceStep = (ReferenceStep) designTimeProperty;
              }
              if (referenceStep == null)
              {
                if (type == null)
                  type = typeId;
                referenceStep = type.GetMember(MemberType.Property, str, MemberAccessTypes.Public) as ReferenceStep ?? TimelineSceneNode.ResolveReferenceStepFromValue(typeResolver, pathRoot, list, str);
              }
              if (referenceStep == null)
                return (PropertyReference) null;
            }
            index1 = num1;
            break;
          case '[':
            int num2 = path.IndexOf(']', index1 + 1);
            if (num2 < 0)
              return (PropertyReference) null;
            int result2;
            if (!int.TryParse(path.Substring(index1 + 1, num2 - index1 - 1), out result2))
              return (PropertyReference) null;
            referenceStep = (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(typeResolver, typeId.RuntimeType, result2, false);
            index1 = num2;
            if (referenceStep == null)
              return (PropertyReference) null;
            break;
          default:
            int startIndex = index1;
            if ((int) ch == 46)
              ++startIndex;
            if (index1 >= path.Length - 1)
              return (PropertyReference) null;
            int index2 = index1 + 1;
            if ((int) path[index2] != 40)
            {
              for (; index2 < path.Length; ++index2)
              {
                switch (path[index2])
                {
                  case '.':
                  case '[':
                    goto label_52;
                  default:
                    goto default;
                }
              }
label_52:
              if (index1 >= index2 - 1)
                return (PropertyReference) null;
              string str = path.Substring(startIndex, index2 - startIndex);
              referenceStep = typeId.GetMember(MemberType.LocalProperty | MemberType.Field, str, TypeHelper.GetAllowableMemberAccess(typeResolver, typeId)) as ReferenceStep;
              if (referenceStep == null)
              {
                referenceStep = TimelineSceneNode.ResolveReferenceStepFromValue(typeResolver, pathRoot, list, str);
                if (referenceStep == null)
                  return (PropertyReference) null;
              }
              index1 = index2 - 1;
              break;
            }
            break;
        }
        if (referenceStep != null)
        {
          list.Add(referenceStep);
          typeId = referenceStep.PropertyType;
        }
      }
      if (list == null || list.Count <= 0)
        return (PropertyReference) null;
      return new PropertyReference(list);
    }

    private static ReferenceStep ResolveReferenceStepFromValue(ITypeResolver typeResolver, SceneNode pathRoot, List<ReferenceStep> referenceSteps, string propertyName)
    {
      if (pathRoot == null)
        return (ReferenceStep) null;
      PropertyReference propertyReference = (PropertyReference) null;
      SceneNode sceneNode;
      if (referenceSteps.Count == 0)
      {
        sceneNode = pathRoot;
      }
      else
      {
        propertyReference = new PropertyReference(referenceSteps);
        sceneNode = pathRoot.GetLocalValueAsSceneNode(propertyReference);
      }
      if (sceneNode != null)
      {
        IType type = sceneNode.Type;
        IMemberId member = type.GetMember(MemberType.LocalProperty | MemberType.Field, propertyName, TypeHelper.GetAllowableMemberAccess(typeResolver, type));
        if (member != null)
          return member as ReferenceStep;
      }
      object target = pathRoot.IsViewObjectValid ? pathRoot.ViewObject.PlatformSpecificObject : (object) null;
      if (propertyReference != null && target != null)
        target = propertyReference.GetValue(target);
      if (target != null)
      {
        IType type = typeResolver.GetType(target.GetType());
        if (type != null)
          return type.GetMember(MemberType.LocalProperty | MemberType.Field, propertyName, TypeHelper.GetAllowableMemberAccess(typeResolver, type)) as ReferenceStep;
      }
      return (ReferenceStep) null;
    }

    public void Invalidate()
    {
      this.cachedChangeStamp = 0U;
      this.cachedTargetAvalonElement = (SceneNode) null;
      this.cachedTargetAvalonProperty = (PropertyReference) null;
    }

    private PropertyReference GetTargetAvalonProperty(SceneNode targetAvalonElement)
    {
      if (this.cachedTargetAvalonProperty == null || (int) this.cachedChangeStamp != (int) this.ViewModel.ChangeStamp)
        this.RecacheTargetProperty(targetAvalonElement);
      return this.cachedTargetAvalonProperty;
    }

    private void RecacheTargetElement()
    {
      this.Invalidate();
      IStoryboardContainer storyboardContainer = this.StoryboardContainer;
      if (storyboardContainer == null)
        return;
      this.cachedTargetAvalonElement = storyboardContainer.ResolveTargetName(this.TargetName);
      this.cachedChangeStamp = this.ViewModel.ChangeStamp;
    }

    private void RecacheTargetProperty(SceneNode targetElement)
    {
      if (this.cachedTargetAvalonElement != targetElement || (int) this.cachedChangeStamp != (int) this.ViewModel.ChangeStamp)
        this.RecacheTargetElement();
      object localOrDefaultValue = this.GetLocalOrDefaultValue(StoryboardTimelineSceneNode.TargetPropertyProperty);
      StyleNode styleNode = this.cachedTargetAvalonElement as StyleNode;
      SceneNode pathRoot = styleNode != null ? (SceneNode) styleNode.TargetElement : this.cachedTargetAvalonElement;
      this.cachedTargetAvalonProperty = TimelineSceneNode.ResolvePropertyPath(this.DocumentContext.TypeResolver, this.Platform.ViewObjectFactory.Instantiate(localOrDefaultValue), pathRoot);
    }

    public class ConcreteTimelineSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TimelineSceneNode();
      }

      public virtual TimelineSceneNode InstantiateWithTarget(SceneViewModel viewModel, SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer, ITypeId type)
      {
        TimelineSceneNode timelineSceneNode = (TimelineSceneNode) this.Instantiate(viewModel, type);
        timelineSceneNode.Initialize(targetElement, targetProperty, referenceStoryboardContainer);
        return timelineSceneNode;
      }
    }

    public struct PropertyNodePair
    {
      private SceneNode node;
      private PropertyReference propertyReference;

      public SceneNode SceneNode
      {
        get
        {
          return this.node;
        }
      }

      public PropertyReference PropertyReference
      {
        get
        {
          return this.propertyReference;
        }
      }

      public PropertyNodePair(SceneNode node, PropertyReference propertyReference)
      {
        this.node = node;
        this.propertyReference = propertyReference;
      }
    }
  }
}

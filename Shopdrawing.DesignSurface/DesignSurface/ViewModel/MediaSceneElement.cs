// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MediaSceneElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.View;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MediaSceneElement : BaseFrameworkElement
  {
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly IPropertyId SpeedRatioProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "SpeedRatio", MemberAccessTypes.Public);
    public static readonly IPropertyId PositionProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "Position", MemberAccessTypes.Public);
    public static readonly IPropertyId StretchProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "Stretch", MemberAccessTypes.Public);
    public static readonly IPropertyId ScrubbingEnabledProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "ScrubbingEnabled", MemberAccessTypes.Public);
    public static readonly IPropertyId LoadedBehaviorProperty = (IPropertyId) PlatformTypes.MediaElement.GetMember(MemberType.LocalProperty, "LoadedBehavior", MemberAccessTypes.Public);
    private static IPropertyId[] MediaProperties = new IPropertyId[3]
    {
      MediaSceneElement.SourceProperty,
      MediaSceneElement.SpeedRatioProperty,
      MediaSceneElement.PositionProperty
    };
    private static IPropertyId[] MappedProperties = new IPropertyId[3]
    {
      MediaTimelineSceneNode.SourceProperty,
      MediaTimelineSceneNode.SpeedRatioProperty,
      TimelineSceneNode.BeginTimeProperty
    };
    public static readonly MediaSceneElement.ConcreteMediaSceneElementFactory Factory = new MediaSceneElement.ConcreteMediaSceneElementFactory();

    public MediaTimelineSceneNode OwningTimeline
    {
      get
      {
        foreach (StoryboardTimelineSceneNode timelineSceneNode1 in this.ViewModel.AnimationEditor.EnumerateStoryboardsForContainer(this.StoryboardContainer))
        {
          foreach (TimelineSceneNode timelineSceneNode2 in (IEnumerable<TimelineSceneNode>) timelineSceneNode1.Children)
          {
            if (timelineSceneNode2.TargetElement == this)
            {
              MediaTimelineSceneNode timelineSceneNode3 = timelineSceneNode2 as MediaTimelineSceneNode;
              if (timelineSceneNode3 != null)
                return timelineSceneNode3;
            }
          }
        }
        return (MediaTimelineSceneNode) null;
      }
    }

    public string Source
    {
      get
      {
        Uri uriValue = DocumentNodeHelper.GetUriValue(((DocumentCompositeNode) this.DocumentNode).Properties[MediaSceneElement.SourceProperty]);
        if (!(uriValue != (Uri) null))
          return (string) null;
        return uriValue.OriginalString;
      }
      set
      {
        if (!(value != this.Source))
          return;
        if (string.IsNullOrEmpty(value))
        {
          this.ClearValue(MediaSceneElement.SourceProperty);
        }
        else
        {
          DocumentPrimitiveNode node = this.DocumentNode.Context.CreateNode(PlatformTypes.Uri, (IDocumentNodeValue) new DocumentNodeStringValue(value));
          this.SetValue(MediaSceneElement.SourceProperty, (object) node);
        }
      }
    }

    protected override object GetComputedValueInternal(PropertyReference propertyReference)
    {
      PropertyReference propertyReference1 = this.GetMediaTimelinePropertyReference(propertyReference);
      if (propertyReference1 != null)
      {
        MediaTimelineSceneNode owningTimeline = this.OwningTimeline;
        if (owningTimeline != null)
        {
          ViewState viewState = ViewState.ElementValid | ViewState.AncestorValid | ViewState.SubtreeValid;
          if (!owningTimeline.ViewModel.DefaultView.IsValid || (owningTimeline.ViewModel.DefaultView.GetViewState((SceneNode) owningTimeline) & viewState) != viewState || propertyReference1.LastStep == MediaTimelineSceneNode.SourceProperty)
            return owningTimeline.GetLocalOrDefaultValue(propertyReference1);
          IViewObject viewObject = owningTimeline.ViewModel.GetViewObject(owningTimeline.DocumentNodePath);
          propertyReference = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, (ITypeId) owningTimeline.TargetElement.Type);
          return propertyReference1.GetCurrentValue(viewObject.PlatformSpecificObject);
        }
      }
      return base.GetComputedValueInternal(propertyReference);
    }

    protected override DocumentNodePath GetLocalValueAsDocumentNode(IProperty[] propertyPath, bool forceEvaluateExpressions)
    {
      PropertyReference propertyReference = (PropertyReference) null;
      if (propertyPath.Length == 1)
      {
        ReferenceStep step = propertyPath[0] as ReferenceStep;
        if (step != null)
          propertyReference = this.GetMediaTimelinePropertyReference(step);
      }
      if (propertyReference != null)
      {
        MediaTimelineSceneNode owningTimeline = this.OwningTimeline;
        if (owningTimeline != null)
          return owningTimeline.GetLocalValueAsDocumentNode(propertyReference, forceEvaluateExpressions);
      }
      return base.GetLocalValueAsDocumentNode(propertyPath, forceEvaluateExpressions);
    }

    protected override object GetLocalValueInternal(PropertyReference propertyReference, PropertyContext context)
    {
      PropertyReference propertyReference1 = this.GetMediaTimelinePropertyReference(propertyReference);
      if (propertyReference1 != null)
      {
        MediaTimelineSceneNode owningTimeline = this.OwningTimeline;
        if (owningTimeline != null)
          return owningTimeline.GetLocalValue(propertyReference1, context);
      }
      return base.GetLocalValueInternal(propertyReference, context);
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      PropertyReference propertyReference1 = this.GetMediaTimelinePropertyReference(propertyReference);
      if (propertyReference1 != null)
      {
        MediaTimelineSceneNode owningTimeline = this.OwningTimeline;
        if (owningTimeline != null)
        {
          if (modification == SceneNode.Modification.ClearValue)
          {
            owningTimeline.ClearValue(propertyReference1);
            return;
          }
          if (modification != SceneNode.Modification.SetValue)
            return;
          owningTimeline.SetValue(propertyReference1, valueToSet);
          return;
        }
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    private PropertyReference GetMediaTimelinePropertyReference(PropertyReference propertyReference)
    {
      if (propertyReference.Count != 1)
        return (PropertyReference) null;
      return this.GetMediaTimelinePropertyReference(propertyReference.LastStep);
    }

    private PropertyReference GetMediaTimelinePropertyReference(ReferenceStep step)
    {
      int index = Array.IndexOf<IPropertyId>(MediaSceneElement.MediaProperties, (IPropertyId) step);
      if (index != -1)
      {
        ReferenceStep singleStep = (ReferenceStep) this.Platform.Metadata.ResolveProperty(MediaSceneElement.MappedProperties[index]);
        if (singleStep != null)
          return new PropertyReference(singleStep);
      }
      return (PropertyReference) null;
    }

    public class ConcreteMediaSceneElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MediaSceneElement();
      }
    }
  }
}

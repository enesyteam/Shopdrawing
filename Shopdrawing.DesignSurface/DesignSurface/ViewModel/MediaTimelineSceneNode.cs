// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MediaTimelineSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MediaTimelineSceneNode : TimelineSceneNode
  {
    public static readonly IPropertyId SourceProperty = (IPropertyId) PlatformTypes.MediaTimeline.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
    public static readonly IPropertyId SpeedRatioProperty = (IPropertyId) PlatformTypes.MediaTimeline.GetMember(MemberType.LocalProperty, "SpeedRatio", MemberAccessTypes.Public);
    public static readonly MediaTimelineSceneNode.ConcreteMediaTimelineSceneNodeFactory Factory = new MediaTimelineSceneNode.ConcreteMediaTimelineSceneNodeFactory();

    public override double NaturalDuration
    {
      get
      {
        return (double) this.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.DesignTimeNaturalDurationProperty);
      }
    }

    public Uri Source
    {
      get
      {
        return this.DocumentCompositeNode.GetUriValue(MediaTimelineSceneNode.SourceProperty);
      }
      set
      {
        DocumentNode documentNode = (DocumentNode) null;
        if (value != (Uri) null)
          documentNode = DocumentNodeUtilities.NewUriDocumentNode(this.ViewModel.Document.DocumentContext, value);
        this.DocumentCompositeNode.Properties[MediaTimelineSceneNode.SourceProperty] = documentNode;
      }
    }

    public Uri DesignTimeSource
    {
      get
      {
        UriNode uriNode = this.GetLocalValueAsSceneNode(MediaTimelineSceneNode.SourceProperty) as UriNode;
        if (uriNode != null)
          return uriNode.DesignTimeUri;
        return (Uri) null;
      }
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (propertyReference.Count == 1 && propertyReference.FirstStep.Equals((object) MediaTimelineSceneNode.SourceProperty))
      {
        bool flag = false;
        if (modification == SceneNode.Modification.SetValue)
        {
          Uri uri = valueToSet as Uri;
          if (uri != (Uri) null)
          {
            this.ClearLocalValue(TimelineSceneNode.DurationProperty);
            Uri mediaUri = this.ViewModel.XamlDocument.DocumentContext.MakeDesignTimeUri(uri);
            MediaOpener mediaOpener = new MediaOpener(uri.OriginalString, mediaUri, this.DesignerContext.MessageDisplayService);
            bool? nullable = mediaOpener.OpenMedia();
            if (nullable.HasValue && nullable.Value)
            {
              Duration naturalDuration = mediaOpener.Player.NaturalDuration;
              if (naturalDuration.HasTimeSpan)
                this.SetLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty, (object) naturalDuration.TimeSpan.TotalSeconds);
              else
                flag = true;
            }
            else
              flag = true;
          }
        }
        else if (modification == SceneNode.Modification.ClearValue)
        {
          this.ClearLocalValue(TimelineSceneNode.DurationProperty);
          flag = true;
        }
        if (flag)
          this.SetLocalValue(DesignTimeProperties.DesignTimeNaturalDurationProperty, (object) 0.0);
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public class ConcreteMediaTimelineSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MediaTimelineSceneNode();
      }
    }
  }
}

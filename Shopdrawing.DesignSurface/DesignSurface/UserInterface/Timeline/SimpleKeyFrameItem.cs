// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.SimpleKeyFrameItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class SimpleKeyFrameItem : KeyFrameItem, ISceneElementSubpart, IComparable
  {
    public KeyFramedTimelineItem KeyFramedTimelineItem
    {
      get
      {
        return (KeyFramedTimelineItem) this.TimelineItem;
      }
    }

    public SceneElement Element
    {
      get
      {
        return this.KeyFramedTimelineItem.Element;
      }
    }

    public SceneElement SceneElement
    {
      get
      {
        return this.KeyFramedTimelineItem.Element;
      }
    }

    public PropertyReference TargetProperty
    {
      get
      {
        PropertyReference propertyReference = (PropertyReference) null;
        PropertyTimelineItem propertyTimelineItem = this.KeyFramedTimelineItem as PropertyTimelineItem;
        if (propertyTimelineItem != null)
          propertyReference = propertyTimelineItem.TargetProperty;
        return propertyReference;
      }
    }

    public KeyFrameSceneNode KeyFrameSceneNode
    {
      get
      {
        return this.KeyFramedTimelineItem.KeyFrameAnimationSceneNode.GetKeyFrameAtTime(this.IsOldTimeSet ? this.OldTime : this.Time);
      }
    }

    public override IList<KeyFrameSceneNode> KeyFrameSceneNodesToSelect
    {
      get
      {
        return (IList<KeyFrameSceneNode>) new List<KeyFrameSceneNode>()
        {
          this.KeyFrameSceneNode
        };
      }
    }

    protected override SimpleKeyFrameItem KeyFrameItemToEdit
    {
      get
      {
        return this;
      }
    }

    public SimpleKeyFrameItem(double time, KeyFramedTimelineItem keyFramedTimelineItem)
      : base(time, (TimelineItem) keyFramedTimelineItem)
    {
    }
  }
}

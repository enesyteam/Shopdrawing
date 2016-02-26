// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelineHeader
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimelineHeader : INotifyPropertyChanged
  {
    private bool matchesFilter = true;
    private StoryboardTimelineSceneNode timeline;

    public StoryboardTimelineSceneNode Timeline
    {
      get
      {
        return this.timeline;
      }
    }

    public virtual string Name
    {
      get
      {
        return StringTable.TimelineNoStoryboardOpen;
      }
      set
      {
        throw new InvalidOperationException("Cannot rename the default TimelineHeader");
      }
    }

    public bool MatchesFilter
    {
      get
      {
        return this.matchesFilter;
      }
      set
      {
        if (this.matchesFilter == value)
          return;
        this.matchesFilter = value;
        this.OnPropertyChanged("MatchesFilter");
      }
    }

    public virtual bool CanRename
    {
      get
      {
        return false;
      }
    }

    public virtual bool CanDelete
    {
      get
      {
        return false;
      }
    }

    public virtual bool CanSelect
    {
      get
      {
        return false;
      }
    }

    public virtual bool CanRecord
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsRecording
    {
      get
      {
        return false;
      }
      set
      {
        throw new InvalidOperationException("Cannot set IsRecording on the default timeline header");
      }
    }

    public virtual bool IsSelected
    {
      get
      {
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TimelineHeader()
      : this((StoryboardTimelineSceneNode) null)
    {
    }

    public TimelineHeader(StoryboardTimelineSceneNode timeline)
    {
      this.timeline = timeline;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void UpdateIsRecording()
    {
    }

    public virtual void Update()
    {
    }
  }
}

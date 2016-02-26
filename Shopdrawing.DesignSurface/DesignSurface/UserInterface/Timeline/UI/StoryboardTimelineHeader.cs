// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.StoryboardTimelineHeader
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class StoryboardTimelineHeader : TimelineHeader
  {
    public bool IsValid
    {
      get
      {
        return this.Timeline.ViewObject != null;
      }
    }

    public string ActualType
    {
      get
      {
        return this.Timeline.TargetType.Name;
      }
    }

    public override string Name
    {
      get
      {
        if (this.Timeline.ViewModel.Document != null)
        {
          string name = this.Timeline.Name;
          if (string.IsNullOrEmpty(name))
            name = this.Timeline.DocumentNode.Name;
          if (!string.IsNullOrEmpty(name))
            return name;
        }
        return StringTable.TimelineStoryboardNameStandin;
      }
      set
      {
        this.Timeline.ViewModel.AnimationEditor.RenameStoryboardWithValidation(this.Timeline, value);
        this.OnPropertyChanged("Name");
      }
    }

    public override bool IsRecording
    {
      get
      {
        if (this.Timeline.ViewModel.AnimationEditor == null || this.Timeline.ViewModel.AnimationEditor.ActiveStoryboardTimeline != this.Timeline)
          return false;
        return this.Timeline.ViewModel.AnimationEditor.IsRecording;
      }
      set
      {
        if (this.Timeline.ViewModel.AnimationEditor.ActiveStoryboardTimeline != this.Timeline)
          return;
        this.Timeline.ViewModel.AnimationEditor.IsRecording = value;
      }
    }

    public override bool IsSelected
    {
      get
      {
        if (this.Timeline.DesignerContext != null)
        {
          SelectionManager selectionManager = this.Timeline.DesignerContext.SelectionManager;
          if (selectionManager != null && selectionManager.StoryboardSelectionSet != null && selectionManager.StoryboardSelectionSet.Count == 1)
            return selectionManager.StoryboardSelectionSet.PrimarySelection == this.Timeline;
        }
        return false;
      }
    }

    public override bool CanRename
    {
      get
      {
        return true;
      }
    }

    public override bool CanSelect
    {
      get
      {
        return true;
      }
    }

    public bool CanKeyframe
    {
      get
      {
        return true;
      }
    }

    public override bool CanDelete
    {
      get
      {
        return true;
      }
    }

    public override bool CanRecord
    {
      get
      {
        return true;
      }
    }

    public StoryboardTimelineHeader(StoryboardTimelineSceneNode timeline)
      : base(timeline)
    {
    }

    public override void UpdateIsRecording()
    {
      base.UpdateIsRecording();
      this.OnPropertyChanged("IsRecording");
    }

    public override void Update()
    {
      base.Update();
      this.UpdateIsRecording();
      this.OnPropertyChanged("Name");
      this.OnPropertyChanged("CanRename");
      this.OnPropertyChanged("IsSelected");
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}

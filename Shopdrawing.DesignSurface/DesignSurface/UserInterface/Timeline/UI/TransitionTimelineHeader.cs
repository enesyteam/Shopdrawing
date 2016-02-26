// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TransitionTimelineHeader
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TransitionTimelineHeader : TimelineHeader
  {
    private DesignerContext designerContext;

    public override bool CanSelect
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
        return false;
      }
    }

    public override bool CanRecord
    {
      get
      {
        return true;
      }
    }

    public override bool IsRecording
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.AnimationEditor != null && activeSceneViewModel.TransitionEditTarget != null)
          return activeSceneViewModel.AnimationEditor.IsRecording;
        return false;
      }
      set
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel == null || activeSceneViewModel.AnimationEditor == null || activeSceneViewModel.TransitionEditTarget == null)
          return;
        activeSceneViewModel.AnimationEditor.IsRecording = value;
      }
    }

    public override bool CanRename
    {
      get
      {
        return false;
      }
    }

    public string FromStateName
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.TransitionEditTarget != null)
          return activeSceneViewModel.TransitionEditTarget.FromStateName;
        return (string) null;
      }
    }

    public string ToStateName
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.TransitionEditTarget != null)
          return activeSceneViewModel.TransitionEditTarget.ToStateName;
        return (string) null;
      }
    }

    public override string Name
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    internal TransitionTimelineHeader(DesignerContext designerContext)
      : base((StoryboardTimelineSceneNode) null)
    {
      this.designerContext = designerContext;
    }

    public override void Update()
    {
      base.Update();
      this.OnPropertyChanged("FromStateName");
      this.OnPropertyChanged("ToStateName");
    }

    public override void UpdateIsRecording()
    {
      base.UpdateIsRecording();
      this.OnPropertyChanged("IsRecording");
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SelectNoneCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class SelectNoneCommand : SceneCommandBase
  {
    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        if (this.SceneViewModel.TextSelectionSet.IsActive)
          return this.SceneViewModel.TextSelectionSet.CanCopy;
        if (this.SceneViewModel.ElementSelectionSet.Count <= 0 && this.SceneViewModel.StoryboardSelectionSet.Count <= 0 && (this.SceneViewModel.KeyFrameSelectionSet.Count <= 0 && this.SceneViewModel.BehaviorSelectionSet.Count <= 0) && (this.SceneViewModel.ChildPropertySelectionSet.Count <= 0 && this.SceneViewModel.AnnotationSelectionSet.Count <= 0 && this.SceneViewModel.GridColumnSelectionSet.Count <= 0))
          return this.SceneViewModel.GridRowSelectionSet.Count > 0;
        return true;
      }
    }

    public SelectNoneCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      if (this.SceneViewModel.TextSelectionSet.IsActive && this.SceneViewModel.TextSelectionSet.TextEditProxy != null)
      {
        this.SceneViewModel.TextSelectionSet.TextEditProxy.SelectNone();
      }
      else
      {
        this.SceneViewModel.KeyFrameSelectionSet.Clear();
        this.SceneViewModel.StoryboardSelectionSet.Clear();
        this.SceneViewModel.ElementSelectionSet.Clear();
        this.SceneViewModel.BehaviorSelectionSet.Clear();
        this.SceneViewModel.ChildPropertySelectionSet.Clear();
        this.SceneViewModel.AnnotationSelectionSet.Clear();
        this.SceneViewModel.GridColumnSelectionSet.Clear();
        this.SceneViewModel.GridRowSelectionSet.Clear();
      }
    }
  }
}

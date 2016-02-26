// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyInspectorHelpCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal abstract class PropertyInspectorHelpCommand : ICommand
  {
    private DesignerContext designerContext;

    protected DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    protected IHelpService HelpService
    {
      get
      {
        return this.designerContext.HelpService;
      }
    }

    public event EventHandler CanExecuteChanged;

    public PropertyInspectorHelpCommand(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.Hook();
    }

    public void Hook()
    {
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.OnSelectionManagerLateActiveSceneUpdatePhase);
    }

    public void Unhook()
    {
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.OnSelectionManagerLateActiveSceneUpdatePhase);
    }

    public bool CanExecute(object parameter)
    {
      return this.HelpService.IsHelpAvailable((object) TopicHelpContext.CreateContextFromParameter(parameter, this.DesignerContext));
    }

    public abstract void Execute(object parameter);

    private void OnSelectionManagerLateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.CanExecuteChanged == null || !args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ActiveTrigger | SceneViewModel.ViewStateBits.ActiveTimeline | SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.TextSelection | SceneViewModel.ViewStateBits.KeyFrameSelection | SceneViewModel.ViewStateBits.AnimationSelection | SceneViewModel.ViewStateBits.StoryboardSelection | SceneViewModel.ViewStateBits.CurrentValues | SceneViewModel.ViewStateBits.ChildPropertySelection | SceneViewModel.ViewStateBits.BehaviorSelection))
        return;
      this.CanExecuteChanged((object) this, EventArgs.Empty);
    }
  }
}

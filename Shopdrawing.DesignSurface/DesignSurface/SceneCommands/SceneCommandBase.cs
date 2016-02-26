// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SceneCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class SceneCommandBase : Command
  {
    private SceneViewModel viewModel;

    protected SceneViewModel SceneViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    protected SceneView SceneView
    {
      get
      {
        return this.viewModel.DefaultView;
      }
    }

    protected DesignerContext DesignerContext
    {
      get
      {
        return this.viewModel.DesignerContext;
      }
    }

    protected virtual ViewState RequiredSelectionViewState
    {
      get
      {
        return SceneCommandBase.DefaultRequiredSelectionViewState;
      }
    }

    public static ViewState DefaultRequiredSelectionViewState
    {
      get
      {
        return ViewState.ElementValid | ViewState.AncestorValid | ViewState.SubtreeValid;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return SceneCommandBase.SceneCommandIsEnabled(this.SceneView, this.RequiredSelectionViewState);
        return false;
      }
    }

    protected SceneCommandBase(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public static bool SceneCommandIsEnabled(SceneView sceneView, ViewState viewState)
    {
      return SceneViewDesignCommand.DesignCommandEnabled(sceneView) && (viewState == ViewState.None || (viewState & sceneView.GetViewStateForSelection()) == viewState);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SelectAllCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class SelectAllCommand : SceneCommandBase
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
        if (base.IsEnabled)
        {
          if (this.SceneViewModel.TextSelectionSet.IsActive)
            return true;
          if (this.SceneViewModel.StoryboardSelectionSet.IsEmpty && this.SceneViewModel.KeyFrameSelectionSet.IsEmpty)
          {
            SceneElement selectionRoot = SelectAllCommand.GetSelectionRoot(this.SceneViewModel);
            if (selectionRoot != null)
            {
              foreach (SceneElement element in SceneElementHelper.GetElementTree(selectionRoot))
              {
                if (this.IsSelectable(element, selectionRoot))
                  return true;
              }
            }
          }
        }
        return false;
      }
    }

    public SelectAllCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public static SceneElement GetSelectionRoot(SceneViewModel viewModel)
    {
      return (SceneElement) viewModel.FindPanelClosestToActiveEditingContainer() ?? viewModel.ActiveEditingContainer as SceneElement;
    }

    public static bool IsSelectable(SceneElement element, SceneElement parentElement, SceneElement selectionRoot)
    {
      if (parentElement == selectionRoot)
        return element.IsVisuallySelectable;
      return false;
    }

    private bool IsSelectable(SceneElement element, SceneElement selectionRoot)
    {
      return SelectAllCommand.IsSelectable(element, element.EffectiveParent, selectionRoot);
    }

    public override void Execute()
    {
      if (this.SceneViewModel.TextSelectionSet.IsActive)
      {
        this.SceneViewModel.TextSelectionSet.TextEditProxy.EditingElement.SelectAll();
      }
      else
      {
        SelectionManagerPerformanceHelper.MeasurePerformanceUntilPipelinePostSceneUpdate(this.SceneViewModel.DesignerContext.SelectionManager, PerformanceEvent.SelectElement);
        ISceneElementCollection elementCollection = (ISceneElementCollection) new SceneElementCollection();
        SceneElement selectionRoot = SelectAllCommand.GetSelectionRoot(this.SceneViewModel);
        if (selectionRoot != null)
        {
          foreach (SceneElement element in SceneElementHelper.GetElementTree(selectionRoot))
          {
            if (this.IsSelectable(element, selectionRoot))
              elementCollection.Add(element);
          }
        }
        this.SceneViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) elementCollection, (SceneElement) null);
        this.SceneViewModel.PathPartSelectionSet.Clear();
      }
    }
  }
}

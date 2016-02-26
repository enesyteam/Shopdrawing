// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.CutCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class CutCommand : SceneCommandBase
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
        if (!base.IsEnabled || !this.SceneViewModel.CanDeleteSelection)
          return false;
        return CopyCommand.CanCopySelection(this.SceneViewModel);
      }
    }

    public CutCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitCut))
      {
        if (this.SceneViewModel.TextSelectionSet.IsActive)
          this.SceneViewModel.TextSelectionSet.TextEditProxy.EditingElement.Cut();
        else if (!this.SceneViewModel.KeyFrameSelectionSet.IsEmpty)
          CutCommand.CutKeyframes(this.SceneViewModel, this.SceneViewModel.KeyFrameSelectionSet.Selection[0].TargetElement as SceneElement, (ICollection<KeyFrameSceneNode>) this.SceneViewModel.KeyFrameSelectionSet.Selection);
        else if (!this.SceneViewModel.ChildPropertySelectionSet.IsEmpty)
          CutCommand.CutPropertyNodes(this.SceneViewModel, (IList<SceneNode>) this.SceneViewModel.ChildPropertySelectionSet.Selection);
        else if (!this.SceneViewModel.BehaviorSelectionSet.IsEmpty)
        {
          CutCommand.CutBehaviorNodes(this.SceneViewModel, (IList<BehaviorBaseNode>) this.SceneViewModel.BehaviorSelectionSet.Selection);
        }
        else
        {
          List<SceneElement> elements = new List<SceneElement>((IEnumerable<SceneElement>) this.SceneViewModel.ElementSelectionSet.Selection);
          elements.Sort((IComparer<SceneElement>) new ZOrderComparer<SceneElement>(this.SceneViewModel.RootNode));
          this.SceneViewModel.ElementSelectionSet.Clear();
          CutCommand.CutElements(this.SceneViewModel, elements);
        }
        editTransaction.Commit();
      }
    }

    private static void CutBehaviorNodes(SceneViewModel viewModel, IList<BehaviorBaseNode> behaviorNodes)
    {
      CopyCommand.CopyBehaviorNodes(viewModel, behaviorNodes);
      foreach (BehaviorBaseNode node in (IEnumerable<BehaviorBaseNode>) behaviorNodes)
        BehaviorHelper.DeleteBehavior(node);
    }

    public static void CutPropertyNodes(SceneViewModel viewModel, IList<SceneNode> nodes)
    {
      CopyCommand.CopyPropertyNodes(viewModel, nodes);
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) nodes)
      {
        if (sceneNode != null)
          sceneNode.Remove();
      }
    }

    public static void CutElements(SceneViewModel viewModel, List<SceneElement> elements)
    {
      CopyCommand.CopyElements(viewModel, elements);
      foreach (SceneElement element in elements)
      {
        if (element.Parent != null)
          viewModel.DeleteElementTree(element);
      }
    }

    public static void CutKeyframes(SceneViewModel viewModel, SceneElement targetElement, ICollection<KeyFrameSceneNode> keyframes)
    {
      CopyCommand.CopyKeyframes(viewModel, targetElement, keyframes);
      foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) keyframes)
        viewModel.AnimationEditor.DeleteKeyframe(keyFrameSceneNode.TargetElement, keyFrameSceneNode.TargetProperty, keyFrameSceneNode.Time);
    }
  }
}

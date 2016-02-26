// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.CopyCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class CopyCommand : SceneCommandBase
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
        return CopyCommand.CanCopySelection(this.SceneViewModel);
      }
    }

    public CopyCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      if (this.SceneViewModel.TextSelectionSet.IsActive)
        this.SceneViewModel.TextSelectionSet.TextEditProxy.EditingElement.Copy();
      else if (!this.SceneViewModel.KeyFrameSelectionSet.IsEmpty)
        CopyCommand.CopyKeyframes(this.SceneViewModel, this.SceneViewModel.KeyFrameSelectionSet.Selection[0].TargetElement as SceneElement, (ICollection<KeyFrameSceneNode>) this.SceneViewModel.KeyFrameSelectionSet.Selection);
      else if (!this.SceneViewModel.ChildPropertySelectionSet.IsEmpty)
        CopyCommand.CopyPropertyNodes(this.SceneViewModel, (IList<SceneNode>) this.SceneViewModel.ChildPropertySelectionSet.Selection);
      else if (!this.SceneViewModel.BehaviorSelectionSet.IsEmpty)
        CopyCommand.CopyBehaviorNodes(this.SceneViewModel, (IList<BehaviorBaseNode>) this.SceneViewModel.BehaviorSelectionSet.Selection);
      else if (!this.SceneViewModel.AnnotationSelectionSet.IsEmpty && this.DesignerContext.AnnotationService != null)
      {
        this.DesignerContext.AnnotationService.CopyToClipboardAsText((IEnumerable<AnnotationSceneNode>) this.SceneViewModel.AnnotationSelectionSet.Selection);
      }
      else
      {
        List<SceneElement> elements = new List<SceneElement>((IEnumerable<SceneElement>) this.SceneViewModel.ElementSelectionSet.Selection);
        elements.Sort((IComparer<SceneElement>) new ZOrderComparer<SceneElement>(this.SceneViewModel.RootNode));
        CopyCommand.CopyElements(this.SceneViewModel, elements);
      }
    }

    public static bool CanCopySelection(SceneViewModel viewModel)
    {
      if (viewModel.DesignerContext.ActiveView.EventRouter.IsEditingText)
      {
        if (viewModel.TextSelectionSet.IsActive)
          return viewModel.TextSelectionSet.CanCopy;
        return false;
      }
      if (viewModel.ElementSelectionSet.IsEmpty && viewModel.KeyFrameSelectionSet.IsEmpty && (viewModel.ChildPropertySelectionSet.IsEmpty && viewModel.BehaviorSelectionSet.IsEmpty) && viewModel.AnnotationSelectionSet.IsEmpty)
        return false;
      if (viewModel.ChildPropertySelectionSet.Selection.Count == 1)
        return true;
      if (viewModel.ChildPropertySelectionSet.Selection.Count > 1)
        return false;
      if (!viewModel.BehaviorSelectionSet.IsEmpty || !viewModel.AnnotationSelectionSet.IsEmpty)
        return true;
      if (viewModel.KeyFrameSelectionSet.IsEmpty)
      {
        if (!CopyCommand.ElementsHaveSameParent((ICollection<SceneElement>) viewModel.ElementSelectionSet.Selection))
          return false;
        foreach (SceneElement sceneElement in viewModel.ElementSelectionSet.Selection)
        {
          if (sceneElement is CameraElement)
            return false;
        }
      }
      else
      {
        IEnumerator<KeyFrameSceneNode> enumerator = viewModel.KeyFrameSelectionSet.Selection.GetEnumerator();
        enumerator.MoveNext();
        SceneNode targetElement = enumerator.Current.TargetElement;
        while (enumerator.MoveNext())
        {
          if (enumerator.Current.TargetElement != targetElement)
            return false;
        }
      }
      return true;
    }

    public static void CopyPropertyNodes(SceneViewModel viewModel, IList<SceneNode> nodes)
    {
      if (nodes.Count == 0)
        return;
      PastePackage pastePackage = new PastePackage(viewModel);
      using (viewModel.ForceBaseValue())
        pastePackage.AddChildPropertyNodes(nodes);
      pastePackage.SendToClipboard();
    }

    public static void CopyBehaviorNodes(SceneViewModel viewModel, IList<BehaviorBaseNode> behaviorNodes)
    {
      PastePackage pastePackage = new PastePackage(viewModel);
      using (viewModel.ForceBaseValue())
      {
        foreach (SceneNode childPropertyNode in (IEnumerable<BehaviorBaseNode>) behaviorNodes)
        {
          if (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) childPropertyNode.Type))
          {
            pastePackage.AddChildPropertyNode(childPropertyNode);
          }
          else
          {
            BehaviorTriggerActionNode triggerActionNode1 = (BehaviorTriggerActionNode) childPropertyNode;
            BehaviorTriggerBaseNode behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) triggerActionNode1.Parent;
            if (behaviorTriggerBaseNode.Actions.Count > 1)
            {
              DocumentNode node1 = triggerActionNode1.DocumentNode.Clone(viewModel.Document.DocumentContext);
              BehaviorTriggerActionNode triggerActionNode2 = (BehaviorTriggerActionNode) viewModel.GetSceneNode(node1);
              DocumentNode node2 = behaviorTriggerBaseNode.DocumentNode.Clone(viewModel.Document.DocumentContext);
              behaviorTriggerBaseNode = (BehaviorTriggerBaseNode) viewModel.GetSceneNode(node2);
              behaviorTriggerBaseNode.Actions.Clear();
              behaviorTriggerBaseNode.Actions.Add((SceneNode) triggerActionNode2);
            }
            pastePackage.AddChildPropertyNode((SceneNode) behaviorTriggerBaseNode);
          }
        }
      }
      pastePackage.SendToClipboard();
    }

    public static void CopyElements(SceneViewModel viewModel, List<SceneElement> elements)
    {
      if (elements.Count == 0 || !CopyCommand.ElementsHaveSameParent((ICollection<SceneElement>) elements))
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.CopyElements);
      SceneNode parent = elements[0].Parent;
      PastePackage pastePackage = new PastePackage(viewModel);
      using (viewModel.ForceBaseValue())
        pastePackage.AddElements(elements, true);
      pastePackage.SendToClipboard();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.CopyElements);
    }

    public static bool ElementsHaveSameParent(ICollection<SceneElement> elements)
    {
      if (elements.Count < 2)
        return true;
      IEnumerator<SceneElement> enumerator = elements.GetEnumerator();
      enumerator.MoveNext();
      SceneNode parent = enumerator.Current.Parent;
      while (enumerator.MoveNext())
      {
        if (enumerator.Current.Parent != parent)
          return false;
      }
      return true;
    }

    public static void CopyKeyframes(SceneViewModel viewModel, SceneElement targetElement, ICollection<KeyFrameSceneNode> keyframes)
    {
      if (keyframes.Count == 0)
        return;
      List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>(keyframes.Count);
      foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) keyframes)
      {
        if (keyFrameSceneNode.KeyFrameAnimation.TargetElement == targetElement)
          list.Add(keyFrameSceneNode);
      }
      PastePackage pastePackage = new PastePackage(viewModel);
      pastePackage.AddKeyframes((ICollection<KeyFrameSceneNode>) list);
      pastePackage.SendToClipboard();
    }
  }
}

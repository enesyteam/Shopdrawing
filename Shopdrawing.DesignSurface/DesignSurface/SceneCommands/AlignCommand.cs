// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AlignCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class AlignCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count < 2)
          return false;
        SceneElement parentElement = elementSelectionSet.Selection[0].ParentElement;
        foreach (SceneElement sceneElement in elementSelectionSet.Selection)
        {
          if (!(sceneElement is BaseFrameworkElement) || (sceneElement.ParentElement == null || sceneElement.ParentElement != parentElement))
            return false;
        }
        return parentElement is BaseFrameworkElement;
      }
    }

    protected abstract string UndoDescription { get; }

    protected AlignCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override sealed void Execute()
    {
      BaseFrameworkElement child1 = this.SceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement;
      BaseFrameworkElement frameworkElement = child1.ParentElement as BaseFrameworkElement;
      ILayoutDesigner designerForParent = frameworkElement.ViewModel.GetLayoutDesignerForParent((SceneElement) frameworkElement, true);
      Rect childRect1 = designerForParent.GetChildRect(child1);
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoDescription))
      {
        foreach (BaseFrameworkElement child2 in this.SceneViewModel.ElementSelectionSet.Selection)
        {
          Rect childRect2 = designerForParent.GetChildRect(child2);
          Vector offset = this.GetOffset(childRect1, childRect2);
          childRect2.Offset(offset);
          designerForParent.SetChildRect(child2, childRect2, false, false);
        }
        editTransaction.Commit();
      }
    }

    protected abstract Vector GetOffset(Rect primaryBoundingBox, Rect elementBoundingBox);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeSameSizeCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class MakeSameSizeCommandBase : SceneCommandBase
  {
    public abstract string UndoDescription { get; }

    private ICollection<BaseFrameworkElement> Selection
    {
      get
      {
        if (this.SceneViewModel == null || this.SceneViewModel.ActiveStoryboardTimeline != null)
          return (ICollection<BaseFrameworkElement>) null;
        return this.SceneViewModel.ElementSelectionSet.GetFilteredSelection<BaseFrameworkElement>();
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.Selection != null)
          return this.Selection.Count > 1;
        return false;
      }
    }

    public MakeSameSizeCommandBase(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public abstract void SetSize(Rect sourceRect, BaseFrameworkElement target, ILayoutDesigner targetLayoutDesigner);

    public override void Execute()
    {
      BaseFrameworkElement child = this.SceneViewModel.ElementSelectionSet.PrimarySelection as BaseFrameworkElement;
      if (child == null)
        return;
      Rect childRect = child.ViewModel.GetLayoutDesignerForParent(child.Parent as SceneElement, true).GetChildRect(child);
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoDescription))
      {
        foreach (BaseFrameworkElement target in (IEnumerable<BaseFrameworkElement>) this.Selection)
        {
          if (target != child)
          {
            ILayoutDesigner designerForParent = target.ViewModel.GetLayoutDesignerForParent(target.Parent as SceneElement, true);
            this.SetSize(childRect, target, designerForParent);
          }
        }
        editTransaction.Commit();
      }
    }
  }
}

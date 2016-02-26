// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GridRowColumnAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class GridRowColumnAdornerSet : GridRowColumnAdornerSetBase
  {
    private GridColumnSelectionSet gridColumnSelectionSet;
    private GridRowSelectionSet gridRowSelectionSet;
    private bool draggingGridline;

    internal bool DraggingGridline
    {
      get
      {
        return this.draggingGridline;
      }
      set
      {
        this.draggingGridline = value;
      }
    }

    public GridColumnSelectionSet GridColumnSelectionSet
    {
      get
      {
        if (this.gridColumnSelectionSet == null)
          this.gridColumnSelectionSet = this.ViewModel.GridColumnSelectionSet;
        return this.gridColumnSelectionSet;
      }
    }

    public GridRowSelectionSet GridRowSelectionSet
    {
      get
      {
        if (this.gridRowSelectionSet == null)
          this.gridRowSelectionSet = this.ViewModel.GridRowSelectionSet;
        return this.gridRowSelectionSet;
      }
    }

    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new LayoutBehavior(this.ToolContext);
      }
    }

    public GridRowColumnAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      bool inGridDesignMode = this.Element.ViewModel.IsInGridDesignMode;
      this.CacheRowColumnCounts();
      this.AddAdorner((Adorner) new NewGridlineAdorner((AdornerSet) this, true));
      for (int index = 0; index < this.Columns; ++index)
      {
        if (inGridDesignMode)
        {
          this.AddAdorner((Adorner) new LayoutBackgroundHeaderAdorner((AdornerSet) this, true, index));
          this.AddAdorner((Adorner) new LayoutBackgroundBodyAdorner((AdornerSet) this, true, index));
          this.AddAdorner((Adorner) new GridLockAdorner((AdornerSet) this, true, index));
        }
        if (index > 0)
        {
          this.AddAdorner((Adorner) new LayoutLineAdorner((AdornerSet) this, true, index, false));
          this.AddAdorner((Adorner) new LayoutLineHeaderAdorner((AdornerSet) this, true, index));
        }
      }
      if (this.Columns == 0 || !inGridDesignMode)
        this.AddAdorner((Adorner) new LayoutEmptyBackgroundHeaderAdorner((AdornerSet) this, true));
      this.AddAdorner((Adorner) new NewGridlineAdorner((AdornerSet) this, false));
      for (int index = 0; index < this.Rows; ++index)
      {
        if (inGridDesignMode)
        {
          this.AddAdorner((Adorner) new LayoutBackgroundHeaderAdorner((AdornerSet) this, false, index));
          this.AddAdorner((Adorner) new LayoutBackgroundBodyAdorner((AdornerSet) this, false, index));
          this.AddAdorner((Adorner) new GridLockAdorner((AdornerSet) this, false, index));
        }
        if (index > 0)
        {
          this.AddAdorner((Adorner) new LayoutLineAdorner((AdornerSet) this, false, index, false));
          this.AddAdorner((Adorner) new LayoutLineHeaderAdorner((AdornerSet) this, false, index));
        }
      }
      if (this.Rows == 0 || !inGridDesignMode)
        this.AddAdorner((Adorner) new LayoutEmptyBackgroundHeaderAdorner((AdornerSet) this, false));
      this.AddAdorner((Adorner) new GridDesignModeAdorner((AdornerSet) this));
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      if (adorner is LayoutEmptyBackgroundHeaderAdorner)
        return Cursors.Arrow;
      if (adorner is GridLockAdorner)
        return Cursors.Hand;
      LayoutLineHeaderAdorner lineHeaderAdorner = adorner as LayoutLineHeaderAdorner;
      if (lineHeaderAdorner != null)
      {
        if (!lineHeaderAdorner.IsX)
          return Cursors.SizeNS;
        return Cursors.SizeWE;
      }
      if (!(adorner is NewGridlineAdorner))
        return ToolCursors.GridRowSelectCursor;
      if (((LayoutBehavior) this.Behavior).IsNewGridlineEnabled)
        return ToolCursors.AddArrowCursor;
      return Cursors.Arrow;
    }
  }
}

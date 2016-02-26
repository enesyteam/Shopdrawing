// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SelectionTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Selection;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SelectionTool : Tool, ISelectionTool
  {
    public override bool IsEnabled
    {
      get
      {
        return this.IsActiveViewValid;
      }
    }

    public override string Identifier
    {
      get
      {
        return "Selection";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.SelectionToolCaption;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.V;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "selection";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Selection;
      }
    }

    protected override ViewState RequiredActiveElementViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    internal override bool ShowExtensibleAdorners
    {
      get
      {
        return true;
      }
    }

    private SceneElement PrimarySelection
    {
      get
      {
        if (this.ToolContext.ActiveView != null)
          return this.ToolContext.ActiveView.ElementSelectionSet.PrimarySelection;
        return (SceneElement) null;
      }
    }

    public SelectionTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return this.CreateToolBehavior(this.GetActiveViewContext());
    }

    public override SelectionAdornerUsages GetSelectionAdornerUsages(SceneElement element)
    {
      SelectionAdornerUsages selectionAdornerUsages = base.GetSelectionAdornerUsages(element);
      if (element == null || !(element is ShapeElement) || element == this.PrimarySelection)
        selectionAdornerUsages = selectionAdornerUsages | SelectionAdornerUsages.ShowBoundingBox | SelectionAdornerUsages.UseBoundingBox | SelectionAdornerUsages.UseFullAdorners | SelectionAdornerUsages.ShowMotionPath;
      return selectionAdornerUsages;
    }

    public ToolBehavior CreateToolBehavior(ToolBehaviorContext behaviorContext)
    {
      ToolBehavior moveBehavior = (ToolBehavior) new RelocateBehavior(behaviorContext);
      return (ToolBehavior) new ElementEditorBehavior(behaviorContext, false, true, true, true, false, ToolCursors.SelectionCursor, ToolCursors.SelectElementCursor, ToolCursors.RelocateCursor, moveBehavior);
    }
  }
}

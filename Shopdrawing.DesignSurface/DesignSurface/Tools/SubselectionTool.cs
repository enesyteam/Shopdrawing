// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SubselectionTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.Tools.Selection;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SubselectionTool : Tool, ISelectionTool
  {
    private static ICollection<IAdornerSetCreator> SubselectionAdornerSetCreators;
    private static ICollection<IAdornerSetCreator> SubselectionAdornerSetCreators3D;

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
        return "Subselection";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.SubselectionToolCaption;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.A;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "subselection";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Subselection;
      }
    }

    protected override ViewState RequiredActiveElementViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    protected override bool AdornMultipleElementsAsOne
    {
      get
      {
        return false;
      }
    }

    internal override bool ShowExtensibleAdorners
    {
      get
      {
        return true;
      }
    }

    public SubselectionTool(ToolContext toolContext)
      : base(toolContext)
    {
      if (SubselectionTool.SubselectionAdornerSetCreators == null)
        SubselectionTool.SubselectionAdornerSetCreators = (ICollection<IAdornerSetCreator>) Array.AsReadOnly<IAdornerSetCreator>(new IAdornerSetCreator[5]
        {
          (IAdornerSetCreator) new PathAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.MotionPath),
          (IAdornerSetCreator) new PathAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.ScenePath),
          (IAdornerSetCreator) new PathAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.ClippingPath),
          (IAdornerSetCreator) new RectangleGeometryMoveAdornerSetCreator(),
          (IAdornerSetCreator) new RectangleGeometryScaleAdornerSetCreator()
        });
      if (SubselectionTool.SubselectionAdornerSetCreators3D != null)
        return;
      SubselectionTool.SubselectionAdornerSetCreators3D = (ICollection<IAdornerSetCreator>) Array.AsReadOnly<IAdornerSetCreator>(new IAdornerSetCreator[3]
      {
        (IAdornerSetCreator) new RotateAdornerSetCreator3D(),
        (IAdornerSetCreator) new ScaleAdornerSetCreator3D(),
        (IAdornerSetCreator) new TranslateAdornerSetCreator3D()
      });
    }

    public override SelectionAdornerUsages GetSelectionAdornerUsages(SceneElement element)
    {
      return base.GetSelectionAdornerUsages(element) | SelectionAdornerUsages.UseBoundingBox | SelectionAdornerUsages.ShowClipping | SelectionAdornerUsages.ShowMotionPath;
    }

    protected override void AddEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
      ViewState viewState = ViewState.ElementValid | ViewState.AncestorValid | ViewState.SubtreeValid;
      if ((this.ActiveView.GetViewState((SceneNode) element) & viewState) == viewState)
      {
        if (element is BaseFrameworkElement)
        {
          using (IEnumerator<IAdornerSetCreator> enumerator = SubselectionTool.SubselectionAdornerSetCreators.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IAdornerSetCreator current = enumerator.Current;
              adornerSetCreators.Add(current);
            }
            return;
          }
        }
        else if (element is Base3DElement)
        {
          if (this.ActiveSceneViewModel.ElementSelectionSet.Count == 1)
          {
            foreach (IAdornerSetCreator adornerSetCreator in (IEnumerable<IAdornerSetCreator>) SubselectionTool.SubselectionAdornerSetCreators3D)
              adornerSetCreators.Add(adornerSetCreator);
          }
          if (!(element is LightElement))
            return;
          Tool.Add3DLightAdorners(adornerSetCreators, element, false);
          return;
        }
      }
      base.AddEditingAdornerSetCreatorsForSelectedElement(adornerSetCreators, element);
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return this.CreateToolBehavior(this.GetActiveViewContext());
    }

    public ToolBehavior CreateToolBehavior(ToolBehaviorContext behaviorContext)
    {
      ToolBehavior moveBehavior = (ToolBehavior) new RelocateBehavior(behaviorContext);
      return (ToolBehavior) new ElementEditorBehavior(behaviorContext, true, true, true, true, true, ToolCursors.SubselectionCursor, ToolCursors.SubselectElementCursor, ToolCursors.SubselectMoveCursor, moveBehavior);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PenTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class PenTool : Tool
  {
    private static ICollection<IAdornerSetCreator> PenAdornerSetCreators;
    private ActivePathEditInformation activePathEditInformation;

    public ActivePathEditInformation ActivePathEditInformation
    {
      get
      {
        return this.activePathEditInformation;
      }
      set
      {
        this.activePathEditInformation = value;
      }
    }

    public override string Identifier
    {
      get
      {
        return "Pen";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.PenToolCaption;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.StackedToolDescription;
      }
    }

    public override Key Key
    {
      get
      {
        return Key.P;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "pen";
      }
    }

    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.Drawing;
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

    public PenTool(ToolContext toolContext)
      : base(toolContext)
    {
      if (PenTool.PenAdornerSetCreators != null)
        return;
      PenTool.PenAdornerSetCreators = (ICollection<IAdornerSetCreator>) Array.AsReadOnly<IAdornerSetCreator>(new IAdornerSetCreator[3]
      {
        (IAdornerSetCreator) new PenAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.ScenePath),
        (IAdornerSetCreator) new PenAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.MotionPath),
        (IAdornerSetCreator) new PenAdornerSetCreator(this.PathEditorTargetMap, PathEditMode.ClippingPath)
      });
    }

    public override SelectionAdornerUsages GetSelectionAdornerUsages(SceneElement element)
    {
      return base.GetSelectionAdornerUsages(element) | SelectionAdornerUsages.ShowClipping | SelectionAdornerUsages.ShowMotionPath;
    }

    protected override void AddEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
      if (element is Base2DElement)
      {
        foreach (IAdornerSetCreator adornerSetCreator in (IEnumerable<IAdornerSetCreator>) PenTool.PenAdornerSetCreators)
          adornerSetCreators.Add(adornerSetCreator);
      }
      else
        base.AddEditingAdornerSetCreatorsForSelectedElement(adornerSetCreators, element);
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      return (ToolBehavior) new PenCreateBehavior(this.GetActiveViewContext());
    }

    protected override void OnDeactivate()
    {
      this.activePathEditInformation = (ActivePathEditInformation) null;
      base.OnDeactivate();
    }

    protected override void OnEarlySceneUpdatePhase(SceneUpdatePhaseEventArgs args)
    {
      base.OnEarlySceneUpdatePhase(args);
      if (this.activePathEditInformation == null || this.activePathEditInformation.ActivePathEditorTarget.EditingElement.ViewObject != null && this.ActiveView.ViewModel.ElementSelectionSet.IsSelected((SceneElement) this.activePathEditInformation.ActivePathEditorTarget.EditingElement))
        return;
      this.activePathEditInformation = (ActivePathEditInformation) null;
    }
  }
}

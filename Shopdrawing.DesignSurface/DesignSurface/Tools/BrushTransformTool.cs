// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.BrushTransformTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools.Selection;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class BrushTransformTool : BrushTool
  {
    private static ICollection<IAdornerSetCreator> BrushTransformAdornerSetCreators;

    public override Key Key
    {
      get
      {
        return Key.B;
      }
    }

    public override string Identifier
    {
      get
      {
        return "BrushTransform";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.BrushTransformToolCaption;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "brushxform";
      }
    }

    public BrushTransformTool(ToolContext toolContext)
      : base(toolContext)
    {
      if (BrushTransformTool.BrushTransformAdornerSetCreators != null)
        return;
      BrushTransformTool.BrushTransformAdornerSetCreators = (ICollection<IAdornerSetCreator>) Array.AsReadOnly<IAdornerSetCreator>(new IAdornerSetCreator[4]
      {
        (IAdornerSetCreator) new BrushTranslateAdornerSetCreator(),
        (IAdornerSetCreator) new BrushSkewAdornerSetCreator(),
        (IAdornerSetCreator) new BrushRotateAdornerSetCreator(),
        (IAdornerSetCreator) new BrushScaleAdornerSetCreator()
      });
    }

    protected override void AddEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
      if (BrushTool.GetBrushPropertyReference((SceneNode) element) != null && element is Base2DElement)
      {
        foreach (IAdornerSetCreator adornerSetCreator in (IEnumerable<IAdornerSetCreator>) BrushTransformTool.BrushTransformAdornerSetCreators)
          adornerSetCreators.Add(adornerSetCreator);
      }
      else
        base.AddEditingAdornerSetCreatorsForSelectedElement(adornerSetCreators, element);
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      ToolBehaviorContext activeViewContext = this.GetActiveViewContext();
      ToolBehavior moveBehavior = (ToolBehavior) new BrushTranslateBehavior(activeViewContext);
      return (ToolBehavior) new ElementEditorBehavior(activeViewContext, true, false, false, false, false, ToolCursors.SubselectionCursor, ToolCursors.SubselectElementCursor, ToolCursors.SubselectMoveCursor, moveBehavior);
    }

    internal override bool ShouldRebuildAdorner(DocumentNodeChange change)
    {
      if (change == null || change.NewChildNode == null || change.OldChildNode == null)
        return false;
      ITypeId type1 = (ITypeId) change.NewChildNode.Type;
      ITypeId type2 = (ITypeId) change.OldChildNode.Type;
      if (!type1.Equals((object) type2) && PlatformTypes.Brush.IsAssignableFrom(type1))
        return PlatformTypes.Brush.IsAssignableFrom(type2);
      return false;
    }
  }
}

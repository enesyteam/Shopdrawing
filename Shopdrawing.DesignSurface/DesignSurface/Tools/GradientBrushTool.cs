// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.GradientBrushTool
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
  internal sealed class GradientBrushTool : BrushTool
  {
    private static ICollection<IAdornerSetCreator> BrushTransformAdornerSetCreators;

    public override Key Key
    {
      get
      {
        return Key.G;
      }
    }

    public override string Identifier
    {
      get
      {
        return "GradientTool";
      }
    }

    public override string Caption
    {
      get
      {
        return StringTable.GradientToolCaption;
      }
    }

    public override string IconBrushKey
    {
      get
      {
        return "brushgradient";
      }
    }

    public GradientBrushTool(ToolContext toolContext)
      : base(toolContext)
    {
      if (GradientBrushTool.BrushTransformAdornerSetCreators != null)
        return;
      GradientBrushTool.BrushTransformAdornerSetCreators = (ICollection<IAdornerSetCreator>) Array.AsReadOnly<IAdornerSetCreator>(new IAdornerSetCreator[6]
      {
        (IAdornerSetCreator) new BrushTranslateAdornerSetCreator(),
        (IAdornerSetCreator) new RadialRotateAdornerSetCreator(),
        (IAdornerSetCreator) new RadialScaleAdornerSetCreator(),
        (IAdornerSetCreator) new LinearGradientAdornerSetCreator(),
        (IAdornerSetCreator) new RadialGradientAdornerSetCreator(),
        (IAdornerSetCreator) new GradientStopAdornerSetCreator()
      });
    }

    protected override void AddEditingAdornerSetCreatorsForSelectedElement(IList<IAdornerSetCreator> adornerSetCreators, SceneElement element)
    {
      if (BrushTool.GetBrushPropertyReference((SceneNode) element) != null && element is Base2DElement)
      {
        foreach (IAdornerSetCreator adornerSetCreator in (IEnumerable<IAdornerSetCreator>) GradientBrushTool.BrushTransformAdornerSetCreators)
          adornerSetCreators.Add(adornerSetCreator);
      }
      else
        base.AddEditingAdornerSetCreatorsForSelectedElement(adornerSetCreators, element);
    }

    protected override ToolBehavior CreateToolBehavior()
    {
      ToolBehaviorContext activeViewContext = this.GetActiveViewContext();
      ToolBehavior moveBehavior = (ToolBehavior) new BrushBackgroundBehavior(activeViewContext);
      return (ToolBehavior) new ElementEditorBehavior(activeViewContext, true, false, false, false, false, ToolCursors.SubselectionCursor, ToolCursors.SubselectElementCursor, ToolCursors.GradientRedefineCursor, moveBehavior);
    }

    internal override bool ShouldRebuildAdorner(DocumentNodeChange change)
    {
      return (change.Action == DocumentNodeChangeAction.Add || change.Action == DocumentNodeChangeAction.Remove) && (change.ParentNode != null && change.ParentNode.Type.Equals((object) PlatformTypes.GradientStopCollection)) || change.Action == DocumentNodeChangeAction.Replace && change.ParentNode != null && (change.ParentNode.Type.Equals((object) PlatformTypes.GradientStop) && change.ParentNode.Parent != null) && (change.ParentNode.Parent.Parent != null && change.ParentNode.Parent.Parent.Parent != null && change.ParentNode.Parent.Parent.Parent.Type.Equals((object) PlatformTypes.DictionaryEntry)) || (change.Action == DocumentNodeChangeAction.Replace && change.ParentNode != null && (change.ParentNode.Type.Equals((object) PlatformTypes.DictionaryEntry) && change.NewChildNode != null) && (change.OldChildNode != null && PlatformTypes.Brush.IsAssignableFrom((ITypeId) change.NewChildNode.Type) && PlatformTypes.Brush.IsAssignableFrom((ITypeId) change.OldChildNode.Type)) || (change.Action == DocumentNodeChangeAction.Replace || change.Action == DocumentNodeChangeAction.Add) && change.NewChildNode != null && (change.NewChildNode.Type.Equals((object) PlatformTypes.LinearGradientBrush) || change.NewChildNode.Type.Equals((object) PlatformTypes.RadialGradientBrush))) || (change.Action == DocumentNodeChangeAction.Replace || change.Action == DocumentNodeChangeAction.Add) && (change.NewChildNode != null && change.NewChildNode.Type.Equals((object) PlatformTypes.GradientStopCollection));
    }
  }
}

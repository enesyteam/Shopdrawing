// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GradientStopAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class GradientStopAdornerSet : BrushTransformAdornerSet
  {
    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new GradientStopBehavior(this.ToolContext);
      }
    }

    public GradientStopAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
      : base(toolContext, adornedElement)
    {
      this.Element.DesignerContext.GradientToolSelectionService.PropertyChanged += new PropertyChangedEventHandler(this.OnGradientStopSelectionChange);
    }

    private void OnGradientStopSelectionChange(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Index"))
        return;
      this.InvalidateRender();
    }

    protected override void CreateAdorners()
    {
      GradientBrush gradientBrush = (GradientBrush) null;
      PropertyReference propertyReference = this.BrushPropertyReference;
      if (propertyReference != null)
      {
        gradientBrush = this.Element.GetComputedValueAsWpf(propertyReference) as GradientBrush;
        DocumentNodePath valueAsDocumentNode = this.Element.GetLocalValueAsDocumentNode(propertyReference);
        if (valueAsDocumentNode != null)
          this.Element.DesignerContext.GradientToolSelectionService.AdornedBrush = valueAsDocumentNode.Node;
        else
          this.Element.DesignerContext.GradientToolSelectionService.AdornedBrush = (DocumentNode) null;
      }
      if (gradientBrush == null)
        return;
      for (int index = 0; index < gradientBrush.GradientStops.Count; ++index)
        this.AddAdorner((Adorner) new GradientStopAdorner((BrushTransformAdornerSet) this, index));
    }

    public override void OnRemove()
    {
      base.OnRemove();
      this.Element.DesignerContext.GradientToolSelectionService.PropertyChanged -= new PropertyChangedEventHandler(this.OnGradientStopSelectionChange);
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      return ToolCursors.SubselectionCursor;
    }
  }
}

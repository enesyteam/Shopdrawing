// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PathAdorner : Adorner
  {
    private PathEditMode pathEditMode;

    public override bool SupportsProjectionTransforms
    {
      get
      {
        return true;
      }
    }

    private Pen ThinPathPen
    {
      get
      {
        switch (this.pathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetThinPen(AdornerType.MotionPathSegment);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetThinPen(AdornerType.ClipPath);
          default:
            return this.ThinPen;
        }
      }
    }

    public PathAdorner(AdornerSet adornerSet, PathEditMode pathEditMode)
      : base(adornerSet)
    {
      this.pathEditMode = pathEditMode;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      foreach (IAdornerSet adornerSet in (IEnumerable<IAdornerSet>) this.AdornerSet.ToolContext.View.AdornerLayer.GetAdornerSets(this.Element))
      {
        PathAdornerSet pathAdornerSet = adornerSet as PathAdornerSet;
        if (pathAdornerSet != null && pathAdornerSet.PathEditorTarget.PathEditMode == this.pathEditMode)
          return;
      }
      System.Windows.Media.Geometry renderedGeometry = (System.Windows.Media.Geometry)null;
      switch (this.pathEditMode)
      {
        case PathEditMode.ScenePath:
          if (this.Element.Visual != null && PlatformTypes.Shape.IsAssignableFrom((ITypeId) this.Element.Visual.GetIType((ITypeResolver) this.Element.ProjectContext)))
          {
            SceneElement element = this.Element;
            renderedGeometry = element.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) ? (element.ViewObject.PlatformSpecificObject as Shape).RenderedGeometry : element.ViewModel.DefaultView.GetRenderedGeometryAsWpf(element);
            break;
          }
          break;
        case PathEditMode.MotionPath:
          MotionPathEditorTarget pathEditorTarget1 = new MotionPathEditorTarget((Base2DElement) this.Element);
          renderedGeometry = (System.Windows.Media.Geometry)pathEditorTarget1.PathGeometry;
          if (renderedGeometry == null || renderedGeometry.IsEmpty())
            return;
          matrix = pathEditorTarget1.GetTransformToAncestor(this.Element.ViewTargetElement) * matrix;
          break;
        case PathEditMode.ClippingPath:
          using (ClippingPathEditorTarget pathEditorTarget2 = new ClippingPathEditorTarget((Base2DElement) this.Element))
          {
              renderedGeometry = (System.Windows.Media.Geometry)pathEditorTarget2.PathGeometry;
            if (renderedGeometry == null || renderedGeometry.IsEmpty())
              return;
            matrix = pathEditorTarget2.GetTransformToAncestor(this.Element.ViewTargetElement) * matrix;
            break;
          }
      }
      if (renderedGeometry == null || renderedGeometry.IsEmpty())
        return;
      ctx.DrawGeometry((Brush) null, this.ThinPathPen, this.GetTransformedGeometry(this.DesignerContext.ActiveView, this.Element, renderedGeometry, matrix));
    }
  }
}

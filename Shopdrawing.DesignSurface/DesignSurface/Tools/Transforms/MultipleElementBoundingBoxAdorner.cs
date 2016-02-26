// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MultipleElementBoundingBoxAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MultipleElementBoundingBoxAdorner : BoundingBoxAdorner
  {
    public MultipleElementBoundingBoxAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (!this.ShouldDraw)
        return;
      SceneElementSelectionSet elementSelectionSet = this.DesignerContext.ActiveSceneViewModel.ElementSelectionSet;
      AdornerLayer adornerLayer = this.DesignerContext.ActiveView.AdornerLayer;
      Tool adornerOwnerTool = this.DesignerContext.ToolManager.ActiveTool.AdornerOwnerTool;
      SceneView activeView = this.DesignerContext.ActiveView;
      Transform transform = activeView.Artboard.CalculateTransformFromContentToArtboard();
      bool flag1 = this.ElementSet.AdornsMultipleElements && (adornerOwnerTool.GetSelectionAdornerUsages((SceneElement) null) & SelectionAdornerUsages.ShowBoundingBox) != SelectionAdornerUsages.None;
      bool flag2 = false;
      bool flag3 = true;
      Rect rectangle = new Rect();
      foreach (SceneElement element in this.ElementSet.Elements)
      {
        IViewObject viewTargetElement = element.ViewTargetElement;
        if (!(element is Base3DElement) && !element.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed && (viewTargetElement != null && element.Visual != null))
        {
          Rect actualBounds = this.DesignerContext.ActiveView.GetActualBounds(viewTargetElement);
          bool flag4 = element == this.ElementSet.PrimaryElement;
          if ((adornerOwnerTool.GetSelectionAdornerUsages(element) & SelectionAdornerUsages.ShowBoundingBox) != SelectionAdornerUsages.None && this.ElementSet.Elements.Count < 100 || flag4)
          {
            Pen pen = flag4 ? this.MediumPen : this.BorderPen;
            System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(this.DesignerContext.ActiveView, element, actualBounds, pen.Thickness, true);
            drawingContext.DrawGeometry((Brush) null, pen, rectangleGeometry);
          }
          if (flag1)
          {
            Rect rect1 = activeView.TransformBounds(element.Visual, (IViewObject) activeView.HitTestRoot, actualBounds);
            Rect rect2 = transform.TransformBounds(rect1);
            if (!flag2)
              flag2 = Adorner.NonAffineTransformInParentStack(element);
            if (flag3)
            {
              rectangle = rect2;
              flag3 = false;
            }
            else
              rectangle.Union(rect2);
          }
        }
      }
      if (!flag1)
        return;
      if (flag2)
      {
        rectangle.Inflate(this.MediumPen.Thickness / 2.0, this.MediumPen.Thickness / 2.0);
        drawingContext.DrawRectangle((Brush) null, this.MediumPen, rectangle);
      }
      else
      {
          System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(this.ElementSet.ElementBounds, this.ElementSet.GetTransformMatrixToAdornerLayer(), this.MediumPen.Thickness);
        drawingContext.DrawGeometry((Brush) null, this.MediumPen, rectangleGeometry);
      }
    }
  }
}

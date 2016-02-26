// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal sealed class AnnotationAdorner : BoundingBoxAdorner, IClickable
  {
    private static Pen clickAidePen = new Pen((Brush) Brushes.Transparent, 6.0);
    public const string ContextMenuTag = "AnnotationAdorner.ContextMenuTag";

    public AnnotationSceneNode Annotation { get; private set; }

    protected override bool ShouldDraw
    {
      get
      {
        if (base.ShouldDraw && this.AnnotationService.ShowAnnotations && (this.AnnotationService.AnnotationsEnabled && this.Annotation.Visual != null))
          return this.Annotation.Visual.ViewModel.Selected;
        return false;
      }
    }

    public override Pen BorderPen
    {
      get
      {
        return this.MediumPen;
      }
    }

    public Pen ClickAidePen
    {
      get
      {
        return AnnotationAdorner.clickAidePen;
      }
    }

    private Rect IconBounds
    {
      get
      {
        return new Rect()
        {
          Location = this.Annotation.Position,
          Size = this.TransformFromRenderToLayout(this.Annotation.Visual.IconView.RenderSize)
        };
      }
    }

    public AnnotationAdorner.LineSegment AttachmentLine
    {
      get
      {
        Rect iconBounds = this.IconBounds;
        Rect rect = Rect.Transform(this.ElementBounds, this.Element.TransformToRoot);
        Point center1 = this.GetCenter(iconBounds);
        Point center2 = this.GetCenter(rect);
        Point startPoint = this.IntersectRectangle(iconBounds, center1, center2) ?? center1;
        Point point = this.IntersectRectangle(rect, startPoint, center2) ?? center2;
        Matrix transformFromRoot = this.Element.TransformFromRoot;
        return new AnnotationAdorner.LineSegment()
        {
          P1 = startPoint * transformFromRoot,
          P2 = point * transformFromRoot
        };
      }
    }

    private AnnotationService AnnotationService
    {
      get
      {
        return this.DesignerContext.AnnotationService;
      }
    }

    static AnnotationAdorner()
    {
      AnnotationAdorner.clickAidePen.Freeze();
    }

    public AnnotationAdorner(AdornerSet adornerSet, AnnotationSceneNode annotation)
      : base(adornerSet)
    {
      this.Annotation = annotation;
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (!this.ShouldDraw)
        return;
      this.DrawBounds(drawingContext, (Brush) Brushes.Transparent, this.BorderPen);
      AnnotationAdorner.LineSegment lineSegment = this.AttachmentLine * matrix;
      drawingContext.DrawLine(this.ClickAidePen, lineSegment.P1, lineSegment.P2);
      drawingContext.DrawLine(this.BorderPen, lineSegment.P1, lineSegment.P2);
    }

    public void DisplayContextMenu(Point pointerPosition)
    {
      SceneView defaultView = this.Element.ViewModel.DefaultView;
      FrameworkElement sceneScrollViewer = defaultView.SceneScrollViewer;
      Point point = defaultView.ViewRootContainer.TransformToAncestor((Visual) sceneScrollViewer).Transform(pointerPosition);
      ContextMenu contextMenu1 = new ContextMenu();
      contextMenu1.Placement = PlacementMode.RelativePoint;
      contextMenu1.PlacementTarget = (UIElement) sceneScrollViewer;
      contextMenu1.PlacementRectangle = new Rect()
      {
        Location = point
      };
      contextMenu1.Tag = (object) "AnnotationAdorner.ContextMenuTag";
      ContextMenu contextMenu2 = contextMenu1;
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Command = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.RemoveAnnotationLink));
      menuItem1.Header = (object) StringTable.UnlinkAnnotationContextMenuText;
      menuItem1.Name = "Unlink";
      MenuItem menuItem2 = menuItem1;
      AutomationElement.SetId((DependencyObject) menuItem2, "AttachmentContextMenuUnlink");
      contextMenu2.Items.Add((object) menuItem2);
      contextMenu2.IsOpen = true;
    }

    private void RemoveAnnotationLink()
    {
      this.AnnotationService.UnlinkAttachment(this.Annotation, this.Element);
    }

    private Point GetCenter(Rect rect)
    {
      return rect.Location + (Vector) rect.Size / 2.0;
    }

    private Point? IntersectRectangle(Rect rectangle, Point startPoint, Point endPoint)
    {
      Point? nullable1 = this.SegmentIntersect(rectangle.TopLeft, rectangle.TopRight, startPoint, endPoint);
      if (nullable1.HasValue)
        return new Point?(nullable1.GetValueOrDefault());
      Point? nullable2 = this.SegmentIntersect(rectangle.TopRight, rectangle.BottomRight, startPoint, endPoint);
      if (nullable2.HasValue)
        return new Point?(nullable2.GetValueOrDefault());
      Point? nullable3 = this.SegmentIntersect(rectangle.BottomRight, rectangle.BottomLeft, startPoint, endPoint);
      if (!nullable3.HasValue)
        return this.SegmentIntersect(rectangle.BottomLeft, rectangle.TopLeft, startPoint, endPoint);
      return new Point?(nullable3.GetValueOrDefault());
    }

    private Point? SegmentIntersect(Point start1, Point end1, Point start2, Point end2)
    {
      double num1 = end1.X - start1.X;
      double num2 = end1.Y - start1.Y;
      double num3 = end2.X - start2.X;
      double num4 = end2.Y - start2.Y;
      if (num1 * num4 - num2 * num3 == 0.0)
        return new Point?();
      double num5 = (num3 * (start1.Y - start2.Y) + num4 * (start2.X - start1.X)) / (num1 * num4 - num2 * num3);
      double num6 = (num1 * (start2.Y - start1.Y) + num2 * (start1.X - start2.X)) / (num2 * num3 - num1 * num4);
      if (num5 < 0.0 || num5 > 1.0 || (num6 < 0.0 || num6 > 1.0))
        return new Point?();
      return new Point?(new Point()
      {
        X = start2.X + num6 * num3,
        Y = start2.Y + num6 * num4
      });
    }

    private Size TransformFromRenderToLayout(Size renderSize)
    {
      return (Size) (this.Annotation.Visual.RenderTransform ?? Transform.Identity).Transform((Point) renderSize);
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.AttachmentLine.MidPoint * matrix;
    }

    internal class LineSegment
    {
      public Point P1 { get; set; }

      public Point P2 { get; set; }

      public Point MidPoint
      {
        get
        {
          return (Point) (((Vector) this.P1 + (Vector) this.P2) / 2.0);
        }
      }

      public static AnnotationAdorner.LineSegment operator *(AnnotationAdorner.LineSegment line, Matrix matrix)
      {
        return new AnnotationAdorner.LineSegment()
        {
          P1 = line.P1 * matrix,
          P2 = line.P2 * matrix
        };
      }
    }
  }
}

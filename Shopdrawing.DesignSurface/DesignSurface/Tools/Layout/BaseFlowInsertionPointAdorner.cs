// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Layout.BaseFlowInsertionPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Layout
{
  internal abstract class BaseFlowInsertionPointAdorner : Adorner
  {
      private static readonly System.Windows.Media.Geometry ArrowGeometry = System.Windows.Media.Geometry.Parse("M-6,-3 L0,0 -6,3z");
    private BaseFlowInsertionPoint baseFlowInsertionPoint;

    public override bool SupportsProjectionTransforms
    {
      get
      {
        return true;
      }
    }

    public BaseFlowInsertionPointAdorner(AdornerSet adornerSet, BaseFlowInsertionPoint baseFlowInsertionPoint)
      : base(adornerSet)
    {
      this.baseFlowInsertionPoint = baseFlowInsertionPoint;
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      Point position;
      double length;
      Orientation orientation;
      if (this.baseFlowInsertionPoint.IsEmpty || MoveStrategy.GetContainerHost((SceneElement) this.baseFlowInsertionPoint.Element) == null || !this.GetInsertionInfo((SceneElement) this.baseFlowInsertionPoint.Element, this.baseFlowInsertionPoint.Index, this.baseFlowInsertionPoint.IsCursorAtEnd, out position, out length, out orientation))
        return;
      Point point1 = position;
      Point point2 = position;
      switch (orientation)
      {
        case Orientation.Horizontal:
          point2.Y += length;
          break;
        case Orientation.Vertical:
          point2.X += length;
          break;
      }
      Brush activeBrush = FeedbackHelper.GetActiveBrush(AdornerType.Default);
      Pen thinPen = FeedbackHelper.GetThinPen(AdornerType.Default);
      Point point0 = this.TransformPoint(point1);
      Point point1_1 = this.TransformPoint(point2);
      Vector vector = point1_1 - point0;
      if (!Tolerances.NearZero(vector))
      {
        vector.Normalize();
        context.PushTransform((Transform) new MatrixTransform(new Matrix(vector.X, vector.Y, -vector.Y, -vector.X, point0.X, point0.Y)));
        context.DrawGeometry(activeBrush, thinPen, BaseFlowInsertionPointAdorner.ArrowGeometry);
        context.Pop();
        context.PushTransform((Transform) new MatrixTransform(new Matrix(-vector.X, -vector.Y, vector.Y, -vector.X, point1_1.X, point1_1.Y)));
        context.DrawGeometry(activeBrush, thinPen, BaseFlowInsertionPointAdorner.ArrowGeometry);
        context.Pop();
      }
      context.DrawLine(thinPen, point0, point1_1);
    }

    protected virtual bool GetInsertionInfo(SceneElement container, int insertionIndex, bool isCursorAtEnd, out Point position, out double length, out Orientation orientation)
    {
      position = new Point();
      length = 0.0;
      orientation = Orientation.Horizontal;
      return false;
    }
  }
}

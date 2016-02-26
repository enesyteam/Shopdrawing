// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RectangleGeometryMoveBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class RectangleGeometryMoveBehavior : AdornedToolBehavior
  {
    private Matrix elementToDocumentTransform;
    private RectangleGeometry startGeometry;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMoveClippingRectangle;
      }
    }

    public RectangleGeometryMoveBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.EnsureEditTransaction();
      this.CreateSubTransaction();
      this.startGeometry = this.EditingElement.GetLocalValueAsWpf(Base2DElement.ClipProperty) as RectangleGeometry;
      this.elementToDocumentTransform = RectangleGeometryAdornerSetBase.GetRectangleClipGeometryTransform(this.EditingElement);
      this.elementToDocumentTransform.Append(this.EditingElementSet.GetTransformMatrix((IViewObject) this.ActiveView.HitTestRoot));
      return base.OnButtonDown(pointerPosition);
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(dragCurrentPosition - dragStartPosition, this.elementToDocumentTransform);
      Rect rect = this.startGeometry.Rect;
      rect.Offset(correspondingVector);
      RectangleGeometry rectangleGeometry = this.startGeometry.Clone();
      rectangleGeometry.Rect = RoundingHelper.RoundRect(rect);
      this.EditingElement.SetLocalValueAsWpf(Base2DElement.ClipProperty, (object) rectangleGeometry);
      this.UpdateEditTransaction();
      this.ActiveView.EnsureVisible(this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.CommitEditTransaction();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.CommitEditTransaction();
      return base.OnClickEnd(pointerPosition, clickCount);
    }
  }
}

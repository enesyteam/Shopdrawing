// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Selection.RoundedRectangleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Selection
{
  internal sealed class RoundedRectangleBehavior : AdornedToolBehavior
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitRectangleRadius;
      }
    }

    private RoundedRectangleAdorner ActiveAdorner
    {
      get
      {
        return (RoundedRectangleAdorner) base.ActiveAdorner;
      }
    }

    public RoundedRectangleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      return base.OnButtonDown(pointerPosition);
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      SceneView activeView = this.ActiveView;
      this.EnsureEditTransaction();
      dragCurrentPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(dragCurrentPosition);
      Point point = dragCurrentPosition * activeView.GetComputedTransformFromRoot(this.EditingElement.Visual);
      if (this.ActiveAdorner.IsX)
      {
        double num = RoundingHelper.RoundLength(Math.Max(0.0, Math.Min(point.X, this.ActiveView.GetRenderSize(this.EditingElement.Visual).Width / 2.0) - this.ActiveAdorner.HalfStrokeThickness));
        this.EditingElement.SetValue(RectangleElement.RadiusXProperty, (object) num);
        if (!this.IsShiftDown)
          this.EditingElement.SetValue(RectangleElement.RadiusYProperty, (object) num);
      }
      else
      {
        double num = RoundingHelper.RoundLength(Math.Max(0.0, Math.Min(point.Y, this.ActiveView.GetRenderSize(this.EditingElement.Visual).Height / 2.0) - this.ActiveAdorner.HalfStrokeThickness));
        this.EditingElement.SetValue(RectangleElement.RadiusYProperty, (object) num);
        if (!this.IsShiftDown)
          this.EditingElement.SetValue(RectangleElement.RadiusXProperty, (object) num);
      }
      this.UpdateEditTransaction();
      activeView.EnsureVisible((IAdorner) this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.AllDone();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.AllDone();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    private void AllDone()
    {
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.CommitEditTransaction();
    }
  }
}

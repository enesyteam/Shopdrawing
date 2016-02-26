// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ZoomToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class ZoomToolBehavior : ToolBehavior
  {
    private bool enableAreaZoom;

    public ZoomToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.AdjustZoom);
      this.enableAreaZoom = !this.IsAltDown;
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (this.enableAreaZoom)
      {
        SceneView activeView = this.ActiveView;
        activeView.EnsureVisible(dragCurrentPosition, scrollNow);
        FeedbackHelper.DrawDashedRectangle(this.OpenFeedback(), activeView.Zoom, dragStartPosition, dragCurrentPosition);
        this.CloseFeedback();
      }
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.enableAreaZoom)
      {
        this.ClearFeedback();
        this.ActiveView.ZoomToFitRectangle(new Rect(dragStartPosition, dragEndPosition));
      }
      else
        this.ActiveView.Artboard.ZoomAroundFixedPoint(!this.IsAltDown, dragEndPosition);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.AdjustZoom);
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.ActiveView.Artboard.ZoomAroundFixedPoint(!this.IsAltDown, pointerPosition);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.AdjustZoom);
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      this.UpdateZoomCursor();
      return base.OnKey(args);
    }

    protected override void OnResume()
    {
      base.OnResume();
      this.UpdateZoomCursor();
    }

    protected override void OnSuspend()
    {
      base.OnSuspend();
      this.ClearFeedback();
    }

    private void UpdateZoomCursor()
    {
      this.Cursor = !this.IsAltDown || this.enableAreaZoom && this.IsDragging ? ToolCursors.ZoomInCursor : ToolCursors.ZoomOutCursor;
    }
  }
}

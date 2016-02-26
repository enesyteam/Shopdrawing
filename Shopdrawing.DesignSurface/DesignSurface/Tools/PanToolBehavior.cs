// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.PanToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class PanToolBehavior : ToolBehavior
  {
    public PanToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.ActiveView.CenterX += dragStartPosition.X - dragCurrentPosition.X;
      this.ActiveView.CenterY += dragStartPosition.Y - dragCurrentPosition.Y;
      return true;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.PanScene);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return this.AllDone();
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      return this.AllDone();
    }

    private bool AllDone()
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.PanScene);
      return true;
    }

    protected override void OnResume()
    {
      base.OnResume();
      this.Cursor = ToolCursors.PanCursor;
    }
  }
}

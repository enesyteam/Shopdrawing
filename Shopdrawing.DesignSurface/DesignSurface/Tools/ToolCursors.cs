// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ToolCursors
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public static class ToolCursors
  {
    private static Cursor centerPointCursor;
    private static Cursor convertCursor;
    private static Cursor convertPointCursor;
    private static Cursor convertSegmentCursor;
    private static Cursor convertTangentCursor;
    private static Cursor crosshairCursor;
    private static Cursor eyedropperCursor;
    private static Cursor minusArrowCursor;
    private static Cursor addArrowCursor;
    private static Cursor gridRowSelectCursor;
    private static Cursor gradientRedefineCursor;
    private static Cursor paintBucketCursor;
    private static Cursor panCursor;
    private static Cursor penCursor;
    private static Cursor orbitCursor;
    private static Cursor penAdjustCursor;
    private static Cursor penCloseCursor;
    private static Cursor penDeleteCursor;
    private static Cursor penExtendCursor;
    private static Cursor penInsertCursor;
    private static Cursor penJoinCursor;
    private static Cursor penStartCursor;
    private static Cursor penTangentCursor;
    private static Cursor pencilCursor;
    private static Cursor relocateCursor;
    private static DirectionalCursor resizeCursor;
    private static DirectionalCursor designTimeResizeCursor;
    private static DirectionalCursor rotateCursor;
    private static Cursor roundedRectangleCursor;
    private static Cursor selectionCursor;
    private static Cursor selectElementCursor;
    private static Cursor pickWhipCursor;
    private static Cursor propertyPickWhipCursor;
    private static DirectionalCursor skewCursor;
    private static Cursor subselectionCursor;
    private static Cursor subselectElementCursor;
    private static Cursor subselectMoveCursor;
    private static Cursor subselectPointCursor;
    private static Cursor subselectSegmentCursor;
    private static Cursor subselectTangentCursor;
    private static Cursor marqueeSelectCursor;
    private static Cursor zoomInCursor;
    private static Cursor zoomOutCursor;
    private static Cursor rotateXAxisCursor;
    private static Cursor rotateYAxisCursor;
    private static Cursor rotateZAxisCursor;
    private static Cursor translateXAxisCursor;
    private static Cursor translateYAxisCursor;
    private static Cursor translateZAxisCursor;
    private static Cursor duplicateWedgeCursor;
    private static Cursor noDropCursor;
    private static Cursor dataBindingDetailsCursor;
    private static Cursor dataBindingDetailsAddCursor;
    private static Cursor dataBindingMasterCursor;
    private static Cursor dataBindingMasterAddCursor;

    public static Cursor CenterPointCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_centerPoint.cur", ref ToolCursors.centerPointCursor);
      }
    }

    public static Cursor ConvertCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_convert.cur", ref ToolCursors.convertCursor);
      }
    }

    public static Cursor ConvertPointCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_convertPoint.cur", ref ToolCursors.convertPointCursor);
      }
    }

    public static Cursor ConvertSegmentCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_convertSegment.cur", ref ToolCursors.convertSegmentCursor);
      }
    }

    public static Cursor ConvertTangentCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_convertTangent.cur", ref ToolCursors.convertTangentCursor);
      }
    }

    public static Cursor CrosshairCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_crosshair.cur", ref ToolCursors.crosshairCursor);
      }
    }

    public static Cursor EyedropperCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_eyedropper.cur", ref ToolCursors.eyedropperCursor);
      }
    }

    public static Cursor MinusArrowCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_minusArrow.cur", ref ToolCursors.minusArrowCursor);
      }
    }

    public static Cursor AddArrowCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_AddArrow.cur", ref ToolCursors.addArrowCursor);
      }
    }

    public static Cursor GridRowSelectCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_gridRowSelect.cur", ref ToolCursors.gridRowSelectCursor);
      }
    }

    public static Cursor GradientRedefineCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_gradientRedefine.cur", ref ToolCursors.gradientRedefineCursor);
      }
    }

    public static Cursor PaintBucketCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_paintbucket.cur", ref ToolCursors.paintBucketCursor);
      }
    }

    public static Cursor PanCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_pan.cur", ref ToolCursors.panCursor);
      }
    }

    public static Cursor PenCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_pen.cur", ref ToolCursors.penCursor);
      }
    }

    public static Cursor OrbitCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_orbit.cur", ref ToolCursors.orbitCursor);
      }
    }

    public static Cursor PenAdjustCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penAdjust.cur", ref ToolCursors.penAdjustCursor);
      }
    }

    public static Cursor PenCloseCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penClose.cur", ref ToolCursors.penCloseCursor);
      }
    }

    public static Cursor PenDeleteCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penDelete.cur", ref ToolCursors.penDeleteCursor);
      }
    }

    public static Cursor PenExtendCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penExtent.cur", ref ToolCursors.penExtendCursor);
      }
    }

    public static Cursor PenInsertCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penInsert.cur", ref ToolCursors.penInsertCursor);
      }
    }

    public static Cursor PenJoinCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penJoin.cur", ref ToolCursors.penJoinCursor);
      }
    }

    public static Cursor PenStartCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penStart.cur", ref ToolCursors.penStartCursor);
      }
    }

    public static Cursor PenTangentCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_penTangent.cur", ref ToolCursors.penTangentCursor);
      }
    }

    public static Cursor PencilCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_pencil.cur", ref ToolCursors.pencilCursor);
      }
    }

    public static Cursor RelocateCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_relocate.cur", ref ToolCursors.relocateCursor);
      }
    }

    public static DirectionalCursor ResizeCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_resize{0}.cur", 4, 180.0, ref ToolCursors.resizeCursor);
      }
    }

    public static DirectionalCursor DesignTimeResizeCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_designTimeResize{0}.cur", 3, 180.0, ref ToolCursors.designTimeResizeCursor);
      }
    }

    public static DirectionalCursor RotateCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_rotate{0}.cur", 16, 360.0, ref ToolCursors.rotateCursor);
      }
    }

    public static Cursor RoundedRectangleCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_roundedRect.cur", ref ToolCursors.roundedRectangleCursor);
      }
    }

    public static Cursor SelectionCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_selection.cur", ref ToolCursors.selectionCursor);
      }
    }

    public static Cursor SelectElementCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_selectElement.cur", ref ToolCursors.selectElementCursor);
      }
    }

    public static Cursor PickWhipCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\pickwhip_cursor.cur", ref ToolCursors.pickWhipCursor);
      }
    }

    public static Cursor PropertyPickWhipCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_propertyPicker.cur", ref ToolCursors.propertyPickWhipCursor);
      }
    }

    public static DirectionalCursor SkewCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_skew{0}.cur", 8, 180.0, ref ToolCursors.skewCursor);
      }
    }

    public static Cursor SubselectionCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselection.cur", ref ToolCursors.subselectionCursor);
      }
    }

    public static Cursor SubselectElementCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselectElement.cur", ref ToolCursors.subselectElementCursor);
      }
    }

    public static Cursor SubselectMoveCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselectMove.cur", ref ToolCursors.subselectMoveCursor);
      }
    }

    public static Cursor SubselectPointCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselectPoint.cur", ref ToolCursors.subselectPointCursor);
      }
    }

    public static Cursor SubselectSegmentCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselectSegment.cur", ref ToolCursors.subselectSegmentCursor);
      }
    }

    public static Cursor SubselectTangentCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_subselectBezier.cur", ref ToolCursors.subselectTangentCursor);
      }
    }

    public static Cursor MarqueeSelectCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_marqueeSelect.cur", ref ToolCursors.marqueeSelectCursor);
      }
    }

    public static Cursor ZoomInCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_zoomIn.cur", ref ToolCursors.zoomInCursor);
      }
    }

    public static Cursor ZoomOutCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_zoomOut.cur", ref ToolCursors.zoomOutCursor);
      }
    }

    public static Cursor RotateXAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_xRotate.cur", ref ToolCursors.rotateXAxisCursor);
      }
    }

    public static Cursor RotateYAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_yRotate.cur", ref ToolCursors.rotateYAxisCursor);
      }
    }

    public static Cursor RotateZAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_zRotate.cur", ref ToolCursors.rotateZAxisCursor);
      }
    }

    public static Cursor TranslateXAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_xTranslate.cur", ref ToolCursors.translateXAxisCursor);
      }
    }

    public static Cursor TranslateYAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_yTranslate.cur", ref ToolCursors.translateYAxisCursor);
      }
    }

    public static Cursor TranslateZAxisCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_zTranslate.cur", ref ToolCursors.translateZAxisCursor);
      }
    }

    public static Cursor DuplicateWedgeCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_dupwedge.cur", ref ToolCursors.duplicateWedgeCursor);
      }
    }

    public static Cursor NoDropCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_noDrop.cur", ref ToolCursors.noDropCursor);
      }
    }

    public static Cursor DataBindingDetailsCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_dataBinding_detail.cur", ref ToolCursors.dataBindingDetailsCursor);
      }
    }

    public static Cursor DataBindingDetailsAddCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_dataBinding_detail_add.cur", ref ToolCursors.dataBindingDetailsAddCursor);
      }
    }

    public static Cursor DataBindingMasterCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_dataBinding_master.cur", ref ToolCursors.dataBindingMasterCursor);
      }
    }

    public static Cursor DataBindingMasterAddCursor
    {
      get
      {
        return ToolCursors.GetCachedCursor("Resources\\Cursors\\cursor_dataBinding_master_add.cur", ref ToolCursors.dataBindingMasterAddCursor);
      }
    }

    private static Cursor GetCachedCursor(string resourceName, ref Cursor cursorCache)
    {
      if (cursorCache == null)
        cursorCache = FileTable.GetCursor(resourceName);
      return cursorCache;
    }

    private static DirectionalCursor GetCachedCursor(string resourceFormatString, int count, double repeatAngle, ref DirectionalCursor cursorCache)
    {
      if (cursorCache == null)
        cursorCache = new DirectionalCursor(resourceFormatString, count, repeatAngle);
      return cursorCache;
    }
  }
}

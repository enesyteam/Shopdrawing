// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.AppendAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class AppendAction : TangentUpdateAction
  {
    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitAppendSegment;
      }
    }

    public override Cursor HoverCursor
    {
      get
      {
        return ToolCursors.PenCursor;
      }
    }

    public AppendAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
      : base(pathEditorTarget, viewModel)
    {
    }

    protected override void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditPath);
      Vector lastTangent = this.LastTangent;
      base.OnBegin(pathEditContext, mouseDevice);
      PathGeometryEditor pathGeometryEditor = this.BeginEditing();
      Matrix elementTransformToRoot = this.EditingElementTransformToRoot;
      Point lastPoint = this.GetLastPoint(pathEditContext.FigureIndex);
      Point point1 = elementTransformToRoot.Transform(lastPoint);
      Vector vector = this.GetPointInViewRootCoordinates(mouseDevice, true) - point1;
      Size devicePixelSize = DeviceUtilities.GetDevicePixelSize(this.View.Zoom);
      if (Math.Abs(vector.X) < devicePixelSize.Width / 2.0)
        vector.X = 0.0;
      if (Math.Abs(vector.Y) < devicePixelSize.Height / 2.0)
        vector.Y = 0.0;
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(vector, elementTransformToRoot, this.IsShiftDown ? this.AxisConstraint : (AxisConstraint) null);
      Point point2 = lastPoint + correspondingVector;
      if (VectorUtilities.IsZero(lastTangent))
      {
        pathGeometryEditor.AppendLineSegment(point2, pathEditContext.FigureIndex);
      }
      else
      {
        Point q = lastPoint + lastTangent;
        pathGeometryEditor.AppendCubicBezier(q, point2, point2, pathEditContext.FigureIndex);
      }
      int figureIndex = pathEditContext.FigureIndex;
      PathFigure figure = this.Path.Figures[figureIndex];
      this.PathEditContext = new PathEditContext(figureIndex, PathFigureUtilities.PointCount(figure) - 1);
      this.Initialize(elementTransformToRoot.Transform(point2), true, elementTransformToRoot);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.LineCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class LineCreateBehavior : ElementDragCreateBehavior
  {
    private const double SnapAngle = 0.261799387799149;

    protected override ITypeId InstanceType
    {
      get
      {
        return PlatformTypes.Path;
      }
    }

    internal LineCreateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext, false)
    {
    }

    protected override BaseFrameworkElement CreateElementOnStartDrag()
    {
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) PathCreateBehavior.CreatePathElement(this.ActiveSceneViewModel, this.ActiveSceneInsertionPoint, this.ToolBehaviorContext);
      this.UpdateEditTransaction();
      return frameworkElement;
    }

    protected override void DoUpdateElementPosition(Point pointBegin, Point pointEnd)
    {
      PathGeometry path = new PathGeometry();
      PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(path);
      pathGeometryEditor.StartFigure(pointBegin);
      pathGeometryEditor.AppendLineSegment(pointEnd);
      using (ScenePathEditorTarget pathEditorTarget = new ScenePathEditorTarget((PathElement) this.EditingElement))
      {
        pathEditorTarget.BeginEditing();
        pathEditorTarget.PathGeometry = path;
        pathEditorTarget.EndEditing(true);
      }
    }

    protected override void ConstrainElementPosition(Point pointBegin, ref Point pointEnd, ref Vector diagonal)
    {
      double length = diagonal.Length;
      if (length <= 0.0)
        return;
      double num = Math.Round(Math.Atan2(diagonal.Y, diagonal.X) / (Math.PI / 12.0)) * (Math.PI / 12.0);
      diagonal.X = length * Math.Cos(num);
      diagonal.Y = length * Math.Sin(num);
      pointEnd = pointBegin + diagonal;
    }
  }
}

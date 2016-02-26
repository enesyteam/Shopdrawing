// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PenAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PenAdornerSet : PathAdornerSet
  {
    private int activePointAdornerIndex = -1;
    private int activeSegmentAdornerIndex = -1;
    private PenTangentAdorner lastTangentAdorner;

    public bool IsActive
    {
      get
      {
        PenTool penTool = this.ToolContext.Tool as PenTool;
        if (penTool != null && penTool.ActivePathEditInformation != null)
          return this.PathEditorTarget == penTool.ActivePathEditInformation.ActivePathEditorTarget;
        return false;
      }
    }

    public int ActiveFigureIndex
    {
      get
      {
        PenTool penTool = this.ToolContext.Tool as PenTool;
        if (penTool.ActivePathEditInformation == null)
          return -1;
        return penTool.ActivePathEditInformation.ActiveFigureIndex;
      }
    }

    public PathFigure ActiveFigure
    {
      get
      {
        if (!this.IsActive)
          throw new InvalidOperationException(ExceptionStringTable.PenAdornerSetPathNotActive);
        return this.PathGeometry.Figures[this.ActiveFigureIndex];
      }
    }

    public Point LastPoint
    {
      get
      {
        return this.lastTangentAdorner.StartPoint;
      }
    }

    public Vector LastTangent
    {
      get
      {
        return PointVectorConverter.FromPoint(this.Element.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.LastTangentProperty));
      }
      set
      {
        this.Element.SetLocalValueAsWpf(DesignTimeProperties.LastTangentProperty, (object) PointVectorConverter.ToPoint(value));
      }
    }

    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) null;
      }
    }

    protected override bool NeedsRebuild
    {
      get
      {
        bool flag = base.NeedsRebuild;
        if (this.IsActive && this.PathGeometry != null && (!PathGeometryUtilities.IsEmpty(this.PathGeometry) && this.lastTangentAdorner == null))
        {
          if (this.ActiveFigureIndex < 0 || this.ActiveFigureIndex >= this.PathGeometry.Figures.Count || PathFigureUtilities.IsOpen(this.PathGeometry.Figures[this.ActiveFigureIndex]))
            flag = true;
        }
        else if (!this.IsActive && this.lastTangentAdorner != null)
        {
          this.PathPartSelectionSet.RemoveSelection(this.PathPartSelectionSet.GetSelectionByElement((SceneElement) this.Element, this.PathEditorTarget.PathEditMode));
          flag = true;
        }
        return flag;
      }
    }

    public PenAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement, PathEditorTarget pathEditorTarget)
      : base(toolContext, adornedElement, pathEditorTarget)
    {
    }

    public override void UpdateActiveStateFromSelection()
    {
      if (this.IsActive)
      {
        if (this.AdornerList == null)
          return;
        if (this.activePointAdornerIndex >= 0 && this.activePointAdornerIndex < this.AdornerList.Count)
        {
          PathPointAdorner pathPointAdorner = this.AdornerList[this.activePointAdornerIndex] as PathPointAdorner;
          if (pathPointAdorner != null)
            pathPointAdorner.IsActive = true;
        }
        if (this.activeSegmentAdornerIndex >= 0 && this.activeSegmentAdornerIndex < this.AdornerList.Count)
        {
          PathSegmentAdorner pathSegmentAdorner = this.AdornerList[this.activeSegmentAdornerIndex] as PathSegmentAdorner;
          if (pathSegmentAdorner != null)
            pathSegmentAdorner.IsActive = true;
        }
        this.InvalidateRender();
      }
      else
        base.UpdateActiveStateFromSelection();
    }

    protected override void CreatePathAdorners(List<Adorner> oldAdornerList, List<Adorner> newAdornerList)
    {
      base.CreatePathAdorners(oldAdornerList, newAdornerList);
      this.lastTangentAdorner = (PenTangentAdorner) null;
      this.activeSegmentAdornerIndex = -1;
      this.activePointAdornerIndex = -1;
      if (!this.IsActive || this.AdornerList.Count <= 0 || (this.PathGeometry == null || this.PathGeometry.Figures.Count <= 0))
        return;
      int index1 = this.ActiveFigureIndex;
      if (index1 < 0 || index1 >= this.PathGeometry.Figures.Count)
        index1 = 0;
      PathFigure figure = this.PathGeometry.Figures[index1];
      int num = !PathFigureUtilities.IsClosed(figure) ? PathFigureUtilities.PointCount(figure) - 1 : 0;
      PathPointAdorner pathPointAdorner1 = (PathPointAdorner) null;
      for (int index2 = 0; index2 < this.AdornerList.Count; ++index2)
      {
        Adorner adorner = this.AdornerList[index2];
        PathPointAdorner pathPointAdorner2 = adorner as PathPointAdorner;
        if (pathPointAdorner2 != null)
        {
          if (pathPointAdorner2.FigureIndex == this.ActiveFigureIndex && pathPointAdorner2.PointIndex == num)
          {
            pathPointAdorner2.IsActive = true;
            pathPointAdorner1 = pathPointAdorner2;
            this.activePointAdornerIndex = index2;
          }
          else
            pathPointAdorner2.IsActive = false;
        }
        PathSegmentAdorner pathSegmentAdorner = adorner as PathSegmentAdorner;
        if (pathSegmentAdorner != null)
        {
          if (pathSegmentAdorner.FigureIndex == this.ActiveFigureIndex && pathSegmentAdorner.LastPointIndex == num)
          {
            pathSegmentAdorner.IsActive = true;
            this.activeSegmentAdornerIndex = index2;
          }
          else
            pathSegmentAdorner.IsActive = false;
        }
      }
      if (pathPointAdorner1 == null || PathFigureUtilities.IsClosed(this.PathGeometry.Figures[this.ActiveFigureIndex]))
        return;
      this.lastTangentAdorner = new PenTangentAdorner(pathPointAdorner1);
      newAdornerList.Add((Adorner) this.lastTangentAdorner);
    }
  }
}

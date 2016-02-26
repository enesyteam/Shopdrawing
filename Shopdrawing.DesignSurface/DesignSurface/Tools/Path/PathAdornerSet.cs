// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class PathAdornerSet : AdornerSet, IDisposable
  {
    private int criticalEditsCache = -1;
    private ArrayList adornerOffsets = new ArrayList();
    private ArrayList adornerCounts = new ArrayList();
    private ArrayList adornerTypes = new ArrayList();
    private bool[] isClosed;
    private PathPartSelectionSet pathPartSelectionSet;
    private PathEditorTarget pathEditorTarget;
    private bool renderHighlight;
    private List<Adorner> oldAdornerList;

    public BaseFrameworkElement Element
    {
      get
      {
        return (BaseFrameworkElement) base.Element;
      }
    }

    public override ToolBehavior Behavior
    {
      get
      {
        return (ToolBehavior) new PathEditBehavior(this.ToolContext, this.pathEditorTarget);
      }
    }

    public PathEditorTarget PathEditorTarget
    {
      get
      {
        return this.pathEditorTarget;
      }
    }

    public PathPartSelectionSet PathPartSelectionSet
    {
      get
      {
        if (this.pathPartSelectionSet == null)
          this.pathPartSelectionSet = this.ViewModel.PathPartSelectionSet;
        return this.pathPartSelectionSet;
      }
    }

    public bool RenderHighlight
    {
      get
      {
        return this.renderHighlight;
      }
      set
      {
        if (this.renderHighlight == value)
          return;
        this.renderHighlight = value;
        this.InvalidateRender();
      }
    }

    public int HighlightFigureIndex { get; set; }

    public int HighlightSegmentIndex { get; set; }

    public int HighlightSegmentPointIndex { get; set; }

    internal PathGeometry PathGeometry
    {
      get
      {
        this.pathEditorTarget.UpdatePathIfNeeded();
        return this.pathEditorTarget.PathGeometry;
      }
    }

    public PathAdornerSet(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement, PathEditorTarget pathEditorTarget)
      : base(toolContext, (SceneElement) adornedElement)
    {
      this.pathEditorTarget = pathEditorTarget;
      this.pathEditorTarget.MatrixChanged += new EventHandler(this.PathEditorTarget_MatrixChanged);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.pathEditorTarget.MatrixChanged -= new EventHandler(this.PathEditorTarget_MatrixChanged);
    }

    private void PathEditorTarget_MatrixChanged(object sender, EventArgs e)
    {
      if (this.Parent == null || this.Element.ParentElement.Visual == null)
        return;
      this.SetMatrix(this.GetTransformMatrix(this.Element.ParentElement.Visual));
      this.pathEditorTarget.UpdatePathIfNeeded();
      this.InvalidateRender();
    }

    public void SetActive(PathPartAdorner pathPartAdorner, bool isActive)
    {
      if (pathPartAdorner.IsActive == isActive)
        return;
      pathPartAdorner.IsActive = isActive;
      this.InvalidateRender();
    }

    public void DeactivateAll()
    {
      foreach (Adorner adorner in this.AdornerList)
      {
        PathPartAdorner pathPartAdorner = adorner as PathPartAdorner;
        if (pathPartAdorner != null)
          pathPartAdorner.IsActive = false;
      }
      this.InvalidateRender();
    }

    public void HighlightProperty(PropertyReference propertyReference)
    {
      if (propertyReference.Count < 4 || !propertyReference[1].Equals((object) PathElement.FiguresProperty))
        return;
      IndexedClrPropertyReferenceStep propertyReferenceStep1 = propertyReference[2] as IndexedClrPropertyReferenceStep;
      if (propertyReferenceStep1 == null)
        return;
      this.HighlightFigureIndex = propertyReferenceStep1.Index;
      if (propertyReference.Count == 4 && propertyReference[3].Equals((object) PathElement.PathFigureStartPointProperty))
      {
        this.HighlightSegmentPointIndex = 0;
        this.HighlightSegmentIndex = -1;
        this.RenderHighlight = true;
      }
      else
      {
        if (propertyReference.Count != 6 || !propertyReference[3].Equals((object) PathElement.PathFigureSegmentsProperty))
          return;
        IndexedClrPropertyReferenceStep propertyReferenceStep2 = propertyReference[4] as IndexedClrPropertyReferenceStep;
        if (propertyReferenceStep2 == null)
          return;
        this.HighlightSegmentIndex = propertyReferenceStep2.Index;
        this.HighlightSegmentPointIndex = PathElement.GetPointIndexFromPointProperty((IPropertyId) propertyReference[5]);
        this.RenderHighlight = true;
      }
    }

    public void ClearHighlight()
    {
      this.RenderHighlight = false;
    }

    public virtual void UpdateActiveStateFromSelection()
    {
      if (this.AdornerList == null)
        return;
      Hashtable hashtable = new Hashtable();
      foreach (Adorner adorner in this.AdornerList)
      {
        PathPointAdorner pathPointAdorner = adorner as PathPointAdorner;
        PathSegmentAdorner pathSegmentAdorner = adorner as PathSegmentAdorner;
        if (pathPointAdorner != null)
        {
          hashtable[(object) (PathPoint) pathPointAdorner] = (object) pathPointAdorner;
          pathPointAdorner.IsActive = false;
        }
        else if (pathSegmentAdorner != null)
        {
          hashtable[(object) (PathSegment) pathSegmentAdorner] = (object) pathSegmentAdorner;
          pathSegmentAdorner.IsActive = false;
        }
      }
      foreach (PathPart pathPart in (IEnumerable<PathPart>) this.PathPartSelectionSet.GetSelectionByElement((SceneElement) this.Element))
      {
        PathPartAdorner pathPartAdorner = (PathPartAdorner) hashtable[(object) pathPart];
        if (pathPartAdorner != null)
          pathPartAdorner.IsActive = true;
      }
      this.InvalidateRender();
    }

    public override Matrix GetTransformMatrix(IViewObject targetViewObject)
    {
      return this.pathEditorTarget.GetTransformToAncestor(targetViewObject);
    }

    public override Matrix GetTransformMatrixToAdornerLayer()
    {
      return this.pathEditorTarget.RefineTransformToAdornerLayer(base.GetTransformMatrixToAdornerLayer());
    }

    public override Cursor GetCursor(IAdorner adorner)
    {
      if (adorner is PathPointAdorner)
        return ToolCursors.SubselectPointCursor;
      if (adorner is PathSegmentAdorner)
        return ToolCursors.SubselectSegmentCursor;
      if (adorner is PathTangentAdorner)
        return ToolCursors.SubselectTangentCursor;
      return ToolCursors.SubselectionCursor;
    }

    protected override void UpdateChildrenVisuals()
    {
      this.oldAdornerList = this.AdornerList;
      if (this.oldAdornerList == null)
        this.oldAdornerList = new List<Adorner>();
      this.AdornerList = new List<Adorner>();
      this.CreateAdorners();
      foreach (Visual visual in this.oldAdornerList)
      {
        if (visual != null)
          this.Children.Remove(visual);
      }
      int index1 = -1;
      int index2 = -1;
      int index3 = -1;
      for (int index4 = 0; index4 < this.Children.Count; ++index4)
      {
        if (this.Children[index4] is PathSegmentAdorner)
          index1 = index4 + 1;
        else if (this.Children[index4] is PathTangentAdorner || this.Children[index4] is PenTangentAdorner)
          index2 = index4 + 1;
        else if (this.Children[index4] is PathPointAdorner)
          index3 = index4 + 1;
      }
      if (index1 == -1)
        index1 = 0;
      if (index2 == -1)
        index2 = index1;
      if (index3 == -1)
        index3 = index2;
      foreach (Adorner adorner in this.AdornerList)
      {
        Visual visual = (Visual) adorner;
        if (visual != null && !this.Children.Contains(visual))
        {
          if (adorner is PathSegmentAdorner)
          {
            this.Children.Insert(index1, visual);
            ++index1;
            ++index2;
            ++index3;
          }
          else if (adorner is PathTangentAdorner || adorner is PenTangentAdorner)
          {
            this.Children.Insert(index2, visual);
            ++index2;
            ++index3;
          }
          else if (adorner is PathPointAdorner)
          {
            this.Children.Insert(index3, visual);
            ++index3;
          }
          else
            this.Children.Add(visual);
        }
      }
    }

    protected override sealed void CreateAdorners()
    {
      this.criticalEditsCache = this.pathEditorTarget.CriticalEdits;
      this.UpdateIsClosed();
      this.CreatePathAdorners(this.oldAdornerList, this.AdornerList);
    }

    protected override void OnRedrawing()
    {
      this.NeedsRebuild = this.criticalEditsCache != this.pathEditorTarget.CriticalEdits;
      this.criticalEditsCache = this.pathEditorTarget.CriticalEdits;
    }

    protected virtual void CreatePathAdorners(List<Adorner> oldAdornerList, List<Adorner> newAdornerList)
    {
      this.adornerCounts.Clear();
      this.adornerOffsets.Clear();
      PathPartSelectionSet partSelectionSet = this.PathPartSelectionSet;
      List<PathPointAdorner> list1 = new List<PathPointAdorner>();
      List<PathSegmentAdorner> list2 = new List<PathSegmentAdorner>();
      List<PathTangentAdorner> list3 = new List<PathTangentAdorner>();
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      foreach (Adorner adorner in oldAdornerList)
      {
        PathPointAdorner pathPointAdorner;
        if ((pathPointAdorner = adorner as PathPointAdorner) != null)
        {
          list1.Add(pathPointAdorner);
        }
        else
        {
          PathSegmentAdorner pathSegmentAdorner;
          if ((pathSegmentAdorner = adorner as PathSegmentAdorner) != null)
          {
            list2.Add(pathSegmentAdorner);
          }
          else
          {
            PathTangentAdorner pathTangentAdorner;
            if ((pathTangentAdorner = adorner as PathTangentAdorner) != null)
              list3.Add(pathTangentAdorner);
          }
        }
      }
      for (int figureIndex = 0; figureIndex < this.PathGeometry.Figures.Count; ++figureIndex)
      {
        PathFigure figure = this.PathGeometry.Figures[figureIndex];
        PathFigureEditor pathFigureEditor = new PathFigureEditor(figure);
        this.adornerOffsets.Add((object) this.AdornerList.Count);
        int length1 = PathFigureUtilities.PointCount(figure);
        int length2 = length1 + (PathFigureUtilities.IsClosed(figure) ? true : false);
        PathSegmentAdorner[] pathSegmentAdornerArray = new PathSegmentAdorner[length2];
        PathPointKind[] pathPointKindArray = new PathPointKind[length2];
        pathPointKindArray[0] = pathFigureEditor.GetPointKind(0);
        int num1 = 1;
        for (int index4 = 0; index4 < figure.Segments.Count; ++index4)
        {
          System.Windows.Media.PathSegment segment = figure.Segments[index4];
          int pointCount = PathSegmentUtilities.GetPointCount(segment);
          for (int index5 = 0; index5 < pointCount; ++index5)
            pathPointKindArray[num1++] = PathSegmentUtilities.GetPointKind(segment, index5);
        }
        int index6 = 1;
        for (int segmentIndex = 0; segmentIndex < figure.Segments.Count; ++segmentIndex)
        {
          int pointCount = PathSegmentUtilities.GetPointCount(figure.Segments[segmentIndex]);
          for (int segmentPointIndex = 0; segmentPointIndex < pointCount; ++segmentPointIndex)
          {
            if (pathPointKindArray[index6] != PathPointKind.BezierHandle)
            {
              PathSegmentAdorner pathSegmentAdorner;
              if (index2 < list2.Count)
              {
                pathSegmentAdorner = list2[index2];
                pathSegmentAdorner.Initialize(figureIndex, index6 % length1, segmentIndex, segmentPointIndex);
                oldAdornerList.Remove((Adorner) pathSegmentAdorner);
                ++index2;
              }
              else
                pathSegmentAdorner = new PathSegmentAdorner(this, figureIndex, index6 % length1, segmentIndex, segmentPointIndex);
              if (partSelectionSet != null)
                pathSegmentAdorner.IsActive = partSelectionSet.IsSelected((PathPart) (PathSegment) pathSegmentAdorner);
              newAdornerList.Add((Adorner) pathSegmentAdorner);
              pathSegmentAdornerArray[index6] = pathSegmentAdorner;
            }
            else
              pathSegmentAdornerArray[index6] = (PathSegmentAdorner) null;
            ++index6;
          }
        }
        if (figure.IsClosed && figure.Segments.Count > 0)
        {
          Point lastPoint = PathSegmentUtilities.GetLastPoint(figure.Segments[figure.Segments.Count - 1]);
          if (!VectorUtilities.ArePathPointsVeryClose(figure.StartPoint, lastPoint))
          {
            PathSegmentAdorner pathSegmentAdorner;
            if (index2 < list2.Count)
            {
              pathSegmentAdorner = list2[index2];
              pathSegmentAdorner.Initialize(figureIndex, index6 % length1, -1, 0);
              oldAdornerList.Remove((Adorner) pathSegmentAdorner);
              ++index2;
            }
            else
              pathSegmentAdorner = new PathSegmentAdorner(this, figureIndex, index6 % length1, -1, 0);
            if (partSelectionSet != null)
              pathSegmentAdorner.IsActive = partSelectionSet.IsSelected((PathPart) (PathSegment) pathSegmentAdorner);
            newAdornerList.Add((Adorner) pathSegmentAdorner);
            pathSegmentAdornerArray[index6] = pathSegmentAdorner;
          }
        }
        this.adornerCounts.Add((object) (this.AdornerList.Count - (int) this.adornerOffsets[this.adornerOffsets.Count - 1]));
        this.adornerTypes.Add((object) PathPart.PartType.PathSegment);
        PathPointAdorner[] pathPointAdornerArray = new PathPointAdorner[length1];
        int index7 = 0;
        for (int segmentIndex = -1; segmentIndex < figure.Segments.Count; ++segmentIndex)
        {
          int num2 = 1;
          if (segmentIndex != -1)
            num2 = PathSegmentUtilities.GetPointCount(figure.Segments[segmentIndex]);
          for (int segmentPointIndex = 0; segmentPointIndex < num2 && index7 < length1; ++segmentPointIndex)
          {
            if (pathPointKindArray[index7] == PathPointKind.BezierHandle)
            {
              pathPointAdornerArray[index7] = (PathPointAdorner) null;
            }
            else
            {
              PathPointAdorner pathPointAdorner;
              if (index1 < list1.Count)
              {
                pathPointAdorner = list1[index1];
                pathPointAdorner.Initialize(figureIndex, index7, segmentIndex, segmentPointIndex);
                oldAdornerList.Remove((Adorner) pathPointAdorner);
                ++index1;
              }
              else
                pathPointAdorner = new PathPointAdorner(this, figureIndex, index7, segmentIndex, segmentPointIndex);
              pathPointAdornerArray[index7] = pathPointAdorner;
              if (partSelectionSet != null)
                pathPointAdornerArray[index7].IsActive = partSelectionSet.IsSelected((PathPart) (PathPoint) pathPointAdornerArray[index7]);
            }
            ++index7;
          }
        }
        int endPointIndex = 0;
        for (int segmentIndex = -1; segmentIndex < figure.Segments.Count; ++segmentIndex)
        {
          int num2 = 1;
          if (segmentIndex != -1)
            num2 = PathSegmentUtilities.GetPointCount(figure.Segments[segmentIndex]);
          for (int segmentPointIndex = 0; segmentPointIndex < num2 && endPointIndex < length1; ++segmentPointIndex)
          {
            if (pathPointKindArray[endPointIndex] == PathPointKind.BezierHandle)
            {
              PathPointAdorner pathPointAdorner = (PathPointAdorner) null;
              PathSegmentAdorner pathSegmentAdorner = (PathSegmentAdorner) null;
              if (pathPointKindArray[endPointIndex] == PathPointKind.BezierHandle && endPointIndex + 2 < pathPointKindArray.Length && pathPointKindArray[endPointIndex + 2] == PathPointKind.Cubic)
              {
                pathPointAdorner = pathPointAdornerArray[endPointIndex - 1];
                pathSegmentAdorner = pathSegmentAdornerArray[endPointIndex + 2];
              }
              else if (pathPointKindArray[endPointIndex] == PathPointKind.BezierHandle && endPointIndex + 1 < pathPointKindArray.Length && pathPointKindArray[endPointIndex + 1] == PathPointKind.Cubic)
              {
                pathPointAdorner = pathPointAdornerArray[(endPointIndex + 1) % length1];
                pathSegmentAdorner = pathSegmentAdornerArray[endPointIndex + 1];
              }
              if (pathPointAdorner != null)
              {
                PathTangentAdorner pathTangentAdorner;
                if (index3 < list3.Count)
                {
                  pathTangentAdorner = list3[index3];
                  pathTangentAdorner.Initialize(figureIndex, endPointIndex, segmentIndex, segmentPointIndex, pathPointAdorner, pathSegmentAdorner);
                  oldAdornerList.Remove((Adorner) pathTangentAdorner);
                  ++index3;
                }
                else
                  pathTangentAdorner = new PathTangentAdorner(this, figureIndex, endPointIndex, segmentIndex, segmentPointIndex, pathPointAdorner, pathSegmentAdorner);
                newAdornerList.Add((Adorner) pathTangentAdorner);
              }
            }
            ++endPointIndex;
          }
        }
        foreach (PathPointAdorner pathPointAdorner in pathPointAdornerArray)
        {
          if (pathPointAdorner != null)
            newAdornerList.Add((Adorner) pathPointAdorner);
        }
        this.adornerOffsets.Add((object) this.AdornerList.Count);
        this.adornerCounts.Add((object) (this.AdornerList.Count - (int) this.adornerOffsets[this.adornerOffsets.Count - 1]));
        this.adornerTypes.Add((object) PathPart.PartType.PathPoint);
      }
    }

    private void UpdateIsClosed()
    {
      this.isClosed = new bool[this.PathGeometry.Figures.Count];
      for (int index = 0; index < this.isClosed.Length; ++index)
        this.isClosed[index] = this.PathGeometry.Figures[index].IsClosed;
    }
  }
}

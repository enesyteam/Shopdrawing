// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.DeleteCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class DeleteCommand : SceneCommandBase
  {
    private DesignerContext designerContext;

    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return this.SceneViewModel.CanDeleteSelection;
        return false;
      }
    }

    public DeleteCommand(SceneViewModel viewModel, DesignerContext designerContext)
      : base(viewModel)
    {
      this.designerContext = designerContext;
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitDelete))
      {
        ToolManager toolManager = this.designerContext.ToolManager;
        if (!this.SceneViewModel.StoryboardSelectionSet.IsEmpty)
          this.StoryboardDelete();
        else if (!this.SceneViewModel.AnimationSelectionSet.IsEmpty && this.SceneViewModel.PathPartSelectionSet.IsEmpty)
          this.AnimationDelete();
        else if (!this.SceneViewModel.KeyFrameSelectionSet.IsEmpty)
          this.KeyFrameDelete();
        else if (!this.SceneViewModel.SetterSelectionSet.IsEmpty)
          this.SetterDelete();
        else if (this.SceneViewModel.TextSelectionSet.IsActive)
          this.TextDelete();
        else if (!this.SceneViewModel.ChildPropertySelectionSet.IsEmpty)
          this.ChildPropertyDelete();
        else if (!this.SceneViewModel.BehaviorSelectionSet.IsEmpty)
          this.BehaviorDelete();
        else if (!this.SceneViewModel.AnnotationSelectionSet.IsEmpty)
          this.designerContext.AnnotationService.Delete((IEnumerable<AnnotationSceneNode>) this.SceneViewModel.AnnotationSelectionSet.Selection);
        else if (this.SceneViewModel.GridColumnSelectionSet.GridLineMode && !this.SceneViewModel.GridColumnSelectionSet.IsEmpty)
          this.GridColumnDelete();
        else if (this.SceneViewModel.GridRowSelectionSet.GridLineMode && !this.SceneViewModel.GridRowSelectionSet.IsEmpty)
          this.GridRowDelete();
        else if (toolManager.ActiveTool == toolManager.FindTool(typeof (SubselectionTool)) && !this.SceneViewModel.PathPartSelectionSet.IsEmpty)
          this.DeleteLeftoverElements(this.PathPartDelete(editTransaction));
        else if (toolManager.ActiveTool == toolManager.FindTool(typeof (PenTool)))
        {
          PenCreateBehavior penCreateBehavior = this.designerContext.ActiveView.EventRouter.ActiveBehavior as PenCreateBehavior;
          if (penCreateBehavior != null && ((PenTool) penCreateBehavior.Tool).ActivePathEditInformation != null)
            penCreateBehavior.DeleteLastSegment();
          else if (!this.SceneViewModel.PathPartSelectionSet.IsEmpty)
            this.DeleteLeftoverElements(this.PathPartDelete(editTransaction));
          else
            this.SceneViewModel.DeleteSelectedElements();
        }
        else
          this.SceneViewModel.DeleteSelectedElements();
        editTransaction.Commit();
      }
    }

    private void ChildPropertyDelete()
    {
      foreach (SceneNode child in this.SceneViewModel.ChildPropertySelectionSet.Selection)
      {
        SceneNode parent = child.Parent;
        PropertyReference propertyReference = new PropertyReference((ReferenceStep) parent.GetPropertyForChild(child));
        child.ViewModel.AnimationEditor.DeleteAllAnimations(parent, propertyReference.ToString());
        child.Remove();
      }
    }

    private void BehaviorDelete()
    {
      foreach (BehaviorBaseNode node in this.SceneViewModel.BehaviorSelectionSet.Selection)
        BehaviorHelper.DeleteBehavior(node);
    }

    private HashSet<SceneElement> PathPartDelete(SceneEditTransaction editTransaction)
    {
      HashSet<SceneElement> hashSet = new HashSet<SceneElement>();
      PathPartSelectionSet partSelectionSet = this.SceneViewModel.PathPartSelectionSet;
      Dictionary<PathEditMode, List<PathEditorTarget>> dictionary = new Dictionary<PathEditMode, List<PathEditorTarget>>();
      dictionary[PathEditMode.ClippingPath] = new List<PathEditorTarget>();
      dictionary[PathEditMode.ScenePath] = new List<PathEditorTarget>();
      dictionary[PathEditMode.MotionPath] = new List<PathEditorTarget>();
      Tool activeTool = this.SceneViewModel.DesignerContext.ToolManager.ActiveTool;
      if (activeTool != null)
      {
        foreach (PathPart pathPart in partSelectionSet.Selection)
        {
          BaseFrameworkElement frameworkElement = pathPart.SceneElement as BaseFrameworkElement;
          if (frameworkElement != null)
          {
            PathEditorTarget pathEditorTarget = activeTool.GetPathEditorTarget((Base2DElement) frameworkElement, pathPart.PathEditMode);
            if (pathEditorTarget != null && !dictionary[pathPart.PathEditMode].Contains(pathEditorTarget))
            {
              dictionary[pathPart.PathEditMode].Add(pathEditorTarget);
              hashSet.Add((SceneElement) frameworkElement);
            }
          }
        }
      }
      List<PathEditorTarget> list1 = new List<PathEditorTarget>();
      List<PathEditorTarget> list2 = new List<PathEditorTarget>();
      foreach (KeyValuePair<PathEditMode, List<PathEditorTarget>> keyValuePair in dictionary)
      {
        foreach (PathEditorTarget pathEditorTarget in keyValuePair.Value)
        {
          pathEditorTarget.BeginEditing();
          int count1 = pathEditorTarget.PathGeometry.Figures.Count;
          ICollection<PathPart> selectionByElement = partSelectionSet.GetSelectionByElement((SceneElement) pathEditorTarget.EditingElement, keyValuePair.Key);
          new DeleteCommand.SelectedComponentsRemover(pathEditorTarget).Execute(selectionByElement);
          pathEditorTarget.EndEditing(false);
          pathEditorTarget.PostDeleteAction();
          if (keyValuePair.Key == PathEditMode.ScenePath)
          {
            if (!PathGeometryUtilities.IsEmpty(pathEditorTarget.PathGeometry))
            {
              PathElement pathElement = (PathElement) pathEditorTarget.EditingElement;
              int count2 = pathElement.PathGeometry.Figures.Count;
              if (count1 == 1 && count2 > 1)
              {
                ISceneNodeCollection<SceneNode> collectionContainer = pathElement.GetCollectionContainer();
                if (!collectionContainer.FixedCapacity.HasValue || collectionContainer.FixedCapacity.Value >= collectionContainer.Count - 1 + count2)
                  list2.Add(pathEditorTarget);
              }
            }
            else
              list1.Add(pathEditorTarget);
          }
          pathEditorTarget.AddCriticalEdit();
        }
      }
      List<SceneElement> list3 = new List<SceneElement>();
      foreach (PathEditorTarget pathEditorTarget in list2)
      {
        PathElement pathElement1 = (PathElement) pathEditorTarget.EditingElement;
        using (this.SceneView.AdornerLayer.SuspendUpdates())
        {
          this.SceneView.UpdateLayout();
          foreach (PathElement pathElement2 in PathCommandHelper.ReleaseCompoundPaths(pathElement1, editTransaction))
            list3.Add((SceneElement) pathElement2);
        }
      }
      foreach (PathEditorTarget pathEditorTarget in list1)
        pathEditorTarget.RemovePath();
      partSelectionSet.Clear();
      this.SceneViewModel.ElementSelectionSet.ExtendSelection((ICollection<SceneElement>) list3);
      foreach (SceneElement sceneElement in list3)
        hashSet.Add(sceneElement);
      return hashSet;
    }

    private void DeleteLeftoverElements(HashSet<SceneElement> elementsWithDeletedParts)
    {
      List<SceneElement> elements = new List<SceneElement>();
      foreach (SceneElement sceneElement in this.SceneViewModel.ElementSelectionSet.Selection)
      {
        if (!elementsWithDeletedParts.Contains(sceneElement))
          elements.Add(sceneElement);
      }
      ElementUtilities.SortElementsByDepth(elements);
      foreach (SceneElement sceneElement in elements)
      {
        this.SceneViewModel.ElementSelectionSet.RemoveSelection(sceneElement);
        this.SceneViewModel.DeleteElementTree(sceneElement);
      }
    }

    private void StoryboardDelete()
    {
      foreach (StoryboardTimelineSceneNode timeline in this.SceneViewModel.StoryboardSelectionSet.Selection)
        this.SceneViewModel.AnimationEditor.DeleteTimeline(timeline);
    }

    private void AnimationDelete()
    {
      foreach (AnimationSceneNode animationNode in this.SceneViewModel.AnimationSelectionSet.Selection)
      {
        if (animationNode is PathAnimationSceneNode)
          this.SceneViewModel.AnimationEditor.DeleteMotionPath(animationNode.TargetElement);
        else
          this.SceneViewModel.AnimationEditor.DeleteAnimation(animationNode);
      }
    }

    private void KeyFrameDelete()
    {
      foreach (KeyFrameSceneNode keyFrameSceneNode in this.SceneViewModel.KeyFrameSelectionSet.Selection)
        this.SceneViewModel.AnimationEditor.DeleteKeyframe(keyFrameSceneNode.TargetElement, keyFrameSceneNode.TargetProperty, keyFrameSceneNode.Time);
    }

    private void SetterDelete()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitDeleteSetters))
      {
        ICollection<SetterSceneNode> collection = (ICollection<SetterSceneNode>) this.SceneViewModel.SetterSelectionSet.Selection;
        this.SceneViewModel.SetterSelectionSet.Selection.CopyTo(new SetterSceneNode[collection.Count], 0);
        this.SceneViewModel.SetterSelectionSet.Clear();
        foreach (SceneNode sceneNode in (IEnumerable<SetterSceneNode>) collection)
          sceneNode.Remove();
        editTransaction.Commit();
      }
    }

    private void GridColumnDelete()
    {
      ColumnDefinitionNode primarySelection = this.SceneViewModel.GridColumnSelectionSet.PrimarySelection;
      GridElement grid = primarySelection.Parent as GridElement;
      int index = grid.ColumnDefinitions.IndexOf(primarySelection);
      (grid.LayoutDesigner as GridLayoutDesigner).RemoveColumn(grid, index);
    }

    private void GridRowDelete()
    {
      RowDefinitionNode primarySelection = this.SceneViewModel.GridRowSelectionSet.PrimarySelection;
      GridElement grid = primarySelection.Parent as GridElement;
      int index = grid.RowDefinitions.IndexOf(primarySelection);
      (grid.LayoutDesigner as GridLayoutDesigner).RemoveRow(grid, index);
    }

    private void TextDelete()
    {
      this.SceneViewModel.TextSelectionSet.TextEditProxy.DeleteSelection();
    }

    private class SelectedComponentsRemover
    {
      private List<List<int>> selectedSegments = new List<List<int>>();
      private List<List<int>> selectedPoints = new List<List<int>>();
      private PathEditorTarget pathEditorTarget;

      public SelectedComponentsRemover(PathEditorTarget pathEditorTarget)
      {
        this.pathEditorTarget = pathEditorTarget;
      }

      public void Execute(ICollection<PathPart> pathPartSelection)
      {
        this.SplitComponentsByFigure(pathPartSelection);
        for (int figureIndex = this.pathEditorTarget.PathGeometry.Figures.Count - 1; figureIndex >= 0; --figureIndex)
        {
          List<int> segmentsByFigure = this.selectedSegments[figureIndex];
          List<int> pointsByFigure = this.selectedPoints[figureIndex];
          segmentsByFigure.Sort();
          segmentsByFigure.Reverse();
          pointsByFigure.Sort();
          pointsByFigure.Reverse();
          if (segmentsByFigure.Count == 0)
          {
            this.RemovePointsByFigure(figureIndex, pointsByFigure);
          }
          else
          {
            if (PathFigureUtilities.IsClosed(this.pathEditorTarget.PathGeometry.Figures[figureIndex]))
              this.OpenFigureAndShiftIndices(figureIndex, segmentsByFigure, pointsByFigure);
            this.RemovePointsAndSegmentsFromOpenFigure(figureIndex, segmentsByFigure, pointsByFigure);
          }
          if (PathFigureUtilities.IsIsolatedPoint(this.pathEditorTarget.PathGeometry.Figures[figureIndex]))
            this.pathEditorTarget.CreateGeometryEditor().RemoveFigure(figureIndex);
        }
      }

      private void SplitComponentsByFigure(ICollection<PathPart> subselection)
      {
        for (int index = 0; index < this.pathEditorTarget.PathGeometry.Figures.Count; ++index)
        {
          this.selectedSegments.Add(new List<int>());
          this.selectedPoints.Add(new List<int>());
        }
        foreach (PathPart pathPart in (IEnumerable<PathPart>) subselection)
        {
          if (pathPart is PathPoint)
            this.selectedPoints[pathPart.FigureIndex].Add(pathPart.PartIndex);
          if (pathPart is PathSegment)
            this.selectedSegments[pathPart.FigureIndex].Add(pathPart.PartIndex);
        }
      }

      private void RemovePointsByFigure(int figureIndex, List<int> pointsByFigure)
      {
        PathFigureEditor figureEditor = this.pathEditorTarget.CreateFigureEditor(figureIndex);
        foreach (int pointIndex in pointsByFigure)
          figureEditor.RemovePoint(pointIndex);
      }

      private void OpenFigureAndShiftIndices(int figureIndex, List<int> segmentsByFigure, List<int> pointsByFigure)
      {
        PathFigureEditor figureEditor = this.pathEditorTarget.CreateFigureEditor(figureIndex);
        int pointIndex = segmentsByFigure[0];
        figureEditor.Open(pointIndex);
        if (pointIndex == 0)
        {
          int num = PathFigureUtilities.PointCount(figureEditor.PathFigure) - 1;
          segmentsByFigure[0] = num;
        }
        else
        {
          int num = PathFigureUtilities.PointCount(figureEditor.PathFigure) - 1 - pointIndex;
          for (int index = 0; index < segmentsByFigure.Count; ++index)
            segmentsByFigure[index] = segmentsByFigure[index] + num;
          for (int index = 0; index < pointsByFigure.Count; ++index)
            pointsByFigure[index] = pointsByFigure[index] < pointIndex ? pointsByFigure[index] + num : pointsByFigure[index] - pointIndex;
        }
        segmentsByFigure.Sort();
        segmentsByFigure.Reverse();
        pointsByFigure.Sort();
        pointsByFigure.Reverse();
      }

      private void RemovePointsAndSegmentsFromOpenFigure(int figureIndex, List<int> segmentsByFigure, List<int> pointsByFigure)
      {
        PathGeometryEditor geometryEditor = this.pathEditorTarget.CreateGeometryEditor();
        int index1 = 0;
        int index2 = 0;
        while (index1 < segmentsByFigure.Count || index2 < pointsByFigure.Count)
        {
          int pointIndex1 = -1;
          int pointIndex2 = -1;
          if (index1 < segmentsByFigure.Count)
            pointIndex1 = segmentsByFigure[index1];
          if (index2 < pointsByFigure.Count)
            pointIndex2 = pointsByFigure[index2];
          int num = PathFigureUtilities.PointCount(geometryEditor.PathGeometry.Figures[figureIndex]) - 1;
          if (pointIndex2 == num || pointIndex1 == num)
          {
            geometryEditor.RemoveLastSegmentOfFigure(figureIndex);
            if (pointIndex2 == num)
              ++index2;
            if (pointIndex1 == num)
              ++index1;
          }
          else if (pointIndex1 > pointIndex2)
          {
            geometryEditor.SplitFigure(figureIndex, pointIndex1);
            geometryEditor.RemoveLastSegmentOfFigure(figureIndex);
            ++index1;
          }
          else if (pointIndex1 < pointIndex2)
          {
            geometryEditor.RemovePoint(figureIndex, pointIndex2);
            ++index2;
          }
          else
          {
            geometryEditor.SplitFigure(figureIndex, pointIndex2);
            geometryEditor.RemoveLastSegmentOfFigure(figureIndex);
            geometryEditor.RemoveFirstSegmentOfFigure(geometryEditor.PathGeometry.Figures.Count - 1);
            ++index2;
            ++index1;
          }
        }
      }
    }
  }
}

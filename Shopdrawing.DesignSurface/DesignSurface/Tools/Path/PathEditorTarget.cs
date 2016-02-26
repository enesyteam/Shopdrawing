// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathEditorTarget
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands.Undo;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public abstract class PathEditorTarget : IDisposable
  {
    private SceneViewModel viewModel;
    private PathGeometry pathGeometry;
    private int criticalEdits;
    private bool isCurrentlyEditing;
    private bool ignoreFirstInserted;
    private PathDiffChangeList changeList;
    private bool isDisposed;

    public PathGeometry PathGeometry
    {
      get
      {
        return this.pathGeometry;
      }
      set
      {
        this.pathGeometry = value;
      }
    }

    public bool IsCurrentlyEditing
    {
      get
      {
        return this.isCurrentlyEditing;
      }
    }

    public int CriticalEdits
    {
      get
      {
        return this.criticalEdits;
      }
    }

    protected SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public PathDiffChangeList PathDiffChangeList
    {
      get
      {
        return this.changeList;
      }
    }

    public Vector LastTangent
    {
      get
      {
        return PointVectorConverter.FromPoint(this.EditingElement.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.LastTangentProperty));
      }
    }

    public abstract PathEditMode PathEditMode { get; }

    public abstract Base2DElement EditingElement { get; }

    public event EventHandler MatrixChanged;

    protected PathEditorTarget(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    ~PathEditorTarget()
    {
      this.Dispose(false);
    }

    protected void OnMatrixChanged()
    {
      if (this.MatrixChanged == null)
        return;
      this.MatrixChanged((object) this, EventArgs.Empty);
    }

    public void AddCriticalEdit()
    {
      ++this.criticalEdits;
    }

    public void BeginEditing()
    {
      if (this.isCurrentlyEditing)
        return;
      this.changeList = new PathDiffChangeList();
      this.UpdateCachedPath();
      this.isCurrentlyEditing = true;
      this.BeginEditingInternal();
      this.EnsureOnlySingleSegments();
      this.UpdateSegmentMapping();
    }

    public void EndEditing(bool pathJustCreated)
    {
      if (!this.isCurrentlyEditing)
        return;
      if (this.ShouldCollapseSegments() && PathGeometryUtilities.CollapseSingleSegmentsToPolySegments(this.PathGeometry))
        this.AddCriticalEdit();
      this.isCurrentlyEditing = false;
      this.ignoreFirstInserted = true;
      this.EditingElement.ViewModel.Document.AddUndoUnit((IUndoUnit) new PathEditorTarget.PathEditorTargetUndoUnit((BaseFrameworkElement) this.EditingElement, this.PathEditMode, true));
      this.EndEditingInternal(pathJustCreated);
      this.EditingElement.ViewModel.Document.AddUndoUnit((IUndoUnit) new PathEditorTarget.PathEditorTargetUndoUnit((BaseFrameworkElement) this.EditingElement, this.PathEditMode, false));
    }

    public virtual void PostDeleteAction()
    {
    }

    public PathGeometryEditor CreateGeometryEditor()
    {
      return new PathGeometryEditor(this.PathGeometry, this.changeList);
    }

    public PathFigureEditor CreateFigureEditor(int figureIndex)
    {
      return new PathFigureEditor(this.PathGeometry.Figures[figureIndex], this.changeList, figureIndex);
    }

    protected void EnsureOnlySingleSegments()
    {
      if (!PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(this.PathGeometry))
        return;
      this.AddCriticalEdit();
    }

    protected virtual void OnBasisSubscriptionPathNodeInserted(object sender, SceneNode basisNode, PathElement basisContent, SceneNode newPathNode, PathGeometry newContent)
    {
      if (this.ignoreFirstInserted)
      {
        this.ignoreFirstInserted = false;
      }
      else
      {
        this.UpdateCachedPath();
        this.AddCriticalEdit();
      }
    }

    protected void UpdateSegmentMapping()
    {
      for (int index1 = 0; index1 < this.PathGeometry.Figures.Count; ++index1)
      {
        PathFigure pathFigure = this.PathGeometry.Figures[index1];
        pathFigure.SetValue(PathFigureUtilities.FigureMappingProperty, (object) index1);
        for (int index2 = 0; index2 < pathFigure.Segments.Count; ++index2)
          pathFigure.Segments[index2].SetValue(PathFigureUtilities.SegmentMappingProperty, (object) index2);
      }
    }

    public abstract Matrix GetTransformToAncestor(IViewObject ancestorViewObject);

    public virtual Matrix RefineTransformToAdornerLayer(Matrix matrix)
    {
      return matrix;
    }

    public abstract void RemovePath();

    public abstract void UpdateFromDamage(SceneUpdatePhaseEventArgs args);

    public abstract void RefreshSubscription();

    protected virtual void BeginEditingInternal()
    {
    }

    protected virtual void EndEditingInternal(bool pathJustCreated)
    {
    }

    public abstract void UpdateCachedPath();

    protected virtual bool ShouldCollapseSegments()
    {
      return this.EditingElement.ProjectContext.IsCapabilitySet(PlatformCapability.ShouldCollapseSinglePathSegments);
    }

    internal virtual void UpdatePathIfNeeded()
    {
    }

    protected virtual void Dispose(bool fromDispose)
    {
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private class PathEditorTargetUndoUnit : UndoUnit
    {
      private bool firstRedo = true;
      private BaseFrameworkElement sceneElement;
      private PathEditMode pathEditMode;
      private bool updateOnUndo;

      public PathEditorTargetUndoUnit(BaseFrameworkElement sceneElement, PathEditMode pathEditMode, bool updateOnUndo)
      {
        this.sceneElement = sceneElement;
        this.pathEditMode = pathEditMode;
        this.updateOnUndo = updateOnUndo;
      }

      public override void Undo()
      {
        if (this.updateOnUndo)
          this.Invalidate();
        base.Undo();
      }

      public override void Redo()
      {
        if (this.firstRedo)
          this.firstRedo = false;
        else if (!this.updateOnUndo)
          this.Invalidate();
        base.Redo();
      }

      private void Invalidate()
      {
        Tool activeTool = this.sceneElement.ViewModel.DesignerContext.ToolManager.ActiveTool;
        if (activeTool == null)
          return;
        PathEditorTarget pathEditorTarget = activeTool.GetPathEditorTarget((Base2DElement) this.sceneElement, this.pathEditMode);
        pathEditorTarget.UpdateCachedPath();
        pathEditorTarget.AddCriticalEdit();
      }
    }
  }
}

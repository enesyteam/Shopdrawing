// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PenAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public abstract class PenAction
  {
    private PathEditorTarget pathEditorTarget;
    private PathEditContext pathEditContext;
    private SceneViewModel viewModel;
    private SceneEditTransaction transaction;
    private AxisConstraint axisConstraint;
    private bool isActive;

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public SceneView View
    {
      get
      {
        return this.viewModel.DefaultView;
      }
    }

    public abstract Cursor HoverCursor { get; }

    public abstract Cursor DragCursor { get; }

    public abstract string ActionString { get; }

    public bool IsActive
    {
      get
      {
        return this.isActive;
      }
    }

    public virtual bool SetPathActive
    {
      get
      {
        return false;
      }
    }

    public PathEditorTarget PathEditorTarget
    {
      get
      {
        return this.pathEditorTarget;
      }
      protected set
      {
        this.pathEditorTarget = value;
      }
    }

    public PathEditContext PathEditContext
    {
      get
      {
        return this.pathEditContext;
      }
      protected set
      {
        this.pathEditContext = value;
      }
    }

    protected SceneElement EditingElement
    {
      get
      {
        return (SceneElement) this.pathEditorTarget.EditingElement;
      }
    }

    protected PathGeometry Path
    {
      get
      {
        return this.pathEditorTarget.PathGeometry;
      }
    }

    protected Matrix EditingElementTransformToRoot
    {
      get
      {
        return this.pathEditorTarget.GetTransformToAncestor((IViewObject) this.View.HitTestRoot);
      }
    }

    protected AxisConstraint AxisConstraint
    {
      get
      {
        return this.axisConstraint;
      }
    }

    public Vector LastTangent
    {
      get
      {
        return PointVectorConverter.FromPoint(this.EditingElement.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.LastTangentProperty));
      }
      set
      {
        this.EditingElement.SetLocalValueAsWpf(DesignTimeProperties.LastTangentProperty, (object) PointVectorConverter.ToPoint(value));
      }
    }

    protected bool IsAltDown
    {
      get
      {
        return (InputManager.Current.PrimaryKeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
      }
    }

    protected bool IsControlDown
    {
      get
      {
        return (InputManager.Current.PrimaryKeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
      }
    }

    protected bool IsShiftDown
    {
      get
      {
        return (InputManager.Current.PrimaryKeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
      }
    }

    protected bool IsEditTransactionOpen
    {
      get
      {
        return this.transaction != null;
      }
    }

    protected SceneEditTransaction EditTransaction
    {
      get
      {
        this.EnsureEditTransaction();
        return this.transaction;
      }
    }

    protected PenAction(PathEditorTarget pathEditorTarget, SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.pathEditorTarget = pathEditorTarget;
      this.axisConstraint = new AxisConstraint();
    }

    public void Begin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
      if (this.IsActive)
        throw new InvalidOperationException(ExceptionStringTable.PenActionActionAlreadyBegun);
      this.isActive = true;
      this.pathEditContext = pathEditContext;
      this.EnsureEditTransaction();
      if (this.pathEditorTarget != null)
        this.pathEditorTarget.BeginEditing();
      this.OnBegin(pathEditContext, mouseDevice);
      if (this.pathEditorTarget == null)
        return;
      this.pathEditorTarget.AddCriticalEdit();
      this.View.AdornerLayer.InvalidateAdornerVisuals(this.EditingElement);
      if (!this.IsEditTransactionOpen)
        return;
      this.UpdateEditTransaction();
    }

    public void Drag(MouseDevice mouseDevice, double zoom)
    {
      if (!this.IsActive)
        return;
      this.EnsureEditTransaction();
      this.OnDrag(mouseDevice, zoom);
      if (this.pathEditorTarget == null)
        return;
      this.View.AdornerLayer.InvalidateAdornerVisuals(this.EditingElement);
      this.UpdateEditTransaction();
    }

    public Point GetPointInViewRootCoordinates(MouseDevice mouseDevice, bool snapPoint)
    {
      Point point = this.View.Artboard.CalculateTransformFromArtboardToContent().Value.Transform(mouseDevice.GetPosition((IInputElement) this.View.Artboard));
      if (snapPoint)
        point = this.ViewModel.DesignerContext.SnappingEngine.SnapPoint(point);
      return point;
    }

    public void End()
    {
      if (this.IsActive)
      {
        this.EnsureEditTransaction();
        this.OnEnd();
        if (this.pathEditorTarget != null)
        {
          this.pathEditorTarget.EndEditing(false);
          this.View.AdornerLayer.InvalidateAdornersStructure(this.EditingElement);
        }
        this.CommitEditTransaction();
      }
      this.isActive = false;
    }

    protected virtual void OnBegin(PathEditContext pathEditContext, MouseDevice mouseDevice)
    {
    }

    protected virtual void OnDrag(MouseDevice mouseDevice, double zoom)
    {
    }

    protected virtual void OnEnd()
    {
    }

    protected PathGeometryEditor BeginEditing()
    {
      return new PathGeometryEditor(this.pathEditorTarget.PathGeometry, this.pathEditorTarget.PathDiffChangeList);
    }

    protected void RemovePath()
    {
      this.pathEditorTarget.RemovePath();
      this.pathEditorTarget = (PathEditorTarget) null;
      this.pathEditContext = (PathEditContext) null;
    }

    protected Point GetLastPoint(int pathFigureIndex)
    {
      PathFigure pathFigure = this.Path.Figures[pathFigureIndex];
      return PathFigureUtilities.GetPoint(pathFigure, PathFigureUtilities.PointCount(pathFigure) - 1);
    }

    protected void EnsureEditTransaction()
    {
      if (this.transaction != null)
        return;
      this.transaction = this.ViewModel.CreateEditTransaction(this.ActionString);
    }

    protected void CancelEditTransaction()
    {
      if (this.transaction == null)
        return;
      this.transaction.Cancel();
      this.transaction = (SceneEditTransaction) null;
    }

    protected void CommitEditTransaction()
    {
      if (this.transaction == null)
        return;
      this.transaction.Commit();
      this.transaction = (SceneEditTransaction) null;
    }

    protected void UpdateEditTransaction()
    {
      this.transaction.Update();
    }
  }
}

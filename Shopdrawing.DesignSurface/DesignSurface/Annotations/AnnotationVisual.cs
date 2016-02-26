// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationVisual
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class AnnotationVisual : UserControl, IComponentConnector
  {
    internal static readonly Vector adjustIncrement = new Vector(10.0, 10.0);
    internal static readonly Size occupationRectSize = new Size(8.0, 8.0);
    private const double autoScrollMargin = 16.0;
    private AnnotationViewModel viewModel;
    private int disableFocusStealingCount;
    private AnnotationVisual.DragData drag;
    internal Border IconView;
    internal Border EditView;
    internal Grid Header;
    internal TextBox Text_TextBox;
    private bool _contentLoaded;

    public AnnotationViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public AnnotationSceneNode Annotation
    {
      get
      {
        return this.viewModel.Annotation;
      }
    }

    private bool AllowFocusStealing
    {
      get
      {
        return this.disableFocusStealingCount == 0;
      }
    }

    private SnappingEngine SnappingEngine
    {
      get
      {
        if (this.ToolContext == null)
          return (SnappingEngine) null;
        return this.ToolContext.SnappingEngine;
      }
    }

    private ToolBehaviorContext ToolContext
    {
      get
      {
        ToolManager toolManager = this.Annotation.DesignerContext.ToolManager;
        if (toolManager != null)
        {
          Tool tool = Enumerable.FirstOrDefault<Tool>((IEnumerable<Tool>) toolManager.Tools);
          if (tool != null)
            return tool.GetActiveViewContext();
        }
        return (ToolBehaviorContext) null;
      }
    }

    public AnnotationVisual(AnnotationViewModel annotationViewModel)
    {
      this.InitializeComponent();
      this.DataContext = (object) (this.viewModel = annotationViewModel);
      this.viewModel.PropertyChanged += new PropertyChangedEventHandler(this.ViewModel_PropertyChanged);
      this.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(this.Self_IsKeyboardFocusWithinChanged);
      this.Text_TextBox.IsVisibleChanged += (DependencyPropertyChangedEventHandler) ((sender, args) => this.Text_TextBox_IsVisibleChanged());
      this.IconView.MouseRightButtonUp += (MouseButtonEventHandler) ((sender, args) => this.DisplayContextMenu(args.GetPosition((IInputElement) this)));
      this.Header.MouseRightButtonUp += (MouseButtonEventHandler) ((sender, args) => this.DisplayContextMenu(args.GetPosition((IInputElement) this)));
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName.Equals("Selected"))
      {
        if (this.viewModel.Selected)
          return;
        this.viewModel.ViewMode = ViewMode.Icon;
      }
      else
      {
        if (!e.PropertyName.Equals("ViewMode"))
          return;
        this.Annotation.ViewModel.AnnotationEditor.InvalidateAdorners(this.Annotation, false);
        if (this.viewModel.ViewMode != ViewMode.Icon || !this.IsKeyboardFocusWithin)
          return;
        this.Annotation.ViewModel.DefaultView.ReturnFocus();
      }
    }

    private void Self_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.IsKeyboardFocusWithin)
        return;
      ContextMenu contextMenu = Keyboard.FocusedElement as ContextMenu;
      if (contextMenu != null && (contextMenu.DataContext == this.viewModel || "AnnotationAdorner.ContextMenuTag".Equals(contextMenu.Tag)))
        return;
      this.viewModel.ViewMode = ViewMode.Icon;
    }

    private void Text_TextBox_IsVisibleChanged()
    {
      if (!this.Text_TextBox.IsVisible || !this.AllowFocusStealing || Keyboard.FocusedElement == this.Text_TextBox)
        return;
      Keyboard.Focus((IInputElement) this.Text_TextBox);
      this.Text_TextBox.CaretIndex = 0;
    }

    internal bool ProcessKey(KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        e.Handled = true;
        if (this.drag != null)
          this.CancelDragMode();
        else
          this.viewModel.ViewMode = ViewMode.Icon;
      }
      else if (this.drag != null)
        e.Handled = true;
      return e.Handled;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      if (this.ProcessKey(e))
        return;
      base.OnPreviewKeyDown(e);
    }

    private Size TransformFromRenderToLayout(Size renderSize)
    {
      return (Size) (this.RenderTransform ?? Transform.Identity).Transform((Point) renderSize);
    }

    public void Initialize(bool treatTopLeftAsCenter)
    {
      using (SceneEditTransaction editTransaction = this.Annotation.ViewModel.CreateEditTransaction("Position Annotation", true))
      {
        bool flag = false;
        if (treatTopLeftAsCenter)
        {
          Size size = this.TransformFromRenderToLayout(this.IconView.RenderSize);
          this.ViewModel.Left -= Math.Round(size.Width / 2.0);
          this.ViewModel.Top -= Math.Round(size.Height / 2.0);
          flag = true;
        }
        if (flag | this.AdjustPositionToAvoidOverlap())
          editTransaction.Commit();
      }
      this.viewModel.Selected = true;
      this.viewModel.ViewMode = ViewMode.Editing;
      this.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => this.Visibility = Visibility.Visible));
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);
      e.Handled = this.viewModel.ViewMode == ViewMode.Editing && Keyboard.FocusedElement is TextBox;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      e.Handled = true;
      this.BringToTopOfZOrder();
      this.viewModel.Selected = true;
      this.OnDragBegin();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      e.Handled = true;
      if (this.drag == null)
        return;
      this.OnDragEnd();
    }

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseRightButtonDown(e);
      e.Handled = true;
    }

    private void DisplayContextMenu(Point pointerPosition)
    {
      FrameworkElement sceneScrollViewer = this.Annotation.ViewModel.DefaultView.SceneScrollViewer;
      Point point = this.TransformToAncestor((Visual) sceneScrollViewer).Transform(pointerPosition);
      ContextMenu contextMenu = (ContextMenu) this.Resources[(object) "AnnotationContextMenu"];
      contextMenu.Placement = PlacementMode.RelativePoint;
      contextMenu.PlacementTarget = (UIElement) sceneScrollViewer;
      contextMenu.PlacementRectangle = new Rect()
      {
        Location = point
      };
      contextMenu.DataContext = (object) null;
      contextMenu.DataContext = (object) this.viewModel;
      contextMenu.IsOpen = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      e.Handled = true;
      if (this.drag == null)
        return;
      Vector delta = e.GetPosition((IInputElement) null) - this.drag.InitialCursorPos;
      if (this.drag != null && !this.drag.Engaged)
        this.drag.Engaged = Math.Abs(delta.X) > this.drag.Threshold.X || Math.Abs(delta.Y) > this.drag.Threshold.Y;
      if (!this.drag.Engaged)
        return;
      if (this.drag.Undo == null)
        this.drag.Undo = this.Annotation.ViewModel.CreateEditTransaction(StringTable.MoveAnnotationUndoUnit);
      this.OnDrag(delta);
    }

    private void OnDragBegin()
    {
      this.CaptureMouse();
      this.drag = new AnnotationVisual.DragData()
      {
        Engaged = false,
        InitialCursorPos = Mouse.GetPosition((IInputElement) null),
        OriginalAnnotationPos = this.Annotation.Position
      };
    }

    private void OnDrag(Vector delta)
    {
      FrameworkElement frameworkElement = (FrameworkElement) this.Annotation.ViewModel.DefaultView.Artboard;
      if (this.BoundaryTest(Mouse.GetPosition((IInputElement) frameworkElement), new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight), 16.0) == AnnotationVisual.BoundaryTestResult.Outside)
      {
        Mouse.OverrideCursor = Cursors.No;
      }
      else
      {
        Mouse.OverrideCursor = (Cursor) null;
        this.OnDragWithin(delta);
      }
    }

    private AnnotationVisual.BoundaryTestResult BoundaryTest(Point point, Rect rect, double margin)
    {
      if (!rect.Contains(point))
        return AnnotationVisual.BoundaryTestResult.Outside;
      return margin <= 0.0 || Rect.Inflate(rect, -margin, -margin).Contains(point) ? AnnotationVisual.BoundaryTestResult.Inside : AnnotationVisual.BoundaryTestResult.InsideMargin;
    }

    private void OnDragWithin(Vector delta)
    {
      Matrix matrix = (((UIElement) this.Parent).RenderTransform ?? Transform.Identity).Clone().Value;
      matrix.Invert();
      Vector offset = matrix.Transform(delta);
      Size size = this.TransformFromRenderToLayout(this.RenderSize);
      SceneElement sceneElement = this.Annotation.ViewModel.ActiveSceneInsertionPoint.SceneElement;
      Rect rectSnap = Rect.Transform(new Rect(this.drag.OriginalAnnotationPos, size), sceneElement.TransformFromRoot);
      Point point = RoundingHelper.RoundPosition(this.drag.OriginalAnnotationPos + offset + this.SnapRect(rectSnap, sceneElement, offset, EdgeFlags.All));
      this.Annotation.Left = point.X;
      this.Annotation.Top = point.Y;
      this.drag.Undo.Update();
    }

    private void OnDragEnd()
    {
      if (this.drag.Engaged)
      {
        this.Annotation.Timestamp = DateTime.UtcNow;
        this.drag.Undo.Commit();
      }
      else
        this.viewModel.ViewMode = this.viewModel.ViewMode == ViewMode.Icon ? ViewMode.Editing : ViewMode.Icon;
      this.CancelDragMode();
    }

    private void CancelDragMode()
    {
      this.StopSnappingEngine();
      Mouse.OverrideCursor = (Cursor) null;
      this.drag.Dispose();
      this.drag = (AnnotationVisual.DragData) null;
      this.ReleaseMouseCapture();
    }

    private void StartSnappingEngine()
    {
      if (this.SnappingEngine == null || this.ToolContext == null)
        return;
      this.SnappingEngine.Start(this.ToolContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
    }

    private void StopSnappingEngine()
    {
      if (this.SnappingEngine == null)
        return;
      this.SnappingEngine.Stop();
    }

    private Vector SnapRect(Rect rectSnap, SceneElement container, Vector offset, EdgeFlags edgeFlags)
    {
      if (this.SnappingEngine == null)
        return new Vector(0.0, 0.0);
      if (!this.SnappingEngine.IsStarted)
        this.StartSnappingEngine();
      return this.SnappingEngine.SnapRect(rectSnap, container, offset, edgeFlags);
    }

    internal void BringToTopOfZOrder()
    {
      Panel panel = this.Parent as Panel;
      if (panel == null || panel.Children.IndexOf((UIElement) this) == panel.Children.Count - 1)
        return;
      panel.Children.Remove((UIElement) this);
      panel.Children.Add((UIElement) this);
    }

    internal bool AdjustPositionToAvoidOverlap()
    {
      Transform childRenderTransform = this.Annotation.ViewModel.DefaultView.Artboard.AnnotationLayer.ChildRenderTransform;
      Matrix matrix = childRenderTransform != null ? childRenderTransform.Value : Matrix.Identity;
      Size scaledSize = (Size) ((Vector) AnnotationVisual.occupationRectSize * matrix);
      Vector vector1 = AnnotationVisual.adjustIncrement * matrix;
      Func<Point, Size, Rect> CreateRectCenteredOnPoint = (Func<Point, Size, Rect>) ((pt, size) =>
      {
        Vector vector2 = (Vector) size;
        return new Rect(pt - vector2 / 2.0, pt + vector2 / 2.0);
      });
      IEnumerable<Rect> source = Enumerable.Select<Point, Rect>(Enumerable.Select<AnnotationSceneNode, Point>(Enumerable.Where<AnnotationSceneNode>(this.Annotation.ViewModel.AnnotationEditor.GetAnnotationsWithoutCache(), (Func<AnnotationSceneNode, bool>) (anno => !anno.Equals((object) this.Annotation))), (Func<AnnotationSceneNode, Point>) (anno => anno.Position)), (Func<Point, Rect>) (pt => CreateRectCenteredOnPoint(pt, scaledSize)));
      Point position = this.Annotation.Position;
      Point newPos = position;
      while (Enumerable.Any<Rect>(source, (Func<Rect, bool>) (rect => rect.Contains(newPos))))
        newPos += vector1;
      if (newPos != position)
      {
        this.ViewModel.Left = newPos.X;
        this.viewModel.Top = newPos.Y;
      }
      return newPos != position;
    }

    internal IDisposable DisableFocusStealing()
    {
      return (IDisposable) new AnnotationVisual.DisableFocusStealingToken(this);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/annotations/annotationvisual.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.IconView = (Border) target;
          break;
        case 2:
          this.EditView = (Border) target;
          break;
        case 3:
          this.Header = (Grid) target;
          break;
        case 4:
          this.Text_TextBox = (TextBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class DragData : IDisposable
    {
      private static readonly Vector threshold = new Vector(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance);

      public Vector Threshold
      {
        get
        {
          return AnnotationVisual.DragData.threshold;
        }
      }

      public bool Engaged { get; set; }

      public Point OriginalAnnotationPos { get; set; }

      public Point InitialCursorPos { get; set; }

      public SceneEditTransaction Undo { get; set; }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        if (this.Undo != null)
          this.Undo.Dispose();
        this.Undo = (SceneEditTransaction) null;
      }
    }

    private enum BoundaryTestResult
    {
      Outside,
      InsideMargin,
      Inside,
    }

    private class DisableFocusStealingToken : IDisposable
    {
      private AnnotationVisual annotationVisual;

      public DisableFocusStealingToken(AnnotationVisual annotationVisual)
      {
        this.annotationVisual = annotationVisual;
        ++this.annotationVisual.disableFocusStealingCount;
      }

      public void Dispose()
      {
        --this.annotationVisual.disableFocusStealingCount;
        GC.SuppressFinalize((object) this);
      }
    }
  }
}

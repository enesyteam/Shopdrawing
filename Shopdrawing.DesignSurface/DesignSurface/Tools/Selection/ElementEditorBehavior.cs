// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Selection.ElementEditorBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Selection
{
  internal class ElementEditorBehavior : ElementToolBehavior
  {
    private const int TextClickTimeout = 1500;
    private const double PathSelectionHitTestTolerance = 2.0;
    private bool isDeepMarqueeEnabled;
    private bool is3DSelectionEnabled;
    private bool isTextEditingEnabled;
    private bool isDuplicationEnabled;
    private bool isPathSubselectionEnabled;
    private Cursor defaultCursor;
    private Cursor elementCursor;
    private Cursor moveCursor;
    private Cursor lastCursor;
    private bool isRubberbanding;
    private bool isEnteringTextEdit;
    private ToolBehavior moveBehavior;
    private DateTime lastClickTime;
    private SceneElement lastClickedElement;
    private SceneElement lastClickedTopElement;
    private Point startPoint;
    private bool isToggling;
    private bool isExtending;
    private bool isCycling;
    private SceneElement lastHoveredElement;
    private PreviewElementHighlighter previewHighlighter;
    private bool isAltDownBeforeDrag;

    public override string ActionString
    {
      get
      {
        return StringTable.DuplicateElementsUndo;
      }
    }

    public new ISceneInsertionPoint ActiveSceneInsertionPoint
    {
      get
      {
        return this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
      }
    }

    protected override Cursor DefaultCursor
    {
      get
      {
        return this.defaultCursor;
      }
    }

    private SceneElement PreviewElement
    {
      set
      {
        this.previewHighlighter.PreviewElement = value;
        this.ActiveSceneViewModel.TimelineItemManager.SetHoverOverrideSelectionItem((SceneNode) value);
      }
    }

    private SelectionFor3D SelectionFor3D
    {
      get
      {
        if (!this.is3DSelectionEnabled)
          return SelectionFor3D.None;
        return !this.isDeepMarqueeEnabled ? SelectionFor3D.TopLevel : SelectionFor3D.Deep;
      }
    }

    private bool IsCycleModifierDown
    {
      get
      {
        if (this.IsAltDown)
          return !this.IsShiftDown;
        return false;
      }
    }

    private bool CanUseRubberBanding
    {
      get
      {
        return !(this.moveBehavior is BrushBackgroundBehavior);
      }
    }

    private bool IsWithinSelectionTool
    {
      get
      {
        return this.ToolBehaviorContext.ActiveTool is ISelectionTool;
      }
    }

    public ElementEditorBehavior(ToolBehaviorContext toolContext, bool isDeepMarqueeEnabled, bool is3DSelectionEnabled, bool isTextEditingEnabled, bool isDuplicationEnabled, bool isPathSubselectionEnabled, Cursor defaultCursor, Cursor elementCursor, Cursor moveCursor, ToolBehavior moveBehavior)
      : base(toolContext)
    {
      this.isDeepMarqueeEnabled = isDeepMarqueeEnabled;
      this.is3DSelectionEnabled = is3DSelectionEnabled;
      this.isPathSubselectionEnabled = isPathSubselectionEnabled;
      this.defaultCursor = defaultCursor;
      this.elementCursor = elementCursor;
      this.moveCursor = moveCursor;
      this.moveBehavior = moveBehavior;
      this.isTextEditingEnabled = isTextEditingEnabled;
      this.isDuplicationEnabled = isDuplicationEnabled;
      this.lastClickTime = new DateTime();
      this.previewHighlighter = new PreviewElementHighlighter(toolContext.View.AdornerLayer, (PreviewElementHighlighter.CreateAdornerSet) (adornedElement => (AnimatableAdornerSet) new ElementEditorBehavior.PreviewElementAdornerSet(toolContext, adornedElement)), (PreviewElementHighlighter.VerifyIsEnabled) (() => toolContext.ToolManager.ShowSelectionPreview));
    }

    protected override bool OnHoverExit()
    {
      this.lastHoveredElement = (SceneElement) null;
      this.PreviewElement = (SceneElement) null;
      return base.OnHoverExit();
    }

    protected override bool OnHoverOverAdorner(IAdorner adorner)
    {
      this.lastHoveredElement = (SceneElement) null;
      this.PreviewElement = (SceneElement) null;
      return base.OnHoverOverAdorner(adorner);
    }

    protected override bool OnHoverOverNonAdorner(Point pointerPosition)
    {
      if (this.IsControlDown && this.IsWithinSelectionTool && this.CanUseRubberBanding)
      {
        this.SetCursor(ToolCursors.MarqueeSelectCursor);
      }
      else
      {
        this.lastHoveredElement = this.ActiveView.GetSelectableElementAtPoint(pointerPosition, this.SelectionFor3D, false);
        if (this.lastHoveredElement != null)
        {
          if (this.IsAltDown && this.CanDuplicate(true))
            this.SetCursor(ToolCursors.DuplicateWedgeCursor);
          else if (this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(this.lastHoveredElement))
            this.SetCursor(this.moveCursor);
          else
            this.SetCursor(this.elementCursor);
        }
        else
          this.SetCursor(this.defaultCursor);
      }
      if (!this.IsOnlyUpdatingCursor)
        this.UpdatePreviewElement();
      return true;
    }

    protected override bool OnButtonDownOverAdorner(IAdorner adorner)
    {
      Adorner3D adorner3D = adorner as Adorner3D;
      if (!this.is3DSelectionEnabled || adorner3D == null || !adorner3D.IsProxyGeometry)
        return base.OnButtonDownOverAdorner(adorner);
      this.lastClickedElement = (SceneElement) adorner3D.Element;
      return true;
    }

    protected override bool OnButtonDownOverNonAdorner(Point pointerPosition)
    {
      this.isEnteringTextEdit = false;
      SceneElement oldClickedElement = this.lastClickedElement;
      this.lastClickedElement = this.ActiveView.GetSelectableElementAtPoint(pointerPosition, this.SelectionFor3D, false, true);
      if (this.lastClickedElement != null)
      {
        this.HandleEnterTextEdit(oldClickedElement, this.lastClickedElement);
        if (this.IsControlDown && this.IsWithinSelectionTool)
          this.isToggling = true;
        if (this.IsShiftDown)
          this.isExtending = true;
        else if (this.IsCycleModifierDown)
          this.isCycling = true;
      }
      else
        this.isRubberbanding = true;
      this.startPoint = pointerPosition;
      this.lastClickTime = DateTime.Now;
      this.UpdateCursor();
      return true;
    }

    private void SetCursor(Cursor newCursor)
    {
      if (this.Cursor == newCursor)
        return;
      this.lastCursor = this.Cursor;
      this.Cursor = newCursor;
    }

    private void SetLastCursor()
    {
      if (this.lastCursor == null)
        return;
      this.Cursor = this.lastCursor;
      this.lastCursor = (Cursor) null;
    }

    private bool CanDuplicate(bool testLastHovered)
    {
      bool flag = false;
      if (this.isDuplicationEnabled)
      {
        List<SceneElement> list = new List<SceneElement>();
        list.AddRange((IEnumerable<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection);
        if (testLastHovered && this.lastHoveredElement != null)
          list.Add(this.lastHoveredElement);
        if (list.Count > 0 && !this.ActiveSceneViewModel.AnimationEditor.IsKeyFraming)
        {
          SceneElement parentElement = list[0].ParentElement;
          if (parentElement != null && !(parentElement is Viewport3DElement) && !(parentElement is ModelVisual3DElement))
          {
            ISceneInsertionPoint defaultInsertionPoint = parentElement.DefaultInsertionPoint;
            if (defaultInsertionPoint != null)
            {
              ISceneNodeCollection<SceneNode> collectionForProperty = defaultInsertionPoint.SceneNode.GetCollectionForProperty((IPropertyId) defaultInsertionPoint.Property);
              if (!collectionForProperty.FixedCapacity.HasValue || collectionForProperty.Count < collectionForProperty.FixedCapacity.Value)
              {
                flag = true;
                for (int index = 1; index < list.Count; ++index)
                {
                  if (list[index].ParentElement != parentElement)
                  {
                    flag = false;
                    break;
                  }
                }
              }
            }
          }
        }
      }
      return flag;
    }

    private Vector DuplicateSelectedElements()
    {
      Vector vector = new Vector();
      if (this.CanDuplicate(false))
      {
        this.EnsureEditTransaction();
        ReadOnlyCollection<SceneElement> selection = this.ActiveSceneViewModel.ElementSelectionSet.Selection;
        List<SceneElement> elements = new List<SceneElement>((IEnumerable<SceneElement>) selection);
        elements.Sort((IComparer<SceneElement>) new ZOrderComparer<SceneElement>(this.ActiveSceneViewModel.RootNode));
        SceneElement sceneElement1 = this.lastClickedElement;
        int index = elements.IndexOf(sceneElement1);
        PastePackage pastePackage = new PastePackage(this.ActiveSceneViewModel);
        using (this.ActiveSceneViewModel.ForceBaseValue())
          pastePackage.AddElements(elements);
        SceneElement sceneElement2 = (SceneElement) selection[0].Parent;
        bool canceledPasteOperation;
        ICollection<SceneNode> nodes = PasteCommand.PasteData(this.ActiveSceneViewModel, new SafeDataObject((IDataObject) pastePackage.GetPasteDataObject()), sceneElement2.DefaultInsertionPoint, out canceledPasteOperation);
        this.UpdateEditTransaction();
        this.ActiveView.UpdateLayout();
        if (nodes.Count > 0)
        {
          this.ActiveSceneViewModel.ClearSelections();
          this.ActiveSceneViewModel.SelectNodes(nodes);
        }
        if (index >= 0 && index < this.ActiveSceneViewModel.ElementSelectionSet.Selection.Count)
        {
          SceneElement sceneElement3 = this.ActiveSceneViewModel.ElementSelectionSet.Selection[index];
          Base2DElement base2Delement1 = sceneElement1 as Base2DElement;
          Base2DElement base2Delement2 = sceneElement3 as Base2DElement;
          Base2DElement context = sceneElement2 as Base2DElement;
          if (base2Delement1 != null && base2Delement2 != null && context != null)
            vector = base2Delement2.GetComputedBounds(context).TopLeft - base2Delement1.GetComputedBounds(context).TopLeft;
        }
      }
      return vector;
    }

    private void SetMoveBehaviorReparenting(bool enableReparenting)
    {
      RelocateBehavior relocateBehavior = this.moveBehavior as RelocateBehavior;
      if (relocateBehavior == null)
        return;
      relocateBehavior.EnableReparenting = enableReparenting;
    }

    private void SetDuplicationOffset(Vector offset)
    {
      RelocateBehavior relocateBehavior = this.moveBehavior as RelocateBehavior;
      if (relocateBehavior == null)
        return;
      relocateBehavior.DuplicationOffset = offset;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      if (this.isRubberbanding && !(this.moveBehavior is BrushBackgroundBehavior))
        return base.ShouldMotionlessAutoScroll(mousePoint, artboardBoundary);
      return false;
    }

    protected override MotionlessAutoScroller CreateMotionlessAutoScroller()
    {
      return new MotionlessAutoScroller((ToolBehavior) this, (Func<Point, Point, bool, bool>) ((start, current, scrollNow) =>
      {
        this.UpdateSelectionBox(current, scrollNow);
        return true;
      }));
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      Point? hitPoint = new Point?();
      if (!this.HasMouseMovedAfterDown)
      {
        this.isAltDownBeforeDrag = this.IsAltDown;
        this.SetMoveBehaviorReparenting(!this.isAltDownBeforeDrag && !this.ActiveSceneViewModel.AnimationEditor.IsRecording);
        if (this.IsControlDown && this.IsWithinSelectionTool && this.CanUseRubberBanding)
        {
          this.isRubberbanding = true;
          this.isToggling = false;
        }
        else
        {
          SceneElement selectableElementAtPoint = this.ActiveView.GetSelectableElementAtPoint(dragStartPosition, this.SelectionFor3D, true);
          if (selectableElementAtPoint == null)
          {
            this.DoElementSelection(this.lastClickedElement);
          }
          else
          {
            this.lastClickedElement = selectableElementAtPoint;
            hitPoint = new Point?(dragStartPosition);
            this.ActiveSceneViewModel.PathPartSelectionSet.Clear();
          }
        }
      }
      this.PreviewElement = (SceneElement) null;
      if (this.isRubberbanding && this.CanUseRubberBanding)
      {
        this.SetCursor(ToolCursors.MarqueeSelectCursor);
        this.UpdateSelectionBox(dragCurrentPosition, scrollNow);
      }
      else if (this.lastClickedElement != null && (this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(this.lastClickedElement) || this.isToggling || this.isExtending) || !this.CanUseRubberBanding)
      {
        if (!this.HasMouseMovedAfterDown && (this.isToggling || this.isExtending) && !this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(this.lastClickedElement))
          this.DoElementSelection(this.lastClickedElement, hitPoint);
        Vector offset = new Vector();
        if (this.IsAltDown)
          offset = this.DuplicateSelectedElements();
        if (this.is3DSelectionEnabled && this.lastClickedElement is Base3DElement)
          this.PushBehavior((ToolBehavior) new ObjectRotateTranslateBehavior(this.ToolBehaviorContext, true));
        else if (this.moveBehavior != null)
        {
          this.SetDuplicationOffset(offset);
          this.PushBehavior(this.moveBehavior);
        }
      }
      else
        this.PushConvertAnchorBehaviorIfAltIsDown();
      return true;
    }

    private void PushConvertAnchorBehaviorIfAltIsDown()
    {
      if (!(this.Tool is SubselectionTool) || this.isRubberbanding || !this.IsAltDown)
        return;
      this.PushBehavior((ToolBehavior) new ConvertAnchorBehavior(this.ToolBehaviorContext));
    }

    protected override void OnResume()
    {
      base.OnResume();
      this.SetMoveBehaviorReparenting(!this.isAltDownBeforeDrag);
      if (this.moveBehavior is RelocateBehavior && ((RelocateBehavior) this.moveBehavior).DragCancelled)
      {
        this.CancelEditTransaction();
      }
      else
      {
        if (!this.IsEditTransactionOpen)
          return;
        this.CommitEditTransaction();
      }
    }

    protected override bool OnRightButtonDown(Point pointerPosition)
    {
      ContextMenuHelper.ContextMenuType menuType = this.IsControlDown ? ContextMenuHelper.ContextMenuType.SelectionContextMenu : ContextMenuHelper.ContextMenuType.PrimaryContextMenu;
      this.DisplayContextMenu(pointerPosition, menuType);
      this.lastClickTime = DateTime.Now;
      return true;
    }

    private void DisplayContextMenu(Point pointerPosition, ContextMenuHelper.ContextMenuType menuType)
    {
      this.PreviewElement = (SceneElement) null;
      Point? hitPoint = new Point?();
      IAdorner hitAdorner = this.ActiveView.AdornerService.GetHitAdorner(this.MouseDevice);
      AnnotationAdorner annotationAdorner = hitAdorner as AnnotationAdorner;
      if (annotationAdorner != null)
      {
        annotationAdorner.DisplayContextMenu(pointerPosition);
      }
      else
      {
        if (hitAdorner is BoundingBoxAdorner)
        {
          this.lastClickedElement = hitAdorner.ElementSet.PrimaryElement;
        }
        else
        {
          this.lastClickedElement = this.ActiveView.GetSelectableElementAtPoint(pointerPosition, this.SelectionFor3D, true);
          if (this.lastClickedElement == null)
            this.lastClickedElement = this.ActiveView.GetSelectableElementAtPoint(pointerPosition, this.SelectionFor3D, false);
          hitPoint = new Point?(pointerPosition);
        }
        if (this.lastClickedElement == null)
          return;
        this.DoElementSelection(this.lastClickedElement, false, false, hitPoint);
        ISelectionSet<SceneElement> selection = (ISelectionSet<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet;
        if (selection.IsEmpty)
          return;
        FrameworkElement sceneScrollViewer = this.ActiveView.SceneScrollViewer;
        Point contextMenuPosition = this.ActiveView.ViewRootContainer.TransformToAncestor((Visual) sceneScrollViewer).Transform(pointerPosition);
        ContextMenuHelper.InvokeContextMenu((UIElement) sceneScrollViewer, selection, this.ActiveSceneViewModel, contextMenuPosition, menuType);
      }
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.isRubberbanding)
      {
        this.DoRubberbandSelection(dragEndPosition);
        if (!this.IsControlDown || !this.IsWithinSelectionTool)
          this.SetLastCursor();
      }
      this.UpdateCursor();
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      bool flag = false;
      if (clickCount > 1 && !this.IsControlDown && (!this.IsShiftDown && !this.IsAltDown))
      {
        SceneElement sceneElement1 = this.ActiveSceneViewModel.LockedInsertionPoint != null ? this.ActiveSceneViewModel.LockedInsertionPoint.SceneElement : (SceneElement) null;
        if (sceneElement1 != null)
        {
          Point point = this.ActiveView.TransformPoint((IViewObject) this.ActiveView.HitTestRoot, sceneElement1.Visual, pointerPosition);
          if (!this.ActiveView.GetActualBounds(sceneElement1.ViewTargetElement).Contains(point))
          {
            this.ActiveSceneViewModel.SetLockedInsertionPoint((SceneElement) null);
            flag = true;
          }
        }
        else if (this.ActiveSceneViewModel.CanPopActiveEditingContainer)
        {
          SceneElement sceneElement2 = this.ActiveSceneViewModel.ActiveEditingContainer as SceneElement;
          if (sceneElement2 != null && sceneElement2.IsViewObjectValid)
          {
            IViewObject element = sceneElement2.ViewTargetElement;
            Point point;
            if (sceneElement2.Visual == null && sceneElement2 is FrameworkTemplateElement)
            {
              element = (IViewObject) this.ActiveView.HitTestRoot;
              point = pointerPosition;
            }
            else
              point = this.ActiveView.TransformPoint((IViewObject) this.ActiveView.HitTestRoot, sceneElement2.Visual, pointerPosition);
            if (element != null && !this.ActiveView.GetActualBounds(element).Contains(point))
            {
              this.ActiveSceneViewModel.PopActiveEditingContainer();
              flag = true;
            }
          }
        }
        else
        {
          SceneElement selectableElementAtPoint = this.ActiveView.GetSelectableElementAtPoint(pointerPosition, SelectionFor3D.None, true);
          if (selectableElementAtPoint != null && selectableElementAtPoint != this.ActiveSceneViewModel.RootNode && selectableElementAtPoint.Type.XamlSourcePath != null)
            EditControlCommand.EditControl(selectableElementAtPoint);
        }
      }
      if (!flag)
      {
        if (this.isRubberbanding && !this.isExtending)
          this.DoClearSelection();
        else if (this.isCycling)
        {
          this.isCycling = false;
          this.DoElementSelection(this.GetNextSelectableElement(pointerPosition, true) ?? this.lastClickedElement, new Point?(pointerPosition));
        }
        else if (this.isEnteringTextEdit)
          this.DoEnterTextEdit();
        else
          this.DoElementSelection(this.lastClickedElement, new Point?(pointerPosition));
      }
      this.isRubberbanding = false;
      this.isToggling = false;
      this.isExtending = false;
      this.UpdateCursor();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      AnnotationSelectionSet annotationSelectionSet = this.ActiveSceneViewModel.DesignerContext.SelectionManager.AnnotationSelectionSet;
      if (!annotationSelectionSet.IsEmpty)
      {
        AnnotationVisual visual = annotationSelectionSet.PrimarySelection.Visual;
        if (visual != null && visual.ProcessKey(args))
          return true;
      }
      Key key = args.Key == Key.System ? args.SystemKey : args.Key;
      bool flag1 = key == Key.LeftAlt || key == Key.RightAlt;
      bool flag2 = key == Key.LeftShift || key == Key.RightShift;
      bool flag3 = key == Key.LeftCtrl || key == Key.RightCtrl;
      if (args.IsDown && (flag1 && !this.IsControlDown || flag3 && this.IsAltDown) && (this.Tool.AdornerOwnerTool is SubselectionTool && !this.isRubberbanding))
      {
        this.PushBehavior((ToolBehavior) new ConvertAnchorBehavior(this.ToolBehaviorContext));
        return true;
      }
      if (args.IsDown && (key == Key.Left || key == Key.Right || (key == Key.Up || key == Key.Down)) && !this.isRubberbanding)
      {
        this.Nudge(key);
        args.Handled = true;
        return true;
      }
      if (!args.IsRepeat)
      {
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
          if (flag2)
            this.isExtending = args.IsDown;
          else if (this.IsWithinSelectionTool && flag3)
            this.isToggling = args.IsDown;
        }
        if (args.IsDown && flag3)
        {
          if (this.CanUseRubberBanding && this.IsWithinSelectionTool)
            this.SetCursor(ToolCursors.MarqueeSelectCursor);
          return true;
        }
        if (args.IsUp && flag3)
        {
          if (this.CanUseRubberBanding && !this.isRubberbanding && this.IsWithinSelectionTool)
            this.SetLastCursor();
          return true;
        }
        if (flag1 && !this.isRubberbanding)
        {
          if (this.CanDuplicate(true) && this.lastHoveredElement != null)
          {
            if (args.IsDown)
            {
              if (this.Cursor == this.moveCursor || this.Cursor == this.elementCursor || (this.Cursor == this.defaultCursor || this.Cursor == ToolCursors.DuplicateWedgeCursor))
                this.SetCursor(ToolCursors.DuplicateWedgeCursor);
            }
            else if (this.Cursor == ToolCursors.DuplicateWedgeCursor)
              this.SetLastCursor();
          }
          this.UpdatePreviewElement();
        }
      }
      IAdorner hitAdorner = this.ActiveView.AdornerService.GetHitAdorner(this.MouseDevice);
      if (hitAdorner != null && !this.IsDragging)
        this.Cursor = hitAdorner.AdornerSet.GetCursor(hitAdorner);
      if (!this.isRubberbanding)
        return base.OnKey(args);
      return true;
    }

    protected override void OnAttach()
    {
      base.OnAttach();
      this.isRubberbanding = false;
      this.isEnteringTextEdit = false;
    }

    protected override void OnDetach()
    {
      this.PreviewElement = (SceneElement) null;
      base.OnDetach();
    }

    private void UpdatePreviewElement()
    {
      SceneElement sceneElement = (SceneElement) null;
      if (!this.IsCycleModifierDown && this.lastHoveredElement != null && this.lastHoveredElement.IsAttached)
        sceneElement = this.lastHoveredElement;
      if (sceneElement != null && this.ActiveSceneViewModel.ElementSelectionSet.IsSelected(sceneElement))
        sceneElement = (SceneElement) null;
      this.PreviewElement = sceneElement;
    }

    private void UpdateSelectionBox(Point point, bool scrollNow)
    {
      SceneView activeView = this.ActiveView;
      activeView.EnsureVisible(point, scrollNow);
      FeedbackHelper.DrawDashedRectangle(this.OpenFeedback(), activeView.Zoom, this.startPoint, point);
      this.CloseFeedback();
    }

    private void DoElementSelection(SceneElement hitElement)
    {
      this.DoElementSelection(hitElement, this.isToggling, this.isExtending, new Point?());
    }

    private void DoElementSelection(SceneElement hitElement, Point? hitPoint)
    {
      this.DoElementSelection(hitElement, this.isToggling, this.isExtending, hitPoint);
    }

    private void DoElementSelection(SceneElement hitElement, bool isToggling, bool isExtending, Point? hitPoint)
    {
      SelectionManagerPerformanceHelper.MeasurePerformanceUntilPipelinePostSceneUpdate(this.ActiveSceneViewModel.DesignerContext.SelectionManager, PerformanceEvent.SelectElement);
      if (hitElement == null || !hitElement.IsAttached)
        return;
      SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
      if (isToggling)
        elementSelectionSet.ToggleSelection(hitElement);
      else if (isExtending)
        elementSelectionSet.ExtendSelection(hitElement);
      else if (elementSelectionSet.IsSelected(hitElement))
      {
        if (elementSelectionSet.PrimarySelection != hitElement)
        {
          elementSelectionSet.ToggleSelection(hitElement);
          elementSelectionSet.ToggleSelection(hitElement);
        }
      }
      else
        elementSelectionSet.SetSelection(hitElement);
      if (!this.isPathSubselectionEnabled || !elementSelectionSet.IsSelected(hitElement))
        return;
      PathPart pathPart = hitPoint.HasValue ? this.GetPathPartAtPoint(hitPoint.Value, hitElement) : (PathPart) null;
      if (pathPart != (PathPart) null)
      {
        if (isToggling)
          this.ActiveSceneViewModel.PathPartSelectionSet.ToggleSelection(pathPart, false);
        else if (isExtending)
          this.ActiveSceneViewModel.PathPartSelectionSet.ExtendSelection(pathPart, false);
        else
          this.ActiveSceneViewModel.PathPartSelectionSet.SetSelection(pathPart, false);
      }
      else
        this.ActiveSceneViewModel.PathPartSelectionSet.RemoveSelection(this.ActiveSceneViewModel.PathPartSelectionSet.GetSelectionByElement(hitElement));
    }

    private void HandleEnterTextEdit(SceneElement oldClickedElement, SceneElement hitElement)
    {
      bool flag = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(1500.0)).CompareTo(this.lastClickTime) < 0;
      if (hitElement != oldClickedElement || this.ActiveSceneViewModel.ElementSelectionSet.Count != 1 || (hitElement != this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection || !flag) || (!this.isTextEditingEnabled || !TextEditProxyFactory.IsEditableElement(hitElement)))
        return;
      this.isEnteringTextEdit = true;
    }

    private void DoEnterTextEdit()
    {
      if (!this.isEnteringTextEdit)
        return;
      this.PushBehavior((ToolBehavior) new TextToolBehavior(this.ToolBehaviorContext, (ToolBehavior) null));
      this.isEnteringTextEdit = false;
    }

    private void DoRubberbandSelection(Point pointerPosition)
    {
      SelectionManagerPerformanceHelper.MeasurePerformanceUntilPipelinePostSceneUpdate(this.ActiveSceneViewModel.DesignerContext.SelectionManager, PerformanceEvent.SelectElement);
      IList<SceneElement> elementsIntersectingBox = this.GetElementsIntersectingBox(this.startPoint, pointerPosition);
      IEnumerable<SceneElement> elementList = Enumerable.Union<SceneElement>((IEnumerable<SceneElement>) elementsIntersectingBox, (IEnumerable<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection);
      IList<PathPart> partsIntersectingBox = this.GetPathPartsIntersectingBox(this.startPoint, pointerPosition, elementList);
      foreach (PathPart pathPart in (IEnumerable<PathPart>) partsIntersectingBox)
      {
        if (!elementsIntersectingBox.Contains(pathPart.SceneElement))
          elementsIntersectingBox.Add(pathPart.SceneElement);
      }
      bool isShiftDown = this.IsShiftDown;
      if (!this.IsAltDown)
      {
        if (isShiftDown)
        {
          this.ActiveSceneViewModel.ElementSelectionSet.ExtendSelection((ICollection<SceneElement>) elementsIntersectingBox);
          foreach (PathPart selectionToExtend in (IEnumerable<PathPart>) partsIntersectingBox)
            this.ActiveSceneViewModel.PathPartSelectionSet.ExtendSelection(selectionToExtend);
        }
        else if (elementsIntersectingBox.Count == 0)
        {
          this.DoClearSelection();
        }
        else
        {
          this.ActiveSceneViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) elementsIntersectingBox, (SceneElement) null);
          this.ActiveSceneViewModel.PathPartSelectionSet.SetSelection((ICollection<PathPart>) partsIntersectingBox, false);
        }
      }
      this.isRubberbanding = false;
      this.ClearFeedback();
      this.PreviewElement = (SceneElement) null;
    }

    private void DoClearSelection()
    {
      this.ActiveSceneViewModel.ElementSelectionSet.Clear();
      this.ActiveSceneViewModel.PathPartSelectionSet.Clear();
      this.ActiveSceneViewModel.AnnotationSelectionSet.Clear();
      if (this.ActiveSceneViewModel.GridColumnSelectionSet.Selection.Count <= 0 && this.ActiveSceneViewModel.GridRowSelectionSet.Selection.Count <= 0)
        return;
      this.ActiveSceneViewModel.GridColumnSelectionSet.Clear();
      this.ActiveSceneViewModel.GridRowSelectionSet.Clear();
      this.ActiveSceneViewModel.RefreshSelection();
    }

    private PathPart GetPathPartAtPoint(Point point, SceneElement hitElement)
    {
      List<PathPart> list = new List<PathPart>();
      Vector vector = new Vector(2.0, 2.0);
      this.GetPathPartsIntersectingElement(hitElement, new Rect(point - vector, point + vector), (IList<PathPart>) list, true);
      if (list.Count <= 0)
        return (PathPart) null;
      foreach (PathPart pathPart in list)
      {
        if (pathPart is PathPoint)
          return pathPart;
      }
      return list[0];
    }

    private IList<PathPart> GetPathPartsIntersectingBox(Point firstPoint, Point secondPoint, IEnumerable<SceneElement> elementList)
    {
      List<PathPart> list = new List<PathPart>();
      foreach (SceneElement sceneElement in elementList)
        this.GetPathPartsIntersectingElement(sceneElement, new Rect(firstPoint, secondPoint), (IList<PathPart>) list, false);
      return (IList<PathPart>) list;
    }

    private void GetPathPartsIntersectingElement(SceneElement sceneElement, Rect hitRect, IList<PathPart> pathParts, bool includePartialSegmentHits)
    {
      foreach (PathEditorTarget pathEditorTarget in this.Tool.GetAllPathEditorTargets(sceneElement))
      {
        if (pathEditorTarget.PathGeometry.Figures.Count > 0)
        {
          SceneView activeView = this.ActiveView;
          Point point1 = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, sceneElement.Visual, hitRect.TopLeft);
          Point point2 = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, sceneElement.Visual, hitRect.BottomRight);
          Point point3 = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, sceneElement.Visual, hitRect.TopRight);
          Point point4 = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, sceneElement.Visual, hitRect.BottomLeft);
          if (sceneElement.ViewTargetElement != null)
          {
            Matrix m = pathEditorTarget.GetTransformToAncestor(sceneElement.ViewTargetElement);
            m = ElementUtilities.GetInverseMatrix(m);
            point1 = m.Transform(point1);
            point2 = m.Transform(point2);
            point3 = m.Transform(point3);
            point4 = m.Transform(point4);
          }
          StreamGeometry streamGeometry = new StreamGeometry();
          StreamGeometryContext streamGeometryContext = streamGeometry.Open();
          streamGeometryContext.BeginFigure(point1, true, true);
          streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
          {
            point3,
            point2,
            point4
          }, 1 != 0, 0 != 0);
          streamGeometryContext.Close();
          streamGeometry.Freeze();
          PathGeometry geometry = pathEditorTarget.PathGeometry.CloneCurrentValue();
          PathGeometryUtilities.EnsureOnlySingleSegmentsInGeometry(geometry);
          for (int figureIndex = 0; figureIndex < geometry.Figures.Count; ++figureIndex)
          {
              foreach (PathSelectionContext selectionContext in new PathFigureEditor(geometry.Figures[figureIndex]).HitDetect((System.Windows.Media.Geometry)streamGeometry, figureIndex, includePartialSegmentHits))
            {
              PathPart pathPart = !selectionContext.IsSegmentSelected ? (PathPart) new PathPoint(sceneElement, pathEditorTarget.PathEditMode, figureIndex, selectionContext.PointIndex) : (PathPart) new Microsoft.Expression.DesignSurface.Tools.Path.PathSegment(sceneElement, pathEditorTarget.PathEditMode, figureIndex, selectionContext.SegmentIndex);
              pathParts.Add(pathPart);
            }
          }
        }
      }
    }

    private IList<SceneElement> GetElementsIntersectingBox(Point firstPoint, Point secondPoint)
    {
      IList<SceneElement> hitElements = this.ActiveView.GetSelectableElementsInRectangle(new Rect(firstPoint, secondPoint), this.SelectionFor3D, false, true);
      if (this.isRubberbanding && !this.isDeepMarqueeEnabled)
        hitElements = new ElementEditorBehavior.SameLevelSort(hitElements).Process();
      return hitElements;
    }

    private SceneElement GetNextSelectableElement(Point pointerPosition, bool isClicking)
    {
      return new ElementEditorBehavior.NextSelectedElementHelper(this).GetNextSelectableElement(pointerPosition, isClicking);
    }

    private void Nudge(Key key)
    {
      double num = (this.IsShiftDown ? 10.0 : 1.0) / this.ActiveView.Zoom;
      LayoutOverrides overrides = LayoutOverrides.None;
      if (Enumerable.Any<Base2DElement>(Enumerable.OfType<Base2DElement>((IEnumerable) this.ActiveSceneViewModel.ElementSelectionSet.Selection), (Func<Base2DElement, bool>) (primary2D => (LayoutRoundingHelper.GetLayoutRoundingStatus((SceneElement) primary2D) & LayoutRoundingStatus.ShouldSnapToPixel) != LayoutRoundingStatus.Off)))
        num = Math.Max(1.0, Math.Round(num));
      Vector vector;
      switch (key)
      {
        case Key.Left:
          vector = new Vector(-num, 0.0);
          overrides |= LayoutOverrides.CenterHorizontalAlignment;
          break;
        case Key.Up:
          vector = new Vector(0.0, -num);
          overrides |= LayoutOverrides.CenterVerticalAlignment;
          break;
        case Key.Right:
          vector = new Vector(num, 0.0);
          overrides |= LayoutOverrides.CenterHorizontalAlignment;
          break;
        case Key.Down:
          vector = new Vector(0.0, num);
          overrides |= LayoutOverrides.CenterVerticalAlignment;
          break;
        default:
          vector = new Vector();
          break;
      }
      if (this.IsAltDown)
        this.DuplicateSelectedElements();
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitNudge))
      {
        PathPartSelectionSet partSelectionSet = this.ActiveSceneViewModel.PathPartSelectionSet;
        if (partSelectionSet.Count > 0 && this.isPathSubselectionEnabled)
        {
          foreach (BaseFrameworkElement sceneElement in (IEnumerable<SceneElement>) partSelectionSet.SelectedPaths)
          {
            this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.ScenePath, vector);
            this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.MotionPath, vector);
            this.MovePathParts(partSelectionSet, sceneElement, PathEditMode.ClippingPath, vector);
          }
        }
        else
        {
          for (int index = 0; index < this.ActiveSceneViewModel.ElementSelectionSet.Count; ++index)
          {
            BaseFrameworkElement element = this.ActiveSceneViewModel.ElementSelectionSet.Selection[index] as BaseFrameworkElement;
            if (element != null)
              ElementEditorBehavior.TranslateElement(element, vector, overrides);
          }
        }
        editTransaction.Commit();
      }
      if (!this.IsEditTransactionOpen)
        return;
      this.CommitEditTransaction();
    }

    private void MovePathParts(PathPartSelectionSet pathPartSelectionSet, BaseFrameworkElement sceneElement, PathEditMode pathEditMode, Vector deltaOffset)
    {
      ICollection<PathPart> selectionByElement = pathPartSelectionSet.GetSelectionByElement((SceneElement) sceneElement, pathEditMode);
      Tool activeTool = this.ActiveSceneViewModel.DesignerContext.ToolManager.ActiveTool;
      if (selectionByElement.Count <= 0 || activeTool == null)
        return;
      PathEditorTarget pathEditorTarget = activeTool.GetPathEditorTarget((Base2DElement) sceneElement, pathEditMode);
      if (pathEditorTarget == null)
        return;
      pathEditorTarget.BeginEditing();
      Matrix transformToAncestor = pathEditorTarget.GetTransformToAncestor((IViewObject) this.ActiveView.HitTestRoot);
      Vector correspondingVector = ElementUtilities.GetCorrespondingVector(deltaOffset, transformToAncestor);
      PathEditBehavior.TranslateSelection(pathEditorTarget, selectionByElement, correspondingVector);
      pathEditorTarget.EndEditing(false);
      this.ActiveView.AdornerLayer.InvalidateAdornerVisuals((SceneElement) pathEditorTarget.EditingElement);
    }

    internal static void TranslateElement(BaseFrameworkElement element, Vector offset, LayoutOverrides overrides)
    {
      Rect childRect = element.ViewModel.GetLayoutDesignerForChild((SceneElement) element, false).GetChildRect(element);
      if (element.ViewModel.UsingEffectDesigner)
      {
        Point elementCoordinates = element.RenderTransformOriginInElementCoordinates;
        Point point = new CanonicalTransform(element.GetEffectiveRenderTransform(false))
        {
          SkewX = 0.0,
          SkewY = 0.0,
          RotationAngle = 0.0
        }.TransformGroup.Transform(elementCoordinates);
        childRect.Offset(elementCoordinates - point);
      }
      childRect.Offset(offset);
      element.ViewModel.GetLayoutDesignerForChild((SceneElement) element, false).SetChildRect(element, childRect, LayoutOverrides.RecomputeDefault, overrides, LayoutOverrides.None);
    }

    private class PreviewElementAdornerSet : AnimatableAdornerSet
    {
      public PreviewElementAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement, AdornerSetOrderTokens.BoundingBoxPriority)
      {
      }

      protected override void CreateAdorners()
      {
        this.AddAdorner((Adorner) new SelectionPreviewBoundingBoxAdorner((AdornerSet) this));
        if (!(bool) this.Element.GetLocalOrDefaultValue(DesignTimeProperties.IsPrototypingCompositionProperty))
          return;
        this.AddAdorner((Adorner) new PrototypingCompositionBadgeAdorner((AdornerSet) this));
      }
    }

    private class SameLevelSort
    {
      private IList<SceneElement> elements;
      private Stack<SceneNode> parentChain;
      private int currentItem;

      public SameLevelSort(IList<SceneElement> hitElements)
      {
        this.elements = hitElements;
        this.parentChain = new Stack<SceneNode>();
        if (hitElements.Count <= 0)
          return;
        this.parentChain.Push((SceneNode) this.elements[0].EffectiveParent);
        ++this.currentItem;
      }

      private void RemoveSubBranch(SceneNode parent)
      {
        for (int index = this.currentItem; index >= 0; --index)
        {
          if (parent == this.elements[index] || parent.IsAncestorOf((SceneNode) this.elements[index]))
          {
            this.elements.RemoveAt(index);
            if (index < this.currentItem)
              --this.currentItem;
          }
        }
        if (this.currentItem >= 0)
          return;
        this.currentItem = 0;
      }

      private void PruneBelowParentChainContainer()
      {
        SceneElement sceneElement = this.parentChain.Peek() as SceneElement;
        this.RemoveSubBranch((SceneNode) sceneElement);
        this.elements.Insert(this.currentItem, sceneElement);
        this.parentChain.Pop();
        this.parentChain.Push((SceneNode) sceneElement.EffectiveParent);
      }

      public IList<SceneElement> Process()
      {
        for (; this.currentItem < this.elements.Count; ++this.currentItem)
        {
          if (this.elements[this.currentItem].EffectiveParent != this.parentChain.Peek())
          {
            if (this.elements[this.currentItem].EffectiveParent == null || this.elements[this.currentItem].EffectiveParent.IsAncestorOf(this.parentChain.Peek()))
              this.PruneBelowParentChainContainer();
            else if (this.parentChain.Peek().IsAncestorOf((SceneNode) this.elements[this.currentItem].EffectiveParent))
            {
              SceneElement effectiveParent = this.elements[this.currentItem].EffectiveParent;
              this.elements.RemoveAt(this.currentItem);
              if (this.currentItem >= this.elements.Count || this.elements[this.currentItem] != effectiveParent)
                this.elements.Insert(this.currentItem, effectiveParent);
              this.parentChain.Push((SceneNode) effectiveParent);
              --this.currentItem;
            }
            else if (this.elements[this.currentItem] == this.parentChain.Peek())
            {
              this.RemoveSubBranch((SceneNode) this.elements[this.currentItem]);
              this.parentChain.Pop();
            }
            else if (this.elements[this.currentItem].EffectiveParent != this.parentChain.Peek())
              this.PruneBelowParentChainContainer();
          }
        }
        return this.elements;
      }
    }

    private class NextSelectedElementHelper
    {
      private ElementEditorBehavior elementEditorBehavior;
      private bool selectionHit;
      private SceneElement firstUnselected;
      private SceneElement topElement;

      public NextSelectedElementHelper(ElementEditorBehavior elementEditorBehavior)
      {
        this.elementEditorBehavior = elementEditorBehavior;
      }

      public SceneElement GetNextSelectableElement(Point pointerPosition, bool isClicking)
      {
        SceneElement sceneElement = this.elementEditorBehavior.ActiveView.GetElementAtPoint(pointerPosition, new HitTestModifier(this.GetNextSelectableElement), (InvisibleObjectHitTestModifier) null, (ICollection<BaseFrameworkElement>) null);
        if (this.elementEditorBehavior.lastClickedTopElement != this.topElement && (this.topElement == null || !this.elementEditorBehavior.ActiveSceneViewModel.ElementSelectionSet.IsSelected(this.topElement)))
          sceneElement = (SceneElement) null;
        if (isClicking)
          this.elementEditorBehavior.lastClickedTopElement = this.topElement;
        if (sceneElement == null && this.selectionHit && this.firstUnselected != null)
          return this.firstUnselected;
        return sceneElement;
      }

      private SceneElement GetNextSelectableElement(DocumentNodePath nodePath)
      {
        SceneElement selectableElement = this.elementEditorBehavior.ActiveView.GetSelectableElement(nodePath);
        if (selectableElement != null && selectableElement.IsVisuallySelectable)
        {
          if (this.topElement == null)
            this.topElement = selectableElement;
          if (this.elementEditorBehavior.ActiveSceneViewModel.ElementSelectionSet.IsSelected(selectableElement))
          {
            this.selectionHit = true;
            return (SceneElement) null;
          }
          if (this.firstUnselected == null)
            this.firstUnselected = selectableElement;
          if (this.selectionHit)
            return selectableElement;
        }
        return (SceneElement) null;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public abstract class ToolBehavior : IToolBehaviorTransaction
  {
    private ToolBehaviorContext toolBehaviorContext;
    private EventRouter eventRouter;
    private SceneEditTransaction transaction;
    private Stack<SceneEditTransaction> subTransactionStack;
    private MotionlessAutoScroller motionlessAutoScroller;
    private ISceneInsertionPoint sceneInsertionPoint;
    private DrawingContext feedbackContext;

    public bool IsSuspended
    {
      get
      {
        if (this.eventRouter != null)
          return this.eventRouter.ActiveBehavior != this;
        return true;
      }
    }

    internal ToolBehaviorContext ToolBehaviorContext
    {
      get
      {
        return this.toolBehaviorContext;
      }
    }

    public Tool Tool
    {
      get
      {
        return this.toolBehaviorContext.Tool;
      }
    }

    public SceneView ActiveView
    {
      get
      {
        return this.toolBehaviorContext.View;
      }
    }

    public SceneDocument ActiveDocument
    {
      get
      {
        return this.ActiveView.Document;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.ActiveView.ViewModel;
      }
    }

    public ISceneInsertionPoint LocalActiveSceneInsertionPoint
    {
      get
      {
        return this.sceneInsertionPoint;
      }
      set
      {
        this.sceneInsertionPoint = value;
      }
    }

    public virtual ISceneInsertionPoint ActiveSceneInsertionPoint
    {
      get
      {
        if (this.sceneInsertionPoint == null || !this.sceneInsertionPoint.SceneElement.IsAttached)
          return this.ActiveSceneViewModel.ActiveSceneInsertionPoint;
        return this.sceneInsertionPoint;
      }
    }

    public SceneNode RootNode
    {
      get
      {
        return this.ActiveSceneViewModel.RootNode;
      }
    }

    public Cursor Cursor
    {
      get
      {
        return this.ActiveView.ServiceRoot.GetValue(FrameworkElement.CursorProperty) as Cursor;
      }
      set
      {
        this.ActiveView.ServiceRoot.SetValue(FrameworkElement.CursorProperty, (object) value);
      }
    }

    protected bool IsButtonDown
    {
      get
      {
        return this.eventRouter.IsButtonDown;
      }
    }

    protected bool IsDragging
    {
      get
      {
        return this.eventRouter.IsDragging;
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

    protected bool IsSpaceDown
    {
      get
      {
        return InputManager.Current.PrimaryKeyboardDevice.IsKeyDown(Key.Space);
      }
    }

    public MouseDevice MouseDevice
    {
      get
      {
        if (InputManager.Current != null)
          return InputManager.Current.PrimaryMouseDevice;
        return (MouseDevice) null;
      }
    }

    public bool HasMouseMovedAfterDown { get; private set; }

    protected SceneEditTransaction EditTransaction
    {
      get
      {
        return this.transaction;
      }
    }

    protected bool IsEditTransactionOpen
    {
      get
      {
        return this.transaction != null;
      }
    }

    public virtual bool ShouldCapture
    {
      get
      {
        return true;
      }
    }

    public virtual bool UseDefaultEditingAdorners
    {
      get
      {
        return true;
      }
    }

    public virtual string ActionString
    {
      get
      {
        return string.Empty;
      }
    }

    protected bool IsOnlyUpdatingCursor { get; private set; }

    internal ToolBehavior(ToolBehaviorContext toolContext)
    {
      this.toolBehaviorContext = toolContext;
    }

    protected void PushBehavior(ToolBehavior toolBehavior)
    {
      if (toolBehavior == null)
        throw new ArgumentNullException("toolBehavior");
      if (this.eventRouter == null || this.eventRouter.ActiveBehavior != this)
        throw new InvalidOperationException(ExceptionStringTable.ToolBehaviorCannotPushUnlessActive);
      toolBehavior.sceneInsertionPoint = this.sceneInsertionPoint;
      this.eventRouter.PushBehavior(toolBehavior);
    }

    protected void PopSelf()
    {
      if (this.eventRouter == null || this.eventRouter.ActiveBehavior != this)
        throw new InvalidOperationException(ExceptionStringTable.ToolBehaviorCannotPopUnlessActive);
      this.eventRouter.PopBehavior();
    }

    protected bool TryPopSelf()
    {
      if (this.eventRouter == null || this.eventRouter.ActiveBehavior != this || this.eventRouter.BehaviorStackCount <= 1)
        return false;
      this.eventRouter.PopBehavior();
      return true;
    }

    protected void EnsureEditTransaction()
    {
      if (this.transaction != null)
        return;
      this.transaction = this.ActiveDocument.CreateEditTransaction(this.ActionString);
    }

    protected void EnsureEditTransaction(bool hidden)
    {
      if (this.transaction != null)
        return;
      this.transaction = this.ActiveDocument.CreateEditTransaction(this.ActionString, hidden);
    }

    protected void CancelEditTransaction()
    {
      if (this.transaction == null)
        return;
      while (this.subTransactionStack != null && this.subTransactionStack.Count > 0)
        this.CancelSubTransaction();
      this.transaction.Cancel();
      this.transaction = (SceneEditTransaction) null;
      this.subTransactionStack = (Stack<SceneEditTransaction>) null;
    }

    protected void CommitEditTransaction()
    {
      if (this.transaction == null)
        return;
      while (this.subTransactionStack != null && this.subTransactionStack.Count > 0)
        this.CommitSubTransaction();
      this.transaction.Commit();
      this.transaction = (SceneEditTransaction) null;
      this.subTransactionStack = (Stack<SceneEditTransaction>) null;
    }

    public void UpdateEditTransaction()
    {
      this.transaction.Update();
    }

    public void CreateSubTransaction()
    {
      if (this.subTransactionStack == null)
        this.subTransactionStack = new Stack<SceneEditTransaction>();
      this.subTransactionStack.Push(this.ActiveDocument.CreateEditTransaction(string.Empty));
    }

    public void CommitSubTransaction()
    {
      this.subTransactionStack.Pop().Commit();
    }

    public void CancelSubTransaction()
    {
      this.subTransactionStack.Pop().Cancel();
    }

    public void ReplaceSubTransaction()
    {
      this.CancelSubTransaction();
      this.CreateSubTransaction();
      this.UpdateEditTransaction();
      using (this.ActiveView.AdornerLayer.SuspendUpdates())
        this.ActiveView.UpdateLayout();
    }

    protected virtual void OnAttach()
    {
    }

    protected virtual void OnResume()
    {
    }

    protected virtual void OnSuspend()
    {
      this.ToolBehaviorContext.SnappingEngine.Stop();
    }

    protected virtual void OnDetach()
    {
    }

    protected virtual bool OnHover(Point pointerPosition)
    {
      return false;
    }

    protected virtual bool OnHoverExit()
    {
      return false;
    }

    protected virtual bool OnButtonDown(Point pointerPosition)
    {
      return false;
    }

    protected virtual bool OnRightButtonDown(Point pointerPosition)
    {
      return false;
    }

    protected virtual bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      return false;
    }

    protected virtual bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return false;
    }

    protected virtual bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      return false;
    }

    protected virtual bool OnKey(KeyEventArgs args)
    {
      if (args.IsDown && !args.IsRepeat && (!this.IsButtonDown && !this.IsControlDown) && (!this.IsAltDown && !this.IsSpaceDown))
      {
        if (args.Key == Key.F2)
          return this.ToolBehaviorContext.View.TryEnterTextEditMode(false);
        foreach (Tool tool in this.ToolBehaviorContext.ToolManager.Tools)
        {
          if (args.Key == tool.Key && tool.IsEnabled && tool.IsVisible)
          {
            this.ToolBehaviorContext.ToolManager.ActiveTool = tool;
            args.Handled = true;
            CultureManager.ClearDeadKeyBuffer();
            return true;
          }
        }
        foreach (ToolCategoryGroup toolCategoryGroup in this.ToolBehaviorContext.ToolManager.ToolCategoryGroups)
        {
          if (args.Key == toolCategoryGroup.Key && toolCategoryGroup.Activate(this.ToolBehaviorContext.ToolManager))
          {
            args.Handled = true;
            CultureManager.ClearDeadKeyBuffer();
            return true;
          }
        }
      }
      return false;
    }

    protected virtual bool OnDragOver(DragEventArgs args)
    {
      DataSchemaNodePathCollection result = (DataSchemaNodePathCollection) null;
      if (DragSourceHelper.FirstDataOfType<DataSchemaNodePathCollection>(args.Data, ref result) && !(this is DataBindingToolBehavior))
        this.PushBehavior((ToolBehavior) new DataBindingToolBehavior(this.ToolBehaviorContext));
      else if (args.Data.GetDataPresent("ResourceEntryItem", true) && !(this is ResourceToolBehavior))
        this.PushBehavior((ToolBehavior) new ResourceToolBehavior(this.ToolBehaviorContext));
      else if (args.Data.GetDataPresent(DataFormats.FileDrop) || args.Data.GetDataPresent("BlendProjectItem") && !(this is FileDropToolBehavior))
        this.PushBehavior((ToolBehavior) new FileDropToolBehavior(this.ToolBehaviorContext));
      else if (AssetDropToolBehavior.CanHandleDropData(args.Data) && !(this is AssetDropToolBehavior))
        this.PushBehavior((ToolBehavior) new AssetDropToolBehavior(this.ToolBehaviorContext));
      return false;
    }

    protected virtual bool OnDragEnter(DragEventArgs args)
    {
      return false;
    }

    protected virtual bool OnDragLeave(DragEventArgs args)
    {
      return false;
    }

    protected virtual bool OnDrop(DragEventArgs args)
    {
      SafeDataObject dataObject = new SafeDataObject(args.Data);
      if (PasteCommand.CanPasteData(dataObject))
      {
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitPaste))
        {
          ICollection<SceneNode> nodes = PasteCommand.PasteData(this.ActiveSceneViewModel, dataObject);
          if (nodes.Count > 0)
          {
            this.ActiveSceneViewModel.ClearSelections();
            this.ActiveSceneViewModel.SelectNodes(nodes);
          }
          editTransaction.Commit();
        }
      }
      return false;
    }

    public virtual void CommitCurrentEdit()
    {
    }

    internal virtual bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return !artboardBoundary.Contains(mousePoint);
    }

    protected virtual MotionlessAutoScroller CreateMotionlessAutoScroller()
    {
      return new MotionlessAutoScroller(this, new Func<Point, Point, bool, bool>(this.OnDrag));
    }

    public virtual void SetLocalActiveSceneInsertionPoint(Point point)
    {
      this.LocalActiveSceneInsertionPoint = this.ActiveSceneViewModel.GetActiveSceneInsertionPointFromPosition(new InsertionPointContext(point));
    }

    internal void Attach(EventRouter eventRouter)
    {
      this.eventRouter = eventRouter;
      this.OnAttach();
    }

    internal void Resume()
    {
      this.OnResume();
    }

    internal void Suspend()
    {
      this.OnSuspend();
    }

    internal void Detach()
    {
      this.OnDetach();
      this.eventRouter = (EventRouter) null;
      this.ActiveView.ServiceRoot.ClearValue(FrameworkElement.CursorProperty);
    }

    internal bool HandleHover(Point pointerPosition)
    {
      return this.OnHover(pointerPosition);
    }

    internal bool HandleHoverExit()
    {
      return this.OnHoverExit();
    }

    internal bool HandleLeftButtonDown(Point pointerPosition)
    {
      this.SetLocalActiveSceneInsertionPoint(pointerPosition);
      this.HasMouseMovedAfterDown = false;
      return this.OnButtonDown(pointerPosition);
    }

    internal bool HandleRightButtonDown(Point pointerPosition)
    {
      this.SetLocalActiveSceneInsertionPoint(pointerPosition);
      return this.OnRightButtonDown(pointerPosition);
    }

    internal bool HandleDrag(Point dragStartPosition, Point dragCurrentPosition)
    {
      PerformanceUtility.MarkInterimStep(PerformanceEvent.DragTool, "HandlingTheDragEvent");
      if (this.HasMouseMovedAfterDown && this.ShouldMotionlessAutoScroll())
      {
        this.StartMotionlessAutoScroll(dragStartPosition, dragCurrentPosition);
        return true;
      }
      this.StopMotionlessAutoScroll();
      try
      {
        return this.OnDrag(dragStartPosition, dragCurrentPosition, false);
      }
      finally
      {
        this.HasMouseMovedAfterDown = true;
      }
    }

    internal bool HandleDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.StopMotionlessAutoScroll();
      return this.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    internal bool HandleClickEnd(Point pointerPosition, int clickCount)
    {
      return this.OnClickEnd(pointerPosition, clickCount);
    }

    internal bool HandleKey(KeyEventArgs args)
    {
      return this.OnKey(args);
    }

    internal bool HandleDragOver(DragEventArgs args)
    {
      return this.OnDragOver(args);
    }

    internal bool HandleDragEnter(DragEventArgs args)
    {
      return this.OnDragEnter(args);
    }

    internal bool HandleDragLeave(DragEventArgs args)
    {
      return this.OnDragLeave(args);
    }

    internal bool HandleDrop(DragEventArgs args)
    {
      return this.OnDrop(args);
    }

    [Conditional("DEBUG")]
    private void DebugMessage(string message)
    {
    }

    protected DrawingContext OpenFeedback()
    {
      this.feedbackContext = this.ActiveView.FeedbackLayer.RenderOpen();
      return this.feedbackContext;
    }

    protected void CloseFeedback()
    {
      if (this.feedbackContext == null)
        return;
      this.feedbackContext.Close();
    }

    protected void ClearFeedback()
    {
      if (this.ActiveView == null)
        return;
      this.ActiveView.FeedbackLayer.Clear();
    }

    public IDisposable OnlyUpdatingCursor()
    {
      return (IDisposable) new ToolBehavior.OnlyUpdateCursorToken(this);
    }

    public void UpdateCursor()
    {
      using (this.OnlyUpdatingCursor())
        this.HandleHover(Mouse.PrimaryDevice.GetPosition((IInputElement) this.ActiveView.ViewRootContainer));
    }

    private void StartMotionlessAutoScroll(Point dragStartPosition, Point dragCurrentPosition)
    {
      if (this.motionlessAutoScroller == null)
        this.motionlessAutoScroller = this.CreateMotionlessAutoScroller();
      this.motionlessAutoScroller.StartScroll(dragStartPosition, dragCurrentPosition);
    }

    private void StopMotionlessAutoScroll()
    {
      if (this.motionlessAutoScroller == null)
        return;
      this.motionlessAutoScroller.StopScroll();
      this.motionlessAutoScroller = (MotionlessAutoScroller) null;
    }

    private bool ShouldMotionlessAutoScroll()
    {
      if (this.ActiveView != null)
      {
        FrameworkElement frameworkElement = (FrameworkElement) this.ActiveView.Artboard;
        if (frameworkElement != null)
          return this.ShouldMotionlessAutoScroll(Mouse.GetPosition((IInputElement) frameworkElement), new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
      }
      return false;
    }

    private class OnlyUpdateCursorToken : IDisposable
    {
      private ToolBehavior toolBehavior;

      public OnlyUpdateCursorToken(ToolBehavior toolBehavior)
      {
        this.toolBehavior = toolBehavior;
        this.toolBehavior.IsOnlyUpdatingCursor = true;
      }

      public void Dispose()
      {
        this.toolBehavior.IsOnlyUpdatingCursor = false;
      }
    }
  }
}

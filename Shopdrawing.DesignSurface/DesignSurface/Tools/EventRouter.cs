// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.EventRouter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Tools.Text;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public sealed class EventRouter
  {
    private Point lastDragPosition = new Point(double.MaxValue, double.MinValue);
    private SceneView view;
    private FrameworkElement scopeElement;
    private Stack<ToolBehavior> behaviorStack;
    private int buttonDownCount;
    private bool buttonDownHandled;
    private int clickCount;
    private bool isDragging;
    private Point dragStartPosition;
    private Point dragCurrentPosition;
    private bool captured;
    private bool consumeAltKey;
    private ModifierKeyBehaviorManager modifierKeyBehaviorManager;
    private static DesignerContext designerContext;

    public ToolBehavior ActiveBehavior
    {
      get
      {
        if (this.behaviorStack.Count <= 0)
          return (ToolBehavior) null;
        return this.behaviorStack.Peek();
      }
    }

    public bool IsButtonDown
    {
      get
      {
        return this.buttonDownCount > 0;
      }
    }

    public bool IsDragging
    {
      get
      {
        return this.isDragging;
      }
    }

    public int BehaviorStackCount
    {
      get
      {
        return this.behaviorStack.Count;
      }
    }

    private FrameworkElement RootElement
    {
      get
      {
        return this.view.ViewRootContainer;
      }
    }

    private bool IsEnabled
    {
      get
      {
        return this.view.IsDesignSurfaceEnabled;
      }
    }

    public bool IsEditingText
    {
      get
      {
        TextToolBehavior textToolBehavior = this.ActiveBehavior as TextToolBehavior;
        if (textToolBehavior != null)
          return textToolBehavior.TextSource != null;
        return false;
      }
    }

    public EventRouter(SceneView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      this.view = view;
      this.scopeElement = (FrameworkElement) this.view.Artboard;
      this.behaviorStack = new Stack<ToolBehavior>();
      this.scopeElement.AddHandler(Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.ScopeElement_MouseDown));
      this.scopeElement.AddHandler(Mouse.MouseMoveEvent, (Delegate) new MouseEventHandler(this.ScopeElement_MouseMove));
      this.scopeElement.AddHandler(Mouse.MouseEnterEvent, (Delegate) new MouseEventHandler(this.ScopeElement_MouseEnter));
      this.scopeElement.AddHandler(Mouse.MouseLeaveEvent, (Delegate) new MouseEventHandler(this.ScopeElement_MouseLeave));
      this.scopeElement.AddHandler(Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.ScopeElement_MouseUp));
      this.scopeElement.AddHandler(Mouse.LostMouseCaptureEvent, (Delegate) new MouseEventHandler(this.ScopeElement_LostMouseCapture));
      this.scopeElement.AddHandler(DragDrop.PreviewDropEvent, (Delegate) new DragEventHandler(this.ScopeElement_Drop));
      this.scopeElement.AddHandler(DragDrop.PreviewDragOverEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragOver));
      this.scopeElement.AddHandler(DragDrop.DragEnterEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragEnter));
      this.scopeElement.AddHandler(DragDrop.DragLeaveEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragLeave));
      this.modifierKeyBehaviorManager = new ModifierKeyBehaviorManager(this);
      this.modifierKeyBehaviorManager.Register((IModifierKeyBehaviorFactory) new PanToolModifierKeyBehaviorFactory());
      this.modifierKeyBehaviorManager.Register((IModifierKeyBehaviorFactory) new ZoomToolModifierKeyBehaviorFactory());
      this.modifierKeyBehaviorManager.Register((IModifierKeyBehaviorFactory) new LastSelectionModifierKeyBehaviorFactory());
    }

    internal void TearDown()
    {
      this.scopeElement.RemoveHandler(Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.ScopeElement_MouseDown));
      this.scopeElement.RemoveHandler(Mouse.MouseMoveEvent, (Delegate) new MouseEventHandler(this.ScopeElement_MouseMove));
      this.scopeElement.RemoveHandler(Mouse.MouseLeaveEvent, (Delegate) new MouseEventHandler(this.ScopeElement_MouseLeave));
      this.scopeElement.RemoveHandler(Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.ScopeElement_MouseUp));
      this.scopeElement.RemoveHandler(Mouse.LostMouseCaptureEvent, (Delegate) new MouseEventHandler(this.ScopeElement_LostMouseCapture));
      this.scopeElement.RemoveHandler(DragDrop.PreviewDropEvent, (Delegate) new DragEventHandler(this.ScopeElement_Drop));
      this.scopeElement.RemoveHandler(DragDrop.PreviewDragOverEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragOver));
      this.scopeElement.RemoveHandler(DragDrop.DragEnterEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragEnter));
      this.scopeElement.RemoveHandler(DragDrop.DragLeaveEvent, (Delegate) new DragEventHandler(this.ScopeElement_DragLeave));
      this.view = (SceneView) null;
      this.scopeElement = (FrameworkElement) null;
      this.behaviorStack = (Stack<ToolBehavior>) null;
    }

    public void PushBehavior(ToolBehavior newActiveBehavior)
    {
      if (newActiveBehavior == null)
        throw new ArgumentNullException("newActiveBehavior");
      ToolBehavior activeBehavior = this.ActiveBehavior;
      if (activeBehavior != null)
        activeBehavior.Suspend();
      this.behaviorStack.Push(newActiveBehavior);
      newActiveBehavior.Attach(this);
      this.ResumeActiveBehavior();
    }

    public void PopAllBehaviors()
    {
      while (this.ActiveBehavior != null)
        this.PopActiveBehavior();
    }

    public void PopBehavior()
    {
      this.PopActiveBehavior();
      if (this.ActiveBehavior == null)
        return;
      this.ResumeActiveBehavior();
    }

    public bool ContainsBehavior(ToolBehavior toolBehavior)
    {
      return this.behaviorStack.Contains(toolBehavior);
    }

    public bool ContainsBehavior(Type behaviorType)
    {
      return Enumerable.Any<ToolBehavior>((IEnumerable<ToolBehavior>) this.behaviorStack, (Func<ToolBehavior, bool>) (b => behaviorType.IsAssignableFrom(b.GetType())));
    }

    public void PopBehavior(Type behaviorType)
    {
      while (this.ActiveBehavior != null)
      {
        bool flag = behaviorType.IsAssignableFrom(this.ActiveBehavior.GetType());
        this.PopBehavior();
        if (flag)
          break;
      }
    }

    private void PopActiveBehavior()
    {
      ToolBehavior activeBehavior = this.ActiveBehavior;
      if (activeBehavior == null)
        throw new InvalidOperationException(ExceptionStringTable.EventRouterCannotPopBehavior);
      this.behaviorStack.Pop();
      activeBehavior.Suspend();
      activeBehavior.Detach();
    }

    private void ResumeActiveBehavior()
    {
      this.ActiveBehavior.Resume();
      if (!this.IsButtonDown)
        return;
      if (this.ActiveBehavior.ShouldCapture)
        this.BeginCapture();
      else
        this.EndCapture();
      this.ActiveBehavior.HandleLeftButtonDown(this.dragStartPosition);
      if (!this.IsDragging)
        return;
      this.ActiveBehavior.HandleDrag(this.dragStartPosition, this.dragCurrentPosition);
    }

    private void ScopeElement_MouseDown(object sender, MouseButtonEventArgs args)
    {
      if (this.ActiveBehavior == null || !this.IsEnabled || this.view.Artboard.DesignerView.IsMouseCaptureWithin && Enumerable.Any<UIElement>((IEnumerable<UIElement>) this.view.Artboard.DesignerView.Adorners, (Func<UIElement, bool>) (adorner => adorner.IsMouseCaptureWithin)))
        return;
      if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
        this.consumeAltKey = true;
      FrameworkElement rootElement = this.RootElement;
      if (rootElement == null)
        return;
      if (args.ChangedButton == MouseButton.Left && ++this.buttonDownCount == 1)
      {
        this.dragStartPosition = this.dragCurrentPosition = args.GetPosition((IInputElement) rootElement);
        this.clickCount = args.ClickCount;
        this.buttonDownHandled = false;
        try
        {
          this.BeginCapture();
          if (this.ActiveBehavior == null)
            return;
          args.Handled = this.ActiveBehavior.HandleLeftButtonDown(this.dragStartPosition);
        }
        finally
        {
          this.buttonDownHandled = true;
        }
      }
      else
      {
        if (args.ChangedButton != MouseButton.Right)
          return;
        Point position = args.GetPosition((IInputElement) rootElement);
        args.Handled = this.ActiveBehavior.HandleRightButtonDown(position);
      }
    }

    private void ScopeElement_MouseMove(object sender, MouseEventArgs args)
    {
      if (this.ActiveBehavior == null)
        return;
      if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
        this.consumeAltKey = true;
      Point position = args.GetPosition((IInputElement) this.scopeElement);
      if (position == this.lastDragPosition)
        return;
      this.lastDragPosition = position;
      FrameworkElement rootElement = this.RootElement;
      if (rootElement == null)
        return;
      GeneralTransform generalTransform = this.scopeElement.TransformToDescendant((Visual) rootElement);
      if (generalTransform == null)
        return;
      Point point = position * VectorUtilities.GetMatrixFromTransform(generalTransform);
      if (this.IsButtonDown && this.buttonDownHandled)
      {
        if (!this.IsDragging && !Tolerances.HaveMoved(this.dragStartPosition, point, this.view.Zoom))
          return;
        this.isDragging = true;
        this.dragCurrentPosition = point;
        args.Handled = this.ActiveBehavior.HandleDrag(this.dragStartPosition, this.dragCurrentPosition);
      }
      else
        args.Handled = this.ActiveBehavior.HandleHover(point);
    }

    private void ScopeElement_MouseEnter(object sender, MouseEventArgs args)
    {
      this.modifierKeyBehaviorManager.TrySwitchModifierKeyBehavior((KeyEventArgs) null);
    }

    private void ScopeElement_MouseLeave(object sender, MouseEventArgs args)
    {
      if (this.ActiveBehavior == null || this.IsDragging)
        return;
      args.Handled = this.ActiveBehavior.HandleHoverExit();
    }

    private void ScopeElement_MouseUp(object sender, MouseButtonEventArgs args)
    {
      if (this.ActiveBehavior != null)
      {
        if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
          this.consumeAltKey = true;
        FrameworkElement rootElement = this.RootElement;
        if (rootElement != null && args.ChangedButton == MouseButton.Left && this.IsButtonDown && --this.buttonDownCount == 0)
        {
          if (this.IsDragging)
          {
            this.isDragging = false;
            this.dragCurrentPosition = args.GetPosition((IInputElement) rootElement);
            args.Handled = this.ActiveBehavior.HandleDragEnd(this.dragStartPosition, this.dragCurrentPosition);
          }
          else
            args.Handled = this.ActiveBehavior.HandleClickEnd(this.dragStartPosition, this.clickCount);
          this.EndCapture();
        }
      }
      this.modifierKeyBehaviorManager.TrySwitchModifierKeyBehavior((KeyEventArgs) null);
    }

    private void ScopeElement_LostMouseCapture(object sender, MouseEventArgs args)
    {
      if (InputManager.Current.PrimaryMouseDevice.Captured == this.scopeElement)
        return;
      if (this.IsButtonDown)
      {
        this.buttonDownCount = 0;
        if (this.IsDragging)
        {
          this.isDragging = false;
          if (this.ActiveBehavior != null)
            this.ActiveBehavior.HandleDragEnd(this.dragStartPosition, this.dragCurrentPosition);
        }
        else if (this.ActiveBehavior != null)
          this.ActiveBehavior.HandleClickEnd(this.dragStartPosition, this.clickCount);
      }
      this.captured = false;
      this.modifierKeyBehaviorManager.TrySwitchModifierKeyBehavior((KeyEventArgs) null);
    }

    private void ScopeElement_KeyDownOrUp(object sender, KeyEventArgs args)
    {
      if (args.IsDown && !args.IsRepeat)
        this.modifierKeyBehaviorManager.TrySwitchModifierKeyBehavior(args);
      bool flag = false;
      if (this.view.Artboard.DesignerView.IsKeyboardFocusWithin)
      {
        foreach (UIElement uiElement in (IEnumerable<UIElement>) this.view.Artboard.DesignerView.Adorners)
        {
          AdornerPanel adornerPanel = uiElement as AdornerPanel;
          if (adornerPanel != null && adornerPanel.IsKeyboardFocusWithin && adornerPanel.IsContentFocusable)
          {
            flag = true;
            break;
          }
        }
      }
      if (this.ActiveBehavior != null && !flag && !this.view.Artboard.IsLiveControlLayerActive)
      {
        this.ActiveBehavior.HandleKey(args);
        if (args.Key == Key.System && (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt))
        {
          if (args.IsRepeat)
            args.Handled = true;
          else if (args.IsDown)
            this.consumeAltKey = false;
          else if (this.consumeAltKey)
            args.Handled = true;
        }
        if (this.IsButtonDown)
          args.Handled = true;
        if (this.view.Artboard.DesignerView.IsMouseCaptureWithin)
          args.Handled = true;
      }
      if (!args.IsUp)
        return;
      this.modifierKeyBehaviorManager.TrySwitchModifierKeyBehavior(args);
    }

    private void ScopeElement_DragOver(object sender, DragEventArgs args)
    {
      if (this.ActiveBehavior == null || !this.IsEnabled)
        return;
      args.Handled = this.ActiveBehavior.HandleDragOver(args);
    }

    private void ScopeElement_DragEnter(object sender, DragEventArgs args)
    {
      if (this.ActiveBehavior == null || !this.IsEnabled)
        return;
      args.Handled = this.ActiveBehavior.HandleDragEnter(args);
    }

    private void ScopeElement_DragLeave(object sender, DragEventArgs args)
    {
      if (this.ActiveBehavior == null || !this.IsEnabled)
        return;
      args.Handled = this.ActiveBehavior.HandleDragLeave(args);
    }

    private void ScopeElement_Drop(object sender, DragEventArgs args)
    {
      if (this.ActiveBehavior == null || !this.IsEnabled)
        return;
      args.Handled = this.ActiveBehavior.HandleDrop(args);
    }

    private void BeginCapture()
    {
      if (InputManager.Current.PrimaryMouseDevice.Captured == this.scopeElement || !this.ActiveBehavior.ShouldCapture || !this.IsEnabled)
        return;
      InputManager.Current.PrimaryMouseDevice.Capture((IInputElement) this.scopeElement);
      InputManager.Current.PrimaryKeyboardDevice.Focus((IInputElement) this.scopeElement);
      this.captured = true;
    }

    private void EndCapture()
    {
      if (!this.captured)
        return;
      InputManager.Current.PrimaryMouseDevice.Capture((IInputElement) null);
      this.captured = false;
    }

    internal static void InitializeKeyboardHook(DesignerContext designerContext)
    {
      if (EventRouter.designerContext != null)
        return;
      EventRouter.designerContext = designerContext;
      EventManager.RegisterClassHandler(typeof (Window), Keyboard.KeyDownEvent, (Delegate) new KeyEventHandler(EventRouter.HandleKeyDownOrUp));
      EventManager.RegisterClassHandler(typeof (Window), Keyboard.KeyUpEvent, (Delegate) new KeyEventHandler(EventRouter.HandleKeyDownOrUp));
      EventManager.RegisterClassHandler(typeof (Window), Mouse.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(EventRouter.HandleMouseDown));
    }

    private static void HandleKeyDownOrUp(object sender, KeyEventArgs e)
    {
      SceneView activeView = EventRouter.designerContext.ActiveView;
      if (activeView == null)
        return;
      Visual visual = e.OriginalSource as Visual;
      if (visual != null && !(visual is FloatingWindow) && (!activeView.Artboard.IsAncestorOf((DependencyObject) visual) || activeView.Artboard.AnnotationLayer.IsAncestorOf((DependencyObject) visual)) || (activeView.FocusedEditor != FocusedEditor.Design || activeView.EventRouter == null))
        return;
      activeView.EventRouter.ScopeElement_KeyDownOrUp(sender, e);
    }

    private static void HandleMouseDown(object sender, MouseButtonEventArgs e)
    {
      SceneView activeView = EventRouter.designerContext.ActiveView;
      if (activeView == null || activeView.ViewMode == ViewMode.Design || activeView.FocusedEditor == FocusedEditor.Design)
        return;
      Visual visual1 = e.OriginalSource as Visual;
      Visual visual2 = (Visual) activeView.CodeEditor.Element;
      if (visual1 != null && visual2.IsAncestorOf((DependencyObject) visual1))
        return;
      Visual visual3 = e.Source as Visual;
      if (visual3 != null && (visual3 is Menu || visual3 is MenuItem || PresentationSource.FromVisual(visual3) != PresentationSource.FromVisual(visual2)))
        return;
      if (activeView.ViewMode == ViewMode.Split)
      {
        activeView.SetFocusToRoot();
        if (activeView.Document.IsEditable)
          return;
        activeView.Document.OnUpdatedEditTransaction();
      }
      else
        activeView.ShowErrors();
    }
  }
}

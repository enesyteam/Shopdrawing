// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.WorkaroundPopup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Controls
{
  public class WorkaroundPopup : Popup, IWorkaroundPopupController
  {
    public static readonly DependencyProperty WorkaroundPopupControllerProperty = DependencyProperty.RegisterAttached("WorkaroundPopupController", typeof (IWorkaroundPopupController), typeof (WorkaroundPopup), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    private static Stack<WorkaroundPopup> PopupStack = new Stack<WorkaroundPopup>();
    private bool recievedEscapeOnPreNotifyInput;
    private bool releasingCapture;
    private bool listenForInputEvents;
    private bool freezeClosingOnLostCapture;
    private IInputElement captureElementBeforeLock;

    public bool RedirectFocusOnOpen { get; set; }

    public bool FreezeClosingOnLostCapture
    {
      get
      {
        return this.freezeClosingOnLostCapture;
      }
      set
      {
        if (value)
          this.captureElementBeforeLock = Mouse.Captured;
        else if (this.captureElementBeforeLock != null)
          Mouse.Capture(this.captureElementBeforeLock, CaptureMode.SubTree);
        this.freezeClosingOnLostCapture = value;
      }
    }

    private bool ListenForInputEvents
    {
      get
      {
        return this.listenForInputEvents;
      }
      set
      {
        if (this.listenForInputEvents == value)
          return;
        this.listenForInputEvents = value;
        if (this.listenForInputEvents)
        {
          InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
          InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(this.Current_PreNotifyInput);
        }
        else
        {
          InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
          InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.Current_PreNotifyInput);
        }
      }
    }

    public event EventHandler SynchronousClosed;

    static WorkaroundPopup()
    {
      Popup.IsOpenProperty.OverrideMetadata(typeof (WorkaroundPopup), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(WorkaroundPopup.OnIsOpenChanged)));
    }

    public WorkaroundPopup()
    {
      this.RedirectFocusOnOpen = true;
    }

    private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      WorkaroundPopup workaroundPopup1 = (WorkaroundPopup) o;
      if (workaroundPopup1.IsOpen)
        return;
      while (WorkaroundPopup.PopupStack.Count > 0)
      {
        WorkaroundPopup workaroundPopup2 = WorkaroundPopup.PopupStack.Peek();
        if (workaroundPopup2 == workaroundPopup1)
        {
          WorkaroundPopup.PopupStack.Pop();
          break;
        }
        if (workaroundPopup2.IsOpen)
          workaroundPopup2.IsOpen = false;
        else
          WorkaroundPopup.PopupStack.Pop();
      }
      workaroundPopup1.OnSynchronousPopupClosed();
    }

    public static IDisposable LockOpen(DependencyObject dependencyObject)
    {
      return (IDisposable) new WorkaroundPopupLockOpen(dependencyObject);
    }

    protected void OnSynchronousPopupClosed()
    {
      if (this.SynchronousClosed == null)
        return;
      this.SynchronousClosed((object) this, EventArgs.Empty);
    }

    protected override void OnOpened(EventArgs e)
    {
      this.releasingCapture = false;
      if (this.Child != null)
      {
        if (this.RedirectFocusOnOpen)
        {
          this.Child.Focusable = true;
          this.Child.Focus();
        }
        Mouse.Capture((IInputElement) this.Child, CaptureMode.SubTree);
      }
      this.SetValue(FocusScopeManager.FocusScopePriorityProperty, (object) 1);
      this.SetValue(WorkaroundPopup.WorkaroundPopupControllerProperty, (object) this);
      this.recievedEscapeOnPreNotifyInput = false;
      this.ListenForInputEvents = true;
      WorkaroundPopup.PopupStack.Push(this);
      base.OnOpened(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      object obj = (object) this;
      if (!this.releasingCapture && Mouse.Captured != this.Child && !this.freezeClosingOnLostCapture)
      {
        if (e.OriginalSource == this.Child)
        {
          if (Mouse.Captured == null)
            this.IsOpen = false;
          else if (this.IsOpen)
            Mouse.Captured.LostMouseCapture += new MouseEventHandler(this.Captured_LostMouseCapture);
        }
        else if (this.IsDescendentOfPopup(obj as DependencyObject))
        {
          if (this.IsOpen && Mouse.Captured == null)
            Mouse.Capture((IInputElement) this.Child, CaptureMode.SubTree);
        }
        else
          this.IsOpen = false;
      }
      base.OnLostMouseCapture(e);
    }

    private void Captured_LostMouseCapture(object sender, MouseEventArgs e)
    {
      ((IInputElement) sender).LostMouseCapture -= new MouseEventHandler(this.Captured_LostMouseCapture);
      if (!this.IsOpen)
        return;
      if (Mouse.Captured == null)
        Mouse.Capture((IInputElement) this.Child, CaptureMode.SubTree);
      else
        Mouse.Captured.LostMouseCapture += new MouseEventHandler(this.Captured_LostMouseCapture);
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      if (e.OriginalSource == this.Child && this.Child.InputHitTest(e.GetPosition((IInputElement) this.Child)) == null)
      {
        this.IsOpen = false;
        this.ReleaseChildMouseCapture();
      }
      base.OnPreviewMouseDown(e);
    }

    private bool IsDescendentOfPopup(DependencyObject currentObject)
    {
      for (; currentObject != null; currentObject = VisualTreeHelper.GetParent(currentObject))
      {
        if (currentObject == this || currentObject == this.Child)
          return true;
      }
      return false;
    }

    private void ReleaseChildMouseCapture()
    {
      if (Mouse.Captured != this.Child)
        return;
      this.releasingCapture = true;
      Mouse.Capture((IInputElement) null);
      this.releasingCapture = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        this.IsOpen = false;
      base.OnKeyDown(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      this.ListenForInputEvents = false;
      this.ReleaseChildMouseCapture();
      base.OnClosed(e);
    }

    private void Current_PreNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (this.StopListeningForInputEventsIfNeeded())
        return;
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs == null || keyEventArgs.Key != Key.Escape || !keyEventArgs.IsDown)
        return;
      this.recievedEscapeOnPreNotifyInput = this.Child != null && this.Child.IsMouseCaptured && !this.IsKeyboardFocusWithin;
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (this.StopListeningForInputEventsIfNeeded())
        return;
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs == null || keyEventArgs.Key != Key.Escape || (!keyEventArgs.IsDown || keyEventArgs.Handled) || (this.Child == null || !this.Child.IsMouseCaptured || !this.recievedEscapeOnPreNotifyInput))
        return;
      this.IsOpen = false;
    }

    private bool StopListeningForInputEventsIfNeeded()
    {
      if (this.IsOpen || this.Child != null)
        return false;
      this.ListenForInputEvents = false;
      return true;
    }
  }
}

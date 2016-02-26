// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.ColorEyedropper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  public class ColorEyedropper : ColorEyedropperBase
  {
    private int lastX = int.MaxValue;
    private int lastY = int.MaxValue;
    private bool isCapturing;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (this.isCapturing)
          this.CommitEditing();
        else
          this.BeginEditing();
        e.Handled = true;
      }
      else if (e.ChangedButton == MouseButton.Right && this.isCapturing)
      {
        this.CancelEditing();
        e.Handled = true;
      }
      base.OnMouseDown(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      this.CancelEditing();
      base.OnLostMouseCapture(e);
    }

    private void BeginEditing()
    {
      this.isCapturing = this.CaptureMouse();
      if (!this.isCapturing)
        return;
      ValueEditorUtils.ExecuteCommand(this.BeginEditCommand, (IInputElement) this, (object) null);
      this.Cursor = ValueEditorCursors.EyedropperCursor;
      this.ShowColorEditorTrack(false);
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.OpenFeedbackWindow();
    }

    protected override void CancelEditing()
    {
      if (!this.isCapturing)
        return;
      ValueEditorUtils.ExecuteCommand(this.CancelEditCommand, (IInputElement) this, (object) null);
      this.FinishEditing();
    }

    private void CommitEditing()
    {
      if (!this.isCapturing)
        return;
      ValueEditorUtils.ExecuteCommand(this.EndEditCommand, (IInputElement) this, (object) null);
      this.FinishEditing();
    }

    private void FinishEditing()
    {
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.Cursor = (Cursor) null;
      this.isCapturing = false;
      this.ReleaseMouseCapture();
      this.CloseFeedbackWindow();
      this.ShowColorEditorTrack(true);
      ValueEditorUtils.ExecuteCommand(this.FinishEditingCommand, (IInputElement) this, (object) null);
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs != null && keyEventArgs.Key == Key.Escape && this.isCapturing)
      {
        this.CancelEditing();
      }
      else
      {
        if (!(e.StagingItem.Input is MouseEventArgs))
          return;
        UnsafeNativeMethods.Win32Point cursorPosition = UnsafeNativeMethods.GetCursorPosition();
        if (this.lastX == cursorPosition.x && this.lastY == cursorPosition.y)
          return;
        this.lastX = cursorPosition.x;
        this.lastY = cursorPosition.y;
        this.StartUpdateColor(cursorPosition.x, cursorPosition.y);
      }
    }
  }
}

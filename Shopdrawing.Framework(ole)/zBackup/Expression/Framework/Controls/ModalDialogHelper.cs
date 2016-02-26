// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ModalDialogHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
  public class ModalDialogHelper : IDisposable
  {
    private List<Window> windows;

    public ModalDialogHelper()
    {
      this.EnterThreadModal();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
      if (!isDisposing)
        return;
      this.ExitThreadModal();
    }

    private void EnterThreadModal()
    {
      foreach (Window window in Application.Current.Windows)
      {
        if (window.IsHitTestVisible)
        {
          if (this.windows == null)
            this.windows = new List<Window>();
          this.windows.Add(window);
        }
      }
      this.EnableThreadWindows(false);
    }

    private void ExitThreadModal()
    {
      this.EnableThreadWindows(true);
      this.windows = (List<Window>) null;
    }

    private void EnableThreadWindows(bool state)
    {
      if (this.windows == null)
        return;
      for (int index = 0; index < this.windows.Count; ++index)
        this.windows[index].IsHitTestVisible = state;
    }
  }
}

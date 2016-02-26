// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TrackFocusEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class TrackFocusEventArgs : EventArgs
  {
    private IntPtr _hwndGainFocus;
    private IntPtr _hwndLoseFocus;

    public IntPtr HwndGainFocus
    {
      get
      {
        return this._hwndGainFocus;
      }
      set
      {
        this._hwndGainFocus = value;
      }
    }

    public IntPtr HwndLoseFocus
    {
      get
      {
        return this._hwndLoseFocus;
      }
      set
      {
        this._hwndLoseFocus = value;
      }
    }

    public TrackFocusEventArgs(IntPtr hwndGainFocus, IntPtr hwndLoseFocus)
    {
      this._hwndGainFocus = hwndGainFocus;
      this._hwndLoseFocus = hwndLoseFocus;
    }
  }
}

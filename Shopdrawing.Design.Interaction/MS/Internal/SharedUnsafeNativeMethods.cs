// Decompiled with JetBrains decompiler
// Type: MS.Internal.SharedUnsafeNativeMethods
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MS.Internal
{
  [SuppressUnmanagedCodeSecurity]
  internal static class SharedUnsafeNativeMethods
  {
    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("Gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hdc, int index);
  }
}

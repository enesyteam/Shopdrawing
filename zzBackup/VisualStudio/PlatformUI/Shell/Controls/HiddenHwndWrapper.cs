// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.HiddenHwndWrapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public sealed class HiddenHwndWrapper : HwndWrapper
  {
    private static HiddenHwndWrapper instance;

    public static HiddenHwndWrapper Instance
    {
      get
      {
        return HiddenHwndWrapper.instance ?? (HiddenHwndWrapper.instance = new HiddenHwndWrapper());
      }
    }

    private HiddenHwndWrapper()
    {
    }

    [CLSCompliant(false)]
    protected override ushort CreateWindowClassCore()
    {
      return this.RegisterClass("ViewManagerHiddenWindowParent");
    }

    protected override IntPtr CreateWindowCore()
    {
      int dwStyle = int.MinValue;
      IntPtr moduleHandle = NativeMethods.GetModuleHandle((string) null);
      return NativeMethods.CreateWindowEx(0, new IntPtr((int) this.WindowClassAtom), (string) null, dwStyle, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, moduleHandle, IntPtr.Zero);
    }
  }
}

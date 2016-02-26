// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.WindowProfileChangingEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class WindowProfileChangingEventArgs : EventArgs
  {
    public WindowProfile OldWindowProfile { get; private set; }

    public WindowProfile NewWindowProfile { get; private set; }

    public WindowProfileChangingEventArgs(WindowProfile oldProfile, WindowProfile newProfile)
    {
      this.OldWindowProfile = oldProfile;
      this.NewWindowProfile = newProfile;
    }
  }
}

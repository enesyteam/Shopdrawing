// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.DispatcherHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public class DispatcherHelper
  {
    private DispatcherFrame Frame;

    public DispatcherHelper()
    {
      this.Frame = new DispatcherFrame();
    }

    public void ClearFrames(Dispatcher dispatcher)
    {
      dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Delegate) new DispatcherOperationCallback(this.ExitFrame), (object) this.Frame);
      Dispatcher.PushFrame(this.Frame);
    }

    private object ExitFrame(object frame)
    {
      ((DispatcherFrame) frame).Continue = false;
      this.Frame = (DispatcherFrame) null;
      return (object) null;
    }

    public void ExitFrame()
    {
      if (this.Frame == null)
        return;
      this.ExitFrame((object) this.Frame);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.SynchronousAsyncMechanism
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class SynchronousAsyncMechanism : IAsyncMechanism
  {
    private bool isPaused;

    public bool IsThreaded
    {
      get
      {
        return false;
      }
    }

    public event DoWorkEventHandler DoWork;

    public void Begin()
    {
      if (this.DoWork == null || this.isPaused)
        return;
      DoWorkEventArgs e = new DoWorkEventArgs((object) null);
      do
      {
        this.DoWork((object) this, e);
      }
      while (object.Equals(e.Result, (object) AsyncProcessResult.StillGoing));
    }

    public void Kill()
    {
    }

    public void Pause()
    {
      this.isPaused = true;
    }

    public void Resume()
    {
      this.isPaused = false;
    }
  }
}

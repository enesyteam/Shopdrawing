// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.DispatcherTimerAsyncMechanism
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public class DispatcherTimerAsyncMechanism : IAsyncMechanism
  {
    private DispatcherTimer dispatcherTimer;
    private bool isPaused;

    public bool IsThreaded
    {
      get
      {
        return false;
      }
    }

    public event DoWorkEventHandler DoWork;

    public DispatcherTimerAsyncMechanism(DispatcherPriority priority, TimeSpan interval)
    {
      this.dispatcherTimer = new DispatcherTimer(priority);
      this.dispatcherTimer.Interval = interval;
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
      if (this.DoWork == null)
        return;
      DoWorkEventArgs e1 = new DoWorkEventArgs((object) null);
      this.DoWork(sender, e1);
      if (object.Equals(e1.Result, (object) AsyncProcessResult.StillGoing))
        return;
      this.Kill();
    }

    public void Begin()
    {
      this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
      this.dispatcherTimer.Start();
    }

    public void Kill()
    {
      this.dispatcherTimer.Stop();
      this.dispatcherTimer.Tick -= new EventHandler(this.DispatcherTimer_Tick);
    }

    public void Pause()
    {
      if (!this.isPaused)
        this.dispatcherTimer.Stop();
      this.isPaused = true;
    }

    public void Resume()
    {
      if (this.isPaused)
        this.dispatcherTimer.Start();
      this.isPaused = false;
    }
  }
}

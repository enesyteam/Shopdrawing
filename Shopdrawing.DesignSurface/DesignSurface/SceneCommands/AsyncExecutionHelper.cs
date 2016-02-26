// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.AsyncExecutionHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public class AsyncExecutionHelper
  {
    private AsyncQueueProcess processingQueue;
    private AsyncProcessDialog asyncProgressDialog;

    public AsyncExecutionHelper()
    {
      this.processingQueue = new AsyncQueueProcess((IAsyncMechanism) new DispatcherTimerAsyncMechanism(DispatcherPriority.Background, new TimeSpan(0, 0, 0, 0, 10)));
    }

    public void Clear()
    {
      this.processingQueue.Clear();
    }

    public void StartAsyncProcess(IExpressionInformationService expressionInformationService, Action<object, DoWorkEventArgs> workerDelegate, EventHandler onBegun, EventHandler onComplete, EventHandler onProgress, EventHandler onKill)
    {
      AsyncExecuteCommandProcess executeCommandProcess = new AsyncExecuteCommandProcess(string.Empty, workerDelegate);
      executeCommandProcess.Begun += onBegun;
      executeCommandProcess.Killed += onKill;
      executeCommandProcess.Complete += onComplete;
      executeCommandProcess.Progress += onProgress;
      executeCommandProcess.Progress += new EventHandler(this.OnProgress);
      executeCommandProcess.Killed += new EventHandler(this.OnKill);
      executeCommandProcess.Complete += new EventHandler(this.OnComplete);
      this.processingQueue.Add((AsyncProcess) executeCommandProcess, false);
      if (this.asyncProgressDialog != null)
        return;
      this.asyncProgressDialog = new AsyncProcessDialog((AsyncProcess) this.processingQueue, expressionInformationService, 0.0);
      this.ShowProgressDialog(true);
      this.CleanUp((AsyncExecuteCommandProcess) null);
      this.asyncProgressDialog = (AsyncProcessDialog) null;
    }

    private void CancelAllTasks()
    {
      this.processingQueue.ForEach(new ForEachAction(this.CancelJob), (object) null);
    }

    private void OnKill(object sender, EventArgs e)
    {
      AsyncExecuteCommandProcess process = (AsyncExecuteCommandProcess) sender;
      this.CancelAllTasks();
      this.CleanUp(process);
    }

    private void OnComplete(object sender, EventArgs e)
    {
      this.CleanUp((AsyncExecuteCommandProcess) sender);
    }

    private void CleanUp(AsyncExecuteCommandProcess process)
    {
      if (process != null)
      {
        process.Killed -= new EventHandler(this.OnKill);
        process.Complete -= new EventHandler(this.OnComplete);
        process.Progress -= new EventHandler(this.OnProgress);
      }
      this.asyncProgressDialog = (AsyncProcessDialog) null;
    }

    private void OnProgress(object sender, EventArgs e)
    {
    }

    private void CancelJob(AsyncProcess process, object hint)
    {
      string str = hint as string;
      AsyncExecuteCommandProcess executeCommandProcess = process as AsyncExecuteCommandProcess;
      if (executeCommandProcess == null || executeCommandProcess.Cancel || !(executeCommandProcess.Name == str) && str != null)
        return;
      executeCommandProcess.Cancel = true;
      executeCommandProcess.Kill();
    }

    private void ShowProgressDialog(bool show)
    {
      if (this.asyncProgressDialog == null)
        return;
      if (show)
      {
        this.asyncProgressDialog.ShowDialog();
      }
      else
      {
        this.asyncProgressDialog.Close(new bool?(false));
        this.asyncProgressDialog = (AsyncProcessDialog) null;
      }
    }
  }
}

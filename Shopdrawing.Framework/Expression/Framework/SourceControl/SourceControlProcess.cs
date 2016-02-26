// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlProcess
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.SourceControl
{
  public abstract class SourceControlProcess : AsyncProcess, IDisposable
  {
    private IExpressionInformationService expressionInformationService;
    private IMessageDisplayService displayService;
    private SourceControlService sourceControlService;

    public SourceControlProcessModel Model { get; private set; }

    protected bool StillWorking { get; set; }

    public override string StatusText
    {
      get
      {
        return this.Model.Status;
      }
    }

    public override int CompletedCount
    {
      get
      {
        return this.Model.CurrentItem;
      }
    }

    public override int Count
    {
      get
      {
        return this.Model.ItemCount;
      }
    }

    protected SourceControlProcess(SourceControlProcessModel model, IServiceProvider serviceProvider)
      : base((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.Background))
    {
      this.Model = model;
      this.StillWorking = true;
      this.expressionInformationService = serviceProvider.GetService(typeof (IExpressionInformationService)) as IExpressionInformationService;
      this.displayService = serviceProvider.GetService(typeof (IMessageDisplayService)) as IMessageDisplayService;
      this.sourceControlService = serviceProvider.GetService(typeof (ISourceControlService)) as SourceControlService;
    }

    public bool WorkWithProgress()
    {
      bool? nullable = new AsyncProcessDialog((AsyncProcess) this, this.expressionInformationService, 0.0).ShowDialog();
      bool flag = nullable.HasValue && nullable.GetValueOrDefault();
      this.ShowDefaultDialogs();
      return flag;
    }

    public void WorkWithoutProgress()
    {
      using (TemporaryCursor.SetWaitCursor())
      {
        this.Begin();
        this.Model.Dispatcher = new DispatcherHelper();
        this.Model.Dispatcher.ClearFrames(Dispatcher.CurrentDispatcher);
      }
      this.ShowDefaultDialogs();
    }

    private string ProcessMessages()
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      foreach (ErrorMessage errorMessage in this.Model.NonfatalMessages)
      {
        if (errorMessage.ErrorLevel == ErrorLevel.Error)
        {
          if (flag)
            stringBuilder.AppendLine();
          stringBuilder.Append(errorMessage.Message);
          flag = true;
        }
        else
          this.sourceControlService.LogMessage(errorMessage.Message);
      }
      if (flag && this.Model.Success == SourceControlSuccess.Success)
        this.Model.Success = SourceControlSuccess.Failed;
      return stringBuilder.ToString();
    }

    public void ShowDefaultDialogs()
    {
      string errorText = this.ProcessMessages();
      if (!string.IsNullOrEmpty(errorText))
        this.sourceControlService.LogMessage(errorText);
      if (this.Model.Success == SourceControlSuccess.Offline)
      {
        if (!string.IsNullOrEmpty(errorText))
          errorText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}\r\n\r\n{1}", new object[2]
          {
            (object) StringTable.SourceControlOfflineDialogIntroduction,
            (object) errorText
          });
        else
          errorText = StringTable.SourceControlOfflineDialogIntroduction;
      }
      if (string.IsNullOrEmpty(errorText))
        return;
      errorText = this.sourceControlService.FormatDialogString(errorText);
      int num;
      UIThreadDispatcher.Instance.Invoke(DispatcherPriority.ApplicationIdle, (Action) (() => num = (int) this.displayService.ShowMessage(new MessageBoxArgs()
      {
        Message = errorText,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Hand,
        AutomationId = "SourceControlErrorDialog"
      })));
    }

    protected override void Work()
    {
      this.DoWork(this.Model);
      this.StillWorking = false;
      if (this.Model.Dispatcher == null)
        return;
      this.Model.Dispatcher.ExitFrame();
    }

    public override void Kill()
    {
      this.Model.OnlineStatus = this.sourceControlService.ActiveProvider.GetOnlineStatus();
      base.Kill();
    }

    protected abstract void DoWork(SourceControlProcessModel baseModel);

    protected override bool MoveNext()
    {
      return this.StillWorking;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}

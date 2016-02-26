// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.AsyncProcessDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class AsyncProcessDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private static TimeSpan ShowAfter = TimeSpan.FromSeconds(2.0);
    private static TimeSpan ShowForAtLeast = TimeSpan.FromSeconds(1.0);
    private AsyncProcess asyncProcess;
    private bool begunProcess;
    private bool completedProcess;
    private bool shown;
    private bool closed;
    private bool cancelled;
    private DateTime shownTime;
    private TimeSpan showAfter;
    internal Button CancelButton;
    private bool _contentLoaded;

    public double Progress
    {
      get
      {
        if (this.asyncProcess.Count == 0)
          return 0.5;
        return (double) this.asyncProcess.CompletedCount / (double) this.asyncProcess.Count;
      }
    }

    public string StatusText
    {
      get
      {
        return this.asyncProcess.StatusText;
      }
    }

    public bool IsStillGoing
    {
      get
      {
        if (!this.cancelled)
          return !this.completedProcess;
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public AsyncProcessDialog(AsyncProcess asyncProcess, IExpressionInformationService expressionInformationService)
      : this(asyncProcess, expressionInformationService.DefaultDialogTitle)
    {
    }

    public AsyncProcessDialog(AsyncProcess asyncProcess, IExpressionInformationService expressionInformationService, double showAfter)
      : this(asyncProcess, expressionInformationService.DefaultDialogTitle, showAfter)
    {
    }

    public AsyncProcessDialog(AsyncProcess asyncProcess, string title)
    {
      this.showAfter = AsyncProcessDialog.ShowAfter;
      this.Initialize(asyncProcess, title);
    }

    public AsyncProcessDialog(AsyncProcess asyncProcess, string title, double showAfter)
    {
      this.showAfter = TimeSpan.FromSeconds(showAfter);
      this.Initialize(asyncProcess, title);
    }

    private void Initialize(AsyncProcess asyncProcess, string title)
    {
      this.asyncProcess = asyncProcess;
      this.begunProcess = asyncProcess.IsAlive;
      this.DataContext = (object) this;
      this.Title = title;
      this.InitializeComponent();
      this.AllowsTransparency = true;
      if (this.showAfter.Ticks == 0L)
        this.Opacity = 1.0;
      else
        this.Opacity = 0.0;
      this.asyncProcess.Complete += new EventHandler(this.OnProcessCompleted);
      this.asyncProcess.Progress += new EventHandler(this.OnProcessUpdated);
    }

    protected override void OnActivated(EventArgs e)
    {
      if (this.asyncProcess != null && !this.begunProcess && !this.asyncProcess.IsAlive)
      {
        Mouse.OverrideCursor = Cursors.Wait;
        if (this.showAfter.Ticks != 0L)
          UIThreadDispatcher.Instance.InvokeAfter(this.showAfter, DispatcherPriority.Normal, (Action) (() =>
          {
            if (this.completedProcess)
              return;
            this.Opacity = 1.0;
            Mouse.OverrideCursor = Cursors.Arrow;
            this.shown = true;
            this.shownTime = DateTime.Now;
          }));
        this.asyncProcess.Begin();
        this.begunProcess = true;
      }
      base.OnActivated(e);
    }

    protected override void OnCancelButtonExecute()
    {
      this.cancelled = true;
      this.asyncProcess.Kill();
      if (this.closed)
        return;
      base.OnCancelButtonExecute();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (this.IsStillGoing)
      {
        this.asyncProcess.Kill();
        this.cancelled = true;
      }
      this.asyncProcess.Complete -= new EventHandler(this.OnProcessCompleted);
      this.asyncProcess.Progress -= new EventHandler(this.OnProcessUpdated);
      this.closed = true;
      base.OnClosing(e);
    }

    private void OnProcessUpdated(object sender, EventArgs e)
    {
      UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Normal, (Action) (() =>
      {
        this.OnPropertyChanged("Progress");
        this.OnPropertyChanged("StatusText");
      }));
    }

    private void OnProcessCompleted(object sender, EventArgs e)
    {
      this.completedProcess = true;
      TimeSpan delay = new TimeSpan(0L);
      if (this.shown)
      {
        TimeSpan timeSpan = DateTime.Now - this.shownTime;
        if (timeSpan < AsyncProcessDialog.ShowForAtLeast)
          delay = AsyncProcessDialog.ShowForAtLeast - timeSpan;
      }
      UIThreadDispatcher.Instance.InvokeAfter(delay, DispatcherPriority.Normal, (Action) (() =>
      {
        if (this.closed)
          return;
        this.Close(new bool?(true));
      }));
      this.OnPropertyChanged("IsStillGoing");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/asyncprocessdialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.CancelButton = (Button) target;
      else
        this._contentLoaded = true;
    }
  }
}

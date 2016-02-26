// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.NotificationBar
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class NotificationBar : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty BuildMessageProperty = DependencyProperty.Register("BuildMessage", typeof (string), typeof (NotificationBar));
    public static readonly DependencyProperty IsBuildingProperty = DependencyProperty.Register("IsBuilding", typeof (bool), typeof (NotificationBar), new PropertyMetadata(new PropertyChangedCallback(NotificationBar.IsBuildingChanged)));
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof (bool), typeof (NotificationBar));
    public static readonly DependencyProperty CloseDelayProperty = DependencyProperty.Register("CloseDelay", typeof (int), typeof (NotificationBar), new PropertyMetadata((object) 5));
    private DispatcherTimer timer;
    private bool _contentLoaded;

    public string BuildMessage
    {
      get
      {
        return (string) this.GetValue(NotificationBar.BuildMessageProperty);
      }
      set
      {
        this.SetValue(NotificationBar.BuildMessageProperty, (object) value);
      }
    }

    public bool IsOpen
    {
      get
      {
        return (bool) this.GetValue(NotificationBar.IsOpenProperty);
      }
      set
      {
        this.SetValue(NotificationBar.IsOpenProperty, (object) (bool) (value ? true : false));
      }
    }

    public int CloseDelay
    {
      get
      {
        return (int) this.GetValue(NotificationBar.CloseDelayProperty);
      }
      set
      {
        this.SetValue(NotificationBar.CloseDelayProperty, (object) value);
      }
    }

    public bool IsBuilding
    {
      get
      {
        return (bool) this.GetValue(NotificationBar.IsBuildingProperty);
      }
      set
      {
        this.SetValue(NotificationBar.IsBuildingProperty, (object) (bool) (value ? true : false));
      }
    }

    public ICommand DismissCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Dismiss));
      }
    }

    public NotificationBar()
    {
      this.InitializeComponent();
      this.timer = new DispatcherTimer();
      this.timer.Tick += new EventHandler(this.Timer_Tick);
    }

    private static void IsBuildingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
      NotificationBar notificationBar = dependencyObject as NotificationBar;
      if (notificationBar == null || !(args.Property.Name == "IsBuilding"))
        return;
      bool flag1 = (bool) args.OldValue;
      bool flag2 = (bool) args.NewValue;
      if (flag2 == flag1)
        return;
      if (flag2)
      {
        notificationBar.timer.Stop();
        notificationBar.IsOpen = flag2;
      }
      else
      {
        notificationBar.timer.Interval = new TimeSpan(0, 0, notificationBar.CloseDelay);
        notificationBar.timer.Start();
      }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      this.timer.Stop();
      this.IsOpen = false;
    }

    private void Dismiss()
    {
      if (!this.IsOpen)
        return;
      this.timer.Stop();
      this.IsOpen = false;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/notificationbar.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}

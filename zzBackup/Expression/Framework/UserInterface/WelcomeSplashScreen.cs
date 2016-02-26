// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.WelcomeSplashScreen
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class WelcomeSplashScreen : Window
  {
    private string splashScreenImage;
    private bool canClose;
    private DateTime timeOpened;
    private DispatcherTimer timer;

    public string BackgroundImage
    {
      get
      {
        return this.splashScreenImage;
      }
    }

    public TimeSpan DismissDelay { get; set; }

    public bool CanClose
    {
      get
      {
        return this.canClose;
      }
      set
      {
        this.canClose = value;
      }
    }

    public WelcomeSplashScreen(string splashScreenImage)
    {
      this.splashScreenImage = splashScreenImage;
      this.Content = (object) FileTable.GetElement("Resources\\WelcomeSplashScreen.xaml");
      this.WindowStartupLocation = WindowStartupLocation.Manual;
      BitmapImage bitmapImage = new BitmapImage(new Uri(splashScreenImage));
      this.Left = (SystemParameters.WorkArea.Width - bitmapImage.Width) / 2.0 + SystemParameters.WorkArea.Left;
      this.Top = (SystemParameters.WorkArea.Height - bitmapImage.Height) / 2.0 + SystemParameters.WorkArea.Top;
      this.ShowInTaskbar = false;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.timeOpened = DateTime.Now);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      e.Cancel = !this.CanClose;
      TimeSpan timeSpan = DateTime.Now - this.timeOpened;
      if (!e.Cancel && timeSpan < this.DismissDelay)
      {
        e.Cancel = true;
        if (this.timer == null)
        {
          this.timer = new DispatcherTimer(DispatcherPriority.Input);
          this.timer.Tick += (EventHandler) ((sender, args) =>
          {
            this.timer.Stop();
            this.Close();
          });
        }
        this.timer.Interval = this.DismissDelay - timeSpan;
        this.timer.Start();
      }
      else if (this.timer != null)
        this.timer.Stop();
      base.OnClosing(e);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      IntPtr handle = new WindowInteropHelper((Window) this).Handle;
      int dwNewLong = UnsafeNativeMethods.GetWindowLong(handle, -20) | 128;
      UnsafeNativeMethods.SetWindowLong(handle, -20, dwNewLong);
    }
  }
}

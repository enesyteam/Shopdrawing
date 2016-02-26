// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingElement
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class FloatingElement : LayoutSynchronizedWindow
  {
    private bool closedInternally;

    protected bool IsClosing { get; private set; }

    public FloatingElement()
    {
      this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.OnIsVisibleChanged);
    }

    public void ForceClose()
    {
      this.closedInternally = true;
      if (this.IsClosing)
        return;
      this.Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (this.closedInternally)
      {
        this.TryActivateOwner();
      }
      else
      {
        this.IsClosing = true;
        try
        {
          base.OnClosing(e);
          if (e.Cancel)
            return;
          this.TryActivateOwner();
        }
        finally
        {
          this.IsClosing = false;
        }
      }
    }

    private void TryActivateOwner()
    {
      WindowInteropHelper windowInteropHelper = new WindowInteropHelper((Window) this);
      if (!(windowInteropHelper.Owner != IntPtr.Zero))
        return;
      NativeMethods.SetActiveWindow(windowInteropHelper.Owner);
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      if (this.IsVisible)
        DockManager.Instance.RegisterSite((Window) this);
      else
        DockManager.Instance.UnregisterSite((Visual) this);
    }
  }
}

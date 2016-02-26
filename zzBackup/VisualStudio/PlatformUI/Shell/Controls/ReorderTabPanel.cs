// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ReorderTabPanel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class ReorderTabPanel : Panel
  {
    public static readonly RoutedEvent PanelLayoutUpdatedEvent = EventManager.RegisterRoutedEvent("PanelLayoutUpdated", RoutingStrategy.Direct, typeof (RoutedEventHandler), typeof (ReorderTabPanel));

    public bool IsNotificationNeeded { get; set; }

    public ReorderTabPanel()
    {
      this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
      this.IsNotificationNeeded = false;
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
      if (!this.IsNotificationNeeded)
        return;
      this.RaiseEvent(new RoutedEventArgs(ReorderTabPanel.PanelLayoutUpdatedEvent));
      this.IsNotificationNeeded = false;
    }
  }
}

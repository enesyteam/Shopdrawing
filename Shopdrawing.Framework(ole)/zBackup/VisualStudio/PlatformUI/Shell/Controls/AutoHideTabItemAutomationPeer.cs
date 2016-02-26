// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideTabItemAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class AutoHideTabItemAutomationPeer : ButtonAutomationPeer
  {
    private View OwnerView
    {
      get
      {
        return ((FrameworkElement) this.Owner).DataContext as View;
      }
    }

    public AutoHideTabItemAutomationPeer(AutoHideTabItem element)
      : base((Button) element)
    {
    }

    protected override string GetItemTypeCore()
    {
      return "AutoHideTabItem";
    }

    protected override string GetAutomationIdCore()
    {
      View ownerView = this.OwnerView;
      if (ownerView != null)
        return "AUTOHIDE_" + ownerView.Name;
      return base.GetAutomationIdCore();
    }

    protected override string GetNameCore()
    {
      View ownerView = this.OwnerView;
      if (ownerView != null)
        return "AUTOHIDE_" + ownerView.Name;
      return base.GetAutomationIdCore();
    }
  }
}

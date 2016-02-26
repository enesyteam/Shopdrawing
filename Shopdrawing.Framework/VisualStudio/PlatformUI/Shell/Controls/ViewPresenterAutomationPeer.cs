// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewPresenterAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class ViewPresenterAutomationPeer : FrameworkElementAutomationPeer
  {
    protected View OwnerView
    {
      get
      {
        return ((FrameworkElement) this.Owner).DataContext as View;
      }
    }

    public ViewPresenterAutomationPeer(ViewPresenter viewPresenter)
      : base((FrameworkElement) viewPresenter)
    {
    }

    protected override string GetAutomationIdCore()
    {
      if (this.OwnerView == null)
        return base.GetAutomationIdCore();
      return this.OwnerView.Name;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }

    protected override string GetNameCore()
    {
      if (this.OwnerView == null)
        return base.GetAutomationIdCore();
      object title = this.OwnerView.Title;
      if (title != null)
        return title.ToString();
      return string.Empty;
    }

    protected override string GetClassNameCore()
    {
      return "ViewPresenter";
    }
  }
}

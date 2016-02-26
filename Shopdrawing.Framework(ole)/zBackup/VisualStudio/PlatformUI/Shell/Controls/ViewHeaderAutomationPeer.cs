// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewHeaderAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class ViewHeaderAutomationPeer : FrameworkElementAutomationPeer
  {
    protected View OwnerView
    {
      get
      {
        return ((ViewHeader) this.Owner).View;
      }
    }

    internal ViewHeaderAutomationPeer(ViewHeader header)
      : base((FrameworkElement) header)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.TitleBar;
    }

    protected override string GetNameCore()
    {
      if (this.OwnerView == null)
        return base.GetNameCore();
      if (this.OwnerView.Title != null)
        return "VIEWHEADER_" + this.OwnerView.Name.ToString();
      return string.Empty;
    }

    protected override string GetClassNameCore()
    {
      return "ToolWindowTitleBar";
    }
  }
}

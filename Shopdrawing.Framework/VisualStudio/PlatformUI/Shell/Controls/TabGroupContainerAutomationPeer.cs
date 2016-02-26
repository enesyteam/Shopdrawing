// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TabGroupContainerAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class TabGroupContainerAutomationPeer : FrameworkElementAutomationPeer
  {
    private ViewHeaderAutomationPeer m_viewHeaderPeer;
    private TabGroupControlAutomationPeer m_tabGroupPeer;
    private List<AutomationPeer> m_childPeers;

    protected TabGroupControl TabGroupOwner
    {
      get
      {
        return this.Owner as TabGroupControl;
      }
    }

    internal TabGroupContainerAutomationPeer(TabGroupControl owner)
      : base((FrameworkElement) owner)
    {
    }

    protected override string GetClassNameCore()
    {
      return "ToolWindowTabGroupContainer";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      bool flag = false;
      if (this.m_tabGroupPeer == null || this.m_tabGroupPeer.Owner != this.TabGroupOwner)
      {
        this.m_tabGroupPeer = new TabGroupControlAutomationPeer(this.TabGroupOwner);
        flag = true;
      }
      ViewHeader header = this.TabGroupOwner.Header;
      if (header != null && (this.m_viewHeaderPeer == null || this.m_viewHeaderPeer.Owner != header))
      {
        this.m_viewHeaderPeer = new ViewHeaderAutomationPeer(header);
        flag = true;
      }
      if (flag)
      {
        this.m_childPeers = new List<AutomationPeer>();
        if (this.m_viewHeaderPeer != null)
          this.m_childPeers.Add((AutomationPeer) this.m_viewHeaderPeer);
        if (this.m_tabGroupPeer != null)
          this.m_childPeers.Add((AutomationPeer) this.m_tabGroupPeer);
      }
      return this.m_childPeers;
    }

    protected override string GetNameCore()
    {
      TabGroupControl tabGroupControl = this.Owner as TabGroupControl;
      if (tabGroupControl != null)
      {
        View view = tabGroupControl.SelectedItem as View;
        if (view != null && view.Title != null)
          return view.Title.ToString();
      }
      return base.GetNameCore();
    }
  }
}

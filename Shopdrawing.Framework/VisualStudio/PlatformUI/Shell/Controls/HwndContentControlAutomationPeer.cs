// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.HwndContentControlAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class HwndContentControlAutomationPeer : FrameworkElementAutomationPeer
  {
    private Visual VisualSubtree { get; set; }

    internal HwndContentControlAutomationPeer(HwndContentControl owner, Visual visualSubtree)
      : base((FrameworkElement) owner)
    {
      this.VisualSubtree = visualSubtree;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      List<AutomationPeer> children = (List<AutomationPeer>) null;
      HwndContentControlAutomationPeer.IterateVisualChildren((DependencyObject) this.VisualSubtree, (HwndContentControlAutomationPeer.IteratorCallback) (peer =>
      {
        if (children == null)
          children = new List<AutomationPeer>();
        children.Add(peer);
      }));
      return children;
    }

    private static void IterateVisualChildren(DependencyObject parent, HwndContentControlAutomationPeer.IteratorCallback callback)
    {
      if (parent == null)
        return;
      int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
      for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, childIndex);
        UIElement element = child as UIElement;
        UIElement3D uiElement3D = child as UIElement3D;
        AutomationPeer peerForElement1;
        if (element != null && (peerForElement1 = UIElementAutomationPeer.CreatePeerForElement(element)) != null)
        {
          callback(peerForElement1);
        }
        else
        {
          AutomationPeer peerForElement2;
          if (uiElement3D != null && (peerForElement2 = UIElement3DAutomationPeer.CreatePeerForElement((UIElement3D) child)) != null)
            callback(peerForElement2);
          else
            HwndContentControlAutomationPeer.IterateVisualChildren(child, callback);
        }
      }
    }

    private delegate void IteratorCallback(AutomationPeer peer);
  }
}

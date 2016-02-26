// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockSiteAdornerAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockSiteAdornerAutomationPeer : FrameworkElementAutomationPeer
  {
    public DockSiteAdornerAutomationPeer(DockSiteAdorner adorner)
      : base((FrameworkElement) adorner)
    {
    }

    protected override string GetClassNameCore()
    {
      return "DockSiteAdorner";
    }

    protected override string GetNameCore()
    {
      DockSiteAdorner dockSiteAdorner = (DockSiteAdorner) this.Owner;
      return dockSiteAdorner.AdornedDockTarget == null || dockSiteAdorner.AdornedDockTarget.TargetElement == null ? "Main" : (dockSiteAdorner.AdornedDockTarget.TargetElement is DocumentGroup || dockSiteAdorner.AdornedDockTarget.TargetElement is DocumentGroupContainer ? "Document" : ExtensionMethods.GetAutomationPeerCaption(dockSiteAdorner.AdornedDockTarget.TargetElement));
    }

    protected override string GetAutomationIdCore()
    {
      DockSiteAdorner dockSiteAdorner = (DockSiteAdorner) this.Owner;
      DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject) dockSiteAdorner);
      return (parent == null || !(parent is DockGroupAdorner) && !(VisualTreeHelper.GetParent(parent) is DockGroupAdorner) ? "Dock_" : "DockGroup_") + dockSiteAdorner.DockDirection.ToString();
    }
  }
}

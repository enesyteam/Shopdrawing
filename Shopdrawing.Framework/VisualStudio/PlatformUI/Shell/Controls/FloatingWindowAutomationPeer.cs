// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingWindowAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class FloatingWindowAutomationPeer : WindowAutomationPeer
  {
    private FloatSite OwnerFloatSite
    {
      get
      {
        return ((FrameworkElement) this.Owner).DataContext as FloatSite;
      }
    }

    private ViewElement OwnerViewElement
    {
      get
      {
        FloatSite ownerFloatSite = this.OwnerFloatSite;
        if (ownerFloatSite != null)
          return ownerFloatSite.Child;
        return (ViewElement) null;
      }
    }

    internal FloatingWindowAutomationPeer(FloatingWindow floatingWindow)
      : base((Window) floatingWindow)
    {
    }

    protected override string GetClassNameCore()
    {
      return "FloatingWindow";
    }

    protected override string GetNameCore()
    {
      if (this.OwnerViewElement != null)
        return ExtensionMethods.GetAutomationPeerCaption(this.OwnerViewElement);
      return string.Empty;
    }
  }
}

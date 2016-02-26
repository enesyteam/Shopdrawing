// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewFrameAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class ViewFrameAutomationPeer : ViewPresenterAutomationPeer
  {
    public ViewFrameAutomationPeer(ViewFrame frame)
      : base((ViewPresenter) frame)
    {
    }

    protected override string GetAutomationIdCore()
    {
      View ownerView = this.OwnerView;
      if (ownerView != null)
        return ownerView.Name;
      return base.GetAutomationIdCore();
    }

    protected override string GetNameCore()
    {
      View ownerView = this.OwnerView;
      if (ownerView != null)
        return ownerView.Name;
      return base.GetNameCore();
    }

    protected override string GetClassNameCore()
    {
      return "ViewFrame";
    }
  }
}

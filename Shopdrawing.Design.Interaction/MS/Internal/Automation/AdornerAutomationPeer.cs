// Decompiled with JetBrains decompiler
// Type: MS.Internal.Automation.AdornerAutomationPeer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace MS.Internal.Automation
{
  internal class AdornerAutomationPeer : UIElementAutomationPeer
  {
    private UIElement _adorner;

    public AdornerAutomationPeer(UIElement adorner)
      : base(adorner)
    {
      this._adorner = adorner;
    }

    protected override string GetAutomationIdCore()
    {
      string str = AutomationProperties.GetAutomationId((DependencyObject) this._adorner);
      if (string.IsNullOrEmpty(str))
        str = this._adorner.GetType().Name;
      return str;
    }

    protected override string GetNameCore()
    {
      return this.GetAutomationIdCore();
    }

    protected override string GetItemTypeCore()
    {
      return this.GetAutomationIdCore();
    }

    protected override string GetClassNameCore()
    {
      return this._adorner.GetType().Name;
    }

    protected override bool IsContentElementCore()
    {
      return false;
    }

    protected override bool IsControlElementCore()
    {
      return false;
    }

    protected override Point GetClickablePointCore()
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(this._adorner);
      if (peerForElement != null)
        return peerForElement.GetClickablePoint();
      return base.GetClickablePointCore();
    }

    protected override Rect GetBoundingRectangleCore()
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(this._adorner);
      if (peerForElement != null)
        return peerForElement.GetBoundingRectangle();
      return base.GetBoundingRectangleCore();
    }
  }
}

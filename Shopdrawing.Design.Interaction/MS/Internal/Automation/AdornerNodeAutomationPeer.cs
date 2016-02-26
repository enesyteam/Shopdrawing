// Decompiled with JetBrains decompiler
// Type: MS.Internal.Automation.AdornerNodeAutomationPeer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using MS.Internal.Properties;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

namespace MS.Internal.Automation
{
  internal class AdornerNodeAutomationPeer : AutomationPeer
  {
    private List<UIElement> _list;

    public AdornerNodeAutomationPeer(List<UIElement> list)
    {
      this._list = list;
    }

    protected override string GetAcceleratorKeyCore()
    {
      return string.Empty;
    }

    protected override string GetAccessKeyCore()
    {
      return string.Empty;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Custom;
    }

    protected override string GetAutomationIdCore()
    {
      return "Adorners";
    }

    protected override Rect GetBoundingRectangleCore()
    {
      return new Rect();
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      List<AutomationPeer> list = new List<AutomationPeer>();
      if (this._list != null)
      {
        foreach (UIElement adorner in this._list)
          AdornerNodeAutomationPeer.AddAdornerAutomationPeer(list, adorner);
      }
      return list;
    }

    private static void AddAdornerAutomationPeer(List<AutomationPeer> list, UIElement adorner)
    {
      list.Add((AutomationPeer) AutomationPeerCache.Create<AdornerAutomationPeer>(adorner));
    }

    protected override string GetClassNameCore()
    {
      return "Adorners";
    }

    protected override Point GetClickablePointCore()
    {
      return new Point();
    }

    protected override string GetHelpTextCore()
    {
        return MS.Internal.Properties.Resources.AdornerNodeAutomationPeer_HelpText;
    }

    protected override string GetItemStatusCore()
    {
      return string.Empty;
    }

    protected override string GetItemTypeCore()
    {
        return MS.Internal.Properties.Resources.AdornerNodeAutomationPeer_ItemType;
    }

    protected override AutomationPeer GetLabeledByCore()
    {
      return (AutomationPeer) null;
    }

    protected override string GetNameCore()
    {
        return MS.Internal.Properties.Resources.AdornerNodeAutomationPeer_Name;
    }

    protected override AutomationOrientation GetOrientationCore()
    {
      return AutomationOrientation.None;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      return (object) null;
    }

    protected override bool HasKeyboardFocusCore()
    {
      return false;
    }

    protected override bool IsContentElementCore()
    {
      return false;
    }

    protected override bool IsControlElementCore()
    {
      return false;
    }

    protected override bool IsEnabledCore()
    {
      return true;
    }

    protected override bool IsKeyboardFocusableCore()
    {
      return false;
    }

    protected override bool IsOffscreenCore()
    {
      return false;
    }

    protected override bool IsPasswordCore()
    {
      return false;
    }

    protected override bool IsRequiredForFormCore()
    {
      return false;
    }

    protected override void SetFocusCore()
    {
    }
  }
}

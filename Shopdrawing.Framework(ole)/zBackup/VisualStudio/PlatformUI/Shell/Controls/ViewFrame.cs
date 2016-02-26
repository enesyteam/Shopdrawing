// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewFrame
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class ViewFrame : ViewPresenter
  {
    static ViewFrame()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ViewFrame), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ViewFrame)));
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new ViewFrameAutomationPeer(this);
    }
  }
}

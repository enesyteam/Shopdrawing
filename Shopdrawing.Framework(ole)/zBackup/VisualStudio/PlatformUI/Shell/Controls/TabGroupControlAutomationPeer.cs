// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TabGroupControlAutomationPeer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  internal class TabGroupControlAutomationPeer : TabControlAutomationPeer
  {
    protected TabGroupControl TabGroupOwner
    {
      get
      {
        return this.Owner as TabGroupControl;
      }
    }

    internal TabGroupControlAutomationPeer(TabGroupControl owner)
      : base((TabControl) owner)
    {
    }

    protected override Rect GetBoundingRectangleCore()
    {
      double num = this.TabGroupOwner.Header != null ? this.TabGroupOwner.Header.RenderSize.Height : 0.0;
      Rect boundingRectangleCore = base.GetBoundingRectangleCore();
      return new Rect(boundingRectangleCore.X, boundingRectangleCore.Y + num, boundingRectangleCore.Width, boundingRectangleCore.Height - num);
    }

    protected override string GetClassNameCore()
    {
      return "ToolWindowTabGroup";
    }
  }
}

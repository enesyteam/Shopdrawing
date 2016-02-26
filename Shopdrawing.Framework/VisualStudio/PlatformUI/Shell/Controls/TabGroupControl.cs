// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TabGroupControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class TabGroupControl : GroupControl
  {
    private static ResourceKey tabItemStyleKey;

    internal ViewHeader Header
    {
      get
      {
        return this.GetTemplateChild("PART_Header") as ViewHeader;
      }
    }

    public static ResourceKey TabItemStyleKey
    {
      get
      {
        return TabGroupControl.tabItemStyleKey ?? (TabGroupControl.tabItemStyleKey = (ResourceKey) new StyleKey<TabGroupControl>());
      }
    }

    static TabGroupControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TabGroupControl), (PropertyMetadata) new FrameworkPropertyMetadata( typeof (TabGroupControl)));
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      View view = item as View;
      if (view != null)
        AutomationProperties.SetName(element, "TAB_" + view.Name);
      ((FrameworkElement) element).SetResourceReference(FrameworkElement.StyleProperty, TabGroupControl.TabItemStyleKey);
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      StringBuilder stringBuilder = new StringBuilder("TabGroup");
      foreach (object obj in (IEnumerable) this.Items)
      {
        stringBuilder.Append("|");
        View view = obj as View;
        if (view != null)
          stringBuilder.Append(view.Name);
        else
          stringBuilder.Append("Unknown");
      }
      AutomationProperties.SetAutomationId((DependencyObject) this, stringBuilder.ToString());
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new TabGroupContainerAutomationPeer(this);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DocumentGroupControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DocumentGroupControl : GroupControl
  {
    private static ResourceKey tabItemStyleKey;

    public static ResourceKey TabItemStyleKey
    {
      get
      {
        return DocumentGroupControl.tabItemStyleKey ?? (DocumentGroupControl.tabItemStyleKey = (ResourceKey) new StyleKey<DocumentGroupControl>());
      }
    }

    static DocumentGroupControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DocumentGroupControl), (PropertyMetadata) new FrameworkPropertyMetadata(typeof (DocumentGroupControl)));
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      ((FrameworkElement) element).SetResourceReference(FrameworkElement.StyleProperty, (object) DocumentGroupControl.TabItemStyleKey);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new DocumentGroupControlAutomationPeer(this);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideGroupControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class AutoHideGroupControl : LayoutSynchronizedItemsControl
  {
    static AutoHideGroupControl()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AutoHideGroupControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AutoHideGroupControl)));
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is AutoHideTabItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new AutoHideTabItem();
    }
  }
}

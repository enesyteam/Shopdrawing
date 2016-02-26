// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterItem
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterItem : ContentControl
  {
    static SplitterItem()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterItem)));
    }

    public SplitterItem()
    {
      AutomationProperties.SetAutomationId((DependencyObject) this, "SplitterItem");
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewHeader
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class ViewHeader : Control
  {
    public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof (View), typeof (ViewHeader));
    public static readonly DependencyProperty ContainingElementProperty = DependencyProperty.Register("ContainingElement", typeof (ViewElement), typeof (ViewHeader));
    public static readonly DependencyProperty ContainingFrameworkElementProperty = DependencyProperty.Register("ContainingFrameworkElement", typeof (FrameworkElement), typeof (ViewHeader));

    public View View
    {
      get
      {
        return (View) this.GetValue(ViewHeader.ViewProperty);
      }
      set
      {
        this.SetValue(ViewHeader.ViewProperty, (object) value);
      }
    }

    public ViewElement ContainingElement
    {
      get
      {
        return (ViewElement) this.GetValue(ViewHeader.ContainingElementProperty);
      }
      set
      {
        this.SetValue(ViewHeader.ContainingElementProperty, (object) value);
      }
    }

    public FrameworkElement ContainingFrameworkElement
    {
      get
      {
        return (FrameworkElement) this.GetValue(ViewHeader.ContainingElementProperty);
      }
      set
      {
        this.SetValue(ViewHeader.ContainingElementProperty, (object) value);
      }
    }

    static ViewHeader()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ViewHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ViewHeader)));
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new ViewHeaderAutomationPeer(this);
    }
  }
}

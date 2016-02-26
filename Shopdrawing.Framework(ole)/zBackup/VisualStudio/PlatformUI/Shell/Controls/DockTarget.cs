// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.DockTarget
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class DockTarget : Border
  {
    public static readonly DependencyProperty DockTargetTypeProperty = DependencyProperty.Register("DockTargetType", typeof (DockTargetType), typeof (DockTarget), (PropertyMetadata) new FrameworkPropertyMetadata((object) DockTargetType.Inside));
    public static readonly DependencyProperty DockSiteTypeProperty = DependencyProperty.Register("DockSiteType", typeof (DockSiteType), typeof (DockTarget), (PropertyMetadata) new FrameworkPropertyMetadata((object) DockSiteType.Default));
    public static readonly DependencyProperty AdornmentTargetProperty = DependencyProperty.Register("AdornmentTarget", typeof (FrameworkElement), typeof (DockTarget));

    public DockTargetType DockTargetType
    {
      get
      {
        return (DockTargetType) this.GetValue(DockTarget.DockTargetTypeProperty);
      }
      set
      {
        this.SetValue(DockTarget.DockTargetTypeProperty, (object) value);
      }
    }

    public DockSiteType DockSiteType
    {
      get
      {
        return (DockSiteType) this.GetValue(DockTarget.DockSiteTypeProperty);
      }
      set
      {
        this.SetValue(DockTarget.DockSiteTypeProperty, (object) value);
      }
    }

    public FrameworkElement AdornmentTarget
    {
      get
      {
        return (FrameworkElement) this.GetValue(DockTarget.AdornmentTargetProperty);
      }
      set
      {
        this.SetValue(DockTarget.AdornmentTargetProperty, (object) value);
      }
    }

    public ViewElement TargetElement
    {
      get
      {
        return this.AdornmentTarget == null ? this.DataContext as ViewElement : this.AdornmentTarget.DataContext as ViewElement;
      }
    }
  }
}

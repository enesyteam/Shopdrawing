// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Preferences.ViewManagerPreferences
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Preferences
{
  public class ViewManagerPreferences : DependencyObject
  {
    public static readonly DependencyProperty DocumentDockPreferenceProperty = DependencyProperty.Register("DocumentDockPreference", typeof (DockPreference), typeof (ViewManagerPreferences), new PropertyMetadata( DockPreference.DockAtBeginning));
    public static readonly DependencyProperty TabDockPreferenceProperty = DependencyProperty.Register("TabDockPreference", typeof (DockPreference), typeof (ViewManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
    public static readonly DependencyProperty AllowDocumentTabAutoDockingProperty = DependencyProperty.Register("AllowDocumentTabAutoDocking", typeof (bool), typeof (ViewManagerPreferences), new PropertyMetadata(false));
    public static readonly DependencyProperty AllowTabGroupTabAutoDockingProperty = DependencyProperty.Register("AllowTabGroupTabAutoDocking", typeof (bool), typeof (ViewManagerPreferences), new PropertyMetadata( false));
    public static readonly DependencyProperty AutoHideHoverDelayProperty = DependencyProperty.Register("AutoHideHoverDelay", typeof (TimeSpan), typeof (ViewManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(250.0)));
    public static readonly DependencyProperty AutoHideMouseExitGracePeriodProperty = DependencyProperty.Register("AutoHideMouseExitGracePeriod", typeof (TimeSpan), typeof (ViewManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
    public static readonly DependencyProperty HideOnlyActiveViewProperty = DependencyProperty.Register("HideOnlyActiveView", typeof (bool), typeof (ViewManagerPreferences), new PropertyMetadata(true));
    public static readonly DependencyProperty AutoHideOnlyActiveViewProperty = DependencyProperty.Register("AutoHideOnlyActiveView", typeof (bool), typeof (ViewManagerPreferences), new PropertyMetadata(false));
    public static readonly DependencyProperty AlwaysHideAllViewInFloatingTabGroupProperty = DependencyProperty.Register("AlwaysHideAllViewInFloatingTabGroup", typeof (bool), typeof (ViewManagerPreferences), new PropertyMetadata(true));

    public TimeSpan AutoHideMouseExitGracePeriod
    {
      get
      {
        return (TimeSpan) this.GetValue(ViewManagerPreferences.AutoHideMouseExitGracePeriodProperty);
      }
      set
      {
        this.SetValue(ViewManagerPreferences.AutoHideMouseExitGracePeriodProperty, (object) value);
      }
    }

    public DockPreference DocumentDockPreference
    {
      get
      {
        return (DockPreference) this.GetValue(ViewManagerPreferences.DocumentDockPreferenceProperty);
      }
      set
      {
        this.SetValue(ViewManagerPreferences.DocumentDockPreferenceProperty, (object) value);
      }
    }

    public DockPreference TabDockPreference
    {
      get
      {
        return (DockPreference) this.GetValue(ViewManagerPreferences.TabDockPreferenceProperty);
      }
      set
      {
        this.SetValue(ViewManagerPreferences.TabDockPreferenceProperty, (object) value);
      }
    }

    public TimeSpan AutoHideHoverDelay
    {
      get
      {
        return (TimeSpan) this.GetValue(ViewManagerPreferences.AutoHideHoverDelayProperty);
      }
      set
      {
        this.SetValue(ViewManagerPreferences.AutoHideHoverDelayProperty, (object) value);
      }
    }

    public bool AllowDocumentTabAutoDocking
    {
        get
        {
            return (bool)base.GetValue(ViewManagerPreferences.AllowDocumentTabAutoDockingProperty);
        }
        set
        {
            base.SetValue(ViewManagerPreferences.AllowDocumentTabAutoDockingProperty, value);
        }
    }

    public bool AllowTabGroupTabAutoDocking
    {
        get
        {
            return (bool)base.GetValue(ViewManagerPreferences.AllowTabGroupTabAutoDockingProperty);
        }
        set
        {
            base.SetValue(ViewManagerPreferences.AllowTabGroupTabAutoDockingProperty, value);
        }
    }

    public bool HideOnlyActiveView
    {
        get
        {
            return (bool)base.GetValue(ViewManagerPreferences.HideOnlyActiveViewProperty);
        }
        set
        {
            base.SetValue(ViewManagerPreferences.HideOnlyActiveViewProperty, value);
        }
    }

    public bool AutoHideOnlyActiveView
    {
        get
        {
            return (bool)base.GetValue(ViewManagerPreferences.AutoHideOnlyActiveViewProperty);
        }
        set
        {
            base.SetValue(ViewManagerPreferences.AutoHideOnlyActiveViewProperty, value);
        }
    }

    public bool AlwaysHideAllViewInFloatingTabGroup
    {
        get
        {
            return (bool)base.GetValue(ViewManagerPreferences.AlwaysHideAllViewInFloatingTabGroupProperty);
        }
        set
        {
            base.SetValue(ViewManagerPreferences.AlwaysHideAllViewInFloatingTabGroupProperty, value);
        }
    }
  }
}

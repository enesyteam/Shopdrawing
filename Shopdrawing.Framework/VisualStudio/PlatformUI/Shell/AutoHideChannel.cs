// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.AutoHideChannel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class AutoHideChannel : ViewGroup
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (AutoHideChannel));

    public Dock Dock
    {
      get
      {
        return (Dock) this.GetValue(DockPanel.DockProperty);
      }
      set
      {
        this.SetValue(DockPanel.DockProperty, (object) value);
      }
    }

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(AutoHideChannel.OrientationProperty);
      }
      set
      {
        this.SetValue(AutoHideChannel.OrientationProperty, (object) value);
      }
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      return element is AutoHideGroup;
    }

    public static bool IsAutoHidden(ViewElement element)
    {
      return Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<AutoHideChannel, ViewElement>(element, (Func<ViewElement, ViewElement>) (e =>
      {
        if (e == null)
          return (ViewElement) null;
        return (ViewElement) e.Parent;
      })) != null;
    }

    public static AutoHideChannel Create()
    {
      return ViewElementFactory.Current.CreateAutoHideChannel();
    }

    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new AutoHideChannelCustomSerializer(this);
    }
  }
}

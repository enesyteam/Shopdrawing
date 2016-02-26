// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.StandaloneView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System.Windows;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class StandaloneView : ConstrainedView, ICustomXmlSerializable
  {
    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new StandaloneViewCustomSerializer(this);
    }

    public override void ApplyFloatingConstraints(DockAdornerWindow adornerLayer, DockTarget target)
    {
      DockAdornerWindow dockAdornerWindow = adornerLayer;
      int num1 = dockAdornerWindow.IsInnerCenterTargetEnabled ? 1 : 0;
      int num2 = 0;
      dockAdornerWindow.IsInnerCenterTargetEnabled = num2 != 0;
    }

    public override void ApplyDockTargetConstraints(DockAdornerWindow adornerLayer, ViewElement floatingElement)
    {
      DockAdornerWindow dockAdornerWindow = adornerLayer;
      int num1 = dockAdornerWindow.IsInnerCenterTargetEnabled ? 1 : 0;
      int num2 = 0;
      dockAdornerWindow.IsInnerCenterTargetEnabled = num2 != 0;
    }

    public override bool IsFloatingElementValidForDock(ViewElement floatingElement)
    {
      DependencyObject dependencyObject = this.Content as DependencyObject;
      if (dependencyObject != null)
        return !(bool) dependencyObject.GetValue(FloatingWindow.IsFloatingProperty);
      return true;
    }

    public override bool IsDockTargetValidForDock(DockTarget target)
    {
      if (target.DockTargetType != DockTargetType.Inside && target.DockTargetType != DockTargetType.Outside)
        return target.DockTargetType == DockTargetType.SidesOnly;
      return true;
    }

    public override bool IsValidFillPreviewOperation(DockTarget target, ViewElement floatingElement)
    {
      return false;
    }

    public override bool CanAutoDock()
    {
      return false;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ExpressionDockManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public class ExpressionDockManager : DockManager
  {
    internal ExpressionDockManager()
    {
    }

    protected override bool IsValidDockTarget(DockTarget target, IList<DockSiteType> types, ViewElement floatingElement)
    {
      if (!base.IsValidDockTarget(target, types, floatingElement))
        return false;
      ConstrainedView constrainedView1 = target.TargetElement as ConstrainedView;
      ConstrainedView constrainedView2 = floatingElement as ConstrainedView;
      if (constrainedView1 != null && !constrainedView1.IsFloatingElementValidForDock(floatingElement))
        return false;
      if (constrainedView2 != null)
        return constrainedView2.IsDockTargetValidForDock(target);
      return true;
    }

    protected override void PrepareAdornerLayer(DockAdornerWindow adornerLayer, ViewElement floatingElement)
    {
      DockTarget target = (DockTarget) adornerLayer.AdornedElement;
      ConstrainedView constrainedView1 = target.TargetElement as ConstrainedView;
      ConstrainedView constrainedView2 = floatingElement as ConstrainedView;
      if (constrainedView2 != null)
        constrainedView2.ApplyFloatingConstraints(adornerLayer, target);
      if (constrainedView1 == null)
        return;
      constrainedView1.ApplyDockTargetConstraints(adornerLayer, floatingElement);
    }

    protected override bool IsValidFillPreviewOperation(DockTarget dockTarget, ViewElement dockingView)
    {
      if (!base.IsValidFillPreviewOperation(dockTarget, dockingView))
        return false;
      ConstrainedView constrainedView1 = dockTarget.TargetElement as ConstrainedView;
      ConstrainedView constrainedView2 = dockingView as ConstrainedView;
      if (constrainedView1 != null && !constrainedView1.IsValidFillPreviewOperation(dockTarget, dockingView))
        return false;
      if (constrainedView2 != null)
        return constrainedView2.IsValidFillPreviewOperation(dockTarget, dockingView);
      return true;
    }

    internal override DraggedTabInfo GetAutodockTarget(DragAbsoluteEventArgs args)
    {
      FloatingElement ancestor = Microsoft.VisualStudio.PlatformUI.ExtensionMethods.FindAncestor<FloatingElement>((Visual) (args.OriginalSource as DragUndockHeader));
      if (ancestor != null)
      {
        FloatSite floatSite = ancestor.DataContext as FloatSite;
        if (floatSite != null)
        {
          ConstrainedView constrainedView = floatSite.Child as ConstrainedView;
          if (constrainedView != null && !constrainedView.CanAutoDock())
            return (DraggedTabInfo) null;
        }
      }
      return base.GetAutodockTarget(args);
    }

    protected override IDockPreviewWindow CreateDockPreviewWindow()
    {
      return (IDockPreviewWindow) new ExpressionDockPreviewWindow();
    }

    protected override void OnDockPreviewWindowShowing(IDockPreviewWindow dockPreviewWindow, DockDirection dockDirection)
    {
      ((ExpressionDockPreviewWindow) dockPreviewWindow).IsFillPreview = dockDirection == DockDirection.Fill;
    }
  }
}

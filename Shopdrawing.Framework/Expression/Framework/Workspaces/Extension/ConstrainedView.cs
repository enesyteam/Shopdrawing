// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Workspaces.Extension.ConstrainedView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
  public abstract class ConstrainedView : ExpressionView
  {
    public abstract void ApplyFloatingConstraints(DockAdornerWindow adornerLayer, DockTarget target);

    public abstract void ApplyDockTargetConstraints(DockAdornerWindow adornerLayer, ViewElement floatingElement);

    public abstract bool IsFloatingElementValidForDock(ViewElement floatingElement);

    public abstract bool IsDockTargetValidForDock(DockTarget target);

    public abstract bool IsValidFillPreviewOperation(DockTarget target, ViewElement floatingElement);

    public abstract bool CanAutoDock();
  }
}

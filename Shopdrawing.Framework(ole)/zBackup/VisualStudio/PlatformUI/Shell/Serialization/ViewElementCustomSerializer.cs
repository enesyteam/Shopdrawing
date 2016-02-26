// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Serialization.ViewElementCustomSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Serialization
{
  public class ViewElementCustomSerializer : DependencyObjectCustomSerializer
  {
    protected override IEnumerable<DependencyProperty> SerializableProperties
    {
      get
      {
        yield return ViewElement.IsSelectedProperty;
        yield return ViewElement.IsVisibleProperty;
        yield return ViewElement.DockedHeightProperty;
        yield return ViewElement.DockedWidthProperty;
        yield return ViewElement.AutoHideWidthProperty;
        yield return ViewElement.AutoHideHeightProperty;
        yield return ViewElement.FloatingTopProperty;
        yield return ViewElement.FloatingLeftProperty;
        yield return ViewElement.FloatingHeightProperty;
        yield return ViewElement.FloatingWidthProperty;
        yield return ViewElement.FloatingWindowStateProperty;
        yield return ViewElement.DockRestrictionProperty;
        yield return ViewElement.AreDockTargetsEnabledProperty;
        yield return ViewElement.MinimumWidthProperty;
        yield return ViewElement.MinimumHeightProperty;
      }
    }

    public ViewElementCustomSerializer(ViewElement element)
      : base((IDependencyObjectCustomSerializerAccess) element)
    {
    }
  }
}

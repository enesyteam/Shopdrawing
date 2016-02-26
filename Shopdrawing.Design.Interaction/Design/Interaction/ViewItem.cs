// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ViewItem
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public abstract class ViewItem
  {
    public abstract object PlatformObject { get; }

    public abstract Type ItemType { get; }

    public abstract FlowDirection FlowDirection { get; }

    public abstract bool IsOffscreen { get; }

    public abstract bool IsVisible { get; }

    public abstract Transform LayoutTransform { get; }

    public abstract IEnumerable<ViewItem> LogicalChildren { get; }

    public abstract ViewItem LogicalParent { get; }

    public abstract Vector Offset { get; }

    public abstract Size RenderSize { get; }

    public abstract Rect RenderSizeBounds { get; }

    public abstract Rect SelectionFrameBounds { get; }

    public abstract Transform Transform { get; }

    public abstract Visibility Visibility { get; }

    public abstract IEnumerable<ViewItem> VisualChildren { get; }

    public abstract ViewItem VisualParent { get; }

    public static bool operator ==(ViewItem obj1, ViewItem obj2)
    {
      if (object.ReferenceEquals((object) obj1, (object) obj2))
        return true;
      if (object.ReferenceEquals((object) obj1, (object) null) || object.ReferenceEquals((object) obj2, (object) null))
        return false;
      return obj1.Equals((object) obj2);
    }

    public static bool operator !=(ViewItem obj1, ViewItem obj2)
    {
      if (object.ReferenceEquals((object) obj1, (object) obj2))
        return false;
      if (object.ReferenceEquals((object) obj1, (object) null) || object.ReferenceEquals((object) obj2, (object) null))
        return true;
      return !obj1.Equals((object) obj2);
    }

    public abstract bool IsDescendantOf(Visual ancestor);

    public abstract bool IsDescendantOf(ViewItem ancestor);

    public abstract ViewHitTestResult HitTest(ViewHitTestFilterCallback filterCallback, ViewHitTestResultCallback resultCallback, HitTestParameters hitTestParameters);

    public abstract Point PointToScreen(Point point);

    public abstract GeneralTransform TransformFromVisual(Visual visual);

    public abstract GeneralTransform TransformToView(ViewItem view);

    public abstract GeneralTransform TransformToVisual(Visual visual);

    public abstract void UpdateLayout();

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}

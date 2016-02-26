// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.NestedGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public abstract class NestedGroup : ViewGroup
  {
    protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
    {
      base.OnChildrenChanged(args);
      if (args.NewItems == null)
        return;
      this.SnapChildrenSizesToSelf((IEnumerable) args.NewItems);
    }

    protected override void ValidateDockedWidth(SplitterLength width)
    {
      if (width.IsFill)
        throw new ArgumentException("NestedGroup does not accept Fill values for DockedWidth.");
    }

    protected override void ValidateDockedHeight(SplitterLength height)
    {
      if (height.IsFill)
        throw new ArgumentException("NestedGroup does not accept Fill values for DockedHeight.");
    }

    protected override void OnDockedWidthChanged()
    {
      base.OnDockedWidthChanged();
      this.SnapChildrenSizesToSelf((IEnumerable) this.Children);
    }

    protected override void OnDockedHeightChanged()
    {
      base.OnDockedHeightChanged();
      this.SnapChildrenSizesToSelf((IEnumerable) this.Children);
    }

    private void SnapChildrenSizesToSelf(IEnumerable children)
    {
      foreach (ViewElement viewElement in children)
      {
        if (!viewElement.DockedWidth.IsFixed)
          viewElement.DockedWidth = this.DockedWidth;
        if (!viewElement.DockedHeight.IsFixed)
          viewElement.DockedHeight = this.DockedHeight;
      }
    }

    public override bool IsChildOnScreen(int childIndex)
    {
      if (this.IsOnScreen)
        return this.Children[childIndex].IsSelected;
      return false;
    }
  }
}

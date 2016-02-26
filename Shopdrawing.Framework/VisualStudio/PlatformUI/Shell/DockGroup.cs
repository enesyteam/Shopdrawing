// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.DockGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class DockGroup : ViewGroup
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Orientation), typeof (DockGroup));

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(DockGroup.OrientationProperty);
      }
      set
      {
        this.SetValue(DockGroup.OrientationProperty, (object) value);
      }
    }

    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new DockGroupCustomSerializer(this);
    }

    public override bool IsChildAllowed(ViewElement element)
    {
      if (!(element is DockGroup) && !(element is View) && !(element is ViewBookmark))
        return element is TabGroup;
      return true;
    }

    protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
    {
      base.OnChildrenChanged(args);
      if (args.NewItems == null)
        return;
      this.SnapChildrenSizesToSelf((IEnumerable) args.NewItems);
    }

    protected override void OnDockedHeightChanged()
    {
      base.OnDockedHeightChanged();
      if (this.Orientation != Orientation.Horizontal)
        return;
      this.SnapChildrenSizesToSelf((IEnumerable) this.Children);
    }

    protected override void OnDockedWidthChanged()
    {
      base.OnDockedWidthChanged();
      if (this.Orientation != Orientation.Vertical)
        return;
      this.SnapChildrenSizesToSelf((IEnumerable) this.Children);
    }

    private void SnapChildrenSizesToSelf(IEnumerable children)
    {
      if (this.Orientation == Orientation.Horizontal)
      {
        if (this.DockedHeight.IsFill)
          return;
        foreach (ViewElement viewElement in children)
        {
          if (!viewElement.DockedHeight.IsFixed)
            viewElement.DockedHeight = this.DockedHeight;
        }
      }
      else
      {
        if (this.DockedWidth.IsFill)
          return;
        foreach (ViewElement viewElement in children)
        {
          if (!viewElement.DockedWidth.IsFixed)
            viewElement.DockedWidth = this.DockedWidth;
        }
      }
    }

    protected override void UpdateHasMultipleOnScreenViews()
    {
      if (this.VisibleChildren.Count > 1)
        this.HasMultipleOnScreenViews = true;
      else
        base.UpdateHasMultipleOnScreenViews();
    }

    public static DockGroup Create()
    {
      return ViewElementFactory.Current.CreateDockGroup();
    }
  }
}

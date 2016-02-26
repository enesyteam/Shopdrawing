// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewGroup
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  [DefaultProperty("Children")]
  [ContentProperty("Children")]
  public abstract class ViewGroup : ViewElement
  {
    private static readonly DependencyPropertyKey HasMultipleOnScreenViewsPropertyKey = DependencyProperty.RegisterReadOnly("HasMultipleOnScreenViews", typeof (bool), typeof (ViewGroup), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ViewGroup.OnHasMultipleOnScreenViewsChanged)));
    public static readonly DependencyProperty HasMultipleOnScreenViewsProperty = ViewGroup.HasMultipleOnScreenViewsPropertyKey.DependencyProperty;
    public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register("SelectedElement", typeof (ViewElement), typeof (ViewGroup), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ViewGroup.OnSelectedElementChanged)));
    private ObservableCollection<ViewElement> visibleChildren;
    private ImmutableObservableCollection<ViewElement> readOnlyVisibleChildren;

    public IObservableCollection<ViewElement> Children { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ViewElement SelectedElement
    {
      get
      {
        return (ViewElement) this.GetValue(ViewGroup.SelectedElementProperty);
      }
      set
      {
        this.SetValue(ViewGroup.SelectedElementProperty, (object) value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasMultipleOnScreenViews
    {
        get
        {
            return (bool)base.GetValue(ViewGroup.HasMultipleOnScreenViewsProperty);
        }
        protected set
        {
            base.SetValue(ViewGroup.HasMultipleOnScreenViewsPropertyKey, value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IObservableCollection<ViewElement> VisibleChildren
    {
      get
      {
        if (this.visibleChildren == null)
          this.InitializeVisibleChildren();
        return (IObservableCollection<ViewElement>) this.readOnlyVisibleChildren;
      }
    }

    protected ViewGroup()
    {
      this.Children = (IObservableCollection<ViewElement>) new ViewGroup.ViewElementCollection(this);
      this.Children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
    }

    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new ViewGroupCustomSerializer(this);
    }

    private static void OnSelectedElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ViewGroup viewGroup = (ViewGroup) obj;
      ViewElement viewElement1 = args.NewValue as ViewElement;
      ViewElement viewElement2 = args.OldValue as ViewElement;
      if (viewElement1 != null && !viewGroup.Children.Contains(viewElement1))
        throw new InvalidOperationException("SelectedElement must be a child of the group");
      if (viewElement2 != null)
        viewElement2.IsSelected = false;
      if (viewElement1 != null)
        viewElement1.IsSelected = true;
      viewGroup.OnSelectedElementChanged();
    }

    public int ChildIndexFromVisibleChildIndex(int visiblePosition)
    {
      int num = -1;
      int index;
      for (index = 0; index < this.Children.Count; ++index)
      {
        if (this.Children[index].IsVisible)
          ++num;
        if (num == visiblePosition)
          break;
      }
      return index;
    }

    private bool IsOwnedVisibleChild(ViewElement element)
    {
      if (this.Children.Contains(element))
        return element.IsVisible;
      return false;
    }

    private void InitializeVisibleChildren()
    {
      this.visibleChildren = new ObservableCollection<ViewElement>();
      this.readOnlyVisibleChildren = new ImmutableObservableCollection<ViewElement>(this.visibleChildren);
      this.RebuildVisibleChildren();
    }

    protected void RebuildVisibleChildren()
    {
      if (this.visibleChildren == null)
      {
        this.InitializeVisibleChildren();
      }
      else
      {
        View activeView = ViewManager.Instance.ActiveView;
        using (ViewManager.Instance.DeferActiveViewChanges())
        {
          for (int index = this.visibleChildren.Count - 1; index >= 0; --index)
          {
            if (!this.IsOwnedVisibleChild(this.visibleChildren[index]))
              this.visibleChildren.RemoveAt(index);
          }
          int visibleChildIndex = 0;
          foreach (ViewElement child in (IEnumerable<ViewElement>) this.Children)
          {
            if (child.IsVisible)
            {
              if (visibleChildIndex < this.visibleChildren.Count)
                this.EnsureVisibleChildAtIndex(visibleChildIndex, child);
              else
                this.visibleChildren.Add(child);
              ++visibleChildIndex;
            }
          }
          ViewManager.Instance.ActiveView = activeView;
        }
        this.UpdateHasMultipleOnScreenViews();
      }
    }

    public virtual bool IsChildOnScreen(int childIndex)
    {
      if (childIndex < 0 || childIndex >= this.Children.Count)
        throw new ArgumentOutOfRangeException("childIndex");
      if (this.IsOnScreen)
        return this.Children[childIndex].IsVisible;
      return false;
    }

    protected virtual void UpdateHasMultipleOnScreenViews()
    {
      foreach (ViewGroup viewGroup in Enumerable.Where<ViewElement>((IEnumerable<ViewElement>) this.VisibleChildren, (Func<ViewElement, bool>) (e => e is ViewGroup)))
      {
        if (viewGroup.HasMultipleOnScreenViews)
        {
          this.HasMultipleOnScreenViews = true;
          return;
        }
      }
      this.HasMultipleOnScreenViews = false;
    }

    private static void OnHasMultipleOnScreenViewsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ViewGroup viewGroup = (ViewGroup) obj;
      if (viewGroup.Parent == null)
        return;
      viewGroup.Parent.UpdateHasMultipleOnScreenViews();
    }

    private void EnsureVisibleChildAtIndex(int visibleChildIndex, ViewElement child)
    {
      if (object.ReferenceEquals((object) this.visibleChildren[visibleChildIndex], (object) child))
        return;
      int oldIndex = this.visibleChildren.IndexOf(child);
      if (oldIndex < 0)
        this.visibleChildren.Insert(visibleChildIndex, child);
      else
        this.visibleChildren.Move(oldIndex, visibleChildIndex);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      this.OnChildVisibilityChanged();
      this.OnChildrenChanged(args);
    }

    protected internal virtual void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
    {
    }

    protected virtual void OnSelectedElementChanged()
    {
    }

    protected internal virtual void OnChildVisibilityChanged()
    {
      bool flag = false;
      foreach (ViewElement viewElement in (IEnumerable<ViewElement>) this.Children)
      {
        if (viewElement.IsVisible)
        {
          flag = true;
          break;
        }
      }
      this.IsVisible = flag;
      this.RebuildVisibleChildren();
    }

    public abstract bool IsChildAllowed(ViewElement element);

    protected override void OnIsVisibleChanged()
    {
      base.OnIsVisibleChanged();
      if (this.IsVisible)
        return;
      foreach (ViewElement viewElement in (IEnumerable<ViewElement>) this.Children)
        viewElement.IsVisible = false;
    }

    public override ViewElement Find(Predicate<ViewElement> predicate)
    {
      if (predicate == null)
        return (ViewElement) null;
      foreach (ViewElement viewElement1 in (IEnumerable<ViewElement>) this.Children)
      {
        ViewElement viewElement2 = viewElement1.Find(predicate);
        if (viewElement2 != null)
          return viewElement2;
      }
      return base.Find(predicate);
    }

    public override IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate)
    {
      if (predicate != null)
      {
        foreach (ViewElement viewElement1 in (IEnumerable<ViewElement>) this.Children)
        {
          foreach (ViewElement viewElement2 in viewElement1.FindAll(predicate))
            yield return viewElement2;
        }
        foreach (ViewElement viewElement in this.BaseFindAll(predicate))
          yield return viewElement;
      }
    }

    private IEnumerable<ViewElement> BaseFindAll(Predicate<ViewElement> predicate)
    {
      return base.FindAll(predicate);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, Children = {1}, VisibleChildren = {2}, DockedWidth = {3}, DockedHeight = {4}", (object) this.GetType().Name, (object) this.Children.Count, (object) this.VisibleChildren.Count, (object) this.DockedWidth, (object) this.DockedHeight);
    }

    private class ViewElementCollection : OwnershipCollection<ViewElement>
    {
      public ViewGroup Owner { get; private set; }

      public ViewElementCollection(ViewGroup owner)
      {
        this.Owner = owner;
      }

      protected override void InsertItem(int index, ViewElement item)
      {
        if (!this.Owner.IsChildAllowed(item))
          throw new InvalidOperationException("Invalid element type added to ViewGroup");
        this.InsertItem(index, item);
      }

      protected override void SetItem(int index, ViewElement item)
      {
        if (!this.Owner.IsChildAllowed(item))
          throw new InvalidOperationException("Invalid element type added to ViewGroup");
        this.SetItem(index, item);
      }

      protected override void RemoveItem(int index)
      {
        base.RemoveItem(index);
        if (this.Owner == null)
          return;
        DockOperations.TryCollapse((ViewElement) this.Owner);
      }

      protected override void ClearItems()
      {
        base.ClearItems();
        if (this.Owner == null)
          return;
        DockOperations.TryCollapse((ViewElement) this.Owner);
      }

      protected override void LoseOwnership(ViewElement element)
      {
        element.Parent = (ViewGroup) null;
      }

      protected override void TakeOwnership(ViewElement element)
      {
        element.Detach();
        element.Parent = this.Owner;
      }
    }
  }
}

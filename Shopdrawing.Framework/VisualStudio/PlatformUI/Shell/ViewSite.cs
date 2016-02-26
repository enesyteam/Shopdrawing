// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewSite
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public abstract class ViewSite : ViewGroup
  {
    public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof (ViewElement), typeof (ViewSite), new PropertyMetadata(new PropertyChangedCallback(ViewSite.OnChildChanged)));

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ViewElement Child
    {
      get
      {
        return (ViewElement) this.GetValue(ViewSite.ChildProperty);
      }
      set
      {
        this.SetValue(ViewSite.ChildProperty, (object) value);
      }
    }

    protected ViewSite()
    {
      this.Children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnChildrenChanged);
    }

    private static void OnChildChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ViewSite viewSite = (ViewSite) obj;
      ViewElement viewElement = args.NewValue as ViewElement;
      if (viewElement == null)
      {
        if (viewSite.Children.Count <= 0)
          return;
        viewSite.Children.Clear();
      }
      else if (viewSite.Children.Count == 0)
      {
        viewSite.Children.Add(viewElement);
      }
      else
      {
        if (viewSite.Children[0] == viewElement)
          return;
        viewSite.Children[0] = viewElement;
      }
    }

    private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (this.Children.Count > 1)
        throw new InvalidOperationException("ViewSite does not support multiple children");
      this.Child = this.Children.Count > 0 ? this.Children[0] : (ViewElement) null;
    }

    public override bool IsChildOnScreen(int childIndex)
    {
      if (childIndex < 0 || childIndex >= this.Children.Count)
        throw new ArgumentOutOfRangeException("childIndex");
      if (this.IsOnScreen && childIndex == 0)
        return this.Child.IsVisible;
      return false;
    }

    protected override bool GetIsOnScreenCore()
    {
      return this.IsVisible;
    }
  }
}

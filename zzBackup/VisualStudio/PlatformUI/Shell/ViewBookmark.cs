// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewBookmark
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class ViewBookmark : ViewElement
  {
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof (string), typeof (ViewBookmark));
    public static readonly DependencyProperty AccessOrderProperty = DependencyProperty.Register("AccessOrder", typeof (int), typeof (ViewBookmark), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
    public static readonly DependencyProperty ViewBookmarkTypeProperty = DependencyProperty.Register("ViewBookmarkType", typeof (ViewBookmarkType), typeof (ViewBookmark), (PropertyMetadata) new FrameworkPropertyMetadata((object) ViewBookmarkType.Default, new PropertyChangedCallback(ViewBookmark.OnViewBookmarkTypeChanged)));

    public string Name
    {
      get
      {
        return (string) this.GetValue(ViewBookmark.NameProperty);
      }
      set
      {
        this.SetValue(ViewBookmark.NameProperty, (object) value);
      }
    }

    [DefaultValue(0)]
    public int AccessOrder
    {
      get
      {
        return (int) this.GetValue(ViewBookmark.AccessOrderProperty);
      }
      set
      {
        this.SetValue(ViewBookmark.AccessOrderProperty, (object) value);
      }
    }

    [DefaultValue(ViewBookmarkType.Default)]
    public ViewBookmarkType ViewBookmarkType
    {
      get
      {
        return (ViewBookmarkType) this.GetValue(ViewBookmark.ViewBookmarkTypeProperty);
      }
      set
      {
        this.SetValue(ViewBookmark.ViewBookmarkTypeProperty, (object) value);
      }
    }

    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new ViewBookmarkCustomSerializer(this);
    }

    private static void OnViewBookmarkTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (ViewBookmarkType.All == (ViewBookmarkType) args.NewValue)
        throw new InvalidOperationException("All is not a valid type for a bookmark instance");
    }

    public static ViewBookmark Create()
    {
      return ViewElementFactory.Current.CreateViewBookmark();
    }

    public static ViewBookmark Create(string name, ViewBookmarkType type)
    {
      ViewBookmark viewBookmark = ViewBookmark.Create();
      viewBookmark.Name = name;
      viewBookmark.AccessOrder = 0;
      viewBookmark.ViewBookmarkType = type;
      return viewBookmark;
    }

    public static ViewBookmark Create(View markedView)
    {
      if (markedView == null)
        throw new ArgumentNullException("markedView");
      ViewBookmark viewBookmark = ViewBookmark.Create(markedView.Name, ExtensionMethods.GetBookmarkType((ViewElement) markedView));
      viewBookmark.DockedHeight = markedView.DockedHeight;
      viewBookmark.DockedWidth = markedView.DockedWidth;
      return viewBookmark;
    }
  }
}

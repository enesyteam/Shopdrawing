// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.View
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class View : ViewElement
  {
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof (object), typeof (View));
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof (object), typeof (View));
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof (string), typeof (View));
    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.RegisterAttached("TitleTemplate", typeof (DataTemplate), typeof (View), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty TabTitleTemplateProperty = DependencyProperty.RegisterAttached("TabTitleTemplate", typeof (DataTemplate), typeof (View), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty DocumentTabTitleTemplateProperty = DependencyProperty.RegisterAttached("DocumentTabTitleTemplate", typeof (DataTemplate), typeof (View), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof (bool), typeof (View), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(View.OnIsActiveChanged)));

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTemplate DocumentTabTitleTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(View.DocumentTabTitleTemplateProperty);
      }
      set
      {
        this.SetValue(View.DocumentTabTitleTemplateProperty, (object) value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTemplate TabTitleTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(View.TabTitleTemplateProperty);
      }
      set
      {
        this.SetValue(View.TabTitleTemplateProperty, (object) value);
      }
    }

    public DataTemplate TitleTemplate
    {
      get
      {
        return (DataTemplate) this.GetValue(View.TitleTemplateProperty);
      }
      set
      {
        this.SetValue(View.TitleTemplateProperty, (object) value);
      }
    }

    public string Name
    {
      get
      {
        return (string) this.GetValue(View.NameProperty);
      }
      set
      {
        this.SetValue(View.NameProperty, (object) value);
      }
    }

    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object Title
    {
      get
      {
        return this.GetValue(View.TitleProperty);
      }
      set
      {
        this.SetValue(View.TitleProperty, value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object Content
    {
      get
      {
        return this.GetValue(View.ContentProperty);
      }
      set
      {
        this.SetValue(View.ContentProperty, value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsActive
    {
        get
        {
            return (bool)base.GetValue(View.IsActiveProperty);
        }
        set
        {
            base.SetValue(View.IsActiveProperty, value);
        }
    }

    public event EventHandler Shown;

    public event CancelEventHandler Showing;

    public event EventHandler Hidden;

    public event CancelEventHandler Hiding;

    public override ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new ViewCustomSerializer(this);
    }

    public static DataTemplate GetTitleTemplate(DependencyObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      return (DataTemplate) obj.GetValue(View.TitleTemplateProperty);
    }

    public static void SetTitleTemplate(DependencyObject obj, DataTemplate value)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      obj.SetValue(View.TitleTemplateProperty, (object) value);
    }

    public static DataTemplate GetTabTitleTemplate(DependencyObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      return (DataTemplate) obj.GetValue(View.TabTitleTemplateProperty);
    }

    public static void SetTabTitleTemplate(DependencyObject obj, DataTemplate value)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      obj.SetValue(View.TabTitleTemplateProperty, (object) value);
    }

    public static DataTemplate GetDocumentTabTitleTemplate(DependencyObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      return (DataTemplate) obj.GetValue(View.DocumentTabTitleTemplateProperty);
    }

    public static void SetDocumentTabTitleTemplate(DependencyObject obj, DataTemplate value)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      obj.SetValue(View.DocumentTabTitleTemplateProperty, (object) value);
    }

    private static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as View).OnIsActiveChanged();
    }

    public bool Show()
    {
      using (LayoutSynchronizer.BeginLayoutSynchronization())
      {
        if (this.IsVisible)
          return true;
        if (!this.RaiseShowing())
          return false;
        this.IsVisible = true;
        Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Shown, (object) this);
        return true;
      }
    }

    public bool Hide()
    {
      if (!this.IsVisible)
        return true;
      if (!this.RaiseHiding())
        return false;
      this.IsVisible = false;
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Hidden, (object) this);
      return true;
    }

    public bool ShowInFront()
    {
      using (LayoutSynchronizer.BeginLayoutSynchronization())
      {
        if (!this.Show())
          return false;
        this.IsSelected = true;
        return true;
      }
    }

    protected bool RaiseShowing()
    {
      CancelEventArgs args = new CancelEventArgs(false);
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Showing, (object) this, args);
      return !args.Cancel;
    }

    protected bool RaiseHiding()
    {
      CancelEventArgs args = new CancelEventArgs(false);
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.Hiding, (object) this, args);
      return !args.Cancel;
    }

    protected void OnIsActiveChanged()
    {
      if (!this.IsActive)
        return;
      ViewManager.Instance.ActiveView = this;
    }

    protected override void ValidateDockedWidth(SplitterLength width)
    {
      if (width.IsFill)
        throw new ArgumentException("View does not accept Fill values for DockedWidth.");
    }

    protected override void ValidateDockedHeight(SplitterLength height)
    {
      if (height.IsFill)
        throw new ArgumentException("View does not accept Fill values for DockedHeight.");
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, Title = {1}, Name = {2}, DockedWidth = {3}, DockedHeight = {4}", (object) this.GetType().Name, (object) (this.Title == null ? "<null>" : this.Title.ToString()), (object) this.Name, (object) this.DockedWidth, (object) this.DockedHeight);
    }

    public static View Create()
    {
      return ViewElementFactory.Current.CreateView();
    }

    public static View Create(WindowProfile owningProfile, string name)
    {
      if (owningProfile == null)
        throw new ArgumentNullException("owningProfile");
      View view = View.Create();
      View.Initialize(view, owningProfile, name);
      return view;
    }

    public static View Create(WindowProfile owningProfile, string name, Type viewType)
    {
      if (owningProfile == null)
        throw new ArgumentNullException("owningProfile");
      View view = ViewElementFactory.Current.CreateView(viewType);
      View.Initialize(view, owningProfile, name);
      return view;
    }

    private static void Initialize(View view, WindowProfile owningProfile, string name)
    {
      view.Name = name;
      DockOperations.Float((ViewElement) view, owningProfile);
    }
  }
}

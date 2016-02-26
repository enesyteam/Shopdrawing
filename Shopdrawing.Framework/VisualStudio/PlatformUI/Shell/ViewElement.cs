// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.ViewElement
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public abstract class ViewElement : DependencyObject, ICustomXmlSerializable, IDependencyObjectCustomSerializerAccess
  {
    private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterReadOnly("Parent", typeof (ViewGroup), typeof (ViewElement), new PropertyMetadata((object) null, new PropertyChangedCallback(ViewElement.OnParentChanged)));
    public static readonly DependencyProperty ParentProperty = ViewElement.ParentPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof (bool), typeof (ViewElement), new PropertyMetadata((object) false, new PropertyChangedCallback(ViewElement.OnIsVisibleChanged)));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof (bool), typeof (ViewElement), new PropertyMetadata((object) false, new PropertyChangedCallback(ViewElement.OnIsSelectedChanged)));
    public static readonly DependencyProperty DockedWidthProperty = DependencyProperty.Register("DockedWidth", typeof (SplitterLength), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SplitterLength(200.0, SplitterUnitType.Stretch), new PropertyChangedCallback(ViewElement.OnDockedWidthChanged)));
    public static readonly DependencyProperty DockedHeightProperty = DependencyProperty.Register("DockedHeight", typeof (SplitterLength), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SplitterLength(200.0, SplitterUnitType.Stretch), new PropertyChangedCallback(ViewElement.OnDockedHeightChanged)));
    public static readonly DependencyProperty AutoHideWidthProperty = DependencyProperty.Register("AutoHideWidth", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(300.0));
    public static readonly DependencyProperty AutoHideHeightProperty = DependencyProperty.Register("AutoHideHeight", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(300.0));
    public static readonly DependencyProperty FloatingWidthProperty = DependencyProperty.Register("FloatingWidth", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(300.0, new PropertyChangedCallback(ViewElement.OnFloatingSizeChanged)));
    public static readonly DependencyProperty FloatingHeightProperty = DependencyProperty.Register("FloatingHeight", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(200.0, new PropertyChangedCallback(ViewElement.OnFloatingSizeChanged)));
    public static readonly DependencyProperty FloatingLeftProperty = DependencyProperty.Register("FloatingLeft", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata( double.NaN));
    public static readonly DependencyProperty FloatingTopProperty = DependencyProperty.Register("FloatingTop", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(double.NaN));
    internal static DependencyPropertyKey WindowProfilePropertyKey = DependencyProperty.RegisterReadOnly("WindowProfile", typeof (WindowProfile), typeof (ViewElement), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty WindowProfileProperty = ViewElement.WindowProfilePropertyKey.DependencyProperty;
    public static readonly DependencyProperty DockRestrictionProperty = DependencyProperty.Register("DockRestriction", typeof (DockRestrictionType), typeof (ViewElement), new PropertyMetadata(DockRestrictionType.None));
    public static readonly DependencyProperty AreDockTargetsEnabledProperty = DependencyProperty.Register("AreDockTargetsEnabled", typeof (bool), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(true));
    public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register("MinimumWidth", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(30.0));
    public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register("MinimumHeight", typeof (double), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(30.0));
    public static readonly DependencyProperty FloatingWindowStateProperty = DependencyProperty.Register("FloatingWindowState", typeof (WindowState), typeof (ViewElement), (PropertyMetadata) new FrameworkPropertyMetadata(WindowState.Normal, new PropertyChangedCallback(ViewElement.OnFloatingWindowStateChanged)));
    internal const double UnfloatedPosition = double.NaN;
    private const double AutoHideWidthDefaultValue = 300.0;
    private const double AutoHideHeightDefaultValue = 300.0;
    private const double FloatingWidthDefaultValue = 300.0;
    private const double FloatingHeightDefaultValue = 200.0;
    private const double FloatingLeftDefaultValue = double.NaN;
    private const double FloatingTopDefaultValue = double.NaN;
    private const double MinimumWidthDefaultValue = 30.0;
    private const double MinimumHeightDefaultValue = 30.0;
    private const WindowState FloatingWindowStateDefaultValue = WindowState.Normal;
    private const DockRestrictionType DockRestrictionDefaultValue = DockRestrictionType.None;
    private int preventCollapseReferences;

    [DefaultValue(false)]
    public bool IsSelected
    {
        get
        {
            return (bool)base.GetValue(ViewElement.IsSelectedProperty);
        }
        set
        {
            base.SetValue(ViewElement.IsSelectedProperty, value);
        }
    }

    [DefaultValue(false)]
    public bool IsVisible
    {
        get
        {
            return (bool)base.GetValue(ViewElement.IsVisibleProperty);
        }
        set
        {
            base.SetValue(ViewElement.IsVisibleProperty, value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ViewGroup Parent
    {
      get
      {
        return (ViewGroup) this.GetValue(ViewElement.ParentProperty);
      }
      internal set
      {
        this.SetValue(ViewElement.ParentPropertyKey, (object) value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WindowProfile WindowProfile
    {
      get
      {
        return WindowProfile.FindWindowProfile(this);
      }
    }

    [DefaultValue(typeof (SplitterLength), "200")]
    public SplitterLength DockedHeight
    {
      get
      {
        return (SplitterLength) this.GetValue(ViewElement.DockedHeightProperty);
      }
      set
      {
        this.SetValue(ViewElement.DockedHeightProperty, (object) value);
      }
    }

    [DefaultValue(typeof (SplitterLength), "200")]
    public SplitterLength DockedWidth
    {
      get
      {
        return (SplitterLength) this.GetValue(ViewElement.DockedWidthProperty);
      }
      set
      {
        this.SetValue(ViewElement.DockedWidthProperty, (object) value);
      }
    }

    [DefaultValue(300.0)]
    public double AutoHideWidth
    {
      get
      {
        return (double) this.GetValue(ViewElement.AutoHideWidthProperty);
      }
      set
      {
        this.SetValue(ViewElement.AutoHideWidthProperty, (object) value);
      }
    }

    [DefaultValue(300.0)]
    public double AutoHideHeight
    {
      get
      {
        return (double) this.GetValue(ViewElement.AutoHideHeightProperty);
      }
      set
      {
        this.SetValue(ViewElement.AutoHideHeightProperty, (object) value);
      }
    }

    [DefaultValue(double.NaN)]
    public double FloatingTop
    {
      get
      {
        return (double) this.GetValue(ViewElement.FloatingTopProperty);
      }
      set
      {
        this.SetValue(ViewElement.FloatingTopProperty, (object) value);
      }
    }

    [DefaultValue(double.NaN)]
    public double FloatingLeft
    {
      get
      {
        return (double) this.GetValue(ViewElement.FloatingLeftProperty);
      }
      set
      {
        this.SetValue(ViewElement.FloatingLeftProperty, (object) value);
      }
    }

    [DefaultValue(200.0)]
    public double FloatingHeight
    {
      get
      {
        return (double) this.GetValue(ViewElement.FloatingHeightProperty);
      }
      set
      {
        this.SetValue(ViewElement.FloatingHeightProperty, (object) value);
      }
    }

    [DefaultValue(300.0)]
    public double FloatingWidth
    {
      get
      {
        return (double) this.GetValue(ViewElement.FloatingWidthProperty);
      }
      set
      {
        this.SetValue(ViewElement.FloatingWidthProperty, (object) value);
      }
    }

    [DefaultValue(WindowState.Normal)]
    public WindowState FloatingWindowState
    {
      get
      {
        return (WindowState) this.GetValue(ViewElement.FloatingWindowStateProperty);
      }
      set
      {
        this.SetValue(ViewElement.FloatingWindowStateProperty, (object) value);
      }
    }

    [DefaultValue(DockRestrictionType.None)]
    public DockRestrictionType DockRestriction
    {
      get
      {
        return (DockRestrictionType) this.GetValue(ViewElement.DockRestrictionProperty);
      }
      set
      {
        this.SetValue(ViewElement.DockRestrictionProperty, (object) value);
      }
    }

    [DefaultValue(true)]
    public bool AreDockTargetsEnabled
    {
        get
        {
            return (bool)base.GetValue(ViewElement.AreDockTargetsEnabledProperty);
        }
        set
        {
            base.SetValue(ViewElement.AreDockTargetsEnabledProperty, value);
        }
    }

    [DefaultValue(30.0)]
    public double MinimumWidth
    {
      get
      {
        return (double) this.GetValue(ViewElement.MinimumWidthProperty);
      }
      set
      {
        this.SetValue(ViewElement.MinimumWidthProperty, (object) value);
      }
    }

    [DefaultValue(30.0)]
    public double MinimumHeight
    {
      get
      {
        return (double) this.GetValue(ViewElement.MinimumHeightProperty);
      }
      set
      {
        this.SetValue(ViewElement.MinimumHeightProperty, (object) value);
      }
    }

    public bool IsCollapsible
    {
      get
      {
        return this.preventCollapseReferences == 0;
      }
    }

    public bool IsOnScreen
    {
      get
      {
        return this.GetIsOnScreenCore();
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool IsFloatingSizeExplicitlySet { get; private set; }

    public event EventHandler IsVisibleChanged;

    public event EventHandler IsSelectedChanged;

    public event EventHandler ParentChanged;

    protected ViewElement()
    {
      if (!ViewElementFactory.Current.IsConstructionAllowed)
        throw new InvalidOperationException("ViewElements cannot be constructed except through factory methods.  Please use the static Create method on the ViewElement, or the ViewElementFactory directly.");
    }

    public virtual ICustomXmlSerializer CreateSerializer()
    {
      return (ICustomXmlSerializer) new ViewElementCustomSerializer(this);
    }

    bool IDependencyObjectCustomSerializerAccess.ShouldSerializeProperty(DependencyProperty dp)
    {
      return this.ShouldSerializeProperty(dp);
    }

    object IDependencyObjectCustomSerializerAccess.GetValue(DependencyProperty dp)
    {
      return this.GetValue(dp);
    }

    private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((ViewElement) obj).OnIsVisibleChanged();
    }

    private static void OnParentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((ViewElement) obj).OnParentChanged((ViewGroup) args.OldValue);
    }

    private static void OnIsSelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((ViewElement) obj).OnIsSelectedChanged();
    }

    protected virtual void OnIsVisibleChanged()
    {
      if (this.Parent != null)
        this.Parent.OnChildVisibilityChanged();
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.IsVisibleChanged, (object) this);
    }

    protected virtual void OnParentChanged(ViewGroup oldParent)
    {
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.ParentChanged, (object) this);
      if (oldParent != null && oldParent.SelectedElement == this)
        oldParent.SelectedElement = (ViewElement) null;
      this.UpdateParentSelectedElement();
    }

    protected virtual void OnIsSelectedChanged()
    {
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.RaiseEvent(this.IsSelectedChanged, (object) this);
      this.UpdateParentSelectedElement();
    }

    private void UpdateParentSelectedElement()
    {
      if (this.Parent == null)
        return;
      if (this.IsSelected)
      {
        this.Parent.SelectedElement = this;
      }
      else
      {
        if (this.Parent.SelectedElement != this)
          return;
        this.Parent.SelectedElement = (ViewElement) null;
      }
    }

    private void AddRefCollapseScope()
    {
      ++this.preventCollapseReferences;
    }

    private void ReleaseCollapseScope()
    {
      --this.preventCollapseReferences;
      if (this.preventCollapseReferences != 0)
        return;
      DockOperations.TryCollapse(this);
    }

    public IDisposable PreventCollapse()
    {
      return (IDisposable) new ViewElement.PreventCollapseScope(this);
    }

    public virtual ViewElement Find(Predicate<ViewElement> predicate)
    {
      if (predicate == null)
        return (ViewElement) null;
      if (predicate(this))
        return this;
      return (ViewElement) null;
    }

    public virtual IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate)
    {
      if (predicate != null && predicate(this))
        yield return this;
    }

    public void Detach()
    {
      if (this.Parent == null)
        return;
      this.Parent.Children.Remove(this);
    }

    protected virtual void OnDockedWidthChanged()
    {
    }

    protected virtual void OnDockedHeightChanged()
    {
    }

    protected virtual void ValidateDockedWidth(SplitterLength width)
    {
    }

    protected virtual bool GetIsOnScreenCore()
    {
      if (this.Parent != null)
        return this.Parent.IsChildOnScreen(this.Parent.Children.IndexOf(this));
      return false;
    }

    protected virtual void ValidateDockedHeight(SplitterLength height)
    {
    }

    private static void OnDockedWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ViewElement viewElement = (ViewElement) obj;
      viewElement.ValidateDockedWidth((SplitterLength) args.NewValue);
      viewElement.OnDockedWidthChanged();
    }

    private static void OnDockedHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ViewElement viewElement = (ViewElement) obj;
      viewElement.ValidateDockedHeight((SplitterLength) args.NewValue);
      viewElement.OnDockedHeightChanged();
    }

    private static void OnFloatingSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      double num = (double) args.NewValue;
      if (num < 0.0)
        throw new ArgumentOutOfRangeException("args", (object) num, "FloatingWidth and FloatingHeight must be non-negative.");
      ((ViewElement) obj).IsFloatingSizeExplicitlySet = true;
    }

    private static void OnFloatingWindowStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      switch ((WindowState) args.NewValue)
      {
        case WindowState.Normal:
          break;
        case WindowState.Maximized:
          break;
        default:
          throw new ArgumentException("FloatingWindowState can only be Normal or Maximized", "args");
      }
    }

    public static ViewElement FindRootElement(ViewElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      while (element.Parent != null)
        element = (ViewElement) element.Parent;
      return element;
    }

    private class PreventCollapseScope : IDisposable
    {
      private ViewElement Element { get; set; }

      private bool IsDisposed { get; set; }

      public PreventCollapseScope(ViewElement element)
      {
        this.Element = element;
        this.Element.AddRefCollapseScope();
      }

      public void Dispose()
      {
        if (this.IsDisposed)
          return;
        this.Element.ReleaseCollapseScope();
        this.IsDisposed = true;
      }
    }
  }
}

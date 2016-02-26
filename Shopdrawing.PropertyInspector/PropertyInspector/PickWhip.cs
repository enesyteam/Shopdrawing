// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PickWhip
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PickWhip : Control
  {
    public static readonly DependencyProperty ParentEditorProperty = DependencyProperty.Register("ParentEditor", typeof (FrameworkElement), typeof (PickWhip), new PropertyMetadata(null, new PropertyChangedCallback(PickWhip.ParentEditorChangedCallback)));
    private PickWhipEngine engine;

    public IPickWhipHost PickWhipHost
    {
      get
      {
        return (IPickWhipHost) this.ParentEditor;
      }
    }

    public FrameworkElement ParentEditor
    {
      get
      {
        return (FrameworkElement) this.GetValue(PickWhip.ParentEditorProperty);
      }
      set
      {
        this.SetValue(PickWhip.ParentEditorProperty, value);
      }
    }

    static PickWhip()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (PickWhip), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (PickWhip)));
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (this.engine == null)
          this.engine = new PickWhipEngine(this.PickWhipHost);
        if (!this.engine.IsActive)
        {
          this.engine.BeginEditing();
          e.Handled = true;
        }
      }
      base.OnMouseDown(e);
    }

    private static void ParentEditorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
      PickWhip pickWhip = d as PickWhip;
      if (pickWhip == null || pickWhip.engine == null)
        return;
      if (pickWhip.engine.IsActive)
        pickWhip.engine.CancelEditing();
      pickWhip.engine = (PickWhipEngine) null;
    }
  }
}

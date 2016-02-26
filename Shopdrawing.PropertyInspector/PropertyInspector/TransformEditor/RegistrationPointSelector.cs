// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.RegistrationPointSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class RegistrationPointSelector : Control, IComponentConnector
  {
    public static readonly DependencyProperty RegistrationPointProperty = DependencyProperty.Register("RegistrationPoint", typeof (RegistrationPointFlags), typeof (RegistrationPointSelector), (PropertyMetadata) new FrameworkPropertyMetadata((object) RegistrationPointFlags.None));
    private bool _contentLoaded;

    public RegistrationPointFlags RegistrationPoint
    {
      get
      {
        return (RegistrationPointFlags) this.GetValue(RegistrationPointSelector.RegistrationPointProperty);
      }
      set
      {
        this.SetValue(RegistrationPointSelector.RegistrationPointProperty, (object) value);
      }
    }

    public RegistrationPointSelector()
    {
      this.InitializeComponent();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
    {
      if (this.IsEnabled)
      {
        Point position = args.MouseDevice.GetPosition((IInputElement) this);
        RegistrationPointFlags registrationPointFlags = RegistrationPointFlags.Center;
        double actualWidth = this.ActualWidth;
        double actualHeight = this.ActualHeight;
        if (position.X < actualWidth / 3.0)
          registrationPointFlags |= RegistrationPointFlags.Left;
        if (position.X > actualWidth * (2.0 / 3.0))
          registrationPointFlags |= RegistrationPointFlags.Right;
        if (position.Y < actualHeight / 3.0)
          registrationPointFlags |= RegistrationPointFlags.Top;
        if (position.Y > actualHeight * (2.0 / 3.0))
          registrationPointFlags |= RegistrationPointFlags.Bottom;
        if (registrationPointFlags != this.RegistrationPoint)
          this.RegistrationPoint = registrationPointFlags;
      }
      args.Handled = true;
      base.OnMouseLeftButtonDown(args);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/utilities/registrationpointselector.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}

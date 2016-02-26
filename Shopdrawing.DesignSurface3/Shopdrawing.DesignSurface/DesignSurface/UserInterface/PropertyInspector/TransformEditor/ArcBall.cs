// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ArcBall
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ArcBall : Control, IComponentConnector
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof (Rotation3D), typeof (ArcBall));
    public static readonly DependencyProperty BeginUpdateCommandProperty = DependencyProperty.Register("BeginUpdateCommand", typeof (ICommand), typeof (ArcBall), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty EndUpdateCommandProperty = DependencyProperty.Register("EndUpdateCommand", typeof (ICommand), typeof (ArcBall), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ContinueUpdateCommandProperty = DependencyProperty.Register("ContinueUpdateCommand", typeof (ICommand), typeof (ArcBall), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    private Vector3D initialPosition = new Vector3D();
    private Quaternion initialOrientation = Quaternion.Identity;
    private bool isCurrentlyUpdating;
    private bool _contentLoaded;

    public Rotation3D Orientation
    {
      get
      {
        return (Rotation3D) this.GetValue(ArcBall.OrientationProperty);
      }
      set
      {
        this.SetValue(ArcBall.OrientationProperty, (object) value);
      }
    }

    public ICommand BeginUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(ArcBall.BeginUpdateCommandProperty);
      }
      set
      {
        this.SetValue(ArcBall.BeginUpdateCommandProperty, (object) value);
      }
    }

    public ICommand EndUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(ArcBall.EndUpdateCommandProperty);
      }
      set
      {
        this.SetValue(ArcBall.EndUpdateCommandProperty, (object) value);
      }
    }

    public ICommand ContinueUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(ArcBall.ContinueUpdateCommandProperty);
      }
      set
      {
        this.SetValue(ArcBall.ContinueUpdateCommandProperty, (object) value);
      }
    }

    public ArcBall()
    {
      this.InitializeComponent();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (!this.isCurrentlyUpdating && ((Mouse.GetPosition((IInputElement) this) - new Point(this.RenderSize.Width / 2.0, this.RenderSize.Height / 2.0)).Length <= this.RenderSize.Width / 2.0 && this.BeginUpdate()))
      {
        this.SetOrientationFromPointer();
        e.MouseDevice.Capture((IInputElement) this);
        e.Handled = true;
      }
      base.OnMouseLeftButtonDown(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      if (this.isCurrentlyUpdating && !e.MouseDevice.Capture((IInputElement) null))
      {
        this.SetOrientationFromPointer();
        this.EndUpdate();
        e.Handled = true;
      }
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (this.isCurrentlyUpdating)
      {
        this.ContinueUpdate();
        this.SetOrientationFromPointer();
        e.Handled = true;
      }
      base.OnMouseMove(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      if (this.isCurrentlyUpdating)
      {
        this.SetOrientationFromPointer();
        this.EndUpdate();
        e.Handled = true;
      }
      base.OnLostMouseCapture(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      this.OnKeyChange(e);
      base.OnKeyUp(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      this.OnKeyChange(e);
      base.OnKeyDown(e);
    }

    private void OnKeyChange(KeyEventArgs args)
    {
      if (!this.isCurrentlyUpdating || args.Key != Key.LeftShift && args.Key != Key.RightShift || Mouse.LeftButton != MouseButtonState.Pressed)
        return;
      this.SetOrientationFromPointer();
    }

    private Vector3D GetPositionFromPointer()
    {
      Vector vector = (Mouse.GetPosition((IInputElement) this) - new Point(this.RenderSize.Width / 2.0, this.RenderSize.Height / 2.0)) * (1.0 / (this.RenderSize.Width * 3.0 / 2.0));
      if (vector.Length <= 1.0)
        return new Vector3D(vector.X, -vector.Y, Math.Sqrt(1.0 - vector.LengthSquared));
      vector.Normalize();
      return new Vector3D(vector.X, -vector.Y, 0.0);
    }

    private void SetOrientationFromPointer()
    {
      Vector3D positionFromPointer = this.GetPositionFromPointer();
      Vector3D axisOfRotation = Vector3D.CrossProduct(this.initialPosition, positionFromPointer);
      if (!this.IsEnabled || Tolerances.NearZero(axisOfRotation.Length))
        return;
      axisOfRotation.Normalize();
      Quaternion quaternion = new Quaternion(axisOfRotation, Math.Acos(Vector3D.DotProduct(this.initialPosition, positionFromPointer)) * 57.2957795130823) * this.initialOrientation;
      this.Orientation = (Rotation3D) new AxisAngleRotation3D(quaternion.Axis, quaternion.Angle);
    }

    private bool BeginUpdate()
    {
      ICommand beginUpdateCommand = this.BeginUpdateCommand;
      Quaternion quaternion = Helper3D.QuaternionFromRotation3D(this.Orientation);
      if (beginUpdateCommand != null)
      {
        if (this.Execute(beginUpdateCommand))
        {
          this.initialPosition = this.GetPositionFromPointer();
          this.initialOrientation = quaternion;
          this.isCurrentlyUpdating = true;
        }
      }
      else
      {
        this.initialPosition = this.GetPositionFromPointer();
        this.initialOrientation = quaternion;
        this.isCurrentlyUpdating = true;
      }
      return this.isCurrentlyUpdating;
    }

    private void ContinueUpdate()
    {
      this.Execute(this.ContinueUpdateCommand);
    }

    private bool EndUpdate()
    {
      this.Execute(this.EndUpdateCommand);
      this.isCurrentlyUpdating = false;
      return true;
    }

    private bool Execute(ICommand command)
    {
      return ValueEditorUtils.ExecuteCommand(command, (IInputElement) this, (object) null);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/utilities/arcball.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}

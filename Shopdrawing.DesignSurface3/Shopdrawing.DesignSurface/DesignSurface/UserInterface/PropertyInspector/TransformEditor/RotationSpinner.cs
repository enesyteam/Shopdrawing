// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.RotationSpinner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
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

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class RotationSpinner : Control, IComponentConnector
  {
    public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof (double), typeof (RotationSpinner), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty BeginUpdateCommandProperty = DependencyProperty.Register("BeginUpdateCommand", typeof (ICommand), typeof (RotationSpinner), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty EndUpdateCommandProperty = DependencyProperty.Register("EndUpdateCommand", typeof (ICommand), typeof (RotationSpinner), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ContinueUpdateCommandProperty = DependencyProperty.Register("ContinueUpdateCommand", typeof (ICommand), typeof (RotationSpinner), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    private bool isCurrentlyUpdating;
    private bool _contentLoaded;

    public double RotationAngle
    {
      get
      {
        return (double) this.GetValue(RotationSpinner.RotationAngleProperty);
      }
      set
      {
        this.SetValue(RotationSpinner.RotationAngleProperty, (object) value);
      }
    }

    public ICommand BeginUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(RotationSpinner.BeginUpdateCommandProperty);
      }
      set
      {
        this.SetValue(RotationSpinner.BeginUpdateCommandProperty, (object) value);
      }
    }

    public ICommand EndUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(RotationSpinner.EndUpdateCommandProperty);
      }
      set
      {
        this.SetValue(RotationSpinner.EndUpdateCommandProperty, (object) value);
      }
    }

    public ICommand ContinueUpdateCommand
    {
      get
      {
        return (ICommand) this.GetValue(RotationSpinner.ContinueUpdateCommandProperty);
      }
      set
      {
        this.SetValue(RotationSpinner.ContinueUpdateCommandProperty, (object) value);
      }
    }

    public RotationSpinner()
    {
      this.InitializeComponent();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (!this.isCurrentlyUpdating && ((Mouse.GetPosition((IInputElement) this) - new Point(this.RenderSize.Width / 2.0, this.RenderSize.Height / 2.0)).Length <= this.RenderSize.Width / 2.0 && this.BeginUpdate()))
      {
        this.SetRotationFromPointer();
        this.Focus();
        e.MouseDevice.Capture((IInputElement) this);
        e.Handled = true;
      }
      base.OnMouseLeftButtonDown(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      if (this.isCurrentlyUpdating)
      {
        bool flag = e.MouseDevice.Capture((IInputElement) null);
        e.Handled = true;
        if (!flag)
        {
          this.SetRotationFromPointer();
          this.EndUpdate();
        }
      }
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (this.isCurrentlyUpdating)
      {
        this.ContinueUpdate();
        this.SetRotationFromPointer();
        e.Handled = true;
      }
      base.OnMouseMove(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      if (this.isCurrentlyUpdating)
      {
        this.SetRotationFromPointer();
        this.EndUpdate();
      }
      base.OnLostMouseCapture(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      this.OnKeyChange(e);
      base.OnKeyUp(e);
      if (!this.isCurrentlyUpdating)
        return;
      e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      this.OnKeyChange(e);
      base.OnKeyUp(e);
      if (!this.isCurrentlyUpdating)
        return;
      e.Handled = true;
    }

    private void OnKeyChange(KeyEventArgs args)
    {
      if (!this.isCurrentlyUpdating || args.Key != Key.LeftShift && args.Key != Key.RightShift || Mouse.LeftButton != MouseButtonState.Pressed)
        return;
      this.SetRotationFromPointer();
    }

    private double ClosestAngleTo(double angle)
    {
      double rotationAngle = this.RotationAngle;
      double num = (angle - rotationAngle) % 360.0;
      if (num > 180.0)
        num -= 360.0;
      if (num < -180.0)
        num += 360.0;
      return rotationAngle + num;
    }

    private void SetRotationFromPointer()
    {
      Vector vector = Mouse.GetPosition((IInputElement) this) - new Point(this.RenderSize.Width / 2.0, this.RenderSize.Height / 2.0);
      if (Tolerances.NearZero(vector) || !this.IsEnabled)
        return;
      double num = Math.Atan2(vector.Y, vector.X) * 180.0 / Math.PI;
      this.RotationAngle = this.ClosestAngleTo(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? Math.Round(num / 15.0) * 15.0 : RoundingHelper.RoundAngle(num));
    }

    private bool BeginUpdate()
    {
      ICommand beginUpdateCommand = this.BeginUpdateCommand;
      if (beginUpdateCommand != null)
      {
        if (this.Execute(beginUpdateCommand))
          this.isCurrentlyUpdating = true;
      }
      else
        this.isCurrentlyUpdating = true;
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
      return this.isCurrentlyUpdating;
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/utilities/rotationspinner.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}

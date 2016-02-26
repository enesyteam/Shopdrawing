// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingEditorPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class EasingEditorPopup : WorkaroundPopup, IComponentConnector
  {
    private SceneView returnView;
    internal EasingEditorPopup EasingEditorPopupControl;
    private bool _contentLoaded;

    public SceneNodeProperty EasingFunctionProperty { get; private set; }

    public EasingEditorPopup(SceneNodeProperty easingFunctionProperty, SceneView returnView)
    {
      this.EasingFunctionProperty = easingFunctionProperty;
      this.returnView = returnView;
      this.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.get_FinishEditing(), new ExecutedRoutedEventHandler(this.OnPropertyValueFinishEditingCommand)));
      this.InitializeComponent();
    }

    private void OnPropertyValueFinishEditingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      Keyboard.Focus((IInputElement) null);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      if (this.returnView == null || this.returnView.ViewModel == null)
        return;
      this.returnView.ReturnFocus();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/skinediting/easingeditorpopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.EasingEditorPopupControl = (EasingEditorPopup) target;
      else
        this._contentLoaded = true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.GradientStopColorPopup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class GradientStopColorPopup : WorkaroundPopup, IComponentConnector
  {
    internal GradientStopColorPopup GradientStopColorPopupControl;
    private bool _contentLoaded;

    public PropertyReferenceProperty ColorProperty { get; private set; }

    private DesignerContext DesignerContext { get; set; }

    public GradientStopColorPopup(PropertyReferenceProperty colorProperty, DesignerContext designerContext)
    {
      this.ColorProperty = colorProperty;
      this.DesignerContext = designerContext;
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
      this.DesignerContext.ActiveView.ReturnFocus();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/brusheditor/gradientstopcolorpopup.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.GradientStopColorPopupControl = (GradientStopColorPopup) target;
      else
        this._contentLoaded = true;
    }
  }
}

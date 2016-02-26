// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ElementBindingPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class ElementBindingPickerEditor : Grid, IComponentConnector
  {
    public static readonly DependencyProperty TypeConstraintProperty = DependencyProperty.Register("TypeConstraint", typeof (ITypeId), typeof (ElementBindingPickerEditor));
    internal ElementBindingPickerEditor ElementBindingPickerEditorControl;
    internal ElementPicker ElementPicker;
    private bool _contentLoaded;

    public ITypeId TypeConstraint
    {
      get
      {
        return (ITypeId) this.GetValue(ElementBindingPickerEditor.TypeConstraintProperty);
      }
      set
      {
        this.SetValue(ElementBindingPickerEditor.TypeConstraintProperty, value);
      }
    }

    public ElementBindingPickerEditor()
    {
      this.InitializeComponent();
    }

    private void InvokeDataBindDialog(object o, EventArgs e)
    {
      this.ElementPicker.EditingProperty.DoSetDataBinding();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/elementbindingpickereditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ElementBindingPickerEditorControl = (ElementBindingPickerEditor) target;
          break;
        case 2:
          this.ElementPicker = (ElementPicker) target;
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.InvokeDataBindDialog);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

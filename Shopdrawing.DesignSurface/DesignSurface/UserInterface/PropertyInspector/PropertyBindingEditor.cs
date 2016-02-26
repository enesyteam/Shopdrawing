// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyBindingEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class PropertyBindingEditor : ComplexValueEditorBase, IPickWhipHost, IComponentConnector
  {
    private IElementSelectionStrategy selectionStrategy;
    internal PropertyBindingEditor ElementPickerControl;
    private bool _contentLoaded;

    public FrameworkElement PropertyEditor
    {
      get
      {
        return (FrameworkElement) this;
      }
    }

    public IElementSelectionStrategy ElementSelectionStrategy
    {
      get
      {
        if (this.selectionStrategy == null)
          this.selectionStrategy = (IElementSelectionStrategy) new CreatePropertyBindingPickWhipStrategy(this.EditingProperty.Reference.LastStep);
        return this.selectionStrategy;
      }
    }

    public new SceneNodeProperty EditingProperty
    {
      get
      {
        return base.EditingProperty;
      }
    }

    public Cursor PickWhipCursor
    {
      get
      {
        return ToolCursors.PropertyPickWhipCursor;
      }
    }

    public string BindingToolTip
    {
      get
      {
        return BindingPropertyHelper.GetElementNameFromBoundProperty(this.EditingProperty);
      }
    }

    public PropertyBindingEditor()
    {
      this.InitializeComponent();
    }

    protected override void Rebuild()
    {
      base.Rebuild();
      this.SuppressValueAreaWrapper();
      this.OnPropertyChanged("BindingToolTip");
    }

    private void InvokeDataBindDialog(object sender, EventArgs e)
    {
      if (this.EditingProperty == null)
        return;
      this.EditingProperty.DoSetDataBinding();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/propertybindingeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ElementPickerControl = (PropertyBindingEditor) target;
          break;
        case 2:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.InvokeDataBindDialog);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

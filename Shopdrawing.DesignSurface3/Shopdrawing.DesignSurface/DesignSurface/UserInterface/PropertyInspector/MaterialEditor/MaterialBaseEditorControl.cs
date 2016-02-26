// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor.MaterialBaseEditorControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class MaterialBaseEditorControl : StackPanel, IComponentConnector
  {
    internal MaterialBaseEditorControl MaterialEditorControl;
    internal PropertyMarker Marker;
    internal PropertyMarker ColorMarker;
    internal PropertyMarker Marker3;
    private bool _contentLoaded;

    public MaterialBaseEditorControl()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/materialeditor/materialbaseeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.MaterialEditorControl = (MaterialBaseEditorControl) target;
          break;
        case 2:
          this.Marker = (PropertyMarker) target;
          break;
        case 3:
          this.ColorMarker = (PropertyMarker) target;
          break;
        case 4:
          this.Marker3 = (PropertyMarker) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

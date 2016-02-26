// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ViewOptionsControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.View
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ViewOptionsControl : Grid, IComponentConnector
  {
    private static List<ViewModeWrapper> viewModes = new List<ViewModeWrapper>(3);
    internal ViewOptionsControl viewOptionsBorder;
    internal ComboBox DefaultViewModeCombo;
    private bool _contentLoaded;

    public static IEnumerable<ViewModeWrapper> ViewModes
    {
      get
      {
        return (IEnumerable<ViewModeWrapper>) ViewOptionsControl.viewModes;
      }
    }

    static ViewOptionsControl()
    {
      ViewOptionsControl.viewModes.Add(new ViewModeWrapper(ViewMode.Design, StringTable.ViewOptionsDesignMode));
      ViewOptionsControl.viewModes.Add(new ViewModeWrapper(ViewMode.Split, StringTable.ViewOptionsSplitViewMode));
      ViewOptionsControl.viewModes.Add(new ViewModeWrapper(ViewMode.Code, StringTable.ViewOptionsXamlMode));
    }

    public ViewOptionsControl(ViewOptionsModel viewOptionsModel)
    {
      this.DataContext = (object) viewOptionsModel;
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/view/viewoptionscontrol.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.viewOptionsBorder = (ViewOptionsControl) target;
          break;
        case 2:
          this.DefaultViewModeCombo = (ComboBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

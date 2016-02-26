// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UnitsOptionsControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class UnitsOptionsControl : Grid, IComponentConnector
  {
    internal UnitsOptionsControl unitsOptionsBorder;
    internal ComboBox DefaultUnitsCombo;
    private bool _contentLoaded;

    public static Array Units
    {
      get
      {
        return Enum.GetValues(typeof (UnitType));
      }
    }

    public UnitsOptionsControl(UnitsOptionsModel unitsOptionsModel)
    {
      this.DataContext = (object) unitsOptionsModel;
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/unitsoptionscontrol.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.unitsOptionsBorder = (UnitsOptionsControl) target;
          break;
        case 2:
          this.DefaultUnitsCombo = (ComboBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

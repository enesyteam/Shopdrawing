// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ToolSeparator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ToolSeparator : Border, IComponentConnector
  {
    internal Rectangle Line;
    private bool _contentLoaded;

    public ToolSeparator()
    {
      this.InitializeComponent();
    }

    protected override Size MeasureOverride(Size constraint)
    {
      double width = double.IsNaN(this.Line.Width) ? 0.0 : this.Line.Width;
      double height = double.IsNaN(this.Line.Height) ? 0.0 : this.Line.Height;
      this.Line.Measure(new Size(width, height));
      return new Size(width + 4.0, height + 4.0);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/toolpane/toolseparator.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.Line = (Rectangle) target;
      else
        this._contentLoaded = true;
    }
  }
}

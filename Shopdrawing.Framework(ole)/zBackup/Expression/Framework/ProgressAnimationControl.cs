// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ProgressAnimationControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Microsoft.Expression.Framework
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class ProgressAnimationControl : UserControl, IComponentConnector
  {
    internal Canvas CircleProgress;
    internal Rectangle circle_point1;
    internal Rectangle circle_point2;
    internal Rectangle circle_point3;
    internal Rectangle circle_point4;
    internal Rectangle circle_point5;
    internal Rectangle circle_point6;
    internal Rectangle circle_point7;
    internal Rectangle circle_point8;
    private bool _contentLoaded;

    public ProgressAnimationControl()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/licensing/progressanimationcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.CircleProgress = (Canvas) target;
          break;
        case 2:
          this.circle_point1 = (Rectangle) target;
          break;
        case 3:
          this.circle_point2 = (Rectangle) target;
          break;
        case 4:
          this.circle_point3 = (Rectangle) target;
          break;
        case 5:
          this.circle_point4 = (Rectangle) target;
          break;
        case 6:
          this.circle_point5 = (Rectangle) target;
          break;
        case 7:
          this.circle_point6 = (Rectangle) target;
          break;
        case 8:
          this.circle_point7 = (Rectangle) target;
          break;
        case 9:
          this.circle_point8 = (Rectangle) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationsOptionsControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class AnnotationsOptionsControl : StackPanel, IComponentConnector
  {
    internal StringEditor AuthorName;
    internal StringEditor AuthorInitials;
    private bool _contentLoaded;

    public AnnotationsOptionsControl(AnnotationsOptionsModel options)
    {
      this.DataContext = (object) options;
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/annotations/options/annotationsoptionscontrol.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.AuthorName = (StringEditor) target;
          break;
        case 2:
          this.AuthorInitials = (StringEditor) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RecordBar
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.View
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class RecordBar : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof (bool), typeof (RecordBar));
    public static readonly DependencyProperty IsRecordingProperty = DependencyProperty.Register("IsRecording", typeof (bool), typeof (RecordBar));
    public static readonly DependencyProperty RecordTargetDescriptionProperty = DependencyProperty.Register("RecordTargetDescription", typeof (RecordTargetDescriptionBase), typeof (RecordBar));
    public static readonly DependencyProperty DefaultCommandProperty = DependencyProperty.Register("DefaultCommand", typeof (ICommand), typeof (RecordBar));
    private bool _contentLoaded;

    public bool IsOpen
    {
      get
      {
        return (bool) this.GetValue(RecordBar.IsOpenProperty);
      }
      set
      {
        this.SetValue(RecordBar.IsOpenProperty, (object) (bool) (value ? true : false));
      }
    }

    public bool IsRecording
    {
      get
      {
        return (bool) this.GetValue(RecordBar.IsRecordingProperty);
      }
      set
      {
        this.SetValue(RecordBar.IsRecordingProperty, (object) (bool) (value ? true : false));
      }
    }

    public RecordTargetDescriptionBase RecordTargetDescription
    {
      get
      {
        return (RecordTargetDescriptionBase) this.GetValue(RecordBar.RecordTargetDescriptionProperty);
      }
      set
      {
        this.SetValue(RecordBar.RecordTargetDescriptionProperty, (object) value);
      }
    }

    public ICommand DefaultCommand
    {
      get
      {
        return (ICommand) this.GetValue(RecordBar.DefaultCommandProperty);
      }
      set
      {
        this.SetValue(RecordBar.DefaultCommandProperty, (object) value);
      }
    }

    public RecordBar()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/recordbar.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}

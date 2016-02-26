// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ViewServiceDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ViewServiceDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private IView activeView;
    private IViewService viewService;
    internal ListBox ViewList;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public IView ActiveView
    {
      get
      {
        if (this.activeView == null)
          return this.viewService.ActiveView;
        return this.activeView;
      }
      set
      {
        this.activeView = value;
        this.OnPropertyChanged("ActiveView");
      }
    }

    public IEnumerable<IView> Views
    {
      get
      {
        IView[] array = new IView[this.viewService.Views.Count];
        this.viewService.Views.CopyTo(array, 0);
        Array.Sort<IView>(array, (IComparer<IView>) new ViewServiceDialog.ViewComparer());
        return (IEnumerable<IView>) array;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ViewServiceDialog(IViewService viewService)
    {
      this.viewService = viewService;
      this.DataContext = (object) this;
      this.InitializeComponent();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.Framework;component/userinterface/viewmanagerdialog.xaml", UriKind.Relative));
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
          this.ViewList = (ListBox) target;
          break;
        case 2:
          this.AcceptButton = (Button) target;
          break;
        case 3:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class ViewComparer : IComparer<IView>
    {
      public int Compare(IView a, IView b)
      {
        return StringLogicalComparer.Instance.Compare(a.Caption, b.Caption);
      }
    }
  }
}

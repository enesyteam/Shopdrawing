// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CustomExpressionEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class CustomExpressionEditor : WorkaroundPopup, INotifyPropertyChanged, IComponentConnector
  {
    private SceneNodeProperty property;
    private string editingExpression;
    internal StringEditor ExpressionTextBox;
    private bool _contentLoaded;

    public string EditingExpression
    {
      get
      {
        return this.editingExpression;
      }
      set
      {
        this.editingExpression = value;
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged(this, new PropertyChangedEventArgs("EditingExpression"));
      }
    }

    public bool IsMixedValue
    {
      get
      {
        return this.property.IsMixedValue;
      }
    }

    public ICommand Commit
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCommit));
      }
    }

    public ICommand Cancel
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCancel));
      }
    }

    public ICommand ReturnFocus
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnReturnFocus));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public CustomExpressionEditor(SceneNodeProperty property)
    {
      this.property = property;
      this.DataContext = this;
      this.editingExpression = this.property.ExpressionAsString;
      this.InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
      base.OnOpened(e);
      this.ExpressionTextBox.Focus();
    }

    private void OnCommit()
    {
        if (!this.property.SetExpressionAsString(this.editingExpression))
        {
            base.Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback((object arg) =>
            {
                CustomExpressionEditor customExpressionEditor = new CustomExpressionEditor(this.property)
                {
                    PlacementTarget = base.PlacementTarget,
                    IsOpen = true
                };
                return null;
            }), null);
        }
    }

    private void OnCancel()
    {
      this.IsOpen = false;
    }

    private void OnReturnFocus()
    {
      this.IsOpen = false;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/customexpressioneditor.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ExpressionTextBox = (StringEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.Commands.FindAnnotationDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.Annotations.Commands
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class FindAnnotationDialog : Dialog, IComponentConnector
  {
    private static FindAnnotationDialog instance;
    private static FindAnnotationsModel findModel;
    private DesignerContext designerContext;
    internal OverlayStringEditor FindText;
    internal CheckBox MatchCaseCheckBox;
    internal RadioButton DirectionUpRadioButton;
    internal RadioButton DirectionDownRadioButton;
    internal Button FindButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    private FindAnnotationDialog(DesignerContext designerContext)
    {
      this.InitializeComponent();
      this.Title = designerContext.ExpressionInformationService.DefaultDialogTitle;
      this.designerContext = designerContext;
      if (FindAnnotationDialog.findModel == null)
        FindAnnotationDialog.findModel = new FindAnnotationsModel(designerContext, new Action<string>(this.ShowMessage));
      this.DataContext = (object) FindAnnotationDialog.findModel;
    }

    public static void ShowAndActivate(DesignerContext designerContext)
    {
      if (FindAnnotationDialog.instance == null)
        FindAnnotationDialog.instance = new FindAnnotationDialog(designerContext);
      FindAnnotationDialog.instance.Show();
      FindAnnotationDialog.instance.Activate();
    }

    public static void FindNext(DesignerContext designerContext)
    {
      if (FindAnnotationDialog.findModel != null)
        FindAnnotationDialog.findModel.FindNextAnnotation();
      else
        FindAnnotationDialog.ShowAndActivate(designerContext);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      FindAnnotationDialog.instance = (FindAnnotationDialog) null;
      base.OnClosing(e);
      this.Owner.Activate();
    }

    protected override void OnCancelButtonExecute()
    {
      this.Close();
    }

    private void FindAnnotationDialog_Activated(object sender, EventArgs e)
    {
      this.FindText.Focus();
      this.FindText.SelectAll();
    }

    private void ShowMessage(string message)
    {
      Window window = this.IsVisible ? (Window) this : this.Owner;
      int num = (int) this.designerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Owner = window,
        Message = message,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Asterisk
      });
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/annotations/find/findannotationdialog.xaml", UriKind.Relative));
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
          ((Window) target).Activated += new EventHandler(this.FindAnnotationDialog_Activated);
          break;
        case 2:
          this.FindText = (OverlayStringEditor) target;
          break;
        case 3:
          this.MatchCaseCheckBox = (CheckBox) target;
          break;
        case 4:
          this.DirectionUpRadioButton = (RadioButton) target;
          break;
        case 5:
          this.DirectionDownRadioButton = (RadioButton) target;
          break;
        case 6:
          this.FindButton = (Button) target;
          break;
        case 7:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

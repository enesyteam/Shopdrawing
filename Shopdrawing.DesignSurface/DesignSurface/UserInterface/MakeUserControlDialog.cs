// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.MakeUserControlDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class MakeUserControlDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private string name;
    private IProjectItemTemplate userControlItem;
    private DesignerContext designerContext;
    private MessageBubbleHelper controlNameMessageBubble;
    internal Grid DocumentRoot;
    internal TextBox Control_Name;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public string ControlName
    {
      get
      {
        return this.name;
      }
      set
      {
        if (!(this.name != value))
          return;
        this.name = value;
        this.OnPropertyChanged("ControlName");
      }
    }

    public bool InputIsValid
    {
      get
      {
        if (this.userControlItem != null)
          return ProjectItemNameValidator.ValidateName(this.designerContext.ActiveProject, this.ControlName);
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal MakeUserControlDialog(DesignerContext designerContext, string dialogTitle, TemplateItemHelper templateItemHelper, string recommendedName)
    {
      this.InitializeComponent();
      this.userControlItem = templateItemHelper.FindTemplateItem("UserControl");
      this.designerContext = designerContext;
      this.Title = dialogTitle;
      this.DataContext = (object) this;
      string str1 = designerContext.ProjectManager.TargetFolderForProject(designerContext.ActiveProject);
      if (!string.IsNullOrEmpty(recommendedName))
      {
        string str2 = recommendedName + ".xaml";
        DocumentReference documentReference = DocumentReference.Create(Path.Combine(str1, str2));
        this.name = designerContext.ActiveProject.FindItem(documentReference) == null ? recommendedName : Path.GetFileNameWithoutExtension(ProjectPathHelper.GetAvailableFilePath(str2, str1, designerContext.ActiveProject));
      }
      else if (this.userControlItem != null)
        this.name = Path.GetFileNameWithoutExtension(ProjectPathHelper.GetAvailableFilePath(this.userControlItem.DefaultName, str1, designerContext.ActiveProject));
      this.controlNameMessageBubble = new MessageBubbleHelper((UIElement) this.Control_Name, (IMessageBubbleValidator) new ProjectItemNameValidator(designerContext.ActiveProject));
    }

    public void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnAcceptButtonExecute()
    {
      if (!this.InputIsValid)
        return;
      base.OnAcceptButtonExecute();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/makeusercontroldialog.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.DocumentRoot = (Grid) target;
          break;
        case 2:
          this.Control_Name = (TextBox) target;
          break;
        case 3:
          this.AcceptButton = (Button) target;
          break;
        case 4:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

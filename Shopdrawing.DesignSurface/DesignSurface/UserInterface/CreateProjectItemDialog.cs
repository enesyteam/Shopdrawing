// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.CreateProjectItemDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class CreateProjectItemDialog : Dialog, INotifyPropertyChanged, IComponentConnector
  {
    private string description = string.Empty;
    private string fileName = string.Empty;
    private IProject project;
    private DesignerContext designerContext;
    private ObservableCollection<IProjectItemTemplate> projectItemTemplates;
    private ICollectionView projectItemTypesView;
    private IProjectItemTemplate itemTemplate;
    private IList<string> typeFilter;
    private MessageBubbleHelper itemListMessageBubble;
    private MessageBubbleHelper itemNameMessageBubble;
    private TemplateItemHelper templateItemHelper;
    internal ListBox ProjectItemTypeList;
    internal TextBlock DescriptionTextBlock;
    internal TextBox NameTextBox;
    internal Button AcceptButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public string Description
    {
      get
      {
        return this.description;
      }
    }

    public string FileName
    {
      get
      {
        return this.fileName;
      }
      set
      {
        this.fileName = value;
        this.OnPropertyChanged("FileName");
        this.OnPropertyChanged("InputIsValid");
      }
    }

    public ICollectionView ProjectItemTypes
    {
      get
      {
        return this.projectItemTypesView;
      }
    }

    public IProjectItemTemplate ItemTemplate
    {
      get
      {
        return this.itemTemplate;
      }
    }

    public bool InputIsValid
    {
      get
      {
        if (ProjectItemNameValidator.ValidateName(this.project, this.FileName))
          return this.projectItemTypesView.CurrentItem != null;
        return false;
      }
    }

    public ICommand SelectAndCloseCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SelectAndClose));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal CreateProjectItemDialog(IProject project, DesignerContext projectContext, IList<string> typeFilter)
    {
      this.InitializeComponent();
      this.project = project;
      this.designerContext = projectContext;
      this.typeFilter = typeFilter;
    }

    internal void InitializeDialog()
    {
      this.itemListMessageBubble = new MessageBubbleHelper((UIElement) this.ProjectItemTypeList, (IMessageBubbleValidator) new ProjectItemTypeValidator());
      this.itemNameMessageBubble = new MessageBubbleHelper((UIElement) this.NameTextBox, (IMessageBubbleValidator) new ProjectItemNameValidator(this.project));
      using (TemporaryCursor.SetWaitCursor())
        this.InitializeDialogInternal();
    }

    private void InitializeDialogInternal()
    {
      this.DataContext = (object) this;
      this.Title = StringTable.CreateProjectItemDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.projectItemTemplates = new ObservableCollection<IProjectItemTemplate>();
      this.projectItemTypesView = CollectionViewSource.GetDefaultView((object) this.projectItemTemplates);
      this.templateItemHelper = new TemplateItemHelper(this.project, this.typeFilter, (IServiceProvider) this.designerContext.Services);
      foreach (IProjectItemTemplate projectItemTemplate in this.templateItemHelper.TemplateItems)
      {
        if (!projectItemTemplate.Hidden)
          this.projectItemTemplates.Add(projectItemTemplate);
      }
      this.projectItemTypesView.SortDescriptions.Clear();
      this.projectItemTypesView.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
      this.projectItemTypesView.SortDescriptions.Add(new SortDescription("ItemName", ListSortDirection.Ascending));
      this.projectItemTypesView.CurrentChanged += new EventHandler(this.ProjectItemTypesView_CurrentChanged);
      if (this.projectItemTemplates.Count <= 0)
        return;
      this.projectItemTypesView.MoveCurrentToFirst();
      this.projectItemTypesView.Refresh();
    }

    protected override void OnAcceptButtonExecute()
    {
      this.OnAccept();
    }

    internal void OnAccept()
    {
      if (!this.InputIsValid)
        return;
      this.itemTemplate = (IProjectItemTemplate) this.projectItemTypesView.CurrentItem;
      base.OnAcceptButtonExecute();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void ProjectItemTypesView_CurrentChanged(object sender, EventArgs e)
    {
      IProjectItemTemplate projectItemTemplate = (IProjectItemTemplate) this.projectItemTypesView.CurrentItem;
      if (projectItemTemplate != null)
      {
        string targetFolder = this.designerContext.ProjectManager.TargetFolderForProject(this.project);
        this.FileName = projectItemTemplate.FindAvailableDefaultName(targetFolder, this.designerContext.ExpressionInformationService);
        this.description = projectItemTemplate.Description;
        this.OnPropertyChanged("Description");
      }
      this.OnPropertyChanged("InputIsValid");
    }

    private void SelectAndClose()
    {
      if (!this.InputIsValid)
        return;
      this.OnAccept();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/documents/createprojectitemdialog.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ProjectItemTypeList = (ListBox) target;
          break;
        case 2:
          this.DescriptionTextBlock = (TextBlock) target;
          break;
        case 3:
          this.NameTextBox = (TextBox) target;
          break;
        case 4:
          this.AcceptButton = (Button) target;
          break;
        case 5:
          this.CancelButton = (Button) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.XmlDataSourceDialogModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public sealed class XmlDataSourceDialogModel : INotifyPropertyChanged
  {
    private bool isEnabledAtRuntime = true;
    private bool isNameValid = true;
    private ICommand browseForXmlFileCommand;
    private string dataSourceName;
    private string dataSourceUrl;
    private bool isProjectScope;
    private string suggestedName;
    private string initialDataSourceName;
    private bool isPathValid;
    private bool isNameModified;
    private DataSourceDialogMode mode;
    private DataPanelModel dataPanelModel;

    public DataSourceDialogMode Mode
    {
      get
      {
        return this.mode;
      }
    }

    public string DataSourceName
    {
      get
      {
        return this.dataSourceName;
      }
      set
      {
        if (!(this.dataSourceName != value))
          return;
        this.dataSourceName = value;
        this.IsNameValid = this.CheckNameValidity();
        this.OnPropertyChanged("DataSourceName");
        this.isNameModified = true;
      }
    }

    public bool IsNameInUse
    {
      get
      {
        return this.SampleData.GetSampleDataSet(this.dataSourceName.Trim(), false) != null;
      }
    }

    public bool IsNameModified
    {
      get
      {
        return this.isNameModified;
      }
    }

    public bool IsNameValid
    {
      get
      {
        return this.isNameValid;
      }
      set
      {
        if (this.isNameValid == value)
          return;
        this.isNameValid = value;
        this.OnPropertyChanged("IsNameValid");
        this.OnPropertyChanged("IsAcceptButtonEnabled");
      }
    }

    public bool IsPathValid
    {
      get
      {
        return this.isPathValid;
      }
      set
      {
        if (this.isPathValid == value)
          return;
        this.isPathValid = value;
        this.OnPropertyChanged("IsPathValid");
        this.OnPropertyChanged("IsAcceptButtonEnabled");
      }
    }

    public bool IsAcceptButtonEnabled
    {
      get
      {
        if (this.Mode == DataSourceDialogMode.DefineNewDataStore || this.Mode == DataSourceDialogMode.DefineNewSampleData || this.IsPathValid)
          return this.IsNameValid;
        return false;
      }
    }

    internal string NormalizedDataSourceUrl { get; set; }

    public string DataSourceUrl
    {
      get
      {
        return this.dataSourceUrl;
      }
      set
      {
        if (!(this.dataSourceUrl != value))
          return;
        this.dataSourceUrl = value;
        this.UpdateNormalizedDataSourceUrl(value);
        if (this.DataSourceName == this.suggestedName && this.IsPathValid)
        {
          this.suggestedName = this.GetUniqueName(Path.GetFileNameWithoutExtension(this.NormalizedDataSourceUrl) + ((this.Mode == DataSourceDialogMode.SampleDataFromXml ? "Sample" : "") + "DataSource")).TrimStart('_');
          this.DataSourceName = this.suggestedName;
          this.isNameModified = true;
        }
        this.OnPropertyChanged("DataSourceUrl");
        this.OnPropertyChanged("XmlDataSource");
      }
    }

    public ICommand BrowseForXmlFileCommand
    {
      get
      {
        return this.browseForXmlFileCommand;
      }
    }

    public DocumentNode XmlDataSource
    {
      get
      {
        if (!this.IsPathValid)
          return (DocumentNode) null;
        IDocumentContext documentContext = this.ViewModel.Document.DocumentContext;
        DocumentCompositeNode node = documentContext.CreateNode(typeof (XmlDataProvider));
        Uri uri = new Uri(this.NormalizedDataSourceUrl, UriKind.RelativeOrAbsolute);
        IProject projectContainingItem = this.ViewModel.DesignerContext.ProjectManager.CurrentSolution.FindProjectContainingItem(DocumentReference.Create(this.NormalizedDataSourceUrl));
        if (projectContainingItem != null)
          uri = new Uri("\\" + DocumentReference.Create(this.ViewModel.ProjectContext.ProjectPath).GetRelativePath(projectContainingItem.FindItem(DocumentReference.Create(this.NormalizedDataSourceUrl)).DocumentReference), UriKind.Relative);
        DocumentNode documentNode = DocumentNodeUtilities.NewUriDocumentNode(documentContext, uri);
        node.Properties[XmlDataProviderSceneNode.SourceProperty] = documentNode;
        return (DocumentNode) node;
      }
    }

    public bool CanDefineInProjectRoot
    {
      get
      {
        if (this.dataPanelModel.SharedHost != null)
          return !this.dataPanelModel.SharedHost.DocumentHasErrors;
        return true;
      }
    }

    public bool CanDefineInThisDocument
    {
      get
      {
        if (this.dataPanelModel.CurrentDocumentHost != null)
          return !this.dataPanelModel.CurrentDocumentHost.DocumentHasErrors;
        return false;
      }
    }

    public bool IsEnabledAtRuntime
    {
      get
      {
        return this.isEnabledAtRuntime;
      }
      set
      {
        if (this.isEnabledAtRuntime == value)
          return;
        this.isEnabledAtRuntime = value;
        this.OnPropertyChanged("IsEnabledAtRuntime");
      }
    }

    public bool IsDefinedInProjectRoot
    {
      get
      {
        return this.isProjectScope;
      }
      set
      {
        this.UpdateIsProjectScope(value);
      }
    }

    public bool IsDefinedInThisDocument
    {
      get
      {
        return !this.isProjectScope;
      }
      set
      {
        this.UpdateIsProjectScope(!value);
      }
    }

    public Visibility XmlImportVisibility
    {
      get
      {
        return this.Mode == DataSourceDialogMode.SampleDataFromXml || this.Mode == DataSourceDialogMode.LiveXmlData ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public Visibility EnabledAtRuntimeVisibility
    {
      get
      {
        return this.Mode == DataSourceDialogMode.DefineNewSampleData || this.Mode == DataSourceDialogMode.SampleDataFromXml ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return this.dataPanelModel.ViewModel;
      }
    }

    private SampleDataCollection SampleData
    {
      get
      {
        return this.ViewModel.DefaultView.SampleData;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public XmlDataSourceDialogModel(DataPanelModel dataPanelModel, string initialDataSourceName, DataSourceDialogMode mode)
    {
      this.dataPanelModel = dataPanelModel;
      this.initialDataSourceName = initialDataSourceName;
      this.dataSourceName = initialDataSourceName;
      this.suggestedName = initialDataSourceName;
      this.browseForXmlFileCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnBrowseForXmlFile));
      this.mode = mode;
      this.UpdateIsProjectScope(true);
    }

    private bool CheckNameValidity()
    {
      string name = this.dataSourceName.Trim();
      return this.GetUniqueName(name) == name;
    }

    public string GetUniqueName(string name)
    {
      string str = this.mode != DataSourceDialogMode.LiveXmlData ? this.SampleData.GetUniqueSampleDataSetName(name) : SceneNodeIDHelper.ToCSharpID(name);
      if (string.IsNullOrEmpty(str))
        str = this.mode != DataSourceDialogMode.LiveXmlData ? this.SampleData.GetUniqueSampleDataSetName(this.initialDataSourceName) : SceneNodeIDHelper.ToCSharpID(this.initialDataSourceName);
      return str;
    }

    private void UpdateNormalizedDataSourceUrl(string value)
    {
      this.IsPathValid = false;
      this.NormalizedDataSourceUrl = value ?? string.Empty;
      this.NormalizedDataSourceUrl = this.NormalizedDataSourceUrl.Trim(" \r\n\t\"".ToCharArray());
      if (this.NormalizedDataSourceUrl.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        return;
      try
      {
        this.IsPathValid = false;
        Uri result = (Uri) null;
        if (!Uri.TryCreate(this.NormalizedDataSourceUrl, UriKind.RelativeOrAbsolute, out result) || !result.IsAbsoluteUri && !Uri.TryCreate(Path.Combine(Path.GetDirectoryName(this.dataPanelModel.ProjectContext.ProjectPath), this.NormalizedDataSourceUrl), UriKind.Absolute, out result) || !result.IsAbsoluteUri)
          return;
        if (result.IsFile && !result.IsUnc)
        {
          this.IsPathValid = Microsoft.Expression.Framework.Documents.PathHelper.FileExists(result.LocalPath);
          if (!this.IsPathValid)
            return;
          this.NormalizedDataSourceUrl = result.LocalPath;
        }
        else
          this.IsPathValid = !string.IsNullOrEmpty(result.DnsSafeHost);
      }
      catch (InvalidOperationException ex)
      {
      }
      catch (IOException ex)
      {
      }
      catch (AccessViolationException ex)
      {
      }
    }

    public void UpdateDataSourceToNormalizedDataSource()
    {
      if (!(this.dataSourceUrl != this.NormalizedDataSourceUrl))
        return;
      this.dataSourceUrl = this.NormalizedDataSourceUrl;
      this.OnPropertyChanged("DataSourceUrl");
    }

    private void UpdateIsProjectScope(bool value)
    {
      value = value && this.CanDefineInProjectRoot;
      if (this.isProjectScope == value)
        return;
      this.isProjectScope = value;
      this.OnPropertyChanged("IsDefinedInThisDocument");
      this.OnPropertyChanged("IsDefinedInProjectRoot");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnBrowseForXmlFile()
    {
      string str = this.BrowseForXmlFile();
      if (str == null)
        return;
      this.DataSourceUrl = str;
    }

    private string BrowseForXmlFile()
    {
      return this.BrowseForXmlFile(StringTable.BrowseForXmlDataSourceDialogTitle);
    }

    private string BrowseForXmlFile(string dialogTitle)
    {
      try
      {
        string path = this.dataPanelModel.XmlDataSourceFolderManager.Path;
        string str = XmlDataSourceDialogModel.BrowseForXmlFile(dialogTitle, ref path);
        this.dataPanelModel.XmlDataSourceFolderManager.Path = path;
        return str;
      }
      catch (Exception ex)
      {
        this.ViewModel.DesignerContext.MessageDisplayService.ShowError(new ErrorArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CommandFailedDialogMessage, new object[1]
          {
            (object) dialogTitle
          }),
          Exception = ex
        });
      }
      return (string) null;
    }

    public static string BrowseForXmlFile(string dialogTitle, ref string initialDirectory)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.XmlFileTypeDescription, new object[1]
      {
        (object) "*.xml"
      }) + "|*.xml" + "|" + (string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AllFilesTypeDescription, new object[1]
      {
        (object) "*.*"
      }) + "|*.*");
      ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog();
      expressionOpenFileDialog.Title = dialogTitle;
      expressionOpenFileDialog.RestoreDirectory = true;
      expressionOpenFileDialog.InitialDirectory = initialDirectory;
      expressionOpenFileDialog.Filter = str;
      bool? nullable = expressionOpenFileDialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return (string) null;
      initialDirectory = Path.GetDirectoryName(expressionOpenFileDialog.FileName);
      return expressionOpenFileDialog.FileName;
    }
  }
}

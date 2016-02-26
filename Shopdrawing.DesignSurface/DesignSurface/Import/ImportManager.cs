// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Import.ImportManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Importers;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Import
{
  public class ImportManager : ISceneNodeImporterContext, IPrototypingImporterContext, IModelItemImporterContext, IImporterContext, IDisposable
  {
    private IImporterService importService;
    private IPrototypingService prototypingService;
    private IMessageDisplayService messageDisplayService;
    private IImportManagerContext importContext;
    private Action<object, DoWorkEventArgs> asyncHandler;
    private object asyncHandlerArgument;
    private static bool importerRegistered;
    private string supportingFolderName;
    private string supportingDirectoryPath;
    private ProjectPathHelper.TemporaryDirectory temporaryFolderPlaceholder;
    private IList<KeyValuePair<string, string>> supportingFiles;
    private bool stripDotsFromLastPathComponent;
    private ISceneInsertionPoint initialInsertionPoint;
    private ISceneInsertionPoint insertionPoint;
    private int countMessages;
    private double completeRatio;
    private bool uilessMode;
    private Dictionary<string, object> hostData;

    public IImportManagerContext ImportContext
    {
      get
      {
        return this.importContext;
      }
      set
      {
        this.importContext = value;
      }
    }

    public bool CurrentlyImporting { get; private set; }

    private string ImportDirectoryPath
    {
      get
      {
        return this.importContext.ImportDirectoryPath;
      }
    }

    public string TargetImportPath
    {
      get
      {
        return Path.Combine(this.ImportDirectoryPath, this.GetSupportingFolderName(true));
      }
    }

    public string ImportFilePath
    {
      get
      {
        return this.ImportContext.FileName;
      }
    }

    public string TemporaryFolder
    {
      get
      {
        return this.temporaryFolderPlaceholder.Path;
      }
    }

    public IDictionary<string, object> HostData
    {
      get
      {
        return (IDictionary<string, object>) this.hostData;
      }
    }

    public FrameworkName HostPlatform
    {
      get
      {
        return this.SceneViewModel.ProjectContext.TargetFramework;
      }
    }

    public bool NoUserInterface
    {
      get
      {
        return this.uilessMode;
      }
    }

    public ModelItem Root
    {
      get
      {
        if (this.insertionPoint != null)
          return (ModelItem) this.insertionPoint.SceneElement.ModelItem;
        ISceneInsertionPoint importInsertionPoint = ImportManager.GetImportInsertionPoint(this.ImportContext.SceneViewModel);
        if (importInsertionPoint != null)
          return (ModelItem) importInsertionPoint.SceneElement.ModelItem;
        return (ModelItem) null;
      }
      set
      {
        this.InitializeRoot(value);
      }
    }

    public SceneViewModel SceneViewModel
    {
      get
      {
        return this.ImportContext.SceneViewModel;
      }
    }

    public ISceneInsertionPoint InsertionPoint
    {
      get
      {
        return this.insertionPoint;
      }
    }

    public ImportManager(IImporterService importService, IPrototypingService prototypingService, IMessageDisplayService messageDisplayService)
    {
      this.importService = importService;
      this.prototypingService = prototypingService;
      this.messageDisplayService = messageDisplayService;
      this.hostData = new Dictionary<string, object>();
      this.ResetInternal();
      this.RegisterImporters();
    }

    public static ISceneInsertionPoint GetImportInsertionPoint(SceneViewModel sceneViewModel)
    {
      SceneElement sceneElement = (SceneElement) sceneViewModel.FindPanelClosestToRoot();
      if (sceneElement != null && sceneElement.DefaultInsertionPoint != null && sceneElement.DefaultInsertionPoint.CanInsert(PlatformTypes.Canvas))
        return sceneElement.DefaultInsertionPoint;
      return (ISceneInsertionPoint) null;
    }

    public bool Import(IImportManagerContext importManagerContext, string[] filterImportersId, DoWorkEventArgs args)
    {
      this.ResetInternal();
      this.importContext = importManagerContext;
      this.uilessMode = !string.IsNullOrEmpty(importManagerContext.FileName);
      IImporter importer = this.PromptUserForAnImporter(filterImportersId);
      if (importer == null)
        return false;
      this.importContext.CreateEditTransaction();
      Stream importData = (Stream) null;
      object property1 = importer.GetProperty("StripDotsFromLastPathComponent");
      if (property1 != null)
        this.stripDotsFromLastPathComponent = (bool) property1;
      object property2 = importer.GetProperty("ImportDataStoreSupport");
      if (property2 != null && (bool) property2)
      {
        IImporterDataStore importerDataStore = this.importService as IImporterDataStore;
        if (importerDataStore != null)
        {
          string path = importerDataStore.GetDataFileEntry(this.importContext.FileName) ?? importerDataStore.CreateDataFileEntry(this.importContext.FileName);
          if (path != null)
          {
            try
            {
              importData = (Stream) new FileStream(path, FileMode.OpenOrCreate);
            }
            catch (IOException ex)
            {
              importData = (Stream) null;
            }
          }
        }
      }
      bool flag1 = false;
      this.importService.ImporterManager = (object) this;
      this.CurrentlyImporting = true;
      bool flag2;
      try
      {
        flag2 = importer.Import((object) this, DocumentReference.Create(this.importContext.FileName), importData);
      }
      catch (OperationCanceledException ex)
      {
        flag2 = false;
      }
      catch (Exception ex)
      {
        flag2 = false;
        flag1 = true;
      }
      if (importData != null)
        importData.Dispose();
      if (this.asyncHandler == null)
      {
        if (flag2)
        {
          this.CommitImportOperation();
          this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImportSucceededReportPane, new object[1]
          {
            (object) this.importContext.FileName
          }));
        }
        else
        {
          this.CancelImportOperation();
          if (flag1)
          {
            int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
            {
              Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.countMessages == 0 ? StringTable.ImportFailed : StringTable.ImportFailedMoreInfo, new object[1]
              {
                (object) this.importContext.FileName
              }),
              Button = MessageBoxButton.OK,
              Image = MessageBoxImage.Hand
            });
          }
        }
        this.CurrentlyImporting = false;
        return flag2;
      }
      args.Result = (object) AsyncProcessResult.StillGoing;
      return true;
    }

    public void CancelImport()
    {
      if (this.importContext == null)
        return;
      this.CancelImportOperation();
    }

    public void ResumeImport(DoWorkEventArgs asyncArg)
    {
      DoWorkEventArgs doWorkEventArgs = new DoWorkEventArgs(this.asyncHandlerArgument);
      doWorkEventArgs.Result = (object) AsyncImportResult.Done;
      bool flag = false;
      try
      {
        this.asyncHandler((object) this, doWorkEventArgs);
      }
      catch (OperationCanceledException ex)
      {
        doWorkEventArgs.Result = (object) AsyncImportResult.Aborted;
        flag = false;
      }
      catch (Exception ex)
      {
        doWorkEventArgs.Result = (object) AsyncImportResult.Aborted;
        flag = true;
      }
      AsyncProgressInfo asyncProgressInfo = (AsyncProgressInfo) asyncArg.Argument;
      asyncProgressInfo.CompleteRatio = this.completeRatio;
      if ((AsyncImportResult) doWorkEventArgs.Result == AsyncImportResult.StillGoing)
        asyncProgressInfo.Status = AsyncProcessResult.StillGoing;
      else if ((AsyncImportResult) doWorkEventArgs.Result == AsyncImportResult.Done)
      {
        asyncProgressInfo.Status = AsyncProcessResult.Done;
        this.CommitImportOperation();
        this.LogMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImportSucceededReportPane, new object[1]
        {
          (object) this.importContext.FileName
        }));
      }
      else
      {
        asyncProgressInfo.Status = AsyncProcessResult.Aborted;
        this.CancelImportOperation();
      }
      if ((AsyncImportResult) doWorkEventArgs.Result != AsyncImportResult.StillGoing)
        this.CurrentlyImporting = false;
      if (flag)
      {
        int num = (int) this.messageDisplayService.ShowMessage(new MessageBoxArgs()
        {
          Message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.countMessages == 0 ? StringTable.ImportFailed : StringTable.ImportFailedMoreInfo, new object[1]
          {
            (object) this.importContext.FileName
          }),
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Hand
        });
      }
      asyncArg.Result = (object) asyncProgressInfo;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Cleanup(true);
    }

    private void ResetInternal()
    {
      this.hostData.Clear();
      this.insertionPoint = (ISceneInsertionPoint) null;
      this.initialInsertionPoint = (ISceneInsertionPoint) null;
      this.supportingFolderName = (string) null;
      this.supportingDirectoryPath = (string) null;
      this.asyncHandler = (Action<object, DoWorkEventArgs>) null;
      this.asyncHandlerArgument = (object) null;
      this.countMessages = 0;
      this.completeRatio = 0.0;
      this.importContext = (IImportManagerContext) null;
      if (this.temporaryFolderPlaceholder != null)
        this.temporaryFolderPlaceholder.Dispose();
      this.temporaryFolderPlaceholder = new ProjectPathHelper.TemporaryDirectory(false, true);
      this.supportingFiles = (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
    }

    private void RegisterImporters()
    {
      if (ImportManager.importerRegistered)
        return;
      this.importService.RegisterImporter("psd", new ImporterCreatorCallback(this.CreatePhotoshopImporter));
      this.importService.SetProperty("psd", "Filter", (object) "*.psd");
      this.importService.SetProperty("psd", "FileTypeDescription", (object) StringTable.PhotoshopFileTypeDescription);
      this.importService.SetProperty("psd", "ImportFileOpenDialogTitle", (object) StringTable.ImportPhotoshopFileOpenDialogTitle);
      this.importService.SetProperty("psd", "ImportDataStoreSupport", (object) true);
      this.importService.RegisterImporter("ai", new ImporterCreatorCallback(this.CreateIllustratorImporter));
      this.importService.SetProperty("ai", "Filter", (object) "*.ai");
      this.importService.SetProperty("ai", "FileTypeDescription", (object) StringTable.IllustratorFileTypeDescription);
      this.importService.SetProperty("ai", "ImportFileOpenDialogTitle", (object) StringTable.ImportIllustratorFileOpenDialogTitle);
      if (this.prototypingService != null)
      {
        this.importService.RegisterImporter("ppt", new ImporterCreatorCallback(this.CreatePowerPointImporter));
        this.importService.SetProperty("ppt", "Filter", (object) "*.ppt;*.pptx");
        this.importService.SetProperty("ppt", "FileTypeDescription", (object) StringTable.PowerPointFileTypeDescription);
        this.importService.SetProperty("ppt", "ImportFileOpenDialogTitle", (object) StringTable.ImportPowerPointFileOpenDialogTitle);
        this.importService.SetProperty("ppt", "ImportDataStoreSupport", (object) false);
        this.importService.SetProperty("ppt", "StripDotsFromLastPathComponent", (object) true);
      }
      ImportManager.importerRegistered = true;
    }

    private IImporter CreatePhotoshopImporter()
    {
      return this.CreateImporter("Microsoft.Expression.Importers.dll", "Microsoft.Expression.Importers.Photoshop.PhotoshopImporter");
    }

    private IImporter CreateIllustratorImporter()
    {
      return this.CreateImporter("Microsoft.Expression.Importers.dll", "Microsoft.Expression.Importers.Illustrator.IllustratorImporter");
    }

    private IImporter CreatePowerPointImporter()
    {
      return this.CreateImporter("Microsoft.Expression.PrototypeHostEnvironment.dll", "Microsoft.Expression.Importers.Prototyping.PowerPointImporter");
    }

    private IImporter CreateImporter(string assemblyName, string typeName)
    {
      Assembly assembly = ProjectAssemblyHelper.LoadFrom(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), assemblyName));
      if (assembly == (Assembly) null)
        return (IImporter) null;
      return (IImporter) Activator.CreateInstance(assembly.GetType(typeName));
    }

    private IImporter PromptUserForAnImporter(string[] filterImportersID)
    {
      ExpressionOpenFileDialog dialog = new ExpressionOpenFileDialog();
      dialog.RestoreDirectory = true;
      dialog.Multiselect = false;
      IEnumerable<string> registeredImporters = this.importService.RegisteredImporters;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      int num = 0;
      string str1 = (string) null;
      foreach (string identifier in registeredImporters)
      {
        if (filterImportersID == null || Enumerable.Contains<string>((IEnumerable<string>) filterImportersID, identifier))
        {
          string str2 = (string) this.importService.GetProperty(identifier, "Filter");
          if (stringBuilder1.Length != 0)
            stringBuilder1.Append("|");
          stringBuilder1.AppendFormat((string) this.importService.GetProperty(identifier, "FileTypeDescription"), (object) (string) this.importService.GetProperty(identifier, "Filter"));
          stringBuilder1.AppendFormat("|{0}", (object) str2);
          ++num;
          if (stringBuilder2.Length != 0)
            stringBuilder2.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, ";{0}", new object[1]
            {
              (object) str2
            });
          else
            stringBuilder2.Append(str2);
          str1 = (string) this.importService.GetProperty(identifier, "ImportFileOpenDialogTitle");
        }
      }
      if (num > 1)
      {
        StringBuilder stringBuilder3 = new StringBuilder();
        stringBuilder3.Append(StringTable.ImportAllSupportedFiles);
        stringBuilder3.AppendFormat("|{0}|", (object) stringBuilder2);
        stringBuilder3.Append((object) stringBuilder1);
        dialog.Title = StringTable.ImportFileOpenDialogTitle;
        dialog.Filter = stringBuilder3.ToString();
      }
      else
      {
        dialog.Filter = stringBuilder1.ToString();
        dialog.Title = str1 ?? StringTable.ImportFileOpenDialogTitle;
      }
      if (string.IsNullOrEmpty(this.ImportContext.FileName))
        this.ImportContext.AskForFileName(dialog);
      if (string.IsNullOrEmpty(this.ImportContext.FileName))
        return (IImporter) null;
      string extension = Path.GetExtension(this.importContext.FileName);
      if (extension.Length > 0)
      {
        foreach (string identifier in registeredImporters)
        {
          string str2 = (string) this.importService.GetProperty(identifier, "Filter");
          char[] chArray = new char[1]
          {
            ';'
          };
          foreach (string str3 in str2.Split(chArray))
          {
            if (str3.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
              return this.importService[identifier];
          }
        }
      }
      return (IImporter) null;
    }

    private bool InitializeRoot(ModelItem container)
    {
      SceneElement sceneElement = ((ISceneNodeModelItem) container).SceneNode as SceneElement;
      if (container == null || sceneElement == null || this.ImportContext.SceneViewModel == null)
        return false;
      ISceneInsertionPoint importInsertionPoint = ImportManager.GetImportInsertionPoint(this.ImportContext.SceneViewModel);
      if (importInsertionPoint == null)
        return false;
      this.insertionPoint = importInsertionPoint;
      this.insertionPoint.Insert((SceneNode) sceneElement);
      this.insertionPoint = sceneElement.DefaultInsertionPoint;
      this.initialInsertionPoint = this.insertionPoint;
      return this.insertionPoint != null;
    }

    private static string TokenizeFontName(string fontName)
    {
      return fontName.Replace(" ", string.Empty);
    }

    private string FindFontFromProject(string fontNameToken, string modifierToken)
    {
      ObservableCollection<IProjectFont> projectFonts = this.SceneViewModel.ProjectContext.ProjectFonts;
      IFontResolver fontResolver = this.SceneViewModel.ProjectContext.FontResolver;
      IEnumerable<IProjectFont> source = Enumerable.Where<IProjectFont>((IEnumerable<IProjectFont>) projectFonts, (Func<IProjectFont, bool>) (fontFamily =>
      {
        if (!ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(fontFamily.FontFamilyName)).StartsWith(fontNameToken, StringComparison.OrdinalIgnoreCase))
          return ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(fontFamily.FontFamilyName)).StartsWith(fontNameToken, StringComparison.OrdinalIgnoreCase);
        return true;
      }));
      string fontMatching = modifierToken == null ? fontNameToken : fontNameToken + modifierToken;
      IProjectFont projectFont1 = (IProjectFont) null;
      foreach (IProjectFont projectFont2 in source)
      {
        string strA1 = ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(projectFont2.FontFamilyName));
        string strA2 = ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(projectFont2.FontFamilyName));
        if (string.Compare(strA1, fontMatching, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA2, fontMatching, StringComparison.OrdinalIgnoreCase) == 0)
          return FontEmbedder.MakeProjectFontReference(projectFont2, this.SceneViewModel.Document.DocumentContext).Source;
        if (projectFont1 == null || projectFont1.FontFamilyName.Length > projectFont2.FontFamilyName.Length)
          projectFont1 = projectFont2;
      }
      if (projectFont1 == null)
      {
        Enumerable.Where<IProjectFont>((IEnumerable<IProjectFont>) projectFonts, (Func<IProjectFont, bool>) (fontFamily =>
        {
          if (!ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(fontFamily.FontFamilyName)).StartsWith(fontMatching, StringComparison.OrdinalIgnoreCase))
            return ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(fontFamily.FontFamilyName)).StartsWith(fontMatching, StringComparison.OrdinalIgnoreCase);
          return true;
        }));
        if (Enumerable.Count<IProjectFont>(source) > 0)
          projectFont1 = Enumerable.First<IProjectFont>(source);
      }
      if (projectFont1 != null)
        return FontEmbedder.MakeProjectFontReference(projectFont1, this.SceneViewModel.Document.DocumentContext).Source;
      return (string) null;
    }

    private string FindFontFromSystem(string fontNameToken, string modifierToken)
    {
      IEnumerable<SystemFontFamily> systemFonts = FontEmbedder.GetSystemFonts((ITypeResolver) this.SceneViewModel.ProjectContext);
      IFontResolver fontResolver = this.SceneViewModel.ProjectContext.FontResolver;
      IEnumerable<SystemFontFamily> enumerable = Enumerable.Where<SystemFontFamily>(systemFonts, (Func<SystemFontFamily, bool>) (fontFamily =>
      {
        if (!ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(fontFamily.FontFamilyName)).StartsWith(fontNameToken, StringComparison.OrdinalIgnoreCase))
          return ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(fontFamily.FontFamilyName)).StartsWith(fontNameToken, StringComparison.OrdinalIgnoreCase);
        return true;
      }));
      string fontMatching = modifierToken == null ? fontNameToken : fontNameToken + modifierToken;
      string str = (string) null;
      foreach (SystemFontFamily systemFontFamily in enumerable)
      {
        string strA1 = ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(systemFontFamily.FontFamilyName));
        string strA2 = ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(systemFontFamily.FontFamilyName));
        if (string.Compare(strA1, fontMatching, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA2, fontMatching, StringComparison.OrdinalIgnoreCase) == 0)
          return systemFontFamily.FontFamilyName;
        if (str == null || str.Length > systemFontFamily.FontFamilyName.Length)
          str = systemFontFamily.FontFamilyName;
      }
      if (str == null)
      {
        IEnumerable<SystemFontFamily> source = Enumerable.Where<SystemFontFamily>(systemFonts, (Func<SystemFontFamily, bool>) (fontFamily =>
        {
          if (!ImportManager.TokenizeFontName(fontResolver.ConvertToGdiFontName(fontFamily.FontFamilyName)).StartsWith(fontMatching, StringComparison.OrdinalIgnoreCase))
            return ImportManager.TokenizeFontName(fontResolver.ConvertToWpfFontName(fontFamily.FontFamilyName)).StartsWith(fontMatching, StringComparison.OrdinalIgnoreCase);
          return true;
        }));
        if (Enumerable.Count<SystemFontFamily>(source) > 0)
          str = Enumerable.First<SystemFontFamily>(source).FontFamilyName;
      }
      return str;
    }

    private void CommitSupportingFileToProject()
    {
      if (this.supportingDirectoryPath == null)
        return;
      this.ImportContext.ActiveProject.AddItems(Enumerable.Select<KeyValuePair<string, string>, DocumentCreationInfo>((IEnumerable<KeyValuePair<string, string>>) this.supportingFiles, (Func<KeyValuePair<string, string>, DocumentCreationInfo>) (suportingItem => new DocumentCreationInfo()
      {
        SourcePath = suportingItem.Key,
        TargetPath = suportingItem.Value,
        CreationOptions = CreationOptions.DoNotSelectCreatedItems | CreationOptions.AlwaysUseDefaultBuildTask
      })));
    }

    private void CommitImportOperation()
    {
      this.CommitSupportingFileToProject();
      this.ImportContext.CommitEditTransaction();
      this.Cleanup(false);
    }

    private void CancelImportOperation()
    {
      this.ImportContext.CancelEditTransaction();
      if (!this.ImportContext.SupportProjectRollbackOnCancel && this.CurrentlyImporting)
      {
        this.CommitSupportingFileToProject();
        this.Cleanup(false);
      }
      else
        this.Cleanup(true);
    }

    private void Cleanup(bool cleanSupportingFolderIfNotInProject)
    {
      if (this.temporaryFolderPlaceholder != null)
      {
        this.temporaryFolderPlaceholder.Dispose();
        this.temporaryFolderPlaceholder = (ProjectPathHelper.TemporaryDirectory) null;
      }
      if (this.supportingDirectoryPath != null && cleanSupportingFolderIfNotInProject)
      {
        if (this.ImportContext.ActiveProject.FindItem(DocumentReference.Create(this.supportingDirectoryPath)) == null)
        {
          try
          {
            Directory.Delete(this.supportingDirectoryPath);
          }
          catch (IOException ex)
          {
          }
        }
      }
      IDisposable disposable = this.asyncHandlerArgument as IDisposable;
      if (disposable != null)
        disposable.Dispose();
      this.asyncHandlerArgument = (object) null;
    }

    private string GetSupportingFolderName(bool create)
    {
      if (this.supportingFolderName == null)
      {
        string str = Path.GetFileNameWithoutExtension(this.importContext.FileName);
        if (this.stripDotsFromLastPathComponent)
          str = str.Replace(".", "_");
        this.GetSupportingDirectory(str + StringTable.ImportImageDirectoryNameSubstring, create);
      }
      if (!create || this.supportingDirectoryPath != null)
        return this.supportingFolderName;
      return this.GetSupportingDirectory(this.supportingFolderName, create);
    }

    private string GetSupportingDirectory(string supportingFolderName, bool create)
    {
      string importDirectoryPath = this.ImportDirectoryPath;
      string path1 = Path.Combine(importDirectoryPath, supportingFolderName);
      IProject activeProject = this.importContext.ActiveProject;
      if (activeProject == null)
        return (string) null;
      if (this.supportingFolderName == null)
      {
        DocumentReference documentReference = DocumentReference.Create(path1);
        if (activeProject.FindItem(documentReference) != null)
        {
          string str1 = ProjectPathHelper.GetAvailableFilePath(supportingFolderName, importDirectoryPath, activeProject, true).Substring(importDirectoryPath.Length).Trim(Path.DirectorySeparatorChar);
          string str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImportImageDirectoryOverwriteDialogMessage, new object[2]
          {
            (object) supportingFolderName,
            (object) str1
          });
          Dictionary<MessageChoice, string> dictionary = new Dictionary<MessageChoice, string>();
          dictionary[MessageChoice.Yes] = StringTable.ImportImageDirectoryOverwriteDialogMessageYesButtonCaption;
          dictionary[MessageChoice.No] = StringTable.ImportImageDirectoryOverwriteDialogMessageNoButtonCaption;
          switch (this.messageDisplayService.ShowMessage(new MessageBoxArgs()
          {
            Message = str2,
            Button = MessageBoxButton.YesNoCancel,
            Image = MessageBoxImage.Exclamation,
            TextOverrides = (IDictionary<MessageChoice, string>) dictionary
          }))
          {
            case MessageBoxResult.Yes:
              supportingFolderName = str1;
              break;
            case MessageBoxResult.No:
              break;
            default:
              throw new OperationCanceledException();
          }
        }
        this.supportingFolderName = supportingFolderName;
      }
      string path2 = Path.Combine(importDirectoryPath, this.supportingFolderName);
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path2) && create)
        Directory.CreateDirectory(path2);
      this.supportingDirectoryPath = path2;
      return this.supportingFolderName;
    }

    public string AddSupportingFileToProject(string fileName, out string sourceReference, bool useExistingFile)
    {
      sourceReference = (string) null;
      IProject activeProject = this.importContext.ActiveProject;
      if (activeProject == null)
        return (string) null;
      this.GetSupportingFolderName(true);
      string path = Microsoft.Expression.Framework.Documents.PathHelper.BuildValidFileName(fileName);
      if (path != null)
      {
        ITypeResolver typeResolver = (ITypeResolver) (activeProject as IXamlProject).ProjectContext;
        if (typeResolver != null && !typeResolver.IsCapabilitySet(PlatformCapability.IsWpf))
          path = path.Replace("%", "");
      }
      string str1;
      if (string.IsNullOrEmpty(path))
      {
        str1 = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(fileName);
        fileName = StringTable.SupportingFilePlaceholder;
        if (str1 != null)
          fileName += str1;
      }
      else
      {
        fileName = path;
        str1 = Path.GetExtension(path);
      }
      string fullPath = (string) null;
      string str2 = (string) null;
      bool flag = false;
      string str3 = !useExistingFile ? this.temporaryFolderPlaceholder.Path : this.supportingDirectoryPath;
      while (true)
      {
        if (str2 != null)
          goto label_20;
label_10:
        try
        {
          if (useExistingFile)
          {
            str2 = Path.Combine(str3, fileName);
          }
          else
          {
            str2 = ProjectPathHelper.GetAvailableFilePath(fileName, str3, (IProject) null, true);
            fileName = Path.GetFileName(str2);
          }
          fullPath = Path.Combine(this.supportingDirectoryPath, fileName);
          if (fullPath.Length > 256 || str2.Length > 256)
          {
            fullPath = (string) null;
            str2 = (string) null;
            throw new PathTooLongException();
          }
          this.supportingFiles.Add(new KeyValuePair<string, string>(str2, fullPath));
          continue;
        }
        catch (Exception ex)
        {
          if (flag)
          {
            fullPath = (string) null;
            str2 = (string) null;
            break;
          }
          fileName = string.IsNullOrEmpty(str1) ? StringTable.SupportingFilePlaceholder : StringTable.SupportingFilePlaceholder + str1;
          flag = true;
          continue;
        }
label_20:
        if (fullPath == null)
          goto label_10;
        else
          break;
      }
      if (fullPath != null && str2 != null)
      {
        sourceReference = this.importContext.MakeSourceReference(fullPath);
        return str2;
      }
      sourceReference = (string) null;
      return (string) null;
    }

    public void ExecuteAsyncImport(Action<object, DoWorkEventArgs> handler, object argument)
    {
      this.asyncHandler = handler;
      this.asyncHandlerArgument = argument;
    }

    public string FindFont(string fontNameToken, string modifierToken)
    {
      if (this.ImportContext.SceneViewModel.FontEmbedder == null)
        return string.Empty;
      return this.FindFontFromProject(fontNameToken, modifierToken) ?? this.FindFontFromSystem(fontNameToken, modifierToken);
    }

    public void LogMessage(string message)
    {
      if (!this.importContext.LogMessage(message))
        return;
      ++this.countMessages;
    }

    public void UpdateProgress(double ratio)
    {
      this.completeRatio = ratio;
    }

    public void SimplifyPathData(ref System.Windows.Media.Geometry geometry)
    {
      PathGeometry geometry1 = geometry as PathGeometry;
      BooleanCommand.CleanUpPathGeometry(ref geometry1);
      PathGeometry geometry2 = PathConversionHelper.RemoveDegeneratePoints((System.Windows.Media.Geometry)geometry1);
      PathGeometryUtilities.CollapseSingleSegmentsToPolySegments(geometry2);
      PathCommandHelper.GrokPathPointPrecision(geometry2, 3);
      geometry = (System.Windows.Media.Geometry)geometry2;
    }

    public MessageBoxResult PromptUserYesNo(string message, string yesText, string noText, string automationId)
    {
      return this.messageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = message,
        TextOverrides = (IDictionary<MessageChoice, string>) new Dictionary<MessageChoice, string>()
        {
          {
            MessageChoice.Yes,
            yesText
          },
          {
            MessageChoice.No,
            noText
          }
        },
        Button = MessageBoxButton.YesNoCancel,
        Image = MessageBoxImage.Exclamation,
        AutomationId = automationId
      });
    }

    public string CreateNameId(ModelItem item, string name)
    {
      ISceneNodeModelItem sceneNodeModelItem = item as ISceneNodeModelItem;
      if (sceneNodeModelItem != null)
        return this.importContext.CreateNameId(sceneNodeModelItem.SceneNode, name);
      return name;
    }

    public void SetValidNameId(SceneNode sceneNode, string name)
    {
      sceneNode.Name = this.importContext.CreateNameId(sceneNode, name);
    }

    public bool InitializeInsertionPoint()
    {
      ISceneInsertionPoint importInsertionPoint = ImportManager.GetImportInsertionPoint(this.ImportContext.SceneViewModel);
      if (importInsertionPoint == null)
        return false;
      if (this.initialInsertionPoint == null)
      {
        this.insertionPoint = importInsertionPoint;
        SceneElement sceneElement = this.ImportContext.SceneViewModel.CreateSceneNode(PlatformTypes.Canvas) as SceneElement;
        new SceneNodeIDHelper().SetValidName((SceneNode) sceneElement, Path.GetFileNameWithoutExtension(this.importContext.FileName));
        this.insertionPoint.Insert((SceneNode) sceneElement);
        this.insertionPoint = sceneElement.DefaultInsertionPoint;
        this.initialInsertionPoint = this.insertionPoint;
      }
      else
        this.insertionPoint = this.initialInsertionPoint;
      return this.insertionPoint != null;
    }

    public bool CreatePrototypingScreen(string name, Point position)
    {
      if (this.prototypingService != null)
        return this.prototypingService.AddScreen(name, position);
      return false;
    }

    public ModelItem GetRootForPrototypingScreen(string name)
    {
      if (this.prototypingService != null)
      {
        ISceneInsertionPoint importInsertionPoint = ImportManager.GetImportInsertionPoint(this.prototypingService.GetSceneViewModelForScreen(name));
        if (importInsertionPoint != null)
          return (ModelItem) importInsertionPoint.SceneElement.ModelItem;
      }
      return (ModelItem) null;
    }

    public bool ConnectPrototypingScreens(string fromName, string toName)
    {
      if (this.prototypingService != null)
        return this.prototypingService.ConnectPrototypingScreens(fromName, toName);
      return false;
    }

    public bool ConnectFromStartScreen(string toName)
    {
      if (this.prototypingService != null)
        return this.prototypingService.ConnectFromStartScreen(toName);
      return false;
    }

    public Point NextOpenGraphPosition()
    {
      if (this.prototypingService != null)
        return this.prototypingService.NextOpenGraphPosition();
      return new Point(100.0, 100.0);
    }

    public bool ScreenExists(string name)
    {
      if (this.prototypingService != null)
        return this.prototypingService.ScreenExists(name);
      return false;
    }

    public void CloseScreen(string name)
    {
      if (this.prototypingService == null)
        return;
      this.prototypingService.CloseScreen(name);
    }
  }
}

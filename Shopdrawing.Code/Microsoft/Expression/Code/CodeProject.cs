// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeProject
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Expression.Code
{
  internal class CodeProject : ICodeProject, IDisposable
  {
    private static Dictionary<string, string> assemblyToLocationMap = new Dictionary<string, string>();
    private static string IntellisenseCachePath = "Microsoft\\Expression\\Blend\\IntellisenseCache";
    private List<ProjectAssembly> activeReferences = new List<ProjectAssembly>();
    private Dictionary<IProjectItem, string> activeFiles = new Dictionary<IProjectItem, string>();
    private Dictionary<DocumentReference, CodeProject.PartialCompileDirtyState?> lastDirtyStateBeforePartialCompileIndex = new Dictionary<DocumentReference, CodeProject.PartialCompileDirtyState?>();
    private static bool hasPruned;
    private DotNetProjectResolver projectResolver;
    private IProject project;
    private IAssemblyService assemblyService;
    private ICodeProjectService codeProjectService;
    private bool isInitialized;
    private CodeProject.AsyncManager asyncManager;
    private BuildManager buildManager;
    private IDocumentTypeManager documentTypeManager;
    private CodeProject.OpenDocumentsChangeWatcher documentChangeWatcher;

    public DotNetProjectResolver ProjectResolver
    {
      get
      {
        this.EnsureProjectResolver();
        return this.projectResolver;
      }
    }

    public string FullyQualifiedAssemblyName
    {
      get
      {
        if (this.project.TargetAssembly.RuntimeAssembly != (Assembly) null)
          return this.project.TargetAssembly.RuntimeAssembly.FullName;
        return string.Empty;
      }
    }

    private string GeneratedCodeExtension
    {
      get
      {
        return ".g" + this.project.CodeDocumentType.DefaultFileExtension;
      }
    }

    public CodeProject(IProject project, IAssemblyService assemblyService, ICodeProjectService codeProjectService, BuildManager buildManager, IViewService viewService, IDocumentTypeManager documentTypeManager)
    {
      this.project = project;
      this.assemblyService = assemblyService;
      this.codeProjectService = codeProjectService;
      this.buildManager = buildManager;
      this.documentTypeManager = documentTypeManager;
      this.documentChangeWatcher = new CodeProject.OpenDocumentsChangeWatcher(viewService);
      this.documentChangeWatcher.DocumentClosed += new EventHandler<CodeProject.DocumentChangedEventArgs>(this.DocumentChangeWatcher_DocumentClosed);
      this.documentChangeWatcher.DocumentOpened += new EventHandler<CodeProject.DocumentChangedEventArgs>(this.DocumentChangeWatcher_DocumentOpened);
      this.documentChangeWatcher.InitializeDocuments();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.isInitialized)
      {
        this.isInitialized = false;
        this.UnregisterProjectEvents();
        this.asyncManager.Stop();
      }
      if (this.documentChangeWatcher != null)
      {
        this.documentChangeWatcher.DocumentClosed -= new EventHandler<CodeProject.DocumentChangedEventArgs>(this.DocumentChangeWatcher_DocumentClosed);
        this.documentChangeWatcher.DocumentOpened -= new EventHandler<CodeProject.DocumentChangedEventArgs>(this.DocumentChangeWatcher_DocumentOpened);
        this.documentChangeWatcher.Dispose();
      }
      if (this.projectResolver == null)
        return;
      this.projectResolver.Dispose();
      this.projectResolver = (DotNetProjectResolver) null;
    }

    public void ActivateEditing(DocumentReference documentReference)
    {
      if (!this.isInitialized)
        this.Initialize();
      else
        this.asyncManager.Resume();
      foreach (ProjectAssembly assembly1 in this.activeReferences)
      {
        Assembly assembly2 = assembly1.ReferenceAssembly ?? assembly1.RuntimeAssembly;
        if (assembly2 != (Assembly) null && CodeProject.HasProjectAssemblyChangedLocation(assembly1.FullName, assembly2))
          this.asyncManager.RefreshAssembly(assembly1);
      }
      this.PerformPartialCompileIfNecessary(documentReference);
    }

    public void DeactivateEditing(DocumentReference documentReference)
    {
      if (this.asyncManager == null)
        return;
      this.asyncManager.Pause();
    }

    internal static void EnsureResolverCachePruned(DotNetProjectResolver projectResolver)
    {
      if (CodeProject.hasPruned)
        return;
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.DoWork += new DoWorkEventHandler(CodeProject.IntellisenseCachePruner_DoWork);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CodeProject.IntellisenseCachePruner_RunWorkerCompleted);
      backgroundWorker.RunWorkerAsync((object) projectResolver);
      CodeProject.hasPruned = true;
    }

    private void Initialize()
    {
      if (!(this.project is MSBuildBasedProject))
        return;
      this.EnsureProjectResolver();
      this.asyncManager = new CodeProject.AsyncManager(this);
      foreach (ProjectAssembly assembly in (IEnumerable<ProjectAssembly>) this.project.ReferencedAssemblies)
        this.asyncManager.AddAssembly(assembly);
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.project.Items)
        this.asyncManager.AddFile(projectItem);
      this.asyncManager.Begin();
      this.RegisterProjectEvents();
      this.isInitialized = true;
    }

    private static bool HasProjectAssemblyChangedLocation(string assemblyName, Assembly assembly)
    {
      string b;
      if (CodeProject.assemblyToLocationMap.TryGetValue(assemblyName, out b) && !assembly.IsDynamic)
        return !string.Equals(assembly.Location, b, StringComparison.OrdinalIgnoreCase);
      return false;
    }

    private static void IntellisenseCachePruner_DoWork(object sender, DoWorkEventArgs e)
    {
      DotNetProjectResolver netProjectResolver = (DotNetProjectResolver) e.Argument;
      string str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CodeProject.IntellisenseCachePath);
      netProjectResolver.CachePath = str;
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(netProjectResolver.CachePath))
        return;
      try
      {
        netProjectResolver.PruneCache();
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    private static void IntellisenseCachePruner_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      BackgroundWorker backgroundWorker = sender as BackgroundWorker;
      if (backgroundWorker == null)
        return;
      backgroundWorker.DoWork -= new DoWorkEventHandler(CodeProject.IntellisenseCachePruner_DoWork);
      backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(CodeProject.IntellisenseCachePruner_RunWorkerCompleted);
      backgroundWorker.Dispose();
    }

    private void EnsureProjectResolver()
    {
      if (this.projectResolver != null)
        return;
      this.projectResolver = new DotNetProjectResolver(new ResolveEventHandler(this.DotNetProjectResolver_HostResolveCallback));
      this.projectResolver.CachePath = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CodeProject.IntellisenseCachePath);
    }

    private void AddSourceText(IProjectItem projectItem, string file)
    {
      ISemanticParserServiceProcessor language = this.codeProjectService.GetSyntaxLanguage(file) as ISemanticParserServiceProcessor;
      if (language == null)
        return;
      string code = string.Empty;
      using (TextReader textReader = (TextReader) new StreamReader(file))
        code = textReader.ReadToEnd();
      this.projectResolver.SourceProjectContent.LoadForCode(language, file, code);
      if (this.activeFiles.ContainsKey(projectItem))
        this.activeFiles.Remove(projectItem);
      this.activeFiles.Add(projectItem, file);
    }

    private void RemoveSourceText(IProjectItem projectItem)
    {
      if (!this.activeFiles.ContainsKey(projectItem))
        return;
      this.projectResolver.SourceProjectContent.Clear(this.activeFiles[projectItem]);
      this.activeFiles.Remove(projectItem);
    }

    private void AddAssemblyReference(ProjectAssembly projectAssembly)
    {
      Assembly assembly = projectAssembly.ReferenceAssembly ?? projectAssembly.RuntimeAssembly;
      if (!(assembly != (Assembly) null))
        return;
      try
      {
        if (assembly != (Assembly) null)
        {
          this.projectResolver.AddExternalReference(assembly);
          if (CodeProject.HasProjectAssemblyChangedLocation(projectAssembly.FullName, assembly))
            this.asyncManager.RefreshAssembly(projectAssembly);
        }
      }
      catch (ApplicationException ex)
      {
        return;
      }
      this.activeReferences.Add(projectAssembly);
      if (!(assembly != (Assembly) null) || assembly.IsDynamic)
        return;
      CodeProject.assemblyToLocationMap[projectAssembly.FullName] = assembly.Location;
    }

    private void RemoveAssemblyReference(ProjectAssembly assembly)
    {
      this.activeReferences.Remove(assembly);
      this.projectResolver.RemoveExternalReference(assembly.FullName);
    }

    private void RefreshAssemblyReference(ProjectAssembly assembly)
    {
      Assembly assembly1 = assembly.ReferenceAssembly ?? assembly.RuntimeAssembly;
      if (!assembly1.IsDynamic)
        CodeProject.assemblyToLocationMap[assembly.FullName] = assembly1.Location;
      AssemblyCodeRepository.Refresh(assembly1, this.projectResolver);
    }

    private Assembly DotNetProjectResolver_HostResolveCallback(object sender, ResolveEventArgs e)
    {
      if (string.IsNullOrEmpty(e.Name))
        return (Assembly) null;
      AssemblyName assemblyName = new AssemblyName(e.Name);
      KnownProjectBase knownProjectBase = this.project as KnownProjectBase;
      if (knownProjectBase != null)
      {
        IReferenceAssemblyContext referenceAssemblyContext = knownProjectBase.ReferenceAssemblyContext;
        if (referenceAssemblyContext != null && referenceAssemblyContext.ReferenceAssemblyMode != ReferenceAssemblyMode.None)
        {
          Assembly assembly1 = this.assemblyService.ResolvePlatformAssembly(assemblyName);
          if (assembly1 != (Assembly) null)
          {
            Assembly assembly2 = referenceAssemblyContext.ResolveReferenceAssembly(assemblyName);
            if (assembly2 != (Assembly) null)
              return assembly2;
            return assembly1;
          }
        }
      }
      return this.assemblyService.ResolveAssembly(assemblyName);
    }

    private void AddGeneratedCode(IProjectItem projectItem, string filename, bool isAlreadyGeneratedFilename)
    {
      string str = isAlreadyGeneratedFilename ? this.ChangeToGeneratedExtension(filename) : this.GetGeneratedFileName(filename);
      if (string.IsNullOrEmpty(str) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
        return;
      this.AddSourceText(projectItem, str);
    }

    private void RemoveGeneratedCode(IProjectItem projectItem)
    {
      if (!this.activeFiles.ContainsKey(projectItem) || !this.IsGeneratedFileName(this.activeFiles[projectItem]))
        return;
      this.RemoveSourceText(projectItem);
    }

    private bool IsGeneratedFileName(string fileName)
    {
      return fileName.EndsWith(this.GeneratedCodeExtension, StringComparison.OrdinalIgnoreCase);
    }

    private string GetGeneratedFileName(string sourceName)
    {
      if (string.Compare(Path.GetExtension(sourceName), ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
      {
        MSBuildBasedProject buildBasedProject = this.project as MSBuildBasedProject;
        if (buildBasedProject != null)
        {
          string str = buildBasedProject.GetEvaluatedPropertyValue("IntermediateOutputPath");
          if (!string.IsNullOrEmpty(str))
          {
            if (Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(str))
              str = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(this.project.ProjectRoot.Path, str);
            string withoutExtension = Path.GetFileNameWithoutExtension(sourceName);
            return Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(str, Path.ChangeExtension(withoutExtension, this.GeneratedCodeExtension));
          }
        }
      }
      return (string) null;
    }

    private string ChangeToGeneratedExtension(string sourceName)
    {
      if (string.Compare(Path.GetExtension(sourceName), ".xaml", StringComparison.OrdinalIgnoreCase) == 0)
        return Path.ChangeExtension(sourceName, this.GeneratedCodeExtension);
      return sourceName;
    }

    private void RegisterProjectEvents()
    {
      if (this.project == null)
        return;
      this.project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
      this.project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
      this.project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
      this.project.ItemDeleted += new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
      this.project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
      this.project.ReferencedAssemblies.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanged);
    }

    private void UnregisterProjectEvents()
    {
      if (this.project == null)
        return;
      this.project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
      this.project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
      this.project.ItemRenamed -= new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
      this.project.ItemDeleted -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
      this.project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
      this.project.ReferencedAssemblies.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanged);
    }

    private void AssemblyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this.asyncManager.AddAssembly((ProjectAssembly) enumerator1.Current);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator1 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
              this.asyncManager.RemoveAssembly((ProjectAssembly) enumerator2.Current);
            break;
          }
          finally
          {
            IDisposable disposable = enumerator2 as IDisposable;
            if (disposable != null)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Reset:
          List<ProjectAssembly> list = new List<ProjectAssembly>((IEnumerable<ProjectAssembly>) this.activeReferences);
          foreach (ProjectAssembly assembly in Enumerable.Intersect<ProjectAssembly>((IEnumerable<ProjectAssembly>) this.project.ReferencedAssemblies, (IEnumerable<ProjectAssembly>) list))
            this.asyncManager.RefreshAssembly(assembly);
          using (IEnumerator<ProjectAssembly> enumerator3 = Enumerable.Except<ProjectAssembly>((IEnumerable<ProjectAssembly>) this.project.ReferencedAssemblies, (IEnumerable<ProjectAssembly>) list).GetEnumerator())
          {
            while (enumerator3.MoveNext())
              this.asyncManager.AddAssembly(enumerator3.Current);
            break;
          }
      }
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.project.Items)
        this.asyncManager.UpdateGeneratedCode(projectItem, projectItem.DocumentReference.Path);
    }

    private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
    {
      if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(e.ProjectItem.DocumentReference.Path))
        return;
      this.asyncManager.AddFile(e.ProjectItem);
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      this.asyncManager.RemoveFile(e.ProjectItem);
    }

    private void Project_ItemDeleted(object sender, ProjectItemEventArgs e)
    {
      this.asyncManager.RemoveFile(e.ProjectItem);
    }

    private void Project_ItemChanged(object sender, ProjectItemEventArgs e)
    {
      this.asyncManager.RemoveFile(e.ProjectItem);
      this.asyncManager.AddFile(e.ProjectItem);
    }

    private void Project_ItemRenamed(object sender, ProjectItemRenamedEventArgs e)
    {
      this.asyncManager.RemoveFile(e.ProjectItem);
      this.asyncManager.AddFile(e.ProjectItem);
    }

    private void DocumentChangeWatcher_DocumentClosed(object sender, CodeProject.DocumentChangedEventArgs e)
    {
      if (e.Document == null || !this.lastDirtyStateBeforePartialCompileIndex.ContainsKey(e.Document.DocumentReference))
        return;
      this.lastDirtyStateBeforePartialCompileIndex.Remove(e.Document.DocumentReference);
    }

    private void DocumentChangeWatcher_DocumentOpened(object sender, CodeProject.DocumentChangedEventArgs e)
    {
      if (e.Document == null)
        return;
      this.lastDirtyStateBeforePartialCompileIndex[e.Document.DocumentReference] = new CodeProject.PartialCompileDirtyState?();
    }

    private void PerformPartialCompileIfNecessary(DocumentReference documentReference)
    {
      IProjectItem xamlProjectItem = this.FindXamlProjectItem(documentReference);
      if (xamlProjectItem == null || this.documentTypeManager == null || (xamlProjectItem.DocumentType == this.documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.LimitedXaml] || !this.IsPartialRecompileNecessary(xamlProjectItem)))
        return;
      this.PerformPartialCompile(xamlProjectItem);
    }

    private IProjectItem FindXamlProjectItem(DocumentReference documentReference)
    {
      IProjectItem projectItem = this.project.FindItem(documentReference);
      if (projectItem != null && projectItem.IsCodeBehindItem)
        return projectItem.Parent;
      return (IProjectItem) null;
    }

    private void PerformPartialCompile(IProjectItem xamlProjectItem)
    {
      if (this.lastDirtyStateBeforePartialCompileIndex.ContainsKey(xamlProjectItem.DocumentReference) && xamlProjectItem.Document != null)
      {
        CodeProject.PartialCompileDirtyState compileDirtyState = new CodeProject.PartialCompileDirtyState()
        {
          DirtyState = xamlProjectItem.Document.IsDirty,
          PartialCompileTime = DateTime.UtcNow
        };
        this.lastDirtyStateBeforePartialCompileIndex[xamlProjectItem.DocumentReference] = new CodeProject.PartialCompileDirtyState?(compileDirtyState);
      }
      string fileName = DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(this.project.DocumentReference.Path)).GetRelativePath(xamlProjectItem.DocumentReference);
      foreach (char oldChar in Microsoft.Expression.Framework.Documents.PathHelper.GetDirectorySeparatorCharacters())
        fileName = fileName.Replace(oldChar, '_');
      string documentText = this.GetDocumentText(xamlProjectItem);
      if (documentText == null)
        return;
      PartialXamlBuilder partialXamlBuilder = new PartialXamlBuilder(this.project, xamlProjectItem, documentText, fileName, this.buildManager);
      partialXamlBuilder.BuildFinished += new EventHandler<PartialXamlBuildFinishedEventArgs>(this.XamlBuilder_BuildFinished);
      partialXamlBuilder.Build();
    }

    private bool IsPartialRecompileNecessary(IProjectItem projectItem)
    {
      if (this.lastDirtyStateBeforePartialCompileIndex.ContainsKey(projectItem.DocumentReference) && this.lastDirtyStateBeforePartialCompileIndex[projectItem.DocumentReference].HasValue)
      {
        if (projectItem.Document.IsDirty || projectItem.Document.IsDirty != this.lastDirtyStateBeforePartialCompileIndex[projectItem.DocumentReference].Value.DirtyState)
          return true;
        FileInfo fileInfo = new FileInfo(projectItem.DocumentReference.Path);
        if (fileInfo.Exists)
          return fileInfo.LastWriteTimeUtc > this.lastDirtyStateBeforePartialCompileIndex[projectItem.DocumentReference].Value.PartialCompileTime;
        return false;
      }
      if (this.IsItemNewerThanBuiltVersion(projectItem))
        return true;
      if (projectItem.Document != null)
        return projectItem.Document.IsDirty;
      return false;
    }

    private bool IsItemNewerThanBuiltVersion(IProjectItem projectItem)
    {
      string generatedFileName = this.GetGeneratedFileName(projectItem.DocumentReference.Path);
      if (generatedFileName != null)
      {
        FileInfo fileInfo1 = new FileInfo(generatedFileName);
        FileInfo fileInfo2 = new FileInfo(projectItem.DocumentReference.Path);
        if (!fileInfo1.Exists || fileInfo2.Exists && fileInfo2.LastWriteTimeUtc > fileInfo1.LastWriteTimeUtc)
          return true;
      }
      return false;
    }

    private string GetDocumentText(IProjectItem projectItem)
    {
      string str = (string) null;
      bool flag = false;
      if (projectItem.Document == null)
        flag = projectItem.OpenDocument(false, true);
      if (projectItem.Document != null)
      {
        IReadableTextBuffer readableTextBuffer = projectItem.Document as IReadableTextBuffer;
        str = readableTextBuffer.GetText(0, readableTextBuffer.Length);
      }
      if (flag)
        projectItem.CloseDocument();
      return str;
    }

    private void XamlBuilder_BuildFinished(object sender, PartialXamlBuildFinishedEventArgs e)
    {
      if (e.BuildResult == BuildResult.Succeeded)
        this.asyncManager.UpdateGeneratedCodeFromPartialCompile(e.ProjectItem, e.XamlFileLocation);
      else if (e.BuildResult == BuildResult.Failed)
        this.asyncManager.InvalidateGeneratedCodeFromPartialCompile(e.ProjectItem);
      PartialXamlBuilder partialXamlBuilder = (PartialXamlBuilder) sender;
      if (partialXamlBuilder == null)
        return;
      partialXamlBuilder.BuildFinished -= new EventHandler<PartialXamlBuildFinishedEventArgs>(this.XamlBuilder_BuildFinished);
    }

    private class DocumentChangedEventArgs : EventArgs
    {
      private IDocument document;

      public IDocument Document
      {
        get
        {
          return this.document;
        }
      }

      public DocumentChangedEventArgs(IDocument document)
      {
        this.document = document;
      }
    }

    private struct PartialCompileDirtyState
    {
      public bool DirtyState { get; set; }

      public DateTime PartialCompileTime { get; set; }
    }

    private class OpenDocumentsChangeWatcher : IDisposable
    {
      private List<DocumentView> openViews = new List<DocumentView>();
      private IViewService viewService;

      public event EventHandler<CodeProject.DocumentChangedEventArgs> DocumentOpened;

      public event EventHandler<CodeProject.DocumentChangedEventArgs> DocumentClosed;

      public OpenDocumentsChangeWatcher(IViewService viewService)
      {
        this.viewService = viewService;
        this.viewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
        this.viewService.ViewOpened += new ViewEventHandler(this.ViewService_ViewOpened);
      }

      public void InitializeDocuments()
      {
        foreach (IView view in (IEnumerable<IView>) this.viewService.Views)
          this.AddView(view);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool isDisposing)
      {
        if (!isDisposing || this.viewService == null)
          return;
        this.viewService.ViewOpened -= new ViewEventHandler(this.ViewService_ViewOpened);
        this.viewService.ViewClosed -= new ViewEventHandler(this.ViewService_ViewClosed);
      }

      private void ViewService_ViewOpened(object sender, ViewEventArgs e)
      {
        this.AddView(e.View);
      }

      private void ViewService_ViewClosed(object sender, ViewEventArgs e)
      {
        this.RemoveView(e.View);
      }

      private void AddView(IView view)
      {
        DocumentView documentView = view as DocumentView;
        if (documentView == null)
          return;
        this.openViews.Add(documentView);
        if (this.DocumentOpened == null)
          return;
        this.DocumentOpened((object) this, new CodeProject.DocumentChangedEventArgs(documentView.Document));
      }

      private void RemoveView(IView view)
      {
        DocumentView documentView = view as DocumentView;
        if (!this.openViews.Contains(documentView))
          return;
        this.openViews.Remove(documentView);
        if (this.DocumentClosed == null)
          return;
        this.DocumentClosed((object) this, new CodeProject.DocumentChangedEventArgs(documentView.Document));
      }
    }

    private class AsyncManager
    {
      private AsyncQueueProcess actions = new AsyncQueueProcess((IAsyncMechanism) new BackgroundWorkerAsyncMechanism(BackgroundWorkMode.ThreadHog));
      private bool startProcessing;
      private CodeProject codeProject;

      public AsyncManager(CodeProject codeProject)
      {
        this.codeProject = codeProject;
      }

      public void Begin()
      {
        this.startProcessing = true;
        this.actions.Begin();
      }

      public void Stop()
      {
        this.startProcessing = false;
        this.actions.Kill();
      }

      public void Pause()
      {
        if (this.actions.IsPaused)
          return;
        this.actions.Pause();
      }

      public void Resume()
      {
        if (!this.actions.IsPaused)
          return;
        this.actions.Resume();
      }

      public void AddAssembly(ProjectAssembly assembly)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) => this.codeProject.AddAssemblyReference(assembly))), this.startProcessing);
      }

      public void RemoveAssembly(ProjectAssembly assembly)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) => this.codeProject.RemoveAssemblyReference(assembly))), this.startProcessing);
      }

      public void RefreshAssembly(ProjectAssembly assembly)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) => this.codeProject.RefreshAssemblyReference(assembly))), this.startProcessing);
      }

      public void AddFile(IProjectItem projectItem)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) =>
        {
          if (projectItem.DocumentType is CodeDocumentType && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(projectItem.DocumentReference.Path))
            this.codeProject.AddSourceText(projectItem, projectItem.DocumentReference.Path);
          this.codeProject.AddGeneratedCode(projectItem, projectItem.DocumentReference.Path, false);
        })), this.startProcessing);
      }

      public void RemoveFile(IProjectItem projectItem)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) =>
        {
          if (projectItem.DocumentType is CodeDocumentType)
            this.codeProject.RemoveSourceText(projectItem);
          this.codeProject.RemoveGeneratedCode(projectItem);
        })), this.startProcessing);
      }

      public void UpdateGeneratedCode(IProjectItem item, string filename)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) =>
        {
          if (this.codeProject.IsGeneratedFileName(filename))
            return;
          this.codeProject.RemoveGeneratedCode(item);
          this.codeProject.AddGeneratedCode(item, filename, false);
        })), this.startProcessing);
      }

      public void UpdateGeneratedCodeFromPartialCompile(IProjectItem item, string fileName)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) =>
        {
          if (this.codeProject.IsGeneratedFileName(fileName))
            return;
          this.codeProject.RemoveGeneratedCode(item);
          this.codeProject.AddGeneratedCode(item, fileName, true);
          PartialXamlBuilder.CleanPartialCompileDirectory();
        })), this.startProcessing);
      }

      public void InvalidateGeneratedCodeFromPartialCompile(IProjectItem item)
      {
        this.actions.Add((AsyncProcess) new DelegateAsyncProcess((Action<object, DoWorkEventArgs>) ((sender, args) =>
        {
          this.codeProject.RemoveGeneratedCode(item);
          PartialXamlBuilder.CleanPartialCompileDirectory();
        })), this.startProcessing);
      }
    }
  }
}

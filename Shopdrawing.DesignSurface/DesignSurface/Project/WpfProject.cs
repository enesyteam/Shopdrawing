// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.WpfProject
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal sealed class WpfProject : XamlProject
  {
    private bool initializing = true;
    private IProjectItem startupScene;

    private IProjectItem ApplicationDefinitionItem
    {
      get
      {
        if (this.IsControlLibrary)
          return (IProjectItem) null;
        IProjectItem projectItem1 = (IProjectItem) null;
        foreach (IProjectItem projectItem2 in (IEnumerable<IProjectItem>) this.Items)
        {
          if (projectItem2.Properties["BuildAction"] == "ApplicationDefinition")
          {
            projectItem1 = projectItem2;
            break;
          }
        }
        return projectItem1;
      }
    }

    public override ICollection<string> TemplateProjectSubtypes
    {
      get
      {
        ICollection<string> templateProjectSubtypes = base.TemplateProjectSubtypes;
        templateProjectSubtypes.Add("WPF");
        return templateProjectSubtypes;
      }
    }

    public override string FullTargetPath
    {
      get
      {
        string fullTargetPath = base.FullTargetPath;
        if (string.IsNullOrEmpty(fullTargetPath) || (!TypeHelper.ConvertType<bool>((object) this.ProjectStore.GetProperty("HostInBrowser")) || TypeHelper.ConvertType<bool>((object) this.ProjectStore.GetProperty("Install"))))
          return fullTargetPath;
        return Path.ChangeExtension(fullTargetPath, ".xbap");
      }
    }

    public override IProjectItem StartupItem
    {
      get
      {
        if (this.IsControlLibrary)
          return (IProjectItem) null;
        return this.startupScene;
      }
      set
      {
        if (this.IsControlLibrary || this.startupScene == value)
          return;
        IProjectItem oldProjectItem = this.startupScene;
        this.startupScene = value;
        this.OnStartupSceneChanged(new ProjectItemChangedEventArgs(oldProjectItem, this.startupScene));
        if (this.initializing)
          return;
        this.UpdateStartupUri(value);
      }
    }

    private WpfProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
      : base(projectStore, codeDocumentType, projectType, serviceProvider)
    {
    }

    public new static IProject Create(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
    {
      return (IProject) KnownProjectBase.TryCreate((Func<KnownProjectBase>) (() => (KnownProjectBase) new WpfProject(projectStore, codeDocumentType, projectType, serviceProvider)));
    }

    protected override bool Initialize()
    {
      if (!base.Initialize())
        return false;
      this.BuildTaskInfoPopulator = this.CreateLargeImagePopulator();
      IProjectItem applicationDefinitionItem = this.ApplicationDefinitionItem;
      if (applicationDefinitionItem != null)
      {
        IProjectItem itemRelative = this.FindItemRelative(this.GetStartupUri(applicationDefinitionItem));
        if (itemRelative != null)
          this.startupScene = itemRelative;
      }
      this.initializing = false;
      return true;
    }

    private BuildTaskInfoPopulator CreateLargeImagePopulator()
    {
      return XamlProject.CreateLargeImagePopulator(this.IsControlLibrary ? StringTable.ImageScalabilityWarningWpfControlLibrary : StringTable.ImageScalabilityWarningWpfApplication, new BuildTaskInfo("None", (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "CopyToOutputDirectory",
          "PreserveNewest"
        }
      }), this.Services);
    }

    protected override void RenameProjectItemInternal(IProjectItem projectItem, DocumentReference newDocumentReference)
    {
      base.RenameProjectItemInternal(projectItem, newDocumentReference);
      if (!projectItem.Equals((object) this.StartupItem))
        return;
      this.UpdateStartupUri(this.StartupItem);
    }

    private void UpdateStartupUri(IProjectItem startupProjectItem)
    {
      IProjectItem applicationDefinitionItem = this.ApplicationDefinitionItem;
      if (applicationDefinitionItem == null)
        return;
      string startupUri = this.GetStartupUri(applicationDefinitionItem);
      if ((startupProjectItem != null || startupUri == null) && this.FindItemRelative(startupUri) == startupProjectItem)
        return;
      SceneDocument sceneDocument = applicationDefinitionItem.Document as SceneDocument;
      if (sceneDocument == null)
        return;
      Uri uri = (Uri) null;
      if (startupProjectItem != null)
      {
        string resourceReference = startupProjectItem.GetResourceReference(applicationDefinitionItem.DocumentReference);
        if (!string.IsNullOrEmpty(resourceReference))
          uri = new Uri(resourceReference, UriKind.Relative);
      }
      using (SceneEditTransaction editTransaction = sceneDocument.CreateEditTransaction("Set StartupUri", true))
      {
        sceneDocument.StartupUri = uri;
        editTransaction.Commit();
      }
      sceneDocument.SourceChanged();
    }

    private string GetStartupUri(IProjectItem applicationDefinitionProjectItem)
    {
      Uri uri = (Uri) null;
      if (!applicationDefinitionProjectItem.IsOpen)
        applicationDefinitionProjectItem.OpenDocument(this.IsReadOnly, !ProjectBase.IsReloadPromptEnabled());
      if (applicationDefinitionProjectItem.IsOpen)
      {
        SceneDocument sceneDocument = applicationDefinitionProjectItem.Document as SceneDocument;
        if (sceneDocument != null)
          uri = sceneDocument.StartupUri;
      }
      if (!(uri != (Uri) null))
        return (string) null;
      return uri.OriginalString;
    }

    private IProjectItem FindItemRelative(string projectRelativeItemName)
    {
      IProjectItem projectItem = (IProjectItem) null;
      if (projectRelativeItemName != null)
        projectItem = this.FindItem(DocumentReference.Create(Path.Combine(this.ProjectRoot.Path, projectRelativeItemName)));
      return projectItem;
    }

    public override bool IsValidStartupItem(IProjectItem projectItem)
    {
      if (this.IsControlLibrary)
        return base.IsValidStartupItem(projectItem);
      ProjectXamlContext projectContext = ProjectXamlContext.GetProjectContext((IProject) this);
      if (projectItem != null && projectContext != null)
      {
        IProjectDocument document = projectContext.GetDocument(DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
        if (document != null && document.DocumentType == ProjectDocumentType.Page)
          return true;
      }
      return false;
    }

    protected override void ProcessNewlyAddedProjectItem(IProjectItem projectItem)
    {
      base.ProcessNewlyAddedProjectItem(projectItem);
      if (this.startupScene != null || !this.IsValidStartupItem(projectItem) || this.IsControlLibrary)
        return;
      this.StartupItem = projectItem;
    }

    protected override void RemoveProjectItem(IProjectItem projectItem, bool deleteFiles)
    {
      base.RemoveProjectItem(projectItem, deleteFiles);
      if (this.StartupItem != null || this.IsControlLibrary)
        return;
      foreach (IProjectItem projectItem1 in (IEnumerable<IProjectItem>) this.Items)
      {
        if (this.IsValidStartupItem(projectItem1))
        {
          this.StartupItem = projectItem1;
          break;
        }
      }
    }

    public override T GetCapability<T>(string name)
    {
      switch (name)
      {
        case "CanHaveStartupItem":
          if (this.IsControlLibrary)
            return default (T);
          return TypeHelper.ConvertType<T>((object) true);
        default:
          return base.GetCapability<T>(name);
      }
    }
  }
}

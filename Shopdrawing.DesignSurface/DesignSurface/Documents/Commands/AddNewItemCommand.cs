// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.Commands.AddNewItemCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Commands;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignSurface.Documents.Commands
{
  internal sealed class AddNewItemCommand : Command, IProjectCommand, ICommand
  {
    private IList<string> typeFilter;
    private DesignerContext designerContext;
    private bool allowOverwrite;

    public string DisplayName
    {
      get
      {
        return string.Empty;
      }
    }

    public IServiceProvider Services { get; private set; }

    public override bool IsEnabled
    {
      get
      {
        IProject project = ProjectCommandExtensions.SelectedProjectOrNull((IProjectCommand) this);
        if (base.IsEnabled)
          return this.EnabledForProject(project);
        return false;
      }
    }

    public override bool IsAvailable
    {
      get
      {
        IProject project = ProjectCommandExtensions.SelectedProjectOrNull((IProjectCommand) this);
        if (base.IsAvailable)
          return !(project is IWebsiteProject);
        return false;
      }
    }

    public AddNewItemCommand(DesignerContext designerContext, params string[] typeFilter)
      : this(designerContext, true, typeFilter)
    {
      this.Services = (IServiceProvider) designerContext.Services;
    }

    public AddNewItemCommand(DesignerContext designerContext, bool allowOverwrite, params string[] typeFilter)
    {
      this.allowOverwrite = allowOverwrite;
      this.designerContext = designerContext;
      if (typeFilter == null)
        return;
      this.typeFilter = (IList<string>) new List<string>((IEnumerable<string>) typeFilter);
    }

    private bool EnabledForProject(IProject project)
    {
      if (project != null && project.TargetFramework != (FrameworkName) null && !(project is IWebsiteProject))
        return !BuildManager.Building;
      return false;
    }

    public override void Execute()
    {
      IProject project = EnumerableExtensions.SingleOrNull<IProject>(this.designerContext.ProjectManager.ItemSelectionSet.SelectedProjects);
      if (project == null)
        return;
      this.ExecuteWithProject(project);
    }

    internal void ExecuteWithProject(IProject project)
    {
      if (!this.EnabledForProject(project))
        return;
      CreateProjectItemDialog projectItemDialog = new CreateProjectItemDialog(project, this.designerContext, this.typeFilter);
      projectItemDialog.InitializeDialog();
      bool? nullable = projectItemDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) == 0)
        return;
      try
      {
        this.ExecuteInternal(project, projectItemDialog.ItemTemplate, projectItemDialog.FileName);
      }
      catch (Exception ex)
      {
        if (ex is NotSupportedException || ErrorHandling.ShouldHandleExceptions(ex))
          this.DisplayCreationFailedExceptionMessage(ex, projectItemDialog.FileName);
        else
          throw;
      }
    }

    internal IEnumerable<IProjectItem> ExecuteInternal(IProject project, IProjectItemTemplate itemTemplate, string userSpecifiedFileName)
    {
      List<IProjectItem> itemsToOpen = (List<IProjectItem>) null;
      TemplateItemHelper templateItemHelper = new TemplateItemHelper(project, (IList<string>) null, (IServiceProvider) this.designerContext.Services);
      CreationOptions creationOptions = CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath;
      if (!this.allowOverwrite)
        creationOptions |= CreationOptions.DoNotAllowOverwrites;
      IEnumerable<IProjectItem> enumerable = templateItemHelper.AddProjectItemsForTemplateItem(itemTemplate, userSpecifiedFileName, this.designerContext.ProjectManager.TargetFolderForProject(project), creationOptions, out itemsToOpen);
      if (enumerable != null)
      {
        foreach (IProjectItem projectItem in enumerable)
        {
          if (projectItem.DocumentType is AssemblyReferenceDocumentType)
            project.AddAssemblyReference(projectItem.DocumentReference.Path, false);
        }
      }
      if (itemsToOpen != null)
      {
        foreach (IProjectItem projectItem in itemsToOpen)
          projectItem.OpenView(true);
      }
      return enumerable;
    }

    private void DisplayCreationFailedExceptionMessage(Exception e, string thingTried)
    {
      this.designerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ProjectNewFileErrorDialogMessage, new object[2]
      {
        (object) thingTried,
        (object) e.Message
      }));
    }
  }
}

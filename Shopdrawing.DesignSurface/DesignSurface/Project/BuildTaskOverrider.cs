// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.BuildTaskOverrider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal class BuildTaskOverrider : IBuildTaskOverrider
  {
    private const string BuildTaskOverrideSUOKey = "OverrideBuildTaskForLargeImages";
    private Func<string> promptMessageFormatter;
    private Func<string> doNotPromptAgainMessageFormatter;
    private string promptAutomationId;
    private IServiceProvider serviceProvider;
    private BuildTaskInfo overrideBuildTask;

    public BuildTaskOverrider(IServiceProvider serviceProvider, Func<string> promptMessageFormatter, Func<string> doNotPromptAgainMessageFormatter, string automationId, BuildTaskInfo overrideBuildTask)
    {
      this.serviceProvider = serviceProvider;
      this.promptMessageFormatter = promptMessageFormatter;
      this.doNotPromptAgainMessageFormatter = doNotPromptAgainMessageFormatter;
      this.promptAutomationId = automationId;
      this.overrideBuildTask = overrideBuildTask;
    }

    public ProjectDialog.ProjectDialogResult ShouldOverrideDefaultBuildAction(IProject project, IList<DocumentCreationInfo> candidateItemsToOverride)
    {
      if (this.ShouldPromptForBuildActionChange(project))
        return this.PromptForOverrideBuildAction(candidateItemsToOverride);
      return ProjectDialog.ProjectDialogResult.Ok;
    }

    private bool ShouldPromptForBuildActionChange(IProject project)
    {
      return !this.IsSketchflowProject(project);
    }

    private bool IsSketchflowProject(IProject project)
    {
      return project.GetCapability<bool>("ExpressionBlendPrototypingEnabled");
    }

    private ProjectDialog.ProjectDialogResult PromptForOverrideBuildAction(IList<DocumentCreationInfo> candidateItems)
    {
      ProjectDialog.ProjectDialogResult persistedResult = this.GetPersistedResult();
      if (persistedResult != ProjectDialog.ProjectDialogResult.Cancel)
        return persistedResult;
      return this.ShowOverridePrompt(Enumerable.ToList<DocumentReference>(Enumerable.Select<DocumentCreationInfo, DocumentReference>((IEnumerable<DocumentCreationInfo>) candidateItems, (Func<DocumentCreationInfo, DocumentReference>) (itemPath => DocumentReference.Create(itemPath.TargetPath)))));
    }

    protected virtual ProjectDialog.ProjectDialogResult ShowOverridePrompt(List<DocumentReference> itemPaths)
    {
      ItemsListDialog itemsListDialog = new ItemsListDialog(this.serviceProvider, this.promptMessageFormatter(), this.promptAutomationId, this.doNotPromptAgainMessageFormatter(), (IEnumerable<DocumentReference>) itemPaths);
      itemsListDialog.InitializeDialog();
      itemsListDialog.ShowDialog();
      if (itemsListDialog.CheckBoxResult)
        this.SetPersistedResult(itemsListDialog.Result);
      return itemsListDialog.Result;
    }

    protected virtual ProjectDialog.ProjectDialogResult GetPersistedResult()
    {
      object property = this.GetSettingsObject().GetProperty("OverrideBuildTaskForLargeImages", (object) ProjectDialog.ProjectDialogResult.Cancel);
      if (property is ProjectDialog.ProjectDialogResult)
        return (ProjectDialog.ProjectDialogResult) property;
      return ProjectDialog.ProjectDialogResult.Cancel;
    }

    private void SetPersistedResult(ProjectDialog.ProjectDialogResult result)
    {
      this.GetSettingsObject().SetProperty("OverrideBuildTaskForLargeImages", (object) result);
    }

    private IConfigurationObject GetSettingsObject()
    {
      return ServiceExtensions.ProjectManager(this.serviceProvider).CurrentSolution.SolutionSettingsManager.SolutionSettings;
    }

    public IEnumerable<DocumentCreationInfo> OverrideBuildTaskFor(IEnumerable<DocumentCreationInfo> creationInfos, IList<DocumentCreationInfo> itemsToOverride)
    {
      return Enumerable.Select<DocumentCreationInfo, DocumentCreationInfo>(creationInfos, (Func<DocumentCreationInfo, DocumentCreationInfo>) (info =>
      {
        if (itemsToOverride.Contains(info))
          info.BuildTaskInfo = this.overrideBuildTask;
        return info;
      }));
    }
  }
}

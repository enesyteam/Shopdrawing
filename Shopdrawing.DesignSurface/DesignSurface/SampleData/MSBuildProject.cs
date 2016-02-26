// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.MSBuildProject
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  [Prototype]
  public class MSBuildProject : IMSBuildProject
  {
    private IProject project;

    public string CodeLanguage
    {
      get
      {
        return this.project.CodeDocumentType.Name;
      }
    }

    public IProject Project
    {
      get
      {
        return this.project;
      }
    }

    public MSBuildProject(IProject project)
    {
      this.project = project;
    }

    public bool IsSafeIdentifier(string identifier)
    {
      return CodeGenerator.MakeSafeIdentifier(this.project.CodeDocumentType, identifier, true) == identifier;
    }

    public bool UpdateProject(IEnumerable<ProjectItemAction> actions)
    {
      if (!EnumerableExtensions.CountIsMoreThan<ProjectItemAction>(actions, 0))
        return true;
      IEnumerable<ProjectItemAction> enumerable1 = Enumerable.Where<ProjectItemAction>(actions, (Func<ProjectItemAction, bool>) (action =>
      {
        if (action.Operation != ProjectItemOperation.Add)
          return action.Operation == ProjectItemOperation.Update;
        return true;
      }));
      IEnumerable<DocumentCreationInfo> creationInfo = Enumerable.Select<ProjectItemAction, DocumentCreationInfo>(enumerable1, (Func<ProjectItemAction, DocumentCreationInfo>) (item => new DocumentCreationInfo()
      {
        TargetFolder = Path.GetDirectoryName(item.ProjectItemPath),
        SourcePath = item.SourceFilePath,
        CreationOptions = CreationOptions.SilentlyOverwrite | CreationOptions.SilentlyOverwriteReadOnly | CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath | CreationOptions.AlwaysUseDefaultBuildTask
      }));
      IEnumerable<IProjectItem> source = Enumerable.Where<IProjectItem>(Enumerable.Select<ProjectItemAction, IProjectItem>(Enumerable.Where<ProjectItemAction>(actions, (Func<ProjectItemAction, bool>) (action => action.Operation == ProjectItemOperation.Delete)), (Func<ProjectItemAction, IProjectItem>) (action => this.project.FindItem(DocumentReference.Create(action.ProjectItemPath)))), (Func<IProjectItem, bool>) (item => item != null));
      IEnumerable<IProjectItem> completedActions = this.project.AddItems(creationInfo);
      if (!MSBuildProject.UpdateCompletedActions(enumerable1, completedActions) || !this.project.RemoveItems(true, Enumerable.ToArray<IProjectItem>(source)))
        return false;
      IEnumerable<ProjectItemAction> enumerable2 = Enumerable.Where<ProjectItemAction>(actions, (Func<ProjectItemAction, bool>) (action => action.Operation != ProjectItemOperation.Delete));
      bool flag1 = false;
      foreach (ProjectItemAction projectItemAction in enumerable2)
      {
        IProjectItem projectItem = this.project.FindItem(DocumentReference.Create(projectItemAction.ProjectItemPath));
        if (projectItem != null)
        {
          string buildTask = projectItem.DocumentType.DefaultBuildTaskInfo.BuildTask;
          bool flag2 = projectItem.Properties["BuildAction"] == buildTask;
          if (projectItemAction.IsEnabled == flag2)
            projectItemAction.OnActionCompleted();
          else if (buildTask == SampleDataSet.DesignTimeBuildType)
          {
            projectItemAction.OnActionCompleted();
          }
          else
          {
            if (!flag1)
            {
              flag1 = true;
              if (!((KnownProjectBase) this.project).AttemptToMakeProjectWritable())
                return false;
            }
            projectItem.Properties["BuildAction"] = !projectItemAction.IsEnabled ? SampleDataSet.DesignTimeBuildType : buildTask;
            projectItemAction.OnActionCompleted();
          }
        }
      }
      return true;
    }

    private static bool UpdateCompletedActions(IEnumerable<ProjectItemAction> actions, IEnumerable<IProjectItem> completedActions)
    {
      if (Enumerable.Count<ProjectItemAction>(actions) == Enumerable.Count<IProjectItem>(completedActions))
      {
        foreach (ProjectItemAction projectItemAction in actions)
          projectItemAction.OnActionCompleted();
        return true;
      }
      using (IEnumerator<IProjectItem> enumerator = completedActions.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          IProjectItem projectItem = enumerator.Current;
          ProjectItemAction projectItemAction = Enumerable.FirstOrDefault<ProjectItemAction>(actions, (Func<ProjectItemAction, bool>) (a => Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(a.ProjectItemPath, projectItem.DocumentReference.Path)));
          if (projectItemAction != null)
            projectItemAction.OnActionCompleted();
        }
      }
      return false;
    }
  }
}

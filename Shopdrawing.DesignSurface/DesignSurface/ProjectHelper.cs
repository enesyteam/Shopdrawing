// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ProjectHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface
{
  internal static class ProjectHelper
  {
    internal static IProject GetProject(IProjectManager projectManager, IDocumentContext projectItemDocumentContext)
    {
      if (projectItemDocumentContext != null && DocumentReferenceLocator.GetDocumentReference(projectItemDocumentContext) != (DocumentReference) null && projectManager.CurrentSolution != null)
      {
        DocumentReference documentReference = DocumentReferenceLocator.GetDocumentReference(projectItemDocumentContext);
        foreach (IProject project in projectManager.CurrentSolution.Projects)
        {
          if (project.FindItem(documentReference) != null)
            return project;
        }
      }
      return (IProject) null;
    }

    internal static bool DoesProjectReferencesContainTarget(IProject source, IProjectContext target)
    {
      return Enumerable.Any<IProject>(source.ReferencedProjects, (Func<IProject, bool>) (project => ProjectXamlContext.GetProjectContext(project) == target));
    }

    internal static bool DoesProjectReferenceHierarchyContainTarget(IProject source, IProjectContext target)
    {
      IndexedHashSet<IProject> projects = new IndexedHashSet<IProject>();
      ProjectHelper.BuildProjectReferences(source, projects);
      return Enumerable.Any<IProject>((IEnumerable<IProject>) projects, (Func<IProject, bool>) (project => ProjectXamlContext.GetProjectContext(project) == target));
    }

    private static void BuildProjectReferences(IProject source, IndexedHashSet<IProject> projects)
    {
      projects.Add(source);
      foreach (IProject source1 in source.ReferencedProjects)
      {
        if (!projects.Contains(source1))
          ProjectHelper.BuildProjectReferences(source1, projects);
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.WpfProjectType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;
using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignSurface.Project
{
  public class WpfProjectType : WindowsExecutableProjectType
  {
    public override string Identifier
    {
      get
      {
        return ProjectTypeNamesHelper.Application;
      }
    }

    public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
    {
      return (INamedProject) WpfProject.Create(projectStore, codeDocumentType, (IProjectType) this, serviceProvider);
    }

    public override bool IsValidTypeForProject(IProjectStore projectStore)
    {
      FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
      if (targetFrameworkName == (FrameworkName) null || targetFrameworkName.Identifier != ".NETFramework" || !(targetFrameworkName.Version == CommonVersions.Version3_5) && !(targetFrameworkName.Version == CommonVersions.Version4_0) || !base.IsValidTypeForProject(projectStore))
        return false;
      return ProjectStoreHelper.UsesWpf(projectStore);
    }

    public override ReferenceAssemblyMode GetReferenceAssemblyMode(IProject project)
    {
      return project.TargetFramework.Version == new Version(4, 0) ? ReferenceAssemblyMode.None : ReferenceAssemblyMode.TargetFramework;
    }
  }
}

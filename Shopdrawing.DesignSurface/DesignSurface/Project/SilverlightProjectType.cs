// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.SilverlightProjectType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;
using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignSurface.Project
{
  public class SilverlightProjectType : MSBuildBasedProjectType
  {
    private static FrameworkName MobileFrameworkName = new FrameworkName("Silverlight", new Version(4, 0), "WindowsPhone");
    private IServiceProvider serviceProvider;

    public override string Identifier
    {
      get
      {
        return ProjectTypeNamesHelper.Silverlight;
      }
    }

    public SilverlightProjectType(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public override bool IsValidTypeForProject(IProjectStore projectStore)
    {
      FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
      if (targetFrameworkName == (FrameworkName) null || targetFrameworkName.Identifier != "Silverlight" || !(targetFrameworkName.Version == CommonVersions.Version3_0) && !(targetFrameworkName.Version == CommonVersions.Version4_0))
        return false;
      string property = projectStore.GetProperty("OutputType");
      if (string.IsNullOrEmpty(property) || !property.Equals("Library", StringComparison.OrdinalIgnoreCase))
        return false;
      return base.IsValidTypeForProject(projectStore);
    }

    public override IProjectCreateError CanCreateProject(IProjectStore projectStore)
    {
      if (!new SilverlightChecker(this.serviceProvider).IsAvailable)
        return (IProjectCreateError) new ProjectCreateError("DoNotWarnAboutSilverlightNotInstalled", StringTable.SilverlightNotInstalled);
      FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
      if (targetFrameworkName != (FrameworkName) null && targetFrameworkName.Identifier.Equals(SilverlightProjectType.MobileFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase) && (targetFrameworkName.Profile.Equals(SilverlightProjectType.MobileFrameworkName.Profile, StringComparison.OrdinalIgnoreCase) && targetFrameworkName.Version >= SilverlightProjectType.MobileFrameworkName.Version) && !ProjectAdapterHelper.IsTargetFrameworkSupported(this.serviceProvider, targetFrameworkName))
        return (IProjectCreateError) new ProjectCreateError("DoNotWarnAboutWindowsPhoneNotInstalled", StringTable.WindowsPhoneNotInstalled);
      return (IProjectCreateError) null;
    }

    public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
    {
      return (INamedProject) SilverlightProject.Create(projectStore, codeDocumentType, (IProjectType) this, serviceProvider);
    }

    public override ReferenceAssemblyMode GetReferenceAssemblyMode(IProject project)
    {
      return ReferenceAssemblyMode.TargetFramework;
    }
  }
}

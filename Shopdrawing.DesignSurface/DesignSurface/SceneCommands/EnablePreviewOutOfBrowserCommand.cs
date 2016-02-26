// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EnablePreviewOutOfBrowserCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Commands;
using System;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class EnablePreviewOutOfBrowserCommand : EnableSilverlightPropertyCommand
  {
    private static readonly string PreviewOutOfBrowserEnabled = "PreviewOutOfBrowserEnabled";

    public override bool IsEnabled
    {
      get
      {
        SilverlightProject currentProject = this.GetCurrentProject();
        if (currentProject == null)
          return false;
        if (!this.ShouldEnforceMutualExclusion())
          return base.IsEnabled;
        if (base.IsEnabled && !currentProject.UsePlatformExtensions)
          return currentProject.EnableOutOfBrowser;
        return false;
      }
    }

    protected override PlatformCapability TargetedCapability
    {
      get
      {
        return PlatformCapability.SupportsEnableOutOfBrowser;
      }
    }

    public EnablePreviewOutOfBrowserCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    protected override object GetProjectPropertyInternal(SilverlightProject project)
    {
      ISolution solution = ProjectCommandExtensions.Solution((IProjectCommand) this);
      if (solution != null && project != null)
      {
        object projectProperty = solution.SolutionSettingsManager.GetProjectProperty((INamedProject) project, EnablePreviewOutOfBrowserCommand.PreviewOutOfBrowserEnabled);
        if (projectProperty != null && projectProperty is bool)
          return (object) (bool) ((bool) projectProperty ? true : false);
      }
      return (object) false;
    }

    protected override void SetProjectPropertyInternal(SilverlightProject project, bool newValue)
    {
      ISolution solution = ProjectCommandExtensions.Solution((IProjectCommand) this);
      if (solution == null || project == null)
        return;
      solution.SolutionSettingsManager.SetProjectProperty((INamedProject) project, EnablePreviewOutOfBrowserCommand.PreviewOutOfBrowserEnabled, (object) (bool) (newValue ? true : false));
    }
  }
}

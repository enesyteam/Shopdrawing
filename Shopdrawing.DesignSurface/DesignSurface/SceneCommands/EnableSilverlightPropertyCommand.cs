// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EnableSilverlightPropertyCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.Commands;
using Microsoft.Win32;
using System;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class EnableSilverlightPropertyCommand : ProjectCommand
  {
    public override string DisplayName
    {
      get
      {
        return string.Empty;
      }
    }

    public override sealed bool IsAvailable
    {
      get
      {
        SilverlightProject currentProject = this.GetCurrentProject();
        if (currentProject == null || !currentProject.GetCapability<bool>("CanBeStartupProject"))
          return false;
        bool? nullable = (bool?) currentProject.ProjectContext.GetCapabilityValue(this.TargetedCapability);
        if (nullable.HasValue && nullable.HasValue)
          return nullable.Value;
        return false;
      }
    }

    protected abstract PlatformCapability TargetedCapability { get; }

    public EnableSilverlightPropertyCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    public override sealed void Execute()
    {
    }

    public override sealed void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "IsChecked")
      {
        SilverlightProject currentProject = this.GetCurrentProject();
        if (currentProject == null)
          return;
        this.SetProjectPropertyInternal(currentProject, (bool) propertyValue);
      }
      else
        base.SetProperty(propertyName, propertyValue);
    }

    public override sealed object GetProperty(string propertyName)
    {
      if (propertyName == "IsChecked")
      {
        SilverlightProject currentProject = this.GetCurrentProject();
        if (currentProject != null)
          return this.GetProjectPropertyInternal(currentProject);
      }
      return base.GetProperty(propertyName);
    }

    protected abstract void SetProjectPropertyInternal(SilverlightProject project, bool newValue);

    protected abstract object GetProjectPropertyInternal(SilverlightProject project);

    protected bool ShouldEnforceMutualExclusion()
    {
      return RegistryHelper.RetrieveRegistryValue<string>(Registry.LocalMachine, "Software\\Microsoft\\Expression\\Blend\\v3.0", "OOBSupportForPlatformExt") == null;
    }

    protected SilverlightProject GetCurrentProject()
    {
      return ProjectCommandExtensions.SelectedProjectOrNull((IProjectCommand) this) as SilverlightProject;
    }
  }
}

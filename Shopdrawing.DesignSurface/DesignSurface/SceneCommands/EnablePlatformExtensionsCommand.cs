// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EnablePlatformExtensionsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Commands;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class EnablePlatformExtensionsCommand : EnableSilverlightPropertyCommand
  {
    private static readonly string PreviewOutOfBrowserEnabled = "PreviewOutOfBrowserEnabled";

    protected override PlatformCapability TargetedCapability
    {
      get
      {
        return PlatformCapability.SupportsTransparentAssemblyCache;
      }
    }

    public EnablePlatformExtensionsCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    protected override object GetProjectPropertyInternal(SilverlightProject project)
    {
      return (object) (bool) (project.UsePlatformExtensions ? true : false);
    }

    protected override void SetProjectPropertyInternal(SilverlightProject project, bool newValue)
    {
      if (this.ShouldEnforceMutualExclusion() && newValue && project.EnableOutOfBrowser)
      {
        if (!this.PromptForPreviewOutOfBrowserToggle())
          return;
        project.EnableOutOfBrowser = false;
        project.UsePlatformExtensions = true;
        ISolution solution = ProjectCommandExtensions.Solution((IProjectCommand) this);
        if (solution == null)
          return;
        solution.SolutionSettingsManager.SetProjectProperty((INamedProject) project, EnablePlatformExtensionsCommand.PreviewOutOfBrowserEnabled, (object) false);
      }
      else
        project.UsePlatformExtensions = newValue;
    }

    private bool PromptForPreviewOutOfBrowserToggle()
    {
      return ServiceExtensions.MessageDisplayService(this.Services).ShowMessage(new MessageBoxArgs()
      {
        Message = StringTable.UsePlatformExtensionsCommandPreviewOutOfBrowserEnabledWarningMessage,
        Button = MessageBoxButton.YesNo,
        Image = MessageBoxImage.Exclamation
      }) == MessageBoxResult.Yes;
    }
  }
}

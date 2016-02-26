// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EnableApplicationOutsideBrowserCommand
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
  internal sealed class EnableApplicationOutsideBrowserCommand : EnableSilverlightPropertyCommand
  {
    private static readonly string PreviewOutOfBrowserEnabled = "PreviewOutOfBrowserEnabled";

    protected override PlatformCapability TargetedCapability
    {
      get
      {
        return PlatformCapability.SupportsEnableOutOfBrowser;
      }
    }

    public EnableApplicationOutsideBrowserCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    protected override object GetProjectPropertyInternal(SilverlightProject project)
    {
      return (object) (bool) (project.EnableOutOfBrowser ? true : false);
    }

    protected override void SetProjectPropertyInternal(SilverlightProject project, bool newValue)
    {
      if (this.ShouldEnforceMutualExclusion() && newValue && project.UsePlatformExtensions)
      {
        if (this.PromptForPlatformExtensionsToggle())
        {
          project.EnableOutOfBrowser = true;
          project.UsePlatformExtensions = false;
        }
      }
      else
        project.EnableOutOfBrowser = newValue;
      ISolution solution = ProjectCommandExtensions.Solution((IProjectCommand) this);
      if (solution == null)
        return;
      solution.SolutionSettingsManager.SetProjectProperty((INamedProject) project, EnableApplicationOutsideBrowserCommand.PreviewOutOfBrowserEnabled, (object) (bool) (project.EnableOutOfBrowser ? true : false));
    }

    private bool PromptForPlatformExtensionsToggle()
    {
      return ServiceExtensions.MessageDisplayService(this.Services).ShowMessage(new MessageBoxArgs()
      {
        Message = StringTable.PreviewOutOfBrowserCommandUsePlatformExtensionsEnabledWarning,
        Button = MessageBoxButton.YesNo,
        Image = MessageBoxImage.Exclamation,
        AutomationId = "TogglePlatformExtensionsDialog"
      }) == MessageBoxResult.Yes;
    }
  }
}

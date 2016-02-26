// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EnableElevatedOutOfBrowserCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class EnableElevatedOutOfBrowserCommand : EnableSilverlightPropertyCommand
  {
    public override bool IsEnabled
    {
      get
      {
        return (bool) ServiceExtensions.CommandService(this.Services).GetCommandProperty("Project_EnablePreviewOutOfBrowser", "IsEnabled");
      }
    }

    protected override PlatformCapability TargetedCapability
    {
      get
      {
        return PlatformCapability.SupportsEnableOutOfBrowser;
      }
    }

    public EnableElevatedOutOfBrowserCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    protected override object GetProjectPropertyInternal(SilverlightProject project)
    {
      return (object) (bool) (project.ElevatedOutOfBrowser ? true : false);
    }

    protected override void SetProjectPropertyInternal(SilverlightProject project, bool newValue)
    {
      project.ElevatedOutOfBrowser = newValue;
    }
  }
}

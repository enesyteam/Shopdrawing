// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands.BuildProjectCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Commands;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands
{
  internal sealed class BuildProjectCommand : ModalCommandBase
  {
    private const string ProjectCommandId = "Project_Build";

    public override bool IsEnabled
    {
      get
      {
        object commandProperty = this.DesignerContext.CommandService.GetCommandProperty("Project_Build", "IsEnabled");
        if (commandProperty is bool)
          return (bool) commandProperty;
        return false;
      }
    }

    public override string Description
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.BuildProjectModalCommandDescription, new object[1]
        {
          (object) this.DesignerContext.ProjectManager.ActiveBuildTarget.DisplayName
        });
      }
    }

    public BuildProjectCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      this.DesignerContext.CommandService.Execute("Project_Build", CommandInvocationSource.Command);
    }
  }
}

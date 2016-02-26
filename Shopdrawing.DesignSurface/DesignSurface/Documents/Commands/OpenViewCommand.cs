// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.Commands.OpenViewCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Commands;
using System;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Documents.Commands
{
  internal class OpenViewCommand : Command, IProjectCommand, ICommand
  {
    public string DisplayName
    {
      get
      {
        return string.Empty;
      }
    }

    public IServiceProvider Services { get; private set; }

    public override bool IsEnabled
    {
      get
      {
        foreach (IDocumentItem documentItem in ProjectCommandExtensions.Selection((IProjectCommand) this))
        {
          IProjectItem projectItem = documentItem as IProjectItem;
          if (projectItem != null && !projectItem.DocumentType.CanView)
            return false;
        }
        return base.IsEnabled;
      }
    }

    public OpenViewCommand(DesignerContext designerContext)
    {
      this.Services = (IServiceProvider) designerContext.Services;
    }

    public override void Execute()
    {
      if (!this.IsEnabled)
        return;
      foreach (IDocumentItem documentItem in Enumerable.ToList<IDocumentItem>(ProjectCommandExtensions.Selection((IProjectCommand) this)))
      {
        IProjectItem projectItem = documentItem as IProjectItem;
        if (projectItem != null)
          projectItem.OpenView(true);
      }
    }
  }
}

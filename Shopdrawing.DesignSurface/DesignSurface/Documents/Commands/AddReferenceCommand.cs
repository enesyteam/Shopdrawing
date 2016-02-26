// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.Commands.AddReferenceCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Commands;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Documents.Commands
{
  public class AddReferenceCommand : AddProjectItemCommand
  {
    private IProject targetedProject;
    private string importedPathFilter;

    public override string DisplayName
    {
      get
      {
        return StringTable.CommandAddReferenceName;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        IProject project = ProjectCommandExtensions.SelectedProjectOrNull((IProjectCommand) this);
        if (base.IsEnabled && project != null)
          return project.GetCapability<bool>("CanAddAssemblyReference");
        return false;
      }
    }

    public override bool IsAvailable
    {
      get
      {
        if (!ProjectCommandExtensions.IsWebSolution((IProjectCommand) this))
          return base.IsAvailable;
        return false;
      }
    }

    internal AddReferenceCommand(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "TargetProject")
        this.targetedProject = propertyValue as IProject;
      else if (propertyName == "ImportedPathFilter")
        this.importedPathFilter = propertyValue as string;
      else
        base.SetProperty(propertyName, propertyValue);
    }

    protected override bool CreateProjectItem()
    {
      IProject project = this.targetedProject != null ? this.targetedProject : ProjectCommandExtensions.SelectedProjectOrNull((IProjectCommand) this);
      if (project == null)
        return false;
      bool flag = false;
      string[] filesToImport = this.GetFilesToImport(this.importedPathFilter != null ? this.importedPathFilter : this.GetImportFolder());
      if (filesToImport != null && filesToImport.Length > 0)
      {
        foreach (string str in filesToImport)
        {
          string withoutExtension = Path.GetFileNameWithoutExtension(str);
          ProjectAssembly projectAssembly = project.ReferencedAssemblies.Find(withoutExtension);
          if (projectAssembly != null && !projectAssembly.IsImplicitlyResolved)
          {
            ProjectCommandExtensions.DisplayCommandFailedMessage((IProjectCommand) this, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, StringTable.ReferenceExistsErrorDialogMessage, new object[2]
            {
              (object) str,
              (object) withoutExtension
            }));
          }
          else
          {
            IProjectItem projectItem = project.AddAssemblyReference(str, true);
            if (projectItem != null)
            {
              ProjectCommandExtensions.ProjectManager((IProjectCommand) this).DefaultImportPath = Path.GetDirectoryName(str);
              ProjectCommandExtensions.ProjectManager((IProjectCommand) this).ItemSelectionSet.Clear();
              ProjectCommandExtensions.ProjectManager((IProjectCommand) this).ItemSelectionSet.ToggleSelection((IDocumentItem) projectItem);
            }
            ProjectXamlContext projectContext = ProjectXamlContext.GetProjectContext(project);
            if (projectContext != null)
              projectContext.EnsureAssemblyReferenced(str);
            if (projectItem != null)
              flag = true;
          }
        }
      }
      return flag;
    }

    protected override AddProjectItemCommand.FileTypeDescription[] GetFileTypeFilter()
    {
      return new AddProjectItemCommand.FileTypeDescription[1]
      {
        new AddProjectItemCommand.FileTypeDescription(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AddProjectItemCommandFileFilterTextFormat, new object[2]
        {
          (object) DocumentServiceExtensions.DocumentTypes(this.Services)[DocumentTypeNamesHelper.Assembly].Description,
          (object) "*.dll, *.exe"
        }), "*.dll; *.exe")
      };
    }
  }
}

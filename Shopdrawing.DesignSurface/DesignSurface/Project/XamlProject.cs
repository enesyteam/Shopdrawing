// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.XamlProject
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal class XamlProject : WindowsExecutableProject, IXamlProject
  {
    public IProjectContext ProjectContext { get; private set; }

    protected XamlProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
      : base(projectStore, codeDocumentType, projectType, serviceProvider)
    {
    }

    protected override bool Initialize()
    {
      if (!base.Initialize())
        return false;
      if (this.ProjectContext == null)
        this.ProjectContext = (IProjectContext) new ProjectXamlContext((IProject) this, this.Services, false);
      return true;
    }

    public override IDocumentType GetDocumentType(string fileName)
    {
      return this.GetDocumentType(fileName, (IList<IDocumentType>) new List<IDocumentType>(1)
      {
        DocumentServiceExtensions.DocumentTypes(this.Services)[DocumentTypeNamesHelper.Xaml]
      });
    }

    protected override IDocumentType GetDocumentTypeForBuildItem(string fileName, string buildItemType)
    {
      if (buildItemType == "ApplicationDefinition" && PathHelper.FileExists(fileName))
        return DocumentServiceExtensions.DocumentTypes(this.Services)[DocumentTypeNamesHelper.ApplicationDefinition];
      return base.GetDocumentTypeForBuildItem(fileName, buildItemType);
    }

    protected static BuildTaskInfoPopulator CreateLargeImagePopulator(string baseWarningMessage, BuildTaskInfo overrideBuildTask, IServiceProvider services)
    {
      Func<string> promptMessageFormatter = (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, baseWarningMessage, new object[1]
      {
        (object) XamlProject.GetLargeImageThreshold(services).ToString((IFormatProvider) CultureInfo.CurrentCulture)
      }));
      Func<string> doNotPromptAgainMessageFormatter = (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ImageScalabilityWarningDoNotShowAgainMessage, new object[1]
      {
        (object) XamlProject.GetLargeImageThreshold(services).ToString((IFormatProvider) CultureInfo.CurrentCulture)
      }));
      IBuildTaskOverrider buildTaskOverrider = (IBuildTaskOverrider) new BuildTaskOverrider(services, promptMessageFormatter, doNotPromptAgainMessageFormatter, "LargeImageDialog", overrideBuildTask);
      return (BuildTaskInfoPopulator) new LargeImageBuildTaskInfoPopulator((ICreationInfoFilter) new LargeImageCreationInfoFilter(services), buildTaskOverrider);
    }

    private static int GetLargeImageThreshold(IServiceProvider services)
    {
      return ServiceExtensions.ProjectManager(services).OptionsModel.LargeImageWarningThreshold;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.OutOfBrowserDeploymentService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class OutOfBrowserDeploymentService : IOutOfBrowserDeploymentService
  {
    private static readonly string PrivateXapName = "application.xap";
    private const string SlLauncherRelativePath = "Microsoft Silverlight\\sllauncher.exe";
    private IServiceProvider serviceProvider;

    public OutOfBrowserDeploymentService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public ProcessStartInfo TryPerformOutOfBrowserDeployment(IProject startupProject, DocumentReference serverRoot, Uri baseWebServerUri, out bool shouldStartServer)
    {
      ProcessStartInfo processStartInfo = (ProcessStartInfo) null;
      shouldStartServer = false;
      DocumentReference xapDeploymentDirectory = (DocumentReference) null;
      IProject xapSourceProject = this.FindXapSourceProject(startupProject, out xapDeploymentDirectory);
      if (xapDeploymentDirectory != (DocumentReference) null && this.SupportsOutOfBrowserDeployment(xapSourceProject))
      {
        DocumentReference documentReference = this.DetermineLauncherLocation();
        if (documentReference != (DocumentReference) null)
        {
          DocumentReference fromRelativePath = DocumentReference.CreateFromRelativePath(xapDeploymentDirectory.Path, PathHelper.GetFileOrDirectoryName(xapSourceProject.FullTargetPath));
          Uri ofBrowserQueryUri = this.CreateOutOfBrowserQueryUri(baseWebServerUri, serverRoot, fromRelativePath);
          string applicationIdentifier = (string) null;
          DocumentReference privateDeploymentDirectory = this.DetermineOutOfBrowserPrivateLocation(xapSourceProject, ofBrowserQueryUri, out applicationIdentifier);
          processStartInfo = new ProcessStartInfo(documentReference.Path);
          if (string.IsNullOrEmpty(applicationIdentifier))
          {
            Uri uri = new Uri(fromRelativePath.Path, UriKind.Absolute);
            processStartInfo.Arguments = "/emulate:\"" + (object) fromRelativePath + "\" /origin:\"" + (string) (object) uri + "\"";
            shouldStartServer = false;
          }
          else if (this.AttemptToCopyToOutOfBrowserPrivateLocation(fromRelativePath, privateDeploymentDirectory))
          {
            processStartInfo.Arguments = applicationIdentifier;
            shouldStartServer = true;
          }
          processStartInfo.UseShellExecute = false;
          processStartInfo.RedirectStandardError = true;
        }
      }
      return processStartInfo;
    }

    private IProject FindXapSourceProject(IProject targetProject, out DocumentReference xapDeploymentDirectory)
    {
      xapDeploymentDirectory = (DocumentReference) null;
      IProject project = (IProject) null;
      if (targetProject is SilverlightProject && targetProject.GetCapability<bool>("CanBeStartupProject"))
      {
        project = targetProject;
        if (project.FullTargetPath != null)
          xapDeploymentDirectory = DocumentReference.Create(PathHelper.GetDirectoryNameOrRoot(project.FullTargetPath));
      }
      else
      {
        ISolution currentSolution = ServiceExtensions.ProjectManager(this.serviceProvider).CurrentSolution;
        IProjectOutputReferenceResolver referenceResolver = currentSolution as IProjectOutputReferenceResolver;
        if (currentSolution != null && referenceResolver != null)
        {
          foreach (IProject sourceProject in currentSolution.Projects)
          {
            Uri deploymentResolvedRoot = referenceResolver.GetDeploymentResolvedRoot(sourceProject);
            if (deploymentResolvedRoot != (Uri) null)
            {
              DocumentReference documentReference = DocumentReference.Create(deploymentResolvedRoot.LocalPath);
              if (documentReference.Path.StartsWith(targetProject.ProjectRoot.Path, StringComparison.OrdinalIgnoreCase))
              {
                xapDeploymentDirectory = documentReference;
                project = sourceProject;
                break;
              }
            }
          }
        }
      }
      return project;
    }

    private bool SupportsOutOfBrowserDeployment(IProject sourceProject)
    {
      ProjectBase projectBase = sourceProject as ProjectBase;
      if (projectBase != null)
        return projectBase.ShouldPerformOutOfBrowserDeployment();
      return false;
    }

    private Uri CreateOutOfBrowserQueryUri(Uri baseWebServerUri, DocumentReference serverRoot, DocumentReference xapBuildDeployedFullPath)
    {
      if (serverRoot == (DocumentReference) null || xapBuildDeployedFullPath == (DocumentReference) null)
        return (Uri) null;
      return new Uri(baseWebServerUri, serverRoot.GetRelativePath(xapBuildDeployedFullPath));
    }

    private DocumentReference DetermineOutOfBrowserPrivateLocation(IProject sourceProject, Uri outOfBrowserQueryUri, out string applicationIdentifier)
    {
      DocumentReference documentReference = (DocumentReference) null;
      applicationIdentifier = (string) null;
      IXamlProject xamlProject = sourceProject as IXamlProject;
      IProjectContext projectContext = xamlProject != null ? xamlProject.ProjectContext : (IProjectContext) null;
      if (projectContext != null && projectContext.Platform != null)
      {
        string deploymentLocation;
        applicationIdentifier = projectContext.Platform.GetDeploymentInformation(outOfBrowserQueryUri, out deploymentLocation);
        if (deploymentLocation != null)
          documentReference = DocumentReference.Create(deploymentLocation);
      }
      return documentReference;
    }

    private bool AttemptToCopyToOutOfBrowserPrivateLocation(DocumentReference xapLocation, DocumentReference privateDeploymentDirectory)
    {
      if (!PathHelper.FileOrDirectoryExists(xapLocation.Path))
        return false;
      string str = PathHelper.ResolveCombinedPath(privateDeploymentDirectory.Path, OutOfBrowserDeploymentService.PrivateXapName);
      IEnumerable<MoveResult> source = (IEnumerable<MoveResult>) ProjectPathHelper.CopySafe((IEnumerable<MoveInfo>) new List<MoveInfo>()
      {
        new MoveInfo()
        {
          Source = xapLocation.Path,
          Destination = str
        }
      }, MoveOptions.OverwriteDestination, false);
      if (source != null)
        return EnumerableExtensions.CountIs<MoveResult>(source, 1);
      return false;
    }

    private DocumentReference DetermineLauncherLocation()
    {
      DocumentReference fromRelativePath = DocumentReference.CreateFromRelativePath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Silverlight\\sllauncher.exe");
      if (PathHelper.FileOrDirectoryExists(fromRelativePath.Path))
        return fromRelativePath;
      return (DocumentReference) null;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.LargeImageCreationInfoFilter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal class LargeImageCreationInfoFilter : ICreationInfoFilter
  {
    private IServiceProvider serviceProvider;

    internal LargeImageCreationInfoFilter(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public IList<DocumentCreationInfo> FilterItems(IEnumerable<DocumentCreationInfo> items, IProject project)
    {
      return (IList<DocumentCreationInfo>) Enumerable.ToList<DocumentCreationInfo>(Enumerable.Where<DocumentCreationInfo>(items, (Func<DocumentCreationInfo, bool>) (potentialImage =>
      {
        if (!this.ItemAlreadyExistsInProject(potentialImage, project) && this.CanOverrideBuildTask(potentialImage))
          return this.IsLargeImage(potentialImage);
        return false;
      })));
    }

    private bool ItemAlreadyExistsInProject(DocumentCreationInfo potentialImage, IProject project)
    {
      return project.FindItem(DocumentReference.Create(potentialImage.TargetPath)) != null;
    }

    private bool CanOverrideBuildTask(DocumentCreationInfo potentialImage)
    {
      if (potentialImage.BuildTaskInfo == null)
        return !this.MustUseDefaultBuildTask(potentialImage);
      return false;
    }

    private bool MustUseDefaultBuildTask(DocumentCreationInfo potentialImage)
    {
      return (potentialImage.CreationOptions & CreationOptions.AlwaysUseDefaultBuildTask) == CreationOptions.AlwaysUseDefaultBuildTask;
    }

    private bool IsLargeImage(DocumentCreationInfo potentialImage)
    {
      if (potentialImage.DocumentType is ImageDocumentType)
        return this.IsFileLargerThan(this.GetLargeImageThreshold(), potentialImage.SourcePath);
      return false;
    }

    protected virtual long GetLargeImageThreshold()
    {
      return (long) ((double) ServiceExtensions.ProjectManager(this.serviceProvider).OptionsModel.LargeImageWarningThreshold * Math.Pow(2.0, 10.0));
    }

    private bool IsFileLargerThan(long size, string path)
    {
      return this.GetFileSize(path) > size;
    }

    protected virtual long GetFileSize(string path)
    {
      try
      {
        if (Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(path))
        {
          if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
            return new FileInfo(path).Length;
        }
      }
      catch (SecurityException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (IOException ex)
      {
      }
      return long.MinValue;
    }
  }
}

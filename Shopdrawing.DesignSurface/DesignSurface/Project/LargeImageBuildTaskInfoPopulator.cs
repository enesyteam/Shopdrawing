// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Project.LargeImageBuildTaskInfoPopulator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Project
{
  internal class LargeImageBuildTaskInfoPopulator : BuildTaskInfoPopulator
  {
    private ICreationInfoFilter largeImageFilter;
    private IBuildTaskOverrider buildTaskOverrider;

    public LargeImageBuildTaskInfoPopulator(ICreationInfoFilter largeImageFilter, IBuildTaskOverrider buildTaskOverrider)
    {
      this.largeImageFilter = largeImageFilter;
      this.buildTaskOverrider = buildTaskOverrider;
    }

    protected override IEnumerable<DocumentCreationInfo> FillOutBuildTaskInfoInternal(IEnumerable<DocumentCreationInfo> creationInfo, IProject project)
    {
      IList<DocumentCreationInfo> list = this.largeImageFilter.FilterItems(creationInfo, project);
      if (list.Count == 0)
        return creationInfo;
      return this.ProcessOverrideResult(this.buildTaskOverrider.ShouldOverrideDefaultBuildAction(project, list), creationInfo, list);
    }

    private IEnumerable<DocumentCreationInfo> ProcessOverrideResult(ProjectDialog.ProjectDialogResult result, IEnumerable<DocumentCreationInfo> creationInfo, IList<DocumentCreationInfo> largeImages)
    {
      switch (result)
      {
        case ProjectDialog.ProjectDialogResult.Ok:
          return this.buildTaskOverrider.OverrideBuildTaskFor(creationInfo, largeImages);
        case ProjectDialog.ProjectDialogResult.Discard:
          return creationInfo;
        default:
          return Enumerable.Empty<DocumentCreationInfo>();
      }
    }
  }
}

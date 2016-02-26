// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ProjectAssetProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Project;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class ProjectAssetProvider : AssetProvider
  {
    private DesignerContext designerContext;

    public ProjectAssetProvider(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    protected override bool UpdateAssets()
    {
      this.Assets.Clear();
      IProject activeProject = this.designerContext.ActiveProject;
      if (activeProject != null)
      {
        foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) activeProject.Items)
        {
          if (projectItem.DocumentType is AssetDocumentType && projectItem.DocumentType.CanAddToProject(activeProject) && projectItem.Visible)
            this.Assets.Add((Asset) new ProjectAsset(projectItem));
        }
      }
      this.Assets.Sort(Asset.DefaultComparer);
      this.NeedsUpdate = false;
      this.NotifyAssetsChanged();
      return true;
    }
  }
}

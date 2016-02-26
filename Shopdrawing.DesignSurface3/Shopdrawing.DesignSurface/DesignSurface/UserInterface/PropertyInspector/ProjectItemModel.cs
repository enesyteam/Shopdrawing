// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ProjectItemModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class ProjectItemModel
  {
    internal const int PrimaryProjectItem = 0;
    internal const int ProjectOutputReferencedProjectItem = 1;
    internal const int UserSpecifiedProjectItem = 2;

    public int SortIndex { get; private set; }

    public string DisplayName { get; private set; }

    public string ProjectRelativeLocation { get; private set; }

    public IProjectItem ProjectItem { get; private set; }

    public string ProjectName { get; private set; }

    public string RelativeUri { get; private set; }

    public ProjectItemModel(IProjectItem projectItem, IDocumentContext documentContext, int sortIndex)
    {
      this.ProjectItem = projectItem;
      this.ProjectName = projectItem.Project.Name;
      this.DisplayName = projectItem.DocumentReference.DisplayName;
      this.ProjectRelativeLocation = projectItem.ProjectRelativeDocumentReference;
      this.SortIndex = sortIndex;
      this.RelativeUri = this.MakeProjectItemRelativeUri(documentContext);
    }

    public ProjectItemModel(string displayName)
    {
      this.DisplayName = displayName;
      this.ProjectRelativeLocation = displayName;
      this.SortIndex = 2;
      this.RelativeUri = this.MakeProjectItemRelativeUri((IDocumentContext) null);
    }

    public override bool Equals(object obj)
    {
      ProjectItemModel projectItemModel = obj as ProjectItemModel;
      if (projectItemModel != null && this.DisplayName == projectItemModel.DisplayName)
        return this.ProjectRelativeLocation == projectItemModel.ProjectRelativeLocation;
      return false;
    }

    public override int GetHashCode()
    {
      return this.DisplayName.GetHashCode() ^ this.ProjectRelativeLocation.GetHashCode();
    }

    public override string ToString()
    {
      return this.RelativeUri;
    }

    private string MakeProjectItemRelativeUri(IDocumentContext documentContext)
    {
      if (documentContext != null && this.ProjectItem != null)
      {
        string str = documentContext.MakeResourceReference(this.ProjectItem.DocumentReference.Path);
        if (!string.IsNullOrEmpty(str))
          return str;
      }
      return this.DisplayName;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.ProjectItemAction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  [Prototype]
  public class ProjectItemAction
  {
    public string ProjectItemPath { get; set; }

    public string SourceFilePath { get; set; }

    public ProjectItemOperation Operation { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsCompleted { get; set; }

    public ProjectItemAction()
    {
    }

    public ProjectItemAction(string projectItemPath, bool enabled, string sourceFilePath)
      : this(projectItemPath, ProjectItemOperation.Add, enabled, sourceFilePath)
    {
    }

    public ProjectItemAction(string projectItemPath, ProjectItemOperation operations, bool enabled, string sourceFilePath)
    {
      this.ProjectItemPath = projectItemPath;
      this.SourceFilePath = sourceFilePath;
      this.Operation = operations;
      this.IsEnabled = enabled;
    }

    public void PrepareToUpdate(string sourcePath, bool enabled)
    {
      this.SourceFilePath = sourcePath;
      this.IsEnabled = enabled;
      this.IsCompleted = false;
      if (this.Operation == ProjectItemOperation.Add)
        return;
      this.Operation = ProjectItemOperation.Update;
    }

    public void PrepareToUpdate(bool enabled)
    {
      this.IsEnabled = enabled;
      this.IsCompleted = false;
    }

    public virtual void OnActionCompleted()
    {
      this.SourceFilePath = (string) null;
      this.Operation = ProjectItemOperation.None;
      this.IsCompleted = true;
    }
  }
}

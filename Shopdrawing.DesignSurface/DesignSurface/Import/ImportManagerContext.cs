// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Import.ImportManagerContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Import
{
  public class ImportManagerContext : IImportManagerContext
  {
    private SceneViewModel sceneViewModel;
    private SceneNodeIDHelper sceneNodeIDHelper;
    private SceneEditTransaction undoPending;
    private bool supportProjectRollbackOnCancel;

    public IProject Project { get; set; }

    internal SceneNodeIDHelper NameIDHelper
    {
      get
      {
        return this.sceneNodeIDHelper;
      }
    }

    public string FileName { get; set; }

    public SceneViewModel SceneViewModel
    {
      get
      {
        return this.sceneViewModel;
      }
    }

    public string ImportDirectoryPath
    {
      get
      {
        if (this.sceneViewModel == null)
          return this.Project.ProjectRoot.Path;
        string str = this.sceneViewModel.DesignerContext.ProjectManager.TargetFolderForProject(this.ActiveProject);
        if (!string.IsNullOrEmpty(str))
          return str;
        return Path.GetDirectoryName(this.sceneViewModel.Document.DocumentReference.Path);
      }
    }

    public IProject ActiveProject
    {
      get
      {
        if (this.sceneViewModel != null)
          return this.sceneViewModel.DesignerContext.ActiveProject;
        return this.Project;
      }
    }

    public bool SupportProjectRollbackOnCancel
    {
      get
      {
        return this.supportProjectRollbackOnCancel;
      }
    }

    public ImportManagerContext(string fileName, SceneViewModel sceneViewModel, IProject project, bool supportProjectRollbackOnCancel)
    {
      this.FileName = fileName;
      if (sceneViewModel != null)
      {
        this.sceneViewModel = sceneViewModel;
        this.sceneNodeIDHelper = new SceneNodeIDHelper(this.sceneViewModel, this.sceneViewModel.RootNode);
      }
      this.Project = project;
      this.supportProjectRollbackOnCancel = supportProjectRollbackOnCancel;
    }

    public void CreateEditTransaction()
    {
      if (this.sceneViewModel == null)
        return;
      this.undoPending = this.sceneViewModel.CreateEditTransaction(StringTable.UndoUnitImport);
    }

    public void CommitEditTransaction()
    {
      if (this.undoPending == null)
        return;
      this.undoPending.Commit();
      this.undoPending.Dispose();
      this.undoPending = (SceneEditTransaction) null;
    }

    public void CancelEditTransaction()
    {
      if (this.undoPending == null)
        return;
      this.undoPending.Cancel();
      this.undoPending.Dispose();
      this.undoPending = (SceneEditTransaction) null;
    }

    public string MakeSourceReference(string fullPath)
    {
      DocumentReference reference = DocumentReference.Create(fullPath);
      if (this.sceneViewModel != null)
      {
        string str = this.sceneViewModel.Document.DocumentReference.GetRelativePath(reference);
        if (str != null && this.sceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.ShouldSanitizeResourceReferences))
          str = str.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return str;
      }
      string str1 = this.ActiveProject.DocumentReference.GetRelativePath(reference);
      if (str1 == null)
        return (string) null;
      IXamlProject xamlProject = this.ActiveProject as IXamlProject;
      if (xamlProject != null)
      {
        ITypeResolver typeResolver = (ITypeResolver) ProjectContext.GetProjectContext(xamlProject.ProjectContext);
        if (typeResolver != null && typeResolver.IsCapabilitySet(PlatformCapability.ShouldSanitizeResourceReferences))
          str1 = str1.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      }
      return str1;
    }

    public bool LogMessage(string message)
    {
      if (this.sceneViewModel != null)
      {
        IMessageLoggingService messageLoggingService = this.sceneViewModel.DesignerContext.MessageLoggingService;
        if (messageLoggingService != null)
        {
          messageLoggingService.WriteLine(message);
          return true;
        }
      }
      return false;
    }

    public string CreateNameId(SceneNode sceneNode, string originalName)
    {
      return this.sceneNodeIDHelper.GetValidElementID(sceneNode, originalName);
    }

    public virtual void AskForFileName(ExpressionOpenFileDialog dialog)
    {
      bool? nullable = dialog.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      this.FileName = dialog.FileName;
    }
  }
}

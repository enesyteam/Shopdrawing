// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.DocumentContextHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.IO;

namespace Microsoft.Expression.DesignSurface.Documents
{
  public static class DocumentContextHelper
  {
    public static readonly string DesignDataBuildTask = "DesignData";
    public static readonly string CreatableDesignDataBuildTask = "DesignDataWithDesignTimeCreatableTypes";
    public static readonly string IsDesignTimeCreatable = "IsDesignTimeCreatable";

    public static DocumentContext CreateDocumentContext(IProject project, IProjectContext projectContext, IDocumentLocator documentLocator)
    {
      return DocumentContextHelper.CreateDocumentContext(project, projectContext, documentLocator, false);
    }

    public static DocumentContext CreateDocumentContext(IProject project, IProjectContext projectContext, IDocumentLocator documentLocator, bool isLooseXaml)
    {
      string path = documentLocator != null ? documentLocator.Path : (string) null;
      if (DocumentContextHelper.GetDesignDataMode(project, path) == DesignDataMode.Reflectable && !(projectContext is TypeReflectingProjectContext))
        projectContext = (IProjectContext) new TypeReflectingProjectContext(projectContext);
      return new DocumentContext(projectContext, documentLocator, isLooseXaml);
    }

    public static DesignDataMode GetDesignDataMode(IProject project, string path)
    {
      if (project == null || string.IsNullOrEmpty(path))
        return DesignDataMode.None;
      return DocumentContextHelper.GetDesignDataMode(project.FindItem(DocumentReference.Create(path)));
    }

    public static DesignDataMode GetDesignDataMode(IProjectItem projectItem)
    {
      if (projectItem == null || !(projectItem.DocumentType is XamlDocumentType))
        return DesignDataMode.None;
      IMSBuildItem msBuildItem = projectItem as IMSBuildItem;
      if (msBuildItem == null)
        return DesignDataMode.None;
      if (projectItem.Properties["BuildAction"] == DocumentContextHelper.DesignDataBuildTask)
      {
        bool result;
        return bool.TryParse(msBuildItem.GetMetadata(DocumentContextHelper.IsDesignTimeCreatable), out result) && result ? DesignDataMode.Creatable : DesignDataMode.Reflectable;
      }
      return projectItem.Properties["BuildAction"] == DocumentContextHelper.CreatableDesignDataBuildTask ? DesignDataMode.Creatable : DesignDataMode.None;
    }

    public static DocumentNode GetParsedOrSniffedRootNode(IProjectItem projectItem, IProjectContext projectContext)
    {
      if (projectItem.Document != null)
      {
        SceneDocument sceneDocument = projectItem.Document as SceneDocument;
        if (sceneDocument != null && sceneDocument.DocumentRoot != null)
          return sceneDocument.DocumentRoot.RootNode;
      }
      DocumentContext documentContext = DocumentContextHelper.CreateDocumentContext((IProject) projectContext.GetService(typeof (IProject)), projectContext, DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
      IType type = DocumentContextHelper.SniffRootNodeType(projectItem, documentContext);
      if (type != null && type.RuntimeType != (Type) null)
        return (DocumentNode) documentContext.CreateNode((ITypeId) type);
      return (DocumentNode) null;
    }

    private static IType SniffRootNodeType(IProjectItem projectItem, DocumentContext context)
    {
      if (projectItem.DocumentType is XamlDocumentType)
      {
        if (projectItem.FileExists)
        {
          try
          {
            using (FileStream fileStream = new FileStream(projectItem.DocumentReference.Path, FileMode.Open, FileAccess.Read))
            {
              string xamlClassAttribute;
              return XamlRootNodeSniffer.SniffRootNodeType((Stream) fileStream, (IDocumentContext) context, out xamlClassAttribute) as IType;
            }
          }
          catch (Exception ex)
          {
            if (!ErrorHandling.ShouldHandleExceptions(ex))
              throw;
          }
        }
      }
      return (IType) null;
    }
  }
}

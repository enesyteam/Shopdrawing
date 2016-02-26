// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.ResourceDictionaryDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal sealed class ResourceDictionaryDocumentType : XamlDocumentType
  {
    private const string BuildTask = "Page";

    public override string Name
    {
      get
      {
        return "ResourceDictionary";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ResourceDictionaryDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return "ResourceDictionary";
      }
    }

    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return false;
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Page";
      }
    }

    public ResourceDictionaryDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    public override bool CanAddToProject(IProject project)
    {
      if ((IProjectContext) ProjectXamlContext.GetProjectContext(project) == null)
        return false;
      return base.CanAddToProject(project);
    }
  }
}

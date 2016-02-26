// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.WavefrontMtlDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class WavefrontMtlDocumentType : DocumentType
  {
    protected DesignerContext designerContext;

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Mtl.png";
      }
    }

    public override string Name
    {
      get
      {
        return "Mtl";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.WavefrontMtlDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.WavefrontMtlDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".mtl"
        };
      }
    }

    public WavefrontMtlDocumentType(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public override bool CanAddToProject(IProject project)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null || !JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.ModelVisual3D))
        return false;
      return base.CanAddToProject(project);
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.FontDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class FontDocumentType : DocumentType
  {
    public const string ResourceBuildItemName = "Resource";
    public const string EmbeddedFontBuildItemName = "BlendEmbeddedFont";

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\FontIcon.png";
      }
    }

    public override string Name
    {
      get
      {
        return DocumentTypeNamesHelper.Font;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.FontDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.FontDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[4]
        {
          ".ttf",
          ".otf",
          ".ttc",
          ".tte"
        };
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Resource";
      }
    }

    public override bool CanAddToProject(IProject project)
    {
      if ((IProjectContext) ProjectXamlContext.GetProjectContext(project) == null)
        return false;
      return base.CanAddToProject(project);
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}

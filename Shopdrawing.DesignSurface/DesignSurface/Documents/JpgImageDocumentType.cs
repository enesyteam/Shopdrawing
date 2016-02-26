// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.JpgImageDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class JpgImageDocumentType : ImageDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Jpg.png";
      }
    }

    public override string Name
    {
      get
      {
        return "JpgImage";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.JpgImageDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.JpgImageDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[2]
        {
          ".jpg",
          ".jpeg"
        };
      }
    }

    public JpgImageDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    public override bool CanAddToProject(IProject project)
    {
      return true;
    }
  }
}

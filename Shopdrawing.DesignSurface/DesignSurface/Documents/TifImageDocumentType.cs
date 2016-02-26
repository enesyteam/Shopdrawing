// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.TifImageDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class TifImageDocumentType : ImageDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Tif.png";
      }
    }

    public override string Name
    {
      get
      {
        return "TIFImage";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.TifImageDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.TifImageDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[2]
        {
          ".tif",
          ".tiff"
        };
      }
    }

    public TifImageDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }
  }
}

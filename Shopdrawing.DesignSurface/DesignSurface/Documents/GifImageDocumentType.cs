// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.GifImageDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class GifImageDocumentType : ImageDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Png.png";
      }
    }

    public override string Name
    {
      get
      {
        return "GifImage";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.GifImageDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.GifImageDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".gif"
        };
      }
    }

    public GifImageDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }
  }
}

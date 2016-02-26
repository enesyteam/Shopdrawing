// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.IconImageDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class IconImageDocumentType : ImageDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Icon.png";
      }
    }

    public override string Name
    {
      get
      {
        return "IconImage";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.IconImageDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.IconImageDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".ico"
        };
      }
    }

    public IconImageDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }
  }
}

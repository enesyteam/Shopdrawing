// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.XapDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal sealed class XapDocumentType : DocumentType
  {
    public override string Name
    {
      get
      {
        return DocumentTypeNamesHelper.Xap;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.XapDocumentTypeDescription;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".xap"
        };
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return "Xap";
      }
    }

    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Xap.png";
      }
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}

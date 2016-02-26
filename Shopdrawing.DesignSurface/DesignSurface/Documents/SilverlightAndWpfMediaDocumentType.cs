// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.SilverlightAndWpfMediaDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class SilverlightAndWpfMediaDocumentType : MediaDocumentType
  {
    public override string[] FileExtensions
    {
      get
      {
        return new string[6]
        {
          ".mp3",
          ".mp4",
          ".asf",
          ".asx",
          ".wma",
          ".wmv"
        };
      }
    }

    public override string Name
    {
      get
      {
        return DocumentTypeNamesHelper.SilverlightAndWpfMedia;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.SilverlightAndWpfMediaDocumentTypeDescription;
      }
    }

    public SilverlightAndWpfMediaDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }
  }
}

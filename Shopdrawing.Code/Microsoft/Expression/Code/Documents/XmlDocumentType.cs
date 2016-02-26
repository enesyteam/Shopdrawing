// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.XmlDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;

namespace Microsoft.Expression.Code.Documents
{
  internal class XmlDocumentType : LimitedDocumentType
  {
    public const string BuildTask = "Resource";

    protected override string ImagePath
    {
      get
      {
        return "Resources\\XML.png";
      }
    }

    public override string Name
    {
      get
      {
        return "Xml Document";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.XmlDocumentTypeDescription;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".xml"
        };
      }
    }

    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return true;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return string.Empty;
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Resource";
      }
    }

    public XmlDocumentType(EditingService editingService)
      : base(editingService)
    {
    }
  }
}

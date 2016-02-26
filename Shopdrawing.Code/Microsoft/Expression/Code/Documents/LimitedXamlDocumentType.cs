// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.LimitedXamlDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class LimitedXamlDocumentType : LimitedDocumentType
  {
    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return true;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".xaml"
        };
      }
    }

    public override string Description
    {
      get
      {
        return "Limited Xaml";
      }
    }

    protected override string ImagePath
    {
      get
      {
        return "Documents\\file_xaml_on_16x16.png";
      }
    }

    public override string Name
    {
      get
      {
        return "LimitedXaml";
      }
    }

    public LimitedXamlDocumentType(EditingService editingService)
      : base(editingService)
    {
    }
  }
}

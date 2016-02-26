// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FSharpTemplateDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class FSharpTemplateDocumentType : CodeDocumentType
  {
    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return true;
      }
    }

    protected override string ImagePath
    {
      get
      {
        return "Documents\\FSharpTemplateDocumentType.png";
      }
    }

    public override string Name
    {
      get
      {
        return "F# Template";
      }
    }

    public override string Description
    {
      get
      {
        return "Visual F#";
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".fsi"
        };
      }
    }

    public FSharpTemplateDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }
  }
}

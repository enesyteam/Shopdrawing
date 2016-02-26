// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.HTMLDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Project;

namespace Microsoft.Expression.Code.Documents
{
  internal class HTMLDocumentType : LimitedDocumentType
  {
    public const string Buildtask = "Content";

    public override PreferredExternalEditCommand PreferredExternalEditCommand
    {
      get
      {
        return PreferredExternalEditCommand.ShellEdit;
      }
    }

    protected override string ImagePath
    {
      get
      {
        return "Resources\\HTML.png";
      }
    }

    public override string Name
    {
      get
      {
        return "HTML";
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Content";
      }
    }

    public override bool IsDefaultTypeForExtension
    {
      get
      {
        return true;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.HTMLDocumentTypeDescription;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[4]
        {
          ".html",
          ".htm",
          ".asp",
          ".aspx"
        };
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.HTMLDocumentTypeFileNameBase;
      }
    }

    public HTMLDocumentType(EditingService editingService)
      : base(editingService)
    {
    }

    public override bool CanAddToProject(IProject project)
    {
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.FSharpDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class FSharpDocumentType : CodeDocumentType, ICodeDocumentType, IDocumentType
  {
    private static ImageSource projectIconCache;

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
        return "Documents\\FSharpDocumentType.png";
      }
    }

    public override string Name
    {
      get
      {
        return "F#";
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
          ".fs"
        };
      }
    }

    public string ProjectFileExtension
    {
      get
      {
        return ".fsproj";
      }
    }

    public ImageSource ProjectIcon
    {
      get
      {
        if (FSharpDocumentType.projectIconCache == null)
        {
          FSharpDocumentType.projectIconCache = Microsoft.Expression.Code.FileTable.GetImageSource("Documents\\file_fsProject_on_16x16.png");
          FSharpDocumentType.projectIconCache.Freeze();
        }
        return FSharpDocumentType.projectIconCache;
      }
    }

    public string PropertiesPath
    {
      get
      {
        return string.Empty;
      }
    }

    public FSharpDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CPlusPlusDocumentType
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
  internal sealed class CPlusPlusDocumentType : CodeDocumentType, ICodeDocumentType, IDocumentType
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
        return "Documents\\CPlusPlusDocumentType.png";
      }
    }

    public override string Name
    {
      get
      {
        return "C++";
      }
    }

    public override string Description
    {
      get
      {
        return "Visual C++";
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[2]
        {
          ".cpp",
          ".cxx"
        };
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return "Code";
      }
    }

    public string ProjectFileExtension
    {
      get
      {
        return ".vcxproj";
      }
    }

    public ImageSource ProjectIcon
    {
      get
      {
        if (CPlusPlusDocumentType.projectIconCache == null)
        {
          CPlusPlusDocumentType.projectIconCache = Microsoft.Expression.Code.FileTable.GetImageSource("Documents\\file_cppProject_on_16x16.png");
          CPlusPlusDocumentType.projectIconCache.Freeze();
        }
        return CPlusPlusDocumentType.projectIconCache;
      }
    }

    public string PropertiesPath
    {
      get
      {
        return string.Empty;
      }
    }

    public CPlusPlusDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.JavascriptDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.JScript;
using System.CodeDom.Compiler;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Documents
{
  internal class JavascriptDocumentType : CodeDocumentType, ICodeDocumentType, IDocumentType, ICodeGeneratorHost
  {
    private const string BuildTask = "Content";
    private JScriptCodeProvider codeProvider;

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
        return "Resources\\JScript.png";
      }
    }

    public override string Name
    {
      get
      {
        return "Javascript";
      }
    }

    protected override string DefaultBuildTask
    {
      get
      {
        return "Content";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.JavascriptDocumentTypeDescription;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".js"
        };
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.JavascriptDocumentTypeFileNameBase;
      }
    }

    public string ProjectFileExtension
    {
      get
      {
        return (string) null;
      }
    }

    public ImageSource ProjectIcon
    {
      get
      {
        return Microsoft.Expression.Code.FileTable.GetImageSource("Documents\\file_website_on_16x16.png");
      }
    }

    public string PropertiesPath
    {
      get
      {
        return string.Empty;
      }
    }

    public override CodeDomProvider CodeDomProvider
    {
      get
      {
        if (this.codeProvider == null)
          this.codeProvider = new JScriptCodeProvider();
        return (CodeDomProvider) this.codeProvider;
      }
    }

    public JavascriptDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }

    public override bool CanAddToProject(IProject project)
    {
      return true;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.CSharpDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.CSharp;
using Microsoft.Expression.Code;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class CSharpDocumentType : CodeDocumentType, ICodeDocumentType, IDocumentType, ICodeGeneratorHost
  {
    private CSharpCodeProvider codeProvider;
    private static ImageSource projectIconCache;

    protected override string ImagePath
    {
      get
      {
        return "Documents\\CSharpDocumentType.png";
      }
    }

    public override string Name
    {
      get
      {
        return "C#";
      }
    }

    public override string Description
    {
      get
      {
        return "Visual C#";
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".cs"
        };
      }
    }

    public string ProjectFileExtension
    {
      get
      {
        return ".csproj";
      }
    }

    public ImageSource ProjectIcon
    {
      get
      {
        if (CSharpDocumentType.projectIconCache == null)
        {
          CSharpDocumentType.projectIconCache = Microsoft.Expression.Code.FileTable.GetImageSource("Documents\\file_csProject_on_16x16.png");
          CSharpDocumentType.projectIconCache.Freeze();
        }
        return CSharpDocumentType.projectIconCache;
      }
    }

    public string PropertiesPath
    {
      get
      {
        return "Properties\\";
      }
    }

    public override CodeDomProvider CodeDomProvider
    {
      get
      {
        if (this.codeProvider == null)
          this.codeProvider = new CSharpCodeProvider();
        return (CodeDomProvider) this.codeProvider;
      }
    }

    public override string ClassEndToken
    {
      get
      {
        return "}";
      }
    }

    public CSharpDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }

    public override string CreateMethod(string indent, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters, out int bodyInsertionOffset)
    {
      string str1 = indent + "private " + TypeNameFormatter.FormatTypeForCSharp(returnType, true) + " " + methodName + "(";
      bool flag = true;
      foreach (IParameterDeclaration parameter in parameters)
      {
        if (flag)
          flag = false;
        else
          str1 += ", ";
        str1 += this.FormatParameter(parameter);
      }
      string str2 = str1 + ")\r\n" + indent + "{\r\n" + indent + "\t";
      bodyInsertionOffset = str2.Length;
      return str2 + "// " + StringTable.EventHandlerTodoText + "\r\n" + indent + "}";
    }

    public override string FormatUsingDirective(string namespaceName)
    {
      return "using " + namespaceName + ";";
    }

    private string FormatParameter(IParameterDeclaration parameter)
    {
      return TypeNameFormatter.FormatTypeForCSharp(parameter.ParameterType, true) + " " + parameter.Name;
    }
  }
}

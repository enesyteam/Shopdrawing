// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Documents.VisualBasicDocumentType
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.Code.Documents
{
  internal sealed class VisualBasicDocumentType : CodeDocumentType, ICodeGeneratorHost, ICodeDocumentType, IDocumentType
  {
    private VBCodeProvider codeProvider;

    internal override bool SupportsGlobalImports
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
        return "Documents\\VisualBasicDocumentType.png";
      }
    }

    public override string Name
    {
      get
      {
        return "VB";
      }
    }

    public override string Description
    {
      get
      {
        return "Visual Basic";
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".vb"
        };
      }
    }

    public string LanguageSpecificTargets
    {
      get
      {
        return "Microsoft.VisualBasic.targets";
      }
    }

    public string ProjectFileExtension
    {
      get
      {
        return ".vbproj";
      }
    }

    public ImageSource ProjectIcon
    {
      get
      {
        return Microsoft.Expression.Code.FileTable.GetImageSource("Documents\\file_vbProject_on_16x16.png");
      }
    }

    public string PropertiesPath
    {
      get
      {
        return "My Project\\";
      }
    }

    public override CodeDomProvider CodeDomProvider
    {
      get
      {
        if (this.codeProvider == null)
          this.codeProvider = new VBCodeProvider();
        return (CodeDomProvider) this.codeProvider;
      }
    }

    public override string ClassEndToken
    {
      get
      {
        return "End Class";
      }
    }

    public VisualBasicDocumentType(ICodeProjectService codeProjectService, IViewService viewService, CodeOptionsModel codeOptionsModel, IWindowService windowService)
      : base(codeProjectService, viewService, codeOptionsModel, windowService)
    {
    }

    public override string CreateMethod(string indent, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters, out int bodyInsertionOffset)
    {
      string str1 = indent + "Private " + "Sub " + methodName + "(";
      bool flag = true;
      foreach (IParameterDeclaration parameterDeclaration in parameters)
      {
        if (flag)
          flag = false;
        else
          str1 += ", ";
        str1 = str1 + "ByVal " + parameterDeclaration.Name + " as " + TypeNameFormatter.FormatTypeForVisualBasic(parameterDeclaration.ParameterType, true);
      }
      string str2 = str1 + ")";
      if (returnType != typeof (void))
        str2 = str2 + " As " + TypeNameFormatter.FormatTypeForVisualBasic(returnType, true);
      string str3 = str2 + "\r\n" + indent + "\t";
      bodyInsertionOffset = str3.Length;
      return str3 + "'" + StringTable.EventHandlerTodoText + "\r\n" + indent + "End Sub";
    }

    public override string FormatUsingDirective(string namespaceName)
    {
      return "Imports " + namespaceName;
    }
  }
}

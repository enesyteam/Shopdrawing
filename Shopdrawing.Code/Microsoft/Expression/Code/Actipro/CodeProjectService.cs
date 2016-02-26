// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Actipro.CodeProjectService
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor;
using ActiproSoftware.SyntaxEditor.Addons.CSharp;
using ActiproSoftware.SyntaxEditor.Addons.Dynamic;
using ActiproSoftware.SyntaxEditor.Addons.VB;
using Microsoft.Expression.Code;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.Code.Actipro
{
  internal class CodeProjectService : ICodeProjectService
  {
    private Dictionary<IProject, ICodeProject> projectDictionary = new Dictionary<IProject, ICodeProject>();
    private static CSharpSyntaxLanguage csharpSyntaxLanguage;
    private static VBSyntaxLanguage vbSyntaxLanguage;
    private static DynamicSyntaxLanguage jsSyntaxLanguage;
    private static DynamicSyntaxLanguage cppSyntaxLanguage;
    private IProjectManager projectManager;
    private IAssemblyService assemblyService;
    private IMessageDisplayService messageDisplayService;
    private IViewService viewService;
    private IDocumentTypeManager documentTypeManager;

    public IMessageDisplayService MessageDisplayService
    {
      get
      {
        return this.messageDisplayService;
      }
    }

    internal CodeProjectService(IProjectManager projectManager, IAssemblyService assemblyService, IMessageDisplayService messageDisplayService, IViewService viewService, IDocumentTypeManager documentTypeManager)
    {
      this.projectManager = projectManager;
      this.assemblyService = assemblyService;
      this.messageDisplayService = messageDisplayService;
      this.viewService = viewService;
      this.documentTypeManager = documentTypeManager;
      if (this.projectManager == null)
        return;
      this.projectManager.ProjectClosed += new EventHandler<ProjectEventArgs>(this.OnProjectClosed);
    }

    public ICodeProject GetCodeProject(IProject project)
    {
      ICodeProject codeProject;
      if (!this.projectDictionary.TryGetValue(project, out codeProject))
      {
        codeProject = (ICodeProject) new CodeProject(project, this.assemblyService, (ICodeProjectService) this, this.projectManager.BuildManager, this.viewService, this.documentTypeManager);
        this.projectDictionary.Add(project, codeProject);
      }
      return codeProject;
    }

    public SyntaxLanguage GetSyntaxLanguage(string file)
    {
      string documentType = this.GetDocumentType(file);
      if (documentType == "text.C#")
      {
        if (CodeProjectService.csharpSyntaxLanguage == null)
          CodeProjectService.csharpSyntaxLanguage = new CSharpSyntaxLanguage();
        return (SyntaxLanguage) CodeProjectService.csharpSyntaxLanguage;
      }
      if (documentType == "text.VB")
      {
        if (CodeProjectService.vbSyntaxLanguage == null)
          CodeProjectService.vbSyntaxLanguage = new VBSyntaxLanguage();
        return (SyntaxLanguage) CodeProjectService.vbSyntaxLanguage;
      }
      if (documentType == "text.JS")
      {
        if (CodeProjectService.jsSyntaxLanguage == null)
        {
          using (Stream stream = (Stream) new MemoryStream(Microsoft.Expression.Code.FileTable.GetByteArray("Resources\\Actipro.JS.xml")))
            CodeProjectService.jsSyntaxLanguage = DynamicSyntaxLanguage.LoadFromXml(stream, 0);
        }
        return (SyntaxLanguage) CodeProjectService.jsSyntaxLanguage;
      }
      if (documentType == "text.C++")
      {
        if (CodeProjectService.cppSyntaxLanguage == null)
        {
          using (Stream stream = (Stream) new MemoryStream(Microsoft.Expression.Code.FileTable.GetByteArray("Resources\\Actipro.CPP.xml")))
          {
            try
            {
              CodeProjectService.cppSyntaxLanguage = DynamicSyntaxLanguage.LoadFromXml(stream, 0);
            }
            catch (Exception ex)
            {
              throw;
            }
          }
        }
        return (SyntaxLanguage) CodeProjectService.cppSyntaxLanguage;
      }
      if (documentType == "text")
        return SyntaxLanguage.PlainText;
      return (SyntaxLanguage) null;
    }

    private string GetDocumentType(string file)
    {
      string str = Path.GetExtension(file).ToUpperInvariant();
      if (str == null)
        return "text";
      switch (str)
      {
        case ".XAML":
        case ".XML":
          return "text.xml";
        case ".CS":
          return "text.C#";
        case ".VB":
          return "text.VB";
        case ".JS":
          return "text.JS";
        case ".CPP":
        case ".CXX":
        case ".H":
        case ".HPP":
        case ".HXX":
          return "text.C++";
        default:
          return "text";
      }
    }

    private void OnProjectClosed(object sender, ProjectEventArgs e)
    {
      ICodeProject codeProject;
      if (!this.projectDictionary.TryGetValue(e.Project, out codeProject))
        return;
      codeProject.Dispose();
      this.projectDictionary.Remove(e.Project);
    }
  }
}

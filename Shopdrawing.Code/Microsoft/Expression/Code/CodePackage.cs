// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodePackage
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.Code.UserInterface;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Code
{
  public sealed class CodePackage : IPackage
  {
    private IServices services;
    private IDocumentType csharpDocumentType;
    private IDocumentType visualBasicDocumentType;
    private IDocumentType javascriptDocumentType;
    private IDocumentType htmlDocumentType;
    private IDocumentType xmlDocumentType;
    private IDocumentType limitedXamlDocumentType;
    private IDocumentType fSharpDocumentType;
    private IDocumentType fSharpScriptDocumentType;
    private IDocumentType fSharpTemplateDocumentType;
    private IDocumentType cPlusPlusDocumentType;
    private IDocumentType headerDocumentType;
    private IDocumentType fxgDocumentType;
    private IOptionsPage codeOptionsPage;
    private CommandTarget commandTarget;
    private EditingService editingService;

    public void Load(IServices services)
    {
      this.services = services;
      ICommandService service1 = this.services.GetService<ICommandService>();
      this.services.GetService<IDocumentService>();
      IDocumentTypeManager service2 = this.services.GetService<IDocumentTypeManager>();
      IViewService service3 = this.services.GetService<IViewService>();
      IProjectManager service4 = this.services.GetService<IProjectManager>();
      IAssemblyService service5 = this.services.GetService<IAssemblyService>();
      IMessageDisplayService service6 = this.services.GetService<IMessageDisplayService>();
      IWindowService service7 = this.services.GetService<IWindowService>();
      CodeOptionsModel codeOptionsModel = new CodeOptionsModel();
      this.editingService = new EditingService((IServiceProvider) this.services, codeOptionsModel);
      this.services.AddService(typeof (ITextEditorService), (object) this.editingService);
      this.services.AddService(typeof (ITextBufferService), (object) this.editingService);
      ICodeProjectService codeProjectService = (ICodeProjectService) new CodeProjectService(service4, service5, service6, service3, service2);
      this.services.AddService(typeof (ICodeModelService), (object) new CodeModelService(this.services, codeProjectService));
      if (service2 != null)
      {
        this.csharpDocumentType = (IDocumentType) new CSharpDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.csharpDocumentType);
        this.visualBasicDocumentType = (IDocumentType) new VisualBasicDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.visualBasicDocumentType);
        this.javascriptDocumentType = (IDocumentType) new JavascriptDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.javascriptDocumentType);
        this.fSharpDocumentType = (IDocumentType) new FSharpDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.fSharpDocumentType);
        this.fSharpScriptDocumentType = (IDocumentType) new FSharpScriptDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.fSharpScriptDocumentType);
        this.fSharpTemplateDocumentType = (IDocumentType) new FSharpTemplateDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.fSharpTemplateDocumentType);
        this.cPlusPlusDocumentType = (IDocumentType) new CPlusPlusDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.cPlusPlusDocumentType);
        this.headerDocumentType = (IDocumentType) new HeaderDocumentType(codeProjectService, service3, codeOptionsModel, service7);
        service2.Register(this.headerDocumentType);
        this.htmlDocumentType = (IDocumentType) new HTMLDocumentType(this.editingService);
        service2.Register(this.htmlDocumentType);
        this.xmlDocumentType = (IDocumentType) new XmlDocumentType(this.editingService);
        service2.Register(this.xmlDocumentType);
        this.limitedXamlDocumentType = (IDocumentType) new LimitedXamlDocumentType(this.editingService);
        service2.Register(this.limitedXamlDocumentType);
        this.fxgDocumentType = (IDocumentType) new FxgDocumentType(this.editingService);
        service2.Register(this.fxgDocumentType);
      }
      IOptionsDialogService service8 = this.services.GetService<IOptionsDialogService>();
      if (service8 != null)
      {
        this.codeOptionsPage = (IOptionsPage) new CodeOptionsPage(this.editingService);
        service8.OptionsPages.Add(this.codeOptionsPage);
      }
      if (service1 == null)
        return;
      this.commandTarget = new CommandTarget();
      this.commandTarget.AddCommand("Project_EditVisualStudio", (ICommand) new EditVisualStudioCommand(service4, service1, service6));
      service1.AddTarget((ICommandTarget) this.commandTarget);
    }

    public void Unload()
    {
      this.services.RemoveService(typeof (ITextEditorService));
      this.services.RemoveService(typeof (ITextBufferService));
      this.services.RemoveService(typeof (ICodeModelService));
      this.services.RemoveService(typeof (ICodeProjectService));
      if (this.editingService != null)
      {
        this.editingService.Dispose();
        this.editingService = (EditingService) null;
      }
      if (this.commandTarget != null)
      {
        this.services.GetService<ICommandService>().RemoveTarget((ICommandTarget) this.commandTarget);
        this.commandTarget = (CommandTarget) null;
      }
      IDocumentTypeManager service = this.services.GetService<IDocumentTypeManager>();
      service.Unregister(this.csharpDocumentType);
      service.Unregister(this.visualBasicDocumentType);
      service.Unregister(this.javascriptDocumentType);
      service.Unregister(this.fSharpDocumentType);
      service.Unregister(this.fSharpScriptDocumentType);
      service.Unregister(this.fSharpTemplateDocumentType);
      service.Unregister(this.cPlusPlusDocumentType);
      service.Unregister(this.headerDocumentType);
      service.Unregister(this.htmlDocumentType);
      service.Unregister(this.xmlDocumentType);
      service.Unregister(this.limitedXamlDocumentType);
      service.Unregister(this.fxgDocumentType);
      this.services.GetService<IOptionsDialogService>().OptionsPages.Remove(this.codeOptionsPage);
    }
  }
}

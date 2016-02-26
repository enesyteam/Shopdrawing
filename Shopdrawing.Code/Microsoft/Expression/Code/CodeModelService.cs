// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeModelService
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using ActiproSoftware.SyntaxEditor.Addons.DotNet.Dom;
using Microsoft.Expression.Code.Actipro;
using Microsoft.Expression.Code.Documents;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Project;
using Microsoft.Expression.VisualStudioAutomation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Expression.Code
{
  internal class CodeModelService : ICodeModelService
  {
    private ICodeModelService visualStudioCodeModelService;
    private ICodeModelService actiproCodeModelService;
    private IServices services;

    public bool ShouldSaveFiles
    {
      get
      {
        return this.ActiveService.ShouldSaveFiles;
      }
    }

    private ICodeModelService ActiveService
    {
      get
      {
        if (!this.ProjectManager.OptionsModel.UseVisualStudioEventHandlerSupport || this.visualStudioCodeModelService == null)
          return this.actiproCodeModelService;
        return this.visualStudioCodeModelService;
      }
    }

    private IProjectManager ProjectManager
    {
      get
      {
        return this.services.GetService<IProjectManager>();
      }
    }

    public CodeModelService(IServices services, ICodeProjectService codeProjectService)
    {
      this.visualStudioCodeModelService = (ICodeModelService) new CodeModelService.VisualStudioCodeModelService(services);
      this.actiproCodeModelService = (ICodeModelService) new CodeModelService.ActiproCodeModelService(codeProjectService);
      this.services = services;
    }

    public ITypeDeclaration FindTypeInFile(object solution, object projectItem, IEnumerable<string> locations, string typeName)
    {
      CodeModelService.VisualStudioCodeModelService codeModelService = this.ActiveService as CodeModelService.VisualStudioCodeModelService;
      if (codeModelService == null || codeModelService.Exists)
        return this.ActiveService.FindTypeInFile(solution, projectItem, locations, typeName);
      this.visualStudioCodeModelService = (ICodeModelService) null;
      this.ProjectManager.OptionsModel.UseVisualStudioEventHandlerSupport = false;
      IConfigurationService service = this.services.GetService<IConfigurationService>();
      if (service != null)
      {
        this.ProjectManager.OptionsModel.Save(service["Options.ProjectSystemOptions"]);
        service.Save();
      }
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() => this.services.GetService<IMessageDisplayService>().ShowError(StringTable.UnableToFindVisualStudio)));
      return this.actiproCodeModelService.FindTypeInFile(solution, projectItem, locations, typeName);
    }

    private class VisualStudioCodeModelService : ICodeModelService
    {
      private const VSLaunchFlags launchFlags = VSLaunchFlags.DoNotClose | VSLaunchFlags.ShowUI;
      private IServices services;

      public bool Exists
      {
        get
        {
          return VSAutomation.SolutionModelProvider != null;
        }
      }

      public bool ShouldSaveFiles
      {
        get
        {
          return true;
        }
      }

      public VisualStudioCodeModelService(IServices services)
      {
        this.services = services;
      }

      public ITypeDeclaration FindTypeInFile(object solution, object projectItemObject, IEnumerable<string> locations, string typeName)
      {
        ISolutionModelProvider solutionModelProvider = VSAutomation.SolutionModelProvider;
        if (solutionModelProvider != null)
        {
          ISolution solution1 = (ISolution) solution;
          SolutionModel solutionModel = solutionModelProvider.FromSolution(solution1.DocumentReference.Path, VSLaunchFlags.DoNotClose | VSLaunchFlags.ShowUI, solution1.VersionNumber);
          if (solutionModel != null)
            return this.FindType(solutionModel, locations, typeName);
        }
        return (ITypeDeclaration) null;
      }

      private ITypeDeclaration FindType(SolutionModel solutionModel, IEnumerable<string> locations, string typeName)
      {
        ITypeDeclaration @class = solutionModel.GetClass(locations, typeName);
        if (@class != null)
          return (ITypeDeclaration) new CodeModelService.VisualStudioCodeModelService.MessageFilterTypeDeclaration(this.services, @class);
        return (ITypeDeclaration) null;
      }

      private class MessageFilterTypeDeclaration : ITypeDeclaration
      {
        private IServices services;
        private ITypeDeclaration typeDeclaration;

        public MessageFilterTypeDeclaration(IServices services, ITypeDeclaration typeDeclaration)
        {
          this.services = services;
          this.typeDeclaration = typeDeclaration;
        }

        public bool AddMethod(Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters)
        {
          CodeModelService.VisualStudioCodeModelService.MessageFilterTypeDeclaration.AddHandlerImplementation handlerImplementation = new CodeModelService.VisualStudioCodeModelService.MessageFilterTypeDeclaration.AddHandlerImplementation(this.services.GetService<IExpressionInformationService>(), this.typeDeclaration, returnType, methodName, parameters);
          handlerImplementation.Execute();
          Exception exception = handlerImplementation.Exception;
          if (exception == null)
            return true;
          bool flag = false;
          COMException comException = exception as COMException;
          if (comException != null && comException.ErrorCode == -2147221492)
            flag = true;
          IMessageDisplayService service = this.services.GetService<IMessageDisplayService>();
          if (service != null)
          {
            if (flag)
              service.ShowError(StringTable.EventHandlerCommunicatingWithVSTerminatedDialogMessage);
            else
              service.ShowError(StringTable.EventHandlerChangeFailedDialogMessage, exception, (string) null);
          }
          return false;
        }

        private sealed class AddHandlerImplementation
        {
          private const int MillisecondsBeforeMessageWindow = 3000;
          private MessageWindow messageWindow;
          private Thread thread;
          private IExpressionInformationService expressionInformationService;
          private ITypeDeclaration classModel;
          private string methodName;
          private IEnumerable<IParameterDeclaration> parameters;
          private Type returnType;
          private Exception exception;

          public Exception Exception
          {
            get
            {
              return this.exception;
            }
          }

          public AddHandlerImplementation(IExpressionInformationService expressionInformationService, ITypeDeclaration classModel, Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters)
          {
            this.expressionInformationService = expressionInformationService;
            this.classModel = classModel;
            this.returnType = returnType;
            this.methodName = methodName;
            this.parameters = parameters;
            this.thread = new Thread(new ThreadStart(this.ThreadExecute));
            this.thread.SetApartmentState(ApartmentState.STA);
          }

          public void Execute()
          {
            this.thread.Start();
            if (!this.thread.Join(3000))
            {
              this.messageWindow = new MessageWindow(Dialog.ActiveModalWindow, MessageIcon.None, StringTable.EventHandlerBusyTitle, string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.EventHandlerBusyMessage, new object[2]
              {
                (object) this.expressionInformationService.LongApplicationName,
                (object) this.expressionInformationService.ShortApplicationName
              }), MessageChoice.Cancel);
              this.messageWindow.Closed += new EventHandler(this.MessageWindow_Closed);
              this.messageWindow.ShowDialog();
            }
            this.thread = (Thread) null;
          }

          private void ThreadExecute()
          {
            try
            {
              using (new MessageFilter())
                this.classModel.AddMethod(this.returnType, this.methodName, this.parameters);
            }
            catch (Exception ex)
            {
              this.exception = ex;
            }
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
            {
              if (this.messageWindow == null)
                return;
              this.messageWindow.Close(MessageChoice.None);
            }));
          }

          private void MessageWindow_Closed(object sender, EventArgs e)
          {
            if (this.thread != null)
            {
              try
              {
                this.thread.Abort();
              }
              catch (ThreadStateException ex)
              {
              }
              this.thread = (Thread) null;
            }
            this.messageWindow = (MessageWindow) null;
          }
        }
      }
    }

    private class ActiproCodeModelService : ICodeModelService
    {
      private ICodeProjectService codeProjectService;

      public bool ShouldSaveFiles
      {
        get
        {
          return false;
        }
      }

      public ActiproCodeModelService(ICodeProjectService codeProjectService)
      {
        this.codeProjectService = codeProjectService;
      }

      public ITypeDeclaration FindTypeInFile(object solution, object projectItemObject, IEnumerable<string> locations, string typeName)
      {
        IProjectItem projectItem = projectItemObject as IProjectItem;
        if (projectItem == null)
          return (ITypeDeclaration) null;
        ICodeProject codeProject = this.codeProjectService.GetCodeProject(projectItem.Project);
        return (ITypeDeclaration) new CodeModelService.ActiproCodeModelService.TypeWrapper(typeName, projectItem, codeProject);
      }

      private class TypeWrapper : ITypeDeclaration
      {
        private string typeName;
        private IProjectItem projectItem;
        private ICodeProject codeProject;

        public TypeWrapper(string typeName, IProjectItem projectItem, ICodeProject codeProject)
        {
          this.typeName = typeName;
          this.projectItem = projectItem;
          this.codeProject = codeProject;
        }

        public bool AddMethod(Type returnType, string methodName, IEnumerable<IParameterDeclaration> parameters)
        {
          CodeView codeView = this.projectItem.OpenView(true) as CodeView;
          if (codeView != null)
          {
            IDomType domType = this.InitializeDomType();
            if (domType != null && codeView.CreateMethod(domType, returnType, methodName, parameters))
              return true;
          }
          return false;
        }

        private IDomType InitializeDomType()
        {
          this.projectItem.OpenDocument(false);
          CSharpExtendedSyntaxLanguage.ForceSynchronousReparse(((CodeDocument) this.projectItem.Document).Document);
          MSBuildBasedProject buildBasedProject = this.projectItem.Project as MSBuildBasedProject;
          string str = (string) null;
          if (buildBasedProject != null && buildBasedProject.HasProperty("RootNamespace"))
            str = buildBasedProject.GetEvaluatedPropertyValue("RootNamespace");
          IDomType type = this.codeProject.ProjectResolver.SourceProjectContent.GetType((IDomType[]) null, this.typeName, DomBindingFlags.AllAccessTypes);
          if (type == null && !string.IsNullOrEmpty(str) && this.typeName.StartsWith(str + ".", StringComparison.Ordinal))
            type = this.codeProject.ProjectResolver.SourceProjectContent.GetType((IDomType[]) null, this.typeName.Substring(str.Length + 1), DomBindingFlags.AllAccessTypes);
          return type;
        }
      }
    }
  }
}

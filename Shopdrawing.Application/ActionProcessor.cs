// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.ActionProcessor
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Shopdrawing.App
{
  internal class ActionProcessor
  {
    private static readonly string xmlNamespace = "http://schemas.microsoft.com/expression/blend/2010";
    private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    private void RegisterCommands(IServices services)
    {
      this.commands["ImportSampleDataFromXml"] = (ICommand) new ActionProcessor.ImportSampleDataCommand(services);
      this.commands["ImportPowerPoint"] = (ICommand) new ActionProcessor.ImportDocumentCommand(services, "Whitecap_ImportFromPowerPoint");
      this.commands["ImportIllustrator"] = (ICommand) new ActionProcessor.ImportDocumentCommand(services, "Application_Import_Illustrator");
      this.commands["ImportPhotoshop"] = (ICommand) new ActionProcessor.ImportDocumentCommand(services, "Application_Import_Photoshop");
      this.commands["AddNewItem"] = (ICommand) new ActionProcessor.ActionProcessorAddItem(services);
      this.commands["CreateNewProject"] = (ICommand) new ActionProcessor.ActionProcessorInvokeCommand(services, "Application_NewProject");
    }

    public void ProcessActionFile(string path, IServices services)
    {
      XmlElement documentElement;
      try
      {
        IProjectManager projectManager = (IProjectManager) services.GetService(typeof (IProjectManager));
        if (projectManager == null || projectManager.CurrentSolution == null)
          return;
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(path);
        documentElement = xmlDocument.DocumentElement;
        if (documentElement.NamespaceURI != ActionProcessor.xmlNamespace || documentElement.LocalName != "Actions")
          return;
        this.RegisterCommands(services);
      }
      catch
      {
        return;
      }
      this.ExecuteCommands(documentElement);
    }

    private void ExecuteCommands(XmlElement rootElement)
    {
      foreach (XmlNode xmlNode in rootElement.ChildNodes)
      {
        XmlElement commandElement = xmlNode as XmlElement;
        ICommand command;
        try
        {
          command = this.LoadCommand(commandElement);
        }
        catch
        {
          command = (ICommand) null;
        }
        if (command != null)
          command.Execute();
      }
    }

    private ICommand LoadCommand(XmlElement commandElement)
    {
      if (commandElement == null || commandElement.NamespaceURI != ActionProcessor.xmlNamespace || commandElement.LocalName != "Command")
        return (ICommand) null;
      XmlAttribute xmlAttribute1 = commandElement.Attributes["Name"];
      if (xmlAttribute1 == null || !string.IsNullOrEmpty(xmlAttribute1.NamespaceURI))
        return (ICommand) null;
      string key = xmlAttribute1.Value;
      if (string.IsNullOrEmpty(key))
        return (ICommand) null;
      ICommand command;
      if (!this.commands.TryGetValue(key, out command))
        return (ICommand) null;
      Type type = command.GetType();
      foreach (XmlAttribute xmlAttribute2 in (XmlNamedNodeMap) commandElement.Attributes)
      {
        if (xmlAttribute2 != xmlAttribute1 && string.IsNullOrEmpty(xmlAttribute2.NamespaceURI))
        {
          PropertyInfo property = type.GetProperty(xmlAttribute2.LocalName);
          if (property != (PropertyInfo) null)
            property.SetValue((object) command, (object) xmlAttribute2.Value, (object[]) null);
        }
      }
      return command;
    }

    private abstract class ActionProcessorCommandBase : Command
    {
      protected IServices services;

      public IProjectManager ProjectManager
      {
        get
        {
          return (IProjectManager) this.services.GetService(typeof (IProjectManager));
        }
      }

      public IProject ValidXamlProject
      {
        get
        {
          IProjectManager projectManager = this.ProjectManager;
          if (projectManager != null && projectManager.CurrentSolution != null)
          {
            foreach (IProject project in projectManager.CurrentSolution.Projects)
            {
              if (ProjectContext.FromProject(project) != null)
                return project;
            }
          }
          return (IProject) null;
        }
      }

      public ActionProcessorCommandBase(IServices services)
      {
        this.services = services;
      }
    }

    private class ImportSampleDataCommand : ActionProcessor.ActionProcessorCommandBase
    {
      public string XmlFilePath { get; set; }

      public string SampleDataName { get; set; }

      private SampleDataCollection SampleData
      {
        get
        {
          IProjectManager projectManager = this.ProjectManager;
          if (projectManager == null)
            return (SampleDataCollection) null;
          ISolution currentSolution = projectManager.CurrentSolution;
          if (currentSolution == null)
            return (SampleDataCollection) null;
          foreach (IProject project in currentSolution.Projects)
          {
            IProjectContext projectContext = ProjectContext.FromProject(project);
            if (projectContext != null)
            {
              SampleDataCollection sampleDataCollection = (SampleDataCollection) projectContext.GetService(typeof (SampleDataCollection));
              if (sampleDataCollection != null)
                return sampleDataCollection;
            }
          }
          return (SampleDataCollection) null;
        }
      }

      public ImportSampleDataCommand(IServices services)
        : base(services)
      {
      }

      public override void Execute()
      {
        SampleDataCollection sampleData = this.SampleData;
        if (sampleData == null)
          return;
        try
        {
          sampleData.ImportSampleDataFromXmlFile(this.SampleDataName, this.XmlFilePath);
        }
        catch (Exception ex)
        {
          this.services.GetService<IMessageDisplayService>().ShowError(ex.Message);
        }
      }
    }

    private class ActionProcessorInvokeCommand : ActionProcessor.ActionProcessorCommandBase
    {
      protected string executionCommand;

      public ActionProcessorInvokeCommand(IServices services, string executionCommand)
        : base(services)
      {
        this.executionCommand = executionCommand;
      }

      public override void Execute()
      {
        if (string.IsNullOrEmpty(this.executionCommand))
          return;
        (this.services.GetService(typeof (ICommandService)) as ICommandService).Execute(this.executionCommand, CommandInvocationSource.Internally);
      }
    }

    private class ImportDocumentCommand : ActionProcessor.ActionProcessorInvokeCommand
    {
      public string FilePath { get; set; }

      public ImportDocumentCommand(IServices services, string executionCommand)
        : base(services, executionCommand)
      {
      }

      public override void Execute()
      {
        if (string.IsNullOrEmpty(this.FilePath))
          return;
        ICommandService commandService = this.services.GetService(typeof (ICommandService)) as ICommandService;
        commandService.SetCommandProperty(this.executionCommand, "FilePath", (object) this.FilePath);
        commandService.Execute(this.executionCommand, CommandInvocationSource.Internally);
        commandService.SetCommandProperty(this.executionCommand, "FilePath", (object) null);
      }
    }

    private class ActionProcessorAddItem : ActionProcessor.ActionProcessorCommandBase
    {
      public string TemplateName { get; set; }

      public string FileName { get; set; }

      public ActionProcessorAddItem(IServices services)
        : base(services)
      {
      }

      public override void Execute()
      {
        IProject validXamlProject = this.ValidXamlProject;
        if (validXamlProject == null || string.IsNullOrEmpty(this.FileName))
          return;
        List<IProjectItem> itemsToOpen = new List<IProjectItem>();
        TemplateItemHelper templateItemHelper = new TemplateItemHelper(validXamlProject, (IList<string>) null, (IServiceProvider) this.services);
        IProjectItemTemplate templateItem = templateItemHelper.FindTemplateItem(this.TemplateName);
        string targetFolder = this.ProjectManager.TargetFolderForProject(validXamlProject);
        if (templateItem == null)
          return;
        if (targetFolder == null)
          return;
        try
        {
          templateItemHelper.AddProjectItemsForTemplateItem(templateItem, Path.GetFileName(this.FileName), targetFolder, CreationOptions.DoNotSelectCreatedItems, out itemsToOpen);
        }
        catch (ArgumentException ex)
        {
        }
      }
    }
  }
}

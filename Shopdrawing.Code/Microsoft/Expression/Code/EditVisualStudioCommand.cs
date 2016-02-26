// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.EditVisualStudioCommand
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using Microsoft.Expression.VisualStudioAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Expression.Code
{
  internal sealed class EditVisualStudioCommand : Command
  {
    private ISolutionModelProvider solutionModelProvider;
    private IProjectManager projectManager;
    private IMessageDisplayService messageDisplayService;
    private ICommandService commandService;

    public override bool IsEnabled
    {
      get
      {
        if (this.solutionModelProvider == null)
          this.solutionModelProvider = VSAutomation.SolutionModelProvider;
        if (this.solutionModelProvider == null || !this.solutionModelProvider.IsAvailable || !base.IsEnabled)
          return false;
        IProjectItem projectItem;
        return Enumerable.Any<IDocumentItem>(Enumerable.Where<IDocumentItem>(this.projectManager.ItemSelectionSet.Selection, (Func<IDocumentItem, bool>) (selectedItem =>
        {
          if (!(selectedItem is INamedProject) && ((projectItem = selectedItem as IProjectItem) == null || !projectItem.DocumentType.AllowVisualStudioEdit))
            return selectedItem.GetType().IsDefined(typeof (VisualStudioSolutionAttribute), true);
          return true;
        })));
      }
    }

    public EditVisualStudioCommand(IProjectManager projectManager, ICommandService commandService, IMessageDisplayService messageDisplayService)
    {
      this.projectManager = projectManager;
      this.commandService = commandService;
      this.messageDisplayService = messageDisplayService;
    }

    public override void Execute()
    {
      if (!this.IsEnabled)
        return;
      this.commandService.Execute("Application_SaveAll", CommandInvocationSource.Internally);
      EditVisualStudioCommand.VisualStudioAsyncOpen visualStudioAsyncOpen = new EditVisualStudioCommand.VisualStudioAsyncOpen(this.messageDisplayService);
      Version version = new Version(0, 0);
      string solutionName = (string) null;
      if (this.projectManager.CurrentSolution != null)
      {
        if (this.projectManager.CurrentSolution.GetType().IsDefined(typeof (VisualStudioSolutionAttribute), true))
          solutionName = this.projectManager.CurrentSolution.DocumentReference.Path;
        version = this.projectManager.CurrentSolution.VersionNumber;
      }
      foreach (IDocumentItem documentItem in this.projectManager.ItemSelectionSet.Selection)
      {
        string projectItemName = (string) null;
        SolutionModel solutionModel;
        if (documentItem is INamedProject)
          solutionModel = this.solutionModelProvider.FromProject(solutionName, documentItem.DocumentReference.Path, VSLaunchFlags.DoNotClose | VSLaunchFlags.ShowUI, version);
        else if (documentItem.GetType().IsDefined(typeof (VisualStudioSolutionAttribute), true))
        {
          solutionModel = this.solutionModelProvider.FromSolution(documentItem.DocumentReference.Path, VSLaunchFlags.DoNotClose | VSLaunchFlags.ShowUI, version);
        }
        else
        {
          IProjectItem projectItem = documentItem as IProjectItem;
          if (projectItem != null && projectItem.DocumentType.AllowVisualStudioEdit)
          {
            solutionModel = this.solutionModelProvider.FromProject(solutionName, projectItem.Project.DocumentReference.Path, VSLaunchFlags.DoNotClose | VSLaunchFlags.ShowUI, version);
            projectItemName = documentItem.DocumentReference.DisplayName;
          }
          else
            continue;
        }
        visualStudioAsyncOpen.Add(solutionModel, projectItemName);
      }
      visualStudioAsyncOpen.Start();
    }

    private sealed class VisualStudioAsyncOpenData
    {
      private SolutionModel solutionModel;
      private string projectItemName;

      public SolutionModel SolutionModel
      {
        get
        {
          return this.solutionModel;
        }
      }

      public string ProjectItemName
      {
        get
        {
          return this.projectItemName;
        }
      }

      public VisualStudioAsyncOpenData(SolutionModel solutionModel, string projectItemName)
      {
        this.solutionModel = solutionModel;
        this.projectItemName = projectItemName;
      }
    }

    private sealed class VisualStudioAsyncOpen
    {
      private static bool opening;
      private IMessageDisplayService messageDisplayService;
      private List<EditVisualStudioCommand.VisualStudioAsyncOpenData> itemsToOpen;

      public VisualStudioAsyncOpen(IMessageDisplayService messageDisplayService)
      {
        this.messageDisplayService = messageDisplayService;
        this.itemsToOpen = new List<EditVisualStudioCommand.VisualStudioAsyncOpenData>();
      }

      public void Add(SolutionModel solutionModel, string projectItemName)
      {
        this.itemsToOpen.Add(new EditVisualStudioCommand.VisualStudioAsyncOpenData(solutionModel, projectItemName));
      }

      public void Start()
      {
        if (EditVisualStudioCommand.VisualStudioAsyncOpen.opening)
        {
          this.messageDisplayService.ShowError(StringTable.EditVisualStudioCommandBusy);
        }
        else
        {
          EditVisualStudioCommand.VisualStudioAsyncOpen.opening = true;
          Thread thread = (Thread) null;
          try
          {
            thread = new Thread(new ThreadStart(this.Open));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
          }
          catch (Exception ex)
          {
            if (thread == null || !thread.IsAlive)
              EditVisualStudioCommand.VisualStudioAsyncOpen.opening = false;
            this.displayException(ex);
          }
        }
      }

      private void Open()
      {
        foreach (EditVisualStudioCommand.VisualStudioAsyncOpenData studioAsyncOpenData in this.itemsToOpen)
        {
          try
          {
            using (new MessageFilter())
              studioAsyncOpenData.SolutionModel.OpenVisualStudio(studioAsyncOpenData.ProjectItemName);
          }
          catch (Exception ex)
          {
            EditVisualStudioCommand.VisualStudioAsyncOpen visualStudioAsyncOpen = this;
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() => visualStudioAsyncOpen.displayException(ex)));
          }
          finally
          {
            EditVisualStudioCommand.VisualStudioAsyncOpen.opening = false;
          }
        }
      }

      private void displayException(Exception e)
      {
        this.messageDisplayService.ShowError(new ErrorArgs()
        {
          Message = StringTable.EditVisualStudioCommandFailed,
          Exception = e
        });
      }
    }
  }
}

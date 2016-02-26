// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.WelcomeScreen
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Shopdrawing.App
{
  public class WelcomeScreen : INotifyPropertyChanged
  {
    private ObservableCollection<IProjectTemplate> samples = new ObservableCollection<IProjectTemplate>();
    private ObservableCollection<RecentProject> projects = new ObservableCollection<RecentProject>();
    private const string FriendlyNameAttributePrefix = "AssemblyTitleAttribute(\"";
    private const string FriendlyNameAttributePostfix = "\"";
    private DelegateCommand tutorialCommand;
    private DelegateCommand communityCommand;
    private DelegateCommand helpCommand;
    private DelegateCommand newProjectDialogCommand;
    private DelegateCommand openProjectDialogCommand;
    private DelegateCommand startBlendCommand;
    private bool showWelcomeScreen;
    private IProjectTemplate selectedSample;
    private RecentProject selectedProject;
    private int selectedTab;
    private IServices services;
    private Window dialogWindow;

    public DelegateCommand TutorialCommand
    {
      get
      {
        return this.tutorialCommand;
      }
    }

    public DelegateCommand CommunityCommand
    {
      get
      {
        return this.communityCommand;
      }
    }

    public DelegateCommand HelpCommand
    {
      get
      {
        return this.helpCommand;
      }
    }

    public DelegateCommand NewProjectDialogCommand
    {
      get
      {
        return this.newProjectDialogCommand;
      }
    }

    public DelegateCommand OpenProjectDialogCommand
    {
      get
      {
        return this.openProjectDialogCommand;
      }
    }

    public DelegateCommand StartBlendCommand
    {
      get
      {
        return this.startBlendCommand;
      }
    }

    public ObservableCollection<IProjectTemplate> Samples
    {
      get
      {
        return this.samples;
      }
    }

    public ObservableCollection<RecentProject> Projects
    {
      get
      {
        return this.projects;
      }
    }

    public bool ShowWelcomeScreen
    {
      get
      {
        return this.showWelcomeScreen;
      }
      set
      {
        if (value == this.showWelcomeScreen)
          return;
        this.showWelcomeScreen = value;
        this.services.GetService<IConfigurationService>()["WindowService"].SetProperty("SkipWelcomeScreen", (object) true);//(bool) (!value ? 1 : 0)
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("ShowWelcomeScreen"));
      }
    }

    public IProjectTemplate SelectedSample
    {
      get
      {
        return this.selectedSample;
      }
      set
      {
        if (value == this.selectedSample)
          return;
        this.selectedSample = value;
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("SelectedSample"));
      }
    }

    public RecentProject SelectedProject
    {
      get
      {
        return this.selectedProject;
      }
      set
      {
        if (value == this.selectedProject)
          return;
        this.selectedProject = value;
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("SelectedProject"));
      }
    }

    public int SelectedTab
    {
      get
      {
        return this.selectedTab;
      }
      set
      {
        if (value == this.selectedTab)
          return;
        this.selectedTab = value;
        this.services.GetService<IConfigurationService>()["WindowService"].SetProperty("DefaultWelcomeTab", (object) value);
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs("SelectedTab"));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private WelcomeScreen(IServices services)
    {
      this.services = services;
      IConfigurationService service = this.services.GetService<IConfigurationService>();
      this.showWelcomeScreen = !(bool) service["WindowService"].GetProperty("SkipWelcomeScreen", (object) false);
      this.selectedTab = (int) service["WindowService"].GetProperty("DefaultWelcomeTab", (object) 1);
      this.tutorialCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.TutorialCommandCore));
      this.communityCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CommunityCommandCore));
      this.helpCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.HelpCommandCore));
      this.newProjectDialogCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.NewProjectDialogCommandCore));
      this.openProjectDialogCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OpenProjectDialogCommandCore));
      this.startBlendCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.StartBlendCommandCore));
      foreach (IProjectTemplate projectTemplate in services.GetService<IProjectManager>().TemplateManager.SampleProjectTemplates)
        this.samples.Add(projectTemplate);
      this.RebuildRecentProjects();
      this.dialogWindow = (Window) FileTable.GetElement("WelcomeScreen\\WelcomeScreen.xaml");
      this.dialogWindow.SourceInitialized += new EventHandler(this.DialogWindow_SourceInitialized);
      this.dialogWindow.Title = StringTable.WelcomeScreenTitle;
      this.dialogWindow.DataContext = (object) this;
      this.dialogWindow.WindowStartupLocation = Dialog.ActiveModalWindow.WindowState != WindowState.Maximized ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
      this.dialogWindow.Owner = Dialog.ActiveModalWindow;
      this.dialogWindow.ShowInTaskbar = false;
      this.dialogWindow.KeyDown += new KeyEventHandler(this.HandleEscape);
      this.dialogWindow.MouseLeftButtonDown += new MouseButtonEventHandler(this.MoveWindow);
      this.dialogWindow.AllowsTransparency = true;
      ((ItemActivateListBox) this.dialogWindow.FindName("SamplesListBox")).ItemActivate += new EventHandler(this.SamplesListBox_ItemActivate);
      ((ItemActivateListBox) this.dialogWindow.FindName("ProjectsListBox")).ItemActivate += new EventHandler(this.ProjectsListBox_ItemActivate);
      this.dialogWindow.SetValue(Dialog.IsModalProperty, (object) true);
      this.dialogWindow.ShowDialog();
      this.dialogWindow.SetValue(Dialog.IsModalProperty, (object) false);
    }

    public static void Show(IServices services)
    {
      WelcomeScreen welcomeScreen = new WelcomeScreen(services);
    }

    private void DialogWindow_SourceInitialized(object sender, EventArgs e)
    {
      this.dialogWindow.SourceInitialized -= new EventHandler(this.DialogWindow_SourceInitialized);
      WindowHelper.UpdateWindowPlacement(this.dialogWindow);
    }

    private void RebuildRecentProjects()
    {
      IProjectManager service = this.services.GetService<IProjectManager>();
      this.projects.Clear();
      for (int index = 0; index < Math.Min(5, service.RecentProjects.Length); ++index)
        this.projects.Add(new RecentProject(service.RecentProjects[index]));
    }

    private void HandleEscape(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Escape)
        return;
      this.StartBlendCommandCore();
    }

    private void SamplesListBox_ItemActivate(object source, EventArgs e)
    {
      using (TemporaryCursor.SetWaitCursor())
        this.OpenSampleCommandCore();
    }

    private void ProjectsListBox_ItemActivate(object source, EventArgs e)
    {
      this.OpenProjectCommandCore();
    }

    private void MoveWindow(object source, EventArgs args)
    {
      this.dialogWindow.DragMove();
    }

    private void TutorialCommandCore()
    {
      this.services.GetService<ICommandService>().Execute("Application_Tutorials", CommandInvocationSource.DialogButton);
    }

    private void CommunityCommandCore()
    {
      this.services.GetService<ICommandService>().Execute("Application_Website", CommandInvocationSource.DialogButton);
    }

    private void HelpCommandCore()
    {
      this.services.GetService<ICommandService>().Execute("Application_HelpTopics", CommandInvocationSource.DialogButton);
    }

    private void OpenSampleCommandCore()
    {
      if (this.selectedSample == null)
        return;
      try
      {
        this.dialogWindow.Opacity = 0.0;
        UIThreadDispatcher.Instance.DoEvents();
        IProjectManager service = this.services.GetService<IProjectManager>();
        string path = Path.Combine(service.DefaultNewSamplePath, this.selectedSample.DefaultName);
        if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
        {
          this.OpenExistingSample(path, this.selectedSample, service);
        }
        else
        {
          if (!service.CloseSolution())
            return;
          List<TemplateArgument> list = new List<TemplateArgument>();
          service.CreateProjectTemplate(service.DefaultNewSamplePath, this.selectedSample.DefaultName, this.selectedSample, (IEnumerable<TemplateArgument>) list);
          this.dialogWindow.Close();
        }
      }
      finally
      {
        this.dialogWindow.Opacity = 1.0;
      }
    }

    private void OpenExistingSample(string path, IProjectTemplate sampleTemplate, IProjectManager projectManager)
    {
      try
      {
        this.dialogWindow.Opacity = 0.0;
        UIThreadDispatcher.Instance.DoEvents();
        if (!sampleTemplate.HasProjectFile)
        {
          if (projectManager.OpenSolution(DocumentReference.Create(path), false, true) == null)
            return;
          this.dialogWindow.Close();
        }
        else
        {
          string projectFilename = sampleTemplate.ProjectFilename;
          string path1 = (string) null;
          if (!string.IsNullOrEmpty(projectFilename))
          {
            string[] strArray = (string[]) null;
            try
            {
              strArray = Directory.GetFiles(path, projectFilename, SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException ex)
            {
            }
            if (strArray != null && strArray.Length != 0)
              path1 = strArray[0];
          }
          if (string.IsNullOrEmpty(path1) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path1) || projectManager.OpenSolution(DocumentReference.Create(path1), false, true) == null)
            return;
          this.dialogWindow.Close();
        }
      }
      finally
      {
        this.dialogWindow.Opacity = 1.0;
      }
    }

    private void OpenProjectCommandCore()
    {
      if (this.selectedProject == null)
        return;
      try
      {
        this.dialogWindow.Opacity = 0.0;
        UIThreadDispatcher.Instance.DoEvents();
        IProjectManager service = this.services.GetService<IProjectManager>();
        ISolution currentSolution = service.CurrentSolution;
        this.services.GetService<ICommandService>().Execute("Project_OpenRecentProject_" + this.projects.IndexOf(this.selectedProject).ToString((IFormatProvider) CultureInfo.InvariantCulture), CommandInvocationSource.DialogButton);
        if (service.CurrentSolution != currentSolution)
          this.dialogWindow.Close();
        else
          this.RebuildRecentProjects();
      }
      finally
      {
        this.dialogWindow.Opacity = 1.0;
      }
    }

    private void NewProjectDialogCommandCore()
    {
      try
      {
        this.dialogWindow.Opacity = 0.0;
        UIThreadDispatcher.Instance.DoEvents();
        IProjectManager service = this.services.GetService<IProjectManager>();
        ISolution currentSolution = service.CurrentSolution;
        this.services.GetService<ICommandService>().Execute("Application_NewProject", CommandInvocationSource.DialogButton);
        if (service.CurrentSolution == currentSolution)
          return;
        this.dialogWindow.Close();
      }
      finally
      {
        this.dialogWindow.Opacity = 1.0;
      }
    }

    private void OpenProjectDialogCommandCore()
    {
      try
      {
        this.dialogWindow.Opacity = 0.0;
        UIThreadDispatcher.Instance.DoEvents();
        IProjectManager service = this.services.GetService<IProjectManager>();
        ISolution currentSolution = service.CurrentSolution;
        this.services.GetService<ICommandService>().Execute("Project_OpenProject", CommandInvocationSource.DialogButton);
        if (service.CurrentSolution == currentSolution)
          return;
        this.dialogWindow.Close();
      }
      finally
      {
        this.dialogWindow.Opacity = 1.0;
      }
    }

    private void StartBlendCommandCore()
    {
      this.dialogWindow.Close();
    }
  }
}

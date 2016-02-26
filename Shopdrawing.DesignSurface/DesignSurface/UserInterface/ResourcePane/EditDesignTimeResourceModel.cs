// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.EditDesignTimeResourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class EditDesignTimeResourceModel : INotifyPropertyChanged
  {
    private static readonly string DoNotAskForDesignTimeResourcesAgain = "DoNotAskForDesignTimeResourcesAgain";
    private static readonly string TargetName = StringTable.DesignTimeResourcesDefaultFileName + ".xaml";
    private ObservableCollection<DesignTimeResourceContainer> designTimeResourceContainers = new ObservableCollection<DesignTimeResourceContainer>();
    private string targetPath;
    private IProjectItem designTimeResources;
    private string missingResourceName;
    private string fileName;
    private SolutionSettingsManager solutionSettingsManager;
    private EditDesignTimeResourceModelMode mode;
    private DesignTimeResourceContainer selected;
    private DesignerContext context;
    private IProject rootProject;
    private bool? doNotAskAgain;

    public ObservableCollection<DesignTimeResourceContainer> DesignTimeResourceContainers
    {
      get
      {
        return this.designTimeResourceContainers;
      }
    }

    public bool CanResolveDesignTimeResources
    {
      get
      {
        if (!Enumerable.Any<DesignTimeResourceContainer>((IEnumerable<DesignTimeResourceContainer>) this.DesignTimeResourceContainers))
          return false;
        if (this.designTimeResources != null)
          return this.designTimeResources.Document != null;
        return true;
      }
    }

    public DesignTimeResourceContainer Selected
    {
      get
      {
        return this.selected;
      }
      set
      {
        this.selected = value;
        this.OnPropertyChanged("Selected");
        this.OnPropertyChanged("IsValid");
      }
    }

    public bool IsValid
    {
      get
      {
        if (this.Selected != null)
          return true;
        bool? doNotAskAgain = this.DoNotAskAgain;
        if (doNotAskAgain.GetValueOrDefault())
          return doNotAskAgain.HasValue;
        return false;
      }
    }

    public EditDesignTimeResourceModelMode Mode
    {
      get
      {
        return this.mode;
      }
      set
      {
        this.mode = value;
        this.OnPropertyChanged("Mode");
      }
    }

    public string Text
    {
      get
      {
        switch (this.Mode)
        {
          case EditDesignTimeResourceModelMode.Manual:
            return StringTable.DesignTimeResourcesManualText;
          case EditDesignTimeResourceModelMode.Warning:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DesignTimeResourcesWarningText, new object[2]
            {
              (object) this.fileName,
              (object) this.missingResourceName
            });
          default:
            return string.Empty;
        }
      }
    }

    public bool? DoNotAskAgain
    {
      get
      {
        if (!this.doNotAskAgain.HasValue && this.solutionSettingsManager != null)
        {
          object projectProperty = this.solutionSettingsManager.GetProjectProperty((INamedProject) this.rootProject, EditDesignTimeResourceModel.DoNotAskForDesignTimeResourcesAgain);
          this.doNotAskAgain = projectProperty == null || !(projectProperty is bool) ? new bool?(false) : new bool?((bool) projectProperty);
        }
        return this.doNotAskAgain;
      }
      set
      {
        this.doNotAskAgain = value;
        this.OnPropertyChanged("DoNotAskAgain");
        this.OnPropertyChanged("IsValid");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public EditDesignTimeResourceModel(IProject rootProject, DesignerContext context, string fileName, EditDesignTimeResourceModelMode mode = EditDesignTimeResourceModelMode.Warning, string missingResourceName = null)
    {
      EditDesignTimeResourceModel timeResourceModel = this;
      if (rootProject == null)
        throw new ArgumentNullException("rootProject");
      if (context == null)
        throw new ArgumentNullException("context");
      this.mode = mode;
      this.rootProject = rootProject;
      this.context = context;
      this.fileName = fileName;
      this.missingResourceName = missingResourceName;
      IProjectManager service = this.context.Services.GetService<IProjectManager>();
      if (service != null)
      {
        ISolution currentSolution = service.CurrentSolution;
        if (currentSolution != null)
          this.solutionSettingsManager = currentSolution.SolutionSettingsManager;
      }
      this.targetPath = Path.Combine(rootProject.ProjectRoot.Path, rootProject.PropertiesPath);
      foreach (DocumentResourceContainer container in Enumerable.Select(Enumerable.ThenBy(Enumerable.OrderBy(Enumerable.Where(Enumerable.Select(Enumerable.Distinct<DocumentResourceContainer>((IEnumerable<DocumentResourceContainer>) context.ResourceManager.DocumentResourceContainers), container => new
      {
        container = container,
        doc = container.Document.XamlDocument
      }), param0 =>
      {
        if (param0.container.Document.ProjectDocumentType == ProjectDocumentType.ResourceDictionary && !param0.container.ProjectItem.ContainsDesignTimeResources && param0.doc != null)
          return param0.doc.IsEditable;
        return false;
      }), param0 => param0.container.ProjectItem.Project.Name), param0 => param0.container.Name), param0 => param0.container))
        this.designTimeResourceContainers.Add(new DesignTimeResourceContainer(container));
      this.designTimeResources = Enumerable.FirstOrDefault<IProjectItem>((IEnumerable<IProjectItem>) rootProject.Items, (Func<IProjectItem, bool>) (item =>
      {
        bool flag = Enumerable.Any<IProject>(rootProject.ReferencedProjects, (Func<IProject, bool>) (p => p.DocumentReference.GetHashCode() == item.DocumentReference.GetHashCode()));
        if (item.ContainsDesignTimeResources)
          return !flag;
        return false;
      }));
      if (this.designTimeResources == null)
        return;
      DocumentResourceContainer resourceContainer1 = Enumerable.FirstOrDefault<DocumentResourceContainer>((IEnumerable<DocumentResourceContainer>) context.ResourceManager.DocumentResourceContainers, (Func<DocumentResourceContainer, bool>) (container => container.ProjectItem == this.designTimeResources));
      if (resourceContainer1 == null)
        return;
      List<ResourceDictionaryItem> list = new List<ResourceDictionaryItem>();
      context.ResourceManager.FindAllReachableDictionaries((ResourceContainer) resourceContainer1, (ICollection<ResourceDictionaryItem>) list);
      foreach (ResourceDictionaryItem resourceDictionaryItem in list)
      {
        DocumentResourceContainer container = context.ResourceManager.FindResourceContainer(resourceDictionaryItem.DesignTimeSource) as DocumentResourceContainer;
        if (container != null)
        {
          DesignTimeResourceContainer resourceContainer2 = Enumerable.FirstOrDefault<DesignTimeResourceContainer>((IEnumerable<DesignTimeResourceContainer>) this.designTimeResourceContainers, (Func<DesignTimeResourceContainer, bool>) (c => c.Container == container));
          if (resourceContainer2 != null)
            this.designTimeResourceContainers.Remove(resourceContainer2);
        }
      }
    }

    public bool TryCreateDesignTimeResourceReference()
    {
      bool? nullable = this.doNotAskAgain;
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
        this.solutionSettingsManager.SetProjectProperty((INamedProject) this.rootProject, EditDesignTimeResourceModel.DoNotAskForDesignTimeResourcesAgain, (object) true);
      else
        this.solutionSettingsManager.ClearProjectProperty((INamedProject) this.rootProject, EditDesignTimeResourceModel.DoNotAskForDesignTimeResourcesAgain);
      if (this.Selected != null)
      {
        try
        {
          List<IProjectItem> itemsToOpen = new List<IProjectItem>();
          if (this.designTimeResources == null)
          {
            TemplateItemHelper templateItemHelper = new TemplateItemHelper(this.rootProject, (IList<string>) null, (IServiceProvider) this.context.Services);
            IProjectItemTemplate templateItem = templateItemHelper.FindTemplateItem("Resource Dictionary");
            string availableFilePath = ProjectPathHelper.GetAvailableFilePath(EditDesignTimeResourceModel.TargetName, this.targetPath, this.rootProject, true);
            this.designTimeResources = Enumerable.SingleOrDefault<IProjectItem>(templateItemHelper.AddProjectItemsForTemplateItem(templateItem, Path.GetFileName(availableFilePath), this.targetPath, CreationOptions.DoNotSelectCreatedItems | CreationOptions.DesignTimeResource, out itemsToOpen));
          }
          if (this.designTimeResources == null)
            return false;
          DocumentResourceContainer resourceContainer = new DocumentResourceContainer(this.context.ResourceManager, this.designTimeResources);
          try
          {
            if (resourceContainer.ProjectItem.Document == null)
              return false;
            IProject project = this.Selected.Container.ProjectItem.Project;
            if (project != this.rootProject && !Enumerable.Contains<IProject>(this.rootProject.ReferencedProjects, project))
            {
              IProjectItem projectItem = this.rootProject.AddProjectReference(project);
              if (projectItem == null)
                return false;
              projectItem.ContainsDesignTimeResources = true;
            }
            uint resourceChangeStamp = this.context.ResourceManager.ResourceChangeStamp;
            this.context.ResourceManager.LinkToResource((ResourceContainer) resourceContainer, this.Selected.Container.DocumentReference);
            this.context.ViewUpdateManager.RefreshViewUpdatesForDesignTimeResources(true);
            return this.context.ResourceManager.ResourceChangeStamp > resourceChangeStamp;
          }
          finally
          {
            resourceContainer.Close();
          }
        }
        catch (Exception ex)
        {
          if (ex is NotSupportedException || ErrorHandling.ShouldHandleExceptions(ex))
            this.context.Services.GetService<IMessageDisplayService>().ShowError(new ErrorArgs()
            {
              Exception = ex
            });
          else
            throw;
        }
      }
      return false;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

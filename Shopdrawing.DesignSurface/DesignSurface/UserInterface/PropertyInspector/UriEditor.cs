// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.UriEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class UriEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private Microsoft.Windows.Design.PropertyEditing.PropertyValue editingValue;
    private PropertyReferenceProperty editingProperty;
    private ICollectionView sourcesView;
    private IEnumerable<ProjectItemModel> projectItems;
    private static IDocumentType[] mediaDocuments;
    private static IDocumentType[] imageDocuments;
    private static IDocumentType[] fontDocuments;
    private static IDocumentType[] multiScaleDocuments;
    private static IDocumentType[] xamlDocuments;
    private static IDocumentType[] defaultDocuments;
    internal UriEditor UriEditorControl;
    private bool _contentLoaded;

    public bool CanAddFile
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          IDocumentType[] typesFromSelection = this.GetSupportedDocumentTypesFromSelection();
          if (typesFromSelection != null)
          {
            foreach (IDocumentType documentType in typesFromSelection)
            {
              if (documentType.CanAddToProject(sceneNodeProperty.SceneNodeObjectSet.DesignerContext.ActiveProject))
                return true;
            }
          }
        }
        return false;
      }
    }

    public ProjectItemModel ProjectItemSource
    {
      get
      {
        if (this.editingValue != null)
        {
          bool isMixed = false;
          SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
          if (this.editingProperty != null)
          {
            DocumentPrimitiveNode documentPrimitiveNode = sceneNodeProperty.GetLocalValueAsDocumentNode(true, out isMixed) as DocumentPrimitiveNode;
            if (documentPrimitiveNode != null)
            {
              Uri uriValue = documentPrimitiveNode.GetUriValue();
              if (uriValue != (Uri) null && !string.IsNullOrEmpty(uriValue.OriginalString))
              {
                string str1 = uriValue.OriginalString.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                foreach (ProjectItemModel projectItemModel in (IEnumerable) this.ProjectItemsView)
                {
                  string str2 = projectItemModel.RelativeUri.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                  if (projectItemModel.ProjectRelativeLocation.TrimStart('\\') == str1 || str2 == str1)
                    return projectItemModel;
                }
                return new ProjectItemModel(uriValue.OriginalString);
              }
            }
          }
        }
        return (ProjectItemModel) null;
      }
      set
      {
        if (this.editingValue == null)
          return;
        SceneNodeProperty sceneNodeProperty = this.editingValue.ParentProperty as SceneNodeProperty;
        if (sceneNodeProperty == null)
          return;
        string str;
        if (value.ProjectItem != null)
        {
          bool useProjectRelativeSyntax = ProjectNeutralTypes.Frame.IsAssignableFrom((ITypeId) sceneNodeProperty.DeclaringTypeId) || PlatformTypes.HyperlinkButton.IsAssignableFrom((ITypeId) sceneNodeProperty.DeclaringTypeId);
          str = this.DocumentContext.MakeResourceReference(value.ProjectItem.DocumentReference.Path, useProjectRelativeSyntax);
        }
        else
          str = value.ProjectRelativeLocation;
        if (string.IsNullOrEmpty(str))
        {
          sceneNodeProperty.ClearValue();
        }
        else
        {
          DocumentNode documentNode = (DocumentNode) new DocumentPrimitiveNode(this.DocumentContext, (ITypeId) sceneNodeProperty.PropertyTypeId, (IDocumentNodeValue) new DocumentNodeStringValue(str));
          sceneNodeProperty.SetValue((object) documentNode);
        }
      }
    }

    public ICollectionView ProjectItemsView
    {
      get
      {
        return this.sourcesView;
      }
    }

    private IDocumentContext DocumentContext
    {
      get
      {
        if (this.editingValue != null)
        {
          SceneNodeProperty sceneNodeProperty = this.editingValue.ParentProperty as SceneNodeProperty;
          if (sceneNodeProperty != null)
            return sceneNodeProperty.SceneNodeObjectSet.ViewModel.Document.DocumentContext;
        }
        return (IDocumentContext) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public UriEditor()
    {
      this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.UriEditor_DataContextChanged);
    }

    public static bool IsPropertySupportedOnType(PropertyReferenceProperty property, Type selectionType)
    {
      if (!PlatformTypes.ImageSource.IsAssignableFrom((ITypeId) property.PropertyTypeId) && !PlatformTypes.Uri.IsAssignableFrom((ITypeId) property.PropertyTypeId))
        return PlatformTypes.MultiScaleTileSource.IsAssignableFrom((ITypeId) property.PropertyTypeId);
      return true;
    }

    private void UriEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.editingValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.editingProperty = (PropertyReferenceProperty) null;
      }
      if (this.editingValue != null)
        this.editingProperty = (PropertyReferenceProperty) this.editingValue.ParentProperty;
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      this.Rebuild();
    }

    public IEnumerable<ProjectItemModel> GetProjectItems()
    {
      List<ProjectItemModel> list = new List<ProjectItemModel>();
      IProjectManager projectManager;
      IProject currentProject = this.GetCurrentProject(out projectManager);
      if (currentProject != null)
      {
        IEnumerable<IDocumentType> documentTypes = (IEnumerable<IDocumentType>) this.GetSupportedDocumentTypesFromSelection();
        if (documentTypes != UriEditor.multiScaleDocuments)
        {
          foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) currentProject.Items)
          {
            if (this.IsMatch(projectItem, documentTypes) && projectItem.Visible)
              list.Add(new ProjectItemModel(projectItem, this.DocumentContext, 0));
          }
        }
        foreach (ProjectItemModel projectItemModel in this.GetWebsiteItems(currentProject, projectManager))
        {
          if (this.IsMatch(projectItemModel.ProjectItem, documentTypes))
            list.Add(projectItemModel);
        }
      }
      return (IEnumerable<ProjectItemModel>) list;
    }

    private bool IsMatch(IProjectItem item, IEnumerable<IDocumentType> documentTypes)
    {
      bool flag = false;
      if (documentTypes == null)
      {
        flag = true;
      }
      else
      {
        foreach (object obj in documentTypes)
        {
          if (obj.GetType().IsAssignableFrom(item.DocumentType.GetType()))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private IEnumerable<ProjectItemModel> GetWebsiteItems(IProject currentProject, IProjectManager projectManager)
    {
      List<ProjectItemModel> list = new List<ProjectItemModel>();
      if (projectManager.CurrentSolution.StartupProject is IWebsiteProject)
      {
        IProject targetProject = projectManager.CurrentSolution.StartupProject as IProject;
        if (targetProject != null)
        {
          IProjectOutputReferenceResolver referenceResolver = projectManager.CurrentSolution as IProjectOutputReferenceResolver;
          if (referenceResolver != null)
          {
            IProjectOutputReferenceInformation outputReferenceInfo = referenceResolver.GetProjectOutputReferenceInfo(targetProject, currentProject);
            if (outputReferenceInfo != null)
            {
              string deploymentPath = outputReferenceInfo.CreateDeploymentPath(targetProject, currentProject);
              if (!string.IsNullOrEmpty(deploymentPath))
              {
                foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) targetProject.Items)
                {
                  if (projectItem.DocumentReference.Path.StartsWith(deploymentPath, StringComparison.OrdinalIgnoreCase))
                    list.Add(new ProjectItemModel(projectItem, this.DocumentContext, 1));
                }
              }
            }
          }
        }
      }
      return (IEnumerable<ProjectItemModel>) list;
    }

    private IProject GetCurrentProject()
    {
      IProjectManager projectManager;
      return this.GetCurrentProject(out projectManager);
    }

    private IProject GetCurrentProject(out IProjectManager projectManager)
    {
      projectManager = (IProjectManager) null;
      if (this.editingValue != null)
      {
        SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          SceneNodeObjectSet sceneNodeObjectSet = sceneNodeProperty.SceneNodeObjectSet;
          if (sceneNodeObjectSet != null && sceneNodeObjectSet.DocumentContext != null)
          {
            projectManager = sceneNodeObjectSet.DesignerContext.ProjectManager;
            return ProjectHelper.GetProject(sceneNodeObjectSet.DesignerContext.ProjectManager, sceneNodeObjectSet.DocumentContext);
          }
        }
      }
      return (IProject) null;
    }

    private void EnsureSupportedDocuments()
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
      if (sceneNodeProperty == null)
        return;
      IDocumentTypeManager documentTypeManager = sceneNodeProperty.SceneNodeObjectSet.DesignerContext.DocumentTypeManager;
      IDocumentType folderDocumentType = documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Folder];
      if (UriEditor.mediaDocuments == null)
        UriEditor.mediaDocuments = Enumerable.ToArray<IDocumentType>(Enumerable.Where<IDocumentType>((IEnumerable<IDocumentType>) documentTypeManager.DocumentTypes, (Func<IDocumentType, bool>) (doc => doc is MediaDocumentType)));
      if (UriEditor.imageDocuments == null)
        UriEditor.imageDocuments = Enumerable.ToArray<IDocumentType>(Enumerable.Where<IDocumentType>((IEnumerable<IDocumentType>) documentTypeManager.DocumentTypes, (Func<IDocumentType, bool>) (doc => doc is ImageDocumentType)));
      if (UriEditor.fontDocuments == null)
        UriEditor.fontDocuments = Enumerable.ToArray<IDocumentType>(Enumerable.Where<IDocumentType>((IEnumerable<IDocumentType>) documentTypeManager.DocumentTypes, (Func<IDocumentType, bool>) (doc => doc is FontDocumentType)));
      if (UriEditor.multiScaleDocuments == null)
        UriEditor.multiScaleDocuments = new IDocumentType[2]
        {
          documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Xml],
          documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.DeepZoom]
        };
      if (UriEditor.xamlDocuments == null)
        UriEditor.xamlDocuments = new IDocumentType[1]
        {
          documentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Xaml]
        };
      if (UriEditor.defaultDocuments != null)
        return;
      UriEditor.defaultDocuments = Enumerable.ToArray<IDocumentType>(Enumerable.Where<IDocumentType>((IEnumerable<IDocumentType>) documentTypeManager.DocumentTypes, (Func<IDocumentType, bool>) (doc =>
      {
        if (!(doc is AssemblyReferenceDocumentType) && !(doc is ICodeDocumentType))
          return doc != folderDocumentType;
        return false;
      })));
    }

    private IDocumentType[] GetSupportedDocumentTypesFromSelection()
    {
      if (this.editingProperty != null)
      {
        this.EnsureSupportedDocuments();
        ITypeId type = (ITypeId) ((SceneNodeProperty) this.editingProperty).SceneNodeObjectSet.ObjectTypeId;
        if (PlatformTypes.Uri.IsAssignableFrom((ITypeId) this.editingProperty.PropertyTypeId))
        {
          if (PlatformTypes.MediaElement.IsAssignableFrom(type))
            return UriEditor.mediaDocuments;
          if (PlatformTypes.Glyphs.IsAssignableFrom(type))
            return UriEditor.fontDocuments;
          if (ProjectNeutralTypes.PlaySoundAction.IsAssignableFrom(type))
            return UriEditor.mediaDocuments;
          if (ProjectNeutralTypes.Frame.IsAssignableFrom(type) || PlatformTypes.HyperlinkButton.IsAssignableFrom(type))
            return UriEditor.xamlDocuments;
        }
        else
        {
          if (PlatformTypes.ImageSource.IsAssignableFrom((ITypeId) this.editingProperty.PropertyTypeId))
            return UriEditor.imageDocuments;
          if (PlatformTypes.MultiScaleTileSource.IsAssignableFrom((ITypeId) this.editingProperty.PropertyTypeId))
            return UriEditor.multiScaleDocuments;
        }
      }
      return UriEditor.defaultDocuments;
    }

    private void OnEditingPropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void Rebuild()
    {
      this.projectItems = this.GetProjectItems();
      this.sourcesView = CollectionViewSource.GetDefaultView((object) this.projectItems);
      PropertyGroupDescription groupDescription = new PropertyGroupDescription();
      groupDescription.PropertyName = "ProjectName";
      this.sourcesView.GroupDescriptions.Add((GroupDescription) groupDescription);
      SortDescription sortDescription1 = new SortDescription("SortIndex", ListSortDirection.Ascending);
      SortDescription sortDescription2 = new SortDescription("DisplayName", ListSortDirection.Ascending);
      this.sourcesView.SortDescriptions.Add(sortDescription1);
      this.sourcesView.SortDescriptions.Add(sortDescription2);
      if (this.sourcesView.Groups.Count <= 1)
        this.sourcesView.GroupDescriptions.Remove((GroupDescription) groupDescription);
      if (this.editingProperty.IsMixedValue)
        return;
      this.OnPropertyChanged("ProjectItemSource");
      this.OnPropertyChanged("ProjectItemsView");
      this.OnPropertyChanged("CanAddFile");
    }

    private void OnAddItem(object sender, EventArgs e)
    {
      if (this.editingValue == null)
        return;
      SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
      if (sceneNodeProperty == null)
        return;
      SceneNodeObjectSet sceneNodeObjectSet = sceneNodeProperty.SceneNodeObjectSet;
      if (sceneNodeObjectSet == null)
        return;
      CommandTarget commandTarget = sceneNodeObjectSet.DesignerContext.ProjectManager as CommandTarget;
      if (commandTarget == null)
        return;
      string commandName = "Project_AddExistingItemOfType";
      IProject currentProject = this.GetCurrentProject();
      if (currentProject == null)
        return;
      IDocumentType[] documentTypeArray = this.GetSupportedDocumentTypesFromSelection();
      if (documentTypeArray == UriEditor.multiScaleDocuments)
      {
        List<IDocumentType> list = Enumerable.ToList<IDocumentType>((IEnumerable<IDocumentType>) documentTypeArray);
        list.Remove(sceneNodeObjectSet.DesignerContext.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.DeepZoom]);
        documentTypeArray = list.ToArray();
      }
      commandTarget.SetCommandProperty(commandName, "DocumentTypes", (object) documentTypeArray);
      commandTarget.SetCommandProperty(commandName, "Project", (object) currentProject);
      commandTarget.SetCommandProperty(commandName, "SelectAddedItems", (object) false);
      commandTarget.SetCommandProperty(commandName, "TargetFolder", (object) Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(sceneNodeObjectSet.ViewModel.Document.DocumentReference.Path));
      commandTarget.SetCommandProperty(commandName, "TemporarilyStopProjectPaneActivation", (object) true);
      using (WorkaroundPopup.LockOpen((DependencyObject) this))
        commandTarget.Execute(commandName, CommandInvocationSource.Palette);
      IProjectItem projectItem = EnumerableExtensions.SingleOrNull<IProjectItem>((IEnumerable<IProjectItem>) commandTarget.GetCommandProperty(commandName, "AddedProjectItems"));
      if (projectItem == null)
        return;
      this.ProjectItemSource = new ProjectItemModel(projectItem, this.DocumentContext, 0);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/urieditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UriEditorControl = (UriEditor) target;
          break;
        case 2:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnAddItem);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

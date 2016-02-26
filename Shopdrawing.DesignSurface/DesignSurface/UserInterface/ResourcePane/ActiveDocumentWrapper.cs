// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ActiveDocumentWrapper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ActiveDocumentWrapper : ResourceEntryBase
  {
    private SceneViewModel activeViewModel;
    private ResourceManager resourceManager;
    private bool hasErrors;

    public string Name
    {
      get
      {
        if (this.activeViewModel == null)
          return (string) null;
        return this.activeViewModel.Document.DocumentReference.DisplayName;
      }
    }

    public override object ToolTip
    {
      get
      {
        if (this.activeViewModel == null)
          return (object) null;
        return (object) this.activeViewModel.Document.DocumentReference.Path;
      }
    }

    public bool DocumentHasErrors
    {
      get
      {
        return this.hasErrors;
      }
      private set
      {
        if (this.hasErrors == value)
          return;
        this.hasErrors = value;
        this.OnPropertyChanged("DocumentHasErrors");
      }
    }

    public bool IsEnabled
    {
      get
      {
        if (this.activeViewModel != null)
        {
          SceneDocument document1 = this.activeViewModel.Document;
          if (document1 != null)
          {
            IProjectContext projectContext = document1.ProjectContext;
            if (projectContext != null)
            {
              IProjectDocument document2 = projectContext.GetDocument(document1.DocumentRoot);
              if (document2 != null && this.activeViewModel.DefaultView != null && (this.activeViewModel.DefaultView.IsDesignSurfaceVisible && document2.DocumentType != ProjectDocumentType.Application))
                return document2.DocumentType != ProjectDocumentType.ResourceDictionary;
            }
          }
        }
        return false;
      }
    }

    public ObservableCollection<ResourceContainer> Children
    {
      get
      {
        return this.resourceManager.LocalResourceContainers;
      }
    }

    protected override ResourceManager ResourceManager
    {
      get
      {
        return this.resourceManager;
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        return (DocumentNode) null;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        return (DocumentNodeMarker) null;
      }
    }

    protected override ResourceContainer DragDropTargetContainer
    {
      get
      {
        return this.resourceManager.ActiveRootContainer;
      }
    }

    public ActiveDocumentWrapper(ResourceManager resourceManager, SceneViewModel activeViewModel)
    {
      this.activeViewModel = activeViewModel;
      this.resourceManager = resourceManager;
      this.hasErrors = this.GetActiveDocumentHasErrors();
      this.IsExpanded = true;
    }

    public void Update(SceneViewModel activeViewModel)
    {
      if (this.activeViewModel != activeViewModel)
      {
        bool isEnabled = this.IsEnabled;
        bool isSelected = this.IsSelected;
        if (this.resourceManager.SelectedItems.IsSelected((ResourceEntryBase) this))
          this.resourceManager.SelectedItems.RemoveSelection((ResourceEntryBase) this);
        this.activeViewModel = activeViewModel;
        if (this.IsEnabled != isEnabled)
          this.IsExpanded = true;
        if (this.IsEnabled && isSelected)
          this.resourceManager.SelectedItems.ExtendSelection((ResourceEntryBase) this);
        this.hasErrors = this.GetActiveDocumentHasErrors();
        this.OnPropertyChanged((string) null);
      }
      else
        this.DocumentHasErrors = this.GetActiveDocumentHasErrors();
    }

    private bool GetActiveDocumentHasErrors()
    {
      if (this.activeViewModel != null)
        return !this.activeViewModel.Document.IsEditable;
      return false;
    }
  }
}

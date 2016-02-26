// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Breadcrumb
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class Breadcrumb : INotifyPropertyChanged
  {
    private SceneViewModel activeViewModel;
    private EditContext editContext;
    private DocumentNodePath selectedElementPath;
    private bool isGhosted;
    private bool hasPreviousBreadcrumb;
    private bool hasNextBreadcrumb;
    private bool nextBreadcrumbIsCrossDocument;

    public SceneViewModel LocalViewModel
    {
      get
      {
        return this.editContext.ViewModel;
      }
    }

    public string DisplayName
    {
      get
      {
        SceneElement selectedElement = this.SelectedElement;
        if (selectedElement != null && PlatformTypes.Style.IsAssignableFrom((ITypeId) selectedElement.Type))
          return (string) null;
        return this.Name;
      }
    }

    public string Name
    {
      get
      {
        if (this.IsActiveScope)
        {
          int count = this.ActiveViewModel.ElementSelectionSet.Count;
          if (count > 1)
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MultiElementSelectionNameFormat, new object[1]
            {
              (object) count
            });
          if (count == 0)
            return StringTable.NoElementSelectionName;
        }
        SceneElement selectedElement = this.SelectedElement;
        if (selectedElement != null)
          return selectedElement.GetDisplayNameFromPath(this.SelectedElementPath, false);
        return StringTable.NoElementSelectionName;
      }
    }

    public string ContainerDisplayName
    {
      get
      {
        SceneElement containerElement = this.EditingContainerElement;
        if (containerElement != null)
          return containerElement.ContainerDisplayName;
        return (string) null;
      }
    }

    public string DocumentName
    {
      get
      {
        return this.LocalViewModel.Document.DocumentReference.DisplayName;
      }
    }

    public ITypeId ContainerType
    {
      get
      {
        if (this.EditingContainerElement != null)
          return (ITypeId) this.EditingContainerElement.Type;
        return (ITypeId) null;
      }
    }

    public DrawingBrush IconBrush
    {
      get
      {
        ITypeId containerType = this.ContainerType;
        if (containerType != null && (PlatformTypes.FrameworkTemplate.IsAssignableFrom(containerType) || PlatformTypes.Style.IsAssignableFrom(containerType)))
          return IconMapper.GetDrawingBrushForType(containerType, false, 12, 12);
        return (DrawingBrush) null;
      }
    }

    public bool IsMenuEnabled
    {
      get
      {
        if (!this.IsActiveScope || this.ActiveViewModel.ElementSelectionSet.Count != 1)
          return false;
        SceneElement primarySelection = this.ActiveViewModel.ElementSelectionSet.PrimarySelection;
        ITypeId type = (ITypeId) primarySelection.Type;
        StyleNode styleNode = primarySelection as StyleNode;
        if (styleNode != null)
          type = (ITypeId) styleNode.StyleTargetTypeId;
        return PlatformTypes.Control.IsAssignableFrom(type) || PlatformTypes.Page.IsAssignableFrom(type);
      }
    }

    public bool IsGhosted
    {
      get
      {
        return this.isGhosted;
      }
    }

    public bool IsActiveScope
    {
      get
      {
        if (this.ActiveViewModel == this.LocalViewModel && this.ActiveViewModel.IsActiveSceneViewModel)
          return this.ActiveViewModel.ActiveEditingContainerPath == this.editContext.EditingContainerPath;
        return false;
      }
    }

    public bool HasPreviousBreadcrumb
    {
      get
      {
        return this.hasPreviousBreadcrumb;
      }
      set
      {
        this.hasPreviousBreadcrumb = value;
      }
    }

    public bool HasNextBreadcrumb
    {
      get
      {
        return this.hasNextBreadcrumb;
      }
      set
      {
        this.hasNextBreadcrumb = value;
      }
    }

    public bool NextBreadcrumbIsCrossDocument
    {
      get
      {
        return this.nextBreadcrumbIsCrossDocument;
      }
      set
      {
        this.nextBreadcrumbIsCrossDocument = value;
      }
    }

    public DocumentNodePath SelectedElementPath
    {
      get
      {
        if (!this.HasExplicitSelection)
          return EditContextHelper.SelectedElementPathInEditContext(this.LocalViewModel, this.editContext, false);
        return this.selectedElementPath;
      }
    }

    private bool HasExplicitSelection
    {
      get
      {
        return this.selectedElementPath != null;
      }
    }

    private SceneElement SelectedElement
    {
      get
      {
        DocumentNodePath selectedElementPath = this.SelectedElementPath;
        if (selectedElementPath != null)
          return this.LocalViewModel.GetSceneNode(selectedElementPath.Node) as SceneElement;
        return (SceneElement) null;
      }
    }

    private SceneElement EditingContainerElement
    {
      get
      {
        return this.editContext.EditingContainer as SceneElement;
      }
    }

    private SceneViewModel ActiveViewModel
    {
      get
      {
        return this.activeViewModel;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public Breadcrumb(SceneViewModel activeViewModel, EditContext editContext, DocumentNodePath selectedElementPath, bool isGhosted)
    {
      this.activeViewModel = activeViewModel;
      this.editContext = editContext;
      this.selectedElementPath = this.IsActiveScope ? (DocumentNodePath) null : selectedElementPath;
      this.isGhosted = isGhosted;
    }

    public void RefreshName()
    {
      this.OnPropertyChanged("Name");
      this.OnPropertyChanged("DisplayName");
    }

    public void RefreshIsMenuEnabled()
    {
      this.OnPropertyChanged("IsMenuEnabled");
    }

    public void Activate()
    {
      if (this.IsActiveScope)
        return;
      using (TemporaryCursor.SetWaitCursor())
      {
        this.ActiveViewModel.MoveToEditContext(this.editContext);
        if (!this.HasExplicitSelection)
          return;
        this.LocalViewModel.ElementSelectionSet.SetSelection(this.SelectedElement);
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      Breadcrumb breadcrumb = obj as Breadcrumb;
      if (breadcrumb != null && this.activeViewModel == breadcrumb.activeViewModel && (object.Equals((object) this.editContext, (object) breadcrumb.editContext) && object.Equals((object) this.selectedElementPath, (object) breadcrumb.selectedElementPath)) && (this.isGhosted == breadcrumb.isGhosted && this.nextBreadcrumbIsCrossDocument == breadcrumb.nextBreadcrumbIsCrossDocument && this.hasNextBreadcrumb == breadcrumb.hasNextBreadcrumb))
        return this.hasPreviousBreadcrumb == breadcrumb.hasPreviousBreadcrumb;
      return false;
    }

    public override int GetHashCode()
    {
      return this.editContext.GetHashCode();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

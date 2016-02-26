// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.BreadcrumbBarModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class BreadcrumbBarModel : INotifyPropertyChanged, IDisposable
  {
    private Dictionary<SceneViewModel, object> viewModels = new Dictionary<SceneViewModel, object>();
    private DesignerContext designerContext;
    private ObservableCollection<Breadcrumb> trail;

    public bool IsExpanded
    {
      get
      {
        return this.ActiveSceneViewModel != null;
      }
    }

    public ObservableCollection<Breadcrumb> Trail
    {
      get
      {
        return this.trail;
      }
      private set
      {
        bool flag = false;
        if (this.trail != null && this.trail.Count == value.Count)
        {
          for (int index = 0; index < this.trail.Count; ++index)
          {
            if (!this.trail[index].Equals((object) value[index]))
            {
              flag = true;
              break;
            }
          }
        }
        else
          flag = true;
        if (!flag)
          return;
        this.trail = value;
        this.OnPropertyChanged("Trail");
      }
    }

    private Dictionary<SceneViewModel, object> ViewModels
    {
      set
      {
        bool flag = false;
        if (this.viewModels.Count == value.Count)
        {
          foreach (KeyValuePair<SceneViewModel, object> keyValuePair in this.viewModels)
          {
            object obj;
            if (!value.TryGetValue(keyValuePair.Key, out obj))
            {
              flag = true;
              break;
            }
          }
        }
        else
          flag = true;
        if (!flag)
          return;
        this.UnregisterViewModelListeners();
        this.viewModels = value;
        this.RegisterViewModelListeners();
      }
    }

    private SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel == null || !activeSceneViewModel.IsEditable || !this.designerContext.ActiveView.IsDesignSurfaceEnabled)
          return (SceneViewModel) null;
        return activeSceneViewModel;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public BreadcrumbBarModel(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.Initialize();
    }

    private void Initialize()
    {
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.ViewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
    }

    public void Dispose()
    {
      this.UnregisterViewModelListeners();
      this.designerContext.ViewService.ViewClosed -= new ViewEventHandler(this.ViewService_ViewClosed);
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    private void RegisterViewModelListeners()
    {
      foreach (KeyValuePair<SceneViewModel, object> keyValuePair in this.viewModels)
        keyValuePair.Key.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_LateSceneUpdatePhase);
    }

    private void UnregisterViewModelListeners()
    {
      foreach (KeyValuePair<SceneViewModel, object> keyValuePair in this.viewModels)
        keyValuePair.Key.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ActiveSceneViewModel_LateSceneUpdatePhase);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs args)
    {
      this.OnPropertyChanged("IsExpanded");
      this.RefreshBreadcrumbTrail();
    }

    private void ViewService_ViewClosed(object sender, ViewEventArgs args)
    {
      SceneView sceneView = args.View as SceneView;
      SceneViewModel key = sceneView != null ? sceneView.ViewModel : (SceneViewModel) null;
      object obj;
      if (key == null || !this.viewModels.TryGetValue(key, out obj))
        return;
      this.RefreshBreadcrumbTrail();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.ActiveSceneViewModel != null && this.viewModels.Count == 0 || this.ActiveSceneViewModel == null && this.viewModels.Count != 0)
      {
        this.RefreshBreadcrumbTrail();
        this.OnPropertyChanged("IsExpanded");
      }
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer | SceneViewModel.ViewStateBits.InactiveEditingContainer | SceneViewModel.ViewStateBits.EditContextHistory | SceneViewModel.ViewStateBits.ElementSelection))
        this.RefreshBreadcrumbTrail();
      if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        return;
      foreach (Breadcrumb breadcrumb in (Collection<Breadcrumb>) this.Trail)
      {
        if (args.ViewModel == breadcrumb.LocalViewModel)
        {
          breadcrumb.RefreshName();
          breadcrumb.RefreshIsMenuEnabled();
        }
      }
    }

    private void ActiveSceneViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.DocumentChanges.Count <= 0)
        return;
      foreach (SceneChange sceneChange in SceneChange.Changes(args.DocumentChanges, args.ViewModel.RootNode, (Type) null))
      {
        PropertyReferenceSceneChange e = sceneChange as PropertyReferenceSceneChange;
        if (e != null)
          this.OnPropertyReferenceSceneChange(e);
      }
    }

    private void OnPropertyReferenceSceneChange(PropertyReferenceSceneChange e)
    {
      if (e.PropertyChanged == null || e.PropertyChanged.Count != 1 || e.Target == null)
        return;
      SceneElement targetElement = e.Target as SceneElement;
      if (targetElement == null)
        return;
      IPropertyId nameProperty = targetElement.NameProperty;
      if (nameProperty == null || !e.PropertyKey.Equals((object) nameProperty))
        return;
      Breadcrumb breadcrumb = this.FindBreadcrumb(targetElement);
      if (breadcrumb == null)
        return;
      breadcrumb.RefreshName();
    }

    private void RefreshBreadcrumbTrail()
    {
      ObservableCollection<Breadcrumb> newTrail = new ObservableCollection<Breadcrumb>();
      Dictionary<SceneViewModel, object> newViewModels = new Dictionary<SceneViewModel, object>();
      if (this.ActiveSceneViewModel != null)
      {
        this.ActiveSceneViewModel.EditContextManager.MultiViewModelEditContextWalker.Walk(false, (MultiHistoryCallback) ((context, selectedElementPath, ownerPropertyKey, isGhosted) =>
        {
          newViewModels[context.ViewModel] = (object) null;
          newTrail.Add(new Breadcrumb(this.ActiveSceneViewModel, context, selectedElementPath, isGhosted));
          return false;
        }));
        for (int index = 0; index < newTrail.Count; ++index)
        {
          Breadcrumb breadcrumb1 = newTrail[index];
          if ((index != 0 ? newTrail[index - 1] : (Breadcrumb) null) != null)
            breadcrumb1.HasPreviousBreadcrumb = true;
          Breadcrumb breadcrumb2 = index != newTrail.Count - 1 ? newTrail[index + 1] : (Breadcrumb) null;
          if (breadcrumb2 != null)
          {
            breadcrumb1.HasNextBreadcrumb = true;
            if (breadcrumb2.LocalViewModel != breadcrumb1.LocalViewModel)
              breadcrumb1.NextBreadcrumbIsCrossDocument = true;
          }
        }
      }
      this.Trail = newTrail;
      this.ViewModels = newViewModels;
    }

    private Breadcrumb FindBreadcrumb(SceneElement targetElement)
    {
      foreach (Breadcrumb breadcrumb in (Collection<Breadcrumb>) this.Trail)
      {
        DocumentNodePath selectedElementPath = breadcrumb.SelectedElementPath;
        if (selectedElementPath != null && selectedElementPath.Equals((object) targetElement.DocumentNodePath))
          return breadcrumb;
      }
      return (Breadcrumb) null;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

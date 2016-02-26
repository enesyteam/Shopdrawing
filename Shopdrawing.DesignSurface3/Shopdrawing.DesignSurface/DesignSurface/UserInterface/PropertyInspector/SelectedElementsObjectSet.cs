// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SelectedElementsObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SelectedElementsObjectSet : SceneNodeObjectSetBase
  {
    private bool selectedTypesStale = true;
    private List<Type> selectedTypes = new List<Type>();
    private uint documentResourcesChangeStamp;
    private SceneViewModel viewModel;

    public override SceneNode[] Objects
    {
      get
      {
        return this.DesignerContext.SelectionManager.SelectedNodes ?? new SceneNode[0];
      }
    }

    public override bool ShouldWalkParentsForGetValue
    {
      get
      {
        return true;
      }
    }

    public override bool ShouldAllowAnimation
    {
      get
      {
        if (this.Objects.Length == 0)
          return true;
        SceneNode sceneNode = this.Objects[0];
        if (!(sceneNode is KeyFrameAnimationSceneNode) && !(sceneNode is TimelineSceneNode) && (!(sceneNode is KeyFrameSceneNode) && !(sceneNode is FromToAnimationSceneNode)))
          return !(sceneNode is BehaviorBaseNode);
        return false;
      }
    }

    public override bool IsValidForUpdate
    {
      get
      {
        return true;
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        return this.SelectedTypes.Count <= 1;
      }
    }

    public int TypeCount
    {
      get
      {
        return this.SelectedTypes.Count;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    internal List<Type> SelectedTypes
    {
      get
      {
        if (this.selectedTypesStale)
          this.UpdateSelectedTypes();
        return this.selectedTypes;
      }
    }

    internal event EventHandler ViewModelChanged;

    internal SelectedElementsObjectSet(DesignerContext designerContext, IPropertyInspector transactionContext)
      : base(designerContext, transactionContext)
    {
      ObservableCollection<SystemResourceModel> systemResources = this.SystemResources;
      this.DesignerContext.SelectionManager.EarlyActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_EarlyActiveSceneUpdatePhase);
      this.DesignerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.SetActiveViewModel(designerContext.ActiveSceneViewModel);
    }

    public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null)
        return;
      this.DesignerContext.PropertyManager.RegisterPropertyReferenceChangedHandler(propertyReference, handler, true);
    }

    public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null)
        return;
      this.DesignerContext.PropertyManager.UnregisterPropertyReferenceChangedHandler(propertyReference, handler);
    }

    public void UpdateOnSelectionChanged()
    {
      this.InvalidateTemplateBindableProperties();
      this.selectedTypesStale = true;
    }

    internal void SetActiveViewModel(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      if (this.ViewModelChanged == null)
        return;
      this.ViewModelChanged((object) this, EventArgs.Empty);
    }

    private bool SelectionSetChanged(SceneUpdatePhaseEventArgs args)
    {
      return args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.TextSelection | SceneViewModel.ViewStateBits.KeyFrameSelection | SceneViewModel.ViewStateBits.StoryboardSelection | SceneViewModel.ViewStateBits.ChildPropertySelection | SceneViewModel.ViewStateBits.BehaviorSelection);
    }

    private void SelectionManager_EarlyActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!this.SelectionSetChanged(args))
        return;
      this.OnSetChanged();
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.DesignerContext.ResourceManager.OnSceneUpdate(args);
      if (!this.SelectionSetChanged(args) && (int) this.DesignerContext.ResourceManager.ResourceChangeStamp == (int) this.documentResourcesChangeStamp)
        return;
      this.InvalidateLocalResourcesCache();
    }

    protected override ObservableCollectionWorkaround<LocalResourceModel> RecalculateLocalResources(ObservableCollectionWorkaround<LocalResourceModel> currentResources)
    {
      this.documentResourcesChangeStamp = this.DesignerContext.ResourceManager.ResourceChangeStamp;
      if (this.DesignerContext.SelectionManager.SelectedNodes != null && this.DesignerContext.SelectionManager.SelectedNodes.Length != 0)
        return this.ProvideLocalResources(new List<ResourceContainer>(this.DesignerContext.ResourceManager.ActiveResourceContainers));
      if (currentResources == null)
        return new ObservableCollectionWorkaround<LocalResourceModel>();
      currentResources.Clear();
      return currentResources;
    }

    private void UpdateSelectedTypes()
    {
      object[] objArray = (object[]) this.Objects;
      this.selectedTypes.Clear();
      if (objArray.Length >= 1)
      {
        this.selectedTypes.Add(((SceneNode) objArray[0]).TargetType);
        SelectedElementsObjectSet.TypeComparer typeComparer = new SelectedElementsObjectSet.TypeComparer();
        for (int index = 1; index < objArray.Length; ++index)
        {
          SceneNode sceneNode = (SceneNode) objArray[index];
          int num = this.selectedTypes.BinarySearch(sceneNode.TargetType, (IComparer<Type>) typeComparer);
          if (num < 0)
            this.selectedTypes.Insert(~num, sceneNode.TargetType);
        }
      }
      this.selectedTypesStale = false;
    }

    private class TypeComparer : System.Collections.Generic.Comparer<Type>
    {
      public override int Compare(Type x, Type y)
      {
        return x.FullName.CompareTo(y.FullName);
      }
    }
  }
}

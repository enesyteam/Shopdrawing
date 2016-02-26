// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.PropertyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal class PropertyManager : IPropertyManager, IDisposable
  {
    private DesignerContext designerContext;
    private SortedList<PropertyReference, List<PropertyManager.HandlerInfo>> propertyReferenceChangedHandlerList;
    private List<PropertyReference> changedPropertyReferences;
    private List<KeyValuePair<PropertyReference, PropertyManager.HandlerInfo>> deferredAddHandlerPairs;
    private List<KeyValuePair<PropertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler>> deferredRemoveHandlerPairs;
    private int deferUpdateHandlerList;

    public event EventHandler<MultiplePropertyReferencesChangedEventArgs> MultiplePropertyReferencesChanged;

    public PropertyManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.propertyReferenceChangedHandlerList = new SortedList<PropertyReference, List<PropertyManager.HandlerInfo>>();
    }

    public void RegisterPropertyReferenceChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler, bool includeSubpropertyChanges)
    {
      PropertyManager.HandlerInfo handlerInfo = new PropertyManager.HandlerInfo(handler, includeSubpropertyChanges);
      if (this.deferUpdateHandlerList > 0)
        this.deferredAddHandlerPairs.Add(new KeyValuePair<PropertyReference, PropertyManager.HandlerInfo>(propertyReference, handlerInfo));
      else
        this.DeferredRegisterPropertyReferenceChangedHandler(propertyReference, handlerInfo);
    }

    private void DeferredRegisterPropertyReferenceChangedHandler(PropertyReference propertyReference, PropertyManager.HandlerInfo handlerInfo)
    {
      if (this.propertyReferenceChangedHandlerList.ContainsKey(propertyReference))
        this.propertyReferenceChangedHandlerList[propertyReference].Add(handlerInfo);
      else
        this.propertyReferenceChangedHandlerList[propertyReference] = new List<PropertyManager.HandlerInfo>()
        {
          handlerInfo
        };
    }

    public void UnregisterPropertyReferenceChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.deferUpdateHandlerList > 0)
      {
        this.deferredRemoveHandlerPairs.Add(new KeyValuePair<PropertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler>(propertyReference, handler));
        List<PropertyManager.HandlerInfo> list;
        if (!this.propertyReferenceChangedHandlerList.TryGetValue(propertyReference, out list))
          return;
        foreach (PropertyManager.HandlerInfo handlerInfo in list)
        {
          if (handlerInfo.Handler == handler)
            handlerInfo.SetHasBeenUnregistered(true);
        }
      }
      else
        this.DeferredUnregisterPropertyReferenceChangedHandler(propertyReference, handler);
    }

    private void DeferredUnregisterPropertyReferenceChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      List<PropertyManager.HandlerInfo> list = this.propertyReferenceChangedHandlerList[propertyReference];
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index].Handler == handler)
        {
          list.RemoveAt(index);
          break;
        }
      }
      if (list.Count != 0)
        return;
      this.propertyReferenceChangedHandlerList.Remove(propertyReference);
    }

    private void DeferUpdateHandlerList(bool defer)
    {
      if (defer)
      {
        ++this.deferUpdateHandlerList;
        if (this.deferredAddHandlerPairs != null)
          return;
        this.deferredAddHandlerPairs = new List<KeyValuePair<PropertyReference, PropertyManager.HandlerInfo>>();
        this.deferredRemoveHandlerPairs = new List<KeyValuePair<PropertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler>>();
      }
      else
      {
        --this.deferUpdateHandlerList;
        if (this.deferUpdateHandlerList != 0)
          return;
        foreach (KeyValuePair<PropertyReference, PropertyManager.HandlerInfo> keyValuePair in this.deferredAddHandlerPairs)
          this.DeferredRegisterPropertyReferenceChangedHandler(keyValuePair.Key, keyValuePair.Value);
        foreach (KeyValuePair<PropertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler> keyValuePair in this.deferredRemoveHandlerPairs)
          this.DeferredUnregisterPropertyReferenceChangedHandler(keyValuePair.Key, keyValuePair.Value);
        this.deferredAddHandlerPairs.Clear();
        this.deferredRemoveHandlerPairs.Clear();
      }
    }

    public void Dispose()
    {
    }

    public PropertyState HasValue(ReferenceStep referenceStep)
    {
      PropertyReference propertyReference1 = new PropertyReference(referenceStep);
      object second = null;
      PropertyState propertyState1 = PropertyState.Unset;
      bool flag = false;
      SceneNode[] selectedNodes = this.designerContext.SelectionManager.SelectedNodes;
      if (selectedNodes != null && selectedNodes.Length > 0)
      {
        foreach (SceneNode node in selectedNodes)
        {
          PropertyReference propertyReference2 = this.FilterProperty(node, propertyReference1);
          if (propertyReference2 != null)
          {
            PropertyState propertyState2 = node.IsSet(propertyReference2);
            object computedValue = node.GetComputedValue(propertyReference2);
            if (!flag)
            {
              second = this.CopyIfFreezable(computedValue);
              propertyState1 = propertyState2;
              flag = true;
            }
            else
            {
              if (propertyState2 != propertyState1)
              {
                propertyState1 = PropertyState.Mixed;
                break;
              }
              if (!PropertyUtilities.Compare(computedValue, second, node.ViewModel.DefaultView))
              {
                object obj = MixedProperty.Mixed;
                propertyState1 = PropertyState.Mixed;
                break;
              }
            }
          }
        }
      }
      if (!flag)
        propertyState1 = this.designerContext.AmbientPropertyManager.GetAmbientValue(propertyReference1) == DependencyProperty.UnsetValue ? PropertyState.Unset : PropertyState.Set;
      return propertyState1;
    }

    public object GetValue(IPropertyId property)
    {
      return this.GetValue(new PropertyReference(this.designerContext.ActiveDocument.ProjectContext.ResolveProperty(property) as ReferenceStep), PropertyReference.GetValueFlags.Computed);
    }

    public object GetValue(PropertyReference propertyReference)
    {
      return this.GetValue(propertyReference, PropertyReference.GetValueFlags.Computed);
    }

    public object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags flags)
    {
      object second = null;
      bool flag = false;
      SceneNode[] selectedNodes = this.designerContext.SelectionManager.SelectedNodes;
      if (selectedNodes != null && selectedNodes.Length > 0)
      {
        foreach (SceneNode node in selectedNodes)
        {
          PropertyReference propertyReference1 = this.FilterProperty(node, propertyReference);
          if (propertyReference1 != null)
          {
            object obj = (flags & PropertyReference.GetValueFlags.Computed) == PropertyReference.GetValueFlags.Local ? node.GetLocalValue(propertyReference1) : node.GetComputedValue(propertyReference1);
            if (!flag)
            {
              second = this.CopyIfFreezable(obj);
              flag = true;
            }
            else if (!PropertyUtilities.Compare(obj, second, node.ViewModel.DefaultView))
            {
              second = MixedProperty.Mixed;
              break;
            }
          }
        }
      }
      if (!flag)
      {
        if (this.designerContext.AmbientPropertyManager.IsAmbientProperty(propertyReference))
        {
          second = this.designerContext.AmbientPropertyManager.GetAmbientValue(propertyReference);
        }
        else
        {
          ReferenceStep referenceStep = propertyReference[propertyReference.Count - 1];
          second = referenceStep.GetDefaultValue(referenceStep.TargetType);
        }
      }
      return second;
    }

    public SceneEditTransaction CreateEditTransaction(string description)
    {
      SceneEditTransaction sceneEditTransaction = (SceneEditTransaction) null;
      if (this.designerContext.ActiveDocument != null)
        sceneEditTransaction = this.designerContext.ActiveDocument.CreateEditTransaction(description);
      return sceneEditTransaction;
    }

    public void SetValue(ReferenceStep referenceStep, object value)
    {
      this.SetValue(new PropertyReference(referenceStep), value);
    }

    public void SetValue(PropertyReference propertyReference, object value)
    {
      this.ConstructiveModifyValue(propertyReference, value, PropertyManager.ConstructiveModification.SetValue);
    }

    private void ConstructiveModifyValue(PropertyReference propertyReference, object value, PropertyManager.ConstructiveModification modification)
    {
      SceneNode[] selectedNodes = this.designerContext.SelectionManager.SelectedNodes;
      if (selectedNodes == null || selectedNodes.Length <= 0)
        return;
      foreach (SceneNode node in selectedNodes)
      {
        PropertyReference propertyReference1 = this.FilterProperty(node, propertyReference);
        if (propertyReference1 != null)
        {
          if (modification == PropertyManager.ConstructiveModification.AddValue)
            node.InsertValue(propertyReference1, -1, value);
          else
            node.SetValue(propertyReference1, value);
        }
      }
    }

    public PropertyReference FilterProperty(SceneNode node, PropertyReference propertyReference)
    {
      return SceneNodeObjectSet.FilterProperty(node, propertyReference);
    }

    public ReferenceStep FilterProperty(SceneNode node, ReferenceStep referenceStep)
    {
      return SceneNodeObjectSet.FilterProperty(node, referenceStep);
    }

    public PropertyReference FilterProperty(ITypeResolver typeResolver, IType type, PropertyReference propertyReference)
    {
      return SceneNodeObjectSet.FilterProperty(typeResolver, type, propertyReference);
    }

    public ReferenceStep FilterProperty(ITypeResolver typeResolver, IType type, ReferenceStep referenceStep)
    {
      return SceneNodeObjectSet.FilterProperty(typeResolver, type, referenceStep);
    }

    private object CopyIfFreezable(object o)
    {
      Freezable freezable = o as Freezable;
      if (freezable == null)
        return o;
      try
      {
        return (object) freezable.CloneCurrentValue();
      }
      catch
      {
        return (object) (Freezable) null;
      }
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.designerContext.ResourceManager.OnSceneUpdate(args);
      TextSelectionSet textSelectionSet = this.designerContext.SelectionManager.TextSelectionSet;
      SceneElementSelectionSet elementSelectionSet = this.designerContext.SelectionManager.ElementSelectionSet;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable | SceneViewModel.ViewStateBits.ActiveTrigger | SceneViewModel.ViewStateBits.ActiveTimeline | SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.TextSelection | SceneViewModel.ViewStateBits.KeyFrameSelection | SceneViewModel.ViewStateBits.AnimationSelection | SceneViewModel.ViewStateBits.StoryboardSelection | SceneViewModel.ViewStateBits.CurrentValues | SceneViewModel.ViewStateBits.ChildPropertySelection | SceneViewModel.ViewStateBits.BehaviorSelection))
      {
        this.FireAllPropertyChangedEvents(args);
      }
      else
      {
        if (args.DocumentChanges.Count <= 0)
          return;
        foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
        {
          if (documentNodeChange.ParentNode != null && documentNodeChange.ParentNode.TargetType == typeof (DictionaryEntry))
          {
            this.FireAllPropertyChangedEvents(args);
            return;
          }
        }
        this.changedPropertyReferences = new List<PropertyReference>();
        foreach (SceneChange sceneChange in SceneChange.ChangesOfType<SceneChange>(args.DocumentChanges, args.ViewModel.RootNode))
        {
          if (sceneChange is StyleSceneChange)
          {
            this.FireAllPropertyChangedEvents(args);
            return;
          }
        }
        this.FireSelectivePropertyChangedEvents(args);
      }
    }

    private void FireAllPropertyChangedEvents(SceneUpdatePhaseEventArgs args)
    {
      using (args.ViewModel != null ? args.ViewModel.ScopeViewObjectCache() : (IDisposable) null)
      {
        if (this.MultiplePropertyReferencesChanged != null)
        {
          bool forceUpdate = args.ViewModel == null;
          this.MultiplePropertyReferencesChanged(this, new MultiplePropertyReferencesChangedEventArgs((IList<PropertyReference>) null, args.DirtyViewState, forceUpdate));
        }
        this.DeferUpdateHandlerList(true);
        PropertyReferenceChangedEventArgs e = new PropertyReferenceChangedEventArgs(args.DirtyViewState, (PropertyReference) null);
        for (int index = 0; index < this.propertyReferenceChangedHandlerList.Count; ++index)
        {
          PropertyReference propertyReference = this.propertyReferenceChangedHandlerList.Keys[index];
          foreach (PropertyManager.HandlerInfo handlerInfo in this.propertyReferenceChangedHandlerList.Values[index])
          {
            e.PropertyReference = propertyReference;
            handlerInfo.CallHandler(this, e);
          }
        }
        this.DeferUpdateHandlerList(false);
      }
    }

    private void InvalidatePropertyFromAnimationChange(SceneElement sceneElement, PropertyReference propertyReference)
    {
      if (this.designerContext.SelectionManager.ElementSelectionSet == null || !this.designerContext.SelectionManager.ElementSelectionSet.IsSelected(sceneElement))
        return;
      this.AddPropertyReferenceToSortedList(propertyReference, this.changedPropertyReferences);
    }

    private void FireSelectivePropertyChangedEvents(SceneUpdatePhaseEventArgs args)
    {
      SceneNode[] selectedNodes = this.designerContext.SelectionManager.SelectedNodes;
      DocumentNodeMarkerSortedList markerSortedList;
      if (selectedNodes == null)
      {
        markerSortedList = new DocumentNodeMarkerSortedList();
      }
      else
      {
        markerSortedList = new DocumentNodeMarkerSortedList(selectedNodes.Length);
        foreach (SceneNode sceneNode in selectedNodes)
        {
          TextRangeElement textRangeElement = sceneNode as TextRangeElement;
          BehaviorTriggerActionNode triggerActionNode = sceneNode as BehaviorTriggerActionNode;
          if (textRangeElement != null)
          {
            this.FireAllPropertyChangedEvents(args);
            return;
          }
          if (triggerActionNode != null && triggerActionNode.Parent != null)
            markerSortedList.Add(triggerActionNode.Parent.DocumentNode.Marker);
          markerSortedList.Add(sceneNode.DocumentNode.Marker);
        }
      }
      foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
      {
        if (SceneView.HandleAnimationChanges(args.ViewModel, documentNodeChange, new SceneView.HandleAnimationChange(this.InvalidatePropertyFromAnimationChange)) == SceneView.AnimationChangeResult.InvalidateAll)
        {
          this.FireAllPropertyChangedEvents(args);
          return;
        }
      }
      if (this.designerContext.SelectionManager.SecondarySelectedNodes != null)
      {
        foreach (SceneNode sceneNode in this.designerContext.SelectionManager.SecondarySelectedNodes)
        {
          if (sceneNode.DocumentNode.Marker != null)
            markerSortedList.Add(sceneNode.DocumentNode.Marker);
        }
      }
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in args.DocumentChanges.Intersect((DocumentNodeMarkerSortedListBase) markerSortedList, DocumentNodeMarkerSortedListBase.Flags.ContainedBy))
      {
        Stack<ReferenceStep> input = DocumentNodeMarkerUtilities.PropertyReferencePath(args.DocumentChanges.MarkerAt(intersectionResult.LeftHandSideIndex), markerSortedList.MarkerAt(intersectionResult.RightHandSideIndex));
        if (input.Count > 0)
        {
          PropertyReference propertyReference1 = new PropertyReference(input);
          this.AddPropertyReferenceToSortedList(propertyReference1, this.changedPropertyReferences);
          if (PlatformTypes.Style.IsAssignableFrom(propertyReference1.FirstStep.DeclaringTypeId))
          {
            int startIndex;
            for (startIndex = 0; startIndex < propertyReference1.Count; ++startIndex)
            {
              if (PlatformTypes.Setter.IsAssignableFrom(propertyReference1.ReferenceSteps[startIndex].DeclaringTypeId))
              {
                ++startIndex;
                break;
              }
            }
            foreach (SceneChange sceneChange in SceneChange.ChangesOfType<SceneChange>(args.DocumentChanges, args.ViewModel.RootNode))
            {
              PropertyReferenceSceneChange referenceSceneChange = sceneChange as PropertyReferenceSceneChange;
              if (referenceSceneChange != null && referenceSceneChange.PropertyChanged != null)
              {
                PropertyReference propertyReference2 = referenceSceneChange.PropertyChanged.Subreference(0);
                if (startIndex < propertyReference1.Count)
                  propertyReference2 = propertyReference2.Append(propertyReference1.Subreference(startIndex));
                this.AddPropertyReferenceToSortedList(propertyReference2, this.changedPropertyReferences);
              }
            }
          }
        }
      }
      this.FireSelectivePropertyChangedEventsWorker(args.DirtyViewState);
    }

    private void FireSelectivePropertyChangedEventsWorker(SceneViewModel.ViewStateBits dirtyViewState)
    {
      this.DeferUpdateHandlerList(true);
      int index1 = 0;
      int index2 = 0;
      PropertyReferenceChangedEventArgs e = new PropertyReferenceChangedEventArgs(dirtyViewState, (PropertyReference) null);
      for (; index1 < this.propertyReferenceChangedHandlerList.Count; ++index1)
      {
        while (index2 < this.changedPropertyReferences.Count && index1 < this.propertyReferenceChangedHandlerList.Count && this.changedPropertyReferences[index2].CompareTo(this.propertyReferenceChangedHandlerList.Keys[index1]) < 0)
        {
          if (this.changedPropertyReferences[index2].IsPrefixOf(this.propertyReferenceChangedHandlerList.Keys[index1]))
          {
            foreach (PropertyManager.HandlerInfo handlerInfo in this.propertyReferenceChangedHandlerList.Values[index1])
            {
              e.PropertyReference = this.propertyReferenceChangedHandlerList.Keys[index1];
              handlerInfo.CallHandler(this, e);
            }
            ++index1;
          }
          else
            ++index2;
        }
        if (index2 < this.changedPropertyReferences.Count && index1 < this.propertyReferenceChangedHandlerList.Count)
        {
          if (this.changedPropertyReferences[index2].CompareTo(this.propertyReferenceChangedHandlerList.Keys[index1]) == 0)
          {
            foreach (PropertyManager.HandlerInfo handlerInfo in this.propertyReferenceChangedHandlerList.Values[index1])
            {
              e.PropertyReference = this.propertyReferenceChangedHandlerList.Keys[index1];
              handlerInfo.CallHandler(this, e);
            }
          }
          else if (this.propertyReferenceChangedHandlerList.Keys[index1].IsPrefixOf(this.changedPropertyReferences[index2]))
          {
            int index3 = index2;
            do
            {
              foreach (PropertyManager.HandlerInfo handlerInfo in this.propertyReferenceChangedHandlerList.Values[index1])
              {
                if (handlerInfo.IncludeSubpropertyChanges)
                {
                  e.PropertyReference = this.changedPropertyReferences[index3];
                  handlerInfo.CallHandler(this, e);
                }
              }
            }
            while (++index3 < this.changedPropertyReferences.Count && this.propertyReferenceChangedHandlerList.Keys[index1].IsPrefixOf(this.changedPropertyReferences[index3]));
          }
        }
        else
          break;
      }
      this.DeferUpdateHandlerList(false);
      if (this.MultiplePropertyReferencesChanged == null)
        return;
      this.MultiplePropertyReferencesChanged(this, new MultiplePropertyReferencesChangedEventArgs((IList<PropertyReference>) this.changedPropertyReferences, dirtyViewState, false));
    }

    private void AddPropertyReferenceToSortedList(PropertyReference propertyReference, List<PropertyReference> propertyReferenceList)
    {
      int num = propertyReferenceList.BinarySearch(propertyReference);
      if (num >= 0)
        return;
      propertyReferenceList.Insert(~num, propertyReference);
    }

    private enum ConstructiveModification
    {
      SetValue,
      AddValue,
    }

    private class HandlerInfo
    {
      private Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler;
      private bool includeSubpropertyChanges;
      private bool hasBeenUnregistered;

      public Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler Handler
      {
        get
        {
          return this.handler;
        }
      }

      public bool IncludeSubpropertyChanges
      {
        get
        {
          return this.includeSubpropertyChanges;
        }
      }

      public HandlerInfo(Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler, bool includeSubpropertyChanges)
      {
        this.handler = handler;
        this.includeSubpropertyChanges = includeSubpropertyChanges;
      }

      public void SetHasBeenUnregistered(bool value)
      {
        this.hasBeenUnregistered = value;
      }

      public void CallHandler(object sender, PropertyReferenceChangedEventArgs e)
      {
        if (this.hasBeenUnregistered)
          return;
        this.handler(sender, e);
      }
    }
  }
}

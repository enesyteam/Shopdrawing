// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.EditContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class EditContext
  {
    private NodeViewPair parentElement;
    private SceneViewModel sceneViewModel;
    private DocumentNodePath editingContainerPath;
    private SceneNode viewScope;
    private StoryboardTimelineSceneNode timeline;
    private TriggerBaseNode trigger;
    private VisualStateSceneNode stateTarget;
    private VisualStateTransitionSceneNode transitionTarget;
    private StoryboardTimelineSceneNode stateStoryboardTarget;
    private StoryboardTimelineSceneNode transitionStoryboardTarget;
    private VisualStateSceneNode[] pinnedStates;
    private ICollection<IProperty> outOfPlaceOverriddenProperties;
    private bool isHidden;
    private bool isFrozen;
    private ISceneInsertionPoint lockedInsertionPoint;
    private DocumentNodePath lastPrimarySelectedPath;

    public bool IsFrozen
    {
      get
      {
        return this.isFrozen;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.sceneViewModel;
      }
      set
      {
        this.ThrowIfFrozen();
        this.sceneViewModel = value;
      }
    }

    public ICollection<IProperty> OutOfPlaceOverriddenProperties
    {
      get
      {
        return this.outOfPlaceOverriddenProperties;
      }
      set
      {
        this.outOfPlaceOverriddenProperties = value;
      }
    }

    public NodeViewPair ParentElement
    {
      get
      {
        if (this.parentElement == null)
          return (NodeViewPair) null;
        DocumentNodePath nodePath = this.parentElement.NodePath;
        if (nodePath == null)
          return (NodeViewPair) null;
        SceneViewModel viewModel = this.parentElement.GetViewModel(this.ViewModel.DesignerContext);
        if (viewModel == null)
          return (NodeViewPair) null;
        if (!viewModel.EditContextManager.ContextExistsForEditingContainer(nodePath.GetContainerNodePath()))
          return (NodeViewPair) null;
        if (!NodeViewPair.EvaluateExpression(viewModel, nodePath, this.parentElement.PropertyKey, this.EditingContainerPath))
          return (NodeViewPair) null;
        return this.parentElement;
      }
      set
      {
        this.ThrowIfFrozen();
        this.parentElement = value;
      }
    }

    public DocumentNodePath EditingContainerPath
    {
      get
      {
        return this.editingContainerPath;
      }
      set
      {
        this.ThrowIfFrozen();
        this.editingContainerPath = value;
      }
    }

    public SceneNode EditingContainer
    {
      get
      {
        if (this.EditingContainerPath != null && !this.sceneViewModel.IsExternal(this.EditingContainerPath.Node))
          return this.sceneViewModel.GetSceneNode(this.EditingContainerPath.Node);
        return (SceneNode) null;
      }
    }

    public IStoryboardContainer StoryboardContainer
    {
      get
      {
        return this.EditingContainer as IStoryboardContainer;
      }
    }

    public StoryboardTimelineSceneNode Timeline
    {
      get
      {
        return this.timeline;
      }
      set
      {
        this.ThrowIfFrozen();
        this.timeline = value;
      }
    }

    public TriggerBaseNode Trigger
    {
      get
      {
        return this.trigger;
      }
      set
      {
        this.ThrowIfFrozen();
        this.trigger = value;
      }
    }

    public VisualStateSceneNode StateEditTarget
    {
      get
      {
        return this.stateTarget;
      }
      set
      {
        this.ThrowIfFrozen();
        this.stateTarget = value;
      }
    }

    public VisualStateTransitionSceneNode TransitionEditTarget
    {
      get
      {
        return this.transitionTarget;
      }
      set
      {
        this.ThrowIfFrozen();
        this.transitionTarget = value;
      }
    }

    public IList<VisualStateSceneNode> PinnedStates
    {
      get
      {
        if (this.pinnedStates == null)
          return (IList<VisualStateSceneNode>) null;
        return (IList<VisualStateSceneNode>) new ReadOnlyCollection<VisualStateSceneNode>((IList<VisualStateSceneNode>) this.pinnedStates);
      }
      internal set
      {
        this.ThrowIfFrozen();
        if (value == null)
        {
          this.pinnedStates = (VisualStateSceneNode[]) null;
        }
        else
        {
          VisualStateSceneNode[] array = new VisualStateSceneNode[value.Count];
          value.CopyTo(array, 0);
          this.pinnedStates = array;
        }
      }
    }

    public StoryboardTimelineSceneNode StateStoryboardEditTarget
    {
      get
      {
        return this.stateStoryboardTarget;
      }
      set
      {
        this.ThrowIfFrozen();
        this.stateStoryboardTarget = value;
      }
    }

    public StoryboardTimelineSceneNode TransitionStoryboardEditTarget
    {
      get
      {
        return this.transitionStoryboardTarget;
      }
      set
      {
        this.ThrowIfFrozen();
        this.transitionStoryboardTarget = value;
      }
    }

    public SceneNode ViewScope
    {
      get
      {
        return this.viewScope;
      }
      set
      {
        this.ThrowIfFrozen();
        this.viewScope = value;
      }
    }

    public ISceneInsertionPoint LockedInsertionPoint
    {
      get
      {
        return this.lockedInsertionPoint;
      }
      set
      {
        this.ThrowIfFrozen();
        this.lockedInsertionPoint = value;
      }
    }

    public bool IsHidden
    {
      get
      {
        return this.isHidden;
      }
      set
      {
        this.ThrowIfFrozen();
        this.isHidden = value;
      }
    }

    public DocumentNodePath LastPrimarySelectedPath
    {
      get
      {
        return this.lastPrimarySelectedPath;
      }
      set
      {
        this.ThrowIfFrozen();
        this.lastPrimarySelectedPath = value;
      }
    }

    public EditContext()
    {
    }

    public EditContext(EditContext oldContext)
    {
      this.parentElement = oldContext.parentElement;
      this.sceneViewModel = oldContext.sceneViewModel;
      this.editingContainerPath = oldContext.editingContainerPath;
      this.viewScope = oldContext.viewScope;
      this.timeline = oldContext.timeline;
      this.trigger = oldContext.trigger;
      this.stateTarget = oldContext.stateTarget;
      this.transitionTarget = oldContext.transitionTarget;
      this.stateStoryboardTarget = oldContext.stateStoryboardTarget;
      this.transitionStoryboardTarget = oldContext.transitionStoryboardTarget;
      if (oldContext.pinnedStates != null)
      {
        VisualStateSceneNode[] newArray = new VisualStateSceneNode[oldContext.pinnedStates.Length];
        oldContext.CopyPinnedStatesTo(newArray, (VisualStateSceneNode) null);
        this.pinnedStates = newArray;
      }
      this.isHidden = oldContext.isHidden;
      this.lockedInsertionPoint = oldContext.lockedInsertionPoint;
      this.lastPrimarySelectedPath = oldContext.lastPrimarySelectedPath;
    }

    public void Freeze()
    {
      this.isFrozen = true;
    }

    public EditContext CreateContextLinkedToParent(NodeViewPair parentElement)
    {
      EditContext editContext = new EditContext(this)
      {
        ParentElement = parentElement
      };
      editContext.Freeze();
      return editContext;
    }

    public void PinState(VisualStateSceneNode state)
    {
      this.ThrowIfFrozen();
      if (state == null)
        throw new ArgumentException("State must not be null");
      if (this.pinnedStates != null)
      {
        VisualStateSceneNode[] newArray = new VisualStateSceneNode[this.pinnedStates.Length + 1];
        this.CopyPinnedStatesTo(newArray, (VisualStateSceneNode) null);
        newArray[this.pinnedStates.Length] = state;
        this.pinnedStates = newArray;
      }
      else
      {
        this.pinnedStates = new VisualStateSceneNode[1];
        this.pinnedStates[0] = state;
      }
    }

    public void UnpinState(VisualStateSceneNode state)
    {
      this.ThrowIfFrozen();
      if (state == null)
        throw new ArgumentException("State must not be null");
      VisualStateSceneNode[] newArray = new VisualStateSceneNode[this.pinnedStates.Length - 1];
      this.CopyPinnedStatesTo(newArray, state);
      if (newArray.Length > 1 || newArray.Length == 1 && newArray[0] != null)
        this.pinnedStates = newArray;
      else
        this.pinnedStates = (VisualStateSceneNode[]) null;
    }

    public void ClearPinnedStates()
    {
      this.ThrowIfFrozen();
      this.pinnedStates = (VisualStateSceneNode[]) null;
    }

    public bool PinnedStatesEquals(EditContext rhs)
    {
      if (this.pinnedStates == null && rhs.pinnedStates == null)
        return true;
      if (this.pinnedStates == null && rhs.pinnedStates != null || this.pinnedStates != null && rhs.pinnedStates == null || this.pinnedStates.Length != rhs.pinnedStates.Length)
        return false;
      for (int index = 0; index < this.pinnedStates.Length; ++index)
      {
        if (this.pinnedStates[index] != rhs.pinnedStates[index])
          return false;
      }
      return true;
    }

    private void CopyPinnedStatesTo(VisualStateSceneNode[] newArray, VisualStateSceneNode skip)
    {
      if (this.pinnedStates == null)
        return;
      if (skip == null)
      {
        this.pinnedStates.CopyTo((Array) newArray, 0);
      }
      else
      {
        int index1 = 0;
        for (int index2 = 0; index2 < this.pinnedStates.Length; ++index2)
        {
          if (this.pinnedStates[index2] != skip)
          {
            newArray[index1] = this.pinnedStates[index2];
            ++index1;
          }
        }
      }
    }

    internal void EnsureHidden()
    {
      this.isHidden = true;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      EditContext rhs = obj as EditContext;
      if (rhs != null && this.parentElement == rhs.parentElement && (this.sceneViewModel == rhs.sceneViewModel && object.Equals((object) this.editingContainerPath, (object) rhs.editingContainerPath)) && (this.viewScope == rhs.viewScope && this.timeline == rhs.timeline && (this.trigger == rhs.trigger && this.stateTarget == rhs.stateTarget)) && (this.transitionTarget == rhs.transitionTarget && this.stateStoryboardTarget == rhs.stateStoryboardTarget && (this.transitionStoryboardTarget == rhs.transitionStoryboardTarget && this.PinnedStatesEquals(rhs)) && object.Equals((object) this.lockedInsertionPoint, (object) rhs.lockedInsertionPoint)))
        return object.Equals((object) this.lastPrimarySelectedPath, (object) rhs.lastPrimarySelectedPath);
      return false;
    }

    public override int GetHashCode()
    {
      if (this.editingContainerPath == null)
        return 0;
      return this.editingContainerPath.GetHashCode();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      EditContext.AppendNameValuePair(stringBuilder, "EditingContainerPath", (object) this.editingContainerPath);
      EditContext.AppendNameValuePair(stringBuilder, "Trigger", (object) this.trigger);
      EditContext.AppendNameValuePair(stringBuilder, "Storyboard", (object) this.timeline);
      EditContext.AppendNameValuePair(stringBuilder, "StateEditTarget", (object) this.stateTarget);
      EditContext.AppendNameValuePair(stringBuilder, "TransitionEditTarget", (object) this.transitionTarget);
      EditContext.AppendNameValuePair(stringBuilder, "StateStoryboardEditTarget", (object) this.stateStoryboardTarget);
      EditContext.AppendNameValuePair(stringBuilder, "TransitionStoryboardEditTarget", (object) this.transitionStoryboardTarget);
      EditContext.AppendNameValuePair(stringBuilder, "ViewScope", (object) this.viewScope);
      EditContext.AppendNameValuePair(stringBuilder, "LockedInsertionPoint", (object) this.lockedInsertionPoint);
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    private static void AppendNameValuePair(StringBuilder stringBuilder, string name, object value)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.Append("; ");
      stringBuilder.Append(name);
      stringBuilder.Append(": ");
      if (value != null)
      {
        stringBuilder.Append("{");
        stringBuilder.Append(value.ToString());
        stringBuilder.Append("}");
      }
      else
        stringBuilder.Append("null");
    }

    private void ThrowIfFrozen()
    {
      if (this.isFrozen)
        throw new InvalidOperationException("Cannot set property on frozen EditContext");
    }
  }
}

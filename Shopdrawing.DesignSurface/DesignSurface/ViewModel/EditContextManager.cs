// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.EditContextManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  [DebuggerDisplay("{System.IO.Path.GetFileName(this.viewModel.DefaultView.InstanceBuilderContext.DocumentContext.DocumentUrl)}")]
  internal sealed class EditContextManager
  {
    private Size preferredSize = Size.Empty;
    private SceneViewModel viewModel;
    private IEditContextHistory editContextHistory;

    public Size PreferredSize
    {
      get
      {
        return this.preferredSize;
      }
    }

    public EditContextManager.SingleHistoryWalker SingleViewModelEditContextWalker
    {
      get
      {
        return new EditContextManager.SingleHistoryWalker(this.editContextHistory);
      }
    }

    public EditContextManager.MultiHistoryWalker MultiViewModelEditContextWalker
    {
      get
      {
        return new EditContextManager.MultiHistoryWalker(this.ViewModel);
      }
    }

    public EditContext NextActiveNonHiddenParentContext
    {
      get
      {
        return this.GetNextActiveNonHiddenParentContext(this.ActiveEditContext, true);
      }
    }

    public bool CanPopActiveEditingContainer
    {
      get
      {
        return this.NextActiveNonHiddenParentContext != null;
      }
    }

    public EditContext ActiveEditContext
    {
      get
      {
        return this.editContextHistory.ActiveEditContext;
      }
    }

    public DocumentNodePath ActiveEditingContainerPath
    {
      get
      {
        return this.ActiveEditContext.EditingContainerPath;
      }
      set
      {
        this.ModifyEditingContainer(value, (SceneView) null, (SceneElement) null, (IPropertyId) null, false);
      }
    }

    public SceneNode ActiveEditingContainer
    {
      get
      {
        return this.ActiveEditContext.EditingContainer;
      }
    }

    public IStoryboardContainer ActiveStoryboardContainer
    {
      get
      {
        return this.ActiveEditContext.StoryboardContainer;
      }
    }

    public TriggerBaseNode ActiveVisualTrigger
    {
      get
      {
        return this.ActiveEditContext.Trigger;
      }
    }

    public StoryboardTimelineSceneNode ActiveStoryboardTimeline
    {
      get
      {
        return this.ActiveEditContext.Timeline;
      }
    }

    public VisualStateSceneNode StateEditTarget
    {
      get
      {
        return this.ActiveEditContext.StateEditTarget;
      }
    }

    public VisualStateTransitionSceneNode TransitionEditTarget
    {
      get
      {
        return this.ActiveEditContext.TransitionEditTarget;
      }
    }

    public StoryboardTimelineSceneNode StateStoryboardEditTarget
    {
      get
      {
        return this.ActiveEditContext.StateStoryboardEditTarget;
      }
    }

    public StoryboardTimelineSceneNode TransitionStoryboardEditTarget
    {
      get
      {
        return this.ActiveEditContext.TransitionStoryboardEditTarget;
      }
    }

    public IList<VisualStateSceneNode> PinnedStates
    {
      get
      {
        return this.ActiveEditContext.PinnedStates;
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    private bool HasNextActiveParentContext
    {
      get
      {
        return this.editContextHistory.NextActiveParentContext != null;
      }
    }

    private DocumentNodePath RootNodePath
    {
      get
      {
        DocumentNode rootNode = this.ViewModel.DocumentRoot.RootNode;
        if (rootNode == null)
          return (DocumentNodePath) null;
        return new DocumentNodePath(rootNode, rootNode);
      }
    }

    public EditContextManager(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.editContextHistory = (IEditContextHistory) new EditContextManager.EditContextHistory(viewModel, this.CreateDefaultEditContext());
    }

    public EditContext GetNextActiveNonHiddenParentContext(EditContext referenceEditContext, bool crossView)
    {
      EditContext editContext = referenceEditContext;
      do
      {
        editContext = this.editContextHistory.GetNextActiveParentContext(editContext, crossView);
      }
      while (editContext != null && editContext.IsHidden);
      return editContext;
    }

    public void SetLockedInsertionPoint(ISceneInsertionPoint lockedInsertionPoint)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        ViewModel = this.ViewModel,
        LockedInsertionPoint = lockedInsertionPoint,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void SetStoryboardTimelineTrigger(DocumentNodePath storyboardPath, StoryboardTimelineSceneNode timeline, TriggerBaseNode visualTrigger)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        ViewModel = this.ViewModel,
        EditingContainerPath = storyboardPath,
        Timeline = timeline,
        Trigger = visualTrigger,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void SetActiveVisualStateContext(VisualStateSceneNode stateTarget, StoryboardTimelineSceneNode storyboardTarget)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        StateEditTarget = stateTarget,
        StateStoryboardEditTarget = storyboardTarget,
        TransitionEditTarget = (VisualStateTransitionSceneNode) null,
        TransitionStoryboardEditTarget = (StoryboardTimelineSceneNode) null,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void SetActiveVisualStateTransitionContext(VisualStateTransitionSceneNode transition, StoryboardTimelineSceneNode storyboardTarget)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        StateEditTarget = (VisualStateSceneNode) null,
        StateStoryboardEditTarget = (StoryboardTimelineSceneNode) null,
        TransitionEditTarget = transition,
        TransitionStoryboardEditTarget = storyboardTarget,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void PinStateInContext(VisualStateSceneNode stateToPin)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.PinState(stateToPin);
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void UnpinStateInContext(VisualStateSceneNode stateToUnpin)
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.UnpinState(stateToUnpin);
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void UnpinAllStatesInContext()
    {
      EditContext editContext = new EditContext(this.ActiveEditContext)
      {
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.ClearPinnedStates();
      editContext.Freeze();
      this.editContextHistory.ReplaceActive(editContext);
    }

    public void ClearEditContextList()
    {
      EditContext editContext = new EditContext()
      {
        ViewModel = this.ViewModel
      };
      editContext.Freeze();
      this.SetEditContext(editContext);
    }

    public void ResetEditContextListToDefault()
    {
      this.SetEditContext(this.CreateDefaultEditContext());
    }

    public void SetViewRoot(SceneView callingView, SceneElement ancestorElement, IPropertyId ancestorPropertyKey, DocumentNode node, Size preferredSize)
    {
      this.preferredSize = preferredSize;
      this.VerifyViewRootIsValid(node);
      this.ModifyEditingContainer(this.CreateSimplestPath(node), callingView, ancestorElement, ancestorPropertyKey, true);
    }

    public bool ContextExistsForEditingContainer(DocumentNodePath editingContainerPath)
    {
      return this.editContextHistory.GetContext(editingContainerPath) != null;
    }

    public void Canonicalize(SceneUpdateTypeFlags flags, DocumentNodeChangeList damage)
    {
      this.editContextHistory.Canonicalize(flags, damage);
    }

    public void PopActiveEditingContainer()
    {
      if (!this.CanPopActiveEditingContainer)
        return;
      SceneView sceneView = (SceneView) null;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
      {
        DocumentNodePath editingContainerPath = this.ActiveEditingContainerPath;
        do
        {
          NodeViewPair parentElement = this.ActiveEditContext.ParentElement;
          sceneView = parentElement == null || parentElement.View == this.ViewModel.DefaultView ? (SceneView) null : parentElement.View;
          this.MoveToParent();
        }
        while (this.HasNextActiveParentContext && this.ActiveEditContext.IsHidden);
        this.SelectNodeInActiveEditingContainer(editingContainerPath);
        editTransaction.Commit();
      }
      if (sceneView == null || !this.ViewModel.DesignerContext.ViewService.Views.Contains((IView) sceneView))
        return;
      this.ViewModel.DesignerContext.ViewService.ActiveView = (IView) sceneView;
    }

    public void MoveToEditContext(EditContext editContext)
    {
      if (editContext.ViewModel != this.ViewModel)
        this.ViewModel.GetViewModel(editContext.EditingContainer.DocumentNode.DocumentRoot, true);
      this.ReassignHistoryLinks(editContext);
      editContext.ViewModel.EditContextManager.MoveToEditContextInternal(editContext);
    }

    private void MoveToEditContextInternal(EditContext editContext)
    {
      if (editContext.Equals((object) this.ActiveEditContext))
        return;
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
      {
        DocumentNodePath documentNodePath = EditContextHelper.SelectedElementPathInEditContext(this.ViewModel, editContext, true);
        while (this.HasNextActiveParentContext && !this.ActiveEditingContainerPath.IsAncestorOf(editContext.EditingContainerPath) && !editContext.EditingContainerPath.IsAncestorOf(this.ActiveEditingContainerPath))
          this.MoveToParent();
        if (!this.editContextHistory.MoveToEditContext(editContext.EditingContainerPath))
          throw new InvalidOperationException();
        this.ViewModel.ElementSelectionSet.Clear();
        if (documentNodePath != null)
        {
          SceneElement selectionToSet = this.ViewModel.GetSceneNode(documentNodePath.Node) as SceneElement;
          if (selectionToSet != null)
            this.ViewModel.ElementSelectionSet.SetSelection(selectionToSet);
        }
        editTransaction.Commit();
      }
    }

    private void ReassignHistoryLinks(EditContext editContext)
    {
      List<NodeViewPair> parents = new List<NodeViewPair>();
      List<EditContext> children = new List<EditContext>();
      bool foundContext = false;
      SceneViewModel previousViewModel = (SceneViewModel) null;
      DocumentNodePath previousSelectedPath = (DocumentNodePath) null;
      this.ViewModel.EditContextManager.MultiViewModelEditContextWalker.Walk(false, (MultiHistoryCallback) ((context, selectedElementPath, ownerPropertyKey, isGhosted) =>
      {
        if (previousViewModel != null && foundContext)
        {
          parents.Add(new NodeViewPair(previousViewModel.DefaultView, previousSelectedPath, ownerPropertyKey));
          children.Add(context);
        }
        if (context.Equals((object) editContext) || context.Equals((object) this.ActiveEditContext))
        {
          if (foundContext)
            return true;
          foundContext = true;
        }
        previousViewModel = context.ViewModel;
        previousSelectedPath = selectedElementPath ?? EditContextHelper.SelectedElementPathInEditContext(context.ViewModel, context, false);
        return false;
      }));
      for (int index = 0; index < parents.Count; ++index)
        this.LinkContainerDrillInHistory(parents[index], children[index]);
    }

    private void SelectNodeInActiveEditingContainer(DocumentNodePath pathToSelection)
    {
      DocumentNodePath documentNodePath = this.ActiveEditContext.EditingContainer.DocumentNodePath;
      if (!documentNodePath.IsAncestorOf(pathToSelection))
        return;
      DocumentNodePath editingContainer = this.ViewModel.GetAncestorInEditingContainer(pathToSelection, documentNodePath, (DocumentNodePath) null);
      if (editingContainer == null)
        return;
      SceneElement selectionToSet = this.ViewModel.GetSceneNode(editingContainer.Node) as SceneElement;
      if (selectionToSet == null)
        return;
      this.ViewModel.ElementSelectionSet.SetSelection(selectionToSet);
    }

    private DocumentNodePath CreateSimplestPath(DocumentNode node)
    {
      List<DocumentNode> list = new List<DocumentNode>();
      DocumentNode documentNode;
      for (documentNode = node; documentNode.Parent != null; documentNode = (DocumentNode) documentNode.Parent)
      {
        if (documentNode.IsProperty && PlatformTypes.Style.IsAssignableFrom((ITypeId) documentNode.Type) || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) documentNode.Type))
          list.Add(documentNode);
      }
      DocumentNodePath documentNodePath = new DocumentNodePath(documentNode, documentNode);
      for (int index = list.Count - 1; index >= 0; --index)
        documentNodePath = documentNodePath.GetPathInContainer((DocumentNode) list[index].Parent).GetPathInSubContainer(list[index].SitePropertyKey, list[index]);
      return documentNodePath.GetPathInContainer(node);
    }

    private void VerifyViewRootIsValid(DocumentNode viewRootNode)
    {
      if (viewRootNode == null || viewRootNode == this.ViewModel.DocumentRoot.RootNode)
        return;
      ITypeId type = (ITypeId) viewRootNode.Type;
      if (!PlatformTypes.Style.IsAssignableFrom(type) && !PlatformTypes.FrameworkTemplate.IsAssignableFrom(type))
        throw new InvalidOperationException();
    }

    private void MoveToParent()
    {
      NodeViewPair parentElement = this.ActiveEditContext.ParentElement;
      DocumentNodePath documentNodePath = (DocumentNodePath) null;
      if (parentElement != null && parentElement.NodePath != null && parentElement.GetViewModel(this.ViewModel.DesignerContext) == this.ViewModel)
      {
        EditContext activeParentContext = this.editContextHistory.NextActiveParentContext;
        DocumentNodePath editingContainer = activeParentContext != null ? activeParentContext.EditingContainerPath : (DocumentNodePath) null;
        documentNodePath = this.ViewModel.GetAncestorInEditingContainer(parentElement.NodePath, editingContainer, (DocumentNodePath) null);
      }
      SceneElement selectionToSet = (documentNodePath != null ? this.ViewModel.GetSceneNode(documentNodePath.Node) : (SceneNode) null) as SceneElement;
      this.editContextHistory.MoveToParent();
      if (selectionToSet != null)
        this.ViewModel.ElementSelectionSet.SetSelection(selectionToSet);
      else
        this.ViewModel.ElementSelectionSet.Clear();
    }

    private void ModifyEditingContainer(DocumentNodePath targetPath, SceneView callingView, SceneElement ancestorElement, IPropertyId ancestorPropertyKey, bool viewScoped)
    {
      NodeViewPair parentElement1 = (NodeViewPair) null;
      if (callingView != null)
      {
        DocumentNodePath nodePath = ancestorElement != null ? ancestorElement.DocumentNodePath : (DocumentNodePath) null;
        if (nodePath != null)
          parentElement1 = new NodeViewPair(callingView, nodePath, ancestorPropertyKey);
      }
      SceneNode editingContainer1 = targetPath != null ? this.ViewModel.GetSceneNode(targetPath.Node) : (SceneNode) null;
      SceneNode sceneNode1 = (SceneNode) null;
      if (viewScoped)
        sceneNode1 = editingContainer1;
      else if (this.ActiveEditContext.ViewScope != null)
        sceneNode1 = this.ActiveEditContext.ViewScope;
      EditContext editContext1 = new EditContext()
      {
        ViewModel = this.ViewModel,
        EditingContainerPath = targetPath,
        ViewScope = sceneNode1,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext1.Freeze();
      if (targetPath == null || this.ActiveEditingContainerPath == null)
      {
        this.SetEditContext(editContext1);
      }
      else
      {
        if (this.ActiveEditingContainerPath.Equals((object) targetPath) && parentElement1 == null && !viewScoped)
          return;
        using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
        {
          using (new EditContextManager.SelectionSetterForContainer(this.ViewModel, editingContainer1))
          {
            editTransaction.Update();
            bool flag1 = false;
            while (this.HasNextActiveParentContext && !this.ActiveEditingContainerPath.IsAncestorOf(targetPath))
            {
              flag1 = true;
              this.MoveToParent();
            }
            List<DocumentNodePath> list = new List<DocumentNodePath>();
            int index1 = 0;
            for (DocumentNodePath other = targetPath; other != null; other = other.ContainerOwner != null ? other.GetContainerOwnerPath() : (DocumentNodePath) null)
            {
              if (this.ActiveEditingContainerPath.IsAncestorOf(other))
                ++index1;
              list.Add(other);
            }
            DocumentNodePath editingContainerPath = this.ActiveEditingContainerPath;
            EditContext activeEditContext = this.ActiveEditContext;
            if (!this.ActiveEditingContainerPath.Equals((object) targetPath))
            {
              NodeViewPair parentElement2 = (NodeViewPair) null;
              bool flag2 = index1 == list.Count;
              if (!flag2)
              {
                if (viewScoped)
                {
                  parentElement2 = parentElement1;
                  if (parentElement1 != null && parentElement1.NodePath.Equals((object) this.ActiveEditingContainerPath))
                    parentElement2 = this.ActiveEditContext.ParentElement;
                }
                else
                {
                  DocumentNodePath editingContainer2 = this.ViewModel.GetAncestorInEditingContainer(list[index1 - 1], list[index1].GetContainerNodePath(), (DocumentNodePath) null);
                  if (editingContainer2 != null)
                    parentElement2 = new NodeViewPair(this.ViewModel.DefaultView, editingContainer2);
                  else if (this.ActiveEditContext.ViewScope != null && index1 == list.Count - 1)
                    parentElement2 = this.ActiveEditContext.ParentElement;
                }
              }
              bool flag3 = true;
              bool flag4 = activeEditContext != null && activeEditContext.IsHidden;
              for (int index2 = index1 - 1; index2 > 0; --index2)
              {
                DocumentNodePath containerNodePath = list[index2].GetContainerNodePath();
                SceneNode sceneNode2 = this.ViewModel.GetSceneNode(containerNodePath.Node);
                SceneNode sceneNode3 = viewScoped ? sceneNode2 : (SceneNode) null;
                if (editingContainerPath.Node.DocumentRoot == targetPath.Node.DocumentRoot && !viewScoped)
                  sceneNode3 = activeEditContext.ViewScope;
                EditContext context = this.editContextHistory.GetContext(containerNodePath);
                EditContext editContext2;
                if (context != null)
                  editContext2 = new EditContext(context)
                  {
                    ParentElement = flag3 ? context.ParentElement : parentElement2,
                    ViewScope = sceneNode3,
                    IsHidden = flag4,
                    LastPrimarySelectedPath = (DocumentNodePath) null
                  };
                else
                  editContext2 = new EditContext()
                  {
                    ParentElement = parentElement2,
                    ViewModel = this.ViewModel,
                    EditingContainerPath = containerNodePath,
                    ViewScope = sceneNode3,
                    IsHidden = flag4,
                    LastPrimarySelectedPath = (DocumentNodePath) null
                  };
                editContext2.Freeze();
                ITriggerContainer triggerContainer = sceneNode2 as ITriggerContainer;
                if (triggerContainer != null && triggerContainer.CanEditTriggers)
                {
                  foreach (TriggerBaseNode triggerBaseNode in (IEnumerable<TriggerBaseNode>) triggerContainer.VisualTriggers)
                  {
                    if (targetPath.Contains(triggerBaseNode.DocumentNode))
                    {
                      editContext2 = new EditContext(editContext2)
                      {
                        Trigger = triggerBaseNode
                      };
                      editContext2.Freeze();
                      break;
                    }
                  }
                }
                this.PushActiveEditContextInternal(editContext2);
                this.UpdateParentDrillInHistory(editContext2);
                parentElement2 = !flag2 || !viewScoped ? new NodeViewPair(this.ViewModel.DefaultView, this.ViewModel.GetAncestorInEditingContainer(list[index2 - 1], containerNodePath, (DocumentNodePath) null)) : parentElement1;
                flag3 = false;
                flag4 = true;
                flag2 = false;
              }
              EditContext contextLinkedToParent = editContext1.CreateContextLinkedToParent(parentElement2);
              this.PushActiveEditContextInternal(contextLinkedToParent);
              this.UpdateParentDrillInHistory(contextLinkedToParent);
            }
            else
            {
              if (!flag1)
                editContext1 = this.ActiveEditContext;
              bool flag2 = false;
              if ((parentElement1 != null || viewScoped) && list.Count > 1)
              {
                DocumentNodePath containerNodePath = list[list.Count - 2].GetContainerNodePath();
                if (parentElement1 == null || !(flag2 = this.DetectCycle(parentElement1.NodePath.GetContainerNodePath(), containerNodePath)))
                {
                  this.LinkContainerDrillInHistory(parentElement1, containerNodePath);
                  NodeViewPair parentElement2 = parentElement1;
                  if (!containerNodePath.Equals((object) editContext1.EditingContainerPath))
                    parentElement2 = new NodeViewPair(this.ViewModel.DefaultView, this.ViewModel.GetAncestorInEditingContainer(editContext1.EditingContainerPath, containerNodePath, (DocumentNodePath) null));
                  editContext1 = editContext1.CreateContextLinkedToParent(parentElement2);
                }
              }
              if (!flag2)
                this.editContextHistory.ReplaceActive(editContext1);
            }
          }
          editTransaction.Commit();
        }
      }
    }

    private bool DetectCycle(DocumentNodePath parentPath, DocumentNodePath childPath)
    {
      bool foundCycle = false;
      bool foundChild = false;
      this.MultiViewModelEditContextWalker.Walk(false, (MultiHistoryCallback) ((context, selectedElementPath, ownerPropertyKey, isGhosted) =>
      {
        if (context.EditingContainerPath.Equals((object) childPath))
          foundChild = true;
        if (!context.EditingContainerPath.Equals((object) parentPath))
          return false;
        if (foundChild)
          foundCycle = true;
        return true;
      }));
      return foundCycle;
    }

    private void LinkContainerDrillInHistory(NodeViewPair parentElement, DocumentNodePath childEditingContainerPath)
    {
      EditContext context = this.editContextHistory.GetContext(childEditingContainerPath);
      this.LinkContainerDrillInHistory(parentElement, context);
    }

    private void LinkContainerDrillInHistory(NodeViewPair parentElement, EditContext childEditContext)
    {
      childEditContext = childEditContext.CreateContextLinkedToParent(parentElement);
      childEditContext.ViewModel.EditContextManager.ReplaceContext(childEditContext);
      this.UpdateParentDrillInHistory(childEditContext);
    }

    private void ReplaceContext(EditContext editContext)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
      {
        this.editContextHistory.ReplaceContext(editContext);
        editTransaction.Commit();
      }
    }

    private void UpdateParentDrillInHistory(EditContext editContext)
    {
      if (editContext.ParentElement == null)
        return;
      DocumentNodePath nodePath = editContext.ParentElement.NodePath;
      SceneViewModel viewModel = editContext.ParentElement.GetViewModel(this.ViewModel.DesignerContext);
      if (viewModel == null || nodePath == null)
        return;
      IPropertyId propertyKey = editContext.ParentElement.PropertyKey;
      NodeViewPair childEditingContainer = new NodeViewPair(editContext.ViewModel.DefaultView, editContext.EditingContainerPath, propertyKey);
      viewModel.EditContextManager.UpdateDrillInHistory(nodePath, childEditingContainer);
    }

    private void UpdateDrillInHistory(DocumentNodePath selectedElementPath, NodeViewPair childEditingContainer)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
      {
        this.editContextHistory.UpdateDrillInHistory(selectedElementPath, childEditingContainer);
        editTransaction.Commit();
      }
    }

    private void PushActiveEditContextInternal(EditContext editContext)
    {
      if (this.editContextHistory.MoveToEditContext(editContext))
        return;
      this.editContextHistory.Push(editContext);
    }

    private void SetEditContext(EditContext editContext)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetActiveEditingContainer, true))
      {
        this.ViewModel.LockedInsertionPoint = (ISceneInsertionPoint) null;
        this.ViewModel.ElementSelectionSet.Clear();
        this.editContextHistory.RemoveContextsUnderNodePath(this.RootNodePath);
        this.editContextHistory.ReplaceActive(editContext);
        editTransaction.Commit();
      }
    }

    private EditContext CreateDefaultEditContext()
    {
      EditContext editContext = new EditContext()
      {
        ViewModel = this.viewModel,
        EditingContainerPath = this.RootNodePath,
        IsHidden = false,
        LastPrimarySelectedPath = (DocumentNodePath) null
      };
      editContext.Freeze();
      return editContext;
    }

    private sealed class EditContextHistory : IEditContextHistory
    {
      private Dictionary<DocumentNodePath, EditContext> contextDictionary = new Dictionary<DocumentNodePath, EditContext>();
      private Dictionary<DocumentNodePath, NodeViewPair> drillInDictionary = new Dictionary<DocumentNodePath, NodeViewPair>();
      private SceneViewModel viewModel;
      private DocumentNodePath activeEditingContainerPath;
      private EditContext cachedActiveEditContext;
      private DocumentNodePath rootNodePath;
      private bool forceCanonicalizeOnComplete;

      public EditContext ActiveEditContext
      {
        get
        {
          if (this.ActiveEditingContainerPath == null)
          {
            EditContext editContext = new EditContext()
            {
              ViewModel = this.ViewModel
            };
            editContext.Freeze();
            return editContext;
          }
          if (this.cachedActiveEditContext == null)
            this.cachedActiveEditContext = this.contextDictionary[this.ActiveEditingContainerPath];
          return this.cachedActiveEditContext;
        }
      }

      public EditContext NextActiveParentContext
      {
        get
        {
          return this.GetNextActiveParentContext(this.ActiveEditContext);
        }
      }

      private DocumentNodePath ActiveEditingContainerPath
      {
        get
        {
          return this.activeEditingContainerPath;
        }
        set
        {
          if ((this.activeEditingContainerPath != null || value == null) && (this.activeEditingContainerPath == null || this.activeEditingContainerPath.Equals((object) value)))
            return;
          this.cachedActiveEditContext = (EditContext) null;
          this.activeEditingContainerPath = value;
        }
      }

      private SceneViewModel ViewModel
      {
        get
        {
          return this.viewModel;
        }
      }

      private EditContext TopEditContext
      {
        get
        {
          EditContext editContext = this.ActiveEditContext;
          EditContext parentContext;
          while ((parentContext = this.GetParentContext(editContext)) != null)
            editContext = parentContext;
          return editContext;
        }
      }

      private EditContext DeepestEditContext
      {
        get
        {
          EditContext editContext = this.ActiveEditContext;
          EditContext childContext;
          while ((childContext = this.GetChildContext(editContext)) != null)
            editContext = childContext;
          return editContext;
        }
      }

      public EditContextHistory(SceneViewModel viewModel, EditContext rootContext)
      {
        this.viewModel = viewModel;
        this.ActiveEditingContainerPath = this.rootNodePath = rootContext.EditingContainerPath;
        if (rootContext.EditingContainerPath == null)
          return;
        this.contextDictionary[rootContext.EditingContainerPath] = rootContext;
      }

      public EditContext GetNextActiveParentContext(EditContext editContext, bool crossView)
      {
        EditContext editContext1 = this.GetParentContext(editContext, crossView);
        if (editContext1 == null && editContext.EditingContainerPath != null && !editContext.EditingContainerPath.Equals((object) this.rootNodePath))
          editContext1 = this.contextDictionary[this.rootNodePath];
        return editContext1;
      }

      public EditContext GetParentContext(EditContext editContext, bool crossView)
      {
        return this.GetContextFromNodeViewPair(this.GetParentNodeViewPair(editContext), crossView) ?? (EditContext) null;
      }

      public EditContext GetChildContext(EditContext editContext, bool crossView)
      {
        return this.GetContextFromNodeViewPair(this.GetChildNodeViewPair(editContext), crossView);
      }

      public EditContext GetContext(DocumentNodePath editingContainerPath)
      {
        EditContext editContext = (EditContext) null;
        this.contextDictionary.TryGetValue(editingContainerPath, out editContext);
        return editContext;
      }

      public EditContext GetContextFromNodeViewPair(NodeViewPair nodeViewPair, bool crossView)
      {
        if (nodeViewPair != null)
        {
          SceneViewModel viewModel = nodeViewPair.GetViewModel(this.ViewModel.DesignerContext);
          DocumentNodePath nodePath = nodeViewPair.NodePath;
          if (nodePath != null)
          {
            if (viewModel == null || viewModel == this.ViewModel)
              return this.GetContext(nodePath.GetContainerNodePath());
            if (viewModel != null && crossView)
              return viewModel.EditContextManager.editContextHistory.GetContextFromNodeViewPair(nodeViewPair, false);
          }
        }
        return (EditContext) null;
      }

      public NodeViewPair GetChildNodeViewPair(EditContext editContext)
      {
        DocumentNodePath selectedElementPath = EditContextHelper.SelectedElementPathInEditContext(this.ViewModel, editContext, true);
        if (selectedElementPath != null)
          return this.GetDrillInEntry(selectedElementPath);
        return (NodeViewPair) null;
      }

      public bool Walk(EditContext startAtContext, bool reverseWalk, SingleHistoryCallback callback)
      {
        if (this.ActiveEditingContainerPath == null)
          return false;
        return this.Walk(startAtContext, (EditContext) null, reverseWalk, callback);
      }

      public bool Walk(bool includeGhostedHistory, bool reverseWalk, SingleHistoryCallback callback)
      {
        if (this.ActiveEditingContainerPath == null)
          return false;
        EditContext startAtContext = (EditContext) null;
        EditContext stopAtContext = (EditContext) null;
        if (reverseWalk)
          startAtContext = includeGhostedHistory ? (EditContext) null : this.ActiveEditContext;
        else
          stopAtContext = includeGhostedHistory ? (EditContext) null : this.ActiveEditContext;
        return this.Walk(startAtContext, stopAtContext, reverseWalk, callback);
      }

      public void Push(EditContext editContext)
      {
        this.UpdateLastPrimarySelectedPathForActiveContext(editContext.EditingContainerPath);
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation.Add));
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.MoveActiveEditContextUndoUnit(this, editContext.EditingContainerPath));
      }

      public void Canonicalize(SceneUpdateTypeFlags flags, DocumentNodeChangeList damage)
      {
        if ((flags & SceneUpdateTypeFlags.Updated) != SceneUpdateTypeFlags.None && damage.Count > 0)
          this.forceCanonicalizeOnComplete = true;
        if ((flags & SceneUpdateTypeFlags.Canceled) != SceneUpdateTypeFlags.None)
          this.forceCanonicalizeOnComplete = false;
        if ((flags & SceneUpdateTypeFlags.Completing) == SceneUpdateTypeFlags.None || damage.Count <= 0 && !this.forceCanonicalizeOnComplete)
          return;
        this.forceCanonicalizeOnComplete = false;
        ExpressionEvaluator evaluator = new ExpressionEvaluator(this.ViewModel.DocumentRootResolver);
        Dictionary<DocumentNodePath, EditContext> contextsToRemove = new Dictionary<DocumentNodePath, EditContext>();
        List<EditContextManager.EditContextHistory.UpdateEditContextData> list1 = new List<EditContextManager.EditContextHistory.UpdateEditContextData>();
        foreach (KeyValuePair<DocumentNodePath, EditContext> keyValuePair in this.contextDictionary)
        {
          DocumentNodePath key = keyValuePair.Key;
          EditContext context = keyValuePair.Value;
          EditContextManager.EditContextHistory.UpdateEditContextData updateEditContextData = new EditContextManager.EditContextHistory.UpdateEditContextData(context);
          if (!key.PathReevaluates(evaluator))
          {
            contextsToRemove.Add(key, context);
          }
          else
          {
            if (context.Timeline != null && !context.Timeline.IsInDocument || context.Trigger != null && !context.Trigger.IsInDocument)
              updateEditContextData.UpdateFlags |= EditContextManager.EditContextHistory.UpdateEditContextFlags.TimelineTrigger;
            if (context.StateEditTarget != null && (!context.StateEditTarget.IsInDocument || context.Timeline != null && !context.Timeline.IsInDocument))
              updateEditContextData.UpdateFlags |= EditContextManager.EditContextHistory.UpdateEditContextFlags.StateEditTarget;
            if (context.TransitionEditTarget != null && (!context.TransitionEditTarget.IsInDocument || context.Timeline != null && !context.Timeline.IsInDocument))
              updateEditContextData.UpdateFlags |= EditContextManager.EditContextHistory.UpdateEditContextFlags.TransitionEditTarget;
            DocumentNodePath primarySelectedPath = context.LastPrimarySelectedPath;
            if (primarySelectedPath != null && !primarySelectedPath.PathReevaluates(evaluator))
              updateEditContextData.UpdateFlags |= EditContextManager.EditContextHistory.UpdateEditContextFlags.SelectedPath;
            IList<VisualStateSceneNode> pinnedStates = context.PinnedStates;
            if (pinnedStates != null)
            {
              for (int index = 0; index < pinnedStates.Count; ++index)
              {
                VisualStateSceneNode visualStateSceneNode = pinnedStates[index];
                if (visualStateSceneNode != null && !visualStateSceneNode.IsInDocument)
                {
                  updateEditContextData.UpdateFlags |= EditContextManager.EditContextHistory.UpdateEditContextFlags.PinnedStates;
                  break;
                }
              }
            }
            if (updateEditContextData.UpdateFlags != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
              list1.Add(updateEditContextData);
          }
        }
        List<VisualStateSceneNode> list2 = new List<VisualStateSceneNode>();
        bool flag1 = false;
        foreach (EditContextManager.EditContextHistory.UpdateEditContextData updateEditContextData in list1)
        {
          EditContextManager.EditContextHistory.UpdateEditContextFlags updateFlags = updateEditContextData.UpdateFlags;
          EditContext context = updateEditContextData.Context;
          EditContext editContext = (EditContext) null;
          bool flag2 = false;
          if ((updateFlags & EditContextManager.EditContextHistory.UpdateEditContextFlags.PinnedStates) != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
          {
            if (editContext == null)
              editContext = new EditContext(context);
            IList<VisualStateSceneNode> pinnedStates = context.PinnedStates;
            for (int index = 0; index < pinnedStates.Count; ++index)
            {
              VisualStateSceneNode visualStateSceneNode = pinnedStates[index];
              if (visualStateSceneNode != null && visualStateSceneNode.IsInDocument)
                list2.Add(visualStateSceneNode);
            }
            editContext.PinnedStates = (IList<VisualStateSceneNode>) list2;
            list2.Clear();
          }
          if ((updateFlags & EditContextManager.EditContextHistory.UpdateEditContextFlags.StateEditTarget) != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
          {
            if (editContext == null)
              editContext = new EditContext(context);
            editContext.StateStoryboardEditTarget = (StoryboardTimelineSceneNode) null;
            editContext.StateEditTarget = (VisualStateSceneNode) null;
            flag2 = true;
          }
          if ((updateFlags & EditContextManager.EditContextHistory.UpdateEditContextFlags.TransitionEditTarget) != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
          {
            if (editContext == null)
              editContext = new EditContext(context);
            editContext.TransitionEditTarget = (VisualStateTransitionSceneNode) null;
            editContext.TransitionStoryboardEditTarget = (StoryboardTimelineSceneNode) null;
            flag2 = true;
          }
          if ((updateFlags & EditContextManager.EditContextHistory.UpdateEditContextFlags.TimelineTrigger) != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
          {
            if (editContext == null)
              editContext = new EditContext(context);
            StoryboardTimelineSceneNode timeline = editContext.Timeline;
            if (timeline != null && !timeline.IsInDocument)
              flag2 = true;
            TriggerBaseNode trigger = editContext.Trigger;
            if (trigger != null && !trigger.IsInDocument)
              editContext.Trigger = (TriggerBaseNode) null;
          }
          if (flag2)
            editContext.Timeline = (StoryboardTimelineSceneNode) null;
          if ((updateFlags & EditContextManager.EditContextHistory.UpdateEditContextFlags.SelectedPath) != EditContextManager.EditContextHistory.UpdateEditContextFlags.None)
          {
            if (editContext == null)
              editContext = new EditContext(context);
            editContext.LastPrimarySelectedPath = (DocumentNodePath) null;
            flag1 = true;
          }
          if (editContext != null)
          {
            editContext.Freeze();
            this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation.Replace));
          }
        }
        if (contextsToRemove.Count > 0)
        {
          this.RemoveContexts(contextsToRemove);
          flag1 = true;
        }
        if (!flag1)
        {
          foreach (KeyValuePair<DocumentNodePath, NodeViewPair> keyValuePair in this.drillInDictionary)
          {
            if (!this.DrillInPathIsValid(keyValuePair.Key, keyValuePair.Value))
            {
              flag1 = true;
              break;
            }
          }
        }
        if (!flag1)
          return;
        this.ViewModel.NotifyEditContextHistoryChanged();
      }

      public void RemoveContextsUnderNodePath(DocumentNodePath targetNodePath)
      {
        Dictionary<DocumentNodePath, EditContext> contextsToRemove = new Dictionary<DocumentNodePath, EditContext>();
        foreach (KeyValuePair<DocumentNodePath, EditContext> keyValuePair in this.contextDictionary)
        {
          if (!keyValuePair.Key.IsValid() || targetNodePath.IsAncestorOf(keyValuePair.Key) && !targetNodePath.Equals((object) keyValuePair.Key))
            contextsToRemove.Add(keyValuePair.Key, keyValuePair.Value);
        }
        this.RemoveContexts(contextsToRemove);
      }

      public void MoveToParent()
      {
        this.MoveActivePosition(-1);
      }

      public bool MoveToEditContext(DocumentNodePath editingContainerPath)
      {
        return this.MoveToEditContext(this.GetContext(editingContainerPath));
      }

      public bool MoveToEditContext(EditContext editContext)
      {
        if (editContext == null)
          return true;
        bool flag1 = false;
        bool flag2 = false;
        int moveDirection = 0;
        EditContext editContext1 = this.DeepestEditContext;
        do
        {
          if (editContext1.EditingContainerPath.Equals((object) this.ActiveEditingContainerPath))
            flag1 = true;
          if (editContext1.EditingContainerPath.Equals((object) editContext.EditingContainerPath))
            flag2 = true;
          if (flag1 && flag2)
          {
            this.MoveActivePosition(moveDirection);
            this.ReplaceActive(editContext);
            return true;
          }
          if (flag1 && !flag2)
            --moveDirection;
          else if (!flag1 && flag2)
            ++moveDirection;
        }
        while ((editContext1 = this.GetNextActiveParentContext(editContext1)) != null);
        EditContext editContext2;
        if (!this.contextDictionary.TryGetValue(editContext.EditingContainerPath, out editContext2))
          return false;
        this.UpdateLastPrimarySelectedPathForActiveContext();
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.MoveActiveEditContextUndoUnit(this, editContext.EditingContainerPath));
        this.ReplaceActive(editContext);
        return true;
      }

      public void UpdateDrillInHistory(DocumentNodePath selectedElementPath, NodeViewPair childEditingContainer)
      {
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.UpdateDrillInHistoryUndoUnit(this, selectedElementPath, childEditingContainer));
      }

      public void ReplaceContext(EditContext editContext)
      {
        if (this.GetContext(editContext.EditingContainerPath).Equals((object) editContext))
          return;
        DocumentNodePath primarySelectedPath = this.ActiveEditContext.LastPrimarySelectedPath;
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation.Replace));
        this.UpdateLastPrimarySelectedPath(this.ActiveEditContext, primarySelectedPath);
      }

      public void ReplaceActive(EditContext editContext)
      {
        if ((editContext != null || this.ActiveEditingContainerPath == null) && this.ActiveEditContext.Equals((object) editContext))
          return;
        DocumentNodePath primarySelectedPath = this.ActiveEditContext.LastPrimarySelectedPath;
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation.ReplaceActive));
        if (this.ActiveEditingContainerPath == null)
          return;
        this.UpdateLastPrimarySelectedPath(this.ActiveEditContext, primarySelectedPath);
      }

      private void UpdateLastPrimarySelectedPath(EditContext editContext, DocumentNodePath selectedPath)
      {
        EditContext editContext1 = new EditContext(editContext)
        {
          LastPrimarySelectedPath = selectedPath
        };
        editContext1.Freeze();
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext1, EditContextManager.EditContextHistory.ChangeEditContextOperation.Replace));
      }

      private void UpdateLastPrimarySelectedPathForActiveContext()
      {
        this.UpdateLastPrimarySelectedPathForActiveContext((DocumentNodePath) null);
      }

      private void UpdateLastPrimarySelectedPathForActiveContext(DocumentNodePath nextEditingContainerPath)
      {
        DocumentNodePath selectedPath = (DocumentNodePath) null;
        if (nextEditingContainerPath != null)
          selectedPath = this.ViewModel.GetAncestorInEditingContainer(nextEditingContainerPath, this.ActiveEditingContainerPath, (DocumentNodePath) null);
        if (selectedPath == null && this.viewModel.ElementSelectionSet.Count > 0)
        {
          selectedPath = this.viewModel.ElementSelectionSet.PrimarySelection.DocumentNodePath;
          if (!this.ActiveEditingContainerPath.Equals((object) selectedPath.GetContainerNodePath()))
            selectedPath = this.ActiveEditingContainerPath;
        }
        this.UpdateLastPrimarySelectedPath(this.ActiveEditContext, selectedPath);
      }

      private void MoveActivePosition(int moveDirection)
      {
        EditContext editContext = this.ActiveEditContext;
        if (moveDirection <= 0)
        {
          SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
          DocumentNodePath selectedPath = (DocumentNodePath) null;
          if (primarySelection != null)
            selectedPath = primarySelection.DocumentNodePath;
          while (editContext != null && moveDirection++ < 0)
          {
            this.UpdateLastPrimarySelectedPath(editContext, selectedPath);
            NodeViewPair parentNodeViewPair = this.GetParentNodeViewPair(editContext);
            editContext = this.GetNextActiveParentContext(editContext);
            selectedPath = (DocumentNodePath) null;
            if (editContext != null && parentNodeViewPair != null && (parentNodeViewPair.NodePath != null && parentNodeViewPair.GetViewModel(this.ViewModel.DesignerContext) == this.ViewModel))
              selectedPath = this.ViewModel.GetAncestorInEditingContainer(parentNodeViewPair.NodePath, editContext.EditingContainerPath, (DocumentNodePath) null);
          }
        }
        else
        {
          while (editContext != null && moveDirection-- > 0)
            editContext = this.GetChildContext(editContext);
        }
        if (editContext == null)
          throw new InvalidOperationException();
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.MoveActiveEditContextUndoUnit(this, editContext.EditingContainerPath));
      }

      private NodeViewPair GetParentNodeViewPair(EditContext editContext)
      {
        return editContext.ParentElement;
      }

      private EditContext GetParentContext(EditContext editContext)
      {
        return this.GetParentContext(editContext, false);
      }

      private EditContext GetChildContext(EditContext editContext)
      {
        return this.GetChildContext(editContext, false);
      }

      private EditContext GetNextActiveParentContext(EditContext editContext)
      {
        return this.GetNextActiveParentContext(editContext, false);
      }

      private NodeViewPair GetDrillInEntry(DocumentNodePath selectedElementPath)
      {
        NodeViewPair drillInContainer = (NodeViewPair) null;
        if (this.drillInDictionary.TryGetValue(selectedElementPath, out drillInContainer) && !this.DrillInPathIsValid(selectedElementPath, drillInContainer))
          return (NodeViewPair) null;
        return drillInContainer;
      }

      private bool DrillInPathIsValid(DocumentNodePath selectedElementPath, NodeViewPair drillInContainer)
      {
        if (drillInContainer == null)
          return false;
        DocumentNodePath nodePath = drillInContainer.NodePath;
        if (nodePath == null)
          return false;
        SceneViewModel viewModel = drillInContainer.GetViewModel(this.ViewModel.DesignerContext);
        return viewModel != null && viewModel.EditContextManager.ContextExistsForEditingContainer(nodePath) && NodeViewPair.EvaluateExpression(this.ViewModel, selectedElementPath, drillInContainer.PropertyKey, nodePath);
      }

      private bool Walk(EditContext startAtContext, EditContext stopAtContext, bool reverseWalk, SingleHistoryCallback callback)
      {
        EditContext editContext1 = startAtContext ?? (!reverseWalk ? this.TopEditContext : this.DeepestEditContext);
        List<EditContext> list = new List<EditContext>();
        bool isGhosted = reverseWalk;
        EditContext editContext2;
        do
        {
          editContext2 = editContext1;
          if (!list.Contains(editContext2))
          {
            list.Add(editContext2);
            if (reverseWalk && editContext2 == this.ActiveEditContext)
              isGhosted = false;
            if (callback(editContext2, isGhosted))
              return true;
            if (!reverseWalk && editContext2 == this.ActiveEditContext)
              isGhosted = true;
            editContext1 = reverseWalk ? this.GetParentContext(editContext2) : this.GetChildContext(editContext2);
          }
          else
            break;
        }
        while (editContext1 != null && (stopAtContext == null || editContext2 != stopAtContext));
        return false;
      }

      private void RemoveContexts(Dictionary<DocumentNodePath, EditContext> contextsToRemove)
      {
        if (contextsToRemove.Count == 0)
          return;
        this.UpdateToValidActiveEditingContainer(contextsToRemove);
        foreach (KeyValuePair<DocumentNodePath, EditContext> keyValuePair in contextsToRemove)
          this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, keyValuePair.Value, EditContextManager.EditContextHistory.ChangeEditContextOperation.Remove));
        List<DocumentNodePath> list1 = new List<DocumentNodePath>();
        foreach (KeyValuePair<DocumentNodePath, NodeViewPair> keyValuePair in this.drillInDictionary)
        {
          ExpressionEvaluator evaluator = new ExpressionEvaluator(this.ViewModel.DocumentRootResolver);
          if (!keyValuePair.Key.PathReevaluates(evaluator) || keyValuePair.Value.GetViewModel(this.ViewModel.DesignerContext) == this.ViewModel && (keyValuePair.Value.NodePath == null || this.GetContext(keyValuePair.Value.NodePath) == null))
            list1.Add(keyValuePair.Key);
        }
        foreach (DocumentNodePath selectedElementPath in list1)
          this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.UpdateDrillInHistoryUndoUnit(this, selectedElementPath, (NodeViewPair) null));
        List<EditContext> list2 = new List<EditContext>();
        foreach (KeyValuePair<DocumentNodePath, EditContext> keyValuePair in this.contextDictionary)
        {
          NodeViewPair parentElement = keyValuePair.Value.ParentElement;
          if (parentElement != null && parentElement.GetViewModel(this.ViewModel.DesignerContext) == this.ViewModel && (parentElement.NodePath == null || this.GetContext(parentElement.NodePath.GetContainerNodePath()) == null))
            list2.Add(keyValuePair.Value);
        }
        foreach (EditContext oldContext in list2)
        {
          EditContext editContext = new EditContext(oldContext)
          {
            ParentElement = (NodeViewPair) null
          };
          editContext.Freeze();
          this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.ChangeEditContextUndoUnit(this, editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation.Replace));
        }
      }

      private void UpdateToValidActiveEditingContainer(Dictionary<DocumentNodePath, EditContext> contextsToRemove)
      {
        DocumentNodePath documentNodePath;
        EditContext editContext;
        EditContext activeParentContext;
        for (documentNodePath = this.ActiveEditingContainerPath; documentNodePath != null && contextsToRemove.TryGetValue(documentNodePath, out editContext); documentNodePath = activeParentContext != null ? activeParentContext.EditingContainerPath : (DocumentNodePath) null)
          activeParentContext = this.GetNextActiveParentContext(editContext);
        if (documentNodePath == this.ActiveEditingContainerPath)
          return;
        this.ViewModel.AddUndoUnit((IUndoUnit) new EditContextManager.EditContextHistory.MoveActiveEditContextUndoUnit(this, documentNodePath));
      }

      private void AddEditContextInternal(EditContext editContext)
      {
        this.contextDictionary.Add(editContext.EditingContainerPath, editContext);
      }

      private void RemoveEditContextInternal(EditContext editContext)
      {
        this.contextDictionary.Remove(editContext.EditingContainerPath);
      }

      private EditContext ReplaceEditContextInternal(EditContext editContext)
      {
        if (editContext.EditingContainerPath.Equals((object) this.ActiveEditingContainerPath))
          return this.ChangeActiveEditContextInternal(editContext);
        EditContext oldContext = this.contextDictionary[editContext.EditingContainerPath];
        this.contextDictionary[editContext.EditingContainerPath] = editContext;
        this.ViewModel.NotifyInactiveEditContextChanged(oldContext, editContext);
        return oldContext;
      }

      private EditContext ChangeActiveEditContextInternal(EditContext editContext)
      {
        EditContext activeEditContext = this.ActiveEditContext;
        ISceneInsertionPoint sceneInsertionPoint = this.ViewModel.ActiveSceneInsertionPoint;
        if (editContext.EditingContainerPath == null)
        {
          if (this.ActiveEditingContainerPath != null)
          {
            this.contextDictionary.Remove(this.ActiveEditingContainerPath);
            if (this.contextDictionary.Count == 0)
              this.rootNodePath = (DocumentNodePath) null;
            this.cachedActiveEditContext = (EditContext) null;
            this.ActiveEditingContainerPath = (DocumentNodePath) null;
          }
        }
        else
        {
          if (this.contextDictionary.Count == 0)
            this.rootNodePath = editContext.EditingContainerPath;
          this.contextDictionary[editContext.EditingContainerPath] = editContext;
          this.cachedActiveEditContext = (EditContext) null;
          this.ActiveEditingContainerPath = editContext.EditingContainerPath;
        }
        this.ViewModel.NotifyActiveEditContextChanged(activeEditContext, this.ActiveEditContext, sceneInsertionPoint, this.ViewModel.ActiveSceneInsertionPoint);
        return activeEditContext;
      }

      private DocumentNodePath MoveActivePathInternal(DocumentNodePath path)
      {
        EditContext activeEditContext = this.ActiveEditContext;
        ISceneInsertionPoint sceneInsertionPoint = this.ViewModel.ActiveSceneInsertionPoint;
        this.ActiveEditingContainerPath = path;
        this.ViewModel.NotifyActiveEditContextChanged(activeEditContext, this.ActiveEditContext, sceneInsertionPoint, this.ViewModel.ActiveSceneInsertionPoint);
        return activeEditContext.EditingContainerPath;
      }

      private NodeViewPair UpdateDrillInHistoryInternal(DocumentNodePath selectedElementPath, NodeViewPair childEditingContainer)
      {
        NodeViewPair nodeViewPair = (NodeViewPair) null;
        this.drillInDictionary.TryGetValue(selectedElementPath, out nodeViewPair);
        if (childEditingContainer == null)
          this.drillInDictionary.Remove(selectedElementPath);
        else
          this.drillInDictionary[selectedElementPath] = childEditingContainer;
        this.ViewModel.NotifyEditContextHistoryChanged();
        return nodeViewPair;
      }

      [Flags]
      private enum UpdateEditContextFlags
      {
        None = 0,
        Remove = 1,
        TimelineTrigger = 2,
        StateEditTarget = 4,
        SelectedPath = 8,
        PinnedStates = 16,
        TransitionEditTarget = 32,
      }

      private struct UpdateEditContextData
      {
        private EditContext context;
        private EditContextManager.EditContextHistory.UpdateEditContextFlags flags;

        public EditContext Context
        {
          get
          {
            return this.context;
          }
        }

        public EditContextManager.EditContextHistory.UpdateEditContextFlags UpdateFlags
        {
          get
          {
            return this.flags;
          }
          set
          {
            this.flags = value;
          }
        }

        public UpdateEditContextData(EditContext context)
        {
          this.context = context;
          this.flags = EditContextManager.EditContextHistory.UpdateEditContextFlags.None;
        }
      }

      private abstract class EditContextUndoUnit : UndoUnit
      {
        private EditContextManager.EditContextHistory editContextHistory;

        public override bool IsHidden
        {
          get
          {
            return true;
          }
        }

        protected EditContextManager.EditContextHistory EditContextHistory
        {
          get
          {
            return this.editContextHistory;
          }
        }

        protected EditContextUndoUnit(EditContextManager.EditContextHistory editContextHistory)
        {
          this.editContextHistory = editContextHistory;
        }

        public override void Undo()
        {
          this.Toggle();
          base.Undo();
        }

        public override void Redo()
        {
          this.Toggle();
          base.Redo();
        }

        protected abstract void Toggle();
      }

      private enum ChangeEditContextOperation
      {
        Add,
        Remove,
        Replace,
        ReplaceActive,
      }

      private sealed class ChangeEditContextUndoUnit : EditContextManager.EditContextHistory.EditContextUndoUnit
      {
        private EditContext editContext;
        private EditContextManager.EditContextHistory.ChangeEditContextOperation operation;

        public ChangeEditContextUndoUnit(EditContextManager.EditContextHistory editContextHistory, EditContext editContext, EditContextManager.EditContextHistory.ChangeEditContextOperation operation)
          : base(editContextHistory)
        {
          this.editContext = editContext;
          this.operation = operation;
        }

        protected override void Toggle()
        {
          switch (this.operation)
          {
            case EditContextManager.EditContextHistory.ChangeEditContextOperation.Add:
              this.EditContextHistory.AddEditContextInternal(this.editContext);
              this.operation = EditContextManager.EditContextHistory.ChangeEditContextOperation.Remove;
              break;
            case EditContextManager.EditContextHistory.ChangeEditContextOperation.Remove:
              this.EditContextHistory.RemoveEditContextInternal(this.editContext);
              this.operation = EditContextManager.EditContextHistory.ChangeEditContextOperation.Add;
              break;
            case EditContextManager.EditContextHistory.ChangeEditContextOperation.Replace:
              this.editContext = this.EditContextHistory.ReplaceEditContextInternal(this.editContext);
              break;
            case EditContextManager.EditContextHistory.ChangeEditContextOperation.ReplaceActive:
              this.editContext = this.EditContextHistory.ChangeActiveEditContextInternal(this.editContext);
              break;
          }
        }

        public override string ToString()
        {
          return "ChangeEditContext: " + this.operation.ToString() + " " + (this.editContext != null ? this.editContext.ToString() : "[null]");
        }
      }

      private sealed class MoveActiveEditContextUndoUnit : EditContextManager.EditContextHistory.EditContextUndoUnit
      {
        private DocumentNodePath activeEditingContainerPath;

        public MoveActiveEditContextUndoUnit(EditContextManager.EditContextHistory editContextHistory, DocumentNodePath activeEditingContainerPath)
          : base(editContextHistory)
        {
          this.activeEditingContainerPath = activeEditingContainerPath;
        }

        protected override void Toggle()
        {
          this.activeEditingContainerPath = this.EditContextHistory.MoveActivePathInternal(this.activeEditingContainerPath);
        }

        public override string ToString()
        {
          return "MoveActiveEditContext: " + (object) this.activeEditingContainerPath;
        }
      }

      private sealed class UpdateDrillInHistoryUndoUnit : EditContextManager.EditContextHistory.EditContextUndoUnit
      {
        private DocumentNodePath selectedElementPath;
        private NodeViewPair childEditingContainer;

        public UpdateDrillInHistoryUndoUnit(EditContextManager.EditContextHistory editContextHistory, DocumentNodePath selectedElementPath, NodeViewPair childEditingContainer)
          : base(editContextHistory)
        {
          this.selectedElementPath = selectedElementPath;
          this.childEditingContainer = childEditingContainer;
        }

        protected override void Toggle()
        {
          this.childEditingContainer = this.EditContextHistory.UpdateDrillInHistoryInternal(this.selectedElementPath, this.childEditingContainer);
        }

        public override string ToString()
        {
          return string.Concat(new object[4]
          {
            (object) "UpdateDrillInHistory: ",
            (object) this.selectedElementPath,
            (object) " ",
            (object) this.childEditingContainer
          });
        }
      }
    }

    private sealed class SelectionSetterForContainer : IDisposable
    {
      private SceneViewModel viewModel;
      private List<SceneElement> newSelection;
      private SceneElement primarySelection;

      public SelectionSetterForContainer(SceneViewModel viewModel, SceneNode editingContainer)
      {
        this.viewModel = viewModel;
        ReadOnlyCollection<SceneElement> selection = this.viewModel.ElementSelectionSet.Selection;
        this.primarySelection = viewModel.ElementSelectionSet.PrimarySelection;
        List<SceneElement> removedElements;
        this.newSelection = this.viewModel.GetSelectionForEditingContainer<SceneElement>(editingContainer, (ICollection<SceneElement>) selection, out removedElements);
      }

      public void Dispose()
      {
        this.viewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) this.newSelection, this.primarySelection);
      }
    }

    internal sealed class SingleHistoryWalker
    {
      private IEditContextHistory editContextHistory;

      public SingleHistoryWalker(IEditContextHistory editContextHistory)
      {
        this.editContextHistory = editContextHistory;
      }

      public bool Walk(bool includeGhostedHistory, bool reverseWalk, SingleHistoryCallback callback)
      {
        return this.editContextHistory.Walk(includeGhostedHistory, reverseWalk, callback);
      }

      public bool Walk(bool reverseWalk, SingleHistoryCallback callback)
      {
        return this.Walk(false, reverseWalk, callback);
      }
    }

    internal sealed class MultiHistoryWalker
    {
      private List<EditContext> alreadyWalkedEditContexts = new List<EditContext>();
      private SceneViewModel viewModel;

      public MultiHistoryWalker(SceneViewModel viewModel)
      {
        this.viewModel = viewModel;
      }

      public bool Walk(bool reverseWalk, MultiHistoryCallback callback)
      {
        try
        {
          if (this.WalkContexts(!reverseWalk, callback))
            return true;
          if (this.WalkContexts(reverseWalk, callback))
            return true;
        }
        finally
        {
          this.alreadyWalkedEditContexts.Clear();
        }
        return false;
      }

      private bool WalkContexts(bool aboveActiveContext, MultiHistoryCallback callback)
      {
        if (aboveActiveContext)
          return this.WalkContextsAboveActive(callback);
        return this.WalkContextsBelowActive(callback);
      }

      private bool WalkContextsAboveActive(MultiHistoryCallback callback)
      {
        List<EditContext> editContextsToOutput = new List<EditContext>();
        EditContext currentContext = this.viewModel.ActiveEditContext;
        while (currentContext != null && !currentContext.ViewModel.EditContextManager.editContextHistory.Walk(currentContext, true, (SingleHistoryCallback) ((context, isGhosted) =>
        {
          if (context != this.viewModel.ActiveEditContext && this.alreadyWalkedEditContexts.Contains(context))
            return true;
          this.alreadyWalkedEditContexts.Add(context);
          editContextsToOutput.Add(context);
          currentContext = context;
          return false;
        })))
          currentContext = currentContext.ViewModel.EditContextManager.editContextHistory.GetParentContext(currentContext, true);
        for (int index = editContextsToOutput.Count - 1; index >= 0; --index)
        {
          EditContext context = editContextsToOutput[index];
          DocumentNodePath selectedElementPath = (DocumentNodePath) null;
          if (context != this.viewModel.ActiveEditContext)
            selectedElementPath = editContextsToOutput[index - 1].ParentElement.NodePath;
          IPropertyId ownerPropertyKey = context.ParentElement != null ? context.ParentElement.PropertyKey : (IPropertyId) null;
          if (callback(context, selectedElementPath, ownerPropertyKey, false))
            return true;
        }
        return false;
      }

      private bool WalkContextsBelowActive(MultiHistoryCallback callback)
      {
        EditContext currentContext = this.viewModel.ActiveEditContext;
        IPropertyId propertyKey = (IPropertyId) null;
        NodeViewPair drillInContainer;
        for (; currentContext != null; currentContext = currentContext.ViewModel.EditContextManager.editContextHistory.GetContextFromNodeViewPair(drillInContainer, true))
        {
          drillInContainer = (NodeViewPair) null;
          if (currentContext.ViewModel.EditContextManager.editContextHistory.Walk(currentContext, false, (SingleHistoryCallback) ((context, isGhosted) =>
          {
            if (context != this.viewModel.ActiveEditContext && this.alreadyWalkedEditContexts.Contains(context))
              return true;
            this.alreadyWalkedEditContexts.Add(context);
            if (context != this.viewModel.ActiveEditContext && callback(context, (DocumentNodePath) null, propertyKey, true))
              return true;
            drillInContainer = context.ViewModel.EditContextManager.editContextHistory.GetChildNodeViewPair(context);
            propertyKey = drillInContainer != null ? drillInContainer.PropertyKey : (IPropertyId) null;
            currentContext = context;
            return false;
          })))
            return true;
        }
        return false;
      }
    }
  }
}

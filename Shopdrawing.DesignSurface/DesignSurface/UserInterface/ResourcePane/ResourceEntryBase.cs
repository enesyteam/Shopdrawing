// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceEntryBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.Utility;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public abstract class ResourceEntryBase : ISelectable, INotifyPropertyChanged, IDragDropHandler
  {
    private bool isSelected;
    private bool isExpanded;
    private bool inDragOver;

    protected abstract ResourceManager ResourceManager { get; }

    public abstract DocumentNode DocumentNode { get; }

    public abstract DocumentNodeMarker Marker { get; }

    public abstract object ToolTip { get; }

    public ICommand SingleSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.ResourceManager.SelectedItems.SetSelection(this)));
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          if (this.ResourceManager.SelectedItems.IsSelected(this))
            return;
          this.ResourceManager.SelectedItems.SetSelection(this);
        }));
      }
    }

    public ICommand ToggleSelectCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.ResourceManager.SelectedItems.ToggleSelection(this)));
      }
    }

    public ICommand ToggleExpandCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.IsExpanded = !this.IsExpanded));
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        if (this.isSelected == value)
          return;
        this.isSelected = value;
        this.OnPropertyChanged("IsSelected");
      }
    }

    public bool IsExpanded
    {
      get
      {
        return this.isExpanded;
      }
      set
      {
        if (this.isExpanded == value)
          return;
        this.isExpanded = value;
        this.OnPropertyChanged("IsExpanded");
      }
    }

    public bool IsEditing { get; set; }

    protected abstract ResourceContainer DragDropTargetContainer { get; }

    public bool InDragOver
    {
      get
      {
        return this.inDragOver;
      }
      set
      {
        if (value == this.inDragOver)
          return;
        this.inDragOver = value;
        this.OnPropertyChanged("InDragOver");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      if (propertyName != null)
      {
        int length = propertyName.Length;
      }
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void OnDragBegin(DragBeginEventArgs e)
    {
    }

    public virtual void OnDragOver(DragEventArgs e)
    {
      e.Handled = true;
      e.Effects = DragDropEffects.None;
      if (this.ResourceManager.IsFiltering)
        return;
      this.InDragOver = true;
      ResourceEntryItem resourceEntry = (ResourceEntryItem) null;
      if (!this.CanDrop(e, out resourceEntry))
        return;
      ResourceContainer dropTargetContainer = this.DragDropTargetContainer;
      if (dropTargetContainer.DocumentHasErrors || dropTargetContainer.DocumentNode == null)
        return;
      if (resourceEntry.Container != dropTargetContainer && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
        e.Effects = DragDropEffects.Copy;
      else
        e.Effects = DragDropEffects.Move;
    }

    public virtual void OnDragEnter(DragEventArgs e)
    {
      this.OnDragOver(e);
      e.Handled = true;
    }

    public virtual void OnDragLeave(DragEventArgs e)
    {
      this.InDragOver = false;
      e.Handled = true;
    }

    public virtual void OnDrop(DragEventArgs e)
    {
      this.InDragOver = false;
      e.Handled = true;
    }

    protected void DoDrop(DragEventArgs eventArgs, int destinationIndex)
    {
      IDataObject data = eventArgs.Data;
      if (this.ResourceManager.IsFiltering)
        return;
      ResourceEntryItem resourceEntry = (ResourceEntryItem) null;
      if (!this.CanDrop(eventArgs, out resourceEntry) || resourceEntry == this)
        return;
      ResourceContainer dropTargetContainer = this.DragDropTargetContainer;
      if (resourceEntry.Container == dropTargetContainer)
      {
        if ((eventArgs.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
        {
          using (SceneEditTransaction editTransaction = resourceEntry.Container.ViewModel.CreateEditTransaction(StringTable.UndoUnitCopyResource))
          {
            DocumentCompositeNode entryNode = resourceEntry.Resource.ResourceNode.Clone(dropTargetContainer.ViewModel.Document.DocumentContext) as DocumentCompositeNode;
            DocumentNode keyNode = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.GenerateUniqueResourceKey(resourceEntry.Key.ToString(), dropTargetContainer.Node);
            ResourceNodeHelper.SetResourceEntryKey(entryNode, keyNode);
            Microsoft.Expression.DesignSurface.Utility.ResourceHelper.AddResource((DictionaryEntryNode) dropTargetContainer.ViewModel.GetSceneNode((DocumentNode) entryNode), dropTargetContainer.ResourceDictionaryNode, destinationIndex);
            editTransaction.Commit();
          }
        }
        else
          resourceEntry.Container.MoveItem(resourceEntry, destinationIndex);
      }
      else
      {
        ObservableCollection<string> observableCollection = new ObservableCollection<string>();
        observableCollection.Add(dropTargetContainer.DocumentContext.DocumentUrl);
        bool skipReferencedResourceCopy = this.IsContainerReachable((IEnumerable<string>) observableCollection, resourceEntry.Container);
        if ((eventArgs.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
        {
          this.CopyItem(resourceEntry, dropTargetContainer, destinationIndex, skipReferencedResourceCopy);
        }
        else
        {
          ReferencesFoundModel referencingResources = resourceEntry.InteractiveGetReferencingResources(ReferencesFoundModel.UseScenario.DeleteResource);
          if (referencingResources == null)
            return;
          bool doReferenceFixup = false;
          if (referencingResources.ReferenceNames.Count > 0 && !this.IsContainerReachable((IEnumerable<string>) referencingResources.ScenesWithReferences, dropTargetContainer))
          {
            if (!new ReferencesFoundDialog(referencingResources).ShowDialog().GetValueOrDefault(false))
              return;
            doReferenceFixup = referencingResources.SelectedUpdateMethod != ReferencesFoundModel.UpdateMethod.DontFix;
          }
          if (dropTargetContainer.ViewModel == resourceEntry.Container.ViewModel)
          {
            using (SceneEditTransaction editTransaction = dropTargetContainer.ViewModel.CreateEditTransaction(StringTable.UndoUnitMoveResource))
            {
              if (this.MoveItem(resourceEntry, dropTargetContainer, destinationIndex, skipReferencedResourceCopy, referencingResources, doReferenceFixup))
                editTransaction.Commit();
              else
                editTransaction.Cancel();
            }
          }
          else
            this.MoveItem(resourceEntry, dropTargetContainer, destinationIndex, skipReferencedResourceCopy, referencingResources, doReferenceFixup);
        }
      }
    }

    private bool CanDrop(DragEventArgs e, out ResourceEntryItem resourceEntry)
    {
      resourceEntry = (ResourceEntryItem) null;
      ResourceContainer dropTargetContainer = this.DragDropTargetContainer;
      if (dropTargetContainer != null && !dropTargetContainer.DocumentHasErrors && dropTargetContainer.DocumentNode != null)
      {
        IDataObject data = e.Data;
        if (data != null && data.GetDataPresent("ResourceEntryItem", true))
        {
          resourceEntry = (ResourceEntryItem) data.GetData("ResourceEntryItem", true);
          if (resourceEntry != null && resourceEntry.DocumentNode != null && PlatformTypes.PlatformsCompatible(dropTargetContainer.ProjectContext.PlatformMetadata, resourceEntry.DocumentNode.PlatformMetadata))
            return true;
        }
      }
      return false;
    }

    private bool IsContainerReachable(IEnumerable<string> sourceDocuments, ResourceContainer targetContainer)
    {
      bool flag = true;
      if (targetContainer != this.ResourceManager.TopLevelResourceContainer)
      {
        List<ResourceDictionaryItem> list = new List<ResourceDictionaryItem>();
        this.ResourceManager.FindAllReachableDictionaries(this.ResourceManager.TopLevelResourceContainer, (ICollection<ResourceDictionaryItem>) list);
        foreach (string str in sourceDocuments)
        {
          ObservableCollection<ResourceDictionaryItem> observableCollection = new ObservableCollection<ResourceDictionaryItem>(list);
          if (StringComparer.OrdinalIgnoreCase.Compare(targetContainer.DocumentReference.Path, str) != 0)
          {
            flag = false;
            this.ResourceManager.FindAllReachableDictionaries(this.ResourceManager.FindResourceContainer(str), (ICollection<ResourceDictionaryItem>) observableCollection);
            foreach (ResourceDictionaryItem resourceDictionaryItem in (Collection<ResourceDictionaryItem>) observableCollection)
            {
              if (!string.IsNullOrEmpty(resourceDictionaryItem.DesignTimeSource) && StringComparer.OrdinalIgnoreCase.Compare(targetContainer.DocumentReference.Path, resourceDictionaryItem.DesignTimeSource) == 0)
              {
                flag = true;
                break;
              }
            }
          }
          if (!flag)
            break;
        }
      }
      return flag;
    }

    private bool MoveItem(ResourceEntryItem resourceEntry, ResourceContainer destinationContainer, int destinationIndex, bool skipReferencedResourceCopy, ReferencesFoundModel referencesFoundModel, bool doReferenceFixup)
    {
      bool flag = this.CopyItem(resourceEntry, destinationContainer, destinationIndex, skipReferencedResourceCopy);
      if (flag)
      {
        if (doReferenceFixup && !new AsyncProcessDialog(referencesFoundModel.FixReferencesAsync(), resourceEntry.Container.ViewModel.DesignerContext.ExpressionInformationService).ShowDialog().GetValueOrDefault(false))
          return false;
        DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) resourceEntry.Container.ViewModel.GetSceneNode((DocumentNode) resourceEntry.Resource.ResourceNode);
        using (SceneEditTransaction editTransaction = resourceEntry.Container.ViewModel.CreateEditTransaction(StringTable.UndoUnitDeleteResource))
        {
          resourceEntry.Container.ResourceDictionaryNode.Remove(dictionaryEntryNode);
          editTransaction.Commit();
        }
      }
      return flag;
    }

    private bool CopyItem(ResourceEntryItem primaryResource, ResourceContainer destinationContainer, int destinationIndex, bool skipReferencedResourceCopy)
    {
      ResourceConflictResolution conflictResolution = ResourceConflictResolution.UseExisting;
      DocumentNode node1 = primaryResource.Resource.ResourceNode.Clone(destinationContainer.ViewModel.Document.DocumentContext);
      DictionaryEntryNode primaryResource1 = (DictionaryEntryNode) destinationContainer.ViewModel.GetSceneNode(node1);
      ResourceEvaluation resourceEvaluation = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EvaluateResource(primaryResource1, destinationContainer.Node);
      List<DictionaryEntryNode> list1 = new List<DictionaryEntryNode>();
      if (!skipReferencedResourceCopy)
      {
        List<DocumentNode> foundResources = new List<DocumentNode>();
        Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindAllReferencedResources(primaryResource.DocumentNode, foundResources, (Microsoft.Expression.DesignSurface.Utility.ResourceHelper.PostOrderOperation) null);
        foreach (DocumentNode documentNode in foundResources)
        {
          DocumentNode node2 = documentNode.Parent.Clone(destinationContainer.ViewModel.Document.DocumentContext);
          list1.Add((DictionaryEntryNode) destinationContainer.ViewModel.GetSceneNode(node2));
        }
      }
      IList<ResourceEvaluation> list2 = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EvaluateResources((IList<DictionaryEntryNode>) list1, destinationContainer.Node);
      if (!Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CheckEvaluationResults(list2, (ResourceEvaluationResult) 7))
      {
        if (!Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CheckEvaluationResult(resourceEvaluation, ResourceEvaluationResult.ConflictingResourceExists, ResourceEvaluationResult.IdenticalResourceExists))
          goto label_10;
      }
      conflictResolution = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.PromptForResourceConflictResolution(ResourceConflictResolution.UseExisting | ResourceConflictResolution.RenameNew | ResourceConflictResolution.OverwriteOld);
      if (conflictResolution == ResourceConflictResolution.Undetermined)
        return false;
label_10:
      using (SceneEditTransaction editTransaction = destinationContainer.ViewModel.CreateEditTransaction(StringTable.UndoUnitCopyResource))
      {
        int num = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.AddResources(list2, (IList<SceneNode>) null, conflictResolution, destinationContainer.Node, destinationIndex);
        Microsoft.Expression.DesignSurface.Utility.ResourceHelper.AddPrimaryResource(resourceEvaluation, (IList<SceneNode>) null, conflictResolution, destinationContainer.Node, destinationIndex + num);
        ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(destinationContainer.ViewModel.DocumentRootResolver);
        SceneNode keyNode = primaryResource1.KeyNode;
        if (keyNode != null)
        {
          DocumentNode documentNode = expressionEvaluator.EvaluateResource(destinationContainer.Node.DocumentNodePath, keyNode.DocumentNode);
          foreach (DictionaryEntryNode dictionaryEntryNode in destinationContainer.ResourceDictionaryNode)
          {
            if (dictionaryEntryNode.Value.DocumentNode == documentNode)
            {
              this.ResourceManager.SelectedItems.SetSelection((ResourceEntryBase) this.ResourceManager.GetResourceItem(destinationContainer, dictionaryEntryNode.DocumentNode as DocumentCompositeNode));
              editTransaction.Commit();
              return true;
            }
          }
        }
        else
        {
          editTransaction.Commit();
          return true;
        }
      }
      return false;
    }

    public virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
    }

    public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
    }

    public class Comparer : System.Collections.Generic.Comparer<ResourceEntryBase>
    {
      public override int Compare(ResourceEntryBase x, ResourceEntryBase y)
      {
        bool flag1 = y == null || y.DocumentNode == null || y.DocumentNode.Context == null || y.DocumentNode.Context.DocumentUrl == null;
        bool flag2 = x == null || x.DocumentNode == null || x.DocumentNode.Context == null || x.DocumentNode.Context.DocumentUrl == null;
        if (flag1)
          return !flag2 ? true : false;
        if (flag2)
          return -1;
        if (x.DocumentNode.Context != y.DocumentNode.Context)
          return x.DocumentNode.Context.DocumentUrl.CompareTo(y.DocumentNode.Context.DocumentUrl);
        return x.Marker.CompareTo((object) y.Marker);
      }
    }
  }
}

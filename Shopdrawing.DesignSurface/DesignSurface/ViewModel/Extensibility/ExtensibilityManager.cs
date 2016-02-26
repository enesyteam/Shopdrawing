// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.ExtensibilityManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class ExtensibilityManager : IDisposable
  {
    private HashSet<Type> processedTypes = new HashSet<Type>();
    private List<SceneEditTransaction> activeTransactions = new List<SceneEditTransaction>();
    private FeatureManager featureManager;
    private SceneViewModel viewModel;
    private EditingContext editingContext;
    private SceneNodeModelService modelService;
    private SceneNodeViewService viewService;
    private DesignModeValueProviderService valueProviderService;
    private bool initialized;
    private bool settingSelection;

    public EditingContext EditingContext
    {
      get
      {
        return this.editingContext;
      }
    }

    public FeatureManager FeatureManager
    {
      get
      {
        return this.featureManager;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public ExtensibilityManager(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.viewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.OnViewModelEarlySceneUpdatePhase);
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.OnViewModelLateSceneUpdatePhase);
      this.viewModel.ProjectContext.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.OnProjectContextTypesChanged);
      this.editingContext = new EditingContext();
      this.featureManager = new FeatureManager(this.editingContext);
      this.featureManager.MetadataProvider = (MetadataProviderCallback) ((type, attributeType) =>
      {
        List<object> list = new List<object>();
        foreach (Attribute attribute in TypeUtilities.GetAttributes((MemberInfo) type, attributeType, true))
          list.Add((object) attribute);
        return (IEnumerable<object>) list;
      });
      this.modelService = new SceneNodeModelService(viewModel);
      this.featureManager.Context.Services.Publish(typeof (ModelService), (object) this.modelService);
      this.viewService = new SceneNodeViewService(viewModel);
      this.featureManager.Context.Services.Publish(typeof (ViewService), (object) this.viewService);
      SelectionOperations.Subscribe(this.editingContext, new SubscribeContextCallback<Microsoft.Windows.Design.Interaction.Selection>(this.OnUserSelectionChanged));
      this.UpdateSelection();
      this.featureManager.Context.Items.SetValue((ContextItem) new SelectionTool());
      this.valueProviderService = new DesignModeValueProviderService(this.viewModel.ProjectContext.Platform, this.featureManager);
      if (this.viewModel.RootNode != null)
        this.ProcessSubtree(this.viewModel.RootNode.DocumentNode);
      this.viewModel.Closing += new EventHandler(this.OnViewModelClosing);
    }

    public void UpdateForAddedTypes(DocumentNodeChangeList damage)
    {
      foreach (DocumentNodeChange documentNodeChange in damage.DistinctChanges)
      {
        if (documentNodeChange.NewChildNode != null)
          this.ProcessSubtree(documentNodeChange.NewChildNode);
      }
    }

    public void UpdateSelection()
    {
      this.SetSelection(this.CreateSelection());
    }

    private void SetSelection(Microsoft.Windows.Design.Interaction.Selection selection)
    {
      if (this.settingSelection)
        return;
      try
      {
        this.settingSelection = true;
        this.featureManager.Context.Items.SetValue((ContextItem) selection);
      }
      finally
      {
        this.settingSelection = false;
      }
    }

    private Microsoft.Windows.Design.Interaction.Selection CreateSelection()
    {
      if (this.settingSelection)
          return new Microsoft.Windows.Design.Interaction.Selection();
      List<ModelItem> list = new List<ModelItem>();
      SceneNode sceneNode1 = (SceneNode) this.viewModel.ElementSelectionSet.PrimarySelection;
      if (sceneNode1 != null)
        list.Add((ModelItem) sceneNode1.ModelItem);
      foreach (SceneNode sceneNode2 in this.viewModel.ElementSelectionSet.Selection)
      {
        if (sceneNode2 != sceneNode1)
          list.Add((ModelItem) sceneNode2.ModelItem);
      }
      return new Microsoft.Windows.Design.Interaction.Selection((IEnumerable<ModelItem>)list);
    }

    public ModelEditingScope CreateEditingScope(string description)
    {
      SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(description);
      SceneNodeModelEditingScope modelEditingScope = new SceneNodeModelEditingScope(this, editTransaction);
      this.activeTransactions.Add(editTransaction);
      return (ModelEditingScope) modelEditingScope;
    }

    public void CompleteEditingScope(SceneEditTransaction transaction)
    {
      int num = this.activeTransactions.IndexOf(transaction);
      if (num < 0)
        return;
      for (int index = this.activeTransactions.Count - 1; index >= num; --index)
      {
        this.activeTransactions[index].Commit();
        this.activeTransactions.RemoveAt(index);
      }
    }

    public void CancelEditingScope(SceneEditTransaction transaction)
    {
      int num = this.activeTransactions.IndexOf(transaction);
      if (num < 0)
        return;
      for (int index = this.activeTransactions.Count - 1; index >= num; --index)
      {
        this.activeTransactions[index].Cancel();
        this.activeTransactions.RemoveAt(index);
      }
    }

    public bool IsEditingScopeActive(SceneEditTransaction transaction)
    {
      if (this.activeTransactions.Count > 0)
        return this.activeTransactions[this.activeTransactions.Count - 1] == transaction;
      return false;
    }

    public Type ResolveType(TypeIdentifier typeIdentifier)
    {
      if (typeIdentifier.XmlNamespace != null)
      {
        IType type = this.viewModel.ProjectContext.GetType((IXmlNamespace) XmlNamespace.ToNamespace(typeIdentifier.XmlNamespace, XmlNamespaceCanonicalization.None), typeIdentifier.Name);
        if (type != null)
          return type.RuntimeType;
      }
      foreach (IAssemblyId assemblyId in (IEnumerable<IAssembly>) this.viewModel.ProjectContext.AssemblyReferences)
      {
        IType type = this.viewModel.ProjectContext.GetType(assemblyId.Name, typeIdentifier.Name);
        if (type != null)
          return type.RuntimeType;
      }
      return (Type) null;
    }

    public void Canonicalize()
    {
        Microsoft.Windows.Design.Interaction.Selection selection = this.featureManager.Context.Items.GetValue<Microsoft.Windows.Design.Interaction.Selection>();
      if (selection == null || !Enumerable.Any<ISceneNodeModelItem>(Enumerable.OfType<ISceneNodeModelItem>((IEnumerable) selection.SelectedObjects), (Func<ISceneNodeModelItem, bool>) (item => item.SceneNode.DocumentNode.Marker == null)))
        return;
      this.UpdateSelection();
    }

    private void OnViewModelEarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!this.initialized)
      {
        this.viewService.Initialize();
        this.initialized = true;
      }
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        this.UpdateSelection();
      this.viewService.OnLayoutUpdated();
    }

    private void OnViewModelLateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.InvalidateAdornersOnDocumentChange(args);
    }

    private void InvalidateAdornersOnDocumentChange(SceneUpdatePhaseEventArgs args)
    {
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.CurrentValues))
      {
        this.ForceAdornerUpdate();
      }
      else
      {
        using (IEnumerator<DocumentNodeChange> enumerator = args.DocumentChanges.DistinctChanges.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            DocumentNodeChange change = enumerator.Current;
            this.modelService.OnModelChanged(change);
            if (SceneView.HandleAnimationChanges(this.viewModel, change, (SceneView.HandleAnimationChange) ((sceneElement, propertyReference) => this.InvalidateModelItemAdorner((SceneNode) sceneElement, (IProperty) propertyReference.LastStep, change))) == SceneView.AnimationChangeResult.InvalidateAll)
            {
              this.ForceAdornerUpdate();
              break;
            }
            if (change.ParentNode != null)
              this.InvalidateModelItemAdorner(this.viewModel.GetSceneNode((DocumentNode) change.ParentNode), change.PropertyKey, change);
          }
        }
      }
    }

    private void InvalidateModelItemAdorner(SceneNode node, IProperty propertyKey, DocumentNodeChange change)
    {
      if (!node.IsModelItemCreated)
        return;
      if (change.IsChildChange)
      {
        ModelProperty source = node.ModelItem.Source;
        if (!(source != (ModelProperty) null))
          return;
        SceneNodeModelItemCollection modelItemCollection = (SceneNodeModelItemCollection) source.Collection;
        modelItemCollection.OnCollectionChanged();
        modelItemCollection.OnPropertyChanged("Item[]");
      }
      else
        node.ModelItem.OnPropertyChanged(propertyKey.Name);
    }

    private void ForceAdornerUpdate()
    {
        this.SetSelection(new Microsoft.Windows.Design.Interaction.Selection());
      this.SetSelection(this.CreateSelection());
    }

    private void OnUserSelectionChanged(Microsoft.Windows.Design.Interaction.Selection selection)
    {
      if (this.settingSelection)
        return;
      try
      {
        this.settingSelection = true;
        using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoUnitSetSelection))
        {
          List<SceneElement> list = new List<SceneElement>();
          SceneElement primarySelection = (SceneElement) null;
          foreach (ModelItem modelItem in selection.SelectedObjects)
          {
            ISceneNodeModelItem sceneNodeModelItem = modelItem as ISceneNodeModelItem;
            if (sceneNodeModelItem != null)
            {
              SceneElement sceneElement = sceneNodeModelItem.SceneNode as SceneElement;
              if (sceneElement != null)
              {
                list.Add(sceneElement);
                if (modelItem == selection.PrimarySelection)
                  primarySelection = sceneElement;
              }
            }
          }
          if (list.Count == 0)
            this.viewModel.ElementSelectionSet.Clear();
          else
            this.viewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, primarySelection);
          editTransaction.Commit();
        }
      }
      finally
      {
        this.settingSelection = false;
      }
    }

    private void OnProjectContextTypesChanged(object sender, TypesChangedEventArgs e)
    {
      if (this.viewModel.RootNode == null)
        return;
      this.ProcessSubtree(this.viewModel.RootNode.DocumentNode);
    }

    private void ProcessSubtree(DocumentNode node)
    {
      this.ProcessType(node.Type);
      foreach (DocumentNode documentNode in node.DescendantNodes)
        this.ProcessType(documentNode.Type);
    }

    private void ProcessType(IType type)
    {
      Type runtimeType = type.RuntimeType;
      if (!(runtimeType != (Type) null))
        return;
      if (this.processedTypes.Contains(runtimeType))
        return;
      try
      {
        this.featureManager.InitializeFeatures(runtimeType);
        this.valueProviderService.ProcessType(type);
      }
      catch (Exception ex)
      {
      }
      this.processedTypes.Add(runtimeType);
    }

    private void OnViewModelClosing(object sender, EventArgs e)
    {
        this.featureManager.Context.Items.SetValue((ContextItem)new Microsoft.Windows.Design.Interaction.Selection());
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool isDispoing)
    {
      if (!isDispoing)
        return;
      SelectionOperations.Unsubscribe(this.editingContext, new SubscribeContextCallback<Microsoft.Windows.Design.Interaction.Selection>(this.OnUserSelectionChanged));
      this.viewModel.Closing -= new EventHandler(this.OnViewModelClosing);
      this.viewModel.ProjectContext.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.OnProjectContextTypesChanged);
      this.viewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.OnViewModelEarlySceneUpdatePhase);
      this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.OnViewModelLateSceneUpdatePhase);
      this.viewModel = (SceneViewModel) null;
      this.valueProviderService.Dispose();
      this.featureManager.Dispose();
      this.editingContext.Dispose();
    }
  }
}

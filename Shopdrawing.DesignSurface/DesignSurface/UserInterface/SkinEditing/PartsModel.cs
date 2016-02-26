// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.PartsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class PartsModel : NotifyingObject
  {
    private Dictionary<string, PartsModel.PartsGroup> parts = new Dictionary<string, PartsModel.PartsGroup>();
    private static Dictionary<ITypeId, string> controlEditingTipsGuids = new Dictionary<ITypeId, string>()
    {
      {
        PlatformTypes.RadioButton,
        "6aacdaac-7759-40a0-a32b-1331dfe18889"
      },
      {
        PlatformTypes.TextBox,
        "f4a00879-a63b-4cab-8741-392a5c26a5ec"
      },
      {
        PlatformTypes.Button,
        "01b45249-aa98-41d8-b98d-6549fe97a4de"
      },
      {
        PlatformTypes.ListBox,
        "e2aab40c-136a-417c-b087-7b55c6eef484"
      },
      {
        PlatformTypes.ScrollBar,
        "1e2da171-5270-4386-b0c8-7e5b938636a2"
      },
      {
        PlatformTypes.CheckBox,
        "d592bebe-3b0c-4a7f-8c47-7e801dc94030"
      },
      {
        PlatformTypes.Slider,
        "e1a6c829-6b6c-4594-85a8-9bc0e85e5a60"
      },
      {
        PlatformTypes.ScrollViewer,
        "b4e143d3-eb2c-493b-9863-b67e1b5230e2"
      },
      {
        PlatformTypes.PasswordBox,
        "352a9ef3-4fc6-4cea-b067-bbe138961b97"
      },
      {
        PlatformTypes.ComboBox,
        "6e4e9012-6fc8-474b-a37d-bf4846514bf8"
      }
    };
    private static List<KeyValuePair<ITypeId, ITypeId>> instanceTypesForAbstractParts = new List<KeyValuePair<ITypeId, ITypeId>>()
    {
      new KeyValuePair<ITypeId, ITypeId>(PlatformTypes.FrameworkElement, PlatformTypes.Grid),
      new KeyValuePair<ITypeId, ITypeId>(PlatformTypes.Selector, PlatformTypes.ListBox)
    };
    private DesignerContext designerContext;
    private SceneViewModel viewModel;
    private SceneNode lastEditingContainer;
    private SceneNodeSubscription<object, object> elementSubscription;
    private bool changedObjectTreeElements;

    public static Dictionary<ITypeId, string> ControlEditingTipsGuids
    {
      get
      {
        return PartsModel.controlEditingTipsGuids;
      }
    }

    public static ICollection<KeyValuePair<ITypeId, ITypeId>> InstanceTypesForAbstractParts
    {
      get
      {
        return (ICollection<KeyValuePair<ITypeId, ITypeId>>) PartsModel.instanceTypesForAbstractParts;
      }
    }

    public bool IsEnabled
    {
      get
      {
        if (this.viewModel.ActiveEditingContainer != null && this.viewModel.ActiveEditingContainer.DocumentNode.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsTemplateParts))
          return this.viewModel.ActiveEditingContainer is ControlTemplateElement;
        return false;
      }
    }

    public DrawingBrush TargetTypeBrush
    {
      get
      {
        if (this.IsEnabled)
          return IconMapper.GetDrawingBrushForType((ITypeId) this.viewModel.ProjectContext.GetType(this.TargetTemplate.TargetElementType), true, 12, 12);
        return (DrawingBrush) null;
      }
    }

    public ObservableCollection<PartInPartsExplorer> PartsList { get; private set; }

    public bool ShowPartsList
    {
      get
      {
        if (this.IsEditable && this.viewModel.DefaultView == this.designerContext.ActiveView && (this.viewModel.DefaultView.IsValid && this.viewModel.DefaultView.IsDesignSurfaceVisible))
          return this.PartsList.Count > 0;
        return false;
      }
    }

    public bool ShowHasNoPartsMessage
    {
      get
      {
        if (this.IsEditable && this.viewModel.DefaultView == this.designerContext.ActiveView && (this.designerContext.ActiveView.IsValid && this.designerContext.ActiveView.IsDesignSurfaceVisible))
          return this.PartsList.Count == 0;
        return false;
      }
    }

    public bool HasControlSkinningTips
    {
      get
      {
        if (this.IsEnabled && this.viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.ProvidesControlStylingTips))
        {
          ControlTemplateElement controlTemplateElement = this.viewModel.ActiveEditingContainer as ControlTemplateElement;
          if (controlTemplateElement != null)
            return Enumerable.FirstOrDefault<ITypeId>((IEnumerable<ITypeId>) PartsModel.ControlEditingTipsGuids.Keys, (Func<ITypeId, bool>) (type => type.Equals((object) controlTemplateElement.ControlTemplateTargetTypeId))) != null;
        }
        return false;
      }
    }

    public string HasNoPartsMessage
    {
      get
      {
        if (!this.IsEnabled)
          return string.Empty;
        if (!this.HasControlSkinningTips)
          return StringTable.PartsListEnabledEmptyMessage;
        return StringTable.PartsListEnabledEmptyMessageWithTips;
      }
    }

    public SceneNode EditingContainer
    {
      get
      {
        return this.lastEditingContainer;
      }
    }

    public bool IsEditable
    {
      get
      {
        if (this.IsEnabled)
          return this.viewModel.IsEditable;
        return false;
      }
    }

    public string TargetTypeName
    {
      get
      {
        if (this.IsEnabled)
          return (this.viewModel.ActiveEditingContainer as ControlTemplateElement).TargetElementType.Name;
        return "";
      }
    }

    public string ControlStylingTipsMessage
    {
      get
      {
        return StringTable.ControlStylingTipsMessage;
      }
    }

    public bool ShowLowerControlSkinningTipsMessage
    {
      get
      {
        if (this.HasControlSkinningTips)
          return this.ShowPartsList;
        return false;
      }
    }

    private ControlTemplateElement TargetTemplate
    {
      get
      {
        if (this.IsEnabled)
          return this.viewModel.ActiveEditingContainer as ControlTemplateElement;
        return (ControlTemplateElement) null;
      }
    }

    internal PartsModel(DesignerContext designerContext, SceneViewModel viewModel)
    {
      this.designerContext = designerContext;
      this.viewModel = viewModel;
      this.PartsList = new ObservableCollection<PartInPartsExplorer>();
      this.Initialize();
    }

    internal void Unload()
    {
      this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.elementSubscription.SetPathNodeInsertedHandler((SceneNodeSubscription<object, object>.PathNodeInsertedHandler) null);
      this.elementSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.ElementSubscription_ElementRemoved);
      this.elementSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.ElementSubscription_ElementContentChanged);
      this.elementSubscription.CurrentViewModel = (SceneViewModel) null;
      this.elementSubscription = (SceneNodeSubscription<object, object>) null;
    }

    public void OnDoubleClickItem(PartInPartsExplorer part)
    {
      SceneElement selectionToSet = (SceneElement) null;
      if (part.Status != PartStatus.Unused)
      {
        selectionToSet = this.viewModel.GetSceneNode(this.viewModel.ActiveEditingContainer.DocumentNode.NameScope.FindNode(part.Name)) as SceneElement;
      }
      else
      {
        using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.UndoDoubleClickToMakePart, false))
        {
          KeyValuePair<ITypeId, ITypeId> keyValuePair = Enumerable.FirstOrDefault<KeyValuePair<ITypeId, ITypeId>>((IEnumerable<KeyValuePair<ITypeId, ITypeId>>) PartsModel.instanceTypesForAbstractParts, (Func<KeyValuePair<ITypeId, ITypeId>, bool>) (pair => pair.Key.Equals((object) part.TargetType)));
          BaseFrameworkElement frameworkElement = keyValuePair.Key == null ? (BaseFrameworkElement) this.viewModel.CreateSceneNode(part.TargetType) : (BaseFrameworkElement) this.viewModel.CreateSceneNode(keyValuePair.Value);
          frameworkElement.Name = part.Name;
          this.viewModel.ActiveSceneInsertionPoint.Insert((SceneNode) frameworkElement);
          selectionToSet = (SceneElement) frameworkElement;
          editTransaction.Update();
          if (frameworkElement.IsViewObjectValid)
          {
            editTransaction.Commit();
          }
          else
          {
            editTransaction.Cancel();
            selectionToSet = (SceneElement) null;
          }
        }
      }
      if (selectionToSet == null || !PlatformTypes.UIElement.IsAssignableFrom((ITypeId) selectionToSet.Type))
        return;
      this.viewModel.ClearSelections();
      this.viewModel.ElementSelectionSet.SetSelection(selectionToSet);
      this.viewModel.TimelineItemManager.EnsureExpandedAncestors(this.viewModel.TimelineItemManager.FindTimelineItem((SceneNode) selectionToSet));
    }

    public PartStatus GetPartStatus(SceneNode element)
    {
      if (element == null || string.IsNullOrEmpty(element.Name) || !this.parts.ContainsKey(element.Name))
        return PartStatus.Unused;
      return !this.parts[element.Name][0].TargetType.IsAssignableFrom((ITypeId) element.Type) ? PartStatus.WrongType : PartStatus.Assigned;
    }

    private void Initialize()
    {
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.elementSubscription = new SceneNodeSubscription<object, object>();
      this.elementSubscription.Path = new SearchPath(new SearchStep[2]
      {
        new SearchStep(SearchAxis.DocumentSelfAndDescendant, (ISearchPredicate) new SceneNodeTypePredicate(typeof (SceneNode))),
        new SearchStep(SearchAxis.DocumentChild, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (node => !string.IsNullOrEmpty(node.Name)), SearchScope.NodeTreeSelf))
      });
      this.elementSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, object>.PathNodeInsertedHandler(this.ElementSubscription_ElementInserted));
      this.elementSubscription.PathNodeRemoved += new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.ElementSubscription_ElementRemoved);
      this.elementSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.ElementSubscription_ElementContentChanged);
    }

    private void UpdateBasisNode()
    {
      List<SceneNode> list = new List<SceneNode>(0);
      this.elementSubscription.SetBasisNodes(this.viewModel, (IEnumerable<SceneNode>) list);
      if (this.IsEnabled)
      {
        ControlTemplateElement controlTemplateElement = this.viewModel.ActiveEditingContainer as ControlTemplateElement;
        if (controlTemplateElement != null)
          list.Insert(0, (SceneNode) controlTemplateElement);
      }
      this.elementSubscription.SetBasisNodes(this.viewModel, (IEnumerable<SceneNode>) list);
    }

    private void AddPartStatus(PartsModel.PartsGroup parts, SceneNode node)
    {
      if (!parts.CanSetStatus())
        return;
      foreach (PartInPartsExplorer partInPartsExplorer in (List<PartInPartsExplorer>) parts)
        partInPartsExplorer.Status = partInPartsExplorer.TargetType.IsAssignableFrom((ITypeId) node.Type) ? PartStatus.Assigned : PartStatus.WrongType;
      this.UpdateElementTimelineItem(node as SceneElement, parts[0]);
    }

    private void RemovePartStatus(PartsModel.PartsGroup parts, SceneNode node)
    {
      if (!parts.CanClearStatus())
        return;
      foreach (PartInPartsExplorer partInPartsExplorer in (List<PartInPartsExplorer>) parts)
        partInPartsExplorer.Status = PartStatus.Unused;
      this.UpdateElementTimelineItem(node as SceneElement, parts[0]);
    }

    private void UpdateElementTimelineItem(SceneElement element, PartInPartsExplorer part)
    {
      if (element == null || element.ViewModel == null || (element.ViewModel.DesignerContext == null || element.ViewModel.TimelineItemManager == null))
        return;
      ElementTimelineItem elementTimelineItem = element.ViewModel.TimelineItemManager.FindTimelineItem((SceneNode) element) as ElementTimelineItem;
      if (elementTimelineItem == null)
        return;
      this.changedObjectTreeElements = true;
      elementTimelineItem.Invalidate();
      elementTimelineItem.PartStatus = part.Status;
      if (part.Status != PartStatus.WrongType)
        return;
      elementTimelineItem.WronglyAssignedPartCorrectType = part.TargetType.Name;
    }

    private void ElementSubscription_ElementContentChanged(object sender, SceneNode pathNode, object savedObject, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (!damage.IsPropertyChange || !pathNode.DocumentNode.IsNameProperty((IPropertyId) damage.PropertyKey) || pathNode.DocumentNode != damage.ParentNode)
        return;
      if (damage.OldChildNodeValue != null)
      {
        string key = ((DocumentNodeStringValue) damage.OldChildNodeValue).Value;
        if (this.parts.ContainsKey(key))
          this.RemovePartStatus(this.parts[key], pathNode);
      }
      if (damage.NewChildNodeValue == null)
        return;
      string key1 = ((DocumentNodeStringValue) damage.NewChildNodeValue).Value;
      if (!this.parts.ContainsKey(key1))
        return;
      this.AddPartStatus(this.parts[key1], pathNode);
    }

    private object ElementSubscription_ElementInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      if (this.parts.ContainsKey(newPathNode.Name))
        this.AddPartStatus(this.parts[newPathNode.Name], newPathNode);
      return (object) null;
    }

    private void ElementSubscription_ElementRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, object savedObject)
    {
      if (oldPathNode.Name == null || !this.parts.ContainsKey(oldPathNode.Name))
        return;
      this.RemovePartStatus(this.parts[oldPathNode.Name], oldPathNode);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (!this.IsEnabled)
      {
        this.UpdatePartsFromMetadata();
        this.UpdateBasisNode();
        this.UpdateUIStatus();
      }
      else
      {
        this.changedObjectTreeElements = false;
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer))
        {
          this.UpdatePartsFromMetadata();
          this.UpdateBasisNode();
        }
        else
        {
          if (args.DocumentChanges.Count <= 0)
            return;
          this.elementSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
        }
        foreach (PartsModel.PartsGroup partsGroup in this.parts.Values)
          partsGroup.ResetClearSuppression();
        if (this.changedObjectTreeElements)
          this.viewModel.TimelineItemManager.UpdateItems();
        this.UpdateUIStatus();
      }
    }

    public void UpdatePartsFromMetadata()
    {
      if (!this.IsEnabled)
      {
        this.parts.Clear();
        this.PartsList.Clear();
        this.lastEditingContainer = (SceneNode) null;
      }
      else
      {
        List<TemplatePartAttributeRecord> list = new List<TemplatePartAttributeRecord>((IEnumerable<TemplatePartAttributeRecord>) ProjectAttributeHelper.GetTemplatePartAttributes(this.TargetTemplate.ProjectContext.ResolveType(this.TargetTemplate.ControlTemplateTargetTypeId), (ITypeResolver) ProjectContext.GetProjectContext(this.viewModel.ProjectContext)));
        list.Sort((Comparison<TemplatePartAttributeRecord>) ((first, second) => string.Compare(first.NameAttribute, second.NameAttribute, StringComparison.CurrentCulture)));
        this.parts.Clear();
        this.PartsList.Clear();
        foreach (TemplatePartAttributeRecord partAttributeRecord in list)
        {
          if (!string.IsNullOrEmpty(partAttributeRecord.NameAttribute) && !(partAttributeRecord.TypeAttribute == (Type) null))
          {
            ITypeId type = (ITypeId) this.TargetTemplate.ProjectContext.GetType(partAttributeRecord.TypeAttribute);
            if (PlatformTypes.FrameworkElement.IsAssignableFrom(type))
            {
              PartInPartsExplorer partInPartsExplorer = new PartInPartsExplorer(partAttributeRecord.NameAttribute, type);
              if (!this.parts.ContainsKey(partAttributeRecord.NameAttribute))
              {
                PartsModel.PartsGroup partsGroup = new PartsModel.PartsGroup();
                partsGroup.Add(partInPartsExplorer);
                this.parts.Add(partAttributeRecord.NameAttribute, partsGroup);
              }
              else
                this.parts[partAttributeRecord.NameAttribute].Add(partInPartsExplorer);
              this.PartsList.Add(partInPartsExplorer);
            }
          }
        }
        this.lastEditingContainer = this.viewModel.ActiveEditingContainer;
      }
    }

    private void UpdateUIStatus()
    {
      this.OnPropertyChanged("ShowPartsList");
      this.OnPropertyChanged("ShowHasNoPartsMessage");
      this.OnPropertyChanged("HasNoPartsMessage");
      this.OnPropertyChanged("IsEditable");
      this.OnPropertyChanged("IsEnabled");
      this.OnPropertyChanged("TargetTypeName");
      this.OnPropertyChanged("TargetTypeBrush");
      this.OnPropertyChanged("HasControlSkinningTips");
      this.OnPropertyChanged("ShowLowerControlSkinningTipsMessage");
    }

    private class PartsGroup : List<PartInPartsExplorer>
    {
      private bool suppressClearing;

      public PartsGroup()
        : base(1)
      {
      }

      public bool CanSetStatus()
      {
        this.suppressClearing = true;
        return true;
      }

      public bool CanClearStatus()
      {
        return !this.suppressClearing;
      }

      public void ResetClearSuppression()
      {
        this.suppressClearing = false;
      }
    }
  }
}

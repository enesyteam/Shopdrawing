// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.PasteCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Clipboard;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class PasteCommand : SceneCommandBase
  {
    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.None;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        if (this.SceneViewModel.TextSelectionSet.IsActive)
          return this.CanPasteText;
        if (this.CanPasteElements)
          return this.SceneViewModel.ActiveSceneInsertionPoint != null;
        return false;
      }
    }

    private bool CanPasteElements
    {
      get
      {
        return PasteCommand.CanPasteData(SafeDataObject.FromClipboard());
      }
    }

    private bool CanPasteText
    {
      get
      {
        bool flag = false;
        SafeDataObject safeDataObject = SafeDataObject.FromClipboard();
        if (safeDataObject != null)
          flag = safeDataObject.GetDataPresent(DataFormats.Text);
        return flag;
      }
    }

    public PasteCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitPaste))
      {
        this.SceneViewModel.GetActiveSceneInsertionPoint(new InsertionPointContext(false));
        if (this.SceneViewModel.TextSelectionSet.IsActive)
        {
          try
          {
            this.SceneViewModel.TextSelectionSet.TextEditProxy.EditingElement.Paste();
          }
          catch (ArgumentException ex)
          {
            this.SceneViewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteTextFailedDialogMessage);
          }
        }
        else
        {
          ICollection<SceneNode> nodes = PasteCommand.PasteData(this.SceneViewModel, SafeDataObject.FromClipboard());
          if (nodes.Count > 0)
          {
            this.SceneViewModel.ClearSelections();
            this.SceneViewModel.SelectNodes(nodes);
          }
        }
        editTransaction.Commit();
      }
    }

    public static ICollection<SceneNode> PasteData(SceneViewModel viewModel, SafeDataObject dataObject)
    {
      bool canceledPasteOperation;
      return PasteCommand.PasteData(viewModel, dataObject, viewModel.ActiveSceneInsertionPoint, viewModel.LockedInsertionPoint == null, out canceledPasteOperation);
    }

    public static ICollection<SceneNode> PasteData(SceneViewModel viewModel, SafeDataObject dataObject, ISceneInsertionPoint insertionPoint, out bool canceledPasteOperation)
    {
      return PasteCommand.PasteData(viewModel, dataObject, insertionPoint, false, out canceledPasteOperation);
    }

    private static ICollection<SceneNode> PasteData(SceneViewModel viewModel, SafeDataObject dataObject, ISceneInsertionPoint insertionPoint, bool allowInsertionPointChange, out bool canceledPasteOperation)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.PasteElements);
      canceledPasteOperation = false;
      List<SceneNode> list = new List<SceneNode>();
      PastePackage pastePackage = PastePackage.FromData(viewModel, dataObject);
      if (pastePackage != null)
      {
        if (allowInsertionPointChange)
          insertionPoint = PasteCommand.ComputeNewInsertionPoint(viewModel, insertionPoint, pastePackage);
        if (!PasteCommand.CanAddMultipleElements(insertionPoint.SceneElement, pastePackage.Elements.Count))
        {
          string name = insertionPoint.SceneElement.TargetType.Name;
          viewModel.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PasteMultipleInSingleContainerError, new object[1]
          {
            (object) name
          }));
        }
        else if (pastePackage.Elements.Count == 0 && pastePackage.Storyboards.Count == 0 && pastePackage.Resources.Count > 0)
        {
          viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteElementsFailedNoElementsDialogMessage);
        }
        else
        {
          IDictionary<DocumentNode, string> imageMap = (IDictionary<DocumentNode, string>) new Dictionary<DocumentNode, string>();
          foreach (SceneNode sceneNode in pastePackage.Elements)
          {
            foreach (KeyValuePair<DocumentNode, string> keyValuePair in (IEnumerable<KeyValuePair<DocumentNode, string>>) Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CreateImageReferenceMap(sceneNode.DocumentNode, pastePackage, viewModel))
              imageMap.Add(keyValuePair);
          }
          foreach (SceneNode sceneNode in pastePackage.Resources)
          {
            foreach (KeyValuePair<DocumentNode, string> keyValuePair in (IEnumerable<KeyValuePair<DocumentNode, string>>) Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CreateImageReferenceMap(sceneNode.DocumentNode, pastePackage, viewModel))
              imageMap.Add(keyValuePair);
          }
          int index = 0;
          ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(viewModel.RootNode.DocumentNode);
          if (resourcesCollection != null && resourcesCollection.Resources != null && resourcesCollection.Resources.SupportsChildren)
            index = resourcesCollection.Resources.Children.Count;
          if (Microsoft.Expression.DesignSurface.Utility.ResourceHelper.PasteResources(pastePackage, imageMap, ResourceConflictResolution.UseExisting | ResourceConflictResolution.RenameNew | ResourceConflictResolution.OverwriteOld, viewModel.RootNode, index, false))
          {
            ILayoutDesigner designerForParent = viewModel.GetLayoutDesignerForParent(insertionPoint.SceneElement, true);
            List<PasteCommand.DelayedElementTranslationInfo> elementsToTranslateLater = new List<PasteCommand.DelayedElementTranslationInfo>(pastePackage.Elements.Count);
            string copyElementToken = pastePackage.ClipboardCopyElementToken;
            foreach (SceneElement element in pastePackage.Elements)
            {
              SceneElement sceneElement = PasteCommand.PasteElement(viewModel, element, elementsToTranslateLater, insertionPoint);
              if (sceneElement != null)
              {
                Microsoft.Expression.DesignSurface.Utility.ResourceHelper.UpdateImageReferences(sceneElement.DocumentNode, imageMap, pastePackage, viewModel);
                list.Add((SceneNode) sceneElement);
                sceneElement.ClearValue(DesignTimeProperties.CopyTokenProperty);
              }
            }
            if (copyElementToken != null)
            {
              pastePackage.SetGlobalCopyElementToken(copyElementToken);
              PastePackage.PasteSelectionChangePending = true;
            }
            foreach (SceneNode childProperty in pastePackage.ChildPropertyNodes)
              PasteCommand.PasteChildProperty(viewModel, childProperty, (IList<SceneNode>) list);
            foreach (StoryboardTimelineSceneNode storyboard in pastePackage.Storyboards)
              PasteCommand.PasteStoryboard(viewModel, storyboard, (IList<SceneNode>) list);
            viewModel.Document.OnUpdatedEditTransaction();
            viewModel.DefaultView.UpdateLayout();
            using (viewModel.ForceBaseValue())
            {
              Rect empty = Rect.Empty;
              foreach (PasteCommand.DelayedElementTranslationInfo elementTranslationInfo in elementsToTranslateLater)
                empty.Union(elementTranslationInfo.Bounds);
              foreach (PasteCommand.DelayedElementTranslationInfo elementTranslationInfo in elementsToTranslateLater)
                elementTranslationInfo.UpdateTranslation(designerForParent, empty);
            }
          }
          else
            canceledPasteOperation = true;
        }
      }
      else if (dataObject.GetDataPresent(DataFormats.FileDrop))
      {
        DesignerContext designerContext = viewModel.DesignerContext;
        string[] supportedFiles = FileDropToolBehavior.CreateImageOrMediaDrop(designerContext).GetSupportedFiles(ClipboardService.GetDataObject());
        if (supportedFiles.Length > 0)
        {
          IEnumerable<IProjectItem> importedItems = designerContext.ActiveProject.AddItems(Enumerable.Select<string, DocumentCreationInfo>((IEnumerable<string>) supportedFiles, (Func<string, DocumentCreationInfo>) (file => new DocumentCreationInfo()
          {
            SourcePath = file
          })));
          FileDropToolBehavior.AddItemsToDocument(viewModel.DefaultView, importedItems, new Point(0.0, 0.0), viewModel.ActiveSceneInsertionPoint);
        }
        else
          viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteElementsFailedDialogMessage);
      }
      else if (dataObject.GetDataPresent(DataFormats.Bitmap))
      {
        DesignerContext designerContext = viewModel.DesignerContext;
        IProjectItem projectItem = CutBuffer.AddImageDataFromClipboard(designerContext.ProjectManager, designerContext.ActiveProject);
        if (projectItem != null)
          FileDropToolBehavior.AddItemsToDocument(viewModel.DefaultView, (IEnumerable<IProjectItem>) new List<IProjectItem>()
          {
            projectItem
          }, new Point(0.0, 0.0), viewModel.ActiveSceneInsertionPoint);
        else
          viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteElementsFailedDialogMessage);
      }
      else
        viewModel.DesignerContext.MessageDisplayService.ShowError(StringTable.PasteElementsFailedDialogMessage);
      return (ICollection<SceneNode>) list;
    }

    private static ISceneInsertionPoint ComputeNewInsertionPoint(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, PastePackage pastePackage)
    {
      if (pastePackage.Elements.Count == 0 || viewModel.ElementSelectionSet.Count == 0 || !viewModel.ElementSelectionSet.PrimarySelection.IsAncestorOf((SceneNode) insertionPoint.SceneElement))
        return insertionPoint;
      string copyElementToken = pastePackage.ClipboardCopyElementToken;
      if (!string.IsNullOrEmpty(copyElementToken) && copyElementToken.Equals(PastePackage.GlobalCopyElementToken))
      {
        SceneElement parentElement = viewModel.ElementSelectionSet.PrimarySelection.ParentElement;
        if (parentElement != null && parentElement.IsViewObjectValid && (viewModel.ActiveEditingContainer.IsAncestorOf((SceneNode) parentElement) && !ElementUtilities.HasVisualTreeAncestorOfType(parentElement.ViewObject.PlatformSpecificObject as DependencyObject, typeof (Viewport2DVisual3D))))
          insertionPoint = parentElement.DefaultInsertionPoint;
      }
      return insertionPoint;
    }

    private static bool CanAddMultipleElements(SceneElement pasteRoot, int elementCount)
    {
      if (pasteRoot is StyleNode && elementCount > 1)
        return false;
      if (elementCount > 1)
      {
        IPropertyId childProperty = (IPropertyId) pasteRoot.DefaultContentProperty;
        ISceneNodeCollection<SceneNode> collectionForProperty = pasteRoot.GetCollectionForProperty(childProperty);
        if (collectionForProperty.FixedCapacity.HasValue && elementCount > collectionForProperty.FixedCapacity.Value - collectionForProperty.Count)
          return false;
      }
      return true;
    }

    private static void PasteChildProperty(SceneViewModel viewModel, SceneNode childProperty, IList<SceneNode> pastedNodes)
    {
      IList<SceneElement> list = (IList<SceneElement>) null;
      DocumentNodeHelper.StripExtraNamespaces(childProperty.DocumentNode);
      if (viewModel.ElementSelectionSet.Selection.Count != 0)
        list = (IList<SceneElement>) viewModel.ElementSelectionSet.Selection;
      else if (viewModel.ChildPropertySelectionSet.Selection.Count != 0 && !(childProperty is BehaviorBaseNode) && !(childProperty is BehaviorTriggerBaseNode))
      {
        list = (IList<SceneElement>) new List<SceneElement>();
        foreach (SceneNode sceneNode in viewModel.ChildPropertySelectionSet.Selection)
        {
          SceneElement sceneElement = sceneNode.Parent as SceneElement;
          list.Add(sceneElement);
        }
      }
      if (list == null)
        return;
      IProperty targetProperty = viewModel.ProjectContext.ResolveProperty(PasteCommand.ChildSceneNodeToPropertyId(childProperty));
      foreach (SceneElement sceneElement in (IEnumerable<SceneElement>) list)
      {
        PropertySceneInsertionPoint sceneInsertionPoint = new PropertySceneInsertionPoint(sceneElement, targetProperty);
        if (sceneInsertionPoint.CanInsert((ITypeId) childProperty.Type))
        {
          if (ProjectNeutralTypes.BehaviorTriggerBase.IsAssignableFrom((ITypeId) childProperty.Type))
          {
            BehaviorTriggerBaseNode behaviorTriggerBaseNode1 = (BehaviorTriggerBaseNode) childProperty;
            bool flag = true;
            foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) behaviorTriggerBaseNode1.Actions)
            {
              if (!sceneInsertionPoint.CanInsert((ITypeId) sceneNode.Type))
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              ISceneNodeCollection<SceneNode> collectionForProperty = sceneElement.GetCollectionForProperty((IPropertyId) targetProperty);
              BehaviorTriggerBaseNode behaviorTriggerBaseNode2 = BehaviorHelper.FindMatchingTriggerNode(behaviorTriggerBaseNode1.DocumentNode, collectionForProperty);
              if (behaviorTriggerBaseNode2 == null)
              {
                DocumentNode node = behaviorTriggerBaseNode1.DocumentNode.Clone(behaviorTriggerBaseNode1.DocumentContext);
                behaviorTriggerBaseNode2 = (BehaviorTriggerBaseNode) viewModel.GetSceneNode(node);
                collectionForProperty.Add((SceneNode) behaviorTriggerBaseNode2);
              }
              else
              {
                DocumentNode node = behaviorTriggerBaseNode1.Actions[0].DocumentNode.Clone(behaviorTriggerBaseNode1.Actions[0].DocumentNode.Context);
                BehaviorTriggerActionNode triggerActionNode = (BehaviorTriggerActionNode) viewModel.GetSceneNode(node);
                behaviorTriggerBaseNode2.Actions.Add((SceneNode) triggerActionNode);
              }
              BehaviorEventTriggerNode eventTriggerNode = behaviorTriggerBaseNode2 as BehaviorEventTriggerNode;
              if (eventTriggerNode != null)
                BehaviorEventTriggerNode.FixUpEventName(eventTriggerNode);
            }
            else
              continue;
          }
          else
          {
            DocumentNode node = childProperty.DocumentNode.Clone(childProperty.DocumentNode.Context);
            SceneNode sceneNode = viewModel.GetSceneNode(node);
            sceneInsertionPoint.Insert(sceneNode);
            if (sceneNode is EffectNode)
              pastedNodes.Add(sceneNode);
          }
          TimelineItem timelineItem = viewModel.TimelineItemManager.FindTimelineItem((SceneNode) sceneElement);
          if (timelineItem != null)
            timelineItem.IsExpanded = true;
        }
      }
    }

    internal static IPropertyId ChildSceneNodeToPropertyId(SceneNode sceneNode)
    {
      if (sceneNode is EffectNode)
        return Base2DElement.EffectProperty;
      if (sceneNode is BehaviorNode)
        return BehaviorHelper.BehaviorsProperty;
      if (sceneNode is BehaviorTriggerBaseNode)
        return BehaviorHelper.BehaviorTriggersProperty;
      return (IPropertyId) null;
    }

    private static SceneElement PasteElement(SceneViewModel viewModel, SceneElement element, List<PasteCommand.DelayedElementTranslationInfo> elementsToTranslateLater, ISceneInsertionPoint insertionPoint)
    {
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(viewModel, (SceneNode) insertionPoint.SceneNode.StoryboardContainer);
      SceneElement element1 = (SceneElement) null;
      using (viewModel.ForceBaseValue())
      {
        PasteCommand.StripStoryboardsAndTriggers(element);
        PasteCommand.StripExtraNamespaces(element);
        BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
        if (frameworkElement != null || element is DataGridColumnNode)
        {
          if (!(insertionPoint.SceneElement is Viewport3DElement) && !(insertionPoint.SceneElement is Base3DElement))
          {
            if (insertionPoint.CanInsert((ITypeId) element.Type))
            {
              if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) insertionPoint.SceneElement.Type) && (!PlatformTypes.UserControl.Equals((object) insertionPoint.SceneElement.Type) || insertionPoint.SceneElement.DocumentNode.DocumentRoot.RootNode != insertionPoint.SceneElement.DocumentNode))
              {
                if (element.Name != null)
                {
                  string validCopiedElementId = sceneNodeIdHelper.GetValidCopiedElementID((SceneNode) element, element.Name);
                  sceneNodeIdHelper.SetLocalName((SceneNode) element, validCopiedElementId);
                }
                sceneNodeIdHelper.FixNameConflicts((SceneNode) element);
              }
              if (viewModel.DesignerContext.PrototypingService != null)
                viewModel.DesignerContext.PrototypingService.ProcessElementBeforeInsertion(insertionPoint, element);
              insertionPoint.Insert((SceneNode) element);
              if (element.GetLocalValue(DesignTimeProperties.LayoutRectProperty) != null && frameworkElement != null)
                elementsToTranslateLater.Add(new PasteCommand.DelayedElementTranslationInfo(frameworkElement));
              element1 = element;
            }
          }
          else if (element.TargetType == typeof (Image))
          {
            GeometryModel3DElement geometryModel3Delement = GeometryCreationHelper3D.ConvertImageTo3D(viewModel, frameworkElement, element.Name);
            IChildContainer3D childContainer3D = insertionPoint.SceneElement as IChildContainer3D;
            if (childContainer3D != null)
            {
              childContainer3D.AddChild(viewModel, (Base3DElement) geometryModel3Delement);
              element1 = (SceneElement) geometryModel3Delement;
            }
          }
        }
        else
        {
          Base3DElement child;
          if ((child = element as Base3DElement) != null)
          {
            if (child.Name != null)
            {
              string validCopiedElementId = sceneNodeIdHelper.GetValidCopiedElementID((SceneNode) child, child.Name);
              sceneNodeIdHelper.SetLocalName((SceneNode) child, validCopiedElementId);
            }
            DependencyPropertyReferenceStep propertyReferenceStep = insertionPoint.Property as DependencyPropertyReferenceStep;
            if (propertyReferenceStep != null && propertyReferenceStep.DependencyProperty == ModelVisual3D.ContentProperty)
            {
              Model3DElement model3Delement = BaseElement3DCoercionHelper.CoerceToModel3D(viewModel, (SceneElement) child);
              if (model3Delement != null)
              {
                insertionPoint.Insert((SceneNode) model3Delement);
                element1 = (SceneElement) model3Delement;
              }
            }
            else
            {
              IChildContainer3D childContainer3D = insertionPoint.SceneElement as IChildContainer3D;
              if (childContainer3D != null)
                element1 = childContainer3D.AddChild(viewModel, child);
            }
          }
        }
        if (element1 != null)
        {
          if (!(viewModel.ActiveEditingContainer is FrameworkTemplateElement))
            PasteCommand.StripTemplateBindings(element1);
        }
      }
      return element1;
    }

    private static void StripStoryboardsAndTriggers(SceneElement element)
    {
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      if (frameworkElement != null && frameworkElement.Resources != null)
      {
        List<DictionaryEntryNode> list = new List<DictionaryEntryNode>();
        foreach (DictionaryEntryNode dictionaryEntryNode in frameworkElement.Resources)
        {
          if (dictionaryEntryNode.Value != null && PlatformTypes.Storyboard.IsAssignableFrom((ITypeId) dictionaryEntryNode.Value.Type))
            list.Add(dictionaryEntryNode);
        }
        foreach (DictionaryEntryNode dictionaryEntryNode in list)
          frameworkElement.Resources.Remove(dictionaryEntryNode);
      }
      ITriggerContainer triggerContainer = (ITriggerContainer) element.StoryboardContainer;
      if (triggerContainer == null || !triggerContainer.CanEditTriggers)
        return;
      triggerContainer.VisualTriggers.Clear();
    }

    public static void StripExtraNamespaces(SceneElement element)
    {
      DocumentNodeHelper.StripExtraNamespaces(element.DocumentNode);
    }

    private static void StripTemplateBindings(SceneElement element)
    {
      List<DocumentNode> list = new List<DocumentNode>();
      foreach (DocumentNode documentNode1 in PasteCommand.FindTemplateBindingTargets(element))
      {
        foreach (DocumentNode documentNode2 in documentNode1.SelectChildNodes(typeof (TemplateBindingExtension)))
          list.Add(documentNode2);
      }
      foreach (DocumentNode documentNode in list)
        documentNode.Parent.ClearValue((IPropertyId) documentNode.SitePropertyKey);
    }

    private static IEnumerable<DocumentCompositeNode> FindTemplateBindingTargets(SceneElement element)
    {
      DocumentCompositeNode elementNode = element.DocumentNode as DocumentCompositeNode;
      if (elementNode != null)
        yield return elementNode;
      IEnumerator<DocumentNode> descendants = element.DocumentNode.DescendantNodes.GetEnumerator();
      while (descendants.MoveNext())
      {
        DocumentNode descendant = descendants.Current;
        if (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) descendant.Type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) descendant.Type))
        {
          IDescendantEnumerator descendantEnumerator = descendants as IDescendantEnumerator;
          if (descendantEnumerator != null)
            descendantEnumerator.SkipPastDescendants(descendant);
        }
        else
        {
          DocumentCompositeNode compositeNode = descendant as DocumentCompositeNode;
          if (compositeNode != null)
            yield return compositeNode;
        }
      }
    }

    private static void PasteStoryboard(SceneViewModel viewModel, StoryboardTimelineSceneNode storyboard, IList<SceneNode> selectionSet)
    {
      double baseKeyframeOffset = double.MaxValue;
      foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) storyboard.Children)
      {
        KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode as KeyFrameAnimationSceneNode;
        if (animationSceneNode != null)
        {
          foreach (KeyFrameSceneNode keyFrameSceneNode in animationSceneNode.KeyFrames)
          {
            if (keyFrameSceneNode.Time < baseKeyframeOffset)
              baseKeyframeOffset = keyFrameSceneNode.Time;
          }
        }
      }
      if (baseKeyframeOffset == double.MaxValue)
        baseKeyframeOffset = 0.0;
      IList<SceneElement> list = (IList<SceneElement>) viewModel.ElementSelectionSet.Selection;
      if (list.Count == 0)
        list = (IList<SceneElement>) viewModel.KeyFrameSelectionSet.DerivedTargetElements;
      foreach (SceneElement element in (IEnumerable<SceneElement>) list)
      {
        foreach (TimelineSceneNode animation in (IEnumerable<TimelineSceneNode>) storyboard.Children)
          viewModel.AnimationEditor.ApplyAnimation(element, animation, baseKeyframeOffset, selectionSet);
      }
    }

    public static bool CanPasteData(SafeDataObject dataObject)
    {
      bool flag = false;
      if (dataObject != null)
        flag = dataObject.GetDataPresent(Container.DataFormat.Name) || dataObject.GetDataPresent(DataFormats.Bitmap) || dataObject.GetDataPresent(DataFormats.FileDrop);
      return flag;
    }

    private struct DelayedElementTranslationInfo
    {
      public BaseFrameworkElement element;
      private LayoutCacheRecord cache;

      public Rect Bounds
      {
        get
        {
          return this.cache.Rect;
        }
      }

      public DelayedElementTranslationInfo(BaseFrameworkElement element)
      {
        Rect rect = (Rect) element.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.LayoutRectProperty);
        LayoutOverrides overrides = (LayoutOverrides) element.GetLocalOrDefaultValue(DesignTimeProperties.LayoutOverridesProperty);
        bool overlapping = element.IsSet(DesignTimeProperties.SlotOriginProperty) != PropertyState.Set;
        Point slotOrigin = overlapping ? new Point() : (Point) element.GetLocalOrDefaultValueAsWpf(DesignTimeProperties.SlotOriginProperty);
        this.element = element;
        this.cache = new LayoutCacheRecord(rect, slotOrigin, overrides, overlapping);
        this.element.ClearValue(DesignTimeProperties.LayoutRectProperty);
        this.element.ClearValue(DesignTimeProperties.LayoutOverridesProperty);
        this.element.ClearValue(DesignTimeProperties.SlotOriginProperty);
      }

      public void UpdateTranslation(ILayoutDesigner layoutDesigner, Rect unionOfBounds)
      {
        if (this.element.ViewObject == null || this.Bounds.IsEmpty || this.Bounds.Width <= 0.0 && this.Bounds.Height <= 0.0)
          return;
        using (layoutDesigner.SuppressLayoutRounding)
          layoutDesigner.SetLayoutFromCache(this.element, this.cache, unionOfBounds);
      }
    }
  }
}

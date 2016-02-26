// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ChangeLayoutTypeCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ChangeLayoutTypeCommand : LayoutTypeCommandBase
  {
    public ChangeLayoutTypeCommand(SceneViewModel viewModel, ITypeId type)
      : base(viewModel, type)
    {
    }

    public override void Execute(object arg)
    {
      ITypeId type = this.Type;
      if (type is ProjectNeutralTypeId && !this.SceneViewModel.ProjectContext.PlatformMetadata.IsSupported((ITypeResolver) this.SceneViewModel.ProjectContext, type))
        return;
      using (SceneEditTransaction editTransaction = this.DesignerContext.ActiveDocument.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitLayoutPaneChangeLayoutType, new object[1]
      {
        (object) this.Type.Name
      })))
      {
        bool flag1 = false;
        SceneViewModel activeSceneViewModel = this.DesignerContext.ActiveSceneViewModel;
        ReadOnlyCollection<SceneElement> selection = this.DesignerContext.SelectionManager.ElementSelectionSet.Selection;
        SceneElement primarySelection1 = this.DesignerContext.SelectionManager.ElementSelectionSet.PrimarySelection;
        List<SceneElement> list = new List<SceneElement>();
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        SceneElement sceneElement1 = (SceneElement) null;
        IStoryboardContainer storyboardContainer = (IStoryboardContainer) null;
        SceneElement primarySelection2 = (SceneElement) null;
        this.DesignerContext.SelectionManager.ElementSelectionSet.Clear();
        List<SceneElement> elements = new List<SceneElement>();
        elements.AddRange((IEnumerable<SceneElement>) selection);
        ElementUtilities.SortElementsByDepth(elements);
        foreach (SceneElement sceneElement2 in elements)
        {
          ITypeId typeId1 = (ITypeId) null;
          foreach (ITypeId typeId2 in ChangeLayoutTypeFlyoutCommand.LayoutTypes)
          {
            if (typeId2.IsAssignableFrom((ITypeId) sceneElement2.Type))
            {
              typeId1 = typeId2;
              break;
            }
          }
          if (typeId1 == null)
          {
            list.Add(sceneElement2);
          }
          else
          {
            if (activeSceneViewModel.LockedInsertionPoint != null)
            {
              if (activeSceneViewModel.LockedInsertionPoint.SceneElement == sceneElement2)
                flag2 = true;
              else if (sceneElement2.IsAncestorOf((SceneNode) activeSceneViewModel.ActiveSceneInsertionPoint.SceneElement))
              {
                sceneElement1 = activeSceneViewModel.ActiveSceneInsertionPoint.SceneElement;
                flag2 = true;
              }
            }
            if (primarySelection1 == sceneElement2)
              flag4 = true;
            if (!flag3 && activeSceneViewModel.ActiveStoryboardContainer == sceneElement2)
            {
              activeSceneViewModel.SetActiveStoryboardTimeline((IStoryboardContainer) null, (StoryboardTimelineSceneNode) null, (TriggerBaseNode) null);
              flag3 = true;
            }
            using (sceneElement2.ViewModel.ForceBaseValue())
            {
              BaseFrameworkElement parent1 = sceneElement2 as BaseFrameworkElement;
              List<LayoutCacheRecord> layoutCache = (List<LayoutCacheRecord>) null;
              ILayoutDesigner designerForParent = activeSceneViewModel.GetLayoutDesignerForParent(sceneElement2, true);
              if (parent1 != null)
                layoutCache = designerForParent.GetCurrentRects(parent1);
              SceneElement elementContainingChildren = (SceneElement) null;
              SceneElement sceneElement3 = ChangeLayoutTypeCommand.ChangeLayoutType(sceneElement2, type, ref elementContainingChildren);
              editTransaction.Update();
              if (sceneElement3.IsViewObjectValid)
              {
                this.SceneViewModel.DefaultView.UpdateLayout();
                BaseFrameworkElement parent2 = elementContainingChildren as BaseFrameworkElement;
                if (parent2 != null && parent2.IsViewObjectValid && layoutCache != null)
                  activeSceneViewModel.GetLayoutDesignerForParent(elementContainingChildren, true).SetCurrentRects(parent2, layoutCache);
                if (flag2 && sceneElement1 == null)
                  sceneElement1 = elementContainingChildren;
                if (flag3 && storyboardContainer == null)
                  storyboardContainer = (IStoryboardContainer) sceneElement3;
                if (flag4 && primarySelection2 == null)
                  primarySelection2 = sceneElement3;
                list.Add(sceneElement3);
              }
              flag1 = true;
            }
          }
        }
        if (flag2 && sceneElement1 != null)
          activeSceneViewModel.SetLockedInsertionPoint(sceneElement1);
        if (flag3 && storyboardContainer != null)
          activeSceneViewModel.SetActiveStoryboardTimeline(storyboardContainer, (StoryboardTimelineSceneNode) null, (TriggerBaseNode) null);
        this.DesignerContext.SelectionManager.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, primarySelection2);
        if (flag1)
          editTransaction.Commit();
        else
          editTransaction.Cancel();
      }
    }

    private static SceneElement ChangeLayoutType(SceneElement sourceElement, ITypeId layoutType, ref SceneElement elementContainingChildren)
    {
      IDocumentRoot documentRoot = sourceElement.DocumentNode.DocumentRoot;
      IDocumentContext documentContext = documentRoot.DocumentContext;
      SceneViewModel viewModel = sourceElement.ViewModel;
      Size size = Size.Empty;
      BaseFrameworkElement frameworkElement = sourceElement as BaseFrameworkElement;
      if (frameworkElement != null)
        size = frameworkElement.GetComputedTightBounds().Size;
      using (viewModel.ForceBaseValue())
      {
        SceneElement sceneElement = (SceneElement) viewModel.CreateSceneNode(layoutType);
        using (viewModel.DisableUpdateChildrenOnAddAndRemove())
        {
          using (viewModel.DisableDrawIntoState())
          {
            viewModel.AnimationEditor.DeleteAllAnimations((SceneNode) sourceElement);
            Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) sourceElement);
            Dictionary<IPropertyId, List<SceneNode>> storedChildren = ChangeLayoutTypeCommand.StoreChildren(sourceElement);
            if (sourceElement.DocumentNode == documentRoot.RootNode)
            {
              documentRoot.RootNode = sceneElement.DocumentNode;
              sceneElement.DocumentNode.NameScope = new DocumentNodeNameScope();
            }
            else
            {
              ISceneNodeCollection<SceneNode> collectionContainer = sourceElement.GetCollectionContainer();
              int index = collectionContainer.IndexOf((SceneNode) sourceElement);
              sourceElement.Remove();
              collectionContainer.Insert(index, (SceneNode) sceneElement);
            }
            SceneElementHelper.ApplyProperties((SceneNode) sceneElement, properties);
            elementContainingChildren = ChangeLayoutTypeCommand.ApplyChildren(sceneElement, storedChildren, size);
          }
        }
        if (sceneElement is GridElement || sceneElement is CanvasElement)
        {
          ILayoutDesigner designerForChild = sceneElement.ViewModel.GetLayoutDesignerForChild(sceneElement, true);
          if ((designerForChild.GetWidthConstraintMode((BaseFrameworkElement) sceneElement) & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
            sceneElement.SetValue(BaseFrameworkElement.WidthProperty, (object) size.Width);
          if ((designerForChild.GetHeightConstraintMode((BaseFrameworkElement) sceneElement) & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike)
            sceneElement.SetValue(BaseFrameworkElement.HeightProperty, (object) size.Height);
        }
        return sceneElement;
      }
    }

    private static Dictionary<IPropertyId, List<SceneNode>> StoreChildren(SceneElement sourceElement)
    {
      Dictionary<IPropertyId, List<SceneNode>> dictionary = new Dictionary<IPropertyId, List<SceneNode>>();
      foreach (IPropertyId propertyId in sourceElement.ContentProperties)
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = sourceElement.GetCollectionForProperty(propertyId);
        if (collectionForProperty != null)
        {
          while (collectionForProperty.Count > 0)
          {
            SceneNode sceneNode = collectionForProperty[0];
            if (sceneNode.DocumentNode != null)
              DocumentNodeHelper.PreserveFormatting(sceneNode.DocumentNode);
            sceneNode.Remove();
            List<SceneNode> list = (List<SceneNode>) null;
            if (!dictionary.TryGetValue(propertyId, out list))
            {
              list = new List<SceneNode>();
              dictionary.Add(propertyId, list);
            }
            list.Add(sceneNode);
          }
        }
      }
      return dictionary;
    }

    internal static SceneElement ApplyChildren(SceneElement destinationElement, Dictionary<IPropertyId, List<SceneNode>> storedChildren, Size size)
    {
      SceneElement sceneElement = (SceneElement) null;
      foreach (KeyValuePair<IPropertyId, List<SceneNode>> keyValuePair in storedChildren)
      {
        IPropertyId childProperty = keyValuePair.Key;
        ReferenceStep referenceStep = childProperty as ReferenceStep;
        if (referenceStep != null)
          childProperty = (IPropertyId) SceneNodeObjectSet.FilterProperty((SceneNode) destinationElement, referenceStep) ?? (IPropertyId) destinationElement.DefaultContentProperty;
        if (childProperty != null)
        {
          ISceneNodeCollection<SceneNode> collectionForProperty = destinationElement.GetCollectionForProperty(childProperty);
          if (collectionForProperty != null)
          {
            sceneElement = destinationElement;
            List<SceneNode> list = keyValuePair.Value;
            if (collectionForProperty.FixedCapacity.HasValue && list.Count > collectionForProperty.FixedCapacity.Value - collectionForProperty.Count)
            {
              SceneNode sceneNode = destinationElement.ViewModel.CreateSceneNode(PlatformTypes.Grid);
              if (ProjectNeutralTypes.Viewbox.IsAssignableFrom((ITypeId) destinationElement.Type) && !size.IsEmpty)
              {
                sceneNode.SetValue(BaseFrameworkElement.WidthProperty, (object) size.Width);
                sceneNode.SetValue(BaseFrameworkElement.HeightProperty, (object) size.Height);
              }
              collectionForProperty.Add(sceneNode);
              collectionForProperty = sceneNode.GetCollectionForProperty((IPropertyId) sceneNode.DefaultContentProperty);
              sceneElement = sceneNode as SceneElement;
            }
            if (ProjectNeutralTypes.Viewbox.IsAssignableFrom((ITypeId) destinationElement.Type))
              destinationElement.SetValue(ViewboxElement.StretchProperty, (object) Stretch.Fill);
            foreach (SceneNode sceneNode in list)
              collectionForProperty.Add(sceneNode);
          }
        }
      }
      return sceneElement;
    }
  }
}

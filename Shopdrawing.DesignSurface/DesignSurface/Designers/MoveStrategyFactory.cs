// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.MoveStrategyFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal static class MoveStrategyFactory
  {
    public static MoveStrategy Create(MoveStrategyContext context, SceneElement hitElement, bool constraining)
    {
      MoveStrategy moveStrategy = (MoveStrategy) null;
      if (context.IsRecordingKeyframes)
        moveStrategy = MoveStrategyFactory.CreateForAnimationMode(context);
      if (moveStrategy == null)
        moveStrategy = MoveStrategyFactory.CreateForLayoutMode(context, hitElement);
      if (moveStrategy != null)
        moveStrategy.IsConstraining = constraining;
      return moveStrategy;
    }

    private static MoveStrategy CreateForAnimationMode(MoveStrategyContext context)
    {
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
      if (context.DraggedElements.Count != 0)
      {
        SceneNode parent = context.DraggedElements[0].Parent;
        while (parent != null && !(parent is BaseFrameworkElement))
          parent = parent.Parent;
        frameworkElement = parent as BaseFrameworkElement;
      }
      if (frameworkElement == null || !context.RootNode.IsAncestorOf((SceneNode) frameworkElement))
        return (MoveStrategy) null;
      LayoutMoveStrategy layoutMoveStrategy = new LayoutMoveStrategy(context);
      layoutMoveStrategy.EnableAnimationMode = true;
      layoutMoveStrategy.LayoutContainer = frameworkElement;
      return (MoveStrategy) layoutMoveStrategy;
    }

    private static MoveStrategy CreateForLayoutMode(MoveStrategyContext context, SceneElement hitElement)
    {
      SceneElement sceneElement = hitElement;
      while (sceneElement != null)
      {
        BaseFrameworkElement frameworkElement = sceneElement as BaseFrameworkElement;
        if (frameworkElement != null && context.RootNode.IsAncestorOf((SceneNode) frameworkElement))
        {
          if (!MoveStrategyFactory.CanMoveInside(context, frameworkElement))
          {
            sceneElement = frameworkElement.ParentElement;
            continue;
          }
          MoveStrategy byParentType = MoveStrategyFactory.CreateByParentType(context, frameworkElement);
          if (byParentType != null)
          {
            bool flag = false;
            ItemsControlElement itemsControlElement = frameworkElement.ParentElement as ItemsControlElement;
            ContentControlElement contentControlElement = frameworkElement as ContentControlElement;
            if (itemsControlElement != null && contentControlElement != null && (ItemsControlElement.ItemsProperty.Equals((object) itemsControlElement.GetPropertyForChild((SceneNode) frameworkElement)) && (itemsControlElement.ViewObject as IViewItemsControl).IsItemItsOwnContainer(contentControlElement.ViewObject)))
              flag = true;
            if (!flag)
            {
              IProperty contentProperty = MoveStrategyFactory.GetContentProperty(context, (SceneNode) frameworkElement);
              int childrenForElement = MoveStrategy.GetMaxChildrenForElement((SceneElement) frameworkElement, contentProperty);
              if (context.DraggedElements.Count <= childrenForElement && (childrenForElement != 1 || context.DraggedElements.Count != 1 || (!frameworkElement.IsAncestorOf((SceneNode) context.DraggedElements[0]) || context.DraggedElements[0].Parent == frameworkElement)))
              {
                byParentType.LayoutContainer = frameworkElement;
                return byParentType;
              }
            }
          }
        }
        if (sceneElement != context.RootNode)
          sceneElement = sceneElement.ParentElement;
        else
          break;
      }
      return (MoveStrategy) null;
    }

    private static IProperty GetContentProperty(MoveStrategyContext context, SceneNode candidateContainer)
    {
      IProperty property = (IProperty) null;
      if (context.PrimarySelection != null && context.PrimarySelection.Parent != null && context.PrimarySelection.Parent.Equals((object) candidateContainer))
        property = candidateContainer.GetPropertyForChild((SceneNode) context.PrimarySelection);
      return property ?? candidateContainer.DefaultContentProperty;
    }

    private static bool CanMoveInside(MoveStrategyContext context, BaseFrameworkElement candidateContainer)
    {
      if (context.PrimarySelection == null)
        return false;
      IProperty contentProperty = MoveStrategyFactory.GetContentProperty(context, (SceneNode) candidateContainer);
      if (contentProperty == null)
        return false;
      PropertySceneInsertionPoint insertionPoint = new PropertySceneInsertionPoint((SceneElement) candidateContainer, contentProperty);
      ControlTemplateElement controlTemplateElement;
      if (candidateContainer.IsLockedOrAncestorLocked || candidateContainer.IsHiddenOrCollapsedOrAncestorHiddenOrCollapsed || (context.SelectedElements.Count == 0 || Enumerable.Any<SceneElement>((IEnumerable<SceneElement>) context.SelectedElements, (Func<SceneElement, bool>) (element => !insertionPoint.CanInsert((ITypeId) element.TrueTargetTypeId)))) || ProjectNeutralTypes.TabItem.IsAssignableFrom((ITypeId) candidateContainer.Type) || (candidateContainer.Name == "PART_ContentHost" && typeof (ScrollViewer).IsAssignableFrom(candidateContainer.TargetType) && ((controlTemplateElement = context.RootNode as ControlTemplateElement) != null && typeof (TextBoxBase).IsAssignableFrom(controlTemplateElement.TargetElementType)) || candidateContainer.IsPlaceholder))
        return false;
      foreach (SceneNode sceneNode in context.DraggedElements)
      {
        if (sceneNode.IsAncestorOf((SceneNode) candidateContainer))
          return false;
      }
      return true;
    }

    private static MoveStrategy CreateByParentType(MoveStrategyContext context, BaseFrameworkElement parentContainer)
    {
      MoveStrategyFactory.MoveStrategyTypeHandlerFactory orCreateCache = DesignSurfacePlatformCaches.GetOrCreateCache<MoveStrategyFactory.MoveStrategyTypeHandlerFactory>(parentContainer.Platform.Metadata, DesignSurfacePlatformCaches.MoveStrategyFactoryCache);
      ItemsControlElement itemsControlElement = parentContainer as ItemsControlElement;
      TextBlockElement textBlockElement = parentContainer as TextBlockElement;
      Type type = (Type) null;
      if (itemsControlElement != null && itemsControlElement.ItemsHost != null)
        type = orCreateCache.GetHandlerType((ITypeResolver) itemsControlElement.ProjectContext, itemsControlElement.ItemsHost.GetIType((ITypeResolver) parentContainer.ProjectContext), true);
      if (type == (Type) null)
        type = orCreateCache.GetHandlerType((ITypeResolver) parentContainer.ProjectContext, parentContainer.Type, false);
      if (type == typeof (GenericMoveStrategy))
      {
        ILayoutDesigner designerForParent = parentContainer.ViewModel.GetLayoutDesignerForParent((SceneElement) parentContainer, true);
        if ((designerForParent.GetWidthConstraintMode(parentContainer, (BaseFrameworkElement) null) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && (designerForParent.GetHeightConstraintMode(parentContainer, (BaseFrameworkElement) null) & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike)
          type = typeof (LayoutMoveStrategy);
      }
      if (textBlockElement != null && !parentContainer.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTextBlockInlineUIContainer))
        type = (Type) null;
      if (!(type != (Type) null))
        return (MoveStrategy) null;
      MoveStrategy moveStrategy = (MoveStrategy) Activator.CreateInstance(type, new object[1]
      {
        (object) context
      });
      moveStrategy.LayoutContainer = parentContainer;
      return moveStrategy;
    }

    private sealed class MoveStrategyTypeHandlerFactory : TypeIdHandlerFactory<MoveStrategyFactory.MoveStrategyTypeHandler>
    {
      public Type GetHandlerType(ITypeResolver typeResolver, IType type, bool appliesToItemsHost)
      {
        MoveStrategyFactory.MoveStrategyTypeHandler handler = this.GetHandler((IMetadataResolver) typeResolver, type);
        if (handler != null && (!appliesToItemsHost || handler.AppliesToItemsHost))
          return handler.DragDropBehaviorType;
        return (Type) null;
      }

      protected override void Initialize()
      {
        base.Initialize();
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.Object, typeof (LayoutMoveStrategy), true));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.Control, typeof (GenericMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(ProjectNeutralTypes.Viewbox, typeof (GenericMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.Popup, typeof (GenericMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(ProjectNeutralTypes.DockPanel, typeof (DockPanelMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.StackPanel, typeof (FlowPanelMoveStrategy), true));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.VirtualizingStackPanel, typeof (FlowPanelMoveStrategy), true));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(ProjectNeutralTypes.WrapPanel, typeof (FlowPanelMoveStrategy), true));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.UniformGrid, typeof (FlowPanelMoveStrategy), true));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.ToolBarPanel, typeof (FlowPanelMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.RichTextBox, typeof (TextFlowMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.TextBlock, typeof (TextFlowMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(PlatformTypes.FlowDocumentScrollViewer, typeof (TextFlowMoveStrategy), false));
        this.RegisterHandler(new MoveStrategyFactory.MoveStrategyTypeHandler(ProjectNeutralTypes.PathListBox, typeof (LayoutMoveStrategy), false));
      }

      protected override ITypeId GetBaseType(MoveStrategyFactory.MoveStrategyTypeHandler handler)
      {
        return handler.BaseType;
      }
    }

    private sealed class MoveStrategyTypeHandler
    {
      private ITypeId baseType;
      private Type dragDropBehaviorType;
      private bool appliesToItemsHost;

      public ITypeId BaseType
      {
        get
        {
          return this.baseType;
        }
      }

      public Type DragDropBehaviorType
      {
        get
        {
          return this.dragDropBehaviorType;
        }
      }

      public bool AppliesToItemsHost
      {
        get
        {
          return this.appliesToItemsHost;
        }
      }

      public MoveStrategyTypeHandler(ITypeId baseType, Type dragDropBehaviorType, bool appliesToItemsHost)
      {
        this.baseType = baseType;
        this.dragDropBehaviorType = dragDropBehaviorType;
        this.appliesToItemsHost = appliesToItemsHost;
      }
    }
  }
}

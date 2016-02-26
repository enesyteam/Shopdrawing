// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ItemsControlElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ItemsControlElement : BaseFrameworkElement
  {
    public static readonly IPropertyId ItemsProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "Items", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemsSourceProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemsSource", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemsPanelProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemsPanel", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemTemplateProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemTemplate", MemberAccessTypes.Public);
    public static readonly IPropertyId ItemContainerStyleProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "ItemContainerStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId DisplayMemberPathProperty = (IPropertyId) PlatformTypes.ItemsControl.GetMember(MemberType.LocalProperty, "DisplayMemberPath", MemberAccessTypes.Public);
    public static readonly ItemsControlElement.ConcreteItemsControlElementFactory Factory = new ItemsControlElement.ConcreteItemsControlElementFactory();

    public IType ItemType
    {
      get
      {
        if (ProjectNeutralTypes.TabControl.IsAssignableFrom((ITypeId) this.Type))
          return this.ProjectContext.ResolveType(ProjectNeutralTypes.TabItem);
        IPropertyId containerStyleProperty = ItemsControlElement.GetItemContainerStyleProperty(this.ProjectContext, (ITypeId) this.Type);
        Type type = containerStyleProperty != null ? this.Metadata.GetStylePropertyTargetType(containerStyleProperty) : (Type) null;
        if (!(type != (Type) null))
          return (IType) null;
        return this.ProjectContext.GetType(type);
      }
    }

    public ISceneNodeCollection<SceneNode> Items
    {
      get
      {
        return (ISceneNodeCollection<SceneNode>) new SceneNode.SceneNodeCollection<SceneNode>((SceneNode) this, ItemsControlElement.ItemsProperty);
      }
    }

    public IViewPanel ItemsHost
    {
      get
      {
        IViewObject viewObject = this.ViewObject;
        Panel panel = viewObject == null ? (Panel) null : this.LocateItemsHostCore(viewObject.PlatformSpecificObject as Visual);
        if (panel != null)
          return (IViewPanel) this.Platform.ViewObjectFactory.Instantiate((object) panel);
        return (IViewPanel) null;
      }
    }

    public static IPropertyId GetItemContainerStyleProperty(IProjectContext projectContext, ITypeId targetType)
    {
      if (projectContext.IsCapabilitySet(PlatformCapability.WorkaroundSL16177And21487))
        return targetType.GetMember(MemberType.LocalProperty, ItemsControlElement.ItemContainerStyleProperty.Name, MemberAccessTypes.Public) as IPropertyId;
      return ItemsControlElement.ItemContainerStyleProperty;
    }

    public override void AddCustomContextMenuCommands(ICommandBar contextMenu)
    {
      base.AddCustomContextMenuCommands(contextMenu);
      contextMenu.Items.AddButton("Edit_BindToData");
      IType itemType = this.ItemType;
      this.ProjectContext.ResolveType(PlatformTypes.FrameworkElement);
      if (itemType == null || PlatformTypes.FrameworkElement.Equals((object) itemType))
        return;
      contextMenu.Items.AddButton("Edit_AddItem");
    }

    protected override DocumentNode ResolveDefaultStyleAsDocumentNode(IType targetType, IPropertyId propertyKey)
    {
      IPropertyId containerStyleProperty = ItemsControlElement.GetItemContainerStyleProperty(this.ProjectContext, (ITypeId) targetType);
      return containerStyleProperty == null || !containerStyleProperty.Equals((object) propertyKey) ? base.ResolveDefaultStyleAsDocumentNode(targetType, propertyKey) : base.ResolveDefaultStyleAsDocumentNode(this.ProjectContext.ResolveType((ITypeId) this.ItemType), BaseFrameworkElement.StyleProperty);
    }

    private Panel LocateItemsHostCore(Visual root)
    {
      if (root == null)
        return (Panel) null;
      Panel panel1 = root as Panel;
      if (panel1 != null && panel1.IsItemsHost)
        return panel1;
      int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) root);
      for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
      {
        Visual root1 = VisualTreeHelper.GetChild((DependencyObject) root, childIndex) as Visual;
        if (!(root1 is ItemsControl))
        {
          Panel panel2 = this.LocateItemsHostCore(root1);
          if (panel2 != null)
            return panel2;
        }
      }
      return (Panel) null;
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      if (modification == SceneNode.Modification.SetValue || modification == SceneNode.Modification.InsertValue)
        ItemsControlElement.ClearMutuallyExclusivePropertyIfNeeded((SceneElement) this, propertyReference);
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    public static void ClearMutuallyExclusivePropertyIfNeeded(SceneElement itemsControl, PropertyReference propertyReference)
    {
      ReferenceStep firstStep = propertyReference.FirstStep;
      if (ItemsControlElement.ItemsSourceProperty.Equals((object) firstStep))
      {
        ISceneNodeCollection<SceneNode> collectionForProperty = itemsControl.GetCollectionForProperty((IPropertyId) itemsControl.DefaultContentProperty);
        if (collectionForProperty != null)
        {
          foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
          {
            SceneElement element = sceneNode as SceneElement;
            if (element != null)
              itemsControl.ViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(element);
          }
          collectionForProperty.Clear();
        }
        itemsControl.ClearValue(ItemsControlElement.ItemsProperty);
      }
      else if (ItemsControlElement.ItemsProperty.Equals((object) firstStep))
        ItemsControlElement.UpdateItemsSourceOnItemsChanged(itemsControl);
      else if (ItemsControlElement.ItemTemplateProperty.Equals((object) firstStep))
      {
        itemsControl.ClearValue(ItemsControlElement.DisplayMemberPathProperty);
      }
      else
      {
        if (!ItemsControlElement.DisplayMemberPathProperty.Equals((object) firstStep))
          return;
        itemsControl.ClearValue(ItemsControlElement.ItemTemplateProperty);
      }
    }

    protected override void OnChildAdded(SceneNode child)
    {
      ItemsControlElement.OnChildAddedHelper((SceneElement) this, child);
      base.OnChildAdded(child);
    }

    public static void OnChildAddedHelper(SceneElement itemsControl, SceneNode child)
    {
      IProperty propertyForChild = itemsControl.GetPropertyForChild(child);
      if (propertyForChild == null || !ItemsControlElement.ItemsProperty.Equals((object) propertyForChild))
        return;
      ItemsControlElement.UpdateItemsSourceOnItemsChanged(itemsControl);
    }

    private static void UpdateItemsSourceOnItemsChanged(SceneElement itemsControl)
    {
      if (PropertyState.Unset == itemsControl.IsSet(ItemsControlElement.ItemsSourceProperty))
        return;
      itemsControl.ClearValue(ItemsControlElement.ItemTemplateProperty);
      itemsControl.ClearValue(ItemsControlElement.ItemsSourceProperty);
    }

    public class ConcreteItemsControlElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ItemsControlElement();
      }
    }
  }
}

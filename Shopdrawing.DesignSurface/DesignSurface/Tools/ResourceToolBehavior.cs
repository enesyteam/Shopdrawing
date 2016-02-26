// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ResourceToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class ResourceToolBehavior : DragDropInsertBehavior
  {
    private Dictionary<SceneElement, bool> cachedApplyProperties = new Dictionary<SceneElement, bool>();
    private Dictionary<SceneElement, bool> cachedCanDrop = new Dictionary<SceneElement, bool>();
    private const int TypeSortOrder = 0;
    private const int PropertiesSortOrder = 1;
    private const int OthersSortOrder = 2;

    public ResourceToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnDragLeave(DragEventArgs args)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return base.OnDragLeave(args);
    }

    protected override bool OnDrop(DragEventArgs args)
    {
      Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
      SceneElement hitElement = this.ActiveView.GetSelectableElementAtPoint(position, SelectionFor3D.Deep, false) ?? this.ActiveView.GetElementAtPoint(position, new HitTestModifier(this.ActiveView.GetSelectableElement), (InvisibleObjectHitTestModifier) null, (ICollection<BaseFrameworkElement>) null);
      ContextMenu menu = new ContextMenu();
      this.TryAddGenericCommands(menu, position);
      int count = menu.Items.Count;
      if (hitElement != null)
      {
        this.AddTargetType(menu, position, this.GetResourceEntry(args.Data));
        this.AddValidElementProperties(menu, args.Data, hitElement);
        menu.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
        menu.Items.SortDescriptions.Add(new SortDescription("IsEnabled", ListSortDirection.Ascending));
        menu.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
        menu.Items.Refresh();
        switch (menu.Items.Count)
        {
          case 0:
          case 1:
            break;
          case 2:
            if (count == 0)
            {
              ((MenuItem) menu.Items[1]).Command.Execute((object) null);
              break;
            }
            menu.IsOpen = true;
            break;
          default:
            menu.IsOpen = true;
            break;
        }
      }
      else
        menu.IsOpen = true;
      if (!this.IsSuspended)
        this.PopSelf();
      return true;
    }

    protected override bool OnDragOver(DragEventArgs args)
    {
      args.Effects = DragDropEffects.None;
      Point position = args.GetPosition((IInputElement) this.ActiveView.ViewRootContainer);
      SceneElement index = this.ActiveView.GetSelectableElementAtPoint(position, SelectionFor3D.Deep, false) ?? this.ActiveView.GetElementAtPoint(position, new HitTestModifier(this.ActiveView.GetSelectableElement), (InvisibleObjectHitTestModifier) null, (ICollection<BaseFrameworkElement>) null);
      if (index != null)
      {
        ContextMenu menu = (ContextMenu) null;
        bool flag;
        if (!this.cachedApplyProperties.TryGetValue(index, out flag))
        {
          menu = new ContextMenu();
          ResourceEntryItem resourceEntry = this.GetResourceEntry(args.Data);
          this.AddTargetType(menu, position, resourceEntry);
          int count = menu.Items.Count;
          this.AddValidElementProperties(menu, args.Data, index);
          this.cachedApplyProperties.Add(index, menu.Items.Count - count > 0);
          this.cachedCanDrop.Add(index, menu.Items.Count >= 2);
        }
        if (this.cachedCanDrop[index] && (menu == null || menu.Items.Count >= 2))
          args.Effects = flag ? DragDropEffects.Copy : DragDropEffects.Move;
      }
      else
      {
        ContextMenu menu = new ContextMenu();
        this.TryAddGenericCommands(menu, position);
        if (menu.Items.Count >= 1)
          args.Effects = DragDropEffects.Copy;
      }
      base.OnDragOver(args);
      return true;
    }

    protected override bool OnHover(Point pointerPosition)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return false;
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      if (!this.IsSuspended)
        this.PopSelf();
      return false;
    }

    private void AddTargetType(ContextMenu menu, Point dropPoint, ResourceEntryItem resource)
    {
      IType type = this.GetType(resource);
      ISceneInsertionPoint pointFromPosition = this.ActiveSceneViewModel.GetActiveSceneInsertionPointFromPosition(new InsertionPointContext(dropPoint));
      if (type == null || !new TypeAsset(type).CanCreateInstance(pointFromPosition))
        return;
      MenuItem menuItem = new MenuItem();
      menuItem.SetValue(AutomationElement.IdProperty, (object) type.Name);
      menuItem.Header = (object) type.Name;
      menuItem.Command = (ICommand) new ResourceToolBehavior.CreateInstanceCommand(this, dropPoint, type, resource);
      menuItem.Tag = (object) 0;
      menu.Items.Add((object) menuItem);
      this.AddHeaderItem(menu, StringTable.DragDropResourceCreateNewControlPopupHeader, 0);
    }

    private void TryAddGenericCommands(ContextMenu menu, Point dropPoint)
    {
      ConvertToElementsCommand toElementsCommand = new ConvertToElementsCommand(this.ActiveSceneViewModel.DesignerContext, this.ActiveSceneViewModel.DesignerContext.ResourceManager, dropPoint);
      if (!toElementsCommand.IsVisible)
        return;
      this.AddHeaderItem(menu, StringTable.ConvertToElementsContextMenuHeader, 2);
      MenuItem menuItem = new MenuItem();
      menuItem.SetValue(AutomationElement.IdProperty, (object) "ConvertItem");
      menuItem.Header = (object) toElementsCommand.CommandName;
      menuItem.Command = (ICommand) toElementsCommand;
      menuItem.Tag = (object) 2;
      menu.Items.Add((object) menuItem);
    }

    private void AddValidElementProperties(ContextMenu menu, IDataObject data, SceneElement hitElement)
    {
      ResourceEntryItem resourceEntry = this.GetResourceEntry(data);
      if (resourceEntry == null || resourceEntry.Resource == null || hitElement == null)
        return;
      ResourceManager resourceManager = this.ActiveSceneViewModel.DesignerContext.ResourceManager;
      DocumentReference documentReference = resourceEntry.Container.DocumentReference;
      if (!PlatformTypes.PlatformsCompatible(hitElement.ProjectContext.PlatformMetadata, resourceEntry.DocumentNode.PlatformMetadata) || !resourceEntry.Resource.IsResourceReachable((SceneNode) hitElement) && (!(documentReference != resourceManager.ActiveRootContainer.DocumentReference) || !(documentReference != resourceManager.TopLevelResourceContainer.DocumentReference)))
        return;
      int count = menu.Items.Count;
      this.AddProperties(hitElement, resourceEntry, menu.Items);
      if (menu.Items.Count <= count)
        return;
      string headerText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DragDropResourceExistingControlPopupHeader, new object[1]
      {
        (object) (string.IsNullOrEmpty(hitElement.Name) ? "[" + hitElement.TargetType.Name + "]" : hitElement.Name)
      });
      this.AddHeaderItem(menu, headerText, 1);
    }

    private void AddHeaderItem(ContextMenu menu, string headerText, int sortOrder)
    {
      MenuItem menuItem = new MenuItem();
      menuItem.IsEnabled = false;
      menuItem.SetResourceReference(Control.BackgroundProperty, (object) "Text1Brush");
      menuItem.SetResourceReference(Control.ForegroundProperty, (object) "DarkBrush");
      menuItem.StaysOpenOnClick = true;
      menuItem.Header = (object) headerText;
      menuItem.Tag = (object) sortOrder;
      menu.Items.Add((object) menuItem);
    }

    private void AddProperties(SceneElement targetElement, ResourceEntryItem resource, System.Windows.Controls.ItemCollection menuItems)
    {
      Type filterType = (Type) null;
      bool flag = true;
      if (PlatformTypes.Brush.IsAssignableFrom((ITypeId) resource.Resource.Type))
        filterType = typeof (Brush);
      else if (PlatformTypes.DrawingImage.IsAssignableFrom((ITypeId) resource.Resource.Type))
        filterType = typeof (ImageSource);
      else if (PlatformTypes.PlatformsCompatible(resource.Resource.ValueNode.Type.PlatformMetadata, targetElement.Type.PlatformMetadata))
      {
        IType styleOrTemplateType;
        ITypeId typeAndTargetType = DocumentNodeUtilities.GetStyleOrTemplateTypeAndTargetType(resource.Resource.ValueNode, out styleOrTemplateType);
        if (typeAndTargetType != null)
        {
          flag = typeAndTargetType.IsAssignableFrom((ITypeId) targetElement.Type);
          filterType = styleOrTemplateType.RuntimeType;
        }
      }
      else
        flag = false;
      if (!flag)
        return;
      int count = menuItems.Count;
      this.AddElementPropertiesWithTypeFilter(targetElement, resource, menuItems, filterType);
      if (menuItems.Count != count)
        return;
      this.AddElementPropertiesWithTypeFilter(targetElement, resource, menuItems, (Type) null);
    }

    private void AddElementPropertiesWithTypeFilter(SceneElement targetElement, ResourceEntryItem resource, System.Windows.Controls.ItemCollection menuItems, Type filterType)
    {
      SceneNode[] selection = new SceneNode[1]
      {
        (SceneNode) targetElement
      };
      foreach (TargetedReferenceStep targetedReferenceStep in (IEnumerable<TargetedReferenceStep>) PropertyMerger.GetMergedProperties((IEnumerable<SceneNode>) selection))
      {
        ReferenceStep referenceStep = targetedReferenceStep.ReferenceStep;
        if (referenceStep.Name != "Name" && PropertyInspectorModel.IsPropertyBrowsable(selection, targetedReferenceStep) && (PropertyInspectorModel.IsAttachedPropertyBrowsable(selection, targetElement.Type, targetedReferenceStep, (ITypeResolver) targetElement.ProjectContext) && PlatformTypeHelper.GetPropertyType((IProperty) referenceStep) != (Type) null) && (PlatformTypeHelper.GetPropertyType((IProperty) referenceStep).IsAssignableFrom(resource.Resource.Type.RuntimeType) && (filterType == (Type) null || filterType.IsAssignableFrom(PlatformTypeHelper.GetPropertyType((IProperty) referenceStep)))))
          menuItems.Add((object) this.BuildPropertyMenuItem(targetElement, referenceStep, resource));
      }
    }

    private MenuItem BuildPropertyMenuItem(SceneElement targetElement, ReferenceStep targetProperty, ResourceEntryItem resource)
    {
      MenuItem menuItem = new MenuItem();
      menuItem.SetValue(AutomationElement.IdProperty, (object) targetProperty.Name);
      menuItem.Header = (object) targetProperty.Name;
      menuItem.Command = (ICommand) new ResourceToolBehavior.ApplyResourceCommand(this, targetElement, targetProperty, resource);
      menuItem.Tag = (object) 1;
      return menuItem;
    }

    private void CreateInstance(Point dropPoint, ResourceEntryItem resource, IType type)
    {
      ISceneInsertionPoint pointFromPosition = this.ActiveSceneViewModel.GetActiveSceneInsertionPointFromPosition(new InsertionPointContext(dropPoint));
      TypeAsset typeAsset = new TypeAsset(type);
      if (!typeAsset.CanCreateInstance(pointFromPosition))
        return;
      this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      dropPoint = this.ToolBehaviorContext.SnappingEngine.SnapPoint(dropPoint, EdgeFlags.All);
      this.ToolBehaviorContext.SnappingEngine.Stop();
      Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(this.ActiveView.GetComputedTransformToRoot(pointFromPosition.SceneElement));
      dropPoint *= inverseMatrix;
      using (SceneEditTransaction editTransaction = this.ActiveDocument.CreateEditTransaction(StringTable.CreateResourceViaToolUndoUnit))
      {
        using (this.ActiveSceneViewModel.DesignerContext.AmbientPropertyManager.SuppressApplyAmbientProperties())
        {
          typeAsset.CreateInstance(this.ActiveSceneViewModel.DesignerContext.LicenseManager, pointFromPosition, new Rect(dropPoint, new Size(double.PositiveInfinity, double.PositiveInfinity)), (OnCreateInstanceAction) (sceneNode =>
          {
            SceneElement targetElement = sceneNode as SceneElement;
            if (targetElement == null)
              return;
            if (resource is StyleResourceItem)
              this.ApplyResourceOnExistingElement(targetElement, BaseFrameworkElement.StyleProperty, resource);
            else if (resource is ControlTemplateResourceItem)
              this.ApplyResourceOnExistingElement(targetElement, ControlElement.TemplateProperty, resource);
            else if (resource is DrawingImageResourceItem)
            {
              this.ApplyResourceOnExistingElement(targetElement, ImageElement.SourceProperty, resource);
            }
            else
            {
              if (!(resource is BrushResourceItem))
                return;
              this.ApplyResourceOnExistingElement(targetElement, ShapeElement.FillProperty, resource);
              targetElement.SetValue(ShapeElement.StrokeProperty, (object) null);
            }
          }));
          editTransaction.Commit();
        }
      }
    }

    private void ApplyResourceOnExistingElement(SceneElement targetElement, IPropertyId targetProperty, ResourceEntryItem resource)
    {
      if (resource.Resource.KeyNode != null || targetProperty.Equals((object) BaseFrameworkElement.StyleProperty))
      {
        using (SceneEditTransaction editTransaction = this.ActiveDocument.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertySetUndo, new object[1]
        {
          (object) targetProperty.Name
        })))
        {
          if (resource.Resource.KeyNode != null)
          {
            IDocumentContext documentContext = this.ActiveSceneViewModel.Document.DocumentContext;
            IProjectContext projectContext = this.ActiveSceneViewModel.Document.ProjectContext;
            DocumentNode keyNode = resource.Resource.KeyNode.Clone(documentContext);
            DocumentNode documentNode = !(projectContext.ResolveProperty(targetProperty) is DependencyPropertyReferenceStep) || !JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.DynamicResource) ? (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode) : (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(documentContext, keyNode);
            targetElement.SetValue(targetProperty, (object) documentNode);
          }
          else if (targetProperty.Equals((object) BaseFrameworkElement.StyleProperty))
            targetElement.ClearValue(targetProperty);
          editTransaction.Commit();
        }
      }
      ResourceManager resourceManager = this.ActiveSceneViewModel.DesignerContext.ResourceManager;
      DocumentReference documentReference = resource.Container.DocumentReference;
      if (resource.Resource.IsResourceReachable((SceneNode) targetElement) || !(documentReference != resourceManager.ActiveRootContainer.DocumentReference) || !(documentReference != resourceManager.TopLevelResourceContainer.DocumentReference))
        return;
      resourceManager.LinkToResource(resourceManager.TopLevelResourceContainer, documentReference);
    }

    private ResourceEntryItem GetResourceEntry(IDataObject dataObject)
    {
      ResourceEntryItem resourceEntryItem = (ResourceEntryItem) null;
      if (dataObject != null && dataObject.GetDataPresent("ResourceEntryItem", true))
        resourceEntryItem = (ResourceEntryItem) dataObject.GetData("ResourceEntryItem", true);
      return resourceEntryItem;
    }

    private IType GetType(ResourceEntryItem resource)
    {
      IType type = (IType) null;
      if (resource != null && resource.Resource != null)
      {
        TypedResourceItem typedResourceItem = (TypedResourceItem) null;
        StyleResourceItem styleResourceItem;
        if ((styleResourceItem = resource as StyleResourceItem) != null)
          typedResourceItem = (TypedResourceItem) styleResourceItem;
        else if (resource is BrushResourceItem)
          type = resource.DocumentNode.TypeResolver.ResolveType(PlatformTypes.Rectangle);
        else if (resource is DrawingImageResourceItem)
        {
          type = resource.DocumentNode.TypeResolver.ResolveType(PlatformTypes.Image);
        }
        else
        {
          ControlTemplateResourceItem templateResourceItem = resource as ControlTemplateResourceItem;
          if (templateResourceItem != null)
            typedResourceItem = (TypedResourceItem) templateResourceItem;
        }
        if (typedResourceItem != null && typedResourceItem.Type != null && !typedResourceItem.Type.IsAbstract)
          type = typedResourceItem.Type;
      }
      return type;
    }

    private class CreateInstanceCommand : ICommand
    {
      private ResourceToolBehavior behavior;
      private Point dropPoint;
      private IType type;
      private ResourceEntryItem resource;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public CreateInstanceCommand(ResourceToolBehavior behavior, Point dropPoint, IType type, ResourceEntryItem resource)
      {
        this.behavior = behavior;
        this.dropPoint = dropPoint;
        this.type = type;
        this.resource = resource;
      }

      public void Execute(object arg)
      {
        this.behavior.CreateInstance(this.dropPoint, this.resource, this.type);
      }

      public bool CanExecute(object arg)
      {
        return true;
      }
    }

    private class ApplyResourceCommand : ICommand
    {
      private ResourceToolBehavior behavior;
      private SceneElement targetElement;
      private ResourceEntryItem resource;
      private ReferenceStep targetProperty;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public ApplyResourceCommand(ResourceToolBehavior behavior, SceneElement targetElement, ReferenceStep targetProperty, ResourceEntryItem resource)
      {
        this.behavior = behavior;
        this.targetElement = targetElement;
        this.resource = resource;
        this.targetProperty = targetProperty;
      }

      public void Execute(object arg)
      {
        this.behavior.ApplyResourceOnExistingElement(this.targetElement, (IPropertyId) this.targetProperty, this.resource);
      }

      public bool CanExecute(object arg)
      {
        return true;
      }
    }
  }
}

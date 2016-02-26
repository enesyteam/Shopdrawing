// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ResourceListCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class ResourceListCommand : SingleTargetDynamicMenuCommandBase
  {
    private ReferenceStep targetProperty;

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || this.Type == null || (this.TargetElement is StyleNode || PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.Type)) && this.TargetProperty.Equals((object) BaseFrameworkElement.StyleProperty) || (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) this.Type) && (!ControlElement.TemplateProperty.Equals((object) this.TargetProperty) || PageElement.TemplateProperty.Equals((object) this.TargetProperty)) || !this.TargetElement.ProjectContext.GetType(this.TargetProperty.TargetType).IsAssignableFrom((ITypeId) this.Type)))
          return false;
        return this.Resources.Count > 0;
      }
    }

    public override IEnumerable Items
    {
      get
      {
        List<MenuItem> list = new List<MenuItem>();
        foreach (LocalResourceModel resourceModel in (IEnumerable<LocalResourceModel>) this.Resources)
        {
          MenuItem menuItem = this.CreateMenuItem(resourceModel.ResourceName, resourceModel.ResourceName, (ICommand) new ResourceListCommand.SetToResourceCommand(this, resourceModel));
          if (this.IsResourceInUse(resourceModel))
            menuItem.IsChecked = true;
          list.Add(menuItem);
        }
        return (IEnumerable) list;
      }
    }

    protected virtual IProperty TargetProperty
    {
      get
      {
        return (IProperty) this.targetProperty;
      }
    }

    protected PropertyReference TargetPropertyReference
    {
      get
      {
        return new PropertyReference(this.targetProperty);
      }
    }

    private IList<LocalResourceModel> Resources
    {
      get
      {
        List<LocalResourceModel> list = new List<LocalResourceModel>();
        SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
        ResourceManager resourceManager = this.DesignerContext.ResourceManager;
        foreach (DocumentCompositeNode entryNode in this.ResourcesInternal)
        {
          DocumentNode resourceEntryKey = ResourceNodeHelper.GetResourceEntryKey(entryNode);
          DocumentNode node = entryNode.Properties[DictionaryEntryNode.ValueProperty];
          if (resourceEntryKey != null && node != null)
          {
            LocalResourceModel resourceModel = new LocalResourceModel(resourceEntryKey, node.TargetType, node, (object) null);
            if (this.IsResourceCompatible(resourceModel))
              list.Add(resourceModel);
          }
        }
        return (IList<LocalResourceModel>) list;
      }
    }

    protected virtual IEnumerable<DocumentCompositeNode> ResourcesInternal
    {
      get
      {
        return this.DesignerContext.ResourceManager.GetResourcesInSelectionScope((ITypeId) this.TargetProperty.PropertyType, ResourceResolutionFlags.IncludeApplicationResources | ResourceResolutionFlags.UniqueKeysOnly);
      }
    }

    public ResourceListCommand(SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewModel)
    {
      this.targetProperty = viewModel.ProjectContext.ResolveProperty(targetProperty) as ReferenceStep;
    }

    public MenuItem CreateSingleInstance()
    {
      MenuItem menuItem1 = this.CreateMenuItem(StringTable.ResourceListCommandName, StringTable.ResourceListCommandName, (ICommand) null);
      menuItem1.IsEnabled = this.IsEnabled;
      foreach (MenuItem menuItem2 in this.Items)
        menuItem1.Items.Add((object) menuItem2);
      return menuItem1;
    }

    protected virtual bool IsResourceCompatible(LocalResourceModel resourceModel)
    {
      if (!PlatformTypeHelper.GetPropertyType(this.TargetProperty).IsAssignableFrom(resourceModel.ResourceType))
        return false;
      return this.DoesStyleOrTemplateApply((IPropertyId) this.TargetProperty, resourceModel.ResourceNode);
    }

    protected bool DoesStyleOrTemplateApply(IPropertyId styleOrTemplateProperty, DocumentNode value)
    {
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) this.Type))
      {
        IType templateTargetType1 = DocumentNodeUtilities.GetStyleOrTemplateTargetType(value);
        Type type = templateTargetType1 != null ? templateTargetType1.RuntimeType : (Type) null;
        Type c = this.TargetElement.ProjectContext.MetadataFactory.GetMetadata(this.Type.RuntimeType).GetStylePropertyTargetType(styleOrTemplateProperty);
        if (type == (Type) null)
          return true;
        if (StyleNode.BasedOnProperty.Equals((object) styleOrTemplateProperty))
        {
          StyleNode styleNode = this.TargetElement as StyleNode;
          if (styleNode != null)
          {
            IType templateTargetType2 = DocumentNodeUtilities.GetStyleOrTemplateTargetType(styleNode.DocumentNode);
            c = templateTargetType2 != null ? templateTargetType2.RuntimeType : (Type) null;
          }
        }
        if (c == (Type) null && ControlStylingOperations.DoesPropertyAffectRoot((IPropertyId) this.TargetProperty))
          c = this.Type.RuntimeType;
        if (c != (Type) null && !type.IsAssignableFrom(c))
          return false;
      }
      return true;
    }

    protected virtual bool IsResourceInUse(LocalResourceModel resourceModel)
    {
      DocumentNodePath valueAsDocumentNode = this.TargetElement.GetLocalValueAsDocumentNode(this.TargetPropertyReference);
      if (valueAsDocumentNode != null)
        return valueAsDocumentNode.Node == resourceModel.ResourceNode;
      return false;
    }

    protected virtual void SetToResource(LocalResourceModel resourceModel)
    {
      this.SetToResourceInternal(this.TargetPropertyReference, resourceModel);
    }

    protected void SetToResourceInternal(PropertyReference propertyReference, LocalResourceModel resourceModel)
    {
      using (SceneEditTransaction editTransaction = this.ViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertySetUndo, new object[1]
      {
        (object) this.TargetProperty.Name
      })))
      {
        IDocumentContext documentContext = this.ViewModel.Document.DocumentContext;
        DocumentNode keyNode = resourceModel.ResourceKey.Clone(documentContext);
        DocumentNode resourceExtensionNode = !(propertyReference[0] is DependencyPropertyReferenceStep) || !JoltHelper.TypeSupported((ITypeResolver) this.ViewModel.ProjectContext, PlatformTypes.DynamicResource) ? (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(documentContext, keyNode) : (DocumentNode) DocumentNodeUtilities.NewDynamicResourceNode(documentContext, keyNode);
        this.TargetElement.SetValue(propertyReference, (object) resourceExtensionNode);
        Microsoft.Expression.DesignSurface.Utility.ResourceHelper.EnsureReferencedResourcesAreReachable(resourceModel.ResourceNode, resourceExtensionNode);
        editTransaction.Commit();
      }
    }

    private class SetToResourceCommand : ICommand
    {
      private ResourceListCommand parentCommand;
      private LocalResourceModel resourceModel;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public SetToResourceCommand(ResourceListCommand parentCommand, LocalResourceModel resourceModel)
      {
        this.parentCommand = parentCommand;
        this.resourceModel = resourceModel;
      }

      public void Execute(object arg)
      {
        this.parentCommand.SetToResource(this.resourceModel);
      }

      public bool CanExecute(object arg)
      {
        return true;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditCopyOfStyleTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditCopyOfStyleTemplateCommand : ReplaceStyleTemplateCommand
  {
    private bool? useStyle;

    protected override string UndoString
    {
      get
      {
        return StringTable.UndoUnitEditCopyTemplate;
      }
    }

    private bool UseStyle
    {
      get
      {
        if (!this.useStyle.HasValue)
          return !(this.TargetElement is StyleNode);
        return this.useStyle.Value;
      }
    }

    protected override ReferenceStep TargetProperty
    {
      get
      {
        if (this.UseStyle)
          return this.SceneViewModel.ProjectContext.ResolveProperty(BaseFrameworkElement.StyleProperty) as ReferenceStep;
        return this.ActiveTemplateProperty;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          if (PlatformTypes.TextBox.IsAssignableFrom((ITypeId) this.TargetElement.Type) && !this.TargetElement.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
            return true;
          if (PlatformTypes.Control.IsAssignableFrom((ITypeId) this.TargetElement.Type) || PlatformTypes.Page.IsAssignableFrom((ITypeId) this.TargetElement.Type) || this.TargetElement is StyleNode)
          {
            Type runtimeType = this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.FrameworkTemplate).RuntimeType;
            object computedValue = this.TargetElement.GetComputedValue((IPropertyId) this.ActiveTemplateProperty);
            if (runtimeType != (Type) null && computedValue != null)
              return runtimeType.IsAssignableFrom(computedValue.GetType());
            return false;
          }
        }
        return false;
      }
    }

    public EditCopyOfStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewHost, viewModel, ControlElement.TemplateProperty)
    {
      this.useStyle = new bool?();
    }

    public override void Execute()
    {
      SceneElement targetElement = this.TargetElement;
      if (targetElement is StyleNode)
      {
        this.useStyle = new bool?(false);
      }
      else
      {
        IList<DocumentCompositeNode> auxillaryResources1;
        DocumentNode documentNode1 = this.ProvideCurrentStyle(targetElement, this.Type, new PropertyReference((ReferenceStep) targetElement.ProjectContext.ResolveProperty(BaseFrameworkElement.StyleProperty)), false, out auxillaryResources1);
        IList<DocumentCompositeNode> auxillaryResources2;
        DocumentNode other = this.ProvideCurrentTemplate(targetElement, new PropertyReference(this.ActiveTemplateProperty), out auxillaryResources2);
        bool flag = false;
        if ((other == null || other.DocumentRoot == null) && documentNode1 != null)
        {
          using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitEditCopyStyle, true))
          {
            if (auxillaryResources1 != null && auxillaryResources1.Count > 0)
            {
              ResourceSite resourceSite = new ResourceSite(targetElement.DocumentNode);
              resourceSite.EnsureResourceCollection();
              foreach (DocumentCompositeNode entryNode in (IEnumerable<DocumentCompositeNode>) auxillaryResources1)
              {
                DocumentNode resourceEntryKey = ResourceNodeHelper.GetResourceEntryKey(entryNode);
                DocumentNode documentNode2 = entryNode.Properties[DictionaryEntryNode.ValueProperty];
                if (resourceEntryKey != null && documentNode2 != null)
                {
                  DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) this.SceneViewModel.CreateSceneNode(PlatformTypes.DictionaryEntry);
                  dictionaryEntryNode.KeyNode = this.SceneViewModel.GetSceneNode(resourceEntryKey.Clone(this.SceneViewModel.Document.DocumentContext));
                  dictionaryEntryNode.Value = this.SceneViewModel.GetSceneNode(documentNode2.Clone(this.SceneViewModel.Document.DocumentContext));
                  resourceSite.ResourcesDictionary.Children.Add(dictionaryEntryNode.DocumentNode);
                }
              }
            }
            documentNode1 = documentNode1.Clone(this.SceneViewModel.Document.DocumentContext);
            DocumentCompositeNode documentCompositeNode = documentNode1 as DocumentCompositeNode;
            if (documentCompositeNode != null && documentCompositeNode.NameProperty != null && documentCompositeNode.Properties.Contains(documentCompositeNode.NameProperty))
              documentCompositeNode.ClearValue((IPropertyId) documentCompositeNode.NameProperty);
            targetElement.SetValue(this.TargetPropertyReference, (object) documentNode1);
            DocumentNodePath valueAsDocumentNode = targetElement.GetLocalValueAsDocumentNode(this.TargetPropertyReference);
            documentNode1 = valueAsDocumentNode != null ? valueAsDocumentNode.Node : (DocumentNode) null;
            editTransaction.Update();
            object computedValue = targetElement.GetComputedValue(new PropertyReference(this.ActiveTemplateProperty));
            if (computedValue != null)
            {
              DocumentNodePath correspondingNodePath = this.SceneView.GetCorrespondingNodePath(this.SceneViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(computedValue), true);
              if (correspondingNodePath != null)
              {
                flag = valueAsDocumentNode.IsAncestorOf(correspondingNodePath);
                other = correspondingNodePath != null ? correspondingNodePath.Node : other;
              }
            }
            editTransaction.Cancel();
          }
        }
        this.useStyle = new bool?(flag || other != null && documentNode1 != null && documentNode1.IsAncestorOf(other));
      }
      base.Execute();
      this.useStyle = new bool?();
    }

    protected override DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources)
    {
      if (this.UseStyle)
        return this.ProvideCurrentStyle(this.TargetElement, this.Type, this.TargetPropertyReference, false, out auxillaryResources);
      return this.ProvideCurrentTemplate(this.TargetElement, this.TargetPropertyReference, out auxillaryResources);
    }

    protected override DocumentNodePath ProvideEditingContainer(SceneElement targetElement, PropertyReference targetProperty, DocumentNode resourceNode)
    {
      if (this.UseStyle)
      {
        if (this.SceneView.IsEditable && targetElement.IsViewObjectValid && (this.SceneView.GetViewState((SceneNode) targetElement) & this.RequiredSelectionViewState) == this.RequiredSelectionViewState)
        {
          Type runtimeType = this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.FrameworkTemplate).RuntimeType;
          return this.SceneView.GetCorrespondingNodePath(this.SceneViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(targetElement.GetComputedValue((IPropertyId) this.ProvideTemplateProperty(SingleTargetCommandBase.GetTypeOfElement(targetElement)))), true);
        }
        SceneNode sceneNode = targetElement.GetLocalValueAsSceneNode(targetProperty);
        if (sceneNode == null && resourceNode != null && resourceNode.IsInDocument)
        {
          SceneViewModel viewModel = SceneViewModel.GetViewModel(this.ViewHost, resourceNode.DocumentRoot, false);
          if (viewModel != null)
            sceneNode = viewModel.GetSceneNode(resourceNode);
        }
        if (sceneNode != null)
          return sceneNode.GetLocalValueAsDocumentNode((IPropertyId) this.ProvideTemplateProperty(SingleTargetCommandBase.GetTypeOfElement(targetElement)));
      }
      return targetElement.GetLocalValueAsDocumentNode(targetProperty);
    }
  }
}

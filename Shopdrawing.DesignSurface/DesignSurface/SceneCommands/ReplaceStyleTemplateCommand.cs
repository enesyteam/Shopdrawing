// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ReplaceStyleTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class ReplaceStyleTemplateCommand : ControlStylingCommandBase
  {
    private bool showUI = true;

    public bool ShowUI
    {
      get
      {
        return this.showUI;
      }
      set
      {
        this.showUI = value;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled || !this.HasValidTarget || !this.TargetElement.GetProperties().Contains((IProperty) this.TargetProperty) || (this.TargetProperty.Equals((object) BaseFrameworkElement.StyleProperty) && !(this.TargetElement is BaseFrameworkElement) || this.TargetProperty == this.ActiveTemplateProperty && this.TargetElement is FrameworkTemplateElement))
          return false;
        if (!(this.TargetElement is FrameworkTemplateElement))
          return true;
        if (this.TargetElement == this.SceneViewModel.ActiveStoryboardContainer)
          return this.SceneViewModel.ActiveVisualTrigger != null;
        return false;
      }
    }

    protected abstract string UndoString { get; }

    public ReplaceStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : base(viewHost, viewModel, targetProperty)
    {
    }

    public ReplaceStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty, bool useRootTemplateProperty)
      : base(viewHost, viewModel, targetProperty, useRootTemplateProperty)
    {
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "ShowUI" && propertyValue is bool)
        this.ShowUI = (bool) propertyValue;
      base.SetProperty(propertyName, propertyValue);
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "ShowUI")
        return (object) (bool) (this.ShowUI ? true : false);
      return base.GetProperty(propertyName);
    }

    protected abstract DocumentNode ProvideValue(out IList<DocumentCompositeNode> auxillaryResources);

    public override void Execute()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.EditStyleOrTemplate);
      if (this.IsEnabled)
      {
        SceneElement targetElement = this.TargetElement;
        IDocumentContext documentContext = this.SceneViewModel.Document.DocumentContext;
        Type propertyTargetType = this.SceneViewModel.Document.ProjectContext.MetadataFactory.GetMetadata(this.Type.RuntimeType).GetStylePropertyTargetType((IPropertyId) this.TargetProperty);
        CreateResourceModel.ContextFlags contextFlags = !PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) this.TargetProperty.PropertyType) ? (this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsImplicitStyles) ? CreateResourceModel.ContextFlags.CanApplyAutomatically : CreateResourceModel.ContextFlags.None) : CreateResourceModel.ContextFlags.None;
        CreateResourceModel model = new CreateResourceModel(this.SceneViewModel, this.DesignerContext.ResourceManager, PlatformTypeHelper.GetPropertyType((IProperty) this.TargetProperty), propertyTargetType, this.TargetProperty.Name, (SceneElement) null, (SceneNode) (targetElement as BaseFrameworkElement), contextFlags);
        IList<DocumentCompositeNode> auxillaryResources;
        DocumentNode node1 = this.ProvideValue(out auxillaryResources);
        if (node1 != null)
        {
          IPropertyId targetTypeProperty = this.GetTargetTypeProperty((ITypeId) this.TargetProperty.PropertyType);
          DocumentCompositeNode documentCompositeNode = node1 as DocumentCompositeNode;
          if (targetTypeProperty != null && documentCompositeNode != null)
          {
            IType valueAsType = DocumentPrimitiveNode.GetValueAsType(documentCompositeNode.Properties[targetTypeProperty]);
            if (valueAsType != null && valueAsType.RuntimeType != (Type) null)
              model.TargetType = valueAsType.RuntimeType;
          }
          else
            model.TargetType = (Type) null;
          ReplaceStyleTemplateCommand.ExtraReferences references = new ReplaceStyleTemplateCommand.ExtraReferences();
          this.CollectExtraReferences(node1, references);
          if (auxillaryResources != null)
          {
            foreach (DocumentNode node2 in (IEnumerable<DocumentCompositeNode>) auxillaryResources)
              this.CollectExtraReferences(node2, references);
          }
          if (references.UnresolvedTypes.Count > 0)
          {
            string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CopyStyleTemplateTypesNotInDocumentMessage, new object[2]
            {
              (object) this.TargetProperty.Name,
              (object) SceneView.GetUnresolvedTypesList(references.UnresolvedTypes)
            });
            if (!this.ShowUI)
              return;
            this.DesignerContext.MessageDisplayService.ShowError(message);
            return;
          }
          if (this.ShowUI)
          {
            bool? nullable = new CreateResourceDialog(this.DesignerContext, model).ShowDialog();
            if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
              return;
          }
          bool flag = model.CurrentResourceSite != null;
          if (!flag || targetElement.DocumentNode.DocumentRoot == null)
            return;
          SceneViewModel viewModel = this.SceneViewModel.GetViewModel(model.CurrentResourceSite.HostNode.DocumentRoot, false);
          if (viewModel == null || !PlatformTypes.PlatformsCompatible(node1.PlatformMetadata, viewModel.ProjectContext.PlatformMetadata) || !this.AddReferences(viewModel.ProjectContext, references, model.KeyString))
            return;
          using (SceneEditTransaction editTransaction1 = this.SceneViewModel.CreateEditTransaction(this.UndoString))
          {
            DocumentNode documentNode1 = (DocumentNode) null;
            DocumentCompositeNode resource;
            using (SceneEditTransaction editTransaction2 = viewModel.CreateEditTransaction(this.UndoString))
            {
              DocumentNode newResourceNode;
              try
              {
                newResourceNode = node1.Clone(viewModel.Document.DocumentContext);
              }
              catch
              {
                editTransaction2.Cancel();
                editTransaction1.Cancel();
                this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CommandFailedDialogMessage, new object[1]
                {
                  (object) this.UndoString
                }));
                return;
              }
              newResourceNode.Name = (string) null;
              bool useStaticResource = !(this.TargetProperty is DependencyPropertyReferenceStep) || !JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.DynamicResource);
              int index = useStaticResource ? model.IndexInResourceSite(targetElement.DocumentNode) : -1;
              resource = model.CreateResource(newResourceNode, targetTypeProperty, index);
              flag = resource != null;
              if (flag)
              {
                documentNode1 = model.CreateResourceReference(this.SceneViewModel.Document.DocumentContext, resource, useStaticResource);
                flag = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.CopyResourcesToNewResourceSite(auxillaryResources, viewModel, model.CurrentResourceSite.HostNode, resource, model.IndexInResourceSite((DocumentNode) resource));
              }
              if (flag)
              {
                editTransaction2.Commit();
                if (this.SceneViewModel == viewModel)
                {
                  editTransaction1.Update();
                  this.DesignerContext.ViewUpdateManager.UpdateRelatedViews(this.SceneViewModel.Document, false);
                }
                this.DesignerContext.ViewUpdateManager.RefreshActiveViewApplicationResources();
              }
              else
                editTransaction2.Cancel();
            }
            if (flag && resource != null)
            {
              DocumentNode documentNode2 = resource.Properties[DictionaryEntryNode.ValueProperty];
              DocumentNodePath documentNodePath;
              if (targetElement.IsAttached)
              {
                if (documentNode1 != null)
                  targetElement.SetValue(this.TargetPropertyReference, (object) documentNode1);
                else
                  targetElement.ClearValue(this.TargetPropertyReference);
                this.SceneView.CandidateEditingContainer = targetElement.DocumentNodePath;
                editTransaction1.Update();
                this.SceneView.CandidateEditingContainer = (DocumentNodePath) null;
                documentNodePath = this.ProvideEditingContainer(targetElement, this.TargetPropertyReference, documentNode2);
              }
              else
                documentNodePath = (DocumentNodePath) null;
              if (this.SceneView.IsValid)
              {
                if (documentNodePath != null && documentNodePath.Node != null && (!DocumentNodeUtilities.IsDynamicResource(documentNodePath.Node) && !DocumentNodeUtilities.IsStaticResource(documentNodePath.Node)))
                {
                  DocumentNode node2 = documentNodePath.Node;
                  bool preferInPlaceEdit = ControlStylingOperations.CanEditInPlace(targetElement, this.TargetProperty, documentNodePath);
                  ControlStylingOperations.SetActiveEditingContainer(targetElement, this.TargetProperty, node2, documentNodePath, preferInPlaceEdit, editTransaction1);
                }
                else
                  ControlStylingOperations.SetActiveEditingContainer(targetElement, this.TargetProperty, documentNode2, (DocumentNodePath) null, false, editTransaction1);
              }
              editTransaction1.Commit();
            }
            else
              editTransaction1.Cancel();
          }
        }
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.EditStyleOrTemplate);
    }

    private bool AddReferences(IProjectContext projectContext, ReplaceStyleTemplateCommand.ExtraReferences references, string resourceKey)
    {
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(projectContext);
      if (projectXamlContext == null)
        return false;
      if (references.Assemblies.Count > 0)
      {
        ISolution currentSolution = this.DesignerContext.ProjectManager.CurrentSolution;
        if (currentSolution != null)
        {
          List<IProject> list1 = new List<IProject>();
          List<IProjectContext> list2 = new List<IProjectContext>();
          foreach (IProject project in currentSolution.Projects)
          {
            ProjectXamlContext projectContext1 = ProjectXamlContext.GetProjectContext(project);
            if (projectContext1 != null && projectContext1.ProjectAssembly != null && references.Assemblies.Contains(projectContext1.ProjectAssembly))
            {
              list1.Add(project);
              list2.Add((IProjectContext) projectContext1);
            }
          }
          for (int index = 0; index < list1.Count; ++index)
          {
            IProject source = list1[index];
            IProjectContext projectContext1 = list2[index];
            if (projectXamlContext != projectContext1 && ProjectHelper.DoesProjectReferenceHierarchyContainTarget(source, (IProjectContext) projectXamlContext))
            {
              this.DesignerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DocumentRefersToUnresolvedTypesHeader, new object[1]
              {
                (object) resourceKey
              }));
              return false;
            }
          }
        }
        foreach (IAssembly assembly in (IEnumerable<IAssembly>) references.Assemblies)
          projectXamlContext.EnsureAssemblyReferenceMatches(this.SceneViewModel.ProjectContext, assembly);
      }
      foreach (string assemblyPath in (IEnumerable<string>) references.PlatformAssemblies)
        projectXamlContext.EnsureAssemblyReferenced(assemblyPath);
      this.SceneViewModel.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      return true;
    }

    protected virtual DocumentNodePath ProvideEditingContainer(SceneElement targetElement, PropertyReference targetProperty, DocumentNode resourceNode)
    {
      return targetElement.GetLocalValueAsDocumentNode(targetProperty);
    }

    protected DocumentNode ProvideCurrentStyle(SceneElement targetElement, IType targetType, PropertyReference targetPropertyReference, bool allowDefaultStyle, out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      DocumentNode currentStyle = (DocumentNode) null;
      bool isThemeStyle = false;
      IType targetType1 = targetType;
      if (!this.TargetProperty.Equals((object) BaseFrameworkElement.StyleProperty))
      {
        IDocumentContext documentContext = this.SceneViewModel.Document.DocumentContext;
        Type propertyTargetType = this.SceneViewModel.Document.ProjectContext.MetadataFactory.GetMetadata(this.Type.RuntimeType).GetStylePropertyTargetType((IPropertyId) this.TargetProperty);
        targetType1 = propertyTargetType == (Type) null ? (IType) null : this.SceneViewModel.ProjectContext.GetType(propertyTargetType);
      }
      if (targetElement.IsSet(targetPropertyReference) == PropertyState.Set)
      {
        DocumentNodePath valueAsDocumentNode = targetElement.GetLocalValueAsDocumentNode(targetPropertyReference);
        if (valueAsDocumentNode != null && valueAsDocumentNode.Node is DocumentCompositeNode && PlatformTypes.Style.IsAssignableFrom((ITypeId) valueAsDocumentNode.Node.Type) && (!StyleNode.IsDefaultValue(valueAsDocumentNode.Node) || allowDefaultStyle))
        {
          StyleNode styleNode = targetElement.ClonePropertyValueAsSceneNode(targetPropertyReference) as StyleNode;
          if (styleNode != null)
          {
            currentStyle = styleNode.DocumentNode;
            auxillaryResources = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindReferencedResources(valueAsDocumentNode.Node);
          }
        }
      }
      if (currentStyle == null)
      {
        object obj = this.ResolveCurrentStyle(targetElement, targetPropertyReference, allowDefaultStyle);
        if (obj != null)
        {
          DocumentNode correspondingDocumentNode = this.SceneView.GetCorrespondingDocumentNode(this.SceneViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(obj), true);
          if (correspondingDocumentNode != null && PlatformTypes.Style.IsAssignableFrom((ITypeId) correspondingDocumentNode.Type))
            currentStyle = correspondingDocumentNode;
          else if (obj is Style)
          {
            StyleNode styleNode = (StyleNode) this.SceneViewModel.CreateSceneNode(obj);
            if (targetType1 != null)
              styleNode.StyleTargetTypeId = targetType1;
            currentStyle = styleNode.DocumentNode;
          }
        }
        if (currentStyle != null && !allowDefaultStyle && StyleNode.IsDefaultValue(currentStyle))
          currentStyle = (DocumentNode) null;
      }
      if (currentStyle == null)
      {
        object defaultStyleKey = this.GetDefaultStyleKey(targetElement, (ITypeId) targetType1, (IPropertyId) this.TargetProperty);
        if (defaultStyleKey != null)
          this.ResolveDefaultStyle(targetElement, defaultStyleKey, allowDefaultStyle, out currentStyle, out isThemeStyle, out auxillaryResources);
      }
      if (currentStyle != null && !allowDefaultStyle)
      {
        List<DocumentCompositeNode> list = new List<DocumentCompositeNode>();
        DocumentCompositeNode node = currentStyle as DocumentCompositeNode;
        IProperty property = this.SceneViewModel.ProjectContext.ResolveProperty(StyleNode.BasedOnProperty);
        while (node != null && property != null && (node.Type.Equals((object) PlatformTypes.Style) && node.Properties.Count == 2) && (node.Properties[(IPropertyId) property] != null && node.Properties[StyleNode.TargetTypeProperty] != null))
        {
          node = node.Properties[StyleNode.BasedOnProperty] as DocumentCompositeNode;
          if (node != null)
          {
            if (DocumentNodeUtilities.IsDynamicResource((DocumentNode) node) || DocumentNodeUtilities.IsStaticResource((DocumentNode) node))
            {
              DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node);
              if (resourceKey != null && auxillaryResources != null)
              {
                foreach (DocumentCompositeNode entryNode in (IEnumerable<DocumentCompositeNode>) auxillaryResources)
                {
                  if (resourceKey.Equals(ResourceNodeHelper.GetResourceEntryKey(entryNode)))
                  {
                    node = entryNode.Properties[DictionaryEntryNode.ValueProperty] as DocumentCompositeNode;
                    break;
                  }
                }
              }
            }
            if (PlatformTypes.Style.IsAssignableFrom((ITypeId) node.Type))
            {
              currentStyle = (DocumentNode) node;
              list.Add(node);
            }
          }
        }
        if (auxillaryResources != null)
        {
          foreach (DocumentNode documentNode in list)
          {
            DocumentCompositeNode parent = documentNode.Parent;
            if (parent != null && parent.Type.Equals((object) PlatformTypes.DictionaryEntry))
              auxillaryResources.Remove(parent);
          }
        }
        if (currentStyle != null && (list.Count > 0 || currentStyle.DocumentRoot != null))
          currentStyle = currentStyle.Clone(targetElement.DocumentContext);
        if (auxillaryResources != null)
        {
          for (int index = 0; index < auxillaryResources.Count; ++index)
          {
            if (auxillaryResources[index].DocumentRoot != null)
              auxillaryResources[index] = (DocumentCompositeNode) auxillaryResources[index].Clone(targetElement.DocumentContext);
          }
        }
        if (isThemeStyle)
        {
          ReplaceStyleTemplateCommand.StripFormatting(currentStyle);
          if (auxillaryResources != null)
          {
            for (int index = 0; index < auxillaryResources.Count; ++index)
              ReplaceStyleTemplateCommand.StripFormatting((DocumentNode) auxillaryResources[index]);
          }
        }
        DocumentCompositeNode documentCompositeNode = currentStyle as DocumentCompositeNode;
        if (documentCompositeNode != null && targetType1 != null)
          documentCompositeNode.Properties[StyleNode.TargetTypeProperty] = (DocumentNode) documentCompositeNode.Context.CreateNode(PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) targetType1));
        if (targetType1 != null)
          ReplaceStyleTemplateCommand.ReplaceTemplateTargetType(currentStyle, auxillaryResources, targetType1);
      }
      return currentStyle;
    }

    private void ResolveDefaultStyle(SceneElement targetElement, object defaultStyleKey, bool allowDefaultStyle, out DocumentNode currentStyle, out bool isThemeStyle, out IList<DocumentCompositeNode> auxillaryResources)
    {
      IProjectContext projectContext1 = this.SceneViewModel.ProjectContext;
      ThemeContentProvider themeContentProvider = this.DesignerContext.ThemeContentProvider;
      currentStyle = (DocumentNode) null;
      isThemeStyle = false;
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      if (defaultStyleKey == null)
        return;
      IAssembly runtimeAssembly = targetElement.Type.RuntimeAssembly;
      IAssembly targetAssembly = PlatformTypeHelper.GetTargetAssembly(targetElement.Type);
      Type type1 = defaultStyleKey as Type;
      if (type1 != (Type) null)
      {
        ITypeId typeId = (ITypeId) projectContext1.GetType(type1);
        if (typeId != null)
        {
          IType type2 = projectContext1.ResolveType(typeId);
          runtimeAssembly = type2.RuntimeAssembly;
          targetAssembly = PlatformTypeHelper.GetTargetAssembly(type2);
        }
      }
      IAssembly designAssembly = projectContext1.GetDesignAssembly(runtimeAssembly);
      if (designAssembly != null)
      {
        currentStyle = themeContentProvider.GetThemeResourceFromAssembly(projectContext1, designAssembly, designAssembly, defaultStyleKey, out auxillaryResources);
        if (currentStyle != null)
          return;
      }
      if (!PlatformTypes.IsPlatformType((ITypeId) this.Type))
      {
        foreach (IProject project in this.DesignerContext.ProjectManager.CurrentSolution.Projects)
        {
          IProjectContext projectContext2 = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
          if (projectContext2 != null && runtimeAssembly.Equals((object) projectContext2.ProjectAssembly))
          {
            currentStyle = themeContentProvider.GetThemeResourceFromProject(project, defaultStyleKey, out auxillaryResources);
            if (currentStyle != null)
              return;
          }
        }
      }
      else if (!allowDefaultStyle || projectContext1.PlatformMetadata.TargetFramework.Identifier == ".NETFramework" && projectContext1.PlatformMetadata.TargetFramework.Version < projectContext1.PlatformMetadata.RuntimeFramework.Version)
      {
        currentStyle = themeContentProvider.GetThemeResourceFromPlatform(projectContext1.Platform, defaultStyleKey, out auxillaryResources);
        if (currentStyle != null)
        {
          isThemeStyle = true;
          return;
        }
      }
      if (!projectContext1.IsCapabilitySet(PlatformCapability.IsWpf))
      {
        currentStyle = themeContentProvider.GetThemeResourceFromAssembly(projectContext1, runtimeAssembly, targetAssembly, defaultStyleKey, out auxillaryResources);
        isThemeStyle = currentStyle != null;
      }
      else
      {
        object resource = this.SceneViewModel.FindResource(defaultStyleKey);
        if (resource == null)
          return;
        if (projectContext1 != null && projectContext1.IsCapabilitySet(PlatformCapability.VsmInToolkit) && (projectContext1.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager) && projectContext1.PlatformMetadata.IsNullType((ITypeId) projectContext1.ResolveType(ProjectNeutralTypes.VisualStateManager))))
        {
          IAssembly usingAssemblyName = projectContext1.Platform.Metadata.GetPlatformAssemblyUsingAssemblyName(targetElement.Type.RuntimeAssembly);
          if (usingAssemblyName == null || !AssemblyHelper.IsPlatformAssembly(usingAssemblyName))
          {
            IDocumentContext documentContext = (IDocumentContext) new DocumentContext((IProjectContext) new ToolkitProjectContext(projectContext1), ((DocumentContext) this.SceneViewModel.Document.DocumentContext).DocumentLocator);
            DocumentNode node = documentContext.CreateNode(resource.GetType(), resource);
            if (ProjectAttributeHelper.GetDefaultStateRecords(this.Type, (ITypeResolver) (documentContext.TypeResolver as ProjectContext)).Count > 0 || (DocumentCompositeNode) node.FindFirst(new Predicate<DocumentNode>(this.SelectVisualStateGroupPredicate)) != null)
              ToolkitHelper.AddToolkitReferenceIfNeeded((ITypeResolver) projectContext1, this.DesignerContext.ViewUpdateManager);
          }
        }
        SceneNode sceneNode = this.SceneViewModel.CreateSceneNode(resource);
        if (!PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type))
          return;
        currentStyle = sceneNode.DocumentNode;
      }
    }

    private bool SelectVisualStateGroupPredicate(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      return documentCompositeNode != null && ProjectNeutralTypes.VisualStateGroup.IsAssignableFrom((ITypeId) documentCompositeNode.Type);
    }

    private object GetDefaultStyleKey(SceneElement targetElement, ITypeId styleTargetType, IPropertyId targetProperty)
    {
      IInstanceBuilder builder = this.SceneView.InstanceBuilderContext.InstanceBuilderFactory.GetBuilder(this.Type.RuntimeType);
      if (BaseFrameworkElement.StyleProperty.Equals((object) targetProperty) && builder.ReplacementType == (Type) null)
      {
        IViewObject viewObject = targetElement.ViewObject;
        if (viewObject == null)
          return (object) null;
        return viewObject.DefaultStyleKey;
      }
      if (!this.SceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && PlatformTypes.ComboBoxItem.Equals((object) styleTargetType))
        styleTargetType = (ITypeId) this.SceneViewModel.ProjectContext.ResolveType(PlatformTypes.ListBoxItem);
      if (styleTargetType != null)
        return (object) this.SceneViewModel.ProjectContext.ResolveType(styleTargetType).RuntimeType;
      return (object) null;
    }

    private static void ReplaceTemplateTargetType(DocumentNode root, IList<DocumentCompositeNode> resources, IType targetType)
    {
      DocumentCompositeNode node = root as DocumentCompositeNode;
      if (node == null)
        return;
      DocumentCompositeNode parent = root.Parent;
      if (parent != null && PlatformTypes.Setter.IsAssignableFrom((ITypeId) parent.Type) && (parent.TypeResolver.ResolveProperty(SetterSceneNode.TargetNameProperty) == null || parent.Properties[SetterSceneNode.TargetNameProperty] == null) && (ControlStylingOperations.DoesPropertyAffectRoot((IPropertyId) root.GetValueProperty()) && parent.Properties[SetterSceneNode.ValueProperty] == root))
      {
        if (PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) root.Type))
          node.Properties[ControlTemplateElement.TargetTypeProperty] = (DocumentNode) node.Context.CreateNode(PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) targetType));
        else if (DocumentNodeUtilities.IsDynamicResource(root) || DocumentNodeUtilities.IsStaticResource(root))
        {
          DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(node);
          if (resourceKey != null && resources != null)
          {
            foreach (DocumentCompositeNode entryNode in (IEnumerable<DocumentCompositeNode>) resources)
            {
              if (resourceKey.Equals(ResourceNodeHelper.GetResourceEntryKey(entryNode)))
              {
                DocumentCompositeNode documentCompositeNode = entryNode.Properties[DictionaryEntryNode.ValueProperty] as DocumentCompositeNode;
                if (documentCompositeNode != null)
                {
                  if (PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
                  {
                    documentCompositeNode.Properties[ControlTemplateElement.TargetTypeProperty] = (DocumentNode) documentCompositeNode.Context.CreateNode(PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) targetType));
                    break;
                  }
                  break;
                }
                break;
              }
            }
          }
        }
      }
      if (parent != null && DocumentNodeUtilities.IsStyleOrTemplate(root.Type))
        return;
      foreach (DocumentNode root1 in node.ChildNodes)
        ReplaceStyleTemplateCommand.ReplaceTemplateTargetType(root1, resources, targetType);
    }

    private void CollectExtraReferences(DocumentNode node, ReplaceStyleTemplateCommand.ExtraReferences references)
    {
      if (node.Parent != null && node.IsProperty)
        ReplaceStyleTemplateCommand.EnsureMember((IMember) node.SitePropertyKey, references);
      ReplaceStyleTemplateCommand.EnsureType(node.Type, references);
      IMember valueAsMember = DocumentPrimitiveNode.GetValueAsMember(node);
      if (valueAsMember != null)
        ReplaceStyleTemplateCommand.EnsureMember(valueAsMember, references);
      foreach (DocumentNode node1 in node.ChildNodes)
        this.CollectExtraReferences(node1, references);
    }

    private static void EnsureMember(IMember member, ReplaceStyleTemplateCommand.ExtraReferences references)
    {
      IProperty property = member as IProperty;
      if (property != null)
        ReplaceStyleTemplateCommand.EnsureType(property.PropertyType, references);
      IType type = member as IType;
      if (type != null)
        ReplaceStyleTemplateCommand.EnsureType(type, references);
      else
        ReplaceStyleTemplateCommand.EnsureType(member.DeclaringType, references);
    }

    private static void EnsureType(IType type, ReplaceStyleTemplateCommand.ExtraReferences references)
    {
      if (!type.IsResolvable)
      {
        references.AddUndesolvedType(type);
      }
      else
      {
        if (type.RuntimeAssembly.IsLoaded && ((IPlatformTypes) type.PlatformMetadata).IsDesignToolAssembly(type.RuntimeAssembly))
          return;
        if (PlatformTypes.IsPlatformType((ITypeId) type))
        {
          IAssembly targetAssembly = PlatformTypeHelper.GetTargetAssembly(type);
          if (targetAssembly == null)
            return;
          string name = targetAssembly.Name;
          if (string.IsNullOrEmpty(name))
            return;
          references.AddPlatformAssembly(name);
        }
        else
          references.AddAssembly(type.RuntimeAssembly);
      }
    }

    private static void StripFormatting(DocumentNode node)
    {
      node.SourceContext = (INodeSourceContext) null;
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        foreach (IPropertyId propertyKey in (IEnumerable<IProperty>) documentCompositeNode.Properties.Keys)
          documentCompositeNode.ClearContainerContext(propertyKey);
      }
      foreach (DocumentNode node1 in node.ChildNodes)
        ReplaceStyleTemplateCommand.StripFormatting(node1);
    }

    protected DocumentNode ProvideCurrentTemplate(SceneElement targetElement, PropertyReference targetPropertyReference, out IList<DocumentCompositeNode> auxillaryResources)
    {
      IPlatform platform = this.SceneViewModel.ProjectContext.Platform;
      FrameworkTemplateElement frameworkTemplateElement1 = (FrameworkTemplateElement) null;
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      if (targetElement.IsSet(targetPropertyReference) == PropertyState.Set)
      {
        FrameworkTemplateElement frameworkTemplateElement2 = targetElement.GetLocalValueAsSceneNode(targetPropertyReference) as FrameworkTemplateElement;
        if (frameworkTemplateElement2 != null)
        {
          frameworkTemplateElement1 = targetElement.ClonePropertyValueAsSceneNode(targetPropertyReference) as FrameworkTemplateElement;
          if (frameworkTemplateElement1 != null)
            auxillaryResources = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindReferencedResources(frameworkTemplateElement2.DocumentNode);
        }
      }
      if (frameworkTemplateElement1 == null)
      {
        object computedValue = targetElement.GetComputedValue(targetPropertyReference);
        DocumentNode root = computedValue == null ? (DocumentNode) null : this.SceneView.GetCorrespondingDocumentNode(platform.ViewObjectFactory.Instantiate(computedValue), true);
        IPropertyId targetProperty = (IPropertyId) targetElement.Platform.Metadata.ResolveProperty(BaseFrameworkElement.StyleProperty);
        DocumentCompositeNode documentCompositeNode1 = targetElement.DocumentNode as DocumentCompositeNode;
        if (!targetElement.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && documentCompositeNode1 != null && (root == null && targetPropertyReference.ReferenceSteps.Count == 1))
        {
          ITypeId styleTargetType = (ITypeId) targetElement.Type;
          DocumentNode currentStyle = (DocumentNode) null;
          ReferenceStep referenceStep = targetPropertyReference.ReferenceSteps[0];
          object defaultStyleKey = this.GetDefaultStyleKey(targetElement, styleTargetType, targetProperty);
          if (defaultStyleKey != null)
          {
            bool isThemeStyle;
            IList<DocumentCompositeNode> auxillaryResources1;
            this.ResolveDefaultStyle(targetElement, defaultStyleKey, true, out currentStyle, out isThemeStyle, out auxillaryResources1);
          }
          DocumentCompositeNode documentCompositeNode2 = currentStyle as DocumentCompositeNode;
          if (documentCompositeNode2 != null)
          {
            DocumentCompositeNode documentCompositeNode3 = documentCompositeNode2.Properties[StyleNode.SettersProperty] as DocumentCompositeNode;
            if (documentCompositeNode3 != null)
            {
              foreach (DocumentNode documentNode1 in (IEnumerable<DocumentNode>) documentCompositeNode3.Children)
              {
                DocumentCompositeNode documentCompositeNode4 = documentNode1 as DocumentCompositeNode;
                if (documentCompositeNode4 != null)
                {
                  IMemberId memberId = (IMemberId) DocumentPrimitiveNode.GetValueAsMember(documentCompositeNode4.Properties[SetterSceneNode.PropertyProperty]);
                  DocumentNode documentNode2 = documentCompositeNode4.Properties[SetterSceneNode.ValueProperty];
                  if (memberId != null && documentNode2 != null && referenceStep.Equals((object) memberId))
                  {
                    root = documentNode2;
                    break;
                  }
                }
              }
            }
          }
        }
        if (root != null)
        {
          frameworkTemplateElement1 = this.SceneViewModel.GetSceneNode(root.Clone(this.SceneViewModel.Document.DocumentContext)) as FrameworkTemplateElement;
          auxillaryResources = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindReferencedResources(root);
        }
        else
          frameworkTemplateElement1 = this.SceneViewModel.CreateSceneNode(computedValue) as FrameworkTemplateElement;
      }
      if (frameworkTemplateElement1 == null)
        return (DocumentNode) null;
      return frameworkTemplateElement1.DocumentNode;
    }

    protected object ResolveCurrentStyle(SceneElement targetElement, PropertyReference propertyReference, bool allowDefaultStyle)
    {
      ReferenceStep referenceStep = propertyReference[0];
      object computedValue = targetElement.GetComputedValue(propertyReference);
      StyleNode styleNode = targetElement.GetLocalOrDefaultValue(propertyReference) as StyleNode;
      bool flag = computedValue == null || styleNode != null && styleNode.IsDefaultStyle;
      if (!allowDefaultStyle && flag && referenceStep.Equals((object) BaseFrameworkElement.StyleProperty))
        return (object) null;
      if (computedValue != null)
      {
        Style style = computedValue as Style;
        if (style != null)
        {
          foreach (SetterBase setterBase in (Collection<SetterBase>) style.Setters)
          {
            Setter setter = setterBase as Setter;
            FrameworkTemplate frameworkTemplate;
            if (setter != null && (frameworkTemplate = setter.Value as FrameworkTemplate) != null && frameworkTemplate.VisualTree != null)
              return (object) null;
          }
        }
      }
      return computedValue;
    }

    private class ExtraReferences
    {
      private Dictionary<IAssembly, bool> assemblies = new Dictionary<IAssembly, bool>();
      private Dictionary<string, bool> platformAssemblies = new Dictionary<string, bool>();
      private Dictionary<IType, bool> unresolvedTypes = new Dictionary<IType, bool>();

      public ICollection<IAssembly> Assemblies
      {
        get
        {
          return (ICollection<IAssembly>) this.assemblies.Keys;
        }
      }

      public ICollection<string> PlatformAssemblies
      {
        get
        {
          return (ICollection<string>) this.platformAssemblies.Keys;
        }
      }

      public ICollection<IType> UnresolvedTypes
      {
        get
        {
          return (ICollection<IType>) this.unresolvedTypes.Keys;
        }
      }

      public void AddAssembly(IAssembly assembly)
      {
        this.assemblies[assembly] = true;
      }

      public void AddPlatformAssembly(string assemblyName)
      {
        this.platformAssemblies[assemblyName] = true;
      }

      public void AddUndesolvedType(IType type)
      {
        this.unresolvedTypes[type] = true;
      }
    }
  }
}

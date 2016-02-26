// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.BindingEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public class BindingEditor
  {
    private SceneViewModel viewModel;

    public BindingEditor(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public static bool CanBindToSchemaNode(SceneNode targetElement, IPropertyId targetPropertyId, DataSchemaNode schemaNode)
    {
      ReferenceStep referenceStep = targetElement.ProjectContext.ResolveProperty(targetPropertyId) as ReferenceStep;
      if (!BindingPropertyHelper.IsPropertyBindable(targetElement, referenceStep))
        return false;
      IType dataType = schemaNode.ResolveType((ITypeResolver) targetElement.ProjectContext);
      if (dataType == null)
        return true;
      return BindingPropertyHelper.GetPropertyCompatibility((IProperty) referenceStep, dataType, (ITypeResolver) targetElement.ProjectContext) != BindingPropertyCompatibility.None;
    }

    public static bool IsDataSource(DocumentNode possibleDataSource)
    {
      bool flag = false;
      if (typeof (DataSourceProvider).IsAssignableFrom(possibleDataSource.TargetType))
      {
        flag = true;
      }
      else
      {
        DocumentCompositeNode documentCompositeNode;
        if ((documentCompositeNode = possibleDataSource as DocumentCompositeNode) != null)
        {
          DocumentPrimitiveNode documentPrimitiveNode = documentCompositeNode.Properties[DesignTimeProperties.IsDataSourceProperty] as DocumentPrimitiveNode;
          if (documentPrimitiveNode != null && documentPrimitiveNode.TargetType == typeof (bool))
            flag = documentPrimitiveNode.GetValue<bool>();
        }
      }
      return flag;
    }

    public static IEnumerable<SceneNode> DataSourceItemEnumerator(SceneNode pivot)
    {
      BaseFrameworkElement baseFrameworkElement;
      ResourceDictionaryNode resourceDictionaryNode;
      ApplicationSceneNode applicationNode;
      ResourceDictionaryNode searchDictionary = (baseFrameworkElement = pivot as BaseFrameworkElement) == null ? ((resourceDictionaryNode = pivot as ResourceDictionaryNode) == null ? ((applicationNode = pivot as ApplicationSceneNode) == null ? (ResourceDictionaryNode) null : applicationNode.Resources) : resourceDictionaryNode) : baseFrameworkElement.Resources;
      if (searchDictionary != null)
      {
        foreach (DictionaryEntryNode dictionaryEntryNode in searchDictionary)
        {
          if (dictionaryEntryNode != null && dictionaryEntryNode.Value != null && BindingEditor.IsDataSource(dictionaryEntryNode.Value.DocumentNode))
            yield return (SceneNode) dictionaryEntryNode;
        }
        foreach (ResourceDictionaryNode resourceDictionaryNode1 in (IEnumerable<ResourceDictionaryNode>) searchDictionary.MergedDictionaries)
        {
          foreach (DictionaryEntryNode dictionaryEntryNode in resourceDictionaryNode1)
          {
            if (dictionaryEntryNode != null && dictionaryEntryNode.Value != null && BindingEditor.IsDataSource(dictionaryEntryNode.Value.DocumentNode))
              yield return (SceneNode) dictionaryEntryNode;
          }
        }
      }
    }

    public bool CanCreateAndSetBindingOrData(SceneNode target, IPropertyId targetProperty, DataSchemaNodePath bindingPath, bool useSourceInherited)
    {
      if (targetProperty == null)
        return this.CanCreateAndSetBindingOrData(target, bindingPath, useSourceInherited);
      return this.CanCreateAndSetBindingOrDataInternal(target, targetProperty, bindingPath, useSourceInherited);
    }

    public bool CanCreateAndSetBindingOrData(SceneNode target, DataSchemaNodePath bindingPath, bool useSourceInherited)
    {
      IPropertyId targetProperty;
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) target.Type))
      {
        targetProperty = BaseFrameworkElement.WidthProperty;
      }
      else
      {
        targetProperty = (IPropertyId) Enumerable.FirstOrDefault<ReferenceStep>((IEnumerable<ReferenceStep>) BindingPropertyHelper.GetBindableTargetProperties(target), (Func<ReferenceStep, bool>) (prop => !DataContextHelper.IsDataContextProperty(target.DocumentNode, (IPropertyId) prop)));
        if (targetProperty == null)
          return false;
      }
      return this.CanCreateAndSetBindingOrDataInternal(target, targetProperty, bindingPath, useSourceInherited);
    }

    private bool CanCreateAndSetBindingOrDataInternal(SceneNode target, IPropertyId targetProperty, DataSchemaNodePath bindingPath, bool useSourceInherited)
    {
      return useSourceInherited && this.SetSourceAsDataContext(target, targetProperty, ref bindingPath, true) || this.CreateDataSource(target, bindingPath, false) != null;
    }

    public SceneNode CreateAndSetBindingOrData(SceneNode target, IPropertyId targetProperty, DataSchemaNodePath bindingPath)
    {
      return this.CreateAndSetBindingOrData(target, targetProperty, bindingPath, true);
    }

    public SceneNode CreateAndSetBindingOrData(SceneNode target, IPropertyId targetProperty, DataSchemaNodePath bindingPath, bool useSourceInherited)
    {
      SceneNode bindingOrData = this.CreateBindingOrData(target, targetProperty, bindingPath, useSourceInherited);
      if (bindingOrData != null)
        BindingEditor.SetBindingOrData(target, targetProperty, bindingOrData, bindingPath);
      return bindingOrData;
    }

    internal static IPropertyId RefineDataContextProperty(SceneNode target, IPropertyId targetProperty, DocumentNode dataNode)
    {
      if (targetProperty.MemberType == MemberType.DesignTimeProperty || !DataContextHelper.IsDataContextProperty(target.DocumentNode, targetProperty))
        return targetProperty;
      SampleNonBasicType sampleNonBasicType = dataNode.Type as SampleNonBasicType;
      if (sampleNonBasicType != null)
      {
        if (!sampleNonBasicType.DeclaringDataSet.IsEnabledAtRuntime)
          return DesignTimeProperties.DesignDataContextProperty;
        if (((DocumentCompositeNode) target.DocumentNode).Properties[targetProperty] != null)
        {
          DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(target, targetProperty, true);
          if (dataContextInfo.DataSource != null && !(dataContextInfo.DataSource.DataSourceType is SampleNonBasicType))
            return DesignTimeProperties.DesignDataContextProperty;
        }
        return targetProperty;
      }
      if (PlatformTypes.IsExpressionInteractiveType(dataNode.Type.RuntimeType))
        return DesignTimeProperties.DesignDataContextProperty;
      return targetProperty;
    }

    public SceneNode CreateBindingOrData(SceneNode target, IPropertyId targetProperty, DataSchemaNodePath bindingPath, bool useSourceInherited)
    {
      BindingSceneNode bindingSceneNode;
      if (!useSourceInherited || !this.SetSourceAsDataContext(target, targetProperty, ref bindingPath, false))
      {
        SceneNode dataSource = this.CreateDataSource(target, bindingPath, false);
        bindingSceneNode = dataSource as BindingSceneNode;
        if (bindingSceneNode == null)
          return dataSource;
      }
      else
        bindingSceneNode = BindingSceneNode.Factory.Instantiate(target.ViewModel);
      string path = bindingPath.Path;
      if (bindingPath.Schema is XmlSchema)
        bindingSceneNode.XPath = path;
      else if (!string.IsNullOrEmpty(path))
        bindingSceneNode.SetPath(path);
      BindingModeInfo defaultBindingMode = BindingPropertyHelper.GetDefaultBindingMode(target.DocumentNode, targetProperty, bindingPath);
      if (!defaultBindingMode.IsOptional)
        bindingSceneNode.Mode = defaultBindingMode.Mode;
      return (SceneNode) bindingSceneNode;
    }

    private SceneNode CreateDataSource(SceneNode target, DataSchemaNodePath bindingPath, bool isSourcePathLess)
    {
      DocumentNode resourceKey = bindingPath.Schema.DataSource.ResourceKey;
      if (resourceKey == null)
      {
        if (!isSourcePathLess && !string.IsNullOrEmpty(bindingPath.Path))
          return (SceneNode) null;
        DocumentNode documentNode = bindingPath.Schema.DataSource.DocumentNode;
        IProjectItem designDataFile = DesignDataHelper.GetDesignDataFile(documentNode);
        DocumentNode node;
        if (designDataFile != null)
        {
          node = DesignDataHelper.CreateDesignDataExtension(designDataFile, target.DocumentContext);
        }
        else
        {
          if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) bindingPath.Schema.DataSource.DocumentNode.Type))
            return (SceneNode) null;
          node = documentNode.Clone(target.DocumentContext);
        }
        return target.ViewModel.GetSceneNode(node);
      }
      if (!this.EnsureDataSourceReachable(target, bindingPath.Schema.DataSource))
        return (SceneNode) null;
      BindingSceneNode bindingSceneNode = BindingSceneNode.Factory.Instantiate(target.ViewModel);
      DocumentNode keyNode = resourceKey.Clone(target.DocumentContext);
      bindingSceneNode.Source = (DocumentNode) DocumentNodeUtilities.NewStaticResourceNode(keyNode.Context, keyNode);
      return (SceneNode) bindingSceneNode;
    }

    public void UpdateElementNameBindings(DocumentCompositeNode namedNode, string oldName, string newName)
    {
      if (namedNode == null || string.IsNullOrEmpty(oldName) || (string.IsNullOrEmpty(newName) || oldName == newName))
        return;
      IProperty elementNameProperty = namedNode.TypeResolver.ResolveProperty(BindingSceneNode.ElementNameProperty);
      if (elementNameProperty == null)
        return;
      DocumentCompositeNode documentNode = namedNode;
      while (documentNode != null && documentNode.NameScope == null)
        documentNode = documentNode.Parent;
      if (documentNode == null)
        return;
      this.UpdateElementNameBindingsInternal(documentNode, elementNameProperty, oldName, newName);
    }

    private void UpdateElementNameBindingsInternal(DocumentCompositeNode documentNode, IProperty elementNameProperty, string oldName, string newName)
    {
      DocumentNode node = documentNode.Properties[(IPropertyId) elementNameProperty];
      if (node != null)
      {
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
        if (oldName == valueAsString)
          documentNode.Properties[(IPropertyId) elementNameProperty] = (DocumentNode) documentNode.Context.CreateNode(newName);
      }
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentNode.Properties)
      {
        DocumentCompositeNode documentNode1 = keyValuePair.Value as DocumentCompositeNode;
        if (documentNode1 != null && documentNode1.NameScope == null)
          this.UpdateElementNameBindingsInternal(documentNode1, elementNameProperty, oldName, newName);
      }
      if (!documentNode.SupportsChildren)
        return;
      for (int index = 0; index < documentNode.Children.Count; ++index)
      {
        DocumentCompositeNode documentNode1 = documentNode.Children[index] as DocumentCompositeNode;
        if (documentNode1 != null && documentNode1.NameScope == null)
          this.UpdateElementNameBindingsInternal(documentNode1, elementNameProperty, oldName, newName);
      }
    }

    private bool SetSourceAsDataContext(SceneNode target, IPropertyId targetPropertyId, ref DataSchemaNodePath bindingPath, bool testOnly)
    {
      if (target == null || targetPropertyId == null)
        return false;
      IProperty property = target.ProjectContext.ResolveProperty(targetPropertyId);
      if (property == null)
        return false;
      SceneNode sceneNode = target;
      IProperty targetProperty = property;
      if (DataContextHelper.IsDataContextProperty(target.DocumentNode, (IPropertyId) property))
      {
        sceneNode = sceneNode.Parent;
        targetProperty = (IProperty) null;
        if (sceneNode == null || sceneNode.Parent == null)
          return false;
      }
      DataSourceInfo dataSourceInfo = new DataSourceInfo(bindingPath);
      DataContextInfo contextPlacement = DataContextPlacementEvaluator.FindDataContextPlacement(sceneNode, targetProperty, dataSourceInfo);
      if (contextPlacement == null || contextPlacement.DataSourceMatch == DataSourceMatchCriteria.Ignore)
        return false;
      if (contextPlacement.DataSourceMatch == DataSourceMatchCriteria.Exact || contextPlacement.DataSourceMatch == DataSourceMatchCriteria.Compatible)
      {
        if (contextPlacement.DataSourceMatch == DataSourceMatchCriteria.Exact)
          bindingPath = bindingPath.GetRelativeNodePath(bindingPath);
        else if (!string.IsNullOrEmpty(contextPlacement.DataSource.Path))
        {
          DataSchemaNodePath nodePathFromPath = bindingPath.Schema.GetNodePathFromPath(contextPlacement.DataSource.Path);
          bindingPath = nodePathFromPath.GetRelativeNodePath(bindingPath);
        }
        return true;
      }
      if (contextPlacement.DataSourceMatch == DataSourceMatchCriteria.Any && contextPlacement.Owner != null && (contextPlacement.Owner != target.DocumentNode || contextPlacement.Property == null || !contextPlacement.Property.Equals((object) property)))
      {
        IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(contextPlacement.Owner.Type);
        if (dataContextProperty != null)
        {
          SceneNode ownerSceneNode = contextPlacement.GetOwnerSceneNode(target.ViewModel);
          SceneNode dataSource = this.CreateDataSource(ownerSceneNode, bindingPath, true);
          if (dataSource != null)
          {
            if (!testOnly)
              BindingEditor.SetBindingOrData(ownerSceneNode, (IPropertyId) dataContextProperty, dataSource, bindingPath);
            return true;
          }
        }
      }
      return false;
    }

    private static void SetBindingOrData(SceneNode targetNode, IPropertyId targetProperty, SceneNode bindingOrData, DataSchemaNodePath bindingPath)
    {
      BindingSceneNode binding = bindingOrData as BindingSceneNode;
      using (targetNode.ViewModel.AnimationEditor.DeferKeyFraming())
      {
        if (binding != null)
        {
          IPropertyId propertyKey = targetProperty;
          if (binding.Source != null)
            propertyKey = BindingEditor.RefineDataContextProperty(targetNode, targetProperty, bindingPath.Schema.DataSource.DocumentNode);
          targetNode.SetBinding(propertyKey, binding);
        }
        else
        {
          IPropertyId propertyKey = BindingEditor.RefineDataContextProperty(targetNode, targetProperty, bindingOrData.DocumentNode);
          targetNode.SetValueAsSceneNode(propertyKey, bindingOrData);
        }
      }
    }

    private bool EnsureDataSourceReachable(SceneNode targetNode, DataSourceNode dataSource)
    {
      if (!targetNode.ViewModel.Document.HasOpenTransaction || new ExpressionEvaluator(targetNode.ViewModel.DocumentRootResolver).EvaluateResource(targetNode.DocumentNodePath, ResourceReferenceType.Static, dataSource.ResourceKey) != null)
        return true;
      IProjectDocument projectDocument = targetNode.ProjectContext.OpenDocument(dataSource.DocumentNode.Context.DocumentUrl);
      if (projectDocument == null || projectDocument.DocumentType != ProjectDocumentType.ResourceDictionary)
        return false;
      SceneDocument resourceDictionary1 = projectDocument.Document as SceneDocument;
      ResourceManager resourceManager = targetNode.ViewModel.DesignerContext.ResourceManager;
      ResourceDictionaryContentProvider resourceDictionary2 = resourceManager.FindContentProviderForResourceDictionary(resourceDictionary1);
      ResourceContainer resourceContainer = resourceManager.FindResourceContainer(targetNode.DocumentContext.DocumentUrl);
      return resourceDictionary2.EnsureLinked(targetNode.ViewModel, resourceContainer);
    }
  }
}

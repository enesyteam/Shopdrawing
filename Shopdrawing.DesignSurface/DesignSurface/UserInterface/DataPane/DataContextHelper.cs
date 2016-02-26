// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataContextHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public static class DataContextHelper
  {
    private static readonly IPropertyId FrameworkContentDataContextNeutralProperty = (IPropertyId) PlatformTypes.FrameworkContentElement.GetMember(MemberType.LocalProperty, "DataContext", MemberAccessTypes.Public);

    public static IProperty GetDataContextProperty(IType type, bool needDesignDataContextProperty)
    {
      IProperty property = DataContextHelper.GetDataContextProperty(type);
      if (property == null)
        return (IProperty) null;
      if (needDesignDataContextProperty)
        property = DesignTimeProperties.ResolveDesignTimePropertyKey(DesignTimeProperties.DesignDataContextProperty, type.PlatformMetadata);
      return property;
    }

    public static bool HasDataContextProperty(IType type)
    {
      return DataContextHelper.GetDataContextProperty(type) != null;
    }

    public static IProperty GetDataContextProperty(IType type)
    {
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) type))
        return type.PlatformMetadata.ResolveProperty(BaseFrameworkElement.DataContextProperty);
      if (PlatformTypes.FrameworkContentElement.IsAssignableFrom((ITypeId) type))
        return type.PlatformMetadata.ResolveProperty(DataContextHelper.FrameworkContentDataContextNeutralProperty);
      return (IProperty) null;
    }

    public static bool IsDataContextProperty(DocumentNode documentNode, IPropertyId property)
    {
      if (property != null && documentNode != null)
      {
        if (property.MemberType == MemberType.DesignTimeProperty)
          return DesignTimeProperties.DesignDataContextProperty.Equals((object) property);
        IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(documentNode.Type);
        if (dataContextProperty != null && dataContextProperty.Equals((object) property))
          return true;
      }
      return false;
    }

    public static RawDataSourceInfoBase GetRawDataSourceInfo(DocumentNode dataSourceNode)
    {
      return !dataSourceNode.Type.IsBinding ? (!dataSourceNode.Type.IsResource ? (RawDataSourceInfoBase) new RawDataSourceInfo(dataSourceNode, (string) null) : DataContextHelper.GetDataSourceInfoFromResourceReference((DocumentCompositeNode) dataSourceNode)) : DataContextHelper.GetDataSourceInfoFromBinding((DocumentCompositeNode) dataSourceNode);
    }

    public static RawDataSourceInfoBase GetDataSourceInfoFromBinding(DocumentCompositeNode bindingNode)
    {
      RawDataSourceInfoBase dataSourceInfoBase = (RawDataSourceInfoBase) null;
      DocumentCompositeNode resourceReferenceNode = bindingNode.Properties[BindingSceneNode.SourceProperty] as DocumentCompositeNode;
      if (resourceReferenceNode == null)
        dataSourceInfoBase = (RawDataSourceInfoBase) DataContextHelper.GetElementNameBinding(bindingNode);
      if (dataSourceInfoBase == null)
      {
        string bindingPath = DataContextHelper.GetBindingPath(bindingNode);
        if (resourceReferenceNode != null && resourceReferenceNode.Type.IsResource)
        {
          dataSourceInfoBase = DataContextHelper.GetDataSourceInfoFromResourceReference(resourceReferenceNode);
          if (!dataSourceInfoBase.IsValid)
            return dataSourceInfoBase;
          dataSourceInfoBase.AppendClrPath(bindingPath);
        }
        else
          dataSourceInfoBase = (RawDataSourceInfoBase) new RawDataSourceInfo((DocumentNode) resourceReferenceNode, bindingPath);
      }
      string bindingXpath = DataContextHelper.GetBindingXPath(bindingNode);
      dataSourceInfoBase.XmlPath = XmlSchema.CombineXPaths(dataSourceInfoBase.XmlPath, bindingXpath);
      if (bindingNode.Properties[BindingSceneNode.RelativeSourceProperty] != null)
        dataSourceInfoBase.SetInvalid();
      return dataSourceInfoBase;
    }

    public static string GetBindingPath(DocumentCompositeNode bindingNode)
    {
      string str = (string) null;
      DocumentCompositeNode documentCompositeNode = bindingNode.Properties[BindingSceneNode.PathProperty] as DocumentCompositeNode;
      if (documentCompositeNode != null)
        str = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[documentCompositeNode.TypeResolver.PlatformMetadata.KnownProperties.PropertyPathPath]);
      return str;
    }

    public static string GetBindingXPath(DocumentCompositeNode bindingNode)
    {
      string str = (string) null;
      IProperty property = bindingNode.TypeResolver.ResolveProperty(BindingSceneNode.XPathProperty);
      if (property != null)
      {
        DocumentPrimitiveNode documentPrimitiveNode = bindingNode.Properties[(IPropertyId) property] as DocumentPrimitiveNode;
        if (documentPrimitiveNode != null)
          str = DocumentPrimitiveNode.GetValueAsString((DocumentNode) documentPrimitiveNode);
      }
      return str;
    }

    private static ElementDataSourceInfo GetElementNameBinding(DocumentCompositeNode bindingNode)
    {
      DocumentNode node = bindingNode.Properties[BindingSceneNode.ElementNameProperty];
      if (node == null)
        return (ElementDataSourceInfo) null;
      string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
      if (string.IsNullOrEmpty(valueAsString))
        return ElementDataSourceInfo.Invalid;
      DocumentCompositeNode namedElement = node.FindContainingNameScope().FindNode(valueAsString) as DocumentCompositeNode;
      if (namedElement == null)
        return ElementDataSourceInfo.Invalid;
      string bindingPath = DataContextHelper.GetBindingPath(bindingNode);
      return new ElementDataSourceInfo(namedElement, bindingPath);
    }

    private static RawDataSourceInfoBase GetDataSourceInfoFromResourceReference(DocumentCompositeNode resourceReferenceNode)
    {
      if (resourceReferenceNode.DocumentRoot == null)
        return (RawDataSourceInfoBase) RawDataSourceInfo.Invalid;
      DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(resourceReferenceNode);
      ResourceReferenceType resourceType = ResourceNodeHelper.GetResourceType((DocumentNode) resourceReferenceNode);
      DocumentNodePath nodePath = new DocumentNodePath(resourceReferenceNode.DocumentRoot.RootNode, (DocumentNode) resourceReferenceNode);
      DocumentNode sourceNode = new ExpressionEvaluator((IDocumentRootResolver) resourceReferenceNode.Context).EvaluateResource(nodePath, resourceType, resourceKey);
      RawDataSourceInfoBase dataSourceInfoBase = (RawDataSourceInfoBase) new RawDataSourceInfo(sourceNode, (string) null);
      if (sourceNode != null && PlatformTypes.XmlDataProvider.IsAssignableFrom((ITypeId) sourceNode.Type))
      {
        DocumentNode node = ((DocumentCompositeNode) sourceNode).Properties[XmlDataProviderSceneNode.XPathProperty];
        if (node != null)
        {
          string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
          if (!string.IsNullOrEmpty(valueAsString))
            dataSourceInfoBase.XmlPath = valueAsString;
        }
      }
      return dataSourceInfoBase;
    }

    public static IType GetDataType(DocumentNode dataNode)
    {
      if (dataNode == null)
        return (IType) null;
      IType type1 = dataNode.Type;
      IType type2;
      if (dataNode is DocumentPrimitiveNode)
      {
        type2 = DocumentPrimitiveNode.GetValueAsType(dataNode) ?? dataNode.Type;
      }
      else
      {
        DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) dataNode;
        if (PlatformTypes.ObjectDataProvider.IsAssignableFrom((ITypeId) dataNode.Type))
          type2 = DataContextHelper.GetObjectDataProviderType(documentCompositeNode);
        else if (PlatformTypes.CollectionViewSource.IsAssignableFrom((ITypeId) dataNode.Type))
          type2 = DataContextHelper.GetCollectionViewSourceType(documentCompositeNode);
        else if (dataNode.Type.IsBinding)
          type2 = DataContextHelper.GetBindingType(documentCompositeNode);
        else if (dataNode.Type.RuntimeType == typeof (DesignInstanceExtension))
          type2 = DataContextHelper.GetDesignInstanceType(documentCompositeNode);
        else if (dataNode.Type.RuntimeType == typeof (DesignDataExtension))
        {
          type2 = DataContextHelper.GetDesignDataType(documentCompositeNode);
        }
        else
        {
          if (!PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) dataNode.Type))
            return dataNode.Type;
          type2 = DataContextHelper.GetDataTemplateType(documentCompositeNode);
        }
        if (type2 == null || type2.RuntimeType == (Type) null)
          type2 = dataNode.TypeResolver.ResolveType(PlatformTypes.Object);
      }
      return type2;
    }

    private static IType GetObjectDataProviderType(DocumentCompositeNode objectDataProviderNode)
    {
      DocumentNode dataNode1 = objectDataProviderNode.Properties[ObjectDataProviderSceneNode.ObjectTypeProperty];
      if (dataNode1 != null)
        return DataContextHelper.GetDataType(dataNode1);
      DocumentNode dataNode2 = objectDataProviderNode.Properties[ObjectDataProviderSceneNode.ObjectInstanceProperty];
      if (dataNode2 != null)
        return DataContextHelper.GetDataType(dataNode2);
      return (IType) null;
    }

    private static IType GetCollectionViewSourceType(DocumentCompositeNode collectionViewNode)
    {
      DocumentNode dataNode = collectionViewNode.Properties[DesignTimeProperties.DesignSourceProperty];
      if (dataNode != null)
        return DataContextHelper.GetDataType(dataNode);
      IPropertyId index = (IPropertyId) PlatformTypes.CollectionViewSource.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
      return DataContextHelper.GetDataType(collectionViewNode.Properties[index]);
    }

    private static IType GetBindingType(DocumentCompositeNode bindingNode)
    {
      return DataContextHelper.GetTypeFromDataSource(DataContextHelper.GetDataSourceInfoFromBinding(bindingNode));
    }

    private static IType GetTypeFromDataSource(RawDataSourceInfoBase dataSource)
    {
      if (!dataSource.IsValid || dataSource.SourceNode == null)
        return (IType) null;
      IType type = DataContextHelper.GetDataType(dataSource.SourceNode);
      if (dataSource.HasClrPath && type != null && type.RuntimeType != (Type) null)
      {
        DataSchemaNodePath nodePathFromPath = new ClrObjectSchema(type.RuntimeType, dataSource.SourceNode).GetNodePathFromPath(dataSource.ClrPath);
        type = nodePathFromPath != null ? nodePathFromPath.Type : (IType) null;
      }
      return type;
    }

    private static IType GetDesignDataType(DocumentCompositeNode designDataNode)
    {
      if (designDataNode.DocumentRoot == null)
        return (IType) null;
      IType type = (IType) null;
      IDocumentRoot sourceXamlDocument = DesignDataInstanceBuilder.GetSourceXamlDocument(designDataNode);
      if (sourceXamlDocument != null && sourceXamlDocument.RootNode != null)
        type = DataContextHelper.GetDataType(sourceXamlDocument.RootNode);
      return type;
    }

    public static IType GetDataTemplateType(DocumentCompositeNode dataTemplateNode)
    {
      IType type = (IType) null;
      IProperty property = dataTemplateNode.TypeResolver.ResolveProperty(DataTemplateElement.DataTypeProperty);
      if (property != null)
        type = DocumentPrimitiveNode.GetValueAsType(dataTemplateNode.Properties[(IPropertyId) property]);
      return type;
    }

    private static IType GetDesignInstanceType(DocumentCompositeNode designInstanceNode)
    {
      if (designInstanceNode.DocumentRoot == null)
        return (IType) null;
      IType type = (IType) null;
      IProperty property1 = (IProperty) designInstanceNode.Type.GetMember(MemberType.LocalProperty, "Type", MemberAccessTypes.Public);
      DocumentNode dataNode = designInstanceNode.Properties[(IPropertyId) property1];
      if (dataNode != null)
      {
        IType dataType = DataContextHelper.GetDataType(dataNode);
        if (dataType.RuntimeType == (Type) null)
          return (IType) null;
        IProperty property2 = (IProperty) designInstanceNode.Type.GetMember(MemberType.LocalProperty, "CreateList", MemberAccessTypes.Public);
        DocumentPrimitiveNode documentPrimitiveNode1 = designInstanceNode.Properties[(IPropertyId) property2] as DocumentPrimitiveNode;
        bool createList = documentPrimitiveNode1 != null && documentPrimitiveNode1.GetValue<bool>();
        IProperty property3 = (IProperty) designInstanceNode.Type.GetMember(MemberType.LocalProperty, "IsDesignTimeCreatable", MemberAccessTypes.Public);
        DocumentPrimitiveNode documentPrimitiveNode2 = designInstanceNode.Properties[(IPropertyId) property3] as DocumentPrimitiveNode;
        bool isDesignTimeCreatable = documentPrimitiveNode2 != null && documentPrimitiveNode2.GetValue<bool>();
        DesignTypeResult typeToInstantiate = DesignTypeInstanceBuilder.GetTypeToInstantiate(designInstanceNode.PlatformMetadata, dataType.RuntimeType, createList, isDesignTimeCreatable);
        if (typeToInstantiate.IsFailed)
          return (IType) null;
        type = designInstanceNode.TypeResolver.GetType(typeToInstantiate.DesignType);
      }
      return type;
    }
  }
}

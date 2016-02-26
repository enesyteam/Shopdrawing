// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.PlatformConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface
{
  public class PlatformConverter
  {
    private static List<string> fastConversionTypes = new List<string>();
    private static string portableUserInterfaceString = "Portable User Interface";
    private DesignerContext designerContext;
    private static FontFamily portableUserInterfaceFont;
    private IDocumentContext standaloneWpfDocumentContext;

    private static FontFamily PortableUserInterfaceFont
    {
      get
      {
        if (PlatformConverter.portableUserInterfaceFont == null)
          PlatformConverter.portableUserInterfaceFont = FileTable.GetFontFamily("resources/PortableUserInterfaceFont.xaml");
        return PlatformConverter.portableUserInterfaceFont;
      }
    }

    private IDocumentContext StandaloneWpfDocumentContext
    {
      get
      {
        if (this.standaloneWpfDocumentContext == null)
          this.standaloneWpfDocumentContext = (IDocumentContext) new DocumentContext((IProjectContext) new PurePlatformProjectContext(this.designerContext.DesignerDefaultPlatformService.DefaultPlatform), (IDocumentLocator) null, true);
        return this.standaloneWpfDocumentContext;
      }
    }

    static PlatformConverter()
    {
      PlatformConverter.fastConversionTypes.Add("System.Windows.Media.PathGeometry");
      PlatformConverter.fastConversionTypes.Add("System.Windows.Media.ImageBrush");
      PlatformConverter.fastConversionTypes.Sort();
    }

    internal PlatformConverter(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public object ConvertToWpf(IDocumentContext sourceDocumentContext, object obj)
    {
      if (PlatformConverter.IsPlatformValue(obj, this.StandaloneWpfDocumentContext))
        return obj;
      if (obj != null)
      {
        ITypeId typeId = (ITypeId) sourceDocumentContext.TypeResolver.GetType(obj.GetType());
        if (PlatformTypes.Style.IsAssignableFrom(typeId) || PlatformTypes.FrameworkTemplate.IsAssignableFrom(typeId))
          return Activator.CreateInstance(PlatformTypeHelper.ConvertTypeId(typeId, this.StandaloneWpfDocumentContext.TypeResolver.PlatformMetadata).RuntimeType);
        if (PlatformTypes.CompositeTransform.IsAssignableFrom(typeId))
        {
          IPlatform platform = PlatformConverter.GetPlatform(sourceDocumentContext);
          if (platform != null)
          {
            Transform transform = (Transform) platform.GeometryHelper.ConvertTransformToWpf(obj);
            if (transform != null)
              return (object) transform;
          }
        }
      }
      object obj1 = this.ConvertInternal(obj, sourceDocumentContext, this.StandaloneWpfDocumentContext, PlatformConverter.ConvertToType.InstanceValue);
      FontFamily fontFamily = obj1 as FontFamily;
      if (fontFamily != null && fontFamily.Source == PlatformConverter.portableUserInterfaceString)
        return (object) PlatformConverter.PortableUserInterfaceFont;
      return obj1;
    }

    public DocumentNode ConvertToWpfAsDocumentNode(IDocumentContext sourceDocumentContext, object obj)
    {
      return (DocumentNode) this.ConvertInternal(obj, sourceDocumentContext, this.StandaloneWpfDocumentContext, PlatformConverter.ConvertToType.DocumentNode);
    }

    public object ConvertToSilverlight(IDocumentContext targetContext, object obj)
    {
      return this.ConvertInternal(obj, this.StandaloneWpfDocumentContext, targetContext, PlatformConverter.ConvertToType.InstanceValue);
    }

    public DocumentNode ConvertToSilverlightAsDocumentNode(IDocumentContext targetContext, object obj)
    {
      return (DocumentNode) this.ConvertInternal(obj, this.StandaloneWpfDocumentContext, targetContext, PlatformConverter.ConvertToType.DocumentNode);
    }

    public IPropertyId ConvertToWpfPropertyKey(IProperty propertyKey)
    {
      if (propertyKey == null)
        return (IPropertyId) null;
      return (IPropertyId) this.ConvertPropertyKey(propertyKey, this.StandaloneWpfDocumentContext.TypeResolver.PlatformMetadata);
    }

    public PropertyReference ConvertFromWpfPropertyReference(PropertyReference propertyReference, IPlatformMetadata destinationPlatformMetadata)
    {
      if (propertyReference != null)
        return PlatformTypes.ConvertToPlatformPropertyReference(propertyReference, destinationPlatformMetadata);
      return (PropertyReference) null;
    }

    public PropertyReference ConvertToWpfPropertyReference(PropertyReference propertyReference, IPlatformMetadata destinationWpfPlatform)
    {
      if (propertyReference != null)
        return PlatformTypes.ConvertToPlatformPropertyReference(propertyReference, destinationWpfPlatform);
      return (PropertyReference) null;
    }

    private static bool IsPlatformValue(object value, IDocumentContext documentContext)
    {
      if (value == null || value == MixedProperty.Mixed)
        return true;
      Type type = value.GetType();
      return ((IPlatformTypes) documentContext.TypeResolver.PlatformMetadata).GetPlatformAssembly(type.Assembly) != null;
    }

    private static bool CanUseFastConversion(Type type)
    {
      if (type.IsEnum)
        return true;
      return PlatformConverter.fastConversionTypes.BinarySearch(type.FullName) >= 0;
    }

    private object ConvertInternalFast(object value, ITypeResolver sourceTypeResolver, ITypeResolver targetTypeResolver)
    {
      if (value == null)
        return value;
      Type type = value.GetType();
      IType platformType = ((IPlatformTypes) targetTypeResolver.PlatformMetadata).GetPlatformType(type.FullName);
      Type runtimeType = platformType.RuntimeType;
      object obj1;
      if (type.IsPrimitive || type.IsEnum)
      {
        if (runtimeType == (Type) null)
          return (object) null;
        if (runtimeType.Equals(type))
        {
          obj1 = value;
        }
        else
        {
          TypeConverter typeConverter = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(type);
          obj1 = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(runtimeType).ConvertFromInvariantString(typeConverter.ConvertToInvariantString(value));
        }
      }
      else
      {
        if (runtimeType == (Type) null)
          return (object) null;
        obj1 = InstanceBuilderOperations.InstantiateType(runtimeType, true);
        if (obj1 != null && PlatformTypes.ImageBrush.IsAssignableFrom((ITypeId) platformType))
          return this.ConvertImageBrush(value, obj1, sourceTypeResolver, targetTypeResolver);
        if (!type.IsValueType)
        {
          CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(type);
          if (adapterDescription != null)
          {
            IList list = CollectionAdapterDescription.GetAdapterDescription(runtimeType).GetCollectionAdapter(obj1) as IList;
            foreach (object obj2 in (IEnumerable) adapterDescription.GetCollectionAdapter(value))
            {
              object obj3 = this.ConvertInternalFast(obj2, sourceTypeResolver, targetTypeResolver);
              list.Add(obj3);
            }
          }
        }
        foreach (IProperty property in ((IPlatformTypes) sourceTypeResolver.PlatformMetadata).GetType(type).GetProperties(MemberAccessTypes.Public))
        {
          ReferenceStep referenceStep1 = property as ReferenceStep;
          if (referenceStep1 != null && referenceStep1.ReadAccess == MemberAccessType.Public && referenceStep1.WriteAccess == MemberAccessType.Public)
          {
            ReferenceStep referenceStep2 = platformType.GetMember(MemberType.Property, referenceStep1.Name, MemberAccessTypes.Public) as ReferenceStep;
            if (referenceStep2 != null && referenceStep2.ReadAccess == MemberAccessType.Public && referenceStep2.WriteAccess == MemberAccessType.Public)
            {
              object valueToSet = this.ConvertInternalFast(referenceStep1.GetValue(value), sourceTypeResolver, targetTypeResolver);
              referenceStep2.SetValue(obj1, valueToSet);
            }
          }
        }
      }
      return obj1;
    }

    private object ConvertImageBrush(object value, object result, ITypeResolver sourceTypeResolver, ITypeResolver targetTypeResolver)
    {
      ReferenceStep referenceStep1 = (ReferenceStep) sourceTypeResolver.ResolveProperty(TileBrushNode.StretchProperty);
      ReferenceStep referenceStep2 = (ReferenceStep) targetTypeResolver.ResolveProperty(TileBrushNode.StretchProperty);
      object valueToSet1 = this.ConvertInternalFast(referenceStep1.GetValue(value), sourceTypeResolver, targetTypeResolver);
      referenceStep2.SetValue(result, valueToSet1);
      object objToInspect = ((ReferenceStep) sourceTypeResolver.ResolveProperty(ImageBrushNode.ImageSourceProperty)).GetValue(value);
      if (!PlatformTypes.IsInstance(objToInspect, PlatformTypes.BitmapImage, sourceTypeResolver))
        return result;
      object obj1 = ((ReferenceStep) sourceTypeResolver.ResolveProperty(BitmapImageNode.UriSourceProperty)).GetValue(objToInspect);
      if (obj1 == null)
        return result;
      Type type1 = obj1.GetType();
      IType type2 = targetTypeResolver.ResolveType(PlatformTypes.Uri);
      string text = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(type1).ConvertToInvariantString(obj1);
      if (string.IsNullOrEmpty(text))
        return result;
      object valueToSet2 = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(type2.RuntimeType).ConvertFromInvariantString(text);
      ReferenceStep referenceStep3 = (ReferenceStep) targetTypeResolver.ResolveProperty(ImageBrushNode.ImageSourceProperty);
      object obj2 = InstanceBuilderOperations.InstantiateType(targetTypeResolver.ResolveType(PlatformTypes.BitmapImage).RuntimeType, true);
      referenceStep3.SetValue(result, obj2);
      try
      {
        BitmapImage bitmapImage = obj2 as BitmapImage;
        if (bitmapImage != null)
        {
          bitmapImage.BeginInit();
          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        }
        ((ReferenceStep) targetTypeResolver.ResolveProperty(BitmapImageNode.UriSourceProperty)).SetValue(obj2, valueToSet2);
        if (bitmapImage != null)
          bitmapImage.EndInit();
      }
      catch (Exception ex)
      {
        result = (object) null;
      }
      return result;
    }

    private static IPlatform GetPlatform(IDocumentContext documentContext)
    {
      if (documentContext == null)
        return (IPlatform) null;
      return ((IProjectContext) documentContext.TypeResolver).Platform;
    }

    private object ConvertInternal(object value, IDocumentContext sourcePlatformDocumentContext, IDocumentContext targetPlatformDocumentContext, PlatformConverter.ConvertToType convertType)
    {
      if (value == null)
        return (object) null;
      if (value == PlatformConverter.PortableUserInterfaceFont)
        value = (object) new FontFamily(PlatformConverter.portableUserInterfaceString);
      if (PlatformConverter.IsPlatformValue(value, targetPlatformDocumentContext))
      {
        if (convertType == PlatformConverter.ConvertToType.DocumentNode)
          return (object) targetPlatformDocumentContext.CreateNode(value.GetType(), value);
        return value;
      }
      Type type1 = value.GetType();
      if (convertType == PlatformConverter.ConvertToType.InstanceValue && PlatformConverter.CanUseFastConversion(type1))
        return this.ConvertInternalFast(value, sourcePlatformDocumentContext.TypeResolver, targetPlatformDocumentContext.TypeResolver);
      SceneDocument activeDocument = this.designerContext.ActiveDocument;
      ((PurePlatformProjectContext) this.StandaloneWpfDocumentContext.TypeResolver).SetActualProjectContext(activeDocument != null ? activeDocument.ProjectContext : (IProjectContext) null);
      DocumentNode documentNode1 = value as DocumentNode;
      DocumentNode node;
      using (((IProjectContext) sourcePlatformDocumentContext.TypeResolver).Platform.DocumentNodeBuilderFactory.ForceBuildAnimatedValue)
        node = documentNode1 ?? sourcePlatformDocumentContext.CreateNode(value.GetType(), value);
      if (node == null)
        return (object) null;
      object obj = (object) null;
      IType type2 = node.Type;
      IType platformType = ((IPlatformTypes) targetPlatformDocumentContext.TypeResolver.PlatformMetadata).GetPlatformType(type2.FullName);
      if (node is DocumentPrimitiveNode)
      {
        if (convertType == PlatformConverter.ConvertToType.DocumentNode)
          return (object) this.ConvertSubtree(node, sourcePlatformDocumentContext, targetPlatformDocumentContext);
        if (platformType == null || type2.TypeConverter == null || platformType.TypeConverter == null)
          return (object) null;
        DocumentPrimitiveNode documentPrimitiveNode1 = value as DocumentPrimitiveNode;
        string text;
        if (documentPrimitiveNode1 != null)
        {
          text = documentPrimitiveNode1.GetValue<string>();
        }
        else
        {
          DocumentPrimitiveNode documentPrimitiveNode2 = node as DocumentPrimitiveNode;
          text = documentPrimitiveNode2 == null || !(documentPrimitiveNode2.Value is DocumentNodeStringValue) ? type2.TypeConverter.ConvertToString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, value) : documentPrimitiveNode2.GetValue<string>();
        }
        try
        {
          obj = platformType.TypeConverter.ConvertFromString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, text);
        }
        catch
        {
        }
      }
      else
      {
        DocumentNode documentNode2 = this.ConvertSubtree(node, sourcePlatformDocumentContext, targetPlatformDocumentContext);
        if (convertType == PlatformConverter.ConvertToType.DocumentNode)
          return (object) documentNode2;
        if (documentNode2 != null)
        {
          DocumentNodePath documentNodePath = new DocumentNodePath(documentNode2, documentNode2);
          using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(targetPlatformDocumentContext, this.designerContext))
          {
            instanceBuilderContext.ViewNodeManager.RootNodePath = documentNodePath;
            instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
            using (instanceBuilderContext.DisablePostponedResourceEvaluation())
              obj = instanceBuilderContext.ViewNodeManager.ValidRootInstance;
            instanceBuilderContext.ViewNodeManager.RootNodePath = (DocumentNodePath) null;
          }
        }
      }
      if (obj == null && platformType != null && (!platformType.SupportsNullValues && platformType.RuntimeType != (Type) null))
        obj = InstanceBuilderOperations.InstantiateType(platformType.RuntimeType, true);
      return obj;
    }

    private DocumentNode ConvertSubtree(DocumentNode node, IDocumentContext sourcePlatformDocumentContext, IDocumentContext targetPlatformDocumentContext)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentPrimitiveNode != null)
      {
        ITypeId typeId = (ITypeId) PlatformTypeHelper.ConvertTypeId((ITypeId) documentPrimitiveNode.Type, targetPlatformDocumentContext.TypeResolver.PlatformMetadata);
        object valueAsObject = DocumentPrimitiveNode.GetValueAsObject((DocumentNode) documentPrimitiveNode);
        if (valueAsObject != null)
          return (DocumentNode) new DocumentPrimitiveNode(targetPlatformDocumentContext, typeId, this.ConvertInternal(valueAsObject, sourcePlatformDocumentContext, targetPlatformDocumentContext, PlatformConverter.ConvertToType.InstanceValue));
        IDocumentNodeValue documentNodeValue = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) != null || documentPrimitiveNode.Value == null ? (IDocumentNodeValue) null : documentPrimitiveNode.Value.Clone(targetPlatformDocumentContext);
        return (DocumentNode) new DocumentPrimitiveNode(targetPlatformDocumentContext, typeId, documentNodeValue);
      }
      if (documentCompositeNode != null)
      {
        if (PlatformTypes.CompositeTransform.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
        {
          IPlatform platform = PlatformConverter.GetPlatform(sourcePlatformDocumentContext);
          if (platform != null)
          {
            TransformGroup transformGroup = platform.GeometryHelper.ConvertTransformToWpf((object) documentCompositeNode);
            if (PlatformConverter.IsPlatformValue((object) transformGroup, targetPlatformDocumentContext))
              return targetPlatformDocumentContext.CreateNode(typeof (TransformGroup), (object) transformGroup);
          }
        }
        ITypeId typeId = (ITypeId) PlatformTypeHelper.ConvertTypeId((ITypeId) documentCompositeNode.Type, targetPlatformDocumentContext.TypeResolver.PlatformMetadata);
        if (!documentCompositeNode.TypeResolver.PlatformMetadata.IsNullType(typeId))
        {
          DocumentCompositeNode node1 = targetPlatformDocumentContext.CreateNode(typeId);
          foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode.Properties)
          {
            IProperty property = this.ConvertPropertyKey(keyValuePair.Key, targetPlatformDocumentContext.TypeResolver.PlatformMetadata);
            if (property != null)
            {
              DocumentNode node2 = keyValuePair.Value;
              if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) node2.Type) && !PlatformTypes.Style.IsAssignableFrom((ITypeId) node2.Type))
              {
                DocumentNode node3 = this.ConvertSubtree(node2, sourcePlatformDocumentContext, targetPlatformDocumentContext);
                if (node3 != null && (PlatformTypeHelper.GetPropertyType(property).IsAssignableFrom(node3.TargetType) || DocumentNodeUtilities.IsMarkupExtension(node3)))
                  node1.Properties[(IPropertyId) property] = node3;
              }
            }
          }
          if (documentCompositeNode.SupportsChildren)
          {
            for (int index = 0; index < documentCompositeNode.Children.Count; ++index)
            {
              DocumentNode node2 = documentCompositeNode.Children[index];
              node1.Children.Add(this.ConvertSubtree(node2, sourcePlatformDocumentContext, targetPlatformDocumentContext));
            }
          }
          return (DocumentNode) node1;
        }
      }
      return (DocumentNode) null;
    }

    private IProperty ConvertPropertyKey(IProperty propertyKey, IPlatformMetadata targetPlatformMetadata)
    {
      ITypeId typeId = (ITypeId) propertyKey.DeclaringType;
      if (typeId.Equals((object) PlatformTypes.TextElement))
        typeId = PlatformTypes.Inline;
      ITypeId type = (ITypeId) PlatformTypeHelper.ConvertTypeId(typeId, targetPlatformMetadata);
      IProperty property1 = type.GetMember(propertyKey.MemberType, propertyKey.Name, (MemberAccessTypes) (propertyKey.ReadAccess | propertyKey.WriteAccess)) as IProperty;
      if (property1 == null && targetPlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsProxyProperties))
      {
        IEnumerable<IProperty> proxyProperties = targetPlatformMetadata.GetProxyProperties(this.StandaloneWpfDocumentContext.TypeResolver);
        if (proxyProperties != null)
        {
          string name = propertyKey.Name;
          foreach (IProperty property2 in proxyProperties)
          {
            if (name == property2.Name && property2.DeclaringType.IsAssignableFrom(type))
            {
              property1 = property2;
              break;
            }
          }
        }
      }
      return property1;
    }

    private enum ConvertToType
    {
      InstanceValue,
      DocumentNode,
    }
  }
}

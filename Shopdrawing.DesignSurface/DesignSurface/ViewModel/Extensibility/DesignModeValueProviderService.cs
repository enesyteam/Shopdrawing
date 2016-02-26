// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.DesignModeValueProviderService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class DesignModeValueProviderService : IDisposable
  {
    private List<DesignModeValueProviderService.DesignModeValueProviderBuilder> instanceBuilders = new List<DesignModeValueProviderService.DesignModeValueProviderBuilder>();
    private static ulong uniqueId;
    private IPlatform platform;
    private FeatureManager featureManager;
    private ValueTranslationService valueTranslationService;

    private ValueTranslationService ValueTranslationService
    {
      get
      {
        if (this.valueTranslationService == null)
        {
          this.valueTranslationService = this.featureManager.Context.Services.GetRequiredService<ValueTranslationService>();
          this.valueTranslationService.PropertyInvalidated += new EventHandler<PropertyInvalidatedEventArgs>(this.ValueTranslationServicePropertyInvalidated);
        }
        return this.valueTranslationService;
      }
    }

    public DesignModeValueProviderService(IPlatform platform, FeatureManager featureManager)
    {
      this.platform = platform;
      this.featureManager = featureManager;
    }

    public void ProcessType(IType type)
    {
      Type runtimeType = type.RuntimeType;
      if (!Enumerable.Any<FeatureProvider>(this.featureManager.CreateFeatureProviders(typeof (DesignModeValueProvider), runtimeType)))
        return;
      IInstanceBuilder builder = this.platform.InstanceBuilderFactory.GetBuilder(runtimeType);
      if (!builder.BaseType.Equals(runtimeType))
      {
        ++DesignModeValueProviderService.uniqueId;
        TypeBuilder typeBuilder = RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.DefineType(runtimeType.Name + (object) ".CustomDesignTimeProperties" + (string) (object) DesignModeValueProviderService.uniqueId, TypeAttributes.Public);
        foreach (PropertyIdentifier propertyIdentifier in this.ValueTranslationService.GetProperties(runtimeType))
        {
          ReferenceStep referenceStep = type.GetMember(MemberType.Property, propertyIdentifier.Name, MemberAccessTypes.All) as ReferenceStep;
          if (referenceStep != null)
          {
            string str1 = "Runtime" + (object) referenceStep.Name + (string) (object) DesignModeValueProviderService.uniqueId;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Get" + str1, MethodAttributes.Public | MethodAttributes.Static, referenceStep.PropertyType.RuntimeType, new Type[1]
            {
              typeof (object)
            });
            methodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            CustomAttributeBuilder customBuilder1 = new CustomAttributeBuilder(typeof (DesignerSerializationVisibilityAttribute).GetConstructor(new Type[1]
            {
              typeof (DesignerSerializationVisibility)
            }), new object[1]
            {
              (object) DesignerSerializationVisibility.Hidden
            });
            methodBuilder.SetCustomAttribute(customBuilder1);
            typeBuilder.DefineMethod("Set" + str1, MethodAttributes.Public | MethodAttributes.Static, (Type) null, new Type[2]
            {
              typeof (object),
              referenceStep.PropertyType.RuntimeType
            }).GetILGenerator().Emit(OpCodes.Ret);
            IType type1 = type.PlatformMetadata.ResolveType(PlatformTypes.TypeConverterAttribute);
            List<string> results;
            if (PlatformNeutralAttributeHelper.TryGetAttributeValues<string>((IEnumerable) referenceStep.GetCustomAttributes(type1.RuntimeType, false), (ITypeId) type1, "ConverterTypeName", out results))
            {
              foreach (string str2 in results)
              {
                CustomAttributeBuilder customBuilder2 = new CustomAttributeBuilder(type1.RuntimeType.GetConstructor(new Type[1]
                {
                  typeof (string)
                }), new object[1]
                {
                  (object) str2
                });
                methodBuilder.SetCustomAttribute(customBuilder2);
              }
            }
            typeBuilder.DefineField(str1 + "Property", this.platform.Metadata.ResolveType(PlatformTypes.DependencyProperty).RuntimeType, FieldAttributes.Public | FieldAttributes.Static);
          }
        }
        IType shadowSourceDeclaringType = (IType) new DesignModeValueProviderService.ShadowPropertyType((IPlatformMetadata) this.platform.Metadata, typeBuilder.CreateType());
        foreach (PropertyIdentifier propertyIdentifier in this.ValueTranslationService.GetProperties(runtimeType))
        {
          ReferenceStep referenceStep = type.GetMember(MemberType.Property, propertyIdentifier.Name, MemberAccessTypes.All) as ReferenceStep;
          if (referenceStep != null)
          {
            string propertyName = "Runtime" + (object) referenceStep.Name + (string) (object) DesignModeValueProviderService.uniqueId;
            PropertyIdentifier localProperty = propertyIdentifier;
            ValueTranslationService valueTranslationService = this.ValueTranslationService;
            object obj = this.platform.Metadata.DesignTimeProperties.ExternalRegisterShadow(propertyName, shadowSourceDeclaringType, (IProperty) referenceStep, DesignerSerializationVisibility.Hidden, true, (DesignTimeProperties.PropertyChangeCallback) (o => DesignModeValueProviderService.RunDesignModeValueProvider(o, this.platform, valueTranslationService, type, runtimeType, localProperty, referenceStep, false)));
            shadowSourceDeclaringType.RuntimeType.GetField(propertyName + "Property", BindingFlags.Static | BindingFlags.Public).SetValue((object) null, obj);
          }
        }
        DesignModeValueProviderService.DesignModeValueProviderBuilder valueProviderBuilder = new DesignModeValueProviderService.DesignModeValueProviderBuilder(runtimeType, builder);
        this.instanceBuilders.Add(valueProviderBuilder);
        this.platform.InstanceBuilderFactory.Register((IInstanceBuilder) valueProviderBuilder);
      }
      else
      {
        DesignModeValueProviderService.DesignModeValueProviderBuilder valueProviderBuilder = builder as DesignModeValueProviderService.DesignModeValueProviderBuilder;
        if (valueProviderBuilder == null)
          return;
        valueProviderBuilder.AddReference();
      }
    }

    private void ValueTranslationServicePropertyInvalidated(object sender, PropertyInvalidatedEventArgs e)
    {
      ISceneNodeModelItem sceneNodeModelItem = e.Item as ISceneNodeModelItem;
      if (sceneNodeModelItem == null || sceneNodeModelItem.SceneNode == null)
        return;
      SceneNode sceneNode = sceneNodeModelItem.SceneNode;
      ReferenceStep referenceStep = sceneNode.Type.GetMember(MemberType.Property, e.InvalidatedProperty.Name, MemberAccessTypes.All) as ReferenceStep;
      if (sceneNode.ViewObject != null && sceneNode.ViewObject.PlatformSpecificObject != null && referenceStep != null)
        DesignModeValueProviderService.RunDesignModeValueProvider(sceneNode.ViewObject.PlatformSpecificObject, this.platform, this.ValueTranslationService, sceneNode.Type, sceneNode.TrueTargetType, e.InvalidatedProperty, referenceStep, false);
      else
        sceneNode.ViewModel.DefaultView.InstanceBuilderContext.ViewNodeManager.Invalidate(sceneNode.DocumentNode, InstanceState.Invalid);
    }

    private static void RunDesignModeValueProvider(object target, IPlatform platform, ValueTranslationService valueTranslationService, IType type, Type runtimeType, PropertyIdentifier property, ReferenceStep referenceStep, bool isFirstTime)
    {
      if (!runtimeType.IsAssignableFrom(target.GetType()))
        return;
      ModelItem modelItemForObject = DesignModeValueProviderService.GetModelItemForObject(platform, target);
      if (modelItemForObject == null)
        return;
      ReferenceStep referenceStep1 = (ReferenceStep) DesignTimeProperties.GetShadowProperty((IProperty) referenceStep, (ITypeId) type);
      try
      {
        object valueToSet;
        if (isFirstTime && !referenceStep1.IsSet(target))
        {
          valueToSet = referenceStep.GetValue(target);
          referenceStep1.SetValue(target, valueToSet);
        }
        else
          valueToSet = referenceStep1.GetValue(target);
        referenceStep.SetValue(target, valueTranslationService.TranslatePropertyValue(runtimeType, modelItemForObject, property, valueToSet));
      }
      catch (Exception ex)
      {
      }
    }

    private static ModelItem GetModelItemForObject(IPlatform platform, object obj)
    {
      IViewObject viewObject = platform.ViewObjectFactory.Instantiate(obj);
      IDesignModeValueProviderContext valueProviderContext = WeakReferenceHelper.Unwrap<IInstanceBuilderContext>(viewObject.GetCurrentValue(platform.Metadata.ResolveProperty(DesignTimeProperties.InstanceBuilderContextProperty))) as IDesignModeValueProviderContext;
      if (valueProviderContext != null)
        return valueProviderContext.GetModelItemForViewObject(viewObject);
      return (ModelItem) null;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool isDispoing)
    {
      if (!isDispoing)
        return;
      foreach (DesignModeValueProviderService.DesignModeValueProviderBuilder valueProviderBuilder in this.instanceBuilders)
      {
        if (valueProviderBuilder.RemoveReference())
          this.platform.InstanceBuilderFactory.Unregister((IInstanceBuilder) valueProviderBuilder);
      }
      this.instanceBuilders.Clear();
      if (this.valueTranslationService == null)
        return;
      this.valueTranslationService.PropertyInvalidated -= new EventHandler<PropertyInvalidatedEventArgs>(this.ValueTranslationServicePropertyInvalidated);
      this.valueTranslationService = (ValueTranslationService) null;
    }

    private class ShadowPropertyType : IType, IMember, ITypeId, IMemberId, ITypeMetadata
    {
      private IPlatformMetadata platformMetadata;
      private Type type;

      public IPlatformMetadata PlatformMetadata
      {
        get
        {
          return this.platformMetadata;
        }
      }

      public IAssembly RuntimeAssembly
      {
        get
        {
          return ((IPlatformTypes) this.PlatformMetadata).CreateAssembly(this.type.Assembly, AssemblySource.Unknown);
        }
      }

      public TypeConverter TypeConverter
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType BaseType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsArray
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsInterface
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsAbstract
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsGenericType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsBinding
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsResource
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool IsExpression
      {
        get
        {
          return false;
        }
      }

      public string XamlSourcePath
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType ItemType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType NearestResolvedType
      {
        get
        {
          return (IType) this;
        }
      }

      public ITypeMetadata Metadata
      {
        get
        {
          return (ITypeMetadata) this;
        }
      }

      public IType NullableType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public bool SupportsNullValues
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public Exception InitializationException
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public Type RuntimeType
      {
        get
        {
          return this.type;
        }
      }

      public ITypeId MemberTypeId
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IType DeclaringType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public MemberAccessType Access
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ITypeId DeclaringTypeId
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string FullName
      {
        get
        {
          return this.type.FullName;
        }
      }

      public string Name
      {
        get
        {
          return this.type.Name;
        }
      }

      public bool IsResolvable
      {
        get
        {
          return true;
        }
      }

      public MemberType MemberType
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string UniqueName
      {
        get
        {
          return this.type.FullName;
        }
      }

      public IXmlNamespace XmlNamespace
      {
        get
        {
          return (IXmlNamespace) null;
        }
      }

      public string Namespace
      {
        get
        {
          return this.type.Namespace;
        }
      }

      public bool IsBuilt
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ITypeResolver TypeResolver
      {
        get
        {
          throw new NotImplementedException();
        }
        set
        {
          throw new NotImplementedException();
        }
      }

      public bool IsNameScope
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public IProperty NameProperty
      {
        get
        {
          return (IProperty) null;
        }
      }

      public IProperty DefaultContentProperty
      {
        get
        {
          return (IProperty) null;
        }
      }

      public IPropertyId ResourcesProperty
      {
        get
        {
          return (IPropertyId) null;
        }
      }

      public IPropertyId ImplicitDictionaryKeyProperty
      {
        get
        {
          return (IPropertyId) null;
        }
      }

      public bool IsWhitespaceSignificant
      {
        get
        {
          return false;
        }
      }

      public bool SupportsInlineXml
      {
        get
        {
          return false;
        }
      }

      public bool ShouldTrimSurroundingWhitespace
      {
        get
        {
          return false;
        }
      }

      public ReadOnlyCollection<IPropertyId> ContentProperties
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ReadOnlyCollection<IProperty> Properties
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ReadOnlyCollection<IPropertyId> StyleProperties
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public ShadowPropertyType(IPlatformMetadata platformMetadata, Type type)
      {
        this.platformMetadata = platformMetadata;
        this.type = type;
      }

      public bool HasDefaultConstructor(bool supportInternal)
      {
        throw new NotImplementedException();
      }

      public IConstructorArgumentProperties GetConstructorArgumentProperties()
      {
        throw new NotImplementedException();
      }

      public IList<IType> GetGenericTypeArguments()
      {
        throw new NotImplementedException();
      }

      public IList<IConstructor> GetConstructors()
      {
        throw new NotImplementedException();
      }

      public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
      {
        throw new NotImplementedException();
      }

      public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
      {
        throw new NotImplementedException();
      }

      public bool IsInProject(ITypeResolver typeResolver)
      {
        return true;
      }

      public void InitializeClass()
      {
        throw new NotImplementedException();
      }

      public IMember Clone(ITypeResolver typeResolver)
      {
        return (IMember) this;
      }

      public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
      {
        return (IMemberId) new DesignModeValueProviderService.ShadowPropertyType.ShadowProperty(this.type.FullName + "." + memberName);
      }

      public bool IsAssignableFrom(ITypeId type)
      {
        throw new NotImplementedException();
      }

      public bool IsNameProperty(IPropertyId propertyKey)
      {
        return false;
      }

      public Type GetStylePropertyTargetType(IPropertyId propertyKey)
      {
        throw new NotImplementedException();
      }

      private class ShadowProperty : IPropertyId, IMemberId
      {
        private string fullName;

        public int SortValue
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public Type TargetType
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public ITypeId DeclaringTypeId
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public string FullName
        {
          get
          {
            return this.fullName;
          }
        }

        public string Name
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public bool IsResolvable
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public MemberType MemberType
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public string UniqueName
        {
          get
          {
            throw new NotImplementedException();
          }
        }

        public ShadowProperty(string fullName)
        {
          this.fullName = fullName;
        }
      }
    }

    private class DesignModeValueProviderBuilder : IInstanceBuilder
    {
      private int refCount = 1;
      private Type targetType;
      private IInstanceBuilder builder;

      public Type BaseType
      {
        get
        {
          return this.targetType;
        }
      }

      public Type ReplacementType
      {
        get
        {
          return this.builder.ReplacementType;
        }
      }

      public DesignModeValueProviderBuilder(Type targetType, IInstanceBuilder builder)
      {
        this.targetType = targetType;
        this.builder = builder;
      }

      public void AddReference()
      {
        ++this.refCount;
      }

      public bool RemoveReference()
      {
        --this.refCount;
        return this.refCount == 0;
      }

      public ViewNode GetViewNode(IInstanceBuilderContext context, DocumentNode documentNode)
      {
        return this.builder.GetViewNode(context, documentNode);
      }

      public AttachmentOrder GetAttachmentOrder(IInstanceBuilderContext context, ViewNode viewNode)
      {
        return this.builder.GetAttachmentOrder(context, viewNode);
      }

      public bool ShouldTryExpandExpression(IInstanceBuilderContext context, ViewNode viewNode, IPropertyId propertyKey, DocumentNode expressionNode)
      {
        return this.builder.ShouldTryExpandExpression(context, viewNode, propertyKey, expressionNode);
      }

      public bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
      {
        return this.builder.AllowPostponedResourceUpdate(context, viewNode, propertyKey, evaluatedResource);
      }

      public bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
      {
        return this.builder.Instantiate(context, viewNode);
      }

      public void Initialize(IInstanceBuilderContext context, ViewNode viewNode, bool isNewInstance)
      {
        this.builder.Initialize(context, viewNode, isNewInstance);
        if (context.IsSerializationScope)
          return;
        this.OnInitialized(context, viewNode, viewNode.Instance);
      }

      public void UpdateInstance(IInstanceBuilderContext context, ViewNode viewNode)
      {
        this.builder.UpdateInstance(context, viewNode);
      }

      public void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
      {
        this.builder.UpdateProperty(context, viewNode, propertyKey, valueNode);
      }

      public void UpdateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode)
      {
        this.builder.UpdateChild(context, viewNode, childIndex, action, childNode);
      }

      public void ModifyValue(IInstanceBuilderContext context, ViewNode target, object onlyThisInstance, IProperty propertyKey, object value, PropertyModification modification)
      {
        this.builder.ModifyValue(context, target, onlyThisInstance, propertyKey, value, modification);
      }

      public void OnInitialized(IInstanceBuilderContext context, ViewNode target, object instance)
      {
        if (instance != null && this.targetType.IsAssignableFrom(instance.GetType()))
        {
          ITypeResolver typeResolver = target.DocumentNode.TypeResolver;
          IPlatform platform = context.Platform;
          platform.ViewObjectFactory.Instantiate(instance).SetValue(typeResolver, typeResolver.ResolveProperty(DesignTimeProperties.InstanceBuilderContextProperty), (object) new WeakReference((object) context));
          IDesignModeValueProviderContext valueProviderContext = context as IDesignModeValueProviderContext;
          if (valueProviderContext != null)
          {
            ValueTranslationService valueTranslationService = valueProviderContext.ValueTranslationService;
            foreach (PropertyIdentifier identifier in valueTranslationService.GetProperties(this.targetType))
            {
              if (valueTranslationService.HasValueTranslation(this.targetType, identifier))
              {
                ReferenceStep referenceStep = target.Type.GetMember(MemberType.Property, identifier.Name, MemberAccessTypes.All) as ReferenceStep;
                if (referenceStep != null)
                {
                  if (!target.Properties.ContainsKey((IProperty) referenceStep))
                  {
                    ReferenceStep referenceStep1 = (ReferenceStep) DesignTimeProperties.GetShadowProperty((IProperty) referenceStep, (ITypeId) target.Type);
                    object valueToSet = referenceStep.GetValue(instance);
                    if (typeResolver.IsCapabilitySet(PlatformCapability.IsWpf) || valueToSet != null)
                      referenceStep1.SetValue(instance, valueToSet);
                  }
                  PropertyIdentifier localProperty = identifier;
                  UIThreadDispatcherHelper.BeginInvoke(DispatcherPriority.Send, (Delegate) (o =>
                  {
                    if (target.Parent != null)
                      DesignModeValueProviderService.RunDesignModeValueProvider(instance, platform, valueTranslationService, target.Type, this.targetType, localProperty, referenceStep, true);
                    return (object) null;
                  }), (object) null);
                }
              }
            }
          }
        }
        if (!context.IsSerializationScope)
          return;
        this.builder.OnInitialized(context, target, instance);
      }

      public void OnDescendantUpdated(IInstanceBuilderContext context, ViewNode viewNode, ViewNode child, InstanceState childState)
      {
        this.builder.OnDescendantUpdated(context, viewNode, child, childState);
      }

      public void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
      {
        this.builder.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
      }

      public void OnChildRemoving(IInstanceBuilderContext context, ViewNode parent, ViewNode child)
      {
        this.builder.OnChildRemoving(context, parent, child);
      }
    }
  }
}

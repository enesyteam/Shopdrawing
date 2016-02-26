// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.TypeReflectingProjectContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Core
{
  public sealed class TypeReflectingProjectContext : IProjectContext, IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
  {
    private Dictionary<Type, IType> typeCache = new Dictionary<Type, IType>();
    private IProjectContext actualProjectContext;
    private DesignTypeGenerator generator;

    private DesignTypeGenerator Generator
    {
      get
      {
        if (this.generator == null)
          this.generator = new DesignTypeGenerator(this.actualProjectContext.PlatformMetadata);
        return this.generator;
      }
    }

    public ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return this.actualProjectContext.AssemblyReferences;
      }
    }

    public IPlatformMetadata PlatformMetadata
    {
      get
      {
        return this.actualProjectContext.PlatformMetadata;
      }
    }

    public string RootNamespace
    {
      get
      {
        return this.actualProjectContext.RootNamespace;
      }
    }

    public IAssembly ProjectAssembly
    {
      get
      {
        return this.actualProjectContext.ProjectAssembly;
      }
    }

    public IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return this.actualProjectContext.ProjectNamespaces;
      }
    }

    public string ProjectPath
    {
      get
      {
        return this.actualProjectContext.ProjectPath;
      }
    }

    public ITypeMetadataFactory MetadataFactory
    {
      get
      {
        return this.actualProjectContext.MetadataFactory;
      }
    }

    public ICollection<IProjectDocument> Documents
    {
      get
      {
        return this.actualProjectContext.Documents;
      }
    }

    public IProjectDocument Application
    {
      get
      {
        return this.actualProjectContext.Application;
      }
    }

    public IProjectDocument LocalApplication
    {
      get
      {
        return this.actualProjectContext.LocalApplication;
      }
    }

    public string ProjectName
    {
      get
      {
        return this.actualProjectContext.ProjectName;
      }
    }

    public ObservableCollection<IProjectFont> ProjectFonts
    {
      get
      {
        return this.actualProjectContext.ProjectFonts;
      }
    }

    public IFontResolver FontResolver
    {
      get
      {
        return this.actualProjectContext.FontResolver;
      }
    }

    public FrameworkName TargetFramework
    {
      get
      {
        return this.actualProjectContext.TargetFramework;
      }
    }

    public IPlatform Platform
    {
      get
      {
        return this.actualProjectContext.Platform;
      }
    }

    public IDocumentRoot ApplicationRoot
    {
      get
      {
        return this.actualProjectContext.ApplicationRoot;
      }
    }

    event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChangedEarly
    {
      add
      {
        this.actualProjectContext.TypesChangedEarly += value;
      }
      remove
      {
        this.actualProjectContext.TypesChangedEarly -= value;
      }
    }

    event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChanged
    {
      add
      {
        this.actualProjectContext.TypesChanged += value;
      }
      remove
      {
        this.actualProjectContext.TypesChanged -= value;
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentOpened
    {
      add
      {
        this.actualProjectContext.DocumentOpened += value;
      }
      remove
      {
        this.actualProjectContext.DocumentOpened -= value;
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosing
    {
      add
      {
        this.actualProjectContext.DocumentClosing += value;
      }
      remove
      {
        this.actualProjectContext.DocumentClosing -= value;
      }
    }

    event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosed
    {
      add
      {
        this.actualProjectContext.DocumentClosed += value;
      }
      remove
      {
        this.actualProjectContext.DocumentClosed -= value;
      }
    }

    public TypeReflectingProjectContext(IProjectContext actualProjectContext)
    {
      this.actualProjectContext = actualProjectContext;
    }

    public IType GetType(Type type)
    {
      if (type == (Type) null)
        return (IType) null;
      IType type1;
      if (this.typeCache.TryGetValue(type, out type1))
        return type1;
      DesignTypeResult designType = this.Generator.GetDesignType(type);
      if (designType.IsFailed)
        return this.actualProjectContext.GetType(type);
      IType type2 = this.actualProjectContext.GetType(designType.SourceType);
      if (designType.SourceType == designType.DesignType)
        return type2;
      type1 = (IType) new TypeReflectingProjectContext.DesignType(this.actualProjectContext.GetType(designType.DesignType), type2, (ITypeResolver) this);
      this.typeCache[designType.SourceType] = type1;
      this.typeCache[designType.DesignType] = type1;
      return type1;
    }

    public IType GetType(IXmlNamespace xmlNamespace, string typeName)
    {
      IType type = this.actualProjectContext.GetType(xmlNamespace, typeName);
      if (type != null && type.RuntimeType != (Type) null)
        type = this.GetType(type.RuntimeType);
      return type;
    }

    public IType GetType(string assemblyName, string typeName)
    {
      IType type = this.actualProjectContext.GetType(assemblyName, typeName);
      if (type != null && type.RuntimeType != (Type) null)
        type = this.GetType(type.RuntimeType);
      return type;
    }

    public IType GetType(IAssembly assembly, string typeName)
    {
      return this.actualProjectContext.GetType(assembly, typeName);
    }

    public IType ResolveType(ITypeId typeId)
    {
      IType type = typeId as IType;
      if (type == null)
        return this.actualProjectContext.ResolveType(typeId);
      if (type.RuntimeType == (Type) null)
        return this.actualProjectContext.ResolveType(typeId);
      if (type is TypeReflectingProjectContext.DesignType)
        return type;
      return this.GetType(type.RuntimeType);
    }

    public IProperty ResolveProperty(IPropertyId propertyId)
    {
      return this.actualProjectContext.ResolveProperty(propertyId);
    }

    public bool InTargetAssembly(IType typeId)
    {
      if (typeId.RuntimeType != (Type) null && typeId.RuntimeType.Assembly.Equals((object) RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly))
        return true;
      return this.actualProjectContext.InTargetAssembly(typeId);
    }

    public bool IsCapabilitySet(PlatformCapability capability)
    {
      return this.actualProjectContext.IsCapabilitySet(capability);
    }

    public object GetCapabilityValue(PlatformCapability capability)
    {
      return this.actualProjectContext.GetCapabilityValue(capability);
    }

    public IProjectDocument GetDocument(IDocumentRoot documentRoot)
    {
      return this.actualProjectContext.GetDocument(documentRoot);
    }

    public IProjectDocument GetDocument(IDocumentLocator documentLocator)
    {
      return this.actualProjectContext.GetDocument(documentLocator);
    }

    public IProjectDocument OpenDocument(string path)
    {
      return this.actualProjectContext.OpenDocument(path);
    }

    public IAssembly GetDesignAssembly(IAssembly assembly)
    {
      return this.actualProjectContext.GetDesignAssembly(assembly);
    }

    public IAssembly GetAssembly(string assemblyName)
    {
      return this.actualProjectContext.GetAssembly(assemblyName);
    }

    public bool EnsureAssemblyReferenced(string assemblyPath)
    {
      return this.actualProjectContext.EnsureAssemblyReferenced(assemblyPath);
    }

    public string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
    {
      return this.actualProjectContext.MakeResourceReference(resourceReference, referringDocument);
    }

    public Uri MakeDesignTimeUri(Uri uri, string documentUrl)
    {
      return this.actualProjectContext.MakeDesignTimeUri(uri, documentUrl);
    }

    public IDocumentRoot GetDocumentRoot(string path)
    {
      return this.actualProjectContext.GetDocumentRoot(path);
    }

    public object GetService(Type serviceType)
    {
      if (serviceType.IsAssignableFrom(this.GetType()))
        return (object) this;
      return this.actualProjectContext.GetService(serviceType);
    }

    public bool IsTypeSupported(ITypeId type)
    {
      return this.actualProjectContext.IsTypeSupported(type);
    }

    public bool IsTypeInSolution(IType type)
    {
      return this.actualProjectContext.IsTypeInSolution(type);
    }

    private class DesignType : IType, IMember, ITypeId, IMemberId, ITemplateSerializationType
    {
      private Dictionary<ReferenceStep, TypeReflectingProjectContext.DesignTypeProperty> designProperties = new Dictionary<ReferenceStep, TypeReflectingProjectContext.DesignTypeProperty>();
      private Dictionary<MemberAccessTypes, ReadOnlyCollection<IProperty>> designPropertySets = new Dictionary<MemberAccessTypes, ReadOnlyCollection<IProperty>>();
      private IType designType;
      private IType runtimeType;
      private ITypeResolver resolver;

      public IPlatformMetadata PlatformMetadata
      {
        get
        {
          return this.designType.PlatformMetadata;
        }
      }

      public IAssembly Assembly
      {
        get
        {
          return this.RuntimeAssembly;
        }
      }

      public IAssembly RuntimeAssembly
      {
        get
        {
          return this.runtimeType.RuntimeAssembly;
        }
      }

      public TypeConverter TypeConverter
      {
        get
        {
          return this.designType.TypeConverter;
        }
      }

      public IType BaseType
      {
        get
        {
          return this.resolver.GetType(this.designType.RuntimeType.BaseType);
        }
      }

      public bool IsArray
      {
        get
        {
          return this.designType.IsArray;
        }
      }

      public bool IsInterface
      {
        get
        {
          return this.designType.IsInterface;
        }
      }

      public bool IsAbstract
      {
        get
        {
          return this.designType.IsAbstract;
        }
      }

      public bool IsGenericType
      {
        get
        {
          return this.designType.IsGenericType;
        }
      }

      public bool IsBinding
      {
        get
        {
          return this.designType.IsBinding;
        }
      }

      public bool IsResource
      {
        get
        {
          return this.designType.IsResource;
        }
      }

      public bool IsExpression
      {
        get
        {
          return this.designType.IsExpression;
        }
      }

      public string XamlSourcePath
      {
        get
        {
          return this.designType.XamlSourcePath;
        }
      }

      public IType ItemType
      {
        get
        {
          if (this.designType.ItemType == null)
            return (IType) null;
          return this.resolver.GetType(this.designType.ItemType.RuntimeType);
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
          return this.designType.Metadata;
        }
      }

      public IType NullableType
      {
        get
        {
          return this.designType.NullableType;
        }
      }

      public bool SupportsNullValues
      {
        get
        {
          return this.designType.SupportsNullValues;
        }
      }

      public Exception InitializationException
      {
        get
        {
          return this.designType.InitializationException;
        }
      }

      public Type RuntimeType
      {
        get
        {
          return this.designType.RuntimeType;
        }
      }

      public ITypeId MemberTypeId
      {
        get
        {
          return this.designType.MemberTypeId;
        }
      }

      public IType DeclaringType
      {
        get
        {
          return this.designType.DeclaringType;
        }
      }

      public MemberAccessType Access
      {
        get
        {
          return MemberAccessType.Public;
        }
      }

      public ITypeId DeclaringTypeId
      {
        get
        {
          return this.designType.DeclaringTypeId;
        }
      }

      public string FullName
      {
        get
        {
          return this.runtimeType.FullName;
        }
      }

      public string Name
      {
        get
        {
          return this.runtimeType.Name;
        }
      }

      public bool IsResolvable
      {
        get
        {
          return this.designType.IsResolvable;
        }
      }

      public MemberType MemberType
      {
        get
        {
          return this.designType.MemberType;
        }
      }

      public string UniqueName
      {
        get
        {
          return this.runtimeType.UniqueName;
        }
      }

      public IXmlNamespace XmlNamespace
      {
        get
        {
          return this.runtimeType.XmlNamespace;
        }
      }

      public string Namespace
      {
        get
        {
          return this.runtimeType.Namespace;
        }
      }

      public bool IsBuilt
      {
        get
        {
          return this.designType.IsBuilt;
        }
      }

      public DesignType(IType designType, IType runtimeType, ITypeResolver typeResolver)
      {
        this.designType = designType;
        this.runtimeType = runtimeType;
        this.resolver = typeResolver;
      }

      public override string ToString()
      {
        if (this.runtimeType == null)
          return "DesignType [uninitialized]";
        return "DesignType: " + this.runtimeType.Name;
      }

      public bool HasDefaultConstructor(bool supportInternal)
      {
        return this.designType.HasDefaultConstructor(supportInternal);
      }

      public IConstructorArgumentProperties GetConstructorArgumentProperties()
      {
        return this.designType.GetConstructorArgumentProperties();
      }

      public IList<IType> GetGenericTypeArguments()
      {
        List<IType> list = new List<IType>();
        foreach (IType type in (IEnumerable<IType>) this.designType.GetGenericTypeArguments())
          list.Add(this.resolver.GetType(type.RuntimeType));
        return (IList<IType>) list;
      }

      public IList<IConstructor> GetConstructors()
      {
        return this.designType.GetConstructors();
      }

      public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
      {
        ReadOnlyCollection<IProperty> readOnlyCollection1;
        if (this.designPropertySets.TryGetValue(access, out readOnlyCollection1))
          return (IEnumerable<IProperty>) readOnlyCollection1;
        IEnumerable<IProperty> properties = this.designType.GetProperties(access);
        List<IProperty> list = new List<IProperty>();
        foreach (IProperty property in properties)
        {
          ReferenceStep actualReferenceStep = property as ReferenceStep;
          if (actualReferenceStep != null)
          {
            TypeReflectingProjectContext.DesignTypeProperty createDesignProperty = this.GetOrCreateDesignProperty(actualReferenceStep);
            list.Add((IProperty) createDesignProperty);
          }
        }
        ReadOnlyCollection<IProperty> readOnlyCollection2 = new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
        this.designPropertySets[access] = readOnlyCollection2;
        return (IEnumerable<IProperty>) readOnlyCollection2;
      }

      private TypeReflectingProjectContext.DesignTypeProperty GetOrCreateDesignProperty(ReferenceStep actualReferenceStep)
      {
        TypeReflectingProjectContext.DesignTypeProperty designTypeProperty;
        if (!this.designProperties.TryGetValue(actualReferenceStep, out designTypeProperty))
        {
          IType type = this.resolver.GetType(actualReferenceStep.PropertyType.RuntimeType);
          designTypeProperty = new TypeReflectingProjectContext.DesignTypeProperty(actualReferenceStep, this, type);
          this.designProperties[actualReferenceStep] = designTypeProperty;
        }
        return designTypeProperty;
      }

      public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
      {
        return this.designType.GetEvents(access);
      }

      public bool IsInProject(ITypeResolver typeResolver)
      {
        if (!this.designType.RuntimeType.Assembly.Equals((object) RuntimeGeneratedTypesHelper.RuntimeGeneratedTypesAssembly.Assembly))
          return this.designType.IsInProject(typeResolver);
        return true;
      }

      public void InitializeClass()
      {
        this.designType.InitializeClass();
      }

      public IMember Clone(ITypeResolver typeResolver)
      {
        return this.designType.Clone(typeResolver);
      }

      public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
      {
        IMember member = (IMember) this.designType.GetMember(memberTypes, memberName, access);
        ReferenceStep actualReferenceStep = member as ReferenceStep;
        if (actualReferenceStep != null)
          return (IMemberId) this.GetOrCreateDesignProperty(actualReferenceStep);
        return (IMemberId) member;
      }

      public bool IsAssignableFrom(ITypeId type)
      {
        if (type != this && !this.runtimeType.IsAssignableFrom(type))
          return this.designType.IsAssignableFrom(type);
        return true;
      }

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        TypeReflectingProjectContext.DesignType designType = obj as TypeReflectingProjectContext.DesignType;
        return designType != null && this.designType.Equals((object) designType.designType) && this.runtimeType.Equals((object) designType.runtimeType);
      }

      public override int GetHashCode()
      {
        return this.designType.GetHashCode() ^ this.runtimeType.GetHashCode();
      }
    }

    [DebuggerDisplay("_d.{actualProperty}")]
    private class DesignTypeProperty : ReferenceStep
    {
      private ReferenceStep actualProperty;

      public override Type TargetType
      {
        get
        {
          return this.actualProperty.TargetType;
        }
      }

      public override bool IsAttachable
      {
        get
        {
          return this.actualProperty.IsAttachable;
        }
      }

      public override bool IsResolvable
      {
        get
        {
          return this.actualProperty.IsResolvable;
        }
      }

      public override ITypeId MemberTypeId
      {
        get
        {
          return this.actualProperty.MemberTypeId;
        }
      }

      public override MemberType MemberType
      {
        get
        {
          return this.actualProperty.MemberType;
        }
      }

      public override MemberAccessType ReadAccess
      {
        get
        {
          return this.actualProperty.ReadAccess;
        }
      }

      public override MemberAccessType WriteAccess
      {
        get
        {
          return this.actualProperty.WriteAccess;
        }
      }

      public DesignTypeProperty(ReferenceStep actualProperty, TypeReflectingProjectContext.DesignType designDeclaringType, IType designPropertyType)
        : base((IType) designDeclaringType, actualProperty.Name, designPropertyType, actualProperty.SortValue)
      {
        this.actualProperty = actualProperty;
      }

      public override object GetValue(object objToInspect)
      {
        return this.actualProperty.GetValue(objToInspect);
      }

      public override object SetValue(object target, object valueToSet)
      {
        return this.actualProperty.SetValue(target, valueToSet);
      }

      public override bool IsSet(object objToInspect)
      {
        return this.actualProperty.IsSet(objToInspect);
      }

      public override bool IsAnimated(object target)
      {
        return this.actualProperty.IsAnimated(target);
      }

      public override bool ShouldSerializeMethod(object objToInspect)
      {
        return this.actualProperty.ShouldSerializeMethod(objToInspect);
      }

      public override void ClearValue(object target)
      {
        this.actualProperty.ClearValue(target);
      }

      public override object[] GetCustomAttributes(Type attributeType, bool inherit)
      {
        return this.actualProperty.GetCustomAttributes(attributeType, inherit);
      }

      public override object GetDefaultValue(Type targetType)
      {
        return this.actualProperty.GetDefaultValue(targetType);
      }

      public override bool HasDefaultValue(Type targetType)
      {
        return this.actualProperty.HasDefaultValue(targetType);
      }
    }
  }
}

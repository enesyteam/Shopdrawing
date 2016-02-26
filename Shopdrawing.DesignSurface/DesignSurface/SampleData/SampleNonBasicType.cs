// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleNonBasicType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public abstract class SampleNonBasicType : SampleType, IType, IMember, ITypeId, IMemberId, ITypeMetadata, ITemplateSerializationType
  {
    protected static readonly ReadOnlyCollection<IPropertyId> emptyPropertyIds = new ReadOnlyCollection<IPropertyId>((IList<IPropertyId>) new List<IPropertyId>());
    protected static readonly ReadOnlyCollection<IProperty> emptyProperties = new ReadOnlyCollection<IProperty>((IList<IProperty>) new List<IProperty>());
    private string name;
    private string fullName;
    private SampleDataSet declaringDataSet;
    private Type designTimeType;

    public override string Name
    {
      get
      {
        return this.name;
      }
    }

    public SampleDataSet DeclaringDataSet
    {
      get
      {
        return this.declaringDataSet;
      }
    }

    public Type DesignTimeType
    {
      get
      {
        return this.designTimeType;
      }
      internal set
      {
        this.designTimeType = value;
      }
    }

    public override ITypeId TypeId
    {
      get
      {
        return (ITypeId) this;
      }
    }

    public override bool IsBasicType
    {
      get
      {
        return false;
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
        return this.declaringDataSet.ClrNamespace;
      }
    }

    public bool IsBuilt
    {
      get
      {
        return true;
      }
    }

    public ITypeId DeclaringTypeId
    {
      get
      {
        return (ITypeId) null;
      }
    }

    string IMemberId.Name
    {
      get
      {
        return this.name;
      }
    }

    public string FullName
    {
      get
      {
        return this.fullName;
      }
    }

    public string UniqueName
    {
      get
      {
        return this.fullName;
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
        return MemberType.Type;
      }
    }

    public IPlatformMetadata PlatformMetadata
    {
      get
      {
        return (IPlatformMetadata) this.declaringDataSet.Platform.Metadata;
      }
    }

    public IAssembly RuntimeAssembly
    {
      get
      {
        return this.declaringDataSet.ProjectContext.ProjectAssembly;
      }
    }

    public TypeConverter TypeConverter
    {
      get
      {
        return MetadataStore.GetTypeConverter(this.designTimeType);
      }
    }

    public abstract IType BaseType { get; }

    public bool IsArray
    {
      get
      {
        return this.designTimeType.IsArray;
      }
    }

    public bool IsInterface
    {
      get
      {
        return this.designTimeType.IsInterface;
      }
    }

    public bool IsAbstract
    {
      get
      {
        return this.designTimeType.IsAbstract;
      }
    }

    public bool IsBinding
    {
      get
      {
        return false;
      }
    }

    public bool IsResource
    {
      get
      {
        return false;
      }
    }

    public bool IsExpression
    {
      get
      {
        return false;
      }
    }

    public bool IsGenericType
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
        return (string) null;
      }
    }

    public abstract IType ItemType { get; }

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
        return (IType) this;
      }
    }

    public bool SupportsNullValues
    {
      get
      {
        return true;
      }
    }

    public Exception InitializationException
    {
      get
      {
        return (Exception) null;
      }
    }

    public Type RuntimeType
    {
      get
      {
        return this.designTimeType;
      }
    }

    public ITypeId MemberTypeId
    {
      get
      {
        return (ITypeId) null;
      }
    }

    public IType DeclaringType
    {
      get
      {
        return (IType) null;
      }
    }

    public MemberAccessType Access
    {
      get
      {
        return MemberAccessType.Public;
      }
    }

    public ITypeResolver TypeResolver
    {
      get
      {
        return (ITypeResolver) this.declaringDataSet.ProjectContext;
      }
      set
      {
      }
    }

    public bool IsNameScope
    {
      get
      {
        return false;
      }
    }

    public IProperty NameProperty
    {
      get
      {
        return (IProperty) DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.XNameProperty, (IPlatformMetadata) this.declaringDataSet.Platform.Metadata);
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

    public bool ShouldTrimSurroundingWhitespace
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

    public ReadOnlyCollection<IPropertyId> ContentProperties
    {
      get
      {
        return SampleNonBasicType.emptyPropertyIds;
      }
    }

    public abstract ReadOnlyCollection<IProperty> Properties { get; }

    public ReadOnlyCollection<IPropertyId> StyleProperties
    {
      get
      {
        return SampleNonBasicType.emptyPropertyIds;
      }
    }

    string ITemplateSerializationType.Namespace
    {
      get
      {
        return this.DesignTimeType.Namespace;
      }
    }

    IAssembly ITemplateSerializationType.Assembly
    {
      get
      {
        return RuntimeGeneratedTypesHelper.BlendDefaultAssembly;
      }
    }

    protected SampleNonBasicType(string name, SampleDataSet declaringDataSet)
    {
      this.name = name;
      this.declaringDataSet = declaringDataSet;
      this.fullName = this.declaringDataSet.ClrNamespace + "." + this.name;
    }

    public string GetUniqueNameForRename(string name)
    {
      string str = (string) null;
      if (this.Name != name)
        str = this.DeclaringDataSet.GetUniqueTypeName(name, this.Name);
      return str;
    }

    public void Rename(string newName)
    {
      if (this.name == newName)
        return;
      if (string.IsNullOrEmpty(newName))
        throw new InvalidOperationException(ExceptionStringTable.EmptyTypeName);
      if (this.declaringDataSet.GetSampleType(newName) != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.NameIsAlreadyInUse, new object[1]
        {
          (object) newName
        }));
      string oldName = this.name;
      this.name = newName;
      this.declaringDataSet.OnTypeRenamed(this, oldName);
    }

    public void Delete()
    {
      this.declaringDataSet.DeleteType(this);
    }

    public abstract IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access);

    public bool IsAssignableFrom(ITypeId type)
    {
      return this == type;
    }

    public IList<IType> GetGenericTypeArguments()
    {
      return (IList<IType>) null;
    }

    public IConstructorArgumentProperties GetConstructorArgumentProperties()
    {
      return (IConstructorArgumentProperties) null;
    }

    public bool HasDefaultConstructor(bool supportInternal)
    {
      return true;
    }

    public IList<IConstructor> GetConstructors()
    {
      return (IList<IConstructor>) ReadOnlyCollections<IConstructor>.Empty;
    }

    public abstract IEnumerable<IProperty> GetProperties(MemberAccessTypes access);

    public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
    {
      return (IEnumerable<IEvent>) ReadOnlyCollections<IEvent>.Empty;
    }

    public bool IsInProject(ITypeResolver typeResolver)
    {
      return true;
    }

    public void InitializeClass()
    {
    }

    public IMember Clone(ITypeResolver typeResolver)
    {
      return (IMember) this;
    }

    public bool IsNameProperty(IPropertyId propertyKey)
    {
      return DesignTimeProperties.XNameProperty.Equals((object) propertyKey);
    }

    public Type GetStylePropertyTargetType(IPropertyId propertyKey)
    {
      return (Type) null;
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.Name))
        return "Uninitialized " + base.ToString();
      return "$" + this.Name;
    }
  }
}

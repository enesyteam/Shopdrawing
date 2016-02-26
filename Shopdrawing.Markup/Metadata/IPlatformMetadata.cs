// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.IPlatformMetadata
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface IPlatformMetadata : IMetadataResolver
  {
    FrameworkName TargetFramework { get; }

    FrameworkName RuntimeFramework { get; }

    IKnownProperties KnownProperties { get; }

    IKnownTypes KnownTypes { get; }

    IType CreateUnknownType(ITypeResolver typeResolver, IAssembly assembly, string clrNamespace, string typeName);

    IType CreateUnknownType(ITypeResolver typeResolver, IXmlNamespace xmlNamespace, string typeName);

    IMember CreateUnknownMember(IType declaringType, MemberType memberType, string memberName);

    bool IsDesignToolType(Type type);

    bool IsNullType(ITypeId type);

    bool IsSupported(ITypeResolver typeResolver, ITypeId type);

    TypeConverter GetTypeConverter(MemberInfo memberInfo);

    Attribute[] GetCustomAttributes(MemberInfo memberInfo);

    Attribute[] GetCustomAttributes(MemberInfo memberInfo, bool inherit);

    bool IsCapabilitySet(PlatformCapability capability);

    object GetCapabilityValue(PlatformCapability capability);

    IType GetDesignTimeType(ITypeResolver typeResolver, IXmlNamespace xmlNamespace, string typeName);

    IProperty GetDesignTimeProperty(string propertyName, IType targetType);

    IEnumerable<IProperty> GetProxyProperties(ITypeResolver typeResolver);

    IType GetContentWrapperType(ITypeResolver typeResolver, IType parentType, IType childType);

    bool IsTrimSurroundingWhitespace(IType type);

    bool GetIsTypeItsOwnNameScope(ITypeId type);
  }
}

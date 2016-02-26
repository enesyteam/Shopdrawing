// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.IType
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface IType : IMember, ITypeId, IMemberId
  {
    IPlatformMetadata PlatformMetadata { get; }

    TypeConverter TypeConverter { get; }

    IType BaseType { get; }

    bool IsArray { get; }

    bool IsInterface { get; }

    bool IsAbstract { get; }

    bool IsGenericType { get; }

    bool IsBinding { get; }

    bool IsResource { get; }

    bool IsExpression { get; }

    string XamlSourcePath { get; }

    IType ItemType { get; }

    IType NearestResolvedType { get; }

    ITypeMetadata Metadata { get; }

    IType NullableType { get; }

    bool SupportsNullValues { get; }

    Exception InitializationException { get; }

    Type RuntimeType { get; }

    IAssembly RuntimeAssembly { get; }

    bool HasDefaultConstructor(bool supportInternal);

    IConstructorArgumentProperties GetConstructorArgumentProperties();

    IList<IType> GetGenericTypeArguments();

    IList<IConstructor> GetConstructors();

    IEnumerable<IProperty> GetProperties(MemberAccessTypes access);

    IEnumerable<IEvent> GetEvents(MemberAccessTypes access);

    bool IsInProject(ITypeResolver typeResolver);

    void InitializeClass();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.IProperty
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface IProperty : IMember, IPropertyId, IMemberId
  {
    MemberAccessType ReadAccess { get; }

    MemberAccessType WriteAccess { get; }

    bool IsProxy { get; }

    bool IsAttachable { get; }

    TypeConverter TypeConverter { get; }

    AttributeCollection Attributes { get; }

    bool ShouldSerialize { get; }

    IType PropertyType { get; }

    bool HasDefaultValue(Type targetType);

    object GetDefaultValue(Type targetType);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.ITypeMetadata
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
  public interface ITypeMetadata
  {
    ITypeResolver TypeResolver { get; set; }

    bool IsNameScope { get; }

    IProperty NameProperty { get; }

    IProperty DefaultContentProperty { get; }

    IPropertyId ResourcesProperty { get; }

    IPropertyId ImplicitDictionaryKeyProperty { get; }

    bool IsWhitespaceSignificant { get; }

    bool ShouldTrimSurroundingWhitespace { get; }

    bool SupportsInlineXml { get; }

    ReadOnlyCollection<IPropertyId> ContentProperties { get; }

    ReadOnlyCollection<IProperty> Properties { get; }

    ReadOnlyCollection<IPropertyId> StyleProperties { get; }

    bool IsNameProperty(IPropertyId propertyKey);

    Type GetStylePropertyTargetType(IPropertyId propertyKey);
  }
}

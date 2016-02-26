// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentPrimitiveNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentPrimitiveNode : DocumentNode
  {
    private IDocumentNodeValue value;
    private IMember valueConverterProvider;
    private static TypeConverter objectTypeConverter;

    public IDocumentNodeValue Value
    {
      get
      {
        return this.value;
      }
    }

    public TypeConverter ValueConverter
    {
      get
      {
        if (this.valueConverterProvider == null)
          this.valueConverterProvider = this.GetValueConverterProvider();
        IType type;
        if ((type = this.valueConverterProvider as IType) != null)
          return type.TypeConverter;
        IProperty property;
        if ((property = this.valueConverterProvider as IProperty) != null)
          return property.TypeConverter;
        return (TypeConverter) null;
      }
    }

    public DocumentPrimitiveNode(IDocumentContext context, ITypeId typeId, object value)
      : this(context, typeId, value != null ? (IDocumentNodeValue) new DocumentPrimitiveNode.UnsitedValue(value) : (IDocumentNodeValue) null)
    {
    }

    public DocumentPrimitiveNode(IDocumentContext context, ITypeId typeId, IDocumentNodeValue value)
      : base(context, typeId)
    {
      if (value == null && !this.Type.SupportsNullValues)
        throw new ArgumentException(ExceptionStringTable.DocumentNodeValueTypeValueIsNull);
      this.value = value;
    }

    public override DocumentNode Clone(IDocumentContext context)
    {
      IDocumentNodeValue documentNodeValue = this.value != null ? this.value.Clone(context) : (IDocumentNodeValue) null;
      ITypeId typeId = (ITypeId) this.Type.Clone(context.TypeResolver);
      DocumentPrimitiveNode documentPrimitiveNode = new DocumentPrimitiveNode(context, typeId, documentNodeValue);
      if (this.SourceContext != null)
        documentPrimitiveNode.SourceContext = this.SourceContext.FreezeText(true);
      return (DocumentNode) documentPrimitiveNode;
    }

    public override bool Equals(DocumentNode other)
    {
      DocumentPrimitiveNode documentPrimitiveNode = other as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null && this.Type.Equals((object) documentPrimitiveNode.Type))
        return object.Equals((object) this.value, (object) documentPrimitiveNode.value);
      return false;
    }

    public override int GetHashCodeInternal()
    {
      if (this.value != null)
        return this.value.GetHashCode();
      return this.Type.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", new object[2]
      {
        (object) this.Type.Name,
        (object) (this.value != null ? this.value.ToString() : "null")
      });
    }

    internal override void SetSite(DocumentCompositeNode parent, IProperty propertyKey, int childIndex)
    {
      base.SetSite(parent, propertyKey, childIndex);
      if (parent != null)
      {
        DocumentPrimitiveNode.UnsitedValue unsitedValue = this.value as DocumentPrimitiveNode.UnsitedValue;
        if (unsitedValue == null)
          return;
        TypeConverter typeConverter = this.ValueConverter;
        if (typeConverter == null)
        {
          if (DocumentPrimitiveNode.objectTypeConverter == null)
            DocumentPrimitiveNode.objectTypeConverter = this.PlatformMetadata.GetTypeConverter((MemberInfo) typeof (object));
          typeConverter = DocumentPrimitiveNode.objectTypeConverter;
        }
        this.value = (IDocumentNodeValue) new DocumentNodeStringValue(typeConverter.ConvertToInvariantString(unsitedValue.Value));
      }
      else
        this.valueConverterProvider = (IMember) null;
    }

    private IMember GetValueConverterProvider()
    {
      IProperty valueProperty = this.GetValueProperty();
      if (valueProperty == null || !this.Type.IsAssignableFrom((ITypeId) valueProperty.PropertyType))
        return (IMember) this.Type;
      if (valueProperty.TypeConverter != null)
        return (IMember) valueProperty;
      return (IMember) valueProperty.PropertyType;
    }

    public static string GetValueAsString(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null && documentPrimitiveNode.TypeResolver.PlatformMetadata.KnownTypes.String.IsAssignableFrom((ITypeId) documentPrimitiveNode.Type))
      {
        DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.value as DocumentNodeStringValue;
        if (documentNodeStringValue != null)
          return documentNodeStringValue.Value;
        DocumentPrimitiveNode.UnsitedValue unsitedValue = documentPrimitiveNode.value as DocumentPrimitiveNode.UnsitedValue;
        if (unsitedValue != null)
          return (string) unsitedValue.Value;
      }
      return (string) null;
    }

    public static IMember GetValueAsMember(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null)
        return (IMember) null;
      DocumentNodeMemberValue documentNodeMemberValue = documentPrimitiveNode.Value as DocumentNodeMemberValue;
      if (documentNodeMemberValue == null)
        return (IMember) null;
      return documentNodeMemberValue.Member;
    }

    public static IType GetValueAsType(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null)
        return (IType) null;
      DocumentNodeMemberValue documentNodeMemberValue = documentPrimitiveNode.Value as DocumentNodeMemberValue;
      if (documentNodeMemberValue == null)
        return (IType) null;
      return documentNodeMemberValue.Member as IType;
    }

    public static object GetValueAsObject(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode == null)
        return (object) null;
      DocumentPrimitiveNode.UnsitedValue unsitedValue = documentPrimitiveNode.Value as DocumentPrimitiveNode.UnsitedValue;
      if (unsitedValue == null)
        return (object) null;
      return unsitedValue.Value;
    }

    public T GetValue<T>()
    {
      DocumentNodeStringValue documentNodeStringValue = this.value as DocumentNodeStringValue;
      if (documentNodeStringValue != null)
      {
        string str = documentNodeStringValue.Value;
        if (str != null)
          return this.ConvertFromString<T>(str);
      }
      return default (T);
    }

    public Uri GetUriValue()
    {
      DocumentNodeStringValue documentNodeStringValue = this.value as DocumentNodeStringValue;
      if (documentNodeStringValue != null)
      {
        string uriString = documentNodeStringValue.Value;
        Uri result;
        if (!string.IsNullOrEmpty(uriString) && Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out result))
          return result;
      }
      return (Uri) null;
    }

    public T ConvertFromString<T>(string value)
    {
      object result;
      if (this.ConvertFromString(typeof (T), value, out result))
        return (T) result;
      return default (T);
    }

    private bool ConvertFromString(Type type, string value, out object result)
    {
      if (type == typeof (string))
      {
        result = (object) value;
        return true;
      }
      TypeConverter typeConverter = this.PlatformMetadata.GetTypeConverter((MemberInfo) type);
      if (typeConverter.CanConvertFrom(typeof (string)))
      {
        try
        {
          result = typeConverter.ConvertFromInvariantString(value);
          return true;
        }
        catch
        {
        }
      }
      result = (object) null;
      return false;
    }

    public static bool IsNull(DocumentNode node)
    {
      DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null)
        return documentPrimitiveNode.Value == null;
      return false;
    }

    private sealed class UnsitedValue : IDocumentNodeValue
    {
      private readonly object value;

      public object Value
      {
        get
        {
          return this.value;
        }
      }

      public UnsitedValue(object value)
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.value = value;
      }

      IDocumentNodeValue IDocumentNodeValue.Clone(IDocumentContext documentContext)
      {
        return (IDocumentNodeValue) new DocumentPrimitiveNode.UnsitedValue(this.value);
      }

      public override string ToString()
      {
        return this.value.ToString();
      }
    }
  }
}

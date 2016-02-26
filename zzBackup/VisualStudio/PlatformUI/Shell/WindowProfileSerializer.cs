// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.WindowProfileSerializer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xml;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  public class WindowProfileSerializer
  {
    private Dictionary<string, string> namespaceAssembly = new Dictionary<string, string>();
    private Dictionary<string, string> namespacePrefix = new Dictionary<string, string>();
    private const string XamlNull = "Null";
    private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    public WindowProfileSerializerMode Mode { get; set; }

    public void MapNamespaceToAssembly(string namespaceName, string assemblyName, string prefix)
    {
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.ThrowIfNullOrEmpty(namespaceName, "No namespace provide to map");
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.ThrowIfNullOrEmpty(assemblyName, "The assemblyName must be specified");
      Microsoft.VisualStudio.PlatformUI.ExtensionMethods.ThrowIfNullOrEmpty(prefix, "The prefix must be specified");
      this.namespaceAssembly.Add(namespaceName, assemblyName);
      this.namespacePrefix.Add(namespaceName, prefix);
    }

    public void Serialize(object element, Stream stream)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (WindowProfileSerializer.IsSequenceType(element.GetType()))
        throw new ArgumentException("Root serialized element must not be a sequence type", "element");
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        CheckCharacters = false,
        CloseOutput = false,
        Indent = false,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = true,
        Encoding = Encoding.UTF8
      };
      using (XmlWriter writer = XmlWriter.Create(stream, settings))
        this.SerializeInternal(element, writer, true);
    }

    private void SerializeInternal(object element, XmlWriter writer, bool isRootElement)
    {
      if (element == null)
        return;
      ICustomXmlSerializer customXmlSerializer = (ICustomXmlSerializer) null;
      if (this.Mode == WindowProfileSerializerMode.Custom)
      {
        ICustomXmlSerializable customXmlSerializable = element as ICustomXmlSerializable;
        if (customXmlSerializable != null)
        {
          customXmlSerializer = customXmlSerializable.CreateSerializer();
          if (customXmlSerializer == null)
            return;
        }
      }
      Type type = element.GetType();
      if (this.Mode == WindowProfileSerializerMode.Reflection && WindowProfileSerializer.IsTypeNonSerializable(type))
        return;
      if (WindowProfileSerializer.IsSequenceType(type))
      {
        this.SerializeSequence(element as IEnumerable, writer);
      }
      else
      {
        if (this.GetPrefix(type) != null)
          writer.WriteStartElement(this.GetPrefix(type), type.Name, this.GetClrNamespace(type));
        else
          writer.WriteStartElement(type.Name, this.GetClrNamespace(type));
        if (isRootElement)
          this.WriteNamespaceDeclarations(writer);
        object contentPropertyValue = (object) null;
        IEnumerable<KeyValuePair<string, object>> enumerable;
        if (customXmlSerializer != null)
        {
          customXmlSerializer.WriteXmlAttributes(writer);
          enumerable = customXmlSerializer.GetNonContentPropertyValues();
          contentPropertyValue = customXmlSerializer.Content;
        }
        else
          enumerable = (IEnumerable<KeyValuePair<string, object>>) WindowProfileSerializer.GetChildPropertiesAndContent(element, writer, type, ref contentPropertyValue);
        foreach (KeyValuePair<string, object> keyValuePair in enumerable)
        {
          string localName = type.Name + "." + keyValuePair.Key;
          if (this.GetPrefix(type) != null)
            writer.WriteStartElement(this.GetPrefix(type), localName, this.GetClrNamespace(type));
          else
            writer.WriteStartElement(localName, this.GetClrNamespace(type));
          this.SerializeInternal(keyValuePair.Value, writer, false);
          writer.WriteEndElement();
        }
        if (contentPropertyValue != null)
          this.SerializeInternal(contentPropertyValue, writer, false);
        writer.WriteEndElement();
      }
    }

    private static bool IsTypeNonSerializable(Type type)
    {
      return WindowProfileSerializer.GetAttribute<NonXamlSerializedAttribute>((MemberInfo) type) != null;
    }

    protected void WriteNamespaceDeclarations(XmlWriter writer)
    {
      writer.WriteAttributeString("xmlns", "x", (string) null, "http://schemas.microsoft.com/winfx/2006/xaml");
      foreach (string namespaceName in this.namespacePrefix.Keys)
        writer.WriteAttributeString("xmlns", this.namespacePrefix[namespaceName], (string) null, WindowProfileSerializer.GetClrNamespace(namespaceName, this.namespaceAssembly[namespaceName]));
    }

    private void SerializeSequence(IEnumerable element, XmlWriter writer)
    {
      foreach (object element1 in element)
        this.SerializeInternal(element1, writer, false);
    }

    private string GetClrNamespace(Type type)
    {
      string assemblyName = (string) null;
      if (!this.namespaceAssembly.TryGetValue(type.Namespace, out assemblyName))
        assemblyName = type.Assembly.GetName().Name;
      return WindowProfileSerializer.GetClrNamespace(type.Namespace, assemblyName);
    }

    private static string GetClrNamespace(string namespaceName, string assemblyName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "clr-namespace:{0};assembly={1}", new object[2]
      {
        (object) namespaceName,
        (object) assemblyName
      });
    }

    private string GetPrefix(Type type)
    {
      string str = (string) null;
      this.namespacePrefix.TryGetValue(type.Namespace, out str);
      return str;
    }

    private static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : class
    {
      object[] customAttributes = member.GetCustomAttributes(typeof (TAttribute), true);
      if (customAttributes.Length > 0)
        return (TAttribute) customAttributes[0];
      return default (TAttribute);
    }

    private static bool IsPropertySerializable(PropertyInfo property)
    {
      DesignerSerializationVisibilityAttribute attribute = WindowProfileSerializer.GetAttribute<DesignerSerializationVisibilityAttribute>((MemberInfo) property);
      if (attribute != null && attribute.Visibility == DesignerSerializationVisibility.Hidden || !property.CanRead)
        return false;
      if (!property.CanWrite)
        return WindowProfileSerializer.IsSequenceType(property.PropertyType);
      return true;
    }

    private static bool IsSequenceType(Type type)
    {
      return typeof (IList).IsAssignableFrom(type);
    }

    private static bool IsContentProperty(ContentPropertyAttribute attribute, PropertyInfo property)
    {
      if (attribute == null)
        return false;
      return attribute.Name == property.Name;
    }

    private static bool IsDefaultValue(PropertyInfo property, object value)
    {
      DefaultValueAttribute attribute = WindowProfileSerializer.GetAttribute<DefaultValueAttribute>((MemberInfo) property);
      if (attribute == null)
        return false;
      if (object.ReferenceEquals(value, attribute.Value))
        return true;
      if (value != null)
        return value.Equals(attribute.Value);
      return false;
    }

    private static void WriteAttributeValue(TypeConverter typeConverter, object value, XmlWriter writer)
    {
      if (value == null)
      {
        writer.WriteString("{");
        writer.WriteQualifiedName("Null", "http://schemas.microsoft.com/winfx/2006/xaml");
        writer.WriteString("}");
      }
      else
      {
        string text = typeConverter.ConvertToInvariantString(value);
        if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
          text = "{}" + text;
        writer.WriteString(text);
      }
    }

    private ICustomXmlSerializer TryGetCustomSerializer(object element)
    {
      if (this.Mode != WindowProfileSerializerMode.Custom)
        return (ICustomXmlSerializer) null;
      ICustomXmlSerializable customXmlSerializable = element as ICustomXmlSerializable;
      if (customXmlSerializable == null)
        return (ICustomXmlSerializer) null;
      return customXmlSerializable.CreateSerializer();
    }

    private static Dictionary<string, object> GetChildPropertiesAndContent(object element, XmlWriter writer, Type type, ref object contentPropertyValue)
    {
      ContentPropertyAttribute attribute = WindowProfileSerializer.GetAttribute<ContentPropertyAttribute>((MemberInfo) type);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (WindowProfileSerializer.IsPropertySerializable(property))
        {
          object obj = property.GetValue(element, (object[]) null);
          if (!WindowProfileSerializer.IsDefaultValue(property, obj))
          {
            if (WindowProfileSerializer.IsContentProperty(attribute, property))
            {
              contentPropertyValue = obj;
            }
            else
            {
              TypeConverter converter = TypeDescriptor.GetConverter(obj == null ? property.PropertyType : obj.GetType());
              if (converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)))
              {
                writer.WriteStartAttribute(property.Name);
                WindowProfileSerializer.WriteAttributeValue(converter, obj, writer);
                writer.WriteEndAttribute();
              }
              else
                dictionary[property.Name] = obj;
            }
          }
        }
      }
      return dictionary;
    }
  }
}

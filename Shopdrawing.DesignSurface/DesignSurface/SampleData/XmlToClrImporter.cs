// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.XmlToClrImporter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class XmlToClrImporter
  {
    private List<XmlToClrImporter.ClrTypeDescription> compositeTypes = new List<XmlToClrImporter.ClrTypeDescription>();
    private List<XmlToClrImporter.ClrTypeDescription> leafTypes = new List<XmlToClrImporter.ClrTypeDescription>();
    private List<IType> collectionTypes = new List<IType>();
    private IXmlToClrAdapter xmlToClrAdapter;
    private XmlToClrImporter.ClrTypeDescription rootType;
    private XmlToClrImporter.ClrTypeDescription stringTypeDescription;

    private XmlToClrImporter(IXmlToClrAdapter resolver)
    {
      this.xmlToClrAdapter = resolver;
      this.stringTypeDescription = XmlToClrImporter.ClrTypeDescription.CreateLeaf(this.xmlToClrAdapter.StringType);
      this.leafTypes.Add(this.stringTypeDescription);
    }

    public static void Import(string xmlFileName, IXmlToClrAdapter resolver)
    {
      new XmlToClrImporter(resolver).ImportFromFile(xmlFileName);
    }

    private void ImportFromFile(string xmlFileName)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XmlToClrImport);
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(xmlFileName);
        XmlElement element1 = (XmlElement) Enumerable.FirstOrDefault<XmlNode>(Enumerable.OfType<XmlNode>((IEnumerable) xmlDocument.ChildNodes), (Func<XmlNode, bool>) (node => node is XmlElement));
        if (element1 == null || !this.xmlToClrAdapter.ShouldProcessElement(element1.LocalName, element1.Prefix, element1.NamespaceURI))
          throw new InvalidDataException(ExceptionStringTable.EmptyXmlDocument);
        XmlToClrImporter.FilteredElement element2 = new XmlToClrImporter.FilteredElement(element1, this.xmlToClrAdapter);
        this.xmlToClrAdapter.Initialize(element2.Name);
        this.rootType = this.GetOrCreateCompositeDescription(element2.Name);
        if (!string.IsNullOrEmpty(this.xmlToClrAdapter.ExpectedRootTypeName) && this.xmlToClrAdapter.ExpectedRootTypeName != this.rootType.Name)
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.SampleDataXmlReimportWrongRootType, new object[2]
          {
            (object) this.rootType.Name,
            (object) this.xmlToClrAdapter.ExpectedRootTypeName
          }));
        this.ParseType(this.rootType, element2);
        this.NormalizeTypeDescriptions();
        this.CreateClrTypes();
        DocumentCompositeNode compositeNode = this.xmlToClrAdapter.CreateCompositeNode(this.rootType.Type);
        if (compositeNode == null)
          return;
        this.ParseValues(compositeNode, element2);
        this.xmlToClrAdapter.Finalize(true);
      }
      finally
      {
        this.xmlToClrAdapter.Finalize(false);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XmlToClrImport);
      }
    }

    private void ParseType(XmlToClrImporter.ClrTypeDescription description, XmlToClrImporter.FilteredElement element)
    {
      description.ResetPropertyOccurenceCounts();
      if (element.HasAttributes)
      {
        foreach (XmlAttribute xmlAttribute in element.Attributes)
        {
          string clrName = this.xmlToClrAdapter.GetClrName(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.NamespaceURI);
          this.UpdateAttributeValueType(description, clrName, xmlAttribute.Value);
        }
      }
      if (!element.HasChildElements)
        return;
      foreach (XmlElement element1 in element.ChildElements)
      {
        XmlToClrImporter.FilteredElement element2 = new XmlToClrImporter.FilteredElement(element1, this.xmlToClrAdapter);
        XmlToClrImporter.ClrPropertyDescription propertyDescription = description[element2.Name];
        XmlToClrImporter.ClrTypeDescription existingType = (XmlToClrImporter.ClrTypeDescription) null;
        if (propertyDescription != null)
        {
          propertyDescription.CountOccurence();
          existingType = propertyDescription.PropertyType;
        }
        XmlToClrImporter.ClrTypeDescription clrTypeDescription;
        if (element2.IsLeaf)
        {
          if (existingType != this.stringTypeDescription)
          {
            string leafValue = element2.LeafValue;
            if (!string.IsNullOrEmpty(leafValue))
              clrTypeDescription = this.GetOrCreateLeafDescription(this.xmlToClrAdapter.InferValueType(leafValue));
            else if (existingType == null)
              clrTypeDescription = this.GetOrCreateEmptyDescription(element2.Name);
            else
              continue;
          }
          else
            continue;
        }
        else
        {
          clrTypeDescription = this.GetOrCreateCompositeDescription(element2.Name);
          this.ParseType(clrTypeDescription, element2);
        }
        if (propertyDescription == null)
        {
          propertyDescription = description[element2.Name];
          if (propertyDescription != null)
            existingType = propertyDescription.PropertyType;
        }
        XmlToClrImporter.ClrTypeDescription description1 = this.ReconcilePropertyType(existingType, clrTypeDescription);
        if (propertyDescription == null)
          description.AddProperty(element2.Name, description1);
        else
          propertyDescription.PropertyType = description1;
      }
    }

    private XmlToClrImporter.ClrTypeDescription ReconcilePropertyType(XmlToClrImporter.ClrTypeDescription existingType, XmlToClrImporter.ClrTypeDescription newType)
    {
      if (existingType == newType)
        return existingType;
      if (existingType == null || existingType == XmlToClrImporter.ClrTypeDescription.EmptyAttribute)
        return newType;
      if (newType.Category == XmlToClrImporter.ClrTypeCategory.Empty)
        return existingType;
      if (existingType.Category == XmlToClrImporter.ClrTypeCategory.Empty)
      {
        foreach (XmlToClrImporter.ClrTypeDescription clrTypeDescription in this.compositeTypes)
        {
          if (clrTypeDescription.Category == XmlToClrImporter.ClrTypeCategory.Composite)
            clrTypeDescription.Properties.ForEach((Action<XmlToClrImporter.ClrPropertyDescription>) (prop =>
            {
              if (prop.PropertyType != existingType)
                return;
              prop.PropertyType = newType;
            }));
        }
        this.compositeTypes.Remove(existingType);
        return newType;
      }
      if (existingType.Category == XmlToClrImporter.ClrTypeCategory.Leaf != (newType.Category == XmlToClrImporter.ClrTypeCategory.Leaf))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.XmlOneNodeUsageNotSupported, new object[1]
        {
          (object) (existingType.Category != XmlToClrImporter.ClrTypeCategory.Leaf ? existingType.Name : newType.Name)
        }));
      return this.stringTypeDescription;
    }

    private void UpdateAttributeValueType(XmlToClrImporter.ClrTypeDescription type, string name, string value)
    {
      XmlToClrImporter.ClrPropertyDescription propertyDescription = type[name];
      XmlToClrImporter.ClrTypeDescription existingType = (XmlToClrImporter.ClrTypeDescription) null;
      if (propertyDescription != null)
      {
        propertyDescription.CountOccurence();
        existingType = propertyDescription.PropertyType;
      }
      if (existingType == this.stringTypeDescription)
        return;
      XmlToClrImporter.ClrTypeDescription newType = XmlToClrImporter.ClrTypeDescription.EmptyAttribute;
      if (!string.IsNullOrEmpty(value))
        newType = this.GetOrCreateLeafDescription(this.xmlToClrAdapter.InferValueType(value));
      XmlToClrImporter.ClrTypeDescription description = this.ReconcilePropertyType(existingType, newType);
      if (propertyDescription == null)
        type.AddProperty(name, description);
      else
        propertyDescription.PropertyType = description;
    }

    private XmlToClrImporter.ClrTypeDescription GetOrCreateLeafDescription(IType leafType)
    {
      XmlToClrImporter.ClrTypeDescription leaf = this.leafTypes.Find((Predicate<XmlToClrImporter.ClrTypeDescription>) (t => t.Type == leafType));
      if (leaf == null)
      {
        leaf = XmlToClrImporter.ClrTypeDescription.CreateLeaf(leafType);
        this.leafTypes.Add(leaf);
      }
      return leaf;
    }

    private XmlToClrImporter.ClrTypeDescription GetOrCreateEmptyDescription(string name)
    {
      XmlToClrImporter.ClrTypeDescription empty = this.compositeTypes.Find((Predicate<XmlToClrImporter.ClrTypeDescription>) (t => name == t.Name));
      if (empty == null)
      {
        empty = XmlToClrImporter.ClrTypeDescription.CreateEmpty(name);
        this.compositeTypes.Add(empty);
      }
      return empty;
    }

    private XmlToClrImporter.ClrTypeDescription GetOrCreateCompositeDescription(string name)
    {
      XmlToClrImporter.ClrTypeDescription composite = this.compositeTypes.Find((Predicate<XmlToClrImporter.ClrTypeDescription>) (t => name == t.Name));
      if (composite == null)
      {
        composite = XmlToClrImporter.ClrTypeDescription.CreateComposite(name);
        this.compositeTypes.Add(composite);
      }
      else if (composite.Category == XmlToClrImporter.ClrTypeCategory.Empty)
        composite.ConvertEmptyToComposite();
      else if (composite == this.rootType)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.XmlRecursiveRootTypeNotSupported, new object[1]
        {
          (object) this.rootType.Name
        }));
      return composite;
    }

    private void NormalizeTypeDescriptions()
    {
      this.rootType.ShouldDelete = false;
      foreach (XmlToClrImporter.ClrTypeDescription clrTypeDescription in this.compositeTypes)
      {
        if (clrTypeDescription.Category == XmlToClrImporter.ClrTypeCategory.Empty)
        {
          clrTypeDescription.ConvertEmptyToComposite();
        }
        else
        {
          foreach (XmlToClrImporter.ClrPropertyDescription propertyDescription1 in clrTypeDescription.Properties)
          {
            if (propertyDescription1.PropertyType == XmlToClrImporter.ClrTypeDescription.EmptyAttribute)
              propertyDescription1.PropertyType = this.stringTypeDescription;
            else if (propertyDescription1.PropertyType.IsCollectionWrapper)
            {
              XmlToClrImporter.ClrPropertyDescription propertyDescription2 = propertyDescription1.PropertyType.Properties[0];
              propertyDescription1.PropertyType.ShouldDelete = propertyDescription1.PropertyType != propertyDescription2.PropertyType;
              if (!propertyDescription1.IsCollection)
              {
                propertyDescription1.PropertyType = propertyDescription2.PropertyType;
                propertyDescription1.IsCollection = true;
                propertyDescription1.IsExplicitCollection = true;
                propertyDescription1.CollectionTypeBaseName = propertyDescription2.CollectionTypeBaseName;
              }
            }
            else
              propertyDescription1.PropertyType.ShouldDelete = false;
          }
        }
      }
      this.compositeTypes.RemoveAll((Predicate<XmlToClrImporter.ClrTypeDescription>) (t => t.ShouldDelete));
    }

    private void CreateClrTypes()
    {
      foreach (XmlToClrImporter.ClrTypeDescription clrTypeDescription in this.compositeTypes)
        clrTypeDescription.Type = this.xmlToClrAdapter.CreateCompositeType(clrTypeDescription.Name);
      foreach (XmlToClrImporter.ClrTypeDescription clrTypeDescription in this.compositeTypes)
      {
        foreach (XmlToClrImporter.ClrPropertyDescription property in clrTypeDescription.Properties)
        {
          string name = property.Name;
          IType propertyType = this.GetPropertyType(property);
          if (property.IsCollection && !property.IsExplicitCollection)
          {
            name += this.xmlToClrAdapter.CollectionSuffix;
            if (clrTypeDescription[name] != null)
              throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.XmlTwoNodesUsageNotSupported, new object[2]
              {
                (object) property.Name,
                (object) name
              }));
          }
          this.xmlToClrAdapter.AddProperty(clrTypeDescription.Type, name, propertyType);
        }
      }
    }

    private IType GetPropertyType(XmlToClrImporter.ClrPropertyDescription property)
    {
      return property.IsCollection ? this.GetOrCreateCollectionType(property.PropertyType, property.CollectionTypeBaseName) : property.PropertyType.Type;
    }

    private IType GetOrCreateCollectionType(XmlToClrImporter.ClrTypeDescription description, string collectionTypeBaseName)
    {
      string collectionTypeName = collectionTypeBaseName + this.xmlToClrAdapter.CollectionSuffix;
      if (this.compositeTypes.Find((Predicate<XmlToClrImporter.ClrTypeDescription>) (t => collectionTypeName == t.Name)) != null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.XmlTwoNodesUsageNotSupported, new object[2]
        {
          (object) collectionTypeBaseName,
          (object) collectionTypeName
        }));
      IType type = this.collectionTypes.Find((Predicate<IType>) (t => collectionTypeName == t.Name));
      if (type != null)
        return type;
      IType itemType;
      if (description.IsCollectionWrapper)
      {
        XmlToClrImporter.ClrPropertyDescription property = description.Properties[0];
        itemType = property.PropertyType != description ? this.GetPropertyType(property) : description.Type;
      }
      else
        itemType = description.Type;
      if (itemType == this.rootType)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.XmlRecursiveRootTypeNotSupported, new object[1]
        {
          (object) this.rootType.Name
        }));
      IType collectionType = this.xmlToClrAdapter.CreateCollectionType(collectionTypeName, itemType);
      this.collectionTypes.Add(collectionType);
      return collectionType;
    }

    private void ParseValues(DocumentCompositeNode documentNode, XmlToClrImporter.FilteredElement element)
    {
      if (element.HasAttributes)
      {
        foreach (XmlAttribute xmlAttribute in element.Attributes)
        {
          string clrName = this.xmlToClrAdapter.GetClrName(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.NamespaceURI);
          this.CreateLeafNode(documentNode, clrName, xmlAttribute.Value);
        }
      }
      if (!element.HasChildElements)
        return;
      foreach (XmlElement element1 in element.ChildElements)
      {
        XmlToClrImporter.FilteredElement element2 = new XmlToClrImporter.FilteredElement(element1, this.xmlToClrAdapter);
        if (element2.IsLeaf)
          this.CreateLeafNode(documentNode, element2.Name, element2.LeafValue);
        else
          this.ParseValues(this.CreateCompositeNode(documentNode, element2.Name), element2);
      }
    }

    private void CreateLeafNode(DocumentCompositeNode documentNode, string name, string value)
    {
      if (documentNode.SupportsChildren)
      {
        this.CreateCollectioItemLeafNode(documentNode, value);
      }
      else
      {
        IProperty property1 = (IProperty) documentNode.Type.GetMember(MemberType.Property, name, MemberAccessTypes.Public);
        if (property1 != null)
        {
          DocumentNode leafNode = this.xmlToClrAdapter.CreateLeafNode(property1.PropertyType, value);
          documentNode.Properties[(IPropertyId) property1] = leafNode;
        }
        else
        {
          string memberName = name + this.xmlToClrAdapter.CollectionSuffix;
          IProperty property2 = (IProperty) documentNode.Type.GetMember(MemberType.Property, memberName, MemberAccessTypes.Public);
          if (property2 == null)
            return;
          DocumentCompositeNode collectionNode = (DocumentCompositeNode) documentNode.Properties[(IPropertyId) property2];
          if (collectionNode == null)
          {
            collectionNode = this.xmlToClrAdapter.CreateCompositeNode(property2.PropertyType);
            documentNode.Properties[(IPropertyId) property2] = (DocumentNode) collectionNode;
          }
          this.CreateCollectioItemLeafNode(collectionNode, value);
        }
      }
    }

    private DocumentNode CreateCollectioItemLeafNode(DocumentCompositeNode collectionNode, string value)
    {
      DocumentNode leafNode = this.xmlToClrAdapter.CreateLeafNode(collectionNode.Type.ItemType, value);
      if (leafNode != null)
        collectionNode.Children.Add(leafNode);
      return leafNode;
    }

    private DocumentCompositeNode CreateCompositeNode(DocumentCompositeNode documentNode, string name)
    {
      if (documentNode.SupportsChildren)
      {
        DocumentCompositeNode compositeNode = this.xmlToClrAdapter.CreateCompositeNode(documentNode.Type.ItemType);
        documentNode.Children.Add((DocumentNode) compositeNode);
        return compositeNode;
      }
      IProperty property1 = (IProperty) documentNode.Type.GetMember(MemberType.Property, name, MemberAccessTypes.Public);
      if (property1 != null)
      {
        DocumentCompositeNode compositeNode = this.xmlToClrAdapter.CreateCompositeNode(property1.PropertyType);
        documentNode.Properties[(IPropertyId) property1] = (DocumentNode) compositeNode;
        return compositeNode;
      }
      string memberName = name + this.xmlToClrAdapter.CollectionSuffix;
      IProperty property2 = (IProperty) documentNode.Type.GetMember(MemberType.Property, memberName, MemberAccessTypes.Public);
      if (property2 == null)
        return (DocumentCompositeNode) null;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) documentNode.Properties[(IPropertyId) property2];
      if (documentCompositeNode == null)
      {
        documentCompositeNode = this.xmlToClrAdapter.CreateCompositeNode(property2.PropertyType);
        documentNode.Properties[(IPropertyId) property2] = (DocumentNode) documentCompositeNode;
      }
      DocumentCompositeNode compositeNode1 = this.xmlToClrAdapter.CreateCompositeNode(property2.PropertyType.ItemType);
      documentCompositeNode.Children.Add((DocumentNode) compositeNode1);
      return compositeNode1;
    }

    private enum ClrTypeCategory
    {
      Empty,
      Leaf,
      Composite,
    }

    private class ClrPropertyDescription
    {
      private int count;

      public string Name { get; set; }

      public XmlToClrImporter.ClrTypeDescription PropertyType { get; set; }

      public string CollectionTypeBaseName { get; set; }

      public bool IsCollection { get; set; }

      public bool IsExplicitCollection { get; set; }

      public ClrPropertyDescription(string name, XmlToClrImporter.ClrTypeDescription description)
      {
        this.Name = name;
        this.PropertyType = description;
        this.count = 1;
      }

      public void CountOccurence()
      {
        ++this.count;
        if (this.count != 2)
          return;
        this.IsCollection = true;
        this.CollectionTypeBaseName = this.Name;
      }

      public void Reset()
      {
        this.count = 0;
      }

      public override string ToString()
      {
        string str = this.Name + (object) ": " + (string) (object) this.PropertyType;
        if (this.IsExplicitCollection)
          str += "[*]";
        else if (this.IsCollection)
          str += "[]";
        return str;
      }
    }

    private class ClrTypeDescription
    {
      public static readonly XmlToClrImporter.ClrTypeDescription EmptyAttribute = new XmlToClrImporter.ClrTypeDescription()
      {
        Category = XmlToClrImporter.ClrTypeCategory.Empty
      };
      private bool? shouldDelete;

      public string Name { get; private set; }

      public XmlToClrImporter.ClrTypeCategory Category { get; private set; }

      public IType Type { get; set; }

      public List<XmlToClrImporter.ClrPropertyDescription> Properties { get; private set; }

      public bool ShouldDelete
      {
        get
        {
          if (this.shouldDelete.HasValue)
            return this.shouldDelete.Value;
          return false;
        }
        set
        {
          if (this.shouldDelete.HasValue && !this.shouldDelete.Value)
            return;
          this.shouldDelete = new bool?(value);
        }
      }

      public bool IsCollectionWrapper
      {
        get
        {
          return this.Category == XmlToClrImporter.ClrTypeCategory.Composite && this.Properties.Count == 1 && this.Properties[0].IsCollection;
        }
      }

      public XmlToClrImporter.ClrPropertyDescription this[string propertyName]
      {
        get
        {
          if (this.Properties == null)
            return (XmlToClrImporter.ClrPropertyDescription) null;
          return this.Properties.Find((Predicate<XmlToClrImporter.ClrPropertyDescription>) (p => propertyName == p.Name));
        }
      }

      private ClrTypeDescription()
      {
      }

      public static XmlToClrImporter.ClrTypeDescription CreateLeaf(IType description)
      {
        return new XmlToClrImporter.ClrTypeDescription()
        {
          Category = XmlToClrImporter.ClrTypeCategory.Leaf,
          Type = description,
          ShouldDelete = false
        };
      }

      public static XmlToClrImporter.ClrTypeDescription CreateComposite(string name)
      {
        return new XmlToClrImporter.ClrTypeDescription()
        {
          Name = name,
          Category = XmlToClrImporter.ClrTypeCategory.Composite,
          Properties = new List<XmlToClrImporter.ClrPropertyDescription>()
        };
      }

      public static XmlToClrImporter.ClrTypeDescription CreateEmpty(string name)
      {
        return new XmlToClrImporter.ClrTypeDescription()
        {
          Name = name,
          Category = XmlToClrImporter.ClrTypeCategory.Empty
        };
      }

      public XmlToClrImporter.ClrPropertyDescription AddProperty(string name, XmlToClrImporter.ClrTypeDescription description)
      {
        XmlToClrImporter.ClrPropertyDescription propertyDescription = new XmlToClrImporter.ClrPropertyDescription(name, description);
        this.Properties.Add(propertyDescription);
        return propertyDescription;
      }

      public void ResetPropertyOccurenceCounts()
      {
        if (this.Properties == null)
          return;
        this.Properties.ForEach((Action<XmlToClrImporter.ClrPropertyDescription>) (p => p.Reset()));
      }

      public void ConvertEmptyToComposite()
      {
        this.Category = XmlToClrImporter.ClrTypeCategory.Composite;
        this.Properties = new List<XmlToClrImporter.ClrPropertyDescription>();
      }

      public override string ToString()
      {
        if (this == XmlToClrImporter.ClrTypeDescription.EmptyAttribute)
          return "[Empty Attribute]";
        if (this.Category == XmlToClrImporter.ClrTypeCategory.Empty)
          return "Empty: " + this.Name;
        if (this.Category == XmlToClrImporter.ClrTypeCategory.Leaf)
          return "Leaf: " + this.Type.Name;
        if (this.Category != XmlToClrImporter.ClrTypeCategory.Composite)
          return base.ToString();
        return this.Name + (object) " [props: " + (string) (object) this.Properties.Count + "]" + (string) (this.ShouldDelete ? (object) " - DEL" : (object) "");
      }
    }

    private class FilteredElement
    {
      public IEnumerable<XmlAttribute> Attributes { get; private set; }

      public IEnumerable<XmlElement> ChildElements { get; private set; }

      public XmlElement Element { get; private set; }

      public string Name { get; private set; }

      public bool IsLeaf
      {
        get
        {
          return !this.HasAttributes && !this.HasChildElements;
        }
      }

      public bool HasAttributes
      {
        get
        {
          if (this.Attributes != null)
            return Enumerable.FirstOrDefault<XmlAttribute>(this.Attributes) != null;
          return false;
        }
      }

      public bool HasChildElements
      {
        get
        {
          if (this.ChildElements != null)
            return Enumerable.FirstOrDefault<XmlElement>(this.ChildElements) != null;
          return false;
        }
      }

      public string LeafValue
      {
        get
        {
          if (!this.Element.HasChildNodes)
            return (string) null;
          XmlText xmlText = Enumerable.FirstOrDefault<XmlNode>(Enumerable.OfType<XmlNode>((IEnumerable) this.Element.ChildNodes), (Func<XmlNode, bool>) (node => node is XmlText)) as XmlText;
          if (xmlText == null)
            return (string) null;
          return xmlText.Value;
        }
      }

      public FilteredElement(XmlElement element, IXmlToClrAdapter adapter)
      {
        this.Element = element;
        this.Name = adapter.GetClrName(element.LocalName, element.Prefix, element.NamespaceURI);
        if (this.Element.HasAttributes)
          this.Attributes = Enumerable.Where<XmlAttribute>(Enumerable.OfType<XmlAttribute>((IEnumerable) this.Element.Attributes), (Func<XmlAttribute, bool>) (attrib => adapter.ShouldProcessAttribute(attrib.LocalName, attrib.Prefix, attrib.NamespaceURI)));
        if (!this.Element.HasChildNodes)
          return;
        this.ChildElements = Enumerable.Where<XmlElement>(Enumerable.Select<XmlNode, XmlElement>(Enumerable.Where<XmlNode>(Enumerable.OfType<XmlNode>((IEnumerable) this.Element.ChildNodes), (Func<XmlNode, bool>) (node => node is XmlElement)), (Func<XmlNode, XmlElement>) (node => (XmlElement) node)), (Func<XmlElement, bool>) (elem => adapter.ShouldProcessElement(elem.LocalName, elem.Prefix, elem.NamespaceURI)));
      }

      public override string ToString()
      {
        return this.Element.Name + (object) ": attr-" + (string) (object) (!this.HasAttributes ? 0 : Enumerable.Count<XmlAttribute>(this.Attributes)) + (object) " elem-" + (string) (object) (!this.HasChildElements ? 0 : Enumerable.Count<XmlElement>(this.ChildElements));
      }
    }
  }
}

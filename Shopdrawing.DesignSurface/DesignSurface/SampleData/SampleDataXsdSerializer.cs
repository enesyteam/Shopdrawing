// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataXsdSerializer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataXsdSerializer
  {
    private List<Exception> compileExceptions = new List<Exception>();
    private Dictionary<string, XmlSchemaType> schemaTypes = new Dictionary<string, XmlSchemaType>();
    private SampleDataSet dataSet;
    private XmlQualifiedName dateType;
    private XmlQualifiedName imageType;
    private XmlQualifiedName numberType;
    private XmlQualifiedName booleanType;
    private XmlQualifiedName stringType;
    private XmlDocument xmlDocument;
    private string xsdFile;

    private string TargetNamespace
    {
      get
      {
        return this.dataSet.ClrNamespace;
      }
    }

    private SampleDataXsdSerializer(SampleDataSet dataSet, string xsdFile)
    {
      this.dataSet = dataSet;
      this.xsdFile = xsdFile;
    }

    public static void Serialize(SampleDataSet dataSet, string xsdFile)
    {
      new SampleDataXsdSerializer(dataSet, xsdFile).Serialize();
    }

    private void Serialize()
    {
      XmlSchema schema = new XmlSchema();
      schema.TargetNamespace = this.TargetNamespace;
      this.xmlDocument = new XmlDocument();
      foreach (SampleNonBasicType sampleType in this.dataSet.Types)
        this.GetOrCreateSchemaType(sampleType);
      schema.Items.Add((XmlSchemaObject) new XmlSchemaElement()
      {
        Name = this.dataSet.RootType.Name,
        SchemaTypeName = new XmlQualifiedName(this.dataSet.RootType.Name, this.TargetNamespace)
      });
      foreach (KeyValuePair<string, XmlSchemaType> keyValuePair in this.schemaTypes)
        schema.Items.Add((XmlSchemaObject) keyValuePair.Value);
      XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
      xmlSchemaSet.ValidationEventHandler += new ValidationEventHandler(this.XmlSchemaValidationHandler);
      xmlSchemaSet.Add(schema);
      xmlSchemaSet.Compile();
      if (this.compileExceptions.Count > 0)
        throw this.compileExceptions[0];
      foreach (XmlSchema xmlSchema in (IEnumerable) xmlSchemaSet.Schemas())
        schema = xmlSchema;
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager((XmlNameTable) new NameTable());
      namespaceManager.AddNamespace(SampleDataSet.XsdNS.Prefix, SampleDataSet.XsdNS.Namespace);
      namespaceManager.AddNamespace(SampleDataSet.BlendNS.Prefix, SampleDataSet.BlendNS.Namespace);
      namespaceManager.AddNamespace("tns", this.TargetNamespace);
      using (XmlWriter writer = this.xmlDocument.CreateNavigator().AppendChild())
        schema.Write(writer, namespaceManager);
      this.xmlDocument.InsertBefore((XmlNode) this.xmlDocument.CreateComment(StringTable.SampleDataDoNotEditComment.TrimEnd(' ')), (XmlNode) this.xmlDocument.DocumentElement);
      using (FileStream fileStream = new FileStream(this.xsdFile, FileMode.Create, FileAccess.Write))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
          this.xmlDocument.Save((TextWriter) streamWriter);
      }
    }

    private void XmlSchemaValidationHandler(object sender, ValidationEventArgs e)
    {
      if (e.Severity != XmlSeverityType.Error)
        return;
      this.compileExceptions.Add((Exception) e.Exception);
    }

    private XmlSchemaType CreateSchemaCollectionType(SampleCollectionType collectionType)
    {
      XmlSchemaComplexType schemaComplexType = new XmlSchemaComplexType();
      schemaComplexType.Name = collectionType.Name;
      this.schemaTypes[schemaComplexType.Name] = (XmlSchemaType) schemaComplexType;
      XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
      XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
      xmlSchemaElement.MaxOccursString = "unbounded";
      if (collectionType.ItemSampleType.IsBasicType)
      {
        xmlSchemaElement.Name = collectionType.Name + "Item";
        xmlSchemaElement.SchemaTypeName = this.GetBasicSchemaType(collectionType.ItemSampleType as SampleBasicType);
      }
      else
      {
        xmlSchemaElement.Name = collectionType.ItemSampleType.Name;
        XmlSchemaType createSchemaType = this.GetOrCreateSchemaType(collectionType.ItemSampleType as SampleNonBasicType);
        xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(createSchemaType.Name, this.TargetNamespace);
      }
      xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaElement);
      schemaComplexType.Particle = (XmlSchemaParticle) xmlSchemaSequence;
      return (XmlSchemaType) schemaComplexType;
    }

    private XmlSchemaType CreateSchemaCompositeType(SampleCompositeType compositeType)
    {
      XmlSchemaComplexType schemaComplexType = new XmlSchemaComplexType();
      schemaComplexType.Name = compositeType.Name;
      this.schemaTypes[schemaComplexType.Name] = (XmlSchemaType) schemaComplexType;
      XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) compositeType.SampleProperties)
      {
        if (!sampleProperty.IsBasicType)
        {
          XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
          xmlSchemaElement.Name = sampleProperty.Name;
          if (!sampleProperty.IsBasicType)
          {
            XmlSchemaType createSchemaType = this.GetOrCreateSchemaType(sampleProperty.PropertySampleType as SampleNonBasicType);
            xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(createSchemaType.Name, this.TargetNamespace);
          }
          else
            xmlSchemaElement.SchemaTypeName = this.GetBasicSchemaType(sampleProperty.PropertySampleType as SampleBasicType);
          this.UpdatePropertyFormatInfo((XmlSchemaAnnotated) xmlSchemaElement, sampleProperty);
          xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaElement);
        }
      }
      if (xmlSchemaSequence.Items.Count > 0)
        schemaComplexType.Particle = (XmlSchemaParticle) xmlSchemaSequence;
      foreach (SampleProperty sampleProperty in (IEnumerable<SampleProperty>) compositeType.SampleProperties)
      {
        if (sampleProperty.IsBasicType)
        {
          XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
          xmlSchemaAttribute.Name = sampleProperty.Name;
          xmlSchemaAttribute.SchemaTypeName = this.GetBasicSchemaType(sampleProperty.PropertySampleType as SampleBasicType);
          this.UpdatePropertyFormatInfo((XmlSchemaAnnotated) xmlSchemaAttribute, sampleProperty);
          schemaComplexType.Attributes.Add((XmlSchemaObject) xmlSchemaAttribute);
        }
      }
      return (XmlSchemaType) schemaComplexType;
    }

    private void UpdatePropertyFormatInfo(XmlSchemaAnnotated schemaItem, SampleProperty sampleProperty)
    {
      XmlAttribute blendAttribute1 = this.CreateBlendAttribute(sampleProperty.Format, SampleProperty.FormatAttribute);
      XmlAttribute blendAttribute2 = this.CreateBlendAttribute(sampleProperty.FormatParameters, SampleProperty.FormatParametersAttribute);
      if (blendAttribute1 == null && blendAttribute2 == null)
        return;
      XmlAttribute[] xmlAttributeArray;
      if (blendAttribute1 == null)
        xmlAttributeArray = new XmlAttribute[1]
        {
          blendAttribute2
        };
      else if (blendAttribute2 == null)
        xmlAttributeArray = new XmlAttribute[1]
        {
          blendAttribute1
        };
      else
        xmlAttributeArray = new XmlAttribute[2]
        {
          blendAttribute1,
          blendAttribute2
        };
      schemaItem.UnhandledAttributes = xmlAttributeArray;
    }

    private XmlAttribute CreateBlendAttribute(string attributeValue, string attributeName)
    {
      if (attributeValue == null)
        return (XmlAttribute) null;
      XmlAttribute attribute = this.xmlDocument.CreateAttribute(SampleDataSet.BlendNS.Prefix, attributeName, SampleDataSet.BlendNS.Namespace);
      attribute.Value = attributeValue;
      return attribute;
    }

    private XmlQualifiedName GetBasicSchemaType(SampleBasicType basicType)
    {
      if (basicType == SampleBasicType.String)
      {
        if (this.stringType == (XmlQualifiedName) null)
          this.stringType = new XmlQualifiedName("string", SampleDataSet.XsdNS.Namespace);
        return this.stringType;
      }
      if (basicType == SampleBasicType.Number)
      {
        if (this.numberType == (XmlQualifiedName) null)
          this.numberType = new XmlQualifiedName("double", SampleDataSet.XsdNS.Namespace);
        return this.numberType;
      }
      if (basicType == SampleBasicType.Boolean)
      {
        if (this.booleanType == (XmlQualifiedName) null)
          this.booleanType = new XmlQualifiedName("boolean", SampleDataSet.XsdNS.Namespace);
        return this.booleanType;
      }
      if (basicType == SampleBasicType.Date)
      {
        if (this.dateType == (XmlQualifiedName) null)
          this.dateType = new XmlQualifiedName("dateTime", SampleDataSet.XsdNS.Namespace);
        return this.dateType;
      }
      if (basicType != SampleBasicType.Image)
        return (XmlQualifiedName) null;
      if (this.imageType == (XmlQualifiedName) null)
      {
        this.imageType = new XmlQualifiedName(SampleBasicType.XsdImageTypeName, this.TargetNamespace);
        XmlSchemaSimpleType schemaSimpleType = new XmlSchemaSimpleType();
        schemaSimpleType.Name = SampleBasicType.XsdImageTypeName;
        schemaSimpleType.Content = (XmlSchemaSimpleTypeContent) new XmlSchemaSimpleTypeRestriction()
        {
          BaseTypeName = new XmlQualifiedName("anyURI", SampleDataSet.XsdNS.Namespace)
        };
        this.schemaTypes["blend:Image"] = (XmlSchemaType) schemaSimpleType;
      }
      return this.imageType;
    }

    private XmlSchemaType GetOrCreateSchemaType(SampleNonBasicType sampleType)
    {
      XmlSchemaType xmlSchemaType = (XmlSchemaType) null;
      if (!this.schemaTypes.TryGetValue(sampleType.Name, out xmlSchemaType))
        xmlSchemaType = !sampleType.IsCollection ? this.CreateSchemaCompositeType(sampleType as SampleCompositeType) : this.CreateSchemaCollectionType(sampleType as SampleCollectionType);
      return xmlSchemaType;
    }
  }
}

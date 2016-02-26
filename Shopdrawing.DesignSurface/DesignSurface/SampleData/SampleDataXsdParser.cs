// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataXsdParser
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleDataXsdParser
  {
    private SampleDataSet dataSet;
    private XmlSchema xmlSchema;
    private string xsdFileName;

    private SampleDataXsdParser(SampleDataSet dataSet, string xsdFileName)
    {
      this.dataSet = dataSet;
      this.xsdFileName = xsdFileName;
    }

    public static void Parse(SampleDataSet dataSet, string xsdFileName)
    {
      new SampleDataXsdParser(dataSet, xsdFileName).Parse();
    }

    private void Parse()
    {
      this.ParseSchemaAndValidateVesrion();
      string rootTypeName = this.GetRootTypeName();
      this.dataSet.RootType.Rename(rootTypeName);
      this.PopulateSampleCompositeType(this.dataSet.RootType, this.GetSchemaType(rootTypeName));
      foreach (XmlSchemaObject xmlSchemaObject in this.xmlSchema.Items)
      {
        XmlSchemaComplexType xmlComplexType = xmlSchemaObject as XmlSchemaComplexType;
        if (xmlComplexType != null)
          this.GetOrReadType(xmlComplexType);
      }
    }

    private SampleNonBasicType GetOrReadType(XmlSchemaComplexType xmlComplexType)
    {
      return this.dataSet.GetSampleType(xmlComplexType.Name) ?? this.ReadType(xmlComplexType);
    }

    private string GetRootTypeName()
    {
      bool flag = false;
      string str = (string) null;
      foreach (XmlSchemaObject xmlSchemaObject in this.xmlSchema.Items)
      {
        XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
        if (xmlSchemaElement != null)
        {
          if (flag)
            throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.SampleDataXsElementMisuse, new object[1]
            {
              (object) this.xsdFileName
            }));
          flag = true;
          str = xmlSchemaElement.Name;
        }
      }
      if (!flag)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.SampleDataXsElementMisuse, new object[1]
        {
          (object) this.xsdFileName
        }));
      return str;
    }

    private SampleType GetSampleType(XmlQualifiedName schemaTypeName)
    {
      if (schemaTypeName.Namespace == SampleDataSet.XsdNS.Namespace)
      {
        if (schemaTypeName.Name == "string")
          return (SampleType) SampleBasicType.String;
        if (schemaTypeName.Name == "double")
          return (SampleType) SampleBasicType.Number;
        if (schemaTypeName.Name == "boolean")
          return (SampleType) SampleBasicType.Boolean;
        if (schemaTypeName.Name == "dateTime")
          return (SampleType) SampleBasicType.Date;
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidBasicType, new object[2]
        {
          (object) this.xsdFileName,
          (object) schemaTypeName.Name
        }));
      }
      if (schemaTypeName.Namespace != this.dataSet.ClrNamespace)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidNamespace, (object) this.xsdFileName, (object) schemaTypeName.Namespace, (object) this.dataSet.ClrNamespace));
      if (schemaTypeName.Name == SampleBasicType.XsdImageTypeName)
        return (SampleType) SampleBasicType.Image;
      return (SampleType) this.dataSet.GetSampleType(schemaTypeName.Name) ?? (SampleType) this.ReadType(this.GetSchemaType(schemaTypeName.Name));
    }

    private void ParseSchemaAndValidateVesrion()
    {
      using (FileStream fileStream = new FileStream(this.xsdFileName, FileMode.Open, FileAccess.Read))
      {
        XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
        xmlSchemaSet.Add(XmlSchema.Read((Stream) fileStream, (ValidationEventHandler) null));
        xmlSchemaSet.Compile();
        foreach (XmlSchema xmlSchema in (IEnumerable) xmlSchemaSet.Schemas())
          this.xmlSchema = xmlSchema;
      }
      bool flag = false;
      XmlQualifiedName[] xmlQualifiedNameArray = this.xmlSchema.Namespaces.ToArray();
      if (xmlQualifiedNameArray != null)
      {
        for (int index = 0; index < xmlQualifiedNameArray.Length; ++index)
        {
          if (xmlQualifiedNameArray[index].Namespace == SampleDataSet.BlendNS.Namespace)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.MissingBlendNamespace, new object[2]
        {
          (object) this.xsdFileName,
          (object) SampleDataSet.BlendNS.Namespace
        }));
    }

    private XmlSchemaComplexType GetSchemaType(string name)
    {
      XmlSchemaComplexType schemaComplexType = (XmlSchemaComplexType) null;
      foreach (XmlSchemaObject xmlSchemaObject in this.xmlSchema.Items)
      {
        schemaComplexType = xmlSchemaObject as XmlSchemaComplexType;
        if (schemaComplexType != null)
        {
          if (schemaComplexType.Name == name)
            break;
        }
        schemaComplexType = (XmlSchemaComplexType) null;
      }
      if (schemaComplexType == null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.MissingTypeName, new object[2]
        {
          (object) this.xsdFileName,
          (object) name
        }));
      return schemaComplexType;
    }

    private SampleNonBasicType ReadType(XmlSchemaComplexType xmlComplexType)
    {
      XmlSchemaSequence xmlSchemaSequence = xmlComplexType.Particle as XmlSchemaSequence;
      if (xmlSchemaSequence == null && xmlComplexType.Particle != null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidTypeFormat, new object[2]
        {
          (object) this.xsdFileName,
          (object) xmlComplexType.Name
        }));
      if (xmlSchemaSequence != null && xmlSchemaSequence.Items.Count == 1)
      {
        XmlSchemaElement xmlSchemaElement = xmlSchemaSequence.Items[0] as XmlSchemaElement;
        if (xmlSchemaElement != null && xmlSchemaElement.MaxOccurs > new Decimal(1))
        {
          SampleCollectionType collectionType = this.dataSet.CreateCollectionType(xmlComplexType.Name, (SampleType) SampleBasicType.String);
          SampleType sampleType = this.GetSampleType(xmlSchemaElement.SchemaTypeName);
          collectionType.ChangeItemType(sampleType);
          if (collectionType.Name != xmlComplexType.Name)
            throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidNodeUsage, new object[2]
            {
              (object) this.xsdFileName,
              (object) xmlComplexType.Name
            }));
          return (SampleNonBasicType) collectionType;
        }
      }
      SampleCompositeType compositeType = this.dataSet.CreateCompositeType(xmlComplexType.Name);
      if (compositeType.Name != xmlComplexType.Name)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidNodeUsage, new object[2]
        {
          (object) this.xsdFileName,
          (object) xmlComplexType.Name
        }));
      this.PopulateSampleCompositeType(compositeType, xmlComplexType);
      return (SampleNonBasicType) compositeType;
    }

    private void PopulateSampleCompositeType(SampleCompositeType compositeType, XmlSchemaComplexType xmlComplexType)
    {
      XmlSchemaSequence xmlSchemaSequence = xmlComplexType.Particle as XmlSchemaSequence;
      if (xmlSchemaSequence == null && xmlComplexType.Particle != null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidTypeFormat, new object[2]
        {
          (object) this.xsdFileName,
          (object) xmlComplexType.Name
        }));
      if (xmlSchemaSequence != null)
      {
        foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
        {
          XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
          if (xmlSchemaElement != null)
          {
            SampleType sampleType = this.GetSampleType(xmlSchemaElement.SchemaTypeName);
            SampleProperty sampleProperty = compositeType.AddProperty(xmlSchemaElement.Name, sampleType);
            SampleDataXsdParser.UpdatePropertyFormatInfo((XmlSchemaAnnotated) xmlSchemaElement, sampleProperty);
            if (sampleProperty.Name != xmlSchemaElement.Name)
              throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidNodeUsage, new object[2]
              {
                (object) this.xsdFileName,
                (object) xmlSchemaElement.Name
              }));
          }
        }
      }
      foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlComplexType.Attributes)
      {
        SampleBasicType sampleBasicType = this.GetSampleType(xmlSchemaAttribute.SchemaTypeName) as SampleBasicType;
        SampleProperty sampleProperty = compositeType.AddProperty(xmlSchemaAttribute.Name, (SampleType) sampleBasicType);
        SampleDataXsdParser.UpdatePropertyFormatInfo((XmlSchemaAnnotated) xmlSchemaAttribute, sampleProperty);
        if (sampleProperty.Name != xmlSchemaAttribute.Name)
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.InvalidNodeUsage, new object[2]
          {
            (object) this.xsdFileName,
            (object) xmlSchemaAttribute.Name
          }));
      }
    }

    private static void UpdatePropertyFormatInfo(XmlSchemaAnnotated schemaItem, SampleProperty sampleProperty)
    {
      XmlAttribute[] unhandledAttributes = schemaItem.UnhandledAttributes;
      string blendAttributeValue1 = SampleDataXsdParser.GetBlendAttributeValue(unhandledAttributes, SampleProperty.FormatAttribute);
      string blendAttributeValue2 = SampleDataXsdParser.GetBlendAttributeValue(unhandledAttributes, SampleProperty.FormatParametersAttribute);
      sampleProperty.ChangeFormat(blendAttributeValue1, blendAttributeValue2);
    }

    private static string GetBlendAttributeValue(XmlAttribute[] attributes, string attributeName)
    {
      if (attributes == null)
        return (string) null;
      string str = (string) null;
      for (int index = 0; index < attributes.Length; ++index)
      {
        XmlAttribute xmlAttribute = attributes[index];
        if (xmlAttribute.LocalName == attributeName && xmlAttribute.NamespaceURI == SampleDataSet.BlendNS.Namespace)
        {
          str = xmlAttribute.Value;
          break;
        }
      }
      return str;
    }
  }
}

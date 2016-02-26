// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Clipboard.Container
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Xml;

namespace Microsoft.Expression.Framework.Clipboard
{
  public static class Container
  {
    public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("cfXaml");
    private const string PackageCorePropertiesRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/relationships/metadata/core-properties";
    private const string PackageSelectedItemRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/selected-item";
    private const string PackageSelectedItemUriRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/selected-item-uri";
    private const string PackageReferencedItemRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/referenced-item";
    private const string PackageItemAttributesRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/item-attributes";
    private const string PackageImageRelationshipTypeV2 = "http://schemas.microsoft.com/cfxaml/2006/06/image";
    private const double version = 0.71;
    private const string CopyContainerCorePropertiesSpecificationV2 = "<?xml version='1.0' encoding='utf-8'?>\r\n<coreProperties\r\n  xmlns='http://schemas.microsoft.com/cfxaml/2006/06/metadata/core-properties'\r\n  xmlns:dc='http://purl.org/dc/elements/1.1'>\r\n  <contentType>cfXaml</contentType>\r\n  <dc:creator>Expression</dc:creator>\r\n  <version>0.71</version>\r\n</coreProperties>";

    public static bool SaveClipboardContentsToFile { get; set; }

    public static MemoryStream CreateClipboardStream(CopyBuffer itemsToPaste)
    {
      Uri partUri = PackUriHelper.CreatePartUri(new Uri("CoreProperties", UriKind.Relative));
      MemoryStream memoryStream = new MemoryStream();
      using (Package package = Package.Open((Stream) memoryStream, FileMode.Create))
      {
        PackagePart part = package.CreatePart(partUri, "text/Xml");
        using (Stream stream = part.GetStream())
        {
          using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
            streamWriter.Write("<?xml version='1.0' encoding='utf-8'?>\r\n<coreProperties\r\n  xmlns='http://schemas.microsoft.com/cfxaml/2006/06/metadata/core-properties'\r\n  xmlns:dc='http://purl.org/dc/elements/1.1'>\r\n  <contentType>cfXaml</contentType>\r\n  <dc:creator>Expression</dc:creator>\r\n  <version>0.71</version>\r\n</coreProperties>");
        }
        package.CreateRelationship(part.Uri, TargetMode.Internal, "http://schemas.microsoft.com/cfxaml/2006/06/relationships/metadata/core-properties");
        List<PackagePart> itemPackageParts1 = new List<PackagePart>();
        for (int index = 0; index < itemsToPaste.SelectedItemCount; ++index)
        {
          CopyItem copyItem = itemsToPaste.SelectedItem(index);
          Container.CreatePackageItem(package, itemPackageParts1, "/CopiedItem" + (object) index, "http://schemas.microsoft.com/cfxaml/2006/06/selected-item", copyItem);
        }
        List<PackagePart> itemPackageParts2 = new List<PackagePart>();
        for (int index = 0; index < itemsToPaste.ReferencedItemCount; ++index)
        {
          CopyItem copyItem = itemsToPaste.ReferencedItem(index);
          Container.CreatePackageItem(package, itemPackageParts2, "/ReferencedItem" + (object) index, "http://schemas.microsoft.com/cfxaml/2006/06/referenced-item", copyItem);
        }
      }
      if (Container.SaveClipboardContentsToFile)
      {
        using (FileStream fileStream = new FileStream("clipboardContents.zip", FileMode.Create, FileAccess.ReadWrite))
        {
          memoryStream.Seek(0L, SeekOrigin.Begin);
          Container.CopyStream((Stream) memoryStream, (Stream) fileStream);
        }
      }
      return memoryStream;
    }

    private static void CreatePackageItem(Package package, List<PackagePart> itemPackageParts, string itemPartRoot, string packageRelationship, CopyItem item)
    {
      Stream stream1 = item.GetStream();
      if (stream1 == null)
        return;
      using (stream1)
      {
        Uri partUri1 = PackUriHelper.CreatePartUri(new Uri(itemPartRoot + "/content" + item.FilenameExtension, UriKind.Relative));
        string mimeType = Container.GetMimeType(item);
        PackagePart part1 = package.CreatePart(partUri1, mimeType);
        itemPackageParts.Add(part1);
        using (Stream stream2 = part1.GetStream())
          Container.CopyStream(stream1, stream2);
        package.CreateRelationship(partUri1, TargetMode.Internal, packageRelationship);
        if (item.Key == null)
          return;
        Uri partUri2 = PackUriHelper.CreatePartUri(new Uri(itemPartRoot + "/Attributes.xml", UriKind.Relative));
        PackagePart part2 = package.CreatePart(partUri2, "text/Xml");
        itemPackageParts.Add(part2);
        Container.WriteAttributes(part2, item);
        part1.CreateRelationship(partUri2, TargetMode.Internal, "http://schemas.microsoft.com/cfxaml/2006/06/item-attributes");
      }
    }

    private static string GetMimeType(CopyItem referencedItem)
    {
      string str = "text/plain";
      if (referencedItem.FilenameExtension == ".gif")
        str = "image/gif";
      else if (referencedItem.FilenameExtension == ".jpg" || referencedItem.FilenameExtension == ".jpeg")
        str = "image/jpeg";
      else if (referencedItem.FilenameExtension == ".png")
        str = "image/png";
      else if (referencedItem.FilenameExtension == ".tiff" || referencedItem.FilenameExtension == ".tif")
        str = "image/tiff";
      else if (referencedItem.FilenameExtension == ".xaml")
        str = "text/xml";
      return str;
    }

    private static void WriteAttributes(PackagePart part, CopyItem item)
    {
      using (Stream stream = part.GetStream())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings()
        {
          Indent = true
        }))
        {
          try
          {
            xmlWriter.WriteStartElement("ItemAttributes");
            xmlWriter.WriteAttributeString("Version", 0.71.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            xmlWriter.WriteStartElement("ContentType");
            xmlWriter.WriteAttributeString("Value", item.ContentType);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("FileExtension");
            xmlWriter.WriteAttributeString("Value", item.FilenameExtension);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("OriginalUri");
            xmlWriter.WriteAttributeString("Value", item.OriginalUri.OriginalString);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("LocalPath");
            xmlWriter.WriteAttributeString("Value", item.LocalPath);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteAttributeString("Value", item.Key);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
          }
          catch (UnauthorizedAccessException ex)
          {
          }
          catch (IOException ex)
          {
          }
        }
      }
    }

    public static CopyBuffer ExtractCopyBufferFromClipboardStream(Stream clipboardStream)
    {
      CopyBuffer result;
      using (Package package = Package.Open(clipboardStream, FileMode.Open, FileAccess.Read))
      {
        result = new CopyBuffer();
        foreach (PackageRelationship packageRelationship in package.GetRelationshipsByType("http://schemas.microsoft.com/cfxaml/2006/06/relationships/metadata/core-properties"))
        {
          Uri partUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), packageRelationship.TargetUri);
          PackagePart part = package.GetPart(partUri);
          Container.ParseCorePropertyPart(result, part);
        }
        foreach (PackageRelationship packageRelationship in package.GetRelationshipsByType("http://schemas.microsoft.com/cfxaml/2006/06/selected-item"))
        {
          Uri partUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), packageRelationship.TargetUri);
          CopyItem itemToPaste = (CopyItem) new MemoryCopyItem(Container.GetBytes(package.GetPart(partUri)));
          result.AddSelectedItem(itemToPaste);
        }
        foreach (PackageRelationship packageRelationship1 in package.GetRelationshipsByType("http://schemas.microsoft.com/cfxaml/2006/06/referenced-item"))
        {
          Uri partUri1 = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), packageRelationship1.TargetUri);
          PackagePart part = package.GetPart(partUri1);
          CopyItem copyItem = (CopyItem) new MemoryCopyItem(Container.GetBytes(part));
          result.AddReferencedItem(copyItem);
          foreach (PackageRelationship packageRelationship2 in part.GetRelationshipsByType("http://schemas.microsoft.com/cfxaml/2006/06/item-attributes"))
          {
            Uri partUri2 = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), packageRelationship2.TargetUri);
            Container.ReadAttributes(package.GetPart(partUri2), copyItem);
          }
        }
      }
      return result;
    }

    public static CopyBuffer ExtractCopyBufferFromClipboardContainer(byte[] clipboardContents)
    {
      if (clipboardContents == null)
        return (CopyBuffer) null;
      using (MemoryStream memoryStream = new MemoryStream(clipboardContents))
        return Container.ExtractCopyBufferFromClipboardStream((Stream) memoryStream);
    }

    private static void ParseCorePropertyPart(CopyBuffer result, PackagePart corePropertyPart)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (Stream stream = corePropertyPart.GetStream())
        xmlDocument.Load(stream);
      Container.GetTagNameValueAsString(xmlDocument, "contentType");
      string nameValueAsString1 = Container.GetTagNameValueAsString(xmlDocument, "version");
      double result1 = -1.0;
      double.TryParse(nameValueAsString1, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1);
      result.Version = result1;
      string nameValueAsString2 = Container.GetTagNameValueAsString(xmlDocument, "dc:creator");
      result.Creator = nameValueAsString2;
    }

    private static string GetTagNameValueAsString(XmlDocument xmlDocument, string tagName)
    {
      XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(tagName);
      string str = (string) null;
      if (elementsByTagName.Count > 0)
      {
        XmlNode firstChild = elementsByTagName[0].FirstChild;
        if (firstChild != null)
          str = firstChild.Value;
      }
      return str;
    }

    public static void ReadAttributes(PackagePart part, CopyItem item)
    {
      using (Stream stream = part.GetStream())
      {
        using (XmlReader xmlReader = XmlReader.Create(stream))
        {
          try
          {
            if (!xmlReader.IsStartElement("ItemAttributes"))
              return;
            double result;
            double.TryParse(xmlReader.GetAttribute("Version"), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
            if (result != 0.71)
              return;
            xmlReader.ReadStartElement("ItemAttributes");
            if (xmlReader.IsStartElement("ContentType"))
            {
              string attribute = xmlReader.GetAttribute("Value");
              item.ContentType = attribute;
              xmlReader.ReadStartElement("ContentType");
            }
            if (xmlReader.IsStartElement("FileExtension"))
            {
              string attribute = xmlReader.GetAttribute("Value");
              item.FilenameExtension = attribute;
              xmlReader.ReadStartElement("FileExtension");
            }
            if (xmlReader.IsStartElement("OriginalUri"))
            {
              string attribute = xmlReader.GetAttribute("Value");
              item.OriginalUri = new Uri(attribute, UriKind.RelativeOrAbsolute);
              xmlReader.ReadStartElement("OriginalUri");
            }
            if (xmlReader.IsStartElement("LocalPath"))
            {
              item.LocalPath = xmlReader.GetAttribute("Value");
              xmlReader.ReadStartElement("LocalPath");
            }
            if (xmlReader.IsStartElement("Key"))
            {
              item.Key = xmlReader.GetAttribute("Value");
              xmlReader.ReadStartElement("Key");
            }
            xmlReader.ReadEndElement();
          }
          catch (XmlException ex)
          {
          }
        }
      }
    }

    public static byte[] GetBytes(PackagePart part)
    {
      using (Stream stream = part.GetStream())
        return Container.GetBytes(stream);
    }

    public static byte[] GetBytes(Stream source)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Container.CopyStream(source, (Stream) memoryStream);
        return memoryStream.ToArray();
      }
    }

    public static void CopyStream(Stream source, Stream target)
    {
      byte[] buffer = new byte[65536];
      source.Seek(0L, SeekOrigin.Begin);
      int count;
      while ((count = source.Read(buffer, 0, 65536)) > 0)
        target.Write(buffer, 0, count);
    }
  }
}

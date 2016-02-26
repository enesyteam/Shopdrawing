// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.DataAccess.PhoneNumberStore
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Expression.Framework.ActivationDialog.DataAccess
{
  internal class PhoneNumberStore
  {
    private const string LocationElement = "Location";
    private const string NameAttribute = "Name";
    private const string IdentifierAttribute = "Identifier";
    private const string TollFreeNumberElement = "TollFree";
    private const string TollNumberElement = "TollNumber";
    private const string NumberElement = "Number";
    private const string CommentElement = "Comment";
    private XDocument document;

    public PhoneNumberStore()
    {
      MemoryStream memoryStream = new MemoryStream(FileTable.GetByteArray("Resources\\PhoneActivationData.xml"));
      try
      {
        this.document = XDocument.Load((Stream) memoryStream);
      }
      catch (Exception ex)
      {
      }
    }

    public IList<LocationInformation> GetLocations()
    {
      if (this.document == null)
        return (IList<LocationInformation>) new List<LocationInformation>();
      return (IList<LocationInformation>) Enumerable.ToList<LocationInformation>(Enumerable.Select<XElement, LocationInformation>(this.document.Descendants((XName) "Location"), (Func<XElement, LocationInformation>) (element => new LocationInformation(element.Attribute((XName) "Name").Value, element.Attribute((XName) "Identifier").Value))));
    }

    public IList<PhoneNumberInformation> GetPhoneNumberInfosForLocation(LocationInformation location)
    {
      XElement locationElement = this.GetLocationElement(location.Identifier);
      if (locationElement != null)
        return (IList<PhoneNumberInformation>) this.ExtractPhoneNumberInfo(locationElement);
      return (IList<PhoneNumberInformation>) new List<PhoneNumberInformation>();
    }

    private XElement GetLocationElement(string location)
    {
      if (this.document == null)
        return (XElement) null;
      return Enumerable.FirstOrDefault<XElement>(this.document.Descendants((XName) "Location"), (Func<XElement, bool>) (locationNode => string.Equals(locationNode.Attribute((XName) "Identifier").Value, location)));
    }

    private List<PhoneNumberInformation> ExtractPhoneNumberInfo(XElement locationElement)
    {
      List<PhoneNumberInformation> list1 = Enumerable.ToList<PhoneNumberInformation>(Enumerable.Select<XElement, PhoneNumberInformation>(locationElement.Descendants((XName) "TollFree"), (Func<XElement, PhoneNumberInformation>) (tollFreeNumber => this.PhoneNumberInfoFromNumberElement(tollFreeNumber, StringTable.LicensingActivationPhoneNumberTollFreeNumber))));
      List<PhoneNumberInformation> list2 = Enumerable.ToList<PhoneNumberInformation>(Enumerable.Select<XElement, PhoneNumberInformation>(locationElement.Descendants((XName) "TollNumber"), (Func<XElement, PhoneNumberInformation>) (tollNumber => this.PhoneNumberInfoFromNumberElement(tollNumber, StringTable.LicensingActivationPhoneNumberTollNumber))));
      if (list1.Count == 0)
        list1.Add(new PhoneNumberInformation(StringTable.LicensingActivationPhoneNumberNotAvailable, StringTable.LicensingActivationPhoneNumberTollFreeNumber, string.Empty));
      if (list2.Count == 0)
        list2.Add(new PhoneNumberInformation(StringTable.LicensingActivationPhoneNumberNotAvailable, StringTable.LicensingActivationPhoneNumberTollNumber, string.Empty));
      list1.AddRange((IEnumerable<PhoneNumberInformation>) list2);
      return list1;
    }

    private PhoneNumberInformation PhoneNumberInfoFromNumberElement(XElement numberElement, string numberType)
    {
      string phoneNumber = numberElement.Element((XName) "Number").Value;
      XElement xelement = numberElement.Element((XName) "Comment");
      string optionalInformation = xelement != null ? xelement.Value : string.Empty;
      return new PhoneNumberInformation(phoneNumber, numberType, optionalInformation);
    }
  }
}

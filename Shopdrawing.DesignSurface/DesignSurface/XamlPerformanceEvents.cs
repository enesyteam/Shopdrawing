// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.XamlPerformanceEvents
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.Framework.Diagnostics;
using System;

namespace Microsoft.Expression.DesignSurface
{
  internal static class XamlPerformanceEvents
  {
    public static void RegisterEvents()
    {
      XamlParser.Parsing += new EventHandler(XamlPerformanceEvents.XamlParser_Parsing);
      XamlParser.Parsed += new EventHandler(XamlPerformanceEvents.XamlParser_Parsed);
      XamlParser.XmlParsing += new EventHandler(XamlPerformanceEvents.XamlParser_XmlParsing);
      XamlParser.XmlParsed += new EventHandler(XamlPerformanceEvents.XamlParser_XmlParsed);
      XamlDocument.IncrementalSerializing += new EventHandler(XamlPerformanceEvents.XamlDocument_IncrementalSerializing);
      XamlDocument.IncrementalSerialized += new EventHandler(XamlPerformanceEvents.XamlDocument_IncrementalSerialized);
      XamlSerializer.Serializing += new EventHandler(XamlPerformanceEvents.XamlSerializer_Serializing);
      XamlSerializer.Serialized += new EventHandler(XamlPerformanceEvents.XamlSerializer_Serialized);
    }

    public static void UnregisterEvents()
    {
      XamlParser.Parsing -= new EventHandler(XamlPerformanceEvents.XamlParser_Parsing);
      XamlParser.Parsed -= new EventHandler(XamlPerformanceEvents.XamlParser_Parsed);
      XamlParser.XmlParsing -= new EventHandler(XamlPerformanceEvents.XamlParser_XmlParsing);
      XamlParser.XmlParsed -= new EventHandler(XamlPerformanceEvents.XamlParser_XmlParsed);
      XamlDocument.IncrementalSerializing -= new EventHandler(XamlPerformanceEvents.XamlDocument_IncrementalSerializing);
      XamlDocument.IncrementalSerialized -= new EventHandler(XamlPerformanceEvents.XamlDocument_IncrementalSerialized);
      XamlSerializer.Serializing -= new EventHandler(XamlPerformanceEvents.XamlSerializer_Serializing);
      XamlSerializer.Serialized -= new EventHandler(XamlPerformanceEvents.XamlSerializer_Serialized);
    }

    private static void XamlParser_Parsing(object sender, EventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XamlParserParse);
    }

    private static void XamlParser_Parsed(object sender, EventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XamlParserParse);
    }

    private static void XamlParser_XmlParsing(object sender, EventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XamlParserParseXml);
    }

    private static void XamlParser_XmlParsed(object sender, EventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XamlParserParseXml);
    }

    private static void XamlDocument_IncrementalSerializing(object sender, EventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.XamlIncrementalSerialize);
    }

    private static void XamlDocument_IncrementalSerialized(object sender, EventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.XamlIncrementalSerialize);
    }

    private static void XamlSerializer_Serializing(object sender, EventArgs e)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SerializeNodeToXaml);
    }

    private static void XamlSerializer_Serialized(object sender, EventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SerializeNodeToXaml);
    }
  }
}

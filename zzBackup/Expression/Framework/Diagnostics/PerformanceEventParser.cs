// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceEventParser
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public static class PerformanceEventParser
  {
    private static Dictionary<PerformanceEvent, string> IdToMessageDictionary = new Dictionary<PerformanceEvent, string>();
    private static Dictionary<PerformanceEvent, Guid> IdToGuidDictionary = new Dictionary<PerformanceEvent, Guid>();

    private static string EventTableLocation
    {
      get
      {
        return "Performance\\EventTable.xml";
      }
    }

    static PerformanceEventParser()
    {
      if (!PathHelper.FileExists(PerformanceEventParser.EventTableLocation))
        return;
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader reader = XmlReader.Create(PerformanceEventParser.EventTableLocation))
        xmlDocument.Load(reader);
      foreach (XmlNode xmlNode in xmlDocument.ChildNodes[0])
      {
        string str = xmlNode.Attributes["message"].Value;
        Guid guid = new Guid(xmlNode.Attributes["guid"].Value);
        PerformanceEvent key = (PerformanceEvent) Enum.Parse(typeof (PerformanceEvent), xmlNode.Attributes["ID"].Value);
        PerformanceEventParser.IdToMessageDictionary.Add(key, str);
        PerformanceEventParser.IdToGuidDictionary.Add(key, guid);
      }
    }

    public static string GetMessageForEvent(PerformanceEvent performanceEvent)
    {
      if (PerformanceEventParser.IdToMessageDictionary.ContainsKey(performanceEvent))
        return PerformanceEventParser.IdToMessageDictionary[performanceEvent];
      return performanceEvent.ToString();
    }

    public static Guid GetGuidForEvent(PerformanceEvent performanceEvent)
    {
      if (PerformanceEventParser.IdToMessageDictionary.ContainsKey(performanceEvent))
        return PerformanceEventParser.IdToGuidDictionary[performanceEvent];
      return EventTracingLogger.InfoEventGuid;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.PerformanceTester
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.PerformanceTests;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Shopdrawing.App
{
  public class PerformanceTester
  {
    private List<PerformanceTestOperation> performanceOperations;
    private IServices services;

    public PerformanceTester(IServices services)
    {
      this.performanceOperations = new List<PerformanceTestOperation>();
      this.services = services;
    }

    public void RunTestFile(string file)
    {
      if (!PathHelper.FileExists(file))
        return;
      XmlTextReader reader = new XmlTextReader(file);
      PerformanceTester.PerformanceTesterFactory performanceTesterFactory = new PerformanceTester.PerformanceTesterFactory(new PerformanceTestProjectUtilities((IServiceProvider) this.services));
      try
      {
        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element && reader.Name == "execution")
          {
            int depth = reader.Depth;
            reader.Read();
            while (reader.Depth > depth)
            {
              if (reader.NodeType == XmlNodeType.Element)
              {
                if (reader.Name == "command")
                {
                  string attribute = reader.GetAttribute("name");
                  Dictionary<string, List<string>> arguments = this.CreateArguments(reader);
                  PerformanceTestOperation command = performanceTesterFactory.CreateCommand(attribute, arguments);
                  if (command != null)
                    this.performanceOperations.Add(command);
                }
                else if (reader.Name == "function")
                {
                  string attribute1 = reader.GetAttribute("name");
                  string attribute2 = reader.GetAttribute("output");
                  Dictionary<string, List<string>> arguments = this.CreateArguments(reader);
                  PerformanceTestOperation function = performanceTesterFactory.CreateFunction(attribute1, attribute2, arguments);
                  if (function != null)
                    this.performanceOperations.Add(function);
                }
              }
              reader.Read();
            }
          }
        }
      }
      catch (XmlException ex)
      {
        ExceptionHandler.Report((Exception) ex);
      }
      finally
      {
        reader.Close();
      }
      int current = 0;
      EventHandler handler = (EventHandler) null;
      handler = (EventHandler) ((sender, args) =>
      {
        if (current == this.performanceOperations.Count)
          return;
        PerformanceTestOperation performanceTestOperation = this.performanceOperations[current];
        ++current;
        performanceTestOperation.ExecutionFinished += handler;
        performanceTestOperation.Execute();
      });
      handler((object) this, EventArgs.Empty);
    }

    private Dictionary<string, List<string>> CreateArguments(XmlTextReader reader)
    {
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
      int depth = reader.Depth;
      reader.Read();
      while (reader.Depth > depth)
      {
        if (reader.NodeType == XmlNodeType.Element && reader.Name == "argument")
        {
          string attribute1 = reader.GetAttribute("name");
          string attribute2 = reader.GetAttribute("value");
          if (attribute1 != null && attribute2 != null)
          {
            if (!dictionary.ContainsKey(attribute1))
              dictionary.Add(attribute1, new List<string>());
            dictionary[attribute1].Add(attribute2);
          }
        }
        reader.Read();
      }
      return dictionary;
    }

    private class PerformanceTesterFactory
    {
      private PerformanceTestProjectUtilities performanceTestProjectUtilities;

      public PerformanceTesterFactory(PerformanceTestProjectUtilities performanceTestProjectUtilities)
      {
        this.performanceTestProjectUtilities = performanceTestProjectUtilities;
      }

      public PerformanceTestOperation CreateCommand(string name, Dictionary<string, List<string>> arguments)
      {
        switch (name)
        {
          case "Shutdown":
            return (PerformanceTestOperation) new ShutdownOperation(this.performanceTestProjectUtilities);
          default:
            return (PerformanceTestOperation) null;
        }
      }

      public PerformanceTestOperation CreateFunction(string name, string output, Dictionary<string, List<string>> arguments)
      {
        switch (name)
        {
          case "MeasureWorkingSetStart":
            return (PerformanceTestOperation) new MeasureWorkingSetStartOperation(output);
          case "MeasureWorkingSetEnd":
            return (PerformanceTestOperation) new MeasureWorkingSetEndOperation(output);
          case "DumpHeap":
            return (PerformanceTestOperation) new DumpHeapOperation();
          default:
            return (PerformanceTestOperation) null;
        }
      }
    }
  }
}

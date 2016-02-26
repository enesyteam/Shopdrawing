// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.DebugCommands
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Shopdrawing.App
{
  internal sealed class DebugCommands : CommandTarget
  {
    public DebugCommands(IServices services)
    {
      services.GetService<IExpressionInformationService>();
      if (!DebugCommands.IsDebugMenuEnabled())
        return;
      this.AddCommand("Debug_GarbageCollect", (ICommand) new DebugCommands.GarbageCollectCommand());
      this.AddCommand("Debug_DumpInstanceDictionary", (ICommand) new DebugCommands.DumpInstanceDictionaryCommand(services));
      this.AddCommand("Debug_DumpViewNodeTree", (ICommand) new DebugCommands.DumpActiveViewTreeCommand(services));
      this.AddCommand("Debug_DisplayTimingInformation", (ICommand) new DebugCommands.DisplayTimingInformationCommand());
    }

    public static void CreateDebugMenu(ICommandBar menuBar, IServices services)
    {
      if (!DebugCommands.IsDebugMenuEnabled())
        return;
      ICommandBarMenu commandBarMenu1 = menuBar.Items.AddMenu("Debug", "Debug");
      commandBarMenu1.Items.AddButton("Debug_FocusTracker", "Focus Tracker");
      commandBarMenu1.Items.AddButton("Debug_DisplayTimingInformation", "Display Timing");
      commandBarMenu1.Items.AddButton("Debug_GarbageCollect", "Force GC");
      commandBarMenu1.Items.AddButton("Debug_UndoManager_Dump", "Dump UndoService");
      commandBarMenu1.Items.AddButton("Debug_UndoManager_Clear", "Clear UndoService");
      ICommandBarMenu commandBarMenu2 = commandBarMenu1.Items.AddMenu("Scene", "Scene");
      commandBarMenu2.Items.AddButton("Debug_CreateDocumentNodeView", "Create DocumentNode View");
      commandBarMenu2.Items.AddButton("Debug_DumpInstanceDictionary", "Dump InstanceDictionary");
      commandBarMenu2.Items.AddButton("Debug_DumpViewNodeTree", "Dump View Node Tree");
      commandBarMenu1.Items.AddMenu("Tree", "Tree");
      if (!(services.GetService<ISourceControlService>() is SourceControlService))
        return;
      commandBarMenu1.Items.AddMenu("TFS", "TFS");
    }

    public static bool IsDebugMenuEnabled()
    {
      return PerformanceUtility.LoggingEnabled;
    }

    internal sealed class GarbageCollectCommand : Command
    {
      public override void Execute()
      {
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
        GC.Collect(2);
        GC.WaitForPendingFinalizers();
      }
    }

    internal sealed class DumpInstanceDictionaryCommand : Command
    {
      private IServices services;

      public DumpInstanceDictionaryCommand(IServices services)
      {
        this.services = services;
      }

      public override void Execute()
      {
        IMessageDisplayService service = this.services.GetService<IMessageDisplayService>();
        SceneView sceneView = (SceneView) this.services.GetService<IViewService>().ActiveView;
        if (sceneView != null)
        {
          IInstanceDictionary instanceDictionary = sceneView.InstanceDictionary;
          if (instanceDictionary == null)
            return;
          StringBuilder stringBuilder1 = new StringBuilder();
          foreach (KeyValuePair<object, ViewNode> keyValuePair in (IEnumerable<KeyValuePair<object, ViewNode>>) instanceDictionary)
          {
            try
            {
              StringBuilder stringBuilder2 = new StringBuilder();
              stringBuilder2.Append(keyValuePair.Key.ToString());
              stringBuilder2.Append(": ");
              stringBuilder2.Append(keyValuePair.Value.ToString());
              stringBuilder1.AppendLine(stringBuilder2.ToString());
            }
            catch (Exception ex)
            {
              StringBuilder stringBuilder2 = new StringBuilder();
              stringBuilder2.Append(keyValuePair.Key.GetType().ToString());
              stringBuilder2.Append(": ");
              stringBuilder2.Append(keyValuePair.Value.ToString());
              stringBuilder1.AppendLine(stringBuilder2.ToString());
            }
          }
          Dump.Write(stringBuilder1.ToString());
        }
        else
          service.ShowError("No Active Scene");
      }
    }

    internal sealed class DumpActiveViewTreeCommand : Command
    {
      private IServices services;

      public DumpActiveViewTreeCommand(IServices services)
      {
        this.services = services;
      }

      public override void Execute()
      {
        IMessageDisplayService service = this.services.GetService<IMessageDisplayService>();
        SceneView sceneView = (SceneView) this.services.GetService<IViewService>().ActiveView;
        if (sceneView != null)
        {
          ViewNodeManager viewNodeManager = sceneView.InstanceBuilderContext.ViewNodeManager;
          if (viewNodeManager == null || viewNodeManager.Root == null)
            return;
          StringBuilder sb = new StringBuilder();
          XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) new StringWriter(sb));
          xmlTextWriter.Indentation = 4;
          xmlTextWriter.Formatting = Formatting.Indented;
          this.DumpViewNodeTree(viewNodeManager.Root, (XmlWriter) xmlTextWriter);
          xmlTextWriter.Flush();
          Dump.Write(sb.ToString());
        }
        else
          service.ShowError("No Active Scene");
      }

      private void DumpViewNodeTree(ViewNode root, XmlWriter xmlWriter)
      {
        if (root.Parent != null && root.IsProperty)
          xmlWriter.WriteStartElement(root.SitePropertyKey.ToString());
        xmlWriter.WriteStartElement(root.TargetType.Name);
        xmlWriter.WriteAttributeString("InstanceState", root.InstanceState.ToString());
        try
        {
          string str = root.Instance != null ? root.Instance.ToString() : "null";
          xmlWriter.WriteAttributeString("Instance", str);
        }
        catch
        {
        }
        foreach (ViewNode root1 in (IEnumerable<ViewNode>) root.Properties.Values)
          this.DumpViewNodeTree(root1, xmlWriter);
        foreach (ViewNode root1 in (IEnumerable<ViewNode>) root.Children)
          this.DumpViewNodeTree(root1, xmlWriter);
        xmlWriter.WriteEndElement();
        if (root.Parent == null || !root.IsProperty)
          return;
        xmlWriter.WriteEndElement();
      }
    }

    internal class SaveToDiskDesignTimeAssembly : Command
    {
      public override bool IsEnabled
      {
        get
        {
          return true;
        }
      }

      public override void Execute()
      {
      }
    }

    internal class DisplayTimingInformationCommand : Command
    {
      public override void Execute()
      {
        new PerformanceStatisticsWindow().Show();
      }
    }
  }
}

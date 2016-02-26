// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.ToolkitHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
  internal static class ToolkitHelper
  {
    private const string SilverlightToolkitFWLink = "http://go.microsoft.com/fwlink/?LinkId=183538";

    public static bool EnsureSilverlightToolkitTypeAvailable(ITypeResolver typeResolver, ITypeId targetType, IMessageDisplayService messageDisplayService, string installHelperMessage, string upgradeHelperMessage)
    {
      bool flag = typeResolver.EnsureAssemblyReferenced("System.Windows.Controls.Toolkit");
      if (!flag)
      {
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Message = installHelperMessage,
          HyperlinkMessage = StringTable.SilverlightToolkitInstallHyperlinkMessage,
          HyperlinkUri = new Uri("http://go.microsoft.com/fwlink/?LinkId=183538", UriKind.Absolute),
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Exclamation,
          AutomationId = "ToolkitNotInstalled"
        };
        int num = (int) messageDisplayService.ShowMessage(args);
      }
      else
      {
        flag = typeResolver.PlatformMetadata.IsSupported(typeResolver, targetType);
        if (!flag)
        {
          MessageBoxArgs args = new MessageBoxArgs()
          {
            Message = upgradeHelperMessage,
            HyperlinkMessage = StringTable.SilverlightToolkitUpdateHyperlinkMessage,
            HyperlinkUri = new Uri("http://go.microsoft.com/fwlink/?LinkId=183538", UriKind.Absolute),
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Exclamation,
            AutomationId = "ToolkitIncorrectVersion"
          };
          int num = (int) messageDisplayService.ShowMessage(args);
        }
      }
      return flag;
    }

    public static void AddToolkitReferenceIfNeeded(ITypeResolver typeResolver, ViewUpdateManager viewUpdateManager)
    {
      if (typeResolver == null || !typeResolver.IsCapabilitySet(PlatformCapability.VsmInToolkit) || !typeResolver.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager))
        return;
      string toolkitPath = ToolkitHelper.GetToolkitPath();
      if (string.IsNullOrEmpty(toolkitPath))
        return;
      typeResolver.EnsureAssemblyReferenced(toolkitPath);
      viewUpdateManager.RebuildPostponedViews();
    }

    public static string GetToolkitPath()
    {
      string path1;
      try
      {
        path1 = (string) Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders\\WPF Toolkit v3.5", (string) null, (object) null);
      }
      catch
      {
        return (string) null;
      }
      if (!string.IsNullOrEmpty(path1))
      {
        string path = Path.Combine(path1, "WPFToolkit.dll");
        if (File.Exists(path))
          return path;
      }
      return (string) null;
    }
  }
}

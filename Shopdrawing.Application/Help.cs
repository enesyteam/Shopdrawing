// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.Help
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Shopdrawing.App
{
  internal abstract class Help
  {
    protected abstract string HelpLocation { get; }

    public abstract bool Available { get; }

    public void ShowHelpTableOfContents()
    {
      if (string.IsNullOrEmpty(this.HelpLocation))
        return;
      NativeMethods.HtmlHelp(new HandleRef((object) null, IntPtr.Zero), this.HelpLocation, NativeMethods.HtmlHelpCommand.HH_DISPLAY_TOPIC, 0);
    }

    public void ShowHelpTopic(string topicUrl)
    {
      if (string.IsNullOrEmpty(this.HelpLocation))
        return;
      NativeMethods.HtmlHelp(new HandleRef((object) null, IntPtr.Zero), this.HelpLocation, NativeMethods.HtmlHelpCommand.HH_DISPLAY_TOPIC, topicUrl);
    }

    protected static string GetLocalizedHelpUrl(string rootFolder, string fileName)
    {
      for (CultureInfo cultureInfo = CultureInfo.CurrentUICulture; cultureInfo != null && !cultureInfo.Equals((object) CultureInfo.InvariantCulture); cultureInfo = cultureInfo.Parent)
      {
        string str = Path.Combine(rootFolder, cultureInfo.Name);
        string path = Path.Combine(str, fileName);
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(str) && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
          return Microsoft.Expression.Framework.Documents.PathHelper.GetShortPathName(path);
      }
      return Microsoft.Expression.Framework.Documents.PathHelper.GetShortPathName(Path.Combine(Path.Combine(rootFolder, "en"), fileName));
    }
  }
}

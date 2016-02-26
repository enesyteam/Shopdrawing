// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.NativeMethods
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System.Runtime.InteropServices;

namespace Shopdrawing.App
{
  internal static class NativeMethods
  {
    [DllImport("hhctrl.ocx", EntryPoint = "HtmlHelpW", CharSet = CharSet.Unicode)]
    public static extern int HtmlHelp(HandleRef hwndCaller, string pszFile, [MarshalAs(UnmanagedType.U4)] NativeMethods.HtmlHelpCommand command, int dwData);

    [DllImport("hhctrl.ocx", EntryPoint = "HtmlHelpW", CharSet = CharSet.Unicode)]
    public static extern int HtmlHelp(HandleRef hwndCaller, string pszFile, [MarshalAs(UnmanagedType.U4)] NativeMethods.HtmlHelpCommand command, string dwData);

    public enum HtmlHelpCommand
    {
      HH_DISPLAY_TOPIC,
      HH_DISPLAY_TOC,
      HH_DISPLAY_INDEX,
      HH_DISPLAY_SEARCH,
    }
  }
}

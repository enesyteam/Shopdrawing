// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Globalization.CultureManager
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Globalization
{
  public static class CultureManager
  {
    private static string controlText = StringTable.ControlKeyText;
    private static string altText = StringTable.AltKeyText;
    private static string shiftText = StringTable.ShiftKeyText;
    private static string windowsText = StringTable.WindowsKeyText;
    private static string forcedCulture = "en";

    public static string ForcedCulture
    {
      get
      {
        return CultureManager.forcedCulture;
      }
    }

    internal static int ForcedCultureLcid
    {
      get
      {
        switch (CultureManager.forcedCulture)
        {
          case "en":
            return 1033;
          case "fr":
            return 1036;
          case "es":
            return 3082;
          case "de":
            return 1031;
          case "it":
            return 1040;
          case "ja":
            return 1041;
          case "ko":
            return 1042;
          case "zh-Hans":
            return 2052;
          case "zh-Hant":
            return 1028;
          default:
            return 1033;
        }
      }
    }

    public static void ForceCulture(string currentDirectory, string neutralResourcesLanguage)
    {
      Dictionary<string, CultureInfo> dictionary = new Dictionary<string, CultureInfo>();
      foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
        dictionary.Add(cultureInfo.Name, cultureInfo);
      List<string> list = new List<string>();
      foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(currentDirectory).GetDirectories())
      {
        string name = fileSystemInfo.Name;
        if (dictionary.ContainsKey(name) && name != neutralResourcesLanguage)
          list.Add(name);
      }
      if (list.Count != 1)
        return;
      CultureManager.forcedCulture = list[0];
      string name1 = list[0];
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      if (!(currentUiCulture.Name != name1) || currentUiCulture.Parent != null && !(currentUiCulture.Parent.Name != name1))
        return;
      CultureInfo cultureInfo1 = new CultureInfo(name1);
      if (cultureInfo1.IsNeutralCulture)
      {
        switch (name1.ToUpper(CultureInfo.InvariantCulture))
        {
          case "ZH-HANT":
            cultureInfo1 = new CultureInfo("zh-TW");
            break;
          case "ZH-HANS":
            cultureInfo1 = new CultureInfo("zh-CN");
            break;
          default:
            cultureInfo1 = CultureInfo.CreateSpecificCulture(name1);
            break;
        }
      }
      Thread.CurrentThread.CurrentUICulture = cultureInfo1;
    }

    public static string GetShortcutText(params KeyBinding[] shortcuts)
    {
      string str1 = string.Empty;
      for (int index = 0; index < shortcuts.Length; ++index)
      {
        KeyBinding keyBinding = shortcuts[index];
        if (keyBinding.Modifiers != ModifierKeys.None || keyBinding.Key != Key.None)
        {
          string str2 = string.Empty;
          KeyGesture keyGesture = (KeyGesture) keyBinding.Gesture;
          string str3 = string.IsNullOrEmpty(keyGesture.DisplayString) ? CultureManager.GetKeyGestureText((KeyGesture) keyBinding.Gesture) : keyGesture.DisplayString;
          if (str1.Length != 0)
            str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.MultipleKeyboardShortcutFormat, new object[2]
            {
              (object) str1,
              (object) str3
            });
          else
            str1 = str3;
        }
      }
      return str1;
    }

    public static string GetKeyText(Key key)
    {
      switch (key)
      {
        case Key.Space:
          return StringTable.SpaceKeyText;
        case Key.End:
          return StringTable.EndKeyText;
        case Key.Left:
          return StringTable.LeftKeyText;
        case Key.Up:
          return StringTable.UpKeyText;
        case Key.Right:
          return StringTable.RightKeyText;
        case Key.Down:
          return StringTable.DownKeyText;
        case Key.Delete:
          return StringTable.DeleteKeyText;
        default:
          string str = new KeyConverter().ConvertToString((ITypeDescriptorContext) null, CultureInfo.InvariantCulture, (object) key);
          if (str.StartsWith("Oem", StringComparison.Ordinal) || str == "Add" || str == "Subtract")
          {
            char ch = (char) CultureManager.NativeMethods.MapVirtualKeyEx(KeyInterop.VirtualKeyFromKey(key), 2, CultureManager.NativeMethods.GetKeyboardLayout(0));
            str = (int) ch == 0 ? string.Empty : ch.ToString();
          }
          return str;
      }
    }

    private static string GetKeyGestureText(KeyGesture keyGesture)
    {
      string str = string.Empty;
      if ((keyGesture.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
        str = str + CultureManager.controlText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
        str = str + CultureManager.altText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        str = str + CultureManager.shiftText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Windows) != ModifierKeys.None)
        str = str + CultureManager.windowsText + "+";
      string keyText = CultureManager.GetKeyText(keyGesture.Key);
      if (string.IsNullOrEmpty(keyText))
        return string.Empty;
      return str + keyText;
    }

    public static void ClearDeadKeyBuffer()
    {
      CultureManager.NativeMethods.ToUnicode(KeyInterop.VirtualKeyFromKey(Key.A), 0, new byte[256], new short[2], 2, 0);
    }

    private static class NativeMethods
    {
      public const int MAPVK_VK_TO_CHAR = 2;

      [DllImport("user32.dll")]
      public static extern IntPtr GetKeyboardLayout(int idThread);

      [DllImport("User32.dll", EntryPoint = "MapVirtualKeyExW", CharSet = CharSet.Unicode)]
      public static extern int MapVirtualKeyEx(int uCode, int uMapType, IntPtr dwhkl);

      [DllImport("user32.dll")]
      public static extern int ToUnicode(int uVirtKey, int uScanCode, byte[] lpKeyState, short[] lpChar, int cchBuff, int uFlags);
    }
  }
}

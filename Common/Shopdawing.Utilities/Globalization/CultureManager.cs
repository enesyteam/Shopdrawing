// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Globalization.CultureManager
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Commands;
using Microsoft.Expression.Utility.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Microsoft.Expression.Utility.Globalization
{
  public static class CultureManager
  {
    private static CultureInfo forcedCulture;

    private static string AltKeyText
    {
      get
      {
        return StringTable.AltKeyText;
      }
    }

    private static string ControlKeyText
    {
      get
      {
        return StringTable.ControlKeyText;
      }
    }

    private static string DeleteKeyText
    {
      get
      {
        return StringTable.DeleteKeyText;
      }
    }

    private static string DownKeyText
    {
      get
      {
        return StringTable.DownKeyText;
      }
    }

    private static string EndKeyText
    {
      get
      {
        return StringTable.EndKeyText;
      }
    }

    private static string LeftKeyText
    {
      get
      {
        return StringTable.LeftKeyText;
      }
    }

    private static string RightKeyText
    {
      get
      {
        return StringTable.RightKeyText;
      }
    }

    private static string UpKeyText
    {
      get
      {
        return StringTable.UpKeyText;
      }
    }

    private static string ShiftKeyText
    {
      get
      {
        return StringTable.ShiftKeyText;
      }
    }

    private static string SpaceKeyText
    {
      get
      {
        return StringTable.SpaceKeyText;
      }
    }

    private static string WindowsKeyText
    {
      get
      {
        return StringTable.WindowsKeyText;
      }
    }

    private static string MultipleKeyboardShortcutFormat
    {
      get
      {
        return StringTable.MultipleKeyboardShortcutFormat;
      }
    }

    public static CultureInfo ForcedCulture
    {
      get
      {
        return CultureManager.forcedCulture;
      }
      private set
      {
        CultureManager.forcedCulture = value;
      }
    }

    public static CultureInfo CommandLineCulture { get; set; }

    public static CultureInfo UserLanguagePreference { get; set; }

    public static CultureInfo PendingUserLanguagePreference { get; set; }

    private static CultureInfo StartupCulture { get; set; }

    private static CultureInfo[] ShippedCultures { get; set; }

    public static bool AvailableLanguagesChangedSinceLastSession { get; private set; }

    public static bool UserLanguagePreferenceAvailableAtStartup { get; private set; }

    public static bool CurrentUserNeedsToSetLanguagePreference { get; private set; }

    public static IEnumerable<CultureInfo> PreferredCultures
    {
      get
      {
        if (CultureManager.CommandLineCulture != null)
          yield return CultureManager.CommandLineCulture;
        if (CultureManager.UserLanguagePreference != null && !CultureManager.UserLanguagePreference.Equals((object) CultureManager.CommandLineCulture))
          yield return CultureManager.UserLanguagePreference;
        if (CultureManager.StartupCulture != null && !CultureManager.StartupCulture.Equals((object) CultureManager.UserLanguagePreference) && !CultureManager.StartupCulture.Equals((object) CultureManager.CommandLineCulture))
          yield return CultureManager.StartupCulture;
        foreach (CultureInfo cultureInfo in CultureManager.ShippedCultures)
        {
          if (!cultureInfo.Equals((object) CultureManager.StartupCulture) && !cultureInfo.Equals((object) CultureManager.UserLanguagePreference) && !cultureInfo.Equals((object) CultureManager.CommandLineCulture))
            yield return cultureInfo;
        }
      }
    }

    public static IEnumerable<CultureInfo> PreferredCulturesExtended
    {
      get
      {
        return Enumerable.Concat<CultureInfo>(CultureManager.PreferredCultures, (IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.AllCultures));
      }
    }

    public static int ForcedCultureLcid
    {
      get
      {
        switch (CultureManager.ForcedCulture.Name)
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
          case "ru":
            return 1049;
          case "zh-Hans":
            return 2052;
          case "zh-Hant":
            return 1028;
          default:
            return 1033;
        }
      }
    }

    static CultureManager()
    {
      CultureManager.StartupCulture = CultureInfo.CurrentUICulture;
      CultureManager.ForcedCulture = CultureInfo.CurrentUICulture;
      CultureManager.ShippedCultures = new CultureInfo[10]
      {
        new CultureInfo("en-US"),
        new CultureInfo("fr-FR"),
        new CultureInfo("de-DE"),
        new CultureInfo("es-ES"),
        new CultureInfo("it-IT"),
        new CultureInfo("ru-RU"),
        new CultureInfo("ja-JP"),
        new CultureInfo("ko-KR"),
        new CultureInfo("zh-TW"),
        new CultureInfo("zh-CN")
      };
    }

    public static CultureInfo ParseCultureName(string cultureName)
    {
      CultureInfo cultureInfo = (CultureInfo) null;
      try
      {
        cultureInfo = new CultureInfo(cultureName);
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
      catch (CultureNotFoundException ex)
      {
      }
      return cultureInfo;
    }

    public static CultureInfo ParseCultureLcid(string lcidText)
    {
      int lcid = -1;
      try
      {
        lcid = int.Parse(lcidText, (IFormatProvider) Thread.CurrentThread.CurrentUICulture.NumberFormat);
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      CultureInfo cultureInfo = (CultureInfo) null;
      if (lcid != -1)
        cultureInfo = CultureManager.ParseCultureLcid(lcid);
      return cultureInfo;
    }

    public static CultureInfo ParseCultureLcid(int lcid)
    {
      CultureInfo cultureInfo = (CultureInfo) null;
      try
      {
        cultureInfo = new CultureInfo(lcid);
      }
      catch (ArgumentOutOfRangeException ex)
      {
      }
      catch (CultureNotFoundException ex)
      {
      }
      return cultureInfo;
    }

    public static ICollection<CultureInfo> FindInstalledCultures(string directory)
    {
      List<CultureInfo> list = new List<CultureInfo>();
      foreach (CultureInfo culture in CultureManager.ShippedCultures)
      {
        if (!string.IsNullOrEmpty(LocalizationHelper.FindFolderForCulture(culture, directory, false)))
          list.Add(culture);
      }
      return (ICollection<CultureInfo>) list;
    }

    public static void ForceCulture(string currentDirectory, CultureInfo preferredCulture)
    {
      CultureManager.CommandLineCulture = preferredCulture;
      ICollection<CultureInfo> installedCultures = CultureManager.FindInstalledCultures(currentDirectory);
      IEnumerable<CultureInfo> preferredCultures = CultureManager.PreferredCultures;
      List<CultureInfo> list = new List<CultureInfo>();
      foreach (CultureInfo cultureInfo in preferredCultures)
      {
        if (installedCultures.Contains(cultureInfo) && !list.Contains(cultureInfo))
          list.Add(cultureInfo);
      }
      if (list.Count < 1)
        return;
      CultureManager.forcedCulture = list[0];
      Thread.CurrentThread.CurrentCulture = CultureManager.forcedCulture;
      Thread.CurrentThread.CurrentUICulture = CultureManager.forcedCulture;
      CultureInfo.DefaultThreadCurrentUICulture = CultureManager.forcedCulture;
    }

    public static void InitializeWithCurrentUserContext(string applicationVersionedRegistryPath)
    {
      CultureManager.ReadUserLanguagePreferencePropertyFromRegistry(applicationVersionedRegistryPath);
      Collection<CultureInfo> collection = CultureManager.ReadLastKnownInstalledLanguagesFromRegistry(applicationVersionedRegistryPath);
      ICollection<CultureInfo> installedCultures = CultureManager.FindInstalledCultures(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
      CultureManager.AvailableLanguagesChangedSinceLastSession = !Enumerable.SequenceEqual<CultureInfo>((IEnumerable<CultureInfo>) collection, (IEnumerable<CultureInfo>) installedCultures, (IEqualityComparer<CultureInfo>) new CultureInfoLcidComparer());
      if (CultureManager.UserLanguagePreference != null)
        CultureManager.UserLanguagePreferenceAvailableAtStartup = Enumerable.Contains<CultureInfo>((IEnumerable<CultureInfo>) installedCultures, CultureManager.UserLanguagePreference, (IEqualityComparer<CultureInfo>) new CultureInfoLcidComparer());
      if (installedCultures.Count > 1 && (CultureManager.AvailableLanguagesChangedSinceLastSession || !CultureManager.UserLanguagePreferenceAvailableAtStartup))
        CultureManager.CurrentUserNeedsToSetLanguagePreference = true;
      CultureManager.WriteLastKnownInstalledLanguagesToRegistry(applicationVersionedRegistryPath);
    }

    public static void ReadUserLanguagePreferencePropertyFromRegistry(string applicationVersionedRegistryPath)
    {
      try
      {
        string cultureName = RegistryHelper.RetrieveCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LanguagePreference");
        if (string.IsNullOrEmpty(cultureName))
          return;
        CultureManager.UserLanguagePreference = CultureManager.ParseCultureName(cultureName);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public static void WritePendingUserLanguagePreferencePropertyToRegistry(string applicationVersionedRegistryPath)
    {
      string str = CultureManager.PendingUserLanguagePreference == null ? CultureManager.UserLanguagePreference.Name : CultureManager.PendingUserLanguagePreference.Name;
      RegistryHelper.SetCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LanguagePreference", str);
    }

    public static Collection<CultureInfo> ReadLastKnownInstalledLanguagesFromRegistry(string applicationVersionedRegistryPath)
    {
      Collection<CultureInfo> collection = new Collection<CultureInfo>();
      try
      {
        string str1 = RegistryHelper.RetrieveCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LastKnownInstalledLanguages");
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1;
          char[] chArray = new char[1]
          {
            ';'
          };
          foreach (string cultureName in str2.Split(chArray))
          {
            CultureInfo cultureInfo = CultureManager.ParseCultureName(cultureName);
            if (cultureInfo != null)
              collection.Add(cultureInfo);
          }
        }
      }
      catch (InvalidCastException ex)
      {
      }
      return collection;
    }

    public static void WriteLastKnownInstalledLanguagesToRegistry(string applicationVersionedRegistryPath)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (CultureInfo cultureInfo in (IEnumerable<CultureInfo>) CultureManager.FindInstalledCultures(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(';');
        stringBuilder.Append(cultureInfo.Name);
      }
      RegistryHelper.SetCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LastKnownInstalledLanguages", stringBuilder.ToString());
    }

    public static string GetShortcutText(params CommandKeyBinding[] shortcuts)
    {
      KeyBinding[] keyBindingArray = new KeyBinding[shortcuts.Length];
      for (int index = 0; index < shortcuts.Length; ++index)
      {
        keyBindingArray[index] = new KeyBinding();
        keyBindingArray[index].Key = shortcuts[index].Key;
        keyBindingArray[index].Modifiers = shortcuts[index].Modifiers;
      }
      return CultureManager.GetShortcutText(keyBindingArray);
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
            str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, CultureManager.MultipleKeyboardShortcutFormat, new object[2]
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
          return CultureManager.SpaceKeyText;
        case Key.End:
          return CultureManager.EndKeyText;
        case Key.Left:
          return CultureManager.LeftKeyText;
        case Key.Up:
          return CultureManager.UpKeyText;
        case Key.Right:
          return CultureManager.RightKeyText;
        case Key.Down:
          return CultureManager.DownKeyText;
        case Key.Delete:
          return CultureManager.DeleteKeyText;
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
        str = str + CultureManager.ControlKeyText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
        str = str + CultureManager.AltKeyText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        str = str + CultureManager.ShiftKeyText + "+";
      if ((keyGesture.Modifiers & ModifierKeys.Windows) != ModifierKeys.None)
        str = str + CultureManager.WindowsKeyText + "+";
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

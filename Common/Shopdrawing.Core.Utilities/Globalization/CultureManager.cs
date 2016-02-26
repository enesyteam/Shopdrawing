using Microsoft.Expression.Utility.Commands;
using Microsoft.Expression.Utility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

		public static bool AvailableLanguagesChangedSinceLastSession
		{
			get;
			private set;
		}

		public static CultureInfo CommandLineCulture
		{
			get;
			set;
		}

		private static string ControlKeyText
		{
			get
			{
				return StringTable.ControlKeyText;
			}
		}

		public static bool CurrentUserNeedsToSetLanguagePreference
		{
			get;
			private set;
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

		public static int ForcedCultureLcid
		{
			get
			{
				string name = CultureManager.ForcedCulture.Name;
				string str = name;
				if (name != null)
				{
					switch (str)
					{
						case "en":
						{
							return 1033;
						}
						case "fr":
						{
							return 1036;
						}
						case "es":
						{
							return 3082;
						}
						case "de":
						{
							return 1031;
						}
						case "it":
						{
							return 1040;
						}
						case "ja":
						{
							return 1041;
						}
						case "ko":
						{
							return 1042;
						}
						case "ru":
						{
							return 1049;
						}
						case "zh-Hans":
						{
							return 2052;
						}
						case "zh-Hant":
						{
							return 1028;
						}
					}
				}
				return 1033;
			}
		}

		private static string LeftKeyText
		{
			get
			{
				return StringTable.LeftKeyText;
			}
		}

		private static string MultipleKeyboardShortcutFormat
		{
			get
			{
				return StringTable.MultipleKeyboardShortcutFormat;
			}
		}

		public static CultureInfo PendingUserLanguagePreference
		{
			get;
			set;
		}

		public static IEnumerable<CultureInfo> PreferredCultures
		{
			get
			{
				if (CultureManager.CommandLineCulture != null)
				{
					yield return CultureManager.CommandLineCulture;
				}
				if (CultureManager.UserLanguagePreference != null && !CultureManager.UserLanguagePreference.Equals(CultureManager.CommandLineCulture))
				{
					yield return CultureManager.UserLanguagePreference;
				}
				if (CultureManager.StartupCulture != null && !CultureManager.StartupCulture.Equals(CultureManager.UserLanguagePreference) && !CultureManager.StartupCulture.Equals(CultureManager.CommandLineCulture))
				{
					yield return CultureManager.StartupCulture;
				}
				CultureInfo[] shippedCultures = CultureManager.ShippedCultures;
				for (int i = 0; i < (int)shippedCultures.Length; i++)
				{
					CultureInfo cultureInfo = shippedCultures[i];
					if (!cultureInfo.Equals(CultureManager.StartupCulture) && !cultureInfo.Equals(CultureManager.UserLanguagePreference) && !cultureInfo.Equals(CultureManager.CommandLineCulture))
					{
						yield return cultureInfo;
					}
				}
			}
		}

		public static IEnumerable<CultureInfo> PreferredCulturesExtended
		{
			get
			{
				return CultureManager.PreferredCultures.Concat<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));
			}
		}

		private static string RightKeyText
		{
			get
			{
				return StringTable.RightKeyText;
			}
		}

		private static string ShiftKeyText
		{
			get
			{
				return StringTable.ShiftKeyText;
			}
		}

		private static CultureInfo[] ShippedCultures
		{
			get;
			set;
		}

		private static string SpaceKeyText
		{
			get
			{
				return StringTable.SpaceKeyText;
			}
		}

		private static CultureInfo StartupCulture
		{
			get;
			set;
		}

		private static string UpKeyText
		{
			get
			{
				return StringTable.UpKeyText;
			}
		}

		public static CultureInfo UserLanguagePreference
		{
			get;
			set;
		}

		public static bool UserLanguagePreferenceAvailableAtStartup
		{
			get;
			private set;
		}

		private static string WindowsKeyText
		{
			get
			{
				return StringTable.WindowsKeyText;
			}
		}

		static CultureManager()
		{
			CultureManager.StartupCulture = CultureInfo.CurrentUICulture;
			CultureManager.ForcedCulture = CultureInfo.CurrentUICulture;
			CultureInfo[] cultureInfo = new CultureInfo[] { new CultureInfo("en-US"), new CultureInfo("fr-FR"), new CultureInfo("de-DE"), new CultureInfo("es-ES"), new CultureInfo("it-IT"), new CultureInfo("ru-RU"), new CultureInfo("ja-JP"), new CultureInfo("ko-KR"), new CultureInfo("zh-TW"), new CultureInfo("zh-CN") };
			CultureManager.ShippedCultures = cultureInfo;
		}

		public static void ClearDeadKeyBuffer()
		{
			int num = KeyInterop.VirtualKeyFromKey(Key.A);
			byte[] numArray = new byte[256];
			short[] numArray1 = new short[2];
			CultureManager.NativeMethods.ToUnicode(num, 0, numArray, numArray1, 2, 0);
		}

		public static ICollection<CultureInfo> FindInstalledCultures(string directory)
		{
			List<CultureInfo> cultureInfos = new List<CultureInfo>();
			CultureInfo[] shippedCultures = CultureManager.ShippedCultures;
			for (int i = 0; i < (int)shippedCultures.Length; i++)
			{
				CultureInfo cultureInfo = shippedCultures[i];
				if (!string.IsNullOrEmpty(LocalizationHelper.FindFolderForCulture(cultureInfo, directory, false)))
				{
					cultureInfos.Add(cultureInfo);
				}
			}
			return cultureInfos;
		}

		public static void ForceCulture(string currentDirectory, CultureInfo preferredCulture)
		{
			CultureManager.CommandLineCulture = preferredCulture;
			ICollection<CultureInfo> cultureInfos = CultureManager.FindInstalledCultures(currentDirectory);
			IEnumerable<CultureInfo> preferredCultures = CultureManager.PreferredCultures;
			List<CultureInfo> cultureInfos1 = new List<CultureInfo>();
			foreach (CultureInfo cultureInfo in preferredCultures)
			{
				if (!cultureInfos.Contains(cultureInfo) || cultureInfos1.Contains(cultureInfo))
				{
					continue;
				}
				cultureInfos1.Add(cultureInfo);
			}
			if (cultureInfos1.Count >= 1)
			{
				CultureManager.forcedCulture = cultureInfos1[0];
				Thread.CurrentThread.CurrentCulture = CultureManager.forcedCulture;
				Thread.CurrentThread.CurrentUICulture = CultureManager.forcedCulture;
				CultureInfo.DefaultThreadCurrentUICulture = CultureManager.forcedCulture;
			}
		}

		private static string GetKeyGestureText(KeyGesture keyGesture)
		{
			string empty = string.Empty;
			if ((keyGesture.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
			{
				empty = string.Concat(empty, CultureManager.ControlKeyText, "+");
			}
			if ((keyGesture.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
			{
				empty = string.Concat(empty, CultureManager.AltKeyText, "+");
			}
			if ((keyGesture.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
			{
				empty = string.Concat(empty, CultureManager.ShiftKeyText, "+");
			}
			if ((keyGesture.Modifiers & ModifierKeys.Windows) != ModifierKeys.None)
			{
				empty = string.Concat(empty, CultureManager.WindowsKeyText, "+");
			}
			string keyText = CultureManager.GetKeyText(keyGesture.Key);
			if (string.IsNullOrEmpty(keyText))
			{
				return string.Empty;
			}
			return string.Concat(empty, keyText);
		}

		public static string GetKeyText(Key key)
		{
			string str;
			int num;
			IntPtr keyboardLayout;
			char chr;
			string str1;
			Key key1 = key;
			switch (key1)
			{
				case Key.Space:
				{
					return CultureManager.SpaceKeyText;
				}
				case Key.Prior:
				case Key.Next:
				case Key.Home:
				{
					str = (new KeyConverter()).ConvertToString(null, CultureInfo.InvariantCulture, key);
					if (str.StartsWith("Oem", StringComparison.Ordinal) || str == "Add" || str == "Subtract")
					{
						num = KeyInterop.VirtualKeyFromKey(key);
						keyboardLayout = CultureManager.NativeMethods.GetKeyboardLayout(0);
						chr = (char)CultureManager.NativeMethods.MapVirtualKeyEx(num, 2, keyboardLayout);
						str1 = (chr == 0 ? string.Empty : chr.ToString());
						str = str1;
					}
					return str;
				}
				case Key.End:
				{
					return CultureManager.EndKeyText;
				}
				case Key.Left:
				{
					return CultureManager.LeftKeyText;
				}
				case Key.Up:
				{
					return CultureManager.UpKeyText;
				}
				case Key.Right:
				{
					return CultureManager.RightKeyText;
				}
				case Key.Down:
				{
					return CultureManager.DownKeyText;
				}
				default:
				{
					if (key1 != Key.Delete)
					{
						str = (new KeyConverter()).ConvertToString(null, CultureInfo.InvariantCulture, key);
						if (str.StartsWith("Oem", StringComparison.Ordinal) || str == "Add" || str == "Subtract")
						{
							num = KeyInterop.VirtualKeyFromKey(key);
							keyboardLayout = CultureManager.NativeMethods.GetKeyboardLayout(0);
							chr = (char)CultureManager.NativeMethods.MapVirtualKeyEx(num, 2, keyboardLayout);
							str1 = (chr == 0 ? string.Empty : chr.ToString());
							str = str1;
						}
						return str;
					}
					return CultureManager.DeleteKeyText;
				}
			}
		}

		public static string GetShortcutText(params CommandKeyBinding[] shortcuts)
		{
			KeyBinding[] keyBinding = new KeyBinding[(int)shortcuts.Length];
			for (int i = 0; i < (int)shortcuts.Length; i++)
			{
				keyBinding[i] = new KeyBinding();
				keyBinding[i].Key = shortcuts[i].Key;
				keyBinding[i].Modifiers = shortcuts[i].Modifiers;
			}
			return CultureManager.GetShortcutText(keyBinding);
		}

		public static string GetShortcutText(params KeyBinding[] shortcuts)
		{
			string empty = string.Empty;
			for (int i = 0; i < (int)shortcuts.Length; i++)
			{
				KeyBinding keyBinding = shortcuts[i];
				if (keyBinding.Modifiers != ModifierKeys.None || keyBinding.Key != Key.None)
				{
					string str = string.Empty;
					KeyGesture gesture = (KeyGesture)keyBinding.Gesture;
					str = (string.IsNullOrEmpty(gesture.DisplayString) ? CultureManager.GetKeyGestureText((KeyGesture)keyBinding.Gesture) : gesture.DisplayString);
					if (empty.Length == 0)
					{
						empty = str;
					}
					else
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string multipleKeyboardShortcutFormat = CultureManager.MultipleKeyboardShortcutFormat;
						object[] objArray = new object[] { empty, str };
						empty = string.Format(currentCulture, multipleKeyboardShortcutFormat, objArray);
					}
				}
			}
			return empty;
		}

		public static void InitializeWithCurrentUserContext(string applicationVersionedRegistryPath)
		{
			CultureManager.ReadUserLanguagePreferencePropertyFromRegistry(applicationVersionedRegistryPath);
			Collection<CultureInfo> cultureInfos = CultureManager.ReadLastKnownInstalledLanguagesFromRegistry(applicationVersionedRegistryPath);
			ICollection<CultureInfo> cultureInfos1 = CultureManager.FindInstalledCultures(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			CultureManager.AvailableLanguagesChangedSinceLastSession = !cultureInfos.SequenceEqual<CultureInfo>(cultureInfos1, new CultureInfoLcidComparer());
			if (CultureManager.UserLanguagePreference != null)
			{
				CultureManager.UserLanguagePreferenceAvailableAtStartup = cultureInfos1.Contains<CultureInfo>(CultureManager.UserLanguagePreference, new CultureInfoLcidComparer());
			}
			if (cultureInfos1.Count > 1 && (CultureManager.AvailableLanguagesChangedSinceLastSession || !CultureManager.UserLanguagePreferenceAvailableAtStartup))
			{
				CultureManager.CurrentUserNeedsToSetLanguagePreference = true;
			}
			CultureManager.WriteLastKnownInstalledLanguagesToRegistry(applicationVersionedRegistryPath);
		}

		public static CultureInfo ParseCultureLcid(string lcidText)
		{
			int num = -1;
			try
			{
				num = int.Parse(lcidText, Thread.CurrentThread.CurrentUICulture.NumberFormat);
			}
			catch (FormatException formatException)
			{
			}
			catch (OverflowException overflowException)
			{
			}
			CultureInfo cultureInfo = null;
			if (num != -1)
			{
				cultureInfo = CultureManager.ParseCultureLcid(num);
			}
			return cultureInfo;
		}

		public static CultureInfo ParseCultureLcid(int lcid)
		{
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(lcid);
			}
			catch (ArgumentOutOfRangeException argumentOutOfRangeException)
			{
			}
			catch (CultureNotFoundException cultureNotFoundException)
			{
			}
			return cultureInfo;
		}

		public static CultureInfo ParseCultureName(string cultureName)
		{
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(cultureName);
			}
			catch (ArgumentOutOfRangeException argumentOutOfRangeException)
			{
			}
			catch (CultureNotFoundException cultureNotFoundException)
			{
			}
			return cultureInfo;
		}

		public static Collection<CultureInfo> ReadLastKnownInstalledLanguagesFromRegistry(string applicationVersionedRegistryPath)
		{
			Collection<CultureInfo> cultureInfos = new Collection<CultureInfo>();
			try
			{
				string str = RegistryHelper.RetrieveCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LastKnownInstalledLanguages");
				if (!string.IsNullOrEmpty(str))
				{
					string[] strArrays = str.Split(new char[] { ';' });
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						CultureInfo cultureInfo = CultureManager.ParseCultureName(strArrays[i]);
						if (cultureInfo != null)
						{
							cultureInfos.Add(cultureInfo);
						}
					}
				}
			}
			catch (InvalidCastException invalidCastException)
			{
			}
			return cultureInfos;
		}

		public static void ReadUserLanguagePreferencePropertyFromRegistry(string applicationVersionedRegistryPath)
		{
			try
			{
				string str = RegistryHelper.RetrieveCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LanguagePreference");
				if (!string.IsNullOrEmpty(str))
				{
					CultureManager.UserLanguagePreference = CultureManager.ParseCultureName(str);
				}
			}
			catch (InvalidCastException invalidCastException)
			{
			}
		}

		public static void WriteLastKnownInstalledLanguagesToRegistry(string applicationVersionedRegistryPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ICollection<CultureInfo> cultureInfos = CultureManager.FindInstalledCultures(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(cultureInfo.Name);
			}
			RegistryHelper.SetCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LastKnownInstalledLanguages", stringBuilder.ToString());
		}

		public static void WritePendingUserLanguagePreferencePropertyToRegistry(string applicationVersionedRegistryPath)
		{
			string str;
			str = (CultureManager.PendingUserLanguagePreference == null ? CultureManager.UserLanguagePreference.Name : CultureManager.PendingUserLanguagePreference.Name);
			RegistryHelper.SetCurrentUserRegistryValue<string>(applicationVersionedRegistryPath, "LanguagePreference", str);
		}

		private static class NativeMethods
		{
			public const int MAPVK_VK_TO_CHAR = 2;

			[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr GetKeyboardLayout(int idThread);

			[DllImport("User32.dll", CharSet=CharSet.Unicode, EntryPoint="MapVirtualKeyExW", ExactSpelling=true)]
			public static extern int MapVirtualKeyEx(int uCode, int uMapType, IntPtr dwhkl);

			[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern int ToUnicode(int uVirtKey, int uScanCode, byte[] lpKeyState, short[] lpChar, int cchBuff, int uFlags);
		}
	}
}
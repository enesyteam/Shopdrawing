// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.RegistryHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.Expression.Utility.IO
{
  public static class RegistryHelper
  {
    private static Exception RegistryExceptionWrapper(Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        if (ex is ArgumentException || ex is IOException || (ex is ObjectDisposedException || ex is SecurityException) || ex is UnauthorizedAccessException)
          return ex;
        throw;
      }
      return (Exception) null;
    }

    private static T RetrieveRegistryValue<T>(RegistryHelper.Hives hive, string subkeyName, string valueName)
    {
      switch (hive)
      {
        case RegistryHelper.Hives.LocalMachine:
          return RegistryHelper.RetrieveRegistryValue<T>(Registry.LocalMachine, subkeyName, valueName);
        case RegistryHelper.Hives.CurrentUser:
          return RegistryHelper.RetrieveRegistryValue<T>(Registry.CurrentUser, subkeyName, valueName);
        case RegistryHelper.Hives.DefaultUser:
          return RegistryHelper.RetrieveRegistryValue<T>(Registry.Users, ".DEFAULT\\" + subkeyName, valueName);
        default:
          return default (T);
      }
    }

    public static T RetrieveCurrentUserRegistryValue<T>(string subkeyName, string valueName)
    {
      return RegistryHelper.RetrieveRegistryValue<T>(Registry.CurrentUser, subkeyName, valueName);
    }

    public static T RetrieveLocalMachineRegistryValue<T>(string subkeyName, string valueName)
    {
      return RegistryHelper.RetrieveRegistryValue<T>(Registry.LocalMachine, subkeyName, valueName);
    }

    public static T RetrieveDefaultUserRegistryValue<T>(string subkeyName, string valueName)
    {
      return RegistryHelper.RetrieveRegistryValue<T>(Registry.Users, ".DEFAULT\\" + subkeyName, valueName);
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string valueName)
    {
      object registryValue = (object) null;
      RegistryHelper.RegistryExceptionWrapper((Action) (() => registryValue = registryKey.GetValue(valueName)));
      if (registryValue != null)
        return TypeHelper.ConvertType<T>(registryValue);
      return default (T);
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string valueName, T defaultValue)
    {
      object registryValue = (object) null;
      RegistryHelper.RegistryExceptionWrapper((Action) (() => registryValue = registryKey.GetValue(valueName)));
      if (registryValue != null)
        return TypeHelper.ConvertType<T>(registryValue);
      return defaultValue;
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName)
    {
      RegistryKey registryKey1 = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registryKey1 == null)
        return default (T);
      using (registryKey1)
        return RegistryHelper.RetrieveRegistryValue<T>(registryKey1, valueName);
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName, T defaultValue)
    {
      RegistryKey registryKey1 = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registryKey1 == null)
        return defaultValue;
      using (registryKey1)
        return RegistryHelper.RetrieveRegistryValue<T>(registryKey1, valueName, defaultValue);
    }

    public static T RetrieveVisualStudioRegistryValue<T>(VisualStudioRegistryLocation primaryKey, string subkeyName, string valueName)
    {
      return Enumerable.FirstOrDefault<T>(RegistryHelper.RetrieveAllVisualStudioRegistryValues<T>(primaryKey, subkeyName, valueName));
    }

    public static IEnumerable<T> RetrieveAllVisualStudioRegistryValues<T>(VisualStudioRegistryLocation primaryKey, string subkeyName, string valueName)
    {
      string subKeyName1;
      string subKeyName2;
      switch (primaryKey)
      {
        case VisualStudioRegistryLocation.LocalMachine:
        case VisualStudioRegistryLocation.User:
          subKeyName1 = "Software\\Microsoft\\VisualStudio\\11.0\\" + subkeyName ?? string.Empty;
          subKeyName2 = "Software\\Microsoft\\VSWinExpress\\11.0\\" + subkeyName ?? string.Empty;
          break;
        case VisualStudioRegistryLocation.UserConfig:
          subKeyName1 = "Software\\Microsoft\\VisualStudio\\11.0_Config\\" + subkeyName ?? string.Empty;
          subKeyName2 = "Software\\Microsoft\\VSWinExpress\\11.0_Config\\" + subkeyName ?? string.Empty;
          break;
        default:
          return Enumerable.Empty<T>();
      }
      switch (primaryKey)
      {
        case VisualStudioRegistryLocation.LocalMachine:
          return RegistryHelper.EnumerateValues<T>(new RegistryHelper.Lookup(RegistryHelper.Hives.LocalMachine, subKeyName1, valueName), new RegistryHelper.Lookup(RegistryHelper.Hives.LocalMachine, subKeyName2, valueName));
        case VisualStudioRegistryLocation.User:
        case VisualStudioRegistryLocation.UserConfig:
          return RegistryHelper.EnumerateValues<T>(new RegistryHelper.Lookup(RegistryHelper.Hives.CurrentUser, subKeyName1, valueName), new RegistryHelper.Lookup(RegistryHelper.Hives.CurrentUser, subKeyName2, valueName), new RegistryHelper.Lookup(RegistryHelper.Hives.DefaultUser, subKeyName1, valueName), new RegistryHelper.Lookup(RegistryHelper.Hives.DefaultUser, subKeyName2, valueName));
        default:
          return Enumerable.Empty<T>();
      }
    }

    private static IEnumerable<T> EnumerateValues<T>(params RegistryHelper.Lookup[] lookups)
    {
      foreach (RegistryHelper.Lookup lookup in lookups)
      {
        object value = RegistryHelper.RetrieveRegistryValue<object>(lookup.Hive, lookup.SubKeyName, lookup.ValueName);
        if (value != null)
          yield return TypeHelper.ConvertType<T>(value);
      }
    }

    public static bool AclRegistryKeyForAllApplicationPackages(RegistryKey key, string subkey)
    {
      RegistryKey registryKey = RegistryHelper.OpenSubkey(key, subkey, true);
      if (registryKey != null)
      {
        RegistrySecurity accessControl = registryKey.GetAccessControl();
        accessControl.AddAccessRule(new RegistryAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, RegistryRights.ExecuteKey, AccessControlType.Allow));
        try
        {
          registryKey.SetAccessControl(accessControl);
        }
        catch (UnauthorizedAccessException ex)
        {
          return false;
        }
        catch (ObjectDisposedException ex)
        {
          return false;
        }
      }
      return true;
    }

    public static RegistryKey OpenSubkey(RegistryKey registryKey, string subkeyName, bool writable = false)
    {
      RegistryKey registrySubkey = (RegistryKey) null;
      RegistryHelper.RegistryExceptionWrapper((Action) (() => registrySubkey = registryKey.OpenSubKey(subkeyName, writable)));
      return registrySubkey;
    }

    public static IEnumerable<RegistryKey> GetSubkeys(RegistryKey registryKey)
    {
      foreach (string subkeyName in RegistryHelper.GetSubkeyNames(registryKey))
      {
        RegistryKey subkey = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
        if (subkey != null)
          yield return subkey;
      }
    }

    public static IEnumerable<RegistryKey> GetSubkeys(RegistryKey registryKey, string subkeyName)
    {
      RegistryKey registrySubkey = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registrySubkey != null)
      {
        using (registrySubkey)
        {
          foreach (RegistryKey registryKey1 in RegistryHelper.GetSubkeys(registrySubkey))
            yield return registryKey1;
        }
      }
    }

    public static IEnumerable<string> GetSubkeyNames(RegistryKey registryKey)
    {
      string[] subkeyNames = (string[]) null;
      RegistryHelper.RegistryExceptionWrapper((Action) (() => subkeyNames = registryKey.GetSubKeyNames()));
      if (subkeyNames != null)
        return (IEnumerable<string>) subkeyNames;
      return Enumerable.Empty<string>();
    }

    public static IEnumerable<string> GetSubkeyNames(RegistryKey registryKey, string subkeyName)
    {
      RegistryKey registryKey1 = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registryKey1 == null)
        return Enumerable.Empty<string>();
      using (registryKey1)
        return RegistryHelper.GetSubkeyNames(registryKey1);
    }

    public static IEnumerable<T> RetrieveAllRegistrySubkeyValues<T>(RegistryKey registryKey, string subkeyName)
    {
      List<T> list = new List<T>();
      RegistryKey registryKey1 = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registryKey1 != null)
      {
        using (registryKey1)
        {
          foreach (RegistryKey registryKey2 in RegistryHelper.GetSubkeys(registryKey1))
          {
            RegistryKey subSubkey = registryKey2;
            using (subSubkey)
            {
              string[] registryValueNames = (string[]) null;
              if (RegistryHelper.RegistryExceptionWrapper((Action) (() => registryValueNames = subSubkey.GetValueNames())) == null)
              {
                foreach (string valueName in registryValueNames)
                {
                  object source = RegistryHelper.RetrieveRegistryValue<object>(subSubkey, valueName);
                  if (source != null)
                  {
                    T obj = TypeHelper.ConvertType<T>(source);
                    list.Add(obj);
                  }
                }
              }
            }
          }
        }
      }
      return (IEnumerable<T>) list;
    }

    public static bool SetRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName, T value)
    {
      return RegistryHelper.SetRegistryValue<T>(registryKey, subkeyName, valueName, value, RegistryValueKind.Unknown);
    }

    public static bool SetRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName, T value, RegistryValueKind valueKind)
    {
      RegistryKey registrySubkey = RegistryHelper.OpenSubkey(registryKey, subkeyName, true);
      if (registrySubkey == null)
      {
        if ((object) value == null)
          return true;
        if (RegistryHelper.RegistryExceptionWrapper((Action) (() => registrySubkey = registryKey.CreateSubKey(subkeyName))) != null)
          return false;
      }
      if (registrySubkey == null)
        return false;
      try
      {
        return ((object) value != null ? RegistryHelper.RegistryExceptionWrapper((Action) (() => registrySubkey.SetValue(valueName, (object) (T) value, valueKind))) : RegistryHelper.RegistryExceptionWrapper((Action) (() => registrySubkey.DeleteValue(valueName, false)))) == null;
      }
      finally
      {
        registrySubkey.Close();
      }
    }

    public static bool SetCurrentUserRegistryValue<T>(string subkeyName, string valueName, T value)
    {
      return RegistryHelper.SetCurrentUserRegistryValue<T>(subkeyName, valueName, value, RegistryValueKind.Unknown);
    }

    public static bool SetCurrentUserRegistryValue<T>(string subkeyName, string valueName, T value, RegistryValueKind valueKind)
    {
      return RegistryHelper.SetRegistryValue<T>(Registry.CurrentUser, subkeyName, valueName, value, valueKind);
    }

    private enum Hives
    {
      LocalMachine,
      CurrentUser,
      DefaultUser,
    }

    private struct Lookup
    {
      public RegistryHelper.Hives Hive;
      public string SubKeyName;
      public string ValueName;

      public Lookup(RegistryHelper.Hives hive, string subKeyName, string valueName)
      {
        this.Hive = hive;
        this.SubKeyName = subKeyName;
        this.ValueName = valueName;
      }
    }
  }
}

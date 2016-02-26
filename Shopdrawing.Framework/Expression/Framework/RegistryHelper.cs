// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.RegistryHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace Microsoft.Expression.Framework
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

    public static T RetrieveCurrentUserRegistryValue<T>(string subkeyName, string valueName)
    {
      return RegistryHelper.RetrieveRegistryValue<T>(Registry.CurrentUser, subkeyName, valueName);
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string valueName)
    {
      object registryValue = (object) null;
      RegistryHelper.RegistryExceptionWrapper((Action) (() => registryValue = registryKey.GetValue(valueName)));
      if (registryValue != null)
        return TypeHelper.ConvertType<T>(registryValue);
      return default (T);
    }

    public static T RetrieveRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName)
    {
      RegistryKey registryKey1 = RegistryHelper.OpenSubkey(registryKey, subkeyName, false);
      if (registryKey1 == null)
        return default (T);
      using (registryKey1)
        return RegistryHelper.RetrieveRegistryValue<T>(registryKey1, valueName);
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
          using (IEnumerator<RegistryKey> enumerator = RegistryHelper.GetSubkeys(registryKey1).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              RegistryKey subSubkey = enumerator.Current;
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
      }
      return (IEnumerable<T>) list;
    }

    public static bool SetRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName, T value)
    {
      return RegistryHelper.SetRegistryValue<T>(registryKey, subkeyName, valueName, value, RegistryValueKind.Unknown);
    }

    public static bool SetRegistryValue<T>(RegistryKey registryKey, string subkeyName, string valueName, T value, RegistryValueKind valueKind)
    {
      bool writable = true;
      RegistryKey registrySubkey = RegistryHelper.OpenSubkey(registryKey, subkeyName, writable);
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
  }
}

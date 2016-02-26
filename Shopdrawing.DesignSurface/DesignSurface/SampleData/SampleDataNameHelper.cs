// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataNameHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public static class SampleDataNameHelper
  {
    public static string GetSafeName(string name, IMSBuildProject msBuildProject, bool fileSystemFriendly)
    {
      string identifier = SampleDataNameHelper.NormalizeName(name, fileSystemFriendly);
      if (string.IsNullOrEmpty(identifier) || !msBuildProject.IsSafeIdentifier(identifier))
        return (string) null;
      return identifier;
    }

    public static string GetUniqueName(string name, IMSBuildProject msBuildProject, IEnumerable<string> existingNames, string nameToIgnore, bool fileSystemFriendly)
    {
      string str = SampleDataNameHelper.NormalizeName(name, fileSystemFriendly);
      if (string.IsNullOrEmpty(str) || !msBuildProject.IsSafeIdentifier(str))
        return (string) null;
      if (SampleDataNameHelper.IsUniqueAndSafeName(str, msBuildProject, existingNames, nameToIgnore, fileSystemFriendly))
        return str;
      NumberedName numberedName = new NumberedName(str);
      while (numberedName.Increment())
      {
        string currentName = numberedName.CurrentName;
        if (SampleDataNameHelper.IsUniqueAndSafeName(currentName, msBuildProject, existingNames, nameToIgnore, fileSystemFriendly))
          return currentName;
      }
      return (string) null;
    }

    private static bool IsUniqueAndSafeName(string normalizedName, IMSBuildProject msBuildProject, IEnumerable<string> existingNames, string nameToIgnore, bool fileSystemFriendly)
    {
      if (!msBuildProject.IsSafeIdentifier(normalizedName) || fileSystemFriendly && PathHelper.IsDeviceName(normalizedName))
        return false;
      if (normalizedName == nameToIgnore)
        return true;
      if (normalizedName == "xmlns")
        return false;
      string normalizedName2 = (int) normalizedName[0] == 95 ? normalizedName.Substring(1) : (string) null;
      return Enumerable.FirstOrDefault<string>(existingNames, (Func<string, bool>) (name => !(name == nameToIgnore) && (string.Compare(normalizedName, name, StringComparison.OrdinalIgnoreCase) == 0 || normalizedName2 != null && string.Compare(normalizedName2, name, StringComparison.OrdinalIgnoreCase) == 0))) == null;
    }

    private static string NormalizeName(string name, bool fileSystemFriendly)
    {
      string str1 = name != null ? name.Trim() : name;
      if (string.IsNullOrEmpty(str1))
        return (string) null;
      bool flag = true;
      for (int charIndex = 0; flag && charIndex < str1.Length; ++charIndex)
        flag = SampleDataNameHelper.IsCharAllowed(str1[charIndex], charIndex);
      if (flag)
      {
        if (!SampleDataNameHelper.IsXmlFriendly(str1))
          return (string) null;
        if (fileSystemFriendly && PathHelper.IsDeviceName(str1))
          str1 = "_" + str1;
        return str1;
      }
      StringBuilder stringBuilder = new StringBuilder(str1.Length + 1);
      int num = 0;
      for (int charIndex = 0; charIndex < str1.Length; ++charIndex)
      {
        char c = str1[charIndex];
        if (!SampleDataNameHelper.IsCharAllowed(c, charIndex))
          c = '_';
        else
          ++num;
        stringBuilder.Append(c);
      }
      if (num == 0)
        return (string) null;
      string str2 = stringBuilder.ToString();
      if (fileSystemFriendly && PathHelper.IsDeviceName(str2))
        str2 = "_" + str2;
      if (!SampleDataNameHelper.IsXmlFriendly(str2))
        str2 = (string) null;
      return str2;
    }

    private static bool IsXmlFriendly(string normalizedName)
    {
      try
      {
        XmlConvert.VerifyNCName(normalizedName);
      }
      catch (XmlException ex)
      {
        return false;
      }
      return true;
    }

    private static bool IsCharAllowed(char c, int charIndex)
    {
      return char.IsLetter(c) || charIndex > 0 && (int) c >= 48 && (int) c <= 57;
    }
  }
}

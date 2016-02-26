// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Globalization.LocalizationHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.Utility.Globalization
{
  public static class LocalizationHelper
  {
    public static string TranslatedFolder(string rootFolderName)
    {
      return LocalizationHelper.FindLocalizedSubdirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof (LocalizationHelper)).Location), rootFolderName), false);
    }

    public static string FindLocalizedSubdirectory(string rootDirectory, bool useLcidFormat = false)
    {
      foreach (CultureInfo culture in CultureManager.PreferredCulturesExtended)
      {
        string folderForCulture = LocalizationHelper.FindFolderForCulture(culture, rootDirectory, useLcidFormat);
        if (!string.IsNullOrEmpty(folderForCulture))
          return folderForCulture;
      }
      return (string) null;
    }

    internal static string FindFolderForCulture(CultureInfo culture, string rootDirectory, bool useLcidFormat)
    {
      for (; culture != null && !culture.Equals((object) CultureInfo.InvariantCulture); culture = culture.Parent)
      {
        string path = useLcidFormat ? Path.Combine(rootDirectory, culture.LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : Path.Combine(rootDirectory, culture.Name);
        if (Microsoft.Expression.Utility.IO.PathHelper.DirectoryExists(path))
          return path;
      }
      return (string) null;
    }
  }
}

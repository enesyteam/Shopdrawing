// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontResolver : IFontResolver
  {
    private Dictionary<string, string> gdiToWpfProjectFontNameMap = new Dictionary<string, string>();
    private Dictionary<string, string> wpfToGdiProjectFontNameMap = new Dictionary<string, string>();
    private Dictionary<string, Dictionary<string, List<KeyValuePair<Typeface, string>>>> projectFontFamilyLookup = new Dictionary<string, Dictionary<string, List<KeyValuePair<Typeface, string>>>>();
    private Dictionary<string, string> fontCache = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, List<KeyValuePair<Typeface, string>>> gdiFontFamilyLookup = new Dictionary<string, List<KeyValuePair<Typeface, string>>>();
    private static Dictionary<string, List<KeyValuePair<Typeface, string>>> wpfFontFamilyLookup = new Dictionary<string, List<KeyValuePair<Typeface, string>>>();
    private static Dictionary<string, string> gdiToWpfSystemFontNameMap = new Dictionary<string, string>();
    private static Dictionary<string, string> wpfToGdiSystemFontNameMap = new Dictionary<string, string>();
    private static string fontCacheFolderRoot = "Microsoft\\Expression\\Blend\\Font Cache";
    private static string fontCacheLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FontResolver.fontCacheFolderRoot);
    private IServiceProvider serviceProvider;
    private static Dictionary<string, List<string>> gdiFontFamilyToFontFileMap;
    private static Dictionary<string, List<string>> wpfFontFamilyToFontFileMap;
    private string fontCacheDirectory;
    private static bool createdAnyFontResolvers;
    private bool useGdiFontNames;

    public string FontCacheDirectory
    {
      get
      {
        return this.fontCacheDirectory;
      }
    }

    internal FontResolver(IServiceProvider serviceProvider, bool useGdiFontNames)
    {
      this.serviceProvider = serviceProvider;
      this.useGdiFontNames = useGdiFontNames;
      FontResolver.createdAnyFontResolvers = true;
    }

    public static void CleanFontCache()
    {
      if (FontResolver.createdAnyFontResolvers)
        return;
      try
      {
        ProjectPathHelper.CleanDirectory(FontResolver.fontCacheLocation, true);
      }
      catch
      {
      }
    }

    public void AddProjectFont(string fontItemPath)
    {
      string directoryName = Path.GetDirectoryName(fontItemPath);
      Dictionary<string, List<KeyValuePair<Typeface, string>>> cache;
      if (!this.projectFontFamilyLookup.TryGetValue(directoryName, out cache))
      {
        cache = new Dictionary<string, List<KeyValuePair<Typeface, string>>>();
        this.projectFontFamilyLookup[directoryName] = cache;
      }
      FontResolver.AddFontFileToCache(this.GetCachedFont(fontItemPath), cache, this.useGdiFontNames, this.gdiToWpfProjectFontNameMap, this.wpfToGdiProjectFontNameMap);
    }

    public void RemoveProjectFont(string fontItemPath)
    {
      string directoryName = Path.GetDirectoryName(fontItemPath);
      Dictionary<string, List<KeyValuePair<Typeface, string>>> cache;
      if (!this.projectFontFamilyLookup.TryGetValue(directoryName, out cache))
        return;
      FontResolver.RemoveFontFileFromCache(this.GetCachedFont(fontItemPath), cache, this.useGdiFontNames);
      if (cache.Count != 0)
        return;
      this.projectFontFamilyLookup.Remove(directoryName);
    }

    public string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext)
    {
      string fontFamilyPath = FontEmbedder.GetFontFamilyPath(fontFamilySource);
      FontFamily fontFamily = FontEmbedder.MakeDesignTimeFontReference(new FontFamily(fontFamilySource), documentContext);
      Dictionary<string, List<KeyValuePair<Typeface, string>>> dictionary;
      if (string.IsNullOrEmpty(fontFamilyPath))
      {
        FontResolver.EnsureSystemFontCached(fontFamilySource);
        dictionary = this.useGdiFontNames ? FontResolver.gdiFontFamilyLookup : FontResolver.wpfFontFamilyLookup;
      }
      else
      {
        Uri uri = documentContext.MakeDesignTimeUri(new Uri(fontFamilyPath, UriKind.RelativeOrAbsolute));
        if (!uri.IsAbsoluteUri)
          return fontFamilySource;
        string localPath = uri.LocalPath;
        if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(localPath) && !localPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
          return FontEmbedder.MakeSilverlightFontReference(this.GetCachedFont(localPath) + FontEmbedder.GetFontFamilySpecifier(fontFamilySource));
        if (!this.projectFontFamilyLookup.TryGetValue(Path.GetDirectoryName(uri.LocalPath), out dictionary))
        {
          int num = fontFamilySource.IndexOf(";component", StringComparison.OrdinalIgnoreCase);
          if (num != -1 && fontFamilySource.StartsWith("/", StringComparison.OrdinalIgnoreCase))
          {
            IFontResolver resolverForComponent = this.GetFontResolverForComponent(fontFamilySource.Substring(1, num - 1));
            if (resolverForComponent != null && resolverForComponent != this)
              return resolverForComponent.ResolveFont(fontFamilySource, fontStretch, fontStyle, fontWeight, documentContext);
          }
          return fontFamilySource;
        }
      }
      string fontNameFromSource = FontEmbedder.GetFontNameFromSource(fontFamily);
      List<KeyValuePair<Typeface, string>> list;
      if (dictionary.TryGetValue(fontNameFromSource, out list))
      {
        string path = (string) null;
        FontResolver.FontMatch fontMatch1 = (FontResolver.FontMatch) null;
        foreach (KeyValuePair<Typeface, string> keyValuePair in list)
        {
          FontResolver.FontMatch fontMatch2 = new FontResolver.FontMatch(keyValuePair.Key, fontStretch, fontStyle, fontWeight);
          if (fontMatch1 == null || fontMatch2.CompareTo((object) fontMatch1) > 0)
          {
            path = keyValuePair.Value;
            fontMatch1 = fontMatch2;
          }
        }
        fontFamilySource = FontEmbedder.MakeSilverlightFontReference(Path.GetFullPath(path));
      }
      return fontFamilySource;
    }

    public string ConvertToWpfFontName(string gdiFontName)
    {
      return this.ConvertFontName(gdiFontName, FontResolver.gdiToWpfSystemFontNameMap, this.gdiToWpfProjectFontNameMap);
    }

    public string ConvertToGdiFontName(string wpfFontName)
    {
      return this.ConvertFontName(wpfFontName, FontResolver.wpfToGdiSystemFontNameMap, this.wpfToGdiProjectFontNameMap);
    }

    private string ConvertFontName(string sourceName, Dictionary<string, string> systemFontNameMap, Dictionary<string, string> projectFontNameMap)
    {
      if (string.IsNullOrEmpty(sourceName))
        return sourceName;
      FontResolver.EnsureSystemFontCache();
      string str;
      if (!systemFontNameMap.TryGetValue(sourceName, out str) && !projectFontNameMap.TryGetValue(sourceName, out str))
        str = sourceName;
      return str;
    }

    public string GetCachedFont(string fontFile)
    {
      string str = (string) null;
      if (!this.fontCache.TryGetValue(fontFile, out str))
      {
        str = Path.Combine(this.EnsureFontCacheDirectory(), Path.GetFileName(fontFile));
        this.fontCache[fontFile] = str;
        if (!File.Exists(str))
        {
          if (File.Exists(fontFile))
          {
            try
            {
              File.Copy(fontFile, str);
            }
            catch (IOException ex)
            {
            }
          }
        }
      }
      return str;
    }

    private IFontResolver GetFontResolverForComponent(string component)
    {
      IProjectManager projectManager = (IProjectManager) this.serviceProvider.GetService(typeof (IProjectManager));
      if (projectManager != null && projectManager.CurrentSolution != null)
      {
        foreach (IProject project in projectManager.CurrentSolution.Projects)
        {
          IXamlProject xamlProject = project as IXamlProject;
          if (xamlProject != null && project.TargetAssembly != null && string.Equals(project.TargetAssembly.Name, component, StringComparison.OrdinalIgnoreCase))
            return xamlProject.ProjectContext.FontResolver;
        }
      }
      return (IFontResolver) null;
    }

    private string EnsureFontCacheDirectory()
    {
      if (this.fontCacheDirectory == null)
      {
        do
        {
          this.fontCacheDirectory = Path.Combine(FontResolver.fontCacheLocation, Path.GetRandomFileName());
        }
        while (Directory.Exists(this.fontCacheDirectory));
        Directory.CreateDirectory(this.fontCacheDirectory);
      }
      else if (!Directory.Exists(this.fontCacheDirectory))
        Directory.CreateDirectory(this.fontCacheDirectory);
      return this.fontCacheDirectory;
    }

    private static void AddFontFileToCache(string fontFile, Dictionary<string, List<KeyValuePair<Typeface, string>>> cache, bool useGdiFontNames, Dictionary<string, string> gdiFontNameMap, Dictionary<string, string> wpfFontNameMap)
    {
      foreach (string str in FontEmbedder.GetFontNamesInFile(fontFile, useGdiFontNames))
      {
        string index = FontFamilyHelper.EnsureFamilyName(str);
        FontFamily fontFamily = new FontFamily(fontFile + "#" + index);
        List<KeyValuePair<Typeface, string>> list;
        if (!cache.TryGetValue(str, out list))
        {
          list = new List<KeyValuePair<Typeface, string>>();
          cache[index] = list;
        }
        foreach (Typeface key in (IEnumerable<Typeface>) FontEmbedder.GetTypefacesSafe(fontFile))
          list.Add(new KeyValuePair<Typeface, string>(key, fontFamily.Source));
      }
      if (gdiFontNameMap == null)
        return;
      foreach (Typeface typeface in (IEnumerable<Typeface>) FontEmbedder.GetTypefacesSafe(fontFile))
      {
        bool useGdiFontNames1 = true;
        string serializeFontFamilyName1 = FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames1);
        bool useGdiFontNames2 = false;
        string serializeFontFamilyName2 = FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames2);
        if (serializeFontFamilyName1 != serializeFontFamilyName2 && !string.IsNullOrEmpty(serializeFontFamilyName1) && !string.IsNullOrEmpty(serializeFontFamilyName2))
        {
          gdiFontNameMap[serializeFontFamilyName1] = serializeFontFamilyName2;
          wpfFontNameMap[serializeFontFamilyName2] = serializeFontFamilyName1;
        }
      }
    }

    private static void RemoveFontFileFromCache(string fontFile, Dictionary<string, List<KeyValuePair<Typeface, string>>> cache, bool useGdiFontNames)
    {
      foreach (string familyName in FontEmbedder.GetFontNamesInFile(fontFile, useGdiFontNames))
      {
        string key = FontFamilyHelper.EnsureFamilyName(familyName);
        List<KeyValuePair<Typeface, string>> list;
        if (cache.TryGetValue(key, out list))
        {
          using (IEnumerator<Typeface> enumerator = FontEmbedder.GetTypefacesSafe(fontFile).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Typeface typeface = enumerator.Current;
              list.RemoveAll((Predicate<KeyValuePair<Typeface, string>>) (putativeTypeface => typeface == putativeTypeface.Key));
            }
          }
          if (list.Count == 0)
            cache.Remove(key);
        }
      }
    }

    private static void StoreSystemFont(Dictionary<string, List<string>> fontFamilyToFontFileMap, string fontName, string fontFile)
    {
      List<string> list;
      if (!fontFamilyToFontFileMap.TryGetValue(fontName, out list))
      {
        list = new List<string>();
        fontFamilyToFontFileMap[fontName] = list;
      }
      if (list.Contains(fontFile))
        return;
      list.Add(fontFile);
    }

    private static void StoreFontLookup(Dictionary<string, List<KeyValuePair<Typeface, string>>> fontFamilyLookup, Dictionary<string, List<string>> fontFamilyToFontFileMap, string fontSource, bool useGdiFontNames)
    {
      if (fontFamilyLookup.ContainsKey(fontSource))
        return;
      string serializeFontFamilyName = FontEmbedder.GetSerializeFontFamilyName(new FontFamily(fontSource), useGdiFontNames);
      List<string> list;
      if (!fontFamilyToFontFileMap.TryGetValue(serializeFontFamilyName, out list))
        return;
      foreach (string fontFile in list)
        FontResolver.AddFontFileToCache(fontFile, fontFamilyLookup, useGdiFontNames, (Dictionary<string, string>) null, (Dictionary<string, string>) null);
    }

    private static void EnsureSystemFontCache()
    {
      if (FontResolver.gdiFontFamilyToFontFileMap != null)
        return;
      FontResolver.gdiFontFamilyToFontFileMap = new Dictionary<string, List<string>>();
      FontResolver.wpfFontFamilyToFontFileMap = new Dictionary<string, List<string>>();
      foreach (string str in Directory.GetFiles(FontEmbedder.SystemFontsDirectory))
      {
        foreach (Typeface typeface in (IEnumerable<Typeface>) FontEmbedder.GetTypefacesSafe(str))
        {
          bool useGdiFontNames1 = true;
          string serializeFontFamilyName1 = FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames1);
          bool useGdiFontNames2 = false;
          string serializeFontFamilyName2 = FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames2);
          FontResolver.StoreSystemFont(FontResolver.gdiFontFamilyToFontFileMap, serializeFontFamilyName1, str);
          FontResolver.StoreSystemFont(FontResolver.wpfFontFamilyToFontFileMap, serializeFontFamilyName2, str);
          if (serializeFontFamilyName1 != serializeFontFamilyName2)
          {
            FontResolver.gdiToWpfSystemFontNameMap[serializeFontFamilyName1] = serializeFontFamilyName2;
            FontResolver.wpfToGdiSystemFontNameMap[serializeFontFamilyName2] = serializeFontFamilyName1;
          }
        }
      }
    }

    private static void EnsureSystemFontCached(string fontSource)
    {
      FontResolver.EnsureSystemFontCache();
      bool useGdiFontNames1 = true;
      FontResolver.StoreFontLookup(FontResolver.gdiFontFamilyLookup, FontResolver.gdiFontFamilyToFontFileMap, fontSource, useGdiFontNames1);
      bool useGdiFontNames2 = false;
      FontResolver.StoreFontLookup(FontResolver.wpfFontFamilyLookup, FontResolver.wpfFontFamilyToFontFileMap, fontSource, useGdiFontNames2);
    }

    private class FontMatch : IComparable
    {
      private int exactMatches;
      private int fontWeightDifference;

      public FontMatch(Typeface typeface, object fontStretch, object fontStyle, object fontWeight)
      {
        if (typeface.Stretch.ToString() == fontStretch.ToString())
          ++this.exactMatches;
        bool flag1 = false;
        bool flag2 = false;
        try
        {
          flag1 = typeface.IsObliqueSimulated;
          flag2 = typeface.IsBoldSimulated;
        }
        catch
        {
        }
        if (typeface.Style.ToString() == fontStyle.ToString() && !flag1)
          ++this.exactMatches;
        if (typeface.Weight.ToString() == fontWeight.ToString())
        {
          if (!flag2)
          {
            ++this.exactMatches;
            return;
          }
        }
        try
        {
          this.fontWeightDifference = int.MaxValue;
          string text = fontWeight.ToString();
          if (string.IsNullOrEmpty(text))
            return;
          this.fontWeightDifference = Math.Abs(((FontWeight) new FontWeightConverter().ConvertFromInvariantString(text)).ToOpenTypeWeight() - typeface.Weight.ToOpenTypeWeight());
        }
        catch
        {
        }
      }

      public int CompareTo(object rhs)
      {
        FontResolver.FontMatch fontMatch = (FontResolver.FontMatch) rhs;
        if (this.exactMatches != fontMatch.exactMatches)
          return this.exactMatches - fontMatch.exactMatches;
        return -(this.fontWeightDifference - fontMatch.fontWeightDifference);
      }
    }
  }
}

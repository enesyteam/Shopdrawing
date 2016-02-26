// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontEmbedder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.DocumentProcessors;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class FontEmbedder
  {
    private static FrameworkNameDictionary<bool> isSubsettingTaskInstalledForPlatform = new FrameworkNameDictionary<bool>();
    internal static string[] silverlightFontNames = new string[11]
    {
      "Portable User Interface",
      "Arial",
      "Arial Black",
      "Comic Sans MS",
      "Courier New",
      "Georgia",
      "Lucida Sans Unicode",
      "Times New Roman",
      "Trebuchet MS",
      "Verdana",
      "Webdings"
    };
    public const string PortableUserInterface = "Portable User Interface";
    private SceneViewModel viewModel;
    private static IEnumerable<SystemFontFamily> wpfFonts;
    private static IEnumerable<SystemFontFamily> gdiFonts;

    public static string SystemFontsDirectory
    {
      get
      {
        return Path.Combine(Environment.GetEnvironmentVariable("windir"), "Fonts");
      }
    }

    public FontEmbedder(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public static string GetSubsetFontTargetFileName(FrameworkName targetFramework)
    {
      return BlendSdkHelper.GetFontTargetsFile(BlendSdkHelper.GetCurrentFramework(targetFramework)) ?? string.Empty;
    }

    public static bool IsSubsetFontTargetInstalled(ITypeResolver context)
    {
      ProjectXamlContext projectXamlContext = (ProjectXamlContext) context;
      FrameworkName targetFramework = projectXamlContext.TargetFramework;
      bool flag;
      if (!FontEmbedder.isSubsettingTaskInstalledForPlatform.TryGetValue(targetFramework, out flag))
      {
        string buildExtensionsPath = projectXamlContext.MSBuildExtensionsPath;
        flag = Microsoft.Expression.Framework.Documents.PathHelper.FileExists(FontEmbedder.GetSubsetFontTargetFileName(targetFramework).Replace("$(MSBuildExtensionsPath)", buildExtensionsPath));
        FontEmbedder.isSubsettingTaskInstalledForPlatform.Add(targetFramework, flag);
      }
      return flag;
    }

    public static bool AreFontsEqual(FontFamily lhs, FontFamily rhs, IDocumentContext documentContext)
    {
      if (FontEmbedder.GetFontNameFromSource(lhs) == FontEmbedder.GetFontNameFromSource(rhs))
        return true;
      bool flag1 = lhs == null || lhs.Source == null;
      bool flag2 = rhs == null || rhs.Source == null;
      if (flag1 && flag2)
        return true;
      if (flag1 || flag2)
        return false;
      if (lhs.Source == rhs.Source)
        return true;
      string fontFamilyPath1 = FontEmbedder.GetFontFamilyPath(lhs.Source);
      string fontFamilyPath2 = FontEmbedder.GetFontFamilyPath(rhs.Source);
      bool flag3 = !string.IsNullOrEmpty(fontFamilyPath1);
      bool flag4 = !string.IsNullOrEmpty(fontFamilyPath2);
      if (!flag3 && !flag4 || (!flag3 || !flag4))
        return false;
      return FontEmbedder.GetFontNameFromSource(FontEmbedder.MakeDesignTimeFontFamily(lhs, documentContext)) == FontEmbedder.GetFontNameFromSource(FontEmbedder.MakeDesignTimeFontFamily(rhs, documentContext));
    }

    public static FontFamily MakeProjectFontReference(IProjectFont projectFont, IDocumentContext documentContext)
    {
      return FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, projectFont.IsEmbedded);
    }

    private static FontFamily MakeRelativeFontFamily(IProjectFont projectFont, IDocumentContext documentContext, bool useZipForSilverlight)
    {
      string path = documentContext.MakeResourceReference(projectFont.FontDocumentPath);
      string path1 = (!string.IsNullOrEmpty(path) ? Path.GetDirectoryName(path) : (string) null) ?? string.Empty;
      string path2 = "#" + projectFont.FontFamilyName;
      if (!documentContext.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf))
        path2 = !useZipForSilverlight ? Path.GetFileName(projectFont.FontDocumentPath) + path2 : "Fonts.zip" + path2;
      else if (string.IsNullOrEmpty(path1))
        path2 = "./" + path2;
      return new FontFamily((Uri) null, FontFamilyHelper.EnsureFamilyName(FontEmbedder.MakeSilverlightFontReference(Path.Combine(path1, path2))));
    }

    public static string MakeSilverlightFontReference(string fontReference)
    {
      return fontReference.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static FontFamily MakeDesignTimeFontReference(FontFamily fontFamily, IDocumentContext documentContext)
    {
      if (fontFamily.Source != null && fontFamily.Source.IndexOf('#') != -1)
        return FontEmbedder.MakeDesignTimeFontFamily(fontFamily, documentContext);
      return fontFamily;
    }

    public static FontFamily MakeDesignTimeFontFamily(FontFamily fontFamily, IDocumentContext documentContext)
    {
      string[] strArray = fontFamily.Source.Split(',');
      List<string> list = new List<string>();
      foreach (string str in strArray)
      {
        if (Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(str))
        {
          Uri result = (Uri) null;
          if (Uri.TryCreate(str, UriKind.Relative, out result))
          {
            Uri uri = documentContext.MakeDesignTimeUri(result);
            if (uri.IsAbsoluteUri)
              list.Add(uri.LocalPath);
            else
              list.Add(uri.ToString());
          }
        }
        else
          list.Add(str);
      }
      for (int index = 0; index < list.Count; ++index)
        list[index] = list[index].Replace("Fonts.zip#", "#");
      if (list.Count <= 0)
        return fontFamily;
      string familyName = list[0];
      for (int index = 1; index < list.Count; ++index)
        familyName = familyName + (object) "," + (string) (object) familyName[index];
      return new FontFamily((Uri) null, FontFamilyHelper.EnsureFamilyName(familyName));
    }

    public static FontFamily MakeSystemFont(FontFamily fontFamily)
    {
      return new FontFamily(fontFamily.BaseUri, FontEmbedder.GetFontNameFromSource(fontFamily));
    }

    public static string GetFontNameFromSource(FontFamily fontFamily)
    {
      if (fontFamily == null)
        return string.Empty;
      if (fontFamily.Source == null)
      {
        XmlLanguage language = XmlLanguage.GetLanguage("en-US");
        try
        {
          string str;
          if (fontFamily.FamilyNames.TryGetValue(language, out str))
            return str;
        }
        catch (ArgumentException ex)
        {
        }
        return string.Empty;
      }
      int num = fontFamily.Source.IndexOf('#');
      if (num != -1)
        return fontFamily.Source.Substring(num + 1);
      return fontFamily.Source;
    }

    public static string GetSerializeFontFamilyName(FontFamily fontFamily, bool useGdiFontNames)
    {
      Typeface typeface = Enumerable.FirstOrDefault<Typeface>((IEnumerable<Typeface>) fontFamily.GetTypefaces());
      if (typeface != null)
        return FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames);
      return string.Empty;
    }

    public static string GetSerializeFontFamilyName(Typeface typeface, bool useGdiFontNames)
    {
      string str1 = FontFamilyHelper.EnsureFamilyName(FontEmbedder.GetGdiName(typeface.FontFamily));
      string familyName;
      if (useGdiFontNames)
      {
        familyName = str1;
      }
      else
      {
        string str2 = FontEmbedder.GetFontFamilyPath(typeface.FontFamily.Source);
        if (string.IsNullOrEmpty(str2))
          str2 = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(FontEmbedder.SystemFontsDirectory);
        FontFamily fontFamily = new FontFamily(str2 + "#" + str1);
        string wpfName1 = FontEmbedder.GetWpfName(typeface.FontFamily);
        string wpfName2 = FontEmbedder.GetWpfName(fontFamily);
        familyName = !(wpfName1 != wpfName2) ? str1 : wpfName1;
      }
      return FontFamilyHelper.EnsureFamilyName(familyName);
    }

    public static string GetGdiName(FontFamily family)
    {
      string str = string.Empty;
      Typeface typeface = Enumerable.FirstOrDefault<Typeface>((IEnumerable<Typeface>) family.GetTypefaces());
      try
      {
        if (typeface != null)
        {
          GlyphTypeface glyphTypeface;
          if (typeface.TryGetGlyphTypeface(out glyphTypeface))
          {
            if (!glyphTypeface.Win32FamilyNames.TryGetValue(CultureInfo.GetCultureInfo("en-US"), out str))
              str = Enumerable.FirstOrDefault<string>((IEnumerable<string>) glyphTypeface.Win32FamilyNames.Values) ?? string.Empty;
          }
        }
      }
      catch
      {
        str = FontEmbedder.GetWpfName(family);
      }
      return str;
    }

    public static string GetWpfName(FontFamily fontFamily)
    {
      string str = string.Empty;
      try
      {
        if (!fontFamily.FamilyNames.TryGetValue(XmlLanguage.GetLanguage("en-US"), out str))
          str = Enumerable.FirstOrDefault<string>((IEnumerable<string>) fontFamily.FamilyNames.Values) ?? string.Empty;
      }
      catch (ArgumentException ex)
      {
        if (!string.IsNullOrEmpty(fontFamily.Source))
          str = FontEmbedder.GetFontNameFromSource(fontFamily);
      }
      return str;
    }

    public static string GetFontFamilyPath(string source)
    {
      int length = source.IndexOf('#');
      if (length != -1)
        return source.Substring(0, length);
      return string.Empty;
    }

    public static string GetFontFamilySpecifier(string source)
    {
      int startIndex = source.IndexOf('#');
      if (startIndex != -1)
        return source.Substring(startIndex);
      return string.Empty;
    }

    public void EnsureFontSubsettingTask()
    {
      this.viewModel.DesignerContext.ActiveProject.AddImport(FontEmbedder.GetSubsetFontTargetFileName(this.viewModel.ProjectContext.TargetFramework));
    }

    public static IEnumerable<string> GetFontNamesInFile(string fontFile, bool useGdiFontNames)
    {
      foreach (FontFamily fontFamily in (IEnumerable<FontFamily>) FontEmbedder.GetFontFamiliesSafe(fontFile))
        yield return FontEmbedder.GetSerializeFontFamilyName(fontFamily, useGdiFontNames);
    }

    public static ICollection<FontFamily> GetFontFamiliesSafe(string file)
    {
      try
      {
        return Fonts.GetFontFamilies(file);
      }
      catch
      {
        return (ICollection<FontFamily>) new EmptyList<FontFamily>();
      }
    }

    public static ICollection<Typeface> GetTypefacesSafe(string file)
    {
      try
      {
        return Fonts.GetTypefaces(file);
      }
      catch
      {
        return (ICollection<Typeface>) new EmptyList<Typeface>();
      }
    }

    public static void CreateSystemFontFamiliesCache()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.FontCacheInitialization);
      Dictionary<string, SystemFontFamily> fontFileMap1 = new Dictionary<string, SystemFontFamily>();
      Dictionary<string, SystemFontFamily> fontFileMap2 = new Dictionary<string, SystemFontFamily>();
      foreach (string str in Directory.GetFiles(FontEmbedder.SystemFontsDirectory))
      {
        foreach (Typeface typeface in (IEnumerable<Typeface>) FontEmbedder.GetTypefacesSafe(str))
        {
          bool useGdiFontNames1 = false;
          FontEmbedder.StoreSystemFont(str, typeface, fontFileMap1, useGdiFontNames1);
          bool useGdiFontNames2 = true;
          FontEmbedder.StoreSystemFont(str, typeface, fontFileMap2, useGdiFontNames2);
        }
      }
      FontEmbedder.wpfFonts = (IEnumerable<SystemFontFamily>) Enumerable.ToList<SystemFontFamily>((IEnumerable<SystemFontFamily>) fontFileMap1.Values);
      SystemFontFamily systemFontFamily = new SystemFontFamily(new FontFamily("Portable User Interface"));
      fontFileMap2["Portable User Interface"] = systemFontFamily;
      FontEmbedder.gdiFonts = (IEnumerable<SystemFontFamily>) Enumerable.ToList<SystemFontFamily>((IEnumerable<SystemFontFamily>) fontFileMap2.Values);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.FontCacheInitialization);
    }

    private static void StoreSystemFont(string fontFile, Typeface typeface, Dictionary<string, SystemFontFamily> fontFileMap, bool useGdiFontNames)
    {
      string serializeFontFamilyName = FontEmbedder.GetSerializeFontFamilyName(typeface, useGdiFontNames);
      if (string.IsNullOrEmpty(serializeFontFamilyName))
        return;
      SystemFontFamily systemFontFamily = (SystemFontFamily) null;
      if (!fontFileMap.TryGetValue(serializeFontFamilyName, out systemFontFamily))
      {
        systemFontFamily = new SystemFontFamily(new FontFamily(serializeFontFamilyName));
        fontFileMap[serializeFontFamilyName] = systemFontFamily;
      }
      if (systemFontFamily.FontSources.Contains(fontFile))
        return;
      systemFontFamily.FontSources.Add(fontFile);
    }

    private static IEnumerable<SystemFontFamily> GetWpfFontFamilies()
    {
      if (FontEmbedder.wpfFonts == null)
        FontEmbedder.CreateSystemFontFamiliesCache();
      return FontEmbedder.wpfFonts;
    }

    private static IEnumerable<SystemFontFamily> GetGdiFontFamilies()
    {
      if (FontEmbedder.gdiFonts == null)
        FontEmbedder.CreateSystemFontFamiliesCache();
      return FontEmbedder.gdiFonts;
    }

    public static IEnumerable<SystemFontFamily> GetSystemFonts(ITypeResolver typeResolver)
    {
      if (typeResolver.IsCapabilitySet(PlatformCapability.UsesGdiFontNames))
        return FontEmbedder.GetGdiFontFamilies();
      return FontEmbedder.GetWpfFontFamilies();
    }

    public static bool DoesFontFileSupportSubsetting(string filePath)
    {
      return !string.Equals(Path.GetExtension(filePath), ".otf", StringComparison.OrdinalIgnoreCase);
    }

    public ProjectFont EmbedSystemFont(SystemFontFamily systemFontFamily)
    {
      string fontsDirectory = Path.Combine(this.viewModel.DesignerContext.ActiveProject.ProjectRoot.Path, "Fonts");
      if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(fontsDirectory))
        Directory.CreateDirectory(fontsDirectory);
      foreach (IProjectItem projectItem in this.viewModel.DesignerContext.ActiveProject.AddItems(Enumerable.Select<string, DocumentCreationInfo>((IEnumerable<string>) systemFontFamily.FontSources, (Func<string, DocumentCreationInfo>) (fontSource => new DocumentCreationInfo()
      {
        TargetFolder = fontsDirectory,
        SourcePath = fontSource,
        CreationOptions = CreationOptions.DoNotSelectCreatedItems
      }))))
      {
        IMSBuildItem msBuildItem = projectItem as IMSBuildItem;
        if (msBuildItem != null)
        {
          projectItem.Properties["BuildAction"] = "BlendEmbeddedFont";
          msBuildItem.SetMetadata("IsSystemFont", "True");
          msBuildItem.SetMetadata("All", "True");
          msBuildItem.SetMetadata("AutoFill", "True");
        }
      }
      this.EnsureFontSubsettingTask();
      foreach (ProjectFont projectFont in (Collection<IProjectFont>) this.viewModel.ProjectContext.ProjectFonts)
      {
        if (projectFont.FontFamilyName == FontEmbedder.GetFontNameFromSource(systemFontFamily.FontFamily))
        {
          this.ChangeFontReferenceToEmbeddedFont((IProjectFont) projectFont);
          return projectFont;
        }
      }
      return (ProjectFont) null;
    }

    public void EmbedProjectFont(ProjectFont projectFont)
    {
      foreach (DocumentReference documentReference in projectFont.FontDocuments)
      {
        IProjectItem projectItem = this.viewModel.DesignerContext.ActiveProject.FindItem(documentReference);
        IMSBuildItem msBuildItem = projectItem as IMSBuildItem;
        if (projectItem != null && msBuildItem != null)
        {
          projectItem.Properties["BuildAction"] = "BlendEmbeddedFont";
          if (string.IsNullOrEmpty(msBuildItem.GetMetadata("All")))
          {
            msBuildItem.SetMetadata("All", "True");
            msBuildItem.SetMetadata("AutoFill", "True");
          }
        }
      }
      this.ChangeFontReferenceToZippedFont((IProjectFont) projectFont);
      this.EnsureFontSubsettingTask();
    }

    public bool UnembedProjectFont(IProjectFont projectFont)
    {
      List<IProjectItem> list = new List<IProjectItem>();
      foreach (DocumentReference documentReference in ((ProjectFont) projectFont).FontDocuments)
      {
        IProjectItem projectItem = this.viewModel.DesignerContext.ActiveProject.FindItem(documentReference);
        if (projectItem != null)
        {
          switch (((IMSBuildItem) projectItem).GetMetadata("IsSystemFont"))
          {
            case "True":
              list.Add(projectItem);
              continue;
            default:
              projectItem.Properties["BuildAction"] = "Resource";
              continue;
          }
        }
      }
      if (list.Count > 0)
        this.ChangeFontReferenceToUnembeddedFont(projectFont);
      else
        this.ChangeFontReferenceToUnzippedFont(projectFont);
      foreach (IProjectItem projectItem in list)
        this.viewModel.DesignerContext.ActiveProject.RemoveItems(1 != 0, projectItem);
      return list.Count > 0;
    }

    public static void CreateFontFamilyChange(FontChangeType fontChange, IProjectFont projectFont, IDocumentContext documentContext, out FontFamily oldFontFamily, out FontFamily newFontFamily)
    {
      switch (fontChange)
      {
        case FontChangeType.SystemToEmbedded:
          newFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, true);
          oldFontFamily = FontEmbedder.MakeSystemFont(newFontFamily);
          break;
        case FontChangeType.EmbeddedToSystem:
          oldFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, true);
          newFontFamily = FontEmbedder.MakeSystemFont(oldFontFamily);
          break;
        case FontChangeType.ProjectToEmbedded:
          oldFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, false);
          newFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, true);
          break;
        case FontChangeType.EmbeddedToProject:
          oldFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, true);
          newFontFamily = FontEmbedder.MakeRelativeFontFamily(projectFont, documentContext, false);
          break;
        default:
          oldFontFamily = newFontFamily = (FontFamily) null;
          break;
      }
    }

    private void ChangeFontReferenceToEmbeddedFont(IProjectFont projectFont)
    {
      this.ChangeFontReference(FontChangeType.SystemToEmbedded, projectFont);
    }

    private void ChangeFontReferenceToZippedFont(IProjectFont projectFont)
    {
      this.ChangeFontReference(FontChangeType.ProjectToEmbedded, projectFont);
      this.ChangeFontReference(FontChangeType.SystemToEmbedded, projectFont);
    }

    private void ChangeFontReferenceToUnzippedFont(IProjectFont projectFont)
    {
      this.ChangeFontReference(FontChangeType.EmbeddedToProject, projectFont);
    }

    private void ChangeFontReferenceToUnembeddedFont(IProjectFont projectFont)
    {
      this.ChangeFontReference(FontChangeType.EmbeddedToSystem, projectFont);
    }

    private void ChangeFontReference(FontChangeType fontChange, IProjectFont projectFont)
    {
      FontFamily oldFontFamily;
      FontFamily newFontFamily;
      FontEmbedder.CreateFontFamilyChange(fontChange, projectFont, this.viewModel.Document.DocumentContext, out oldFontFamily, out newFontFamily);
      if (!(oldFontFamily.Source != newFontFamily.Source))
        return;
      SceneElement sceneElement = this.viewModel.RootNode as SceneElement;
      if (sceneElement == null)
        return;
      FontFamilyRepairProcessor familyRepairProcessor = new FontFamilyRepairProcessor(this.viewModel.DesignerContext, new FontFamilyChangeModel(oldFontFamily.Source, newFontFamily.Source, fontChange, projectFont, sceneElement.DocumentNode.DocumentRoot, this.viewModel.ProjectContext));
      using (SceneEditTransaction editTransaction = this.viewModel.CreateEditTransaction(StringTable.EmbedFontUndoUnit))
      {
        familyRepairProcessor.Begin();
        if (familyRepairProcessor.Cancelled)
          editTransaction.Cancel();
        else
          editTransaction.Commit();
      }
    }
  }
}

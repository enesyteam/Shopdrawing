// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.UriHelperUtility
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.Utility.IO
{
  public static class UriHelperUtility
  {
    private static Regex SdkUriRegex = new Regex("^(?:ms-appx:)?(///?)([^/]+)/?(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex AppXUriRegex = new Regex("^(?:ms-appx:)?(//?/?)([^/]+)/?(.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly int SdkUriRegexSlashGroup = 1;
    public static readonly int SdkUriRegexPackageNameGroup = 2;
    public static readonly int SdkUriRegexFilePathGroup = 3;
    public static readonly string CorsicaName = "Microsoft.WinJS";

    public static bool TryCombineUris(Uri rootUri, Uri relativeUri, out Uri combinedUri)
    {
      combinedUri = (Uri) null;
      if (rootUri == (Uri) null && relativeUri == (Uri) null)
        return false;
      if (relativeUri == (Uri) null)
      {
        combinedUri = rootUri;
        return true;
      }
      if (relativeUri.IsAbsoluteUri || UriHelperUtility.IsSdkUri(relativeUri))
        return false;
      if (rootUri == (Uri) null)
      {
        combinedUri = relativeUri;
        return true;
      }
      if (UriHelperUtility.IsSdkUri(rootUri))
        return UriHelperUtility.TryCombineSdkUris(rootUri, relativeUri, out combinedUri);
      rootUri = UriHelperUtility.EnsureUriEndsWithDirectorySeperator(rootUri);
      string originalString = relativeUri.OriginalString;
      if (Path.IsPathRooted(originalString))
      {
        string pathRoot = Path.GetPathRoot(originalString);
        relativeUri = new Uri(originalString.Substring(pathRoot.Length), UriKind.Relative);
      }
      if (rootUri.IsAbsoluteUri)
        return Uri.TryCreate(rootUri, relativeUri, out combinedUri);
      string fromPath = "C:\\";
      int length = rootUri.OriginalString.Split(new string[1]
      {
        ".."
      }, StringSplitOptions.None).Length;
      while (length-- > 0)
        fromPath = fromPath + (object) "dummy" + (string) (object) Path.DirectorySeparatorChar;
      rootUri = new Uri(fromPath + rootUri.ToString(), UriKind.Absolute);
      if (!UriHelperUtility.TryCombineUris(rootUri, relativeUri, out combinedUri))
      {
        combinedUri = (Uri) null;
        return false;
      }
      string relativePath = PathHelper.GetRelativePath(fromPath, combinedUri.LocalPath);
      if (string.IsNullOrEmpty(relativePath))
      {
        combinedUri = (Uri) null;
        return false;
      }
      combinedUri = new Uri(relativePath, UriKind.Relative);
      return true;
    }

    private static bool TryCombineSdkUris(Uri rootUri, Uri relativeUri, out Uri combinedUri)
    {
      rootUri = UriHelperUtility.EnsureUriEndsWithDirectorySeperator(rootUri);
      if (rootUri.IsAbsoluteUri)
        return Uri.TryCreate(rootUri, relativeUri, out combinedUri);
      rootUri = UriHelperUtility.EnsureUriEndsWithDirectorySeperator(rootUri);
      string originalString = relativeUri.OriginalString;
      if (Path.IsPathRooted(originalString))
      {
        string pathRoot = Path.GetPathRoot(originalString);
        relativeUri = new Uri(originalString.Substring(pathRoot.Length), UriKind.Relative);
      }
      return Uri.TryCreate((rootUri.ToString() + relativeUri.ToString()).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), UriKind.Relative, out combinedUri);
    }

    public static bool IsCombinedUriWithin(Uri first, Uri second)
    {
      if (first == (Uri) null || second == (Uri) null)
        return false;
      if (!second.IsAbsoluteUri)
      {
        Uri combinedUri = (Uri) null;
        if (!UriHelperUtility.TryCombineUris(first, second, out combinedUri))
          return false;
        second = combinedUri;
      }
      return UriHelperUtility.IsUriWithin(second, first);
    }

    public static bool IsUriWithin(Uri first, Uri second)
    {
      if (first == (Uri) null || second == (Uri) null)
        return false;
      if (second.IsAbsoluteUri)
      {
        if (first.IsAbsoluteUri)
          return first.AbsoluteUri.StartsWith(second.AbsoluteUri, StringComparison.OrdinalIgnoreCase);
        return false;
      }
      if (!UriHelperUtility.IsSdkUri(first))
        return PathHelper.IsPathWithin(first.ToString(), second.ToString());
      if ((int) second.ToString()[second.ToString().Length - 1] != (int) Path.AltDirectorySeparatorChar)
        second = new Uri(second.ToString() + (object) Path.AltDirectorySeparatorChar, UriKind.Relative);
      return first.ToString().StartsWith(second.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public static bool CheckUriEquality(Uri thisUri, Uri otherUri)
    {
      if (thisUri == (Uri) null && otherUri == (Uri) null)
        return true;
      if (thisUri == (Uri) null ^ otherUri == (Uri) null)
        return false;
      if (object.Equals((object) thisUri, (object) otherUri))
        return true;
      if (UriHelperUtility.IsSdkUri(thisUri) && UriHelperUtility.IsSdkUri(otherUri))
        return UriHelperUtility.CheckSdkUriEquality(thisUri, otherUri);
      if (thisUri.IsAbsoluteUri && otherUri.IsAbsoluteUri)
        return Uri.Compare(thisUri, otherUri, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0;
      if (!thisUri.IsAbsoluteUri && !otherUri.IsAbsoluteUri)
        return string.Compare(thisUri.ToString(), otherUri.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
      return false;
    }

    private static Uri EnsureUriEndsWithDirectorySeperator(Uri uri)
    {
      if (uri.IsAbsoluteUri)
      {
        if ((int) uri.AbsoluteUri[uri.AbsoluteUri.Length - 1] == (int) Path.AltDirectorySeparatorChar)
          return uri;
        return new Uri(uri.AbsoluteUri + (object) Path.AltDirectorySeparatorChar, UriKind.Absolute);
      }
      Uri uri1 = new Uri(uri.ToString().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), UriKind.Relative);
      string str = uri1.ToString();
      if ((int) str[str.Length - 1] == (int) Path.DirectorySeparatorChar)
        return uri1;
      return new Uri(str + (object) Path.DirectorySeparatorChar, UriKind.Relative);
    }

    public static bool IsSdkUri(Uri uri)
    {
      if (uri != (Uri) null)
        return UriHelperUtility.SdkUriRegex.IsMatch(uri.ToString());
      return false;
    }

    public static bool IsAppXUri(Uri uri)
    {
      if (uri != (Uri) null)
        return UriHelperUtility.AppXUriRegex.IsMatch(uri.ToString());
      return false;
    }

    public static Match MatchSdkUri(Uri uri)
    {
      if (!(uri != (Uri) null))
        return (Match) null;
      return UriHelperUtility.SdkUriRegex.Match(uri.ToString());
    }

    public static Match MatchAppXUri(Uri uri)
    {
      if (!(uri != (Uri) null))
        return (Match) null;
      return UriHelperUtility.AppXUriRegex.Match(uri.ToString());
    }

    private static bool CheckSdkUriEquality(Uri sdkUri1, Uri sdkUri2)
    {
      Match match1 = UriHelperUtility.MatchSdkUri(sdkUri1);
      Match match2 = UriHelperUtility.MatchSdkUri(sdkUri2);
      string str1 = match1.Groups[UriHelperUtility.SdkUriRegexSlashGroup].Value;
      string str2 = match2.Groups[UriHelperUtility.SdkUriRegexSlashGroup].Value;
      if (str1 == "///" && str2 == "//" || str1 == "//" && str2 == "///")
        return false;
      string a1 = match1.Groups[UriHelperUtility.SdkUriRegexPackageNameGroup].Value;
      string b1 = match2.Groups[UriHelperUtility.SdkUriRegexPackageNameGroup].Value;
      string a2 = match1.Groups[UriHelperUtility.SdkUriRegexFilePathGroup].Value;
      string b2 = match2.Groups[UriHelperUtility.SdkUriRegexFilePathGroup].Value;
      if (string.Equals(a1, b1, StringComparison.OrdinalIgnoreCase))
        return string.Equals(a2, b2, StringComparison.OrdinalIgnoreCase);
      return false;
    }

    public static bool TryCreateRelative(string input, out Uri result)
    {
      if (input == null)
      {
        result = (Uri) null;
        return false;
      }
      if (!input.StartsWith("//", StringComparison.Ordinal))
        return Uri.TryCreate(input, UriKind.Relative, out result);
      result = (Uri) null;
      return false;
    }

    public static string GetAbsolutePathIfUnderBaseUri(Uri baseUri, Uri absoluteOrRelativeUri)
    {
      if (!absoluteOrRelativeUri.IsAbsoluteUri || !baseUri.IsAbsoluteUri || !baseUri.IsBaseOf(absoluteOrRelativeUri))
        return absoluteOrRelativeUri.ToString();
      return absoluteOrRelativeUri.AbsolutePath;
    }

    public static string GetAbsolutePathIfUnderBaseUri(Uri baseUri, string absoluteOrRelative, bool trimTrailingSlash)
    {
      Uri result;
      if (!Uri.TryCreate(absoluteOrRelative, UriKind.RelativeOrAbsolute, out result))
        return absoluteOrRelative;
      string str = UriHelperUtility.GetAbsolutePathIfUnderBaseUri(baseUri, result);
      if (trimTrailingSlash && !absoluteOrRelative.EndsWith("/", StringComparison.Ordinal) && str.EndsWith("/", StringComparison.Ordinal))
        str = str.TrimEnd('/');
      return str;
    }
  }
}

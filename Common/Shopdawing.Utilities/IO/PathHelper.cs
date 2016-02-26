// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.PathHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility.Collections;
using Microsoft.Expression.Utility.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Expression.Utility.IO
{
  public static class PathHelper
  {
    private static readonly char[] invalidFileNameCharacters = Path.GetInvalidFileNameChars();
    private static readonly char[] invalidPathCharacters = Enumerable.ToArray<char>(Enumerable.Concat<char>((IEnumerable<char>) Path.GetInvalidPathChars(), (IEnumerable<char>) new char[3]
    {
      '*',
      '?',
      ':'
    }));
    private static readonly char[] wildcardCharacters = new char[2]
    {
      '*',
      '?'
    };
    private static readonly char[] directorySeparatorCharacters = new char[2]
    {
      Path.DirectorySeparatorChar,
      Path.AltDirectorySeparatorChar
    };
    private static readonly string[] deviceNames = new string[22]
    {
      "CON",
      "PRN",
      "AUX",
      "NUL",
      "COM1",
      "COM2",
      "COM3",
      "COM4",
      "COM5",
      "COM6",
      "COM7",
      "COM8",
      "COM9",
      "LPT1",
      "LPT2",
      "LPT3",
      "LPT4",
      "LPT5",
      "LPT6",
      "LPT7",
      "LPT8",
      "LPT9"
    };
    private static readonly char[] trailingWhitespaceCharacters = new char[2]
    {
      ' ',
      '.'
    };
    private static IndexedHashSet<string> resolvedPathsCache = IndexedHashSet<string>.Create(1024, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public const int MaximumPathLength = 260;
    private const int MaxResolvedPaths = 10000;

    public static bool IsShortFileNameGenerationEnabled
    {
      get
      {
        return object.Equals(AccessHelper.AccessService.RegistryGetValue("HKEY_LOCAL_MACHINE\\\\SYSTEM\\CurrentControlSet\\Control\\FileSystem", "NtfsDisable8dot3NameCreation", (object) false), (object) false);
      }
    }

    public static string LocalLowFolder
    {
      get
      {
        return NativeMethods.GetKnownFolderPath(NativeMethods.FOLDERID_LocalAppDataLow);
      }
    }

    public static bool ContainsWildcard(string path)
    {
      return path.IndexOfAny(PathHelper.wildcardCharacters) != -1;
    }

    public static IEnumerable<string> ResolveWildcard(string path)
    {
      if (PathHelper.ContainsWildcard(path))
      {
        string filename = PathHelper.GetFileOrDirectoryName(path);
        string directory = path.Substring(0, path.Length - filename.Length);
        if (PathHelper.DirectoryExists(directory))
        {
          foreach (string str in PathHelper.EnumerateFiles(directory, filename))
            yield return str;
        }
      }
      else
        yield return path;
    }

    public static char[] GetDirectorySeparatorCharacters()
    {
      return (char[]) PathHelper.directorySeparatorCharacters.Clone();
    }

    public static string GetShortPathName(string path)
    {
      return AccessHelper.AccessService.MiscGetShortPathName(path);
    }

    public static void CleanDirectory(string directoryName, bool deleteTopDirectoryOnError)
    {
      if (!PathHelper.DirectoryExists(directoryName))
        return;
      foreach (string path in AccessHelper.AccessService.DirectoryGetFiles(directoryName))
      {
        try
        {
          AccessHelper.AccessService.FileSetAttributes(path, FileAttributes.Normal);
          AccessHelper.AccessService.FileDelete(path);
        }
        catch (IOException ex)
        {
          deleteTopDirectoryOnError = false;
          break;
        }
      }
      foreach (string directoryName1 in AccessHelper.AccessService.DirectoryGetDirectories(directoryName))
        PathHelper.CleanDirectory(directoryName1, deleteTopDirectoryOnError);
      if (!deleteTopDirectoryOnError)
        return;
      AccessHelper.AccessService.DirectoryDelete(directoryName, true);
    }

    public static string GetFullPathName(string path)
    {
      return AccessHelper.AccessService.MiscGetFullPathName(path);
    }

    public static bool IsDirectory(string path)
    {
      if (PathHelper.PathEndsInDirectorySeparator(path))
        return true;
      return AccessHelper.AccessService.MiscDirectoryExists(path);
    }

    public static bool IsDirectory(DocumentReference documentReference)
    {
      return PathHelper.IsDirectory(documentReference.Path);
    }

    public static bool IsHidden(string path)
    {
      return AccessHelper.AccessService.MiscPathHidden(path);
    }

    public static bool IsHidden(DocumentReference documentReference)
    {
      return AccessHelper.AccessService.MiscPathHidden(documentReference.Path);
    }

    public static bool IsPathRelative(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (path.Length < 2)
        return true;
      if ((int) path[0] == 92 || (int) path[0] == 47)
      {
        if ((int) path[1] != 92)
          return (int) path[1] != 47;
        return false;
      }
      if (path.Length < 3 || (int) path[1] != 58)
        return true;
      if ((int) path[2] != 92)
        return (int) path[2] != 47;
      return false;
    }

    public static bool ResolvePath(ref string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (path.Length > 4 && (int) path[4] == 58)
        return false;
      if (PathHelper.resolvedPathsCache.Contains(path))
        return true;
      if (PathHelper.resolvedPathsCache.Count > 10000)
        PathHelper.resolvedPathsCache.Clear();
      try
      {
        string fullPath = PathHelper.TryGetFullPath(path);
        if (fullPath == null)
          return false;
        path = fullPath;
      }
      catch (PathTooLongException ex1)
      {
        string fullPathName = PathHelper.GetFullPathName(path);
        if (string.IsNullOrEmpty(fullPathName))
          return false;
        try
        {
          string fullPath = PathHelper.TryGetFullPath(fullPathName);
          if (fullPath == null)
            return false;
          path = fullPath;
        }
        catch (PathTooLongException ex2)
        {
          return false;
        }
      }
      PathHelper.resolvedPathsCache.Add(path);
      return true;
    }

    private static string TryGetFullPath(string path)
    {
      try
      {
        if (string.IsNullOrEmpty(path))
          return (string) null;
        path = Path.GetFullPath(path);
        try
        {
          if (PathHelper.GetPathFormat(path) == PathFormat.DriveAbsolute)
          {
            if (PathHelper.IsDirectory(path))
              path = PathHelper.EnsurePathEndsInDirectorySeparator(path);
          }
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
      catch (ArgumentException ex)
      {
        return (string) null;
      }
      catch (NotSupportedException ex)
      {
        return (string) null;
      }
      return path;
    }

    [CLSCompliant(false)]
    public static string ResolvePath(string path)
    {
      string path1 = path;
      PathHelper.ResolvePath(ref path1);
      return path1;
    }

    public static string ResolveRelativePath(string rootFolder, string relativePath)
    {
      if (rootFolder == null)
        throw new ArgumentNullException("rootFolder");
      if (relativePath == null)
        throw new ArgumentNullException("relativePath");
      if (PathHelper.IsPathRelative(rootFolder))
        throw new ArgumentException("rootFolder must not be relative", "rootFolder");
      if (relativePath.Length == 0)
        return PathHelper.ResolvePath(rootFolder);
      if (!PathHelper.IsValidPath(relativePath))
        throw new ArgumentException("relativePath must be a valid path", "relativePath");
      switch (PathHelper.GetPathFormat(relativePath))
      {
        case PathFormat.DriveAbsolute:
        case PathFormat.DriveRelative:
        case PathFormat.UniformNamingConvention:
          return PathHelper.ResolvePath(relativePath);
        case PathFormat.DriveAbsoluteExtended:
        case PathFormat.UniformNamingConventionExtended:
          throw new NotSupportedException("Extended file formats are not currently supported.");
        case PathFormat.Rooted:
          return PathHelper.ResolvePath(PathHelper.EnsurePathEndsInDirectorySeparator(Path.GetPathRoot(rootFolder)) + relativePath);
        default:
          return PathHelper.ResolvePath(PathHelper.EnsurePathEndsInDirectorySeparator(rootFolder) + relativePath);
      }
    }

    public static string ResolveCombinedPath(string rootFolder, string relativePath)
    {
      if (rootFolder == null)
        throw new ArgumentNullException("rootFolder");
      if (relativePath == null)
        throw new ArgumentNullException("relativePath");
      if (PathHelper.IsPathRelative(rootFolder))
        throw new ArgumentException("rootFolder must not be relative", "rootFolder");
      if (relativePath.Length == 0)
        return PathHelper.ResolvePath(rootFolder);
      if (!PathHelper.IsValidPath(relativePath))
        throw new ArgumentException("relativePath must be a valid path", "relativePath");
      switch (PathHelper.GetPathFormat(relativePath))
      {
        case PathFormat.DriveAbsolute:
        case PathFormat.UniformNamingConvention:
          return PathHelper.ResolvePath(relativePath);
        case PathFormat.DriveAbsoluteExtended:
        case PathFormat.UniformNamingConventionExtended:
          throw new NotSupportedException("Extended file formats are not currently supported.");
        case PathFormat.DriveRelative:
          if (relativePath.Length == 2)
            return rootFolder;
          return PathHelper.ResolveRelativePath(rootFolder, relativePath.Substring(2));
        default:
          return PathHelper.ResolveRelativePath(rootFolder, relativePath.TrimStart(PathHelper.directorySeparatorCharacters));
      }
    }

    public static bool IsValidPath(string path)
    {
      return PathHelper.ValidatePathInternal(path, false) == PathHelper.PathValidity.ValidPath;
    }

    public static bool ValidateAndFixPathIfPossible(ref string path)
    {
      switch (PathHelper.ValidatePathInternal(path, false))
      {
        case PathHelper.PathValidity.ValidPath:
          return true;
        case PathHelper.PathValidity.TrailingSpaceOrPeriod:
          path = PathHelper.TrimTrailingPeriodsAndSpaces(path);
          return true;
        default:
          return false;
      }
    }

    public static bool IsValidPath(string path, bool allowExtendedSyntax)
    {
      return PathHelper.ValidatePathInternal(path, allowExtendedSyntax) == PathHelper.PathValidity.ValidPath;
    }

    public static string ValidatePath(string path)
    {
      switch (PathHelper.ValidatePathInternal(path, false))
      {
        case PathHelper.PathValidity.ValidPath:
          return (string) null;
        case PathHelper.PathValidity.EmptyPath:
          return StringTable.InvalidPathEmpty;
        case PathHelper.PathValidity.InvalidServerShare:
          return StringTable.InvalidPathBadUncFormat;
        case PathHelper.PathValidity.TrailingSpaceOrPeriod:
          return StringTable.InvalidPathEndingPeriodOrSpace;
        case PathHelper.PathValidity.AllDots:
          return StringTable.InvalidPathAllDots;
        case PathHelper.PathValidity.InvalidPathCharacter:
          return StringTable.InvalidPathCharacters;
        case PathHelper.PathValidity.DeviceNamePresent:
          return StringTable.InvalidPathDeviceName;
        default:
          return StringTable.InvalidPathUnknownFormat;
      }
    }

    private static PathHelper.PathValidity ValidatePathInternal(string path, bool allowExtendedSyntax)
    {
      if (string.IsNullOrEmpty(path))
        return PathHelper.PathValidity.EmptyPath;
      PathFormat pathFormat = PathHelper.GetPathFormat(path);
      if (pathFormat == PathFormat.UnknownFormat)
        return PathHelper.PathValidity.UnknownFormat;
      int length = path.Length;
      int startIndex = 0;
      bool flag1 = false;
      bool flag2 = false;
      switch (pathFormat - 1)
      {
        case PathFormat.UnknownFormat:
        case PathFormat.DriveAbsoluteExtended:
          startIndex = 2;
          break;
        case PathFormat.DriveAbsolute:
          startIndex = 7;
          flag2 = true;
          break;
        case PathFormat.Rooted:
          startIndex = 2;
          flag1 = true;
          break;
        case PathFormat.UniformNamingConvention:
          startIndex = 8;
          flag1 = true;
          flag2 = true;
          break;
      }
      if (flag2 && !allowExtendedSyntax)
        return PathHelper.PathValidity.UnknownFormat;
      if (startIndex == length)
        return flag1 ? PathHelper.PathValidity.InvalidServerShare : PathHelper.PathValidity.ValidPath;
      if (path.IndexOfAny(PathHelper.invalidPathCharacters, startIndex) != -1)
        return PathHelper.PathValidity.InvalidPathCharacter;
      bool flag3 = true;
      int num1 = 0;
      int num2 = 0;
      bool flag4 = true;
      char ch1 = 'A';
      do
      {
        if (flag4)
        {
          if (PathHelper.IsDeviceNameInternal(path, startIndex))
            return PathHelper.PathValidity.DeviceNamePresent;
          flag4 = false;
          flag3 = true;
          num1 = 0;
        }
        char ch2 = path[startIndex];
        switch (ch2)
        {
          case ';':
          case '=':
          case ' ':
          case '+':
          case ',':
            if (flag1 && num2 < 1)
              return PathHelper.PathValidity.InvalidServerShare;
            goto default;
          case '\\':
            flag4 = true;
            break;
          case '.':
            ++num1;
            break;
          case '/':
            if (flag2)
              return PathHelper.PathValidity.InvalidExtendedPathCharacter;
            ch2 = '\\';
            goto case '\\';
          default:
            flag3 = false;
            break;
        }
        ++startIndex;
        if (startIndex == length && !flag4)
        {
          ch1 = ch2;
          flag4 = true;
        }
        if (flag4)
        {
          ++num2;
          if (flag1 && num2 < 3 && (flag3 || (int) ch2 == 92 && (int) ch1 == 92))
            return PathHelper.PathValidity.InvalidServerShare;
          if ((int) ch1 == 32)
            return PathHelper.PathValidity.TrailingSpaceOrPeriod;
          if (flag3)
          {
            if (num1 > 2)
              return PathHelper.PathValidity.AllDots;
            if (num1 == 1 && flag2)
              return PathHelper.PathValidity.SingleDotInExtendedPath;
          }
          else if ((int) ch1 == 46)
            return PathHelper.PathValidity.TrailingSpaceOrPeriod;
        }
        ch1 = ch2;
      }
      while (startIndex < length);
      return flag1 && num2 < 2 ? PathHelper.PathValidity.InvalidServerShare : PathHelper.PathValidity.ValidPath;
    }

    private static bool IsDeviceNameInternal(string path, int startIndex)
    {
      if (startIndex + 2 >= path.Length)
        return false;
      foreach (string strB in PathHelper.deviceNames)
      {
        if (string.Compare(path, startIndex, strB, 0, strB.Length, StringComparison.OrdinalIgnoreCase) == 0)
        {
          for (int index = startIndex + strB.Length; index < path.Length; ++index)
          {
            switch (path[index])
            {
              case '\\':
              case '/':
              case '.':
                return true;
              case ' ':
                goto case ' ';
              default:
                return false;
            }
          }
          return true;
        }
      }
      return false;
    }

    public static bool IsDeviceName(string name)
    {
      if (name == null || name.Length < 3)
        return false;
      return PathHelper.IsDeviceNameInternal(name, 0);
    }

    public static bool IsValidDrive(char drive)
    {
      drive = char.ToUpperInvariant(drive);
      bool flag = false;
      foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
      {
        if ((int) char.ToUpperInvariant(driveInfo.Name[0]) == (int) drive)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static bool IsValidFileOrDirectoryName(string name)
    {
      return PathHelper.ValidateFileOrDirectoryNameInternal(name) == PathHelper.FileNameValidity.ValidFileName;
    }

    public static string BuildValidFileName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return (string) null;
      if (PathHelper.IsValidFileOrDirectoryName(name))
        return name;
      string str = new string(Enumerable.ToArray<char>(Enumerable.Where<char>((IEnumerable<char>) name, new Func<char, bool>(((Enumerable) Enumerable.Except<char>((IEnumerable<char>) name.ToCharArray(), (IEnumerable<char>) PathHelper.invalidFileNameCharacters)).Contains<char>))));
      string extension = PathHelper.GetExtension(str);
      if (string.IsNullOrEmpty(str) || str.LastIndexOf(extension, StringComparison.CurrentCultureIgnoreCase) == 0 || !PathHelper.IsValidFileOrDirectoryName(str))
        return (string) null;
      return str;
    }

    private static int FindExtensionOffset(string pathOrFileName)
    {
      if (string.IsNullOrEmpty(pathOrFileName))
        return -1;
      int length = pathOrFileName.Length;
      if (length == 1 || (int) pathOrFileName[length - 1] == 46)
        return -1;
      int index = length;
      while (--index >= 0)
      {
        char ch = pathOrFileName[index];
        if ((int) ch == 46)
          return index;
        if ((int) ch == (int) Path.DirectorySeparatorChar || (int) ch == (int) Path.AltDirectorySeparatorChar || (int) ch == (int) Path.VolumeSeparatorChar)
          return -1;
      }
      return -1;
    }

    public static string TrimFileOrDirectoryName(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      int num = path.LastIndexOfAny(PathHelper.directorySeparatorCharacters);
      if (num == -1)
        return path;
      return path.Substring(0, num + 1);
    }

    public static string TrimExtension(string pathOrFileName)
    {
      int extensionOffset = PathHelper.FindExtensionOffset(pathOrFileName);
      if (extensionOffset == -1)
        return pathOrFileName;
      return pathOrFileName.Substring(0, extensionOffset);
    }

    public static string GetExtension(string pathOrFileName)
    {
      int extensionOffset = PathHelper.FindExtensionOffset(pathOrFileName);
      if (extensionOffset == -1)
        return string.Empty;
      return pathOrFileName.Substring(extensionOffset);
    }

    public static string ValidateFileOrDirectoryName(string name)
    {
      switch (PathHelper.ValidateFileOrDirectoryNameInternal(name))
      {
        case PathHelper.FileNameValidity.ValidFileName:
          return (string) null;
        case PathHelper.FileNameValidity.EmptyName:
          return StringTable.InvalidNameEmpty;
        case PathHelper.FileNameValidity.TrailingSpaceOrPeriod:
          return StringTable.InvalidNameEndingPeriodOrSpace;
        case PathHelper.FileNameValidity.DeviceNamePresent:
          return StringTable.InvalidNameDeviceName;
        case PathHelper.FileNameValidity.InvalidCharacters:
          return StringTable.InvalidNameCharacters;
        default:
          return (string) null;
      }
    }

    private static PathHelper.FileNameValidity ValidateFileOrDirectoryNameInternal(string name)
    {
      if (string.IsNullOrEmpty(name))
        return PathHelper.FileNameValidity.EmptyName;
      if (name.IndexOfAny(PathHelper.invalidFileNameCharacters) != -1)
        return PathHelper.FileNameValidity.InvalidCharacters;
      if (PathHelper.IsDeviceName(name))
        return PathHelper.FileNameValidity.DeviceNamePresent;
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          return PathHelper.FileNameValidity.TrailingSpaceOrPeriod;
        default:
          return PathHelper.FileNameValidity.ValidFileName;
      }
    }

    public static PathFormat GetPathFormat(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (path.Length == 0)
        return PathFormat.UnknownFormat;
      if ((int) path[0] != 92 && (int) path[0] != 47)
      {
        if (path.Length < 2 || (int) path[1] != 58)
          return PathFormat.Relative;
        char ch = char.ToUpperInvariant(path[0]);
        if ((int) ch < 65 || (int) ch > 90)
          return PathFormat.UnknownFormat;
        return path.Length < 3 || (int) path[2] != 92 && (int) path[2] != 47 ? PathFormat.DriveRelative : PathFormat.DriveAbsolute;
      }
      if (path.Length < 2 || (int) path[1] != 92 && (int) path[1] != 47)
        return PathFormat.Rooted;
      if (path.Length < 3)
        return PathFormat.UnknownFormat;
      if ((int) path[2] != 63 && ((int) path[2] != 46 || path.Length != 3 && (int) path[3] != 92))
        return PathFormat.UniformNamingConvention;
      if (path.StartsWith("\\\\?\\UNC\\", StringComparison.Ordinal))
        return PathFormat.UniformNamingConventionExtended;
      if (path.StartsWith("\\\\?\\", StringComparison.Ordinal) && path.Length >= 7 && ((int) path[5] == 58 && (int) path[6] == 92))
      {
        char ch = char.ToUpperInvariant(path[4]);
        if ((int) ch >= 65 && (int) ch <= 90)
          return PathFormat.DriveAbsoluteExtended;
      }
      return PathFormat.UnknownFormat;
    }

    public static string NormalizePath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return path;
      return PathHelper.NormalizePathInternal(PathHelper.ResolvePath(path));
    }

    public static string NormalizePathInternal(string path)
    {
      return PathHelper.TrimTrailingDirectorySeparators(path).ToUpperInvariant();
    }

    private static int InternalGenerateHashFromPath(string path)
    {
      return PathHelper.NormalizePathInternal(path).GetHashCode();
    }

    public static int GenerateHashFromPath(DocumentReference reference)
    {
      if (reference == (DocumentReference) null)
        throw new ArgumentNullException("reference");
      return PathHelper.InternalGenerateHashFromPath(reference.Path);
    }

    public static int GenerateHashFromPath(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return PathHelper.InternalGenerateHashFromPath(PathHelper.ResolvePath(path));
    }

    public static bool ArePathsEquivalent(string firstPath, string secondPath)
    {
      if (firstPath == secondPath)
        return true;
      if (firstPath == null || secondPath == null)
        return false;
      return string.Equals(PathHelper.NormalizePath(firstPath), PathHelper.NormalizePath(secondPath), StringComparison.Ordinal);
    }

    public static bool ArePathsEquivalent(DocumentReference firstPath, string secondPath)
    {
      if (firstPath == (DocumentReference) null)
        return secondPath == null;
      if (secondPath == null)
        return false;
      string b = PathHelper.NormalizePath(secondPath);
      if (firstPath.GetHashCode() != b.GetHashCode())
        return false;
      return string.Equals(PathHelper.NormalizePathInternal(firstPath.Path), b, StringComparison.Ordinal);
    }

    public static bool ArePathsEquivalent(DocumentReference firstPath, DocumentReference secondPath)
    {
      if (firstPath == (DocumentReference) null || secondPath == (DocumentReference) null)
      {
        if (secondPath == (DocumentReference) null)
          return firstPath == (DocumentReference) null;
        return false;
      }
      if (firstPath.GetHashCode() != secondPath.GetHashCode())
        return false;
      return string.Equals(PathHelper.NormalizePathInternal(firstPath.Path), PathHelper.NormalizePathInternal(secondPath.Path));
    }

    public static string GetDirectoryNameOrRoot(string fullPath)
    {
      return PathHelper.GetDirectoryNameOrRoot(DocumentReference.Create(fullPath));
    }

    public static string GetDirectoryNameOrRoot(DocumentReference documentReference)
    {
      if (documentReference == (DocumentReference) null)
        return (string) null;
      if (!documentReference.IsValidPathFormat)
        return (string) null;
      string str = documentReference.Path;
      if ((int) str[0] != (int) Path.DirectorySeparatorChar || (int) str[1] != (int) Path.DirectorySeparatorChar || !PathHelper.IsUncJustServerShare(str))
      {
        int length = str.Length;
        do
          ;
        while (length > 2 && (int) str[--length] != (int) Path.DirectorySeparatorChar && (int) str[length] != (int) Path.AltDirectorySeparatorChar);
        str = str.Substring(0, length);
      }
      return PathHelper.EnsurePathEndsInDirectorySeparator(str);
    }

    public static bool PathBeginsWithDirectorySeparator(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      return PathHelper.IsDirectorySeparator(path[0]);
    }

    public static bool PathEndsInDirectorySeparator(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      return PathHelper.IsDirectorySeparator(path[path.Length - 1]);
    }

    public static bool IsDirectorySeparator(char character)
    {
      if ((int) character == (int) Path.DirectorySeparatorChar)
        return true;
      return (int) character == (int) Path.AltDirectorySeparatorChar;
    }

    public static string EnsurePathBeginsWithDirectorySeparator(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (PathHelper.PathBeginsWithDirectorySeparator(path) || Path.IsPathRooted(path))
        return path;
      return (string) (object) Path.DirectorySeparatorChar + (object) path;
    }

    public static string EnsurePathEndsInDirectorySeparator(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (PathHelper.PathEndsInDirectorySeparator(path))
        return path;
      return path + (object) Path.DirectorySeparatorChar;
    }

    public static string TrimTrailingDirectorySeparators(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.TrimEnd(PathHelper.directorySeparatorCharacters);
    }

    public static string TrimLeadingDirectorySeparators(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.TrimStart(PathHelper.directorySeparatorCharacters);
    }

    public static string TrimTrailingPeriodsAndSpaces(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.TrimEnd(PathHelper.trailingWhitespaceCharacters);
    }

    public static string ChangePathToWebFormat(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.Replace(Path.DirectorySeparatorChar, '/');
    }

    public static string ChangePathToLocalPathFormat(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.Replace('/', Path.DirectorySeparatorChar);
    }

    public static string GetAvailableFileOrDirectoryName(string desiredNameWithoutExtension, string desiredExtension, string directory, bool alwaysAppendDigit)
    {
      if (desiredNameWithoutExtension == null)
        throw new ArgumentNullException("desiredNameWithoutExtension");
      if (directory == null)
        throw new ArgumentNullException("directory");
      directory = PathHelper.EnsurePathEndsInDirectorySeparator(PathHelper.ResolvePath(directory));
      string str = string.Empty;
      if (!string.IsNullOrEmpty(desiredExtension))
        str = (int) desiredExtension[0] == 46 ? desiredExtension : "." + desiredExtension;
      string availablePaths = PathHelper.FindAvailablePaths((IEnumerable<string>) new string[1]
      {
        directory + desiredNameWithoutExtension + "{0}" + str
      }, alwaysAppendDigit, 1);
      return desiredNameWithoutExtension + availablePaths.ToString() + str;
    }

    public static string FindAvailablePaths(IEnumerable<string> pathFormatStrings, bool alwaysUseDigit, int startDigit = 1)
    {
      if (pathFormatStrings == null)
        throw new ArgumentNullException("pathFormatStrings");
      if (Enumerable.Any<string>(pathFormatStrings, (Func<string, bool>) (formatString => formatString == string.Format((IFormatProvider) CultureInfo.InvariantCulture, formatString, new object[1]
      {
        (object) "1"
      }))))
        throw new FormatException("Format strings evaluate to input with format argument.");
      if (startDigit < 0)
        throw new ArgumentOutOfRangeException("startDigit");
      Func<IEnumerable<string>, string, bool> func = (Func<IEnumerable<string>, string, bool>) ((formatStrings, formatArgument) => !Enumerable.Any<string>(formatStrings, (Func<string, bool>) (formatString => PathHelper.FileOrDirectoryExists(string.Format((IFormatProvider) CultureInfo.InvariantCulture, formatString, new object[1]
      {
        (object) formatArgument
      })))));
      if (!alwaysUseDigit && func(pathFormatStrings, ""))
        return string.Empty;
      int num = startDigit;
      while (!func(pathFormatStrings, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        ++num;
        if (num > 10000)
          return string.Empty;
      }
      return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string ChangeFileOrDirectoryName(string path, string newName)
    {
      return PathHelper.ResolveCombinedPath(PathHelper.GetParentDirectory(path), newName);
    }

    public static string GetFileOrDirectoryName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      path = PathHelper.TrimTrailingDirectorySeparators(path);
      if (path.Length == 0)
        return path;
      int length = path.Length;
      while (--length >= 0)
      {
        char ch = path[length];
        if ((int) ch == (int) Path.DirectorySeparatorChar || (int) ch == (int) Path.AltDirectorySeparatorChar || (int) ch == (int) Path.VolumeSeparatorChar)
          return path.Substring(length + 1);
      }
      return path;
    }

    public static string GetFileOrDirectoryNameWithoutExtension(string path)
    {
      return PathHelper.TrimExtension(PathHelper.GetFileOrDirectoryName(path));
    }

    private static bool IsUncJustServerShare(string unc)
    {
      int num1 = unc.IndexOfAny(PathHelper.directorySeparatorCharacters, 2);
      int num2;
      int num3 = unc.IndexOfAny(PathHelper.directorySeparatorCharacters, num2 = num1 + 1);
      if (num3 != -1)
        return num3 == unc.Length - 1;
      return true;
    }

    public static string GetDirectory(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (string.IsNullOrEmpty(path))
        return string.Empty;
      switch (PathHelper.GetPathFormat(path))
      {
        case PathFormat.UnknownFormat:
          throw new ArgumentOutOfRangeException("path", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid path format: '{0}'", new object[1]
          {
            (object) path
          }));
        case PathFormat.UniformNamingConvention:
          if (PathHelper.IsUncJustServerShare(path))
            return PathHelper.EnsurePathEndsInDirectorySeparator(path);
          break;
      }
      if (PathHelper.IsDirectory(path))
        return PathHelper.EnsurePathEndsInDirectorySeparator(path);
      string directoryName = Path.GetDirectoryName(path);
      if (!string.IsNullOrEmpty(directoryName))
        return PathHelper.EnsurePathEndsInDirectorySeparator(directoryName);
      return string.Empty;
    }

    public static DocumentReference GetParentDirectory(DocumentReference documentReference)
    {
      return DocumentReference.Create(PathHelper.GetParentDirectory(documentReference.Path));
    }

    public static string GetParentDirectory(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (string.IsNullOrEmpty(path))
        return string.Empty;
      switch (PathHelper.GetPathFormat(path))
      {
        case PathFormat.UnknownFormat:
          throw new ArgumentOutOfRangeException("path", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid path format: '{0}'", new object[1]
          {
            (object) path
          }));
        case PathFormat.UniformNamingConvention:
          if (PathHelper.IsUncJustServerShare(path))
            return PathHelper.EnsurePathEndsInDirectorySeparator(path);
          break;
      }
      if (PathHelper.IsDirectory(path))
      {
        string path1 = PathHelper.TrimTrailingDirectorySeparators(path);
        if (path1.Length == 0)
          return string.Empty;
        string directoryName = Path.GetDirectoryName(path1);
        if (string.IsNullOrEmpty(directoryName))
          return string.Empty;
        return PathHelper.EnsurePathEndsInDirectorySeparator(directoryName);
      }
      string directoryName1 = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(directoryName1))
        return string.Empty;
      return PathHelper.EnsurePathEndsInDirectorySeparator(directoryName1);
    }

    public static string CompressPathForDisplay(string path, int maximumLength)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (maximumLength < 10)
        throw new ArgumentOutOfRangeException("maximumLength", (object) maximumLength, "The maximum length must be at least 10");
      if (path.Length <= maximumLength)
        return path;
      StringBuilder stringBuilder = new StringBuilder(maximumLength);
      stringBuilder.Append(path, 0, 5);
      stringBuilder.Append("...");
      stringBuilder.Append(path, path.Length - (maximumLength - stringBuilder.Length), maximumLength - stringBuilder.Length);
      return stringBuilder.ToString();
    }

    public static bool FileOrDirectoryExists(string path)
    {
      if (path == null)
        return false;
      return AccessHelper.AccessService.MiscPathExists(path);
    }

    public static bool FileExists(string path)
    {
      if (path == null)
        return false;
      return AccessHelper.AccessService.MiscFileExists(path);
    }

    public static bool DirectoryExists(string path)
    {
      if (path == null)
        return false;
      return AccessHelper.AccessService.MiscDirectoryExists(path);
    }

    public static IEnumerable<string> EnumerateFiles(string directory, string fileName)
    {
      if (directory == null || fileName == null)
        return (IEnumerable<string>) new string[0];
      return AccessHelper.AccessService.DirectoryEnumerateFiles(directory, fileName);
    }

    public static bool ClearFileOrDirectoryReadOnlyAttribute(string path)
    {
      FileAttributes attributes;
      return !PathHelper.IsFileOrDirectoryReadOnly(path, out attributes) || AccessHelper.AccessService.MiscSetFileAttributes(path, attributes & ~FileAttributes.ReadOnly);
    }

    public static bool GetPathAttributes(string path, out FileAttributes attributes, bool useAccessService = true)
    {
      if (useAccessService)
        return AccessHelper.AccessService.MiscGetFileAttributes(path, out attributes);
      return NativeMethods.GetFileAttributes(path, out attributes);
    }

    public static bool IsFileOrDirectoryReadOnly(string path)
    {
      FileAttributes attributes;
      return PathHelper.IsFileOrDirectoryReadOnly(path, out attributes);
    }

    private static bool IsFileOrDirectoryReadOnly(string path, out FileAttributes attributes)
    {
      if (!AccessHelper.AccessService.MiscGetFileAttributes(path, out attributes))
        return false;
      return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
    }

    public static bool ShellAddPathToRecentDocuments(string path)
    {
      return AccessHelper.AccessService.MiscShellAddPathToRecentDocuments(path);
    }

    public static IEnumerable<string> FindCommonPathRoots(IEnumerable<DocumentReference> documentReferences)
    {
      bool anyNestedFiles;
      return PathHelper.FindCommonPathRoots(documentReferences, out anyNestedFiles);
    }

    public static IEnumerable<string> FindCommonPathRoots(IEnumerable<DocumentReference> documentReferences, out bool anyNestedFiles)
    {
      anyNestedFiles = false;
      HashSet<string> hashSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (DocumentReference documentReference in documentReferences)
      {
        if (documentReference.IsValidPathFormat)
        {
          string directory = PathHelper.GetDirectoryNameOrRoot(documentReference);
          if (!hashSet.Contains(directory))
          {
            if (hashSet.RemoveWhere((Predicate<string>) (existingDirectory => existingDirectory.StartsWith(directory, StringComparison.OrdinalIgnoreCase))) > 0)
            {
              anyNestedFiles = true;
              hashSet.Add(directory);
            }
            else if (Enumerable.Any<string>((IEnumerable<string>) hashSet, (Func<string, bool>) (root => directory.StartsWith(root, StringComparison.OrdinalIgnoreCase))))
              anyNestedFiles = true;
            else
              hashSet.Add(directory);
          }
        }
      }
      return (IEnumerable<string>) hashSet;
    }

    public static void DeleteWithUndo(string path)
    {
      if (!PathHelper.FileOrDirectoryExists(path))
        return;
      AccessHelper.AccessService.MiscDeleteWithUndo(PathHelper.TrimTrailingDirectorySeparators(path));
    }

    public static string GetRelativePath(string fromPath, string toPath)
    {
      return PathHelper.GetRelativePath(DocumentReference.Create(fromPath), DocumentReference.Create(toPath));
    }

    public static bool IsPathWithin(DocumentReference firstPath, DocumentReference secondPath)
    {
      if (firstPath != (DocumentReference) null && secondPath != (DocumentReference) null && (firstPath.IsValidPathFormat && secondPath.IsValidPathFormat))
        return firstPath.Path.StartsWith(secondPath.Path, StringComparison.OrdinalIgnoreCase);
      return false;
    }

    public static bool IsPathWithin(string firstPath, string secondPath)
    {
      return PathHelper.IsPathWithin(DocumentReference.Create(firstPath), DocumentReference.Create(secondPath));
    }

    public static string ConvertSourceToTarget(string pathInSource, string sourceDir, string targetDir)
    {
      if (pathInSource == null)
        throw new ArgumentNullException("pathInSource");
      if (sourceDir == null)
        throw new ArgumentNullException("sourceDir");
      if (targetDir == null)
        throw new ArgumentNullException("targetDir");
      string path1 = pathInSource;
      string path2 = sourceDir;
      if (!PathHelper.ResolvePath(ref path1) || !PathHelper.ResolvePath(ref path2))
        return (string) null;
      if (path2.Length >= path1.Length || !path1.StartsWith(path2, StringComparison.OrdinalIgnoreCase))
        return (string) null;
      string relativePath;
      if (!PathHelper.PathEndsInDirectorySeparator(path2))
      {
        if (!PathHelper.IsDirectorySeparator(path1[path2.Length]))
          return (string) null;
        if (path2.Length + 1 >= path1.Length)
          return (string) null;
        relativePath = path1.Substring(path2.Length + 1);
      }
      else
        relativePath = path1.Substring(path2.Length);
      return PathHelper.ResolveRelativePath(PathHelper.EnsurePathEndsInDirectorySeparator(targetDir), relativePath);
    }

    public static string GetRelativePath(DocumentReference fromPath, DocumentReference toPath)
    {
      if (fromPath == (DocumentReference) null)
        throw new ArgumentNullException("fromPath");
      if (toPath == (DocumentReference) null)
        throw new ArgumentNullException("toPath");
      if (!fromPath.IsValidPathFormat || !toPath.IsValidPathFormat)
        return (string) null;
      if (string.Equals(fromPath.Path, toPath.Path, StringComparison.OrdinalIgnoreCase))
        return string.Empty;
      string directory = PathHelper.GetDirectory(fromPath.Path);
      string path = toPath.Path;
      if (object.ReferenceEquals((object) directory, (object) path))
        return string.Empty;
      int index1;
      if ((int) directory[1] == 58)
      {
        if ((int) path[1] != 58 || (int) char.ToLowerInvariant(directory[0]) != (int) char.ToLowerInvariant(path[0]))
          return path;
        index1 = 2;
      }
      else
      {
        if ((int) path[1] == 58)
          return path;
        int num = directory.IndexOfAny(PathHelper.directorySeparatorCharacters, 2);
        index1 = directory.IndexOfAny(PathHelper.directorySeparatorCharacters, num + 1);
        if (path.Length <= index1 || (int) directory[index1] != (int) path[index1])
          return path;
      }
      string str1 = PathHelper.NormalizePathInternal(directory);
      string str2 = PathHelper.NormalizePathInternal(path);
      if (index1 != 2)
      {
        for (int index2 = 2; index2 < index1; ++index2)
        {
          if ((int) str1[index2] != (int) str2[index2])
            return path;
        }
      }
      int startIndex = index1;
      int num1 = index1;
      for (int index2 = startIndex; index2 < str1.Length && index2 < str2.Length; ++index2)
      {
        char character1 = str1[index2];
        char character2 = str2[index2];
        if (PathHelper.IsDirectorySeparator(character1) && PathHelper.IsDirectorySeparator(character2))
        {
          startIndex = index2;
          num1 = index2;
        }
        else if ((int) character1 == (int) character2)
          num1 = index2;
        else
          break;
      }
      if (str2.Length - 1 == num1 && str1.Length == str2.Length && PathHelper.IsDirectorySeparator(directory[num1 + 1]))
        return ".." + path.Substring(startIndex);
      if (PathHelper.IsDirectorySeparator(str2[num1 + 1]))
        startIndex = num1 + 1;
      int num2 = startIndex + 1;
      int num3 = 0;
      if (num2 < str1.Length)
      {
        ++num3;
        for (int index2 = num2; index2 < str1.Length; ++index2)
        {
          if (PathHelper.IsDirectorySeparator(str1[index2]))
            ++num3;
        }
      }
      if (num3 == 0)
        return path.Substring(startIndex + 1);
      StringBuilder stringBuilder = new StringBuilder(num3 * 3 + path.Length - startIndex);
      for (int index2 = 0; index2 < num3; ++index2)
        stringBuilder.Append("..\\");
      stringBuilder.Append(path.Substring(startIndex + 1));
      return stringBuilder.ToString();
    }

    public static bool HasExtension(string path, params string[] extensions)
    {
      string pathExtension = PathHelper.GetExtension(path);
      return Enumerable.Any<string>((IEnumerable<string>) extensions, (Func<string, bool>) (extension => string.Equals(pathExtension, extension, StringComparison.OrdinalIgnoreCase)));
    }

    public static bool TryRepeatedFileCopy(string source, string destination, bool overwrite, int timesToTry, TimeSpan timeout)
    {
      bool flag = true;
      for (int index = 0; index <= timesToTry; ++index)
      {
        Exception exception = (Exception) null;
        try
        {
          File.Copy(source, destination, overwrite);
          PathHelper.ClearFileOrDirectoryReadOnlyAttribute(destination);
        }
        catch (UnauthorizedAccessException ex)
        {
          exception = (Exception) ex;
        }
        catch (IOException ex)
        {
          exception = (Exception) ex;
        }
        flag = exception == null;
        if (!flag)
          Thread.Sleep((int) timeout.TotalMilliseconds);
        else
          break;
      }
      return flag;
    }

    private enum PathValidity
    {
      ValidPath,
      EmptyPath,
      InvalidServerShare,
      UnknownFormat,
      TrailingSpaceOrPeriod,
      AllDots,
      SingleDotInExtendedPath,
      InvalidPathCharacter,
      InvalidExtendedPathCharacter,
      ExtendedPathFormat,
      DeviceNamePresent,
    }

    private enum FileNameValidity
    {
      ValidFileName,
      EmptyName,
      TrailingSpaceOrPeriod,
      DeviceNamePresent,
      InvalidCharacters,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.PathHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Expression.Framework.Documents
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
    private static IndexedHashSet<string> resolvedPathsCache = new IndexedHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, 1024);
    public const int MaximumPathLength = 260;
    private const int MaxResolvedPaths = 10000;

    public static bool IsShortFileNameGenerationEnabled
    {
      get
      {
        return !RegistryHelper.RetrieveRegistryValue<bool>(Registry.LocalMachine, "HKEY_LOCAL_MACHINE\\\\SYSTEM\\CurrentControlSet\\Control\\FileSystem", "NtfsDisable8dot3NameCreation");
      }
    }

    public static char[] GetDirectorySeparatorCharacters()
    {
      return (char[]) PathHelper.directorySeparatorCharacters.Clone();
    }

    public static string GetShortPathName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      uint cchBuffer = 64U;
      StringBuilder lpszShortPath = new StringBuilder((int) cchBuffer);
      uint shortPathName;
      for (shortPathName = Microsoft.Expression.Framework.UnsafeNativeMethods.GetShortPathName(path, lpszShortPath, cchBuffer); shortPathName > cchBuffer; shortPathName = Microsoft.Expression.Framework.UnsafeNativeMethods.GetShortPathName(path, lpszShortPath, cchBuffer))
      {
        cchBuffer = shortPathName;
        lpszShortPath.EnsureCapacity((int) cchBuffer);
      }
      if ((int) shortPathName != 0)
        return lpszShortPath.ToString();
      switch (Marshal.GetLastWin32Error())
      {
        default:
          return (string) null;
      }
    }

    public static string GetFullPathName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      uint nBufferLength = 256U;
      StringBuilder lpBuffer = new StringBuilder((int) nBufferLength);
      uint fullPathName;
      for (fullPathName = Microsoft.Expression.Framework.UnsafeNativeMethods.GetFullPathName(path, nBufferLength, lpBuffer, IntPtr.Zero); fullPathName > nBufferLength; fullPathName = Microsoft.Expression.Framework.UnsafeNativeMethods.GetFullPathName(path, nBufferLength, lpBuffer, IntPtr.Zero))
      {
        nBufferLength = fullPathName;
        lpBuffer.EnsureCapacity((int) nBufferLength);
      }
      if ((int) fullPathName != 0)
        return lpBuffer.ToString();
      switch (Marshal.GetLastWin32Error())
      {
        case 2:
        case 3:
          return (string) null;
        default:
          goto case 2;
      }
    }

    public static bool IsDirectory(string path)
    {
      if (PathHelper.PathEndsInDirectorySeparator(path))
        return true;
      return Microsoft.Expression.Framework.NativeMethods.DirectoryExists(path);
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
      if (PathHelper.FileOrDirectoryExists(path))
        PathHelper.resolvedPathsCache.Add(path);
      return true;
    }

    private static string TryGetFullPath(string path)
    {
      try
      {
        path = Path.GetFullPath(path);
        if (PathHelper.IsDirectory(path))
          path = PathHelper.EnsurePathEndsInDirectorySeparator(path);
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
        {
            return null;
        }
        if (PathHelper.IsValidFileOrDirectoryName(name))
        {
            return name;
        }
        IEnumerable<char> chrs = name.Where<char>(new Func<char, bool>(name.ToCharArray().Except<char>(PathHelper.invalidFileNameCharacters).Contains<char>));
        string str = new string(chrs.ToArray<char>());
        string safeExtension = PathHelper.GetSafeExtension(str);
        if (!string.IsNullOrEmpty(str) && str.LastIndexOf(safeExtension, StringComparison.CurrentCultureIgnoreCase) != 0 && PathHelper.IsValidFileOrDirectoryName(str))
        {
            return str;
        }
        return null;
    }

    public static string GetSafeExtension(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return string.Empty;
      string str = (string) null;
      try
      {
        str = Path.GetExtension(fileName);
      }
      catch (ArgumentException ex)
      {
        int length = fileName.Length;
        int startIndex = length;
        while (--startIndex >= 0)
        {
          char ch = fileName[startIndex];
          if ((int) ch == 46)
          {
            if (startIndex != length - 1)
              return fileName.Substring(startIndex, length - startIndex);
            return string.Empty;
          }
          if ((int) ch == (int) Path.DirectorySeparatorChar || (int) ch == (int) Path.AltDirectorySeparatorChar || (int) ch == (int) Path.VolumeSeparatorChar)
            return string.Empty;
        }
      }
      return str;
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

    private static int InternalGenerateHashFromPath(string path)
    {
      return PathHelper.TrimTrailingDirectorySeparators(path.ToUpperInvariant()).GetHashCode();
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
      if (firstPath == null)
        throw new ArgumentNullException("firstPath");
      if (secondPath == null)
        throw new ArgumentNullException("secondPath");
      return PathHelper.GenerateHashFromPath(firstPath) == PathHelper.GenerateHashFromPath(secondPath);
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

    public static bool PathEndsInDirectorySeparator(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      char ch = path[path.Length - 1];
      if ((int) ch == (int) Path.DirectorySeparatorChar)
        return true;
      return (int) ch == (int) Path.AltDirectorySeparatorChar;
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

    public static string TrimTrailingPeriodsAndSpaces(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      return path.TrimEnd(PathHelper.trailingWhitespaceCharacters);
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
      }, alwaysAppendDigit);
      return desiredNameWithoutExtension + availablePaths.ToString() + str;
    }

    public static string FindAvailablePaths(IEnumerable<string> pathFormatStrings, bool alwaysUseDigit)
    {
      if (pathFormatStrings == null)
        throw new ArgumentNullException("pathFormatStrings");
      if (Enumerable.Any<string>(pathFormatStrings, (Func<string, bool>) (formatString => formatString == string.Format((IFormatProvider) CultureInfo.InvariantCulture, formatString, new object[1]
      {
        (object) "1"
      }))))
        throw new FormatException("Format strings evaluate to input with format argument.");
      Func<IEnumerable<string>, string, bool> func = (Func<IEnumerable<string>, string, bool>) ((formatStrings, formatArgument) => !Enumerable.Any<string>(formatStrings, (Func<string, bool>) (formatString => PathHelper.FileOrDirectoryExists(string.Format((IFormatProvider) CultureInfo.InvariantCulture, formatString, new object[1]
      {
        (object) formatArgument
      })))));
      if (!alwaysUseDigit && func(pathFormatStrings, ""))
        return string.Empty;
      int num = 1;
      while (!func(pathFormatStrings, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        ++num;
        if (num > 10000)
          return string.Empty;
      }
      return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string GetFileOrDirectoryName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (string.IsNullOrEmpty(path))
        return path;
      return Path.GetFileName(PathHelper.TrimTrailingDirectorySeparators(path));
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
      return Microsoft.Expression.Framework.NativeMethods.PathExists(path);
    }

    public static bool FileExists(string path)
    {
      return Microsoft.Expression.Framework.NativeMethods.FileExists(path);
    }

    public static bool DirectoryExists(string path)
    {
      return Microsoft.Expression.Framework.NativeMethods.DirectoryExists(path);
    }

    public static bool ClearFileOrDirectoryReadOnlyAttribute(string path)
    {
      FileAttributes attributes;
      return !PathHelper.IsFileOrDirectoryReadOnly(path, out attributes) || Microsoft.Expression.Framework.NativeMethods.SetFileAttributes(path, attributes & ~FileAttributes.ReadOnly);
    }

    public static bool IsFileOrDirectoryReadOnly(string path)
    {
      FileAttributes attributes;
      return PathHelper.IsFileOrDirectoryReadOnly(path, out attributes);
    }

    private static bool IsFileOrDirectoryReadOnly(string path, out FileAttributes attributes)
    {
      if (!Microsoft.Expression.Framework.NativeMethods.GetFileAttributes(path, out attributes))
        return false;
      return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
    }

    public static bool ShellAddPathToRecentDocuments(string path)
    {
      return Microsoft.Expression.Framework.NativeMethods.ShellAddPathToRecentDocuments(path);
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

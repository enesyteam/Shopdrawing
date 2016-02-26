// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.AccessService
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Utility.IO
{
  internal sealed class AccessService : IAccessService
  {
    private bool isWin8;

    internal AccessService(bool isWin8)
    {
      this.isWin8 = isWin8;
    }

    public FileStream FileCreate(string path)
    {
      FileStream fileStream = File.Create(path);
      this.AddFileLowBoxPermissionsIfNecessary(path, FileMode.Create);
      return fileStream;
    }

    public FileStream FileCreate(string path, int bufferSize)
    {
      FileStream fileStream = File.Create(path, bufferSize);
      this.AddFileLowBoxPermissionsIfNecessary(path, FileMode.Create);
      return fileStream;
    }

    public FileStream FileCreate(string path, int bufferSize, FileOptions options)
    {
      FileStream fileStream = File.Create(path, bufferSize, options);
      this.AddFileLowBoxPermissionsIfNecessary(path, FileMode.Create);
      return fileStream;
    }

    public FileStream FileCreate(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
    {
      FileStream fileStream = File.Create(path, bufferSize, options, fileSecurity);
      this.AddFileLowBoxPermissionsIfNecessary(path, FileMode.Create);
      return fileStream;
    }

    public FileStream FileOpen(string path, FileMode mode)
    {
      FileStream fileStream = File.Open(path, mode);
      this.AddFileLowBoxPermissionsIfNecessary(path, mode);
      return fileStream;
    }

    public FileStream FileOpen(string path, FileMode mode, FileAccess access)
    {
      FileStream fileStream = File.Open(path, mode, access);
      this.AddFileLowBoxPermissionsIfNecessary(path, mode);
      return fileStream;
    }

    public FileStream FileOpen(string path, FileMode mode, FileAccess access, FileShare share)
    {
      FileStream fileStream = File.Open(path, mode, access, share);
      this.AddFileLowBoxPermissionsIfNecessary(path, mode);
      return fileStream;
    }

    public FileStream FileOpenRead(string path)
    {
      return File.OpenRead(path);
    }

    public FileStream FileOpenWrite(string path)
    {
      return File.OpenWrite(path);
    }

    public void FileAppendAllLines(string path, IEnumerable<string> contents)
    {
      File.AppendAllLines(path, contents);
    }

    public void FileAppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
      File.AppendAllLines(path, contents, encoding);
    }

    public void FileAppendAllText(string path, string contents)
    {
      File.AppendAllText(path, contents);
    }

    public void FileAppendAllText(string path, string contents, Encoding encoding)
    {
      File.AppendAllText(path, contents, encoding);
    }

    public StreamWriter FileAppendText(string path)
    {
      return File.AppendText(path);
    }

    public StreamWriter FileCreateText(string path)
    {
      return File.CreateText(path);
    }

    public StreamReader FileOpenText(string path)
    {
      return File.OpenText(path);
    }

    public byte[] FileReadBytes(string path, int count)
    {
      using (BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(path)))
        return binaryReader.ReadBytes(count);
    }

    public byte[] FileReadAllBytes(string path)
    {
      return File.ReadAllBytes(path);
    }

    public string[] FileReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    public string[] FileReadAllLines(string path, Encoding encoding)
    {
      return File.ReadAllLines(path, encoding);
    }

    public string FileReadAllText(string path)
    {
      return File.ReadAllText(path);
    }

    public string FileReadAllText(string path, Encoding encoding)
    {
      return File.ReadAllText(path, encoding);
    }

    public IEnumerable<string> FileReadLines(string path)
    {
      return File.ReadLines(path);
    }

    public IEnumerable<string> FileReadLines(string path, Encoding encoding)
    {
      return File.ReadLines(path, encoding);
    }

    public void FileWriteAllBytes(string path, byte[] bytes)
    {
      File.WriteAllBytes(path, bytes);
    }

    public void FileWriteAllLines(string path, IEnumerable<string> contents)
    {
      File.WriteAllLines(path, contents);
    }

    public void FileWriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
      File.WriteAllLines(path, contents, encoding);
    }

    public void FileWriteAllText(string path, string contents)
    {
      File.WriteAllText(path, contents);
    }

    public void FileWriteAllText(string path, string contents, Encoding encoding)
    {
      File.WriteAllText(path, contents, encoding);
    }

    public void FileCopy(string sourceFileName, string destFileName)
    {
      File.Copy(sourceFileName, destFileName);
    }

    public void FileCopy(string sourceFileName, string destFileName, bool overwrite)
    {
      File.Copy(sourceFileName, destFileName, overwrite);
    }

    public void FileDecrypt(string path)
    {
      File.Decrypt(path);
    }

    public void FileDelete(string path)
    {
      File.Delete(path);
    }

    public void FileEncrypt(string path)
    {
      File.Encrypt(path);
    }

    public bool FileExists(string path)
    {
      return PathHelper.FileExists(path);
    }

    public FileSecurity FileGetAccessControl(string path)
    {
      return File.GetAccessControl(path);
    }

    public FileSecurity FileGetAccessControl(string path, AccessControlSections includeSections)
    {
      return File.GetAccessControl(path, includeSections);
    }

    public FileAttributes FileGetAttributes(string path)
    {
      FileAttributes attributes;
      PathHelper.GetPathAttributes(path, out attributes, false);
      return attributes;
    }

    public DateTime FileGetCreationTime(string path)
    {
      return File.GetCreationTime(path);
    }

    public DateTime FileGetLastAccessTime(string path)
    {
      return File.GetLastAccessTime(path);
    }

    public DateTime FileGetLastWriteTime(string path)
    {
      return File.GetLastWriteTime(path);
    }

    public void FileMove(string sourceFileName, string destFileName)
    {
      File.Move(sourceFileName, destFileName);
    }

    public void FileReplace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
    {
      File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
    }

    public void FileReplace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
      File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
    }

    public void FileSetAccessControl(string path, FileSecurity fileSecurity)
    {
      File.SetAccessControl(path, fileSecurity);
    }

    public void FileSetAttributes(string path, FileAttributes fileAttributes)
    {
      File.SetAttributes(path, fileAttributes);
    }

    public void FileSetCreationTime(string path, DateTime creationTime)
    {
      File.SetCreationTime(path, creationTime);
    }

    public void FileSetLastAccessTime(string path, DateTime lastAccessTime)
    {
      File.SetLastAccessTime(path, lastAccessTime);
    }

    public void FileSetLastWriteTime(string path, DateTime lastWriteTime)
    {
      File.SetLastWriteTime(path, lastWriteTime);
    }

    public DateTime FileGetCreationTimeUtc(string path)
    {
      return File.GetCreationTimeUtc(path);
    }

    public DateTime FileGetLastAccessTimeUtc(string path)
    {
      return File.GetLastAccessTimeUtc(path);
    }

    public DateTime FileGetLastWriteTimeUtc(string path)
    {
      return File.GetLastWriteTimeUtc(path);
    }

    public void FileSetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
      File.SetCreationTimeUtc(path, creationTimeUtc);
    }

    public void FileSetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
      File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
    }

    public void FileSetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
      File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
    }

    public DirectoryInfo DirectoryCreateDirectory(string path)
    {
      DirectoryInfo directory = Directory.CreateDirectory(path);
      this.AddDirectoryLowBoxPermissionsIfNecessary(path);
      return directory;
    }

    public DirectoryInfo DirectoryCreateDirectory(string path, DirectorySecurity directorySecurity)
    {
      DirectoryInfo directory = Directory.CreateDirectory(path, directorySecurity);
      this.AddDirectoryLowBoxPermissionsIfNecessary(path);
      return directory;
    }

    public void DirectoryDelete(string path)
    {
      Directory.Delete(path);
    }

    public void DirectoryDelete(string path, bool recursive)
    {
      Directory.Delete(path, recursive);
    }

    public IEnumerable<string> DirectoryEnumerateDirectories(string path)
    {
      return Directory.EnumerateDirectories(path);
    }

    public IEnumerable<string> DirectoryEnumerateDirectories(string path, string searchPattern)
    {
      return Directory.EnumerateDirectories(path, searchPattern);
    }

    public IEnumerable<string> DirectoryEnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public IEnumerable<string> DirectoryEnumerateFiles(string path)
    {
      return Directory.EnumerateFiles(path);
    }

    public IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern)
    {
      return Directory.EnumerateFiles(path, searchPattern);
    }

    public IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path)
    {
      return Directory.EnumerateFileSystemEntries(path);
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path, string searchPattern)
    {
      return Directory.EnumerateFileSystemEntries(path, searchPattern);
    }

    public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
    }

    public bool DirectoryExists(string path)
    {
      return PathHelper.DirectoryExists(path);
    }

    public DirectorySecurity DirectoryGetAccessControl(string path)
    {
      return Directory.GetAccessControl(path);
    }

    public DirectorySecurity DirectoryGetAccessControl(string path, AccessControlSections includeSections)
    {
      return Directory.GetAccessControl(path, includeSections);
    }

    public DateTime DirectoryGetCreationTime(string path)
    {
      return Directory.GetCreationTime(path);
    }

    public string DirectoryGetCurrentDirectory()
    {
      return Directory.GetCurrentDirectory();
    }

    public string[] DirectoryGetDirectories(string path)
    {
      return Directory.GetDirectories(path);
    }

    public string[] DirectoryGetDirectories(string path, string searchPattern)
    {
      return Directory.GetDirectories(path, searchPattern);
    }

    public string[] DirectoryGetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetDirectories(path, searchPattern, searchOption);
    }

    public string DirectoryGetDirectoryRoot(string path)
    {
      return Directory.GetDirectoryRoot(path);
    }

    public string[] DirectoryGetFiles(string path)
    {
      return Directory.GetFiles(path);
    }

    public string[] DirectoryGetFiles(string path, string searchPattern)
    {
      return Directory.GetFiles(path, searchPattern);
    }

    public string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public string[] DirectoryGetFileSystemEntries(string path)
    {
      return Directory.GetFileSystemEntries(path);
    }

    public string[] DirectoryGetFileSystemEntries(string path, string searchPattern)
    {
      return Directory.GetFileSystemEntries(path, searchPattern);
    }

    public string[] DirectoryGetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetFileSystemEntries(path, searchPattern, searchOption);
    }

    public DateTime DirectoryGetLastAccessTime(string path)
    {
      return Directory.GetLastAccessTime(path);
    }

    public DateTime DirectoryGetLastWriteTime(string path)
    {
      return Directory.GetLastWriteTime(path);
    }

    public string[] DirectoryGetLogicalDrives()
    {
      return Directory.GetLogicalDrives();
    }

    public DirectoryInfo DirectoryGetParent(string path)
    {
      return Directory.GetParent(path);
    }

    public void DirectoryMove(string sourceDirName, string destDirName)
    {
      Directory.Move(sourceDirName, destDirName);
    }

    public void DirectorySetAccessControl(string path, DirectorySecurity directorySecurity)
    {
      Directory.SetAccessControl(path, directorySecurity);
    }

    public void DirectorySetCreationTime(string path, DateTime creationTime)
    {
      Directory.SetCreationTime(path, creationTime);
    }

    public void DirectorySetCurrentDirectory(string path)
    {
      Directory.SetCurrentDirectory(path);
    }

    public void DirectorySetLastAccessTime(string path, DateTime lastAccessTime)
    {
      Directory.SetLastAccessTime(path, lastAccessTime);
    }

    public void DirectorySetLastWriteTime(string path, DateTime lastWriteTime)
    {
      Directory.SetLastWriteTime(path, lastWriteTime);
    }

    public DateTime DirectoryGetCreationTimeUtc(string path)
    {
      return Directory.GetCreationTimeUtc(path);
    }

    public DateTime DirectoryGetLastAccessTimeUtc(string path)
    {
      return Directory.GetLastAccessTimeUtc(path);
    }

    public DateTime DirectoryGetLastWriteTimeUtc(string path)
    {
      return Directory.GetLastWriteTimeUtc(path);
    }

    public void DirectorySetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
      Directory.SetCreationTimeUtc(path, creationTimeUtc);
    }

    public void DirectorySetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
      Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
    }

    public void DirectorySetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
      Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
    }

    public object RegistryGetValue(string keyName, string valueName, object defaultValue)
    {
      return Registry.GetValue(keyName, valueName, defaultValue);
    }

    public void RegistrySetValue(string keyName, string valueName, object value)
    {
      Registry.SetValue(keyName, valueName, value);
    }

    public void RegistrySetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
    {
      Registry.SetValue(keyName, valueName, value, valueKind);
    }

    public string MiscGetShortPathName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      uint cchBuffer = 64U;
      StringBuilder lpszShortPath = new StringBuilder((int) cchBuffer);
      uint shortPathName;
      for (shortPathName = Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.GetShortPathName(path, lpszShortPath, cchBuffer); shortPathName > cchBuffer; shortPathName = Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.GetShortPathName(path, lpszShortPath, cchBuffer))
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

    public string MiscGetFullPathName(string path)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      uint nBufferLength = 256U;
      StringBuilder lpBuffer = new StringBuilder((int) nBufferLength);
      uint fullPathName;
      for (fullPathName = Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.GetFullPathName(path, nBufferLength, lpBuffer, IntPtr.Zero); fullPathName > nBufferLength; fullPathName = Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.GetFullPathName(path, nBufferLength, lpBuffer, IntPtr.Zero))
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

    public bool CompareFileContents(string file1FullPath, string file2FullPath)
    {
      try
      {
        if (string.IsNullOrEmpty(file1FullPath) || !PathHelper.FileExists(file1FullPath) || (string.IsNullOrEmpty(file2FullPath) || !PathHelper.FileExists(file2FullPath)))
          return false;
        if (file1FullPath == file2FullPath)
          return true;
        FileInfo fileInfo1 = new FileInfo(file1FullPath);
        FileInfo fileInfo2 = new FileInfo(file2FullPath);
        if (fileInfo1.Length != fileInfo2.Length)
          return false;
        using (FileStream fileStream1 = fileInfo1.OpenRead())
        {
          using (FileStream fileStream2 = fileInfo2.OpenRead())
          {
            int num1 = fileStream1.ReadByte();
            int num2 = fileStream2.ReadByte();
            while (num1 != -1)
            {
              if (num1 != num2)
                return false;
              num1 = fileStream1.ReadByte();
              num2 = fileStream2.ReadByte();
            }
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool MiscCanLoadAssembly(string path)
    {
      return AssemblyHelper.CanLoadAssembly(path);
    }

    public string MiscGetTempFileName()
    {
      return Path.GetTempFileName();
    }

    public bool MiscGetFileAttributes(string path, out FileAttributes attributes)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.GetFileAttributes(path, out attributes);
    }

    public bool MiscPathExists(string path)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.PathExists(path);
    }

    public bool MiscDirectoryExists(string path)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.DirectoryExists(path);
    }

    public bool MiscFileExists(string path)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.FileExists(path);
    }

    public bool MiscPathHidden(string path)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.PathHidden(path);
    }

    public bool MiscSetFileAttributes(string path, FileAttributes attributes)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.SetFileAttributes(path, attributes);
    }

    public bool MiscShellAddPathToRecentDocuments(string path)
    {
      return Microsoft.Expression.Utility.Interop.NativeMethods.ShellAddPathToRecentDocuments(path);
    }

    public void MiscDeleteWithUndo(string path)
    {
      Microsoft.Expression.Utility.Interop.NativeMethods.DeleteWithUndo(path);
    }

    public void MiscConvertExeToDll(string exeAsDllPath)
    {
      PEHeader.ConvertExeToDll(exeAsDllPath);
    }

    public void LaunchNotepad(string arguments)
    {
      Process.Start("Notepad.exe", arguments);
    }

    public void LaunchWebPage(Uri uri)
    {
      WebPageLauncher.Navigate(uri, (Action<string>) null, (string) null);
    }

    public bool SetCursorPos(System.Windows.Point position)
    {
      return Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.SetCursorPos((int) position.X, (int) position.Y);
    }

    public AssemblyName GetAssemblyNameFromPath(string path)
    {
      return AssemblyHelper.GetAssemblyNameFromPath(path);
    }

    public byte[] GetAssociatedIcon(string path, int width, int height)
    {
      IntPtr num = IntPtr.Zero;
      BitmapSource bitmapSource = (BitmapSource) null;
      try
      {
        Microsoft.Expression.Utility.Interop.NativeMethods.SHFILEINFO psfi = new Microsoft.Expression.Utility.Interop.NativeMethods.SHFILEINFO();
        Microsoft.Expression.Utility.Interop.NativeMethods.SHGetFileInfo(path, 0U, ref psfi, (uint) Marshal.SizeOf((object) psfi), Microsoft.Expression.Utility.Interop.NativeMethods.SHGFI.Icon | Microsoft.Expression.Utility.Interop.NativeMethods.SHGFI.SmallIcon);
        num = psfi.hIcon;
        bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(Icon.FromHandle(num).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));
      }
      catch (Exception ex)
      {
      }
      if (num != IntPtr.Zero)
        Microsoft.Expression.Utility.Interop.UnsafeNativeMethods.DestroyIcon(num);
      byte[] numArray = (byte[]) null;
      if (bitmapSource != null)
      {
        int stride = 4 * width;
        numArray = new byte[stride * height];
        bitmapSource.CopyPixels((Array) numArray, stride, 0);
      }
      return numArray;
    }

    private void AddFileLowBoxPermissionsIfNecessary(string path, FileMode mode)
    {
      if (!this.isWin8 || !path.Contains("AppData") || mode != FileMode.OpenOrCreate && mode != FileMode.Create && mode != FileMode.CreateNew)
        return;
      FileSecurity accessControl = File.GetAccessControl(path);
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, FileSystemRights.FullControl, AccessControlType.Allow));
      File.SetAccessControl(path, accessControl);
    }

    private void AddDirectoryLowBoxPermissionsIfNecessary(string path)
    {
      if (!this.isWin8 || !path.Contains("AppData"))
        return;
      DirectorySecurity accessControl = Directory.GetAccessControl(path);
      accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) AccessHelper.AllApplicationPackagesSecurityIdentifier, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
      Directory.SetAccessControl(path, accessControl);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.IAccessService
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Windows;

namespace Microsoft.Expression.Utility.IO
{
  public interface IAccessService
  {
    FileStream FileCreate(string path);

    FileStream FileCreate(string path, int bufferSize);

    FileStream FileCreate(string path, int bufferSize, FileOptions options);

    FileStream FileCreate(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity);

    FileStream FileOpen(string path, FileMode mode);

    FileStream FileOpen(string path, FileMode mode, FileAccess access);

    FileStream FileOpen(string path, FileMode mode, FileAccess access, FileShare share);

    FileStream FileOpenRead(string path);

    FileStream FileOpenWrite(string path);

    void FileAppendAllLines(string path, IEnumerable<string> contents);

    void FileAppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    void FileAppendAllText(string path, string contents);

    void FileAppendAllText(string path, string contents, Encoding encoding);

    StreamWriter FileAppendText(string path);

    StreamWriter FileCreateText(string path);

    StreamReader FileOpenText(string path);

    byte[] FileReadBytes(string path, int count);

    byte[] FileReadAllBytes(string path);

    string[] FileReadAllLines(string path);

    string[] FileReadAllLines(string path, Encoding encoding);

    string FileReadAllText(string path);

    string FileReadAllText(string path, Encoding encoding);

    IEnumerable<string> FileReadLines(string path);

    IEnumerable<string> FileReadLines(string path, Encoding encoding);

    void FileWriteAllBytes(string path, byte[] bytes);

    void FileWriteAllLines(string path, IEnumerable<string> contents);

    void FileWriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    void FileWriteAllText(string path, string contents);

    void FileWriteAllText(string path, string contents, Encoding encoding);

    void FileCopy(string sourceFileName, string destFileName);

    void FileCopy(string sourceFileName, string destFileName, bool overwrite);

    void FileDecrypt(string path);

    void FileDelete(string path);

    void FileEncrypt(string path);

    bool FileExists(string path);

    FileSecurity FileGetAccessControl(string path);

    FileSecurity FileGetAccessControl(string path, AccessControlSections includeSections);

    FileAttributes FileGetAttributes(string path);

    DateTime FileGetCreationTime(string path);

    DateTime FileGetLastAccessTime(string path);

    DateTime FileGetLastWriteTime(string path);

    void FileMove(string sourceFileName, string destFileName);

    void FileReplace(string sourceFileName, string destinationFileName, string destinationBackupFileName);

    void FileReplace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);

    void FileSetAccessControl(string path, FileSecurity fileSecurity);

    void FileSetAttributes(string path, FileAttributes fileAttributes);

    void FileSetCreationTime(string path, DateTime creationTime);

    void FileSetLastAccessTime(string path, DateTime lastAccessTime);

    void FileSetLastWriteTime(string path, DateTime lastWriteTime);

    DateTime FileGetCreationTimeUtc(string path);

    DateTime FileGetLastAccessTimeUtc(string path);

    DateTime FileGetLastWriteTimeUtc(string path);

    void FileSetCreationTimeUtc(string path, DateTime creationTimeUtc);

    void FileSetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

    void FileSetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

    bool CompareFileContents(string file1FullPath, string file2FullPath);

    DirectoryInfo DirectoryCreateDirectory(string path);

    DirectoryInfo DirectoryCreateDirectory(string path, DirectorySecurity directorySecurity);

    void DirectoryDelete(string path);

    void DirectoryDelete(string path, bool recursive);

    IEnumerable<string> DirectoryEnumerateDirectories(string path);

    IEnumerable<string> DirectoryEnumerateDirectories(string path, string searchPattern);

    IEnumerable<string> DirectoryEnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

    IEnumerable<string> DirectoryEnumerateFiles(string path);

    IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern);

    IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern, SearchOption searchOption);

    IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path);

    IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path, string searchPattern);

    IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption);

    bool DirectoryExists(string path);

    DirectorySecurity DirectoryGetAccessControl(string path);

    DirectorySecurity DirectoryGetAccessControl(string path, AccessControlSections includeSections);

    DateTime DirectoryGetCreationTime(string path);

    string DirectoryGetCurrentDirectory();

    string[] DirectoryGetDirectories(string path);

    string[] DirectoryGetDirectories(string path, string searchPattern);

    string[] DirectoryGetDirectories(string path, string searchPattern, SearchOption searchOption);

    string DirectoryGetDirectoryRoot(string path);

    string[] DirectoryGetFiles(string path);

    string[] DirectoryGetFiles(string path, string searchPattern);

    string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption);

    string[] DirectoryGetFileSystemEntries(string path);

    string[] DirectoryGetFileSystemEntries(string path, string searchPattern);

    string[] DirectoryGetFileSystemEntries(string path, string searchPattern, SearchOption searchOption);

    DateTime DirectoryGetLastAccessTime(string path);

    DateTime DirectoryGetLastWriteTime(string path);

    string[] DirectoryGetLogicalDrives();

    DirectoryInfo DirectoryGetParent(string path);

    void DirectoryMove(string sourceDirName, string destDirName);

    void DirectorySetAccessControl(string path, DirectorySecurity directorySecurity);

    void DirectorySetCreationTime(string path, DateTime creationTime);

    void DirectorySetCurrentDirectory(string path);

    void DirectorySetLastAccessTime(string path, DateTime lastAccessTime);

    void DirectorySetLastWriteTime(string path, DateTime lastWriteTime);

    DateTime DirectoryGetCreationTimeUtc(string path);

    DateTime DirectoryGetLastAccessTimeUtc(string path);

    DateTime DirectoryGetLastWriteTimeUtc(string path);

    void DirectorySetCreationTimeUtc(string path, DateTime creationTimeUtc);

    void DirectorySetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

    void DirectorySetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

    object RegistryGetValue(string keyName, string valueName, object defaultValue);

    void RegistrySetValue(string keyName, string valueName, object value);

    void RegistrySetValue(string keyName, string valueName, object value, RegistryValueKind valueKind);

    string MiscGetShortPathName(string path);

    string MiscGetFullPathName(string path);

    string MiscGetTempFileName();

    bool MiscCanLoadAssembly(string path);

    bool MiscGetFileAttributes(string path, out FileAttributes attributes);

    bool MiscPathExists(string path);

    bool MiscDirectoryExists(string path);

    bool MiscFileExists(string path);

    bool MiscPathHidden(string path);

    bool MiscSetFileAttributes(string path, FileAttributes attributes);

    bool MiscShellAddPathToRecentDocuments(string path);

    void MiscDeleteWithUndo(string path);

    void MiscConvertExeToDll(string exeAsDllPath);

    void LaunchNotepad(string arguments);

    bool SetCursorPos(Point position);

    void LaunchWebPage(Uri uri);

    AssemblyName GetAssemblyNameFromPath(string path);

    byte[] GetAssociatedIcon(string path, int width, int height);
  }
}

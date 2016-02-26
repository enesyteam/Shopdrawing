// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.FileChangeWatcher
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using Microsoft.Expression.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.Expression.Utility.IO
{
  public class FileChangeWatcher : IFileChangeWatcher, IDisposable
  {
    private Dictionary<string, FileChangeWatcher.WatchedDirectory> pathToWatchedDirectories = new Dictionary<string, FileChangeWatcher.WatchedDirectory>((IEqualityComparer<string>) PathComparer.Instance);
    private object syncLock = new object();

    public event EventHandler<FileSystemEventArgs> FileChanged;

    public void WatchFile(string filePath)
    {
      bool flag = false;
      string str = PathHelper.GetDirectory(filePath);
      if (!FileChangeWatcher.DirectoryIsAccessible(str))
      {
        flag = true;
        str = FileChangeWatcher.GetExistingParentDirectory(str);
      }
      if (string.IsNullOrEmpty(str))
        return;
      FileChangeWatcher.WatchedDirectory watchedDirectory;
      lock (this.syncLock)
      {
        if (!this.pathToWatchedDirectories.TryGetValue(str, out watchedDirectory))
        {
          watchedDirectory = new FileChangeWatcher.WatchedDirectory(this, str);
          this.pathToWatchedDirectories.Add(str, watchedDirectory);
        }
      }
      if (flag)
        watchedDirectory.IncludeSubdirectories = flag;
      watchedDirectory.WatchFile(filePath);
    }

    private void MoveToParentDirectory(FileChangeWatcher.WatchedDirectory watchedDirectory)
    {
      this.pathToWatchedDirectories.Remove(watchedDirectory.Path);
      string existingParentDirectory = FileChangeWatcher.GetExistingParentDirectory(watchedDirectory.Path);
      if (string.IsNullOrEmpty(existingParentDirectory))
        return;
      string[] strArray = Enumerable.ToArray<string>(watchedDirectory.FilePaths);
      if (!this.pathToWatchedDirectories.TryGetValue(existingParentDirectory, out watchedDirectory))
      {
        watchedDirectory = new FileChangeWatcher.WatchedDirectory(this, existingParentDirectory);
        this.pathToWatchedDirectories.Add(existingParentDirectory, watchedDirectory);
      }
      watchedDirectory.IncludeSubdirectories = true;
      foreach (string filePath in strArray)
        watchedDirectory.WatchFile(filePath);
    }

    private static bool DirectoryIsAccessible(string directory)
    {
      try
      {
        return PathHelper.DirectoryExists(directory);
      }
      catch (UnauthorizedAccessException ex)
      {
        return false;
      }
    }

    private static string GetExistingParentDirectory(string directory)
    {
      do
      {
        string parentDirectory = PathHelper.GetParentDirectory(directory);
        if (string.Equals(parentDirectory, directory, StringComparison.OrdinalIgnoreCase))
          return (string) null;
        directory = parentDirectory;
      }
      while (!FileChangeWatcher.DirectoryIsAccessible(directory));
      return directory;
    }

    public void StopWatchingFile(string filePath)
    {
      string directory = PathHelper.GetDirectory(filePath);
      FileChangeWatcher.WatchedDirectory watchedDirectory;
      lock (this.syncLock)
      {
        if (this.pathToWatchedDirectories.TryGetValue(directory, out watchedDirectory))
        {
          if (watchedDirectory.StopWatchingFile(filePath))
            return;
          this.pathToWatchedDirectories.Remove(directory);
        }
      }
      if (watchedDirectory == null)
        return;
      watchedDirectory.Dispose();
    }

    private void OnFileChanged(FileChangeWatcher.WatchedDirectory sender, FileSystemEventArgs eventArgs, bool raiseEvent)
    {
      if (raiseEvent && this.FileChanged != null)
        this.FileChanged((object) this, eventArgs);
      if (!Monitor.TryEnter(this.syncLock))
        return;
      try
      {
        if (this.CanWatchChildren(sender))
          return;
        this.MoveToParentDirectory(sender);
        sender.Dispose();
      }
      finally
      {
        Monitor.Exit(this.syncLock);
      }
    }

    private bool CanWatchChildren(FileChangeWatcher.WatchedDirectory watchedDirectory)
    {
      bool canWatchChildren = true;
      ErrorHandling.HandleBasicExceptions((Action) (() =>
      {
        if (PathHelper.DirectoryExists(watchedDirectory.Path) && Enumerable.Any<string>(Directory.EnumerateFileSystemEntries(watchedDirectory.Path)))
          return;
        canWatchChildren = false;
      }), (Action<Exception>) (exception => canWatchChildren = false), new Func<Exception, bool>(ErrorHandling.BasicIOExceptionHandler));
      return canWatchChildren;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      lock (this.syncLock)
      {
        foreach (Component item_0 in this.pathToWatchedDirectories.Values)
          item_0.Dispose();
        this.pathToWatchedDirectories.Clear();
      }
    }

    private class WatchedDirectory : FileSystemWatcher
    {
      private HashSet<string> filePaths = new HashSet<string>((IEqualityComparer<string>) PathComparer.Instance);
      private FileChangeWatcher watcher;

      public IEnumerable<string> FilePaths
      {
        get
        {
          return (IEnumerable<string>) this.filePaths;
        }
      }

      internal WatchedDirectory(FileChangeWatcher watcher, string directory)
        : base(directory)
      {
        this.watcher = watcher;
        this.IncludeSubdirectories = false;
        this.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        this.Changed += new FileSystemEventHandler(this.OnFileChanged);
        this.Deleted += new FileSystemEventHandler(this.OnFileChanged);
        this.Renamed += new RenamedEventHandler(this.OnFileChanged);
      }

      internal void WatchFile(string filePath)
      {
        if (!this.filePaths.Add(filePath))
          return;
        this.EnableRaisingEvents = true;
      }

      internal bool StopWatchingFile(string filePath)
      {
        if (!this.filePaths.Remove(filePath) || this.filePaths.Count != 0)
          return true;
        this.EnableRaisingEvents = false;
        return false;
      }

      private void OnFileChanged(object sender, FileSystemEventArgs eventArgs)
      {
        string str = eventArgs.ChangeType == WatcherChangeTypes.Renamed ? ((RenamedEventArgs) eventArgs).OldFullPath : eventArgs.FullPath;
        try
        {
          this.watcher.OnFileChanged(this, eventArgs, this.filePaths.Contains(str));
        }
        catch (AccessViolationException ex)
        {
        }
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          this.EnableRaisingEvents = false;
          this.Changed -= new FileSystemEventHandler(this.OnFileChanged);
          this.Deleted -= new FileSystemEventHandler(this.OnFileChanged);
          this.Renamed -= new RenamedEventHandler(this.OnFileChanged);
        }
        base.Dispose(disposing);
      }
    }
  }
}

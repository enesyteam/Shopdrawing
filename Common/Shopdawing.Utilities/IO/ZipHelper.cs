// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.ZipHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Microsoft.Expression.Utility.IO
{
  public static class ZipHelper
  {
    public static string GetFileText(string zipPath, string fileName)
    {
      using (ZipArchive zipArchive = ZipHelper.SafeOpenFile(zipPath, true))
      {
        ZipArchiveEntry entryCaseInsensitive = ZipHelper.GetEntryCaseInsensitive(zipArchive, fileName);
        if (entryCaseInsensitive == null)
          return (string) null;
        using (StreamReader streamReader = new StreamReader(entryCaseInsensitive.Open()))
          return streamReader.ReadToEnd();
      }
    }

    public static Stream GetFileStream(string zipPath, string fileName)
    {
      return (Stream) ZipHelper.ZipSubStream.Create(zipPath, fileName);
    }

    public static bool SaveFile(string zipPath, string sourceFileName, string destinationPath)
    {
      using (ZipArchive zipArchive = ZipHelper.SafeOpenFile(zipPath, true))
      {
        ZipArchiveEntry entryCaseInsensitive = ZipHelper.GetEntryCaseInsensitive(zipArchive, sourceFileName);
        if (entryCaseInsensitive == null)
          return false;
        using (Stream stream = entryCaseInsensitive.Open())
        {
          using (FileStream fileStream = File.OpenWrite(destinationPath))
            stream.CopyTo((Stream) fileStream);
        }
        return true;
      }
    }

    public static void SaveAllFiles(string zipPath, string destinationDirectory)
    {
      using (ZipArchive zipArchive = ZipHelper.SafeOpenFile(zipPath, true))
      {
        foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
        {
          string path = PathHelper.ResolveRelativePath(destinationDirectory, zipArchiveEntry.FullName);
          string directory = PathHelper.GetDirectory(path);
          if (!PathHelper.DirectoryExists(directory))
            Directory.CreateDirectory(directory);
          using (Stream stream = zipArchiveEntry.Open())
          {
            using (FileStream fileStream = File.OpenWrite(path))
              stream.CopyTo((Stream) fileStream);
          }
        }
      }
    }

    public static IEnumerable<string> GetFileNames(string zipPath)
    {
      using (ZipArchive zipArchive = ZipHelper.SafeOpenFile(zipPath, true))
      {
        ReadOnlyCollection<ZipArchiveEntry> entries = zipArchive.Entries;
        List<string> list = new List<string>(entries.Count);
        foreach (ZipArchiveEntry zipArchiveEntry in entries)
          list.Add(zipArchiveEntry.FullName);
        return (IEnumerable<string>) list;
      }
    }

    private static ZipArchive SafeOpenFile(string zipPath, bool readOnly = true)
    {
      FileStream fileStream = new FileStream(zipPath, FileMode.Open, readOnly ? FileAccess.Read : FileAccess.ReadWrite);
      try
      {
        return new ZipArchive((Stream) fileStream);
      }
      catch
      {
        fileStream.Dispose();
        throw;
      }
    }

    private static ZipArchiveEntry GetEntryCaseInsensitive(this ZipArchive zipArchive, string entryName)
    {
      foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
      {
        if (string.Equals(zipArchiveEntry.FullName, entryName, StringComparison.OrdinalIgnoreCase))
          return zipArchiveEntry;
      }
      return (ZipArchiveEntry) null;
    }

    private class ZipSubStream : Stream
    {
      private ZipArchive zipArchive;
      private Stream zipFileStream;

      public override bool CanRead
      {
        get
        {
          return this.zipFileStream.CanRead;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return this.zipFileStream.CanSeek;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return this.zipFileStream.CanWrite;
        }
      }

      public override long Length
      {
        get
        {
          return this.zipFileStream.Length;
        }
      }

      public override long Position
      {
        get
        {
          return this.zipFileStream.Position;
        }
        set
        {
          this.zipFileStream.Position = value;
        }
      }

      private ZipSubStream(ZipArchive zipArchive, Stream zipFileStream)
      {
        this.zipArchive = zipArchive;
        this.zipFileStream = zipFileStream;
      }

      public static ZipHelper.ZipSubStream Create(string zipPath, string fileName)
      {
        ZipArchive zipArchive = ZipHelper.SafeOpenFile(zipPath, true);
        ZipArchiveEntry entryCaseInsensitive;
        try
        {
          entryCaseInsensitive = ZipHelper.GetEntryCaseInsensitive(zipArchive, fileName);
        }
        catch
        {
          zipArchive.Dispose();
          throw;
        }
        if (entryCaseInsensitive != null)
          return new ZipHelper.ZipSubStream(zipArchive, entryCaseInsensitive.Open());
        zipArchive.Dispose();
        return (ZipHelper.ZipSubStream) null;
      }

      public override void Flush()
      {
        this.zipFileStream.Flush();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return this.zipFileStream.Read(buffer, offset, count);
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return this.zipFileStream.Seek(offset, origin);
      }

      public override void SetLength(long value)
      {
        this.zipFileStream.SetLength(value);
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.zipFileStream.Write(buffer, offset, count);
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          using (Interlocked.Exchange<ZipArchive>(ref this.zipArchive, (ZipArchive) null))
            ;
        }
        base.Dispose(disposing);
      }
    }
  }
}

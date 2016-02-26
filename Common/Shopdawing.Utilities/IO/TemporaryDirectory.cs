// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.TemporaryDirectory
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

namespace Microsoft.Expression.Utility.IO
{
  [DebuggerDisplay("{Path}")]
  public sealed class TemporaryDirectory : IDisposable
  {
    public string Path { get; private set; }

    public TemporaryDirectory()
      : this(false, false, (DirectorySecurity) null)
    {
    }

    public TemporaryDirectory(bool throwIOError)
      : this(throwIOError, false, (DirectorySecurity) null)
    {
    }

    public TemporaryDirectory(bool throwIOError, bool noTrailing8Dot3FolderStructure)
      : this(throwIOError, noTrailing8Dot3FolderStructure, (DirectorySecurity) null)
    {
    }

    public TemporaryDirectory(bool throwIOError, bool noTrailing8Dot3FolderStructure, DirectorySecurity security)
    {
      string path2 = System.IO.Path.GetRandomFileName();
      if (noTrailing8Dot3FolderStructure)
        path2 = path2.Replace(".", "_");
      this.Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), path2) + (object) System.IO.Path.DirectorySeparatorChar;
      try
      {
        Directory.CreateDirectory(this.Path, security);
      }
      catch (IOException ex)
      {
        if (throwIOError)
          throw;
        else
          this.Path = string.Empty;
      }
      catch (UnauthorizedAccessException ex)
      {
        if (throwIOError)
          throw;
        else
          this.Path = string.Empty;
      }
    }

    ~TemporaryDirectory()
    {
      this.Dispose(false);
    }

    public string GenerateTemporaryFileName()
    {
      return System.IO.Path.Combine(this.Path, System.IO.Path.GetRandomFileName());
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
      try
      {
        Directory.Delete(this.Path, true);
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public void SuppressFinalizerAssert()
    {
    }
  }
}

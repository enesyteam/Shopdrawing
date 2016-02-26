// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IO.DocumentReference
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.IO;
using System.Runtime;
using System.Text;

namespace Microsoft.Expression.Utility.IO
{
  public sealed class DocumentReference : IEquatable<DocumentReference>, IEquatable<string>
  {
    private const int MaxPreambleLength = 4;
    private readonly string path;
    private readonly int hash;
    private readonly Func<string, FileAccess, Stream> streamProvider;

    public string Path
    {
      get
      {
        return this.path;
      }
    }

    public bool IsValidPathFormat { get; private set; }

    public string DisplayName
    {
      get
      {
        return PathHelper.GetFileOrDirectoryName(this.Path);
      }
    }

    public string DisplayNameShort
    {
      get
      {
        string path = this.DisplayName;
        if (!PathHelper.IsDirectory(this.Path))
          path = System.IO.Path.GetFileNameWithoutExtension(path);
        if (path != null)
          return path.Trim();
        return path;
      }
    }

    private DocumentReference(string path, Func<string, FileAccess, Stream> streamProvider)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (streamProvider == null)
        throw new ArgumentNullException("streamProvider");
      this.IsValidPathFormat = PathHelper.ResolvePath(ref path);
      this.path = path;
      this.hash = PathHelper.GenerateHashFromPath(this);
      this.streamProvider = streamProvider;
    }

    private DocumentReference(string resolvedPath, bool isValid = true)
    {
      if (resolvedPath == null)
        throw new ArgumentNullException("resolvedPath");
      this.IsValidPathFormat = isValid;
      this.path = resolvedPath;
      this.hash = PathHelper.GenerateHashFromPath(this);
      this.streamProvider = new Func<string, FileAccess, Stream>(DocumentReference.GetStream);
    }

    public static bool operator ==(DocumentReference left, DocumentReference right)
    {
      if (object.ReferenceEquals((object) left, (object) right))
        return true;
      if (left == null || right == null)
        return false;
      return left.Equals(right);
    }

    public static bool operator !=(DocumentReference left, DocumentReference right)
    {
      return !(left == right);
    }

    public static DocumentReference Create(string path)
    {
      return new DocumentReference(path, new Func<string, FileAccess, Stream>(DocumentReference.GetStream));
    }

    public static DocumentReference CreateFromRelativePath(string rootPath, string relativePath)
    {
      return new DocumentReference(PathHelper.ResolveRelativePath(rootPath, relativePath), true);
    }

    public static DocumentReference Create(string path, Func<string, FileAccess, Stream> streamProvider)
    {
      return new DocumentReference(path, streamProvider);
    }

    public static DocumentReference TryCreateFromRelativePath(string rootPath, string relativePath)
    {
      if (rootPath == null || relativePath == null || PathHelper.IsPathRelative(rootPath))
        return (DocumentReference) null;
      if (PathHelper.ValidateAndFixPathIfPossible(ref relativePath))
        return new DocumentReference(PathHelper.ResolveRelativePath(rootPath, relativePath), true);
      return new DocumentReference(relativePath, false);
    }

    public string GetRelativePath(DocumentReference reference, bool useWebDirectorySeparatorChar = false)
    {
      string str = PathHelper.GetRelativePath(this, reference);
      if (str == null)
        return string.Empty;
      if (!useWebDirectorySeparatorChar)
        str = str.Replace('/', System.IO.Path.DirectorySeparatorChar);
      return str;
    }

    public bool Equals(DocumentReference other)
    {
      return PathHelper.ArePathsEquivalent(this, other);
    }

    public bool Equals(string other)
    {
      return PathHelper.ArePathsEquivalent(this, other);
    }

    public override sealed bool Equals(object obj)
    {
      return this.Equals(obj as DocumentReference);
    }

    public override int GetHashCode()
    {
      return this.hash;
    }

    public override string ToString()
    {
      return this.Path;
    }

    public Stream GetStream(FileAccess access)
    {
      return this.streamProvider(this.Path, access);
    }

    private static Stream GetStream(string path, FileAccess access)
    {
      if (PathHelper.IsDirectory(path))
        return (Stream) null;
      if (access == FileAccess.Read)
      {
        if (!PathHelper.FileExists(path))
          return (Stream) null;
        return (Stream) new FileStream(path, FileMode.Open, FileAccess.Read);
      }
      if (access == FileAccess.Write)
        return (Stream) new FileStream(path, FileMode.Create, FileAccess.Write);
      throw new NotImplementedException();
    }

    public static string ReadDocumentContents(Stream stream, out Encoding encoding)
    {
      if (stream.Length >= 500000000L)
        throw new FileLoadException();
      int capacity = 1024;
      if (stream.Length > (long) capacity && stream.Length < (long) int.MaxValue)
        capacity = (int) (stream.Length + 1L);
      byte[] numArray = new byte[4];
      stream.Read(numArray, 0, 4);
      stream.Seek(0L, SeekOrigin.Begin);
      encoding = DocumentReference.DetermineDocumentEncoding(numArray);
      try
      {
        using (new MemoryFailPoint(1 + capacity / 524288))
        {
          StringBuilder stringBuilder = new StringBuilder(capacity);
          using (StreamReader streamReader = new StreamReader(stream, encoding))
          {
            char[] buffer = new char[16384];
            int charCount;
            do
            {
              charCount = streamReader.Read(buffer, 0, 16384);
              stringBuilder.Append(buffer, 0, charCount);
            }
            while (charCount > 0);
          }
          return stringBuilder.ToString();
        }
      }
      catch (InsufficientMemoryException ex)
      {
        throw new FileLoadException(ex.Message, (Exception) ex);
      }
      catch (OutOfMemoryException ex)
      {
        throw new FileLoadException(ex.Message, (Exception) ex);
      }
    }

    public static Encoding DetermineDocumentEncoding(byte[] documentBuffer)
    {
      byte[] preamble1 = Encoding.UTF8.GetPreamble();
      byte[] preamble2 = Encoding.Unicode.GetPreamble();
      if (documentBuffer.Length > preamble1.Length && DocumentReference.AreBufferPrefixesEquivalent(preamble1, documentBuffer))
        return Encoding.UTF8;
      if (documentBuffer.Length > preamble2.Length && DocumentReference.AreBufferPrefixesEquivalent(preamble2, documentBuffer))
        return Encoding.Unicode;
      return Encoding.Default;
    }

    private static bool AreBufferPrefixesEquivalent(byte[] bufferOne, byte[] bufferTwo)
    {
      if (bufferOne.Length > bufferTwo.Length)
        return false;
      for (int index = 0; index < bufferOne.Length; ++index)
      {
        if ((int) bufferOne[index] != (int) bufferTwo[index])
          return false;
      }
      return true;
    }
  }
}

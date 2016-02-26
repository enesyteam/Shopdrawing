// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Clipboard.CopyItem
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.IO;

namespace Microsoft.Expression.Framework.Clipboard
{
  public abstract class CopyItem
  {
    private string key;
    private string contentType;
    private string filenameExtension;
    private Uri originalUri;
    private string localPath;

    public string ContentType
    {
      get
      {
        return this.contentType;
      }
      set
      {
        this.contentType = value;
      }
    }

    public string FilenameExtension
    {
      get
      {
        return this.filenameExtension;
      }
      set
      {
        this.filenameExtension = value;
      }
    }

    public Uri OriginalUri
    {
      get
      {
        return this.originalUri;
      }
      set
      {
        this.originalUri = value;
      }
    }

    public string LocalPath
    {
      get
      {
        return this.localPath;
      }
      set
      {
        this.localPath = value;
      }
    }

    public string Key
    {
      get
      {
        return this.key;
      }
      set
      {
        this.key = value;
      }
    }

    public abstract Stream GetStream();
  }
}

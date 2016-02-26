// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.DocumentChangedEventArgs
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Documents
{
  public class DocumentChangedEventArgs : EventArgs
  {
    private IDocument oldDocument;
    private IDocument newDocument;

    public IDocument OldDocument
    {
      get
      {
        return this.oldDocument;
      }
    }

    public IDocument NewDocument
    {
      get
      {
        return this.newDocument;
      }
    }

    public DocumentChangedEventArgs(IDocument oldDocument, IDocument newDocument)
    {
      this.oldDocument = oldDocument;
      this.newDocument = newDocument;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.DocumentViewReference
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Documents;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class DocumentViewReference
  {
    private IDocumentView documentView;

    public string Caption
    {
      get
      {
        return this.documentView.Caption;
      }
    }

    public string Path
    {
      get
      {
        return this.documentView.Document.DocumentReference.Path;
      }
    }

    public IDocumentView DocumentView
    {
      get
      {
        return this.documentView;
      }
    }

    public DocumentViewReference(IDocumentView documentView)
    {
      this.documentView = documentView;
    }
  }
}

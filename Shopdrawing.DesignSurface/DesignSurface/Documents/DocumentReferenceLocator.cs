// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.DocumentReferenceLocator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Framework.Documents;
using System.Diagnostics;

namespace Microsoft.Expression.DesignSurface.Documents
{
  [DebuggerDisplay("{Path}")]
  internal sealed class DocumentReferenceLocator : IDocumentLocator
  {
    private DocumentReference documentReference;

    public string Path
    {
      get
      {
        return this.documentReference.Path;
      }
    }

    internal DocumentReference DocumentReference
    {
      get
      {
        return this.documentReference;
      }
    }

    private DocumentReferenceLocator(DocumentReference documentReference)
    {
      this.documentReference = documentReference;
    }

    public static IDocumentLocator GetDocumentLocator(DocumentReference documentReference)
    {
      if (documentReference == (DocumentReference) null)
        return (IDocumentLocator) null;
      return (IDocumentLocator) new DocumentReferenceLocator(documentReference);
    }

    public static DocumentReference GetDocumentReference(IDocumentLocator documentLocator)
    {
      if (documentLocator == null)
        return (DocumentReference) null;
      DocumentReferenceLocator referenceLocator = documentLocator as DocumentReferenceLocator;
      if (referenceLocator != null)
        return referenceLocator.DocumentReference;
      IDocument document = documentLocator as IDocument;
      if (document != null)
        return document.DocumentReference;
      return DocumentReference.Create(documentLocator.Path);
    }

    public static DocumentReference GetDocumentReference(IDocumentContext documentContext)
    {
      DocumentContext documentContext1 = documentContext as DocumentContext;
      if (documentContext1 != null)
        return DocumentReferenceLocator.GetDocumentReference(documentContext1.DocumentLocator);
      return DocumentReference.Create(documentContext.DocumentUrl);
    }
  }
}

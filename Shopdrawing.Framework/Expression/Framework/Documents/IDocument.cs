// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.IDocument
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Documents
{
  public interface IDocument : INotifyPropertyChanged, IDisposable
  {
    string Caption { get; }

    DocumentReference DocumentReference { get; }

    bool IsReadOnly { get; }

    bool IsDirty { get; }

    IDocumentContainer Container { get; set; }

    event CancelEventHandler Saving;

    event EventHandler Saved;

    event EventHandler IsDirtyChanged;

    event EventHandler Renamed;

    bool Save();

    void ProjectItemChanged(DocumentReference documentReference);

    void ProjectItemRemoved(DocumentReference documentReference);

    void ProjectItemRenamed(DocumentReference oldReference, DocumentReference newReference);

    void SourceChanged();

    bool ReferencesDocument(DocumentReference documentReference);

    IDocumentView CreateDefaultView();
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.Document
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Documents
{
  public abstract class Document : CommandTarget, IDocument, INotifyPropertyChanged, IDisposable
  {
    private DocumentReference documentReference;
    private bool isReadOnly;
    private IDocumentContainer container;
    private bool sourceIsDirty;

    protected bool SourceIsDirty
    {
      get
      {
        return this.sourceIsDirty;
      }
      set
      {
        this.sourceIsDirty = value;
      }
    }

    public DocumentReference DocumentReference
    {
      get
      {
        return this.documentReference;
      }
    }

    public string Caption
    {
      get
      {
        return this.DocumentReference.DisplayName;
      }
    }

    public virtual bool IsReadOnly
    {
      get
      {
        return this.isReadOnly;
      }
    }

    public virtual bool IsDirty
    {
      get
      {
        return this.sourceIsDirty;
      }
    }

    public IDocumentContainer Container
    {
      get
      {
        return this.container;
      }
      set
      {
        this.container = value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler IsDirtyChanged;

    public event CancelEventHandler Saving;

    public event EventHandler Saved;

    public event EventHandler Renamed;

    protected Document(DocumentReference documentReference, bool isReadOnly)
    {
      this.documentReference = documentReference;
      this.isReadOnly = isReadOnly;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      this.documentReference = (DocumentReference) null;
    }

    public virtual IDocumentView CreateDefaultView()
    {
      return (IDocumentView) null;
    }

    public virtual bool ReferencesDocument(DocumentReference documentReference)
    {
      return false;
    }

    public abstract bool Save();

    public void SourceChanged()
    {
      bool isDirty = this.IsDirty;
      this.sourceIsDirty = true;
      if (isDirty == this.IsDirty)
        return;
      this.OnIsDirtyChanged(EventArgs.Empty);
    }

    public virtual void ProjectItemChanged(DocumentReference documentReference)
    {
    }

    public virtual void ProjectItemRemoved(DocumentReference documentReference)
    {
    }

    public virtual void ProjectItemRenamed(DocumentReference oldReference, DocumentReference newReference)
    {
      if (!(this.DocumentReference != (DocumentReference) null) || !this.DocumentReference.Equals(oldReference))
        return;
      this.documentReference = newReference;
      this.OnRenamed(EventArgs.Empty);
    }

    public override string ToString()
    {
      return this.DocumentReference.ToString();
    }

    protected virtual void OnIsDirtyChanged(EventArgs e)
    {
      if (this.IsDirtyChanged == null)
        return;
      this.IsDirtyChanged((object) this, e);
    }

    protected virtual void OnSaving(CancelEventArgs e)
    {
      if (this.Saving == null)
        return;
      this.Saving((object) this, e);
    }

    protected virtual void OnSaved(EventArgs e)
    {
      if (this.Saved == null)
        return;
      this.Saved((object) this, e);
    }

    protected virtual void OnRenamed(EventArgs e)
    {
      this.OnPropertyChanged(new PropertyChangedEventArgs("Caption"));
      if (this.Renamed == null)
        return;
      this.Renamed((object) this, e);
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }
  }
}

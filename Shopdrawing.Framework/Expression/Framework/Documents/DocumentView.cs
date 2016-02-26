// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.DocumentView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Documents
{
  public abstract class DocumentView : View, IDocumentView, IView, INotifyPropertyChanged, IDisposable
  {
    private IDocument document;

    public virtual IDocument Document
    {
      get
      {
        return this.document;
      }
    }

    public override string Caption
    {
      get
      {
        if (this.document == null)
          return string.Empty;
        return this.document.Caption;
      }
    }

    public override bool IsDirty
    {
      get
      {
        if (this.document == null)
          return false;
        return this.document.IsDirty;
      }
    }

    public override string TabToolTip
    {
      get
      {
        if (this.document == null)
          return string.Empty;
        return this.document.DocumentReference.Path;
      }
    }

    protected DocumentView(IDocument document)
    {
      this.document = document;
      this.document.PropertyChanged += new PropertyChangedEventHandler(this.Document_PropertyChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.document.PropertyChanged -= new PropertyChangedEventHandler(this.Document_PropertyChanged);
        this.document = (IDocument) null;
      }
      base.Dispose(disposing);
    }

    private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Caption":
          this.OnPropertyChanged(new PropertyChangedEventArgs("Caption"));
          break;
        case "IsDirty":
          this.OnPropertyChanged(new PropertyChangedEventArgs("IsDirty"));
          break;
        case "ToolTip":
          this.OnPropertyChanged(new PropertyChangedEventArgs("TabToolTip"));
          break;
      }
    }
  }
}

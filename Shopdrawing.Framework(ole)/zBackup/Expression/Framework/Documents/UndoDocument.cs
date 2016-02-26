// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.UndoDocument
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands.Undo;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Framework.Documents
{
  public abstract class UndoDocument : Document
  {
    private IUndoService undoService;

    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;
        if (this.UndoService != null)
          return this.UndoService.IsDirty;
        return false;
      }
    }

    protected IUndoService UndoService
    {
      get
      {
        return this.undoService;
      }
    }

    protected UndoDocument(DocumentReference documentReference, bool isReadOnly)
      : this(documentReference, (IUndoService) new Microsoft.Expression.Framework.Commands.Undo.UndoService(), isReadOnly)
    {
    }

    protected UndoDocument(DocumentReference documentReference, IUndoService undoService, bool isReadOnly)
      : base(documentReference, isReadOnly)
    {
      this.undoService = undoService;
      this.UndoService.UndoStackChanged += new UndoStackChangedEventHandler(this.UndoService_UndoStackChanged);
    }

    protected override void Dispose(bool disposing)
    {
      this.undoService = (IUndoService) null;
      base.Dispose(disposing);
    }

    public override bool Save()
    {
      CancelEventArgs e = new CancelEventArgs(false);
      this.OnSaving(e);
      if (!e.Cancel)
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SaveDocument, this.DocumentReference.Path);
        bool saveSucceeded = false;
        string str = this.DocumentReference.Path + ".temporary";
        if (PathHelper.FileExists(this.DocumentReference.Path))
        {
          if ((File.GetAttributes(this.DocumentReference.Path) & FileAttributes.ReadOnly) != (FileAttributes) 0)
            throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.DocumentIsReadOnly, new object[1]
            {
              (object) this.DocumentReference.Path
            }));
        }
        try
        {
          DirectoryInfo directory = new FileInfo(str).Directory;
          if (!PathHelper.DirectoryExists(Path.GetDirectoryName(str)))
            directory.Create();
          PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "SaveCore");
          using (Stream stream = (Stream) new FileStream(str, FileMode.Create, FileAccess.Write))
          {
            this.SaveCore(stream);
            saveSucceeded = true;
          }
        }
        finally
        {
          if (this.Container != null)
          {
            PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "BeginDocumentSave");
            this.Container.BeginDocumentSave((IDocument) this);
          }
          try
          {
            if (saveSucceeded)
            {
              bool flag = false;
              DateTime creationTime = DateTime.Now;
              try
              {
                if (PathHelper.FileExists(this.DocumentReference.Path))
                {
                  PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "Delete document");
                  creationTime = File.GetCreationTime(this.DocumentReference.Path);
                  flag = true;
                  File.SetAttributes(this.DocumentReference.Path, FileAttributes.Normal);
                  File.Delete(this.DocumentReference.Path);
                }
              }
              catch (IOException ex)
              {
              }
              PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "Move");
              File.Move(str, this.DocumentReference.Path);
              if (flag)
                File.SetCreationTime(this.DocumentReference.Path, creationTime);
            }
            else
            {
              PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "Delete temporary");
              if (PathHelper.FileExists(str))
                File.Delete(str);
            }
          }
          finally
          {
            if (this.Container != null)
            {
              PerformanceUtility.MarkInterimStep(PerformanceEvent.SaveDocument, "DocumentSaveCompleted");
              this.Container.DocumentSaveCompleted((IDocument) this, saveSucceeded);
            }
          }
        }
        this.SourceIsDirty = false;
        this.UndoService.SetClean();
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SaveDocument, this.DocumentReference.Path);
        this.OnSaved(EventArgs.Empty);
        this.OnIsDirtyChanged(EventArgs.Empty);
      }
      return !e.Cancel;
    }

    protected abstract void SaveCore(Stream stream);

    private void UndoService_UndoStackChanged(object sender, UndoStackChangedEventArgs e)
    {
      this.OnIsDirtyChanged(EventArgs.Empty);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Pipeline.SceneEditTransaction
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands.Undo;
using System;

namespace Microsoft.Expression.DesignSurface.Pipeline
{
  public class SceneEditTransaction : IDisposable
  {
    private SceneDocument document;
    private IDisposable delayExternalChanges;
    private IUndoTransaction undo;
    private bool isDisposed;

    internal SceneEditTransaction(IExternalChanges externalChanges, SceneDocument document, IUndoTransaction undo)
    {
      this.document = document;
      this.delayExternalChanges = externalChanges != null ? externalChanges.DelayNotification() : (IDisposable) null;
      this.undo = undo;
    }

    public void Commit()
    {
      this.Commit(true);
    }

    public void Commit(bool notifyDocument)
    {
      if (this.undo == null)
        return;
      try
      {
        if (notifyDocument)
          this.document.OnCompletingEditTransaction();
        this.undo.Commit();
        this.undo = (IUndoTransaction) null;
        this.document.OnCompletedEditTransaction(notifyDocument);
      }
      finally
      {
        this.EnableExternalChanges();
      }
    }

    public void Cancel()
    {
      this.Cancel(true);
    }

    internal void Cancel(bool notifyDocument)
    {
      if (this.undo == null)
        return;
      try
      {
        this.undo.Cancel();
        this.undo = (IUndoTransaction) null;
        this.document.OnCanceledEditTransaction(notifyDocument);
      }
      finally
      {
        this.EnableExternalChanges();
      }
    }

    public void Update()
    {
      this.document.OnUpdatedEditTransaction();
    }

    public void AddUndoUnit(IUndoUnit undoUnit)
    {
      this.document.AddUndoUnit(undoUnit);
    }

    public void Dispose()
    {
      if (!this.isDisposed)
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
      this.isDisposed = true;
    }

    protected virtual void Dispose(bool isDisposing)
    {
      if (!isDisposing)
        return;
      try
      {
        if (this.undo == null)
          return;
        this.Cancel();
      }
      finally
      {
        this.EnableExternalChanges();
      }
    }

    private void EnableExternalChanges()
    {
      if (this.delayExternalChanges == null)
        return;
      this.delayExternalChanges.Dispose();
      this.delayExternalChanges = (IDisposable) null;
    }
  }
}

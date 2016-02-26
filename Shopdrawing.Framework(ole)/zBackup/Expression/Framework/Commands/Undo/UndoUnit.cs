// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.Undo.UndoUnit
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Commands.Undo
{
  public abstract class UndoUnit : MarshalByRefObject, IUndoUnit, IDisposable
  {
    private UndoUnit.UndoState state;

    public virtual bool CanUndo
    {
      get
      {
        return this.state == UndoUnit.UndoState.Redo;
      }
    }

    public virtual bool CanRedo
    {
      get
      {
        return this.state == UndoUnit.UndoState.Undo;
      }
    }

    public virtual bool IsHidden
    {
      get
      {
        return false;
      }
    }

    public virtual bool AllowsDeepMerge
    {
      get
      {
        return false;
      }
    }

    public virtual int MergeKey
    {
      get
      {
        return 0;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public virtual void Undo()
    {
      this.state = UndoUnit.UndoState.Undo;
    }

    public virtual void Redo()
    {
      this.state = UndoUnit.UndoState.Redo;
    }

    public virtual UndoUnitMergeResult Merge(IUndoUnit otherUnit, out IUndoUnit mergedUnit)
    {
      mergedUnit = (IUndoUnit) null;
      return UndoUnitMergeResult.CouldNotMerge;
    }

    private enum UndoState
    {
      None,
      Undo,
      Redo,
    }
  }
}

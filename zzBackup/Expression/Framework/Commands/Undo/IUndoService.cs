// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.Undo.IUndoService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.Commands.Undo
{
  public interface IUndoService
  {
    bool IsEnabled { get; set; }

    string UndoDescription { get; }

    string RedoDescription { get; }

    bool CanUndo { get; }

    bool CanRedo { get; }

    bool IsUndoing { get; }

    bool IsRedoing { get; }

    bool IsDirty { get; }

    event UndoStackChangedEventHandler UndoStackChanged;

    event UndoStackChangedEventHandler RedoStackChanged;

    IUndoTransaction CreateUndo(string description);

    IUndoTransaction CreateUndo(string description, bool isHidden);

    void Add(IUndoUnit unit);

    void Clear();

    void Undo();

    void Redo();

    void SetClean();
  }
}

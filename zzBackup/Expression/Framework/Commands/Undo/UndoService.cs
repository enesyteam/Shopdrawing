// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.Undo.UndoService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Framework.Commands.Undo
{
  public sealed class UndoService : IUndoService
  {
    private Stack<IUndoUnitContainer> openStack = new Stack<IUndoUnitContainer>();
    private Stack<IUndoUnitContainer> undoStack = new Stack<IUndoUnitContainer>();
    private Stack<IUndoUnitContainer> redoStack = new Stack<IUndoUnitContainer>();
    private bool isEnabled = true;
    private int openStackDepth;
    private object cleanMarker;
    private bool isUndoing;
    private bool isRedoing;
    private bool adding;

    public bool IsEnabled
    {
      get
      {
        return this.isEnabled;
      }
      set
      {
        this.isEnabled = value;
      }
    }

    public string UndoDescription
    {
      get
      {
        IUndoUnitContainer undoUnitContainer = this.FirstNonHiddenContainer(this.undoStack);
        if (undoUnitContainer == null)
          return string.Empty;
        return undoUnitContainer.Description;
      }
    }

    public string RedoDescription
    {
      get
      {
        IUndoUnitContainer undoUnitContainer = this.FirstNonHiddenContainer(this.redoStack);
        if (undoUnitContainer == null)
          return string.Empty;
        return undoUnitContainer.Description;
      }
    }

    public bool CanUndo
    {
      get
      {
        if (this.openStackDepth == 0)
          return this.FirstNonHiddenContainer(this.undoStack) != null;
        return false;
      }
    }

    public bool CanRedo
    {
      get
      {
        if (this.openStackDepth == 0)
          return this.FirstNonHiddenContainer(this.redoStack) != null;
        return false;
      }
    }

    public bool IsUndoing
    {
      get
      {
        return this.isUndoing;
      }
    }

    public bool IsRedoing
    {
      get
      {
        return this.isRedoing;
      }
    }

    public bool IsDirty
    {
      get
      {
        if (this.undoStack.Count == 0 && this.openStack.Count == this.openStackDepth)
          return this.cleanMarker != null;
        return this.cleanMarker != this.FirstNonHiddenContainer(this.undoStack);
      }
    }

    public event UndoStackChangedEventHandler UndoStackChanged;

    public event UndoStackChangedEventHandler RedoStackChanged;

    public IUndoTransaction CreateUndo(string description)
    {
      return this.CreateUndo(description, false);
    }

    public IUndoTransaction CreateUndo(string description, bool isHidden)
    {
      return (IUndoTransaction) new UndoService.UndoTransaction(this, description, isHidden);
    }

    public void Add(IUndoUnit unit)
    {
      if (this.adding)
        throw new InvalidOperationException(ExceptionStringTable.CannotNestAdds);
      if (this.isEnabled)
      {
        if (this.openStackDepth <= 0)
          throw new InvalidOperationException(ExceptionStringTable.CannotAddAtomicUnitToUndoManager);
        this.adding = true;
        try
        {
          unit.Redo();
          this.openStack.Peek().Add(unit);
        }
        finally
        {
          this.adding = false;
        }
      }
      else
        unit.Redo();
    }

    public void Clear()
    {
      if (this.openStackDepth != 0)
        throw new InvalidOperationException(ExceptionStringTable.CannotClearFromWithinAnOpenUndoContainer);
      bool isDirty = this.IsDirty;
      this.openStackDepth = 0;
      this.openStack.Clear();
      this.ClearRedoStack();
      this.ClearUndoStack();
      this.cleanMarker = isDirty ? new object() : (object) null;
      this.OnRedoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.StackCleared));
      this.OnUndoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.StackCleared));
    }

    public void Undo()
    {
      this.isUndoing = true;
      try
      {
        if (this.openStackDepth != 0)
          throw new InvalidOperationException(ExceptionStringTable.CannotUndoFromWithinAnOpenUndoContainer);
        this.UndoHiddenContainersOnOpenStack();
        if (this.undoStack.Count <= 0)
          throw new InvalidOperationException(ExceptionStringTable.UndoStackIsEmpty);
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.Undo);
        IUndoUnitContainer undoUnitContainer;
        do
        {
          undoUnitContainer = this.undoStack.Pop();
          this.OnUndoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.NodeRemoved));
          undoUnitContainer.Undo();
          this.redoStack.Push(undoUnitContainer);
          this.OnRedoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.NodeAdded));
        }
        while (undoUnitContainer.IsHidden && this.undoStack.Count > 0);
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.Undo);
      }
      finally
      {
        this.isUndoing = false;
      }
    }

    public void Redo()
    {
      this.isRedoing = true;
      try
      {
        if (this.openStackDepth != 0)
          throw new InvalidOperationException(ExceptionStringTable.CannotRedoFromWithinAnOpenUndoContainer);
        this.UndoHiddenContainersOnOpenStack();
        if (this.redoStack.Count <= 0)
          throw new InvalidOperationException(ExceptionStringTable.RedoStackIsEmpty);
        IUndoUnitContainer undoUnitContainer;
        do
        {
          undoUnitContainer = this.redoStack.Pop();
          this.OnRedoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.NodeRemoved));
          undoUnitContainer.Redo();
          this.undoStack.Push(undoUnitContainer);
          this.OnUndoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.NodeAdded));
        }
        while (undoUnitContainer.IsHidden && this.redoStack.Count > 0);
      }
      finally
      {
        this.isRedoing = false;
      }
    }

    public void SetClean()
    {
      this.cleanMarker = (object) this.FirstNonHiddenContainer(this.undoStack);
    }

    public override string ToString()
    {
      using (StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        UndoService.WriteStack(writer, this.openStack, "[Open]");
        UndoService.WriteStack(writer, this.redoStack, "[Redo]");
        UndoService.WriteStack(writer, this.undoStack, "[Undo]");
        return writer.ToString();
      }
    }

    private void OnUndoStackChanged(UndoStackChangedEventArgs e)
    {
      if (this.UndoStackChanged == null)
        return;
      this.UndoStackChanged((object) this, e);
    }

    private void OnRedoStackChanged(UndoStackChangedEventArgs e)
    {
      if (this.RedoStackChanged == null)
        return;
      this.RedoStackChanged((object) this, e);
    }

    private IUndoUnitContainer Begin(string description, bool isHidden)
    {
      UndoUnitContainer undoUnitContainer = new UndoUnitContainer(description, isHidden);
      this.openStack.Push((IUndoUnitContainer) undoUnitContainer);
      ++this.openStackDepth;
      return (IUndoUnitContainer) undoUnitContainer;
    }

    private void End(IUndoUnitContainer container)
    {
      this.PopAndValidate(container);
      if (container.IsEmpty)
        return;
      if (this.openStackDepth != 0)
        this.openStack.Peek().Add((IUndoUnit) container);
      else if (container.IsHidden)
      {
        if (this.openStack.Count > 0)
        {
          IUndoUnitContainer undoUnitContainer = this.openStack.Peek();
          if (undoUnitContainer != this.cleanMarker)
          {
            undoUnitContainer.IsClosed = false;
            undoUnitContainer.Add((IUndoUnit) container);
            undoUnitContainer.IsClosed = true;
          }
          else
            this.openStack.Push(container);
        }
        else
          this.openStack.Push(container);
      }
      else
      {
        IUndoUnitContainer[] undoUnitContainerArray = this.openStack.ToArray();
        for (int index = undoUnitContainerArray.Length - 1; index >= 0; --index)
          this.undoStack.Push(undoUnitContainerArray[index]);
        this.openStack.Clear();
        this.undoStack.Push(container);
        this.OnUndoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.NodeAdded));
        this.ClearRedoStack();
        this.OnRedoStackChanged(new UndoStackChangedEventArgs(UndoStackChangeType.StackCleared));
      }
    }

    private void ClearUndoStack()
    {
      while (this.undoStack.Count > 0)
        this.undoStack.Pop().Dispose();
    }

    private void ClearRedoStack()
    {
      while (this.redoStack.Count > 0)
        this.redoStack.Pop().Dispose();
    }

    private void Cancel(IUndoUnitContainer container)
    {
      this.PopAndValidate(container);
      container.Undo();
    }

    private void PopAndValidate(IUndoUnitContainer container)
    {
      if (this.openStackDepth == 0)
        throw new InvalidOperationException(ExceptionStringTable.CannotEndWithoutOpenUndoContainer);
      IUndoUnitContainer undoUnitContainer = this.openStack.Peek();
      if (container != undoUnitContainer)
        throw new InvalidOperationException(ExceptionStringTable.CannotEndUndoContainersOutOfOrder);
      container.IsClosed = true;
      this.openStack.Pop();
      --this.openStackDepth;
    }

    private IUndoUnitContainer FirstNonHiddenContainer(Stack<IUndoUnitContainer> stack)
    {
      IUndoUnitContainer undoUnitContainer1 = (IUndoUnitContainer) null;
      foreach (IUndoUnitContainer undoUnitContainer2 in stack)
      {
        if (!undoUnitContainer2.IsHidden)
        {
          undoUnitContainer1 = undoUnitContainer2;
          break;
        }
      }
      return undoUnitContainer1;
    }

    private void UndoHiddenContainersOnOpenStack()
    {
      while (this.openStack.Count > 0)
      {
        IUndoUnitContainer undoUnitContainer = this.openStack.Pop();
        undoUnitContainer.Undo();
        undoUnitContainer.Dispose();
      }
    }

    private static void WriteStack(StringWriter writer, Stack<IUndoUnitContainer> stack, string prefix)
    {
      if (stack.Count <= 0)
        return;
      writer.WriteLine(prefix);
      foreach (object obj in stack)
      {
        using (StringReader stringReader = new StringReader(obj.ToString()))
        {
          while (stringReader.Peek() != -1)
          {
            string str = stringReader.ReadLine();
            writer.Write("\t");
            writer.WriteLine(str);
          }
        }
      }
    }

    private sealed class UndoTransaction : IUndoTransaction, IDisposable
    {
      private UndoService undoService;
      private IUndoUnitContainer container;

      public UndoTransaction(UndoService undoService, string description, bool isHidden)
      {
        this.undoService = undoService;
        this.container = this.undoService.Begin(description, isHidden);
      }

      public void Commit()
      {
        this.CheckNotClosed();
        try
        {
          this.undoService.End(this.container);
        }
        finally
        {
          this.container = (IUndoUnitContainer) null;
        }
      }

      public void Cancel()
      {
        this.CheckNotClosed();
        try
        {
          this.undoService.Cancel(this.container);
        }
        finally
        {
          this.container = (IUndoUnitContainer) null;
        }
      }

      public void Dispose()
      {
        if (this.container == null)
          return;
        this.Cancel();
      }

      private void CheckNotClosed()
      {
        if (this.container == null)
          throw new InvalidOperationException(ExceptionStringTable.CannotEndClosedUndoContainer);
      }
    }
  }
}

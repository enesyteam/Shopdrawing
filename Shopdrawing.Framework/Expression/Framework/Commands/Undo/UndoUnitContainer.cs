// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.Undo.UndoUnitContainer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.Framework.Commands.Undo
{
  internal class UndoUnitContainer : UndoUnit, IUndoUnitContainer, IUndoUnit, IDisposable
  {
    private Dictionary<int, List<IUndoUnit>> undoDictionary = new Dictionary<int, List<IUndoUnit>>();
    private List<IUndoUnit> units = new List<IUndoUnit>();
    private string description;
    private bool isHidden;
    private bool isClosed;

    public string Description
    {
      get
      {
        return this.description;
      }
    }

    public bool IsEmpty
    {
      get
      {
        return this.units.Count == 0;
      }
    }

    public override bool IsHidden
    {
      get
      {
        bool flag = this.isHidden;
        if (!flag)
        {
          flag = true;
          foreach (IUndoUnit undoUnit in this.units)
          {
            if (!undoUnit.IsHidden)
            {
              flag = false;
              break;
            }
          }
        }
        return flag;
      }
    }

    public bool IsClosed
    {
      get
      {
        return this.isClosed;
      }
      set
      {
        this.isClosed = value;
        if (!this.isClosed)
          return;
        this.isHidden = this.IsHidden;
      }
    }

    public UndoUnitContainer(string description)
      : this(description, false)
    {
    }

    public UndoUnitContainer(string description, bool isHidden)
    {
      this.description = description;
      this.isHidden = isHidden;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (IDisposable disposable in this.units)
          disposable.Dispose();
        this.undoDictionary.Clear();
        this.units.Clear();
      }
      base.Dispose(disposing);
    }

    public override void Undo()
    {
      this.units.Reverse();
      foreach (IUndoUnit undoUnit in this.units)
        undoUnit.Undo();
      base.Undo();
    }

    public override void Redo()
    {
      this.units.Reverse();
      foreach (IUndoUnit undoUnit in this.units)
        undoUnit.Redo();
      base.Redo();
    }

    public void Add(IUndoUnit unit)
    {
      if (this.isClosed)
        throw new InvalidOperationException(ExceptionStringTable.CannotAddUndoUnitsToAnUndoContainerThatIsAlreadyClosed);
      if (this.CanRedo || this.CanUndo)
        throw new InvalidOperationException(ExceptionStringTable.CannotModifySealedUndoUnitContainer);
      UndoUnitContainer undoUnitContainer;
      if ((undoUnitContainer = unit as UndoUnitContainer) != null)
      {
        foreach (IUndoUnit unit1 in undoUnitContainer.units)
          this.Add(unit1);
      }
      else
      {
        bool flag = false;
        bool allowsDeepMerge = unit.AllowsDeepMerge;
        List<IUndoUnit> list;
        if (this.undoDictionary.TryGetValue(unit.MergeKey, out list))
        {
          for (int index = list.Count - 1; index >= 0; --index)
          {
            IUndoUnit undoUnit = list[index];
            IUndoUnit mergedUnit;
            switch (undoUnit.Merge(unit, out mergedUnit))
            {
              case UndoUnitMergeResult.MergedIntoNothing:
                this.units.Remove(undoUnit);
                list.RemoveAt(index);
                flag = true;
                goto label_19;
              case UndoUnitMergeResult.MergedIntoOneUnit:
                list[index] = mergedUnit;
                this.units[this.units.IndexOf(undoUnit)] = mergedUnit;
                flag = true;
                goto label_19;
              default:
                if (allowsDeepMerge && undoUnit.AllowsDeepMerge)
                  continue;
                goto label_19;
            }
          }
        }
label_19:
        if (!allowsDeepMerge)
        {
          this.undoDictionary.Clear();
          list = (List<IUndoUnit>) null;
        }
        if (flag)
          return;
        if (list == null)
        {
          list = new List<IUndoUnit>();
          this.undoDictionary[unit.MergeKey] = list;
        }
        list.Add(unit);
        this.units.Add(unit);
      }
    }

    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        stringWriter.WriteLine("Container(\"" + this.Description + "\")" + (this.IsHidden ? " [hidden]" : ""));
        for (int index = this.units.Count - 1; index >= 0; --index)
        {
          using (StringReader stringReader = new StringReader(this.units[index].ToString()))
          {
            while (stringReader.Peek() != -1)
            {
              string str = stringReader.ReadLine();
              stringWriter.Write("\t");
              stringWriter.WriteLine(str);
            }
          }
        }
        return stringWriter.ToString();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.CommandCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;

namespace Microsoft.Expression.Framework.Commands
{
  internal class CommandCollection : ICommandCollection, ICollection, IEnumerable
  {
    private ArrayList list = new ArrayList();

    public int Count
    {
      get
      {
        return this.list.Count;
      }
    }

    public string this[int index]
    {
      get
      {
        return (string) this.list[index];
      }
    }

    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public object SyncRoot
    {
      get
      {
        return (object) null;
      }
    }

    public IEnumerator GetEnumerator()
    {
      return this.list.GetEnumerator();
    }

    public void Add(string command)
    {
      if (this.list.Contains((object) command))
        return;
      this.list.Add((object) command);
    }

    public void Remove(string command)
    {
      while (this.list.Contains((object) command))
        this.list.Remove((object) command);
    }

    public bool Contains(string command)
    {
      return this.list.Contains((object) command);
    }

    public void CopyTo(Array array, int index)
    {
      this.list.CopyTo(array, index);
    }

    public void CopyTo(string[] array, int index)
    {
      this.list.CopyTo((Array) array, index);
    }
  }
}

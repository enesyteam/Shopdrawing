// Decompiled with JetBrains decompiler
// Type: MS.Internal.Identifier
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System.Collections;
using System.Collections.Generic;

namespace MS.Internal
{
  internal struct Identifier
  {
    private static Hashtable _identifiers = new Hashtable(1024);
    private static List<string> _names = new List<string>(1024);
    private static int _nextId = 1;
    public static Identifier Undefined = new Identifier();
    private const int DEFAULT_TABLE_SIZE = 1024;
    public readonly int UniqueId;

    public string Name
    {
      get
      {
        return Identifier._names[this.UniqueId];
      }
    }

    public bool IsDefined
    {
      get
      {
        return this.UniqueId != 0;
      }
    }

    static Identifier()
    {
      Identifier._names.Add("<Unknown>");
    }

    private Identifier(int id)
    {
      this.UniqueId = id;
    }

    public static implicit operator string(Identifier identifier)
    {
      return identifier.Name;
    }

    public static bool operator ==(Identifier one, Identifier two)
    {
      return one.UniqueId == two.UniqueId;
    }

    public static bool operator !=(Identifier one, Identifier two)
    {
      return one.UniqueId != two.UniqueId;
    }

    public static Identifier For(string name)
    {
      if (name == null)
        name = string.Empty;
      object obj = Identifier._identifiers[(object) name];
      if (obj == null)
      {
        lock (Identifier._identifiers)
        {
          obj = Identifier._identifiers[(object) name];
          if (obj == null)
          {
            obj = (object) new Identifier(Identifier._nextId++);
            Identifier._names.Add(name);
            Identifier._identifiers[(object) name] = obj;
          }
        }
      }
      return (Identifier) obj;
    }

    public override bool Equals(object other)
    {
      if (other is Identifier)
        return this.UniqueId == ((Identifier) other).UniqueId;
      return false;
    }

    public override int GetHashCode()
    {
      return this.UniqueId;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}

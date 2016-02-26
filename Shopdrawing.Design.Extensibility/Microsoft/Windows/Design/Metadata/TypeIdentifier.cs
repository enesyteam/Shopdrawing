// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.TypeIdentifier
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using System;

namespace Microsoft.Windows.Design.Metadata
{
  public struct TypeIdentifier : IEquatable<TypeIdentifier>
  {
    private Identifier _xmlNamespace;
    private Identifier _name;
    private int _hashCode;

    public bool IsEmpty
    {
      get
      {
        return !this._name.IsDefined;
      }
    }

    public string Name
    {
      get
      {
        if (!this._name.IsDefined)
          return (string) null;
        return this._name.Name;
      }
    }

    internal string SimpleName
    {
      get
      {
        if (this._xmlNamespace.IsDefined)
          return this._name.Name;
        string str = this._name.Name;
        int length = str.IndexOf(',');
        if (length >= 0)
          str = str.Substring(0, length);
        int num = str.LastIndexOf('.');
        if (num >= 0)
          str = str.Substring(num + 1);
        return str.Trim();
      }
    }

    public string XmlNamespace
    {
      get
      {
        if (!this._xmlNamespace.IsDefined)
          return (string) null;
        return this._xmlNamespace.Name;
      }
    }

    public TypeIdentifier(string xmlNamespace, string name)
    {
      if (xmlNamespace == null)
        throw new ArgumentNullException("xmlNamespace");
      if (name == null)
        throw new ArgumentNullException("name");
      this._xmlNamespace = Identifier.For(xmlNamespace);
      this._name = Identifier.For(name);
      this._hashCode = this._xmlNamespace.GetHashCode() ^ this._name.GetHashCode();
    }

    public TypeIdentifier(string fullyQualifiedName)
    {
      if (fullyQualifiedName == null)
        throw new ArgumentNullException("fullyQualifiedName");
      this._xmlNamespace = Identifier.Undefined;
      this._name = Identifier.For(fullyQualifiedName);
      this._hashCode = this._name.GetHashCode();
    }

    public static bool operator ==(TypeIdentifier first, TypeIdentifier second)
    {
      return first.Equals(second);
    }

    public static bool operator !=(TypeIdentifier first, TypeIdentifier second)
    {
      return !first.Equals(second);
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override bool Equals(object obj)
    {
      if (obj is TypeIdentifier)
        return this.Equals((TypeIdentifier) obj);
      return false;
    }

    public bool Equals(TypeIdentifier other)
    {
      if (this._xmlNamespace == other._xmlNamespace)
        return this._name == other._name;
      return false;
    }

    public override string ToString()
    {
      return this._name.Name;
    }
  }
}

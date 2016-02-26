// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Metadata.EventIdentifier
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using MS.Internal;
using System;

namespace Microsoft.Windows.Design.Metadata
{
  public struct EventIdentifier : IEquatable<EventIdentifier>
  {
    private Type _declaringType;
    private TypeIdentifier _declaringTypeId;
    private Identifier _name;

    public Type DeclaringType
    {
      get
      {
        return this._declaringType;
      }
    }

    public TypeIdentifier DeclaringTypeIdentifier
    {
      get
      {
        return this._declaringTypeId;
      }
    }

    public string FullName
    {
      get
      {
        if (this._name.IsDefined)
          return (this._declaringType != null ? this._declaringType.Name : this._declaringTypeId.SimpleName) + "." + this._name.Name;
        return string.Empty;
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

    public bool IsEmpty
    {
      get
      {
        return !this._name.IsDefined;
      }
    }

    public EventIdentifier(Type declaringType, string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (declaringType == null)
        throw new ArgumentNullException("declaringType");
      this._declaringType = declaringType;
      this._declaringTypeId = new TypeIdentifier();
      this._name = Identifier.For(name);
    }

    public EventIdentifier(TypeIdentifier declaringTypeId, string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (declaringTypeId.IsEmpty)
        throw new ArgumentNullException("declaringTypeId");
      this._declaringType = (Type) null;
      this._declaringTypeId = declaringTypeId;
      this._name = Identifier.For(name);
    }

    public static bool operator ==(EventIdentifier first, EventIdentifier second)
    {
      return first.Equals(second);
    }

    public static bool operator !=(EventIdentifier first, EventIdentifier second)
    {
      return !first.Equals(second);
    }

    public override int GetHashCode()
    {
      return (this._declaringType != null ? this._declaringType.GetHashCode() : this._declaringTypeId.GetHashCode()) ^ this._name.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if (obj is EventIdentifier)
        return this.Equals((EventIdentifier) obj);
      return false;
    }

    public bool Equals(EventIdentifier other)
    {
      if (this._declaringType != null)
      {
        if (this._declaringType == other._declaringType)
          return this._name == other._name;
        return false;
      }
      if (this._declaringTypeId.Equals(other._declaringTypeId))
        return this._name == other._name;
      return false;
    }

    public override string ToString()
    {
      return this.FullName;
    }
  }
}

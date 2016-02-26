// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.OrderToken
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design
{
  public abstract class OrderToken : IComparable<OrderToken>
  {
    private readonly OrderToken _reference;
    private readonly OrderTokenPrecedence _precedence;
    private readonly OrderTokenConflictResolution _conflictResolution;
    private readonly int _depth;
    private readonly int _index;
    private int _nextChildIndex;

    protected OrderToken(OrderTokenPrecedence precedence, OrderToken reference, OrderTokenConflictResolution conflictResolution)
    {
      if (!EnumValidator.IsValid(precedence))
        throw new ArgumentOutOfRangeException("precedence");
      if (!EnumValidator.IsValid(conflictResolution))
        throw new ArgumentOutOfRangeException("conflictResolution");
      this._reference = reference;
      this._precedence = precedence;
      this._conflictResolution = conflictResolution;
      this._depth = reference == (OrderToken) null ? 0 : reference._depth + 1;
      this._index = reference == (OrderToken) null ? -1 : reference._nextChildIndex++;
    }

    public static bool operator ==(OrderToken first, OrderToken second)
    {
      if (object.ReferenceEquals((object) first, (object) second))
        return true;
      if (object.ReferenceEquals((object) first, (object) null) || object.ReferenceEquals((object) second, (object) null))
        return false;
      return first.CompareTo(second) == 0;
    }

    public static bool operator !=(OrderToken first, OrderToken second)
    {
      return !(first == second);
    }

    public static bool operator <(OrderToken first, OrderToken second)
    {
      return first.CompareTo(second) < 0;
    }

    public static bool operator >(OrderToken first, OrderToken second)
    {
      return first.CompareTo(second) > 0;
    }

    public virtual int CompareTo(OrderToken other)
    {
      if (object.ReferenceEquals((object) other, (object) null))
        throw new ArgumentNullException("other");
      if (object.ReferenceEquals((object) other, (object) this))
        return 0;
      OrderToken left = this;
      while (left._reference != other._reference)
      {
        if (left._depth == other._depth)
        {
          left = left._reference;
          other = other._reference;
        }
        else if (left._depth > other._depth)
        {
          if (left._reference == other)
            return left._precedence != OrderTokenPrecedence.After ? -1 : 1;
          left = left._reference;
        }
        else
        {
          if (other._reference == left)
            return other._precedence != OrderTokenPrecedence.After ? 1 : -1;
          other = other._reference;
        }
      }
      if (left._precedence != other._precedence)
        return left._precedence != OrderTokenPrecedence.Before ? 1 : -1;
      if (left._precedence == OrderTokenPrecedence.Before)
      {
        if (left._conflictResolution == OrderTokenConflictResolution.Win)
          return -1;
        if (other._conflictResolution == OrderTokenConflictResolution.Win)
          return 1;
        return this.ResolveConflict(left, other);
      }
      if (left._conflictResolution == OrderTokenConflictResolution.Win)
        return 1;
      if (other._conflictResolution == OrderTokenConflictResolution.Win)
        return -1;
      return this.ResolveConflict(left, other);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is OrderToken))
        return false;
      return this.CompareTo((OrderToken) obj) == 0;
    }

    public override int GetHashCode()
    {
      return this._reference.GetHashCode() ^ this._precedence.GetHashCode() ^ this._conflictResolution.GetHashCode();
    }

    protected virtual int ResolveConflict(OrderToken left, OrderToken right)
    {
      return left._index.CompareTo(right._index);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Metadata.MemberTypeEnumerable
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
  internal struct MemberTypeEnumerable : IEnumerable<MemberType>, IEnumerable
  {
    private MemberType memberTypes;

    public MemberTypeEnumerable(MemberType memberTypes)
    {
      this.memberTypes = memberTypes;
    }

    public IEnumerator<MemberType> GetEnumerator()
    {
      return (IEnumerator<MemberType>) new MemberTypeEnumerable.MemberTypeEnumerator(this.memberTypes);
    }

    IEnumerator<MemberType> IEnumerable<MemberType>.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private struct MemberTypeEnumerator : IEnumerator<MemberType>, IDisposable, IEnumerator
    {
      private int memberTypes;
      private int bitMask;

      public MemberType Current
      {
        get
        {
          return (MemberType) this.Value;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      private int Value
      {
        get
        {
          return this.memberTypes & this.bitMask;
        }
      }

      public MemberTypeEnumerator(MemberType memberTypes)
      {
        this.memberTypes = (int) memberTypes;
        this.bitMask = 0;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        while (this.bitMask < 4096)
        {
          if (this.bitMask == 0)
            this.bitMask = 1;
          else
            this.bitMask <<= 1;
          if (this.Value != 0)
            return true;
        }
        return false;
      }

      public void Reset()
      {
        this.bitMask = 0;
      }
    }
  }
}

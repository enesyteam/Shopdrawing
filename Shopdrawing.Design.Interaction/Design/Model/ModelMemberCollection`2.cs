// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelMemberCollection`2
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelMemberCollection<TItemType, TKeyType> : IEnumerable<TItemType>, IEnumerable where TKeyType : IEquatable<TKeyType>
  {
    public TItemType this[string name]
    {
      get
      {
        if (name == null)
          throw new ArgumentNullException("name");
        return this.Find(name, true);
      }
    }

    public TItemType this[TKeyType value]
    {
      get
      {
        ModelMemberCollection<TItemType, TKeyType>.Validate(value);
        return this.Find(value, true);
      }
    }

    internal ModelMemberCollection()
    {
    }

    public TItemType Find(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      return this.Find(name, false);
    }

    protected abstract TItemType Find(string name, bool throwOnError);

    public TItemType Find(TKeyType value)
    {
      ModelMemberCollection<TItemType, TKeyType>.Validate(value);
      return this.Find(value, false);
    }

    protected abstract TItemType Find(TKeyType value, bool throwOnError);

    public abstract IEnumerator<TItemType> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private static void Validate(TKeyType value)
    {
      if (typeof (TKeyType).IsValueType)
      {
        if (value.Equals(default (TKeyType)))
          throw new ArgumentNullException("value");
      }
      else if ((object) value == null)
        throw new ArgumentNullException("value");
    }
  }
}

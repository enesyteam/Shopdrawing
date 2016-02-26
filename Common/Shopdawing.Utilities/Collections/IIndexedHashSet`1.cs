// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.IIndexedHashSet`1
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public interface IIndexedHashSet<T> : ICollection, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
  {
    T this[int hash] { get; }
  }
}

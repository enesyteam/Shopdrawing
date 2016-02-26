// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.IThemeCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;

namespace Microsoft.Expression.Framework.UserInterface
{
  public interface IThemeCollection : ICollection, IEnumerable
  {
    ITheme this[int index] { get; }

    void Add(ITheme value);

    void Remove(ITheme value);
  }
}

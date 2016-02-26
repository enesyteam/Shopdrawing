// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ICommandBarCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;

namespace Microsoft.Expression.Framework.UserInterface
{
  public interface ICommandBarCollection : ICollection, IEnumerable
  {
    ICommandBar this[int index] { get; }

    ICommandBar this[string identifier] { get; }

    ICommandBar AddMenuBar(string identifier);

    ICommandBar AddToolBar(string identifier);

    ICommandBar AddContextMenu(string identifier);

    void Remove(string identifier);
  }
}

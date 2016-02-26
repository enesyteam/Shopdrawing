// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ICommandBarItemCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public interface ICommandBarItemCollection : ICollection, IEnumerable
  {
    ICommandBarItem this[int index] { get; }

    ICommandBarButton AddButton(string commandName);

    ICommandBarButton AddButton(string commandName, string text);

    ICommandBarButton AddButton(string commandName, string text, params KeyBinding[] keyBindings);

    ICommandBarCheckBox AddCheckBox(string commandName);

    ICommandBarCheckBox AddCheckBox(string commandName, string text);

    ICommandBarCheckBox AddCheckBox(string commandName, string text, params KeyBinding[] keyBindings);

    ICommandBarMenu AddMenu(string identifier, string text);

    ICommandBarMenu AddMenu(string identifier, string text, bool isCollapsible);

    ICommandBarDynamicMenu AddDynamicMenu(string commandName);

    ICommandBarDynamicMenu AddDynamicMenu(string commandName, string text);

    ICommandBarDynamicMenu AddDynamicMenu(string commandName, string text, params KeyBinding[] keyBindings);

    void AddInPlaceDynamicMenuItemsFrom(string commandName);

    ICommandBarSeparator AddSeparator();

    ICommandBarButton InsertButton(int index, string commandName);

    ICommandBarButton InsertButton(int index, string commandName, string text);

    ICommandBarButton InsertButton(int index, string commandName, string text, params KeyBinding[] keyBindings);

    ICommandBarCheckBox InsertCheckBox(int index, string commandName);

    ICommandBarCheckBox InsertCheckBox(int index, string commandName, string text);

    ICommandBarCheckBox InsertCheckBox(int index, string commandName, string text, params KeyBinding[] keyBindings);

    ICommandBarMenu InsertMenu(int index, string identifier, string text);

    ICommandBarMenu InsertMenu(int index, string identifier, string text, bool isCollapsible);

    ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName);

    ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName, string text);

    ICommandBarDynamicMenu InsertDynamicMenu(int index, string commandName, string text, params KeyBinding[] keyBindings);

    ICommandBarSeparator InsertSeparator(int index);

    void Clear();

    void Remove(ICommandBarItem item);

    void RemoveAt(int index);

    bool Contains(ICommandBarItem item);

    int IndexOf(ICommandBarItem item);

    void CopyTo(ICommandBarItem[] array, int index);
  }
}

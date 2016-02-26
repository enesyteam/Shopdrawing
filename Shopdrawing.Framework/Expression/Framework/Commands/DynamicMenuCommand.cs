// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.DynamicMenuCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections;

namespace Microsoft.Expression.Framework.Commands
{
  public abstract class DynamicMenuCommand : Command
  {
    public abstract IEnumerable Items { get; }

    public virtual string EmptyMenuItemText
    {
      get
      {
        return StringTable.EmptyMenuItemText;
      }
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "Items")
        return (object) this.Items;
      if (propertyName == "EmptyMenuItemText")
        return (object) this.EmptyMenuItemText;
      return base.GetProperty(propertyName);
    }
  }
}

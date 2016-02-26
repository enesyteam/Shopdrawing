// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.CommandBar
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Commands;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
  internal sealed class CommandBar : Menu, ICommandBar
  {
    private string identifier;
    private ICommandBarItemCollection items;

    public string Identifier
    {
      get
      {
        return this.identifier;
      }
      set
      {
        this.identifier = value;
      }
    }

    public ICommandBarItemCollection Items
    {
      get
      {
        return this.items;
      }
    }

    public string Text
    {
      get
      {
        return string.Empty;
      }
      set
      {
      }
    }

    internal CommandBar(ICommandService commandService)
    {
      this.items = (ICommandBarItemCollection) new CommandBarItemCollection(commandService, CommandInvocationSource.MenuItem, base.Items);
    }
  }
}

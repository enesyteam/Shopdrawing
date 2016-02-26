// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ToolboxBrowsableAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ToolboxBrowsableAttribute : Attribute
  {
    private static ToolboxBrowsableAttribute _yes;
    private static ToolboxBrowsableAttribute _no;
    private bool _browsable;

    public bool Browsable
    {
      get
      {
        return this._browsable;
      }
    }

    public static ToolboxBrowsableAttribute Yes
    {
      get
      {
        if (ToolboxBrowsableAttribute._yes == null)
          ToolboxBrowsableAttribute._yes = new ToolboxBrowsableAttribute(true);
        return ToolboxBrowsableAttribute._yes;
      }
    }

    public static ToolboxBrowsableAttribute No
    {
      get
      {
        if (ToolboxBrowsableAttribute._no == null)
          ToolboxBrowsableAttribute._no = new ToolboxBrowsableAttribute(false);
        return ToolboxBrowsableAttribute._no;
      }
    }

    public ToolboxBrowsableAttribute(bool browsable)
    {
      this._browsable = browsable;
    }
  }
}

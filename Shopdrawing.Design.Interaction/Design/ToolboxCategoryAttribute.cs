// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ToolboxCategoryAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ToolboxCategoryAttribute : Attribute
  {
    public string CategoryPath { get; private set; }

    public bool AlwaysShows { get; private set; }

    public ToolboxCategoryAttribute(string categoryPath)
      : this(categoryPath, false)
    {
    }

    public ToolboxCategoryAttribute(string categoryPath, bool alwaysShows)
    {
      this.CategoryPath = categoryPath;
      this.AlwaysShows = alwaysShows;
    }
  }
}

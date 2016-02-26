// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.DataContextValueSourceAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class DataContextValueSourceAttribute : Attribute
  {
    public string DataContextValueSourceProperty { get; private set; }

    public bool IsCollectionItem { get; private set; }

    public string AncestorPath { get; private set; }

    public DataContextValueSourceAttribute(string dataContextValueSourceProperty, bool isCollectionItem)
      : this(dataContextValueSourceProperty, (string) null, isCollectionItem)
    {
    }

    public DataContextValueSourceAttribute(string dataContextValueSourceProperty, string ancestorPath, bool isCollectionItem)
    {
      this.DataContextValueSourceProperty = dataContextValueSourceProperty;
      this.AncestorPath = ancestorPath;
      this.IsCollectionItem = isCollectionItem;
    }
  }
}

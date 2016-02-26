// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.ToolboxExampleAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.Globalization;

namespace Microsoft.Windows.Design
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ToolboxExampleAttribute : Attribute
  {
    public Type ToolboxExampleFactoryType { get; private set; }

    public ToolboxExampleAttribute(Type toolboxExampleFactoryType)
    {
      if (!typeof (IToolboxExampleFactory).IsAssignableFrom(toolboxExampleFactoryType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidToolboxExampleFactoryType, new object[1]
        {
          (object) typeof (IToolboxExampleFactory).Name
        }), "toolboxExampleFactoryType");
      this.ToolboxExampleFactoryType = toolboxExampleFactoryType;
    }
  }
}

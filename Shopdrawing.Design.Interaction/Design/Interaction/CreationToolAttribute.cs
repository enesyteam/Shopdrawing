// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.CreationToolAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.Globalization;

namespace Microsoft.Windows.Design.Interaction
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CreationToolAttribute : Attribute
  {
    private Type _toolType;

    public Type ToolType
    {
      get
      {
        return this._toolType;
      }
    }

    public CreationToolAttribute(Type toolType)
    {
      if (toolType != null && !typeof (CreationTool).IsAssignableFrom(toolType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ArgIncorrectType, new object[2]
        {
          (object) "toolType",
          (object) typeof (CreationTool).Name
        }));
      this._toolType = toolType;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      CreationToolAttribute creationToolAttribute = obj as CreationToolAttribute;
      if (creationToolAttribute != null)
        return creationToolAttribute._toolType == this._toolType;
      return false;
    }

    public override int GetHashCode()
    {
      if (this._toolType != null)
        return this._toolType.GetHashCode();
      return typeof (CreationToolAttribute).GetHashCode();
    }
  }
}

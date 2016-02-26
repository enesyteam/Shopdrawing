// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.BindingPropertyMatchInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class BindingPropertyMatchInfo
  {
    public IProperty Property { get; private set; }

    public IType PropertyType
    {
      get
      {
        return this.Property.PropertyType;
      }
    }

    public IType NullableNormalizedPropertyType { get; private set; }

    public BindingPropertyCompatibility Compatibility { get; set; }

    public BindingPropertyMatchInfo(IProperty property)
    {
      this.Property = property;
      if (property == null)
        return;
      this.NullableNormalizedPropertyType = property.PropertyType.NullableType;
      if (this.NullableNormalizedPropertyType != null)
        return;
      this.NullableNormalizedPropertyType = property.PropertyType;
    }

    public override string ToString()
    {
      if (this.Property == null)
        return this.Compatibility.ToString();
      return this.Property.ToString() + (object) " - " + (string) (object) this.Compatibility;
    }
  }
}

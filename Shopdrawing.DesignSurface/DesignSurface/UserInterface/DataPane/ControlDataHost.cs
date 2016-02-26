// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ControlDataHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ControlDataHost : IDataHost
  {
    private ITypeId controlType;
    private IPropertyId property;

    public string ElementName
    {
      get
      {
        return this.ControlType.Name;
      }
    }

    public string PropertyName
    {
      get
      {
        return this.Property.Name;
      }
    }

    public ITypeId ControlType
    {
      get
      {
        return this.controlType;
      }
    }

    public IPropertyId Property
    {
      get
      {
        return this.property;
      }
    }

    public ControlDataHost(ITypeId controlType, IPropertyId property)
    {
      this.controlType = controlType;
      this.property = property;
    }
  }
}

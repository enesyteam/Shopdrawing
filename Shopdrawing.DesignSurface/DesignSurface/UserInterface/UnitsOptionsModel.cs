// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.UnitsOptionsModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.Configuration;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class UnitsOptionsModel : INotifyPropertyChanged
  {
    private const UnitType DefaultUnitType = UnitType.Points;
    private IConfigurationObject configurationObject;
    private bool saveOnPropertyChanged;
    private bool isInitializing;
    private UnitType unitType;

    public UnitType UnitType
    {
      get
      {
        return this.unitType;
      }
      set
      {
        this.SetProperty<UnitType>("UnitType", ref this.unitType, value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public UnitsOptionsModel(bool saveOnPropertyChanged)
    {
      this.saveOnPropertyChanged = saveOnPropertyChanged;
    }

    public void Load(IConfigurationObject value)
    {
      this.configurationObject = value;
      this.isInitializing = true;
      this.UnitType = (UnitType) value.GetProperty("UnitType", (object) UnitType.Points);
      this.isInitializing = false;
    }

    public void Save(IConfigurationObject value)
    {
      value.SetProperty("UnitType", (object) this.UnitType, (object) UnitType.Points);
    }

    private void SetProperty<T>(string propertyName, ref T propertyStorage, T value)
    {
      if (object.Equals((object) propertyStorage, (object) value))
        return;
      propertyStorage = value;
      if (!this.isInitializing && this.saveOnPropertyChanged && this.configurationObject != null)
        this.Save(this.configurationObject);
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

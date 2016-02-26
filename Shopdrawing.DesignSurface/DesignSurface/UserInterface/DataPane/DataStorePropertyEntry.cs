// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataStorePropertyEntry
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataStorePropertyEntry : NotifyingObject
  {
    private SampleDataSet dataStore;
    private IProperty property;
    private string propertyName;
    private string stringValue;
    private DataStoreType type;
    private DataStorePropertyEntryDirtyStates dirtyState;
    private Dictionary<DataStoreType, string> cacheValue;
    private readonly IList<DataStorePropertyEntry> properties;

    public DataStorePropertyEntryDirtyStates DirtyState
    {
      get
      {
        return this.dirtyState;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.property;
      }
    }

    public bool IsNameDirty
    {
      get
      {
        return (this.dirtyState & DataStorePropertyEntryDirtyStates.DirtyName) != DataStorePropertyEntryDirtyStates.None;
      }
    }

    public bool IsValueDirty
    {
      get
      {
        return (this.dirtyState & DataStorePropertyEntryDirtyStates.DirtyValue) != DataStorePropertyEntryDirtyStates.None;
      }
    }

    public bool IsTypeDirty
    {
      get
      {
        return (this.dirtyState & DataStorePropertyEntryDirtyStates.DirtyType) != DataStorePropertyEntryDirtyStates.None;
      }
    }

    public bool IsDirty
    {
      get
      {
        return (this.dirtyState & DataStorePropertyEntryDirtyStates.Dirty) != DataStorePropertyEntryDirtyStates.None;
      }
    }

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }
      set
      {
        string propertyName = this.ValidatePropertyName(value);
        if (!(this.propertyName != propertyName))
          return;
        if (!string.IsNullOrEmpty(propertyName) && (this.property.Name == propertyName || Enumerable.FirstOrDefault<DataStorePropertyEntry>(Enumerable.Where<DataStorePropertyEntry>((IEnumerable<DataStorePropertyEntry>) this.properties, (Func<DataStorePropertyEntry, bool>) (item =>
        {
          if (!(item.PropertyName == propertyName))
            return item.property.Name == propertyName;
          return true;
        }))) == null))
        {
          this.propertyName = propertyName;
          this.dirtyState |= DataStorePropertyEntryDirtyStates.DirtyName;
        }
        this.OnPropertyChanged("PropertyName");
      }
    }

    public string PropertyValue
    {
      get
      {
        return this.stringValue;
      }
      set
      {
        if (!(this.stringValue != value))
          return;
        if (this.ValidateValue(value))
        {
          this.stringValue = value;
          this.dirtyState |= DataStorePropertyEntryDirtyStates.DirtyValue;
        }
        this.OnPropertyChanged("PropertyValue");
        this.OnPropertyChanged("PropertyValueAsBoolean");
      }
    }

    public bool PropertyValueAsBoolean
    {
      get
      {
        return string.Compare(this.stringValue, "true", StringComparison.OrdinalIgnoreCase) == 0;
      }
      set
      {
        string str = value ? "true" : "false";
        if (!(this.stringValue != str))
          return;
        this.stringValue = str;
        this.dirtyState |= DataStorePropertyEntryDirtyStates.DirtyValue;
        this.OnPropertyChanged("PropertyValue");
        this.OnPropertyChanged("PropertyValueAsBoolean");
      }
    }

    public bool IsPropertyTypeBoolean
    {
      get
      {
        return this.type == DataStoreType.Boolean;
      }
    }

    public DataStoreType PropertyType
    {
      get
      {
        return this.type;
      }
      set
      {
        if (this.PropertyType == value)
          return;
        this.cacheValue[this.type] = this.stringValue;
        this.type = value;
        this.dirtyState |= DataStorePropertyEntryDirtyStates.DirtyType;
        this.AssignNewValueForType();
        this.OnPropertyChanged("PropertyType");
        this.OnPropertyChanged("IsPropertyTypeBoolean");
        this.OnPropertyChanged("PropertyValue");
        this.OnPropertyChanged("PropertyValueAsBoolean");
      }
    }

    public SampleProperty DataSetProperty
    {
      get
      {
        return this.dataStore.RootType.GetSampleProperty(this.property.Name);
      }
    }

    public DataStorePropertyEntry(SampleDataSet dataStore, IProperty property, IList<DataStorePropertyEntry> properties)
    {
      this.dataStore = dataStore;
      this.property = property;
      this.stringValue = dataStore.RootNode.GetValue<string>((IPropertyId) this.property);
      this.type = DataStorePropertyEntry.DataStoreTypeFromSampleProperty(dataStore.RootType.GetSampleProperty(property.Name));
      this.dirtyState = DataStorePropertyEntryDirtyStates.None;
      this.propertyName = property.Name;
      this.cacheValue = new Dictionary<DataStoreType, string>();
      this.cacheValue[this.type] = this.stringValue;
      this.properties = properties;
    }

    public static DataStoreType DataStoreTypeFromSampleProperty(SampleProperty sampleProperty)
    {
      if (sampleProperty.PropertySampleType == SampleBasicType.String)
        return DataStoreType.String;
      if (sampleProperty.PropertySampleType == SampleBasicType.Boolean)
        return DataStoreType.Boolean;
      return sampleProperty.PropertySampleType == SampleBasicType.Number ? DataStoreType.Number : DataStoreType.String;
    }

    public static SampleType SampleTypeFromDataStoreType(DataStoreType dataStoreType)
    {
      if (dataStoreType == DataStoreType.String)
        return (SampleType) SampleBasicType.String;
      if (dataStoreType == DataStoreType.Boolean)
        return (SampleType) SampleBasicType.Boolean;
      if (dataStoreType == DataStoreType.Number)
        return (SampleType) SampleBasicType.Number;
      return (SampleType) SampleBasicType.String;
    }

    public string ValidatePropertyName()
    {
      return this.PropertyName = this.ValidatePropertyName(this.PropertyName);
    }

    private string ValidatePropertyName(string propertyName)
    {
      SampleProperty dataSetProperty = this.DataSetProperty;
      propertyName = this.dataStore.GetUniqueTypeName(propertyName, this.PropertyName);
      if (string.IsNullOrEmpty(propertyName))
        return string.Empty;
      return propertyName;
    }

    private bool ValidateValue(string value)
    {
      bool flag = false;
      switch (this.type)
      {
        case DataStoreType.String:
          flag = true;
          break;
        case DataStoreType.Number:
          double result1;
          if (double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.CurrentCulture, out result1))
          {
            flag = true;
            break;
          }
          break;
        case DataStoreType.Boolean:
          bool result2;
          if (bool.TryParse(value, out result2))
          {
            flag = true;
            break;
          }
          break;
      }
      return flag;
    }

    private void AssignNewValueForType()
    {
      string str = (string) null;
      if (this.cacheValue.TryGetValue(this.type, out str))
      {
        this.stringValue = str;
      }
      else
      {
        switch (this.type)
        {
          case DataStoreType.String:
            this.stringValue = StringTable.DefaultValueDataStore;
            break;
          case DataStoreType.Number:
            this.stringValue = 0.ToString((IFormatProvider) CultureInfo.CurrentCulture);
            break;
          case DataStoreType.Boolean:
            this.stringValue = false.ToString((IFormatProvider) CultureInfo.CurrentCulture);
            break;
        }
      }
      this.dirtyState |= DataStorePropertyEntryDirtyStates.DirtyValue;
    }
  }
}

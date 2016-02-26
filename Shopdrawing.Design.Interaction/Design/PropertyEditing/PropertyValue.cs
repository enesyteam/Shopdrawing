// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.PropertyValue
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public abstract class PropertyValue : INotifyPropertyChanged
  {
    private PropertyEntry _parentProperty;

    public virtual PropertyEntry ParentProperty
    {
      get
      {
        return this._parentProperty;
      }
    }

    public abstract PropertyValueSource Source { get; }

    public abstract bool IsDefaultValue { get; }

    public abstract bool IsMixedValue { get; }

    public abstract bool CanConvertFromString { get; }

    public object Value
    {
      get
      {
        object obj = (object) null;
        if (this.CatchExceptions)
        {
          try
          {
            obj = this.GetValueCore();
          }
          catch (Exception ex)
          {
            this.OnPropertyValueException(new PropertyValueExceptionEventArgs(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueGetFailed, new object[0]), this, PropertyValueExceptionSource.Get, ex));
          }
        }
        else
          obj = this.GetValueCore();
        return obj;
      }
      set
      {
        if (this.CatchExceptions)
        {
          try
          {
            this.SetValueImpl(value);
          }
          catch (Exception ex)
          {
            this.OnPropertyValueException(new PropertyValueExceptionEventArgs(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueSetFailed, new object[0]), this, PropertyValueExceptionSource.Set, ex));
          }
        }
        else
          this.SetValueImpl(value);
      }
    }

    public string StringValue
    {
      get
      {
        string str = string.Empty;
        if (this.CatchExceptions)
        {
          try
          {
            str = this.ConvertValueToString(this.Value);
          }
          catch (Exception ex)
          {
            this.OnPropertyValueException(new PropertyValueExceptionEventArgs(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotConvertValueToString, new object[0]), this, PropertyValueExceptionSource.Get, ex));
          }
        }
        else
          str = this.ConvertValueToString(this.Value);
        return str;
      }
      set
      {
        if (this.CatchExceptions)
        {
          try
          {
            this.Value = this.ConvertStringToValue(value);
          }
          catch (Exception ex)
          {
            this.OnPropertyValueException(new PropertyValueExceptionEventArgs(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotUpdateValueFromStringValue, new object[0]), this, PropertyValueExceptionSource.Set, ex));
          }
        }
        else
          this.Value = this.ConvertStringToValue(value);
      }
    }

    public abstract bool HasSubProperties { get; }

    public abstract PropertyEntryCollection SubProperties { get; }

    public abstract bool IsCollection { get; }

    public abstract PropertyValueCollection Collection { get; }

    protected virtual bool CatchExceptions
    {
      get
      {
        return this.PropertyValueException != null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler RootValueChanged;

    public event EventHandler SubPropertyChanged;

    public event EventHandler<PropertyValueExceptionEventArgs> PropertyValueException;

    protected PropertyValue(PropertyEntry parentProperty)
    {
      if (parentProperty == null)
        throw new ArgumentNullException("parentProperty");
      this._parentProperty = parentProperty;
    }

    protected abstract void ValidateValue(object valueToValidate);

    protected abstract object ConvertStringToValue(string value);

    protected abstract string ConvertValueToString(object value);

    protected abstract object GetValueCore();

    protected abstract void SetValueCore(object value);

    public abstract void ClearValue();

    private void SetValueImpl(object value)
    {
      this.ValidateValue(value);
      this.SetValueCore(value);
      this.NotifyValueChanged();
      this.OnRootValueChanged();
    }

    protected virtual void OnPropertyValueException(PropertyValueExceptionEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      if (this.PropertyValueException == null)
        return;
      this.PropertyValueException((object) this, e);
    }

    protected virtual void NotifyRootValueChanged()
    {
      this.OnPropertyChanged("IsDefaultValue");
      this.OnPropertyChanged("IsMixedValue");
      this.OnPropertyChanged("IsCollection");
      this.OnPropertyChanged("Collection");
      this.OnPropertyChanged("HasSubProperties");
      this.OnPropertyChanged("SubProperties");
      this.OnPropertyChanged("Source");
      this.OnPropertyChanged("CanConvertFromString");
      this.NotifyValueChanged();
      this.OnRootValueChanged();
    }

    protected void NotifySubPropertyChanged()
    {
      this.NotifyValueChanged();
      this.OnSubPropertyChanged();
    }

    private void NotifyValueChanged()
    {
      this.OnPropertyChanged("Value");
      this.NotifyStringValueChanged();
    }

    private void NotifyStringValueChanged()
    {
      this.OnPropertyChanged("StringValue");
    }

    private void OnRootValueChanged()
    {
      if (this.RootValueChanged == null)
        return;
      this.RootValueChanged((object) this, EventArgs.Empty);
    }

    private void OnSubPropertyChanged()
    {
      if (this.SubPropertyChanged == null)
        return;
      this.SubPropertyChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
  }
}

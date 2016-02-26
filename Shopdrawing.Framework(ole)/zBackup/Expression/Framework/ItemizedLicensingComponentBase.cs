// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ItemizedLicensingComponentBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Framework
{
  public abstract class ItemizedLicensingComponentBase : NotifyingObject
  {
    private static readonly double DefaultHeight = 43.0;

    public string TrialStatus
    {
      get
      {
        if (this.IsExpired)
          return StringTable.LicensingItemizedDialogExpiredText;
        if (this.IsTrial)
        {
          if (this.DaysRemaining == 1)
            return StringTable.LicensingItemizedDialogTrialTextDaysSingular;
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.LicensingItemizedDialogTrialTextDaysPlural, new object[1]
          {
            (object) this.DaysRemaining.ToString((IFormatProvider) CultureInfo.CurrentCulture)
          });
        }
        if (!this.DoesLicenseExpire)
          return StringTable.LicensingItemizedDialogLicensedText;
        if (this.DaysRemaining == 1)
          return StringTable.LicensingItemizedDialogTempLicenseSingularText;
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.LicensingItemizedDialogTempLicensePluralText, new object[1]
        {
          (object) this.DaysRemaining.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      }
    }

    public virtual double RowHeight
    {
      get
      {
        return ItemizedLicensingComponentBase.DefaultHeight;
      }
    }

    public string ButtonText
    {
      get
      {
        if (this.IsActivatable)
          return StringTable.LicensingItemizedDialogActivateButton;
        return StringTable.LicensingItemizedDialogEnterKeyButton;
      }
    }

    public bool ActionAvailable
    {
      get
      {
        if (this.IsLicensed)
          return this.RequiresActivation;
        return true;
      }
    }

    public abstract string Name { get; }

    public abstract ImageSource Icon { get; }

    public abstract ICommand LicenseButtonCommand { get; }

    public bool IsVisible { get; protected set; }

    public bool IsTrial { get; protected set; }

    public bool IsExpired { get; protected set; }

    public bool IsLicensed { get; protected set; }

    public bool RequiresActivation { get; protected set; }

    public bool IsActivatable { get; protected set; }

    public bool DoesLicenseExpire { get; protected set; }

    public int DaysRemaining { get; protected set; }

    public event EventHandler StatusChanged;

    protected ItemizedLicensingComponentBase()
    {
      this.IsVisible = true;
    }

    protected internal abstract void RefreshLicenseValues();

    protected void OnStatusChanged()
    {
      if (this.StatusChanged == null)
        return;
      this.StatusChanged((object) this, EventArgs.Empty);
    }
  }
}

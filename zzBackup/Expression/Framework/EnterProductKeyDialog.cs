// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.EnterProductKeyDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Licenses;
using System;
using System.Globalization;

namespace Microsoft.Expression.Framework
{
  internal sealed class EnterProductKeyDialog : LicensingDialog
  {
    public override bool ShowActivationCheckBox
    {
      get
      {
        return true;
      }
    }

    public override bool HasCancelButton
    {
      get
      {
        return false;
      }
    }

    public override string FallbackOptionText
    {
      get
      {
        if (this.IsPermanentLicense)
          return StringTable.LicensingContinueUsingCurrentLicense;
        return this.GetNonPermanentLicenseOptionText();
      }
    }

    public EnterProductKeyDialog(IServices services)
      : base(services)
    {
    }

    private string GetNonPermanentLicenseOptionText()
    {
      ILicenseService service = this.Services.GetService<ILicenseService>();
      bool flag1 = service.FeaturesFromSku(service.MostPermissiveSkuId).Contains(ExpressionFeatureMapper.ActivationLicense);
      bool flag2 = (int) this.GraceDaysRemaining == 1;
      if (flag1)
      {
        if (flag2)
          return StringTable.LicensingRemainingDaysInActivationTextSingular;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.LicensingRemainingDaysInActivationTextPlural, new object[1]
        {
          (object) this.GraceDaysRemaining
        });
      }
      if (flag2)
        return StringTable.LicensingUseTrialTextSingular;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.LicensingUseTrialTextPlural, new object[1]
      {
        (object) this.GraceDaysRemaining
      });
    }

    protected override bool ProcessFallbackOption()
    {
      this.IsLicensed = true;
      return true;
    }
  }
}

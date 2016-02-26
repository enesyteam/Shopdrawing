// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.DataAccess.ActivationDataRepository
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.Framework.ActivationDialog.DataAccess
{
  internal class ActivationDataRepository
  {
    private const int NumberOfInstallationIdGroups = 9;
    private const int InstallationIdGroupIdentifierLength = 6;
    private const int NumberOfValidationIdGroups = 8;
    private readonly ILicenseService licenseService;
    private PhoneNumberStore phoneNumberStore;
    private IList<LocationInformation> validLocations;
    private string cachedInstallationId;

    private PhoneNumberStore PhoneNumberStore
    {
      get
      {
        if (this.phoneNumberStore == null)
          this.phoneNumberStore = new PhoneNumberStore();
        return this.phoneNumberStore;
      }
    }

    public IList<LocationInformation> ValidLocations
    {
      get
      {
        if (this.validLocations == null)
          this.validLocations = this.PhoneNumberStore.GetLocations();
        return this.validLocations;
      }
    }

    public ActivationDataRepository(ILicenseService licenseService)
    {
      this.licenseService = licenseService;
    }

    public IList<PhoneNumberInformation> GetPhoneNumberInfosForLocation(LocationInformation country)
    {
      return this.PhoneNumberStore.GetPhoneNumberInfosForLocation(country);
    }

    public List<PhoneActivationData> GetInstallationIdsForMachine()
    {
      this.cachedInstallationId = this.licenseService.GetOfflineInstallationId();
      return ActivationDataRepository.SplitLicenseStringIntoInstallationIdGroups(this.cachedInstallationId);
    }

    private static List<PhoneActivationData> SplitLicenseStringIntoInstallationIdGroups(string installationId)
    {
      List<PhoneActivationData> list = new List<PhoneActivationData>();
      if (!string.IsNullOrEmpty(installationId) && installationId.Length == 54)
      {
        for (int index = 0; index < 9; ++index)
        {
          int num = index + 1;
          list.Add(new PhoneActivationData(num.ToString((IFormatProvider) CultureInfo.InvariantCulture), installationId.Substring(index * 6, 6)));
        }
      }
      return list;
    }

    public IActivateResponse TryPhoneActivation(IList<PhoneActivationData> validationIds)
    {
      if (!string.IsNullOrEmpty(this.cachedInstallationId))
        return this.licenseService.EnterOfflineConfirmationId(this.cachedInstallationId, ActivationDataRepository.BuildValidationIdString(validationIds));
      return (IActivateResponse) new ActivateResponse();
    }

    private static string BuildValidationIdString(IList<PhoneActivationData> validationIds)
    {
      StringBuilder stringBuilder = new StringBuilder(54);
      if (validationIds.Count == 8)
      {
        foreach (PhoneActivationData phoneActivationData in (IEnumerable<PhoneActivationData>) validationIds)
          stringBuilder.Append(phoneActivationData.IdNumber);
      }
      return stringBuilder.ToString();
    }

    public IActivateResponse TryInternetActivation()
    {
      return this.licenseService.ActivateProduct();
    }
  }
}

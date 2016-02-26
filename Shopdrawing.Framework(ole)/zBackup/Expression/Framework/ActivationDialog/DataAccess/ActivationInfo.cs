// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.DataAccess.ActivationInfo
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Framework.ActivationDialog.DataAccess
{
  internal class ActivationInfo : NotifyingObject
  {
    private ActivationDataRepository dataRepository;
    private ReadOnlyCollection<LocationInformation> validLocations;
    private LocationInformation currentLocation;
    private ReadOnlyCollection<PhoneActivationData> installationIds;

    public ReadOnlyCollection<LocationInformation> ValidLocations
    {
      get
      {
        if (this.validLocations == null)
          this.validLocations = new ReadOnlyCollection<LocationInformation>(this.dataRepository.ValidLocations);
        return this.validLocations;
      }
    }

    public LocationInformation CurrentLocation
    {
      get
      {
        return this.currentLocation;
      }
      set
      {
        if (this.currentLocation == value)
          return;
        this.currentLocation = value;
        this.OnPropertyChanged("CurrentLocation");
        this.UpdatePhoneNumbersForCurrentLocation();
      }
    }

    public ReadOnlyCollection<PhoneNumberInformation> PhoneNumbers { get; private set; }

    public ReadOnlyCollection<PhoneActivationData> InstallationIds
    {
      get
      {
        if (this.installationIds == null)
          this.installationIds = new ReadOnlyCollection<PhoneActivationData>((IList<PhoneActivationData>) this.dataRepository.GetInstallationIdsForMachine());
        return this.installationIds;
      }
    }

    public ReadOnlyCollection<PhoneActivationData> ValidationIds { get; private set; }

    public ActivationInfo(ActivationDataRepository dataRepository)
    {
      this.dataRepository = dataRepository;
      this.ValidationIds = new ReadOnlyCollection<PhoneActivationData>((IList<PhoneActivationData>) new List<PhoneActivationData>()
      {
        new PhoneActivationData("A"),
        new PhoneActivationData("B"),
        new PhoneActivationData("C"),
        new PhoneActivationData("D"),
        new PhoneActivationData("E"),
        new PhoneActivationData("F"),
        new PhoneActivationData("G"),
        new PhoneActivationData("H")
      });
    }

    private void UpdatePhoneNumbersForCurrentLocation()
    {
      this.PhoneNumbers = new ReadOnlyCollection<PhoneNumberInformation>(this.dataRepository.GetPhoneNumberInfosForLocation(this.CurrentLocation));
      this.OnPropertyChanged("PhoneNumbers");
    }
  }
}

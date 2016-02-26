// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.ChooseActivationTypePageViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class ChooseActivationTypePageViewModel : ActivationWizardPageViewModelBase
  {
    private ICollectionView validLocations;
    private bool isPhoneActivation;

    public IServices Services { get; private set; }

    public override ActivationWizardAction PreviousAction
    {
      get
      {
        return ActivationWizardAction.None;
      }
    }

    public override ActivationWizardAction NextAction
    {
      get
      {
        return this.IsPhoneActivation ? ActivationWizardAction.PhoneNumber : ActivationWizardAction.AttemptOnlineActivation;
      }
    }

    public override bool IsValid
    {
      get
      {
        if (this.IsPhoneActivation)
          return this.SelectedLocation != null;
        return true;
      }
    }

    public ICollectionView ValidLocations
    {
      get
      {
        if (this.validLocations == null)
          this.RetrieveValidLocations();
        return this.validLocations;
      }
    }

    public LocationInformation SelectedLocation
    {
      get
      {
        return this.ActivationInfo.CurrentLocation;
      }
      set
      {
        this.ActivationInfo.CurrentLocation = value;
        this.OnPropertyChanged("IsValid");
        this.OnPropertyChanged("SelectedLocation");
      }
    }

    public bool IsPhoneActivation
    {
      get
      {
        return this.isPhoneActivation;
      }
      set
      {
        if (this.isPhoneActivation == value)
          return;
        this.isPhoneActivation = value;
        this.OnPropertyChanged("IsPhoneActivation");
        this.OnPropertyChanged("IsValid");
      }
    }

    public BitmapSource ShieldImage { get; private set; }

    public ChooseActivationTypePageViewModel(ActivationInfo activationInfo, IServices services)
      : base(activationInfo)
    {
      this.InitializeShieldImage();
      this.Services = services;
      this.IsPhoneActivation = false;
    }

    private void InitializeShieldImage()
    {
      this.ShieldImage = LicensingDialog.CreateShieldImage();
    }

    private void RetrieveValidLocations()
    {
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.ActivationInfo.ValidLocations);
      defaultView.SortDescriptions.Add(new SortDescription("LocationName", ListSortDirection.Ascending));
      this.validLocations = defaultView;
    }
  }
}

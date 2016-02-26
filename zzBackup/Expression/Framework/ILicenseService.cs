// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ILicenseService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Licenses;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public interface ILicenseService
  {
    ResourceDictionary LicensingDialogResources { get; set; }

    ResourceDictionary AboutDialogResources { get; set; }

    string MostPermissivePid { get; }

    Guid MostPermissiveApplicationId { get; }

    Guid MostPermissiveSkuId { get; }

    bool RightNotGranted { get; }

    bool ProductLicenseNotInstalled { get; }

    ILicenseSkuFeatureMapper LicenseSkuFeatureMapper { get; }

    void Start();

    void Stop();

    void WaitForValidLicenseInformation();

    bool UpdateLicensing();

    void EstablishLicensingDialogs(ResourceDictionary licenseDialogResource, ResourceDictionary aboutDialogResource);

    IEnterKeyResponse EnterProductKey(string productKey, bool shouldActivate);

    IActivateResponse ActivateProduct();

    string GetOfflineInstallationId();

    IActivateResponse EnterOfflineConfirmationId(string installationId, string confirmationId);

    bool IsActivatable(Guid skuId);

    bool IsLicensed(Guid skuId);

    bool IsUnlicensed(Guid skuId);

    bool IsInGrace(Guid skuId);

    bool HasKey(Guid skuId);

    int GetRemainingGraceMinutes(Guid skuId);

    [CLSCompliant(false)]
    UnlicensedReason GetUnlicensedReason(Guid skuId);

    DateTime GetExpiration(Guid skuId);

    bool IsAnySkuEnabled(IList<Guid> licenseSkus);

    Guid MostPermissiveLicenseSku(IList<Guid> skuIds);

    IList<string> FeaturesFromSku(Guid skuId);

    IList<string> SkuStringsFromFeature(string feature);

    IList<Guid> SkusFromFeature(string feature);
  }
}

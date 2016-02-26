using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.Expression.Framework.Globalization;
using Microsoft.Expression.Framework.Licenses;
using Microsoft.Expression.Licensing;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Windows;

namespace Microsoft.Expression.Framework
{
    public sealed class LicenseService : ILicenseService, IDisposable
    {
        private readonly IServices services;

        private readonly string productSpecifierFeature;

        private readonly AggregateLicenseInformation aggregatedLicenseInformation;

        private readonly List<ApplicationLicenses> licenses;

        private ILicenseSkuFeatureMapper licenseSkuFeatureMapper;

        private readonly string baseProductPidRegistryPath;

        private readonly string baseStudioPidRegistryPath;

        private Guid lastAttemptedActivationSkuId;

        private Guid lastProductKeyEntrySku = Guid.Empty;

        private bool? lastProductKeyEntrySucceeded;

        private Microsoft.Expression.Licensing.Licensing.SkuLicenseStatus lastProductKeyLicense;

        private ManualResetEvent licensingFinished = new ManualResetEvent(false);

        private Dictionary<Guid, Guid> skuIdToAppIdTable = new Dictionary<Guid, Guid>();

        private Dictionary<Guid, Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus>> perAppIdSkuStatusTable;

        private Dictionary<Guid, List<Guid>> perSkuPKeys;

        private Licensing.Licensing.SkuLicenseStatus mostPermissiveLicense;

        private Licensing.Licensing.OverallLicensingStatus mostPermissiveLicenseStatus = Licensing.Licensing.OverallLicensingStatus.ProductSkuNotInstalled;

        private static int licensingInitializationCount;

        public ResourceDictionary AboutDialogResources
        {
            get;
            set;
        }

        private AggregateLicenseInformation AggregatedLicenseInformation
        {
            get
            {
                return this.aggregatedLicenseInformation;
            }
        }

        public ILicenseSkuFeatureMapper LicenseSkuFeatureMapper
        {
            get
            {
                return JustDecompileGenerated_get_LicenseSkuFeatureMapper();
            }
            set
            {
                JustDecompileGenerated_set_LicenseSkuFeatureMapper(value);
            }
        }

        public ILicenseSkuFeatureMapper JustDecompileGenerated_get_LicenseSkuFeatureMapper()
        {
            return this.licenseSkuFeatureMapper;
        }

        private void JustDecompileGenerated_set_LicenseSkuFeatureMapper(ILicenseSkuFeatureMapper value)
        {
            if (this.LicenseSkuFeatureMapper != null)
            {
                throw new InvalidOperationException("Attempt to assign a new value to LicenseSkuFeature Mapper.");
            }
            this.licenseSkuFeatureMapper = value;
        }

        public ResourceDictionary LicensingDialogResources
        {
            get;
            set;
        }

        public Guid MostPermissiveApplicationId
        {
            get
            {
                return JustDecompileGenerated_get_MostPermissiveApplicationId();
            }
            set
            {
                JustDecompileGenerated_set_MostPermissiveApplicationId(value);
            }
        }

        private Guid JustDecompileGenerated_MostPermissiveApplicationId_k__BackingField;

        public Guid JustDecompileGenerated_get_MostPermissiveApplicationId()
        {
            return this.JustDecompileGenerated_MostPermissiveApplicationId_k__BackingField;
        }

        private void JustDecompileGenerated_set_MostPermissiveApplicationId(Guid value)
        {
            this.JustDecompileGenerated_MostPermissiveApplicationId_k__BackingField = value;
        }

        public string MostPermissivePid
        {
            get
            {
                return JustDecompileGenerated_get_MostPermissivePid();
            }
            set
            {
                JustDecompileGenerated_set_MostPermissivePid(value);
            }
        }

        private string JustDecompileGenerated_MostPermissivePid_k__BackingField;

        public string JustDecompileGenerated_get_MostPermissivePid()
        {
            return this.JustDecompileGenerated_MostPermissivePid_k__BackingField;
        }

        private void JustDecompileGenerated_set_MostPermissivePid(string value)
        {
            this.JustDecompileGenerated_MostPermissivePid_k__BackingField = value;
        }

        public Guid MostPermissiveSkuId
        {
            get
            {
                return JustDecompileGenerated_get_MostPermissiveSkuId();
            }
            set
            {
                JustDecompileGenerated_set_MostPermissiveSkuId(value);
            }
        }

        private Guid JustDecompileGenerated_MostPermissiveSkuId_k__BackingField;

        public Guid JustDecompileGenerated_get_MostPermissiveSkuId()
        {
            return this.JustDecompileGenerated_MostPermissiveSkuId_k__BackingField;
        }

        private void JustDecompileGenerated_set_MostPermissiveSkuId(Guid value)
        {
            this.JustDecompileGenerated_MostPermissiveSkuId_k__BackingField = value;
        }

        public bool ProductLicenseNotInstalled
        {
            get
            {
                return this.mostPermissiveLicenseStatus == Licensing.Licensing.OverallLicensingStatus.ProductSkuNotInstalled;
            }
        }

        private string ProductPidRegistryPath
        {
            get
            {
                if (!this.FeaturesFromSku(this.MostPermissiveSkuId).Contains(ExpressionFeatureMapper.StudioLicense))
                {
                    return this.baseProductPidRegistryPath;
                }
                return string.Concat(this.baseProductPidRegistryPath, this.MostPermissiveApplicationId);
            }
        }

        public bool RightNotGranted
        {
            get
            {
                return this.mostPermissiveLicenseStatus == Licensing.Licensing.OverallLicensingStatus.RightNotGranted;
            }
        }

        internal Guid SkuToActivate
        {
            get
            {
                Guid guid;
                this.WaitForValidLicenseInformation();
                using (IEnumerator<Guid> enumerator = this.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Guid current = enumerator.Current;
                        if (this.IsLicensed(current) || !this.IsInGrace(current) && this.GetUnlicensedReason(current) != UnlicensedReason.GraceTimeExpired || !this.HasKey(current))
                        {
                            continue;
                        }
                        guid = current;
                        return guid;
                    }
                    return Guid.Empty;
                }
                return guid;
            }
        }

        private string StudioPidRegistryPath
        {
            get
            {
                if (!this.FeaturesFromSku(this.MostPermissiveSkuId).Contains(ExpressionFeatureMapper.StudioLicense))
                {
                    return this.baseStudioPidRegistryPath;
                }
                return string.Concat(this.baseStudioPidRegistryPath, this.MostPermissiveApplicationId);
            }
        }

        public LicenseService(IServices services, string productSpecifierFeature, ILicenseSkuFeatureMapper featureMapper, string baseStudioPidRegistryPath, string baseProductPidRegistryPath, ApplicationLicenses[] studioLicenses, ApplicationLicenses[] applicationLicenses)
        {
            this.services = services;
            if (ExpressionFeatureMapper.Blend != productSpecifierFeature && ExpressionFeatureMapper.Design != productSpecifierFeature && ExpressionFeatureMapper.Encoder != productSpecifierFeature && ExpressionFeatureMapper.Web != productSpecifierFeature)
            {
                throw new ArgumentOutOfRangeException("productSpecifierFeature", "Product specified is not one of: Blend, Design, Encoder, or Web");
            }
            this.productSpecifierFeature = productSpecifierFeature;
            if (string.IsNullOrEmpty(baseStudioPidRegistryPath))
            {
                throw new ArgumentNullException("baseStudioPidRegistryPath");
            }
            if (string.IsNullOrEmpty(baseProductPidRegistryPath))
            {
                throw new ArgumentNullException("baseProductPidRegistryPath");
            }
            this.LicenseSkuFeatureMapper = featureMapper;
            this.licenses = studioLicenses.Concat<ApplicationLicenses>(applicationLicenses).ToList<ApplicationLicenses>();
            this.aggregatedLicenseInformation = new AggregateLicenseInformation(this.licenses);
            this.baseProductPidRegistryPath = baseProductPidRegistryPath;
            this.baseStudioPidRegistryPath = baseStudioPidRegistryPath;
            Licensing.Licensing.ShouldEmitLoggingInformation = LicenseService.RetrieveExpressionLicenseLoggingPolicyFromRegistry();
        }

        [CLSCompliant(false)]
        public IActivateResponse ActivateProduct()
        {
            if (this.lastProductKeyEntrySucceeded.HasValue)
            {
                bool? nullable = this.lastProductKeyEntrySucceeded;
                if ((!nullable.GetValueOrDefault() ? false : nullable.HasValue))
                {
                    if (!this.FeaturesFromSku(this.lastProductKeyEntrySku).Contains(ExpressionFeatureMapper.ActivationLicense))
                    {
                        return new ActivateResponse()
                        {
                            ActivationStatus = ActivationStatus.NoActivatableSkus
                        };
                    }
                    if (this.IsLicensed(this.lastProductKeyEntrySku))
                    {
                        return new ActivateResponse()
                        {
                            ActivationStatus = ActivationStatus.AlreadyActivated
                        };
                    }
                    this.lastAttemptedActivationSkuId = this.lastProductKeyEntrySku;
                    return this.ActivateProduct(this.lastProductKeyEntrySku);
                }
            }
            IList<Guid> guids = this.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense);
            if (guids.Count < 1)
            {
                return new ActivateResponse()
                {
                    ActivationStatus = ActivationStatus.NoActivatableSkus
                };
            }
            IList<Guid> skusThatCanBeActivated = this.GetSkusThatCanBeActivated(guids);
            if (skusThatCanBeActivated.Count == 0)
            {
                return new ActivateResponse()
                {
                    ActivationStatus = ActivationStatus.AlreadyActivated
                };
            }
            if (skusThatCanBeActivated.Count > 1)
            {
                return new ActivateResponse()
                {
                    ActivationStatus = ActivationStatus.MultipleSkus
                };
            }
            this.lastAttemptedActivationSkuId = skusThatCanBeActivated[0];
            return this.ActivateProduct(skusThatCanBeActivated[0]);
        }

        private IActivateResponse ActivateProduct(Guid skuId)
        {
            ActivateResponse activateResponse = LicenseService.ActivateProductProcess(skuId.ToString("B"));
            if (activateResponse.Success)
            {
                this.UpdateLicensing();
            }
            activateResponse.ActivationStatus = (activateResponse.Success ? ActivationStatus.Activated : ActivationStatus.Failed);
            return activateResponse;
        }

        private static ActivateResponse ActivateProductProcess(string skuGuidString)
        {
            string str = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Microsoft.Expression.LicenseActivator.exe");
            string.IsNullOrEmpty(str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo(str, skuGuidString)
            {
                UseShellExecute = true,
                CreateNoWindow = true
            };
            return new ActivateResponse(LicenseService.CallExternalProcess(processStartInfo));
        }

        private static LicenseSubServiceResponse CallExternalProcess(ProcessStartInfo info)
        {
            LicenseSubServiceResponse licenseSubServiceResponse;
            LicenseSubServiceResponse exitCode = new LicenseSubServiceResponse();
            try
            {
                Process process = Process.Start(info);
                if (process != null)
                {
                    process.WaitForExit();
                    if (process.ExitCode < 0)
                    {
                        exitCode.ErrorCode = (uint)process.ExitCode;
                        licenseSubServiceResponse = exitCode;
                        return licenseSubServiceResponse;
                    }
                }
                return exitCode;
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                exitCode.Exception = objectDisposedException;
                licenseSubServiceResponse = exitCode;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                exitCode.Exception = invalidOperationException;
                licenseSubServiceResponse = exitCode;
            }
            catch (Win32Exception win32Exception)
            {
                exitCode.Exception = win32Exception;
                licenseSubServiceResponse = exitCode;
            }
            catch (Exception exception)
            {
                exitCode.Exception = exception;
                licenseSubServiceResponse = exitCode;
            }
            return licenseSubServiceResponse;
        }

        private string ConstructPidAndInsertInRegistry(Guid productSku, string partialProductId, int mpc)
        {
            string str = LicenseService.ConstructProductId(partialProductId, mpc);
            this.UpdateRegistryWithProductId(productSku, str);
            return str;
        }

        private static string ConstructProductId(string partialProductId, int mpc)
        {
            string str;
            string str1;
            string[] strArrays = partialProductId.Split(new char[] { '-' });
            if ((int)strArrays.Length != 4)
            {
                strArrays = new string[] { "", "", "", "" };
            }
            try
            {
                strArrays[0] = mpc.ToString("00000", CultureInfo.InvariantCulture);
                goto Label0;
            }
            catch (FormatException formatException)
            {
                str1 = "99999";
            }
            return str1;
        Label0:
            if (strArrays[1] != "OEM")
            {
                string[] strArrays1 = new string[] { strArrays[0], "-", strArrays[1], "-", strArrays[2], "-", strArrays[3] };
                str = string.Concat(strArrays1);
            }
            else
            {
                string[] strArrays2 = new string[] { strArrays[0], "-OEM-", strArrays[2], "-", strArrays[3] };
                str = string.Concat(strArrays2);
            }
            return str;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.licensingFinished != null)
                {
                    this.licensingFinished.Dispose();
                    this.licensingFinished = null;
                }
                if (this.licensingFinished != null)
                {
                    this.licensingFinished.Dispose();
                    this.licensingFinished = null;
                }
            }
        }

        [CLSCompliant(false)]
        public IActivateResponse EnterOfflineConfirmationId(string installationId, string confirmationId)
        {
            if (this.lastAttemptedActivationSkuId == Guid.Empty)
            {
                return new ActivateResponse()
                {
                    ActivationStatus = ActivationStatus.Failed
                };
            }
            return this.EnterOfflineConfirmationId(this.lastAttemptedActivationSkuId, installationId, confirmationId);
        }

        private IActivateResponse EnterOfflineConfirmationId(Guid skuId, string installationId, string confirmationId)
        {
            string str = skuId.ToString("B");
            ActivateResponse activateResponse = LicenseService.EnterOfflineConfirmationIdProcess(str, installationId, confirmationId);
            if (activateResponse.Success)
            {
                this.UpdateLicensing();
            }
            activateResponse.ActivationStatus = (activateResponse.Success ? ActivationStatus.Activated : ActivationStatus.Failed);
            return activateResponse;
        }

        private static ActivateResponse EnterOfflineConfirmationIdProcess(string sku, string installationId, string confirmationId)
        {
            string str = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Microsoft.Expression.LicensePhoneActivator.exe");
            string.IsNullOrEmpty(str);
            string[] strArrays = new string[] { sku, " ", installationId, " ", confirmationId };
            ProcessStartInfo processStartInfo = new ProcessStartInfo(str, string.Concat(strArrays))
            {
                UseShellExecute = true,
                CreateNoWindow = true
            };
            return new ActivateResponse(LicenseService.CallExternalProcess(processStartInfo));
        }

        [CLSCompliant(false)]
        public IEnterKeyResponse EnterProductKey(string productKey, bool shouldActivate)
        {
            Guid guid;
            IEnterKeyResponse enterKeyResponse;
            EnterKeyResponse enterKeyResponse1 = LicenseService.InsertProductKeyIntoLicenseStore(productKey, shouldActivate);
            this.UpdateLicensing();
            Guid mostPermissiveSkuId = this.MostPermissiveSkuId;
            Guid empty = Guid.Empty;
            string str = string.Empty;
            try
            {
                if (!LicenseService.InitializeProductLicensing(true) || Licensing.Licensing.GetPartialProductId(productKey, ref str, ref empty))
                {
                    goto Label0;
                }
                else
                {
                    this.lastProductKeyEntrySucceeded = new bool?(false);
                    enterKeyResponse = enterKeyResponse1;
                }
            }
            finally
            {
                LicenseService.ShutdownProductLicensing(true);
            }
            return enterKeyResponse;
        Label0:
            enterKeyResponse1.KeySku = empty;
            this.lastProductKeyEntrySku = empty;
            this.lastProductKeyEntrySucceeded = new bool?(true);
            if (!this.skuIdToAppIdTable.TryGetValue(empty, out guid))
            {
                return enterKeyResponse1;
            }
            this.lastProductKeyLicense = this.perAppIdSkuStatusTable[guid][empty];
            enterKeyResponse1.IsEnabled = (this.lastProductKeyLicense.Status == Licensing.Licensing.LicenseStatus.Licensed ? true : this.lastProductKeyLicense.Status == Licensing.Licensing.LicenseStatus.InGracePeriod);
            enterKeyResponse1.IsActivated = (!this.IsLicensed(this.lastProductKeyEntrySku) ? false : this.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(this.lastProductKeyEntrySku));
            if (enterKeyResponse1.ErrorCode != 0 && enterKeyResponse1.IsEnabled && !this.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(empty))
            {
                enterKeyResponse1.ErrorCode = 0;
            }
            Guid mostPermissiveSkuId1 = this.MostPermissiveSkuId;
            if (mostPermissiveSkuId != this.lastProductKeyEntrySku)
            {
                this.RecordLicenseSku(this.lastProductKeyEntrySku);
                this.ConstructPidAndInsertInRegistry(empty, str, this.Mpc(mostPermissiveSkuId1));
            }
            return enterKeyResponse1;
        }

        public void EstablishLicensingDialogs(ResourceDictionary licenseDialogResource, ResourceDictionary aboutDialogResource)
        {
            this.AboutDialogResources = aboutDialogResource;
            this.LicensingDialogResources = licenseDialogResource;
        }

        private static int ExtractChannelId(string productId)
        {
            int num;
            if (productId == null)
            {
                return 0;
            }
            string[] strArrays = productId.Split(new char[] { '-' });
            if ((int)strArrays.Length != 4)
            {
                return 0;
            }
            if (strArrays[1] == "OEM")
            {
                return -1;
            }
            if (!int.TryParse(strArrays[1], out num))
            {
                return 0;
            }
            if (num >= 0 && num < 1000)
            {
                return num;
            }
            return 0;
        }

        private static int ExtractMpc(string productId)
        {
            int num;
            if (productId == null)
            {
                return 0;
            }
            string[] strArrays = productId.Split(new char[] { '-' });
            if ((int)strArrays.Length != 4)
            {
                return 0;
            }
            if (!int.TryParse(strArrays[0], out num))
            {
                return 0;
            }
            if (num >= 0 && num < 100000)
            {
                return num;
            }
            return 0;
        }

        public IList<string> FeaturesFromSku(Guid skuId)
        {
            return this.LicenseSkuFeatureMapper.FeaturesFromSku(skuId);
        }

        private static int FeaturesToLicenseClass(ICollection<string> features)
        {
            if (features.Contains(ExpressionFeatureMapper.StudioLicense))
            {
                if (features.Contains(ExpressionFeatureMapper.StudioPremiumLicense))
                {
                    return 4;
                }
                if (features.Contains(ExpressionFeatureMapper.StudioUltimateLicense))
                {
                    return 5;
                }
                if (features.Contains(ExpressionFeatureMapper.StudioWebLicense))
                {
                    return 6;
                }
            }
            if (features.Contains(ExpressionFeatureMapper.TrialLicense))
            {
                return 2;
            }
            if (features.Contains(ExpressionFeatureMapper.ProductLicense))
            {
                return 3;
            }
            return 0;
        }

        private string GetEnsuredPid(Guid effectiveSku)
        {
            bool flag = false;
            string productIdFromRegistry = LicenseService.GetProductIdFromRegistry(this.StudioPidRegistryPath);
            if (string.IsNullOrEmpty(productIdFromRegistry))
            {
                productIdFromRegistry = LicenseService.GetProductIdFromRegistry(this.ProductPidRegistryPath);
                if (!string.IsNullOrEmpty(productIdFromRegistry) && LicenseService.IsProductIdStructureValid(productIdFromRegistry))
                {
                    flag = true;
                }
            }
            else if (LicenseService.IsProductIdStructureValid(productIdFromRegistry))
            {
                flag = true;
            }
            int num = LicenseService.ExtractMpc(productIdFromRegistry);
            int num1 = LicenseService.ExtractChannelId(productIdFromRegistry);
            string partialPidFromSku = LicenseService.GetPartialPidFromSku(effectiveSku);
            if (!flag)
            {
                productIdFromRegistry = partialPidFromSku;
            }
            int num2 = this.Mpc(effectiveSku);
            if (num != num2 || num1 != LicenseService.ExtractChannelId(partialPidFromSku))
            {
                flag = false;
            }
            if (!flag)
            {
                productIdFromRegistry = this.ConstructPidAndInsertInRegistry(effectiveSku, partialPidFromSku, num2);
            }
            return productIdFromRegistry;
        }

        public DateTime GetExpiration(Guid skuId)
        {
            return DateTime.FromFileTimeUtc((long)this.SkuStatusFromSkuId(skuId).ValidityExpiration);
        }

        public string GetOfflineInstallationId()
        {
            if (this.lastAttemptedActivationSkuId == Guid.Empty)
            {
                return string.Empty;
            }
            return LicenseService.GetOfflineInstallationId(this.lastAttemptedActivationSkuId);
        }

        private static string GetOfflineInstallationId(Guid skuId)
        {
            bool flag = false;
            string empty = string.Empty;
            try
            {
                if (LicenseService.InitializeProductLicensing(true))
                {
                    flag = Licensing.Licensing.GenerateOfflineInstallationId(skuId, ref empty);
                }
            }
            finally
            {
                LicenseService.ShutdownProductLicensing(true);
            }
            if (flag)
            {
                return empty;
            }
            return string.Empty;
        }

        private static string GetPartialPidFromSku(Guid skuId)
        {
            Guid guid = skuId;
            string empty = string.Empty;
            Licensing.Licensing.GetPidFromProductSku(ref guid, ref empty);
            return empty;
        }

        private static string GetProductIdFromRegistry(string productIdSubKeyString)
        {
            string value;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(productIdSubKeyString);
            if (registryKey != null)
            {
                try
                {
                    value = (string)registryKey.GetValue("ProductID");
                }
                catch (ArgumentNullException argumentNullException)
                {
                    return string.Empty;
                }
                catch (ArgumentException argumentException)
                {
                    return string.Empty;
                }
                catch (IOException oException)
                {
                    return string.Empty;
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    return string.Empty;
                }
                catch (SecurityException securityException)
                {
                    return string.Empty;
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    return string.Empty;
                }
                return value;
            }
            return string.Empty;
        }

        public int GetRemainingGraceMinutes(Guid skuId)
        {
            return (int)this.SkuStatusFromSkuId(skuId).RemainingGraceMinutes;
        }

        private int GetSkuPriority(Licensing.Licensing.SkuLicenseStatus skuStatus)
        {
            if (skuStatus.SkuId == Guid.Empty)
            {
                return 0;
            }
            IList<string> strs = this.FeaturesFromSku(skuStatus.SkuId);
            int num = 0;
            if (strs.Contains(ExpressionFeatureMapper.UltimateLicense))
            {
                num++;
            }
            if (strs.Contains(ExpressionFeatureMapper.StudioLicense))
            {
                num++;
            }
            if (strs.Contains(ExpressionFeatureMapper.FullPackagedLicense))
            {
                num++;
            }
            if (strs.Contains(ExpressionFeatureMapper.ActivationLicense))
            {
                num++;
            }
            switch (skuStatus.Status)
            {
                case Licensing.Licensing.LicenseStatus.Unlicensed:
                    {
                        UnlicensedReason unlicensedErrorReason = (UnlicensedReason)skuStatus.UnlicensedErrorReason;
                        if (unlicensedErrorReason != UnlicensedReason.GraceTimeExpired)
                        {
                            if (unlicensedErrorReason == UnlicensedReason.ProductKeyNotInstalled)
                            {
                                return 20 + num;
                            }
                            return 10 + num;
                        }
                        if (this.HasKey(skuStatus.SkuId))
                        {
                            return 40 + num;
                        }
                        return 30 + num;
                    }
                case Licensing.Licensing.LicenseStatus.Licensed:
                    {
                        return 50 + num;
                    }
                case Licensing.Licensing.LicenseStatus.InGracePeriod:
                    {
                        return 40 + num;
                    }
            }
            throw new InvalidEnumArgumentException("skuStatus", (int)skuStatus.Status, typeof(Licensing.Licensing.LicenseStatus));
        }

        private IList<Guid> GetSkusThatCanBeActivated(IEnumerable<Guid> possibleActivationSkus)
        {
            IList<Guid> guids = new List<Guid>();
            if (possibleActivationSkus == null)
            {
                throw new ArgumentNullException("possibleActivationSkus");
            }
            try
            {
                if (LicenseService.InitializeProductLicensing(true))
                {
                    foreach (Guid possibleActivationSku in possibleActivationSkus)
                    {
                        if (this.IsLicensed(possibleActivationSku) || !this.IsActivatable(possibleActivationSku))
                        {
                            continue;
                        }
                        guids.Add(possibleActivationSku);
                    }
                }
            }
            finally
            {
                LicenseService.ShutdownProductLicensing(true);
            }
            return guids;
        }

        [CLSCompliant(false)]
        public UnlicensedReason GetUnlicensedReason(Guid skuId)
        {
            return (UnlicensedReason)this.SkuStatusFromSkuId(skuId).UnlicensedErrorReason;
        }

        public bool HasKey(Guid skuId)
        {
            List<Guid> guids;
            if (this.perSkuPKeys != null && this.perSkuPKeys.TryGetValue(skuId, out guids) && guids != null && guids.Count > 0)
            {
                return true;
            }
            return false;
        }

        private static bool InitializeProductLicensing()
        {
            return LicenseService.InitializeProductLicensing(false);
        }

        private static bool InitializeProductLicensing(bool initializeExtensionDll)
        {
            if (LicenseService.licensingInitializationCount > 0)
            {
                LicenseService.licensingInitializationCount = LicenseService.licensingInitializationCount + 1;
                return true;
            }
            if (Licensing.Licensing.SLDLInitialize() != Licensing.Licensing.InitializeLicensingStatus.Success)
            {
                return false;
            }
            LicenseService.licensingInitializationCount = LicenseService.licensingInitializationCount + 1;
            if (initializeExtensionDll && Licensing.Licensing.SLDLExtInitialize() != Licensing.Licensing.InitializeLicensingStatus.Success)
            {
                return false;
            }
            if (!Licensing.Licensing.RegisterLoggingCallback())
            {
                return false;
            }
            return true;
        }

        private static EnterKeyResponse InsertProductKeyIntoLicenseStore(string productKey, bool shouldActivate)
        {
            string str = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Microsoft.Expression.LicenseKeyInstaller.exe");
            string.IsNullOrEmpty(str);
            string str1 = productKey;
            if (shouldActivate)
            {
                str1 = string.Concat(str1, " activate");
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo(str, str1)
            {
                UseShellExecute = true,
                CreateNoWindow = true
            };
            return new EnterKeyResponse(LicenseService.CallExternalProcess(processStartInfo));
        }

        public bool IsActivatable(Guid skuId)
        {
            bool flag;
            bool flag1 = this.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(skuId);
            if (this.IsInGrace(skuId))
            {
                flag = true;
            }
            else
            {
                flag = (!this.IsUnlicensed(skuId) ? false : this.GetUnlicensedReason(skuId) == UnlicensedReason.GraceTimeExpired);
            }
            if (!flag1 || !flag)
            {
                return false;
            }
            return this.HasKey(skuId);
        }

        public bool IsAnySkuEnabled(IList<Guid> licenseSkus)
        {
            if (licenseSkus == null)
            {
                return false;
            }
            this.WaitForValidLicenseInformation();
            return this.IsAnySkuEnabledIn(licenseSkus, this.aggregatedLicenseInformation.Licenses);
        }

        private bool IsAnySkuEnabledIn(IList<Guid> licenseSkus, IEnumerable<LicenseInformation> licenses)
        {
            bool flag;
            if (licenses != null && this.perAppIdSkuStatusTable != null)
            {
                using (IEnumerator<LicenseInformation> enumerator = licenses.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        LicenseInformation current = enumerator.Current;
                        if (!this.perAppIdSkuStatusTable.ContainsKey(current.ApplicationId))
                        {
                            continue;
                        }
                        Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus>.Enumerator enumerator1 = this.perAppIdSkuStatusTable[current.ApplicationId].GetEnumerator();
                        try
                        {
                            while (enumerator1.MoveNext())
                            {
                                KeyValuePair<Guid, Licensing.Licensing.SkuLicenseStatus> keyValuePair = enumerator1.Current;
                                if (!licenseSkus.Contains(keyValuePair.Key) || keyValuePair.Value.Status == Licensing.Licensing.LicenseStatus.Unlicensed)
                                {
                                    continue;
                                }
                                flag = true;
                                return flag;
                            }
                        }
                        finally
                        {
                            ((IDisposable)enumerator1).Dispose();
                        }
                    }
                    return false;
                }
                return flag;
            }
            return false;
        }

        public bool IsInGrace(Guid skuId)
        {
            return this.SkuStatusFromSkuId(skuId).Status == Licensing.Licensing.LicenseStatus.InGracePeriod;
        }

        public bool IsLicensed(Guid skuId)
        {
            return this.SkuStatusFromSkuId(skuId).Status == Licensing.Licensing.LicenseStatus.Licensed;
        }

        private static bool IsProductIdStructureValid(string productId)
        {
            int num;
            int num1;
            int num2;
            int num3;
            if (productId == null)
            {
                return false;
            }
            string[] strArrays = productId.Split(new char[] { '-' });
            if ((int)strArrays.Length != 4)
            {
                return false;
            }
            if (!int.TryParse(strArrays[0], out num))
            {
                return false;
            }
            if (num < 0 || num >= 100000)
            {
                return false;
            }
            if (strArrays[1] != "OEM")
            {
                if (!int.TryParse(strArrays[1], out num1))
                {
                    return false;
                }
                if (num1 < 0 || num1 >= 1000)
                {
                    return false;
                }
            }
            if (!int.TryParse(strArrays[2], out num2))
            {
                return false;
            }
            if (num2 < 0 || num2 >= 10000000)
            {
                return false;
            }
            if (!int.TryParse(strArrays[3], out num3))
            {
                return false;
            }
            if (num3 >= 0 && num3 < 100000)
            {
                return true;
            }
            return false;
        }

        public bool IsUnlicensed(Guid skuId)
        {
            return this.SkuStatusFromSkuId(skuId).Status == Licensing.Licensing.LicenseStatus.Unlicensed;
        }

        private void LicensingInformationRetrievalWorker(object parameter)
        {
            this.UpdateLicensing();
            this.licensingFinished.Set();
        }

        public Guid MostPermissiveLicenseSku(IList<Guid> skuIds)
        {
            Licensing.Licensing.SkuLicenseStatus skuLicenseStatu = new Licensing.Licensing.SkuLicenseStatus();
            if (this.perAppIdSkuStatusTable != null && this.skuIdToAppIdTable != null)
            {
                foreach (Guid skuId in skuIds)
                {
                    if (!this.skuIdToAppIdTable.ContainsKey(skuId))
                    {
                        continue;
                    }
                    Guid item = this.skuIdToAppIdTable[skuId];
                    Licensing.Licensing.SkuLicenseStatus item1 = this.perAppIdSkuStatusTable[item][skuId];
                    if (this.GetSkuPriority(item1) <= this.GetSkuPriority(skuLicenseStatu))
                    {
                        continue;
                    }
                    skuLicenseStatu = item1;
                }
            }
            return skuLicenseStatu.SkuId;
        }

        private int Mpc(Guid licenseSkuGuid)
        {
            int num;
            List<ApplicationLicenses>.Enumerator enumerator = this.licenses.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ApplicationLicenses current = enumerator.Current;
                    if (!current.Licenses.ContainsSku(licenseSkuGuid))
                    {
                        continue;
                    }
                    int defaultMpc = current.DefaultMpc;
                    if (current.MpcDictionary.ContainsKey(CultureManager.ForcedCultureLcid.ToString(CultureInfo.InvariantCulture)))
                    {
                        Dictionary<string, int> mpcDictionary = current.MpcDictionary;
                        int forcedCultureLcid = CultureManager.ForcedCultureLcid;
                        defaultMpc = mpcDictionary[forcedCultureLcid.ToString(CultureInfo.InvariantCulture)];
                    }
                    num = defaultMpc;
                    return num;
                }
                return 99999;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            return num;
        }

        private void ProcessLicense(IEnumerable<LicenseInformation> licenses, IList<Guid> validSkus, List<Licensing.Licensing.SkuLicenseStatus> enabledLicenseSkuList)
        {
            List<Licensing.Licensing.SkuLicenseStatus> skuLicenseStatuses;
            List<Guid> guids;
            foreach (LicenseInformation license in licenses)
            {
                if (!license.ContainsAnySku(validSkus))
                {
                    continue;
                }
                Licensing.Licensing.OverallLicensingStatus licensingStatus = Licensing.Licensing.GetLicensingStatus(license.ApplicationId, out skuLicenseStatuses);
                Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus> guids1 = new Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus>();
                foreach (Licensing.Licensing.SkuLicenseStatus skuLicenseStatu in skuLicenseStatuses)
                {
                    Guid skuId = skuLicenseStatu.SkuId;
                    if (!license.ContainsSku(skuId))
                    {
                        continue;
                    }
                    this.skuIdToAppIdTable[skuId] = license.ApplicationId;
                    guids1[skuId] = skuLicenseStatu;
                    if (licensingStatus == Licensing.Licensing.OverallLicensingStatus.Success && (skuLicenseStatu.Status == Licensing.Licensing.LicenseStatus.InGracePeriod || skuLicenseStatu.Status == Licensing.Licensing.LicenseStatus.Licensed))
                    {
                        enabledLicenseSkuList.AddRange(skuLicenseStatuses);
                    }
                    if (Licensing.Licensing.GetProductKeyIds(skuId, out guids))
                    {
                        this.perSkuPKeys[skuId] = guids;
                    }
                    if (this.GetSkuPriority(skuLicenseStatu) <= this.GetSkuPriority(this.mostPermissiveLicense))
                    {
                        continue;
                    }
                    this.MostPermissiveApplicationId = license.ApplicationId;
                    this.MostPermissiveSkuId = skuLicenseStatu.SkuId;
                    this.mostPermissiveLicenseStatus = licensingStatus;
                    this.mostPermissiveLicense = skuLicenseStatu;
                }
                this.perAppIdSkuStatusTable[license.ApplicationId] = guids1;
            }
        }

        private void RecacheLicensingStatus()
        {
            this.UpdateLicenseInformationCache();
            this.RecordLicenseSku(this.MostPermissiveSkuId);
            this.MostPermissivePid = this.GetEnsuredPid(this.MostPermissiveSkuId);
        }

        private void RecordLicenseSku(Guid skuId)
        {
            IFeedbackService service = this.services.GetService<IFeedbackService>();
            if (service != null && this.services.GetService<ILicenseService>() != null)
            {
                int num = this.aggregatedLicenseInformation.FindSqmSkuId(skuId);
                int licenseClass = LicenseService.FeaturesToLicenseClass(this.FeaturesFromSku(skuId));
                if (num != 0)
                {
                    service.SetData(24, num);
                    service.SetData(41, licenseClass);
                }
            }
        }

        private static bool RetrieveExpressionLicenseLoggingPolicyFromRegistry()
        {
            return LicenseService.RetrieveLicenseLoggingPolicyFromRegistry("SOFTWARE\\Policies\\Microsoft\\Expression\\Licensing");
        }

        private static bool RetrieveLicenseLoggingPolicyFromRegistry(string registryPolicyKey)
        {
            bool flag;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPolicyKey);
            bool value = false;
            if (registryKey != null)
            {
                try
                {
                    value = (string)registryKey.GetValue("Logging") == "true";
                    return value;
                }
                catch (ArgumentNullException argumentNullException)
                {
                    flag = false;
                }
                catch (ArgumentException argumentException)
                {
                    flag = false;
                }
                catch (IOException oException)
                {
                    flag = false;
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    flag = false;
                }
                catch (SecurityException securityException)
                {
                    flag = false;
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                    flag = false;
                }
                return flag;
            }
            return value;
        }

        private static bool ShutdownProductLicensing()
        {
            return LicenseService.ShutdownProductLicensing(false);
        }

        private static bool ShutdownProductLicensing(bool shutdownExtensionDll)
        {
            int num = LicenseService.licensingInitializationCount - 1;
            LicenseService.licensingInitializationCount = num;
            if (num > 0)
            {
                return true;
            }
            if (Licensing.Licensing.SLDLShutdown() < 0)
            {
                return false;
            }
            if (shutdownExtensionDll && Licensing.Licensing.SLDLExtShutdown() < 0)
            {
                return false;
            }
            return true;
        }

        public IList<Guid> SkusFromFeature(string feature)
        {
            return this.LicenseSkuFeatureMapper.SkusFromFeature(feature);
        }

        private Licensing.Licensing.SkuLicenseStatus SkuStatusFromSkuId(Guid skuId)
        {
            Guid guid;
            Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus> guids;
            Licensing.Licensing.SkuLicenseStatus skuLicenseStatu;
            if (this.perAppIdSkuStatusTable != null && this.skuIdToAppIdTable != null && this.skuIdToAppIdTable.TryGetValue(skuId, out guid) && this.perAppIdSkuStatusTable.TryGetValue(guid, out guids) && guids.TryGetValue(skuId, out skuLicenseStatu))
            {
                return skuLicenseStatu;
            }
            return new Licensing.Licensing.SkuLicenseStatus();
        }

        public IList<string> SkuStringsFromFeature(string feature)
        {
            return this.LicenseSkuFeatureMapper.SkuStringsFromFeature(feature);
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.LicensingInformationRetrievalWorker), null);
        }

        public void Stop()
        {
            this.WaitForValidLicenseInformation();
            Licensing.Licensing.SLDLExtShutdown();
        }

        private void UpdateLicenseInformationCache()
        {
            this.MostPermissiveApplicationId = new Guid();
            this.MostPermissiveSkuId = new Guid();
            this.mostPermissiveLicense = new Licensing.Licensing.SkuLicenseStatus();
            this.mostPermissiveLicenseStatus = Licensing.Licensing.OverallLicensingStatus.ProductSkuNotInstalled;
            this.skuIdToAppIdTable = new Dictionary<Guid, Guid>();
            this.perAppIdSkuStatusTable = new Dictionary<Guid, Dictionary<Guid, Licensing.Licensing.SkuLicenseStatus>>();
            this.perSkuPKeys = new Dictionary<Guid, List<Guid>>();
            IList<Guid> guids = this.SkusFromFeature(this.productSpecifierFeature);
            try
            {
                List<Licensing.Licensing.SkuLicenseStatus> skuLicenseStatuses = new List<Licensing.Licensing.SkuLicenseStatus>();
                this.ProcessLicense(this.AggregatedLicenseInformation.Licenses, guids, skuLicenseStatuses);
            }
            catch (Exception exception)
            {
            }
        }

        public bool UpdateLicensing()
        {
            bool flag;
            try
            {
                if (!LicenseService.InitializeProductLicensing())
                {
                    flag = false;
                }
                else
                {
                    this.RecacheLicensingStatus();
                    flag = true;
                }
            }
            finally
            {
                LicenseService.ShutdownProductLicensing();
            }
            return flag;
        }

        private void UpdateRegistryWithProductId(Guid productSku, string newProductId)
        {
            RegistryKey registryKey;
            try
            {
                if (!this.FeaturesFromSku(productSku).Contains(ExpressionFeatureMapper.StudioLicense))
                {
                    registryKey = Registry.LocalMachine.CreateSubKey(this.ProductPidRegistryPath);
                    if (registryKey != null && newProductId != null)
                    {
                        registryKey.SetValue("ProductID", newProductId);
                    }
                }
                else
                {
                    registryKey = Registry.LocalMachine.CreateSubKey(this.StudioPidRegistryPath);
                    if (registryKey != null && newProductId != null)
                    {
                        registryKey.SetValue("ProductID", newProductId);
                    }
                }
            }
            catch (SecurityException securityException)
            {
            }
            catch (ArgumentNullException argumentNullException)
            {
            }
            catch (ArgumentException argumentException)
            {
            }
            catch (ObjectDisposedException objectDisposedException)
            {
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
            }
            catch (IOException oException)
            {
            }
        }

        public void WaitForValidLicenseInformation()
        {
            this.licensingFinished.WaitOne();
        }
    }
}
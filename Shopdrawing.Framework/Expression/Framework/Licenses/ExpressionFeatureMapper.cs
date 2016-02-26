// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.ExpressionFeatureMapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Framework.Licenses
{
  public class ExpressionFeatureMapper : ILicenseSkuFeatureMapper
  {
    public static readonly string MobileFeature = "mobile";
    public static readonly string WpfFeature = "wpf";
    public static readonly string SilverlightFeature = "silverlight";
    public static readonly string HobbledSketchFlowFeature = "hobbledsketchflow";
    public static readonly string SketchFlowFeature = "sketchflow";
    public static readonly string StudioLicense = "studio";
    public static readonly string FullPackagedLicense = "fpp";
    public static readonly string UltimateLicense = "ultimate";
    public static readonly string TrialLicense = "trial";
    public static readonly string ActivationLicense = "activation";
    public static readonly string RoyaltyCodecs = "royaltycodecs";
    public static readonly string Blend = "blend";
    public static readonly string Design = "design";
    public static readonly string Encoder = "encoder";
    public static readonly string Web = "web";
    public static readonly string StudioPremiumLicense = "studiopremium";
    public static readonly string StudioUltimateLicense = "studioultimate";
    public static readonly string StudioWebLicense = "studioweb";
    public static readonly string ProductLicense = "product";
    private readonly Dictionary<string, IList<string>> skuToFeatureMapping = new Dictionary<string, IList<string>>();
    private readonly Dictionary<string, IList<string>> featureToSkuMapping = new Dictionary<string, IList<string>>();

    protected void CreateCrossMapping()
    {
      foreach (KeyValuePair<string, IList<string>> keyValuePair in this.skuToFeatureMapping)
      {
        foreach (string key in (IEnumerable<string>) keyValuePair.Value)
        {
          if (this.featureToSkuMapping.ContainsKey(key))
          {
            IList<string> list = this.featureToSkuMapping[key];
            list.Add(keyValuePair.Key);
            this.featureToSkuMapping[key] = list;
          }
          else
          {
            IList<string> list = (IList<string>) new List<string>()
            {
              keyValuePair.Key
            };
            this.featureToSkuMapping[key] = list;
          }
        }
      }
    }

    public IList<string> FeaturesFromSku(Guid skuId)
    {
      string key = skuId.ToString("B");
      if (this.skuToFeatureMapping.ContainsKey(key))
        return this.skuToFeatureMapping[key];
      return (IList<string>) new List<string>();
    }

    public IList<string> SkuStringsFromFeature(string feature)
    {
      if (this.featureToSkuMapping.ContainsKey(feature))
        return this.featureToSkuMapping[feature];
      return (IList<string>) new List<string>();
    }

    public IList<Guid> SkusFromFeature(string feature)
    {
      return (IList<Guid>) Enumerable.ToList<Guid>(Enumerable.Select<string, Guid>(!this.featureToSkuMapping.ContainsKey(feature) ? (IEnumerable<string>) new List<string>() : (IEnumerable<string>) this.featureToSkuMapping[feature], (Func<string, Guid>) (skuIdString => new Guid(skuIdString))));
    }

    public IList<string> GetFeatures(string sku)
    {
      return this.skuToFeatureMapping[sku];
    }

    protected void SetFeatures(string sku, IList<string> feature)
    {
      this.skuToFeatureMapping[sku] = feature;
    }

    public IList<string> GetSkus(string feature)
    {
      return this.featureToSkuMapping[feature];
    }

    protected void SetSkus(string feature, IList<string> skus)
    {
      this.featureToSkuMapping[feature] = skus;
    }
  }
}

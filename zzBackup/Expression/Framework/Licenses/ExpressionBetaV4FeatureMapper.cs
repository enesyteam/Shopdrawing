// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.ExpressionBetaV4FeatureMapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class ExpressionBetaV4FeatureMapper : ExpressionFeatureMapper
  {
    public ExpressionBetaV4FeatureMapper()
    {
      List<string> list = new List<string>()
      {
        ExpressionFeatureMapper.SilverlightFeature,
        ExpressionFeatureMapper.WpfFeature
      };
      this.SetFeatures("{e197e308-61d1-4057-ba06-c3e8392e0fbd}", (IList<string>) new List<string>()
      {
        ExpressionFeatureMapper.SketchFlowFeature,
        ExpressionFeatureMapper.MobileFeature
      });
      this.SetFeatures("{0fc4f414-8785-46b5-80e7-5350c7937714}", (IList<string>) new List<string>((IEnumerable<string>) list)
      {
        ExpressionFeatureMapper.StudioLicense,
        ExpressionFeatureMapper.Blend,
        ExpressionFeatureMapper.Design,
        ExpressionFeatureMapper.Encoder,
        ExpressionFeatureMapper.Web,
        ExpressionFeatureMapper.HobbledSketchFlowFeature
      });
      this.SetFeatures("{d17e9278-a600-483e-9335-8336142a6c0e}", (IList<string>) new List<string>((IEnumerable<string>) this.GetFeatures("{0fc4f414-8785-46b5-80e7-5350c7937714}"))
      {
        ExpressionFeatureMapper.ActivationLicense
      });
      this.SetFeatures("{10f04870-5296-4be3-81f9-fd61794f7d1a}", (IList<string>) new List<string>((IEnumerable<string>) list)
      {
        ExpressionFeatureMapper.StudioLicense,
        ExpressionFeatureMapper.ActivationLicense,
        ExpressionFeatureMapper.Design,
        ExpressionFeatureMapper.Encoder,
        ExpressionFeatureMapper.Web
      });
      this.CreateCrossMapping();
    }
  }
}

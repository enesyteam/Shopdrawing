// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Licenses.ExpressionRtmV4FeatureMapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Licenses
{
  public sealed class ExpressionRtmV4FeatureMapper : ExpressionFeatureMapper
  {
    private readonly List<string> silverlightAndWpfFeatures = new List<string>()
    {
      ExpressionFeatureMapper.SilverlightFeature,
      ExpressionFeatureMapper.WpfFeature
    };

    public ExpressionRtmV4FeatureMapper()
    {
      this.SetFeatures("{7ff82fbc-d611-4eac-93a8-b58985e3074b}", (IList<string>) new List<string>((IEnumerable<string>) this.silverlightAndWpfFeatures)
      {
        ExpressionFeatureMapper.TrialLicense,
        ExpressionFeatureMapper.Blend,
        ExpressionFeatureMapper.SketchFlowFeature
      });
      this.SetFeatures("{0d5b4a1d-12e2-4c28-9a80-06f9e0a880b3}", (IList<string>) new List<string>()
      {
        ExpressionFeatureMapper.TrialLicense,
        ExpressionFeatureMapper.Design
      });
      this.SetFeatures("{df43e38d-b4cc-4153-ad69-381ff01d7d9c}", (IList<string>) new List<string>()
      {
        ExpressionFeatureMapper.TrialLicense,
        ExpressionFeatureMapper.Web
      });
      List<string> list1 = new List<string>((IEnumerable<string>) this.silverlightAndWpfFeatures)
      {
        ExpressionFeatureMapper.StudioLicense,
        ExpressionFeatureMapper.StudioPremiumLicense,
        ExpressionFeatureMapper.Blend,
        ExpressionFeatureMapper.Design,
        ExpressionFeatureMapper.Encoder,
        ExpressionFeatureMapper.Web
      };
      List<string> list2 = new List<string>((IEnumerable<string>) list1)
      {
        ExpressionFeatureMapper.ActivationLicense
      };
      this.SetFeatures("{463e13b0-6790-4187-9f8c-802e800c2d1a}", (IList<string>) new List<string>((IEnumerable<string>) list2)
      {
        ExpressionFeatureMapper.FullPackagedLicense
      });
      this.SetFeatures("{069de601-cfd0-4dbd-a5b5-0633730332a0}", (IList<string>) list2);
      this.SetFeatures("{c6e70907-d45f-4a6c-943c-750b0ccd59b3}", (IList<string>) list2);
      this.SetFeatures("{e9ee0197-890d-4d52-9aeb-256f78981d25}", (IList<string>) list1);
      this.SetFeatures("{d29b1461-3dc9-4148-910f-bd5c7e7fc2d1}", (IList<string>) list2);
      List<string> list3 = new List<string>((IEnumerable<string>) this.silverlightAndWpfFeatures)
      {
        ExpressionFeatureMapper.StudioLicense,
        ExpressionFeatureMapper.StudioUltimateLicense,
        ExpressionFeatureMapper.Blend,
        ExpressionFeatureMapper.Design,
        ExpressionFeatureMapper.Encoder,
        ExpressionFeatureMapper.Web,
        ExpressionFeatureMapper.SketchFlowFeature,
        ExpressionFeatureMapper.UltimateLicense
      };
      List<string> list4 = new List<string>((IEnumerable<string>) list3)
      {
        ExpressionFeatureMapper.ActivationLicense
      };
      List<string> list5 = new List<string>((IEnumerable<string>) list3)
      {
        ExpressionFeatureMapper.RoyaltyCodecs
      };
      List<string> list6 = new List<string>((IEnumerable<string>) list5)
      {
        ExpressionFeatureMapper.ActivationLicense
      };
      this.SetFeatures("{b8e73f9e-e01a-4f6c-815d-8eb0f54c26a4}", (IList<string>) new List<string>((IEnumerable<string>) list6)
      {
        ExpressionFeatureMapper.FullPackagedLicense
      });
      this.SetFeatures("{b4c18846-4ffa-4945-b455-97d3c005a1a4}", (IList<string>) list6);
      this.SetFeatures("{856b0b25-0069-4573-ba07-11fc7978cd72}", (IList<string>) list3);
      this.SetFeatures("{7f5bf21d-4ef0-43d0-bb95-59ac9e6c9724}", (IList<string>) list4);
      this.SetFeatures("{cf91f2ad-8883-44fa-be48-dc9544e730bf}", (IList<string>) list4);
      this.SetFeatures("{f4bbf0ae-cb89-47e7-9cac-559a1886036c}", (IList<string>) list5);
      List<string> list7 = new List<string>()
      {
        ExpressionFeatureMapper.StudioLicense,
        ExpressionFeatureMapper.StudioWebLicense,
        ExpressionFeatureMapper.Design,
        ExpressionFeatureMapper.Encoder,
        ExpressionFeatureMapper.Web
      };
      List<string> list8 = new List<string>((IEnumerable<string>) list7)
      {
        ExpressionFeatureMapper.ActivationLicense
      };
      this.SetFeatures("{d4fc22d4-1099-4520-baed-53f7bebc2dda}", (IList<string>) new List<string>((IEnumerable<string>) list8)
      {
        ExpressionFeatureMapper.FullPackagedLicense
      });
      this.SetFeatures("{cd98ea84-3800-4822-9e90-a250b51e5870}", (IList<string>) list8);
      this.SetFeatures("{6a17ac99-d3d6-4dc0-a682-b823790a66e9}", (IList<string>) list8);
      this.SetFeatures("{5fe02fcb-f830-477d-a83f-feb71e26ded3}", (IList<string>) list7);
      this.SetFeatures("{9b031466-127f-45b1-9aae-94dfcf9fda46}", (IList<string>) list8);
      List<string> list9 = new List<string>()
      {
        ExpressionFeatureMapper.ProductLicense,
        ExpressionFeatureMapper.RoyaltyCodecs,
        ExpressionFeatureMapper.Encoder
      };
      this.SetFeatures("{e27610de-1b2b-4c66-884e-b1a6820c44c2}", (IList<string>) new List<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) list9)
      {
        ExpressionFeatureMapper.ActivationLicense
      })
      {
        ExpressionFeatureMapper.FullPackagedLicense
      });
      this.SetFeatures("{e0cb3045-63a2-496c-aef2-17f27afc62ca}", (IList<string>) new List<string>((IEnumerable<string>) list9)
      {
        ExpressionFeatureMapper.FullPackagedLicense
      });
      this.SetFeatures("{5a269d7b-546f-4e2b-9b59-c115f074240d}", (IList<string>) list5);
      this.SetFeatures("{0f5ced6d-1c9b-47e4-a61b-a22c0e855268}", (IList<string>) list7);
      this.SetFeatures("{4bd8f8e5-c877-40a3-ae7f-8cfa524be8a1}", (IList<string>) list9);
      List<string> list10 = new List<string>()
      {
        ExpressionFeatureMapper.TrialLicense,
        ExpressionFeatureMapper.MobileFeature
      };
      List<string> list11 = new List<string>()
      {
        ExpressionFeatureMapper.ProductLicense,
        ExpressionFeatureMapper.MobileFeature
      };
      this.SetFeatures("{f688aae7-8353-4f77-a45c-b15abbc2dd0d}", (IList<string>) list10);
      this.SetFeatures("{def1f499-3ff3-4e86-afcc-afb5ffa89659}", (IList<string>) list11);
      this.CreateCrossMapping();
    }
  }
}

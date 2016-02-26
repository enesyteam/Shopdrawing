// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendSdkHelp
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Project;
using System.Runtime.Versioning;

namespace Shopdrawing.App
{
  internal sealed class BlendSdkHelp : Help
  {
    private static FrameworkNameDictionary<BlendSdkHelp> instances;
    private FrameworkName frameworkName;
    private string helpPath;

    public override bool Available
    {
      get
      {
        return BlendSdkHelper.IsSdkInstalled(this.frameworkName);
      }
    }

    protected override string HelpLocation
    {
      get
      {
        if (this.helpPath == null)
        {
          string helpPath = BlendSdkHelper.GetHelpPath(this.frameworkName);
          string helpFileName = BlendSdkHelper.GetHelpFileName(this.frameworkName);
          if (!string.IsNullOrEmpty(helpPath) && !string.IsNullOrEmpty(helpFileName))
            this.helpPath = Help.GetLocalizedHelpUrl(helpPath, helpFileName);
        }
        return this.helpPath;
      }
    }

    private BlendSdkHelp(FrameworkName frameworkName)
    {
      this.frameworkName = frameworkName;
    }

    public static BlendSdkHelp GetInstanceForFramework(FrameworkName frameworkName)
    {
      if (BlendSdkHelp.instances == null)
        BlendSdkHelp.instances = new FrameworkNameDictionary<BlendSdkHelp>();
      BlendSdkHelp blendSdkHelp;
      if (!BlendSdkHelp.instances.TryGetValue(frameworkName, out blendSdkHelp))
      {
        blendSdkHelp = new BlendSdkHelp(frameworkName);
        BlendSdkHelp.instances.Add(frameworkName, blendSdkHelp);
      }
      return blendSdkHelp;
    }
  }
}

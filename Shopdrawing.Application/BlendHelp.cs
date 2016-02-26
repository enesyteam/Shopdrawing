// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendHelp
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System.IO;

namespace Shopdrawing.App
{
  internal sealed class BlendHelp : Help
  {
    private static BlendHelp instance;
    private string helpPath;

    public static BlendHelp Instance
    {
      get
      {
        if (BlendHelp.instance == null)
          BlendHelp.instance = new BlendHelp();
        return BlendHelp.instance;
      }
    }

    public override bool Available
    {
      get
      {
        return true;
      }
    }

    protected override string HelpLocation
    {
      get
      {
        if (this.helpPath == null)
          this.helpPath = Help.GetLocalizedHelpUrl(Path.GetDirectoryName(typeof (Help).Assembly.Location), "Blend.chm");
        return this.helpPath;
      }
    }

    private BlendHelp()
    {
    }
  }
}

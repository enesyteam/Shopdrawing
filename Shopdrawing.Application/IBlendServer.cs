// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.IBlendServer
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

namespace Shopdrawing.App
{
  public interface IBlendServer
  {
    bool CanProcessCommandLineArgs(string[] args);

    void ProcessCommandLineArgs(string[] args);
  }
}

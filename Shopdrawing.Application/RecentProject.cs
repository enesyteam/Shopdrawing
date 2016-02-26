// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.RecentProject
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Framework.Documents;

namespace Shopdrawing.App
{
  public class RecentProject
  {
    private string name;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string NameWithoutExtension
    {
      get
      {
        return DocumentReference.Create(this.name).DisplayNameShort;
      }
    }

    public RecentProject(string name)
    {
      this.name = name;
    }

    public override string ToString()
    {
      return this.NameWithoutExtension;
    }
  }
}

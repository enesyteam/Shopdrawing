// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PathChangeInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class PathChangeInfo
  {
    public string NewPath { get; set; }

    public string OldPath { get; set; }

    public bool BreakingChange { get; set; }

    public IType TargetType { get; set; }

    public PathChangeInfo()
    {
    }

    public PathChangeInfo(string newPath, string oldPath, bool breakingChange)
    {
      this.NewPath = newPath;
      this.OldPath = oldPath;
      this.BreakingChange = breakingChange;
    }

    public override string ToString()
    {
      if (this.OldPath == null && this.NewPath == null)
        return "Empty path change";
      string str = this.BreakingChange ? "Breaking! " : "";
      if (this.OldPath != null && this.NewPath != null)
        str = str + this.OldPath + " -> " + this.NewPath;
      else if (this.OldPath != null)
        str = str + "New: " + this.NewPath;
      else if (this.NewPath != null)
        str = str + "Old: " + this.OldPath;
      return str;
    }
  }
}

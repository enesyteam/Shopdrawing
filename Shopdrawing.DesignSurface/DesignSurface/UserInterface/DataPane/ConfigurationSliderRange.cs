// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigurationSliderRange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ConfigurationSliderRange
  {
    public int Minimum { get; private set; }

    public int Maximum { get; private set; }

    public int Default { get; private set; }

    public double SmallIncrement { get; private set; }

    public double LargeIncrement { get; private set; }

    public ConfigurationSliderRange(int min, int max, int defaultValue)
    {
      this.Minimum = min;
      this.Maximum = max;
      this.Default = defaultValue;
      this.LargeIncrement = 1.0;
      this.SmallIncrement = 1.0;
    }
  }
}

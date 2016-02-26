// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TimeSpanSourceCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TimeSpanSourceCollection : ObservableCollection<string>
  {
    public TimeSpanSourceCollection()
    {
      this.Add("1x");
      this.Add("2x");
      this.Add("3x");
      this.Add("Forever");
    }
  }
}

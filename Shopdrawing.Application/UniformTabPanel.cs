// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.UniformTabPanel
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Shopdrawing.App
{
  public class UniformTabPanel : UniformGrid
  {
    static UniformTabPanel()
    {
      KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (UniformTabPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Once));
      KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof (UniformTabPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Cycle));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ComposedScreenPickerEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class ComposedScreenPickerEditor : ScreenPickerEditor
  {
    protected override IPrototypingScreen DefaultValue
    {
      get
      {
        return this.DesignerContext.PrototypingService.ActiveScreen;
      }
    }

    protected override IEnumerable<IPrototypingScreen> FindScreensToDisplay(IPrototypingScreen activeScreen)
    {
      if (this.PrototypingService == null)
        return Enumerable.Empty<IPrototypingScreen>();
      return Enumerable.Concat<IPrototypingScreen>(this.PrototypingService.FindComposedScreens(activeScreen), (IEnumerable<IPrototypingScreen>) new IPrototypingScreen[1]
      {
        activeScreen
      });
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.UtilityMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
  internal static class UtilityMethods
  {
    internal static void AddPresentationSourceCleanupAction(UIElement element, Action handler)
    {
      SourceChangedEventHandler relayHandler = (SourceChangedEventHandler) null;
      relayHandler = (SourceChangedEventHandler) ((sender, args) =>
      {
        if (args.NewSource != null)
          return;
        if (!element.Dispatcher.HasShutdownStarted)
          handler();
        PresentationSource.RemoveSourceChangedHandler((IInputElement) element, relayHandler);
      });
      PresentationSource.AddSourceChangedHandler((IInputElement) element, relayHandler);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: MS.Internal.HitTestHelper
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal
{
  internal static class HitTestHelper
  {
    internal static ViewHitTestResult HitTest(ViewItem reference, Point point, ViewHitTestFilterCallback filterCallback)
    {
      return reference.HitTest(filterCallback, (ViewHitTestResultCallback) null, (HitTestParameters) new PointHitTestParameters(point));
    }

    internal static HitTestResult HitTest(Visual reference, Point point, bool ignoreDisabled, HitTestFilterCallback filterCallback)
    {
      return HitTestHelper.HitTest(reference, (HitTestParameters) new PointHitTestParameters(point), ignoreDisabled, filterCallback);
    }

    public static HitTestResult HitTest(Visual reference, HitTestParameters hitTestParameters, bool ignoreDisabled, HitTestFilterCallback filterCallback)
    {
      HitTestResult hitTestResult = (HitTestResult) null;
      HitTestResultCallback resultCallback = (HitTestResultCallback) (hitItemsResult =>
      {
        hitTestResult = hitItemsResult;
        return HitTestResultBehavior.Stop;
      });
      HitTestFilterCallback filterCallback1 = (HitTestFilterCallback) (hit =>
      {
        UIElement uiElement = hit as UIElement;
        if (uiElement != null && (!uiElement.IsVisible || !uiElement.IsHitTestVisible || ignoreDisabled && !uiElement.IsEnabled))
          return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        HitTestFilterBehavior testFilterBehavior = HitTestFilterBehavior.Continue;
        if (filterCallback != null)
          testFilterBehavior = filterCallback(hit);
        return testFilterBehavior;
      });
      VisualTreeHelper.HitTest(reference, filterCallback1, resultCallback, hitTestParameters);
      return hitTestResult;
    }
  }
}

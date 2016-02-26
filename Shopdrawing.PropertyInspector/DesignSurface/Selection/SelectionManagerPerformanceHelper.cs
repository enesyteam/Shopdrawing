// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.SelectionManagerPerformanceHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Diagnostics;
using System;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public static class SelectionManagerPerformanceHelper
  {
    public static void MeasurePerformanceUntilPipelinePostSceneUpdate(SelectionManager selectionManager, PerformanceEvent performanceEvent)
    {
      if (!PerformanceUtility.LoggingEnabled)
        return;
      PerformanceUtility.StartPerformanceSequence(performanceEvent);
      EventHandler listenAndUnhookEvent = (EventHandler) null;
      listenAndUnhookEvent = (EventHandler) delegate
      {
        PerformanceUtility.EndPerformanceSequence(performanceEvent);
        selectionManager.PostSceneUpdatePhase -= listenAndUnhookEvent;
      };
      selectionManager.PostSceneUpdatePhase += listenAndUnhookEvent;
    }
  }
}

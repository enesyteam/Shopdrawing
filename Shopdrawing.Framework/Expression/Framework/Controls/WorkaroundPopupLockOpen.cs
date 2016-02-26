// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.WorkaroundPopupLockOpen
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
  internal class WorkaroundPopupLockOpen : IDisposable
  {
    private IWorkaroundPopupController controller;
    private bool lockState;

    public WorkaroundPopupLockOpen(DependencyObject dependencyObject)
    {
      this.controller = (IWorkaroundPopupController) dependencyObject.GetValue(WorkaroundPopup.WorkaroundPopupControllerProperty);
      if (this.controller == null)
        return;
      this.lockState = this.controller.FreezeClosingOnLostCapture;
      this.controller.FreezeClosingOnLostCapture = true;
    }

    public void Dispose()
    {
      if (this.controller != null)
        this.controller.FreezeClosingOnLostCapture = this.lockState;
      GC.SuppressFinalize((object) this);
    }
  }
}

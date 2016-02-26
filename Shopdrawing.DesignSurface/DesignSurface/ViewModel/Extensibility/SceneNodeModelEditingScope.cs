// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelEditingScope
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.Model;
using System;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.ViewModel.Extensibility
{
  public class SceneNodeModelEditingScope : ModelEditingScope
  {
    private ExtensibilityManager extensibilityManager;
    private SceneEditTransaction transaction;

    public SceneNodeModelEditingScope(ExtensibilityManager extensibilityManager, SceneEditTransaction transaction)
    {
      this.extensibilityManager = extensibilityManager;
      this.transaction = transaction;
    }

    protected override void OnComplete()
    {
      if (this.extensibilityManager == null)
        return;
      this.extensibilityManager.CompleteEditingScope(this.transaction);
      this.extensibilityManager = (ExtensibilityManager) null;
      this.transaction = (SceneEditTransaction) null;
    }

    protected override void OnRevert(bool finalizing)
    {
      if (finalizing)
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() => this.Cancel()));
      else
        this.Cancel();
    }

    private void Cancel()
    {
      if (this.extensibilityManager == null)
        return;
      this.extensibilityManager.CancelEditingScope(this.transaction);
      this.extensibilityManager = (ExtensibilityManager) null;
      this.transaction = (SceneEditTransaction) null;
    }

    protected override bool CanComplete()
    {
      if (this.extensibilityManager != null)
        return this.extensibilityManager.IsEditingScopeActive(this.transaction);
      return false;
    }

    public override void Update()
    {
      if (this.transaction == null)
        return;
      this.transaction.Update();
    }
  }
}

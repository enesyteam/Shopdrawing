// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.ActivationWizardPageViewModelBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal abstract class ActivationWizardPageViewModelBase : NotifyingObject
  {
    protected ActivationInfo ActivationInfo { get; private set; }

    public virtual bool IsValid
    {
      get
      {
        return true;
      }
    }

    public virtual bool SupportsNavigation
    {
      get
      {
        return true;
      }
    }

    public abstract ActivationWizardAction NextAction { get; }

    public abstract ActivationWizardAction PreviousAction { get; }

    public event EventHandler MoveNextRequested;

    protected ActivationWizardPageViewModelBase(ActivationInfo activationInfo)
    {
      this.ActivationInfo = activationInfo;
    }

    protected void RequestMoveNext()
    {
      if (this.MoveNextRequested == null)
        return;
      this.MoveNextRequested((object) this, EventArgs.Empty);
    }

    public virtual void Initialize()
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialogCreator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.ActivationDialog;
using Microsoft.Expression.Framework.ActivationDialog.ViewModel;
using System;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public sealed class ActivationDialogCreator : CommonDialogCreator
  {
    private ActivationWizardAction wizardStartAction;

    [CLSCompliant(false)]
    public override ILicensingDialogQuery GetInstance
    {
      get
      {
          ActivationDialog.ActivationDialog activationDialog = new ActivationDialog.ActivationDialog(this.Services, this.wizardStartAction);
        this.MergeLicensingResources((LicensingDialogBase) activationDialog);
        return (ILicensingDialogQuery) activationDialog;
      }
    }

    public ActivationDialogCreator(IServices services, ActivationWizardAction wizardStartAction)
      : base(services)
    {
      this.wizardStartAction = wizardStartAction;
    }
  }
}

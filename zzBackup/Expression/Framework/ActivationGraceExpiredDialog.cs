// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationGraceExpiredDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.ActivationDialog.ViewModel;

namespace Microsoft.Expression.Framework
{
  internal sealed class ActivationGraceExpiredDialog : LicensingDialog
  {
    public override bool ShowActivationCheckBox
    {
      get
      {
        return false;
      }
    }

    public override bool HasCancelButton
    {
      get
      {
        return true;
      }
    }

    public override string FallbackOptionText
    {
      get
      {
        return StringTable.LicensingMustActivateDialogActivateNowOption;
      }
    }

    public override string HeaderText
    {
      get
      {
        return StringTable.LicensingMustActivateDialogHeader;
      }
    }

    public ActivationGraceExpiredDialog(IServices services)
      : base(services)
    {
    }

    protected override bool ProcessFallbackOption()
    {
      return LicensingDialogHelper.ShowActivationDialog(this.Services, (CommonDialogCreator) new ActivationDialogCreator(this.Services, ActivationWizardAction.AttemptOnlineActivation));
    }
  }
}

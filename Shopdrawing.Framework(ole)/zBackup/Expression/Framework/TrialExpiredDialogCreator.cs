// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.TrialExpiredDialogCreator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using System;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public sealed class TrialExpiredDialogCreator : CommonDialogCreator
  {
    [CLSCompliant(false)]
    public override ILicensingDialogQuery GetInstance
    {
      get
      {
        TrialExpiredDialog trialExpiredDialog = new TrialExpiredDialog(this.Services);
        this.MergeLicensingResources((LicensingDialogBase) trialExpiredDialog);
        return (ILicensingDialogQuery) trialExpiredDialog;
      }
    }

    public TrialExpiredDialogCreator(IServices services)
      : base(services)
    {
    }
  }
}

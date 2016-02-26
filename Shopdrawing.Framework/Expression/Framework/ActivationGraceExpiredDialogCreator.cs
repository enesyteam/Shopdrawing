// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationGraceExpiredDialogCreator
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using System;

namespace Microsoft.Expression.Framework
{
  [CLSCompliant(false)]
  public sealed class ActivationGraceExpiredDialogCreator : CommonDialogCreator
  {
    [CLSCompliant(false)]
    public override ILicensingDialogQuery GetInstance
    {
      get
      {
        ActivationGraceExpiredDialog graceExpiredDialog = new ActivationGraceExpiredDialog(this.Services);
        this.MergeLicensingResources((LicensingDialogBase) graceExpiredDialog);
        return (ILicensingDialogQuery) graceExpiredDialog;
      }
    }

    public ActivationGraceExpiredDialogCreator(IServices services)
      : base(services)
    {
    }
  }
}

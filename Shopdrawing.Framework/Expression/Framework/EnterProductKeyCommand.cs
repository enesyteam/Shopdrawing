// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.EnterProductKeyCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.Framework
{
  public class EnterProductKeyCommand : Command
  {
    protected IServices Services { get; private set; }

    public EnterProductKeyCommand(IServices services)
    {
      this.Services = services;
    }

    public override void Execute()
    {
      LicensingDialogHelper.ShowLicensingDialog(this.Services, (CommonDialogCreator) new LicensingDialogCreator(this.Services));
    }
  }
}

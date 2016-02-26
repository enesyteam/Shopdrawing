// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Commands.AboutCommand
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using System;

namespace Microsoft.Expression.Framework.Commands
{
  public class AboutCommand : Command
  {
    private Version version;
    private IServices services;

    public AboutCommand(IServices services)
    {
      this.services = services;
      this.version = ExpressionApplication.Version;
    }

    public override void Execute()
    {
      new AboutDialog(this.services)
      {
        Version = this.version
      }.ShowDialog();
    }
  }
}

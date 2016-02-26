// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ShellOptionsPage
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Configuration;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class ShellOptionsPage : IOptionsPage
  {
    private IWindowService windowService;
    private ShellOptionsModel shellOptionsModel;
    private ShellOptionsControl shellOptionsControl;

    public object Content
    {
      get
      {
        if (this.shellOptionsModel == null)
          this.shellOptionsModel = new ShellOptionsModel(this.windowService);
        if (this.shellOptionsControl == null)
          this.shellOptionsControl = new ShellOptionsControl(this.shellOptionsModel);
        return (object) this.shellOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.ShellOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "Shell";
      }
    }

    public ShellOptionsPage(IWindowService windowService)
    {
      this.windowService = windowService;
    }

    public void Load(IConfigurationObject value)
    {
    }

    public void Commit()
    {
      this.shellOptionsModel = (ShellOptionsModel) null;
      this.shellOptionsControl = (ShellOptionsControl) null;
    }

    public void Cancel()
    {
      if (this.shellOptionsModel == null)
        return;
      this.shellOptionsModel.Cancel();
      this.shellOptionsModel = (ShellOptionsModel) null;
      this.shellOptionsControl = (ShellOptionsControl) null;
    }
  }
}

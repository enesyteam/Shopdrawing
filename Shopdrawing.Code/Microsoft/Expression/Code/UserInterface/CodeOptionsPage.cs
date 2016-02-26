// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.CodeOptionsPage
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Data;

namespace Microsoft.Expression.Code.UserInterface
{
  internal sealed class CodeOptionsPage : IOptionsPage
  {
    private EditingService editingService;
    private CodeOptionsModel codeOptionsModel;
    private CodeOptionsControl codeOptionsControl;
    private IConfigurationObject configurationObject;

    public object Content
    {
      get
      {
        if (this.codeOptionsModel == null)
        {
          this.codeOptionsModel = new CodeOptionsModel();
          this.codeOptionsModel.Load(this.configurationObject);
        }
        if (this.codeOptionsControl == null)
          this.codeOptionsControl = new CodeOptionsControl(this.codeOptionsModel, new DelegateCommand.SimpleEventHandler(this.ResetToDefault));
        return (object) this.codeOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.CodeOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "CodeEditor";
      }
    }

    public CodeOptionsPage(EditingService editingService)
    {
      this.editingService = editingService;
    }

    public void Load(IConfigurationObject value)
    {
      this.configurationObject = value;
      this.Apply();
    }

    public void Commit()
    {
      if (this.codeOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.codeOptionsModel.Save(this.configurationObject);
      this.codeOptionsModel = (CodeOptionsModel) null;
      this.codeOptionsControl = (CodeOptionsControl) null;
      this.Apply();
    }

    public void Cancel()
    {
      this.codeOptionsModel = (CodeOptionsModel) null;
      this.codeOptionsControl = (CodeOptionsControl) null;
    }

    private void ResetToDefault()
    {
      if (this.codeOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.codeOptionsModel.LoadMaintainingSelection(this.configurationObject);
    }

    private void Apply()
    {
      this.editingService.CodeOptionsModel.Load(this.configurationObject);
    }
  }
}

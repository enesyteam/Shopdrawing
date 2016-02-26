// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UnitsOptionsPage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Configuration;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class UnitsOptionsPage : IOptionsPage
  {
    private DesignerContext designerContext;
    private UnitsOptionsModel unitsOptionsModel;
    private UnitsOptionsControl unitsOptionsControl;
    private IConfigurationObject configurationObject;

    public object Content
    {
      get
      {
        if (this.unitsOptionsModel == null)
        {
          this.unitsOptionsModel = new UnitsOptionsModel(false);
          this.unitsOptionsModel.Load(this.configurationObject);
        }
        if (this.unitsOptionsControl == null)
          this.unitsOptionsControl = new UnitsOptionsControl(this.unitsOptionsModel);
        return (object) this.unitsOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.UnitsOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "Units";
      }
    }

    public UnitsOptionsPage(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public void Load(IConfigurationObject value)
    {
      this.configurationObject = value;
      this.Apply();
    }

    public void Commit()
    {
      if (this.unitsOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.unitsOptionsModel.Save(this.configurationObject);
      this.unitsOptionsModel = (UnitsOptionsModel) null;
      this.unitsOptionsControl = (UnitsOptionsControl) null;
      this.Apply();
    }

    public void Cancel()
    {
      this.unitsOptionsModel = (UnitsOptionsModel) null;
      this.unitsOptionsControl = (UnitsOptionsControl) null;
    }

    private void Apply()
    {
      this.designerContext.UnitsOptionsModel.Load(this.configurationObject);
    }
  }
}

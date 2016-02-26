// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ViewOptionsPage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Configuration;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class ViewOptionsPage : IOptionsPage
  {
    private DesignerContext designerContext;
    private ViewOptionsModel viewOptionsModel;
    private ViewOptionsControl viewOptionsControl;
    private IConfigurationObject configurationObject;

    public object Content
    {
      get
      {
        if (this.viewOptionsModel == null)
        {
          this.viewOptionsModel = new ViewOptionsModel(false);
          this.viewOptionsModel.Load(this.configurationObject);
        }
        if (this.viewOptionsControl == null)
          this.viewOptionsControl = new ViewOptionsControl(this.viewOptionsModel);
        return (object) this.viewOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.ViewOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "View";
      }
    }

    public ViewOptionsPage(DesignerContext designerContext)
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
      if (this.viewOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.viewOptionsModel.Save(this.configurationObject);
      this.viewOptionsModel = (ViewOptionsModel) null;
      this.viewOptionsControl = (ViewOptionsControl) null;
      this.Apply();
    }

    public void Cancel()
    {
      this.viewOptionsModel = (ViewOptionsModel) null;
      this.viewOptionsControl = (ViewOptionsControl) null;
    }

    private void Apply()
    {
      this.designerContext.ViewOptionsModel.Load(this.configurationObject);
    }
  }
}

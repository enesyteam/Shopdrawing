// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ArtboardOptionsPage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Configuration;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class ArtboardOptionsPage : IOptionsPage
  {
    private DesignerContext designerContext;
    private ArtboardOptionsModel artboardOptionsModel;
    private ArtboardOptionsControl artboardOptionsControl;
    private IConfigurationObject configurationObject;

    public object Content
    {
      get
      {
        if (this.artboardOptionsModel == null)
          this.artboardOptionsModel = new ArtboardOptionsModel(this.configurationObject, false);
        if (this.artboardOptionsControl == null)
          this.artboardOptionsControl = new ArtboardOptionsControl(this.artboardOptionsModel);
        return (object) this.artboardOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.ArtboardOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "ArtboardOptions";
      }
    }

    public ArtboardOptionsPage(DesignerContext designerContext)
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
      if (this.artboardOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.artboardOptionsModel.Save();
      this.artboardOptionsModel = (ArtboardOptionsModel) null;
      this.artboardOptionsControl = (ArtboardOptionsControl) null;
      this.Apply();
    }

    public void Cancel()
    {
      this.artboardOptionsModel = (ArtboardOptionsModel) null;
      this.artboardOptionsControl = (ArtboardOptionsControl) null;
    }

    private void Apply()
    {
      this.designerContext.ArtboardOptionsModel = new ArtboardOptionsModel(this.configurationObject, true);
    }
  }
}

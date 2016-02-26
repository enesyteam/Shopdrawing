// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationsOptionsPage
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Configuration;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal sealed class AnnotationsOptionsPage : IOptionsPage
  {
    private DesignerContext designerContext;
    private AnnotationsOptionsModel annotationsOptionsModel;
    private AnnotationsOptionsControl annotationsOptionsControl;
    private IConfigurationObject configurationObject;

    public object Content
    {
      get
      {
        if (this.annotationsOptionsModel == null)
        {
          this.annotationsOptionsModel = new AnnotationsOptionsModel(false);
          this.annotationsOptionsModel.Load(this.configurationObject);
          this.annotationsOptionsModel.InitAuthorInfoIfNeeded();
        }
        if (this.annotationsOptionsControl == null)
          this.annotationsOptionsControl = new AnnotationsOptionsControl(this.annotationsOptionsModel);
        return (object) this.annotationsOptionsControl;
      }
    }

    public string Title
    {
      get
      {
        return StringTable.AnnotationsOptionsPageTitle;
      }
    }

    public string Name
    {
      get
      {
        return "AnnotationsOptions";
      }
    }

    public AnnotationsOptionsPage(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
    }

    public void Load(IConfigurationObject configurationObject)
    {
      this.configurationObject = configurationObject;
      this.Apply();
    }

    public void Commit()
    {
      if (this.annotationsOptionsModel == null)
        return;
      this.configurationObject.Clear();
      this.annotationsOptionsModel.Save(this.configurationObject);
      this.annotationsOptionsModel = (AnnotationsOptionsModel) null;
      this.annotationsOptionsControl = (AnnotationsOptionsControl) null;
      this.Apply();
    }

    public void Cancel()
    {
      this.annotationsOptionsModel = (AnnotationsOptionsModel) null;
      this.annotationsOptionsControl = (AnnotationsOptionsControl) null;
    }

    private void Apply()
    {
      this.designerContext.AnnotationsOptionsModel.Load(this.configurationObject);
    }
  }
}

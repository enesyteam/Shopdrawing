// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BehaviorCommandCategoryEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class BehaviorCommandCategoryEditor : CategoryEditor
  {
    private string targetCategoryName;

    public virtual DataTemplate EditorTemplate
    {
      get
      {
        DataTemplate dataTemplate = new DataTemplate();
        dataTemplate.VisualTree = new FrameworkElementFactory(typeof (BehaviorCommandCategoryEditorControl));
        return dataTemplate;
      }
    }

    public virtual string TargetCategory
    {
      get
      {
        return this.targetCategoryName;
      }
    }

    public BehaviorCommandCategoryEditor(string targetCategoryName)
    {
      //this.\u002Ector();
      this.targetCategoryName = targetCategoryName;
    }

    public virtual object GetImage(Size desiredSize)
    {
      return null;
    }

    public virtual bool ConsumesProperty(PropertyEntry propertyEntry)
    {
      return true;
    }
  }
}

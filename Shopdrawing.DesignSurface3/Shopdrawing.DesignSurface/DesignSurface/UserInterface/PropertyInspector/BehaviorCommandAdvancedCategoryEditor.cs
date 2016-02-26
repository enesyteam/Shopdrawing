// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BehaviorCommandAdvancedCategoryEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public class BehaviorCommandAdvancedCategoryEditor : CategoryEditor
  {
    private string targetCategoryName;

    public override DataTemplate EditorTemplate
    {
      get
      {
        DataTemplate dataTemplate = new DataTemplate();
        dataTemplate.VisualTree = new FrameworkElementFactory(typeof (BehaviorCommandAdvancedCategoryEditorControl));
        return dataTemplate;
      }
    }

    public override string TargetCategory
    {
      get
      {
        return this.targetCategoryName;
      }
    }

    public BehaviorCommandAdvancedCategoryEditor(string targetCategoryName)
    {
      this.targetCategoryName = targetCategoryName;
    }

    public override object GetImage(Size desiredSize)
    {
      return (object) null;
    }

    public override bool ConsumesProperty(PropertyEntry propertyEntry)
    {
      return true;
    }
  }
}

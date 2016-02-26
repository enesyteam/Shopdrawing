// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor.SpecularMaterialEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor
{
  public class SpecularMaterialEditor : MaterialBaseEditor
  {
    private SceneNodeProperty specularPowerProperty;

    public override string Name
    {
      get
      {
        return StringTable.SpecularMaterialName;
      }
    }

    public override string ColorName
    {
      get
      {
        return StringTable.SpecularMaterialColorName;
      }
    }

    public override Type Type
    {
      get
      {
        return typeof (SpecularMaterial);
      }
    }

    public SceneNodeProperty SpecularPowerProperty
    {
      get
      {
        return this.specularPowerProperty;
      }
    }

    public double SpecularPower
    {
      get
      {
        return (double) this.specularPowerProperty.GetValue();
      }
      set
      {
        this.specularPowerProperty.SetValue((object) value);
      }
    }

    public SpecularMaterialEditor(SceneNodeObjectSet objectSet, PropertyReference materialReference)
      : base(objectSet, materialReference)
    {
      this.ColorProperty = objectSet.CreateSceneNodeProperty(materialReference.Append(MaterialNode.SpecularColorProperty), (AttributeCollection) null);
      this.BrushProperty = objectSet.CreateSceneNodeProperty(materialReference.Append(MaterialNode.SpecularBrushProperty), (AttributeCollection) null);
      this.specularPowerProperty = objectSet.CreateSceneNodeProperty(materialReference.Append(MaterialNode.SpecularPowerProperty), (AttributeCollection) null);
      this.specularPowerProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSpecularPowerChanged);
    }

    public override void Unhook()
    {
      if (this.specularPowerProperty != null)
      {
        this.specularPowerProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnSpecularPowerChanged);
        this.specularPowerProperty.OnRemoveFromCategory();
        this.specularPowerProperty = (SceneNodeProperty) null;
      }
      base.Unhook();
    }

    private void OnSpecularPowerChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.OnPropertyChanged("SpecularPower");
    }
  }
}

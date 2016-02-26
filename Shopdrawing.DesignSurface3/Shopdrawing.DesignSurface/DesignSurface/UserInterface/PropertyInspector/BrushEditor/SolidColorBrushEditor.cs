// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.SolidColorBrushEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal class SolidColorBrushEditor : BrushSubtypeEditor
  {
    private DataTemplate editorTemplate;
    private SceneNodeProperty colorProperty;
    private SceneNodeProperty opacityProperty;

    public override BrushCategory Category
    {
      get
      {
        return BrushCategory.SolidColor;
      }
    }

    public override DataTemplate EditorTemplate
    {
      get
      {
        return this.editorTemplate;
      }
    }

    public Color Color
    {
      get
      {
        return (Color) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.colorProperty.GetValue());
      }
      set
      {
        this.colorProperty.SetValue(this.BasisProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public SceneNodeProperty ColorProperty
    {
      get
      {
        return this.colorProperty;
      }
    }

    public SolidColorBrushEditor(BrushEditor brushEditor, SceneNodeProperty basisProperty)
      : base(brushEditor, basisProperty)
    {
      ReferenceStep step = (ReferenceStep) basisProperty.Reference.PlatformMetadata.ResolveProperty(SolidColorBrushNode.ColorProperty);
      this.colorProperty = this.RequestUpdates(basisProperty.Reference.Append(step), new PropertyChangedEventHandler(this.OnColorChanged));
      this.editorTemplate = new DataTemplate();
      this.editorTemplate.VisualTree = new FrameworkElementFactory(typeof (SolidColorBrushEditorControl));
      this.opacityProperty = this.RegisterProperty(BrushNode.OpacityProperty, new PropertyChangedEventHandler(this.OnOpacityChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.opacityProperty);
    }

    private void OnColorChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Color");
    }

    private void OnOpacityChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Opacity");
      this.OnPropertyChanged("TileBrush");
    }
  }
}

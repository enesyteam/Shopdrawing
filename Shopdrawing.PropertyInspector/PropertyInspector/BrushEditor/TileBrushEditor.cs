// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.TileBrushEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal class TileBrushEditor : BrushSubtypeEditor
  {
    private DataTemplate editorTemplate;
    private BrushCategory brushCategory;
    private SceneNodeProperty stretchProperty;
    private SceneNodeProperty tileModeProperty;
    private SceneNodeProperty relativeTransformProperty;
    private SceneNodeProperty opacityProperty;
    private SceneNodeProperty imageSourceProperty;

    public bool IsSilverlight { get; set; }

    public bool IsVideoBrush
    {
      get
      {
        return this.Category.Equals((object) BrushCategory.Video);
      }
    }

    public bool IsImageBrush
    {
      get
      {
        return this.Category.Equals((object) BrushCategory.Image);
      }
    }

    public override BrushCategory Category
    {
      get
      {
        return this.brushCategory;
      }
    }

    public SceneNodeProperty TileModeProperty
    {
      get
      {
        return this.tileModeProperty;
      }
    }

    public SceneNodeProperty StretchProperty
    {
      get
      {
        return this.stretchProperty;
      }
    }

    public SceneNodeProperty ImageSourceProperty
    {
      get
      {
        return this.imageSourceProperty;
      }
    }

    public override DataTemplate EditorTemplate
    {
      get
      {
        return this.editorTemplate;
      }
    }

    public TileBrush TileBrush
    {
      get
      {
        TileBrush tileBrush = (TileBrush) this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.BasisProperty.GetValue());
        if (tileBrush == null)
          return (TileBrush) null;
        if (!this.IsSilverlight)
          tileBrush = tileBrush.Clone();
        tileBrush.Viewport = new Rect(0.2, 0.2, 0.6, 0.6);
        tileBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
        tileBrush.Transform = (Transform) null;
        tileBrush.RelativeTransform = (Transform) null;
        tileBrush.AlignmentX = AlignmentX.Center;
        tileBrush.AlignmentY = AlignmentY.Center;
        return tileBrush;
      }
    }

    public Stretch Stretch
    {
      get
      {
        return (Stretch) TileBrushEditor.EnsureEnum(typeof (Stretch), this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.stretchProperty.GetValue()));
      }
      set
      {
        this.stretchProperty.SetValue(this.stretchProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public TileMode TileMode
    {
      get
      {
        return (TileMode) TileBrushEditor.EnsureEnum(typeof (TileMode), this.BasisProperty.SceneNodeObjectSet.DesignerContext.PlatformConverter.ConvertToWpf(this.BasisProperty.SceneNodeObjectSet.DocumentContext, this.tileModeProperty.GetValue()));
      }
      set
      {
        this.tileModeProperty.SetValue(this.tileModeProperty.SceneNodeObjectSet.ViewModel.DefaultView.ConvertFromWpfValue((object) value));
      }
    }

    public TileBrushEditor(BrushEditor brushEditor, ITypeId brushType, SceneNodeProperty basisProperty)
      : base(brushEditor, basisProperty)
    {
      if (PlatformTypes.VisualBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.Visual;
      else if (PlatformTypes.ImageBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.Image;
      else if (PlatformTypes.DrawingBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.Drawing;
      else if (PlatformTypes.VideoBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.Video;
      else if (PlatformTypes.WebBrowserBrush.Equals((object) brushType))
        this.brushCategory = BrushCategory.Html;
      this.editorTemplate = new DataTemplate();
      this.editorTemplate.VisualTree = new FrameworkElementFactory(typeof (TileBrushEditorControl));
      this.stretchProperty = this.RegisterProperty(TileBrushNode.StretchProperty, new PropertyChangedEventHandler(this.OnStretchChanged));
      this.tileModeProperty = this.RegisterProperty(TileBrushNode.TileModeProperty, new PropertyChangedEventHandler(this.OnTileModeChanged));
      this.imageSourceProperty = this.RegisterProperty(ImageBrushNode.ImageSourceProperty, new PropertyChangedEventHandler(this.OnImageSourceChanged));
      this.opacityProperty = this.RegisterProperty(BrushNode.OpacityProperty, new PropertyChangedEventHandler(this.OnOpacityChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.opacityProperty);
      this.relativeTransformProperty = this.RegisterProperty(BrushNode.RelativeTransformProperty, new PropertyChangedEventHandler(this.OnRelativeTransformChanged));
      this.AdvancedProperties.Add((PropertyEntry) this.relativeTransformProperty);
      this.IsSilverlight = !basisProperty.SceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf);
    }

    private static object EnsureEnum(Type enumType, object value)
    {
      object obj = value;
      string str = value as string;
      if (str != null)
        obj = Enum.Parse(enumType, str);
      return obj;
    }

    private void OnStretchChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Stretch");
      this.OnPropertyChanged("TileBrush");
    }

    private void OnTileModeChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("TileMode");
      this.OnPropertyChanged("TileBrush");
    }

    private void OnImageSourceChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("TileMode");
      this.OnPropertyChanged("TileBrush");
    }

    private void OnOpacityChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Opacity");
      this.OnPropertyChanged("TileBrush");
    }

    private void OnRelativeTransformChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("RelativeTransform");
      this.OnPropertyChanged("TileBrush");
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor.GradientStopEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.BrushEditor
{
  internal class GradientStopEditor : BrushPropertyEditor
  {
    private SceneNodeProperty gradientStopProperty;
    private SceneNodeProperty colorProperty;
    private SceneNodeProperty offsetProperty;
    private int propertyIndex;

    public Brush Brush
    {
      get
      {
        return (Brush) new SolidColorBrush(this.Color);
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

    public double Offset
    {
      get
      {
        return (double) this.offsetProperty.GetValue();
      }
      set
      {
        this.offsetProperty.SetValue((object) value);
      }
    }

    public int PropertyIndex
    {
      get
      {
        return this.propertyIndex;
      }
    }

    public string ToolTipContent
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.GradientStopToolTip, new object[2]
        {
          (object) this.Color.ToString(),
          (object) (this.Offset * 100.0)
        });
      }
    }

    public SceneNodeProperty ColorProperty
    {
      get
      {
        return this.colorProperty;
      }
    }

    public SceneNodeProperty OffsetProperty
    {
      get
      {
        return this.offsetProperty;
      }
    }

    public string Name
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Stop{0}", new object[1]
        {
          (object) this.propertyIndex
        });
      }
    }

    public GradientStopEditor(BrushEditor brushEditor, SceneNodeProperty basisProperty, int propertyIndex)
      : base(brushEditor, basisProperty)
    {
      ITypeResolver typeResolver = (ITypeResolver) basisProperty.SceneNodeObjectSet.ProjectContext;
      IType type = typeResolver.ResolveType(PlatformTypes.GradientStopCollection);
      if (type == null)
        return;
      IndexedClrPropertyReferenceStep referenceStep = IndexedClrPropertyReferenceStep.GetReferenceStep(typeResolver, type.RuntimeType, propertyIndex);
      PropertyReference propertyReference = basisProperty.Reference.Append((ReferenceStep) referenceStep);
      this.gradientStopProperty = basisProperty.SceneNodeObjectSet.CreateSceneNodeProperty(propertyReference, (AttributeCollection) null);
      ReferenceStep step1 = (ReferenceStep) typeResolver.ResolveProperty(GradientStopNode.ColorProperty);
      ReferenceStep step2 = (ReferenceStep) typeResolver.ResolveProperty(GradientStopNode.OffsetProperty);
      this.colorProperty = this.RequestUpdates(propertyReference.Append(step1), new PropertyChangedEventHandler(this.OnColorChanged));
      this.offsetProperty = this.RequestUpdates(propertyReference.Append(step2), new PropertyChangedEventHandler(this.OnOffsetChanged));
      this.propertyIndex = propertyIndex;
    }

    public override void Disassociate()
    {
      base.Disassociate();
      if (this.gradientStopProperty == null)
        return;
      this.gradientStopProperty.OnRemoveFromCategory();
      this.gradientStopProperty = (SceneNodeProperty) null;
    }

    public DocumentNode CacheDocumentNodeForGradientStop()
    {
      bool isMixed;
      return this.gradientStopProperty.GetLocalValueAsDocumentNode(true, out isMixed);
    }

    private void OnColorChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Color");
      this.OnPropertyChanged("Brush");
      this.OnPropertyChanged("ToolTipContent");
    }

    private void OnOffsetChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this.OnPropertyChanged("Offset");
      this.OnPropertyChanged("Brush");
      this.OnPropertyChanged("ToolTipContent");
    }
  }
}

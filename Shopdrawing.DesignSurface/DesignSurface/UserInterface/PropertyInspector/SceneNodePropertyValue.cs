// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodePropertyValue
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SceneNodePropertyValue : PropertyReferencePropertyValue, IDragEditNotifier
  {
    private SceneNodePropertyCollection subProperties;
    private SceneNodePropertyValueCollection children;
    private SceneNodeProperty property;
    private SceneViewModel viewModel;
    private bool isEditorDragging;

    public override bool HasSubProperties
    {
      get
      {
        TypeConverter converter = this.property.Converter;
        if (converter == null)
          return false;
        return converter.GetPropertiesSupported();
      }
    }

    public override PropertyEntryCollection SubProperties
    {
      get
      {
        if (this.subProperties == null)
          this.subProperties = new SceneNodePropertyCollection(this.property, this);
        return (PropertyEntryCollection) this.subProperties;
      }
    }

    public override bool IsCollection
    {
      get
      {
        return typeof (IList).IsAssignableFrom(this.property.PropertyType);
      }
    }

    public override PropertyValueCollection Collection
    {
      get
      {
        if (this.children == null)
          this.children = new SceneNodePropertyValueCollection(this.property, this);
        return (PropertyValueCollection) this.children;
      }
    }

    public bool IsPrimitiveDocumentNode
    {
      get
      {
        bool isMixed;
        return this.property.GetLocalValueAsDocumentNode(true, out isMixed) is DocumentPrimitiveNode;
      }
    }

    public override PropertyValueSource Source
    {
      get
      {
        return (PropertyValueSource) this.property.ValueSource;
      }
    }

    private bool IsAutoDraggableProperty
    {
      get
      {
        return this.property.Reference.FirstStep.Equals((object) BaseFrameworkElement.WidthProperty) || this.property.Reference.FirstStep.Equals((object) BaseFrameworkElement.HeightProperty) || this.property.Reference.FirstStep.Equals((object) LayoutPathNode.CapacityProperty);
      }
    }

    public double AutoValue
    {
      get
      {
        object obj = (object) null;
        if (this.property.SceneNodeObjectSet.IsSelectionInvalid)
          return double.NaN;
        if (this.property.Reference.FirstStep.Equals((object) LayoutPathNode.CapacityProperty))
          obj = this.property.SceneNodeObjectSet.GetValue(LayoutPathNode.ActualCapacityProperty);
        else if (this.property.Reference.FirstStep.Equals((object) BaseFrameworkElement.WidthProperty))
          obj = this.property.SceneNodeObjectSet.GetValue(BaseFrameworkElement.ActualWidthProperty);
        else if (this.property.Reference.FirstStep.Equals((object) BaseFrameworkElement.HeightProperty))
          obj = this.property.SceneNodeObjectSet.GetValue(BaseFrameworkElement.ActualHeightProperty);
        if (obj is double)
          return (double) obj;
        return double.NaN;
      }
    }

    public bool CanBeAuto
    {
      get
      {
        if (this.property.ValueEditorParameters != null)
        {
          NumberRangesAttribute numberRangesAttribute = (NumberRangesAttribute) this.property.ValueEditorParameters["NumberRanges"];
          if (numberRangesAttribute != null && (numberRangesAttribute.CanBeAuto.HasValue && numberRangesAttribute.CanBeAuto.Value))
            return this.property.SceneNodeObjectSet.ViewModel != null;
        }
        return false;
      }
    }

    public SceneNodePropertyValue(SceneNodeProperty property)
      : base((PropertyReferenceProperty) property)
    {
      this.property = property;
      if (!this.IsAutoDraggableProperty || this.property.SceneNodeObjectSet.ViewModel == null)
        return;
      this.viewModel = this.property.SceneNodeObjectSet.ViewModel;
      this.viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.OnPropertyChanged("AutoValue");
    }

    void IDragEditNotifier.BeginDragEdit()
    {
      this.isEditorDragging = true;
    }

    void IDragEditNotifier.EndDragEdit()
    {
      this.isEditorDragging = false;
    }

    protected override void NotifyPropertyReferenceChanged(PropertyReference propertyReference)
    {
      if (propertyReference.Count > this.property.Reference.Count)
        return;
      this.Rebuild();
      this.OnPropertyChanged("CanBeAuto");
      this.OnPropertyChanged("AutoValue");
      this.OnPropertyChanged("IsPrimitiveDocumentNode");
    }

    protected override void SetValueCore(object value)
    {
      bool flag = false;
      if (this.isEditorDragging && this.IsAutoDraggableProperty && (this.Value is double && double.IsNaN((double) this.Value)))
      {
        flag = this.TrySetAutoSizeProperty(LayoutPathNode.CapacityProperty, value, (IPropertyId) null, (object) null, (Func<Thickness, Size, double, Thickness>) null);
        if (!flag)
          flag = this.TrySetAutoSizeProperty(BaseFrameworkElement.WidthProperty, value, BaseFrameworkElement.HorizontalAlignmentProperty, (object) HorizontalAlignment.Stretch, new Func<Thickness, Size, double, Thickness>(this.AdjustWidth));
        if (!flag)
          flag = this.TrySetAutoSizeProperty(BaseFrameworkElement.HeightProperty, value, BaseFrameworkElement.VerticalAlignmentProperty, (object) VerticalAlignment.Stretch, new Func<Thickness, Size, double, Thickness>(this.AdjustHeight));
      }
      if (flag)
        return;
      base.SetValueCore(value);
    }

    private Thickness AdjustWidth(Thickness margin, Size renderSize, double value)
    {
      double num = renderSize.Width - value;
      margin.Right += num;
      return margin;
    }

    private Thickness AdjustHeight(Thickness margin, Size renderSize, double value)
    {
      double num = renderSize.Height - value;
      margin.Bottom += num;
      return margin;
    }

    private bool TrySetAutoSizeProperty(IPropertyId layoutProperty, object value, IPropertyId alignmentProperty, object autoAlignmentValue, Func<Thickness, Size, double, Thickness> adjustMarginFunction)
    {
      if (this.property.Reference.FirstStep.Equals((object) layoutProperty))
      {
        string description = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
        {
          (object) this.property.PropertyName
        });
        if (this.property.SceneNodeObjectSet.Document.IsEditable)
        {
          using (SceneEditTransaction editTransaction = this.property.SceneNodeObjectSet.ViewModel.CreateEditTransaction(description, false, SceneEditTransactionType.NestedInAutoClosing))
          {
            foreach (SceneNode sceneNode in this.property.SceneNodeObjectSet.Objects)
            {
              Base2DElement base2Delement = sceneNode as Base2DElement;
              if (base2Delement != null)
              {
                bool flag1 = base2Delement.GetComputedValueAsWpf(alignmentProperty).Equals(autoAlignmentValue);
                bool flag2 = value is double;
                bool flag3 = base2Delement.Parent == null || base2Delement.Parent is FrameworkTemplateElement;
                if (!flag1 || !flag2 || (!base2Delement.IsViewObjectValid || flag3))
                  base2Delement.SetValue(this.property.Reference, value);
                else if (base2Delement.Visual != null)
                {
                  Thickness thickness1 = (Thickness) base2Delement.GetComputedValueAsWpf(BaseFrameworkElement.MarginProperty);
                  Thickness thickness2 = adjustMarginFunction(thickness1, ((IViewVisual) base2Delement.Visual).RenderSize, (double) value);
                  base2Delement.SetValueAsWpf(BaseFrameworkElement.MarginProperty, (object) thickness2);
                }
              }
              else
                sceneNode.SetValue(this.property.Reference, value);
            }
            editTransaction.Commit();
          }
          this.property.SceneNodeObjectSet.TransactionContext.UpdateTransaction();
          return true;
        }
      }
      return false;
    }

    public void Rebuild()
    {
      if (this.subProperties != null)
        this.subProperties.Rebuild();
      if (this.children == null)
        return;
      this.children.Rebuild();
    }

    public override void Unhook()
    {
      if (this.IsAutoDraggableProperty && this.viewModel != null)
      {
        this.viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
        this.viewModel = (SceneViewModel) null;
      }
      if (this.subProperties != null)
        this.subProperties.Unhook();
      if (this.children != null)
        this.children.Unhook();
      base.Unhook();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.LayoutPositionCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class LayoutPositionCategory : SceneNodeCategory
  {
    private static IPropertyId wrapPanelOrientationProperty = (IPropertyId) ProjectNeutralTypes.WrapPanel.GetMember(MemberType.LocalProperty, "Orientation", MemberAccessTypes.Public);
    private static IPropertyId stackPanelOrientationProperty = (IPropertyId) PlatformTypes.StackPanel.GetMember(MemberType.LocalProperty, "Orientation", MemberAccessTypes.Public);
    private static readonly IPropertyId[] customProperties = new IPropertyId[13]
    {
      BaseFrameworkElement.WidthProperty,
      BaseFrameworkElement.HeightProperty,
      GridElement.ColumnProperty,
      GridElement.ColumnSpanProperty,
      GridElement.RowProperty,
      GridElement.RowSpanProperty,
      CanvasElement.LeftProperty,
      CanvasElement.TopProperty,
      PanelElement.ZIndexProperty,
      CanvasElement.CanvasZIndexProperty,
      DockPanelElement.DockProperty,
      LayoutPositionCategory.wrapPanelOrientationProperty,
      LayoutPositionCategory.stackPanelOrientationProperty
    };
    private static readonly IPropertyId[] advancedProperties = new IPropertyId[2]
    {
      ScrollViewerElement.HorizontalScrollBarVisibilityProperty,
      ScrollViewerElement.VerticalScrollBarVisibilityProperty
    };
    private bool isPanelVisible;
    private bool isGridVisible;
    private bool isDockVisible;
    private bool isCanvasVisible;
    private bool isChildStackVisible;
    private bool isChildWrapVisible;

    public bool IsPanelVisible
    {
      get
      {
        return this.isPanelVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isPanelVisible, value, "IsPanelVisible");
      }
    }

    public bool IsGridVisible
    {
      get
      {
        return this.isGridVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isGridVisible, value, "IsGridVisible");
      }
    }

    public bool IsDockVisible
    {
      get
      {
        return this.isDockVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isDockVisible, value, "IsDockVisible");
      }
    }

    public bool IsCanvasVisible
    {
      get
      {
        return this.isCanvasVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isCanvasVisible, value, "IsCanvasVisible");
      }
    }

    public bool IsChildStackVisible
    {
      get
      {
        return this.isChildStackVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isChildStackVisible, value, "IsChildStackVisible");
      }
    }

    public bool IsChildWrapVisible
    {
      get
      {
        return this.isChildWrapVisible;
      }
      private set
      {
        this.SetIfDifferent(ref this.isChildWrapVisible, value, "IsChildWrapVisible");
      }
    }

    public LayoutPositionCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Layout, localizedName, messageLogger)
    {
    }

    public static bool IsLayoutProperty(ReferenceStep referenceStep)
    {
      return Array.IndexOf<IPropertyId>(LayoutPositionCategory.customProperties, (IPropertyId) referenceStep) != -1;
    }

    public static bool IsLayoutAdvancedProperty(ReferenceStep referenceStep)
    {
      return Array.IndexOf<IPropertyId>(LayoutPositionCategory.advancedProperties, (IPropertyId) referenceStep) != -1;
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      ITypeId type1 = this.GetSelectedElementsType(selectedObjects) as ITypeId;
      ITypeId type2 = this.GetSelectedElementsParentType(selectedObjects) as ITypeId;
      this.IsChildStackVisible = PlatformTypes.StackPanel.IsAssignableFrom(type1);
      this.IsChildWrapVisible = ProjectNeutralTypes.WrapPanel.IsAssignableFrom(type1);
      this.IsGridVisible = PlatformTypes.Grid.IsAssignableFrom(type2) && (!PlatformTypes.RowDefinition.IsAssignableFrom(type1) && !PlatformTypes.ColumnDefinition.IsAssignableFrom(type1));
      this.IsDockVisible = ProjectNeutralTypes.DockPanel.IsAssignableFrom(type2);
      this.IsCanvasVisible = PlatformTypes.Canvas.IsAssignableFrom(type2);
      this.IsPanelVisible = PlatformTypes.Panel.IsAssignableFrom(type2);
    }

    private ITypeId GetTargetTypeId(SceneNode sceneNode)
    {
      StyleNode styleNode = sceneNode as StyleNode;
      return styleNode == null ? (ITypeId) sceneNode.Type : (ITypeId) styleNode.StyleTargetTypeId;
    }

    private object GetSelectedElementsType(SceneNode[] selectedObjects)
    {
      object obj = (object) null;
      foreach (SceneNode sceneNode in selectedObjects)
      {
        SceneElement sceneElement = sceneNode as SceneElement;
        if (sceneElement != null)
        {
          if (obj == null)
            obj = (object) this.GetTargetTypeId((SceneNode) sceneElement);
          else if (obj != this.GetTargetTypeId((SceneNode) sceneElement))
          {
            obj = MixedProperty.Mixed;
            break;
          }
        }
      }
      return obj;
    }

    private object GetSelectedElementsParentType(SceneNode[] selectedObjects)
    {
      object obj = (object) null;
      foreach (SceneNode sceneNode in selectedObjects)
      {
        SceneElement sceneElement = sceneNode as SceneElement;
        if (sceneElement != null && sceneElement.ParentElement != null)
        {
          if (obj == null)
            obj = (object) this.GetTargetTypeId((SceneNode) sceneElement.ParentElement);
          else if (obj != this.GetTargetTypeId((SceneNode) sceneElement.ParentElement))
          {
            obj = MixedProperty.Mixed;
            break;
          }
        }
      }
      return obj;
    }

    private void SetIfDifferent(ref bool value, bool newValue, string propertyName)
    {
      if (value == newValue)
        return;
      value = newValue;
      this.OnPropertyChanged(propertyName);
    }

    protected override bool DoesPropertyMatchFilter(PropertyFilter filter, PropertyEntry property)
    {
      ReferenceStep lastStep = ((PropertyReferenceProperty) property).Reference.LastStep;
      if (lastStep.Equals((object) BaseFrameworkElement.WidthProperty) || lastStep.Equals((object) BaseFrameworkElement.HeightProperty) || (lastStep.Equals((object) PanelElement.ZIndexProperty) || lastStep.Equals((object) CanvasElement.CanvasZIndexProperty)) || (lastStep.Equals((object) ScrollViewerElement.HorizontalScrollBarVisibilityProperty) || lastStep.Equals((object) ScrollViewerElement.VerticalScrollBarVisibilityProperty)))
      {
        if (!this.IsTargetingFrameworkElement)
          return false;
      }
      else if (lastStep.Equals((object) GridElement.ColumnProperty) || lastStep.Equals((object) GridElement.ColumnSpanProperty) || (lastStep.Equals((object) GridElement.RowProperty) || lastStep.Equals((object) GridElement.RowSpanProperty)))
      {
        if (!this.IsGridVisible)
          return false;
      }
      else if (lastStep.Equals((object) CanvasElement.LeftProperty) || lastStep.Equals((object) CanvasElement.TopProperty))
      {
        if (!this.IsCanvasVisible)
          return false;
      }
      else if (lastStep.Equals((object) PanelElement.ZIndexProperty) || lastStep.Equals((object) CanvasElement.CanvasZIndexProperty))
      {
        if (!this.IsPanelVisible)
          return false;
      }
      else if (lastStep.Equals((object) DockPanelElement.DockProperty))
      {
        if (!this.IsDockVisible)
          return false;
      }
      else if (lastStep.Equals((object) LayoutPositionCategory.wrapPanelOrientationProperty))
      {
        if (!this.IsChildWrapVisible)
          return false;
      }
      else if (lastStep.Equals((object) LayoutPositionCategory.stackPanelOrientationProperty) && !this.IsChildStackVisible)
        return false;
      return base.DoesPropertyMatchFilter(filter, property);
    }
  }
}

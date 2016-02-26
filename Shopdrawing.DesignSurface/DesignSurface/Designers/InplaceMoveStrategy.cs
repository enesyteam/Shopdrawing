// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.InplaceMoveStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal abstract class InplaceMoveStrategy : MoveStrategy
  {
    private IList<Transform> originalTransforms = (IList<Transform>) new List<Transform>();
    private List<Dictionary<IPropertyId, object>> storedValues = new List<Dictionary<IPropertyId, object>>();
    private readonly List<IPropertyId> propertiesToStore = new List<IPropertyId>()
    {
      Base2DElement.RenderTransformProperty,
      Base2DElement.OpacityProperty,
      PanelElement.ZIndexProperty
    };

    protected virtual IEnumerable<IPropertyId> PropertiesToStore
    {
      get
      {
        return (IEnumerable<IPropertyId>) this.propertiesToStore;
      }
    }

    internal InplaceMoveStrategy(MoveStrategyContext context)
      : base(context)
    {
    }

    protected void PrepareDraggedElementsForDragging()
    {
      this.originalTransforms.Clear();
      this.storedValues.Clear();
      foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
      {
        this.originalTransforms.Add((Transform) null);
        this.storedValues.Add((Dictionary<IPropertyId, object>) null);
      }
      bool flag = false;
      for (int index = 0; index < this.DraggedElements.Count; ++index)
      {
        BaseFrameworkElement frameworkElement = this.DraggedElements[index];
        if (frameworkElement.IsAttached)
        {
          SceneElement sceneElement = frameworkElement.Parent as SceneElement;
          if (sceneElement != null && PlatformTypes.InlineUIContainer.IsAssignableFrom((ITypeId) sceneElement.Type))
            sceneElement = sceneElement.Parent as SceneElement;
          if (sceneElement != null && sceneElement.GetChildren() != null && frameworkElement.Visual != null)
          {
            this.storedValues[index] = InplaceMoveStrategy.StoreProperties(frameworkElement.Platform, frameworkElement.Visual, this.PropertiesToStore);
            this.SetPropertyOnVisual((SceneElement) frameworkElement, PanelElement.ZIndexProperty, (object) int.MaxValue);
            this.SetPropertyOnVisual((SceneElement) frameworkElement, Base2DElement.OpacityProperty, (object) 0.5);
            frameworkElement.IsPlaceholder = true;
            flag = true;
          }
        }
      }
      this.Context.Transaction.UpdateEditTransaction();
      if (!flag)
        return;
      this.Context.ToolBehaviorContext.Tool.RebuildAdornerSets();
    }

    protected void RestoreDraggedElements()
    {
      bool flag = false;
      for (int index = 0; index < this.DraggedElements.Count; ++index)
      {
        BaseFrameworkElement frameworkElement = this.DraggedElements[index];
        if (frameworkElement.Visual != null && this.storedValues[index] != null)
        {
          InplaceMoveStrategy.RestoreProperties((ITypeResolver) frameworkElement.ProjectContext, frameworkElement.Visual, this.PropertiesToStore, this.storedValues[index]);
          frameworkElement.IsPlaceholder = false;
          flag = true;
        }
      }
      if (!flag)
        return;
      this.Context.ToolBehaviorContext.Tool.RebuildAdornerSets();
    }

    private void SetPropertyOnVisual(SceneElement element, IPropertyId property, object value)
    {
      if (element == null || element.Visual == null)
        return;
      IProperty propertyKey = element.ProjectContext.ResolveProperty(property);
      if (propertyKey == null)
        return;
      element.Visual.SetValue((ITypeResolver) element.ProjectContext, propertyKey, value);
    }

    private static Dictionary<IPropertyId, object> StoreProperties(IPlatform platform, IViewObject viewObject, IEnumerable<IPropertyId> properties)
    {
      Dictionary<IPropertyId, object> dictionary = new Dictionary<IPropertyId, object>();
      foreach (IPropertyId propertyId in properties)
      {
        IProperty propertyKey = platform.Metadata.ResolveProperty(propertyId);
        if (propertyKey != null && viewObject.IsSet(propertyKey))
          dictionary[(IPropertyId) propertyKey] = viewObject.GetValue(propertyKey);
      }
      return dictionary;
    }

    private static void RestoreProperties(ITypeResolver typeResolver, IViewObject viewObject, IEnumerable<IPropertyId> storedProperties, Dictionary<IPropertyId, object> values)
    {
      foreach (IPropertyId propertyId in storedProperties)
      {
        IProperty propertyKey = typeResolver.ResolveProperty(propertyId);
        if (propertyKey != null)
        {
          object obj;
          if (values.TryGetValue((IPropertyId) propertyKey, out obj))
            viewObject.SetValue(typeResolver, propertyKey, obj);
          else
            viewObject.ClearValue(propertyKey);
        }
      }
    }

    protected virtual void AdjustIndexBeforeRemovingFromSceneView()
    {
    }

    protected virtual void AdjustIndexAfterRemovingFromSceneView()
    {
    }

    protected int ComputeIndexOffsetBeforeRemove(int index)
    {
      int num1 = 0;
      if (index >= 0)
      {
        ISceneNodeCollection<SceneNode> defaultContent = this.LayoutContainer.DefaultContent;
        if (defaultContent != null && defaultContent.Count > 0)
        {
          foreach (BaseFrameworkElement frameworkElement in this.DraggedElements)
          {
            if (this.LayoutContainer == frameworkElement.Parent)
            {
              int num2 = defaultContent.IndexOf((SceneNode) frameworkElement);
              if (num2 >= 0 && num2 < index)
                --num1;
            }
          }
        }
      }
      return num1;
    }

    protected void MoveDraggedElementsWithTempTransform()
    {
      Point point1 = this.DragCurrentPosition;
      if (this.IsConstraining)
        point1 = this.ConstrainPointToAxis(this.DragStartPosition, this.DragCurrentPosition);
      for (int index = 0; index < this.DraggedElements.Count; ++index)
      {
        BaseFrameworkElement frameworkElement = this.DraggedElements[index];
        if (this.originalTransforms[index] == null)
        {
          Transform transform = (Transform) frameworkElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
          this.originalTransforms[index] = transform == null ? Transform.Identity : transform;
        }
        IViewVisual viewVisual = frameworkElement.Visual as IViewVisual;
        if (viewVisual != null)
        {
          SceneView activeView = this.ActiveView;
          Point point2 = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, viewVisual.VisualParent, this.DragStartPosition);
          Vector vector = activeView.TransformPoint((IViewObject) activeView.HitTestRoot, viewVisual.VisualParent, point1) - point2 - this.Context.DuplicationOffset;
          Matrix matrix = this.originalTransforms[index].Value;
          matrix.Translate(vector.X, vector.Y);
          Transform transform = (Transform) new MatrixTransform(matrix);
          viewVisual.SetValue((ITypeResolver) this.ActiveSceneViewModel.ProjectContext, this.ActiveSceneViewModel.ProjectContext.ResolveProperty(Base2DElement.RenderTransformProperty), this.ActiveView.ConvertFromWpfValue((object) transform));
        }
      }
    }
  }
}

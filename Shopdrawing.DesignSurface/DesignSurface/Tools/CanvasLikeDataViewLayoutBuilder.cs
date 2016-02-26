// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.CanvasLikeDataViewLayoutBuilder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public class CanvasLikeDataViewLayoutBuilder : PanelDataViewLayoutBuilder
  {
    private static Size defaultInnerMargin = new Size(4.0, 4.0);
    private List<CanvasLikeDataViewLayoutBuilder.LabelField> records = new List<CanvasLikeDataViewLayoutBuilder.LabelField>();
    private bool shouldCreateLabels;

    public CanvasLikeDataViewLayoutBuilder(int insertIndex, bool shouldCreateLabels)
      : base(insertIndex)
    {
      this.shouldCreateLabels = shouldCreateLabels;
    }

    public override void SetElementLayout(DataViewBuilderContext context)
    {
      if (!this.shouldCreateLabels)
        context.CurrentLabelNode = (DocumentCompositeNode) null;
      this.records.Add(new CanvasLikeDataViewLayoutBuilder.LabelField((DocumentNode) context.CurrentLabelNode, (DocumentNode) context.CurrentFieldNode));
      base.SetElementLayout(context);
    }

    public Size CalcCombinedSize(SceneViewModel viewModel)
    {
      Size size = new Size();
      for (int index = 0; index < this.records.Count; ++index)
      {
        if (index > 0)
          size.Height += CanvasLikeDataViewLayoutBuilder.defaultInnerMargin.Height;
        CanvasLikeDataViewLayoutBuilder.LabelField labelField = this.records[index];
        BaseFrameworkElement frameworkElement = (BaseFrameworkElement) viewModel.GetSceneNode(labelField.Label);
        Rect computedTightBounds1 = ((Base2DElement) viewModel.GetSceneNode(labelField.Field)).GetComputedTightBounds();
        if (frameworkElement != null)
        {
          Rect computedTightBounds2 = frameworkElement.GetComputedTightBounds();
          size.Height += Math.Max(computedTightBounds2.Height, computedTightBounds1.Height);
          size.Width = Math.Max(size.Width, computedTightBounds2.Width + computedTightBounds1.Width + CanvasLikeDataViewLayoutBuilder.defaultInnerMargin.Width);
        }
        else
        {
          size.Width += computedTightBounds1.Width;
          size.Height += computedTightBounds1.Height;
        }
      }
      return size;
    }

    public void RebindFields(SceneViewModel viewModel, ISchema schema, IList<DataSchemaNodePath> viewEntries)
    {
      for (int index = 0; index < viewEntries.Count; ++index)
      {
        DataSchemaNodePath dataSchemaNodePath = viewEntries[index];
        DataSchemaNodePath bindingPath = new DataSchemaNodePath(schema, dataSchemaNodePath.Node);
        DocumentCompositeNode fieldNode = (DocumentCompositeNode) this.records[index].Field;
        SceneNode sceneNode = viewModel.GetSceneNode((DocumentNode) fieldNode);
        IProperty property = Enumerable.First<IProperty>((IEnumerable<IProperty>) fieldNode.Properties.Keys, (Func<IProperty, bool>) (p => fieldNode.Properties[(IPropertyId) p].Type.IsBinding));
        viewModel.BindingEditor.CreateAndSetBindingOrData(sceneNode, (IPropertyId) property, bindingPath);
      }
    }

    public void ApplyActiveUserThemeStyle(SceneViewModel viewModel)
    {
      for (int index = 0; index < this.records.Count; ++index)
      {
        CanvasLikeDataViewLayoutBuilder.LabelField labelField = this.records[index];
        AssetLibrary.ApplyActiveUserThemeStyle(viewModel.GetSceneNode(labelField.Label));
        AssetLibrary.ApplyActiveUserThemeStyle(viewModel.GetSceneNode(labelField.Field));
      }
    }

    public void CompleteLayout(SceneViewModel viewModel, Point startPoint)
    {
      ILayoutDesigner layoutDesigner = (ILayoutDesigner) null;
      Point point = startPoint;
      for (int index = 0; index < this.records.Count; ++index)
      {
        if (index > 0)
          point.Y += CanvasLikeDataViewLayoutBuilder.defaultInnerMargin.Height;
        CanvasLikeDataViewLayoutBuilder.LabelField labelField = this.records[index];
        BaseFrameworkElement element1 = (BaseFrameworkElement) viewModel.GetSceneNode(labelField.Label);
        BaseFrameworkElement element2 = (BaseFrameworkElement) viewModel.GetSceneNode(labelField.Field);
        if (layoutDesigner == null)
          layoutDesigner = viewModel.GetLayoutDesignerForChild((SceneElement) element2, true);
        Rect rect1 = element1 != null ? element1.GetComputedTightBounds() : Rect.Empty;
        Rect computedTightBounds = element2.GetComputedTightBounds();
        Point location = point;
        double num = element1 != null ? Math.Max(rect1.Height, computedTightBounds.Height) : computedTightBounds.Height;
        if (element1 != null)
        {
          location.Y += (num - rect1.Height) / 2.0;
          Rect rect2 = new Rect(location, rect1.Size);
          CanvasLikeDataViewLayoutBuilder.SetElementRect(layoutDesigner, element1, rect2);
          location.Y -= (num - rect1.Height) / 2.0;
          location.X = rect2.Right + CanvasLikeDataViewLayoutBuilder.defaultInnerMargin.Width;
        }
        location.Y += (num - computedTightBounds.Height) / 2.0;
        Rect rect3 = new Rect(location, computedTightBounds.Size);
        CanvasLikeDataViewLayoutBuilder.SetElementRect(layoutDesigner, element2, rect3);
        double right = rect3.Right;
        double y = point.Y;
        point.Y += num;
      }
    }

    public void SelectElements(SceneViewModel viewModel)
    {
      List<SceneElement> list = new List<SceneElement>(this.records.Count * 2);
      for (int index = 0; index < this.records.Count; ++index)
      {
        CanvasLikeDataViewLayoutBuilder.LabelField labelField = this.records[index];
        SceneElement sceneElement1 = (SceneElement) viewModel.GetSceneNode(labelField.Label);
        SceneElement sceneElement2 = (SceneElement) viewModel.GetSceneNode(labelField.Field);
        if (sceneElement1 != null)
          list.Add(sceneElement1);
        list.Add(sceneElement2);
      }
      if (list.Count < 1)
        return;
      viewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, (SceneElement) null);
    }

    private static void SetElementRect(ILayoutDesigner layoutDesigner, BaseFrameworkElement element, Rect rect)
    {
      layoutDesigner.SetChildRect(element, rect, LayoutOverrides.HorizontalAlignment | LayoutOverrides.VerticalAlignment | LayoutOverrides.Width | LayoutOverrides.Height, LayoutOverrides.None, LayoutOverrides.None);
      element.ClearLocalValue(DesignTimeProperties.LayoutOverridesProperty);
    }

    private class LabelField
    {
      public DocumentNode Label { get; private set; }

      public DocumentNode Field { get; private set; }

      public LabelField(DocumentNode label, DocumentNode field)
      {
        this.Label = label;
        this.Field = field;
      }
    }
  }
}

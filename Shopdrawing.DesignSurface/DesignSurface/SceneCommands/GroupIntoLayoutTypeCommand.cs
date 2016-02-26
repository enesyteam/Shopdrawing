// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.GroupIntoLayoutTypeCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class GroupIntoLayoutTypeCommand : LayoutTypeCommandBase
  {
    private bool IsDragDropContainer { get; set; }

    public GroupIntoLayoutTypeCommand(SceneViewModel viewModel, ITypeId type, bool isDragDropContainer)
      : base(viewModel, type)
    {
      this.IsDragDropContainer = isDragDropContainer;
    }

    public override void Execute(object arg)
    {
      DesignerContext designerContext = this.DesignerContext;
      SceneViewModel activeSceneViewModel = designerContext.ActiveSceneViewModel;
      ITypeId type = this.Type;
      if (type is ProjectNeutralTypeId)
      {
        bool flag = activeSceneViewModel.ProjectContext.PlatformMetadata.IsSupported((ITypeResolver) this.DesignerContext.ActiveSceneViewModel.ProjectContext, type);
        if (!flag && this.IsDragDropContainer)
        {
          IMessageDisplayService messageDisplayService = activeSceneViewModel.DesignerContext.MessageDisplayService;
          flag = ToolkitHelper.EnsureSilverlightToolkitTypeAvailable((ITypeResolver) activeSceneViewModel.ProjectContext, type, messageDisplayService, StringTable.SilverlightToolkitDragDropNotInstalled, StringTable.SilverlightToolkitDragDropIncorrectVersion);
        }
        if (!flag)
          return;
      }
      using (SceneEditTransaction editTransaction = designerContext.ActiveDocument.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitGroupIntoLayoutType, new object[1]
      {
        (object) this.Type.Name
      })))
      {
        List<SceneElement> list1 = new List<SceneElement>();
        List<SceneNode> elements = new List<SceneNode>();
        list1.AddRange((IEnumerable<SceneElement>) designerContext.SelectionManager.ElementSelectionSet.Selection);
        GroupIntoLayoutTypeCommand.OrientationHelper orientationHelper = new GroupIntoLayoutTypeCommand.OrientationHelper();
        list1.Sort((IComparer<SceneElement>) new ZOrderComparer<SceneElement>(this.SceneViewModel.RootNode));
        List<LayoutCacheRecord> list2 = new List<LayoutCacheRecord>(list1.Count);
        Rect empty = Rect.Empty;
        for (int index = 0; index < list1.Count; ++index)
        {
          list2.Add((LayoutCacheRecord) null);
          BaseFrameworkElement element = list1[index] as BaseFrameworkElement;
          if (element != null)
          {
            ILayoutDesigner designerForChild = element.ViewModel.GetLayoutDesignerForChild((SceneElement) element, true);
            Rect roundedUpChildRect = LayoutRoundingHelper.GetRoundedUpChildRect(designerForChild, element);
            empty.Union(roundedUpChildRect);
            orientationHelper.AddChildRect((SceneNode) element, roundedUpChildRect);
            LayoutCacheRecord layoutCacheRecord = designerForChild.CacheLayout(element);
            list2[index] = layoutCacheRecord;
            elements.Add((SceneNode) element);
          }
        }
        Dictionary<IPropertyId, SceneNode> properties = (Dictionary<IPropertyId, SceneNode>) null;
        using (this.SceneViewModel.ForceBaseValue())
        {
          using (this.SceneViewModel.DisableDrawIntoState())
          {
            SceneElement sceneElement1 = (SceneElement) null;
            SceneElement sceneElement2 = (SceneElement) activeSceneViewModel.CreateSceneNode(type);
            Orientation? nullable = orientationHelper.ApplyOrientation(sceneElement2);
            if (nullable.HasValue)
              orientationHelper.SortElements(elements, nullable.Value);
            int num;
            if (list1.Count == 1 && list1[0] is BaseFrameworkElement)
            {
              int? fixedCapacity = sceneElement2.DefaultContent.FixedCapacity;
              num = fixedCapacity.GetValueOrDefault() != 1 ? 0 : (fixedCapacity.HasValue ? true : false);
            }
            else
              num = 0;
            bool flag = num != 0;
            if (sceneElement2 != null)
              activeSceneViewModel.GetLayoutDesignerForParent(sceneElement2, true);
            foreach (SceneElement sceneElement3 in list1)
            {
              if (sceneElement3 != null)
              {
                BaseFrameworkElement frameworkElement = sceneElement3 as BaseFrameworkElement;
                ILayoutDesigner layoutDesigner = frameworkElement == null ? (ILayoutDesigner) null : sceneElement3.ViewModel.GetLayoutDesignerForChild((SceneElement) frameworkElement, true);
                if (flag)
                  properties = SceneElementHelper.StoreProperties((SceneNode) sceneElement3, layoutDesigner.GetLayoutProperties(), true);
                if (this.SceneViewModel.LockedInsertionPoint != null && this.SceneViewModel.LockedInsertionPoint.SceneElement == sceneElement3)
                  sceneElement1 = sceneElement3;
              }
            }
            if (list1.Count == 1)
              VisualStateManagerSceneNode.MoveStates(list1[0], sceneElement2);
            using (activeSceneViewModel.DisableUpdateChildrenOnAddAndRemove())
            {
              SceneElement primarySelection = designerContext.SelectionManager.ElementSelectionSet.PrimarySelection;
              designerContext.SelectionManager.ElementSelectionSet.Clear();
              Dictionary<IPropertyId, List<SceneNode>> storedChildren = new Dictionary<IPropertyId, List<SceneNode>>();
              storedChildren.Add((IPropertyId) sceneElement2.DefaultContentProperty, elements);
              ISceneNodeCollection<SceneNode> collectionContainer = primarySelection.GetCollectionContainer();
              foreach (SceneElement sceneElement3 in list1)
              {
                if (sceneElement3 != primarySelection)
                  sceneElement3.Remove();
              }
              int index1 = collectionContainer.IndexOf((SceneNode) primarySelection);
              primarySelection.Remove();
              collectionContainer.Insert(index1, (SceneNode) sceneElement2);
              ChangeLayoutTypeCommand.ApplyChildren(sceneElement2, storedChildren, empty.Size);
              if (flag)
              {
                SceneElementHelper.FixElementNameBindingsInStoredProperties((SceneNode) list1[0], (SceneNode) sceneElement2, properties);
                SceneElementHelper.ApplyProperties((SceneNode) sceneElement2, properties);
              }
              else
              {
                ILayoutDesigner designerForChild = sceneElement2.ViewModel.GetLayoutDesignerForChild(sceneElement2, true);
                if (sceneElement2.IsViewObjectValid)
                {
                  LayoutOverrides layoutOverrides = LayoutOverrides.None;
                  LayoutOverrides overridesToIgnore = LayoutOverrides.None;
                  if (nullable.HasValue && nullable.Value == Orientation.Horizontal)
                    layoutOverrides |= LayoutOverrides.Width;
                  else
                    overridesToIgnore |= LayoutOverrides.Width;
                  if (nullable.HasValue && nullable.Value == Orientation.Vertical)
                    layoutOverrides |= LayoutOverrides.Height;
                  else
                    overridesToIgnore |= LayoutOverrides.Height;
                  designerForChild.SetChildRect((BaseFrameworkElement) sceneElement2, empty, layoutOverrides, overridesToIgnore, LayoutOverrides.None);
                }
              }
              editTransaction.Update();
              if (sceneElement2.IsViewObjectValid)
              {
                this.SceneViewModel.DefaultView.UpdateLayout();
                SceneElement parentElement = primarySelection.ParentElement;
                ILayoutDesigner designerForParent = activeSceneViewModel.GetLayoutDesignerForParent(parentElement, true);
                for (int index2 = 0; index2 < list1.Count; ++index2)
                {
                  SceneElement sceneElement3 = list1[index2];
                  BaseFrameworkElement frameworkElement = sceneElement3 as BaseFrameworkElement;
                  if (frameworkElement != null)
                  {
                    LayoutCacheRecord layoutCacheRecord = list2[index2];
                    Rect rect = LayoutRoundingHelper.RoundUpLayoutRect(frameworkElement, layoutCacheRecord.Rect);
                    rect.Location = (Point) (rect.Location - empty.Location);
                    designerForParent.ClearUnusedLayoutProperties(frameworkElement);
                    designerForParent.SetChildRect(frameworkElement, rect, flag ? LayoutOverrides.None : layoutCacheRecord.Overrides, LayoutOverrides.Margin | LayoutOverrides.GridBox, LayoutOverrides.None);
                    if (this.IsDragDropContainer)
                      sceneElement3.SetValue(Base2DElement.AllowDropProperty, (object) true);
                  }
                }
              }
              if (sceneElement1 != null)
                this.SceneViewModel.SetLockedInsertionPoint(sceneElement1);
            }
            designerContext.SelectionManager.ElementSelectionSet.SetSelection(sceneElement2);
            editTransaction.Commit();
          }
        }
      }
    }

    private class OrientationHelper
    {
      private Dictionary<SceneNode, Point> centers = new Dictionary<SceneNode, Point>();

      public void AddChildRect(SceneNode node, Rect rect)
      {
        this.centers[node] = new Point((rect.Left + rect.Right) / 2.0, (rect.Top + rect.Bottom) / 2.0);
      }

      public Orientation? ApplyOrientation(SceneElement container)
      {
        if (this.centers.Count > 0 && container != null)
        {
          IPropertyId orientationProperty = this.GetOrientationProperty(container);
          if (orientationProperty != null)
          {
            Orientation orientation = this.GetOrientation();
            container.SetValueAsWpf(orientationProperty, (object) orientation);
            return new Orientation?(orientation);
          }
        }
        return new Orientation?();
      }

      public void SortElements(List<SceneNode> elements, Orientation orientation)
      {
        elements.Sort((IComparer<SceneNode>) new GroupIntoLayoutTypeCommand.OrientationHelper.CenterPointComparer(this.centers, orientation));
      }

      private Orientation GetOrientation()
      {
        Point point1 = new Point(0.0, 0.0);
        Point point2 = new Point(0.0, 0.0);
        foreach (Point point3 in this.centers.Values)
        {
          point1.X += point3.X;
          point1.Y += point3.Y;
          point2.X += point3.X * point3.X;
          point2.Y += point3.Y * point3.Y;
        }
        double num = (double) this.centers.Count;
        return point2.X * num - point1.X * point1.X > point2.Y * num - point1.Y * point1.Y ? Orientation.Horizontal : Orientation.Vertical;
      }

      private IPropertyId GetOrientationProperty(SceneElement container)
      {
        if (PlatformTypes.StackPanel.IsAssignableFrom((ITypeId) container.Type))
          return StackPanelElement.OrientationProperty;
        if (ProjectNeutralTypes.WrapPanel.IsAssignableFrom((ITypeId) container.Type))
          return WrapPanelElement.OrientationProperty;
        if (PlatformTypes.VirtualizingStackPanel.IsAssignableFrom((ITypeId) container.Type))
          return VirtualizingStackPanelElement.OrientationProperty;
        return (IPropertyId) null;
      }

      private class CenterPointComparer : IComparer<SceneNode>
      {
        private Dictionary<SceneNode, Point> centers;
        private Orientation orientation;

        public CenterPointComparer(Dictionary<SceneNode, Point> centers, Orientation orientation)
        {
          this.centers = centers;
          this.orientation = orientation;
        }

        public int Compare(SceneNode x, SceneNode y)
        {
          Point point1 = this.CenterPoint(x);
          Point point2 = this.CenterPoint(y);
          if (this.orientation != Orientation.Horizontal)
            return point1.Y.CompareTo(point2.Y);
          return point1.X.CompareTo(point2.X);
        }

        private Point CenterPoint(SceneNode node)
        {
          Point point;
          if (!this.centers.TryGetValue(node, out point))
            point = new Point();
          return point;
        }
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.UngroupCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class UngroupCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        foreach (SceneElement element in (IEnumerable<SceneElement>) this.SceneViewModel.ElementSelectionSet.Selection)
        {
          if (this.CanUngroupAsBaseFrameworkElement(element) || this.CanUngroupAs3D(element))
            return true;
        }
        return false;
      }
    }

    public UngroupCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      ICollection<SceneElement> collection = (ICollection<SceneElement>) this.SceneViewModel.ElementSelectionSet.Selection;
      ISceneElementCollection elementCollection = (ISceneElementCollection) new SceneElementCollection();
      bool flag = false;
      using (this.SceneViewModel.DisableDrawIntoState())
      {
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoUnitUngroup))
        {
          this.SceneViewModel.ElementSelectionSet.Clear();
          foreach (SceneElement element in (IEnumerable<SceneElement>) collection)
          {
            SceneNode[] sceneNodeArray;
            if (this.CanUngroupAsBaseFrameworkElement(element))
            {
              int count = element.DefaultContent.Count;
              sceneNodeArray = this.Ungroup(editTransaction, (BaseFrameworkElement) element);
              if (count != sceneNodeArray.Length)
                flag = true;
            }
            else if (this.CanUngroupAs3D(element))
            {
              sceneNodeArray = this.Ungroup((Base3DElement) element);
            }
            else
            {
              elementCollection.Add(element);
              continue;
            }
            foreach (SceneElement sceneElement in sceneNodeArray)
            {
              if (sceneElement.IsSelectable)
                elementCollection.Add(sceneElement);
            }
          }
          this.SceneViewModel.ElementSelectionSet.ExtendSelection((ICollection<SceneElement>) elementCollection);
          editTransaction.Commit();
        }
      }
      if (!flag)
        return;
      int num = (int) this.SceneViewModel.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.UngroupDiscardedElementsWarningMessage, new object[2]
        {
          (object) "FrameworkElement",
          (object) "StaticResourceExtension"
        }),
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Exclamation
      });
    }

    private bool CanUngroupAsBaseFrameworkElement(SceneElement element)
    {
      BaseFrameworkElement frameworkElement = element as BaseFrameworkElement;
      if (frameworkElement == null || !Enumerable.Any<ITypeId>((IEnumerable<ITypeId>) GroupIntoLayoutTypeFlyoutCommand.LayoutTypes, (Func<ITypeId, bool>) (type => type.IsAssignableFrom((ITypeId) element.Type))) && !Enumerable.Any<GroupCommandHelpers.DragDropLayoutAssocation>((IEnumerable<GroupCommandHelpers.DragDropLayoutAssocation>) GroupCommandHelpers.DragDropLayoutTypes, (Func<GroupCommandHelpers.DragDropLayoutAssocation, bool>) (layoutAssociation => layoutAssociation.Container.IsAssignableFrom((ITypeId) element.Type))))
        return false;
      bool flag = false;
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) frameworkElement.DefaultContent)
      {
        if (sceneNode is BaseFrameworkElement)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return false;
      int count = frameworkElement.DefaultContent.Count;
      ISceneNodeCollection<SceneNode> collectionContainer = frameworkElement.GetCollectionContainer();
      if (collectionContainer == null)
        return false;
      int? fixedCapacity = collectionContainer.FixedCapacity;
      if (!fixedCapacity.HasValue)
        return true;
      int num = count;
      int? nullable = fixedCapacity;
      if (num <= nullable.GetValueOrDefault())
        return nullable.HasValue;
      return false;
    }

    private bool CanUngroupAs3D(SceneElement element)
    {
      if (!Model3DGroupElement.GetIsGroup((SceneNode) element))
        return ModelVisual3DElement.GetIsGroup(element);
      return true;
    }

    private SceneNode[] Ungroup(SceneEditTransaction transaction, BaseFrameworkElement group)
    {
      SceneElement parentElement = group.ParentElement;
      if (parentElement == null)
        return new SceneNode[0];
      transaction.Update();
      ILayoutDesigner designerForParent1 = this.SceneViewModel.GetLayoutDesignerForParent(parentElement, true);
      Rect childRect = designerForParent1.GetChildRect(group);
      Matrix effectiveRenderTransform = group.GetEffectiveRenderTransform(false);
      SceneNode[] array = new SceneNode[group.DefaultContent.Count];
      group.DefaultContent.CopyTo(array, 0);
      using (this.SceneViewModel.DisableUpdateChildrenOnAddAndRemove())
      {
        Transform[] transformArray = new Transform[array.Length];
        Point[] pointArray = new Point[array.Length];
        LayoutCacheRecord[] layoutCacheRecordArray = new LayoutCacheRecord[array.Length];
        ILayoutDesigner designerForParent2 = group.ViewModel.GetLayoutDesignerForParent((SceneElement) group, true);
        for (int index = 0; index < array.Length; ++index)
        {
          BaseFrameworkElement frameworkElement = array[index] as BaseFrameworkElement;
          if (frameworkElement != null)
          {
            transformArray[index] = (Transform) frameworkElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
            pointArray[index] = frameworkElement.RenderTransformOrigin;
            designerForParent2.GetChildRect(frameworkElement);
            LayoutCacheRecord layoutCacheRecord = designerForParent2.CacheLayout(frameworkElement);
            layoutCacheRecordArray[index] = layoutCacheRecord;
          }
        }
        int num1;
        if (array.Length == 1 && array[0] is BaseFrameworkElement)
        {
          int? fixedCapacity = group.DefaultContent.FixedCapacity;
          num1 = fixedCapacity.GetValueOrDefault() != 1 ? 0 : (fixedCapacity.HasValue ? true : false);
        }
        else
          num1 = 0;
        bool flag = num1 != 0;
        if (flag)
        {
          Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) group, designerForParent1.GetLayoutProperties(), true);
          SceneElementHelper.FixElementNameBindingsInStoredProperties((SceneNode) group, array[0], properties);
          if (!SceneElementHelper.ApplyProperties(array[0], properties))
            flag = false;
        }
        if (array.Length == 1 && array[0] is SceneElement)
          VisualStateManagerSceneNode.MoveStates((SceneElement) group, (SceneElement) array[0]);
        using (this.SceneViewModel.ForceBaseValue())
        {
          SceneElement sceneElement = (SceneElement) null;
          for (int index = 0; index < array.Length; ++index)
          {
            BaseFrameworkElement frameworkElement = array[index] as BaseFrameworkElement;
            if (frameworkElement != null)
            {
              if (this.SceneViewModel.LockedInsertionPoint != null && this.SceneViewModel.LockedInsertionPoint.SceneElement == frameworkElement)
                sceneElement = (SceneElement) frameworkElement;
              DocumentNodeHelper.PreserveFormatting(frameworkElement.DocumentNode);
              frameworkElement.Remove();
            }
          }
          ISceneNodeCollection<SceneNode> collectionContainer = group.GetCollectionContainer();
          int index1 = collectionContainer.IndexOf((SceneNode) group);
          this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) group);
          this.SceneViewModel.RemoveElement((SceneNode) group);
          Matrix matrix1 = effectiveRenderTransform;
          matrix1.OffsetX = 0.0;
          matrix1.OffsetY = 0.0;
          for (int index2 = array.Length - 1; index2 >= 0; --index2)
          {
            BaseFrameworkElement frameworkElement = array[index2] as BaseFrameworkElement;
            if (frameworkElement != null)
            {
              Matrix matrix2 = (transformArray[index2] ?? Transform.Identity).Value;
              collectionContainer.Insert(index1, (SceneNode) frameworkElement);
              CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix2 * matrix1));
              if (frameworkElement.GetLocalValue(Base2DElement.RenderTransformProperty) != null || !canonicalTransform.TransformGroup.Value.IsIdentity)
              {
                frameworkElement.SetValue(Base2DElement.RenderTransformProperty, canonicalTransform.GetPlatformTransform(frameworkElement.Platform.GeometryHelper));
                if (frameworkElement.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Unset)
                  frameworkElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) new Point(0.5, 0.5));
              }
            }
          }
          transaction.Update();
          if (sceneElement != null)
            this.SceneViewModel.SetLockedInsertionPoint(sceneElement);
          bool[] flagArray = new bool[array.Length];
          int length = 0;
          for (int index2 = 0; index2 < array.Length; ++index2)
          {
            BaseFrameworkElement frameworkElement = array[index2] as BaseFrameworkElement;
            if (frameworkElement != null && array[index2].Parent != null)
            {
              if (!flag)
              {
                LayoutCacheRecord layoutCacheRecord = layoutCacheRecordArray[index2];
                Rect rect1 = layoutCacheRecord.Rect;
                Point point1 = new Point(rect1.X + rect1.Width * pointArray[index2].X, rect1.Y + rect1.Height * pointArray[index2].Y);
                Point point2 = effectiveRenderTransform.Transform(point1);
                Rect rect2 = new Rect(rect1.TopLeft + point2 - point1 + (Vector) childRect.TopLeft, rect1.Size);
                designerForParent1.ClearUnusedLayoutProperties(frameworkElement);
                designerForParent1.SetChildRect(frameworkElement, rect2, layoutCacheRecord.Overrides, LayoutOverrides.Margin | LayoutOverrides.GridBox, LayoutOverrides.None);
              }
              flagArray[index2] = true;
              ++length;
            }
          }
          SceneNode[] sceneNodeArray = new SceneNode[length];
          int num2 = 0;
          for (int index2 = 0; index2 < array.Length; ++index2)
          {
            if (flagArray[index2])
              sceneNodeArray[num2++] = array[index2];
          }
          return sceneNodeArray;
        }
      }
    }

    private SceneNode[] Ungroup(Base3DElement group)
    {
      SceneElement parentElement = group.ParentElement;
      Matrix3D matrix3D = group.Transform.Value;
      Model3DGroupElement model3DgroupElement = group as Model3DGroupElement;
      ModelVisual3DElement modelVisual3Delement1 = group as ModelVisual3DElement;
      int index1 = 0;
      bool flag = false;
      if (model3DgroupElement != null)
        index1 = model3DgroupElement.Children.Count;
      else if (modelVisual3Delement1 != null)
      {
        index1 = modelVisual3Delement1.Children.Count;
        flag = modelVisual3Delement1.Model3DContent != null;
      }
      Base3DElement[] base3DelementArray = new Base3DElement[index1 + (flag ? true : false)];
      Matrix3D[] matrix3DArray = new Matrix3D[index1 + (flag ? true : false)];
      if (model3DgroupElement != null)
      {
        for (int index2 = 0; index2 < model3DgroupElement.Children.Count; ++index2)
          base3DelementArray[index2] = (Base3DElement) model3DgroupElement.Children[index2];
      }
      else if (modelVisual3Delement1 != null)
      {
        for (int index2 = 0; index2 < modelVisual3Delement1.Children.Count; ++index2)
          base3DelementArray[index2] = (Base3DElement) modelVisual3Delement1.Children[index2];
        if (flag)
        {
          base3DelementArray[index1] = (Base3DElement) modelVisual3Delement1.Model3DContent;
          matrix3DArray[index1] = modelVisual3Delement1.Transform.Value;
        }
      }
      for (int index2 = 0; index2 < index1; ++index2)
      {
        Base3DElement base3Delement = base3DelementArray[index2];
        matrix3DArray[index2] = base3Delement.Transform.Value;
      }
      using (this.SceneViewModel.ForceBaseValue())
      {
        int insertAt = group.GetCollectionContainer().IndexOf((SceneNode) group);
        if (group.ParentElement is Base3DElement)
          this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree(group.ParentElement);
        else
          this.SceneViewModel.AnimationEditor.DeleteAllAnimationsInSubtree((SceneElement) group);
        this.SceneViewModel.RemoveElement((SceneNode) group);
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          Base3DElement base3Delement = base3DelementArray[index2];
          base3Delement.Remove();
          CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D(matrix3DArray[index2] * matrix3D);
          base3Delement.Transform = (Transform3D) canonicalTransform3D.ToTransform();
          if (parentElement is Base3DElement)
            GroupCommandHelpers.InsertElement(this.SceneViewModel, (Base3DElement) parentElement, (SceneNode) base3Delement, new int?(insertAt));
          else
            GroupCommandHelpers.InsertElement(parentElement, (SceneNode) base3Delement, insertAt);
        }
        if (flag)
        {
          ModelVisual3DElement modelVisual3Delement2 = parentElement as ModelVisual3DElement;
          base3DelementArray[index1].Remove();
          CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D(matrix3DArray[index1] * matrix3D);
          base3DelementArray[index1].Transform = (Transform3D) canonicalTransform3D.ToTransform();
          ModelVisual3DElement modelVisual3Delement3 = (ModelVisual3DElement) this.SceneViewModel.CreateSceneNode(typeof (ModelVisual3D));
          modelVisual3Delement2.Children.Add((Visual3DElement) modelVisual3Delement3);
          modelVisual3Delement3.Content = (SceneNode) base3DelementArray[index1];
        }
      }
      return (SceneNode[]) base3DelementArray;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.GroupCommandHelpers
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class GroupCommandHelpers
  {
    internal static GroupCommandHelpers.DragDropLayoutAssocation[] DragDropLayoutTypes = new GroupCommandHelpers.DragDropLayoutAssocation[2]
    {
      new GroupCommandHelpers.DragDropLayoutAssocation(ProjectNeutralTypes.ListBoxDragDropTarget, PlatformTypes.ListBox),
      new GroupCommandHelpers.DragDropLayoutAssocation(ProjectNeutralTypes.TreeViewDragDropTarget, ProjectNeutralTypes.TreeView)
    };

    internal static void InsertElement(SceneElement parent, SceneNode child, int insertAt)
    {
      parent.DefaultContent.Insert(insertAt, child);
    }

    internal static void InsertElement(SceneViewModel sceneViewModel, Base3DElement parent, SceneNode child, int? insertAt)
    {
      Model3DGroupElement model3DgroupElement = parent as Model3DGroupElement;
      ModelVisual3DElement modelVisual3Delement = parent as ModelVisual3DElement;
      Model3DElement model3Delement1 = child as Model3DElement;
      Visual3DElement visual3Delement = child as Visual3DElement;
      if (modelVisual3Delement != null)
      {
        if (visual3Delement != null)
        {
          if (insertAt.HasValue)
            modelVisual3Delement.Children.Insert(insertAt.Value, visual3Delement);
          else
            modelVisual3Delement.Children.Add(visual3Delement);
        }
        else
        {
          if (model3Delement1 == null)
            return;
          modelVisual3Delement.Content = (SceneNode) model3Delement1;
        }
      }
      else
      {
        if (model3DgroupElement == null)
          return;
        if (visual3Delement != null)
        {
          Model3DElement model3Delement2 = BaseElement3DCoercionHelper.CoerceToModel3D(sceneViewModel, (SceneElement) visual3Delement);
          if (insertAt.HasValue)
            model3DgroupElement.Children.Insert(insertAt.Value, model3Delement2);
          else
            model3DgroupElement.Children.Add(model3Delement2);
        }
        else
        {
          if (model3Delement1 == null)
            return;
          if (insertAt.HasValue)
            model3DgroupElement.Children.Insert(insertAt.Value, model3Delement1);
          else
            model3DgroupElement.Children.Add(model3Delement1);
        }
      }
    }

    internal static bool IsGrouping2D(SceneViewModel model)
    {
      SceneElementSelectionSet elementSelectionSet = model.ElementSelectionSet;
      if (elementSelectionSet.Count > 0)
        return elementSelectionSet.PrimarySelection is Base2DElement;
      return false;
    }

    internal class DragDropLayoutAssocation
    {
      public ITypeId Container { get; private set; }

      public ITypeId Child { get; private set; }

      public DragDropLayoutAssocation(ITypeId container, ITypeId child)
      {
        this.Container = container;
        this.Child = child;
      }
    }
  }
}

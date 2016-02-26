// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BaseElement3DCoercionHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class BaseElement3DCoercionHelper
  {
    public static Model3DElement CoerceToModel3D(SceneViewModel viewModel, SceneElement sceneElement)
    {
      Model3DElement model3Delement = sceneElement as Model3DElement;
      if (model3Delement != null)
        return model3Delement;
      ModelVisual3DElement modelVisual3Delement1;
      if ((modelVisual3Delement1 = sceneElement as ModelVisual3DElement) != null)
      {
        Model3DGroupElement model3DgroupElement = (Model3DGroupElement) viewModel.CreateSceneNode(typeof (Model3DGroup));
        SceneNode valueAsSceneNode1 = modelVisual3Delement1.GetLocalValueAsSceneNode(ModelVisual3DElement.TransformProperty);
        if (valueAsSceneNode1 != null)
        {
          valueAsSceneNode1.Remove();
          model3DgroupElement.SetValueAsSceneNode(ModelVisual3DElement.TransformProperty, valueAsSceneNode1);
        }
        model3DgroupElement.Name = modelVisual3Delement1.Name;
        SceneNode valueAsSceneNode2 = modelVisual3Delement1.GetLocalValueAsSceneNode(ModelVisual3DElement.ContentProperty);
        if (valueAsSceneNode2 != null)
        {
          valueAsSceneNode2.Remove();
          if (valueAsSceneNode2 is Model3DElement)
            model3DgroupElement.GetCollectionForProperty(Model3DGroupElement.ChildrenProperty).Add(valueAsSceneNode2);
        }
        foreach (ModelVisual3DElement modelVisual3Delement2 in (IEnumerable<Visual3DElement>) modelVisual3Delement1.Children)
          model3DgroupElement.Children.Add(BaseElement3DCoercionHelper.CoerceToModel3D(viewModel, (SceneElement) modelVisual3Delement2));
        return (Model3DElement) model3DgroupElement;
      }
      if (sceneElement is Viewport2DVisual3DElement)
        return (Model3DElement) null;
      Viewport3DElement viewport3Delement = sceneElement as Viewport3DElement;
      return (Model3DElement) null;
    }

    public static Visual3DElement CoerceToVisual3D(SceneViewModel sceneView, SceneElement sceneElement)
    {
      Model3DElement model3Delement = sceneElement as Model3DElement;
      if (model3Delement != null)
      {
        ModelVisual3DElement modelVisual3Delement = ModelVisual3DElement.Factory.Instantiate(sceneView);
        modelVisual3Delement.Content = (SceneNode) model3Delement;
        return (Visual3DElement) modelVisual3Delement;
      }
      Visual3DElement visual3Delement;
      if ((visual3Delement = sceneElement as Visual3DElement) != null)
        return visual3Delement;
      Viewport3DElement viewport3Delement = sceneElement as Viewport3DElement;
      return (Visual3DElement) null;
    }
  }
}

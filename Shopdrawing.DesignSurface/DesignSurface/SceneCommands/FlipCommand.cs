// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.FlipCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class FlipCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
        {
          SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
          if (elementSelectionSet.Count > 0)
          {
            SceneElement sceneElement1 = this.SceneViewModel.RootNode as SceneElement;
            if (elementSelectionSet.Selection.Contains(sceneElement1))
              return false;
            foreach (SceneElement sceneElement2 in elementSelectionSet.Selection)
            {
              if (sceneElement2 is Base3DElement)
                return false;
            }
            return true;
          }
        }
        return false;
      }
    }

    protected FlipCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected void FlipElements(BasisComponent basisComponent, string description)
    {
      SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
      PropertyReference propertyReference = (PropertyReference) null;
      foreach (SceneElement sceneElement in elementSelectionSet.Selection)
      {
        if (sceneElement is BaseFrameworkElement || sceneElement is StyleNode)
        {
          propertyReference = new PropertyReference((ReferenceStep) sceneElement.ProjectContext.ResolveProperty(Base2DElement.RenderTransformProperty));
          break;
        }
        if (sceneElement is Base3DElement)
        {
          propertyReference = new PropertyReference((ReferenceStep) sceneElement.ProjectContext.ResolveProperty((sceneElement as Base3DElement).TransformPropertyId));
          break;
        }
      }
      if (propertyReference == null)
        return;
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(description))
      {
        foreach (SceneElement sceneElement in elementSelectionSet.Selection)
        {
          object computedValueAsWpf = sceneElement.GetComputedValueAsWpf(propertyReference);
          object obj = new ReflectTransform(basisComponent).ApplyRelativeTransform(computedValueAsWpf);
          CanonicalTransform canonicalTransform = obj as CanonicalTransform;
          if (canonicalTransform != (CanonicalTransform) null)
          {
            sceneElement.SetValue(propertyReference, canonicalTransform.GetPlatformTransform(sceneElement.Platform.GeometryHelper));
            if (sceneElement.IsSet(Base2DElement.RenderTransformOriginProperty) == PropertyState.Unset)
              sceneElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) new Point(0.5, 0.5));
          }
          else
          {
            CanonicalTransform3D canonicalTransform3D = obj as CanonicalTransform3D;
            if (canonicalTransform3D != (CanonicalTransform3D) null)
              sceneElement.SetValue(propertyReference, (object) canonicalTransform3D.ToTransform());
          }
        }
        editTransaction.Commit();
      }
    }
  }
}

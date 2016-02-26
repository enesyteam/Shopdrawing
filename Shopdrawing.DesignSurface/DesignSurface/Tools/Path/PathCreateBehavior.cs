// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal abstract class PathCreateBehavior : ElementCreateBehavior
  {
    protected BaseFrameworkElement EditingElement { get; set; }

    protected bool IsProjectedInsertionPoint { get; set; }

    protected override ITypeId InstanceType
    {
      get
      {
        return PlatformTypes.Path;
      }
    }

    protected PathCreateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    internal PathElement CreatePathElement()
    {
      PathElement pathElement = PathCreateBehavior.CreatePathElement(this.ActiveSceneViewModel, this.ActiveSceneInsertionPoint, this.ToolBehaviorContext);
      this.EditingElement = (BaseFrameworkElement) pathElement;
      return pathElement;
    }

    protected override void OnPreviewInsertionPointChanged()
    {
      base.OnPreviewInsertionPointChanged();
      if (this.PreviewInsertionPoint == null)
      {
        if (this.ActiveSceneViewModel.ActiveSceneInsertionPoint == null)
          this.IsProjectedInsertionPoint = false;
        else
          this.IsProjectedInsertionPoint = Adorner.NonAffineTransformInParentStack(this.ActiveSceneViewModel.ActiveSceneInsertionPoint.SceneElement);
      }
      else
        this.IsProjectedInsertionPoint = Adorner.NonAffineTransformInParentStack(this.PreviewInsertionPoint.SceneElement);
    }

    internal static PathElement CreatePathElement(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, ToolBehaviorContext toolContext)
    {
      PathElement pathElement = (PathElement) viewModel.CreateSceneNode(PlatformTypes.Path);
      if (insertionPoint.CanInsert((ITypeId) pathElement.Type))
      {
        insertionPoint.Insert((SceneNode) pathElement);
        using (pathElement.ViewModel.ForceBaseValue())
        {
          toolContext.AmbientPropertyManager.ApplyAmbientProperties((SceneNode) pathElement);
          pathElement.PathGeometry = new PathGeometry();
          pathElement.SetValueAsWpf(ShapeElement.StretchProperty, (object) Stretch.Fill);
        }
        viewModel.ElementSelectionSet.SetSelection((SceneElement) pathElement);
        viewModel.CanonicalizeViewState(SceneUpdateTypeFlags.Completing);
      }
      return pathElement;
    }

    protected override void OnAttach()
    {
      base.OnAttach();
      this.EditingElement = (BaseFrameworkElement) null;
    }
  }
}

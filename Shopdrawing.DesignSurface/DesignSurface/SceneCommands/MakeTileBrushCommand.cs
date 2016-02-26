// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeTileBrushCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class MakeTileBrushCommand : ConvertElementCommand
  {
    public override bool IsAvailable
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, this.Type);
      }
    }

    public MakeTileBrushCommand(SceneViewModel sceneViewModel)
      : base(sceneViewModel)
    {
    }

    protected abstract object CreateTileBrush(BaseFrameworkElement element);

    protected override DocumentNode CreateValue(BaseFrameworkElement source)
    {
      object tileBrush = this.CreateTileBrush(source);
      if (tileBrush == null)
        return (DocumentNode) null;
      ITypeResolver typeResolver = (ITypeResolver) source.ProjectContext;
      IViewObject visual = source.Visual;
      if (visual != null)
      {
        object platformSpecificObject = visual.PlatformSpecificObject;
        if (platformSpecificObject != null && PlatformTypes.Image.IsAssignableFrom((ITypeId) visual.GetIType((ITypeResolver) this.SceneViewModel.ProjectContext)))
        {
          ReferenceStep referenceStep = typeResolver.ResolveProperty(TileBrushNode.StretchProperty) as ReferenceStep;
          object obj = referenceStep.GetValue(tileBrush);
          object valueToSet = (typeResolver.ResolveProperty(ImageElement.StretchProperty) as ReferenceStep).GetValue(platformSpecificObject);
          if (!obj.Equals(valueToSet))
            referenceStep.SetValue(tileBrush, valueToSet);
        }
      }
      return this.SceneViewModel.Document.DocumentContext.CreateNode(typeResolver.ResolveType(PlatformTypes.TileBrush).RuntimeType, tileBrush);
    }
  }
}

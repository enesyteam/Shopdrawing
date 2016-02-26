// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.EffectAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class EffectAsset : TypeAsset
  {
    public EffectAsset(IType type, string displayName, ExampleAssetInfo exampleInfo, string onDemandAssemblyFileName, AssemblyNameAndLocation[] resolvableAssemblyReferences)
      : base(type, displayName, exampleInfo, onDemandAssemblyFileName, resolvableAssemblyReferences)
    {
    }

    protected override IEnumerable<ISceneInsertionPoint> InternalFindInsertionPoints(SceneViewModel viewModel)
    {
      IEnumerable<ISceneInsertionPoint> source = Enumerable.Select<SceneNode, ISceneInsertionPoint>(Enumerable.Where<SceneNode>((IEnumerable<SceneNode>) viewModel.ChildPropertySelectionSet.Selection, (Func<SceneNode, bool>) (childNode => childNode.Parent is SceneElement)), (Func<SceneNode, ISceneInsertionPoint>) (childNode => new EffectInsertionPointCreator((SceneElement) childNode.Parent).Create((object) childNode)));
      if (Enumerable.FirstOrDefault<ISceneInsertionPoint>(source) != null)
        return source;
      return Enumerable.Select<SceneElement, ISceneInsertionPoint>((IEnumerable<SceneElement>) viewModel.ElementSelectionSet.Selection, (Func<SceneElement, ISceneInsertionPoint>) (element => new EffectInsertionPointCreator(element).Create((object) this)));
    }
  }
}

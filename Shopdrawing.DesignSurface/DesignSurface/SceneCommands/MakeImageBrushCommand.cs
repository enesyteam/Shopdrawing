// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeImageBrushCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeImageBrushCommand : MakeTileBrushCommand
  {
    protected override bool CreateResource
    {
      get
      {
        return true;
      }
    }

    protected override ITypeId Type
    {
      get
      {
        return PlatformTypes.ImageBrush;
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (!base.IsEnabled)
          return false;
        SceneElementSelectionSet elementSelectionSet = this.SceneViewModel.ElementSelectionSet;
        if (elementSelectionSet.Count == 1)
          return PlatformTypes.Image.IsAssignableFrom((ITypeId) elementSelectionSet.PrimarySelection.Type);
        return false;
      }
    }

    protected override string UndoUnitName
    {
      get
      {
        return StringTable.UndoUnitMakeImageBrush;
      }
    }

    public MakeImageBrushCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override object CreateTileBrush(BaseFrameworkElement element)
    {
      DocumentNode documentNode = (DocumentNode) element.ViewModel.Document.DocumentContext.CreateNode(PlatformTypes.ImageBrush);
      return element.ViewModel.CreateInstance(new DocumentNodePath(documentNode, documentNode));
    }

    protected override DocumentNode CreateValue(BaseFrameworkElement source)
    {
      DocumentNode node = base.CreateValue(source);
      ImageElement imageElement = source as ImageElement;
      if (imageElement != null)
      {
        string uri = imageElement.Uri;
        if (uri != null)
          ((ImageBrushNode) this.SceneViewModel.GetSceneNode(node)).Source = uri;
      }
      return node;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeVisualBrushCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class MakeVisualBrushCommand : MakeTileBrushCommand
  {
    protected override string UndoUnitName
    {
      get
      {
        return StringTable.UndoUnitMakeVisualBrush;
      }
    }

    protected override bool CreateResource
    {
      get
      {
        return true;
      }
    }

    protected override CreateResourceModel.ContextFlags ResourceContextFlags
    {
      get
      {
        return CreateResourceModel.ContextFlags.CanOnlyUseCurrentDocument;
      }
    }

    protected override ITypeId Type
    {
      get
      {
        return PlatformTypes.VisualBrush;
      }
    }

    public MakeVisualBrushCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override DocumentNode CreateValue(BaseFrameworkElement source)
    {
      source.EnsureNamed();
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) base.CreateValue(source);
      BindingSceneNode bindingSceneNode = BindingSceneNode.Factory.Instantiate(source.ViewModel);
      bindingSceneNode.ElementName = source.Name;
      documentCompositeNode.Properties[BrushNode.VisualBrushVisualProperty] = bindingSceneNode.DocumentNode;
      return (DocumentNode) documentCompositeNode;
    }

    protected override object CreateTileBrush(BaseFrameworkElement element)
    {
      DocumentNode documentNode = (DocumentNode) element.ViewModel.Document.DocumentContext.CreateNode(PlatformTypes.VisualBrush);
      return element.ViewModel.CreateInstance(new DocumentNodePath(documentNode, documentNode));
    }
  }
}

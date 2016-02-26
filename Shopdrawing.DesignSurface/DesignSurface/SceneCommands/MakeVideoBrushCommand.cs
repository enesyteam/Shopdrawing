// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.MakeVideoBrushCommand
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
  internal sealed class MakeVideoBrushCommand : MakeTileBrushCommand
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
        return PlatformTypes.VideoBrush;
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
          return PlatformTypes.MediaElement.IsAssignableFrom((ITypeId) elementSelectionSet.PrimarySelection.Type);
        return false;
      }
    }

    protected override string UndoUnitName
    {
      get
      {
        return StringTable.UndoUnitMakeVideoBrush;
      }
    }

    public MakeVideoBrushCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override object CreateTileBrush(BaseFrameworkElement element)
    {
      DocumentNode documentNode = (DocumentNode) element.ViewModel.Document.DocumentContext.CreateNode(PlatformTypes.VideoBrush);
      return element.ViewModel.CreateInstance(new DocumentNodePath(documentNode, documentNode));
    }

    protected override DocumentNode CreateValue(BaseFrameworkElement source)
    {
      DocumentNode node = base.CreateValue(source);
      source.EnsureNamed();
      this.SceneViewModel.GetSceneNode(node).SetValue((IPropertyId) PlatformTypes.VideoBrush.GetMember(MemberType.LocalProperty, "SourceName", MemberAccessTypes.Public), (object) source.Name);
      return node;
    }
  }
}

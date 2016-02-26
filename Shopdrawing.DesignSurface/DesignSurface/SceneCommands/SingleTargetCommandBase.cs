// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.SingleTargetCommandBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal abstract class SingleTargetCommandBase : SceneCommandBase
  {
    protected bool HasValidTarget
    {
      get
      {
        if (this.SceneViewModel.ElementSelectionSet.Count == 1)
          return this.TargetElement != null;
        return false;
      }
    }

    protected SceneElement TargetElement
    {
      get
      {
        return this.SceneViewModel.ElementSelectionSet.PrimarySelection;
      }
    }

    protected IType Type
    {
      get
      {
        if (this.HasValidTarget)
          return SingleTargetCommandBase.GetTypeOfElement(this.TargetElement);
        return (IType) null;
      }
    }

    public SingleTargetCommandBase(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    internal static IType GetTypeOfElement(SceneElement element)
    {
      IType type = (IType) null;
      StyleNode styleNode = element as StyleNode;
      if (styleNode != null)
        type = styleNode.StyleTargetTypeId;
      else if (element != null)
        type = element.Type;
      return type;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ToggleAutoSizeHorizontalCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ToggleAutoSizeHorizontalCommand : ToggleAutoSizeBaseCommand
  {
    protected override IPropertyId SizeProperty
    {
      get
      {
        return BaseFrameworkElement.WidthProperty;
      }
    }

    protected override IPropertyId DesignSizeProperty
    {
      get
      {
        return DesignTimeProperties.DesignWidthProperty;
      }
    }

    public ToggleAutoSizeHorizontalCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    protected override double GetActualSize(BaseFrameworkElement element)
    {
      return element.GetComputedTightBounds().Width;
    }
  }
}

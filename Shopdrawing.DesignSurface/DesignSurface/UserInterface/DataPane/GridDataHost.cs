// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.GridDataHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class GridDataHost : PanelDataHostBase
  {
    public GridDataHost(ITypeId resolvedGridType)
      : base(resolvedGridType)
    {
    }

    public override BaseFrameworkElement BuildPanel(SceneViewModel viewModel, ICollection<BaseFrameworkElement> children)
    {
      GridElement gridElement = (GridElement) viewModel.CreateSceneNode(this.PanelType);
      int num = 0;
      foreach (BaseFrameworkElement frameworkElement in (IEnumerable<BaseFrameworkElement>) children)
      {
        gridElement.Children.Add((SceneNode) frameworkElement);
        gridElement.RowDefinitions.Add(RowDefinitionNode.Factory.Instantiate(viewModel));
        frameworkElement.SetValue(GridElement.RowProperty, (object) num);
        ++num;
      }
      return (BaseFrameworkElement) gridElement;
    }
  }
}

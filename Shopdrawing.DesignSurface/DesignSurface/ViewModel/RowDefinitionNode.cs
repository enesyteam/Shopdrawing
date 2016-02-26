// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.RowDefinitionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class RowDefinitionNode : SceneElement
  {
    public static readonly IPropertyId MinHeightProperty = (IPropertyId) PlatformTypes.RowDefinition.GetMember(MemberType.LocalProperty, "MinHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId MaxHeightProperty = (IPropertyId) PlatformTypes.RowDefinition.GetMember(MemberType.LocalProperty, "MaxHeight", MemberAccessTypes.Public);
    public static readonly IPropertyId HeightProperty = (IPropertyId) PlatformTypes.RowDefinition.GetMember(MemberType.LocalProperty, "Height", MemberAccessTypes.Public);
    public static readonly RowDefinitionNode.ConcreteRowDefinitionNodeFactory Factory = new RowDefinitionNode.ConcreteRowDefinitionNodeFactory();

    public double ComputedHeight
    {
      get
      {
        if (this.ViewObject != null)
        {
          IViewRowDefinition viewRowDefinition = this.ViewObject as IViewRowDefinition;
          if (viewRowDefinition != null)
            return viewRowDefinition.ActualHeight;
        }
        return 0.0;
      }
    }

    public GridLength Height
    {
      get
      {
        object computedValueAsWpf = this.GetComputedValueAsWpf(RowDefinitionNode.HeightProperty);
        if (computedValueAsWpf != null)
          return (GridLength) computedValueAsWpf;
        return new GridLength(1.0, GridUnitType.Star);
      }
      set
      {
        this.SetValueAsWpf(RowDefinitionNode.HeightProperty, (object) value);
      }
    }

    public double MinHeight
    {
      get
      {
        return (double) this.GetComputedValue(RowDefinitionNode.MinHeightProperty);
      }
      set
      {
        this.SetValue(RowDefinitionNode.MinHeightProperty, (object) value);
      }
    }

    public class ConcreteRowDefinitionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RowDefinitionNode();
      }

      public RowDefinitionNode Instantiate(SceneViewModel viewModel)
      {
        return (RowDefinitionNode) this.Instantiate(viewModel, PlatformTypes.RowDefinition);
      }
    }
  }
}

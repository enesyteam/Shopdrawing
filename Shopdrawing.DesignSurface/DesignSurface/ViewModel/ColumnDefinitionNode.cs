// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ColumnDefinitionNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class ColumnDefinitionNode : SceneElement
  {
    public static readonly IPropertyId MinWidthProperty = (IPropertyId) PlatformTypes.ColumnDefinition.GetMember(MemberType.LocalProperty, "MinWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId MaxWidthProperty = (IPropertyId) PlatformTypes.ColumnDefinition.GetMember(MemberType.LocalProperty, "MaxWidth", MemberAccessTypes.Public);
    public static readonly IPropertyId WidthProperty = (IPropertyId) PlatformTypes.ColumnDefinition.GetMember(MemberType.LocalProperty, "Width", MemberAccessTypes.Public);
    public static readonly ColumnDefinitionNode.ConcreteColumnDefinitionNodeFactory Factory = new ColumnDefinitionNode.ConcreteColumnDefinitionNodeFactory();

    public double ComputedWidth
    {
      get
      {
        if (this.ViewObject != null)
        {
          IViewColumnDefinition columnDefinition = this.ViewObject as IViewColumnDefinition;
          if (columnDefinition != null)
            return columnDefinition.ActualWidth;
        }
        return 0.0;
      }
    }

    public GridLength Width
    {
      get
      {
        object computedValueAsWpf = this.GetComputedValueAsWpf(ColumnDefinitionNode.WidthProperty);
        if (computedValueAsWpf != null)
          return (GridLength) computedValueAsWpf;
        return new GridLength(1.0, GridUnitType.Star);
      }
      set
      {
        this.SetValueAsWpf(ColumnDefinitionNode.WidthProperty, (object) value);
      }
    }

    public double MinWidth
    {
      get
      {
        return (double) this.GetComputedValue(ColumnDefinitionNode.MinWidthProperty);
      }
      set
      {
        this.SetValue(ColumnDefinitionNode.MinWidthProperty, (object) value);
      }
    }

    public class ConcreteColumnDefinitionNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ColumnDefinitionNode();
      }

      public ColumnDefinitionNode Instantiate(SceneViewModel viewModel)
      {
        return (ColumnDefinitionNode) this.Instantiate(viewModel, PlatformTypes.ColumnDefinition);
      }
    }
  }
}

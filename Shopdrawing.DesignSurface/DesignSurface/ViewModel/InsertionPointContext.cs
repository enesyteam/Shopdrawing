// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.InsertionPointContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class InsertionPointContext
  {
    private bool checkContainerCapacity = true;
    private ITypeId typeToInsert;
    private Point position;

    public bool ValidPosition { get; set; }

    public Point Position
    {
      get
      {
        return this.position;
      }
    }

    public bool CheckContainerCapacity
    {
      get
      {
        if (!this.ValidPosition)
          return this.checkContainerCapacity;
        return false;
      }
    }

    public ITypeId TypeToInsert
    {
      get
      {
        return this.typeToInsert;
      }
    }

    public InsertionPointContext(bool checkContainerCapacity)
    {
      this.checkContainerCapacity = checkContainerCapacity;
    }

    public InsertionPointContext(Point position, ITypeId typeToInsert)
    {
      this.position = position;
      this.ValidPosition = true;
      this.typeToInsert = typeToInsert;
    }

    internal InsertionPointContext(Point position)
    {
      this.position = position;
      this.ValidPosition = true;
    }

    internal InsertionPointContext()
    {
    }

    public bool TypeToInsertIsSingleProperty(SceneViewModel viewModel)
    {
      IType type = this.typeToInsert != null ? viewModel.ProjectContext.ResolveType(this.typeToInsert) : (IType) null;
      return type != null && (PlatformTypes.IsEffectType((ITypeId) type) || ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) type) || (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) type) || ProjectNeutralTypes.DataGridColumn.IsAssignableFrom((ITypeId) type)));
    }

    public ISceneInsertionPoint GetSceneInsertionPoint(SceneElement element)
    {
      return this.GetSceneInsertionPoint(element, (SceneNode) null);
    }

    public ISceneInsertionPoint GetSceneInsertionPoint(SceneElement element, SceneNode child)
    {
      return InsertionPointContext.GetSceneInsertionPoint(element, this.typeToInsert, child);
    }

    public IProperty GetPropertyToInsert(SceneElement element)
    {
      return InsertionPointContext.GetPropertyToInsert(element, this.typeToInsert, (SceneNode) null);
    }

    public IProperty GetPropertyToInsert(SceneElement element, SceneNode child)
    {
      return InsertionPointContext.GetPropertyToInsert(element, this.typeToInsert, child);
    }

    internal static IProperty GetPropertyToInsert(SceneElement element, ITypeId typeToInsert)
    {
      return InsertionPointContext.GetPropertyToInsert(element, typeToInsert, (SceneNode) null);
    }

    internal static IProperty GetPropertyToInsert(SceneElement element, ITypeId typeToInsert, SceneNode child)
    {
      IProperty property = (IProperty) null;
      if (typeToInsert == null)
      {
        if (child != null)
        {
          if (child.DocumentNode.SitePropertyKey != null)
            return child.DocumentNode.SitePropertyKey;
          if (child.DocumentNode.Parent.SitePropertyKey != null)
            return child.DocumentNode.Parent.SitePropertyKey;
        }
        return element.InsertionTargetProperty;
      }
      IType type = typeToInsert != null ? element.ProjectContext.ResolveType(typeToInsert) : (IType) null;
      if (type != null)
      {
        if (PlatformTypes.IsEffectType((ITypeId) type))
          property = element.ProjectContext.ResolveProperty(Base2DElement.EffectProperty);
        else if (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) type))
          property = element.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorTriggersProperty);
        else if (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) type))
          property = element.ProjectContext.ResolveProperty(BehaviorHelper.BehaviorsProperty);
        else if (ProjectNeutralTypes.DataGridColumn.IsAssignableFrom((ITypeId) type))
          property = element.ProjectContext.ResolveProperty(DataGridElement.ColumnsProperty);
      }
      return property ?? element.InsertionTargetProperty;
    }

    internal static ISceneInsertionPoint GetSceneInsertionPoint(SceneElement element, ITypeId typeToInsert, SceneNode child)
    {
      IProperty propertyToInsert = InsertionPointContext.GetPropertyToInsert(element, typeToInsert, child);
      if (propertyToInsert != null)
        return (ISceneInsertionPoint) new PropertySceneInsertionPoint(element, propertyToInsert);
      return element.DefaultInsertionPoint;
    }
  }
}

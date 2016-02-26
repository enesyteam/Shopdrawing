// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.BrushTool
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class BrushTool : Tool
  {
    public override ToolCategory Category
    {
      get
      {
        return ToolCategory.BrushTransform;
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.StackedToolDescription;
      }
    }

    protected override ViewState RequiredSelectionViewState
    {
      get
      {
        return ViewState.ElementValid | ViewState.AncestorValid | ViewState.SubtreeValid;
      }
    }

    protected override bool UseDefaultEditingAdorners
    {
      get
      {
        return false;
      }
    }

    public BrushTool(ToolContext toolContext)
      : base(toolContext)
    {
    }

    public static PropertyReference GetBrushPropertyReference(SceneNode sceneNode)
    {
      PropertyReference propertyReference1 = sceneNode.ViewModel.PropertySelectionSet.PrimarySelection;
      if (propertyReference1 != null && !PlatformTypes.Brush.IsAssignableFrom((ITypeId) propertyReference1.ValueTypeId))
        propertyReference1 = (PropertyReference) null;
      PropertyReference propertyReference2;
      if (propertyReference1 == null)
      {
        PropertyReference propertyReference3 = new PropertyReference(sceneNode.ProjectContext.ResolveProperty(ShapeElement.FillProperty) as ReferenceStep);
        propertyReference2 = SceneNodeObjectSet.FilterProperty(sceneNode, propertyReference3);
        if (propertyReference2 == null)
        {
          PropertyReference propertyReference4 = new PropertyReference(sceneNode.ProjectContext.ResolveProperty(ControlElement.BackgroundProperty) as ReferenceStep);
          propertyReference2 = SceneNodeObjectSet.FilterProperty(sceneNode, propertyReference4);
        }
      }
      else
        propertyReference2 = SceneNodeObjectSet.FilterProperty(sceneNode, propertyReference1);
      return propertyReference2;
    }

    protected override void OnEarlySceneUpdatePhase(SceneUpdatePhaseEventArgs e)
    {
      base.OnEarlySceneUpdatePhase(e);
      if (e.IsDirtyViewState(SceneViewModel.ViewStateBits.PropertySelection))
      {
        this.RebuildAdornerSets();
      }
      else
      {
        foreach (DocumentNodeChange change in e.DocumentChanges.DistinctChanges)
        {
          if (this.ShouldRebuildAdorner(change))
          {
            this.RebuildAdornerSets();
            break;
          }
        }
      }
    }

    internal virtual bool ShouldRebuildAdorner(DocumentNodeChange change)
    {
      return false;
    }
  }
}

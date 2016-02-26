// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.TypeElementDragCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class TypeElementDragCreateBehavior : ElementDragCreateBehavior
  {
    private ITypeId elementType;
    private UserThemeTypeInstantiator typeInstantiator;

    protected override ITypeId InstanceType
    {
      get
      {
        return this.elementType;
      }
    }

    internal TypeElementDragCreateBehavior(ToolBehaviorContext toolContext, ITypeId elementType, bool popBehaviorAfterClick)
      : base(toolContext, popBehaviorAfterClick)
    {
      this.elementType = elementType;
      this.typeInstantiator = new UserThemeTypeInstantiator(toolContext.View);
    }

    protected override BaseFrameworkElement CreateElementOnStartDrag()
    {
      BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
      using (this.ActiveSceneViewModel.ForceBaseValue())
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDragCreateElement);
        frameworkElement = (BaseFrameworkElement) this.ActiveSceneViewModel.CreateSceneNode(this.elementType);
        if (this.ActiveSceneInsertionPoint.CanInsert((ITypeId) frameworkElement.Type))
        {
          IList<SceneNode> nodeTreeOnInsertion = this.typeInstantiator.CreateNodeTreeOnInsertion((SceneNode) frameworkElement);
          this.typeInstantiator.ApplyBeforeInsertionDefaultsToElements(nodeTreeOnInsertion, (SceneNode) frameworkElement, new DefaultTypeInstantiator.SceneElementNamingCallback(DefaultTypeInstantiator.TypeNameCallback));
          this.ActiveSceneInsertionPoint.Insert((SceneNode) frameworkElement);
          this.UpdateEditTransaction();
          this.typeInstantiator.ApplyAfterInsertionDefaultsToElements(nodeTreeOnInsertion, (SceneNode) frameworkElement);
          this.ActiveSceneViewModel.ElementSelectionSet.SetSelection(DefaultTypeInstantiator.GetSelectionTarget((SceneNode) frameworkElement));
          this.ActiveSceneViewModel.CanonicalizeViewState(SceneUpdateTypeFlags.Completing);
        }
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ShapeCreateBehaviorOnDragCreateElement);
      }
      return frameworkElement;
    }
  }
}

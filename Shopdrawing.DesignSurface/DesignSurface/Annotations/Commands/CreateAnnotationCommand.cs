// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.Commands.CreateAnnotationCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Annotations.Commands
{
  internal sealed class CreateAnnotationCommand : AnnotationsCommand
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled)
          return Enumerable.Any<SceneElement>(this.ApplicableElements);
        return false;
      }
    }

    private IEnumerable<SceneElement> ApplicableElements
    {
      get
      {
        IEnumerable<SceneElement> source = Enumerable.Empty<SceneElement>();
        SelectionManager selectionManager = this.SceneView.DesignerContext.SelectionManager;
        if (selectionManager != null && selectionManager.ElementSelectionSet != null && selectionManager.ElementSelectionSet.Selection != null)
          source = (IEnumerable<SceneElement>) selectionManager.ElementSelectionSet.Selection;
        if (!Enumerable.Any<SceneElement>(source))
          source = (IEnumerable<SceneElement>) new SceneElement[1]
          {
            this.SceneView.ViewModel.RootNode as SceneElement
          };
        return source;
      }
    }

    public CreateAnnotationCommand(SceneView view)
      : base(view)
    {
    }

    public override void Execute()
    {
      List<SceneElement> list = Enumerable.ToList<SceneElement>(this.ApplicableElements);
      if (!Enumerable.Any<SceneElement>((IEnumerable<SceneElement>) list))
        throw new InvalidOperationException();
      this.SceneView.DesignerContext.AnnotationsOptionsModel.InitAuthorInfoIfNeeded();
      PerformanceUtility.MeasurePerformanceUntilRender(list.Count != 1 || list[0] != this.SceneView.ViewModel.RootNode ? PerformanceEvent.AnnotationCreationAttached : PerformanceEvent.AnnotationCreationFloating);
      this.AnnotationService.Create((IEnumerable<SceneElement>) list, true);
    }
  }
}

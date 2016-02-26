// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.UnlinkAnnotationCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Linq;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  public class UnlinkAnnotationCommand : ICommand
  {
    private readonly AnnotationSceneNode annotationNode;

    private AnnotationService AnnotationService
    {
      get
      {
        if (this.annotationNode.DesignerContext == null)
          return (AnnotationService) null;
        return this.annotationNode.DesignerContext.AnnotationService;
      }
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public UnlinkAnnotationCommand(AnnotationSceneNode annotationNode)
    {
      this.annotationNode = annotationNode;
    }

    public bool CanExecute(object parameter)
    {
      return Enumerable.Any<SceneElement>(this.annotationNode.AttachedElements);
    }

    public void Execute(object parameter)
    {
      this.AnnotationService.UnlinkAllAttachments(this.annotationNode);
    }
  }
}
